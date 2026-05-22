using UnityEngine;

namespace EcoGarden.UI
{
    public static class AndroidHudLayoutMetrics
    {
        public static readonly Vector2 DeliveryAnchorMin = new Vector2(0.04f, 0.19f);
        public static readonly Vector2 DeliveryAnchorMax = new Vector2(0.29f, 0.325f);
        public static readonly Vector2 SellAnchorMin = new Vector2(0.71f, 0.19f);
        public static readonly Vector2 SellAnchorMax = new Vector2(0.96f, 0.325f);
        public static readonly Vector2 FeedbackAnchorMin = new Vector2(0.04f, 0.43f);
        public static readonly Vector2 FeedbackAnchorMax = new Vector2(0.96f, 0.53f);
        public static readonly Vector2 ResultAnchorMin = new Vector2(0.10f, 0.35f);
        public static readonly Vector2 ResultAnchorMax = new Vector2(0.90f, 0.65f);
        public static readonly Vector2 PanelAnchorMin = new Vector2(0.06f, 0.18f);
        public static readonly Vector2 PanelAnchorMax = new Vector2(0.94f, 0.84f);
        public static readonly Vector2 MissionTrackerAnchorMin = new Vector2(0.64f, 0.36f);
        public static readonly Vector2 MissionTrackerAnchorMax = new Vector2(0.96f, 0.70f);

        public const float TopBarHeight = 92f;
        public const float ObjectiveTopOffset = 172f;
        public const float ObjectiveHeight = 78f;
        public const float AbilityBottomOffset = 34f;
        public const float AbilityHeight = 132f;
        public const int MaxCompactMissionRows = 2;
        public const float MinimumCompactMissionTrackerWidth = 220f;
        public const float MinimumCompactMissionActionWidth = 58f;
        public const float MinimumExternalDropZoneWidth = 170f;
        public const float MinimumExternalDropZoneHeight = 150f;
        public const float MinimumExternalDropZoneGap = 260f;

        public static Rect ToPixelRect(Vector2 anchorMin, Vector2 anchorMax, Vector2 screenSize)
        {
            return Rect.MinMaxRect(
                anchorMin.x * screenSize.x,
                anchorMin.y * screenSize.y,
                anchorMax.x * screenSize.x,
                anchorMax.y * screenSize.y);
        }

        public static Rect BottomBarRect(Vector2 screenSize, float bottomOffset, float height)
        {
            return Rect.MinMaxRect(0f, bottomOffset, screenSize.x, bottomOffset + height);
        }

        public static bool Overlaps(Rect first, Rect second, float padding = 0f)
        {
            Rect expanded = new Rect(
                first.xMin - padding,
                first.yMin - padding,
                first.width + padding * 2f,
                first.height + padding * 2f);
            return expanded.Overlaps(second);
        }
    }
}
