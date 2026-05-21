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
            RewardGrantResult rewardResult)
        {
            Status = status;
            Item = item;
            TransactionId = transactionId;
            RewardResult = rewardResult;
        }

        public IapPurchaseStatus Status { get; }
        public ShopItemDefinition Item { get; }
        public string TransactionId { get; }
        public RewardGrantResult RewardResult { get; }
        public bool Succeeded { get { return Status == IapPurchaseStatus.Success; } }
    }
}
