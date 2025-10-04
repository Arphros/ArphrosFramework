using UnityEngine;

namespace ArphrosFramework {
    public class SetActiveOnIndex : MonoBehaviour {
        public int targetIndex;
        public void SetActive(int index) {
            gameObject.SetActive(targetIndex == index);
        }
    }
}