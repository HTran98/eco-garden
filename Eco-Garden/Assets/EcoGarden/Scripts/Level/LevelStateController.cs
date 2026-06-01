using EcoGarden.Audio;
using EcoGarden.Board;
using EcoGarden.Config;
using EcoGarden.Progression;
using EcoGarden.Save;
using EcoGarden.UI;
using EcoGarden.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EcoGarden.Level
{
    public sealed class LevelStateController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private Text timerText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private GameObject objectivePanel;
        [SerializeField] private Text feedbackText;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Text resultTitleText;
        [SerializeField] private Text resultMessageText;
        [SerializeField] private Text resultCountdownText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button pauseResumeButton;
        [SerializeField] private Button pauseRestartButton;
        [SerializeField] private LevelCatalogController levelCatalogController;
        [SerializeField] private float timerWarningSeconds = 20f;
        [SerializeField] private float timerCriticalSeconds = 10f;
        [SerializeField] private bool autoAdvanceToNextLevel = true;
        [SerializeField] private float autoAdvanceDelaySeconds = 5f;

        private const float MinimumAutoAdvanceDelaySeconds = 5f;
        private float remainingSeconds;
        private RectTransform timerRectTransform;
        private Color timerBaseColor;
        private Vector3 timerBaseScale;
        private bool timerPresentationCached;
        private Coroutine autoAdvanceRoutine;
        private float autoAdvanceStartRealtime;

        public LevelPlayState State { get; private set; } = LevelPlayState.NotStarted;
        public bool IsPlaying { get { return State == LevelPlayState.Playing; } }
        public float RemainingSeconds { get { return remainingSeconds; } }

        public event Action LevelCompleted;
        public event Action LevelFailed;

        private void Awake()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            AutoWireReferences();
            WireRestartButton();
            WireNextLevelButton();
            WirePauseButton();
            WirePausePanelButtons();
        }

        private void OnEnable()
        {
            if (boardController != null)
            {
                boardController.OrderProgressChanged += RefreshObjective;
                boardController.OrderCompleted += CompleteLevel;
            }
        }

        private void OnDisable()
        {
            if (autoAdvanceRoutine != null)
            {
                StopCoroutine(autoAdvanceRoutine);
                autoAdvanceRoutine = null;
            }

            if (boardController != null)
            {
                boardController.OrderProgressChanged -= RefreshObjective;
                boardController.OrderCompleted -= CompleteLevel;
            }
        }

        private void Start()
        {
            StartLevel();
        }

        private void Update()
        {
            if (State == LevelPlayState.Completed)
            {
                RefreshResultCountdown();
                return;
            }

            if (State != LevelPlayState.Playing)
            {
                return;
            }

            remainingSeconds -= Time.deltaTime;
            if (remainingSeconds <= 0f)
            {
                remainingSeconds = 0f;
                FailLevel();
            }

            RefreshTimer();
        }

        public void StartLevel()
        {
            if (boardController == null || boardController.LevelDefinition == null)
            {
                return;
            }

            remainingSeconds = boardController.LevelDefinition.TimerSeconds;
            State = LevelPlayState.Playing;
            SetResultPanelVisible(false);
            SetPausePanelVisible(false);
            SetFeedback(string.Empty);
            ClearResultCountdown();
            RefreshObjective();
            CacheTimerPresentation();
            RefreshTimer();
            RefreshPauseButton();
            RefreshNextLevelButton();
            ShowLevelStartHint();
        }

        public void CompleteLevel()
        {
            if (State != LevelPlayState.Playing)
            {
                return;
            }

            State = LevelPlayState.Completed;
            RefreshPauseButton();
            RefreshNextLevelButton();
            RefreshResultActionLabels();
            ShowResult("Level Complete", BuildCompletionMessage());
            LevelCompleted?.Invoke();
            ScheduleAutoAdvanceToNextLevel();
            RefreshResultCountdown();
        }

        public void FailLevel()
        {
            if (State != LevelPlayState.Playing)
            {
                return;
            }

            State = LevelPlayState.Failed;
            RefreshPauseButton();
            RefreshNextLevelButton();
            RefreshResultActionLabels();
            ShowResult("Time Up", BuildFailureMessage());
            ClearResultCountdown();
            LevelFailed?.Invoke();
        }

        public void TogglePause()
        {
            if (State == LevelPlayState.Playing)
            {
                State = LevelPlayState.Paused;
                HideBlockingPanels();
                SetFeedback("Paused");
                EcoGardenAudioController.Instance?.PlayPauseOpen();
                SetPausePanelVisible(true);
                RefreshPauseButton();
                return;
            }

            if (State == LevelPlayState.Paused)
            {
                State = LevelPlayState.Playing;
                SetFeedback(string.Empty);
                SetPausePanelVisible(false);
                RefreshPauseButton();
            }
        }

        public void RestartLevel()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.buildIndex >= 0)
            {
                SceneManager.LoadScene(activeScene.buildIndex);
            }
            else
            {
                SceneManager.LoadScene(activeScene.name);
            }
        }

        public void StartNextLevel()
        {
            if (autoAdvanceRoutine != null)
            {
                StopCoroutine(autoAdvanceRoutine);
                autoAdvanceRoutine = null;
            }

            if (boardController == null || boardController.LevelDefinition == null)
            {
                SetResultPanelVisible(false);
                return;
            }

            if (levelCatalogController == null)
            {
                levelCatalogController = FindObjectOfTypeIncludingInactive<LevelCatalogController>();
            }

            if (levelCatalogController == null)
            {
                RestartCurrentLevelFromComplete();
                return;
            }

            if (!TryGetNextCatalogLevel(out LevelDefinition nextLevel))
            {
                RestartCurrentLevelFromComplete();
                return;
            }

            SaveData saveData = SaveService.Load();
            LevelProgressionService.TryUnlockNextLevel(saveData, boardController.LevelDefinition);
            if (nextLevel.LevelId > saveData.highestUnlockedLevel)
            {
                saveData.highestUnlockedLevel = nextLevel.LevelId;
            }

            SaveService.Save(saveData);

            levelCatalogController.SetBoardController(boardController);
            if (levelCatalogController.SelectLevel(nextLevel, saveData) ||
                levelCatalogController.SelectLevelAfterUnlock(nextLevel))
            {
                HideBlockingPanels();
                SetResultPanelVisible(false);
                StartLevel();
                return;
            }

            RestartCurrentLevelFromComplete();
        }

        private void ScheduleAutoAdvanceToNextLevel()
        {
            autoAdvanceStartRealtime = Time.unscaledTime;
            if (!autoAdvanceToNextLevel || !Application.isPlaying || State != LevelPlayState.Completed)
            {
                return;
            }

            if (autoAdvanceRoutine != null)
            {
                StopCoroutine(autoAdvanceRoutine);
            }

            autoAdvanceRoutine = StartCoroutine(AutoAdvanceToNextLevelRoutine());
        }

        private IEnumerator AutoAdvanceToNextLevelRoutine()
        {
            yield return new WaitForSecondsRealtime(GetAutoAdvanceDelaySeconds());
            autoAdvanceRoutine = null;

            if (State == LevelPlayState.Completed)
            {
                StartNextLevel();
            }
        }

        private void RefreshTimer()
        {
            if (timerText == null)
            {
                return;
            }

            int totalSeconds = Mathf.CeilToInt(remainingSeconds);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            RefreshTimerUrgency();
        }

        private void RefreshTimerUrgency()
        {
            CacheTimerPresentation();
            if (timerText == null || timerRectTransform == null)
            {
                return;
            }

            if (remainingSeconds > timerWarningSeconds || State != LevelPlayState.Playing)
            {
                timerText.color = timerBaseColor;
                timerRectTransform.localScale = timerBaseScale;
                return;
            }

            float pulse = (Mathf.Sin(Time.unscaledTime * 7.5f) + 1f) * 0.5f;
            bool critical = remainingSeconds <= timerCriticalSeconds;
            Color warningColor = critical
                ? new Color(1f, 0.42f, 0.34f, 1f)
                : UiThemePalette.SecondaryButton;
            timerText.color = Color.Lerp(warningColor, Color.white, pulse * 0.18f);
            timerRectTransform.localScale = timerBaseScale * Mathf.Lerp(1.02f, 1.075f, pulse);
        }

        private void CacheTimerPresentation()
        {
            if (timerPresentationCached || timerText == null)
            {
                return;
            }

            timerRectTransform = timerText.GetComponent<RectTransform>();
            timerBaseColor = timerText.color;
            timerBaseScale = timerRectTransform != null ? timerRectTransform.localScale : Vector3.one;
            timerPresentationCached = true;
        }

        private void RefreshObjective()
        {
            if (objectiveText == null || boardController == null || boardController.LevelDefinition == null)
            {
                return;
            }

            var order = boardController.LevelDefinition.NpcOrder;
            if (order == null)
            {
                objectiveText.text = "Deliver order";
                RefreshObjectivePresentation(0, 0);
                return;
            }

            objectiveText.text = "Deliver: " + BuildOrderDescription(order);
            GetOrderProgress(out int submittedCount, out int requiredCount);
            RefreshObjectivePresentation(submittedCount, requiredCount);
        }

        private void RefreshObjectivePresentation(int submittedCount, int requiredCount)
        {
            if (objectiveText == null)
            {
                return;
            }

            if (objectivePanel == null)
            {
                objectivePanel = FindObjectIncludingInactive("ObjectivePanel");
            }

            float progress = requiredCount > 0
                ? Mathf.Clamp01((float)submittedCount / requiredCount)
                : 0f;
            Color accentColor = progress >= 1f
                ? UiThemePalette.Success
                : progress >= 0.66f
                    ? UiThemePalette.SecondaryButton
                    : UiThemePalette.PrimaryButton;

            objectiveText.color = progress >= 1f
                ? UiThemePalette.Success
                : UiThemePalette.TextDark;

            if (objectivePanel != null)
            {
                UiRowAccent.Apply(objectivePanel.transform, accentColor);
            }
        }

        private void GetOrderProgress(out int submittedCount, out int requiredCount)
        {
            submittedCount = 0;
            requiredCount = 0;

            var runtimeRequirements = boardController != null
                ? boardController.ActiveOrderRequirements
                : null;
            if (runtimeRequirements == null)
            {
                return;
            }

            for (int i = 0; i < runtimeRequirements.Count; i++)
            {
                var requirement = runtimeRequirements[i];
                if (requirement == null)
                {
                    continue;
                }

                submittedCount += Mathf.Clamp(requirement.SubmittedCount, 0, requirement.RequiredCount);
                requiredCount += Mathf.Max(0, requirement.RequiredCount);
            }
        }

        private string BuildOrderDescription(NpcOrderDefinition order)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            var runtimeRequirements = boardController.ActiveOrderRequirements;
            if (runtimeRequirements != null && runtimeRequirements.Count > 0)
            {
                for (int i = 0; i < runtimeRequirements.Count; i++)
                {
                    var requirement = runtimeRequirements[i];
                    AppendRequirement(builder, requirement.FamilyId, requirement.Level, requirement.RequiredCount, requirement.SubmittedCount);
                }

                return builder.Length > 0 ? builder.ToString() : "order";
            }

            var requirements = order.Requirements;
            for (int i = 0; i < requirements.Count; i++)
            {
                OrderRequirementDefinition requirement = requirements[i];
                if (requirement == null)
                {
                    continue;
                }

                AppendRequirement(builder, requirement.FamilyId, requirement.Level, requirement.Quantity, 0);
            }

            return builder.Length > 0 ? builder.ToString() : "order";
        }

        private void AppendRequirement(System.Text.StringBuilder builder, string familyId, int level, int requiredCount, int submittedCount)
        {
            if (builder.Length > 0)
            {
                builder.Append(", ");
            }

            string itemName = familyId;
            var itemDefinition = boardController.LevelDefinition.GetItemDefinitionForLevel(level);
            if (itemDefinition != null && !string.IsNullOrEmpty(itemDefinition.DisplayName))
            {
                itemName = itemDefinition.DisplayName;
            }

            builder.Append(itemName);
            builder.Append(" ");
            builder.Append(submittedCount);
            builder.Append("/");
            builder.Append(requiredCount);
        }

        private void ShowResult(string title, string message)
        {
            HideBlockingPanels();
            SetPausePanelVisible(false);
            SetResultPanelVisible(true);

            if (resultTitleText != null)
            {
                resultTitleText.text = title;
            }

            if (resultMessageText != null)
            {
                resultMessageText.text = message;
            }

            SetFeedback(message);
            RefreshResultCountdown();
        }

        private string BuildCompletionMessage()
        {
            if (boardController == null || boardController.LevelDefinition == null)
            {
                return "Order delivered.";
            }

            string levelName = string.IsNullOrEmpty(boardController.LevelDefinition.LevelName)
                ? "Level " + boardController.LevelDefinition.LevelId
                : boardController.LevelDefinition.LevelName;
            return levelName + " complete. Reward granted.";
        }

        private string BuildFailureMessage()
        {
            if (boardController == null || boardController.LevelDefinition == null)
            {
                return "The customer left before the order was delivered.";
            }

            string levelName = string.IsNullOrEmpty(boardController.LevelDefinition.LevelName)
                ? "Level " + boardController.LevelDefinition.LevelId
                : boardController.LevelDefinition.LevelName;
            return levelName + " failed. Retry the order.";
        }

        private void ShowLevelStartHint()
        {
            if (boardController == null || boardController.LevelDefinition == null)
            {
                return;
            }

            switch (boardController.LevelDefinition.LevelId)
            {
                case 1:
                    SetFeedback("Tap the seed pot, then drag matching sprouts together.");
                    break;
                case 2:
                    SetFeedback("Use Sell for spare plants and keep board space open.");
                    break;
                case 3:
                    SetFeedback("Use the Magic Wand when a plant needs one quick upgrade.");
                    break;
                default:
                    SetFeedback(string.Empty);
                    break;
            }
        }

        private void AutoWireReferences()
        {
            if (timerText == null)
            {
                timerText = FindText("TimerText");
            }

            if (feedbackText == null)
            {
                feedbackText = FindText("FeedbackText");
            }

            if (objectiveText == null)
            {
                objectiveText = FindText("ObjectiveText");
            }

            if (objectivePanel == null)
            {
                objectivePanel = FindObjectIncludingInactive("ObjectivePanel");
            }

            if (resultPanel == null)
            {
                GameObject resultObject = FindObjectIncludingInactive("ResultPanel");
                if (resultObject != null)
                {
                    resultPanel = resultObject;
                }
            }

            if (pausePanel == null)
            {
                GameObject pauseObject = FindObjectIncludingInactive("PausePanel");
                if (pauseObject != null)
                {
                    pausePanel = pauseObject;
                }
            }

            if (resultTitleText == null)
            {
                resultTitleText = FindText("ResultTitleText");
            }

            if (resultMessageText == null)
            {
                resultMessageText = FindText("ResultMessageText");
            }

            if (resultCountdownText == null)
            {
                resultCountdownText = FindText("ResultCountdownText");
            }

            if (restartButton == null)
            {
                GameObject restartObject = FindObjectIncludingInactive("RestartButton");
                if (restartObject != null)
                {
                    restartButton = restartObject.GetComponent<Button>();
                }
            }

            if (nextLevelButton == null)
            {
                GameObject nextObject = FindObjectIncludingInactive("NextLevelButton");
                if (nextObject != null)
                {
                    nextLevelButton = nextObject.GetComponent<Button>();
                }
            }

            if (pauseButton == null)
            {
                GameObject pauseObject = FindObjectIncludingInactive("PauseButton");
                if (pauseObject != null)
                {
                    pauseButton = pauseObject.GetComponent<Button>();
                }
            }

            if (pauseResumeButton == null)
            {
                GameObject resumeObject = FindObjectIncludingInactive("PauseResumeButton");
                if (resumeObject != null)
                {
                    pauseResumeButton = resumeObject.GetComponent<Button>();
                }
            }

            if (pauseRestartButton == null)
            {
                GameObject restartObject = FindObjectIncludingInactive("PauseRestartButton");
                if (restartObject != null)
                {
                    pauseRestartButton = restartObject.GetComponent<Button>();
                }
            }

            if (levelCatalogController == null)
            {
                levelCatalogController = FindAnyObjectByType<LevelCatalogController>();
            }
        }

        private void WireRestartButton()
        {
            if (restartButton == null)
            {
                return;
            }

            restartButton.onClick.RemoveListener(RestartLevel);
            restartButton.onClick.AddListener(RestartLevel);
        }

        private void WireNextLevelButton()
        {
            EnsureNextLevelButton();
            if (nextLevelButton == null)
            {
                return;
            }

            nextLevelButton.onClick.RemoveListener(StartNextLevel);
            nextLevelButton.onClick.AddListener(StartNextLevel);
            RefreshNextLevelButton();
        }

        private void WirePauseButton()
        {
            if (pauseButton == null)
            {
                return;
            }

            pauseButton.onClick.RemoveListener(TogglePause);
            pauseButton.onClick.AddListener(TogglePause);
            RefreshPauseButton();
        }

        private void WirePausePanelButtons()
        {
            EnsurePausePanel();
            if (pauseResumeButton != null)
            {
                pauseResumeButton.onClick.RemoveListener(TogglePause);
                pauseResumeButton.onClick.AddListener(TogglePause);
            }

            if (pauseRestartButton != null)
            {
                pauseRestartButton.onClick.RemoveListener(RestartLevel);
                pauseRestartButton.onClick.AddListener(RestartLevel);
            }
        }

        private void RefreshPauseButton()
        {
            if (pauseButton == null)
            {
                return;
            }

            pauseButton.interactable = State == LevelPlayState.Playing || State == LevelPlayState.Paused;

            Text label = pauseButton.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = State == LevelPlayState.Paused ? "Play" : "Pause";
            }

            RefreshPauseButtonIcon();
        }

        private void RefreshPauseButtonIcon()
        {
            if (pauseButton == null)
            {
                return;
            }

            Image iconImage = EnsureButtonRuntimeIcon(pauseButton);
            if (iconImage == null)
            {
                return;
            }

            Sprite pauseSprite = Resources.Load<Sprite>("UiIcons/icon_pause");
            iconImage.sprite = State == LevelPlayState.Paused
                ? PlaceholderSpriteFactory.PlayIconSprite
                : pauseSprite ?? iconImage.sprite;
            iconImage.enabled = iconImage.sprite != null;
        }

        private void RefreshNextLevelButton()
        {
            EnsureNextLevelButton();
            if (nextLevelButton == null)
            {
                return;
            }

            bool canUseNext = State == LevelPlayState.Completed && boardController != null;

            nextLevelButton.gameObject.SetActive(canUseNext);
            nextLevelButton.interactable = canUseNext;
        }

        private bool CanStartNextLevel()
        {
            if (State != LevelPlayState.Completed ||
                boardController == null ||
                boardController.LevelDefinition == null)
            {
                return false;
            }

            return TryGetNextCatalogLevel(out _);
        }

        private bool TryGetNextCatalogLevel(out LevelDefinition nextLevel)
        {
            nextLevel = null;
            if (levelCatalogController == null)
            {
                levelCatalogController = FindObjectOfTypeIncludingInactive<LevelCatalogController>();
            }

            if (levelCatalogController == null ||
                levelCatalogController.Catalog == null ||
                levelCatalogController.Catalog.Levels.Count == 0 ||
                boardController == null ||
                boardController.LevelDefinition == null)
            {
                return false;
            }

            int currentLevelId = boardController.LevelDefinition.LevelId;
            bool catalogContainsCurrent = false;
            var levels = levelCatalogController.Catalog.Levels;
            for (int i = 0; i < levels.Count; i++)
            {
                LevelDefinition candidate = levels[i];
                if (candidate == null)
                {
                    continue;
                }

                if (candidate.LevelId == currentLevelId)
                {
                    catalogContainsCurrent = true;
                    continue;
                }

                if (candidate.LevelId > currentLevelId)
                {
                    nextLevel = candidate;
                    return true;
                }
            }

            if (!catalogContainsCurrent)
            {
                nextLevel = levels[0];
                return nextLevel != null;
            }

            return false;
        }

        private void HideBlockingPanels()
        {
            SetPanelsInactive("ShopPanel");
            SetPanelsInactive("MissionPanel");
            SetPanelsInactive("MissionTrackerPanel");
            SetPanelsInactive("LevelPanel");
            SetPanelsInactive("PausePanel");
        }

        private static void SetPanelsInactive(string objectName)
        {
            Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform candidate = transforms[i];
                if (candidate != null &&
                    candidate.gameObject.scene.IsValid() &&
                    candidate.name == objectName)
                {
                    candidate.gameObject.SetActive(false);
                }
            }
        }

        private void RefreshResultActionLabels()
        {
            SetButtonLabel(restartButton, State == LevelPlayState.Failed ? "Retry" : "Replay");
            SetButtonLabel(nextLevelButton, "Next");
        }

        private float GetAutoAdvanceDelaySeconds()
        {
            return Mathf.Max(MinimumAutoAdvanceDelaySeconds, autoAdvanceDelaySeconds);
        }

        private void RefreshResultCountdown()
        {
            EnsureResultCountdownText();
            if (resultCountdownText == null)
            {
                return;
            }

            if (State != LevelPlayState.Completed)
            {
                ClearResultCountdown();
                return;
            }

            if (!autoAdvanceToNextLevel || !Application.isPlaying)
            {
                resultCountdownText.text = "Tap Next to continue";
                resultCountdownText.gameObject.SetActive(true);
                return;
            }

            float remaining = Mathf.Max(0f, GetAutoAdvanceDelaySeconds() - (Time.unscaledTime - autoAdvanceStartRealtime));
            resultCountdownText.text = "Auto next in " + Mathf.CeilToInt(remaining) + "s";
            resultCountdownText.gameObject.SetActive(true);
        }

        private void ClearResultCountdown()
        {
            if (resultCountdownText != null)
            {
                resultCountdownText.text = string.Empty;
                resultCountdownText.gameObject.SetActive(false);
            }
        }

        private void RestartCurrentLevelFromComplete()
        {
            HideBlockingPanels();
            SetResultPanelVisible(false);
            if (boardController != null && boardController.LevelDefinition != null)
            {
                boardController.LoadLevel();
                StartLevel();
                SetFeedback("Next level unavailable. Replaying current level.");
            }
        }

        private void SetResultPanelVisible(bool visible)
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(visible);
                if (visible)
                {
                    UiModalPanelUtility.RaiseModalPanel(resultPanel);
                }
            }
        }

        private void SetPausePanelVisible(bool visible)
        {
            EnsurePausePanel();
            if (pausePanel != null)
            {
                pausePanel.SetActive(visible);
                if (visible)
                {
                    UiModalPanelUtility.RaiseModalPanel(pausePanel);
                }
            }
        }

        private void EnsurePausePanel()
        {
            if (pausePanel == null)
            {
                pausePanel = FindObjectIncludingInactive("PausePanel");
            }

            if (pausePanel == null)
            {
                GameObject canvasRoot = FindObjectIncludingInactive("HUDRoot");
                Transform parent = canvasRoot != null ? canvasRoot.transform : transform;
                pausePanel = CreatePausePanel(parent);
            }

            if (pauseResumeButton == null)
            {
                GameObject resumeObject = FindObjectIncludingInactive("PauseResumeButton");
                if (resumeObject != null)
                {
                    pauseResumeButton = resumeObject.GetComponent<Button>();
                }
            }

            if (pauseRestartButton == null)
            {
                GameObject restartObject = FindObjectIncludingInactive("PauseRestartButton");
                if (restartObject != null)
                {
                    pauseRestartButton = restartObject.GetComponent<Button>();
                }
            }
        }

        private void EnsureNextLevelButton()
        {
            if (nextLevelButton == null)
            {
                GameObject nextObject = FindObjectIncludingInactive("NextLevelButton");
                if (nextObject != null)
                {
                    nextLevelButton = nextObject.GetComponent<Button>();
                }
            }

            if (nextLevelButton == null && resultPanel != null)
            {
                GameObject buttonObject = CreateResultButton(
                    "NextLevelButton",
                    resultPanel.transform,
                    "Next",
                    PanelUiLayoutMetrics.ResultNextAnchorMin,
                    PanelUiLayoutMetrics.ResultNextAnchorMax);
                nextLevelButton = buttonObject.GetComponent<Button>();
            }

            if (nextLevelButton == null)
            {
                return;
            }

            nextLevelButton.onClick.RemoveListener(StartNextLevel);
            nextLevelButton.onClick.AddListener(StartNextLevel);
        }

        private void EnsureResultCountdownText()
        {
            if (resultCountdownText == null)
            {
                resultCountdownText = FindText("ResultCountdownText");
            }

            if (resultCountdownText != null || resultPanel == null)
            {
                return;
            }

            GameObject textObject = new GameObject("ResultCountdownText", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(resultPanel.transform, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = PanelUiLayoutMetrics.ResultCountdownAnchorMin;
            rect.anchorMax = PanelUiLayoutMetrics.ResultCountdownAnchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            resultCountdownText = textObject.GetComponent<Text>();
            resultCountdownText.alignment = TextAnchor.MiddleCenter;
            resultCountdownText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            resultCountdownText.fontSize = 20;
            resultCountdownText.resizeTextForBestFit = true;
            resultCountdownText.resizeTextMinSize = 12;
            resultCountdownText.resizeTextMaxSize = 20;
            resultCountdownText.color = UiThemePalette.TextLight;
            resultCountdownText.raycastTarget = false;
        }

        private void SetFeedback(string message)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
            }
        }

        private static Text FindText(string objectName)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            return gameObject != null ? gameObject.GetComponent<Text>() : null;
        }

        private static void SetButtonLabel(Button button, string label)
        {
            if (button == null)
            {
                return;
            }

            Text text = button.GetComponentInChildren<Text>(true);
            if (text != null)
            {
                text.text = label;
            }
        }

        private static GameObject CreateResultButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            Image image = buttonObject.GetComponent<Image>();
            Sprite skinSprite = Resources.Load<Sprite>("UiSkins/ui_button_primary");
            image.sprite = skinSprite ?? PlaceholderSpriteFactory.HudButtonSprite;
            image.color = skinSprite != null ? Color.white : UiThemePalette.PrimaryButton;

            Button button = buttonObject.GetComponent<Button>();
            button.colors = UiThemePalette.BuildButtonColors(UiThemePalette.PrimaryButton);
            buttonObject.AddComponent<UiButtonFeedback>();

            GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(buttonObject.transform, false);
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.pivot = new Vector2(0.5f, 0.5f);
            labelRect.anchoredPosition = Vector2.zero;
            labelRect.sizeDelta = Vector2.zero;

            Text text = labelObject.GetComponent<Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 28;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 14;
            text.resizeTextMaxSize = 28;
            text.color = UiThemePalette.TextLight;
            text.raycastTarget = false;

            return buttonObject;
        }

        private static GameObject CreatePausePanel(Transform parent)
        {
            GameObject panel = new GameObject("PausePanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = AndroidHudLayoutMetrics.ResultAnchorMin;
            rect.anchorMax = AndroidHudLayoutMetrics.ResultAnchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            Image image = panel.GetComponent<Image>();
            Sprite skinSprite = Resources.Load<Sprite>("UiSkins/ui_panel_overlay");
            image.sprite = skinSprite ?? PlaceholderSpriteFactory.HudPanelSprite;
            image.color = skinSprite != null ? Color.white : UiThemePalette.PanelOverlay;

            CreatePauseText(panel.transform, "PauseTitleText", "Paused", 40, PanelUiLayoutMetrics.PauseTitleAnchorMin, PanelUiLayoutMetrics.PauseTitleAnchorMax);
            CreatePauseText(panel.transform, "PauseMessageText", "Take a break or restart this level.", 22, PanelUiLayoutMetrics.PauseMessageAnchorMin, PanelUiLayoutMetrics.PauseMessageAnchorMax);
            CreateResultButton("PauseResumeButton", panel.transform, "Resume", PanelUiLayoutMetrics.PauseResumeAnchorMin, PanelUiLayoutMetrics.PauseResumeAnchorMax);
            CreateResultButton("PauseRestartButton", panel.transform, "Restart", PanelUiLayoutMetrics.PauseRestartAnchorMin, PanelUiLayoutMetrics.PauseRestartAnchorMax);
            panel.SetActive(false);
            return panel;
        }

        private static void CreatePauseText(Transform parent, string name, string content, int fontSize, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            Text text = textObject.GetComponent<Text>();
            text.text = content;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 12;
            text.resizeTextMaxSize = fontSize;
            text.color = UiThemePalette.TextLight;
            text.raycastTarget = false;
        }

        private static Image EnsureButtonRuntimeIcon(Button button)
        {
            if (button == null)
            {
                return null;
            }

            Transform iconTransform = button.transform.Find("RuntimeIcon");
            GameObject iconObject = iconTransform != null
                ? iconTransform.gameObject
                : new GameObject("RuntimeIcon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(button.transform, false);
            iconObject.transform.SetAsLastSibling();

            RectTransform iconRect = iconObject.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.18f, 0.18f);
            iconRect.anchorMax = new Vector2(0.82f, 0.82f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            Image iconImage = iconObject.GetComponent<Image>();
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            return iconImage;
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

        private static T FindObjectOfTypeIncludingInactive<T>() where T : Component
        {
            T activeObject = FindAnyObjectByType<T>();
            if (activeObject != null)
            {
                return activeObject;
            }

            T[] candidates = Resources.FindObjectsOfTypeAll<T>();
            for (int i = 0; i < candidates.Length; i++)
            {
                T candidate = candidates[i];
                if (candidate != null && candidate.gameObject.scene.IsValid())
                {
                    return candidate;
                }
            }

            return null;
        }
    }
}
