using UnityEngine;

namespace EcoGarden.UI
{
    public static class InventoryUiLayoutMetrics
    {
        public static readonly Vector2 SummaryAnchorMin = new Vector2(0.06f, 0.80f);
        public static readonly Vector2 SummaryAnchorMax = new Vector2(0.94f, 0.88f);
        public static readonly Vector2 ContentAnchorMin = new Vector2(0.06f, 0.08f);
        public static readonly Vector2 ContentAnchorMax = new Vector2(0.94f, 0.78f);

        public static readonly Vector2 IconAnchorMin = new Vector2(0.03f, 0.18f);
        public static readonly Vector2 IconAnchorMax = new Vector2(0.17f, 0.82f);
        public static readonly Vector2 NameAnchorMin = new Vector2(0.20f, 0.50f);
        public static readonly Vector2 NameAnchorMax = new Vector2(0.66f, 0.86f);
        public static readonly Vector2 DetailAnchorMin = new Vector2(0.20f, 0.12f);
        public static readonly Vector2 DetailAnchorMax = new Vector2(0.66f, 0.48f);
        public static readonly Vector2 CountAnchorMin = new Vector2(0.68f, 0.24f);
        public static readonly Vector2 CountAnchorMax = new Vector2(0.80f, 0.76f);
        public static readonly Vector2 ActionAnchorMin = new Vector2(0.82f, 0.18f);
        public static readonly Vector2 ActionAnchorMax = new Vector2(0.98f, 0.82f);

        public const float ItemRowHeight = 112f;
        public const float MinimumSummaryHeight = 44f;
        public const float MinimumContentHeight = 560f;
        public const float MinimumActionWidth = 92f;
    }
}
