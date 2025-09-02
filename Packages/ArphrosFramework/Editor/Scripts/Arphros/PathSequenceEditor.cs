#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace ArphrosFramework {
    [CustomEditor(typeof(PathSequence))]
    public class PathSequenceEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var sequence = (PathSequence)target;

            if (GUILayout.Button("Save Points to JSON")) {
                sequence.SavePointsToJson();
            }

            if (GUILayout.Button("Load Points from JSON")) {
                sequence.LoadPointsFromJson();
            }
        }
    }
}
#endif