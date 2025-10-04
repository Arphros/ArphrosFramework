using UnityEngine;
using static ArphrosFramework.Animatable;

namespace ArphrosFramework {
    [System.Serializable]
    public class AnimatableData {
        public StartMode mode = StartMode.ByTime;
        /// <summary>
        /// If enabled, it will move the original object, if not, it will create a clone that it can move
        /// </summary>
        public bool animateCollider = true;

        /// <summary>
        /// If true, the animation will go from the target to the origin
        /// </summary>
        public bool reverseAnimation;
        public LeanTweenType ease = LeanTweenType.linear;
        public float duration = 1f;

        [Header("Time")]
        public float timeMinimum = 2f;

        [Header("Distance")]
        public float distanceMinimum = 12f;
        public bool basedOnOrigin;

        [Header("Position")]
        public bool animatePosition;
        public ValueType positionValueType = ValueType.Fixed;
        public bool asOffset;

        // If fixed
        public Vector3 positionValue;

        // If random
        public Vector3 startPositionValue;
        public Vector3 endPositionValue;

        [Header("Rotation")]
        public bool animateRotation;
        public ValueType rotationValueType = ValueType.Fixed;
        public bool asOffsetRotation;

        // If fixed
        public Vector3 rotationValue;

        // If random
        public Vector3 startRotationValue;
        public Vector3 endRotationValue;

        [Header("Scale")]
        public bool animateScale;
        public ValueType scaleValueType = ValueType.Fixed;
        public bool asOffsetScale;

        // If fixed
        public Vector3 scaleValue;

        // If random
        public Vector3 startScaleValue;
        public Vector3 endScaleValue;

        [Header("Color")]
        public bool animateColor;
        public ValueType colorValueType = ValueType.Fixed;

        // If fixed
        public Color colorValue;

        // If random
        public Color startColorValue;
        public Color endColorValue;

        public bool onlyColorThisObject;

        public AnimatableData() { }
        public AnimatableData(Animatable source) {
            mode = source.mode;
            animateCollider = source.animateCollider;
            reverseAnimation = source.reverseAnimation;
            ease = source.ease;
            duration = source.duration;
            timeMinimum = source.timeMinimum;
            distanceMinimum = source.distanceMinimum;
            basedOnOrigin = source.basedOnOrigin;
            asOffset = source.asOffset;

            animatePosition = source.animatePosition;
            positionValueType = source.positionValueType;
            positionValue = source.positionValue;
            startPositionValue = source.startPositionValue;
            endPositionValue = source.endPositionValue;

            animateRotation = source.animateRotation;
            rotationValueType = source.rotationValueType;
            asOffsetRotation = source.asOffsetRotation;
            rotationValue = source.rotationValue;
            startRotationValue = source.startRotationValue;
            endRotationValue = source.endRotationValue;

            animateScale = source.animateScale;
            scaleValueType = source.scaleValueType;
            asOffsetScale = source.asOffsetScale;
            scaleValue = source.scaleValue;
            startScaleValue = source.startScaleValue;
            endScaleValue = source.endScaleValue;

            animateColor = source.animateColor;
            colorValueType = source.colorValueType;
            colorValue = source.colorValue;
            startColorValue = source.startColorValue;
            endColorValue = source.endColorValue;
            onlyColorThisObject = source.onlyColorThisObject;
        }

        public void Apply(Animatable target) {
            target.mode = mode;
            target.animateCollider = animateCollider;
            target.reverseAnimation = reverseAnimation;
            target.ease = ease;
            target.duration = duration;
            target.timeMinimum = timeMinimum;
            target.distanceMinimum = distanceMinimum;
            target.basedOnOrigin = basedOnOrigin;
            target.asOffset = asOffset;

            target.animatePosition = animatePosition;
            target.positionValueType = positionValueType;
            target.positionValue = positionValue;
            target.startPositionValue = startPositionValue;
            target.endPositionValue = endPositionValue;

            target.animateRotation = animateRotation;
            target.rotationValueType = rotationValueType;
            target.asOffsetRotation = asOffsetRotation;
            target.rotationValue = rotationValue;
            target.startRotationValue = startRotationValue;
            target.endRotationValue = endRotationValue;

            target.animateScale = animateScale;
            target.scaleValueType = scaleValueType;
            target.asOffsetScale = asOffsetScale;
            target.scaleValue = scaleValue;
            target.startScaleValue = startScaleValue;
            target.endScaleValue = endScaleValue;

            target.animateColor = animateColor;
            target.colorValueType = colorValueType;
            target.colorValue = colorValue;
            target.startColorValue = startColorValue;
            target.endColorValue = endColorValue;
            target.onlyColorThisObject = onlyColorThisObject;
        }
    }
}