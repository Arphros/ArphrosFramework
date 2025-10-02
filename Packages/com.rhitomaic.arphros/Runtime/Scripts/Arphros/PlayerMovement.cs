using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ArphrosFramework {
    /// <summary>
    /// The core class that moves the player and handles all events and inputs related to the cube.
    /// </summary>
    public class PlayerMovement : MonoBehaviour {
        [Header("Pathing")]
        public PlayerMode mode = PlayerMode.Freeroam;

        [ShowIf(nameof(mode), PlayerMode.Path)]
        public PathSequence sequence;
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public int pointIndex;
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public int judgedPointIndex;

        [Header("Properties")]
        public float speed = 15;
        public int rotationIndex;
        public Vector3[] rotations = {
            new (0, 0, 0),
            new (0, 90, 0),
        };

        [Header("Scoring")]
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public TMP_Text scoreText;
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public TMP_Text judgementText;
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public float score = 0;
        float shownScore;

        [Header("Audio")]
        public AudioSource source;

        [Header("Timing")]
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public float perfectThreshold = 0.05f;
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public float goodThreshold = 0.1f;
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public float okayThreshold = 0.2f;

        [Header("States")]
        public bool isStarted;
        public bool isControllable = true;
        public int inputQueue;

        [Header("Tail")]
        public Transform tailParent; 
        
        [Header("Animation Settings")]
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public float colorFadeSpeed = 5f;
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public float scaleSpeed = 5f;
        [ShowIf(nameof(mode), PlayerMode.Path)]
        public float scoreSpeed = 20f;

        // Cache
        private GameObject _currentTail;
        private MeshRenderer _meshRenderer;
        private BoxCollider _boxCollider;
        private Rigidbody _rigidbody;
        private bool _isGrounded;
        private float _currentTime;

        void Start() {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            AInput.Initialize();

            _meshRenderer = GetComponent<MeshRenderer>();
            _rigidbody = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<BoxCollider>();
        }

        void Update() {
            UpdateInputQueue();

            if (mode == PlayerMode.Freeroam) {
                HandleFreeroam();
            }
            else {
                HandleRhythmMode();
                AdjustRhythmInterface();
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        #region Rhythm Mode
        void HandleRhythmMode() {
            if (isStarted) {
                _currentTime += Time.deltaTime;

                if (pointIndex < sequence.points.Count - 1) {
                    sequence.points[pointIndex].Interpolate(
                        transform,
                        _currentTime - sequence.points[pointIndex].time,
                        sequence.points[pointIndex + 1]
                    );

                    if (_currentTime >= sequence.points[pointIndex + 1].time) {
                        pointIndex++;
                    }
                }
                else {
                    transform.localPosition = sequence.points[pointIndex].position + ((_currentTime - sequence.points[pointIndex].time) * transform.forward * speed);
                    transform.localEulerAngles = sequence.points[pointIndex].eulerAngles;
                }
            }

            JudgeInput();
        }

        public void AdjustRhythmInterface() {
            judgementText.color = Color.Lerp(
                judgementText.color,
                Color.white,
                1f - Mathf.Pow(0.1f, colorFadeSpeed * Time.deltaTime)
            );

            judgementText.transform.localScale = Vector3.Lerp(
                judgementText.transform.localScale,
                Vector3.one,
                1f - Mathf.Pow(0.1f, scaleSpeed * Time.deltaTime)
            );

            shownScore = Mathf.Lerp(shownScore, score, 1f - Mathf.Pow(0.1f, scoreSpeed * Time.deltaTime));

            if (Mathf.Abs(shownScore - score) < 1f) {
                shownScore = score;
                scoreText.text = ((int)score).ToString("D7");
            }
            else {
                scoreText.text = Mathf.FloorToInt(shownScore).ToString("D7");
            }
        }

        public void JudgeInput(bool eatAll = false) {
            // Step 1: Handle misses
            while (judgedPointIndex < sequence.points.Count &&
                   _currentTime > sequence.points[judgedPointIndex].time + okayThreshold) {
                Score("Miss", 0);
                Debug.Log($"Missed! {sequence.points[judgedPointIndex].time:F3} (no input)");
                judgedPointIndex++;
            }

            // Step 2: Handle inputs
            while (inputQueue > 0 && judgedPointIndex < sequence.points.Count) {
                if (!isStarted) {
                    StartGameplay();
                    continue;
                }

                inputQueue--;

                float inputTiming = _currentTime;
                float pointTiming = sequence.points[judgedPointIndex].time;
                float earlyDelta = pointTiming - inputTiming;
                float delta = Mathf.Abs(inputTiming - pointTiming);

                string judgement = GetJudgement(delta);
                if (judgement != null) {
                    Score(judgement, earlyDelta);
                    Debug.Log($"{judgement}! DIFF-T = {delta:F3}");
                    judgedPointIndex++;
                }
                else {
                    // Invalid input: too early or too late
                    break;
                }

                if (!eatAll) break;
            }
        }

        private string GetJudgement(float delta) {
            if (delta <= perfectThreshold) return "PERFECT";
            if (delta <= goodThreshold) return "GOOD";
            if (delta <= okayThreshold) return "OKAY";
            return null; // Too early or too late
        }
        public void Score(string result, float deviation) {
            judgementText.text = result;
            float mult = 1f;
            switch (result) {
                case "PERFECT":
                    mult = 1f;
                    judgementText.color = Color.green;
                    break;
                case "GOOD":
                    mult = 0.6f;
                    judgementText.color = Color.yellow;
                    judgementText.text += deviation > 0f ? " [E]" : " [L]";
                    break;
                case "OKAY":
                    mult = 0.2f;
                    judgementText.color = Color.blue;
                    judgementText.text += deviation > 0f ? " [E]" : " [L]";
                    break;
                case "Miss":
                    mult = 0f;
                    judgementText.color = Color.red;
                    break;
            }
            score += (mult / (sequence.points.Count - 1)) * 1000000;
            judgementText.transform.localScale = Vector3.one * 1.2f;
        }
        #endregion

        private void StartGameplay() {
            isStarted = true;
            source.Play();
            judgedPointIndex++;
            inputQueue--;
        }

        #region Freeroam
        void HandleFreeroam() {
            EatInput();

            if (isStarted) {
                _currentTime += Time.deltaTime;
                float velocity = speed * Time.deltaTime;
                transform.localPosition += velocity * transform.forward;

                if (_currentTail) {
                    _currentTail.transform.localPosition += velocity * transform.forward / 2;
                    _currentTail.transform.localScale += new Vector3(0, 0, velocity);
                }
            }
        }
        public void EatInput(bool eatAll = false) {
            while (inputQueue > 0) {
                if (isStarted) {
                    rotationIndex = (rotationIndex + 1) % rotations.Length;
                    transform.localEulerAngles = rotations[rotationIndex];
                    _currentTail = CreateTail();
                    RecordPoint();
                }
                else {
                    RecordPoint();
                    isStarted = true;
                    _currentTail = CreateTail();
                    source.Play();
                }

                inputQueue--;
                if (!eatAll)
                    break;
            }
        }

        public void RecordPoint() {
            sequence.points.Add(new ModifyPoint(_currentTime, transform.localPosition, transform.localEulerAngles));
        }
        #endregion

        #region Shared
        public void UpdateInputQueue() {
            foreach (var touch in Input.touches) {
                if (touch.phase == TouchPhase.Began)
                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        inputQueue++;
            }

            foreach (var key in AInput.playerTurn) {
                if (Input.GetKeyDown(key))
                    if (!AInput.IsMouseOverUI((int)key))
                        inputQueue++;
            }
        }

        private int ReadInput() {
            if (!isControllable) return 0;

            return AInput.playerTurn.Count(key =>
                Input.GetKeyDown(key) &&
                !AInput.IsMouseAndTouchOverUI((int)key));
        }

        public GameObject CreateTail() {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (tailParent)
                obj.transform.parent = tailParent;
            obj.GetComponent<MeshRenderer>().sharedMaterial = _meshRenderer.sharedMaterial;
            obj.GetComponent<BoxCollider>().isTrigger = true;
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.transform.localScale = transform.localScale;
            return obj;
        }
        #endregion
    }

    public enum PlayerMode {
        Freeroam,
        Path
    }
}