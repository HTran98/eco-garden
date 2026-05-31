using UnityEngine;

namespace EcoGarden.UI
{
    public static class AndroidHudLayoutMetrics
    {
        public static readonly Vector2 DeliveryAnchorMin = new Vector2(0.06f, 0.125f);
        public static readonly Vector2 DeliveryAnchorMax = new Vector2(0.24f, 0.235f);
        public static readonly Vector2 SellAnchorMin = new Vector2(0.76f, 0.125f);
        public static readonly Vector2 SellAnchorMax = new Vector2(0.94f, 0.235f);
        public static readonly Vector2 FeedbackAnchorMin = new Vector2(0.04f, 0.43f);
        public static readonly Vector2 FeedbackAnchorMax = new Vector2(0.96f, 0.53f);
        public static readonly Vector2 ResultAnchorMin = new Vector2(0.10f, 0.35f);
        public static readonly Vector2 ResultAnchorMax = new Vector2(0.90f, 0.65f);
        public static readonly Vector2 PanelAnchorMin = new Vector2(0.06f, 0.18f);
        public static readonly Vector2 PanelAnchorMax = new Vector2(0.94f, 0.84f);
        public static readonly Vector2 MissionTrackerAnchorMin = new Vector2(0.64f, 0.36f);
        public static readonly Vector2 MissionTrackerAnchorMax = new Vector2(0.96f, 0.70f);
        public static readonly Vector2 TimerAnchorMin = new Vector2(0.02f, 0.12f);
        public static readonly Vector2 TimerAnchorMax = new Vector2(0.17f, 0.88f);
        public static readonly Vector2 GoldAnchorMin = new Vector2(0.18f, 0.12f);
        public static readonly Vector2 GoldAnchorMax = new Vector2(0.33f, 0.88f);
        public static readonly Vector2 GemAnchorMin = new Vector2(0.34f, 0.12f);
        public static readonly Vector2 GemAnchorMax = new Vector2(0.49f, 0.88f);
        public static readonly Vector2 LevelButtonAnchorMin = new Vector2(0.50f, 0.14f);
        public static readonly Vector2 LevelButtonAnchorMax = new Vector2(0.59f, 0.86f);
        public static readonly Vector2 MissionButtonAnchorMin = new Vector2(0.60f, 0.14f);
        public static readonly Vector2 MissionButtonAnchorMax = new Vector2(0.69f, 0.86f);
        public static readonly Vector2 ShopButtonAnchorMin = new Vector2(0.70f, 0.14f);
        public static readonly Vector2 ShopButtonAnchorMax = new Vector2(0.79f, 0.86f);
        public static readonly Vector2 BagButtonAnchorMin = new Vector2(0.80f, 0.14f);
        public static readonly Vector2 BagButtonAnchorMax = new Vector2(0.89f, 0.86f);
        public static readonly Vector2 PauseButtonAnchorMin = new Vector2(0.90f, 0.14f);
        public static readonly Vector2 PauseButtonAnchorMax = new Vector2(0.99f, 0.86f);
        public static readonly Vector2 ShovelButtonAnchorMin = new Vector2(0.03f, 0.14f);
        public static readonly Vector2 ShovelButtonAnchorMax = new Vector2(0.32f, 0.86f);
        public static readonly Vector2 MagicWandButtonAnchorMin = new Vector2(0.355f, 0.14f);
        public static readonly Vector2 MagicWandButtonAnchorMax = new Vector2(0.645f, 0.86f);
        public static readonly Vector2 SortingMagnetButtonAnchorMin = new Vector2(0.68f, 0.14f);
        public static readonly Vector2 SortingMagnetButtonAnchorMax = new Vector2(0.97f, 0.86f);

        public const float TopBarHeight = 92f;
        public const float ObjectiveTopOffset = 172f;
        public const float ObjectiveHeight = 78f;
        public const float AbilityBottomOffset = 34f;
        public const float AbilityHeight = 132f;
        public const int MaxCompactMissionRows = 2;
        public const float MinimumCompactMissionTrackerWidth = 220f;
        public const float MinimumCompactMissionActionWidth = 58f;
        public const float MinimumExternalDropZoneWidth = 128f;
        public const float MinimumExternalDropZoneHeight = 118f;
        public const float MinimumExternalDropZoneGap = 360f;
        public const float MinimumTopBarActionWidth = 64f;
        public const float MinimumTopBarStatWidth = 100f;
        public const float MinimumAbilityButtonWidth = 160f;
        public const float MinimumAbilityButtonHeight = 92f;

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
