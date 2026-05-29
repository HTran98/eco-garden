using UnityEngine;

namespace EcoGarden.UI
{
    public static class ShopUiLayoutMetrics
    {
        public const float ProductRowHeight = 124f;
        public const float MinimumCategoryTabWidth = 96f;
        public const float MinimumPriceBadgeWidth = 132f;
        public const float MinimumBuyButtonWidth = 132f;
        public const float MinimumProductTextWidth = 250f;
        public const float MinimumShopSummaryHeight = 44f;

        public static readonly Vector2 SummaryAnchorMin = new Vector2(0.05f, 0.705f);
        public static readonly Vector2 SummaryAnchorMax = new Vector2(0.95f, 0.765f);
        public static readonly Vector2 ContentAnchorMin = new Vector2(0.04f, 0.06f);
        public static readonly Vector2 ContentAnchorMax = new Vector2(0.96f, 0.685f);

        public static readonly Vector2 TypeBadgeAnchorMin = new Vector2(0.025f, 0.18f);
        public static readonly Vector2 TypeBadgeAnchorMax = new Vector2(0.17f, 0.82f);
        public static readonly Vector2 NameAnchorMin = new Vector2(0.20f, 0.56f);
        public static readonly Vector2 NameAnchorMax = new Vector2(0.63f, 0.91f);
        public static readonly Vector2 DescriptionAnchorMin = new Vector2(0.20f, 0.18f);
        public static readonly Vector2 DescriptionAnchorMax = new Vector2(0.63f, 0.56f);
        public static readonly Vector2 PriceAnchorMin = new Vector2(0.66f, 0.56f);
        public static readonly Vector2 PriceAnchorMax = new Vector2(0.96f, 0.90f);
        public static readonly Vector2 BuyAnchorMin = new Vector2(0.66f, 0.13f);
        public static readonly Vector2 BuyAnchorMax = new Vector2(0.96f, 0.49f);

        public static float Width(Vector2 anchorMin, Vector2 anchorMax, float parentWidth)
        {
            return Mathf.Max(0f, anchorMax.x - anchorMin.x) * parentWidth;
        }
    }
}
