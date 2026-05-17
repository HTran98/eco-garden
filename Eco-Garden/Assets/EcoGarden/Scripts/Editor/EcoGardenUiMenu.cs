using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using EcoGarden.UI;
using EcoGarden.Input;

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
            CreateSellBasket(hudRoot.transform);
            CreateFeedbackText(hudRoot.transform);
            CreateCoinFeedback(hudRoot.transform);
            CreateResultPanel(hudRoot.transform);
            hudRoot.AddComponent<AbilityHudController>();
            hudRoot.AddComponent<DraggedItemCanvasGhost>();
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

            CreateText("TimerText", topBar.transform, "03:00", TextAnchor.MiddleLeft, new Vector2(0f, 0f), new Vector2(0.33f, 1f));
            CreateText("GoldText", topBar.transform, "Gold 0", TextAnchor.MiddleCenter, new Vector2(0.33f, 0f), new Vector2(0.66f, 1f));
            CreateButton("PauseButton", topBar.transform, "Pause", new Vector2(0.78f, 0.15f), new Vector2(0.98f, 0.85f));
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
            GameObject basket = CreatePanel("SellBasket", parent, new Vector2(0.72f, 0.16f), new Vector2(0.96f, 0.29f), new Vector2(0.5f, 0.5f), Vector2.zero);
            Image image = basket.GetComponent<Image>();
            image.color = new Color(0.30f, 0.44f, 0.34f, 0.92f);
            ExternalDropZone zone = basket.AddComponent<ExternalDropZone>();
            CreateText("SellBasketLabel", basket.transform, "Sell", TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
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

            CreateButton("RestartButton", panel.transform, "Restart", new Vector2(0.28f, 0.08f), new Vector2(0.72f, 0.30f));
            panel.SetActive(false);
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition)
        {
            GameObject panel = CreateRect(name, parent, anchorMin, anchorMax, pivot, anchoredPosition);
            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.12f, 0.16f, 0.20f, 0.78f);
            return panel;
        }

        private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObject = CreateRect(name, parent, anchorMin, anchorMax, new Vector2(0.5f, 0.5f), Vector2.zero);
            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.24f, 0.48f, 0.62f, 1f);
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
