using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class ShopUiLayoutMetricsTests
    {
        [Test]
        public void ShopPanel_KeepsControlsReadableOnSmallPortraitProfile()
        {
            Vector2 screenSize = new Vector2(720f, 1280f);
            Rect panel = AndroidHudLayoutMetrics.ToPixelRect(
                AndroidHudLayoutMetrics.PanelAnchorMin,
                AndroidHudLayoutMetrics.PanelAnchorMax,
                screenSize);
            float contentWidth = panel.width * 0.92f;
            float categoryTabWidth = contentWidth * 0.19f;
            float priceWidth = ShopUiLayoutMetrics.Width(
                ShopUiLayoutMetrics.PriceAnchorMin,
                ShopUiLayoutMetrics.PriceAnchorMax,
                contentWidth);
            float buyWidth = ShopUiLayoutMetrics.Width(
                ShopUiLayoutMetrics.BuyAnchorMin,
                ShopUiLayoutMetrics.BuyAnchorMax,
                contentWidth);
            float textWidth = ShopUiLayoutMetrics.Width(
                ShopUiLayoutMetrics.NameAnchorMin,
                ShopUiLayoutMetrics.NameAnchorMax,
                contentWidth);

            Assert.GreaterOrEqual(categoryTabWidth, ShopUiLayoutMetrics.MinimumCategoryTabWidth);
            Assert.GreaterOrEqual(priceWidth, ShopUiLayoutMetrics.MinimumPriceBadgeWidth);
            Assert.GreaterOrEqual(buyWidth, ShopUiLayoutMetrics.MinimumBuyButtonWidth);
            Assert.GreaterOrEqual(textWidth, ShopUiLayoutMetrics.MinimumProductTextWidth);
            Assert.GreaterOrEqual(ShopUiLayoutMetrics.ProductRowHeight, 112f);
        }
    }
}
