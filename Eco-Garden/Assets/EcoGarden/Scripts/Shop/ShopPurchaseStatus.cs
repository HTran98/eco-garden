namespace EcoGarden.Shop
{
    public enum ShopPurchaseStatus
    {
        Success,
        Pending,
        ProductNotFound,
        AlreadyOwned,
        UnsupportedPurchaseKind,
        InsufficientCurrency,
        InvalidProduct,
        IapCancelled,
        IapFailed,
        DuplicateTransaction
    }
}
