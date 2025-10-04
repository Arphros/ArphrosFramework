using UnityEngine;

namespace ArphrosFramework {
    public class CameraHost : MonoBehaviour {
        public Transform target;

        public virtual Camera GetMainCamera() => null;
        public virtual Transform GetTarget() => target;

        public virtual void StayInPosition() {
            GameObject currPos = new();
            currPos.transform.position = target.position;
            currPos.transform.rotation = transform.rotation;
            target = currPos.transform;
        }
    }
}