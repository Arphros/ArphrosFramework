using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArphrosFramework {

    // TODO: Implement code trigger and separate all trigger systems
    /// <summary>
    /// The bare bone of the trigger system, might separate this into different components since you can't change trigger types anyway
    /// </summary>
    public class Trigger : ObjectSerializer<TriggerData> {
        public static List<int> calledTriggers = new();

        #region Variables
        [Header("Properties")]
        public TriggerType triggerType;

        [Header("Data")]
        public float speedData = 12;
        public VisibilityType visibilityData = VisibilityType.Gone;
        public TailMode clearTailData = TailMode.Clear;

        // Object Targetting
        public ObjectInfo target;
        public bool useGroup;
        public List<int> groups = new();

        // Easing
        public LeanTweenType ease = LeanTweenType.linear;
        public float duration = 1;

        // Code
        // public ScriptEngine scriptEngine;

        // This is the basic functions so the triggers can actually... you know... work?
        private Action<Collider> TriggerEnter;
        private Action<Collider> TriggerExit;
        private new Func<string> Serialize;
        private new Action<string> Deserialize;
        private Action Cancel;
        private List<LTDescr> tweens = new();

        private static ArphrosCameraType levelCameraType =>
            LevelManager.Instance.cameraType;

        // Tweens
        static LTDescr[] fogTween = new LTDescr[2];
        static LTDescr[] environmentTween = new LTDescr[2];
        private bool quickMode;
        #endregion
        
        #region Tween Compact
        public LTDescr TweenVector3(Vector3 start, Vector3 end, float duration, LeanTweenType ease, Action<Vector3> update) {
            var tween = LeanTween.value(gameObject, start, end, duration).setEase(ease).setOnUpdate(update);
            tweens.Add(tween);
            return tween;
        }

        public LTDescr TweenVector2(Vector2 start, Vector2 end, float duration, LeanTweenType ease, Action<Vector2> update) {
            var tween = LeanTween.value(gameObject, start, end, duration).setEase(ease).setOnUpdate(update);
            tweens.Add(tween);
            return tween;
        }

        public LTDescr TweenColor(Color start, Color end, float duration, LeanTweenType ease, Action<Color> update) {
            var tween = LeanTween.value(gameObject, start, end, duration).setEase(ease).setOnUpdate(update);
            tweens.Add(tween);
            return tween;
        }

        public LTDescr TweenFloat(float start, float end, float duration, LeanTweenType ease, Action<float> update) {
            var tween = LeanTween.value(gameObject, start, end, duration).setEase(ease).setOnUpdate(update);
            tweens.Add(tween);
            return tween;
        }

        public void CancelTween(LTDescr tween) {
            if (tween != null)
                LeanTween.cancel(tween.uniqueId);
        }

        public void CancelTweens(LTDescr[] tweens) {
            foreach (var tween in tweens)
                if (tween != null) LeanTween.cancel(tween.uniqueId);
        }
        #endregion
    }

    #region Enums
    public enum TriggerType {
        // Finished
        Camera,
        Jump,
        Speed,
        FreezePlayer,
        ShakeCamera,
        Move,
        Rotate,
        Scale,
        Color,
        Teleport,
        Sequence,
        Direction,
        Finish,
        Fov,
        Stop,
        Tail,
        AnalogGlitch,
        Material,
        Visibility,
        Environment,
        Code,

        // Legacy
        LegacyCamera,
        Fog,
        Light,

        // New
        Gravity,
        Tap,
        None = 128
    }
    #endregion
}