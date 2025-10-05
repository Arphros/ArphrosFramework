using System;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ArphrosFramework {
    public static class ArphrosUtil {
        public static string ExtractFileName(string encodedUrl) {
            if (string.IsNullOrEmpty(encodedUrl))
                return string.Empty;

            try {
                // Decode the URL
                string decodedUrl = Uri.UnescapeDataString(encodedUrl);

                // Extract the file name
                string fileName = Path.GetFileName(decodedUrl);

                return fileName;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error extracting file name: {ex.Message}");
                return string.Empty;
            }
        }
        public static ObjectInfo GetInfo(this GameObject obj) =>
            obj.GetComponent<ObjectInfo>();

        public static ObjectInfo GetInfo(this Component comp) =>
            comp.GetComponent<ObjectInfo>();

        public static Color GetColor(this GameObject obj) {
            if (obj.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                return spriteRenderer.color;
            if (obj.TryGetComponent<Light>(out var light))
                return light.color;
            if (obj.TryGetComponent<TextMeshPro>(out var tmp))
                return tmp.color;
            if (obj.TryGetComponent<Renderer>(out var renderer))
                return renderer.sharedMaterial.color;
            return Color.white;
        }

        public static Func<Color> GetColorAction(this GameObject obj) {
            if (obj.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                return () => spriteRenderer.color;
            if (obj.TryGetComponent<Light>(out var light))
                return () => light.color;
            if (obj.TryGetComponent<TextMeshPro>(out var tmp))
                return () => tmp.color;
            if (obj.TryGetComponent<Renderer>(out var renderer))
                return () => renderer.sharedMaterial.color;

            return () => Color.white;
        }

        public static void SetColor(this GameObject obj, Color color) {
            if (obj.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                spriteRenderer.color = color;
            else if (obj.TryGetComponent<Light>(out var light))
                light.color = color;
            else if (obj.TryGetComponent<TextMeshPro>(out var tmp))
                tmp.color = color;
            else if (obj.TryGetComponent<Renderer>(out var renderer))
                renderer.sharedMaterial.color = color;
        }

        public static T GetComponentInChildrenNoDepth<T>(this Component component) where T : Component {
            foreach (Transform t in component.transform) {
                var comp = t.GetComponent<T>();
                if (comp != null)
                    return comp;
            }

            foreach (Transform t in component.transform)
                return GetComponentInChildrenNoDepth<T>(t.gameObject);

            return null;
        }

        public static T GetComponentInChildrenNoDepth<T>(this GameObject gameObject) where T : Component {
            foreach (Transform t in gameObject.transform) {
                var comp = t.GetComponent<T>();
                if (comp != null)
                    return comp;
            }

            foreach (Transform t in gameObject.transform)
                return GetComponentInChildrenNoDepth<T>(t.gameObject);

            return null;
        }

        public static Action<Color> SetColorAction(this GameObject obj) {
            if (obj.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                return (col) => spriteRenderer.color = col;
            if (obj.TryGetComponent<Light>(out var light))
                return (col) => light.color = col;
            if (obj.TryGetComponent<TextMeshPro>(out var tmp))
                return (col) => tmp.color = col;
            if (obj.TryGetComponent<Renderer>(out var renderer))
                return (col) => renderer.sharedMaterial.color = col;

            return (col) => { };
        }

        public static Action GetCloneAction(this GameObject obj) {
            var info = obj.GetInfo();
            if (!info)
                return () => info.CloneMaterial();
            return null;
        }

        public static int GetRandomId(int length) {
            if (length < 1) return 0;
            return (int)UnityEngine.Random.Range(1 * Mathf.Pow(10, length - 1), (1 * Mathf.Pow(10, length)) - 1);
        }

        public static T Deserialize<T>(string value) =>
            ArphrosSerializer.Deserialize<T>(value);
        public static string Serialize(object obj, bool formatted = false) =>
            ArphrosSerializer.Serialize(obj, formatted);

        public static void OpenFileExplorer(string directory) {
            if (Storage.DirectoryExists(directory)) {
                if (Application.isMobilePlatform)
                    // TODO: When message boxes are implemented, implement this.
                    // MessageBox.Show("This feature isn't available on Android!");
                    throw new NotImplementedException();
                else
                    Application.OpenURL("file://" + directory);
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static void AdjustAspectRatio(RawImage image) {
            image.GetComponent<AspectRatioFitter>().aspectRatio = Divide(image.texture.width, image.texture.height);
        }

        public static void AdjustAspectRatio(RawImage image, Texture texture) {
            image.GetComponent<AspectRatioFitter>().aspectRatio = Divide(texture.width, texture.height);
        }
        public static void AdjustAspectRatioAndApply(RawImage image, Texture texture) {
            AdjustAspectRatio(image, texture);
            image.texture = texture;
        }

        public static void AdjustAspectRatio(Image image) {
            image.GetComponent<AspectRatioFitter>().aspectRatio = Divide(image.sprite.texture.width, image.sprite.texture.height);
        }

        public static void AdjustAspectRatio(Image image, Sprite sprite) {
            image.GetComponent<AspectRatioFitter>().aspectRatio = Divide(sprite.texture.width, sprite.texture.height);
        }
        public static void AdjustAspectRatioAndApply(Image image, Sprite sprite) {
            AdjustAspectRatio(image, sprite);
            image.sprite = sprite;
        }

        private static float Divide(float left, float right) => left / right;

        public static string BytesToString(long byteCount) {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return Mathf.Sign(byteCount) * num + " " + suf[place];
        }

        public static long GetDirectorySize(string path) => GetDirectorySize(new DirectoryInfo(path));
        public static long GetDirectorySize(DirectoryInfo directoryInfo) {
            var files = directoryInfo.GetFiles();
            var size = files.Sum(fi => fi.Length);
            var directories = directoryInfo.GetDirectories();
            size += directories.Sum(GetDirectorySize);
            return size;
        }

        public static Vector3 Singularite(Vector3 from) {
            Quaternion rot = Quaternion.Euler(from);

            Vector3 result = rot * Vector3.forward;
            return result;
        }

        public static Vector3[] GetLine(Vector3 from, Vector3 to, Vector3 minimumScale) {
            Vector3 centeredPosition = CenterOfVectors(new[] { from, to });
            Vector3 scaledLine = from - to;
            Vector3 fixedScaledLine = new Vector3(II(Mathf.Abs(scaledLine.x), minimumScale.x), II(Mathf.Abs(scaledLine.y), minimumScale.y), II(Mathf.Abs(scaledLine.z), minimumScale.z));
            return new[] { centeredPosition, fixedScaledLine };
        }

        private static float II(float val, float limit) {
            if (val < limit) {
                val = limit;
            }
            return val;
        }

        private static Vector3 CenterOfVectors(IReadOnlyCollection<Vector3> vectors) {
            Vector3 sum = Vector3.zero;
            if (vectors == null || vectors.Count == 0) {
                return sum;
            }

            sum = vectors.Aggregate(sum, (current, vec) => current + vec);
            return sum / vectors.Count;
        }
    }

    /// <summary>
    /// String manipulation utility for Arphros based data
    /// </summary>
    public static class ArphrosPacker {
        private static DataTable DefaultTable = new DataTable();
        // private static string AllowedComputeCharacters = "1234567890+-=*/();infty%";

        public static bool ToBool(this string value, bool defaultValue = false) {
            bool.TryParse(value, out defaultValue);
            return defaultValue;
        }

        public static float ToFloat(this string value, float defaultValue = 0) {
            if (value.ToLower() == "infinity")
                return float.PositiveInfinity;
            else if (value.ToLower() == "-infinity")
                return float.NegativeInfinity;
            float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue);
            return defaultValue;
        }

        public static bool Compute(string expression, float defaultValue, out float value) {
            try {
                if (expression.ToLower() == "infinity") {
                    value = float.PositiveInfinity;
                }
                else if (expression.ToLower() == "-infinity") {
                    value = float.NegativeInfinity;
                }
                else {
                    value = Convert.ToSingle(DefaultTable.Compute(expression, null), CultureInfo.InvariantCulture);
                }
                return true;
            }
            catch {
                value = defaultValue;
                return false;
            }
        }

        public static float Compute(string expression, float defaultValue = 0) {
            try {
                if (expression.ToLower() == "infinity") {
                    return float.PositiveInfinity;
                }
                else if (expression.ToLower() == "-infinity") {
                    return float.NegativeInfinity;
                }
                else {
                    return Convert.ToSingle(DefaultTable.Compute(expression, null), CultureInfo.InvariantCulture);
                }
            }
            catch {
                return defaultValue;
            }
        }

        public static List<int> ToListInt(this string value, List<int> defaultValue = default) {
            if (string.IsNullOrWhiteSpace(value)) return new List<int>();
            try {
                var split = value.Split(',');
                defaultValue = new List<int>();
                foreach (var str in split)
                    defaultValue.Add(str.ToInt());
            }
            catch (Exception e) {
                Debug.LogWarning("Unable to parse List<int> string: " + e.Message);
            }
            return defaultValue;
        }

        public static int ToInt(this string value, int defaultValue = 0) {
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out defaultValue);
            return defaultValue;
        }

        public static T ToEnum<T>(this string value, T defaultValue = default) where T : struct {
            Enum.TryParse(value, out defaultValue);
            return defaultValue;
        }

        public static Vector2 ToVector2(this string value, Vector2 defaultValue = default) {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            var split = value.Split(',');
            float.TryParse(split[0], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.x);
            float.TryParse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.y);
            return defaultValue;
        }

        public static Vector3 ToVector3(this string value, Vector3 defaultValue = default) {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            var split = value.Split(',');
            float.TryParse(split[0], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.x);
            float.TryParse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.y);
            float.TryParse(split[2], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.z);
            return defaultValue;
        }

        public static void AsObject(this int id, Action<ObjectInfo> onFetched, ObjectInfo defaultValue = null) {
            if (id > -1)
                LevelManager.afterLoad += () => onFetched?.Invoke(LevelManager.GetObject(id));
            else
                onFetched?.Invoke(defaultValue);
        }

        public static void AsObject(this string value, Action<ObjectInfo> onFetched) {
            try {
                var id = value.ToInt(-1);
                if (id > -1) {
                    var obj = LevelManager.GetObject(id);
                    if (obj)
                        onFetched?.Invoke(obj);

                    void act() {
                        onFetched?.Invoke(LevelManager.GetObject(id));
                        LevelManager.afterLoad -= act;
                    }

                    LevelManager.afterLoad += act;
                }
                else
                    onFetched?.Invoke(null);
            }
            catch (Exception ex) {
                Debug.LogException(ex);
            }
        }

        // TODO: When material assets are implemented, implement this
        /*public static void AsMaterial(this string value, Action<Material> onFetched)
        {
            var id = value.ToInt(-1);
            if (id > -1)
            {
                var instance = MaterialManager.GetObject(id);
                LevelManager.afterLoad += () => onFetched?.Invoke(instance != null ? instance.material : LevelManager.Instance.modelMaterial);
            }
            else
                onFetched?.Invoke(LevelManager.Instance.modelMaterial);
        }*/

        public static Vector4 ToVector4(this string value, Vector4 defaultValue = default) {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            var split = value.Split(',');
            float.TryParse(split[0], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.x);
            float.TryParse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.y);
            float.TryParse(split[2], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.z);
            float.TryParse(split[3], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.w);
            return defaultValue;
        }

        public static Quaternion ToQuaternion(this string value, Quaternion defaultValue = default) {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            var split = value.Split(',');
            float.TryParse(split[0], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.x);
            float.TryParse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.y);
            float.TryParse(split[2], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.z);
            float.TryParse(split[3], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.w);
            return defaultValue;
        }

        public static Color ToColor(this string value, Color defaultValue = default) {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            var split = value.Split(',');
            float.TryParse(split[0], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.r);
            float.TryParse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.g);
            float.TryParse(split[2], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.b);
            float.TryParse(split[3], NumberStyles.Float, CultureInfo.InvariantCulture, out defaultValue.a);
            return defaultValue;
        }

        public static string Pack(this bool value) =>
            value.ToString(CultureInfo.InvariantCulture);
        public static string Pack(this float value) {
            if (float.IsPositiveInfinity(value))
                return "Infinity";
            else if (float.IsNegativeInfinity(value))
                return "-Infinity";
            return value.ToString(CultureInfo.InvariantCulture);
        }
        public static string Pack(this int value) =>
            value.ToString(CultureInfo.InvariantCulture);
        public static string Pack(this Vector2 value) =>
            $"{value.x.Pack()}, {value.y.Pack()}";
        public static string Pack(this Vector3 value) =>
            $"{value.x.Pack()}, {value.y.Pack()}, {value.z.Pack()}";
        public static string Pack(this Vector4 value) =>
            $"{value.x.Pack()}, {value.y.Pack()}, {value.z.Pack()}, {value.w.Pack()}";
        public static string Pack(this Quaternion value) =>
            $"{value.x.Pack()}, {value.y.Pack()}, {value.z.Pack()}, {value.w.Pack()}";
        public static string Pack(this Color value) =>
            $"{value.r.Pack()}, {value.g.Pack()}, {value.b.Pack()}, {value.a.Pack()}";
        public static string Pack(this Enum value) =>
            value.ToString("G");
        public static string Pack(this List<int> value) =>
            value == null ? "" : string.Join(", ", value);
        public static string Pack(this ObjectInfo value) =>
            value != null ? value.instanceId.Pack() : "-1";

        // TODO: Implement this too when the material system is implemented
        /*public static string Pack(this Material value) => 
            value != null ? MaterialManager.GetMaterialId(value).Pack() : "-1";*/

        private static AnimationCurve CreateCustomCurve(Func<float, float> easingFunction, int lod = 100) {
            AnimationCurve curve = new AnimationCurve {
                postWrapMode = WrapMode.Clamp,
                preWrapMode = WrapMode.Clamp
            };

            for (float i = 0; i < lod + 1; i++) {
                float div = i / lod;
                curve.AddKey(div, div);
            }

            curve = SetCustomEasingFunction(curve, easingFunction);
            return curve;
        }

        private static AnimationCurve SetCustomEasingFunction(AnimationCurve curve, Func<float, float> easingFunction) {
            Keyframe[] keys = curve.keys;
            int numKeys = keys.Length;

            for (int i = 1; i < numKeys - 1; i++)
                keys[i].value = easingFunction(keys[i].time);

            return new AnimationCurve(keys);
        }

        private static float EaseSpringEvaluator(float t) {
            t = Mathf.Clamp01(t);
            return (Mathf.Sin(t * Mathf.PI * (0.2f + 2.5f * t * t * t)) * Mathf.Pow(1f - t, 2.2f) + t) * (1f + (1.2f * (1f - t)));
        }
        public static T GetStaticInstance<T>(T cache) {
            return EqualityComparer<T>.Default.Equals(cache, default(T)) ? (T)(object)UnityEngine.Object.FindFirstObjectByType(typeof(T)) : cache;
        }

        public static bool DoesObjectHaveComponent<T>(this GameObject obj) {
            return obj.GetComponent(typeof(T)) != null;
        }

        public static T AddOrGetComponent<T>(this GameObject obj) {
            var comp = obj.GetComponent(typeof(T));
            return comp == null ? (T)(object)obj.AddComponent(typeof(T)) : (T)(object)comp;
        }

        public static object AddOrGetComponent(this GameObject obj, Type type) {
            var comp = obj.GetComponent(type);
            return comp ?? obj.AddComponent(type);
        }

        public static void ReplaceWithNewMaterials(MeshRenderer rend) {
            var mats = new List<Material>();
            foreach (var mtrl in rend.sharedMaterials) {
                if (mtrl != null) mats.Add(new Material(mtrl));
            }
            rend.sharedMaterials = mats.ToArray();
        }

        public static List<Type> availableTypes;

        public static void CheckTypes() {
            if (availableTypes == null) {
                availableTypes = new List<Type>();
                var unityAssembly = Assembly.Load("UnityEngine");
                availableTypes.AddRange(unityAssembly.GetTypes());
                availableTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes());
            }
        }

        public static Type GetType(string name) {
            CheckTypes();
            foreach (var type in availableTypes) {
                if (type.FullName == name) return type;
            }
            return null;
        }

        public static Type GetTypeWithPartialName(string name) {
            CheckTypes();
            foreach (var type in availableTypes) {
                if (type.Name == name) return type;
            }
            return null;
        }
    }
}