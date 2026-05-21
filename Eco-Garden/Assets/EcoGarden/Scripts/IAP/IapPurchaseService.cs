using System.Collections.Generic;
using System;
using EcoGarden.Abilities;
using EcoGarden.Economy;
using EcoGarden.Progression;
using EcoGarden.Rewards;
using EcoGarden.Shop;

namespace EcoGarden.IAP
{
    public sealed class IapPurchaseService
    {
        private readonly IIapProvider provider;
        private readonly EconomyController economyController;
        private readonly AbilityInventory abilityInventory;
        private readonly PlantUnlockService plantUnlockService;
        private readonly ShopInventory inventory;
        private readonly HashSet<string> grantedTransactionIds = new HashSet<string>();
        private readonly Action<string> processedTransactionAdded;

        public IapPurchaseService(
            IIapProvider provider,
            EconomyController economyController,
            AbilityInventory abilityInventory,
            PlantUnlockService plantUnlockService,
            ShopInventory inventory,
            IEnumerable<string> processedTransactionIds = null,
            Action<string> processedTransactionAdded = null)
        {
            this.provider = provider;
            this.economyController = economyController;
            this.abilityInventory = abilityInventory;
            this.plantUnlockService = plantUnlockService;
            this.inventory = inventory;
            this.processedTransactionAdded = processedTransactionAdded;

            if (processedTransactionIds != null)
            {
                foreach (string transactionId in processedTransactionIds)
                {
                    if (!string.IsNullOrWhiteSpace(transactionId))
                    {
                        grantedTransactionIds.Add(transactionId);
                    }
                }
            }
        }

        public string[] GetProcessedTransactionIds()
        {
            string[] result = new string[grantedTransactionIds.Count];
            grantedTransactionIds.CopyTo(result);
            return result;
        }

        public IapProductPurchaseResult Purchase(ShopItemDefinition item)
        {
            if (item == null ||
                item.Price == null ||
                item.Price.PurchaseKind != ShopPurchaseKind.Iap ||
                item.Grant == null)
            {
                return new IapProductPurchaseResult(IapPurchaseStatus.InvalidProduct, item, string.Empty, default);
            }

            if (!item.Repeatable && inventory != null && inventory.IsProductPurchased(item.ProductId))
            {
                return new IapProductPurchaseResult(IapPurchaseStatus.AlreadyOwned, item, string.Empty, default);
            }

            if (provider == null || !provider.IsProductAvailable(item.Price.IapProductId))
            {
                return new IapProductPurchaseResult(IapPurchaseStatus.ProductUnavailable, item, string.Empty, default);
            }

            IapPurchaseResult purchaseResult = provider.Purchase(item.Price.IapProductId);
            if (purchaseResult.Status == IapPurchaseStatus.Pending)
            {
                return new IapProductPurchaseResult(IapPurchaseStatus.Pending, item, purchaseResult.TransactionId, default);
            }

            return CompletePurchase(item, purchaseResult);
        }

        public IapProductPurchaseResult CompletePurchase(ShopItemDefinition item, IapPurchaseResult purchaseResult)
        {
            if (item == null ||
                item.Price == null ||
                item.Price.PurchaseKind != ShopPurchaseKind.Iap ||
                item.Grant == null)
            {
                return new IapProductPurchaseResult(IapPurchaseStatus.InvalidProduct, item, purchaseResult.TransactionId, default);
            }

            if (!item.Repeatable && inventory != null && inventory.IsProductPurchased(item.ProductId))
            {
                return new IapProductPurchaseResult(IapPurchaseStatus.AlreadyOwned, item, purchaseResult.TransactionId, default);
            }

            if (!purchaseResult.Succeeded)
            {
                return new IapProductPurchaseResult(purchaseResult.Status, item, purchaseResult.TransactionId, default);
            }

            if (!string.IsNullOrWhiteSpace(purchaseResult.TransactionId) &&
                !grantedTransactionIds.Add(purchaseResult.TransactionId))
            {
                return new IapProductPurchaseResult(IapPurchaseStatus.DuplicateTransaction, item, purchaseResult.TransactionId, default);
            }

            if (!string.IsNullOrWhiteSpace(purchaseResult.TransactionId))
            {
                processedTransactionAdded?.Invoke(purchaseResult.TransactionId);
            }

            RewardGrantResult rewardResult = RewardService.Grant(
                item.Grant,
                economyController,
                abilityInventory,
                plantUnlockService);

            if (inventory != null)
            {
                inventory.AddDecorations(item.Grant.DecorationIds);
                if (!item.Repeatable)
                {
                    inventory.MarkProductPurchased(item.ProductId);
                }
            }

            return new IapProductPurchaseResult(
                IapPurchaseStatus.Success,
                item,
                purchaseResult.TransactionId,
                rewardResult);
        }
    }
}
