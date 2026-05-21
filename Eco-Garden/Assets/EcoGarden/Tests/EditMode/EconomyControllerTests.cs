using EcoGarden.Economy;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class EconomyControllerTests
    {
        private GameObject gameObject;
        private EconomyController economyController;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject("EconomyControllerTests");
            economyController = gameObject.AddComponent<EconomyController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void AddCurrency_UpdatesGoldAndGemIndependently()
        {
            economyController.AddCurrency(CurrencyKind.Gold, 12);
            economyController.AddCurrency(CurrencyKind.Gem, 3);

            Assert.AreEqual(12, economyController.Gold);
            Assert.AreEqual(3, economyController.Gem);
            Assert.AreEqual(12, economyController.GetBalance(CurrencyKind.Gold));
            Assert.AreEqual(3, economyController.GetBalance(CurrencyKind.Gem));
        }

        [Test]
        public void TrySpendCurrency_OnlySpendsRequestedCurrency()
        {
            economyController.SetGold(10);
            economyController.SetGem(2);

            bool spentGold = economyController.TrySpendCurrency(CurrencyKind.Gold, 4);
            bool spentGem = economyController.TrySpendCurrency(CurrencyKind.Gem, 3);

            Assert.IsTrue(spentGold);
            Assert.IsFalse(spentGem);
            Assert.AreEqual(6, economyController.Gold);
            Assert.AreEqual(2, economyController.Gem);
        }

        [Test]
        public void SetCurrency_ClampsNegativeBalancesToZero()
        {
            economyController.SetCurrency(CurrencyKind.Gold, -10);
            economyController.SetCurrency(CurrencyKind.Gem, -5);

            Assert.AreEqual(0, economyController.Gold);
            Assert.AreEqual(0, economyController.Gem);
        }
    }
}
