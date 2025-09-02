using System.Collections.Generic;
using UnityEngine;

namespace ArphrosFramework {
    /// <summary>
    /// A class that basically extends the basic old input system that Unity has
    /// </summary>
    public static class AInput {
        /// <summary>
        /// The keys that are used to control the player
        /// </summary>
        public static KeyCode[] playerTurn = {
            KeyCode.J,
            KeyCode.K,
            KeyCode.Space,
            KeyCode.UpArrow,
            KeyCode.Mouse0
        };

        public static void Initialize() {

        }
    }
}
