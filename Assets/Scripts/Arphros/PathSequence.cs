using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Arphros {
    public class PathSequence : MonoBehaviour {
        public List<ModifyPoint> points = new List<ModifyPoint>();

        [Header("Runtime Gizmos")]
        public GameObject gizmoPrefab; // Assign your cube+TMP prefab
        private List<GameObject> runtimeGizmos = new List<GameObject>();
        private Camera mainCamera;

        private void Start() {
            mainCamera = Camera.main;
            SpawnRuntimeGizmos();
        }

        private void Update() {
            if (mainCamera == null) return;

            // Make all TMP texts face the camera
            foreach (var gizmo in runtimeGizmos) {
                TextMeshPro tmp = gizmo.GetComponentInChildren<TextMeshPro>();
                if (tmp != null) {
                    tmp.transform.rotation = mainCamera.transform.rotation;
                    // For strict upright: tmp.transform.forward = mainCamera.transform.forward;
                }
            }
        }

        private void SpawnRuntimeGizmos() {
            if (gizmoPrefab == null) return;

            foreach (var point in points) {
                GameObject gizmo = Instantiate(gizmoPrefab, point.position, Quaternion.identity);
                TextMeshPro tmp = gizmo.GetComponentInChildren<TextMeshPro>();
                if (tmp != null) tmp.text = point.time.ToString("F2");
                runtimeGizmos.Add(gizmo);
            }
        }

        private void OnDestroy() {
            foreach (var gizmo in runtimeGizmos) {
                if (gizmo != null) Destroy(gizmo);
            }
        }

#if UNITY_EDITOR
        private const string SavePath = "Assets/path_sequence.json";

        [ContextMenu("Save Points to JSON")]
        public void SavePointsToJson() {
            string json = ArphrosSerializer.Serialize(points, Formatting.Indented);
            File.WriteAllText(SavePath, json);
            AssetDatabase.Refresh();
            Debug.Log($"Saved PathSequence to {SavePath}");
        }

        [ContextMenu("Load Points from JSON")]
        public void LoadPointsFromJson() {
            if (!File.Exists(SavePath)) {
                Debug.LogWarning("No JSON file found to load.");
                return;
            }

            string json = File.ReadAllText(SavePath);
            points = ArphrosSerializer.Deserialize<List<ModifyPoint>>(json);
            Debug.Log($"Loaded PathSequence from {SavePath}");
        }

        private void OnDrawGizmosSelected() {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 14;
            style.alignment = TextAnchor.MiddleCenter;

            foreach (var point in points) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(point.position, Vector3.one);

                Handles.Label(point.position + Vector3.up * 1.2f, point.time.ToString("F2"), style);
            }
        }
#endif
    }

    [Serializable]
    public class ModifyPoint {
        public float time;
        public Vector3 position;
        public Vector3 eulerAngles;

        public ModifyPoint() { }
        public ModifyPoint(float time, Vector3 position, Vector3 eulerAngles) {
            this.time = time;
            this.position = position;
            this.eulerAngles = eulerAngles;
        }

        /// <summary>
        /// Interpolates the transform to the next point, basic implementation based on Dancing Line
        /// </summary>
        /// <param name="transform">The transform to apply the interpolation to</param>
        /// <param name="currentTime">The current time you want to interpolate</param>
        /// <param name="nextPoint">The next <see cref="ModifyPoint"/> to base on</param>
        public virtual void Interpolate(Transform transform, float currentTime, ModifyPoint nextPoint) {
            float progress = currentTime / (nextPoint.time - time);
            transform.localPosition = Vector3.Lerp(position, nextPoint.position, progress);
            transform.localEulerAngles = currentTime >= 1f ? nextPoint.eulerAngles : eulerAngles;
        }
    }
}