using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Economy;
using EcoGarden.IAP;
using EcoGarden.Progression;
using EcoGarden.Rewards;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EcoGarden.Shop
{
    public sealed class ShopController : MonoBehaviour
    {
        [SerializeField] private ShopItemDefinition[] catalogItems;
        [SerializeField] private EconomyController economyController;
        [SerializeField] private BoardController boardController;
        [SerializeField] private MockIapProvider mockIapProvider;
        [SerializeField] private MonoBehaviour iapProviderBehaviour;
        [SerializeField] private MonoBehaviour receiptValidatorBehaviour;
        [SerializeField] private bool includeDecorationCatalogItems = true;

        private ShopCatalogService catalog;
        private IapPurchaseService iapPurchaseService;
        private IIapProvider activeIapProvider;
        private IIapPurchaseEventSource activeIapEventSource;
        private readonly HashSet<string> processedIapTransactionIds = new HashSet<string>();

        public event Action ProcessedIapTransactionsChanged;
        public event Action<ShopPurchaseResult> IapPurchaseCompleted;
        public ShopInventory Inventory { get; private set; } = new ShopInventory();
        public ShopCatalogService Catalog
        {
            get
            {
                if (catalog == null)
                {
                    catalog = new ShopCatalogService(catalogItems, includeDecorationCatalogItems);
                }

                return catalog;
            }
        }

        private void Awake()
        {
            ResolveReferences();
            catalog = new ShopCatalogService(catalogItems, includeDecorationCatalogItems);
        }

        public void SetCatalogItems(ShopItemDefinition[] items)
        {
            catalogItems = items;
            catalog = new ShopCatalogService(catalogItems, includeDecorationCatalogItems);
        }

        public void RestoreInventory(string[] purchasedProductIds, string[] ownedDecorationIds)
        {
            Inventory.Restore(purchasedProductIds, ownedDecorationIds);
        }

        public void RestoreInventory(string[] purchasedProductIds, string[] ownedDecorationIds, string[] activeDecorationIds)
        {
            Inventory.Restore(purchasedProductIds, ownedDecorationIds, activeDecorationIds);
        }

        public bool UseDecoration(string decorationId)
        {
            return Inventory != null && Inventory.UseDecoration(decorationId);
        }

        public bool UseDecorationExclusive(string decorationId, IEnumerable<string> exclusiveDecorationIds)
        {
            return Inventory != null && Inventory.UseDecorationExclusive(decorationId, exclusiveDecorationIds);
        }

        public bool RemoveDecoration(string decorationId)
        {
            return Inventory != null && Inventory.RemoveDecoration(decorationId);
        }

        public void RestoreProcessedIapTransactionIds(string[] transactionIds)
        {
            processedIapTransactionIds.Clear();
            if (transactionIds != null)
            {
                for (int i = 0; i < transactionIds.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(transactionIds[i]))
                    {
                        processedIapTransactionIds.Add(transactionIds[i]);
                    }
                }
            }

            iapPurchaseService = null;
        }

        public string[] GetProcessedIapTransactionIds()
        {
            if (iapPurchaseService != null)
            {
                return iapPurchaseService.GetProcessedTransactionIds();
            }

            string[] result = new string[processedIapTransactionIds.Count];
            processedIapTransactionIds.CopyTo(result);
            return result;
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
            IIapProvider provider = ResolveIapProvider();
            if (provider == null)
            {
                return new ShopPurchaseResult(ShopPurchaseStatus.UnsupportedPurchaseKind, item);
            }

            AbilityInventory abilityInventory = boardController != null ? boardController.AbilityInventory : null;
            PlantUnlockService plantUnlockService = boardController != null ? boardController.PlantUnlockService : null;
            if (iapPurchaseService == null)
            {
                iapPurchaseService = new IapPurchaseService(
                    provider,
                    economyController,
                    abilityInventory,
                    plantUnlockService,
                    Inventory,
                    processedIapTransactionIds,
                    OnProcessedIapTransactionAdded,
                    ResolveReceiptValidator());
            }

            IapPurchaseService service = iapPurchaseService;
            IapProductPurchaseResult result = service.Purchase(item);
            return new ShopPurchaseResult(MapIapStatus(result.Status), item);
        }

        private IIapProvider ResolveIapProvider()
        {
            IIapProvider provider = iapProviderBehaviour as IIapProvider;
            if (provider == null)
            {
                provider = mockIapProvider;
            }

            if (ReferenceEquals(provider, activeIapProvider))
            {
                return provider;
            }

            if (activeIapEventSource != null)
            {
                activeIapEventSource.PurchaseCompleted -= OnIapProviderPurchaseCompleted;
            }

            activeIapProvider = provider;
            activeIapEventSource = provider as IIapPurchaseEventSource;
            if (activeIapEventSource != null)
            {
                activeIapEventSource.PurchaseCompleted += OnIapProviderPurchaseCompleted;
            }

            iapPurchaseService = null;
            return provider;
        }

        private void OnIapProviderPurchaseCompleted(IapPurchaseResult purchaseResult)
        {
            if (iapPurchaseService == null || string.IsNullOrWhiteSpace(purchaseResult.StoreProductId))
            {
                return;
            }

            ShopItemDefinition item = FindIapItemByStoreProductId(purchaseResult.StoreProductId);
            if (item == null)
            {
                IapPurchaseCompleted?.Invoke(new ShopPurchaseResult(ShopPurchaseStatus.ProductNotFound, null));
                return;
            }

            IapProductPurchaseResult result = iapPurchaseService.CompletePurchase(item, purchaseResult);
            IapPurchaseCompleted?.Invoke(new ShopPurchaseResult(MapIapStatus(result.Status), item));
        }

        private ShopItemDefinition FindIapItemByStoreProductId(string storeProductId)
        {
            IReadOnlyList<ShopItemDefinition> items = Catalog.Items;
            for (int i = 0; i < items.Count; i++)
            {
                ShopItemDefinition item = items[i];
                if (item != null &&
                    item.Price != null &&
                    item.Price.PurchaseKind == ShopPurchaseKind.Iap &&
                    item.Price.IapProductId == storeProductId)
                {
                    return item;
                }
            }

            return null;
        }

        private void OnProcessedIapTransactionAdded(string transactionId)
        {
            if (!string.IsNullOrWhiteSpace(transactionId) && processedIapTransactionIds.Add(transactionId))
            {
                ProcessedIapTransactionsChanged?.Invoke();
            }
        }

        private static ShopPurchaseStatus MapIapStatus(IapPurchaseStatus status)
        {
            switch (status)
            {
                case IapPurchaseStatus.Success:
                    return ShopPurchaseStatus.Success;
                case IapPurchaseStatus.Pending:
                    return ShopPurchaseStatus.Pending;
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

        private IIapReceiptValidator ResolveReceiptValidator()
        {
            IIapReceiptValidator validator = receiptValidatorBehaviour as IIapReceiptValidator;
            if (validator != null)
            {
                return validator;
            }

            BackendIapReceiptValidator backendValidator = FindAnyObjectByType<BackendIapReceiptValidator>();
            if (backendValidator != null)
            {
                receiptValidatorBehaviour = backendValidator;
                return backendValidator;
            }

            return null;
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

            if (iapProviderBehaviour == null)
            {
                UnityIapProvider unityIapProvider = FindAnyObjectByType<UnityIapProvider>();
                if (unityIapProvider != null)
                {
                    iapProviderBehaviour = unityIapProvider;
                }
            }
        }
    }
}
