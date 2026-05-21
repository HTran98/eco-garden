using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Economy;
using EcoGarden.IAP;
using EcoGarden.Progression;
using EcoGarden.Rewards;
using UnityEngine;

namespace EcoGarden.Shop
{
    public sealed class ShopController : MonoBehaviour
    {
        [SerializeField] private ShopItemDefinition[] catalogItems;
        [SerializeField] private EconomyController economyController;
        [SerializeField] private BoardController boardController;
        [SerializeField] private MockIapProvider mockIapProvider;

        private ShopCatalogService catalog;
        private IapPurchaseService iapPurchaseService;

        public ShopInventory Inventory { get; private set; } = new ShopInventory();
        public ShopCatalogService Catalog
        {
            get
            {
                if (catalog == null)
                {
                    catalog = new ShopCatalogService(catalogItems);
                }

                return catalog;
            }
        }

        private void Awake()
        {
            ResolveReferences();
            catalog = new ShopCatalogService(catalogItems);
        }

        public void SetCatalogItems(ShopItemDefinition[] items)
        {
            catalogItems = items;
            catalog = new ShopCatalogService(catalogItems);
        }

        public void RestoreInventory(string[] purchasedProductIds, string[] ownedDecorationIds)
        {
            Inventory.Restore(purchasedProductIds, ownedDecorationIds);
        }

        public ShopPurchaseResult TryPurchase(string productId)
        {
            ResolveReferences();

            if (!Catalog.TryGetItem(productId, out ShopItemDefinition item))
            {
                return new ShopPurchaseResult(ShopPurchaseStatus.ProductNotFound, null);
            }

            if (!item.IsValid || item.Price == null || item.Grant == null)
            {
                return new ShopPurchaseResult(ShopPurchaseStatus.InvalidProduct, item);
            }

            if (!item.Repeatable && Inventory.IsProductPurchased(item.ProductId))
            {
                return new ShopPurchaseResult(ShopPurchaseStatus.AlreadyOwned, item);
            }

            if (item.Price.PurchaseKind == ShopPurchaseKind.Iap)
            {
                return TryPurchaseIap(item);
            }

            if (!item.Price.IsSoftCurrency)
            {
                return new ShopPurchaseResult(ShopPurchaseStatus.UnsupportedPurchaseKind, item);
            }

            if (economyController == null ||
                !economyController.TrySpendCurrency(item.Price.CurrencyKind, item.Price.Amount))
            {
                return new ShopPurchaseResult(ShopPurchaseStatus.InsufficientCurrency, item);
            }

            AbilityInventory abilityInventory = boardController != null ? boardController.AbilityInventory : null;
            PlantUnlockService plantUnlockService = boardController != null ? boardController.PlantUnlockService : null;
            RewardService.Grant(item.Grant, economyController, abilityInventory, plantUnlockService);

            Inventory.AddDecorations(item.Grant.DecorationIds);
            if (!item.Repeatable)
            {
                Inventory.MarkProductPurchased(item.ProductId);
            }

            return new ShopPurchaseResult(ShopPurchaseStatus.Success, item);
        }

        private ShopPurchaseResult TryPurchaseIap(ShopItemDefinition item)
        {
            if (mockIapProvider == null)
            {
                return new ShopPurchaseResult(ShopPurchaseStatus.UnsupportedPurchaseKind, item);
            }

            AbilityInventory abilityInventory = boardController != null ? boardController.AbilityInventory : null;
            PlantUnlockService plantUnlockService = boardController != null ? boardController.PlantUnlockService : null;
            if (iapPurchaseService == null)
            {
                iapPurchaseService = new IapPurchaseService(
                    mockIapProvider,
                    economyController,
                    abilityInventory,
                    plantUnlockService,
                    Inventory);
            }

            IapPurchaseService service = iapPurchaseService;
            IapProductPurchaseResult result = service.Purchase(item);
            return new ShopPurchaseResult(MapIapStatus(result.Status), item);
        }

        private static ShopPurchaseStatus MapIapStatus(IapPurchaseStatus status)
        {
            switch (status)
            {
                case IapPurchaseStatus.Success:
                    return ShopPurchaseStatus.Success;
                case IapPurchaseStatus.Cancelled:
                    return ShopPurchaseStatus.IapCancelled;
                case IapPurchaseStatus.DuplicateTransaction:
                    return ShopPurchaseStatus.DuplicateTransaction;
                case IapPurchaseStatus.AlreadyOwned:
                    return ShopPurchaseStatus.AlreadyOwned;
                case IapPurchaseStatus.InvalidProduct:
                    return ShopPurchaseStatus.InvalidProduct;
                default:
                    return ShopPurchaseStatus.IapFailed;
            }
        }

        private void ResolveReferences()
        {
            if (economyController == null)
            {
                economyController = FindAnyObjectByType<EconomyController>();
            }

            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (mockIapProvider == null)
            {
                mockIapProvider = FindAnyObjectByType<MockIapProvider>();
            }
        }
    }
}
