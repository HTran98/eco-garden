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

            Assert.GreaterOrEqual(LevelSelectUiLayoutMetrics.LevelRowHeight, 120f);
            Assert.GreaterOrEqual(titleWidth, LevelSelectUiLayoutMetrics.MinimumTitleWidth);
            Assert.GreaterOrEqual(actionWidth, LevelSelectUiLayoutMetrics.MinimumActionWidth);
        }
    }
}
