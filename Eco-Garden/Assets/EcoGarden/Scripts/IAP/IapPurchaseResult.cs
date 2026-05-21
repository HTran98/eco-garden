namespace EcoGarden.IAP
{
    public readonly struct IapPurchaseResult
    {
        public IapPurchaseResult(
            IapPurchaseStatus status,
            string storeProductId,
            string transactionId,
            string message = "")
        {
            Status = status;
            StoreProductId = storeProductId;
            TransactionId = transactionId;
            Message = message;
        }

        public IapPurchaseStatus Status { get; }
        public string StoreProductId { get; }
        public string TransactionId { get; }
        public string Message { get; }
        public bool Succeeded { get { return Status == IapPurchaseStatus.Success; } }
    }
}
