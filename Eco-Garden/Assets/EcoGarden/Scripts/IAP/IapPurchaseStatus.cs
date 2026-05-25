namespace EcoGarden.IAP
{
    public enum IapPurchaseStatus
    {
        Success,
        Pending,
        Cancelled,
        Failed,
        ProductUnavailable,
        InvalidProduct,
        AlreadyOwned,
        DuplicateTransaction,
        ReceiptValidationFailed
    }
}
