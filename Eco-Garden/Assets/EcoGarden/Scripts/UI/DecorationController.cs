using System.Collections.Generic;
using EcoGarden.AI;
using EcoGarden.Board;
using EcoGarden.Shop;
using EcoGarden.Utilities;
using UnityEngine;

namespace EcoGarden.UI
{
    public sealed class DecorationController : MonoBehaviour
    {
        public const string BoardMossStoneId = "skin_board_moss_stone";
        public const string ButterflyVariantId = "deco_butterfly_variant";
        public const string BeeVisitorId = "deco_bee_visitor";
        public const string LegacyBirdVisitorId = "deco_bird_visitor";
        public const string NpcTravelerId = "skin_npc_traveler";
        public const string NpcMerchantId = "skin_npc_merchant";
        public const string NpcMoonId = "skin_npc_moon";
        public const string BackgroundLilyPondId = "skin_background_lily_pond";
        public const string BackgroundCrystalLotusId = "skin_background_crystal_lotus";
        public const string BackgroundMoonLotusId = "skin_background_moon_lotus";

        [SerializeField] private ShopController shopController;
        [SerializeField] private BoardBackdropController boardBackdrop;
        [SerializeField] private EcoGardenBackgroundController backgroundController;
        [SerializeField] private NpcMovementController npcController;
        [SerializeField] private BoardView boardView;

        private bool subscribed;

        private void OnEnable()
        {
            ResolveReferences();
            Subscribe();
            ApplyActiveDecorationsFromShop();
        }

        private void Start()
        {
            ResolveReferences();
            Subscribe();
            ApplyActiveDecorationsFromShop();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void ApplyOwnedDecorations(IEnumerable<string> decorationIds)
        {
            HashSet<string> owned = new HashSet<string>();
            if (decorationIds != null)
            {
                foreach (string decorationId in decorationIds)
                {
                    if (!string.IsNullOrWhiteSpace(decorationId))
                    {
                        owned.Add(decorationId);
                    }
                }
            }

            ApplyBoardSkin(owned.Contains(BoardMossStoneId));
            ApplyButterflyVariant(owned.Contains(ButterflyVariantId));
            ApplyBeeVisitor(owned.Contains(BeeVisitorId) || owned.Contains(LegacyBirdVisitorId));
            ApplyNpcSkin(owned);
            ApplyBackgroundSkin(owned);
        }

        public void ApplyActiveDecorationsFromShop()
        {
            if (shopController == null || shopController.Inventory == null)
            {
                return;
            }

            ApplyOwnedDecorations(shopController.Inventory.GetActiveDecorationIds());
        }

        private void ApplyBoardSkin(bool owned)
        {
            if (!owned)
            {
                if (boardBackdrop == null)
                {
                    boardBackdrop = FindAnyObjectByType<BoardBackdropController>();
                }

                if (backgroundController == null)
                {
                    backgroundController = FindAnyObjectByType<EcoGardenBackgroundController>();
                }

                if (boardBackdrop != null)
                {
                    boardBackdrop.ResetCosmeticTint();
                }

                return;
            }

            if (boardBackdrop == null)
            {
                boardBackdrop = FindAnyObjectByType<BoardBackdropController>();
            }

            if (backgroundController == null)
            {
                backgroundController = FindAnyObjectByType<EcoGardenBackgroundController>();
            }

            if (boardBackdrop != null)
            {
                boardBackdrop.SetCosmeticTint(new Color(0.66f, 0.82f, 0.62f, 0.12f));
            }

            if (backgroundController != null)
            {
                backgroundController.SetCosmeticTint(new Color(0.88f, 0.97f, 0.86f, 1f));
            }
        }

        private static void ApplyButterflyVariant(bool owned)
        {
            if (!owned)
            {
                ButterflyMovementController[] defaultButterflies = FindObjectsByType<ButterflyMovementController>(FindObjectsInactive.Include);
                for (int i = 0; i < defaultButterflies.Length; i++)
                {
                    if (defaultButterflies[i] != null && defaultButterflies[i].name != "DecorButterflyVariant")
                    {
                        defaultButterflies[i].ResetCosmeticColor();
                    }
                }

                GameObject decorButterfly = GameObject.Find("DecorButterflyVariant");
                if (decorButterfly != null)
                {
                    decorButterfly.SetActive(false);
                }

                return;
            }

            ButterflyMovementController[] butterflies = FindObjectsByType<ButterflyMovementController>(FindObjectsInactive.Include);
            for (int i = 0; i < butterflies.Length; i++)
            {
                Color color = i % 2 == 0
                    ? new Color(1f, 0.58f, 0.78f, 1f)
                    : new Color(0.46f, 0.82f, 1f, 1f);
                butterflies[i].SetCosmeticColor(color);
            }

            EnsureDecorButterfly();
        }

        private static void ApplyBeeVisitor(bool owned)
        {
            GameObject bee = GameObject.Find("DecorBeeVisitor");
            if (!owned)
            {
                if (bee != null)
                {
                    bee.SetActive(false);
                }

                return;
            }

            if (bee == null)
            {
                bee = new GameObject("DecorBeeVisitor");
                ButterflyMovementController controller = bee.AddComponent<ButterflyMovementController>();
                controller.ConfigureHover(
                    new Vector3(2.7f, 2.65f, -0.22f),
                    new Vector2(0.62f, 0.24f),
                    1.05f,
                    0.7f,
                    new Color(1f, 0.82f, 0.18f, 1f));
                bee.transform.localScale = new Vector3(0.13f, 0.09f, 1f);
            }

            bee.SetActive(true);
        }

        private void ApplyNpcSkin(HashSet<string> owned)
        {
            if (npcController == null)
            {
                npcController = FindAnyObjectByType<NpcMovementController>();
            }

            if (npcController == null)
            {
                return;
            }

            if (owned.Contains(NpcMerchantId))
            {
                npcController.SetCosmeticSprite("Characters/char_customer_merchant_01_alpha", Color.white);
                return;
            }

            if (owned.Contains(NpcMoonId))
            {
                npcController.SetCosmeticSprite("Characters/char_customer_moon_01_alpha", Color.white);
                return;
            }

            npcController.ResetCosmeticSprite();
            if (owned.Contains(NpcTravelerId))
            {
                npcController.SetCosmeticColor(new Color(0.42f, 0.74f, 0.94f, 1f));
            }
        }

        private void ApplyBackgroundSkin(HashSet<string> owned)
        {
            if (backgroundController == null)
            {
                backgroundController = FindAnyObjectByType<EcoGardenBackgroundController>();
            }

            if (backgroundController == null)
            {
                return;
            }

            if (owned.Contains(BackgroundCrystalLotusId))
            {
                backgroundController.SetCosmeticBackground(
                    "Backgrounds/bg_crystal_lotus_pond_01",
                    Color.white,
                    new Vector2(0f, -0.72f));
                ApplyBoardTilePalette(
                    new Color(0.72f, 0.65f, 0.48f, 1f),
                    new Color(0.94f, 0.86f, 0.62f, 1f),
                    new Color(0.25f, 0.72f, 0.78f, 1f));
                return;
            }

            if (owned.Contains(BackgroundMoonLotusId))
            {
                backgroundController.SetCosmeticBackground("Backgrounds/bg_moon_lotus_garden_01", Color.white);
                ApplyBoardTilePalette(
                    new Color(0.24f, 0.35f, 0.54f, 1f),
                    new Color(0.50f, 0.50f, 0.70f, 1f),
                    new Color(0.38f, 0.54f, 0.95f, 1f));
                return;
            }

            if (owned.Contains(BackgroundLilyPondId))
            {
                backgroundController.SetCosmeticBackground("Backgrounds/bg_lily_pond_sunset_01", new Color(1f, 0.94f, 0.84f, 1f));
                ApplyBoardTilePalette(
                    new Color(0.64f, 0.53f, 0.40f, 1f),
                    new Color(0.92f, 0.74f, 0.52f, 1f),
                    new Color(0.52f, 0.58f, 0.70f, 1f));
                return;
            }

            backgroundController.ResetCosmeticBackground();
            ApplyBoardTilePalette(
                new Color(0.42f, 0.68f, 0.66f, 1f),
                new Color(0.58f, 0.78f, 0.70f, 1f),
                new Color(0.20f, 0.49f, 0.62f, 1f));
        }

        private void ApplyBoardTilePalette(Color edgeColor, Color centerColor, Color producerAccentColor)
        {
            if (boardView == null)
            {
                boardView = FindAnyObjectByType<BoardView>();
            }

            if (boardView != null)
            {
                boardView.SetCosmeticTilePalette(edgeColor, centerColor, producerAccentColor);
            }
        }

        private static void EnsureDecorButterfly()
        {
            if (GameObject.Find("DecorButterflyVariant") != null)
            {
                GameObject.Find("DecorButterflyVariant").SetActive(true);
                return;
            }

            GameObject butterflyObject = new GameObject("DecorButterflyVariant");
            ButterflyMovementController butterfly = butterflyObject.AddComponent<ButterflyMovementController>();
            butterfly.ConfigureHover(
                new Vector3(-2.8f, 2.45f, -0.2f),
                new Vector2(0.72f, 0.34f),
                0.92f,
                2.2f,
                new Color(1f, 0.52f, 0.82f, 1f));
        }

        private void ResolveReferences()
        {
            if (shopController == null)
            {
                shopController = FindAnyObjectByType<ShopController>();
            }

            if (boardBackdrop == null)
            {
                boardBackdrop = FindAnyObjectByType<BoardBackdropController>();
            }

            if (backgroundController == null)
            {
                backgroundController = FindAnyObjectByType<EcoGardenBackgroundController>();
            }

            if (npcController == null)
            {
                npcController = FindAnyObjectByType<NpcMovementController>();
            }

            if (boardView == null)
            {
                boardView = FindAnyObjectByType<BoardView>();
            }
        }

        private void Subscribe()
        {
            if (subscribed || shopController == null || shopController.Inventory == null)
            {
                return;
            }

            shopController.Inventory.Changed += ApplyActiveDecorationsFromShop;
            subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!subscribed || shopController == null || shopController.Inventory == null)
            {
                return;
            }

            shopController.Inventory.Changed -= ApplyActiveDecorationsFromShop;
            subscribed = false;
        }
    }
}
