using System;
using System.Collections.Generic;

namespace ArphrosFramework {
    [Serializable]
    public class ModelData {
        public int meshId;
        public List<int> materialIds = new();
    }
}