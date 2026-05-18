using UnityEngine;

namespace EcoGarden.Utilities
{
    public static class PlaceholderSpriteFactory
    {
        private const int SpriteSize = 64;

        private static Sprite squareSprite;
        private static Sprite emptyTileSprite;
        private static Sprite lockedTileSprite;
        private static Sprite weedSprite;
        private static Sprite pebbleSprite;
        private static Sprite producerSprite;
        private static Sprite npcSprite;
        private static Sprite butterflySprite;
        private static Sprite hudTopBarSprite;
        private static Sprite hudPanelSprite;
        private static Sprite hudButtonSprite;
        private static Sprite deliverZoneSprite;
        private static Sprite sellBasketSprite;
        private static readonly Sprite[] lotusSprites = new Sprite[6];

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

        public static Sprite EmptyTileSprite
        {
            get
            {
                if (emptyTileSprite == null)
                {
                    emptyTileSprite = CreateRoundedRectSprite(
                        "tile_empty_01_runtime",
                        new Color(0.58f, 0.78f, 0.74f, 1f),
                        new Color(0.78f, 0.91f, 0.87f, 1f),
                        9,
                        3);
                }

                return emptyTileSprite;
            }
        }

        public static Sprite LockedTileSprite
        {
            get
            {
                if (lockedTileSprite == null)
                {
                    lockedTileSprite = CreateRoundedRectSprite(
                        "tile_locked_01_runtime",
                        new Color(0.15f, 0.18f, 0.22f, 1f),
                        new Color(0.29f, 0.35f, 0.42f, 1f),
                        8,
                        3);
                }

                return lockedTileSprite;
            }
        }

        public static Sprite WeedSprite
        {
            get
            {
                if (weedSprite == null)
                {
                    weedSprite = CreateWeedSprite();
                }

                return weedSprite;
            }
        }

        public static Sprite PebbleSprite
        {
            get
            {
                if (pebbleSprite == null)
                {
                    pebbleSprite = CreatePebbleSprite();
                }

                return pebbleSprite;
            }
        }

        public static Sprite ProducerSprite
        {
            get
            {
                if (producerSprite == null)
                {
                    producerSprite = CreateProducerSprite();
                }

                return producerSprite;
            }
        }

        public static Sprite NpcSprite
        {
            get
            {
                if (npcSprite == null)
                {
                    npcSprite = CreateNpcSprite();
                }

                return npcSprite;
            }
        }

        public static Sprite ButterflySprite
        {
            get
            {
                if (butterflySprite == null)
                {
                    butterflySprite = CreateButterflySprite();
                }

                return butterflySprite;
            }
        }

        public static Sprite HudTopBarSprite
        {
            get
            {
                if (hudTopBarSprite == null)
                {
                    hudTopBarSprite = CreateRoundedRectSprite(
                        "ui_top_bar_runtime",
                        new Color(0.12f, 0.20f, 0.25f, 0.94f),
                        new Color(0.42f, 0.66f, 0.64f, 1f),
                        10,
                        4);
                }

                return hudTopBarSprite;
            }
        }

        public static Sprite HudPanelSprite
        {
            get
            {
                if (hudPanelSprite == null)
                {
                    hudPanelSprite = CreateRoundedRectSprite(
                        "ui_panel_runtime",
                        new Color(0.16f, 0.23f, 0.25f, 0.90f),
                        new Color(0.58f, 0.78f, 0.72f, 1f),
                        12,
                        4);
                }

                return hudPanelSprite;
            }
        }

        public static Sprite HudButtonSprite
        {
            get
            {
                if (hudButtonSprite == null)
                {
                    hudButtonSprite = CreateButtonSprite();
                }

                return hudButtonSprite;
            }
        }

        public static Sprite DeliverZoneSprite
        {
            get
            {
                if (deliverZoneSprite == null)
                {
                    deliverZoneSprite = CreateDeliverZoneSprite();
                }

                return deliverZoneSprite;
            }
        }

        public static Sprite SellBasketSprite
        {
            get
            {
                if (sellBasketSprite == null)
                {
                    sellBasketSprite = CreateSellBasketSprite();
                }

                return sellBasketSprite;
            }
        }

        public static Sprite GetLotusSprite(int level)
        {
            int safeLevel = Mathf.Clamp(level, 1, 5);
            if (lotusSprites[safeLevel] == null)
            {
                lotusSprites[safeLevel] = CreateLotusSprite(safeLevel);
            }

            return lotusSprites[safeLevel];
        }

        private static Sprite CreateRoundedRectSprite(string name, Color fill, Color accent, int cornerRadius, int border)
        {
            Texture2D texture = CreateTexture(name);
            Vector2 center = new Vector2((SpriteSize - 1) * 0.5f, (SpriteSize - 1) * 0.5f);

            for (int y = 0; y < SpriteSize; y++)
            {
                for (int x = 0; x < SpriteSize; x++)
                {
                    bool inside = IsInsideRoundedRect(x, y, SpriteSize, SpriteSize, cornerRadius);
                    if (!inside)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    bool edge = x < border || y < border || x >= SpriteSize - border || y >= SpriteSize - border;
                    float highlight = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(x, y), center) / 48f);
                    Color color = edge ? accent : Color.Lerp(fill, accent, highlight * 0.18f);
                    texture.SetPixel(x, y, color);
                }
            }

            return FinalizeSprite(texture);
        }

        private static Sprite CreateLotusSprite(int level)
        {
            Texture2D texture = CreateTexture("item_lotus_lv0" + level + "_runtime");
            Color stem = new Color(0.25f, 0.55f, 0.34f, 1f);
            Color leaf = new Color(0.35f, 0.72f, 0.50f, 1f);
            Color bud = new Color(0.84f, 0.48f, 0.68f, 1f);
            Color petal = new Color(0.98f, 0.70f, 0.86f, 1f);
            Color seed = new Color(0.73f, 0.55f, 0.34f, 1f);

            FillClear(texture);

            if (level == 1)
            {
                DrawEllipse(texture, 32, 30, 12, 10, seed);
                DrawEllipse(texture, 27, 34, 5, 4, new Color(0.58f, 0.42f, 0.26f, 1f));
                DrawEllipse(texture, 37, 27, 4, 3, new Color(0.88f, 0.68f, 0.45f, 1f));
            }
            else
            {
                DrawLine(texture, 32, 14, 32, 40, 5, stem);
                DrawEllipse(texture, 24, 24, 13, 6, leaf);

                if (level >= 3)
                {
                    DrawEllipse(texture, 41, 28, 13, 6, leaf);
                }

                if (level >= 2)
                {
                    DrawEllipse(texture, 32, 43, 8, 12, level >= 4 ? bud : leaf);
                }

                if (level >= 5)
                {
                    DrawPetal(texture, 32, 44, 0f, petal);
                    DrawPetal(texture, 32, 42, 55f, petal);
                    DrawPetal(texture, 32, 42, -55f, petal);
                    DrawPetal(texture, 31, 39, 105f, new Color(0.94f, 0.56f, 0.78f, 1f));
                    DrawPetal(texture, 33, 39, -105f, new Color(0.94f, 0.56f, 0.78f, 1f));
                    DrawEllipse(texture, 32, 38, 5, 4, new Color(1f, 0.84f, 0.34f, 1f));
                }
            }

            return FinalizeSprite(texture);
        }

        private static Sprite CreateWeedSprite()
        {
            Texture2D texture = CreateTexture("obs_weed_01_runtime");
            FillClear(texture);
            Color dark = new Color(0.22f, 0.47f, 0.24f, 1f);
            Color light = new Color(0.42f, 0.74f, 0.34f, 1f);
            DrawLine(texture, 18, 14, 31, 48, 5, dark);
            DrawLine(texture, 31, 12, 31, 50, 6, light);
            DrawLine(texture, 46, 15, 33, 48, 5, dark);
            DrawEllipse(texture, 23, 36, 11, 5, light);
            DrawEllipse(texture, 40, 33, 11, 5, light);
            return FinalizeSprite(texture);
        }

        private static Sprite CreatePebbleSprite()
        {
            Texture2D texture = CreateTexture("obs_pebble_01_runtime");
            FillClear(texture);
            DrawEllipse(texture, 28, 30, 17, 12, new Color(0.48f, 0.44f, 0.39f, 1f));
            DrawEllipse(texture, 40, 27, 12, 9, new Color(0.58f, 0.54f, 0.49f, 1f));
            DrawEllipse(texture, 24, 35, 8, 5, new Color(0.36f, 0.33f, 0.30f, 1f));
            return FinalizeSprite(texture);
        }

        private static Sprite CreateProducerSprite()
        {
            Texture2D texture = CreateTexture("producer_lotus_seed_01_runtime");
            FillClear(texture);
            DrawEllipse(texture, 32, 30, 22, 17, new Color(0.30f, 0.62f, 0.78f, 1f));
            DrawEllipse(texture, 32, 35, 15, 10, new Color(0.58f, 0.82f, 0.87f, 1f));
            DrawEllipse(texture, 32, 35, 7, 5, new Color(0.74f, 0.55f, 0.33f, 1f));
            DrawLine(texture, 16, 20, 48, 20, 3, new Color(0.22f, 0.45f, 0.63f, 1f));
            return FinalizeSprite(texture);
        }

        private static Sprite CreateNpcSprite()
        {
            Texture2D texture = CreateTexture("char_customer_01_runtime");
            FillClear(texture);
            DrawEllipse(texture, 32, 44, 10, 9, new Color(0.98f, 0.78f, 0.62f, 1f));
            DrawEllipse(texture, 32, 25, 13, 15, new Color(0.64f, 0.38f, 0.74f, 1f));
            DrawEllipse(texture, 25, 48, 3, 2, new Color(0.18f, 0.15f, 0.20f, 1f));
            DrawEllipse(texture, 39, 48, 3, 2, new Color(0.18f, 0.15f, 0.20f, 1f));
            DrawLine(texture, 26, 12, 26, 20, 3, new Color(0.30f, 0.23f, 0.34f, 1f));
            DrawLine(texture, 38, 12, 38, 20, 3, new Color(0.30f, 0.23f, 0.34f, 1f));
            return FinalizeSprite(texture);
        }

        private static Sprite CreateButterflySprite()
        {
            Texture2D texture = CreateTexture("char_butterfly_runtime");
            FillClear(texture);
            DrawEllipse(texture, 23, 36, 13, 10, new Color(1f, 0.72f, 0.30f, 1f));
            DrawEllipse(texture, 41, 36, 13, 10, new Color(1f, 0.72f, 0.30f, 1f));
            DrawEllipse(texture, 25, 25, 10, 8, new Color(0.92f, 0.48f, 0.76f, 1f));
            DrawEllipse(texture, 39, 25, 10, 8, new Color(0.92f, 0.48f, 0.76f, 1f));
            DrawLine(texture, 32, 20, 32, 43, 3, new Color(0.20f, 0.15f, 0.24f, 1f));
            return FinalizeSprite(texture);
        }

        private static Sprite CreateButtonSprite()
        {
            Texture2D texture = CreateTexture("ui_button_runtime");
            FillClear(texture);
            DrawRoundedFill(texture, new Color(0.20f, 0.45f, 0.55f, 1f), new Color(0.58f, 0.88f, 0.84f, 1f), 12, 4);
            DrawEllipse(texture, 18, 43, 5, 5, new Color(0.78f, 0.94f, 0.90f, 0.65f));
            return FinalizeSprite(texture);
        }

        private static Sprite CreateDeliverZoneSprite()
        {
            Texture2D texture = CreateTexture("ui_deliver_zone_runtime");
            FillClear(texture);
            DrawRoundedFill(texture, new Color(0.44f, 0.28f, 0.58f, 0.96f), new Color(0.88f, 0.66f, 0.96f, 1f), 14, 4);
            DrawEllipse(texture, 32, 38, 18, 9, new Color(0.96f, 0.70f, 0.88f, 1f));
            DrawEllipse(texture, 32, 36, 9, 5, new Color(1f, 0.86f, 0.42f, 1f));
            DrawLine(texture, 19, 24, 45, 24, 4, new Color(0.30f, 0.18f, 0.40f, 1f));
            return FinalizeSprite(texture);
        }

        private static Sprite CreateSellBasketSprite()
        {
            Texture2D texture = CreateTexture("ui_sell_basket_runtime");
            FillClear(texture);
            DrawRoundedFill(texture, new Color(0.28f, 0.43f, 0.32f, 0.96f), new Color(0.72f, 0.86f, 0.48f, 1f), 14, 4);
            DrawLine(texture, 19, 26, 45, 26, 5, new Color(0.66f, 0.45f, 0.25f, 1f));
            DrawLine(texture, 23, 24, 27, 45, 4, new Color(0.74f, 0.52f, 0.30f, 1f));
            DrawLine(texture, 41, 24, 37, 45, 4, new Color(0.74f, 0.52f, 0.30f, 1f));
            DrawLine(texture, 25, 44, 39, 44, 4, new Color(0.74f, 0.52f, 0.30f, 1f));
            DrawEllipse(texture, 32, 34, 8, 8, new Color(1f, 0.82f, 0.24f, 1f));
            return FinalizeSprite(texture);
        }

        private static Texture2D CreateTexture(string name)
        {
            Texture2D texture = new Texture2D(SpriteSize, SpriteSize, TextureFormat.RGBA32, false);
            texture.name = name + "_texture";
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.hideFlags = HideFlags.HideAndDontSave;
            return texture;
        }

        private static void DrawRoundedFill(Texture2D texture, Color fill, Color borderColor, int cornerRadius, int border)
        {
            Vector2 center = new Vector2((SpriteSize - 1) * 0.5f, (SpriteSize - 1) * 0.5f);
            for (int y = 0; y < SpriteSize; y++)
            {
                for (int x = 0; x < SpriteSize; x++)
                {
                    bool inside = IsInsideRoundedRect(x, y, SpriteSize, SpriteSize, cornerRadius);
                    if (!inside)
                    {
                        continue;
                    }

                    bool edge = x < border || y < border || x >= SpriteSize - border || y >= SpriteSize - border;
                    float highlight = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(x, y), center) / 46f);
                    texture.SetPixel(x, y, edge ? borderColor : Color.Lerp(fill, borderColor, highlight * 0.16f));
                }
            }
        }

        private static Sprite FinalizeSprite(Texture2D texture)
        {
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, SpriteSize, SpriteSize), new Vector2(0.5f, 0.5f), SpriteSize);
            sprite.name = texture.name.Replace("_texture", "_sprite");
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }

        private static void FillClear(Texture2D texture)
        {
            for (int y = 0; y < SpriteSize; y++)
            {
                for (int x = 0; x < SpriteSize; x++)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        private static bool IsInsideRoundedRect(int x, int y, int width, int height, int radius)
        {
            int left = radius;
            int right = width - 1 - radius;
            int bottom = radius;
            int top = height - 1 - radius;

            int nearestX = Mathf.Clamp(x, left, right);
            int nearestY = Mathf.Clamp(y, bottom, top);
            int dx = x - nearestX;
            int dy = y - nearestY;
            return dx * dx + dy * dy <= radius * radius;
        }

        private static void DrawEllipse(Texture2D texture, int centerX, int centerY, int radiusX, int radiusY, Color color)
        {
            for (int y = centerY - radiusY; y <= centerY + radiusY; y++)
            {
                for (int x = centerX - radiusX; x <= centerX + radiusX; x++)
                {
                    if (x < 0 || y < 0 || x >= SpriteSize || y >= SpriteSize)
                    {
                        continue;
                    }

                    float nx = (x - centerX) / (float)radiusX;
                    float ny = (y - centerY) / (float)radiusY;
                    if (nx * nx + ny * ny <= 1f)
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
            }
        }

        private static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, int width, Color color)
        {
            Vector2 start = new Vector2(x0, y0);
            Vector2 end = new Vector2(x1, y1);
            Vector2 segment = end - start;
            float segmentLength = segment.magnitude;
            if (segmentLength <= 0.001f)
            {
                return;
            }

            for (int y = 0; y < SpriteSize; y++)
            {
                for (int x = 0; x < SpriteSize; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    float t = Mathf.Clamp01(Vector2.Dot(point - start, segment) / (segmentLength * segmentLength));
                    Vector2 closest = start + segment * t;
                    if (Vector2.Distance(point, closest) <= width * 0.5f)
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
            }
        }

        private static void DrawPetal(Texture2D texture, int centerX, int centerY, float angleDegrees, Color color)
        {
            float radians = angleDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);

            for (int y = 0; y < SpriteSize; y++)
            {
                for (int x = 0; x < SpriteSize; x++)
                {
                    float dx = x - centerX;
                    float dy = y - centerY;
                    float rx = dx * cos + dy * sin;
                    float ry = -dx * sin + dy * cos;
                    if ((rx * rx) / 48f + (ry * ry) / 160f <= 1f && ry >= -9f)
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
            }
        }
    }
}
