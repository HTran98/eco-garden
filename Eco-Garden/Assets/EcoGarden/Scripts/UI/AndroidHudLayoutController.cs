using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class AndroidHudLayoutController : MonoBehaviour
    {
        [SerializeField] private Vector2 referenceResolution = new Vector2(1080f, 1920f);
        [SerializeField] private float minimumSidePadding = 32f;

        private RectTransform rectTransform;
        private Rect lastSafeArea;
        private Vector2Int lastScreenSize;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            ApplyLayout();
        }

        private void Update()
        {
            if (Screen.safeArea != lastSafeArea ||
                Screen.width != lastScreenSize.x ||
                Screen.height != lastScreenSize.y)
            {
                ApplyLayout();
            }
        }

        public void ApplyLayout()
        {
            ApplySafeArea();
            ApplyHudRects();
            ApplyTextRules();
        }

        private void ApplySafeArea()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            Rect safeArea = Screen.safeArea;
            if (safeArea.width <= 0f || safeArea.height <= 0f)
            {
                safeArea = new Rect(0f, 0f, Screen.width, Screen.height);
            }

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            lastSafeArea = safeArea;
            lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        }

        private void ApplyHudRects()
        {
            float sidePadding = Mathf.Max(minimumSidePadding, referenceResolution.x * 0.035f);

            SetStretchTop("TopBar", 0f, 92f);
            SetStretchTop("ObjectivePanel", sidePadding, 172f, -80f, 78f);
            SetStretchBottom("AbilityBar", sidePadding, 112f, -80f, 142f);
            SetAnchoredBox("DeliveryDropZone", new Vector2(0.04f, 0.165f), new Vector2(0.29f, 0.305f));
            SetAnchoredBox("SellBasket", new Vector2(0.71f, 0.165f), new Vector2(0.96f, 0.305f));
            SetAnchoredBox("FeedbackText", new Vector2(0.04f, 0.43f), new Vector2(0.96f, 0.53f));
            SetAnchoredBox("ResultPanel", new Vector2(0.10f, 0.35f), new Vector2(0.90f, 0.65f));
            SetAnchoredBox("ShopPanel", new Vector2(0.06f, 0.19f), new Vector2(0.94f, 0.82f));
            SetAnchoredBox("MissionPanel", new Vector2(0.06f, 0.19f), new Vector2(0.94f, 0.82f));
            SetAnchoredBox("MissionTrackerPanel", new Vector2(0.70f, 0.31f), new Vector2(0.96f, 0.72f));
        }

        private void ApplyTextRules()
        {
            Text[] texts = GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                Text text = texts[i];
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = Mathf.Min(18, text.fontSize);
                text.resizeTextMaxSize = text.fontSize;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
            }
        }

        private static void SetStretchTop(string objectName, float horizontalPadding, float height)
        {
            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject == null)
            {
                return;
            }

            RectTransform rect = gameObject.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(-horizontalPadding * 2f, height);
        }

        private static void SetStretchTop(string objectName, float horizontalPadding, float topOffset, float widthDelta, float height)
        {
            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject == null)
            {
                return;
            }

            RectTransform rect = gameObject.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -topOffset);
            rect.sizeDelta = new Vector2(widthDelta - horizontalPadding * 2f, height);
        }

        private static void SetStretchBottom(string objectName, float horizontalPadding, float bottomOffset, float widthDelta, float height)
        {
            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject == null)
            {
                return;
            }

            RectTransform rect = gameObject.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, bottomOffset);
            rect.sizeDelta = new Vector2(widthDelta - horizontalPadding * 2f, height);
        }

        private static void SetAnchoredBox(string objectName, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject == null)
            {
                return;
            }

            RectTransform rect = gameObject.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }
    }
}
