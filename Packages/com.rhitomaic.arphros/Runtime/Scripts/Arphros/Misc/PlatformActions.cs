using UnityEngine;
using UnityEngine.Events;

namespace ArphrosFramework {
    /// <summary>
    /// Calls a specific UnityEvent depending on the platform
    /// </summary>
    public class PlatformActions : MonoBehaviour {
        public UnityEvent onWindows;
        public UnityEvent onAndroid;
        public UnityEvent onLinux;
        public UnityEvent onEditor;

        private void Awake() {
            if (Application.isEditor)
                onEditor?.Invoke();

            if (Application.platform == RuntimePlatform.WindowsPlayer)
                onWindows?.Invoke();
            else if (Application.platform == RuntimePlatform.Android)
                onAndroid?.Invoke();
            else if (Application.platform == RuntimePlatform.LinuxPlayer)
                onLinux?.Invoke();
        }
    }
}