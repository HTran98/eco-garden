using EcoGarden.Input;
using EcoGarden.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class HudSkinController : MonoBehaviour
    {
        private void Start()
        {
            Apply();
        }

        public void Apply()
        {
            SkinImage("TopBar", PlaceholderSpriteFactory.HudTopBarSprite, UiThemePalette.TopBar, true, "UiSkins/ui_panel_overlay");
            SkinImage("ObjectivePanel", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.Panel, true, "UiSkins/ui_panel_light");
            SkinImage("AbilityBar", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.PanelStrong, true, "UiSkins/ui_panel_strong");
            SkinImage("ResultPanel", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.PanelOverlay, true, "UiSkins/ui_panel_overlay");
            SkinImage("PausePanel", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.PanelOverlay, true, "UiSkins/ui_panel_overlay");
            SkinImage("LevelPanel", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.Panel, true, "UiSkins/ui_panel_light");
            SkinImage("LevelViewport", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.PanelMuted, true, "UiSkins/ui_row_light");
            SkinImage("LevelPreviewPanel", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.PanelOverlay, true, "UiSkins/ui_panel_overlay");
            SkinImage("InventoryPanel", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.Panel, true, "UiSkins/ui_panel_light");
            SkinImage("InventoryViewport", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.PanelMuted, true, "UiSkins/ui_row_light");
            SkinImage("ShopPanel", PlaceholderSpriteFactory.ShopPanelSprite, UiThemePalette.Panel, true, "UiSkins/ui_panel_light");
            SkinImage("ShopProductViewport", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.PanelMuted, true, "UiSkins/ui_row_light");
            SkinImage("MissionPanel", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.Panel, true, "UiSkins/ui_panel_light");
            SkinImage("MissionViewport", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.PanelMuted, true, "UiSkins/ui_row_light");
            SkinImage("MissionTrackerPanel", PlaceholderSpriteFactory.HudPanelSprite, UiThemePalette.PanelOverlay, true, "UiSkins/ui_panel_overlay");
            SkinDropZone("DeliveryDropZone", ExternalDropZoneKind.Delivery);
            SkinDropZone("SellBasket", ExternalDropZoneKind.SellBasket);

            SkinButton("PauseButton");
            SkinButton("LevelButton");
            SkinButton("LevelCloseButton");
            SkinButton("LevelPreviewPlayButton");
            SkinButton("LevelPreviewCloseButton");
            SkinButton("MissionButton");
            SkinButton("MissionCloseButton");
            SkinButton("MissionTrackerOpenButton");
            SkinButton("ShopButton");
            SkinButton("ShopCloseButton");
            SkinButton("BagButton");
            SkinButton("InventoryCloseButton");
            SkinButton("ShopCategoryBoosterButton");
            SkinButton("ShopCategoryDecorationButton");
            SkinButton("ShopCategoryUnlockButton");
            SkinButton("ShopCategoryCurrencyButton");
            SkinButton("ShopCategoryBundleButton");
            SkinButton("RestartButton");
            SkinButton("NextLevelButton");
            SkinButton("PauseResumeButton");
            SkinButton("PauseRestartButton");
            SkinButton("ShovelButton");
            SkinButton("MagicWandButton");
            SkinButton("SortingMagnetButton");

            SkinButtonIcon("PauseButton", "UiIcons/icon_pause", true);
            SkinButtonIcon("LevelButton", "UiIcons/icon_nav_level", true);
            SkinButtonIcon("MissionButton", "UiIcons/icon_nav_mission", true);
            SkinButtonIcon("ShopButton", "UiIcons/icon_nav_shop", true);
            SkinButtonIcon("BagButton", "UiIcons/icon_nav_bag", true);
            SkinButtonIcon("LevelCloseButton", "UiIcons/icon_close", true);
            SkinButtonIcon("MissionCloseButton", "UiIcons/icon_close", true);
            SkinButtonIcon("ShopCloseButton", "UiIcons/icon_close", true);
            SkinButtonIcon("InventoryCloseButton", "UiIcons/icon_close", true);
            SkinButtonIcon("RestartButton", "UiIcons/icon_restart", false);
            SkinButtonIcon("NextLevelButton", "UiIcons/icon_next", false);
            SkinButtonIcon("PauseResumeButton", null, false);
            SkinButtonIcon("PauseRestartButton", "UiIcons/icon_restart", false);
            SkinInlineIcon("TimerText", "UiIcons/icon_timer", UiThemePalette.TextLight);
            SkinInlineIcon("GoldText", "UiIcons/icon_currency_gold", UiThemePalette.TextLight);
            SkinInlineIcon("GemText", "UiIcons/icon_currency_gem", UiThemePalette.TextLight);
            SkinAbilityButtonIcon("ShovelButton", "UiIcons/icon_ability_shovel");
            SkinAbilityButtonIcon("MagicWandButton", "UiIcons/icon_ability_magic_wand");
            SkinAbilityButtonIcon("SortingMagnetButton", "UiIcons/icon_ability_sorting_magnet");

            EnsurePanelTransition("ResultPanel");
            EnsurePanelTransition("PausePanel");
            EnsurePanelTransition("LevelPanel");
            EnsurePanelTransition("InventoryPanel");
            EnsurePanelTransition("ShopPanel");
            EnsurePanelTransition("MissionPanel");
            EnsureModalBackdrop();
        }

        private static void SkinDropZone(string objectName, ExternalDropZoneKind kind)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            if (gameObject == null)
            {
                return;
            }

            Image image = gameObject.GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            image.sprite = LoadSprite(kind == ExternalDropZoneKind.SellBasket ? "UiSkins/ui_drop_sell" : "UiSkins/ui_drop_delivery") ??
                (kind == ExternalDropZoneKind.SellBasket
                ? PlaceholderSpriteFactory.SellBasketSprite
                : PlaceholderSpriteFactory.DeliverZoneSprite);
            image.color = Color.white;

            ExternalDropZone zone = gameObject.GetComponent<ExternalDropZone>();
            if (zone != null)
            {
                Color highlighted = kind == ExternalDropZoneKind.SellBasket
                    ? UiThemePalette.Sell
                    : UiThemePalette.Delivery;
                zone.Configure(kind, Color.Lerp(highlighted, Color.white, 0.28f), highlighted);
                EnsureAmbientPulse(gameObject, highlighted);
            }

            ApplyTextColor(gameObject, UiThemePalette.TextDark);
        }

        private static void SkinButton(string objectName)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            if (gameObject == null)
            {
                return;
            }

            Image image = gameObject.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = LoadSprite("UiSkins/ui_button_primary") ?? PlaceholderSpriteFactory.HudButtonSprite;
                image.color = UiThemePalette.PrimaryButton;
            }

            Button button = gameObject.GetComponent<Button>();
            if (button != null)
            {
                button.colors = UiThemePalette.BuildButtonColors(UiThemePalette.PrimaryButton);
                EnsureButtonFeedback(gameObject);
            }

            ApplyTextColor(gameObject, UiThemePalette.TextLight);
        }

        private static void SkinButtonIcon(string objectName, string resourcePath, bool hideText)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            if (gameObject == null)
            {
                return;
            }

            Sprite sprite = LoadSprite(resourcePath);
            if (sprite == null && objectName == "PauseResumeButton")
            {
                sprite = PlaceholderSpriteFactory.PlayIconSprite;
            }

            if (sprite == null)
            {
                return;
            }

            RectTransform parentRect = gameObject.GetComponent<RectTransform>();
            if (parentRect == null)
            {
                return;
            }

            Transform iconTransform = gameObject.transform.Find("RuntimeIcon");
            GameObject iconObject = iconTransform != null
                ? iconTransform.gameObject
                : new GameObject("RuntimeIcon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(gameObject.transform, false);
            iconObject.transform.SetAsLastSibling();

            RectTransform iconRect = iconObject.GetComponent<RectTransform>();
            iconRect.anchorMin = hideText ? new Vector2(0.18f, 0.18f) : new Vector2(0.10f, 0.24f);
            iconRect.anchorMax = hideText ? new Vector2(0.82f, 0.82f) : new Vector2(0.38f, 0.76f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            Image iconImage = iconObject.GetComponent<Image>();
            iconImage.sprite = sprite;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;

            if (hideText)
            {
                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i].gameObject != iconObject)
                    {
                        texts[i].enabled = false;
                    }
                }

                return;
            }

            Text[] visibleTexts = gameObject.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < visibleTexts.Length; i++)
            {
                visibleTexts[i].enabled = true;
                visibleTexts[i].alignment = TextAnchor.MiddleCenter;
                visibleTexts[i].color = UiThemePalette.TextLight;

                RectTransform textRect = visibleTexts[i].GetComponent<RectTransform>();
                if (textRect != null)
                {
                    textRect.anchorMin = new Vector2(0.38f, 0.08f);
                    textRect.anchorMax = new Vector2(0.96f, 0.92f);
                    textRect.offsetMin = Vector2.zero;
                    textRect.offsetMax = Vector2.zero;
                }
            }
        }

        private static void SkinInlineIcon(string objectName, string resourcePath, Color textColor)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            if (gameObject == null)
            {
                return;
            }

            Sprite sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite == null)
            {
                return;
            }

            Image iconImage = EnsureRuntimeIcon(gameObject, sprite);
            RectTransform iconRect = iconImage.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.02f, 0.20f);
            iconRect.anchorMax = new Vector2(0.34f, 0.80f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            Text text = gameObject.GetComponent<Text>();
            if (text != null)
            {
                text.alignment = TextAnchor.MiddleRight;
                text.color = textColor;
            }
        }

        private static void SkinAbilityButtonIcon(string objectName, string resourcePath)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            if (gameObject == null)
            {
                return;
            }

            Sprite sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite == null)
            {
                return;
            }

            Image iconImage = EnsureRuntimeIcon(gameObject, sprite);
            RectTransform iconRect = iconImage.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.12f, 0.32f);
            iconRect.anchorMax = new Vector2(0.62f, 0.88f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                RectTransform textRect = texts[i].GetComponent<RectTransform>();
                if (textRect != null)
                {
                    textRect.anchorMin = new Vector2(0.58f, 0.12f);
                    textRect.anchorMax = new Vector2(0.96f, 0.88f);
                    textRect.offsetMin = Vector2.zero;
                    textRect.offsetMax = Vector2.zero;
                }

                texts[i].alignment = TextAnchor.MiddleCenter;
                texts[i].color = UiThemePalette.TextLight;
            }
        }

        private static Image EnsureRuntimeIcon(GameObject gameObject, Sprite sprite)
        {
            Transform iconTransform = gameObject.transform.Find("RuntimeIcon");
            GameObject iconObject = iconTransform != null
                ? iconTransform.gameObject
                : new GameObject("RuntimeIcon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(gameObject.transform, false);
            iconObject.transform.SetAsLastSibling();

            Image iconImage = iconObject.GetComponent<Image>();
            iconImage.sprite = sprite;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            return iconImage;
        }

        private static void SkinImage(string objectName, Sprite sprite, Color color, bool skinText, string resourcePath = null)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            if (gameObject == null)
            {
                return;
            }

            Image image = gameObject.GetComponent<Image>();
            if (image != null)
            {
                Sprite resourceSprite = LoadSprite(resourcePath);
                image.sprite = resourceSprite ?? sprite;
                image.color = resourceSprite != null ? Color.white : color;
            }

            if (skinText)
            {
                ApplyTextColor(gameObject, UiThemePalette.GetTextForBackground(color));
            }
        }

        private static Sprite LoadSprite(string resourcePath)
        {
            return string.IsNullOrWhiteSpace(resourcePath)
                ? null
                : Resources.Load<Sprite>(resourcePath);
        }

        private static void ApplyTextColor(GameObject root, Color color)
        {
            if (root == null)
            {
                return;
            }

            Text[] texts = root.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = color;
                EnsureTextShadow(texts[i], color);
            }
        }

        private static void EnsureButtonFeedback(GameObject gameObject)
        {
            if (gameObject != null && gameObject.GetComponent<UiButtonFeedback>() == null)
            {
                gameObject.AddComponent<UiButtonFeedback>();
            }
        }

        private static void EnsureAmbientPulse(GameObject gameObject, Color color)
        {
            if (gameObject == null)
            {
                return;
            }

            UiAmbientPulse pulse = gameObject.GetComponent<UiAmbientPulse>();
            if (pulse == null)
            {
                pulse = gameObject.AddComponent<UiAmbientPulse>();
            }

            pulse.Configure(color, 0.06f, 0.18f, 2.1f);
        }

        private static void EnsurePanelTransition(string objectName)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            if (gameObject != null && gameObject.GetComponent<UiPanelTransition>() == null)
            {
                gameObject.AddComponent<UiPanelTransition>();
            }
        }

        private void EnsureModalBackdrop()
        {
            GameObject hudRoot = FindObjectIncludingInactive("HUDRoot");
            GameObject target = hudRoot != null ? hudRoot : gameObject;
            UiModalBackdropController backdrop = target.GetComponent<UiModalBackdropController>();
            if (backdrop == null)
            {
                backdrop = target.AddComponent<UiModalBackdropController>();
            }

            backdrop.RefreshBackdrop();
        }

        private static void EnsureTextShadow(Text text, Color color)
        {
            if (text == null)
            {
                return;
            }

            Shadow shadow = text.GetComponent<Shadow>();
            if (shadow == null)
            {
                shadow = text.gameObject.AddComponent<Shadow>();
            }

            bool lightText = color == UiThemePalette.TextLight;
            shadow.effectColor = lightText
                ? new Color(0.03f, 0.08f, 0.06f, 0.42f)
                : new Color(1f, 1f, 1f, 0.22f);
            shadow.effectDistance = new Vector2(1.1f, -1.1f);
            shadow.useGraphicAlpha = true;
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
    }
}
