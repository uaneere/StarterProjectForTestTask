using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Presentation
{
    public static class SpriteLoader
    {
        private static readonly Dictionary<string, Sprite> Cache = new();
        private static bool _loaded;

        public static Sprite Load(string logicalName, Color fallback)
        {
            var sprite = Load(logicalName);
            return sprite != null ? sprite : SpriteFactory.Create(fallback);
        }

        public static Sprite Load(string logicalName)
        {
            EnsureLoaded();
            var key = Normalize(logicalName);
            return Cache.TryGetValue(key, out var sprite) ? sprite : null;
        }

        private static void EnsureLoaded()
        {
            if (_loaded)
                return;

            _loaded = true;
            Collect(Resources.LoadAll<Sprite>("Sprites"));
            CollectTextures(Resources.LoadAll<Texture2D>("Sprites"));
        }

        private static void Collect(Sprite[] sprites)
        {
            foreach (var sprite in sprites)
            {
                if (sprite == null)
                    continue;
                Cache[Normalize(sprite.name)] = sprite;
            }
        }

        private static void CollectTextures(Texture2D[] textures)
        {
            foreach (var texture in textures)
            {
                if (texture == null)
                    continue;

                var key = Normalize(texture.name);
                if (Cache.ContainsKey(key))
                    continue;

                Cache[key] = Sprite.Create(
                    texture,
                    new Rect(0f, 0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    100f);
            }
        }

        private static string Normalize(string name)
        {
            return name.Replace(" ", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
        }
    }
}