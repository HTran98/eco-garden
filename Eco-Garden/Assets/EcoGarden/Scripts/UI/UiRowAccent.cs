using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public static class UiRowAccent
    {
        private const string AccentName = "RuntimeRowAccent";

        public static void Apply(Transform rowTransform, Color color)
        {
            if (rowTransform == null)
            {
                return;
            }

            Transform existing = rowTransform.Find(AccentName);
            GameObject accentObject = existing != null
                ? existing.gameObject
                : new GameObject(AccentName, typeof(RectTransform), typeof(Image));
            accentObject.transform.SetParent(rowTransform, false);
            accentObject.transform.SetAsFirstSibling();

            RectTransform accentRect = accentObject.GetComponent<RectTransform>();
            accentRect.anchorMin = new Vector2(0f, 0.12f);
            accentRect.anchorMax = new Vector2(0f, 0.88f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.offsetMin = new Vector2(8f, 0f);
            accentRect.offsetMax = new Vector2(14f, 0f);

            Color accentColor = color;
            accentColor.a = Mathf.Max(accentColor.a, 0.88f);

            Image accentImage = accentObject.GetComponent<Image>();
            accentImage.sprite = null;
            accentImage.color = accentColor;
            accentImage.raycastTarget = false;
        }
    }
}
