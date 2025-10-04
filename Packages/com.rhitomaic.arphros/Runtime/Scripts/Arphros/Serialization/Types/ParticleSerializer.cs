using UnityEngine;
using ArphrosFramework.Level;
using TMPro;

namespace ArphrosFramework {
    public class ParticleSerializer : ObjectSerializer<ParticleData> {
        public new ParticleSystem renderer;
        public override void OnDeserialized(ParticleData obj) {
            renderer = GetComponent<ParticleSystem>();
        }

        public override ParticleData OnSerialize() {
            renderer = GetComponent<ParticleSystem>();
            return new ParticleData() {
            };
        }

        Color _mainColor;
        bool _wasCached;
        public override void OnPlay(bool wasPaused) {
            if (wasPaused) {
            }
            else {
                // _mainColor = renderer.color;
                _wasCached = true;
            }
        }

        public override void OnStop() {
            if (_wasCached) {
                // renderer.color = _mainColor;
                _wasCached = false;
            }
        }

        public override void OnVisibilityChange(VisibilityType visibilityType) {
            switch (visibilityType) {
                case VisibilityType.Shown:
                    gameObject.SetActive(true);
                    break;
                case VisibilityType.Hidden:
                case VisibilityType.Gone:
                    gameObject.SetActive(false);
                    break;
            }
        }
    }
}