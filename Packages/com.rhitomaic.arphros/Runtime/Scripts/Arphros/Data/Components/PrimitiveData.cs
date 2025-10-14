using System;
using UnityEngine;

namespace ArphrosFramework.Data {
    [Serializable]
    public class PrimitiveData {
        public PrimitiveType type = PrimitiveType.Cube;
        public ModelData data = new();
    }
}