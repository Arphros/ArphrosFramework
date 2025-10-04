using System;
using UnityEngine;

namespace ArphrosFramework {
    [Serializable]
    public class PlayerData {
        public float speed = 12;
        public Vector3 direction1 = new(0, 90, 0);
        public Vector3 direction2;

        public ModelData data;
    }
}