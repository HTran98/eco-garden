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
        [SerializeField] private Text missionSummaryText;
        [SerializeField] private Transform missionListRoot;
        [SerializeField] private GameObject missionTrackerPanel;
        [SerializeField] private Transform missionTrackerRoot;
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private GameObject levelPanel;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private GameplayFeedbackController gameplayFeedbackController;

        private MissionController missionController;
        private readonly List<GameObject> missionRows = new List<GameObject>();
        private readonly List<GameObject> trackerRows = new List<GameObject>();
        private GameObject missionAlertBadge;
        private bool missionReadyMessageShown;
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
            if (visible)
            {
                UiModalPanelUtility.HideOtherModalPanels("MissionPanel");
            }

            if (missionPanel != null)
            {
                missionPanel.SetActive(visible);
                if (visible)
                {
                    UiModalPanelUtility.RaiseModalPanel(missionPanel);
                }
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
                RefreshMissionAlertBadge();
                return;
            }

            List<MissionRuntimeState> sortedMissions = new List<MissionRuntimeState>(missionController.Missions);
            sortedMissions.Sort(CompareMissionStates);
            for (int i = 0; i < sortedMissions.Count; i++)
            {
                CreateMissionRow(sortedMissions[i]);
            }

            RefreshMissionSummary(sortedMissions);
            RefreshTrackerRows();
            RefreshMissionAlertBadge();
        }

        private void RefreshTrackerRows()
        {
            ClearTrackerRows();
            if (missionTrackerPanel != null && missionTrackerPanel.activeSelf)
            {
                missionTrackerPanel.SetActive(false);
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
            rowRect.sizeDelta = new Vector2(0f, MissionUiLayoutMetrics.MissionRowHeight);

            Image rowImage = row.GetComponent<Image>();
            rowImage.sprite = Resources.Load<Sprite>("UiSkins/ui_row_light") ?? PlaceholderSpriteFactory.HudPanelSprite;
            rowImage.color = GetMissionRowColor(state);
            UiRowAccent.Apply(row.transform, GetMissionAccentColor(state));

            CreateText("Name", row.transform, BuildMissionTitle(definition), TextAnchor.MiddleLeft, MissionUiLayoutMetrics.RowTitleAnchorMin, MissionUiLayoutMetrics.RowTitleAnchorMax, 24);
            CreateText("Progress", row.transform, BuildProgressText(state), TextAnchor.MiddleLeft, MissionUiLayoutMetrics.RowProgressAnchorMin, MissionUiLayoutMetrics.RowProgressAnchorMax, 20);
            CreateText("Reward", row.transform, BuildRewardText(definition.Reward), TextAnchor.MiddleLeft, MissionUiLayoutMetrics.RowRewardAnchorMin, MissionUiLayoutMetrics.RowRewardAnchorMax, 18);

            GameObject claimObject = CreateButton("ClaimButton", row.transform, BuildClaimLabel(state), MissionUiLayoutMetrics.RowClaimAnchorMin, MissionUiLayoutMetrics.RowClaimAnchorMax);
            Button claimButton = claimObject.GetComponent<Button>();
            claimButton.interactable = state.CanClaim;
            ApplyClaimButtonState(claimObject, state);
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
            rowRect.sizeDelta = new Vector2(0f, MissionUiLayoutMetrics.TrackerRowHeight);

            Image rowImage = row.GetComponent<Image>();
            rowImage.sprite = Resources.Load<Sprite>("UiSkins/ui_row_light") ?? PlaceholderSpriteFactory.HudPanelSprite;
            rowImage.color = GetMissionRowColor(state);
            UiRowAccent.Apply(row.transform, GetMissionAccentColor(state));

            CreateText("Name", row.transform, BuildMissionTitle(definition), TextAnchor.MiddleLeft, MissionUiLayoutMetrics.TrackerTitleAnchorMin, MissionUiLayoutMetrics.TrackerTitleAnchorMax, 22);
            CreateText("Progress", row.transform, BuildTrackerProgressText(state), TextAnchor.MiddleLeft, MissionUiLayoutMetrics.TrackerProgressAnchorMin, MissionUiLayoutMetrics.TrackerProgressAnchorMax, 19);

            GameObject claimObject = CreateButton("ClaimButton", row.transform, BuildClaimLabel(state), MissionUiLayoutMetrics.TrackerClaimAnchorMin, MissionUiLayoutMetrics.TrackerClaimAnchorMax);
            Button claimButton = claimObject.GetComponent<Button>();
            claimButton.interactable = state.CanClaim;
            ApplyClaimButtonState(claimObject, state);
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

            if (missionSummaryText == null)
            {
                GameObject summaryObject = FindObjectIncludingInactive("MissionSummaryText");
                missionSummaryText = summaryObject != null ? summaryObject.GetComponent<Text>() : null;
            }

            if (missionSummaryText == null && missionPanel != null)
            {
                missionSummaryText = CreateText(
                    "MissionSummaryText",
                    missionPanel.transform,
                    string.Empty,
                    TextAnchor.MiddleLeft,
                    MissionUiLayoutMetrics.SummaryAnchorMin,
                    MissionUiLayoutMetrics.SummaryAnchorMax,
                    22).GetComponent<Text>();
            }

            if (missionTrackerPanel == null)
            {
                missionTrackerPanel = FindObjectIncludingInactive("MissionTrackerPanel");
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

            if (resultPanel == null)
            {
                resultPanel = FindObjectIncludingInactive("ResultPanel");
            }
        }

        private void WireButtons()
        {
            if (missionButton != null)
            {
                missionButton.onClick.RemoveListener(ToggleMissions);
                missionButton.onClick.AddListener(ToggleMissions);
                EnsureMissionAlertBadge();
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

            if (missionTrackerPanel.activeSelf)
            {
                missionTrackerPanel.SetActive(false);
            }
        }

        private void RefreshMissionAlertBadge()
        {
            bool hasClaimableMission = HasClaimableMission();
            EnsureMissionAlertBadge();

            if (missionAlertBadge != null)
            {
                missionAlertBadge.SetActive(hasClaimableMission);
            }

            if (hasClaimableMission && !missionReadyMessageShown)
            {
                PlayMessage("Mission complete. Claim reward from Missions.");
            }

            missionReadyMessageShown = hasClaimableMission;
        }

        private void RefreshMissionSummary(IReadOnlyList<MissionRuntimeState> missions)
        {
            if (missionSummaryText == null)
            {
                return;
            }

            int readyCount = 0;
            int activeCount = 0;
            int claimedCount = 0;
            for (int i = 0; i < missions.Count; i++)
            {
                MissionRuntimeState state = missions[i];
                if (state == null)
                {
                    continue;
                }

                if (state.RewardClaimed)
                {
                    claimedCount++;
                }
                else if (state.CanClaim)
                {
                    readyCount++;
                }
                else
                {
                    activeCount++;
                }
            }

            missionSummaryText.text = "Ready " + readyCount + "  /  Active " + activeCount + "  /  Claimed " + claimedCount;
        }

        private static int CompareMissionStates(MissionRuntimeState first, MissionRuntimeState second)
        {
            return GetMissionSortRank(first).CompareTo(GetMissionSortRank(second));
        }

        private static int GetMissionSortRank(MissionRuntimeState state)
        {
            if (state == null)
            {
                return int.MaxValue;
            }

            if (state.CanClaim)
            {
                return 0;
            }

            return state.RewardClaimed ? 2 : 1;
        }

        private bool HasClaimableMission()
        {
            if (missionController == null)
            {
                return false;
            }

            IReadOnlyList<MissionRuntimeState> missions = missionController.Missions;
            for (int i = 0; i < missions.Count; i++)
            {
                MissionRuntimeState state = missions[i];
                if (state != null && state.CanClaim)
                {
                    return true;
                }
            }

            return false;
        }

        private void EnsureMissionAlertBadge()
        {
            if (missionAlertBadge != null || missionButton == null)
            {
                return;
            }

            Transform existing = missionButton.transform.Find("MissionAlertBadge");
            if (existing != null)
            {
                missionAlertBadge = existing.gameObject;
                return;
            }

            missionAlertBadge = new GameObject("MissionAlertBadge", typeof(RectTransform), typeof(Image));
            missionAlertBadge.transform.SetParent(missionButton.transform, false);
            missionAlertBadge.transform.SetAsLastSibling();

            RectTransform rect = missionAlertBadge.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.one;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(-4f, -4f);
            rect.sizeDelta = new Vector2(26f, 26f);

            Image image = missionAlertBadge.GetComponent<Image>();
            image.sprite = PlaceholderSpriteFactory.SquareSprite;
            image.color = new Color(0.96f, 0.12f, 0.10f, 1f);
            image.raycastTarget = false;
            Text badgeText = CreateText("Label", missionAlertBadge.transform, "!", TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 18).GetComponent<Text>();
            badgeText.color = Color.white;
            missionAlertBadge.SetActive(false);
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
            missionSummaryText = CreateText("MissionSummaryText", panel.transform, string.Empty, TextAnchor.MiddleLeft, MissionUiLayoutMetrics.SummaryAnchorMin, MissionUiLayoutMetrics.SummaryAnchorMax, 22).GetComponent<Text>();

            GameObject viewport = CreatePanel("MissionViewport", panel.transform, MissionUiLayoutMetrics.ContentAnchorMin, MissionUiLayoutMetrics.ContentAnchorMax);
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

            if (state.CanClaim)
            {
                return "Ready to claim";
            }

            return "Progress: " + state.Progress + "/" + state.RequiredCount;
        }

        private static string BuildTrackerProgressText(MissionRuntimeState state)
        {
            if (state.CanClaim)
            {
                return "Ready";
            }

            return state.Progress + "/" + state.RequiredCount;
        }

        private static string BuildClaimLabel(MissionRuntimeState state)
        {
            if (state.RewardClaimed)
            {
                return "Done";
            }

            return state.CanClaim ? "Claim" : "Active";
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

            return parts.Count > 0 ? "Reward: " + string.Join(", ", parts) : "Reward unavailable";
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

        private static Color GetMissionRowColor(MissionRuntimeState state)
        {
            if (state.RewardClaimed)
            {
                return Color.Lerp(UiThemePalette.PanelMuted, UiThemePalette.DisabledButton, 0.35f);
            }

            if (state.CanClaim)
            {
                return Color.Lerp(UiThemePalette.PanelStrong, UiThemePalette.Success, 0.24f);
            }

            return UiThemePalette.Panel;
        }

        private static Color GetMissionAccentColor(MissionRuntimeState state)
        {
            if (state.RewardClaimed)
            {
                return UiThemePalette.DisabledButton;
            }

            if (state.CanClaim)
            {
                return UiThemePalette.Success;
            }

            return UiThemePalette.PrimaryButton;
        }

        private static void ApplyClaimButtonState(GameObject claimObject, MissionRuntimeState state)
        {
            Image image = claimObject.GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            if (state.CanClaim)
            {
                image.color = UiThemePalette.Success;
                return;
            }

            image.color = state.RewardClaimed
                ? UiThemePalette.DisabledButton
                : UiThemePalette.PrimaryButton;
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
            Sprite resourceSprite = Resources.Load<Sprite>("UiSkins/ui_panel_light");
            image.sprite = resourceSprite ?? PlaceholderSpriteFactory.HudPanelSprite;
            image.color = resourceSprite != null ? Color.white : UiThemePalette.Panel;
            return panel;
        }

        private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObject = CreateRect(name, parent, anchorMin, anchorMax);
            Image image = buttonObject.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>("UiSkins/ui_button_primary") ?? PlaceholderSpriteFactory.HudButtonSprite;
            image.color = UiThemePalette.PrimaryButton;
            Button button = buttonObject.AddComponent<Button>();
            button.colors = UiThemePalette.BuildButtonColors(UiThemePalette.PrimaryButton);
            buttonObject.AddComponent<UiButtonFeedback>();
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
            text.color = UiThemePalette.TextDark;
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
