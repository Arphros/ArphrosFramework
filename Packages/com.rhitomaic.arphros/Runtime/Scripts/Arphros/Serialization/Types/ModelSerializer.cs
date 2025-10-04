using UnityEngine;
using ArphrosFramework.Level;

namespace ArphrosFramework {
    public class ModelSerializer : ObjectSerializer<ModelData> {
        // TODO: Restore when assets system are implemented
        /*
            private MeshFilter _filter;
            private void Awake() {
                MeshManager.preRefresh += MeshManager_preRefresh;
                MeshManager.postRefresh += MeshManager_postRefresh;
            }

            private int meshId;
            private void MeshManager_preRefresh() {
                _filter ??= GetComponent<MeshFilter>();
                meshId = MeshManager.GetMeshId(_filter.sharedMesh.name);
            }

            private void MeshManager_postRefresh() {
                _filter ??= GetComponent<MeshFilter>();
                _filter.mesh = MeshManager.GetObject(meshId).GetMesh();
                GetComponent<MeshCollider>().sharedMesh = _filter.sharedMesh;
            }

            public override void OnDeserialized(ModelData obj) {
                gameObject.AddComponent<MeshCollider>();
                ApplyMeshData(gameObject, obj, true);
            }

            public override ModelData OnSerialize() => GetMeshData(gameObject);

            Material mainMaterial;
            public override void OnPlay(bool wasPaused) {
                if (wasPaused) {
                }
                else {
                    var mCollider = GetComponent<MeshCollider>();
                    if (mCollider) {
                        mCollider.isTrigger = !info.canCollide;
                        mCollider.convex = false;
                    }

                    mainMaterial = GetComponent<MeshRenderer>().sharedMaterial;
                }
            }

            public override void OnStop() {
                if (mainMaterial)
                    GetComponent<MeshRenderer>().sharedMaterial = mainMaterial;

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

            private void OnDestroy() {
                MeshManager.preRefresh -= MeshManager_preRefresh;
                MeshManager.postRefresh -= MeshManager_postRefresh;
            }
        */

        public static ModelData GetMeshData(GameObject obj) {
            /*var data = new ModelData();
            var filter = obj.GetComponent<MeshFilter>();
            data.meshId = MeshManager.GetMeshId(filter.sharedMesh.name);
            if (data.meshId == -1) {
                Debug.LogError("Failed to get: " + filter.sharedMesh.name);
            }

            var renderer = obj.GetComponent<MeshRenderer>();
            var materials = renderer.sharedMaterials;
            foreach (var material in materials) {
                var id = MaterialManager.GetMaterialId(material);
                data.materialIds.Add(id);
            }
            return data;*/
            return null;
        }

        /// <summary>
        /// Applying the mesh the data to the GameObject.
        /// If model isn't found, it will use the cube mesh.
        /// If material isn't found, it will use the default material.
        /// </summary>
        /// <param name="obj">Target GameObject</param>
        /// <param name="data">The model data</param>
        /// <param name="withCollider">Using mesh collider with the mesh from the data</param>
        public static void ApplyMeshData(GameObject obj, ModelData data, bool withCollider = false) {
            /*var filter = obj.AddOrGetComponent<MeshFilter>();
            var inst = new MeshManager.GetObject(data?.meshId ?? -1);

            Mesh mesh;
            if (inst) {
                mesh = inst.GetMesh();
            }
            else {
                mesh = LevelManager.Instance.primitiveMeshes[0].sharedMesh;
                Debug.LogWarning("The mesh data of " + obj.name + " is not valid");
            }

            if (mesh) {
                filter.sharedMesh = mesh;
            }
            else {
                filter.sharedMesh = LevelManager.Instance.primitiveMeshes[0].sharedMesh;
                Debug.LogWarning("The mesh of " + obj.name + " is not valid");
            }

            if (withCollider)
                obj.AddOrGetComponent<MeshCollider>().sharedMesh = filter.sharedMesh;

            var renderer = obj.AddOrGetComponent<MeshRenderer>();
            var materials = new Material[data.materialIds.Count];
            for (int i = 0; i < materials.Length; i++) {
                var id = data.materialIds[i];
                if (id > -1) {
                    var instance = MaterialManager.GetObject(id);
                    materials[i] = instance ? instance.material : LevelManager.Instance.modelMaterial;
                    continue;
                }

                materials[i] = LevelManager.Instance.modelMaterial;
                Debug.Log($"Material isn't found: {id}");
            }
            renderer.sharedMaterials = materials;*/
        }
    }
}