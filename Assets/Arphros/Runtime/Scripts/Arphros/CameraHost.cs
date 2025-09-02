using UnityEngine;

namespace Arphros
{
    public class CameraHost : MonoBehaviour
    {
        // [AllowSavingState]
        public Transform target;

        public virtual Camera GetMainCamera() => null;
        public virtual Transform GetTarget() => target;

        public virtual void StayInPosition()
        {
            GameObject currPos = new GameObject();
            currPos.transform.position = target.position;
            currPos.transform.rotation = transform.rotation;
            target = currPos.transform;
        }
    }
}
