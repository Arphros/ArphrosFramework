using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

namespace ArphrosFramework {
    public class LevelManager : InstanceManager<ObjectInfo> {
        public static LevelManager Instance;

        public static Playmode playmode = Playmode.Stopped;
        /// <summary>
        /// This marks if Time.timeScale is actually set to 0 instead of the game being paused while time scale is unchanged
        /// </summary>
        public static bool isTimePaused;
        public static bool isEditor;

        public static Action afterLoad;
        public static string DirectoryQueue;
        public string currentDirectory;

        [Header("Reference")]
        public GameObject game;
        public PlayerMovement player;
        public AudioClip defaultMusic;

        [Header("Info")]
        public LevelData originalData;
        public LevelInfo levelInfo;

        public EnvironmentData defaultEnvironment = new();

        [Header("Object")]
        public PrimitiveType[] primitiveTypes = {
            PrimitiveType.Cube,
            PrimitiveType.Sphere,
            PrimitiveType.Capsule,
            PrimitiveType.Cylinder,
            PrimitiveType.Plane
        };

        public MeshFilter[] primitiveMeshes = { };
        public TriggerType[] triggerTypes =
        {
            TriggerType.Camera,
            TriggerType.Jump,
            TriggerType.LegacyCamera,
        };

        public Color[] triggerColors = {
            Color.green,
            Color.red,
            Color.blue
        };

        public TMP_FontAsset[] fonts = {
        };

        public Mesh halfPyramid;
        public Sprite lightSprite;
        public Sprite squareSprite;
        public Sprite startPosSprite;

        public ObjectInfo lastCreatedObject { get; private set; }

        [Header("Materials")]
        public Material modelMaterial;
        public Material triggerMaterial;
        public Shader foggedTextShader;
        public float triggerAlpha = 0.5f;
        public Material spriteMaterial;
        public Material fogSpriteMaterial;
        public Material[] skyboxMaterials;

        [Header("Spawn Ray")]
        public float rayDistance = 10;
        public float spawnDistance = 5;

        [Header("Level States")]
        public ArphrosCameraType cameraType;

        [Header("Layers")]
        [Layer]
        public int normalLayer;
        [Layer]
        public int playerLayer;
        [Layer]
        public int triggerLayer;
        [Layer]
        public int spriteLayer;
        [Layer]
        public int passthroughLayer;
        [Layer]
        public int floatingObstacleLayer;
        [Layer]
        public int spawnPointIgnoreLayer;
        public LayerMask spawnPointLayerMask;

        [Header("Scenes")]
        public Transform standardTransform;
        public Transform screenTransform;
        public Transform environmentTransform;

#pragma warning disable IDE0044 // Add readonly modifier
        private bool _firstProject = true;
#pragma warning restore IDE0044 // Add readonly modifier

        private void Awake() {
            Instance = this;
        }

        /// <summary>
        /// Called before project loads
        /// </summary>
        public void Initialize(bool refuseBreaking = false) {
            Instance = this;
            ClearDictionary();

            int len = standardTransform.childCount;
            for (int i = 0; i < len; i++) {
                var info = standardTransform.GetChild(i).GetInfo();
                if (info != null)
                    info.Initialize();
            }

            References.MainCamera.GetInfo().Initialize();

            len = environmentTransform.childCount;
            for (int i = 0; i < len; i++) {
                var child = environmentTransform.GetChild(i);
                if (child) {
                    var info = child.GetInfo();
                    if (info)
                        info.Initialize();
                }
            }

            // Initialize assets
            /*materialManager.Initialize();
            meshManager.Initialize();
            spriteManager.Initialize();
            scriptManager.Initialize();*/

            if (!refuseBreaking) {
                levelInfo.environment = defaultEnvironment;
                levelInfo.environment.Apply();
            }

            if (_firstProject) {
                if (!string.IsNullOrWhiteSpace(DirectoryQueue)) {
                    if (Storage.DirectoryExists(DirectoryQueue))
                        LoadLevel(DirectoryQueue);
                }
                else {
                    currentDirectory = Storage.GetPathLocal("Levels/Cache");
                    // meshManager.Refresh();
                    // spriteManager.Refresh();
                }

                _firstProject = false;
            }
        }

        public void LoadLevel(string path) {
            var rawData = Storage.ReadAllText(path);
            var data = ArphrosSerializer.Deserialize<LevelData>(rawData);
            LoadLevel(data);
        }

        public async void LoadLevel(LevelData data) {
            try {
                await UniTask.Delay(20);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        private void OnDestroy() {
            if (Instance == this)
                Instance = null;
        }
    }

    public enum ArphrosCameraType {
        StableCamera,
        WeirdCamera,
        OldCamera
    }

    public enum SkyboxType {
        Color,
        UnitySkybox,
        BuiltinSkybox,
        CustomSkybox
    }

    public enum Playmode {
        Playing,
        Paused,
        Stopped
    }
}