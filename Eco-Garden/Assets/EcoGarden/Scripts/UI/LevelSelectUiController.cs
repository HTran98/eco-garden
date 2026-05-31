using System.Collections.Generic;
using EcoGarden.Config;
using EcoGarden.Progression;
using EcoGarden.Rewards;
using EcoGarden.Save;
using EcoGarden.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class LevelSelectUiController : MonoBehaviour
    {
        [SerializeField] private Button levelButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject levelPanel;
        [SerializeField] private Text levelSummaryText;
        [SerializeField] private Transform levelListRoot;
        [SerializeField] private GameObject previewPanel;
        [SerializeField] private Text previewTitleText;
        [SerializeField] private Text previewMetaText;
        [SerializeField] private Text previewObjectiveText;
        [SerializeField] private Text previewRewardText;
        [SerializeField] private Button previewPlayButton;
        [SerializeField] private Button previewCloseButton;
        [SerializeField] private GameplayFeedbackController gameplayFeedbackController;

        private LevelCatalogController levelCatalogController;
        private readonly List<GameObject> levelRows = new List<GameObject>();
        private LevelDefinition previewLevel;
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
            if (visible)
            {
                UiModalPanelUtility.HideOtherModalPanels("LevelPanel");
            }

            if (levelPanel != null)
            {
                levelPanel.SetActive(visible);
                if (visible)
                {
                    UiModalPanelUtility.RaiseModalPanel(levelPanel);
                }
            }

            if (visible)
            {
                RefreshLevels();
            }
            else
            {
                SetPreviewVisible(false);
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
            RefreshLevelSummary(levels, saveData);
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
            string status = BuildLevelStatus(level, saveData, unlocked);
            GameObject row = new GameObject("LevelRow_" + level.LevelId, typeof(RectTransform), typeof(Image));
            row.transform.SetParent(levelListRoot, false);

            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0f, LevelSelectUiLayoutMetrics.LevelRowHeight);

            Image rowImage = row.GetComponent<Image>();
            rowImage.sprite = Resources.Load<Sprite>("UiSkins/ui_row_light") ?? PlaceholderSpriteFactory.HudPanelSprite;
            rowImage.color = GetRowColor(status);
            UiRowAccent.Apply(row.transform, GetRowAccentColor(status));

            CreateText("Status", row.transform, status, TextAnchor.MiddleCenter, LevelSelectUiLayoutMetrics.StatusAnchorMin, LevelSelectUiLayoutMetrics.StatusAnchorMax, 18);
            CreateText("Name", row.transform, BuildLevelTitle(level), TextAnchor.MiddleLeft, LevelSelectUiLayoutMetrics.TitleAnchorMin, LevelSelectUiLayoutMetrics.TitleAnchorMax, 24);
            CreateText("Meta", row.transform, BuildLevelMeta(level), TextAnchor.MiddleLeft, LevelSelectUiLayoutMetrics.MetaAnchorMin, LevelSelectUiLayoutMetrics.MetaAnchorMax, 18);
            CreateText("Summary", row.transform, BuildLevelSummary(level), TextAnchor.MiddleLeft, LevelSelectUiLayoutMetrics.SummaryAnchorMin, LevelSelectUiLayoutMetrics.SummaryAnchorMax, 18);

            GameObject buttonObject = CreateButton("PlayButton", row.transform, BuildActionLabel(status, unlocked), LevelSelectUiLayoutMetrics.ActionAnchorMin, LevelSelectUiLayoutMetrics.ActionAnchorMax);
            Button playButton = buttonObject.GetComponent<Button>();
            playButton.interactable = unlocked;
            ApplyPlayButtonState(buttonObject, unlocked);
            int levelId = level.LevelId;
            playButton.onClick.AddListener(() => ShowPreview(levelId));

            levelRows.Add(row);
        }

        private void ShowPreview(int levelId)
        {
            if (levelCatalogController == null || !levelCatalogController.Catalog.TryGetLevel(levelId, out LevelDefinition level))
            {
                PlayMessage("Level unavailable");
                return;
            }

            SaveData saveData = SaveService.Load();
            if (!LevelProgressionService.IsLevelUnlocked(saveData, level))
            {
                PlayMessage("Level locked");
                RefreshLevels();
                return;
            }

            previewLevel = level;
            EnsurePreviewPanel();
            RefreshPreview(level);
            SetPreviewVisible(true);
        }

        private void SelectPreviewLevel()
        {
            if (previewLevel == null)
            {
                PlayMessage("Select a level");
                return;
            }

            SelectLevel(previewLevel.LevelId);
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

        private static string BuildLevelMeta(LevelDefinition level)
        {
            string difficulty = level.Difficulty != null
                ? level.Difficulty.DifficultyKind.ToString()
                : "Normal";
            return difficulty + " / " + Mathf.CeilToInt(level.TimerSeconds) + "s";
        }

        private static string BuildLevelSummary(LevelDefinition level)
        {
            if (level.NpcOrder == null)
            {
                return "Order: delivery";
            }

            return "Order: " + level.NpcOrder.TotalRequiredItems + " item(s), tier " + level.NpcOrder.HighestRequiredLevel;
        }

        private static string BuildPreviewObjective(LevelDefinition level)
        {
            if (level == null || level.NpcOrder == null)
            {
                return "Objective: deliver the customer order";
            }

            return "Objective: " + BuildOrderText(level);
        }

        private static string BuildOrderText(LevelDefinition level)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            var requirements = level.NpcOrder.Requirements;
            for (int i = 0; i < requirements.Count; i++)
            {
                OrderRequirementDefinition requirement = requirements[i];
                if (requirement == null)
                {
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                ItemDefinitionName(level, requirement.Level, out string itemName);
                builder.Append(itemName);
                builder.Append(" x");
                builder.Append(requirement.Quantity);
            }

            return builder.Length > 0 ? builder.ToString() : "deliver order";
        }

        private static void ItemDefinitionName(LevelDefinition level, int itemLevel, out string itemName)
        {
            itemName = "Tier " + itemLevel;
            if (level == null)
            {
                return;
            }

            var item = level.GetItemDefinitionForLevel(itemLevel);
            if (item != null && !string.IsNullOrEmpty(item.DisplayName))
            {
                itemName = item.DisplayName;
            }
        }

        private static string BuildPreviewReward(LevelDefinition level)
        {
            RewardDefinition reward = level != null && level.NpcOrder != null
                ? level.NpcOrder.Reward
                : null;
            string rewardText = BuildRewardText(reward);
            string abilityText = BuildStartingAbilityText(level);
            return "Reward: " + rewardText + "  /  Start: " + abilityText;
        }

        private static string BuildRewardText(RewardDefinition reward)
        {
            if (reward == null)
            {
                return "none";
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            if (reward.Currencies != null)
            {
                for (int i = 0; i < reward.Currencies.Length; i++)
                {
                    CurrencyReward currency = reward.Currencies[i];
                    if (currency == null || currency.Amount <= 0)
                    {
                        continue;
                    }

                    AppendPart(builder, currency.CurrencyKind + " +" + currency.Amount);
                }
            }

            if (reward.Abilities != null)
            {
                for (int i = 0; i < reward.Abilities.Length; i++)
                {
                    AbilityReward ability = reward.Abilities[i];
                    if (ability == null || ability.Count <= 0)
                    {
                        continue;
                    }

                    AppendPart(builder, ability.AbilityKind + " +" + ability.Count);
                }
            }

            return builder.Length > 0 ? builder.ToString() : "none";
        }

        private static string BuildStartingAbilityText(LevelDefinition level)
        {
            if (level == null || level.StartingAbilities == null || level.StartingAbilities.Count == 0)
            {
                return "no boosters";
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            for (int i = 0; i < level.StartingAbilities.Count; i++)
            {
                AbilityCountDefinition ability = level.StartingAbilities[i];
                if (ability == null || ability.Count <= 0)
                {
                    continue;
                }

                AppendPart(builder, ability.AbilityKind + " x" + ability.Count);
            }

            return builder.Length > 0 ? builder.ToString() : "no boosters";
        }

        private static void AppendPart(System.Text.StringBuilder builder, string text)
        {
            if (builder.Length > 0)
            {
                builder.Append(", ");
            }

            builder.Append(text);
        }

        private static string BuildLevelStatus(LevelDefinition level, SaveData saveData, bool unlocked)
        {
            if (!unlocked)
            {
                return "Locked";
            }

            int highestUnlocked = saveData != null ? Mathf.Max(1, saveData.highestUnlockedLevel) : 1;
            return level.LevelId < highestUnlocked ? "Done" : "Current";
        }

        private static string BuildActionLabel(string status, bool unlocked)
        {
            if (!unlocked)
            {
                return "Locked";
            }

            if (status == "Current")
            {
                return "Preview";
            }

            if (status == "Done")
            {
                return "Replay";
            }

            return "Play";
        }

        private void RefreshLevelSummary(IReadOnlyList<LevelDefinition> levels, SaveData saveData)
        {
            EnsureLevelSummaryText();
            if (levelSummaryText == null)
            {
                return;
            }

            int total = levels != null ? levels.Count : 0;
            int highestUnlocked = saveData != null ? Mathf.Max(1, saveData.highestUnlockedLevel) : 1;
            int unlockedCount = 0;
            for (int i = 0; i < total; i++)
            {
                if (LevelProgressionService.IsLevelUnlocked(saveData, levels[i]))
                {
                    unlockedCount++;
                }
            }

            levelSummaryText.text = "Unlocked " + unlockedCount + "/" + total + "  /  Current Level " + highestUnlocked;
        }

        private static Color GetRowColor(string status)
        {
            if (status == "Locked")
            {
                return Color.Lerp(UiThemePalette.PanelMuted, UiThemePalette.DisabledButton, 0.32f);
            }

            if (status == "Done")
            {
                return Color.Lerp(UiThemePalette.PanelStrong, UiThemePalette.Success, 0.20f);
            }

            return UiThemePalette.Panel;
        }

        private static Color GetRowAccentColor(string status)
        {
            if (status == "Locked")
            {
                return UiThemePalette.DisabledButton;
            }

            if (status == "Done")
            {
                return UiThemePalette.Success;
            }

            return UiThemePalette.Selected;
        }

        private static void ApplyPlayButtonState(GameObject buttonObject, bool unlocked)
        {
            Image image = buttonObject.GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            image.color = unlocked
                ? UiThemePalette.PrimaryButton
                : UiThemePalette.DisabledButton;
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

            EnsureLevelSummaryText();
            EnsurePreviewPanel();

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

            if (previewPlayButton != null)
            {
                previewPlayButton.onClick.RemoveListener(SelectPreviewLevel);
                previewPlayButton.onClick.AddListener(SelectPreviewLevel);
            }

            if (previewCloseButton != null)
            {
                previewCloseButton.onClick.RemoveListener(HidePreview);
                previewCloseButton.onClick.AddListener(HidePreview);
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

        private void RefreshPreview(LevelDefinition level)
        {
            if (level == null)
            {
                return;
            }

            if (previewTitleText != null)
            {
                previewTitleText.text = BuildLevelTitle(level);
            }

            if (previewMetaText != null)
            {
                string notes = level.Difficulty != null && !string.IsNullOrEmpty(level.Difficulty.Notes)
                    ? " / " + level.Difficulty.Notes
                    : string.Empty;
                previewMetaText.text = BuildLevelMeta(level) + notes;
            }

            if (previewObjectiveText != null)
            {
                previewObjectiveText.text = BuildPreviewObjective(level);
            }

            if (previewRewardText != null)
            {
                previewRewardText.text = BuildPreviewReward(level);
            }
        }

        private void HidePreview()
        {
            SetPreviewVisible(false);
        }

        private void SetPreviewVisible(bool visible)
        {
            if (previewPanel != null)
            {
                previewPanel.SetActive(visible);
                if (visible)
                {
                    previewPanel.transform.SetAsLastSibling();
                }
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
            image.sprite = Resources.Load<Sprite>("UiSkins/ui_button_primary") ?? PlaceholderSpriteFactory.HudButtonSprite;
            image.color = UiThemePalette.PrimaryButton;
            Button button = gameObject.GetComponent<Button>();
            button.colors = UiThemePalette.BuildButtonColors(UiThemePalette.PrimaryButton);
            gameObject.AddComponent<UiButtonFeedback>();
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
            label.color = UiThemePalette.TextDark;
            return gameObject;
        }

        private void EnsureLevelSummaryText()
        {
            if (levelSummaryText == null)
            {
                GameObject summaryObject = FindObjectIncludingInactive("LevelSummaryText");
                levelSummaryText = summaryObject != null ? summaryObject.GetComponent<Text>() : null;
            }

            if (levelSummaryText != null || levelPanel == null)
            {
                return;
            }

            levelSummaryText = CreateText(
                "LevelSummaryText",
                levelPanel.transform,
                string.Empty,
                TextAnchor.MiddleLeft,
                LevelSelectUiLayoutMetrics.PanelSummaryAnchorMin,
                LevelSelectUiLayoutMetrics.PanelSummaryAnchorMax,
                22).GetComponent<Text>();
        }

        private void EnsurePreviewPanel()
        {
            if (previewPanel == null)
            {
                previewPanel = FindObjectIncludingInactive("LevelPreviewPanel");
            }

            if (previewPanel == null && levelPanel != null)
            {
                previewPanel = CreatePreviewPanel(levelPanel.transform);
            }

            if (previewPanel == null)
            {
                return;
            }

            previewTitleText = previewTitleText ?? GetText("LevelPreviewTitleText");
            previewMetaText = previewMetaText ?? GetText("LevelPreviewMetaText");
            previewObjectiveText = previewObjectiveText ?? GetText("LevelPreviewObjectiveText");
            previewRewardText = previewRewardText ?? GetText("LevelPreviewRewardText");
            previewPlayButton = previewPlayButton ?? GetButton("LevelPreviewPlayButton");
            previewCloseButton = previewCloseButton ?? GetButton("LevelPreviewCloseButton");
            previewPanel.SetActive(false);
        }

        private static GameObject CreatePreviewPanel(Transform parent)
        {
            GameObject panel = new GameObject("LevelPreviewPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = LevelSelectUiLayoutMetrics.PreviewAnchorMin;
            rect.anchorMax = LevelSelectUiLayoutMetrics.PreviewAnchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = panel.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>("UiSkins/ui_panel_overlay") ?? PlaceholderSpriteFactory.HudPanelSprite;
            image.color = UiThemePalette.PanelOverlay;

            CreateText("LevelPreviewTitleText", panel.transform, "Level", TextAnchor.MiddleLeft, LevelSelectUiLayoutMetrics.PreviewTitleAnchorMin, LevelSelectUiLayoutMetrics.PreviewTitleAnchorMax, 25);
            CreateText("LevelPreviewMetaText", panel.transform, string.Empty, TextAnchor.MiddleLeft, LevelSelectUiLayoutMetrics.PreviewMetaAnchorMin, LevelSelectUiLayoutMetrics.PreviewMetaAnchorMax, 17);
            CreateText("LevelPreviewObjectiveText", panel.transform, string.Empty, TextAnchor.MiddleLeft, LevelSelectUiLayoutMetrics.PreviewObjectiveAnchorMin, LevelSelectUiLayoutMetrics.PreviewObjectiveAnchorMax, 18);
            CreateText("LevelPreviewRewardText", panel.transform, string.Empty, TextAnchor.MiddleLeft, LevelSelectUiLayoutMetrics.PreviewRewardAnchorMin, LevelSelectUiLayoutMetrics.PreviewRewardAnchorMax, 18);
            CreateButton("LevelPreviewPlayButton", panel.transform, "Play", LevelSelectUiLayoutMetrics.PreviewPlayAnchorMin, LevelSelectUiLayoutMetrics.PreviewPlayAnchorMax);
            CreateButton("LevelPreviewCloseButton", panel.transform, "Close", LevelSelectUiLayoutMetrics.PreviewCloseAnchorMin, LevelSelectUiLayoutMetrics.PreviewCloseAnchorMax);
            Text[] texts = panel.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = UiThemePalette.TextLight;
            }

            return panel;
        }

        private static Text GetText(string objectName)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            return gameObject != null ? gameObject.GetComponent<Text>() : null;
        }

        private static Button GetButton(string objectName)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            return gameObject != null ? gameObject.GetComponent<Button>() : null;
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
