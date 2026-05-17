using EcoGarden.Board;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EcoGarden.Level
{
    public sealed class LevelStateController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private Text timerText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Text resultTitleText;
        [SerializeField] private Text resultMessageText;
        [SerializeField] private Button restartButton;

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
        }

        private void OnEnable()
        {
            if (boardController != null)
            {
                boardController.ObjectiveCompleted += CompleteLevel;
            }
        }

        private void OnDisable()
        {
            if (boardController != null)
            {
                boardController.ObjectiveCompleted -= CompleteLevel;
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
            RefreshTimer();
        }

        public void CompleteLevel()
        {
            if (State != LevelPlayState.Playing)
            {
                return;
            }

            State = LevelPlayState.Completed;
            ShowResult("Level Complete", "Blooming Lotus delivered.");
        }

        public void FailLevel()
        {
            if (State != LevelPlayState.Playing)
            {
                return;
            }

            State = LevelPlayState.Failed;
            ShowResult("Time Up", "The customer left before the order was delivered.");
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
