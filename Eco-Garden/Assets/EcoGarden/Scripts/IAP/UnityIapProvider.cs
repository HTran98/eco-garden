using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

namespace EcoGarden.IAP
{
    public sealed class UnityIapProvider : MonoBehaviour, IIapProvider, IIapPurchaseEventSource
    {
        [SerializeField] private bool initializeOnAwake = true;
        [SerializeField] private string[] consumableProductIds = IapProductIds.CreateRequiredConsumableIds();

        private readonly HashSet<string> availableProductIds = new HashSet<string>();
        private StoreController storeController;
        private bool storeConnected;
        private bool productsFetched;

        public event Action<IapPurchaseResult> PurchaseCompleted;

        private void Awake()
        {
            if (initializeOnAwake)
            {
                Initialize();
            }
        }

        public async void Initialize()
        {
            if (storeController != null)
            {
                return;
            }

            storeController = UnityIAPServices.StoreController();
            storeController.OnPurchasePending += OnPurchasePending;
            storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            storeController.OnPurchaseFailed += OnPurchaseFailed;
            storeController.OnPurchaseDeferred += OnPurchaseDeferred;
            storeController.OnStoreConnected += OnStoreConnected;
            storeController.OnStoreDisconnected += OnStoreDisconnected;
            storeController.OnProductsFetched += OnProductsFetched;
            storeController.OnProductsFetchFailed += OnProductsFetchFailed;
            storeController.OnPurchasesFetched += OnPurchasesFetched;
            storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;

            try
            {
                await storeController.Connect();
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Unity IAP connect failed: " + exception.Message);
            }
        }

        public bool IsProductAvailable(string storeProductId)
        {
            return storeConnected &&
                productsFetched &&
                !string.IsNullOrWhiteSpace(storeProductId) &&
                availableProductIds.Contains(storeProductId);
        }

        public IapPurchaseResult Purchase(string storeProductId)
        {
            if (!IsProductAvailable(storeProductId))
            {
                return new IapPurchaseResult(IapPurchaseStatus.ProductUnavailable, storeProductId, string.Empty);
            }

            try
            {
                storeController.PurchaseProduct(storeProductId);
                return new IapPurchaseResult(IapPurchaseStatus.Pending, storeProductId, string.Empty);
            }
            catch (Exception exception)
            {
                return new IapPurchaseResult(IapPurchaseStatus.Failed, storeProductId, string.Empty, exception.Message);
            }
        }

        private void OnStoreConnected()
        {
            storeConnected = true;
            FetchProducts();
        }

        private void FetchProducts()
        {
            List<ProductDefinition> productDefinitions = new List<ProductDefinition>();
            for (int i = 0; i < consumableProductIds.Length; i++)
            {
                string productId = consumableProductIds[i];
                if (!string.IsNullOrWhiteSpace(productId))
                {
                    productDefinitions.Add(new ProductDefinition(productId, ProductType.Consumable));
                }
            }

            storeController.FetchProducts(productDefinitions);
        }

        private void OnProductsFetched(List<Product> products)
        {
            availableProductIds.Clear();
            for (int i = 0; i < products.Count; i++)
            {
                Product product = products[i];
                if (product != null && product.availableToPurchase)
                {
                    availableProductIds.Add(product.definition.id);
                }
            }

            productsFetched = true;
            storeController.FetchPurchases();
        }

        private void OnPurchasePending(PendingOrder order)
        {
            string productId = GetFirstProductId(order);
            string transactionId = order != null && order.Info != null ? order.Info.TransactionID : string.Empty;
            string receiptPayload = order != null && order.Info != null ? order.Info.Receipt : string.Empty;
            PurchaseCompleted?.Invoke(new IapPurchaseResult(
                IapPurchaseStatus.Success,
                productId,
                transactionId,
                string.Empty,
                receiptPayload));
            storeController.ConfirmPurchase(order);
        }

        private void OnPurchaseFailed(FailedOrder order)
        {
            PurchaseCompleted?.Invoke(new IapPurchaseResult(
                MapFailure(order != null ? order.FailureReason : PurchaseFailureReason.Unknown),
                GetFirstProductId(order),
                order != null && order.Info != null ? order.Info.TransactionID : string.Empty,
                order != null ? order.Details : string.Empty,
                order != null && order.Info != null ? order.Info.Receipt : string.Empty));
        }

        private static string GetFirstProductId(Order order)
        {
            Product product = order != null ? order.CartOrdered.Items().FirstOrDefault()?.Product : null;
            return product != null ? product.definition.id : string.Empty;
        }

        private static IapPurchaseStatus MapFailure(PurchaseFailureReason reason)
        {
            switch (reason)
            {
                case PurchaseFailureReason.UserCancelled:
                    return IapPurchaseStatus.Cancelled;
                case PurchaseFailureReason.ProductUnavailable:
                    return IapPurchaseStatus.ProductUnavailable;
                case PurchaseFailureReason.DuplicateTransaction:
                    return IapPurchaseStatus.DuplicateTransaction;
                default:
                    return IapPurchaseStatus.Failed;
            }
        }

        private static void OnPurchaseConfirmed(Order order)
        {
        }

        private static void OnPurchaseDeferred(DeferredOrder order)
        {
        }

        private static void OnStoreDisconnected(StoreConnectionFailureDescription description)
        {
            Debug.LogWarning("Unity IAP store disconnected: " + description.message);
        }

        private static void OnProductsFetchFailed(ProductFetchFailed failure)
        {
            Debug.LogWarning("Unity IAP product fetch failed: " + failure.FailureReason);
        }

        private static void OnPurchasesFetched(Orders orders)
        {
        }

        private static void OnPurchasesFetchFailed(PurchasesFetchFailureDescription description)
        {
            Debug.LogWarning("Unity IAP purchases fetch failed: " + description.Message);
        }
    }
}
