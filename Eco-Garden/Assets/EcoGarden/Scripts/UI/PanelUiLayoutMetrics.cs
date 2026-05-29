using UnityEngine;

namespace EcoGarden.UI
{
    public static class PanelUiLayoutMetrics
    {
        public static readonly Vector2 TitleAnchorMin = new Vector2(0.05f, 0.895f);
        public static readonly Vector2 TitleAnchorMax = new Vector2(0.72f, 0.975f);
        public static readonly Vector2 CloseAnchorMin = new Vector2(0.86f, 0.895f);
        public static readonly Vector2 CloseAnchorMax = new Vector2(0.96f, 0.975f);
        public static readonly Vector2 FullContentAnchorMin = new Vector2(0.04f, 0.06f);
        public static readonly Vector2 FullContentAnchorMax = new Vector2(0.96f, 0.875f);
        public static readonly Vector2 ShopCategoryAnchorMin = new Vector2(0.04f, 0.785f);
        public static readonly Vector2 ShopCategoryAnchorMax = new Vector2(0.96f, 0.875f);
        public static readonly Vector2 ShopContentAnchorMin = new Vector2(0.04f, 0.06f);
        public static readonly Vector2 ShopContentAnchorMax = new Vector2(0.96f, 0.765f);
        public static readonly Vector2 ResultTitleAnchorMin = new Vector2(0.07f, 0.66f);
        public static readonly Vector2 ResultTitleAnchorMax = new Vector2(0.93f, 0.91f);
        public static readonly Vector2 ResultMessageAnchorMin = new Vector2(0.08f, 0.40f);
        public static readonly Vector2 ResultMessageAnchorMax = new Vector2(0.92f, 0.64f);
        public static readonly Vector2 ResultCountdownAnchorMin = new Vector2(0.12f, 0.30f);
        public static readonly Vector2 ResultCountdownAnchorMax = new Vector2(0.88f, 0.38f);
        public static readonly Vector2 ResultRestartAnchorMin = new Vector2(0.08f, 0.07f);
        public static readonly Vector2 ResultRestartAnchorMax = new Vector2(0.46f, 0.28f);
        public static readonly Vector2 ResultNextAnchorMin = new Vector2(0.54f, 0.07f);
        public static readonly Vector2 ResultNextAnchorMax = new Vector2(0.92f, 0.28f);

        public const float MinimumPanelTitleWidth = 320f;
        public const float MinimumPanelCloseWidth = 58f;
        public const float MinimumPanelCloseHeight = 58f;
        public const float MinimumResultActionWidth = 190f;
        public const float MinimumResultActionHeight = 70f;
        public const float MinimumResultCountdownHeight = 24f;
    }
}
