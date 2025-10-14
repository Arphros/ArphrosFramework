using ArphrosFramework.Data;
using System;
using UnityEngine;

namespace ArphrosFramework {
    public class LightSerializer : ObjectSerializer<LightData> {
        // TODO: Restore when assets system are implemented
        /*
            private Light _pLight;

            private Light _light
            {
                get => _pLight ??= GetComponent<Light>();
                set => _pLight = value;
            }

            public override void OnDeserialized(LightData obj)
            {
                _pLight ??= gameObject.AddOrGetComponent<Light>();
                gameObject.AddOrGetComponent<BoxCollider>().isTrigger = true;

                _light.color = obj.color;
                _light.shadows = obj.shadowType;
                _light.range = obj.range;
                _light.intensity = obj.intensity;

                SpawnBillboard();
            }

            public void SpawnBillboard()
            {
                var obj = transform.Find("Billboard");
                if (obj != null) return;

                var billboard = new GameObject("Billboard");
                billboard.AddOrGetComponent<SpriteRenderer>().sprite = Level.LevelManager.Instance.lightSprite;
                billboard.AddOrGetComponent<BillboardSprite>();
                billboard.transform.SetParent(transform, false);
            }

            public override LightData OnSerialize()
            {
                return new LightData() {
                    color = _light.color,
                    intensity = _light.intensity,
                    shadowType = _light.shadows,
                    range = _light.range
                };
            }

            private bool _isCached;
            private Color _lightColor;
            public override void OnPlay(bool wasPaused)
            {
                if (wasPaused) return;

                _lightColor = _light.color;
                _isCached = true;
            }

            public override void OnStop()
            {
                if (!_isCached) return;

                _light.color = _lightColor;
                _isCached = false;
            }

            public override void OnVisibilityChange(VisibilityType visibilityType)
            {
                switch (visibilityType)
                {
                    case VisibilityType.Shown:
                        gameObject.SetActive(true);
                        _light.enabled = true;
                        break;
                    case VisibilityType.Hidden:
                        gameObject.SetActive(true);
                        _light.enabled = false;
                        break;
                    case VisibilityType.Gone:
                        gameObject.SetActive(false);
                        break;
                }
            }

            public override bool OnPostRenderSelected()
            {
                RGizmos.DrawSphere(transform.position, transform.rotation, _light.range);
                return true;
            }

            private void OnDestroy() {
                if (!References.Inspector) return;
                if (References.Inspector.selectedDrawCalls.Contains(OnPostRenderSelected))
                    References.Inspector.selectedDrawCalls.Remove(OnPostRenderSelected);
            }
        */
    }
}