using System.Collections.Generic;
using EcoGarden.Config;
using EcoGarden.Progression;
using EcoGarden.Save;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class LevelSelectUiController : MonoBehaviour
    {
        [SerializeField] private Button levelButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject levelPanel;
        [SerializeField] private Transform levelListRoot;
        [SerializeField] private GameplayFeedbackController gameplayFeedbackController;

        private LevelCatalogController levelCatalogController;
        private readonly List<GameObject> levelRows = new List<GameObject>();
        private bool buttonsWired;

        private void Awake()
        {
            ResolveReferences();
            WireButtons();
            SetPanelVisible(false);
        }

        private void Start()
        {
            ResolveReferences();
            WireButtons();
            SetPanelVisible(false);
        }

        private void OnEnable()
        {
            ResolveReferences();
            WireButtons();
            RefreshLevels();
        }

        public void ToggleLevels()
        {
            ResolveReferences();
            WireButtons();
            SetPanelVisible(levelPanel == null || !levelPanel.activeSelf);
        }

        public void CloseLevels()
        {
            SetPanelVisible(false);
        }

        private void SetPanelVisible(bool visible)
        {
            if (levelPanel != null)
            {
                levelPanel.SetActive(visible);
            }

            if (visible)
            {
                RefreshLevels();
            }
        }

        private void RefreshLevels()
        {
            ResolveReferences();
            ClearRows();

            if (levelCatalogController == null || levelListRoot == null)
            {
                return;
            }

            SaveData saveData = SaveService.Load();
            IReadOnlyList<LevelDefinition> levels = levelCatalogController.Catalog.Levels;
            for (int i = 0; i < levels.Count; i++)
            {
                CreateLevelRow(levels[i], saveData);
            }
        }

        private void CreateLevelRow(LevelDefinition level, SaveData saveData)
        {
            if (level == null)
            {
                return;
            }

            bool unlocked = LevelProgressionService.IsLevelUnlocked(saveData, level);
            GameObject row = new GameObject("LevelRow_" + level.LevelId, typeof(RectTransform), typeof(Image));
            row.transform.SetParent(levelListRoot, false);

            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0f, 104f);

            Image rowImage = row.GetComponent<Image>();
            rowImage.color = unlocked
                ? new Color(0.10f, 0.15f, 0.16f, 0.90f)
                : new Color(0.08f, 0.09f, 0.10f, 0.72f);

            CreateText("Name", row.transform, BuildLevelTitle(level), TextAnchor.MiddleLeft, new Vector2(0.04f, 0.48f), new Vector2(0.64f, 0.94f), 24);
            CreateText("Meta", row.transform, BuildLevelMeta(level, unlocked), TextAnchor.MiddleLeft, new Vector2(0.04f, 0.08f), new Vector2(0.64f, 0.48f), 18);

            GameObject buttonObject = CreateButton("PlayButton", row.transform, unlocked ? "Play" : "Locked", new Vector2(0.68f, 0.20f), new Vector2(0.96f, 0.80f));
            Button playButton = buttonObject.GetComponent<Button>();
            playButton.interactable = unlocked;
            int levelId = level.LevelId;
            playButton.onClick.AddListener(() => SelectLevel(levelId));

            levelRows.Add(row);
        }

        private void SelectLevel(int levelId)
        {
            if (levelCatalogController == null)
            {
                PlayMessage("Level list unavailable");
                return;
            }

            if (levelCatalogController.SelectLevel(levelId, SaveService.Load()))
            {
                SetPanelVisible(false);
                PlayMessage("Level " + levelId);
                return;
            }

            PlayMessage("Level locked");
            RefreshLevels();
        }

        private static string BuildLevelTitle(LevelDefinition level)
        {
            string name = string.IsNullOrEmpty(level.LevelName)
                ? "Level " + level.LevelId
                : level.LevelName;
            return level.LevelId + ". " + name;
        }

        private static string BuildLevelMeta(LevelDefinition level, bool unlocked)
        {
            string difficulty = level.Difficulty != null
                ? level.Difficulty.DifficultyKind.ToString()
                : "Normal";
            return difficulty + " / " + Mathf.CeilToInt(level.TimerSeconds) + "s" + (unlocked ? string.Empty : " / Locked");
        }

        private void ResolveReferences()
        {
            if (levelButton == null)
            {
                levelButton = FindButton("LevelButton");
            }

            if (closeButton == null)
            {
                closeButton = FindButton("LevelCloseButton");
            }

            if (levelPanel == null)
            {
                levelPanel = FindObjectIncludingInactive("LevelPanel");
            }

            if (levelListRoot == null)
            {
                GameObject listObject = FindObjectIncludingInactive("LevelList");
                if (listObject != null)
                {
                    levelListRoot = listObject.transform;
                }
            }

            if (levelCatalogController == null)
            {
                levelCatalogController = FindAnyObjectByType<LevelCatalogController>();
            }

            if (gameplayFeedbackController == null)
            {
                gameplayFeedbackController = FindAnyObjectByType<GameplayFeedbackController>();
            }
        }

        private void WireButtons()
        {
            if (buttonsWired)
            {
                return;
            }

            if (levelButton != null)
            {
                levelButton.onClick.RemoveListener(ToggleLevels);
                levelButton.onClick.AddListener(ToggleLevels);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(CloseLevels);
                closeButton.onClick.AddListener(CloseLevels);
            }

            buttonsWired = levelButton != null && closeButton != null;
        }

        private void ClearRows()
        {
            for (int i = 0; i < levelRows.Count; i++)
            {
                if (levelRows[i] != null)
                {
                    Destroy(levelRows[i]);
                }
            }

            levelRows.Clear();
        }

        private void PlayMessage(string message)
        {
            if (gameplayFeedbackController != null)
            {
                gameplayFeedbackController.PlayHudMessage(message);
            }
        }

        private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            gameObject.transform.SetParent(parent, false);
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = gameObject.GetComponent<Image>();
            image.color = new Color(0.20f, 0.34f, 0.36f, 0.95f);
            CreateText("Label", gameObject.transform, label, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 21);
            return gameObject;
        }

        private static GameObject CreateText(string name, Transform parent, string text, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, int fontSize)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Text label = gameObject.GetComponent<Text>();
            label.text = text;
            label.alignment = alignment;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = fontSize;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 12;
            label.resizeTextMaxSize = fontSize;
            label.color = Color.white;
            return gameObject;
        }

        private static Button FindButton(string objectName)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            return gameObject != null ? gameObject.GetComponent<Button>() : null;
        }

        private static GameObject FindObjectIncludingInactive(string objectName)
        {
            Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform transform = transforms[i];
                if (transform != null && transform.gameObject.name == objectName)
                {
                    return transform.gameObject;
                }
            }

            return null;
        }
    }
}
