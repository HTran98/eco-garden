using EcoGarden.Rewards;
using EcoGarden.Shop;

namespace EcoGarden.IAP
{
    public readonly struct IapProductPurchaseResult
    {
        public IapProductPurchaseResult(
            IapPurchaseStatus status,
            ShopItemDefinition item,
            string transactionId,
            RewardGrantResult rewardResult,
            string receiptPayload = "")
            : this(status, item, transactionId, rewardResult, receiptPayload, IapReceiptValidationResult.NotRequired())
        {
        }

        public IapProductPurchaseResult(
            IapPurchaseStatus status,
            ShopItemDefinition item,
            string transactionId,
            RewardGrantResult rewardResult,
            string receiptPayload,
            IapReceiptValidationResult receiptValidationResult)
        {
            Status = status;
            Item = item;
            TransactionId = transactionId;
            RewardResult = rewardResult;
            ReceiptPayload = receiptPayload;
            ReceiptValidationResult = receiptValidationResult;
        }

        public IapPurchaseStatus Status { get; }
        public ShopItemDefinition Item { get; }
        public string TransactionId { get; }
        public RewardGrantResult RewardResult { get; }
        public string ReceiptPayload { get; }
        public IapReceiptValidationResult ReceiptValidationResult { get; }
        public bool Succeeded { get { return Status == IapPurchaseStatus.Success; } }
    }
}
