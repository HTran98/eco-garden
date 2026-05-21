using System;
using System.Collections.Generic;

namespace EcoGarden.Shop
{
    public sealed class ShopInventory
    {
        private readonly HashSet<string> purchasedProductIds = new HashSet<string>();
        private readonly HashSet<string> ownedDecorationIds = new HashSet<string>();

        public event Action Changed;

        public bool IsProductPurchased(string productId)
        {
            return !string.IsNullOrWhiteSpace(productId) && purchasedProductIds.Contains(productId);
        }

        public bool IsDecorationOwned(string decorationId)
        {
            return !string.IsNullOrWhiteSpace(decorationId) && ownedDecorationIds.Contains(decorationId);
        }

        public bool MarkProductPurchased(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId) || !purchasedProductIds.Add(productId))
            {
                return false;
            }

            Changed?.Invoke();
            return true;
        }

        public int AddDecorations(IEnumerable<string> decorationIds)
        {
            if (decorationIds == null)
            {
                return 0;
            }

            int addedCount = 0;
            foreach (string decorationId in decorationIds)
            {
                if (!string.IsNullOrWhiteSpace(decorationId) && ownedDecorationIds.Add(decorationId))
                {
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                Changed?.Invoke();
            }

            return addedCount;
        }

        public void Restore(string[] productIds, string[] decorationIds)
        {
            purchasedProductIds.Clear();
            ownedDecorationIds.Clear();

            if (productIds != null)
            {
                for (int i = 0; i < productIds.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(productIds[i]))
                    {
                        purchasedProductIds.Add(productIds[i]);
                    }
                }
            }

            if (decorationIds != null)
            {
                for (int i = 0; i < decorationIds.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(decorationIds[i]))
                    {
                        ownedDecorationIds.Add(decorationIds[i]);
                    }
                }
            }
        }

        public string[] GetPurchasedProductIds()
        {
            string[] result = new string[purchasedProductIds.Count];
            purchasedProductIds.CopyTo(result);
            return result;
        }

        public string[] GetOwnedDecorationIds()
        {
            string[] result = new string[ownedDecorationIds.Count];
            ownedDecorationIds.CopyTo(result);
            return result;
        }
    }
}
