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
            float storeBuyWidth = ShopUiLayoutMetrics.Width(
                ShopUiLayoutMetrics.StoreBuyAnchorMin,
                ShopUiLayoutMetrics.StoreBuyAnchorMax,
                contentWidth);
            float textWidth = ShopUiLayoutMetrics.Width(
                ShopUiLayoutMetrics.NameAnchorMin,
                ShopUiLayoutMetrics.NameAnchorMax,
                contentWidth);
            Rect tabs = AndroidHudLayoutMetrics.ToPixelRect(
                PanelUiLayoutMetrics.ShopCategoryAnchorMin,
                PanelUiLayoutMetrics.ShopCategoryAnchorMax,
                panel.size);
            Rect summary = AndroidHudLayoutMetrics.ToPixelRect(
                ShopUiLayoutMetrics.SummaryAnchorMin,
                ShopUiLayoutMetrics.SummaryAnchorMax,
                panel.size);
            Rect content = AndroidHudLayoutMetrics.ToPixelRect(
                ShopUiLayoutMetrics.ContentAnchorMin,
                ShopUiLayoutMetrics.ContentAnchorMax,
                panel.size);

            Assert.GreaterOrEqual(categoryTabWidth, ShopUiLayoutMetrics.MinimumCategoryTabWidth);
            Assert.GreaterOrEqual(priceWidth, ShopUiLayoutMetrics.MinimumPriceBadgeWidth);
            Assert.GreaterOrEqual(buyWidth, ShopUiLayoutMetrics.MinimumBuyButtonWidth);
            Assert.GreaterOrEqual(storeBuyWidth, ShopUiLayoutMetrics.MinimumBuyButtonWidth);
            Assert.GreaterOrEqual(textWidth, ShopUiLayoutMetrics.MinimumProductTextWidth);
            Assert.GreaterOrEqual(ShopUiLayoutMetrics.ProductRowHeight, 180f);
            Assert.GreaterOrEqual(summary.height, ShopUiLayoutMetrics.MinimumShopSummaryHeight);
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(tabs, summary, 8f));
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(summary, content, 8f));
        }
    }
}
