using UnityEngine;
using UnityEngine.Audio;

namespace ArphrosFramework
{
    public class References : MonoBehaviour
    {
        private static References _Instance;
        public static bool AreReferencesInitialized { get; private set; }

        public static LevelManager Manager;

        public static Transform Porter;
        public static CameraPort CamPort;
        public static CameraMovement Camera;
        public static V1CameraMovement WeirdCamera;
        public static CTTSCameraMovement OldCamera;
        public static Light DirectionalLight;
        public static Camera MainCamera;
        public static PlayerMovement Player;
        public static AudioMixerGroup ButtonMixerGroup;

        [Header("Defaults")]
        public GameObject[] defaultToDisable;
        public GameObject[] defaultToEnable;

        [Header("Game")]
        [NotNull] public LevelManager manager;

        [Header("Game Objects")]
        [NotNull] public Transform porter;
        [NotNull] public Light directionalLight;
        [NotNull] public CameraPort camPort;
        [NotNull] public new CameraMovement camera;
        [NotNull] public V1CameraMovement weirdCamera;
        [NotNull] public CTTSCameraMovement oldCamera;
        [NotNull] public Camera mainCamera;
        [NotNull] public PlayerMovement player;
        [NotNull] public AudioMixerGroup buttonMixerGroup;

        private void Awake()
        {
            _Instance = this;
            AssignAllAsStatic();

            foreach (var obj in defaultToDisable)
                obj.SetActive(false);
            foreach (var obj in defaultToEnable)
                obj.SetActive(true);

            AreReferencesInitialized = true;
        }

        private void OnDestroy()
        {
            Clear();
        }

        public static void Clear() {
            AreReferencesInitialized = false;
            _Instance = null;
            Manager = null;

            Porter = null;
            CamPort = null;
            Camera = null;
            MainCamera = null;
            Player = null;
            DirectionalLight = null;
            WeirdCamera = null;
            OldCamera = null;

            ButtonMixerGroup = null;
        }

        private void AssignAllAsStatic()
        {
            _Instance = this;

            Manager = manager;

            Porter = porter;
            CamPort = camPort;
            Camera = camera;
            DirectionalLight = directionalLight;
            MainCamera = mainCamera;
            Player = player;
            WeirdCamera = weirdCamera;
            OldCamera = oldCamera;

            ButtonMixerGroup = buttonMixerGroup;
        }

        public static void TryInitialize()
        {
            if(!_Instance)
                _Instance = FindFirstObjectByType<References>();
            _Instance.AssignAllAsStatic();
        }
    }
}