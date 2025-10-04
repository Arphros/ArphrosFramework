using System;

namespace ArphrosFramework {
    [Serializable]
    public class SpriteData {
        public int id;
        public string fileName;
        public string content;

        public void Populate(byte[] bytes)
            => content = Convert.ToBase64String(bytes);

        public byte[] GetBytes() =>
            string.IsNullOrWhiteSpace(content) ? Array.Empty<byte>() : Convert.FromBase64String(content);
    }
}