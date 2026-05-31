using System.Collections.Generic;
using EcoGarden.AI;
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
        public const string BackgroundLilyPondId = "skin_background_lily_pond";

        [SerializeField] private ShopController shopController;
        [SerializeField] private BoardBackdropController boardBackdrop;
        [SerializeField] private EcoGardenBackgroundController backgroundController;
        [SerializeField] private NpcMovementController npcController;

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
            ApplyNpcSkin(owned.Contains(NpcTravelerId));
            ApplyBackgroundSkin(owned.Contains(BackgroundLilyPondId));
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
                boardBackdrop.SetCosmeticTint(new Color(0.76f, 0.88f, 0.68f, 1f));
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

        private void ApplyNpcSkin(bool owned)
        {
            if (!owned)
            {
                return;
            }

            if (npcController == null)
            {
                npcController = FindAnyObjectByType<NpcMovementController>();
            }

            if (npcController != null)
            {
                npcController.SetCosmeticColor(new Color(0.42f, 0.74f, 0.94f, 1f));
            }
        }

        private void ApplyBackgroundSkin(bool owned)
        {
            if (!owned)
            {
                return;
            }

            if (backgroundController == null)
            {
                backgroundController = FindAnyObjectByType<EcoGardenBackgroundController>();
            }

            if (backgroundController != null)
            {
                backgroundController.SetCosmeticBackground("Backgrounds/bg_lily_pond_sunset_01", new Color(1f, 0.94f, 0.84f, 1f));
            }
        }

        private static void EnsureDecorButterfly()
        {
            if (GameObject.Find("DecorButterflyVariant") != null)
            {
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
