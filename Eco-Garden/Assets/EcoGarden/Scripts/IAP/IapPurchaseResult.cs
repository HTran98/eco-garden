namespace EcoGarden.IAP
{
    public readonly struct IapPurchaseResult
    {
        public IapPurchaseResult(
            IapPurchaseStatus status,
            string storeProductId,
            string transactionId,
            string message = "",
            string receiptPayload = "")
        {
            Status = status;
            StoreProductId = storeProductId;
            TransactionId = transactionId;
            Message = message;
            ReceiptPayload = receiptPayload;
        }

        public IapPurchaseStatus Status { get; }
        public string StoreProductId { get; }
        public string TransactionId { get; }
        public string Message { get; }
        public string ReceiptPayload { get; }
        public bool Succeeded { get { return Status == IapPurchaseStatus.Success; } }
    }
}
