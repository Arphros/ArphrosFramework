using System;
using UnityEngine;

namespace ArphrosFramework.Level {
    [Serializable]
    public class MeshMetadata {
        public int instanceId = -1;
        public int defaultMaterialId = -1;
        public Vector3 scale = Vector3.one;
    }
}