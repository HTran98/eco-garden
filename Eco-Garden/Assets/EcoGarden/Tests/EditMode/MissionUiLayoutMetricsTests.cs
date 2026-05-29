using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class MissionUiLayoutMetricsTests
    {
        [Test]
        public void MissionRows_KeepTextAndClaimButtonReadableOnSmallPortraitProfile()
        {
            Vector2 screenSize = new Vector2(720f, 1280f);
            Rect panel = AndroidHudLayoutMetrics.ToPixelRect(
                AndroidHudLayoutMetrics.PanelAnchorMin,
                AndroidHudLayoutMetrics.PanelAnchorMax,
                screenSize);
            float contentWidth = panel.width * 0.92f;
            float textWidth = MissionUiLayoutMetrics.Width(
                MissionUiLayoutMetrics.RowTitleAnchorMin,
                MissionUiLayoutMetrics.RowTitleAnchorMax,
                contentWidth);
            float claimWidth = MissionUiLayoutMetrics.Width(
                MissionUiLayoutMetrics.RowClaimAnchorMin,
                MissionUiLayoutMetrics.RowClaimAnchorMax,
                contentWidth);
            Rect summary = AndroidHudLayoutMetrics.ToPixelRect(
                MissionUiLayoutMetrics.SummaryAnchorMin,
                MissionUiLayoutMetrics.SummaryAnchorMax,
                panel.size);
            Rect content = AndroidHudLayoutMetrics.ToPixelRect(
                MissionUiLayoutMetrics.ContentAnchorMin,
                MissionUiLayoutMetrics.ContentAnchorMax,
                panel.size);

            Assert.GreaterOrEqual(MissionUiLayoutMetrics.MissionRowHeight, 120f);
            Assert.GreaterOrEqual(textWidth, MissionUiLayoutMetrics.MinimumMissionTextWidth);
            Assert.GreaterOrEqual(claimWidth, MissionUiLayoutMetrics.MinimumMissionClaimWidth);
            Assert.GreaterOrEqual(summary.height, MissionUiLayoutMetrics.MinimumMissionSummaryHeight);
            Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(summary, content, 8f));
        }

        [Test]
        public void CompactTrackerRows_KeepClaimButtonReachableOnSmallPortraitProfile()
        {
            Vector2 screenSize = new Vector2(720f, 1280f);
            Rect tracker = AndroidHudLayoutMetrics.ToPixelRect(
                AndroidHudLayoutMetrics.MissionTrackerAnchorMin,
                AndroidHudLayoutMetrics.MissionTrackerAnchorMax,
                screenSize);
            float contentWidth = tracker.width * 0.92f;
            float textWidth = MissionUiLayoutMetrics.Width(
                MissionUiLayoutMetrics.TrackerTitleAnchorMin,
                MissionUiLayoutMetrics.TrackerTitleAnchorMax,
                contentWidth);
            float claimWidth = MissionUiLayoutMetrics.Width(
                MissionUiLayoutMetrics.TrackerClaimAnchorMin,
                MissionUiLayoutMetrics.TrackerClaimAnchorMax,
                contentWidth);

            Assert.GreaterOrEqual(MissionUiLayoutMetrics.TrackerRowHeight, 88f);
            Assert.GreaterOrEqual(textWidth, MissionUiLayoutMetrics.MinimumTrackerTextWidth);
            Assert.GreaterOrEqual(claimWidth, MissionUiLayoutMetrics.MinimumTrackerClaimWidth);
        }
    }
}
