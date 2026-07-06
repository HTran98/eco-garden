using EcoGarden.Audio;
using EcoGarden.Shop;
using EcoGarden.Abilities;
using System.Collections.Generic;
using EcoGarden.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class ShopUiController : MonoBehaviour
    {
        [SerializeField] private Button shopButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private Text shopSummaryText;
        [SerializeField] private Transform categoryRoot;
        [SerializeField] private Transform productListRoot;
        [SerializeField] private GameplayFeedbackController gameplayFeedbackController;

        private ShopController shopController;
        private ShopController subscribedShopController;
        private ShopItemCategory selectedCategory = ShopItemCategory.Booster;
        private readonly List<GameObject> productRows = new List<GameObject>();
        private readonly HashSet<string> pendingProductIds = new HashSet<string>();
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
            SubscribeShopEvents();
            RefreshProducts();
        }

        private void OnDisable()
        {
            UnsubscribeShopEvents();
        }

        public void ToggleShop()
        {
            ResolveReferences();
            WireButtons();
            SetPanelVisible(shopPanel == null || !shopPanel.activeSelf);
        }

        public void CloseShop()
        {
            SetPanelVisible(false);
        }

        public void SelectCategory(ShopItemCategory category)
        {
            selectedCategory = category;
            RefreshProducts();
        }

        private void SetPanelVisible(bool visible)
        {
            if (visible)
            {
                UiModalPanelUtility.HideOtherModalPanels("ShopPanel");
            }

            if (shopPanel != null)
            {
                shopPanel.SetActive(visible);
                if (visible)
                {
                    UiModalPanelUtility.RaiseModalPanel(shopPanel);
                }
            }

            if (visible)
            {
                RefreshProducts();
            }
        }

        private void RefreshProducts()
        {
            ResolveReferences();
            RefreshCategoryTabs();
            ClearProductRows();

            if (shopController == null || productListRoot == null)
            {
                return;
            }

            EnsureSelectedCategoryHasItems();
            RefreshCategoryTabs();

            List<ShopItemDefinition> items = shopController.Catalog.GetItemsByCategory(selectedCategory);
            RefreshShopSummary(items);
            for (int i = 0; i < items.Count; i++)
            {
                CreateProductRow(items[i]);
            }
        }

        private void CreateProductRow(ShopItemDefinition item)
        {
            if (item == null)
            {
                return;
            }

            GameObject row = new GameObject("ShopProduct_" + item.ProductId, typeof(RectTransform), typeof(Image));
            row.transform.SetParent(productListRoot, false);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0f, ShopUiLayoutMetrics.ProductRowHeight);

            Image rowImage = row.GetComponent<Image>();
            rowImage.sprite = LoadSprite("UiSkins/ui_row_light") ?? PlaceholderSpriteFactory.ShopProductRowSprite;
            rowImage.color = Color.white;
            UiRowAccent.Apply(row.transform, GetShopRowAccentColor(item));

            GameObject iconBadge = CreateImage("TypeBadge", row.transform, PlaceholderSpriteFactory.ShopIconBadgeSprite, GetCategoryAccentColor(item.Category), ShopUiLayoutMetrics.TypeBadgeAnchorMin, ShopUiLayoutMetrics.TypeBadgeAnchorMax);
            Sprite itemIcon = item.Icon != null ? item.Icon : LoadSprite(GetShopIconPath(item));
            if (itemIcon != null)
            {
                CreateImage("ItemIcon", iconBadge.transform, itemIcon, Color.white, new Vector2(0.14f, 0.14f), new Vector2(0.86f, 0.86f));
            }
            else
            {
                CreateText("Type", iconBadge.transform, GetCategoryShortName(item.Category), TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 22);
            }
            GameObject statusBadge = CreateImage("StatusBadge", row.transform, PlaceholderSpriteFactory.ShopIconBadgeSprite, GetStatusColor(item), ShopUiLayoutMetrics.StatusAnchorMin, ShopUiLayoutMetrics.StatusAnchorMax);
            CreateText("Status", statusBadge.transform, BuildStatusLabel(item), TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 15);

            CreateText("Name", row.transform, item.DisplayName, TextAnchor.MiddleLeft, ShopUiLayoutMetrics.NameAnchorMin, ShopUiLayoutMetrics.NameAnchorMax, 25);
            CreateText("Description", row.transform, BuildDescription(item), TextAnchor.UpperLeft, ShopUiLayoutMetrics.DescriptionAnchorMin, ShopUiLayoutMetrics.DescriptionAnchorMax, 18);
            Text effectText = CreateText("Effect", row.transform, BuildEffectText(item), TextAnchor.MiddleLeft, ShopUiLayoutMetrics.EffectAnchorMin, ShopUiLayoutMetrics.EffectAnchorMax, 17).GetComponent<Text>();
            effectText.color = UiThemePalette.TextMuted;

            GameObject priceBadge = CreateImage("PriceBadge", row.transform, GetPriceBadgeSprite(item), GetPriceColor(item), ShopUiLayoutMetrics.PriceAnchorMin, ShopUiLayoutMetrics.PriceAnchorMax);
            CreateText("Price", priceBadge.transform, BuildPriceText(item), TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 20);

            GameObject buyObject = CreateButton("BuyButton", row.transform, BuildBuyLabel(item), ShopUiLayoutMetrics.BuyAnchorMin, ShopUiLayoutMetrics.BuyAnchorMax);
            Button buyButton = buyObject.GetComponent<Button>();
            buyButton.interactable = CanBuy(item);
            Image buyImage = buyObject.GetComponent<Image>();
            if (buyImage != null && !buyButton.interactable)
            {
                buyImage.color = UiThemePalette.DisabledButton;
            }

            buyButton.onClick.AddListener(() => TryBuy(item.ProductId));

            productRows.Add(row);
        }

        private void TryBuy(string productId)
        {
            if (shopController == null)
            {
                PlayMessage("Shop unavailable");
                return;
            }

            ShopPurchaseResult result = shopController.TryPurchase(productId);
            EcoGardenAudioController.Instance?.PlayShopPurchase(result.Status);
            TrackPurchaseState(productId, result);
            PlayMessage(BuildPurchaseMessage(result));
            RefreshProducts();
        }

        private static string BuildPriceText(ShopItemDefinition item)
        {
            if (item == null || item.Price == null)
            {
                return "N/A";
            }

            if (item.Price.PurchaseKind == ShopPurchaseKind.Iap)
            {
                return "Store";
            }

            return item.Price.CurrencyKind + " " + item.Price.Amount;
        }

        private static string BuildDescription(ShopItemDefinition item)
        {
            if (!string.IsNullOrWhiteSpace(item.Description))
            {
                return item.Description;
            }

            return GetCategoryDisplayName(item.Category);
        }

        private string BuildBuyLabel(ShopItemDefinition item)
        {
            if (item == null || !item.IsValid)
            {
                return "Unavailable";
            }

            if (pendingProductIds.Contains(item.ProductId))
            {
                return "Pending";
            }

            if (!item.Repeatable && shopController != null && shopController.Inventory.IsProductPurchased(item.ProductId))
            {
                return "Owned";
            }

            if (item.Price.PurchaseKind == ShopPurchaseKind.Iap)
            {
                return "Store";
            }

            return "Buy";
        }

        private string BuildStatusLabel(ShopItemDefinition item)
        {
            if (item == null || !item.IsValid)
            {
                return "OFF";
            }

            if (pendingProductIds.Contains(item.ProductId))
            {
                return "WAIT";
            }

            if (!item.Repeatable && shopController != null && shopController.Inventory.IsProductPurchased(item.ProductId))
            {
                if (item.Category == ShopItemCategory.Decoration && item.Grant != null && item.Grant.DecorationIds != null)
                {
                    for (int i = 0; i < item.Grant.DecorationIds.Length; i++)
                    {
                        if (shopController.Inventory.IsDecorationActive(item.Grant.DecorationIds[i]))
                        {
                            return "ON";
                        }
                    }
                }

                return "OWN";
            }

            return item.Category == ShopItemCategory.Decoration ? "VIS" : "NEW";
        }

        private static string BuildEffectText(ShopItemDefinition item)
        {
            if (item == null)
            {
                return "No effect";
            }

            if (item.Category == ShopItemCategory.Decoration)
            {
                return "Visual: " + BuildDecorationEffectText(item);
            }

            if (item.Category == ShopItemCategory.Booster)
            {
                return "Tools added to booster bar";
            }

            if (item.Category == ShopItemCategory.Unlock)
            {
                return "Progression unlock";
            }

            if (item.Category == ShopItemCategory.Currency)
            {
                return "Adds Gems to wallet";
            }

            if (item.Category == ShopItemCategory.Bundle)
            {
                return "Bundle reward";
            }

            return "Reward item";
        }

        private static string BuildDecorationEffectText(ShopItemDefinition item)
        {
            if (item.Grant == null || item.Grant.DecorationIds == null || item.Grant.DecorationIds.Length == 0)
            {
                return "cosmetic";
            }

            string decorationId = item.Grant.DecorationIds[0];
            switch (decorationId)
            {
                case DecorationController.BoardMossStoneId:
                    return "board skin";
                case DecorationController.ButterflyVariantId:
                    return "butterfly color + extra butterfly";
                case DecorationController.BeeVisitorId:
                case DecorationController.LegacyBirdVisitorId:
                    return "ambient bee visitor";
                case DecorationController.NpcTravelerId:
                    return "customer NPC outfit";
                case DecorationController.BackgroundLilyPondId:
                    return "background skin";
                default:
                    return "cosmetic";
            }
        }

        private static string GetShopIconPath(ShopItemDefinition item)
        {
            if (item == null)
            {
                return null;
            }

            if (item.Category == ShopItemCategory.Decoration)
            {
                return GetDecorationIconPath(item);
            }

            if (item.Category == ShopItemCategory.Booster)
            {
                return GetBoosterIconPath(item);
            }

            if (item.Category == ShopItemCategory.Currency)
            {
                return item.Price != null && item.Price.PurchaseKind == ShopPurchaseKind.Gem
                    ? "UiIcons/icon_currency_gem"
                    : "UiIcons/icon_currency_gold";
            }

            if (item.Category == ShopItemCategory.Unlock)
            {
                return "UiIcons/icon_nav_level";
            }

            if (item.Category == ShopItemCategory.Bundle)
            {
                return "UiIcons/icon_shop_booster";
            }

            return "UiIcons/icon_nav_shop";
        }

        private static string GetBoosterIconPath(ShopItemDefinition item)
        {
            if (item.Grant != null && item.Grant.Abilities != null && item.Grant.Abilities.Length > 0)
            {
                switch (item.Grant.Abilities[0].AbilityKind)
                {
                    case AbilityKind.Shovel:
                        return "UiIcons/icon_ability_shovel";
                    case AbilityKind.MagicWand:
                        return "UiIcons/icon_ability_magic_wand";
                    case AbilityKind.SortingMagnet:
                        return "UiIcons/icon_ability_sorting_magnet";
                }
            }

            return "UiIcons/icon_shop_booster";
        }

        private static string GetDecorationIconPath(ShopItemDefinition item)
        {
            if (item.Grant == null || item.Grant.DecorationIds == null || item.Grant.DecorationIds.Length == 0)
            {
                return "UiIcons/icon_shop_decor";
            }

            switch (item.Grant.DecorationIds[0])
            {
                case DecorationController.BoardMossStoneId:
                    return "UiIcons/icon_decor_board";
                case DecorationController.ButterflyVariantId:
                    return "UiIcons/icon_decor_butterfly";
                case DecorationController.BeeVisitorId:
                case DecorationController.LegacyBirdVisitorId:
                    return "UiIcons/icon_decor_bee";
                case DecorationController.NpcTravelerId:
                    return "UiIcons/icon_decor_npc";
                case DecorationController.BackgroundLilyPondId:
                    return "UiIcons/icon_decor_background";
                default:
                    return "UiIcons/icon_shop_decor";
            }
        }

        private static string BuildPurchaseMessage(ShopPurchaseResult result)
        {
            switch (result.Status)
            {
                case ShopPurchaseStatus.Success:
                    return "Purchased";
                case ShopPurchaseStatus.Pending:
                    return "Purchase pending";
                case ShopPurchaseStatus.AlreadyOwned:
                    return "Already owned";
                case ShopPurchaseStatus.InsufficientCurrency:
                    return "Not enough currency";
                case ShopPurchaseStatus.UnsupportedPurchaseKind:
                    return "Store unavailable";
                case ShopPurchaseStatus.InvalidProduct:
                    return "Item unavailable";
                case ShopPurchaseStatus.IapCancelled:
                    return "Purchase cancelled";
                case ShopPurchaseStatus.IapFailed:
                    return "Purchase failed";
                case ShopPurchaseStatus.DuplicateTransaction:
                    return "Purchase already processed";
                case ShopPurchaseStatus.ProductNotFound:
                    return "Product not found";
                default:
                    return "Cannot buy";
            }
        }

        private bool CanBuy(ShopItemDefinition item)
        {
            if (item == null || !item.IsValid || pendingProductIds.Contains(item.ProductId))
            {
                return false;
            }

            return item.Repeatable || shopController == null || !shopController.Inventory.IsProductPurchased(item.ProductId);
        }

        private void TrackPurchaseState(string productId, ShopPurchaseResult result)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return;
            }

            if (result.Status == ShopPurchaseStatus.Pending)
            {
                pendingProductIds.Add(productId);
                return;
            }

            pendingProductIds.Remove(productId);
        }

        private void OnIapPurchaseCompleted(ShopPurchaseResult result)
        {
            if (result.Item != null && !string.IsNullOrWhiteSpace(result.Item.ProductId))
            {
                pendingProductIds.Remove(result.Item.ProductId);
            }

            PlayMessage(BuildPurchaseMessage(result));
            EcoGardenAudioController.Instance?.PlayShopPurchase(result.Status);
            RefreshProducts();
        }

        private void SubscribeShopEvents()
        {
            if (ReferenceEquals(subscribedShopController, shopController))
            {
                return;
            }

            UnsubscribeShopEvents();

            if (shopController != null)
            {
                shopController.IapPurchaseCompleted += OnIapPurchaseCompleted;
                subscribedShopController = shopController;
            }
        }

        private void UnsubscribeShopEvents()
        {
            if (subscribedShopController != null)
            {
                subscribedShopController.IapPurchaseCompleted -= OnIapPurchaseCompleted;
                subscribedShopController = null;
            }
        }

        private void ClearProductRows()
        {
            for (int i = 0; i < productRows.Count; i++)
            {
                if (productRows[i] != null)
                {
                    Destroy(productRows[i]);
                }
            }

            productRows.Clear();
        }

        private void ResolveReferences()
        {
            if (shopController == null)
            {
                shopController = FindAnyObjectByType<ShopController>();
                SubscribeShopEvents();
            }

            if (gameplayFeedbackController == null)
            {
                gameplayFeedbackController = FindAnyObjectByType<GameplayFeedbackController>();
            }

            if (shopButton == null)
            {
                shopButton = FindButton("ShopButton");
            }

            if (closeButton == null)
            {
                closeButton = FindButton("ShopCloseButton");
            }

            if (shopPanel == null)
            {
                shopPanel = FindObjectIncludingInactive("ShopPanel");
            }

            if (shopPanel == null)
            {
                CreateRuntimeShopPanel();
            }

            if (categoryRoot == null)
            {
                GameObject categoryObject = FindObjectIncludingInactive("ShopCategoryBar");
                categoryRoot = categoryObject != null ? categoryObject.transform : null;
            }

            if (shopSummaryText == null)
            {
                GameObject summaryObject = FindObjectIncludingInactive("ShopSummaryText");
                shopSummaryText = summaryObject != null ? summaryObject.GetComponent<Text>() : null;
            }

            if (shopSummaryText == null && shopPanel != null)
            {
                shopSummaryText = CreateText(
                    "ShopSummaryText",
                    shopPanel.transform,
                    string.Empty,
                    TextAnchor.MiddleLeft,
                    ShopUiLayoutMetrics.SummaryAnchorMin,
                    ShopUiLayoutMetrics.SummaryAnchorMax,
                    22).GetComponent<Text>();
            }

            if (productListRoot == null)
            {
                GameObject listObject = FindObjectIncludingInactive("ShopProductList");
                productListRoot = listObject != null ? listObject.transform : null;
            }
        }

        private void WireButtons()
        {
            ResolveButtonReferencesOnly();

            if (shopButton != null)
            {
                shopButton.onClick.RemoveListener(ToggleShop);
                shopButton.onClick.AddListener(ToggleShop);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(CloseShop);
                closeButton.onClick.AddListener(CloseShop);
            }

            WireCategoryButton("ShopCategoryBoosterButton", ShopItemCategory.Booster);
            WireCategoryButton("ShopCategoryDecorationButton", ShopItemCategory.Decoration);
            WireCategoryButton("ShopCategoryUnlockButton", ShopItemCategory.Unlock);
            WireCategoryButton("ShopCategoryCurrencyButton", ShopItemCategory.Currency);
            WireCategoryButton("ShopCategoryBundleButton", ShopItemCategory.Bundle);
            buttonsWired = shopButton != null;
        }

        private void ResolveButtonReferencesOnly()
        {
            if (shopButton == null)
            {
                shopButton = FindButton("ShopButton");
            }

            if (closeButton == null)
            {
                closeButton = FindButton("ShopCloseButton");
            }
        }

        private void Update()
        {
            if (!buttonsWired)
            {
                ResolveReferences();
                WireButtons();
            }
        }

        private void WireCategoryButton(string objectName, ShopItemCategory category)
        {
            Button button = FindButton(objectName);
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectCategory(category));
        }

        private void RefreshCategoryTabs()
        {
            SetCategoryTab("ShopCategoryBoosterButton", ShopItemCategory.Booster);
            SetCategoryTab("ShopCategoryDecorationButton", ShopItemCategory.Decoration);
            SetCategoryTab("ShopCategoryUnlockButton", ShopItemCategory.Unlock);
            SetCategoryTab("ShopCategoryCurrencyButton", ShopItemCategory.Currency);
            SetCategoryTab("ShopCategoryBundleButton", ShopItemCategory.Bundle);
        }

        private void SetCategoryTab(string objectName, ShopItemCategory category)
        {
            Button button = FindButton(objectName);
            if (button == null)
            {
                return;
            }

            Image image = button.GetComponent<Image>();
            bool hasItems = CategoryHasItems(category);
            button.interactable = hasItems;

            if (image == null)
            {
                return;
            }

            image.sprite = PlaceholderSpriteFactory.HudButtonSprite;
            if (!hasItems)
            {
                image.sprite = LoadSprite("UiSkins/ui_button_disabled") ?? PlaceholderSpriteFactory.HudButtonSprite;
                image.color = UiThemePalette.DisabledButton;
                return;
            }

            image.sprite = LoadSprite(selectedCategory == category ? "UiSkins/ui_button_secondary" : "UiSkins/ui_button_primary")
                ?? PlaceholderSpriteFactory.HudButtonSprite;
            image.color = selectedCategory == category
                ? GetCategoryAccentColor(category)
                : UiThemePalette.PanelMuted;
        }

        private void EnsureSelectedCategoryHasItems()
        {
            if (CategoryHasItems(selectedCategory))
            {
                return;
            }

            if (CategoryHasItems(ShopItemCategory.Booster))
            {
                selectedCategory = ShopItemCategory.Booster;
                return;
            }

            if (CategoryHasItems(ShopItemCategory.Unlock))
            {
                selectedCategory = ShopItemCategory.Unlock;
                return;
            }

            if (CategoryHasItems(ShopItemCategory.Currency))
            {
                selectedCategory = ShopItemCategory.Currency;
                return;
            }

            if (CategoryHasItems(ShopItemCategory.Bundle))
            {
                selectedCategory = ShopItemCategory.Bundle;
            }
        }

        private bool CategoryHasItems(ShopItemCategory category)
        {
            return shopController != null && shopController.Catalog.GetItemsByCategory(category).Count > 0;
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

        private void CreateRuntimeShopPanel()
        {
            Transform parent = transform;
            GameObject panel = CreatePanel("ShopPanel", parent, AndroidHudLayoutMetrics.PanelAnchorMin, AndroidHudLayoutMetrics.PanelAnchorMax);
            shopPanel = panel;

            CreateText("ShopTitleText", panel.transform, "Shop", TextAnchor.MiddleLeft, PanelUiLayoutMetrics.TitleAnchorMin, PanelUiLayoutMetrics.TitleAnchorMax, 34);
            CreateButton("ShopCloseButton", panel.transform, "X", PanelUiLayoutMetrics.CloseAnchorMin, PanelUiLayoutMetrics.CloseAnchorMax);

            GameObject categoryBar = CreateRect("ShopCategoryBar", panel.transform, PanelUiLayoutMetrics.ShopCategoryAnchorMin, PanelUiLayoutMetrics.ShopCategoryAnchorMax);
            categoryRoot = categoryBar.transform;
            CreateButton("ShopCategoryBoosterButton", categoryRoot, "Boost", new Vector2(0.00f, 0.05f), new Vector2(0.19f, 0.95f));
            CreateButton("ShopCategoryDecorationButton", categoryRoot, "Decor", new Vector2(0.205f, 0.05f), new Vector2(0.395f, 0.95f));
            CreateButton("ShopCategoryUnlockButton", categoryRoot, "Unlock", new Vector2(0.41f, 0.05f), new Vector2(0.60f, 0.95f));
            CreateButton("ShopCategoryCurrencyButton", categoryRoot, "Gem", new Vector2(0.615f, 0.05f), new Vector2(0.805f, 0.95f));
            CreateButton("ShopCategoryBundleButton", categoryRoot, "Bundle", new Vector2(0.82f, 0.05f), new Vector2(1f, 0.95f));

            shopSummaryText = CreateText("ShopSummaryText", panel.transform, string.Empty, TextAnchor.MiddleLeft, ShopUiLayoutMetrics.SummaryAnchorMin, ShopUiLayoutMetrics.SummaryAnchorMax, 22).GetComponent<Text>();

            GameObject viewport = CreatePanel("ShopProductViewport", panel.transform, ShopUiLayoutMetrics.ContentAnchorMin, ShopUiLayoutMetrics.ContentAnchorMax);
            Image viewportImage = viewport.GetComponent<Image>();
            if (viewportImage != null)
            {
                viewportImage.sprite = LoadSprite("UiSkins/ui_row_light") ?? PlaceholderSpriteFactory.HudPanelSprite;
                viewportImage.color = UiThemePalette.PanelMuted;
            }

            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            GameObject list = CreateRect("ShopProductList", viewport.transform, new Vector2(0f, 1f), new Vector2(1f, 1f));
            productListRoot = list.transform;
            RectTransform listRect = list.GetComponent<RectTransform>();
            listRect.pivot = new Vector2(0.5f, 1f);
            VerticalLayoutGroup layout = list.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 12f;
            layout.padding = new RectOffset(14, 14, 14, 14);
            ContentSizeFitter fitter = list.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scrollRect = viewport.AddComponent<ScrollRect>();
            scrollRect.content = listRect;
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            HudSkinController skinController = GetComponent<HudSkinController>();
            if (skinController != null)
            {
                skinController.Apply();
            }

            panel.SetActive(false);
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject panel = CreateRect(name, parent, anchorMin, anchorMax);
            Image image = panel.AddComponent<Image>();
            Sprite resourceSprite = LoadSprite(name == "ShopPanel" ? "UiSkins/ui_panel_light" : "UiSkins/ui_row_light");
            image.sprite = resourceSprite ?? (name == "ShopPanel" ? PlaceholderSpriteFactory.ShopPanelSprite : PlaceholderSpriteFactory.HudPanelSprite);
            image.color = resourceSprite != null ? Color.white : (name == "ShopPanel" ? UiThemePalette.Panel : UiThemePalette.PanelMuted);
            return panel;
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

        private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObject = CreateRect(name, parent, anchorMin, anchorMax);
            Image image = buttonObject.AddComponent<Image>();
            image.sprite = LoadSprite("UiSkins/ui_button_primary") ?? PlaceholderSpriteFactory.HudButtonSprite;
            image.color = UiThemePalette.PrimaryButton;
            Button button = buttonObject.AddComponent<Button>();
            button.colors = UiThemePalette.BuildButtonColors(UiThemePalette.PrimaryButton);
            buttonObject.AddComponent<UiButtonFeedback>();
            CreateText("Label", buttonObject.transform, label, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 22);
            return buttonObject;
        }

        private static GameObject CreateImage(string name, Transform parent, Sprite sprite, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject imageObject = CreateRect(name, parent, anchorMin, anchorMax);
            Image image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
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

        private static string GetCategoryShortName(ShopItemCategory category)
        {
            switch (category)
            {
                case ShopItemCategory.Booster:
                    return "BST";
                case ShopItemCategory.Decoration:
                    return "DEC";
                case ShopItemCategory.Unlock:
                    return "UNL";
                case ShopItemCategory.Currency:
                    return "GEM";
                case ShopItemCategory.Bundle:
                    return "BND";
                default:
                    return "ITEM";
            }
        }

        private static string GetCategoryDisplayName(ShopItemCategory category)
        {
            switch (category)
            {
                case ShopItemCategory.Booster:
                    return "Support item";
                case ShopItemCategory.Decoration:
                    return "Decoration";
                case ShopItemCategory.Unlock:
                    return "Unlock";
                case ShopItemCategory.Currency:
                    return "Currency pack";
                case ShopItemCategory.Bundle:
                    return "Bundle";
                default:
                    return "Shop item";
            }
        }

        private void RefreshShopSummary(IReadOnlyList<ShopItemDefinition> visibleItems)
        {
            if (shopSummaryText == null)
            {
                return;
            }

            int visibleCount = visibleItems != null ? visibleItems.Count : 0;
            int storeCount = 0;
            int ownedCount = 0;
            if (visibleItems != null)
            {
                for (int i = 0; i < visibleItems.Count; i++)
                {
                    ShopItemDefinition item = visibleItems[i];
                    if (item == null)
                    {
                        continue;
                    }

                    if (item.Price != null && item.Price.PurchaseKind == ShopPurchaseKind.Iap)
                    {
                        storeCount++;
                    }

                    if (!item.Repeatable && shopController != null && shopController.Inventory.IsProductPurchased(item.ProductId))
                    {
                        ownedCount++;
                    }
                }
            }

            shopSummaryText.text = GetCategoryDisplayName(selectedCategory) + "  /  Items " + visibleCount + "  /  Store " + storeCount + "  /  Owned " + ownedCount;
        }

        private static Color GetCategoryAccentColor(ShopItemCategory category)
        {
            switch (category)
            {
                case ShopItemCategory.Booster:
                    return new Color(0.36f, 0.73f, 0.72f, 1f);
                case ShopItemCategory.Decoration:
                    return UiThemePalette.Gem;
                case ShopItemCategory.Unlock:
                    return UiThemePalette.Selected;
                case ShopItemCategory.Currency:
                    return UiThemePalette.Gem;
                case ShopItemCategory.Bundle:
                    return UiThemePalette.SecondaryButton;
                default:
                    return Color.white;
            }
        }

        private Color GetShopRowAccentColor(ShopItemDefinition item)
        {
            if (item == null || !CanBuy(item))
            {
                return UiThemePalette.DisabledButton;
            }

            if (item.Price != null && item.Price.PurchaseKind == ShopPurchaseKind.Iap)
            {
                return UiThemePalette.Store;
            }

            return GetCategoryAccentColor(item.Category);
        }

        private Color GetStatusColor(ShopItemDefinition item)
        {
            if (item == null || !item.IsValid)
            {
                return UiThemePalette.DisabledButton;
            }

            if (!item.Repeatable && shopController != null && shopController.Inventory.IsProductPurchased(item.ProductId))
            {
                return UiThemePalette.Success;
            }

            if (item.Category == ShopItemCategory.Decoration)
            {
                return UiThemePalette.Gem;
            }

            return UiThemePalette.SecondaryButton;
        }

        private static Color GetPriceColor(ShopItemDefinition item)
        {
            if (item == null || item.Price == null)
            {
                return UiThemePalette.DisabledButton;
            }

            if (item.Price.PurchaseKind == ShopPurchaseKind.Iap)
            {
                return UiThemePalette.Store;
            }

            if (item.Price.PurchaseKind == ShopPurchaseKind.Gem)
            {
                return UiThemePalette.Gem;
            }

            return UiThemePalette.Gold;
        }

        private static Sprite GetPriceBadgeSprite(ShopItemDefinition item)
        {
            if (item == null || item.Price == null)
            {
                return LoadSprite("UiSkins/ui_button_disabled") ?? PlaceholderSpriteFactory.ShopPriceBadgeSprite;
            }

            if (item.Price.PurchaseKind == ShopPurchaseKind.Iap)
            {
                return LoadSprite("UiSkins/ui_badge_store") ?? PlaceholderSpriteFactory.ShopPriceBadgeSprite;
            }

            if (item.Price.PurchaseKind == ShopPurchaseKind.Gem)
            {
                return LoadSprite("UiSkins/ui_badge_gem") ?? PlaceholderSpriteFactory.ShopPriceBadgeSprite;
            }

            return LoadSprite("UiSkins/ui_badge_gold") ?? PlaceholderSpriteFactory.ShopPriceBadgeSprite;
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
    }
}
