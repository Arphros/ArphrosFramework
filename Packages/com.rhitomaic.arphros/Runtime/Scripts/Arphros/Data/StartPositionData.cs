using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArphrosFramework.Data {
    [Serializable]
    public class StartPositionData {
        public float audioTime;
        public float speed = 12;
        public int loopCount;
        public Vector3 turn1 = new(0, 90, 0);
        public Vector3 turn2;
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;
        public List<int> calledTriggers = new();
    }
}