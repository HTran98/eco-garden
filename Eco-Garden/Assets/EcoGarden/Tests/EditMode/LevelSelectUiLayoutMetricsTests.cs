using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class LevelSelectUiLayoutMetricsTests
    {
        [Test]
        public void LevelRows_KeepSummaryAndActionReadableOnSmallPortraitProfile()
        {
            Vector2 screenSize = new Vector2(720f, 1280f);
            Rect panel = AndroidHudLayoutMetrics.ToPixelRect(
                AndroidHudLayoutMetrics.PanelAnchorMin,
                AndroidHudLayoutMetrics.PanelAnchorMax,
                screenSize);
            float contentWidth = panel.width * 0.92f;
            float titleWidth = LevelSelectUiLayoutMetrics.Width(
                LevelSelectUiLayoutMetrics.TitleAnchorMin,
                LevelSelectUiLayoutMetrics.TitleAnchorMax,
                contentWidth);
            float actionWidth = LevelSelectUiLayoutMetrics.Width(
                LevelSelectUiLayoutMetrics.ActionAnchorMin,
                LevelSelectUiLayoutMetrics.ActionAnchorMax,
                contentWidth);
            Rect summary = AndroidHudLayoutMetrics.ToPixelRect(
                LevelSelectUiLayoutMetrics.PanelSummaryAnchorMin,
                LevelSelectUiLayoutMetrics.PanelSummaryAnchorMax,
                panel.size);
            Rect content = AndroidHudLayoutMetrics.ToPixelRect(
                LevelSelectUiLayoutMetrics.PanelContentAnchorMin,
                LevelSelectUiLayoutMetrics.PanelContentAnchorMax,
                panel.size);

            Assert.GreaterOrEqual(LevelSelectUiLayoutMetrics.LevelRowHeight, 120f);
            Assert.GreaterOrEqual(titleWidth, LevelSelectUiLayoutMetrics.MinimumTitleWidth);
            Assert.GreaterOrEqual(actionWidth, LevelSelectUiLayoutMetrics.MinimumActionWidth);
            Assert.GreaterOrEqual(summary.height, LevelSelectUiLayoutMetrics.MinimumLevelSummaryHeight);
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(summary, content, 8f));
        }

        [Test]
        public void LevelPreview_KeepsActionsReadableOnSmallPortraitProfile()
        {
            Vector2 screenSize = new Vector2(720f, 1280f);
            Rect panel = AndroidHudLayoutMetrics.ToPixelRect(
                AndroidHudLayoutMetrics.PanelAnchorMin,
                AndroidHudLayoutMetrics.PanelAnchorMax,
                screenSize);
            Rect preview = AndroidHudLayoutMetrics.ToPixelRect(
                LevelSelectUiLayoutMetrics.PreviewAnchorMin,
                LevelSelectUiLayoutMetrics.PreviewAnchorMax,
                panel.size);
            Rect play = AndroidHudLayoutMetrics.ToPixelRect(
                LevelSelectUiLayoutMetrics.PreviewPlayAnchorMin,
                LevelSelectUiLayoutMetrics.PreviewPlayAnchorMax,
                preview.size);
            Rect close = AndroidHudLayoutMetrics.ToPixelRect(
                LevelSelectUiLayoutMetrics.PreviewCloseAnchorMin,
                LevelSelectUiLayoutMetrics.PreviewCloseAnchorMax,
                preview.size);
            Rect title = AndroidHudLayoutMetrics.ToPixelRect(
                LevelSelectUiLayoutMetrics.PreviewTitleAnchorMin,
                LevelSelectUiLayoutMetrics.PreviewTitleAnchorMax,
                preview.size);
            Rect objective = AndroidHudLayoutMetrics.ToPixelRect(
                LevelSelectUiLayoutMetrics.PreviewObjectiveAnchorMin,
                LevelSelectUiLayoutMetrics.PreviewObjectiveAnchorMax,
                preview.size);

            Assert.GreaterOrEqual(play.width, 100f);
            Assert.GreaterOrEqual(close.width, 100f);
            Assert.GreaterOrEqual(play.height, 64f);
            Assert.GreaterOrEqual(close.height, 64f);
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(title, objective, 4f));
        }
    }
}
