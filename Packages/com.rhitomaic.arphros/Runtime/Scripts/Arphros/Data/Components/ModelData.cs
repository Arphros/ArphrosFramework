using System;
using System.Collections.Generic;

namespace ArphrosFramework.Data {
    [Serializable]
    public class ModelData {
        public int meshId;
        public List<int> materialIds = new();
    }
}