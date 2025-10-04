using UnityEngine;
using ArphrosFramework.Level;

namespace ArphrosFramework {
    public class PrimitiveSerializer : ObjectSerializer<PrimitiveData> {
        public PrimitiveType type = PrimitiveType.Cube;

        public override void OnDeserialized(PrimitiveData obj) {
            type = obj.type;
            switch (type) {
                case PrimitiveType.Cube:
                    ModelSerializer.ApplyMeshData(gameObject, obj.data);
                    var col = gameObject.AddComponent<BoxCollider>();
                    col.size = Vector3.one;
                    break;
                case PrimitiveType.Sphere:
                    ModelSerializer.ApplyMeshData(gameObject, obj.data);
                    gameObject.AddComponent<SphereCollider>();
                    break;
                default:
                    gameObject.AddComponent<MeshCollider>();
                    ModelSerializer.ApplyMeshData(gameObject, obj.data, true);
                    break;
            }
        }

        public override PrimitiveData OnSerialize() {
            var pData = new PrimitiveData {
                type = type,
                data = ModelSerializer.GetMeshData(gameObject)
            };
            return pData;
        }

        public void AddProperCollider(Mesh mesh) {
            switch (type) {
                case PrimitiveType.Cube:
                    var col = gameObject.AddComponent<BoxCollider>();
                    col.size = Vector3.one;
                    break;
                case PrimitiveType.Sphere:
                    gameObject.AddComponent<SphereCollider>();
                    break;
                default:
                    gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
                    break;
            }
        }

        private Material _mainMaterial;
        public override void OnPlay(bool wasPaused) {
            if (wasPaused) {
            }
            else {
                var mCollider = GetComponent<MeshCollider>();
                if (mCollider) {
                    mCollider.isTrigger = !info.canCollide;
                    mCollider.convex = false;
                }

                _mainMaterial = GetComponent<MeshRenderer>().sharedMaterial;
            }
        }

        public override void OnStop() {
            if (_mainMaterial && !info.isModified)
                GetComponent<MeshRenderer>().sharedMaterial = _mainMaterial;

            var mCollider = GetComponent<MeshCollider>();
            if (mCollider) {
                mCollider.isTrigger = !info.canCollide;
                mCollider.convex = false;
            }
        }

        public override void OnVisibilityChange(VisibilityType visibilityType) {
            switch (visibilityType) {
                case VisibilityType.Shown:
                    gameObject.SetActive(true);
                    GetComponent<MeshRenderer>().enabled = true;
                    break;
                case VisibilityType.Hidden:
                    gameObject.SetActive(true);
                    GetComponent<MeshRenderer>().enabled = false;
                    break;
                case VisibilityType.Gone:
                    gameObject.SetActive(false);
                    break;
            }
        }
    }
}