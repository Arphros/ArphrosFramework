using ArphrosFramework.Level;
using System.Collections;
using UnityEngine;

namespace ArphrosFramework
{
    public class StartPositionSerializer : ObjectSerializer<StartPositionData>
    {
        public StartPositionData positionData;

        public override void OnDeserialized(StartPositionData obj)
        {
            tag = "StartPosition";
            positionData = obj;
            SpawnBillboard();
        }

        public void SpawnBillboard()
        {
            var obj = transform.Find("Billboard");
            if (obj != null) return;

            var billboard = new GameObject("Billboard");
            billboard.AddOrGetComponent<SpriteRenderer>().sprite = LevelManager.Instance.startPosSprite;
            // TODO: Reimplement when billboard is available
            // billboard.AddOrGetComponent<BillboardSprite>();
            billboard.transform.SetParent(transform, false);
        }

        public override StartPositionData OnSerialize()
        {
            return positionData;
        }

        public override void OnPlay(bool wasPaused)
        {
            // TODO: What the hell is happening? Why did I do this, am I trying to move it when everything's ready or something?
            /*References.Editor.PostPlay += val =>
            {
                if (!val && !LevelManager.IsNotEditor)
                    StartCoroutine(AftermathAsync());
            };*/
        }

        IEnumerator AftermathAsync()
        {
            yield return new WaitForEndOfFrame();
            // TODO: Too many to fix, let's deal with this later
            /*var player = References.Player;

            if (!player) yield break;
            
            foreach (var id in positionData.calledTriggers)
            {
                var obj = LevelManager.GetObject(id);
                if (!obj) continue;
                
                var trigger = obj.GetComponent<Trigger>();
                if (!trigger) continue;
                if (trigger.IsColliding()) continue;
                
                trigger.CallInQuickMode();
                trigger.OnTriggerExit(References.Player.collider);
            }

            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
            player.turn1 = positionData.turn1;
            player.turn2 = positionData.turn2;
            player.speed = positionData.speed;
            player.loopCount = positionData.loopCount;
            player.additionalOffset = positionData.audioTime;

            var cameraMovement = References.CamPort.movement;
            cameraMovement.transform.position = positionData.cameraPosition;
            cameraMovement.transform.eulerAngles = positionData.cameraRotation;*/
        }
    }
}