using System;
using TMPro;
using UnityEngine;

namespace ArphrosFramework.Data {
    [Serializable]
    public class TextData {
        public string text = "New Text";
        public int fontIndex = 0;
        public float fontSize = 36;
        public Color color = Color.white;
        public HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Left;
        public VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Top;
        public Vector2 boundsSize = new(20, 5);
    }
}