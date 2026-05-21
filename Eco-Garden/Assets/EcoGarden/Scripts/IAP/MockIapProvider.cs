using UnityEngine;

namespace EcoGarden.IAP
{
    public sealed class MockIapProvider : MonoBehaviour, IIapProvider
    {
        [SerializeField] private MockIapOutcome nextOutcome = MockIapOutcome.Success;
        [SerializeField] private string fixedTransactionId;

        private int transactionCounter;

        public void SetNextOutcome(MockIapOutcome outcome)
        {
            nextOutcome = outcome;
        }

        public void SetFixedTransactionId(string transactionId)
        {
            fixedTransactionId = transactionId ?? string.Empty;
        }

        public bool IsProductAvailable(string storeProductId)
        {
            return !string.IsNullOrWhiteSpace(storeProductId);
        }

        public IapPurchaseResult Purchase(string storeProductId)
        {
            if (!IsProductAvailable(storeProductId))
            {
                return new IapPurchaseResult(IapPurchaseStatus.ProductUnavailable, storeProductId, string.Empty);
            }

            switch (nextOutcome)
            {
                case MockIapOutcome.Cancelled:
                    return new IapPurchaseResult(IapPurchaseStatus.Cancelled, storeProductId, string.Empty, "Mock purchase cancelled.");
                case MockIapOutcome.Failed:
                    return new IapPurchaseResult(IapPurchaseStatus.Failed, storeProductId, string.Empty, "Mock purchase failed.");
                default:
                    transactionCounter++;
                    string transactionId = string.IsNullOrWhiteSpace(fixedTransactionId)
                        ? "mock_tx_" + transactionCounter.ToString("0000")
                        : fixedTransactionId;
                    return new IapPurchaseResult(IapPurchaseStatus.Success, storeProductId, transactionId);
            }
        }
    }
}
