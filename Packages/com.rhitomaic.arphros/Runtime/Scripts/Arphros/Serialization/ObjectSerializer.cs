using System;
using UnityEngine;

namespace ArphrosFramework {
    /// <summary>
    /// TODO: I might need to rename this class to something else later, since it does more than just serialization. 
    /// Or separate into another class or interface.
    /// </summary>
    public class ObjectSerializer : MonoBehaviour {
        public ObjectInfo info;
        public string data;

        public virtual object GetObject() {
            return null;
        }

        public virtual string Serialize() {
            return "{}";
        }

        public virtual void Deserialize(string data) {
            this.data = data;
        }

        public virtual void OnIdRedirected(int previousId, int newId) {

        }

        public virtual void OnPlay(bool wasPaused) {

        }

        public virtual void OnPause() {

        }

        public virtual void OnStop() {

        }

        public virtual void OnVisibilityChange(VisibilityType visibilityType) {

        }

        public virtual void OnCloned(ObjectInfo original) {

        }

        public virtual void OnCollidableChanged(bool to) {

        }

        public virtual bool OnPostRenderSelected() {
            return false;
        }

        public virtual Action<ObjectInfo> GetRestoreReference(ObjectInfo info) {
            return null;
        }
    }

    public class ObjectSerializer<T> : ObjectSerializer {
        public override object GetObject() => OnSerialize();
        public override string Serialize() => JsonUtility.ToJson(GetObject());
        public override void Deserialize(string data) {
            var obj = JsonUtility.FromJson<T>(data);
            OnDeserialized(obj);
        }

        public virtual T OnSerialize() {
            return default(T);
        }

        public virtual void OnDeserialized(T obj) {

        }
    }
}