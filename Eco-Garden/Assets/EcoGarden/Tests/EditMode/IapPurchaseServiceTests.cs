using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Economy;
using EcoGarden.IAP;
using EcoGarden.Rewards;
using EcoGarden.Shop;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class IapPurchaseServiceTests
    {
        private GameObject gameObject;
        private EconomyController economyController;
        private BoardController boardController;
        private MockIapProvider provider;
        private ShopInventory inventory;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject("IapPurchaseServiceTests");
            economyController = gameObject.AddComponent<EconomyController>();
            gameObject.AddComponent<BoardView>();
            boardController = gameObject.AddComponent<BoardController>();
            boardController.SetLevelDefinition(TestLevelFactory.CreateLevel15());
            boardController.LoadLevel();
            provider = gameObject.AddComponent<MockIapProvider>();
            inventory = new ShopInventory();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void Purchase_SuccessGrantsConfiguredReward()
        {
            ShopItemDefinition item = CreateIapItem(
                "shop_iap_gems_small",
                true,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null));
            IapPurchaseService service = CreateService();

            IapProductPurchaseResult result = service.Purchase(item);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(IapPurchaseStatus.Success, result.Status);
            Assert.AreEqual(80, economyController.Gem);
            Assert.IsTrue(result.RewardResult.HasAnyGrant);
            Object.DestroyImmediate(item);
        }

        [Test]
        public void Purchase_CancelledDoesNotGrantReward()
        {
            provider.SetNextOutcome(MockIapOutcome.Cancelled);
            ShopItemDefinition item = CreateIapItem(
                "shop_iap_gems_small",
                true,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null));
            IapPurchaseService service = CreateService();

            IapProductPurchaseResult result = service.Purchase(item);

            Assert.AreEqual(IapPurchaseStatus.Cancelled, result.Status);
            Assert.AreEqual(0, economyController.Gem);
            Object.DestroyImmediate(item);
        }

        [Test]
        public void Purchase_DuplicateTransactionDoesNotDoubleGrant()
        {
            provider.SetFixedTransactionId("mock_tx_fixed");
            ShopItemDefinition item = CreateIapItem(
                "shop_iap_gems_small",
                true,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null));
            IapPurchaseService service = CreateService();

            IapProductPurchaseResult first = service.Purchase(item);
            IapProductPurchaseResult second = service.Purchase(item);

            Assert.AreEqual(IapPurchaseStatus.Success, first.Status);
            Assert.AreEqual(IapPurchaseStatus.DuplicateTransaction, second.Status);
            Assert.AreEqual(80, economyController.Gem);
            Object.DestroyImmediate(item);
        }

        [Test]
        public void Purchase_SeededProcessedTransactionDoesNotGrantAfterRestart()
        {
            provider.SetFixedTransactionId("mock_tx_saved");
            ShopItemDefinition item = CreateIapItem(
                "shop_iap_gems_small",
                true,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null));
            IapPurchaseService service = CreateService(new[] { "mock_tx_saved" });

            IapProductPurchaseResult result = service.Purchase(item);

            Assert.AreEqual(IapPurchaseStatus.DuplicateTransaction, result.Status);
            Assert.AreEqual(0, economyController.Gem);
            Object.DestroyImmediate(item);
        }

        [Test]
        public void Purchase_SuccessExposesProcessedTransactionForSave()
        {
            provider.SetFixedTransactionId("mock_tx_to_save");
            ShopItemDefinition item = CreateIapItem(
                "shop_iap_gems_small",
                true,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null));
            string capturedTransactionId = string.Empty;
            IapPurchaseService service = CreateService(null, id => capturedTransactionId = id);

            IapProductPurchaseResult result = service.Purchase(item);

            Assert.AreEqual(IapPurchaseStatus.Success, result.Status);
            CollectionAssert.Contains(service.GetProcessedTransactionIds(), "mock_tx_to_save");
            Assert.AreEqual("mock_tx_to_save", capturedTransactionId);
            Object.DestroyImmediate(item);
        }

        [Test]
        public void CompletePurchase_PendingUnityResultGrantsOnce()
        {
            ShopItemDefinition item = CreateIapItem(
                "shop_iap_gems_small",
                true,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null));
            IapPurchaseService service = CreateService();

            IapProductPurchaseResult first = service.CompletePurchase(
                item,
                new IapPurchaseResult(IapPurchaseStatus.Success, item.Price.IapProductId, "unity_tx_001"));
            IapProductPurchaseResult duplicate = service.CompletePurchase(
                item,
                new IapPurchaseResult(IapPurchaseStatus.Success, item.Price.IapProductId, "unity_tx_001"));

            Assert.AreEqual(IapPurchaseStatus.Success, first.Status);
            Assert.AreEqual(IapPurchaseStatus.DuplicateTransaction, duplicate.Status);
            Assert.AreEqual(80, economyController.Gem);
            Object.DestroyImmediate(item);
        }

        [Test]
        public void Purchase_NonRepeatableProductMarksInventory()
        {
            ShopItemDefinition item = CreateIapItem(
                "shop_iap_deco",
                false,
                new RewardDefinition(null, null, new[] { "deco_premium" }));
            IapPurchaseService service = CreateService();

            IapProductPurchaseResult first = service.Purchase(item);
            IapProductPurchaseResult second = service.Purchase(item);

            Assert.AreEqual(IapPurchaseStatus.Success, first.Status);
            Assert.AreEqual(IapPurchaseStatus.AlreadyOwned, second.Status);
            Assert.IsTrue(inventory.IsProductPurchased("shop_iap_deco"));
            Assert.IsTrue(inventory.IsDecorationOwned("deco_premium"));
            Object.DestroyImmediate(item);
        }

        private IapPurchaseService CreateService(
            string[] processedTransactionIds = null,
            System.Action<string> processedTransactionAdded = null)
        {
            return new IapPurchaseService(
                provider,
                economyController,
                boardController.AbilityInventory,
                boardController.PlantUnlockService,
                inventory,
                processedTransactionIds,
                processedTransactionAdded);
        }

        private static ShopItemDefinition CreateIapItem(
            string productId,
            bool repeatable,
            RewardDefinition grant)
        {
            ShopItemDefinition item = ScriptableObject.CreateInstance<ShopItemDefinition>();
            item.EditorSetValues(
                productId,
                productId,
                string.Empty,
                ShopItemCategory.Currency,
                new ShopPriceDefinition(ShopPurchaseKind.Iap, 0, productId),
                grant,
                repeatable);
            return item;
        }
    }
}
