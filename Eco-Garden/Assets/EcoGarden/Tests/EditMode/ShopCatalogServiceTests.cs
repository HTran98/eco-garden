using EcoGarden.Abilities;
using EcoGarden.Economy;
using EcoGarden.Rewards;
using EcoGarden.Shop;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class ShopCatalogServiceTests
    {
        [Test]
        public void Catalog_IgnoresInvalidAndDuplicateProductIds()
        {
            ShopItemDefinition first = CreateItem("shop_booster_shovel_small", ShopItemCategory.Booster);
            ShopItemDefinition duplicate = CreateItem("shop_booster_shovel_small", ShopItemCategory.Booster);
            ShopItemDefinition invalid = ScriptableObject.CreateInstance<ShopItemDefinition>();

            ShopCatalogService catalog = new ShopCatalogService(new[] { first, duplicate, invalid });

            Assert.AreEqual(1, catalog.Items.Count);
            Assert.IsTrue(catalog.TryGetItem("shop_booster_shovel_small", out ShopItemDefinition found));
            Assert.AreSame(first, found);

            Object.DestroyImmediate(first);
            Object.DestroyImmediate(duplicate);
            Object.DestroyImmediate(invalid);
        }

        [Test]
        public void Catalog_FiltersItemsByCategory()
        {
            ShopItemDefinition booster = CreateItem("shop_booster_wand_small", ShopItemCategory.Booster);
            ShopItemDefinition unlock = CreateItem("shop_unlock_lotus_tier_4", ShopItemCategory.Unlock);
            ShopCatalogService catalog = new ShopCatalogService(new[] { booster, unlock });

            var boosters = catalog.GetItemsByCategory(ShopItemCategory.Booster);
            var unlocks = catalog.GetItemsByCategory(ShopItemCategory.Unlock);

            Assert.AreEqual(1, boosters.Count);
            Assert.AreSame(booster, boosters[0]);
            Assert.AreEqual(1, unlocks.Count);
            Assert.AreSame(unlock, unlocks[0]);

            Object.DestroyImmediate(booster);
            Object.DestroyImmediate(unlock);
        }

        [Test]
        public void Catalog_ExcludesDecorationItemsByDefault()
        {
            ShopItemDefinition booster = CreateItem("shop_booster_shovel_small", ShopItemCategory.Booster);
            ShopItemDefinition decoration = CreateItem("shop_deco_butterfly", ShopItemCategory.Decoration);
            ShopCatalogService catalog = new ShopCatalogService(new[] { booster, decoration });

            Assert.AreEqual(1, catalog.Items.Count);
            Assert.IsFalse(catalog.TryGetItem("shop_deco_butterfly", out _));
            Assert.AreEqual(0, catalog.GetItemsByCategory(ShopItemCategory.Decoration).Count);

            Object.DestroyImmediate(booster);
            Object.DestroyImmediate(decoration);
        }

        [Test]
        public void Catalog_CanIncludeDecorationItemsForFutureCosmeticBuilds()
        {
            ShopItemDefinition decoration = CreateItem("shop_deco_butterfly", ShopItemCategory.Decoration);
            ShopCatalogService catalog = new ShopCatalogService(new[] { decoration }, true);

            Assert.AreEqual(1, catalog.Items.Count);
            Assert.IsTrue(catalog.TryGetItem("shop_deco_butterfly", out ShopItemDefinition found));
            Assert.AreSame(decoration, found);

            Object.DestroyImmediate(decoration);
        }

        [Test]
        public void PriceDefinition_MapsPurchaseKindToCurrency()
        {
            ShopPriceDefinition goldPrice = new ShopPriceDefinition(ShopPurchaseKind.Gold, 50);
            ShopPriceDefinition gemPrice = new ShopPriceDefinition(ShopPurchaseKind.Gem, 5);
            ShopPriceDefinition iapPrice = new ShopPriceDefinition(ShopPurchaseKind.Iap, 0, "eco_garden_gems_small");

            Assert.IsTrue(goldPrice.IsSoftCurrency);
            Assert.AreEqual(CurrencyKind.Gold, goldPrice.CurrencyKind);
            Assert.AreEqual(CurrencyKind.Gem, gemPrice.CurrencyKind);
            Assert.IsFalse(iapPrice.IsSoftCurrency);
            Assert.AreEqual("eco_garden_gems_small", iapPrice.IapProductId);
        }

        private static ShopItemDefinition CreateItem(string productId, ShopItemCategory category)
        {
            ShopItemDefinition item = ScriptableObject.CreateInstance<ShopItemDefinition>();
            item.EditorSetValues(
                productId,
                productId,
                string.Empty,
                category,
                new ShopPriceDefinition(ShopPurchaseKind.Gold, 10),
                new RewardDefinition(null, new[] { new AbilityReward(AbilityKind.Shovel, 1) }),
                true);
            return item;
        }
    }
}
