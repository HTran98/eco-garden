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
            SkinImage("TopBar", PlaceholderSpriteFactory.HudTopBarSprite, Color.white);
            SkinImage("ObjectivePanel", PlaceholderSpriteFactory.HudPanelSprite, Color.white);
            SkinImage("AbilityBar", PlaceholderSpriteFactory.HudPanelSprite, Color.white);
            SkinImage("ResultPanel", PlaceholderSpriteFactory.HudPanelSprite, new Color(0.18f, 0.22f, 0.26f, 0.96f));
            SkinImage("ShopPanel", PlaceholderSpriteFactory.ShopPanelSprite, Color.white);
            SkinImage("ShopProductViewport", PlaceholderSpriteFactory.HudPanelSprite, new Color(0.06f, 0.09f, 0.10f, 0.70f));
            SkinImage("MissionPanel", PlaceholderSpriteFactory.HudPanelSprite, new Color(0.12f, 0.16f, 0.18f, 0.97f));
            SkinImage("MissionViewport", PlaceholderSpriteFactory.HudPanelSprite, new Color(0.06f, 0.08f, 0.10f, 0.55f));
            SkinImage("MissionTrackerPanel", PlaceholderSpriteFactory.HudPanelSprite, new Color(0.12f, 0.16f, 0.18f, 0.90f));
            SkinDropZone("DeliveryDropZone", ExternalDropZoneKind.Delivery);
            SkinDropZone("SellBasket", ExternalDropZoneKind.SellBasket);

            SkinButton("PauseButton");
            SkinButton("MissionButton");
            SkinButton("MissionCloseButton");
            SkinButton("MissionTrackerOpenButton");
            SkinButton("ShopButton");
            SkinButton("ShopCloseButton");
            SkinButton("ShopCategoryBoosterButton");
            SkinButton("ShopCategoryDecorationButton");
            SkinButton("ShopCategoryUnlockButton");
            SkinButton("ShopCategoryCurrencyButton");
            SkinButton("ShopCategoryBundleButton");
            SkinButton("RestartButton");
            SkinButton("ShovelButton");
            SkinButton("MagicWandButton");
            SkinButton("SortingMagnetButton");
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

            image.sprite = kind == ExternalDropZoneKind.SellBasket
                ? PlaceholderSpriteFactory.SellBasketSprite
                : PlaceholderSpriteFactory.DeliverZoneSprite;
            image.color = Color.white;

            ExternalDropZone zone = gameObject.GetComponent<ExternalDropZone>();
            if (zone != null)
            {
                Color highlighted = kind == ExternalDropZoneKind.SellBasket
                    ? new Color(0.84f, 1f, 0.54f, 1f)
                    : new Color(1f, 0.76f, 1f, 1f);
                zone.Configure(kind, Color.white, highlighted);
            }
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
                image.sprite = PlaceholderSpriteFactory.HudButtonSprite;
                image.color = Color.white;
            }
        }

        private static void SkinImage(string objectName, Sprite sprite, Color color)
        {
            GameObject gameObject = FindObjectIncludingInactive(objectName);
            if (gameObject == null)
            {
                return;
            }

            Image image = gameObject.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
                image.color = color;
            }
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
