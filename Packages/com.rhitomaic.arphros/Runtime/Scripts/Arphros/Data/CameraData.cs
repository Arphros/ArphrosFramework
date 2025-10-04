using UnityEngine;
using System;
using Newtonsoft.Json;
namespace ArphrosFramework {
    [Serializable]
    public class CameraData {
        public int targetId = 0;
        public Vector3 pivotOffset = new Vector3(5f, 0, 5f);
        public Vector3 targetRotation = new Vector3(45f, 45f, 0);
        public float targetDistance = 20f;
        public float smoothFactor = 1f;

        public Vector3 localPositionOffset;
        public Vector3 localEulerOffset;

        public float fov = 60f;
    }
}