using System;
using UnityEngine;

namespace ArphrosFramework {
    // TODO: Properly reimplement this when the assets system is reworked
    [Serializable]
    public class MaterialData {
        public int id = -1;
        public int spriteId = -1;
        public string name = "Unnamed";
        public int type;

        public Color color;

        public float metallic;
        public float smoothness;

        public bool emission;
        public Color emissionColor = Color.white;

        // public string textureFile;
        public bool specularHighlights = true;
        public bool reflections = true;

        private static readonly int mainTex = Shader.PropertyToID("_MainTex");
        private static readonly int glossyReflections = Shader.PropertyToID("_GlossyReflections");
        private static readonly int highlights = Shader.PropertyToID("_SpecularHighlights");
        private static readonly int emmColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int glossiness = Shader.PropertyToID("_Glossiness");
        private static readonly int metallicKey = Shader.PropertyToID("_Metallic");
        private static readonly int colorKey = Shader.PropertyToID("_Color");
        private static readonly int mode = Shader.PropertyToID("_Mode");

        public MaterialData() { }
        public MaterialData(int id, string name, Color color) {
            this.id = id;
            type = 0;
            this.name = name;

            this.color = color;
            metallic = 0;
            smoothness = 0;

            emission = false;
            emissionColor = Color.black;

            /*var tex = material.GetTexture("_MainTex");
            if (tex != null)
                textureFile = tex.name;*/

            specularHighlights = true;
            reflections = true;
        }

        public MaterialData(int id, string name, Color color, float smoothness) {
            this.id = id;
            type = 0;
            this.name = name;

            this.color = color;
            metallic = 0;
            this.smoothness = smoothness;

            emission = false;
            emissionColor = Color.black;

            /*var tex = material.GetTexture("_MainTex");
            if (tex != null)
                textureFile = tex.name;*/

            specularHighlights = true;
            reflections = true;
        }

        public MaterialData(Material material) {
            // id = MaterialManager.GetMaterialId(material);

            type = material.GetInt(mode);
            name = material.name;

            color = material.GetColor(colorKey);
            metallic = material.GetFloat(metallicKey);
            smoothness = material.GetFloat(glossiness);

            emission = material.IsKeywordEnabled("_EMISSION");
            emissionColor = material.GetColor(emmColor);

            /*var tex = material.GetTexture("_MainTex");
            if (tex != null)
                textureFile = tex.name;*/

            specularHighlights = material.GetInt(highlights) == 1;
            reflections = material.GetInt(glossyReflections) == 1;

            var texture = material.GetTexture(mainTex);
            // var instanced = texture ? SpriteManager.Find(val => val.sprite.texture == texture) : null;
            // spriteId = instanced ? instanced.instanceId : -1;
        }

        public void Apply(Material material) {
            material.name = name;

            // MeshRendererHost.SetupMaterialWithBlendMode(material, (MaterialType)type);
            material.SetInt(mode, type);
            material.SetColor(colorKey, color);
            material.SetFloat(metallicKey, metallic);
            material.SetFloat(glossiness, smoothness);

            if (emission)
                material.EnableKeyword("_EMISSION");
            else
                material.DisableKeyword("_EMISSION");
            material.SetColor(emmColor, emissionColor);

            material.SetInt(highlights, specularHighlights ? 1 : 0);
            material.SetInt(glossyReflections, reflections ? 1 : 0);

            /*if (spriteId > -1)
                material.SetTexture(mainTex, SpriteManager.GetObject(spriteId).sprite.texture);*/
        }
    }

    public enum MaterialType {
        Opaque = 0,
        Cutout = 1,
        Fade = 2,
        Transparent = 3
    }
}