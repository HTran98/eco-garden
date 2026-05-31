using UnityEngine;

namespace EcoGarden.UI
{
    public static class LevelSelectUiLayoutMetrics
    {
        public const float LevelRowHeight = 126f;
        public const float MinimumTitleWidth = 320f;
        public const float MinimumActionWidth = 160f;
        public const float MinimumLevelSummaryHeight = 48f;

        public static readonly Vector2 PanelSummaryAnchorMin = new Vector2(0.05f, 0.795f);
        public static readonly Vector2 PanelSummaryAnchorMax = new Vector2(0.95f, 0.875f);
        public static readonly Vector2 PanelContentAnchorMin = new Vector2(0.04f, 0.06f);
        public static readonly Vector2 PanelContentAnchorMax = new Vector2(0.96f, 0.775f);
        public static readonly Vector2 PreviewAnchorMin = new Vector2(0.06f, 0.06f);
        public static readonly Vector2 PreviewAnchorMax = new Vector2(0.94f, 0.42f);

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
        public static readonly Vector2 PreviewTitleAnchorMin = new Vector2(0.05f, 0.74f);
        public static readonly Vector2 PreviewTitleAnchorMax = new Vector2(0.72f, 0.96f);
        public static readonly Vector2 PreviewMetaAnchorMin = new Vector2(0.05f, 0.54f);
        public static readonly Vector2 PreviewMetaAnchorMax = new Vector2(0.72f, 0.73f);
        public static readonly Vector2 PreviewObjectiveAnchorMin = new Vector2(0.05f, 0.31f);
        public static readonly Vector2 PreviewObjectiveAnchorMax = new Vector2(0.72f, 0.53f);
        public static readonly Vector2 PreviewRewardAnchorMin = new Vector2(0.05f, 0.08f);
        public static readonly Vector2 PreviewRewardAnchorMax = new Vector2(0.72f, 0.30f);
        public static readonly Vector2 PreviewPlayAnchorMin = new Vector2(0.76f, 0.55f);
        public static readonly Vector2 PreviewPlayAnchorMax = new Vector2(0.96f, 0.88f);
        public static readonly Vector2 PreviewCloseAnchorMin = new Vector2(0.76f, 0.14f);
        public static readonly Vector2 PreviewCloseAnchorMax = new Vector2(0.96f, 0.47f);

        public static float Width(Vector2 anchorMin, Vector2 anchorMax, float parentWidth)
        {
            return Mathf.Max(0f, anchorMax.x - anchorMin.x) * parentWidth;
        }
    }
}
