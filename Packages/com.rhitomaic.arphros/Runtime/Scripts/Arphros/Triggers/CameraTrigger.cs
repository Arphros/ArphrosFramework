using System;
using UnityEngine;

namespace ArphrosFramework
{
    public class CameraTrigger : TriggerBehavior
    {
        private Data _data;
        private LeanTweenType _ease;
        private float _duration;
        private ObjectInfo _target;

        public CameraTrigger(Trigger owner, Data data, LeanTweenType ease, float duration, ObjectInfo target)
            : base(owner)
        {
            _data = data;
            _ease = ease;
            _duration = duration;
            _target = target;
        }

        public override void OnTriggerEnter(Collider other)
        {
            if (LevelManager.Instance.cameraType != ArphrosCameraType.StableCamera)
            {
                // TODO: Make a toast/level error when people put this trigger in an incompatible level
                Debug.LogWarning("This camera trigger is not supported in this level!");
                // Toast.Show("This camera trigger is not supported in this level!");
                return;
            }

            var camera = References.Camera;
            camera.oldSystemActive = false;

            if (_duration > 0 && !Owner.quickMode)
            {
                if (_data.bRotation)
                    Owner.TweenVector3(camera.targetRotation, _data.rotation, _duration, _ease, x => camera.targetRotation = x);
                if (_data.bPivotOffset)
                    Owner.TweenVector3(camera.pivotOffset, _data.pivotOffset, _duration, _ease, x => camera.pivotOffset = x);
                if (_data.bDistance)
                    Owner.TweenFloat(camera.targetDistance, _data.distance, _duration, _ease, x => camera.targetDistance = x);
                if (_data.bFactor)
                    Owner.TweenFloat(camera.smoothFactor, _data.factor, _duration, _ease, x => camera.smoothFactor = x);
            }
            else
            {
                if (_data.bRotation) camera.targetRotation = _data.rotation;
                if (_data.bPivotOffset) camera.pivotOffset = _data.pivotOffset;
                if (_data.bDistance) camera.targetDistance = _data.distance;
                if (_data.bFactor) camera.smoothFactor = _data.factor;
            }

            if (_data.bTarget && _target)
                camera.target = _target.transform;

            if (_data.followImmediate)
                camera.Process(true);
        }

        public override string Serialize()
        {
            string[] data = {
                _data.bRotation.Pack(),
                _data.rotation.Pack(),
                _data.bPivotOffset.Pack(),
                _data.pivotOffset.Pack(),
                _data.bDistance.Pack(),
                _data.distance.Pack(),
                _data.bFactor.Pack(),
                _data.factor.Pack(),
                _ease.Pack(),
                _duration.Pack(),
                _data.followImmediate.Pack(),
                _data.bTarget.Pack(),
                _target.Pack()
            };
            return Join(data);
        }

        public override void Deserialize(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;

            var split = Split(data);
            if (split.Length < 11) return;

            _data = new Data()
            {
                bRotation = split[0].ToBool(),
                rotation = split[1].ToVector3(),
                bPivotOffset = split[2].ToBool(),
                pivotOffset = split[3].ToVector3(),
                bDistance = split[4].ToBool(),
                distance = split[5].ToFloat(),
                bFactor = split[6].ToBool(),
                factor = split[7].ToFloat(),
                followImmediate = split[10].ToBool(),
                bTarget = split[11].ToBool()
            };

            _ease = split[8].ToEnum(LeanTweenType.linear);
            _duration = split[9].ToFloat();

            if (_data.bTarget)
                split[12].AsObject(val => _target = val);
        }

        [Serializable]
        public class Data
        {
            public bool bRotation;
            public Vector3 rotation = new(45, 45, 0);
            public bool bPivotOffset;
            public Vector3 pivotOffset = new(-5, 0, -5);
            public bool bDistance;
            public float distance = 20;
            public bool bFactor;
            public float factor = 0.1f;

            public bool bTarget;
            public bool followImmediate;

            public Data Clone()
            {
                return CloneObject(this);
            }
        }
    }
}
