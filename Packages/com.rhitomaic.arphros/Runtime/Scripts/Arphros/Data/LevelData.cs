using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

using UnityEngine;

namespace ArphrosFramework {
    /// <summary>
    /// .arproj
    /// </summary>
    [Serializable]
    public class LegacyLevelData {
        public LegacyLevelInfo info = new();

        public ObjectData player = new();
        public ObjectData directionalLight = new();
        public ObjectData mainCamera = new();

        public List<MeshData> meshes = new();
        public List<MaterialData> materials = new();
        public List<SpriteData> sprites = new();
        public List<ObjectData> objects = new();
        public List<CodeData> codes = new();

        /// <summary>
        /// Make it into an .arproj model
        /// </summary>
        public LevelData ToEditable() {
            var data = new LevelData {
                info = new LevelInfo() {
                    id = info.id,
                    levelName = info.levelName,
                    description = info.description,
                    difficulty = info.difficulty,
                    theme = info.theme,
                    genre = info.genre,

                    musicName = info.musicName,
                    musicAuthor = info.musicAuthor,

                    gameVersion = info.gameVersion,
                    levelVersion = info.levelVersion,
                    editedTime = info.editedTime,

                    finishedLevelVersion = info.finishedLevelVersion,
                    isFinished = info.isFinished,

                    cameraType = info.cameraType,
                    environment = info.environment
                },

                player = player,
                directionalLight = directionalLight,
                mainCamera = mainCamera,

                meshes = meshes,
                materials = materials,
                sprites = sprites,
                objects = objects,
                scripts = codes
            };

            return data;
        }
    }

    /// <summary>
    /// .arproj
    /// </summary>
    [Serializable]
    public class LevelData {
        public LevelInfo info = new();

        public ObjectData player = new();
        public ObjectData directionalLight = new();
        public ObjectData mainCamera = new();

        public List<MeshData> meshes = new();
        public List<MaterialData> materials = new();
        public List<SpriteData> sprites = new();
        public List<ObjectData> objects = new();
        public List<CodeData> scripts = new();
    }

    [Serializable]
    public class LegacyLevelInfo {
        public int id;

        public string levelName = "Untitled";
        public string description;
        public string difficulty = "Auto";
        public string theme = "Other";
        public string genre = "Other";

        public string authorName;
        public int authorId;

        public string musicName = "Untitled";
        public string musicAuthor = "Unknown";

        public string gameVersion;
        public int levelVersion;
        public string editedTime;

        public int finishedLevelVersion;
        public bool isFinished;

        public ArphrosCameraType cameraType = ArphrosCameraType.StableCamera;
        public EnvironmentData environment = new EnvironmentData();
    }

    [Serializable]
    public class LevelInfo {
        public int id;

        public string levelName = "Untitled";
        public string description;
        public string difficulty = "Auto";
        public string theme = "Other";
        public string genre = "Other";

        public string musicName = "Untitled";
        public string musicAuthor = "Unknown";

        public string gameVersion;
        public int levelVersion;
        public string editedTime;

        public int finishedLevelVersion;
        public bool isFinished;

        public ArphrosCameraType cameraType = ArphrosCameraType.StableCamera;
        public EnvironmentData environment = new EnvironmentData();
    }

    // TODO: Reimplement when the level system is finished
    [Serializable]
    public class EnvironmentData {
        public SkyboxType skybox = SkyboxType.Color;
        public Color backgroundColor = Color.white;
        public bool enableFog;
        public float fogDensity = 0.01f;
        public Color fogColor = Color.white;
        [ColorUsage(true, true)]
        public Color ambientColor = Color.white;

        public void Apply() {
            /*LevelManager.Instance.ChangeSkybox(skybox);
            LevelManager.Instance.ChangeBackgroundColor(backgroundColor);
            References.Editor.ChangeFogState(enableFog);
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogColor = fogColor;
            RenderSettings.ambientLight = ambientColor;*/
        }

        public EnvironmentData Clone() {
            var data = new EnvironmentData() {
                skybox = skybox,
                backgroundColor = backgroundColor,
                enableFog = enableFog,
                fogDensity = fogDensity,
                fogColor = fogColor,
                ambientColor = ambientColor
            };
            return data;
        }

        public static EnvironmentData Get() {
            var env = LevelManager.Instance.levelInfo.environment;
            var data = new EnvironmentData() {
                skybox = env.skybox,
                // backgroundColor = LevelManager.GetBackgroundColor(),
                enableFog = env.enableFog,
                fogDensity = RenderSettings.fogDensity,
                fogColor = RenderSettings.fogColor,
                ambientColor = RenderSettings.ambientLight
            };
            return data;
        }
    }

    [Serializable]
    public class ReplayData {
        public bool isLegacy;

        public List<float> times = new List<float>();
        public List<Vector3> positions = new List<Vector3>();
        public List<Vector3> rotations = new List<Vector3>();
        public float duration;
        public bool isFinished;

        public ReplayData() { }
        /// <summary>
        /// Only use this constructor for legacy replays
        /// </summary>
        public ReplayData(List<Vector3> positions) {
            isLegacy = true;
            this.positions = positions;
        }

        public string SerializeCompact() {
            var list = new List<string>();
            for (int i = 0; i < times.Count; i++)
                list.Add($"{times[i].Pack()}|{positions[i].Pack()}|{rotations[i].Pack()}");
            return string.Join("\n", list);
        }

        public void DeserializeCompact(string data) {
            var list = data.Split('\n');
            for (int i = 0; i < list.Length; i++) {
                var parts = list[i].Split('|');
                times.Add(parts[0].ToFloat());
                positions.Add(parts[1].ToVector3());
                rotations.Add(parts[2].ToVector3());
            }
        }

        public static ReplayData Load(string data) {
            var replay = new ReplayData();
            replay.DeserializeCompact(data);
            return replay;
        }

        public ReplayData Clone() {
            var data = new ReplayData() {
                isLegacy = isLegacy,
                times = new List<float>(times),
                positions = new List<Vector3>(positions),
                rotations = new List<Vector3>(rotations),
                duration = duration,
                isFinished = isFinished
            };
            return data;
        }
    }
}