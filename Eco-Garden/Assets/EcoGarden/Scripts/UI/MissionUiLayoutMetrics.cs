using UnityEngine;

namespace EcoGarden.UI
{
    public static class MissionUiLayoutMetrics
    {
        public const float MissionRowHeight = 126f;
        public const float TrackerRowHeight = 92f;
        public const float MinimumMissionTextWidth = 360f;
        public const float MinimumMissionClaimWidth = 160f;
        public const float MinimumTrackerTextWidth = 128f;
        public const float MinimumTrackerClaimWidth = 54f;
        public const float MinimumMissionSummaryHeight = 48f;

        public static readonly Vector2 SummaryAnchorMin = new Vector2(0.05f, 0.795f);
        public static readonly Vector2 SummaryAnchorMax = new Vector2(0.95f, 0.875f);
        public static readonly Vector2 ContentAnchorMin = new Vector2(0.04f, 0.06f);
        public static readonly Vector2 ContentAnchorMax = new Vector2(0.96f, 0.775f);

        public static readonly Vector2 RowTitleAnchorMin = new Vector2(0.035f, 0.60f);
        public static readonly Vector2 RowTitleAnchorMax = new Vector2(0.66f, 0.94f);
        public static readonly Vector2 RowProgressAnchorMin = new Vector2(0.035f, 0.33f);
        public static readonly Vector2 RowProgressAnchorMax = new Vector2(0.66f, 0.60f);
        public static readonly Vector2 RowRewardAnchorMin = new Vector2(0.035f, 0.08f);
        public static readonly Vector2 RowRewardAnchorMax = new Vector2(0.66f, 0.33f);
        public static readonly Vector2 RowClaimAnchorMin = new Vector2(0.68f, 0.20f);
        public static readonly Vector2 RowClaimAnchorMax = new Vector2(0.96f, 0.80f);

        public static readonly Vector2 TrackerTitleAnchorMin = new Vector2(0.04f, 0.50f);
        public static readonly Vector2 TrackerTitleAnchorMax = new Vector2(0.66f, 0.96f);
        public static readonly Vector2 TrackerProgressAnchorMin = new Vector2(0.04f, 0.08f);
        public static readonly Vector2 TrackerProgressAnchorMax = new Vector2(0.66f, 0.50f);
        public static readonly Vector2 TrackerClaimAnchorMin = new Vector2(0.70f, 0.20f);
        public static readonly Vector2 TrackerClaimAnchorMax = new Vector2(0.96f, 0.80f);

        public static float Width(Vector2 anchorMin, Vector2 anchorMax, float parentWidth)
        {
            return Mathf.Max(0f, anchorMax.x - anchorMin.x) * parentWidth;
        }
    }
}
