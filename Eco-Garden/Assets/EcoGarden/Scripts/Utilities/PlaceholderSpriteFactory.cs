using UnityEngine;

namespace EcoGarden.Utilities
{
    public static class PlaceholderSpriteFactory
    {
        private static Sprite squareSprite;

        public static Sprite SquareSprite
        {
            get
            {
                if (squareSprite == null)
                {
                    Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    texture.name = "placeholder_square_texture";
                    texture.SetPixel(0, 0, Color.white);
                    texture.Apply();
                    texture.hideFlags = HideFlags.HideAndDontSave;

                    squareSprite = Sprite.Create(
                        texture,
                        new Rect(0f, 0f, 1f, 1f),
                        new Vector2(0.5f, 0.5f),
                        1f);
                    squareSprite.name = "placeholder_square_sprite";
                    squareSprite.hideFlags = HideFlags.HideAndDontSave;
                }

                return squareSprite;
            }
        }
    }
}
