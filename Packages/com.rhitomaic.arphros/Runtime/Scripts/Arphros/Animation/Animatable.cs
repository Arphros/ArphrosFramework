using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

namespace ArphrosFramework {
    public class Animatable : ObjectSerializer<AnimatableData> {
        public StartMode mode = StartMode.ByDistance;
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
        public Vector3 endPositionValue = Vector3.one;

        [Header("Rotation")]
        public bool animateRotation;
        public ValueType rotationValueType = ValueType.Fixed;
        public bool asOffsetRotation;

        // If fixed
        public Vector3 rotationValue;

        // If random
        public Vector3 startRotationValue;
        public Vector3 endRotationValue = new Vector3(360, 360, 360);

        [Header("Scale")]
        public bool animateScale;
        public ValueType scaleValueType = ValueType.Fixed;
        public bool asOffsetScale;

        // If fixed
        public Vector3 scaleValue = Vector3.one;

        // If random
        public Vector3 startScaleValue;
        public Vector3 endScaleValue = Vector3.one;

        [Header("Color")]
        public bool animateColor;
        public ValueType colorValueType = ValueType.Fixed;

        // If fixed
        public Color colorValue = Color.white;
        public bool onlyColorThisObject = false;

        // If random
        public Color startColorValue = Color.black;
        public Color endColorValue = Color.white;

        [SerializeField]
        private Vector3 positionReference;
        private Vector3 originalPosition;
        [SerializeField]
        private Vector3 positionOffset;
        private Vector3 originalRotation;
        private Vector3 originalScale;
        private Color originalColor;

        private Action CloneMaterialAction;
        private Action<Color> SetColorAction;
        private Func<Color> GetColorAction;

        private PlayerMovement movement;
        private GameObject clone;
        private bool isInvoked;
        private List<LTDescr> tweens = new();

        private void Start() {
            movement = LevelManager.Instance.player;
        }

        void Update() {
            if (LevelManager.playmode != Playmode.Playing || isInvoked) return;

            if (animatePosition && animateCollider && reverseAnimation)
                positionReference = transform.position - positionOffset;
            else
                positionReference = transform.position;

            if (mode == StartMode.ByDistance) {
                if (Vector3.Distance(positionReference, LevelManager.Instance.player.transform.position) <= distanceMinimum)
                    OnAnimationTriggered();
            }
            else if (mode == StartMode.ByTime) {
                // TODO: Add offset support
                /*if (LevelManager.Instance.player.source.time + References.GameManager.totalOffset >= timeMinimum)
                    OnAnimationTriggered();*/
                if (LevelManager.Instance.player.source.time >= timeMinimum)
                    OnAnimationTriggered();
            }
        }

        public void OnAnimationTriggered() {
            if (LevelManager.playmode != Playmode.Playing) return;
            isInvoked = true;

            if (reverseAnimation) {
                if (animateCollider) {
                    if (animatePosition)
                        tweens.Add(transform.LeanMoveLocal(originalPosition, duration).setEase(ease));

                    if (animateRotation)
                        tweens.Add(transform.LeanRotateLocal(originalRotation, duration).setEase(ease));

                    if (animateScale)
                        tweens.Add(transform.LeanScale(originalScale, duration).setEase(ease));

                    if (animateColor) {
                        if (onlyColorThisObject)
                            info.CloneMaterial();
                        tweens.Add(LeanTween.value(gameObject, (col) => info.SetColor(col), info.GetColor(), originalColor, duration).setEase(ease));
                    }
                }
                else {
                    if (animatePosition)
                        tweens.Add(clone.transform.LeanMoveLocal(originalPosition, duration).setEase(ease));

                    if (animateRotation)
                        tweens.Add(clone.transform.LeanRotateLocal(originalRotation, duration).setEase(ease));

                    if (animateScale)
                        tweens.Add(clone.transform.LeanScale(originalScale, duration).setEase(ease));

                    if (animateColor) {
                        if (onlyColorThisObject)
                            CloneMaterialAction?.Invoke();
                        tweens.Add(LeanTween.value(gameObject, SetColorAction, GetColorAction(), originalColor, duration).setEase(ease));
                    }
                }
            }
            else {
                if (animateCollider) {
                    if (animatePosition)
                        tweens.Add(transform.LeanMoveLocal(GetValue(positionValueType, asOffset, transform.localPosition, positionValue, startPositionValue, endPositionValue), duration).setEase(ease));

                    if (animateRotation)
                        tweens.Add(transform.LeanRotateLocal(GetValue(rotationValueType, asOffsetRotation, transform.localEulerAngles, rotationValue, startRotationValue, endRotationValue), duration).setEase(ease));

                    if (animateScale)
                        tweens.Add(transform.LeanScale(GetValue(scaleValueType, asOffsetScale, transform.localScale, scaleValue, startScaleValue, endScaleValue), duration).setEase(ease));

                    if (animateColor) {
                        if (onlyColorThisObject)
                            info.CloneMaterial();
                        tweens.Add(LeanTween.value(gameObject, (col) => info.SetColor(col), info.GetColor(), GetValue(colorValueType, colorValue, startColorValue, endColorValue), duration).setEase(ease));
                    }
                }
                else {
                    if (animatePosition)
                        tweens.Add(clone.transform.LeanMoveLocal(GetValue(positionValueType, asOffset, transform.localPosition, positionValue, startPositionValue, endPositionValue), duration).setEase(ease));

                    if (animateRotation)
                        tweens.Add(clone.transform.LeanRotateLocal(GetValue(rotationValueType, asOffsetRotation, transform.localEulerAngles, rotationValue, startRotationValue, endRotationValue), duration).setEase(ease));

                    if (animateScale)
                        tweens.Add(clone.transform.LeanScale(GetValue(scaleValueType, asOffsetScale, transform.localScale, scaleValue, startScaleValue, endScaleValue), duration).setEase(ease));

                    if (animateColor) {
                        if (onlyColorThisObject)
                            CloneMaterialAction?.Invoke();
                        tweens.Add(LeanTween.value(gameObject, SetColorAction, GetColorAction(), GetValue(colorValueType, colorValue, startColorValue, endColorValue), duration).setEase(ease));
                    }
                }
            }
        }

        public override void OnPlay(bool wasPaused) {
            if (wasPaused) {
                foreach (var tween in tweens)
                    tween?.resume();
                return;
            }

            positionReference = transform.position;
            originalPosition = transform.localPosition;
            originalRotation = transform.localEulerAngles;
            originalScale = transform.localScale;
            originalColor = info.GetColor();

            if (reverseAnimation) {
                if (animateCollider) {
                    if (animatePosition)
                        transform.localPosition = GetValue(positionValueType, asOffset, transform.localPosition, positionValue, startPositionValue, endPositionValue);

                    if (animateRotation)
                        transform.localEulerAngles = GetValue(rotationValueType, asOffsetRotation, transform.localEulerAngles, rotationValue, startRotationValue, endRotationValue);

                    if (animateScale)
                        transform.localScale = GetValue(scaleValueType, asOffsetScale, transform.localScale, scaleValue, startScaleValue, endScaleValue);

                    if (animateColor) {
                        if (onlyColorThisObject)
                            info.CloneMaterial();
                        info.SetColor(GetValue(colorValueType, colorValue, startColorValue, endColorValue));
                    }

                    positionOffset = transform.position - positionReference;
                }
                else {
                    clone = Instantiate(gameObject, transform.parent);

                    var info = clone.GetComponent<ObjectInfo>();
                    if (animateColor && onlyColorThisObject)
                        info.CloneMaterial();

                    SetColorAction = ArphrosUtil.SetColorAction(clone);
                    GetColorAction = ArphrosUtil.GetColorAction(clone);
                    CloneMaterialAction = ArphrosUtil.GetCloneAction(clone);
                    DestroyImmediate(clone.GetComponent<Animatable>());
                    DestroyImmediate(info);

                    if (animatePosition)
                        clone.transform.localPosition = GetValue(positionValueType, asOffset, transform.localPosition, positionValue, startPositionValue, endPositionValue);

                    if (animateRotation)
                        clone.transform.localEulerAngles = GetValue(rotationValueType, asOffsetRotation, transform.localEulerAngles, rotationValue, startRotationValue, endRotationValue);

                    if (animateScale)
                        clone.transform.localScale = GetValue(scaleValueType, asOffsetScale, transform.localScale, scaleValue, startScaleValue, endScaleValue);

                    if (animateColor) {
                        if (onlyColorThisObject)
                            CloneMaterialAction?.Invoke();
                        SetColorAction?.Invoke(GetValue(colorValueType, colorValue, startColorValue, endColorValue));
                    }

                    if (clone.TryGetComponent<Collider>(out var col))
                        Destroy(col);

                    if (TryGetComponent<Renderer>(out var rend))
                        rend.enabled = false;
                }
            }
            else {
                if (!animateCollider) {
                    clone = Instantiate(gameObject, transform.parent);
                    SetColorAction = ArphrosUtil.SetColorAction(clone);
                    GetColorAction = ArphrosUtil.GetColorAction(clone);
                    CloneMaterialAction = ArphrosUtil.GetCloneAction(clone);
                    DestroyImmediate(clone.GetComponent<Animatable>());
                    DestroyImmediate(clone.GetComponent<ObjectInfo>());

                    if (clone.TryGetComponent<Collider>(out var col))
                        Destroy(col);

                    if (TryGetComponent<Renderer>(out var rend))
                        rend.enabled = false;
                }
            }
        }

        public override void OnPause() {
            foreach (var tween in tweens)
                tween?.pause();
        }

        public override void OnStop() {
            if (clone) Destroy(clone);
            clone = null;

            foreach (var tween in tweens)
                LeanTween.cancel(tween.uniqueId);
            tweens.Clear();

            isInvoked = false;
        }

        public Vector3 GetValue(ValueType type, bool asOffset, Vector3 origin, Vector3 fixedValue, Vector3 startValue, Vector3 endValue) {
            if (type == ValueType.Fixed)
                return asOffset ? fixedValue + origin : fixedValue;
            else
                return asOffset ? GetRandomVector3(startValue, endValue) + origin : GetRandomVector3(startValue, endValue);
        }
        public Color GetValue(ValueType type, Color fixedValue, Color startValue, Color endValue) {
            if (type == ValueType.Fixed)
                return fixedValue;
            else
                return GetRandomColor(startValue, endValue);
        }

        public Vector3 GetRandomVector3(Vector3 start, Vector3 end) {
            return new Vector3
            (
                Random.Range(start.x, end.x),
                Random.Range(start.y, end.y),
                Random.Range(start.z, end.z)
            );
        }

        public Color GetRandomColor(Color start, Color end) {
            return new Color(
                Random.Range(start.r, end.r),
                Random.Range(start.g, end.g),
                Random.Range(start.b, end.b),
                Random.Range(start.a, end.a)
            );
        }

        /*private void OnCollisionEnter(Collision collision)
        {
            OnAnimationTriggered();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnAnimationTriggered();
        }*/

        public override AnimatableData OnSerialize() {
            return new(this);
        }

        public override void OnDeserialized(AnimatableData obj) {
            obj.Apply(this);
        }

        /*public override bool OnPostRenderSelected()
        {
            if (mode == StartMode.ByDistance)
                RGizmos.DrawSphere(transform.position, transform.rotation, distanceMinimum);
            return true;
        }

        private void OnDestroy()
        {
            if (References.Inspector)
            {
                if (References.Inspector.selectedDrawCalls.Contains(OnPostRenderSelected))
                {
                    References.Inspector.selectedDrawCalls.Remove(OnPostRenderSelected);
                }
            }
        }*/

        public enum StartMode {
            ByDistance,
            ByTime
        }

        public enum ValueType {
            Fixed,
            Random
        }
    }
}