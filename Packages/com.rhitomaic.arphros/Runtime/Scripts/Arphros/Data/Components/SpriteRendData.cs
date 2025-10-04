using System;
using UnityEngine;

namespace ArphrosFramework {
    [Serializable]
    public class SpriteRendData {
        public int spriteId;
        public Color color = Color.white;
        public bool affectByFog = false;
        public int zIndex = 0;
    }
}