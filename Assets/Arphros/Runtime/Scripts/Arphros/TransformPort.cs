using System;
using UnityEngine;
using UnityEngine.Events;

namespace Arphros.Custom {
    public class TransformPort : MonoBehaviour {
        public Vector3 localPosition {
            get => transform.localPosition;
            set {
                transform.localPosition = value;
                onPositionChanged.Invoke(value);
            }
        }

        public Quaternion localRotation {
            get => transform.localRotation;
            set {
                transform.localRotation = value;
                onRotationChanged.Invoke(value);
                onEulerAnglesChanged.Invoke(transform.localEulerAngles);
            }
        }

        public Vector3 localEulerAngles {
            get => transform.localEulerAngles;
            set {
                transform.localEulerAngles = value;
                onEulerAnglesChanged.Invoke(value);
                onRotationChanged.Invoke(transform.localRotation);
            }
        }

        public Vector3 localScale {
            get => transform.localScale;
            set { transform.localScale = value; onScaleChanged.Invoke(value); }
        }

        public Vector3UnityEvent onPositionChanged = new();
        public QuaternionUnityEvent onRotationChanged = new();
        public Vector3UnityEvent onEulerAnglesChanged = new();
        public Vector3UnityEvent onScaleChanged = new();
    }

    [Serializable]
    public class Vector3UnityEvent : UnityEvent<Vector3> {}

    [Serializable]
    public class QuaternionUnityEvent : UnityEvent<Quaternion> {}
}