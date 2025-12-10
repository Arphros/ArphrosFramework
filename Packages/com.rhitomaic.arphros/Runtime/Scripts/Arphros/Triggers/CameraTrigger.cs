using System;
using UnityEngine;

namespace ArphrosFramework.Triggers
{
    public class CameraTrigger : TriggerBehavior
    {
        public Data data = new();
        public LeanTweenType ease = LeanTweenType.linear;
        public float duration = 1f;
        public ObjectInfo target;

        public CameraTrigger(Trigger owner) : base(owner) {}

        public override void OnTriggerEnter(Collider other)
        {
            if (References.Manager.cameraType != ArphrosCameraType.StableCamera)
            {
                // TODO: Make a toast/level error when people put this trigger in an incompatible level
                Debug.LogWarning("This camera trigger is not supported in this level!");
                // Toast.Show("This camera trigger is not supported in this level!");
                return;
            }

            var camera = References.Camera;
            camera.oldSystemActive = false;

            if (duration > 0 && !Owner.quickMode)
            {
                if (data.bRotation)
                    Owner.TweenVector3(camera.targetRotation, data.rotation, duration, ease, x => camera.targetRotation = x);
                if (data.bPivotOffset)
                    Owner.TweenVector3(camera.pivotOffset, data.pivotOffset, duration, ease, x => camera.pivotOffset = x);
                if (data.bDistance)
                    Owner.TweenFloat(camera.targetDistance, data.distance, duration, ease, x => camera.targetDistance = x);
                if (data.bFactor)
                    Owner.TweenFloat(camera.smoothFactor, data.factor, duration, ease, x => camera.smoothFactor = x);
            }
            else
            {
                if (data.bRotation) camera.targetRotation = data.rotation;
                if (data.bPivotOffset) camera.pivotOffset = data.pivotOffset;
                if (data.bDistance) camera.targetDistance = data.distance;
                if (data.bFactor) camera.smoothFactor = data.factor;
            }

            if (data.bTarget && target)
                camera.target = target.transform;

            if (data.followImmediate)
                camera.Process(true);
        }

        public override string Serialize()
        {
            string[] packed = {
                data.bRotation.Pack(),
                data.rotation.Pack(),
                data.bPivotOffset.Pack(),
                data.pivotOffset.Pack(),
                data.bDistance.Pack(),
                data.distance.Pack(),
                data.bFactor.Pack(),
                data.factor.Pack(),
                ease.Pack(),
                duration.Pack(),
                data.followImmediate.Pack(),
                data.bTarget.Pack(),
                target.Pack()
            };
            return Join(packed);
        }

        public override void Deserialize(string packed)
        {
            if (string.IsNullOrWhiteSpace(packed)) return;

            var split = Split(packed);
            if (split.Length < 11) return;

            data = new Data()
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

            ease = split[8].ToEnum(LeanTweenType.linear);
            duration = split[9].ToFloat();

            if (data.bTarget)
                split[12].AsObject(val => target = val);
        }

        public override void OnCloned(ITriggerBehavior original) => data = data.Clone();

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

            public Data Clone() => CloneObject(this);
        }
    }
}
