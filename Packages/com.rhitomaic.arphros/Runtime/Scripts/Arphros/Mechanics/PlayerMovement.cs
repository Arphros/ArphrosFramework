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
        public PathSequence sequence;

        [Header("Properties")]
        public float speed = 15;
        public int rotationIndex;
        public Vector3[] rotations = {
            new (0, 0, 0),
            new (0, 90, 0),
        };

        [Header("Audio")]
        public AudioSource source;

        [Header("States")]
        public bool isStarted;
        public bool isControllable = true;
        public int inputQueue;

        [Header("Tail")]
        public Transform tailParent; 

        // Cache
        private GameObject _currentTail;
        private MeshRenderer _meshRenderer;
        private BoxCollider _boxCollider;
        private Rigidbody _rigidbody;
        private bool _isGrounded;
        private float _currentTime;

        void Start() {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            ArphrosInput.Initialize();

            _meshRenderer = GetComponent<MeshRenderer>();
            _rigidbody = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<BoxCollider>();
        }

        void Update() {
            UpdateInputQueue();
            HandleFreeroam();

            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
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

            foreach (var key in ArphrosInput.playerTurn) {
                if (Input.GetKeyDown(key))
                    if (!ArphrosInput.IsMouseOverUI((int)key))
                        inputQueue++;
            }
        }

        private int ReadInput() {
            if (!isControllable) return 0;

            return ArphrosInput.playerTurn.Count(key =>
                Input.GetKeyDown(key) &&
                !ArphrosInput.IsMouseAndTouchOverUI((int)key));
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