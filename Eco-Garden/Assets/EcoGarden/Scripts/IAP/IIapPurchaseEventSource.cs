using System;

namespace EcoGarden.IAP
{
    public interface IIapPurchaseEventSource
    {
        event Action<IapPurchaseResult> PurchaseCompleted;
    }
}
