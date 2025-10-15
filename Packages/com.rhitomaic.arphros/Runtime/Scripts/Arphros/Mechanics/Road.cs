using System;
using System.Collections.Generic;
using ArphrosFramework.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ArphrosFramework {
    public class Road : ObjectSerializer<RoadData> {
        private float DistanceFromPlayer => basedOnOrigin ? Vector3.Distance(References.Player.transform.position, _initialPosition) : Vector3.Distance(References.Player.transform.position, transform.position);

        [Header("Animated")]
        public GameObject invisibleRoad;

        [Header("Animation")]
        public bool enableAnimation;
        public float activateDistance = 15;
        public bool basedOnOrigin;
        public LeanTweenType ease = LeanTweenType.linear;
        public float duration = 1;

        [Header("Random")]
        public bool isRandom = true;
        public RoadRandomOptions randomOption = RoadRandomOptions.Move;
        public float randomMoveStrength = 5;
        public float randomRotateStrength = 5;

        [Header("Target position")]
        public Vector3 startPosition;
        public bool posAsOffset = true;
        public Vector3 startRotation;
        public bool rotAsOffset = true;

        private Vector3 _initialPosition;
        private bool _wasReached;
        private void Update() {
            if (!enableAnimation || LevelManager.playmode != Playmode.Playing) return;
            if (_wasReached) return;
            if (!(DistanceFromPlayer <= activateDistance)) return;

            OnReached();
            _wasReached = true;
        }

        private void OnReached() {
            if (!invisibleRoad) return;

            _tweens.Add(transform.LeanMoveLocal(invisibleRoad.transform.localPosition, duration).setEase(ease));
            _tweens.Add(transform.LeanRotateLocal(invisibleRoad.transform.localEulerAngles, duration).setEase(ease));
        }

        private void MoveOnStart() {
            if (isRandom) {
                switch (randomOption) {
                    case RoadRandomOptions.Move:
                        SetupRandomMove();
                        break;
                    case RoadRandomOptions.Rotate:
                        SetupRandomRotate();
                        break;
                    case RoadRandomOptions.Both:
                        SetupRandomMove();
                        SetupRandomRotate();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else {
                SetupMove();
                SetupRotate();
            }
        }

        private void SetupRandomMove() {
            var x = Random.Range(randomMoveStrength * -1, randomMoveStrength);
            var y = Random.Range(randomMoveStrength * -1, randomMoveStrength);
            var z = Random.Range(randomMoveStrength * -1, randomMoveStrength);
            var position = new Vector3(x, y, z);
            position += transform.localPosition;
            transform.localPosition = position;
        }

        private void SetupRandomRotate() {
            var x = Random.Range(randomRotateStrength * -1, randomRotateStrength);
            var y = Random.Range(randomRotateStrength * -1, randomRotateStrength);
            var z = Random.Range(randomRotateStrength * -1, randomRotateStrength);
            var rotation = new Vector3(x, y, z);
            // rotation += transform.localEulerAngles;
            transform.localEulerAngles = rotation;
        }

        private void SetupMove() {
            var targetPosition = startPosition;
            if (posAsOffset) targetPosition += transform.localPosition;
            transform.localPosition = targetPosition;
        }

        private void SetupRotate() {
            var targetRotation = startRotation;
            if (rotAsOffset) targetRotation += transform.localEulerAngles;
            transform.localEulerAngles = targetRotation;
        }

        public override RoadData OnSerialize() {
            return new RoadData() {
                enableAnimation = enableAnimation,
                activateDistance = activateDistance,
                ease = ease,
                basedOnOrigin = basedOnOrigin,
                duration = duration,
                isRandom = isRandom,
                randomOption = randomOption,
                randomMoveStrength = randomMoveStrength,
                randomRotateStrength = randomRotateStrength,
                startPosition = startPosition,
                posAsOffset = posAsOffset,
                startRotation = startRotation,
                rotAsOffset = rotAsOffset,
                meshData = ModelSerializer.GetMeshData(gameObject)
            };
        }

        public override void OnDeserialized(RoadData obj) {
            gameObject.layer = References.Manager.passthroughLayer;
            Decompile(obj);
            ModelSerializer.ApplyMeshData(gameObject, obj.meshData, false);
        }

        public void Decompile(RoadData obj) {
            enableAnimation = obj.enableAnimation;
            activateDistance = obj.activateDistance;
            basedOnOrigin = obj.basedOnOrigin;
            ease = obj.ease;
            duration = obj.duration;
            isRandom = obj.isRandom;
            randomOption = obj.randomOption;
            randomMoveStrength = obj.randomMoveStrength;
            randomRotateStrength = obj.randomRotateStrength;
            startPosition = obj.startPosition;
            posAsOffset = obj.posAsOffset;
            startRotation = obj.startRotation;
            rotAsOffset = obj.rotAsOffset;
        }

        private void SpawnInvisibleCollider() {
            if (invisibleRoad)
                DestroyImmediate(invisibleRoad);

            var roadCollider = new GameObject("RoadCollider");
            roadCollider.transform.SetParent(transform.parent);
            roadCollider.transform.localPosition = transform.localPosition;
            roadCollider.transform.localEulerAngles = transform.localEulerAngles;
            roadCollider.transform.localScale = transform.localScale;
            roadCollider.AddComponent<BoxCollider>();
            invisibleRoad = roadCollider;
        }

        private Material _mainMaterial;
        private readonly List<LTDescr> _tweens = new();
        public override void OnPlay(bool wasPaused) {
            if (wasPaused) return;

            _initialPosition = transform.position;
            _mainMaterial = GetComponent<MeshRenderer>().sharedMaterial;
            if (enableAnimation) {
                GetComponent<BoxCollider>().isTrigger = true;
                gameObject.layer = References.Manager.passthroughLayer;
                SpawnInvisibleCollider();
                MoveOnStart();
            }
            else {
                GetComponent<BoxCollider>().isTrigger = !info.canCollide;
                gameObject.layer = References.Manager.normalLayer;
            }
        }

        public override void OnStop() {
            if (invisibleRoad)
                DestroyImmediate(invisibleRoad);
            _wasReached = false;
            invisibleRoad = null;
            foreach (var tween in _tweens)
                LeanTween.cancel(tween.uniqueId);
            _tweens.Clear();

            if (_mainMaterial)
                GetComponent<MeshRenderer>().sharedMaterial = _mainMaterial;
        }

        public override void OnVisibilityChange(VisibilityType visibilityType) {
            base.OnVisibilityChange(visibilityType);
            GetComponent<MeshRenderer>().enabled = visibilityType switch {
                VisibilityType.Hidden or VisibilityType.Gone => false,
                _ => true
            };
        }
    }
}