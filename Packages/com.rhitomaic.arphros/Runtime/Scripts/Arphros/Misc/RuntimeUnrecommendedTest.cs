using UnityEngine;

namespace ArphrosFramework {
    public class RuntimeUnrecommendedTest : MonoBehaviour {
        public string objPath;
        public Material material;

        private void Start() {
            ImportObject();
        }

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
