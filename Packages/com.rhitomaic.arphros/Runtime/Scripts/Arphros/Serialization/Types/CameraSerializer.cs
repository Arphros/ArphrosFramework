using ArphrosFramework.Data;
using System;
using UnityEngine;

namespace ArphrosFramework
{
    public class CameraSerializer : ObjectSerializer<CameraData>
    {
        public override void OnDeserialized(CameraData obj)
        {
            var camera = GetComponent<CameraPort>().movement;
            obj.targetId.AsObject(val => camera.target = val.transform, LevelManager.Instance.player.GetInfo());
            camera.pivotOffset = obj.pivotOffset;
            camera.targetRotation = obj.targetRotation;
            camera.targetDistance = obj.targetDistance;
            camera.smoothFactor = obj.smoothFactor;
            camera.localPositionOffset = obj.localPositionOffset;
            camera.localEulerOffset = obj.localEulerOffset;
            camera.mainCamera.fieldOfView = obj.fov;
        }

        public override CameraData OnSerialize()
        {
            var camera = GetComponent<CameraPort>().movement;
            var targetInfo = camera.target.GetInfo();
            return new CameraData()
            {
                targetId = targetInfo != null ? targetInfo.instanceId : 0,
                pivotOffset = camera.pivotOffset,
                targetRotation = camera.targetRotation,
                targetDistance = camera.targetDistance,
                smoothFactor = camera.smoothFactor,
                localPositionOffset = camera.localPositionOffset,
                localEulerOffset = camera.localEulerOffset,
                fov = camera.mainCamera.fieldOfView
            };
        }

        private float fov;
        private float scj;
        private float vj;
        private float hs;
        private float cd;
        private bool wasTouched;
        public override void OnPlay(bool wasPaused)
        {
            base.OnPlay(wasPaused);
            if (wasPaused) return;
            
            var glitch = GetComponent<Kino.AnalogGlitch>();
            // TODO: Why have I not made the References class already?
            // fov = References.MainCamera.fieldOfView;
            scj = glitch.scanLineJitter;
            vj = glitch.verticalJump;
            hs = glitch.horizontalShake;
            cd = glitch.colorDrift;
            wasTouched = true;
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnStop()
        {
            if (!wasTouched) return;
            var glitch = GetComponent<Kino.AnalogGlitch>();

            // TODO: Why have I not made the References class already?
            // References.MainCamera.fieldOfView = fov;
            glitch.scanLineJitter = scj;
            glitch.verticalJump = vj;
            glitch.horizontalShake = hs;
            glitch.colorDrift = cd;
            wasTouched = false;
        }

        private Camera _camera;
        public override bool OnPostRenderSelected()
        {
            _camera ??= GetComponent<Camera>();
            // TODO: Reimplement when RGizmos is restored
            // RGizmos.DrawFrustum(transform.position, transform.rotation, _camera.fieldOfView, _camera.aspect, _camera.nearClipPlane, 2);
            return true;
        }

        public override Action<ObjectInfo> GetRestoreReference(ObjectInfo info) {
            // TODO: Why have I not made the References class already?
            /*if (References.Camera.target == info.transform) {
                return obj => References.Camera.target = obj.transform;
            }*/
            return null;
        }
    }
}