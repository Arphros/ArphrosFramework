using ArphrosFramework.Custom;
using UnityEngine;

namespace ArphrosFramework {
    [ExecuteInEditMode]
    public class CameraMovement : CameraHost, IBackupable {
        public Camera mainCamera;

        [Header("Variables")]
        public Vector3 pivotOffset = Vector3.zero;
        public Vector3 targetRotation = new Vector3(45f, 60f, 0);
        public float targetDistance = 20f;
        [Range(0.001f, 10f)]
        public float smoothFactor = 1f;

        public Vector3 localPositionOffset;
        public Vector3 localEulerOffset;

        [Header("Shake")]
        public ShakeCameraType shakeType = ShakeCameraType.Timed;
        public float shakeDuration;
        public float shakeAmount = 0f;

        [Header("Compatibility")]
        public bool oldSystemActive;
        public CameraValues oldValues = new();

        [Header("Misc")]
        public bool simulateInEditor = true;

        public TransformPort cameraTransform;
        private CameraMovementParams backupParams;
        private CameraValues backupValues;

        private void Start() {
            if (!mainCamera)
                return;

            if (target)
                transform.position = target.position + pivotOffset;
            else
                transform.position = Vector3.zero + pivotOffset;

            cameraTransform.localPosition = new Vector3(0, 0, -targetDistance);
            transform.eulerAngles = targetRotation;
            _rotationTemp = targetRotation;
        }

        private void Update() {
            if (!mainCamera)
                return;

            if (Application.isPlaying) {
                Process(false);
            }
            else {
                if (simulateInEditor)
                    Process(true);
            }
        }

        public override Camera GetMainCamera() => mainCamera;

        public void Process(bool quick) {
            var targetDist = new Vector3(0, 0, -targetDistance) + localPositionOffset;
            transform.eulerAngles = targetRotation;

            if (oldSystemActive)
                ProcessOldSystem(quick);

            if (quick) {
                transform.position = (target ? target.position : Vector3.zero) + pivotOffset;
            }
            else {
                transform.position = Vector3.Slerp(transform.position, (target ? target.position : Vector3.zero) + pivotOffset, smoothFactor * Time.deltaTime);
            }

            if (shakeType == ShakeCameraType.Timed) {
                if (shakeDuration > 0) {
                    cameraTransform.localPosition = targetDist + Random.onUnitSphere * shakeAmount;
                    shakeDuration -= Time.deltaTime;
                }
                else {
                    cameraTransform.localPosition = targetDist;
                    shakeDuration = 0f;
                    shakeAmount = 0f;
                }
            }
            else {
                cameraTransform.localPosition = shakeAmount > 0 ? targetDist + Random.onUnitSphere * shakeAmount : targetDist;
            }

            cameraTransform.localEulerAngles = localEulerOffset;
        }

        private Vector3 _rotationTemp;
        private Vector3 _rotationReference = Vector3.one;

        private void ProcessOldSystem(bool quick) {
            if (quick) {
                _rotationTemp = oldValues.rotation;
                targetRotation = _rotationTemp;

                targetDistance = oldValues.distance;
            }
            else {
                _rotationTemp.x = Mathf.SmoothDampAngle(_rotationTemp.x, oldValues.rotation.x, ref _rotationReference.x, oldValues.rotationSmoothing);
                _rotationTemp.y = Mathf.SmoothDampAngle(_rotationTemp.y, oldValues.rotation.y, ref _rotationReference.y, oldValues.rotationSmoothing);
                _rotationTemp.z = Mathf.SmoothDampAngle(_rotationTemp.z, oldValues.rotation.z, ref _rotationReference.z, oldValues.rotationSmoothing);
                targetRotation = _rotationTemp;

                targetDistance = Mathf.Lerp(targetDistance, oldValues.distance, Time.deltaTime * oldValues.smoothing);
            }
        }

        public override Transform GetTarget() => target;

        public void Cache() {
            backupValues = oldValues;
            backupParams = new CameraMovementParams() {
                target = target,
                pivotOffset = pivotOffset,
                targetRotation = targetRotation,
                targetDistance = targetDistance,
                smoothFactor = smoothFactor,
                localPositionOffset = localPositionOffset,
                localEulerOffset = localEulerOffset,
                shakeType = shakeType,
                shakeDuration = shakeDuration,
                shakeAmount = shakeAmount,
                oldSystemActive = oldSystemActive
            };
        }

        public void Clear() {
            backupValues = null;
            backupParams = null;
        }

        public void Restore() {
            if (backupParams == null)
                return;

            target = backupParams.target;
            pivotOffset = backupParams.pivotOffset;
            targetRotation = backupParams.targetRotation;
            targetDistance = backupParams.targetDistance;
            smoothFactor = backupParams.smoothFactor;
            localPositionOffset = backupParams.localPositionOffset;
            localEulerOffset = backupParams.localEulerOffset;
            shakeType = backupParams.shakeType;
            shakeDuration = backupParams.shakeDuration;
            shakeAmount = backupParams.shakeAmount;
            oldSystemActive = backupParams.oldSystemActive;

            if (backupValues != null) {
                oldValues = backupValues;
            }
        }

        public record CameraMovementParams {
            public Transform target;
            public Vector3 pivotOffset;
            public Vector3 targetRotation;
            public float targetDistance;
            public float smoothFactor;
            public Vector3 localPositionOffset;
            public Vector3 localEulerOffset;
            public ShakeCameraType shakeType;
            public float shakeDuration;
            public float shakeAmount;
            public bool oldSystemActive;
        }
    }

    [System.Serializable]
    public class CameraValues {
        public Vector3 rotation = new Vector3(30, 45, 0);
        public float distance = 40;

        public float smoothing = 1f;
        public float rotationSmoothing = 1f;
    }

    public enum ShakeCameraType {
        Timed,
        Tweened
    }
}