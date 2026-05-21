namespace EcoGarden.IAP
{
    public interface IIapProvider
    {
        bool IsProductAvailable(string storeProductId);
        IapPurchaseResult Purchase(string storeProductId);
    }
}
