using System.Collections.Generic;
using EcoGarden.Missions;
using EcoGarden.Rewards;
using EcoGarden.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class MissionUiController : MonoBehaviour
    {
        [SerializeField] private Button missionButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject missionPanel;
        [SerializeField] private Transform missionListRoot;
        [SerializeField] private GameObject missionTrackerPanel;
        [SerializeField] private Transform missionTrackerRoot;
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private GameObject levelPanel;
        [SerializeField] private GameplayFeedbackController gameplayFeedbackController;

        private MissionController missionController;
        private readonly List<GameObject> missionRows = new List<GameObject>();
        private readonly List<GameObject> trackerRows = new List<GameObject>();
        private bool buttonsWired;

        private void Awake()
        {
            ResolveReferences();
            WireButtons();
            SetPanelVisible(false);
            RefreshMissions();
        }

        private void Start()
        {
            ResolveReferences();
            WireButtons();
            SetPanelVisible(false);
            Subscribe();
        }

        private void OnEnable()
        {
            ResolveReferences();
            WireButtons();
            Subscribe();
            RefreshMissions();
        }

        private void OnDisable()
        {
            if (missionController != null)
            {
                missionController.MissionsChanged -= RefreshMissions;
            }
        }

        private void Update()
        {
            if (!buttonsWired)
            {
                ResolveReferences();
                WireButtons();
            }

            UpdateTrackerVisibility();
        }

        public void ToggleMissions()
        {
            ResolveReferences();
            WireButtons();
            SetPanelVisible(missionPanel == null || !missionPanel.activeSelf);
        }

        public void CloseMissions()
        {
            SetPanelVisible(false);
        }

        private void SetPanelVisible(bool visible)
        {
            if (missionPanel != null)
            {
                missionPanel.SetActive(visible);
            }

            if (visible)
            {
                RefreshMissions();
            }
        }

        private void RefreshMissions()
        {
            ResolveReferences();
            ClearMissionRows();

            if (missionController == null || missionListRoot == null)
            {
                RefreshTrackerRows();
                return;
            }

            IReadOnlyList<MissionRuntimeState> missions = missionController.Missions;
            for (int i = 0; i < missions.Count; i++)
            {
                CreateMissionRow(missions[i]);
            }

            RefreshTrackerRows();
        }

        private void RefreshTrackerRows()
        {
            ClearTrackerRows();

            if (missionController == null || missionTrackerRoot == null)
            {
                return;
            }

            IReadOnlyList<MissionRuntimeState> missions = missionController.Missions;
            int createdCount = 0;
            for (int i = 0; i < missions.Count && createdCount < AndroidHudLayoutMetrics.MaxCompactMissionRows; i++)
            {
                MissionRuntimeState state = missions[i];
                if (state == null || state.RewardClaimed)
                {
                    continue;
                }

                CreateTrackerRow(state);
                createdCount++;
            }
        }

        private void CreateMissionRow(MissionRuntimeState state)
        {
            if (state == null || state.Definition == null)
            {
                return;
            }

            MissionDefinition definition = state.Definition;
            GameObject row = new GameObject("MissionRow_" + definition.MissionId, typeof(RectTransform), typeof(Image));
            row.transform.SetParent(missionListRoot, false);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0f, 112f);

            Image rowImage = row.GetComponent<Image>();
            rowImage.color = state.RewardClaimed
                ? new Color(0.08f, 0.13f, 0.11f, 0.78f)
                : new Color(0.10f, 0.14f, 0.16f, 0.88f);

            CreateText("Name", row.transform, BuildMissionTitle(definition), TextAnchor.MiddleLeft, new Vector2(0.03f, 0.58f), new Vector2(0.60f, 0.96f), 24);
            CreateText("Progress", row.transform, BuildProgressText(state), TextAnchor.MiddleLeft, new Vector2(0.03f, 0.30f), new Vector2(0.60f, 0.58f), 20);
            CreateText("Reward", row.transform, BuildRewardText(definition.Reward), TextAnchor.MiddleLeft, new Vector2(0.03f, 0.04f), new Vector2(0.60f, 0.30f), 18);

            GameObject claimObject = CreateButton("ClaimButton", row.transform, BuildClaimLabel(state), new Vector2(0.66f, 0.22f), new Vector2(0.96f, 0.78f));
            Button claimButton = claimObject.GetComponent<Button>();
            claimButton.interactable = state.CanClaim;
            claimButton.onClick.AddListener(() => ClaimMission(definition.MissionId));

            missionRows.Add(row);
        }

        private void CreateTrackerRow(MissionRuntimeState state)
        {
            if (state == null || state.Definition == null)
            {
                return;
            }

            MissionDefinition definition = state.Definition;
            GameObject row = new GameObject("MissionTrackerRow_" + definition.MissionId, typeof(RectTransform), typeof(Image));
            row.transform.SetParent(missionTrackerRoot, false);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0f, 92f);

            Image rowImage = row.GetComponent<Image>();
            rowImage.color = state.CanClaim
                ? new Color(0.18f, 0.30f, 0.18f, 0.88f)
                : new Color(0.10f, 0.14f, 0.16f, 0.82f);

            CreateText("Name", row.transform, BuildMissionTitle(definition), TextAnchor.MiddleLeft, new Vector2(0.04f, 0.50f), new Vector2(0.66f, 0.96f), 22);
            CreateText("Progress", row.transform, BuildProgressText(state), TextAnchor.MiddleLeft, new Vector2(0.04f, 0.08f), new Vector2(0.66f, 0.50f), 19);

            GameObject claimObject = CreateButton("ClaimButton", row.transform, BuildClaimLabel(state), new Vector2(0.69f, 0.20f), new Vector2(0.96f, 0.80f));
            Button claimButton = claimObject.GetComponent<Button>();
            claimButton.interactable = state.CanClaim;
            claimButton.onClick.AddListener(() => ClaimMission(definition.MissionId));

            trackerRows.Add(row);
        }

        private void ClaimMission(string missionId)
        {
            if (missionController == null)
            {
                PlayMessage("Missions unavailable");
                return;
            }

            MissionClaimResult result = missionController.TryClaimMissionReward(missionId);
            PlayMessage(BuildClaimMessage(result.Status));
            RefreshMissions();
        }

        private void ResolveReferences()
        {
            if (missionController == null)
            {
                missionController = FindAnyObjectByType<MissionController>();
            }

            if (gameplayFeedbackController == null)
            {
                gameplayFeedbackController = FindAnyObjectByType<GameplayFeedbackController>();
            }

            if (missionButton == null)
            {
                missionButton = FindButton("MissionButton");
            }

            if (missionButton == null)
            {
                CreateRuntimeMissionButton();
            }

            if (closeButton == null)
            {
                closeButton = FindButton("MissionCloseButton");
            }

            if (missionPanel == null)
            {
                missionPanel = FindObjectIncludingInactive("MissionPanel");
            }

            if (missionPanel == null)
            {
                CreateRuntimeMissionPanel();
            }

            if (missionListRoot == null)
            {
                GameObject listObject = FindObjectIncludingInactive("MissionList");
                missionListRoot = listObject != null ? listObject.transform : null;
            }

            if (missionTrackerPanel == null)
            {
                missionTrackerPanel = FindObjectIncludingInactive("MissionTrackerPanel");
            }

            if (missionTrackerPanel == null)
            {
                CreateRuntimeMissionTracker();
            }

            if (missionTrackerRoot == null)
            {
                GameObject trackerListObject = FindObjectIncludingInactive("MissionTrackerList");
                missionTrackerRoot = trackerListObject != null ? trackerListObject.transform : null;
            }

            if (shopPanel == null)
            {
                shopPanel = FindObjectIncludingInactive("ShopPanel");
            }

            if (levelPanel == null)
            {
                levelPanel = FindObjectIncludingInactive("LevelPanel");
            }
        }

        private void WireButtons()
        {
            if (missionButton != null)
            {
                missionButton.onClick.RemoveListener(ToggleMissions);
                missionButton.onClick.AddListener(ToggleMissions);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(CloseMissions);
                closeButton.onClick.AddListener(CloseMissions);
            }

            buttonsWired = missionButton != null;
        }

        private void Subscribe()
        {
            ResolveReferences();
            if (missionController == null)
            {
                return;
            }

            missionController.MissionsChanged -= RefreshMissions;
            missionController.MissionsChanged += RefreshMissions;
        }

        private void ClearMissionRows()
        {
            for (int i = 0; i < missionRows.Count; i++)
            {
                if (missionRows[i] != null)
                {
                    Destroy(missionRows[i]);
                }
            }

            missionRows.Clear();
        }

        private void ClearTrackerRows()
        {
            for (int i = 0; i < trackerRows.Count; i++)
            {
                if (trackerRows[i] != null)
                {
                    Destroy(trackerRows[i]);
                }
            }

            trackerRows.Clear();
        }

        private void UpdateTrackerVisibility()
        {
            if (missionTrackerPanel == null)
            {
                return;
            }

            bool shopOpen = shopPanel != null && shopPanel.activeSelf;
            bool levelOpen = levelPanel != null && levelPanel.activeSelf;
            bool fullMissionOpen = missionPanel != null && missionPanel.activeSelf;
            bool shouldShow = !shopOpen && !levelOpen && !fullMissionOpen;
            if (missionTrackerPanel.activeSelf != shouldShow)
            {
                missionTrackerPanel.SetActive(shouldShow);
            }
        }

        private void CreateRuntimeMissionButton()
        {
            GameObject topBar = FindObjectIncludingInactive("TopBar");
            if (topBar == null)
            {
                return;
            }

            GameObject buttonObject = CreateButton("MissionButton", topBar.transform, "Tasks", AndroidHudLayoutMetrics.MissionButtonAnchorMin, AndroidHudLayoutMetrics.MissionButtonAnchorMax);
            missionButton = buttonObject.GetComponent<Button>();
        }

        private void CreateRuntimeMissionPanel()
        {
            Transform parent = transform;
            GameObject panel = CreatePanel("MissionPanel", parent, AndroidHudLayoutMetrics.PanelAnchorMin, AndroidHudLayoutMetrics.PanelAnchorMax);
            missionPanel = panel;

            CreateText("MissionTitleText", panel.transform, "Missions", TextAnchor.MiddleLeft, PanelUiLayoutMetrics.TitleAnchorMin, PanelUiLayoutMetrics.TitleAnchorMax, 34);
            CreateButton("MissionCloseButton", panel.transform, "X", PanelUiLayoutMetrics.CloseAnchorMin, PanelUiLayoutMetrics.CloseAnchorMax);

            GameObject viewport = CreatePanel("MissionViewport", panel.transform, PanelUiLayoutMetrics.FullContentAnchorMin, PanelUiLayoutMetrics.FullContentAnchorMax);
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            GameObject list = CreateRect("MissionList", viewport.transform, new Vector2(0f, 1f), new Vector2(1f, 1f));
            missionListRoot = list.transform;
            RectTransform listRect = list.GetComponent<RectTransform>();
            listRect.pivot = new Vector2(0.5f, 1f);
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
            scrollRect.content = listRect;
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            HudSkinController skinController = GetComponent<HudSkinController>();
            if (skinController != null)
            {
                skinController.Apply();
            }

            panel.SetActive(false);
        }

        private void CreateRuntimeMissionTracker()
        {
            Transform parent = transform;
            GameObject panel = CreatePanel("MissionTrackerPanel", parent, AndroidHudLayoutMetrics.MissionTrackerAnchorMin, AndroidHudLayoutMetrics.MissionTrackerAnchorMax);
            missionTrackerPanel = panel;

            CreateText("MissionTrackerTitleText", panel.transform, "Missions", TextAnchor.MiddleLeft, new Vector2(0.07f, 0.88f), new Vector2(0.66f, 0.98f), 24);
            CreateButton("MissionTrackerOpenButton", panel.transform, "All", new Vector2(0.70f, 0.88f), new Vector2(0.96f, 0.98f));

            GameObject list = CreateRect("MissionTrackerList", panel.transform, new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.86f));
            missionTrackerRoot = list.transform;
            VerticalLayoutGroup layout = list.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 6f;
            layout.padding = new RectOffset(6, 6, 6, 6);

            Button openButton = FindButton("MissionTrackerOpenButton");
            if (openButton != null)
            {
                openButton.onClick.RemoveListener(ToggleMissions);
                openButton.onClick.AddListener(ToggleMissions);
            }

            HudSkinController skinController = GetComponent<HudSkinController>();
            if (skinController != null)
            {
                skinController.Apply();
            }
        }

        private static string BuildMissionTitle(MissionDefinition definition)
        {
            if (!string.IsNullOrWhiteSpace(definition.DisplayName))
            {
                return definition.DisplayName;
            }

            return definition.MissionId;
        }

        private static string BuildProgressText(MissionRuntimeState state)
        {
            if (state.RewardClaimed)
            {
                return "Claimed";
            }

            return "Progress " + state.Progress + "/" + state.RequiredCount;
        }

        private static string BuildClaimLabel(MissionRuntimeState state)
        {
            if (state.RewardClaimed)
            {
                return "Claimed";
            }

            return state.CanClaim ? "Claim" : "Pending";
        }

        private static string BuildRewardText(RewardDefinition reward)
        {
            if (reward == null)
            {
                return "Reward unavailable";
            }

            List<string> parts = new List<string>();
            if (reward.Currencies != null)
            {
                for (int i = 0; i < reward.Currencies.Length; i++)
                {
                    if (reward.Currencies[i] != null && reward.Currencies[i].Amount > 0)
                    {
                        parts.Add(reward.Currencies[i].CurrencyKind + " " + reward.Currencies[i].Amount);
                    }
                }
            }

            if (reward.Abilities != null)
            {
                for (int i = 0; i < reward.Abilities.Length; i++)
                {
                    if (reward.Abilities[i] != null && reward.Abilities[i].Count > 0)
                    {
                        parts.Add(reward.Abilities[i].AbilityKind + " x" + reward.Abilities[i].Count);
                    }
                }
            }

            if (reward.DecorationIds != null && reward.DecorationIds.Length > 0)
            {
                parts.Add("Decoration");
            }

            if (reward.PlantTierUnlocks != null && reward.PlantTierUnlocks.Length > 0)
            {
                parts.Add("Unlock");
            }

            return parts.Count > 0 ? string.Join(", ", parts) : "Reward unavailable";
        }

        private static string BuildClaimMessage(MissionClaimStatus status)
        {
            switch (status)
            {
                case MissionClaimStatus.Claimed:
                    return "Mission reward claimed";
                case MissionClaimStatus.NotComplete:
                    return "Mission not complete";
                case MissionClaimStatus.AlreadyClaimed:
                    return "Already claimed";
                case MissionClaimStatus.RewardUnavailable:
                    return "Reward unavailable";
                case MissionClaimStatus.NotFound:
                    return "Mission not found";
                default:
                    return "Cannot claim";
            }
        }

        private void PlayMessage(string message)
        {
            if (gameplayFeedbackController != null)
            {
                gameplayFeedbackController.PlayHudMessage(message);
            }
        }

        private static Button FindButton(string objectName)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            return gameObject != null ? gameObject.GetComponent<Button>() : null;
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

        private static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject panel = CreateRect(name, parent, anchorMin, anchorMax);
            Image image = panel.AddComponent<Image>();
            image.sprite = PlaceholderSpriteFactory.HudPanelSprite;
            image.color = new Color(0.12f, 0.16f, 0.18f, 0.97f);
            return panel;
        }

        private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObject = CreateRect(name, parent, anchorMin, anchorMax);
            Image image = buttonObject.AddComponent<Image>();
            image.sprite = PlaceholderSpriteFactory.HudButtonSprite;
            image.color = Color.white;
            buttonObject.AddComponent<Button>();
            CreateText("Label", buttonObject.transform, label, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 22);
            return buttonObject;
        }

        private static GameObject CreateText(string name, Transform parent, string content, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, int fontSize)
        {
            GameObject textObject = CreateRect(name, parent, anchorMin, anchorMax);
            Text text = textObject.AddComponent<Text>();
            text.text = content;
            text.alignment = alignment;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 12;
            text.resizeTextMaxSize = fontSize;
            text.color = Color.white;
            text.raycastTarget = false;
            return textObject;
        }

        private static GameObject CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            return gameObject;
        }
    }
}
