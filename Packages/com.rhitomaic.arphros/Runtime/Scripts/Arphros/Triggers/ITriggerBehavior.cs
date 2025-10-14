using UnityEngine;

namespace ArphrosFramework
{
    public interface ITriggerBehavior
    {
        /// <summary>
        /// Called when the player finally enters the trigger
        /// </summary>
        void OnTriggerEnter(Collider other);
        /// <summary>
        /// Called when the player finally exits the trigger
        /// </summary>
        void OnTriggerExit(Collider other);


        /// <summary>
        /// Gets the serialized data from the trigger type data
        /// </summary>
        string Serialize();
        /// <summary>
        /// Applying the serialized data to the trigger type data
        /// </summary>
        void Deserialize(string data);
    }
}