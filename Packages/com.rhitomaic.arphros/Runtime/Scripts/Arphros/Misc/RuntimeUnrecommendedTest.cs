using UnityEngine;

namespace ArphrosFramework {
    /// <summary>
    /// An unreliable way to test basic features.
    /// </summary>
    public class RuntimeUnrecommendedTest : MonoBehaviour {
        public string objPath;
        public Material material;

        private void Start() {
            ImportObject();
        }

        /// <summary>
        /// Testing .obj importing capabilities i guess lol
        /// </summary>
        public void ImportObject() {
            var mesh = ObjImporter.ImportFile(objPath);

            var obj = new GameObject(mesh.name);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            var mFilter = obj.AddComponent<MeshFilter>();
            mFilter.sharedMesh = mesh;

            var mRend = obj.AddComponent<MeshRenderer>();
            mRend.sharedMaterial = material;
        }
    }
}
