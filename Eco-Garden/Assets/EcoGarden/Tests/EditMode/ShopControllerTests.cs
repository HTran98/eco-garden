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
    public sealed class ShopControllerTests
    {
        private GameObject gameObject;
        private EconomyController economyController;
        private BoardController boardController;
        private ShopController shopController;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject("ShopControllerTests");
            economyController = gameObject.AddComponent<EconomyController>();
            gameObject.AddComponent<BoardView>();
            boardController = gameObject.AddComponent<BoardController>();
            boardController.SetLevelDefinition(TestLevelFactory.CreateLevel15());
            boardController.LoadLevel();
            shopController = gameObject.AddComponent<ShopController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void TryPurchase_SpendsGoldAndGrantsAbilityReward()
        {
            economyController.SetGold(100);
            shopController.SetCatalogItems(new[]
            {
                CreateItem(
                    "shop_booster_shovel_small",
                    ShopItemCategory.Booster,
                    new ShopPriceDefinition(ShopPurchaseKind.Gold, 40),
                    new RewardDefinition(null, new[] { new AbilityReward(AbilityKind.Shovel, 2) }),
                    true)
            });
            int startingShovels = boardController.AbilityInventory.GetCount(AbilityKind.Shovel);

            ShopPurchaseResult result = shopController.TryPurchase("shop_booster_shovel_small");

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(60, economyController.Gold);
            Assert.AreEqual(startingShovels + 2, boardController.AbilityInventory.GetCount(AbilityKind.Shovel));
        }

        [Test]
        public void TryPurchase_FailsWhenCurrencyIsInsufficient()
        {
            economyController.SetGold(10);
            shopController.SetCatalogItems(new[]
            {
                CreateItem(
                    "shop_booster_wand_small",
                    ShopItemCategory.Booster,
                    new ShopPriceDefinition(ShopPurchaseKind.Gold, 40),
                    new RewardDefinition(null, new[] { new AbilityReward(AbilityKind.MagicWand, 1) }),
                    true)
            });

            ShopPurchaseResult result = shopController.TryPurchase("shop_booster_wand_small");

            Assert.AreEqual(ShopPurchaseStatus.InsufficientCurrency, result.Status);
            Assert.AreEqual(10, economyController.Gold);
        }

        [Test]
        public void TryPurchase_BlocksRepeatedNonRepeatablePurchase()
        {
            economyController.SetGold(500);
            shopController.SetCatalogItems(new[]
            {
                CreateItem(
                    "shop_unlock_lotus_tier_4",
                    ShopItemCategory.Unlock,
                    new ShopPriceDefinition(ShopPurchaseKind.Gold, 100),
                    new RewardDefinition(null, new[] { new AbilityReward(AbilityKind.Shovel, 1) }),
                    false)
            });

            ShopPurchaseResult first = shopController.TryPurchase("shop_unlock_lotus_tier_4");
            ShopPurchaseResult second = shopController.TryPurchase("shop_unlock_lotus_tier_4");

            Assert.IsTrue(first.Succeeded);
            Assert.AreEqual(ShopPurchaseStatus.AlreadyOwned, second.Status);
            Assert.AreEqual(400, economyController.Gold);
            Assert.IsTrue(shopController.Inventory.IsProductPurchased("shop_unlock_lotus_tier_4"));
        }

        [Test]
        public void TryPurchase_DoesNotSellDeferredDecorationProductsByDefault()
        {
            economyController.SetGold(500);
            shopController.SetCatalogItems(new[]
            {
                CreateItem(
                    "shop_deco_butterfly",
                    ShopItemCategory.Decoration,
                    new ShopPriceDefinition(ShopPurchaseKind.Gold, 100),
                    new RewardDefinition(null, null, new[] { "deco_butterfly_variant" }),
                    false)
            });

            ShopPurchaseResult result = shopController.TryPurchase("shop_deco_butterfly");

            Assert.AreEqual(ShopPurchaseStatus.ProductNotFound, result.Status);
            Assert.AreEqual(500, economyController.Gold);
            Assert.IsFalse(shopController.Inventory.IsProductPurchased("shop_deco_butterfly"));
            Assert.IsFalse(shopController.Inventory.IsDecorationOwned("deco_butterfly_variant"));
        }

        [Test]
        public void TryPurchase_RejectsIapProductsUntilProviderExists()
        {
            shopController.SetCatalogItems(new[]
            {
                CreateItem(
                    "shop_iap_gems_small",
                    ShopItemCategory.Currency,
                    new ShopPriceDefinition(ShopPurchaseKind.Iap, 0, "eco_garden_gems_small"),
                    new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null),
                    true)
            });

            ShopPurchaseResult result = shopController.TryPurchase("shop_iap_gems_small");

            Assert.AreEqual(ShopPurchaseStatus.UnsupportedPurchaseKind, result.Status);
            Assert.AreEqual(0, economyController.Gem);
        }

        [Test]
        public void TryPurchase_IapProductUsesMockProviderAndGrantsReward()
        {
            gameObject.AddComponent<MockIapProvider>();
            shopController.SetCatalogItems(new[]
            {
                CreateItem(
                    "shop_iap_gems_small",
                    ShopItemCategory.Currency,
                    new ShopPriceDefinition(ShopPurchaseKind.Iap, 0, "eco_garden_gems_small"),
                    new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null),
                    true)
            });

            ShopPurchaseResult result = shopController.TryPurchase("shop_iap_gems_small");

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(ShopPurchaseStatus.Success, result.Status);
            Assert.AreEqual(80, economyController.Gem);
        }

        [Test]
        public void TryPurchase_IapCancelledDoesNotGrantReward()
        {
            MockIapProvider provider = gameObject.AddComponent<MockIapProvider>();
            provider.SetNextOutcome(MockIapOutcome.Cancelled);
            shopController.SetCatalogItems(new[]
            {
                CreateItem(
                    "shop_iap_gems_small",
                    ShopItemCategory.Currency,
                    new ShopPriceDefinition(ShopPurchaseKind.Iap, 0, "eco_garden_gems_small"),
                    new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null),
                    true)
            });

            ShopPurchaseResult result = shopController.TryPurchase("shop_iap_gems_small");

            Assert.AreEqual(ShopPurchaseStatus.IapCancelled, result.Status);
            Assert.AreEqual(0, economyController.Gem);
        }

        [Test]
        public void TryPurchase_RestoredIapTransactionRejectsDuplicateAfterRestart()
        {
            MockIapProvider provider = gameObject.AddComponent<MockIapProvider>();
            provider.SetFixedTransactionId("mock_tx_saved");
            shopController.RestoreProcessedIapTransactionIds(new[] { "mock_tx_saved" });
            shopController.SetCatalogItems(new[]
            {
                CreateItem(
                    "shop_iap_gems_small",
                    ShopItemCategory.Currency,
                    new ShopPriceDefinition(ShopPurchaseKind.Iap, 0, "eco_garden_gems_small"),
                    new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null),
                    true)
            });

            ShopPurchaseResult result = shopController.TryPurchase("shop_iap_gems_small");

            Assert.AreEqual(ShopPurchaseStatus.DuplicateTransaction, result.Status);
            Assert.AreEqual(0, economyController.Gem);
        }

        private static ShopItemDefinition CreateItem(
            string productId,
            ShopItemCategory category,
            ShopPriceDefinition price,
            RewardDefinition grant,
            bool repeatable)
        {
            ShopItemDefinition item = ScriptableObject.CreateInstance<ShopItemDefinition>();
            item.EditorSetValues(productId, productId, string.Empty, category, price, grant, repeatable);
            return item;
        }
    }
}
