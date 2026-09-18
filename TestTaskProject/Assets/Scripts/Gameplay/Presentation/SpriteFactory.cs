using UnityEngine;

namespace Gameplay.Presentation
{
    public static class SpriteFactory
    {
        public static Sprite Create(Color color, int size = 32)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color[size * size];
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var border = x == 0 || y == 0 || x == size - 1 || y == size - 1;
                    pixels[y * size + x] = border ? Color.Lerp(color, Color.black, 0.45f) : color;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }
    }
}