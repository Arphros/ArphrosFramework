using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ArphrosFramework {
    /// <summary>
    /// Snapshot of a GameObject (transform + components) for restoring later.
    /// </summary>
    public class ReflectionedObj {
        public TransformSnapshot transformSnapshot;
        public List<ComponentSnapshot> componentSnapshots = new();
        public GameObject lastGameObject;

        public ReflectionedObj() { }

        public ReflectionedObj(GameObject obj) {
            Capture(obj);
        }

        /// <summary>
        /// Captures the current state of a GameObject and its components.
        /// </summary>
        public void Capture(GameObject obj) {
            lastGameObject = obj;
            transformSnapshot = new TransformSnapshot(obj.transform);

            componentSnapshots.Clear();
            foreach (var comp in obj.GetComponents<MonoBehaviour>()) {
                try {
                    componentSnapshots.Add(new ComponentSnapshot(comp));
                }
                catch (Exception e) {
                    Debug.LogWarning($"[ReflectionedObj] Failed to cache {comp.GetType().Name} on {obj.name}: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Applies the cached state back to a GameObject.
        /// </summary>
        public void Apply(GameObject obj) {
            if (obj == null) return;

            lastGameObject = obj;
            transformSnapshot.Apply(obj.transform);

            foreach (var cache in componentSnapshots) {
                var comp = obj.GetComponent(cache.ComponentType);
                if (comp != null) {
                    cache.Cache.ApplyTo(comp);
                }
            }
        }

        #region Inner Classes

        [Serializable]
        public struct TransformSnapshot {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;

            public TransformSnapshot(Transform tr) {
                position = tr.position;
                rotation = tr.rotation;
                scale = tr.localScale;
            }

            public void Apply(Transform tr) {
                tr.position = position;
                tr.rotation = rotation;
                tr.localScale = scale;
            }
        }

        public class ComponentSnapshot {
            public Type ComponentType { get; }
            public Reflector Cache { get; }

            public ComponentSnapshot(MonoBehaviour component) {
                ComponentType = component.GetType();
                Cache = new Reflector(component, ComponentType);
            }
        }

        #endregion
    }

    /// <summary>
    /// Handles saving and restoring fields/properties of an object via reflection.
    /// </summary>
    public class Reflector {
        private readonly List<MemberSnapshot> members = new();

        public Reflector() { }

        public Reflector(object source, Type mainType, Type excludedType = null) {
            Save(source, mainType, excludedType);
        }

        public void Save(object source, Type mainType, Type excludeType = null) {
            members.Clear();

            var excluded = excludeType != null ? new HashSet<string>() : new HashSet<string>();
            if (excludeType != null) {
                foreach (var f in excludeType.GetFields())
                    excluded.Add(f.Name);
            }

            bool serializeAll = mainType.GetCustomAttribute(typeof(SaveAllStateAttribute)) != null;

            foreach (var field in mainType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                if (excluded.Contains(field.Name)) continue;

                bool valid = serializeAll
                    ? field.GetCustomAttribute(typeof(IgnoreSavingStateAttribute)) == null
                    : field.GetCustomAttribute(typeof(AllowSavingStateAttribute)) != null;

                if (valid) {
                    try { members.Add(new MemberSnapshot(field, source)); }
                    catch (Exception e) { Debug.LogWarning($"[Reflector] {field.Name} -> {e.Message}"); }
                }
            }

            foreach (var prop in mainType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                if (!prop.CanRead || !prop.CanWrite) continue;

                bool valid = serializeAll
                    ? prop.GetCustomAttribute(typeof(IgnoreSavingStateAttribute)) == null
                    : prop.GetCustomAttribute(typeof(AllowSavingStateAttribute)) != null;

                if (valid) {
                    try { members.Add(new MemberSnapshot(prop, source)); }
                    catch (Exception e) { Debug.LogWarning($"[Reflector] {prop.Name} -> {e.Message}"); Debug.LogException(e); }
                }
            }
        }

        public void ApplyTo(object target) {
            foreach (var m in members) {
                try { m.ApplyTo(target); }
                catch (Exception e) { Debug.LogWarning($"[Reflector] Failed to apply {m.Name} -> {e.Message}"); Debug.LogException(e); }
            }
        }

        #region Inner Class

        public class MemberSnapshot {
            private readonly FieldInfo field;
            private readonly PropertyInfo property;
            public string Name { get; }
            private readonly object value;

            public MemberSnapshot(FieldInfo f, object source) {
                field = f;
                Name = f.Name;
                value = f.GetValue(source);
            }

            public MemberSnapshot(PropertyInfo p, object source) {
                property = p;
                Name = p.Name;
                value = p.GetValue(source);
            }

            public void ApplyTo(object target) {
                if (field != null) field.SetValue(target, value);
                else property?.SetValue(target, value);
            }
        }

        #endregion
    }

    #region Attributes
    public class IgnoreSavingStateAttribute : Attribute { }
    public class AllowSavingStateAttribute : Attribute { }
    public class SaveAllStateAttribute : Attribute { }
    #endregion
}
