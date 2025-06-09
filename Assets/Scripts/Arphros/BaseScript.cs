using UnityEngine;

namespace Arphros {
    public class BaseScript : MonoBehaviour {
        void Start() {

        }

        void Update() {
            transform.Rotate(0, 0, 90 * Time.deltaTime);
        }
    }
}