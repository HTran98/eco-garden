using EcoGarden.Shop;
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
        [SerializeField] private Transform categoryRoot;
        [SerializeField] private Transform productListRoot;
        [SerializeField] private GameplayFeedbackController gameplayFeedbackController;

        private ShopController shopController;
        private ShopItemCategory selectedCategory = ShopItemCategory.Booster;
        private readonly List<GameObject> productRows = new List<GameObject>();
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
            RefreshProducts();
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
            if (shopPanel != null)
            {
                shopPanel.SetActive(visible);
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

            List<ShopItemDefinition> items = shopController.Catalog.GetItemsByCategory(selectedCategory);
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
            rowRect.sizeDelta = new Vector2(0f, 124f);

            Image rowImage = row.GetComponent<Image>();
            rowImage.sprite = PlaceholderSpriteFactory.ShopProductRowSprite;
            rowImage.color = Color.white;

            GameObject iconBadge = CreateImage("TypeBadge", row.transform, PlaceholderSpriteFactory.ShopIconBadgeSprite, GetCategoryAccentColor(item.Category), new Vector2(0.025f, 0.18f), new Vector2(0.17f, 0.82f));
            CreateText("Type", iconBadge.transform, GetCategoryShortName(item.Category), TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 22);

            CreateText("Name", row.transform, item.DisplayName, TextAnchor.MiddleLeft, new Vector2(0.20f, 0.56f), new Vector2(0.63f, 0.91f), 25);
            CreateText("Description", row.transform, BuildDescription(item), TextAnchor.UpperLeft, new Vector2(0.20f, 0.18f), new Vector2(0.63f, 0.56f), 18);

            GameObject priceBadge = CreateImage("PriceBadge", row.transform, PlaceholderSpriteFactory.ShopPriceBadgeSprite, GetPriceColor(item), new Vector2(0.66f, 0.56f), new Vector2(0.96f, 0.90f));
            CreateText("Price", priceBadge.transform, BuildPriceText(item), TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, 20);

            GameObject buyObject = CreateButton("BuyButton", row.transform, BuildBuyLabel(item), new Vector2(0.66f, 0.13f), new Vector2(0.96f, 0.49f));
            Button buyButton = buyObject.GetComponent<Button>();
            buyButton.interactable = item.Repeatable || !shopController.Inventory.IsProductPurchased(item.ProductId);
            Image buyImage = buyObject.GetComponent<Image>();
            if (buyImage != null && !buyButton.interactable)
            {
                buyImage.color = new Color(0.42f, 0.48f, 0.48f, 0.92f);
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
            PlayMessage(BuildPurchaseMessage(result));
            RefreshProducts();
        }

        private static string BuildPriceText(ShopItemDefinition item)
        {
            if (item.Price.PurchaseKind == ShopPurchaseKind.Iap)
            {
                return "IAP";
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
            if (!item.Repeatable && shopController != null && shopController.Inventory.IsProductPurchased(item.ProductId))
            {
                return "Owned";
            }

            return "Buy";
        }

        private static string BuildPurchaseMessage(ShopPurchaseResult result)
        {
            switch (result.Status)
            {
                case ShopPurchaseStatus.Success:
                    return "Purchased";
                case ShopPurchaseStatus.AlreadyOwned:
                    return "Already owned";
                case ShopPurchaseStatus.InsufficientCurrency:
                    return "Not enough currency";
                case ShopPurchaseStatus.UnsupportedPurchaseKind:
                    return "IAP not available";
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
            if (image == null)
            {
                return;
            }

            image.sprite = PlaceholderSpriteFactory.HudButtonSprite;
            image.color = selectedCategory == category
                ? GetCategoryAccentColor(category)
                : new Color(0.76f, 0.88f, 0.84f, 0.82f);
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
            GameObject panel = CreatePanel("ShopPanel", parent, new Vector2(0.06f, 0.19f), new Vector2(0.94f, 0.82f));
            shopPanel = panel;

            CreateText("ShopTitleText", panel.transform, "Shop", TextAnchor.MiddleLeft, new Vector2(0.05f, 0.90f), new Vector2(0.55f, 0.98f), 34);
            CreateButton("ShopCloseButton", panel.transform, "X", new Vector2(0.86f, 0.90f), new Vector2(0.96f, 0.98f));

            GameObject categoryBar = CreateRect("ShopCategoryBar", panel.transform, new Vector2(0.04f, 0.80f), new Vector2(0.96f, 0.89f));
            categoryRoot = categoryBar.transform;
            CreateButton("ShopCategoryBoosterButton", categoryRoot, "Boost", new Vector2(0.00f, 0.05f), new Vector2(0.19f, 0.95f));
            CreateButton("ShopCategoryDecorationButton", categoryRoot, "Decor", new Vector2(0.205f, 0.05f), new Vector2(0.395f, 0.95f));
            CreateButton("ShopCategoryUnlockButton", categoryRoot, "Unlock", new Vector2(0.41f, 0.05f), new Vector2(0.60f, 0.95f));
            CreateButton("ShopCategoryCurrencyButton", categoryRoot, "Gem", new Vector2(0.615f, 0.05f), new Vector2(0.805f, 0.95f));
            CreateButton("ShopCategoryBundleButton", categoryRoot, "Bundle", new Vector2(0.82f, 0.05f), new Vector2(1f, 0.95f));

            GameObject viewport = CreatePanel("ShopProductViewport", panel.transform, new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.78f));
            Image viewportImage = viewport.GetComponent<Image>();
            if (viewportImage != null)
            {
                viewportImage.sprite = PlaceholderSpriteFactory.HudPanelSprite;
                viewportImage.color = new Color(0.06f, 0.09f, 0.10f, 0.70f);
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
            image.sprite = name == "ShopPanel" ? PlaceholderSpriteFactory.ShopPanelSprite : PlaceholderSpriteFactory.HudPanelSprite;
            image.color = name == "ShopPanel" ? Color.white : new Color(0.12f, 0.16f, 0.18f, 0.97f);
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
            image.sprite = PlaceholderSpriteFactory.HudButtonSprite;
            image.color = Color.white;
            buttonObject.AddComponent<Button>();
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
            text.color = Color.white;
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

        private static Color GetCategoryAccentColor(ShopItemCategory category)
        {
            switch (category)
            {
                case ShopItemCategory.Booster:
                    return new Color(0.38f, 0.78f, 0.82f, 1f);
                case ShopItemCategory.Decoration:
                    return new Color(0.78f, 0.70f, 0.96f, 1f);
                case ShopItemCategory.Unlock:
                    return new Color(0.82f, 0.92f, 0.48f, 1f);
                case ShopItemCategory.Currency:
                    return new Color(0.95f, 0.68f, 0.92f, 1f);
                case ShopItemCategory.Bundle:
                    return new Color(0.96f, 0.74f, 0.42f, 1f);
                default:
                    return Color.white;
            }
        }

        private static Color GetPriceColor(ShopItemDefinition item)
        {
            if (item.Price.PurchaseKind == ShopPurchaseKind.Iap)
            {
                return new Color(0.48f, 0.42f, 0.76f, 1f);
            }

            if (item.Price.PurchaseKind == ShopPurchaseKind.Gem)
            {
                return new Color(0.42f, 0.30f, 0.58f, 1f);
            }

            return new Color(0.58f, 0.42f, 0.18f, 1f);
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
