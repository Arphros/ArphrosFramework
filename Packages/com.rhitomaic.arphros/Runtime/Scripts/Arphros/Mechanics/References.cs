using UnityEngine;
using UnityEngine.Audio;

namespace ArphrosFramework
{
    public class References : MonoBehaviour
    {
        private static References _Instance;

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
        public LevelManager manager;

        [Header("Game Objects")] 
        public Transform porter;
        public Light directionalLight;
        public CameraPort camPort;
        public new CameraMovement camera;
        public V1CameraMovement weirdCamera;
        public CTTSCameraMovement oldCamera;
        public Camera mainCamera;
        public PlayerMovement player;
        public AudioMixerGroup buttonMixerGroup;

        private void Awake()
        {
            _Instance = this;
            AssignAllAsStatic();

            foreach (var obj in defaultToDisable)
                obj.SetActive(false);
            foreach (var obj in defaultToEnable)
                obj.SetActive(true);
        }

        private void OnDestroy()
        {
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