namespace EcoGarden.Shop
{
    public enum ShopPurchaseStatus
    {
        Success,
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
