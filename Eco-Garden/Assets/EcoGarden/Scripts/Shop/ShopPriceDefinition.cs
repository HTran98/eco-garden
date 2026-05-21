using System;
using EcoGarden.Economy;
using UnityEngine;

namespace EcoGarden.Shop
{
    [Serializable]
    public sealed class ShopPriceDefinition
    {
        [SerializeField] private ShopPurchaseKind purchaseKind;
        [SerializeField] private CurrencyKind currencyKind;
        [SerializeField] private int amount;
        [SerializeField] private string iapProductId;

        public ShopPurchaseKind PurchaseKind { get { return purchaseKind; } }
        public CurrencyKind CurrencyKind { get { return currencyKind; } }
        public int Amount { get { return amount; } }
        public string IapProductId { get { return iapProductId; } }
        public bool IsSoftCurrency { get { return purchaseKind == ShopPurchaseKind.Gold || purchaseKind == ShopPurchaseKind.Gem; } }

        public ShopPriceDefinition()
        {
        }

        public ShopPriceDefinition(ShopPurchaseKind purchaseKind, int amount, string iapProductId = "")
        {
            this.purchaseKind = purchaseKind;
            this.amount = Mathf.Max(0, amount);
            this.iapProductId = iapProductId ?? string.Empty;
            currencyKind = purchaseKind == ShopPurchaseKind.Gem ? CurrencyKind.Gem : CurrencyKind.Gold;
        }
    }
}
