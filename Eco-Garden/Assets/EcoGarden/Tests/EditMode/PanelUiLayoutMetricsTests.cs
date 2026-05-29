using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class PanelUiLayoutMetricsTests
    {
        [Test]
        public void PanelHeader_KeepsTitleAndCloseButtonReadableOnSmallPortraitProfile()
        {
            Vector2 screenSize = new Vector2(720f, 1280f);
            Rect panel = AndroidHudLayoutMetrics.ToPixelRect(
                AndroidHudLayoutMetrics.PanelAnchorMin,
                AndroidHudLayoutMetrics.PanelAnchorMax,
                screenSize);
            Rect title = AndroidHudLayoutMetrics.ToPixelRect(
                PanelUiLayoutMetrics.TitleAnchorMin,
                PanelUiLayoutMetrics.TitleAnchorMax,
                panel.size);
            Rect close = AndroidHudLayoutMetrics.ToPixelRect(
                PanelUiLayoutMetrics.CloseAnchorMin,
                PanelUiLayoutMetrics.CloseAnchorMax,
                panel.size);

            Assert.GreaterOrEqual(title.width, PanelUiLayoutMetrics.MinimumPanelTitleWidth);
            Assert.GreaterOrEqual(close.width, PanelUiLayoutMetrics.MinimumPanelCloseWidth);
            Assert.GreaterOrEqual(close.height, PanelUiLayoutMetrics.MinimumPanelCloseHeight);
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(title, close, 8f));
        }

        [Test]
        public void ShopPanel_ContentAreasLeaveRoomForHeaderAndTabs()
        {
            Vector2 screenSize = new Vector2(720f, 1280f);
            Rect panel = AndroidHudLayoutMetrics.ToPixelRect(
                AndroidHudLayoutMetrics.PanelAnchorMin,
                AndroidHudLayoutMetrics.PanelAnchorMax,
                screenSize);
            Rect title = AndroidHudLayoutMetrics.ToPixelRect(
                PanelUiLayoutMetrics.TitleAnchorMin,
                PanelUiLayoutMetrics.TitleAnchorMax,
                panel.size);
            Rect tabs = AndroidHudLayoutMetrics.ToPixelRect(
                PanelUiLayoutMetrics.ShopCategoryAnchorMin,
                PanelUiLayoutMetrics.ShopCategoryAnchorMax,
                panel.size);
            Rect content = AndroidHudLayoutMetrics.ToPixelRect(
                PanelUiLayoutMetrics.ShopContentAnchorMin,
                PanelUiLayoutMetrics.ShopContentAnchorMax,
                panel.size);

            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(title, tabs, 8f));
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(tabs, content, 8f));
            Assert.Greater(content.height, tabs.height * 5f);
        }

        [Test]
        public void ResultActions_KeepTouchSizeOnSmallPortraitProfile()
        {
            Vector2 screenSize = new Vector2(720f, 1280f);
            Rect resultPanel = AndroidHudLayoutMetrics.ToPixelRect(
                AndroidHudLayoutMetrics.ResultAnchorMin,
                AndroidHudLayoutMetrics.ResultAnchorMax,
                screenSize);
            Rect restart = AndroidHudLayoutMetrics.ToPixelRect(
                PanelUiLayoutMetrics.ResultRestartAnchorMin,
                PanelUiLayoutMetrics.ResultRestartAnchorMax,
                resultPanel.size);
            Rect next = AndroidHudLayoutMetrics.ToPixelRect(
                PanelUiLayoutMetrics.ResultNextAnchorMin,
                PanelUiLayoutMetrics.ResultNextAnchorMax,
                resultPanel.size);
            Rect countdown = AndroidHudLayoutMetrics.ToPixelRect(
                PanelUiLayoutMetrics.ResultCountdownAnchorMin,
                PanelUiLayoutMetrics.ResultCountdownAnchorMax,
                resultPanel.size);
            Rect message = AndroidHudLayoutMetrics.ToPixelRect(
                PanelUiLayoutMetrics.ResultMessageAnchorMin,
                PanelUiLayoutMetrics.ResultMessageAnchorMax,
                resultPanel.size);

            Assert.GreaterOrEqual(restart.width, PanelUiLayoutMetrics.MinimumResultActionWidth);
            Assert.GreaterOrEqual(next.width, PanelUiLayoutMetrics.MinimumResultActionWidth);
            Assert.GreaterOrEqual(restart.height, PanelUiLayoutMetrics.MinimumResultActionHeight);
            Assert.GreaterOrEqual(next.height, PanelUiLayoutMetrics.MinimumResultActionHeight);
            Assert.GreaterOrEqual(countdown.height, PanelUiLayoutMetrics.MinimumResultCountdownHeight);
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(message, countdown, 6f));
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(countdown, restart, 6f));
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(countdown, next, 6f));
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(restart, next, 12f));
        }
    }
}
