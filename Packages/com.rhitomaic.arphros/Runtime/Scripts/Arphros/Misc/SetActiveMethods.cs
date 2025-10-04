using UnityEngine;

namespace ArphrosFramework {
    /// <summary>
    /// A collection of methods that is going to be used by simple UnityEvents
    /// </summary>
    public class SetActiveMethods : MonoBehaviour {
        /// <summary>
        /// Used for UnityEvents that accepts boolean, simply reverses the action
        /// </summary>
        public void SetActiveReverse(bool to) =>
            gameObject.SetActive(!to);

        /// <summary>
        /// Used for UnityEvents, just toggles between active and inactive
        /// </summary>
        public void SetActiveToggle() =>
            gameObject.SetActive(!gameObject.activeSelf);
    }
}