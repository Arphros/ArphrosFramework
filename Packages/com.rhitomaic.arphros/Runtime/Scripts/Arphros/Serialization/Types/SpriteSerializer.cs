using UnityEngine;
using ArphrosFramework.Level;

namespace ArphrosFramework {
    public class SpriteSerializer : ObjectSerializer<SpriteRendData> {
        public new SpriteRenderer renderer;
        // TODO: Restore when assets system are implemented
        /*
            private void Awake()
            {
                SpriteManager.preRefresh += SpriteManager_preRefresh;
                SpriteManager.postRefresh += SpriteManager_postRefresh;
            }

            private int spriteId;
            private void SpriteManager_preRefresh() => spriteId = SpriteManager.GetSpriteId(renderer.sprite);
            private void SpriteManager_postRefresh() => renderer.sprite = SpriteManager.GetObject(spriteId).sprite;

            public override void OnDeserialized(SpriteRendData obj)
            {
                ApplySpriteData(gameObject, obj, true);
                renderer = GetComponent<SpriteRenderer>();
            }

            public override SpriteRendData OnSerialize() => GetSpriteData(gameObject);

            Color _mainColor;
            bool _wasCached;
            public override void OnPlay(bool wasPaused)
            {
                if (wasPaused)
                {
                }
                else
                {
                    _mainColor = renderer.color;
                    _wasCached = true;
                }
            }

            public override void OnStop()
            {
                if (_wasCached)
                {
                    renderer.color = _mainColor;
                    _wasCached = false;
                }
            }

            public static SpriteRendData GetSpriteData(GameObject obj)
            {
                var data = new SpriteRendData();
                var renderer = obj.GetComponent<SpriteRenderer>();
                data.spriteId = SpriteManager.GetSpriteId(renderer.sprite);
                data.color = renderer.color;
                data.affectByFog = renderer.sharedMaterial == LevelManager.Instance.fogSpriteMaterial;
                data.zIndex = renderer.sortingOrder;
                return data;
            }

            /// <summary>
            /// Applying the sprite the data to the GameObject.
            /// If model isn't found, it will use the cube sprite.
            /// If material isn't found, it will use the default material.
            /// </summary>
            /// <param name="obj">Target GameObject</param>
            /// <param name="data">The model data</param>
            /// <param name="withCollider">Using sprite collider with the sprite from the data</param>
            public static void ApplySpriteData(GameObject obj, SpriteRendData data, bool withCollider = false)
            {
                var renderer = obj.AddOrGetComponent<SpriteRenderer>();
                renderer.sortingOrder = data.zIndex;
                renderer.sharedMaterial = data.affectByFog ? LevelManager.Instance.fogSpriteMaterial : LevelManager.Instance.spriteMaterial;

                obj.layer = References.Manager.passthroughLayer;
                var inst = SpriteManager.GetObject(data.spriteId);

                if (inst != null)
                    renderer.sprite = inst.sprite;
                else
                    renderer.sprite = LevelManager.Instance.squareSprite;

                if (withCollider)
                    obj.AddOrGetComponent<BoxCollider>().isTrigger = true;

                renderer.color = data.color;
            }

            public override void OnVisibilityChange(VisibilityType visibilityType)
            {
                switch (visibilityType)
                {
                    case VisibilityType.Shown:
                        gameObject.SetActive(true);
                        renderer.enabled = true;
                        break;
                    case VisibilityType.Hidden:
                        gameObject.SetActive(true);
                        renderer.enabled = false;
                        break;
                    case VisibilityType.Gone:
                        gameObject.SetActive(false);
                        break;
                }
            }

            private void OnDestroy()
            {
                SpriteManager.preRefresh -= SpriteManager_preRefresh;
                SpriteManager.postRefresh -= SpriteManager_postRefresh;

                if (References.Inspector) {
                    if (References.Inspector.selectedDrawCalls.Contains(OnPostRenderSelected)) {
                        References.Inspector.selectedDrawCalls.Remove(OnPostRenderSelected);
                    }
                }
            }

            private BoxCollider _collider;
            private Vector2 _bounds;
            public override bool OnPostRenderSelected()
            {
                _collider ??= GetComponent<BoxCollider>();
                if (!_collider) return false;

                _bounds.x = _collider.size.x * transform.localScale.x;
                _bounds.y = _collider.size.y * transform.localScale.y;

                RGizmos.DrawCube(transform.position, transform.rotation, new Vector3(_bounds.x, _bounds.y, 0.2f));
                return true;
            }
        */
    }
}