using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public static class UiThemePalette
    {
        public static readonly Color Panel = new Color(0.94f, 0.98f, 0.91f, 0.96f);
        public static readonly Color PanelStrong = new Color(0.84f, 0.94f, 0.78f, 0.97f);
        public static readonly Color PanelMuted = new Color(0.78f, 0.90f, 0.84f, 0.82f);
        public static readonly Color PanelOverlay = new Color(0.15f, 0.26f, 0.22f, 0.88f);
        public static readonly Color TopBar = new Color(0.17f, 0.38f, 0.31f, 0.95f);
        public static readonly Color PrimaryButton = new Color(0.28f, 0.62f, 0.42f, 0.98f);
        public static readonly Color SecondaryButton = new Color(0.96f, 0.72f, 0.30f, 0.96f);
        public static readonly Color DisabledButton = new Color(0.53f, 0.61f, 0.57f, 0.72f);
        public static readonly Color Selected = new Color(0.55f, 0.82f, 0.48f, 1f);
        public static readonly Color TextDark = new Color(0.13f, 0.20f, 0.17f, 1f);
        public static readonly Color TextLight = new Color(0.98f, 1f, 0.95f, 1f);
        public static readonly Color TextMuted = new Color(0.40f, 0.50f, 0.45f, 1f);
        public static readonly Color Gold = new Color(0.96f, 0.72f, 0.23f, 1f);
        public static readonly Color Gem = new Color(0.72f, 0.55f, 0.96f, 1f);
        public static readonly Color Store = new Color(0.42f, 0.66f, 0.92f, 1f);
        public static readonly Color Success = new Color(0.45f, 0.78f, 0.35f, 1f);
        public static readonly Color Warning = new Color(0.96f, 0.64f, 0.24f, 1f);
        public static readonly Color Error = new Color(0.88f, 0.34f, 0.30f, 1f);
        public static readonly Color Delivery = new Color(0.86f, 0.55f, 0.88f, 1f);
        public static readonly Color Sell = new Color(0.78f, 0.88f, 0.42f, 1f);

        public static Color GetTextForBackground(Color background)
        {
            return RelativeLuminance(background) > 0.55f ? TextDark : TextLight;
        }

        public static ColorBlock BuildButtonColors(Color normal)
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = normal;
            colors.highlightedColor = Color.Lerp(normal, Color.white, 0.18f);
            colors.pressedColor = Color.Lerp(normal, Color.black, 0.12f);
            colors.selectedColor = Color.Lerp(normal, Selected, 0.18f);
            colors.disabledColor = DisabledButton;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            return colors;
        }

        public static float ContrastRatio(Color a, Color b)
        {
            float lighter = Mathf.Max(RelativeLuminance(a), RelativeLuminance(b));
            float darker = Mathf.Min(RelativeLuminance(a), RelativeLuminance(b));
            return (lighter + 0.05f) / (darker + 0.05f);
        }

        private static float RelativeLuminance(Color color)
        {
            return 0.2126f * Linearize(color.r) + 0.7152f * Linearize(color.g) + 0.0722f * Linearize(color.b);
        }

        private static float Linearize(float channel)
        {
            return channel <= 0.03928f
                ? channel / 12.92f
                : Mathf.Pow((channel + 0.055f) / 1.055f, 2.4f);
        }
    }
}
