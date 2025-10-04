using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System;
using Debug = UnityEngine.Debug;

namespace ArphrosFramework {
    /// <summary>
    /// The new implementation of ObjImporter, though probably still works the same as v2
    /// </summary>
    public static class ObjImporter {
        private class ObjMeshData {
            public Vector3[] vertices;
            public Vector2[] uv;
            public Vector3[] normals;
            public int[] triangles;
            public Vector3[] faceData;
        }

        #region Public API
        public static Mesh ImportFile(string filePath)
            => ImportMesh(Path.GetFileName(filePath), File.ReadAllText(filePath));

        public static Mesh ImportMesh(string name, string content) {
            var meshData = CreateMeshData(content);
            PopulateMeshData(meshData, content);

            var verts = new Vector3[meshData.faceData.Length];
            var uvs = new Vector2[meshData.faceData.Length];
            var normals = new Vector3[meshData.faceData.Length];
            bool broken = false;

            for (int i = 0; i < meshData.faceData.Length; i++) {
                Vector3 fd = meshData.faceData[i];

                int vIndex = ResolveIndex((int)fd.x, meshData.vertices.Length);
                if (vIndex >= 0) verts[i] = meshData.vertices[vIndex];

                if (fd.y != 0) {
                    int uvIndex = ResolveIndex((int)fd.y, meshData.uv.Length);
                    if (uvIndex >= 0) uvs[i] = meshData.uv[uvIndex];
                    else broken = true;
                }

                if (fd.z != 0) {
                    int nIndex = ResolveIndex((int)fd.z, meshData.normals.Length);
                    if (nIndex >= 0) normals[i] = meshData.normals[nIndex];
                    else broken = true;
                }
            }

            Mesh mesh = new() {
                name = name,
                vertices = verts,
                uv = uvs,
                normals = normals,
                triangles = meshData.triangles
            };

            if (broken) {
                Debug.LogWarning($"[{name}] OBJ missing UVs or normals, recalculating...");
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
            }

            mesh.RecalculateBounds();
#if UNITY_2020_1_OR_NEWER
            mesh.OptimizeIndexBuffers();
            mesh.OptimizeReorderVertexBuffer();
#endif
            return mesh;
        }

        #endregion

        #region Parsing
        private static ObjMeshData CreateMeshData(string content) {
            int vertices = 0, uvs = 0, normals = 0, faces = 0, tris = 0;

            using var reader = new StringReader(content);
            string line;
            while ((line = reader.ReadLine()) != null) {
                if (line.StartsWith("v ")) vertices++;
                else if (line.StartsWith("vt ")) uvs++;
                else if (line.StartsWith("vn ")) normals++;
                else if (line.StartsWith("f ")) {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    int faceVerts = parts.Length - 1;
                    faces += faceVerts;
                    tris += 3 * (faceVerts - 1);
                }
            }

            return new ObjMeshData {
                vertices = new Vector3[vertices],
                uv = new Vector2[uvs],
                normals = new Vector3[normals],
                faceData = new Vector3[faces],
                triangles = new int[tris]
            };
        }

        private static void PopulateMeshData(ObjMeshData mesh, string content) {
            int v = 0, vt = 0, vn = 0, f = 0, f2 = 0;

            using var reader = new StringReader(content);
            string line;
            while ((line = reader.ReadLine()) != null) {
                if (string.IsNullOrWhiteSpace(line)) continue;
                line = line.Trim();

                if (line.StartsWith("v ")) {
                    mesh.vertices[v++] = ParseVector3(line, 1);
                }
                else if (line.StartsWith("vt ")) {
                    mesh.uv[vt++] = ParseVector2(line, 1);
                }
                else if (line.StartsWith("vn ")) {
                    mesh.normals[vn++] = ParseVector3(line, 1);
                }
                else if (line.StartsWith("f ")) {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var faceIndices = new List<int>();

                    for (int i = 1; i < parts.Length; i++) {
                        var comps = parts[i].Split('/');
                        int vIndex = ToInt(comps, 0);
                        int tIndex = ToInt(comps, 1);
                        int nIndex = ToInt(comps, 2);

                        mesh.faceData[f2] = new Vector3(vIndex, tIndex, nIndex);
                        faceIndices.Add(f2);
                        f2++;
                    }

                    // triangulate polygon fan
                    for (int i = 1; i + 1 < faceIndices.Count; i++) {
                        mesh.triangles[f++] = faceIndices[0];
                        mesh.triangles[f++] = faceIndices[i];
                        mesh.triangles[f++] = faceIndices[i + 1];
                    }
                }
            }
        }

        #endregion

        #region Helpers
        private static int ResolveIndex(int objIndex, int arrayLength) {
            if (objIndex == 0) return -1;
            if (objIndex > 0) return objIndex - 1;
            return arrayLength + objIndex; // negative indices
        }

        private static Vector3 ParseVector3(string line, int start) {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return new Vector3(ToFloat(parts[start]), ToFloat(parts[start + 1]), ToFloat(parts[start + 2]));
        }

        private static Vector2 ParseVector2(string line, int start) {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return new Vector2(ToFloat(parts[start]), parts.Length > start + 1 ? ToFloat(parts[start + 1]) : 0f);
        }

        private static int ToInt(string[] comps, int idx) {
            if (comps.Length <= idx || string.IsNullOrWhiteSpace(comps[idx])) return 0;
            return int.Parse(comps[idx], CultureInfo.InvariantCulture);
        }

        private static float ToFloat(string s) {
            if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                return result;
            Debug.LogWarning($"Failed to parse float: {s}");
            return 0f;
        }
        #endregion
    }
}