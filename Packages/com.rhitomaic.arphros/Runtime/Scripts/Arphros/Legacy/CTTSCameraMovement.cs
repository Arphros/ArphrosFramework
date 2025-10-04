using ArphrosFramework.Custom;
using UnityEngine;

namespace ArphrosFramework {
    /// <summary>
    /// This camera originated from Theo5970's fanmades.
    /// </summary>
    public class CTTSCameraMovement : CameraHost {
        [AllowSavingState]
        public Vector3 distance;
        [AllowSavingState]
        public Vector3 pivotOffset;
        [AllowSavingState]
        public float smoothing = 1;
        [AllowSavingState]
        public float rotationSmoothing = 1;

        private TransformPort _transform;

        private void Start() {
            if (!target) return;
            _transform ??= GetComponent<TransformPort>();
        }

        private void Update() {
            if (LevelManager.isTimePaused) return;
            if (!target) return;

            _transform.localPosition = Vector3.zero;
            _transform.localEulerAngles = Vector3.zero;

            _transform.localPosition = Vector3.Lerp(_transform.localPosition, target.transform.position + distance + pivotOffset, smoothing * Time.deltaTime);
            _transform.localRotation = Quaternion.Lerp(_transform.localRotation, Quaternion.LookRotation(target.transform.position - target.transform.position - distance), rotationSmoothing * Time.deltaTime);
        }

        public void ChangeVar(Vector3 targetDistance, float targetSmoothing, float targetRotSmoothing, Vector3 targetPiv) {
            pivotOffset = targetPiv;
            distance = targetDistance;
            smoothing = targetSmoothing;

            rotationSmoothing = 1;
        }
    }
}