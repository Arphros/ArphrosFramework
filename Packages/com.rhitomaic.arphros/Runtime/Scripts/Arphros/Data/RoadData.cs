using System;
using UnityEngine;

namespace ArphrosFramework.Data {
    [Serializable]
    public class RoadData {
        public bool enableAnimation;
        public float activateDistance = 15;
        public bool basedOnOrigin;
        public LeanTweenType ease = LeanTweenType.linear;
        public float duration = 1;

        public bool isRandom;
        public RoadRandomOptions randomOption = RoadRandomOptions.Move;
        public float randomMoveStrength = 2;
        public float randomRotateStrength = 2;

        public Vector3 startPosition = new Vector3(0, -100, 0);
        public bool posAsOffset = true;
        public Vector3 startRotation = new Vector3(0, 45, 0);
        public bool rotAsOffset = true;

        public ModelData meshData;
    }
}