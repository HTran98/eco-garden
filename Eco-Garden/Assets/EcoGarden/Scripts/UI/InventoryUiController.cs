using EcoGarden.Abilities;
using EcoGarden.Audio;
using EcoGarden.Board;
using EcoGarden.Shop;
using EcoGarden.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class InventoryUiController : MonoBehaviour
    {
        [SerializeField] private Button bagButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Text inventorySummaryText;
        [SerializeField] private Transform itemListRoot;
        [SerializeField] private GameplayFeedbackController gameplayFeedbackController;

        private ShopController shopController;
        private BoardController boardController;
        private DecorationController decorationController;
        private readonly List<GameObject> rows = new List<GameObject>();
        private bool buttonsWired;
        private bool subscribed;

        private static readonly DecorationEntry[] DecorationEntries =
        {
            new DecorationEntry(DecorationController.BoardMossStoneId, "Moss Board", "Board skin", "UiIcons/icon_decor_board"),
            new DecorationEntry(DecorationController.ButterflyVariantId, "Butterfly Set", "Butterfly color + visitor", "UiIcons/icon_decor_butterfly"),
            new DecorationEntry(DecorationController.BeeVisitorId, "Bee Visitor", "Ambient bee", "UiIcons/icon_decor_bee"),
            new DecorationEntry(DecorationController.LegacyBirdVisitorId, "Bee Visitor", "Ambient bee", "UiIcons/icon_decor_bee"),
            new DecorationEntry(DecorationController.NpcTravelerId, "Traveler NPC", "Customer outfit", "UiIcons/icon_decor_npc"),
            new DecorationEntry(DecorationController.BackgroundLilyPondId, "Sunset Pond", "Background skin", "UiIcons/icon_decor_background")
        };

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
            Subscribe();
            SetPanelVisible(false);
        }

        private void OnEnable()
        {
            ResolveReferences();
            WireButtons();
            Subscribe();
            RefreshItems();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Update()
        {
            if (!buttonsWired)
            {
                ResolveReferences();
                WireButtons();
            }
        }

        public void ToggleInventory()
        {
            ResolveReferences();
            WireButtons();
            SetPanelVisible(inventoryPanel == null || !inventoryPanel.activeSelf);
        }

        public void CloseInventory()
        {
            SetPanelVisible(false);
        }

        private void SetPanelVisible(bool visible)
        {
            if (visible)
            {
                UiModalPanelUtility.HideOtherModalPanels("InventoryPanel");
            }

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(visible);
                if (visible)
                {
                    UiModalPanelUtility.RaiseModalPanel(inventoryPanel);
                }
            }

            if (visible)
            {
                RefreshItems();
            }
        }

        private void RefreshItems()
        {
            ResolveReferences();
            ClearRows();

            if (itemListRoot == null)
            {
                return;
            }

            int boosterCount = CreateBoosterRows();
            int decorationCount = CreateDecorationRows();
            if (inventorySummaryText != null)
            {
                inventorySummaryText.text = "Boosters " + boosterCount + "  /  Decor " + decorationCount;
            }
        }

        private int CreateBoosterRows()
        {
            CreateBoosterRow(AbilityKind.Shovel, "Shovel", "Clear one blocker", "UiIcons/icon_ability_shovel");
            CreateBoosterRow(AbilityKind.MagicWand, "Magic Wand", "Upgrade one plant", "UiIcons/icon_ability_magic_wand");
            CreateBoosterRow(AbilityKind.SortingMagnet, "Sorting Magnet", "Collect matching items", "UiIcons/icon_ability_sorting_magnet");
            return 3;
        }

        private void CreateBoosterRow(AbilityKind abilityKind, string name, string detail, string iconPath)
        {
            int count = boardController != null && boardController.AbilityInventory != null
                ? boardController.AbilityInventory.GetCount(abilityKind)
                : 0;
            CreateItemRow(name, detail, "x" + count, "HUD", false, iconPath, null);
        }

        private int CreateDecorationRows()
        {
            if (shopController == null || shopController.Inventory == null)
            {
                return 0;
            }

            int count = 0;
            HashSet<string> shown = new HashSet<string>();
            for (int i = 0; i < DecorationEntries.Length; i++)
            {
                DecorationEntry entry = DecorationEntries[i];
                if (!shopController.Inventory.IsDecorationOwned(entry.DecorationId) || !shown.Add(entry.DisplayName))
                {
                    continue;
                }

                bool active = shopController.Inventory.IsDecorationActive(entry.DecorationId);
                string action = active ? "Using" : "Use";
                CreateItemRow(entry.DisplayName, entry.Detail, string.Empty, action, !active, entry.IconPath, () => UseDecoration(entry.DecorationId));
                count++;
            }

            if (count == 0)
            {
                CreateItemRow("No decor yet", "Buy decor in the shop", string.Empty, "Shop", false, "UiIcons/icon_shop_decor", null);
            }

            return count;
        }

        private void UseDecoration(string decorationId)
        {
            if (shopController == null || !shopController.UseDecoration(decorationId))
            {
                EcoGardenAudioController.Instance?.PlayAbilityUnavailable();
                PlayMessage("Decor unavailable");
                RefreshItems();
                return;
            }

            if (decorationController != null)
            {
                decorationController.ApplyActiveDecorationsFromShop();
            }

            EcoGardenAudioController.Instance?.PlayDecorationApply();
            PlayMessage("Decor applied");
            RefreshItems();
        }

        private void CreateItemRow(string name, string detail, string countText, string actionText, bool actionEnabled, string iconPath, UnityEngine.Events.UnityAction action)
        {
            GameObject row = new GameObject("InventoryItem_" + name.Replace(" ", string.Empty), typeof(RectTransform), typeof(Image));
            row.transform.SetParent(itemListRoot, false);
            row.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, InventoryUiLayoutMetrics.ItemRowHeight);

            Image rowImage = row.GetComponent<Image>();
            rowImage.sprite = LoadSprite("UiSkins/ui_row_light") ?? PlaceholderSpriteFactory.ShopProductRowSprite;
            rowImage.color = Color.white;
            UiRowAccent.Apply(row.transform, actionEnabled ? UiThemePalette.Selected : UiThemePalette.PanelMuted);

            CreateImage("Icon", row.transform, LoadSprite(iconPath) ?? PlaceholderSpriteFactory.ShopIconBadgeSprite, Color.white, InventoryUiLayoutMetrics.IconAnchorMin, InventoryUiLayoutMetrics.IconAnchorMax);
            CreateText("Name", row.transform, name, TextAnchor.MiddleLeft, InventoryUiLayoutMetrics.NameAnchorMin, InventoryUiLayoutMetrics.NameAnchorMax, 24);
            Text detailText = CreateText("Detail", row.transform, detail, TextAnchor.MiddleLeft, InventoryUiLayoutMetrics.DetailAnchorMin, InventoryUiLayoutMetrics.DetailAnchorMax, 18).GetComponent<Text>();
            detailText.color = UiThemePalette.TextMuted;

            if (!string.IsNullOrWhiteSpace(countText))
            {
                CreateText("Count", row.transform, countText, TextAnchor.MiddleCenter, InventoryUiLayoutMetrics.CountAnchorMin, InventoryUiLayoutMetrics.CountAnchorMax, 22);
            }

            GameObject actionObject = CreateButton("ActionButton", row.transform, actionText, InventoryUiLayoutMetrics.ActionAnchorMin, InventoryUiLayoutMetrics.ActionAnchorMax);
            Button button = actionObject.GetComponent<Button>();
            button.interactable = actionEnabled;
            if (action != null)
            {
                button.onClick.AddListener(action);
            }

            Image actionImage = actionObject.GetComponent<Image>();
            if (actionImage != null && !actionEnabled)
            {
                actionImage.color = UiThemePalette.DisabledButton;
            }

            rows.Add(row);
        }

        private void ResolveReferences()
        {
            if (shopController == null)
            {
                shopController = FindAnyObjectByType<ShopController>();
            }

            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (decorationController == null)
            {
                decorationController = FindAnyObjectByType<DecorationController>();
            }

            if (gameplayFeedbackController == null)
            {
                gameplayFeedbackController = FindAnyObjectByType<GameplayFeedbackController>();
            }

            if (bagButton == null)
            {
                bagButton = FindButton("BagButton");
                if (bagButton == null)
                {
                    bagButton = CreateRuntimeBagButton();
                }
            }

            if (closeButton == null)
            {
                closeButton = FindButton("InventoryCloseButton");
            }

            if (inventoryPanel == null)
            {
                inventoryPanel = FindObjectIncludingInactive("InventoryPanel");
            }

            if (inventoryPanel == null)
            {
                CreateRuntimeInventoryPanel();
            }

            if (inventorySummaryText == null)
            {
                GameObject summaryObject = FindObjectIncludingInactive("InventorySummaryText");
                inventorySummaryText = summaryObject != null ? summaryObject.GetComponent<Text>() : null;
            }

            if (itemListRoot == null)
            {
                GameObject listObject = FindObjectIncludingInactive("InventoryItemList");
                itemListRoot = listObject != null ? listObject.transform : null;
            }
        }

        private void WireButtons()
        {
            if (bagButton == null)
            {
                bagButton = FindButton("BagButton");
                if (bagButton == null)
                {
                    bagButton = CreateRuntimeBagButton();
                }
            }

            if (closeButton == null)
            {
                closeButton = FindButton("InventoryCloseButton");
            }

            if (bagButton != null)
            {
                bagButton.onClick.RemoveListener(ToggleInventory);
                bagButton.onClick.AddListener(ToggleInventory);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(CloseInventory);
                closeButton.onClick.AddListener(CloseInventory);
            }

            buttonsWired = bagButton != null;
        }

        private void Subscribe()
        {
            if (subscribed)
            {
                return;
            }

            if (shopController != null && shopController.Inventory != null)
            {
                shopController.Inventory.Changed += RefreshItems;
            }

            if (boardController != null && boardController.AbilityInventory != null)
            {
                boardController.AbilityInventory.CountChanged += OnAbilityCountChanged;
            }

            subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!subscribed)
            {
                return;
            }

            if (shopController != null && shopController.Inventory != null)
            {
                shopController.Inventory.Changed -= RefreshItems;
            }

            if (boardController != null && boardController.AbilityInventory != null)
            {
                boardController.AbilityInventory.CountChanged -= OnAbilityCountChanged;
            }

            subscribed = false;
        }

        private void OnAbilityCountChanged(AbilityKind abilityKind, int count)
        {
            RefreshItems();
        }

        private void ClearRows()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] != null)
                {
                    Destroy(rows[i]);
                }
            }

            rows.Clear();
        }

        private void PlayMessage(string message)
        {
            if (gameplayFeedbackController != null)
            {
                gameplayFeedbackController.PlayHudMessage(message);
            }
        }

        private void CreateRuntimeInventoryPanel()
        {
            GameObject panel = CreatePanel("InventoryPanel", transform, AndroidHudLayoutMetrics.PanelAnchorMin, AndroidHudLayoutMetrics.PanelAnchorMax);
            inventoryPanel = panel;
            CreateText("InventoryTitleText", panel.transform, "Bag", TextAnchor.MiddleLeft, PanelUiLayoutMetrics.TitleAnchorMin, PanelUiLayoutMetrics.TitleAnchorMax, 34);
            CreateButton("InventoryCloseButton", panel.transform, UiIconLabelCatalog.Close, PanelUiLayoutMetrics.CloseAnchorMin, PanelUiLayoutMetrics.CloseAnchorMax);
            inventorySummaryText = CreateText("InventorySummaryText", panel.transform, string.Empty, TextAnchor.MiddleLeft, InventoryUiLayoutMetrics.SummaryAnchorMin, InventoryUiLayoutMetrics.SummaryAnchorMax, 22).GetComponent<Text>();

            GameObject viewport = CreatePanel("InventoryViewport", panel.transform, InventoryUiLayoutMetrics.ContentAnchorMin, InventoryUiLayoutMetrics.ContentAnchorMax);
            viewport.GetComponent<Image>().color = UiThemePalette.PanelMuted;
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            GameObject list = CreateRect("InventoryItemList", viewport.transform, new Vector2(0f, 1f), new Vector2(1f, 1f));
            itemListRoot = list.transform;
            RectTransform listRect = list.GetComponent<RectTransform>();
            listRect.pivot = new Vector2(0.5f, 1f);
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
            scrollRect.content = listRect;
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            panel.SetActive(false);
        }

        private static Button CreateRuntimeBagButton()
        {
            GameObject topBar = FindObjectIncludingInactive("TopBar");
            if (topBar == null)
            {
                return null;
            }

            GameObject buttonObject = CreateButton(
                "BagButton",
                topBar.transform,
                UiIconLabelCatalog.Bag,
                AndroidHudLayoutMetrics.BagButtonAnchorMin,
                AndroidHudLayoutMetrics.BagButtonAnchorMax);
            HudSkinController skinController = FindAnyObjectByType<HudSkinController>();
            if (skinController != null)
            {
                skinController.Apply();
            }

            return buttonObject.GetComponent<Button>();
        }

        private static Button FindButton(string objectName)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            return gameObject != null ? gameObject.GetComponent<Button>() : null;
        }

        private static GameObject FindObjectIncludingInactive(string objectName)
        {
            return UiModalPanelUtility.FindObjectIncludingInactive(objectName);
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject panel = CreateRect(name, parent, anchorMin, anchorMax);
            Image image = panel.AddComponent<Image>();
            Sprite sprite = LoadSprite(name == "InventoryPanel" ? "UiSkins/ui_panel_light" : "UiSkins/ui_row_light");
            image.sprite = sprite ?? PlaceholderSpriteFactory.HudPanelSprite;
            image.color = sprite != null ? Color.white : UiThemePalette.Panel;
            return panel;
        }

        private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObject = CreateRect(name, parent, anchorMin, anchorMax);
            Image image = buttonObject.AddComponent<Image>();
            image.sprite = LoadSprite("UiSkins/ui_button_primary") ?? PlaceholderSpriteFactory.HudButtonSprite;
            image.color = UiThemePalette.PrimaryButton;
            Button button = buttonObject.AddComponent<Button>();
            button.colors = UiThemePalette.BuildButtonColors(UiThemePalette.PrimaryButton);
            buttonObject.AddComponent<UiButtonFeedback>();
            CreateText("Label", buttonObject.transform, label, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 20);
            return buttonObject;
        }

        private static GameObject CreateImage(string name, Transform parent, Sprite sprite, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject imageObject = CreateRect(name, parent, anchorMin, anchorMax);
            Image image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.preserveAspect = true;
            image.raycastTarget = false;
            return imageObject;
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
            text.resizeTextMinSize = 14;
            text.resizeTextMaxSize = fontSize;
            text.color = UiThemePalette.TextDark;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return textObject;
        }

        private static Sprite LoadSprite(string resourcePath)
        {
            return string.IsNullOrWhiteSpace(resourcePath)
                ? null
                : Resources.Load<Sprite>(resourcePath);
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

        private readonly struct DecorationEntry
        {
            public DecorationEntry(string decorationId, string displayName, string detail, string iconPath)
            {
                DecorationId = decorationId;
                DisplayName = displayName;
                Detail = detail;
                IconPath = iconPath;
            }

            public string DecorationId { get; }
            public string DisplayName { get; }
            public string Detail { get; }
            public string IconPath { get; }
        }
    }
}
