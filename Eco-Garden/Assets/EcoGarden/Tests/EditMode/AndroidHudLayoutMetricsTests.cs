using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class AndroidHudLayoutMetricsTests
    {
        private static readonly Vector2[] TargetProfiles =
        {
            new Vector2(720f, 1280f),
            new Vector2(1080f, 1920f),
            new Vector2(1080f, 2400f),
            new Vector2(720f, 1560f)
        };

        [Test]
        public void DropZones_DoNotOverlapAbilityBarOnPortraitProfiles()
        {
            for (int i = 0; i < TargetProfiles.Length; i++)
            {
                Vector2 screenSize = TargetProfiles[i];
                Rect ability = AndroidHudLayoutMetrics.BottomBarRect(
                    screenSize,
                    AndroidHudLayoutMetrics.AbilityBottomOffset,
                    AndroidHudLayoutMetrics.AbilityHeight);
                Rect delivery = AndroidHudLayoutMetrics.ToPixelRect(
                    AndroidHudLayoutMetrics.DeliveryAnchorMin,
                    AndroidHudLayoutMetrics.DeliveryAnchorMax,
                    screenSize);
                Rect sell = AndroidHudLayoutMetrics.ToPixelRect(
                    AndroidHudLayoutMetrics.SellAnchorMin,
                    AndroidHudLayoutMetrics.SellAnchorMax,
                    screenSize);

                Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(ability, delivery, 12f), "Ability overlaps Delivery on " + screenSize);
                Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(ability, sell, 12f), "Ability overlaps Sell on " + screenSize);
            }
        }

        [Test]
        public void TopBar_KeepsStatsAndActionsReadableOnSmallPortraitProfile()
        {
            Vector2 screenSize = new Vector2(720f, 1280f);

            Rect timer = AndroidHudLayoutMetrics.ToPixelRect(AndroidHudLayoutMetrics.TimerAnchorMin, AndroidHudLayoutMetrics.TimerAnchorMax, screenSize);
            Rect gold = AndroidHudLayoutMetrics.ToPixelRect(AndroidHudLayoutMetrics.GoldAnchorMin, AndroidHudLayoutMetrics.GoldAnchorMax, screenSize);
            Rect gem = AndroidHudLayoutMetrics.ToPixelRect(AndroidHudLayoutMetrics.GemAnchorMin, AndroidHudLayoutMetrics.GemAnchorMax, screenSize);
            Rect level = AndroidHudLayoutMetrics.ToPixelRect(AndroidHudLayoutMetrics.LevelButtonAnchorMin, AndroidHudLayoutMetrics.LevelButtonAnchorMax, screenSize);
            Rect mission = AndroidHudLayoutMetrics.ToPixelRect(AndroidHudLayoutMetrics.MissionButtonAnchorMin, AndroidHudLayoutMetrics.MissionButtonAnchorMax, screenSize);
            Rect shop = AndroidHudLayoutMetrics.ToPixelRect(AndroidHudLayoutMetrics.ShopButtonAnchorMin, AndroidHudLayoutMetrics.ShopButtonAnchorMax, screenSize);
            Rect pause = AndroidHudLayoutMetrics.ToPixelRect(AndroidHudLayoutMetrics.PauseButtonAnchorMin, AndroidHudLayoutMetrics.PauseButtonAnchorMax, screenSize);

            Assert.GreaterOrEqual(timer.width, AndroidHudLayoutMetrics.MinimumTopBarStatWidth);
            Assert.GreaterOrEqual(gold.width, AndroidHudLayoutMetrics.MinimumTopBarStatWidth);
            Assert.GreaterOrEqual(gem.width, AndroidHudLayoutMetrics.MinimumTopBarStatWidth);
            Assert.GreaterOrEqual(level.width, AndroidHudLayoutMetrics.MinimumTopBarActionWidth);
            Assert.GreaterOrEqual(mission.width, AndroidHudLayoutMetrics.MinimumTopBarActionWidth);
            Assert.GreaterOrEqual(shop.width, AndroidHudLayoutMetrics.MinimumTopBarActionWidth);
            Assert.GreaterOrEqual(pause.width, AndroidHudLayoutMetrics.MinimumTopBarActionWidth);
        }

        [Test]
        public void AbilityButtons_KeepTouchSizeOnSmallPortraitProfile()
        {
            float sidePadding = Mathf.Max(32f, 1080f * 0.035f);
            Vector2 abilityBarSize = new Vector2(720f - 80f - sidePadding * 2f, AndroidHudLayoutMetrics.AbilityHeight);
            Rect shovel = AndroidHudLayoutMetrics.ToPixelRect(AndroidHudLayoutMetrics.ShovelButtonAnchorMin, AndroidHudLayoutMetrics.ShovelButtonAnchorMax, abilityBarSize);
            Rect wand = AndroidHudLayoutMetrics.ToPixelRect(AndroidHudLayoutMetrics.MagicWandButtonAnchorMin, AndroidHudLayoutMetrics.MagicWandButtonAnchorMax, abilityBarSize);
            Rect magnet = AndroidHudLayoutMetrics.ToPixelRect(AndroidHudLayoutMetrics.SortingMagnetButtonAnchorMin, AndroidHudLayoutMetrics.SortingMagnetButtonAnchorMax, abilityBarSize);

            Assert.GreaterOrEqual(shovel.width, AndroidHudLayoutMetrics.MinimumAbilityButtonWidth);
            Assert.GreaterOrEqual(wand.width, AndroidHudLayoutMetrics.MinimumAbilityButtonWidth);
            Assert.GreaterOrEqual(magnet.width, AndroidHudLayoutMetrics.MinimumAbilityButtonWidth);
            Assert.GreaterOrEqual(shovel.height, AndroidHudLayoutMetrics.MinimumAbilityButtonHeight);
            Assert.GreaterOrEqual(wand.height, AndroidHudLayoutMetrics.MinimumAbilityButtonHeight);
            Assert.GreaterOrEqual(magnet.height, AndroidHudLayoutMetrics.MinimumAbilityButtonHeight);
        }

        [Test]
        public void DropZones_AreLargeAndSeparatedEnoughForTouchOnPortraitProfiles()
        {
            for (int i = 0; i < TargetProfiles.Length; i++)
            {
                Vector2 screenSize = TargetProfiles[i];
                Rect delivery = AndroidHudLayoutMetrics.ToPixelRect(
                    AndroidHudLayoutMetrics.DeliveryAnchorMin,
                    AndroidHudLayoutMetrics.DeliveryAnchorMax,
                    screenSize);
                Rect sell = AndroidHudLayoutMetrics.ToPixelRect(
                    AndroidHudLayoutMetrics.SellAnchorMin,
                    AndroidHudLayoutMetrics.SellAnchorMax,
                    screenSize);
                float gap = sell.xMin - delivery.xMax;

                Assert.GreaterOrEqual(delivery.width, AndroidHudLayoutMetrics.MinimumExternalDropZoneWidth, "Delivery width on " + screenSize);
                Assert.GreaterOrEqual(delivery.height, AndroidHudLayoutMetrics.MinimumExternalDropZoneHeight, "Delivery height on " + screenSize);
                Assert.GreaterOrEqual(sell.width, AndroidHudLayoutMetrics.MinimumExternalDropZoneWidth, "Sell width on " + screenSize);
                Assert.GreaterOrEqual(sell.height, AndroidHudLayoutMetrics.MinimumExternalDropZoneHeight, "Sell height on " + screenSize);
                Assert.GreaterOrEqual(gap, AndroidHudLayoutMetrics.MinimumExternalDropZoneGap, "Drop-zone gap on " + screenSize);
            }
        }

        [Test]
        public void MissionTracker_DoesNotOverlapDropZonesOnPortraitProfiles()
        {
            for (int i = 0; i < TargetProfiles.Length; i++)
            {
                Vector2 screenSize = TargetProfiles[i];
                Rect tracker = AndroidHudLayoutMetrics.ToPixelRect(
                    AndroidHudLayoutMetrics.MissionTrackerAnchorMin,
                    AndroidHudLayoutMetrics.MissionTrackerAnchorMax,
                    screenSize);
                Rect delivery = AndroidHudLayoutMetrics.ToPixelRect(
                    AndroidHudLayoutMetrics.DeliveryAnchorMin,
                    AndroidHudLayoutMetrics.DeliveryAnchorMax,
                    screenSize);
                Rect sell = AndroidHudLayoutMetrics.ToPixelRect(
                    AndroidHudLayoutMetrics.SellAnchorMin,
                    AndroidHudLayoutMetrics.SellAnchorMax,
                    screenSize);

                Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(tracker, delivery, 8f), "Tracker overlaps Delivery on " + screenSize);
                Assert.IsFalse(AndroidHudLayoutMetrics.Overlaps(tracker, sell, 8f), "Tracker overlaps Sell on " + screenSize);
            }
        }

        [Test]
        public void MissionTracker_KeepsReadableWidthOnSmallestPortraitProfile()
        {
            Vector2 screenSize = new Vector2(720f, 1280f);
            Rect tracker = AndroidHudLayoutMetrics.ToPixelRect(
                AndroidHudLayoutMetrics.MissionTrackerAnchorMin,
                AndroidHudLayoutMetrics.MissionTrackerAnchorMax,
                screenSize);

            float actionWidth = tracker.width * 0.27f;

            Assert.GreaterOrEqual(tracker.width, AndroidHudLayoutMetrics.MinimumCompactMissionTrackerWidth);
            Assert.GreaterOrEqual(actionWidth, AndroidHudLayoutMetrics.MinimumCompactMissionActionWidth);
            Assert.AreEqual(2, AndroidHudLayoutMetrics.MaxCompactMissionRows);
        }
    }
}
