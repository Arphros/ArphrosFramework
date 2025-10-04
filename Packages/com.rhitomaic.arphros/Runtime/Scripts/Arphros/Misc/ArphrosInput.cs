using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ArphrosFramework {
    /// <summary>
    /// A class that basically extends the basic old input system that Unity has
    /// Plans:
    /// - Add customizable array of inputs
    /// </summary>
    public static class ArphrosInput {
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

        public static bool IsMouseAndTouchOverUI(int keyInt) {
            if (keyInt is >= 330 or <= 322) return false;

            var touches = Input.touches;
            return touches.Any(touch => EventSystem.current.IsPointerOverGameObject(touch.fingerId)) || EventSystem.current.IsPointerOverGameObject();
        }

        public static bool IsMouseOverUI(int keyInt) {
            if (keyInt is >= 330 or <= 322) return false;
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
