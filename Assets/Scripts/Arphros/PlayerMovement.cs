using UnityEngine;

namespace Arphros {
    /// <summary>
    /// The core class that moves the player and handles all events and inputs related to the cube.
    /// </summary>
    public class PlayerMovement : MonoBehaviour {
        /// <summary>
        /// The linear movement speed of the player
        /// </summary>
        public float speed = 15;

        /// <summary>
        /// The index of the current rotation
        /// </summary>
        public int rotationIndex;
        /// <summary>
        /// The rotations the line supports, in degrees
        /// </summary>
        public Vector3[] rotations = {
            new (0, 0, 0),
            new (0, 90, 0),
        };
        /// <summary>
        /// The transform where the tail is attached to
        /// </summary>
        public Transform tailParent;

        // Cached Components
        private MeshRenderer meshRenderer;

        void Start() {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            meshRenderer = GetComponent<MeshRenderer>();
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetMouseButtonDown(0)) {
                rotationIndex = (rotationIndex + 1) % rotations.Length;
                transform.localEulerAngles = rotations[rotationIndex];
            }

            InefficientTail();
            transform.localPosition += speed * Time.deltaTime * transform.forward;
        }

        /// <summary>
        /// A placeholder method that creates a cube that follows the player every frame.
        /// </summary>
        public void InefficientTail() {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (tailParent)
                obj.transform.parent = tailParent;
            obj.GetComponent<MeshRenderer>().sharedMaterial = meshRenderer.sharedMaterial;
            obj.GetComponent<BoxCollider>().isTrigger = true;
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
        }
    }
}