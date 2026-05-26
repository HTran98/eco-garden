using UnityEngine;

namespace EcoGarden.UI
{
    public static class LevelSelectUiLayoutMetrics
    {
        public const float LevelRowHeight = 126f;
        public const float MinimumTitleWidth = 320f;
        public const float MinimumActionWidth = 160f;

        public static readonly Vector2 StatusAnchorMin = new Vector2(0.035f, 0.58f);
        public static readonly Vector2 StatusAnchorMax = new Vector2(0.18f, 0.88f);
        public static readonly Vector2 TitleAnchorMin = new Vector2(0.205f, 0.58f);
        public static readonly Vector2 TitleAnchorMax = new Vector2(0.66f, 0.94f);
        public static readonly Vector2 MetaAnchorMin = new Vector2(0.205f, 0.31f);
        public static readonly Vector2 MetaAnchorMax = new Vector2(0.66f, 0.58f);
        public static readonly Vector2 SummaryAnchorMin = new Vector2(0.035f, 0.08f);
        public static readonly Vector2 SummaryAnchorMax = new Vector2(0.66f, 0.31f);
        public static readonly Vector2 ActionAnchorMin = new Vector2(0.70f, 0.20f);
        public static readonly Vector2 ActionAnchorMax = new Vector2(0.96f, 0.80f);

        public static float Width(Vector2 anchorMin, Vector2 anchorMax, float parentWidth)
        {
            return Mathf.Max(0f, anchorMax.x - anchorMin.x) * parentWidth;
        }
    }
}
