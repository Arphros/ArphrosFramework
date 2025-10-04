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

        private bool _firstProject = true;

        private void Awake() {
            Instance = this;
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