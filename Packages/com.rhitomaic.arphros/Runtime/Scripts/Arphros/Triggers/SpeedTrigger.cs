using UnityEngine;

namespace ArphrosFramework.Triggers
{
    public class SpeedTrigger : TriggerBehavior
    {
        public float targetSpeed;

        public SpeedTrigger(Trigger owner) : base(owner) {}

        public override void OnTriggerEnter(Collider other) => References.Player.speed = targetSpeed;
        public override string Serialize() => targetSpeed.Pack();
        public override void Deserialize(string packed) => targetSpeed = packed.ToFloat();
    }
}
