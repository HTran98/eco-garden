using EcoGarden.Board;
using EcoGarden.Config;
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
        [SerializeField] private Text feedbackText;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Text resultTitleText;
        [SerializeField] private Text resultMessageText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button pauseButton;

        private float remainingSeconds;

        public LevelPlayState State { get; private set; } = LevelPlayState.NotStarted;
        public bool IsPlaying { get { return State == LevelPlayState.Playing; } }

        private void Awake()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            AutoWireReferences();
            WireRestartButton();
            WirePauseButton();
        }

        private void OnEnable()
        {
            if (boardController != null)
            {
                boardController.OrderProgressChanged += RefreshObjective;
            }
        }

        private void OnDisable()
        {
            if (boardController != null)
            {
                boardController.OrderProgressChanged -= RefreshObjective;
            }
        }

        private void Start()
        {
            StartLevel();
        }

        private void Update()
        {
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
            SetFeedback(string.Empty);
            RefreshObjective();
            RefreshTimer();
            RefreshPauseButton();
        }

        public void CompleteLevel()
        {
            if (State != LevelPlayState.Playing)
            {
                return;
            }

            State = LevelPlayState.Completed;
            RefreshPauseButton();
            ShowResult("Level Complete", "Blooming Lotus delivered.");
        }

        public void FailLevel()
        {
            if (State != LevelPlayState.Playing)
            {
                return;
            }

            State = LevelPlayState.Failed;
            RefreshPauseButton();
            ShowResult("Time Up", "The customer left before the order was delivered.");
        }

        public void TogglePause()
        {
            if (State == LevelPlayState.Playing)
            {
                State = LevelPlayState.Paused;
                SetFeedback("Paused");
                RefreshPauseButton();
                return;
            }

            if (State == LevelPlayState.Paused)
            {
                State = LevelPlayState.Playing;
                SetFeedback(string.Empty);
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
                return;
            }

            objectiveText.text = "Deliver: " + BuildOrderDescription(order);
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

            if (resultPanel == null)
            {
                GameObject resultObject = GameObject.Find("ResultPanel");
                if (resultObject != null)
                {
                    resultPanel = resultObject;
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

            if (restartButton == null)
            {
                GameObject restartObject = GameObject.Find("RestartButton");
                if (restartObject != null)
                {
                    restartButton = restartObject.GetComponent<Button>();
                }
            }

            if (pauseButton == null)
            {
                GameObject pauseObject = GameObject.Find("PauseButton");
                if (pauseObject != null)
                {
                    pauseButton = pauseObject.GetComponent<Button>();
                }
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
                label.text = State == LevelPlayState.Paused ? "Resume" : "Pause";
            }
        }

        private void SetResultPanelVisible(bool visible)
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(visible);
            }
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
            GameObject gameObject = GameObject.Find(objectName);
            return gameObject != null ? gameObject.GetComponent<Text>() : null;
        }
    }
}
