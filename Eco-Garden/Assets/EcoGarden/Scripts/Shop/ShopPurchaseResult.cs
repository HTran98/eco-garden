namespace EcoGarden.Shop
{
    public readonly struct ShopPurchaseResult
    {
        public ShopPurchaseResult(ShopPurchaseStatus status, ShopItemDefinition item)
        {
            Status = status;
            Item = item;
        }

        public ShopPurchaseStatus Status { get; }
        public ShopItemDefinition Item { get; }
        public bool Succeeded { get { return Status == ShopPurchaseStatus.Success; } }
    }
}
