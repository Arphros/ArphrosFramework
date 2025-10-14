using UnityEngine;
using TMPro;
using ArphrosFramework.Data;

namespace ArphrosFramework
{
    public class TextSerializer : ObjectSerializer<TextData>
    {
        public new TextMeshPro renderer;
        public new BoxCollider collider;

        public override void OnDeserialized(TextData obj)
        {
            renderer = GetComponent<TextMeshPro>();
            collider = GetComponent<BoxCollider>();
            renderer.text = obj.text;
            renderer.font = LevelManager.Instance.fonts[obj.fontIndex];
            renderer.fontSize = obj.fontSize;
            renderer.color = obj.color;
            renderer.horizontalAlignment = obj.horizontalAlignment;
            renderer.verticalAlignment = obj.verticalAlignment;
            (transform as RectTransform).sizeDelta = obj.boundsSize;
            AdjustBoxCollider();
            FuckUpMyMaterials();
        }

        public override TextData OnSerialize()
        {
            renderer = GetComponent<TextMeshPro>();
            collider = GetComponent<BoxCollider>();
            return new TextData()
            {
                text = renderer.text,
                fontIndex = GetFontIndex(renderer.font),
                fontSize = renderer.fontSize,
                color = renderer.color,
                horizontalAlignment = renderer.horizontalAlignment,
                verticalAlignment = renderer.verticalAlignment,
                boundsSize = (transform as RectTransform).sizeDelta
            };
        }

        /// <summary>
        /// Changes the material so it's compatible with fog
        /// </summary>
        public void FuckUpMyMaterials()
        {
            var newMaterials = renderer.fontSharedMaterials;
            for (int i = 0; i < newMaterials.Length; i++)
                newMaterials[i] = RefogMaterial(newMaterials[i]);
            renderer.fontSharedMaterials = newMaterials;
        }

        /// <summary>
        /// Changes the material so it's compatible with fog
        /// </summary>
        public static void FuckUpMyMaterials(TextMeshPro renderer)
        {
            var newMaterials = renderer.fontSharedMaterials;
            for (int i = 0; i < newMaterials.Length; i++)
                newMaterials[i] = RefogMaterial(newMaterials[i]);
            renderer.fontSharedMaterials = newMaterials;
        }

        public static Material RefogMaterial(Material material)
        {
            var newMaterial = new Material(material);
            newMaterial.shader = LevelManager.Instance.foggedTextShader;
            return newMaterial;
        }

        private int GetFontIndex(TMP_FontAsset source)
        {
            var fonts = LevelManager.Instance.fonts;
            for (int i = 0; i < fonts.Length; i++)
            {
                if (fonts[i] == source)
                    return i;
            }
            return 0;
        }

        public void AdjustBoxCollider()
        {
            collider = GetComponent<BoxCollider>();
            var bounds = (transform as RectTransform).sizeDelta;
            collider.size = new Vector3(bounds.x, bounds.y, 1);
        }

        Color _mainColor;
        bool _wasCached;
        string _text;

        public override void OnPlay(bool wasPaused)
        {
            if (wasPaused)
            {
            }
            else
            {
                _text = renderer.text;
                _mainColor = renderer.color;
                _wasCached = true;
            }
        }

        public override void OnStop()
        {
            if (_wasCached)
            {
                renderer.color = _mainColor;
                renderer.text = _text;
                _wasCached = false;
            }
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

        // TODO: Reimplement when gizmos are implemented
        public override bool OnPostRenderSelected()
        {
            var bounds = ((RectTransform)transform).sizeDelta * transform.localScale;
            // RGizmos.DrawCube(transform.position, transform.rotation, new Vector3(bounds.x, bounds.y, 0.2f));
            return true;
        }
    }
}