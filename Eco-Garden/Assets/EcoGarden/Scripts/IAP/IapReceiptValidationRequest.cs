namespace EcoGarden.IAP
{
    public readonly struct IapReceiptValidationRequest
    {
        public IapReceiptValidationRequest(
            string storeProductId,
            string transactionId,
            string receiptPayload)
        {
            StoreProductId = storeProductId;
            TransactionId = transactionId;
            ReceiptPayload = receiptPayload;
        }

        public string StoreProductId { get; }
        public string TransactionId { get; }
        public string ReceiptPayload { get; }
        public bool HasRequiredPayload
        {
            get
            {
                return !string.IsNullOrWhiteSpace(StoreProductId) &&
                    !string.IsNullOrWhiteSpace(TransactionId) &&
                    !string.IsNullOrWhiteSpace(ReceiptPayload);
            }
        }
    }
}
