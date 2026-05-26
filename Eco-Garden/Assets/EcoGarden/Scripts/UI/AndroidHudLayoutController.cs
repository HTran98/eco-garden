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

            SetStretchTop("TopBar", 0f, AndroidHudLayoutMetrics.TopBarHeight);
            SetStretchTop("ObjectivePanel", sidePadding, AndroidHudLayoutMetrics.ObjectiveTopOffset, -80f, AndroidHudLayoutMetrics.ObjectiveHeight);
            SetStretchBottom("AbilityBar", sidePadding, AndroidHudLayoutMetrics.AbilityBottomOffset, -80f, AndroidHudLayoutMetrics.AbilityHeight);
            SetAnchoredBox("DeliveryDropZone", AndroidHudLayoutMetrics.DeliveryAnchorMin, AndroidHudLayoutMetrics.DeliveryAnchorMax);
            SetAnchoredBox("SellBasket", AndroidHudLayoutMetrics.SellAnchorMin, AndroidHudLayoutMetrics.SellAnchorMax);
            SetAnchoredBox("FeedbackText", AndroidHudLayoutMetrics.FeedbackAnchorMin, AndroidHudLayoutMetrics.FeedbackAnchorMax);
            SetAnchoredBox("ResultPanel", AndroidHudLayoutMetrics.ResultAnchorMin, AndroidHudLayoutMetrics.ResultAnchorMax);
            SetAnchoredBox("LevelPanel", AndroidHudLayoutMetrics.PanelAnchorMin, AndroidHudLayoutMetrics.PanelAnchorMax);
            SetAnchoredBox("ShopPanel", AndroidHudLayoutMetrics.PanelAnchorMin, AndroidHudLayoutMetrics.PanelAnchorMax);
            SetAnchoredBox("MissionPanel", AndroidHudLayoutMetrics.PanelAnchorMin, AndroidHudLayoutMetrics.PanelAnchorMax);
            SetAnchoredBox("MissionTrackerPanel", AndroidHudLayoutMetrics.MissionTrackerAnchorMin, AndroidHudLayoutMetrics.MissionTrackerAnchorMax);
            ApplyTopBarChildRects();
            ApplyAbilityBarChildRects();
            ApplyPanelChildRects();
            ApplyCompactHudLabels();
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

        private static void ApplyTopBarChildRects()
        {
            SetAnchoredBox("TimerText", AndroidHudLayoutMetrics.TimerAnchorMin, AndroidHudLayoutMetrics.TimerAnchorMax);
            SetAnchoredBox("GoldText", AndroidHudLayoutMetrics.GoldAnchorMin, AndroidHudLayoutMetrics.GoldAnchorMax);
            SetAnchoredBox("GemText", AndroidHudLayoutMetrics.GemAnchorMin, AndroidHudLayoutMetrics.GemAnchorMax);
            SetAnchoredBox("LevelButton", AndroidHudLayoutMetrics.LevelButtonAnchorMin, AndroidHudLayoutMetrics.LevelButtonAnchorMax);
            SetAnchoredBox("MissionButton", AndroidHudLayoutMetrics.MissionButtonAnchorMin, AndroidHudLayoutMetrics.MissionButtonAnchorMax);
            SetAnchoredBox("ShopButton", AndroidHudLayoutMetrics.ShopButtonAnchorMin, AndroidHudLayoutMetrics.ShopButtonAnchorMax);
            SetAnchoredBox("PauseButton", AndroidHudLayoutMetrics.PauseButtonAnchorMin, AndroidHudLayoutMetrics.PauseButtonAnchorMax);
        }

        private static void ApplyAbilityBarChildRects()
        {
            SetAnchoredBox("ShovelButton", AndroidHudLayoutMetrics.ShovelButtonAnchorMin, AndroidHudLayoutMetrics.ShovelButtonAnchorMax);
            SetAnchoredBox("MagicWandButton", AndroidHudLayoutMetrics.MagicWandButtonAnchorMin, AndroidHudLayoutMetrics.MagicWandButtonAnchorMax);
            SetAnchoredBox("SortingMagnetButton", AndroidHudLayoutMetrics.SortingMagnetButtonAnchorMin, AndroidHudLayoutMetrics.SortingMagnetButtonAnchorMax);
        }

        private static void ApplyPanelChildRects()
        {
            SetAnchoredBox("LevelTitleText", PanelUiLayoutMetrics.TitleAnchorMin, PanelUiLayoutMetrics.TitleAnchorMax);
            SetAnchoredBox("ShopTitleText", PanelUiLayoutMetrics.TitleAnchorMin, PanelUiLayoutMetrics.TitleAnchorMax);
            SetAnchoredBox("MissionTitleText", PanelUiLayoutMetrics.TitleAnchorMin, PanelUiLayoutMetrics.TitleAnchorMax);
            SetAnchoredBox("LevelCloseButton", PanelUiLayoutMetrics.CloseAnchorMin, PanelUiLayoutMetrics.CloseAnchorMax);
            SetAnchoredBox("ShopCloseButton", PanelUiLayoutMetrics.CloseAnchorMin, PanelUiLayoutMetrics.CloseAnchorMax);
            SetAnchoredBox("MissionCloseButton", PanelUiLayoutMetrics.CloseAnchorMin, PanelUiLayoutMetrics.CloseAnchorMax);
            SetAnchoredBox("LevelViewport", PanelUiLayoutMetrics.FullContentAnchorMin, PanelUiLayoutMetrics.FullContentAnchorMax);
            SetAnchoredBox("MissionViewport", PanelUiLayoutMetrics.FullContentAnchorMin, PanelUiLayoutMetrics.FullContentAnchorMax);
            SetAnchoredBox("ShopCategoryBar", PanelUiLayoutMetrics.ShopCategoryAnchorMin, PanelUiLayoutMetrics.ShopCategoryAnchorMax);
            SetAnchoredBox("ShopProductViewport", PanelUiLayoutMetrics.ShopContentAnchorMin, PanelUiLayoutMetrics.ShopContentAnchorMax);
            SetAnchoredBox("ResultTitleText", PanelUiLayoutMetrics.ResultTitleAnchorMin, PanelUiLayoutMetrics.ResultTitleAnchorMax);
            SetAnchoredBox("ResultMessageText", PanelUiLayoutMetrics.ResultMessageAnchorMin, PanelUiLayoutMetrics.ResultMessageAnchorMax);
            SetAnchoredBox("RestartButton", PanelUiLayoutMetrics.ResultRestartAnchorMin, PanelUiLayoutMetrics.ResultRestartAnchorMax);
            SetAnchoredBox("NextLevelButton", PanelUiLayoutMetrics.ResultNextAnchorMin, PanelUiLayoutMetrics.ResultNextAnchorMax);
        }

        private static void ApplyCompactHudLabels()
        {
            SetButtonLabel("LevelButton", "Lvl");
            SetButtonLabel("MissionButton", "Tasks");
            SetButtonLabel("ShopButton", "Shop");
            SetButtonLabel("PauseButton", "II");
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
            GameObject gameObject = FindObjectIncludingInactive(objectName);
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

        private static void SetButtonLabel(string objectName, string label)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            if (gameObject == null)
            {
                return;
            }

            Text text = gameObject.GetComponentInChildren<Text>(true);
            if (text != null)
            {
                text.text = label;
            }
        }

        private static GameObject FindObjectIncludingInactive(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                return null;
            }

            GameObject activeObject = GameObject.Find(objectName);
            if (activeObject != null)
            {
                return activeObject;
            }

            Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform candidate = transforms[i];
                if (candidate != null &&
                    candidate.gameObject.scene.IsValid() &&
                    candidate.name == objectName)
                {
                    return candidate.gameObject;
                }
            }

            return null;
        }
    }
}
