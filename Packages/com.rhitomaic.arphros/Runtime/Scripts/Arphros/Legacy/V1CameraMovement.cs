using ArphrosFramework.Custom;
using UnityEngine;

namespace ArphrosFramework {
    /// <summary>
    /// This camera code originated from MaxIceFlame's template. Although heavily modified
    /// </summary>
    [ExecuteInEditMode]
    public class V1CameraMovement : CameraHost {
        private static V1CameraMovement _self;
        public static V1CameraMovement self => _self = _self ?? FindFirstObjectByType<V1CameraMovement>();

        [Header("Camera Variables")]
        public new Camera camera;

        [AllowSavingState]
        public Vector3 pivotOffset = Vector3.zero;

        private float _x;
        private float _y;
        private float _z;

        [AllowSavingState]
        public float targetX = 45f;
        [AllowSavingState]
        public float targetY = 60f;
        [AllowSavingState]
        public float targetZ;

        [AllowSavingState]
        public float distance = 20f;

        [AllowSavingState]
        public float smoothing = 1f;
        [AllowSavingState]
        [Range(0.001f, 10f)]
        public float factor = 1f;
        [AllowSavingState]
        public float rotationTime = 1f;

        private float _xVelocity = 1f;
        private float _yVelocity = 1f;
        private float _zVelocity = 1f;

        [Header("Built-in Camera Shake")]
        [AllowSavingState]
        public ShakeCameraType shakeType = ShakeCameraType.Timed;

        [AllowSavingState]
        public float shakeDuration = 0f;

        [AllowSavingState]
        public float shakeAmount = 0f;

        [Header("Misc")]
        public bool simulateInEditor = true;

        public TransformPort cameraTransform;

        private void Start() {
            _self = this;
            _x = targetX;
            _y = targetY;
            _z = targetZ;
            transform.position = target.position + pivotOffset;
            cameraTransform.localPosition = new Vector3(0, 0, -distance);
            transform.eulerAngles = new Vector3(_x, _y, _z);
        }

        private void Update() {
            if (Application.isPlaying) {
                if (LevelManager.isTimePaused) return;
                var speedTime = Time.deltaTime * smoothing;
                var targetDist = new Vector3(0, 0, -distance);

                _x = Mathf.SmoothDampAngle(_x, targetX, ref _xVelocity, rotationTime);
                _y = Mathf.SmoothDampAngle(_y, targetY, ref _yVelocity, rotationTime);
                _z = Mathf.SmoothDampAngle(_z, targetZ, ref _zVelocity, rotationTime);
                transform.eulerAngles = new Vector3(_x, _y, _z);
                transform.position = Vector3.Slerp(
                    transform.position,
                    target ? target.position + pivotOffset : transform.position + pivotOffset,
                    factor * Time.deltaTime
                );

                if (shakeType == ShakeCameraType.Timed) {
                    if (shakeDuration > 0) {
                        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetDist, speedTime) + Random.onUnitSphere * shakeAmount;
                        shakeDuration -= Time.deltaTime;
                    }
                    else {
                        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetDist, speedTime);
                        shakeDuration = 0f;
                        shakeAmount = 0f;
                    }
                }
                else {
                    cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetDist, speedTime) + (shakeAmount > 0 ? Random.onUnitSphere * shakeAmount : Vector3.zero);
                }

                if (cameraTransform.localEulerAngles != Vector3.zero)
                    cameraTransform.localEulerAngles = Vector3.Lerp(
                        cameraTransform.localEulerAngles,
                        Vector3.zero,
                        speedTime
                    );
            }
            else {
                if (!simulateInEditor) return;

                var targetDist = new Vector3(0, 0, -distance);
                transform.eulerAngles = new Vector3(targetX, targetY, targetZ);
                cameraTransform.localPosition = targetDist;
                transform.position = target ? target.position + pivotOffset : transform.position + pivotOffset;
            }
        }

        public void ChangeVar(Vector3 targetRot, float targetSmooth, float targetRotSpeed, Vector3 targetPivotOffset, float targetDistance) {
            targetX = targetRot.x;
            targetY = targetRot.y;
            targetZ = targetRot.z;
            smoothing = targetSmooth;
            rotationTime = targetRotSpeed;
            pivotOffset = targetPivotOffset;
            distance = targetDistance;
        }

        public void Shake(float duration, float strength) {
            shakeDuration = duration;
            shakeAmount = strength;
        }

        public void SetStarterAtNow() {
            _x = transform.eulerAngles.x;
            _y = transform.eulerAngles.y;
            _z = transform.eulerAngles.z;
        }

        public void SetCurrentAsTarget() {
            _x = targetX;
            _y = targetY;
            _z = targetZ;
        }

        public void QuickProcess() {
            _x = targetX;
            _y = targetY;
            _z = targetZ;
            transform.eulerAngles = new Vector3(_x, _y, _z);
            transform.position = target != null ? target.position + pivotOffset : transform.position + pivotOffset;
            cameraTransform.localPosition = new Vector3(0, 0, -distance);
        }

        public override Camera GetMainCamera() => camera;
    }
}