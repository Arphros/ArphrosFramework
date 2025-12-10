using System;
using UnityEngine;

namespace ArphrosFramework.Triggers
{
    public class JumpTrigger : TriggerBehavior
    {
        public Data data = new();
        private bool alreadyTapped;
        private bool tapRegistered;

        public JumpTrigger(Trigger owner) : base(owner) { }

        public override void OnTriggerEnter(Collider other)
        {
            if (data.requiresTap)
            {
                if (alreadyTapped) return;
                if (tapRegistered) return;

                if (data.haltControl)
                    References.Player.noControlRequest++;
                References.Player.onTurn.AddListener(OnLineTurn);
                tapRegistered = true;
            }
            else
            {
                if (Owner.quickMode && !Owner.IsColliding()) return;

                var rigidbody = References.Player.rigidbody;
                rigidbody.AddForce(data.power, ForceMode.Acceleration);
            }
        }

        public override void OnTriggerExit(Collider other)
        {
            if (alreadyTapped) return;
            if (data.requiresTap)
            {
                if (tapRegistered)
                    References.Player.onTurn.RemoveListener(OnLineTurn);
                if (data.haltControl)
                    References.Player.noControlRequest--;
                tapRegistered = false;
            }
        }

        private void OnLineTurn()
        {
            if (data.onlyTapOnce)
            {
                if (tapRegistered)
                    References.Player.onTurn.RemoveListener(OnLineTurn);
                if (data.haltControl)
                    References.Player.noControlRequest--;

                alreadyTapped = true;
                tapRegistered = false;
            }

            var rigidbody = References.Player.rigidbody;
            rigidbody.AddForce(data.power, ForceMode.Acceleration);
        }

        public override string Serialize()
        {
            string[] packed = {
                data.power.Pack(),
                data.requiresTap.Pack(),
                data.haltControl.Pack(),
                data.onlyTapOnce.Pack()
            };
            return Join(packed);
        }

        public override void Deserialize(string packed)
        {
            if (string.IsNullOrWhiteSpace(packed)) return;

            var split = packed.Split('|');
            if (split.Length < 1) return;

            // Old version only has jump power with no x and z axis
            if (split.Length == 1)
            {
                data.power = new Vector3(0, split[0].ToFloat(), 0);
                return;
            }

            if (split.Length < 4) return;

            data.power = split[0].ToVector3();
            data.requiresTap = split[1].ToBool();
            data.haltControl = split[2].ToBool();
            data.onlyTapOnce = split[3].ToBool();
        }

        public override void OnCloned(ITriggerBehavior original) => data = data.Clone();

        [Serializable]
        public class Data
        {
            public Vector3 power = new(0f, 500f, 0f);
            public bool requiresTap;
            public bool haltControl = true;
            public bool onlyTapOnce = true;

            public Data Clone() => CloneObject(this);
        }
    }
}
