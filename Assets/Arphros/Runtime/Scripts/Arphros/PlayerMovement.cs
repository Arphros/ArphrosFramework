using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Arphros {
    /// <summary>
    /// The core class that moves the player and handles all events and inputs related to the cube.
    /// </summary>
    public class PlayerMovement : MonoBehaviour {
        [Header("Pathing")]
        public PlayerMode mode = PlayerMode.Freeroam;
        public PathSequence sequence;
        public int pointIndex;
        public int judgedPointIndex;

        [Header("Properties")]
        public float speed = 15;
        public int rotationIndex;
        public Vector3[] rotations = {
            new (0, 0, 0),
            new (0, 90, 0),
        };

        [Header("Scoring")]
        public TMP_Text scoreText;
        public TMP_Text judgementText;
        public float score = 0;
        float shownScore;

        [Header("Audio")]
        public AudioSource source;

        [Header("Timing")]
        public float perfectThreshold = 0.05f;
        public float goodThreshold = 0.1f;
        public float okayThreshold = 0.2f;

        [Header("States")]
        public bool isStarted;
        public bool isControllable = true;
        public int inputQueue;

        [Header("Tail")]
        public Transform tailParent; 
        
        [Header("Animation Settings")]
        public float colorFadeSpeed = 5f;
        public float scaleSpeed = 5f;
        public float scoreSpeed = 20f;

        // Cache
        private MeshRenderer meshRenderer;
        private float currentTime;

        void Start() {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            AInput.Initialize();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        void Update() {
            UpdateInputQueue();

            if (mode == PlayerMode.Freeroam) {
                HandleFreeroam();
            }
            else {
                HandleRhythmMode();
            }

            AdjustInterface();

            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        public void AdjustInterface() {
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

        void HandleFreeroam() {
            EatInput();

            if (isStarted) {
                currentTime += Time.deltaTime;
                InefficientTail();
                transform.localPosition += speed * Time.deltaTime * transform.forward;
            }
        }

        void HandleRhythmMode() {
            if (isStarted) {
                currentTime += Time.deltaTime;

                if (pointIndex < sequence.points.Count - 1) {
                    sequence.points[pointIndex].Interpolate(
                        transform,
                        currentTime - sequence.points[pointIndex].time,
                        sequence.points[pointIndex + 1]
                    );

                    if (currentTime >= sequence.points[pointIndex + 1].time) {
                        pointIndex++;
                    }
                }
                else {
                    transform.localPosition = sequence.points[pointIndex].position + ((currentTime - sequence.points[pointIndex].time) * transform.forward * speed);
                    transform.localEulerAngles = sequence.points[pointIndex].eulerAngles;
                }
            }

            JudgeInput();
        }

        public void JudgeInput(bool eatAll = false) {
            // Step 1: Handle misses
            while (judgedPointIndex < sequence.points.Count &&
                   currentTime > sequence.points[judgedPointIndex].time + okayThreshold) {
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

                float inputTiming = currentTime;
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

        private void StartGameplay() {
            isStarted = true;
            source.Play();
            judgedPointIndex++;
            inputQueue--;
        }

        private string GetJudgement(float delta) {
            if (delta <= perfectThreshold) return "PERFECT";
            if (delta <= goodThreshold) return "GOOD";
            if (delta <= okayThreshold) return "OKAY";
            return null; // Too early or too late
        }

        public void EatInput(bool eatAll = false) {
            while (inputQueue > 0) {
                if (isStarted) {
                    rotationIndex = (rotationIndex + 1) % rotations.Length;
                    transform.localEulerAngles = rotations[rotationIndex];
                    RecordPoint();
                }
                else {
                    RecordPoint();
                    isStarted = true;
                    source.Play();
                }

                inputQueue--;
                if (!eatAll)
                    break;
            }
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

        public void RecordPoint() {
            sequence.points.Add(new ModifyPoint(currentTime, transform.localPosition, transform.localEulerAngles));
        }

        public void UpdateInputQueue() {
            foreach (var touch in Input.touches) {
                if (touch.phase == TouchPhase.Began)
                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        inputQueue++;
            }

            foreach (var key in AInput.playerTurn) {
                if (Input.GetKeyDown(key))
                    if (!IsMouseOverUI((int)key))
                        inputQueue++;
            }
        }

        private int ReadInput() {
            if (!isControllable) return 0;

            return AInput.playerTurn.Count(key =>
                Input.GetKeyDown(key) &&
                !IsMouseAndTouchOverUI((int)key));
        }

        private static bool IsMouseAndTouchOverUI(int keyInt) {
            if (keyInt is >= 330 or <= 322) return false;

            var touches = Input.touches;
            return touches.Any(touch => EventSystem.current.IsPointerOverGameObject(touch.fingerId)) || EventSystem.current.IsPointerOverGameObject();
        }

        private static bool IsMouseOverUI(int keyInt) {
            if (keyInt is >= 330 or <= 322) return false;
            return EventSystem.current.IsPointerOverGameObject();
        }

        public void InefficientTail() {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (tailParent)
                obj.transform.parent = tailParent;
            obj.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;
            obj.GetComponent<BoxCollider>().isTrigger = true;
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
        }
    }

    public enum PlayerMode {
        Freeroam,
        Path
    }
}