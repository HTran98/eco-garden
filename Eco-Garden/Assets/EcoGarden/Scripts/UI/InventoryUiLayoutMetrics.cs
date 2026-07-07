using UnityEngine;

namespace EcoGarden.UI
{
    public static class InventoryUiLayoutMetrics
    {
        public static readonly Vector2 SummaryAnchorMin = new Vector2(0.06f, 0.80f);
        public static readonly Vector2 SummaryAnchorMax = new Vector2(0.94f, 0.88f);
        public static readonly Vector2 ContentAnchorMin = new Vector2(0.06f, 0.08f);
        public static readonly Vector2 ContentAnchorMax = new Vector2(0.94f, 0.78f);

        public static readonly Vector2 IconAnchorMin = new Vector2(0.025f, 0.10f);
        public static readonly Vector2 IconAnchorMax = new Vector2(0.28f, 0.90f);
        public static readonly Vector2 NameAnchorMin = new Vector2(0.32f, 0.55f);
        public static readonly Vector2 NameAnchorMax = new Vector2(0.68f, 0.88f);
        public static readonly Vector2 DetailAnchorMin = new Vector2(0.32f, 0.14f);
        public static readonly Vector2 DetailAnchorMax = new Vector2(0.68f, 0.52f);
        public static readonly Vector2 CountAnchorMin = new Vector2(0.58f, 0.24f);
        public static readonly Vector2 CountAnchorMax = new Vector2(0.70f, 0.76f);
        public static readonly Vector2 ActionAnchorMin = new Vector2(0.72f, 0.20f);
        public static readonly Vector2 ActionAnchorMax = new Vector2(0.98f, 0.80f);

        public const float ItemRowHeight = 164f;
        public const float MinimumSummaryHeight = 44f;
        public const float MinimumContentHeight = 560f;
        public const float MinimumActionWidth = 140f;
        public const float MinimumIconPreviewWidth = 148f;
    }
}
