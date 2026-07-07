using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class InventoryUiLayoutMetricsTests
    {
        [Test]
        public void PanelSummaryAndContent_DoNotOverlapOnSmallPortraitProfile()
        {
            Vector2 panelSize = AndroidHudLayoutMetrics.ToPixelRect(
                AndroidHudLayoutMetrics.PanelAnchorMin,
                AndroidHudLayoutMetrics.PanelAnchorMax,
                new Vector2(720f, 1280f)).size;

            Rect summary = AndroidHudLayoutMetrics.ToPixelRect(
                InventoryUiLayoutMetrics.SummaryAnchorMin,
                InventoryUiLayoutMetrics.SummaryAnchorMax,
                panelSize);
            Rect content = AndroidHudLayoutMetrics.ToPixelRect(
                InventoryUiLayoutMetrics.ContentAnchorMin,
                InventoryUiLayoutMetrics.ContentAnchorMax,
                panelSize);
            Rect action = AndroidHudLayoutMetrics.ToPixelRect(
                InventoryUiLayoutMetrics.ActionAnchorMin,
                InventoryUiLayoutMetrics.ActionAnchorMax,
                new Vector2(640f, InventoryUiLayoutMetrics.ItemRowHeight));
            Rect icon = AndroidHudLayoutMetrics.ToPixelRect(
                InventoryUiLayoutMetrics.IconAnchorMin,
                InventoryUiLayoutMetrics.IconAnchorMax,
                new Vector2(640f, InventoryUiLayoutMetrics.ItemRowHeight));

            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(summary, content, 8f));
            Assert.GreaterOrEqual(summary.height, InventoryUiLayoutMetrics.MinimumSummaryHeight);
            Assert.GreaterOrEqual(content.height, InventoryUiLayoutMetrics.MinimumContentHeight);
            Assert.GreaterOrEqual(action.width, InventoryUiLayoutMetrics.MinimumActionWidth);
            Assert.GreaterOrEqual(icon.width, InventoryUiLayoutMetrics.MinimumIconPreviewWidth);
        }
    }
}
