using EcoGarden.Save;
using EcoGarden.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class TutorialUiController : MonoBehaviour
    {
        private const int StepCount = 4;

        private static readonly string[] Titles =
        {
            "Welcome to Eco Garden",
            "Grow Seeds",
            "Merge Plants",
            "Deliver Orders"
        };

        private static readonly string[] Messages =
        {
            "Build a clean garden by growing plants, merging them, and completing simple orders.",
            "Tap seed maker tiles on the board to create new plants when there is room.",
            "Drag two matching plants together to merge them into a stronger plant.",
            "Drop requested plants on Deliver. Sell extra plants for gold when your board gets crowded."
        };

        private Canvas canvas;
        private GameObject overlayObject;
        private Text titleText;
        private Text bodyText;
        private Text stepText;
        private Text nextButtonText;
        private int currentStep;

        private void Start()
        {
            SaveData saveData = SaveService.Load();
            if (saveData.tutorialCompleted)
            {
                enabled = false;
                return;
            }

            EnsureCanvas();
            EnsureEventSystem();
            BuildOverlay();
            ShowStep(0);
        }

        public void MarkCompleted()
        {
            SaveData saveData = SaveService.Load();
            saveData.tutorialCompleted = true;
            SaveService.Save(saveData);

            SaveController saveController = FindAnyObjectByType<SaveController>();
            if (saveController != null && saveController.Data != null)
            {
                saveController.Data.tutorialCompleted = true;
            }

            if (overlayObject != null)
            {
                Destroy(overlayObject);
            }

            enabled = false;
        }

        private void NextStep()
        {
            if (currentStep >= StepCount - 1)
            {
                MarkCompleted();
                return;
            }

            ShowStep(currentStep + 1);
        }

        private void ShowStep(int step)
        {
            currentStep = Mathf.Clamp(step, 0, StepCount - 1);

            if (titleText != null)
            {
                titleText.text = Titles[currentStep];
            }

            if (bodyText != null)
            {
                bodyText.text = Messages[currentStep];
            }

            if (stepText != null)
            {
                stepText.text = (currentStep + 1) + " / " + StepCount;
            }

            if (nextButtonText != null)
            {
                nextButtonText.text = currentStep >= StepCount - 1 ? "Done" : "Next";
            }
        }

        private void EnsureCanvas()
        {
            canvas = FindAnyObjectByType<Canvas>();
            if (canvas != null)
            {
                return;
            }

            GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                if (EventSystem.current.GetComponent<InputSystemUIInputModule>() == null)
                {
                    EventSystem.current.gameObject.AddComponent<InputSystemUIInputModule>();
                }

                return;
            }

            GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystemObject.transform.SetAsLastSibling();
        }

        private void BuildOverlay()
        {
            overlayObject = new GameObject("TutorialOverlay", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            overlayObject.transform.SetParent(canvas.transform, false);
            overlayObject.transform.SetAsLastSibling();

            RectTransform overlayRect = overlayObject.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            Image overlayImage = overlayObject.GetComponent<Image>();
            overlayImage.sprite = PlaceholderSpriteFactory.SquareSprite;
            overlayImage.color = UiThemePalette.PanelOverlay;

            CanvasGroup group = overlayObject.GetComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject panel = CreateImage("TutorialPanel", overlayObject.transform, UiThemePalette.Panel, new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.48f));
            Image panelImage = panel.GetComponent<Image>();
            panelImage.sprite = Resources.Load<Sprite>("UiSkins/ui_panel_light") ?? PlaceholderSpriteFactory.HudPanelSprite;
            panelImage.color = Color.white;

            titleText = CreateText("TutorialTitle", panel.transform, TextAnchor.MiddleLeft, new Vector2(0.08f, 0.70f), new Vector2(0.92f, 0.90f), 34, UiThemePalette.TextDark);
            bodyText = CreateText("TutorialBody", panel.transform, TextAnchor.UpperLeft, new Vector2(0.08f, 0.32f), new Vector2(0.92f, 0.68f), 25, UiThemePalette.TextDark);
            bodyText.horizontalOverflow = HorizontalWrapMode.Wrap;
            bodyText.verticalOverflow = VerticalWrapMode.Truncate;

            stepText = CreateText("TutorialStep", panel.transform, TextAnchor.MiddleLeft, new Vector2(0.08f, 0.08f), new Vector2(0.30f, 0.24f), 20, UiThemePalette.TextMuted);

            CreateButton("TutorialSkipButton", panel.transform, "Skip", new Vector2(0.43f, 0.07f), new Vector2(0.62f, 0.24f), UiThemePalette.DisabledButton, MarkCompleted);
            GameObject nextButton = CreateButton("TutorialNextButton", panel.transform, "Next", new Vector2(0.66f, 0.07f), new Vector2(0.92f, 0.24f), UiThemePalette.PrimaryButton, NextStep);
            nextButtonText = nextButton.GetComponentInChildren<Text>();
        }

        private static GameObject CreateImage(string name, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            imageObject.transform.SetParent(parent, false);

            RectTransform rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = imageObject.GetComponent<Image>();
            image.sprite = PlaceholderSpriteFactory.HudPanelSprite;
            image.color = color;

            return imageObject;
        }

        private static Text CreateText(string name, Transform parent, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, int fontSize, Color color)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Text text = textObject.GetComponent<Text>();
            text.alignment = alignment;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = color;
            text.raycastTarget = false;

            return text;
        }

        private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Color color, UnityEngine.Events.UnityAction action)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = buttonObject.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>("UiSkins/ui_button_primary") ?? PlaceholderSpriteFactory.HudButtonSprite;
            image.color = color;

            Button button = buttonObject.GetComponent<Button>();
            button.colors = UiThemePalette.BuildButtonColors(color);
            button.onClick.AddListener(action);

            Text text = CreateText("Label", buttonObject.transform, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 24, UiThemePalette.TextLight);
            text.text = label;

            return buttonObject;
        }
    }
}
