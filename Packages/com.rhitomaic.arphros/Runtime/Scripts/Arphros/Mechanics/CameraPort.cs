using Kino;
using UnityEngine;

namespace ArphrosFramework {
    public class CameraPort : MonoBehaviour {
        [Header("Main Components")]
        public CameraMovement movement;

        // Legacy
        public V1CameraMovement weirdController;
        public CTTSCameraMovement oldController;

        [Header("Effects")]
        public AnalogGlitch glitchEffect;

        public void ChangeCameraType(ArphrosCameraType type) {
            movement.enabled = type == ArphrosCameraType.StableCamera;
            weirdController.enabled = type == ArphrosCameraType.WeirdCamera;
            oldController.enabled = type == ArphrosCameraType.OldCamera;
        }

        public void StayInPosition() {
            switch (LevelManager.Instance.cameraType) {
                case ArphrosCameraType.StableCamera:
                    movement.StayInPosition();
                    break;
                case ArphrosCameraType.WeirdCamera:
                    weirdController.StayInPosition();
                    break;
                case ArphrosCameraType.OldCamera:
                    oldController.StayInPosition();
                    break;
            }
        }

        public void SetActiveCamera(bool to) {
            switch (LevelManager.Instance.cameraType) {
                case ArphrosCameraType.StableCamera:
                    movement.enabled = to;
                    weirdController.enabled = false;
                    oldController.enabled = false;
                    break;
                case ArphrosCameraType.WeirdCamera:
                    movement.enabled = false;
                    weirdController.enabled = to;
                    oldController.enabled = false;
                    break;
                case ArphrosCameraType.OldCamera:
                    movement.enabled = false;
                    weirdController.enabled = false;
                    oldController.enabled = to;
                    break;
            }
        }
    }
}