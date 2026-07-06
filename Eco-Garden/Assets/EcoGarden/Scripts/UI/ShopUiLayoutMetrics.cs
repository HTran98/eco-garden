using UnityEngine;

namespace EcoGarden.UI
{
    public static class ShopUiLayoutMetrics
    {
        public const float ProductRowHeight = 264f;
        public const float MinimumCategoryTabWidth = 96f;
        public const float MinimumPriceBadgeWidth = 128f;
        public const float MinimumBuyButtonWidth = 128f;
        public const float MinimumProductTextWidth = 220f;
        public const float MinimumShopSummaryHeight = 44f;

        public static readonly Vector2 SummaryAnchorMin = new Vector2(0.05f, 0.705f);
        public static readonly Vector2 SummaryAnchorMax = new Vector2(0.95f, 0.765f);
        public static readonly Vector2 ContentAnchorMin = new Vector2(0.04f, 0.06f);
        public static readonly Vector2 ContentAnchorMax = new Vector2(0.96f, 0.685f);

        public static readonly Vector2 TypeBadgeAnchorMin = new Vector2(0.02f, 0.13f);
        public static readonly Vector2 TypeBadgeAnchorMax = new Vector2(0.31f, 0.90f);
        public static readonly Vector2 NameAnchorMin = new Vector2(0.34f, 0.71f);
        public static readonly Vector2 NameAnchorMax = new Vector2(0.72f, 0.93f);
        public static readonly Vector2 DescriptionAnchorMin = new Vector2(0.34f, 0.36f);
        public static readonly Vector2 DescriptionAnchorMax = new Vector2(0.72f, 0.69f);
        public static readonly Vector2 EffectAnchorMin = new Vector2(0.34f, 0.11f);
        public static readonly Vector2 EffectAnchorMax = new Vector2(0.72f, 0.34f);
        public static readonly Vector2 StatusAnchorMin = new Vector2(0.025f, 0.07f);
        public static readonly Vector2 StatusAnchorMax = new Vector2(0.31f, 0.17f);
        public static readonly Vector2 PriceAnchorMin = new Vector2(0.735f, 0.56f);
        public static readonly Vector2 PriceAnchorMax = new Vector2(0.97f, 0.90f);
        public static readonly Vector2 BuyAnchorMin = new Vector2(0.735f, 0.13f);
        public static readonly Vector2 BuyAnchorMax = new Vector2(0.97f, 0.49f);
        public static readonly Vector2 StoreBuyAnchorMin = new Vector2(0.735f, 0.22f);
        public static readonly Vector2 StoreBuyAnchorMax = new Vector2(0.97f, 0.78f);

        public static float Width(Vector2 anchorMin, Vector2 anchorMax, float parentWidth)
        {
            return Mathf.Max(0f, anchorMax.x - anchorMin.x) * parentWidth;
        }
    }
}
