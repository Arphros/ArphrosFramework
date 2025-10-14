using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using PrimeTween;
using ArphrosFramework.Data;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArphrosFramework {
    public class PlayerMovement : ObjectSerializer<PlayerData> {
        #region Variables
        [Header("Components")]
        public MeshFilter meshFilter;
        public new MeshRenderer renderer;
        public new BoxCollider collider;
        public AudioSource audioSource;
        public AudioSource soundSource;
        public new Rigidbody rigidbody;
        public GameObject currentGround;

        [Header("Audio")]
        public AudioClip wallSound;
        public AudioClip waterSound;

        [Header("Objects")]
        public Transform tailParent;
        public GameObject fallParticle;

        /// <summary>
        /// How many triggers are requesting to make the player not controllable
        /// </summary>
        [Header("Parameters")]
        public int noControlRequest;
        [AllowSavingState] public TailType tailType = TailType.Overlap;
        public bool funkyTail;
        [AllowSavingState] public float speed = 12;
        public float minimumTimeForParticle = 0.1f;
        public LayerMask groundLayers;
        [Layer]
        public int tailLayer;
        [Layer]
        public int remainsLayer;
        public bool allowRestartByKey;
        public KeyCode restartKey = KeyCode.Escape;
        public KeyCode[] keys = {
            KeyCode.Mouse0,
            KeyCode.Space,
            KeyCode.UpArrow
        };
        public int[] specialKeys = {
            23
        };
        public bool usePhysicsReading = true;
        public bool disableDistanceCheck = true;

        [Header("Directions")]
        [AllowSavingState] public Vector3 turn1 = new(0, 90, 0);
        [AllowSavingState] public Vector3 turn2;
        [AllowSavingState] public Vector3 downDirection = Vector3.down;
        [AllowSavingState] public bool adjustYTail = true;

        [Header("Explosion")]
        public int pieceCount = 5;
        public float yeetPower = 5;
        public Vector3[] presetOffsets;

        [Header("State")]
        [AllowSavingState] public bool isInvicible;
        [AllowSavingState] public bool isStarted;
        [AllowSavingState] public bool isFinished;
        [AllowSavingState] public bool isAlive = true;
        [AllowSavingState] public bool isControllable = true;
        [AllowSavingState] public bool isMoving = true;
        [AllowSavingState] public int loopCount = 1;
        public Transform causedBy;
        /// <summary>
        /// If this is more than 0, try to make it uncontrollable
        /// </summary>
        public int triggerBlockAmount;

        [Header("Events")]
        public UnityEvent onStarted;
        public UnityEvent onTurn;
        public UnityEvent onTap;
        public UnityEvent onTailCreate;
        public UnityEvent onStop;
        public FloatEvent onDied;
        public FloatEvent onFinish;

        // Private
        private float _timeOnAir;
        private readonly List<MeshRenderer> _tails = new();
        private readonly List<GameObject> _pieces = new();
        private GameObject _currentTail;
        private int _inputQueue;
        [AllowSavingState][HideInInspector] public bool wasFlying;
        [AllowSavingState][HideInInspector] public bool isGrounded = true;
        [AllowSavingState][HideInInspector] public Vector3 lineStart;
        private Tween _volumeTween;
        [HideInInspector]
        public Vector3 previousVelocity;
        private Material _previousMaterial;
        #endregion 

        #region Events
        private void Start() {
            meshFilter ??= GetComponent<MeshFilter>();
            renderer ??= GetComponent<MeshRenderer>();
            collider ??= GetComponent<BoxCollider>();

            audioSource ??= GetComponent<AudioSource>();
        }

        private void Update()
        {
            InitialUpdate();
            GroundUpdate();
            MovementUpdate();
        }
        
        private void DetectMaterialChange()
        {
            if (_previousMaterial == null)
                _previousMaterial = renderer.sharedMaterial;

            if (renderer.sharedMaterial != _previousMaterial) {
                _previousMaterial = renderer.sharedMaterial;
                ChangeAllTailMaterials(_previousMaterial);
            }
        }

        private void FixedUpdate() {
            previousVelocity = rigidbody.linearVelocity;
            if (!usePhysicsReading) return;
            if (isStarted)
                ProcessInputsAndMovement();
        }

        public void AttemptTurn() {
            if (IsGroundedLenient()) {
                if (noControlRequest <= 0)
                    Turn(true);
                else
                    onTurn.Invoke();
            }
            else {
                if (noControlRequest > 0)
                    onTurn.Invoke();
            }
        }

        private void ProcessInputsAndMovement() {
            if (_inputQueue > 0 && isAlive) {
                AttemptTurn();
                onTap?.Invoke();
                _inputQueue--;
            }

            if (!isMoving) return;

            var translation = transform.forward * (speed * Time.deltaTime);
            transform.localPosition += translation;

            if (!_currentTail || !isGrounded) return;

            var pos = _currentTail.transform.localPosition;
            pos += translation / 2;
            if (adjustYTail)
                pos.y = transform.localPosition.y;
            _currentTail.transform.localPosition = pos;
            _currentTail.transform.localScale += new Vector3(0, 0, speed * Time.deltaTime);
        }

        private void OnDisable() => _inputQueue = 0;
        private void OnEnable() => _inputQueue = 0;
        #endregion

        #region Processes
        private void InitialUpdate() {
            isGrounded = IsGrounded();
            if (!allowRestartByKey) return;
            if (Input.GetKeyDown(restartKey)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        private void GroundUpdate() {
            if (isGrounded) {
                if (!wasFlying) return;

                if (isStarted)
                    CreateTail();
                if (_timeOnAir >= minimumTimeForParticle && fallParticle)
                    SpawnFallParticle();
                _timeOnAir = 0;
                wasFlying = false;
            }
            else {
                _timeOnAir += Time.deltaTime;
                if (wasFlying) return;

                ExtendByOne(true);
                wasFlying = true;
            }
        }

        private void MovementUpdate() {
            if (isControllable || !isStarted)
                UpdateInputQueue();

            // var input = ReadInput();
            if (isStarted) {
                if (usePhysicsReading) return;
                ProcessInputsAndMovement();
            }
            else {
                if (_inputQueue < 1 || !isAlive) return;
                _inputQueue--;

                isStarted = true;
                transform.localEulerAngles = loopCount % 2 != 0 ? turn2 : turn1;
                CreateTail();

                PlayMusic();
                onStarted.Invoke();
            }
        }

        public float additionalOffset;

        private void PlayMusic() {
            var offset = GetAudioOffset() - additionalOffset;
            switch (offset) {
                case > 0:
                    audioSource.PlayDelayed(offset);
                    break;
                case < 0:
                    audioSource.Play();
                    audioSource.time = -offset;
                    break;
                default:
                    audioSource.Play();
                    break;
            }
        }

        /// <summary>
        /// Reads input
        /// </summary>
        /// <returns>True if input was read</returns>
        public bool ReadInputRaw() => (from key in keys where Input.GetKeyDown(key) select !IsMouseAndTouchOverUI((int)key))
            .FirstOrDefault();

        public void SimulateInput() =>
            _inputQueue++;

        public void ChangeTailType(TailType type) {
            if (type == tailType) return;

            switch (type) {
                case TailType.None:
                    ExtendByOne();
                    _currentTail = null;
                    tailType = type;
                    break;
                case TailType.Overlap:
                case TailType.Separate:
                    tailType = type;
                    _currentTail = CreateTail();
                    break;
            }
        }

        public void ExtendByOne(bool ignoreGround = false) {
            if (tailType == TailType.Separate && (isGrounded || ignoreGround)) {
                if (_currentTail) {
                    var pos = _currentTail.transform.localPosition;
                    pos += (transform.forward / 2) * transform.localScale.z;
                    if (IsGrounded())
                        pos.y = transform.localPosition.y;
                    _currentTail.transform.localPosition = pos;

                    _currentTail.transform.localScale += new Vector3(0, 0, transform.localScale.z);
                }
            }
        }

        public void ReduceByOne() {
            if (tailType == TailType.Separate && isGrounded) {
                if (_currentTail) {
                    var pos = _currentTail.transform.localPosition;
                    pos -= (transform.forward / 2) * transform.localScale.z;
                    pos.y = transform.localPosition.y;
                    _currentTail.transform.localPosition = pos;

                    _currentTail.transform.localScale -= new Vector3(0, 0, transform.localScale.z);
                }
            }
        }

        private void UpdateInputQueue() {
            // TODO: Check if is mobile actually soon
            bool isMobile = false;
            if (isMobile) {
                foreach (var touch in Input.touches) {
                    if (touch.phase != TouchPhase.Began) continue;
                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        _inputQueue++;
                }
            }
            else {
                foreach (var key in keys) {
                    if (!Input.GetKeyDown(key)) continue;
                    if (!IsMouseOverUI((int)key))
                        _inputQueue++;
                }

                foreach (var specialKey in specialKeys) {
                    var key = (KeyCode)specialKey;
                    if (!Input.GetKeyDown(key)) continue;
                    if (!IsMouseOverUI(specialKey))
                        _inputQueue++;
                }
            }
        }

        private float GetAudioOffset() {
            // TODO: Soon implements offset
            /*var id = LevelManager.Instance.levelInfo.id;
            var globalOffset = PlayerPrefs.GetFloat(Settings.AUDIO_OFFSET_NAME, 0);
            var localOffset = PlayerPrefs.GetFloat($"{Settings.LEVEL_AUDIO_OFFSET_NAME}{id}", 0);

            return (globalOffset + localOffset) * 0.001f;*/
            return 0f;
        }

        public void ResetInputQueue() {
            _inputQueue = 0;
        }

        /// <summary>
        /// Checks if mouse is over UI
        /// </summary>
        /// <param name="keyInt">The key code</param>
        /// <returns>True if mouse is over UI</returns>
        private static bool IsMouseAndTouchOverUI(int keyInt) {
            if (keyInt is >= 330 or <= 322) return false;

            var touches = Input.touches;
            return touches.Any(touch => EventSystem.current.IsPointerOverGameObject(touch.fingerId)) || EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// Checks if mouse is over UI
        /// </summary>
        /// <param name="keyInt">The key code</param>
        /// <returns>True if mouse is over UI</returns>
        private static bool IsMouseOverUI(int keyInt) {
            if (keyInt is >= 330 or <= 322) return false;
            return EventSystem.current.IsPointerOverGameObject();
        }
        #endregion

        #region Collisions
        private void OnCollisionEnter(Collision collision) {
            if (!enabled) return;
            if (collision.collider.tag.StartsWith("Obstacle-")) {
                EndGame(StringToOverType(collision.collider.tag), 0, collision.collider.transform);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!enabled) return;
            if (other.tag.StartsWith("Obstacle-")) {
                EndGame(StringToOverType(other.tag), 0, other.transform);
            }
            else if (other.name == "Turn1") {
                Turn(turn1, true);
            }
            else if (other.name == "Turn2") {
                Turn(turn2, true);
            }
        }

        private OverType StringToOverType(string str) {
            return str switch {
                "Finish" => OverType.Win,
                "Obstacle-Wall" => OverType.DieCollide,
                "Obstacle-Water" => OverType.DieWater,
                "Obstacle-Float" => OverType.DieFloating,
                _ => OverType.None
            };
        }

        public void EndGame(OverType type, float delay, Transform cause) {
            if (!isAlive) return;

            causedBy = cause;
            switch (type) {
                case OverType.Win:
                    isControllable = false;
                    isFinished = true;
                    onFinish.Invoke(delay);
                    break;
                case OverType.DieCollide:
                    if (isInvicible)
                        return;
                    isControllable = false;
                    isAlive = false;
                    isMoving = false;
                    FadeOutMusic(3);
                    if (!isFinished) {
                        CreateExplosion(pieceCount, -1, yeetPower);
                        onDied.Invoke(delay);
                    }
                    break;
                case OverType.DieWater:
                case OverType.DieFloating:
                    if (isInvicible)
                        return;
                    isControllable = false;
                    isAlive = false;
                    FadeOutMusic(3);
                    if (!isFinished)
                        onDied.Invoke(delay);
                    break;
            }

            if (!isFinished) {
                if (type == OverType.DieCollide)
                    soundSource.PlayOneShot(wallSound);
                else if (type == OverType.DieWater)
                    soundSource.PlayOneShot(waterSound);
            }
        }
        #endregion

        #region Methods
        public void Turn(bool createTail) {
            Turn(loopCount % 2 != 0 ? turn1 : turn2, createTail);
            loopCount++;
        }

        public void Turn(Vector3 direction, bool createTail, bool finishTurn = false) {
            var change = direction.y - transform.localEulerAngles.y;
            if (finishTurn) {
                if (tailType == TailType.Separate)
                    ExtendByOne();
                MoveForwardCross(change);
            }
            transform.localEulerAngles = direction;
            if (finishTurn)
                MoveForward(change);
            if (createTail)
                CreateTail();

            onTurn.Invoke();
        }

        private void MoveForward(float rotationAngle) {
            var moveDistance = (1 - Mathf.Cos(Mathf.Abs(rotationAngle) * Mathf.Deg2Rad)) * transform.localScale.z;
            transform.Translate(Vector3.forward * moveDistance);
        }

        private void MoveForwardCross(float rotationAngle) {
            var moveDistance = (1 - Mathf.Sin(Mathf.Abs(rotationAngle) * Mathf.Deg2Rad)) * transform.localScale.z;
            transform.Translate(Vector3.forward * moveDistance);
        }

        private float CalculateAlignTranslationDistance(float rotationAngle) {
            var alignTranslationDistance = 0.5f * (1 - Mathf.Cos(rotationAngle * Mathf.Deg2Rad));
            return alignTranslationDistance;
        }

        public void ChangeAllTailMaterials(Material to) {
            foreach (var tail in _tails)
                tail.sharedMaterial = to;
        }

        public void AdjustLine(Vector3 to) {
            transform.position = to;
            if (!_currentTail)
                return;

            var stretchTransform = _currentTail.transform;
            stretchTransform.eulerAngles = Vector3.zero;

            var result = ArphrosUtil.GetLine(lineStart, to, Vector3.one);
            stretchTransform.position = result[0];
            stretchTransform.localScale = result[1];
        }

        public GameObject CreateTail() {
            switch (tailType) {
                case TailType.Overlap:
                    return CreateTail(meshFilter.sharedMesh, renderer.sharedMaterial, transform.localPosition, transform.localEulerAngles, transform.localScale, tailParent ? tailParent : transform.parent != null ? transform.parent : null, true, true);
                case TailType.Separate:
                    var scale = transform.localScale;
                    var offset = (transform.forward / 2) * scale.z;
                    return CreateTail(meshFilter.sharedMesh, renderer.sharedMaterial, transform.localPosition - offset, transform.localEulerAngles, new Vector3(scale.x, scale.y, 0), tailParent ? tailParent : transform.parent != null ? transform.parent : null, true, true);
                case TailType.None:
                default:
                    return null;
            }
        }

        private GameObject[] CreateExplosion(int count, float delayDestruction, float power) {
            var objs = new GameObject[count];
            for (var i = 0; i < count; i++) {
                var piece = new GameObject("Piece");
                piece.AddComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
                piece.AddComponent<MeshRenderer>().sharedMaterial = renderer.sharedMaterial;
                piece.AddComponent<BoxCollider>();
                piece.AddComponent<Rigidbody>().linearVelocity = Random.onUnitSphere * power;
                if (transform.parent)
                    piece.transform.SetParent(transform.parent);
                var offset = i < presetOffsets.Length ? presetOffsets[i] : new Vector3(Random.Range(-1, 1), Random.Range(0, 1), Random.Range(-1, 1));
                piece.transform.localPosition = transform.localPosition + offset;
                piece.transform.localEulerAngles = transform.localEulerAngles;
                piece.transform.localScale = transform.localScale;
                piece.layer = remainsLayer;
                _pieces.Add(piece);

                objs[i] = piece;
                if (delayDestruction > 0)
                    Destroy(piece, delayDestruction);
            }
            return objs;
        }

        private GameObject CreateTail(Mesh mesh, Material material, Vector3 position, Vector3 eulerAngles, Vector3 scale, Transform parent, bool register, bool setAsCurrent) {
            var tail = new GameObject("Tail");
            tail.AddComponent<MeshFilter>().sharedMesh = mesh;

            var tailRenderer = tail.AddComponent<MeshRenderer>();
            tailRenderer.sharedMaterial = material;

            var tailCollider = tail.AddComponent<BoxCollider>();

            if (funkyTail)
                tail.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            tailCollider.isTrigger = !funkyTail;

            if (parent)
                tail.transform.SetParent(parent);
            tail.transform.localPosition = position;
            tail.transform.localEulerAngles = eulerAngles;
            tail.transform.localScale = scale;
            tail.AddComponent<Tail>();
            tail.layer = tailLayer;

            if (register)
                _tails.Add(tailRenderer);

            if (!setAsCurrent) return tail;

            _currentTail = tail;
            lineStart = position;
            onTailCreate.Invoke();

            return tail;
        }

        private ParticleSystem SpawnFallParticle() {
            var obj = Instantiate(fallParticle, transform.parent ? transform.parent : null); ;
            obj.transform.localPosition = transform.localPosition - (transform.up * 0.3f);
            Destroy(obj, 5f);

            var particle = obj.GetComponent<ParticleSystem>();
            particle.Play();

            return particle;
        }

        private void FadeOutMusic(float duration) {
            _volumeTween.Stop();
            _volumeTween = Tween.AudioVolume(audioSource, 0f, duration);
        }

        public void CancelFade() {
            _volumeTween.Stop();
        }

        public void ClearPieces() {
            foreach (var piece in _pieces.Where(piece => piece != null)) Destroy(piece);
            _pieces.Clear();
        }

        public void ClearLastTail() {
            if (_currentTail && isGrounded) {
                Destroy(_currentTail);
                _currentTail = null;
            }
        }

        public void ClearTails() {
            foreach (var tail in _tails.Where(tail => tail != null)) Destroy(tail.gameObject);
            _tails.Clear();
        }

        /// <summary>
        /// Checks if the player is on the ground or not
        /// </summary>
        public bool IsGrounded() {
            var yLevel = (transform.localScale.y / 2) + 0.03f;
            if (!Physics.Raycast(transform.position, downDirection, out var hitInfo, yLevel, groundLayers)) return false;
            if (!disableDistanceCheck)
                if (Vector3.Distance(hitInfo.point, transform.position) > ((transform.localScale.y / 2) + 0.01f)) return false;
            currentGround = hitInfo.collider.gameObject;

            return true;
        }

        /// <summary>
        /// Checks if the player is on the ground or not
        /// </summary>
        public bool IsGroundedLenient(float offset = 0.04f) {
            var yLevel = (transform.localScale.y / 2) + offset;
            if (!Physics.Raycast(transform.position, downDirection, out var hitInfo, yLevel, groundLayers)) return false;
            // if (Vector3.Distance(hitInfo.point, transform.position) > ((transform.localScale.y / 2) + 0.01f)) return false;
            currentGround = hitInfo.collider.gameObject;

            return true;
        }

        /// <summary>
        /// Literally just get all of the tails as a Transform list
        /// </summary>
        public List<Transform> GetAllTail() =>
            _tails.Select(t => t.transform).ToList();

        /// <summary>
        /// Get all of the tail starting from the given tail
        /// </summary>
        public List<Transform> GetTailFrom(Transform tail) {
            var list = new List<Transform>();
            var index = _tails.FindIndex(val => val.transform == tail);
            if (index < 0) index = 0;

            for (var i = index; i < _tails.Count; i++)
                list.Add(_tails[i].transform);
            return list;
        }

        /// <summary>
        /// Get all of the tail in between two tails
        /// </summary>
        public List<Transform> GetTailInBetween(Transform transform1, Transform transform2)
        {
            var list = new List<Transform>();
            var index1 = _tails.FindIndex(val => val.transform == transform1);
            var index2 = _tails.FindIndex(val => val.transform == transform2);

            if (index1 < 0) index1 = 0;
            if (index2 < 0) index2 = _tails.Count - 1;

            if (index1 > index2)
                (index1, index2) = (index2, index1);

            for (var i = index1; i < index2 + 1; i++)
                list.Add(_tails[i].transform);
            return list;
        }
        #endregion
        
        #region Serialization
        public override void OnDeserialized(PlayerData obj) {
            ModelSerializer.ApplyMeshData(gameObject, obj.data);
            gameObject.GetComponent<BoxCollider>();

            speed = obj.speed;
            turn1 = obj.direction1;
            turn2 = obj.direction2;
        }

        public override PlayerData OnSerialize()
        {
            var data = new PlayerData
            {
                direction1 = turn1,
                direction2 = turn2,
                speed = speed,
                data = ModelSerializer.GetMeshData(gameObject)
            };
            return data;
        }
        #endregion
        
        #region Playmode Actions
        Material mainMaterial;
        public override void OnPlay(bool wasPaused) {
            if (wasPaused) {
                References.Player.ReduceByOne();
            }
            else {
                mainMaterial = renderer.sharedMaterial;
            }
        }

        public override void OnPause() {
            base.OnPause();
            References.Player.ExtendByOne();
        }

        public override void OnStop() {
            renderer.sharedMaterial = mainMaterial;
        }

        public override void OnVisibilityChange(VisibilityType visibilityType) {
            base.OnVisibilityChange(visibilityType);
            switch (visibilityType) {
                case VisibilityType.Shown:
                    renderer.enabled = true;
                    break;
                case VisibilityType.Hidden:
                    renderer.enabled = false;
                    break;
                default:
                    // TODO: Reimplement this weird warning I guess lol
                    //if (!LevelManager.IsNotEditor)
                    //Toast.Show("The player resists.");
                    break;
            }
        }
        #endregion
    }

    #region Others
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    public enum OverType {
        Win,
        DieCollide,
        DieWater,
        DieFloating,
        None
    }

    public enum TailType {
        Overlap,
        Separate,
        None
    }
    #endregion
}