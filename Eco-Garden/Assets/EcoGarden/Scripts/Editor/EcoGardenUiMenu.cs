using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using EcoGarden.UI;
using EcoGarden.Input;
using EcoGarden.Utilities;

namespace EcoGarden.Editor
{
    public static class EcoGardenUiMenu
    {
        [MenuItem("Eco Garden/Create UI/Game HUD Skeleton")]
        public static void CreateGameHudSkeleton()
        {
            Canvas canvas = CreateCanvas();
            GameObject hudRoot = CreateRect("HUDRoot", canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            CreateTopBar(hudRoot.transform);
            CreateObjectivePanel(hudRoot.transform);
            CreateAbilityBar(hudRoot.transform);
            CreateDeliveryDropZone(hudRoot.transform);
            CreateSellBasket(hudRoot.transform);
            CreateFeedbackText(hudRoot.transform);
            CreateCoinFeedback(hudRoot.transform);
            CreateLevelPanel(hudRoot.transform);
            CreateShopPanel(hudRoot.transform);
            CreateMissionPanel(hudRoot.transform);
            CreateMissionTracker(hudRoot.transform);
            CreateResultPanel(hudRoot.transform);
            hudRoot.AddComponent<AbilityHudController>();
            hudRoot.AddComponent<LevelSelectUiController>();
            hudRoot.AddComponent<ShopUiController>();
            hudRoot.AddComponent<MissionUiController>();
            hudRoot.AddComponent<DraggedItemCanvasGhost>();
            hudRoot.AddComponent<HudSkinController>().Apply();
            hudRoot.AddComponent<AndroidHudLayoutController>().ApplyLayout();
            EnsureInputSystemEventSystem();

            Selection.activeGameObject = hudRoot;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static Canvas CreateCanvas()
        {
            GameObject canvasObject = new GameObject("EcoGardenCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;

            return canvas;
        }

        private static void CreateTopBar(Transform parent)
        {
            GameObject topBar = CreatePanel("TopBar", parent, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(0f, -92f));
            RectTransform rect = topBar.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 92f);

            CreateText("TimerText", topBar.transform, "03:00", TextAnchor.MiddleLeft, new Vector2(0f, 0f), new Vector2(0.21f, 1f));
            CreateText("GoldText", topBar.transform, "Gold 0", TextAnchor.MiddleCenter, new Vector2(0.21f, 0f), new Vector2(0.42f, 1f));
            CreateText("GemText", topBar.transform, "Gem 0", TextAnchor.MiddleCenter, new Vector2(0.42f, 0f), new Vector2(0.62f, 1f));
            CreateButton("LevelButton", topBar.transform, "Level", new Vector2(0.63f, 0.15f), new Vector2(0.715f, 0.85f));
            CreateButton("MissionButton", topBar.transform, "Mission", new Vector2(0.725f, 0.15f), new Vector2(0.81f, 0.85f));
            CreateButton("ShopButton", topBar.transform, "Shop", new Vector2(0.82f, 0.15f), new Vector2(0.895f, 0.85f));
            CreateButton("PauseButton", topBar.transform, "Pause", new Vector2(0.905f, 0.15f), new Vector2(0.99f, 0.85f));
        }

        private static void CreateObjectivePanel(Transform parent)
        {
            GameObject panel = CreatePanel("ObjectivePanel", parent, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -185f));
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(-80f, 78f);
            CreateText("ObjectiveText", panel.transform, "Deliver: Blooming Lotus x1", TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        }

        private static void CreateAbilityBar(Transform parent)
        {
            GameObject bar = CreatePanel("AbilityBar", parent, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 120f));
            RectTransform rect = bar.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(-80f, 150f);

            CreateButton("ShovelButton", bar.transform, "Shovel x2", new Vector2(0.02f, 0.15f), new Vector2(0.31f, 0.85f));
            CreateButton("MagicWandButton", bar.transform, "Wand x1", new Vector2(0.355f, 0.15f), new Vector2(0.645f, 0.85f));
            CreateButton("SortingMagnetButton", bar.transform, "Magnet x1", new Vector2(0.69f, 0.15f), new Vector2(0.98f, 0.85f));
        }

        private static void CreateSellBasket(Transform parent)
        {
            GameObject basket = CreatePanel("SellBasket", parent, AndroidHudLayoutMetrics.SellAnchorMin, AndroidHudLayoutMetrics.SellAnchorMax, new Vector2(0.5f, 0.5f), Vector2.zero);
            Image image = basket.GetComponent<Image>();
            image.sprite = PlaceholderSpriteFactory.SellBasketSprite;
            image.color = Color.white;
            ExternalDropZone zone = basket.AddComponent<ExternalDropZone>();
            zone.Configure(ExternalDropZoneKind.SellBasket, Color.white, new Color(0.84f, 1f, 0.54f, 1f));
            CreateText("SellBasketLabel", basket.transform, "Sell", TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        }

        private static void CreateDeliveryDropZone(Transform parent)
        {
            GameObject delivery = CreatePanel("DeliveryDropZone", parent, AndroidHudLayoutMetrics.DeliveryAnchorMin, AndroidHudLayoutMetrics.DeliveryAnchorMax, new Vector2(0.5f, 0.5f), Vector2.zero);
            Image image = delivery.GetComponent<Image>();
            image.sprite = PlaceholderSpriteFactory.DeliverZoneSprite;
            image.color = Color.white;
            ExternalDropZone zone = delivery.AddComponent<ExternalDropZone>();
            zone.Configure(ExternalDropZoneKind.Delivery, Color.white, new Color(1f, 0.76f, 1f, 1f));
            CreateText("DeliveryDropZoneLabel", delivery.transform, "Deliver", TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        }

        private static void CreateFeedbackText(Transform parent)
        {
            GameObject feedback = CreateText("FeedbackText", parent, string.Empty, TextAnchor.MiddleCenter, new Vector2(0f, 0.45f), new Vector2(1f, 0.55f));
            Text text = feedback.GetComponent<Text>();
            text.color = new Color(0.95f, 0.96f, 1f, 1f);
            text.fontSize = 42;
        }

        private static void CreateCoinFeedback(Transform parent)
        {
            GameObject coinText = CreateText("CoinFeedbackText", parent, string.Empty, TextAnchor.MiddleCenter, new Vector2(0f, 0f), new Vector2(0f, 0f));
            Text text = coinText.GetComponent<Text>();
            text.fontSize = 30;
            text.color = new Color(1f, 0.84f, 0.22f, 0f);
            coinText.AddComponent<CoinBurstFeedback>();
        }

        private static void CreateResultPanel(Transform parent)
        {
            GameObject panel = CreatePanel("ResultPanel", parent, new Vector2(0.12f, 0.36f), new Vector2(0.88f, 0.64f), new Vector2(0.5f, 0.5f), Vector2.zero);
            Image image = panel.GetComponent<Image>();
            image.color = new Color(0.08f, 0.11f, 0.15f, 0.92f);

            Text title = CreateText("ResultTitleText", panel.transform, "Level Complete", TextAnchor.MiddleCenter, new Vector2(0.05f, 0.62f), new Vector2(0.95f, 0.92f)).GetComponent<Text>();
            title.fontSize = 42;

            Text message = CreateText("ResultMessageText", panel.transform, "Blooming Lotus delivered.", TextAnchor.MiddleCenter, new Vector2(0.08f, 0.34f), new Vector2(0.92f, 0.62f)).GetComponent<Text>();
            message.fontSize = 26;

            CreateButton("RestartButton", panel.transform, "Restart", new Vector2(0.08f, 0.08f), new Vector2(0.46f, 0.30f));
            CreateButton("NextLevelButton", panel.transform, "Next", new Vector2(0.54f, 0.08f), new Vector2(0.92f, 0.30f));
            panel.SetActive(false);
        }

        private static void CreateLevelPanel(Transform parent)
        {
            GameObject panel = CreatePanel("LevelPanel", parent, AndroidHudLayoutMetrics.PanelAnchorMin, AndroidHudLayoutMetrics.PanelAnchorMax, new Vector2(0.5f, 0.5f), Vector2.zero);
            Image image = panel.GetComponent<Image>();
            image.color = new Color(0.12f, 0.16f, 0.18f, 0.97f);

            Text title = CreateText("LevelTitleText", panel.transform, "Levels", TextAnchor.MiddleLeft, new Vector2(0.05f, 0.90f), new Vector2(0.62f, 0.98f)).GetComponent<Text>();
            title.fontSize = 36;
            CreateButton("LevelCloseButton", panel.transform, "X", new Vector2(0.86f, 0.90f), new Vector2(0.96f, 0.98f));

            GameObject viewport = CreatePanel("LevelViewport", panel.transform, new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.88f), new Vector2(0.5f, 0.5f), Vector2.zero);
            viewport.GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.10f, 0.55f);
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            GameObject list = CreateRect("LevelList", viewport.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero);
            VerticalLayoutGroup layout = list.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 10f;
            layout.padding = new RectOffset(14, 14, 14, 14);
            ContentSizeFitter fitter = list.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scrollRect = viewport.AddComponent<ScrollRect>();
            scrollRect.content = list.GetComponent<RectTransform>();
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            panel.SetActive(false);
        }

        private static void CreateShopPanel(Transform parent)
        {
            GameObject panel = CreatePanel("ShopPanel", parent, AndroidHudLayoutMetrics.PanelAnchorMin, AndroidHudLayoutMetrics.PanelAnchorMax, new Vector2(0.5f, 0.5f), Vector2.zero);
            Image image = panel.GetComponent<Image>();
            image.sprite = PlaceholderSpriteFactory.ShopPanelSprite;
            image.color = Color.white;

            Text title = CreateText("ShopTitleText", panel.transform, "Shop", TextAnchor.MiddleLeft, new Vector2(0.05f, 0.90f), new Vector2(0.55f, 0.98f)).GetComponent<Text>();
            title.fontSize = 36;
            CreateButton("ShopCloseButton", panel.transform, "X", new Vector2(0.86f, 0.90f), new Vector2(0.96f, 0.98f));

            GameObject categoryBar = CreateRect("ShopCategoryBar", panel.transform, new Vector2(0.04f, 0.80f), new Vector2(0.96f, 0.89f), new Vector2(0.5f, 0.5f), Vector2.zero);
            CreateButton("ShopCategoryBoosterButton", categoryBar.transform, "Boost", new Vector2(0.00f, 0.05f), new Vector2(0.19f, 0.95f));
            CreateButton("ShopCategoryDecorationButton", categoryBar.transform, "Decor", new Vector2(0.205f, 0.05f), new Vector2(0.395f, 0.95f));
            CreateButton("ShopCategoryUnlockButton", categoryBar.transform, "Unlock", new Vector2(0.41f, 0.05f), new Vector2(0.60f, 0.95f));
            CreateButton("ShopCategoryCurrencyButton", categoryBar.transform, "Gem", new Vector2(0.615f, 0.05f), new Vector2(0.805f, 0.95f));
            CreateButton("ShopCategoryBundleButton", categoryBar.transform, "Bundle", new Vector2(0.82f, 0.05f), new Vector2(1f, 0.95f));

            GameObject viewport = CreatePanel("ShopProductViewport", panel.transform, new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.78f), new Vector2(0.5f, 0.5f), Vector2.zero);
            viewport.GetComponent<Image>().color = new Color(0.06f, 0.09f, 0.10f, 0.70f);
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            GameObject list = CreateRect("ShopProductList", viewport.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero);
            VerticalLayoutGroup layout = list.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 12f;
            layout.padding = new RectOffset(14, 14, 14, 14);
            ContentSizeFitter fitter = list.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scrollRect = viewport.AddComponent<ScrollRect>();
            scrollRect.content = list.GetComponent<RectTransform>();
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            panel.SetActive(false);
        }

        private static void CreateMissionPanel(Transform parent)
        {
            GameObject panel = CreatePanel("MissionPanel", parent, AndroidHudLayoutMetrics.PanelAnchorMin, AndroidHudLayoutMetrics.PanelAnchorMax, new Vector2(0.5f, 0.5f), Vector2.zero);
            Image image = panel.GetComponent<Image>();
            image.color = new Color(0.12f, 0.16f, 0.18f, 0.97f);

            Text title = CreateText("MissionTitleText", panel.transform, "Missions", TextAnchor.MiddleLeft, new Vector2(0.05f, 0.90f), new Vector2(0.62f, 0.98f)).GetComponent<Text>();
            title.fontSize = 36;
            CreateButton("MissionCloseButton", panel.transform, "X", new Vector2(0.86f, 0.90f), new Vector2(0.96f, 0.98f));

            GameObject viewport = CreatePanel("MissionViewport", panel.transform, new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.88f), new Vector2(0.5f, 0.5f), Vector2.zero);
            viewport.GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.10f, 0.55f);
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            GameObject list = CreateRect("MissionList", viewport.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero);
            VerticalLayoutGroup layout = list.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 10f;
            layout.padding = new RectOffset(12, 12, 12, 12);
            ContentSizeFitter fitter = list.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scrollRect = viewport.AddComponent<ScrollRect>();
            scrollRect.content = list.GetComponent<RectTransform>();
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            panel.SetActive(false);
        }

        private static void CreateMissionTracker(Transform parent)
        {
            GameObject panel = CreatePanel("MissionTrackerPanel", parent, AndroidHudLayoutMetrics.MissionTrackerAnchorMin, AndroidHudLayoutMetrics.MissionTrackerAnchorMax, new Vector2(0.5f, 0.5f), Vector2.zero);
            Image image = panel.GetComponent<Image>();
            image.color = new Color(0.12f, 0.16f, 0.18f, 0.90f);

            Text title = CreateText("MissionTrackerTitleText", panel.transform, "Missions", TextAnchor.MiddleLeft, new Vector2(0.07f, 0.88f), new Vector2(0.66f, 0.98f)).GetComponent<Text>();
            title.fontSize = 26;
            CreateButton("MissionTrackerOpenButton", panel.transform, "All", new Vector2(0.70f, 0.88f), new Vector2(0.96f, 0.98f));

            GameObject list = CreateRect("MissionTrackerList", panel.transform, new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.86f), new Vector2(0.5f, 0.5f), Vector2.zero);
            VerticalLayoutGroup layout = list.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 6f;
            layout.padding = new RectOffset(6, 6, 6, 6);
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition)
        {
            GameObject panel = CreateRect(name, parent, anchorMin, anchorMax, pivot, anchoredPosition);
            Image image = panel.AddComponent<Image>();
            image.sprite = PlaceholderSpriteFactory.HudPanelSprite;
            image.color = Color.white;
            return panel;
        }

        private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObject = CreateRect(name, parent, anchorMin, anchorMax, new Vector2(0.5f, 0.5f), Vector2.zero);
            Image image = buttonObject.AddComponent<Image>();
            image.sprite = PlaceholderSpriteFactory.HudButtonSprite;
            image.color = Color.white;
            buttonObject.AddComponent<Button>();
            CreateText("Label", buttonObject.transform, label, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            return buttonObject;
        }

        private static GameObject CreateText(string name, Transform parent, string content, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject textObject = CreateRect(name, parent, anchorMin, anchorMax, new Vector2(0.5f, 0.5f), Vector2.zero);
            Text text = textObject.AddComponent<Text>();
            text.text = content;
            text.alignment = alignment;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 30;
            text.color = Color.white;
            text.raycastTarget = false;
            return textObject;
        }

        private static GameObject CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = Vector2.zero;
            return gameObject;
        }

        [MenuItem("Eco Garden/Fix UI/EventSystem Input System Module")]
        public static void FixEventSystemInputModule()
        {
            EnsureInputSystemEventSystem();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void EnsureInputSystemEventSystem()
        {
            EventSystem eventSystem = Object.FindAnyObjectByType<EventSystem>();
            GameObject eventSystemObject;

            if (eventSystem == null)
            {
                eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
                eventSystem = eventSystemObject.GetComponent<EventSystem>();
            }
            else
            {
                eventSystemObject = eventSystem.gameObject;
            }

            StandaloneInputModule legacyModule = eventSystemObject.GetComponent<StandaloneInputModule>();
            if (legacyModule != null)
            {
                Object.DestroyImmediate(legacyModule);
            }

            if (eventSystemObject.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystemObject.AddComponent<InputSystemUIInputModule>();
            }

            Selection.activeGameObject = eventSystemObject;
        }
    }
}
