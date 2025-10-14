using System;
using UnityEngine;

namespace ArphrosFramework.Data {
    [Serializable]
    public class LightData {
        public Color color = Color.white;
        public float range = 10;
        public float intensity = 1;
        public LightShadows shadowType = LightShadows.Soft;
    }
}