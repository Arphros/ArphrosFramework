using UnityEngine;

namespace ArphrosFramework {
    // TODO: Combine this with the player script instead of being separate
    public class PlayerSerializer : ObjectSerializer<PlayerData> {
        [SerializeField]
        private new MeshRenderer renderer;
        [SerializeField]
        private PlayerMovement movement;

        public override void OnDeserialized(PlayerData obj) {
            ModelSerializer.ApplyMeshData(gameObject, obj.data);
            gameObject.GetComponent<BoxCollider>();

            movement = GetComponent<PlayerMovement>();
            movement.speed = obj.speed;
            movement.turn1 = obj.direction1;
            movement.turn2 = obj.direction2;
        }

        public override PlayerData OnSerialize() {
            var data = new PlayerData();
            movement = GetComponent<PlayerMovement>();
            data.direction1 = movement.turn1;
            data.direction2 = movement.turn2;
            data.speed = movement.speed;
            data.data = ModelSerializer.GetMeshData(gameObject);
            return data;
        }

        private Material _previousMaterial;
        void Update() {
            if (_previousMaterial == null)
                _previousMaterial = renderer.sharedMaterial;

            if (renderer.sharedMaterial != _previousMaterial) {
                _previousMaterial = renderer.sharedMaterial;
                movement.ChangeAllTailMaterials(_previousMaterial);
            }
        }

        Material mainMaterial;
        // TODO: Reimplement the tail type workaround
        public override void OnPlay(bool wasPaused) {
            if (wasPaused) {
                //References.Player.ReduceByOne();
            }
            else {
                mainMaterial = renderer.sharedMaterial;
            }
        }

        public override void OnPause() {
            base.OnPause();
            //References.Player.ExtendByOne();
        }

        public override void OnStop() {
            renderer.sharedMaterial = mainMaterial;
        }

        public override void OnVisibilityChange(VisibilityType visibilityType) {
            base.OnVisibilityChange(visibilityType);
            switch (visibilityType) {
                case VisibilityType.Shown:
                    renderer.enabled = true;
                    break;
                case VisibilityType.Hidden:
                    renderer.enabled = false;
                    break;
                default:
                    // TODO: Reimplement this weird warning I guess lol
                    //if (!LevelManager.IsNotEditor)
                    //Toast.Show("The player resists.");
                    break;
            }
        }
    }
}