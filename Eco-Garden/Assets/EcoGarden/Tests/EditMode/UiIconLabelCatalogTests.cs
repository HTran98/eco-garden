using EcoGarden.Abilities;
using EcoGarden.Economy;
using EcoGarden.UI;
using NUnit.Framework;

namespace EcoGarden.Tests.EditMode
{
    public sealed class UiIconLabelCatalogTests
    {
        [Test]
        public void AbilityLabels_StayCompactForMobileButtons()
        {
            Assert.AreEqual("SH\nx2", UiIconLabelCatalog.AbilityWithCount(AbilityKind.Shovel, 2));
            Assert.AreEqual("WD\nx1", UiIconLabelCatalog.AbilityWithCount(AbilityKind.MagicWand, 1));
            Assert.AreEqual("MG\nx1", UiIconLabelCatalog.AbilityWithCount(AbilityKind.SortingMagnet, 1));
        }

        [Test]
        public void CurrencyLabels_UseCompactRuntimePlaceholders()
        {
            Assert.AreEqual("G", UiIconLabelCatalog.Currency(CurrencyKind.Gold));
            Assert.AreEqual("*", UiIconLabelCatalog.Currency(CurrencyKind.Gem));
        }

        [Test]
        public void ActionLabels_AreShortEnoughForTopBarAndResultButtons()
        {
            Assert.LessOrEqual(UiIconLabelCatalog.Level.Length, 4);
            Assert.LessOrEqual(UiIconLabelCatalog.Mission.Length, 4);
            Assert.LessOrEqual(UiIconLabelCatalog.Shop.Length, 2);
            Assert.LessOrEqual(UiIconLabelCatalog.Pause.Length, 2);
            Assert.LessOrEqual(UiIconLabelCatalog.Restart.Length, 2);
            Assert.LessOrEqual(UiIconLabelCatalog.Next.Length, 2);
        }
    }
}
