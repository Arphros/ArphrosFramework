using System;
using System.Reflection;
using UnityEngine;

namespace ArphrosFramework
{
    public abstract class TriggerBehavior : ITriggerBehavior
    {
        protected readonly Trigger Owner;
        protected TriggerBehavior(Trigger owner) => Owner = owner;

        public abstract void OnTriggerEnter(Collider other);
        public virtual void OnTriggerExit(Collider other) { }
        public abstract string Serialize();
        public abstract void Deserialize(string data);

        // Optional shared helpers (simplified)
        protected string Join(params string[] values) => string.Join("|", values);
        protected string[] Split(string data) => data.Split('|');

        public static T CloneObject<T>(T obj) {
            T clonedObject = Activator.CreateInstance<T>();

            // Copy properties
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties) {
                if (property.CanRead && property.CanWrite) {
                    object value = property.GetValue(obj);
                    property.SetValue(clonedObject, value);
                }
            }

            // Copy fields
            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields) {
                object value = field.GetValue(obj);
                field.SetValue(clonedObject, value);
            }

            return clonedObject;
        }
    }
}