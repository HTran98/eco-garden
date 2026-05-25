using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Config;
using EcoGarden.Economy;
using EcoGarden.Items;
using EcoGarden.Missions;
using EcoGarden.Rewards;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class MissionDefinitionTests
    {
        [Test]
        public void RuntimeState_ClampsProgressAndBlocksClaimAfterRewardClaimed()
        {
            MissionDefinition mission = CreateMission("mission_a", 5, 10);
            MissionRuntimeState state = new MissionRuntimeState(mission);

            state.AddProgress(10);

            Assert.AreEqual(5, state.Progress);
            Assert.IsTrue(state.IsComplete);
            Assert.IsTrue(state.CanClaim);

            state.SetRewardClaimed(true);

            Assert.IsFalse(state.CanClaim);
            Object.DestroyImmediate(mission);
        }

        [Test]
        public void Controller_LoadsValidUniqueMissionsSortedBySortOrder()
        {
            GameObject gameObject = new GameObject("MissionControllerTests");
            MissionController controller = gameObject.AddComponent<MissionController>();
            MissionDefinition late = CreateMission("mission_late", 2, 20);
            MissionDefinition early = CreateMission("mission_early", 2, 5);
            MissionDefinition duplicate = CreateMission("mission_early", 2, 1);
            MissionDefinition invalid = CreateInvalidMission();

            controller.SetMissionDefinitions(new[] { late, early, duplicate, invalid });

            Assert.AreEqual(2, controller.Missions.Count);
            Assert.AreEqual("mission_early", controller.Missions[0].MissionId);
            Assert.AreEqual("mission_late", controller.Missions[1].MissionId);

            Object.DestroyImmediate(gameObject);
            Object.DestroyImmediate(late);
            Object.DestroyImmediate(early);
            Object.DestroyImmediate(duplicate);
            Object.DestroyImmediate(invalid);
        }

        [Test]
        public void Controller_SkipsDailyMissionsByDefault()
        {
            GameObject gameObject = new GameObject("MissionControllerTests");
            MissionController controller = gameObject.AddComponent<MissionController>();
            MissionDefinition staticMission = CreateMission("mission_static", 2, 10);
            MissionDefinition dailyMission = CreateMission("mission_daily", 2, 5, true);

            controller.SetMissionDefinitions(new[] { dailyMission, staticMission });

            Assert.AreEqual(1, controller.Missions.Count);
            Assert.AreEqual("mission_static", controller.Missions[0].MissionId);
            Assert.IsFalse(controller.TryGetMission("mission_daily", out _));

            Object.DestroyImmediate(gameObject);
            Object.DestroyImmediate(staticMission);
            Object.DestroyImmediate(dailyMission);
        }

        [Test]
        public void Controller_RestoresAndCapturesMissionState()
        {
            GameObject gameObject = new GameObject("MissionControllerTests");
            MissionController controller = gameObject.AddComponent<MissionController>();
            MissionDefinition mission = CreateMission("mission_restore", 4, 1);
            controller.SetMissionDefinitions(new[] { mission });

            controller.RestoreMissionStates(new[]
            {
                new MissionSaveState("mission_restore", 3, true)
            });
            MissionSaveState[] captured = controller.CaptureMissionStates();

            Assert.AreEqual(1, captured.Length);
            Assert.AreEqual("mission_restore", captured[0].MissionId);
            Assert.AreEqual(3, captured[0].Progress);
            Assert.IsTrue(captured[0].RewardClaimed);

            Object.DestroyImmediate(gameObject);
            Object.DestroyImmediate(mission);
        }

        [Test]
        public void Controller_TracksProgressFromBoardGameplayEvents()
        {
            GameObject boardObject = new GameObject("BoardControllerMissionTests");
            boardObject.AddComponent<BoardView>();
            BoardController boardController = boardObject.AddComponent<BoardController>();
            boardController.SetLevelDefinition(TestLevelFactory.CreateLevel15());
            boardController.LoadLevel();

            GameObject missionObject = new GameObject("MissionControllerTests");
            MissionController missionController = missionObject.AddComponent<MissionController>();
            MissionDefinition sellMission = CreateMission(
                "mission_sell",
                MissionType.Sell,
                string.Empty,
                0,
                AbilityKind.Shovel,
                1,
                1);
            missionController.SetMissionDefinitions(new[] { sellMission });

            Assert.IsTrue(boardController.TrySellItem(new GridPosition(3, 1), out _, false));

            Assert.IsTrue(missionController.TryGetMission("mission_sell", out MissionRuntimeState state));
            Assert.AreEqual(1, state.Progress);
            Assert.IsTrue(state.IsComplete);

            Object.DestroyImmediate(boardObject);
            Object.DestroyImmediate(missionObject);
            Object.DestroyImmediate(sellMission);
        }

        [Test]
        public void Controller_TracksAbilityUseProgress()
        {
            MissionDefinition abilityMission = CreateMission(
                "mission_shovel",
                MissionType.UseAbility,
                string.Empty,
                0,
                AbilityKind.Shovel,
                2,
                1);
            GameObject gameObject = new GameObject("MissionControllerTests");
            MissionController controller = gameObject.AddComponent<MissionController>();
            controller.SetMissionDefinitions(new[] { abilityMission });

            controller.RecordAbilityUse(AbilityKind.Shovel);
            controller.RecordAbilityUse(AbilityKind.MagicWand);
            controller.RecordAbilityUse(AbilityKind.Shovel);

            Assert.IsTrue(controller.TryGetMission("mission_shovel", out MissionRuntimeState state));
            Assert.AreEqual(2, state.Progress);
            Assert.IsTrue(state.IsComplete);

            Object.DestroyImmediate(gameObject);
            Object.DestroyImmediate(abilityMission);
        }

        [Test]
        public void TryClaimMissionReward_GrantsRewardOnceAndMarksClaimed()
        {
            GameObject gameObject = new GameObject("MissionControllerTests");
            EconomyController economyController = gameObject.AddComponent<EconomyController>();
            MissionController controller = gameObject.AddComponent<MissionController>();
            MissionDefinition mission = CreateMission("mission_claim", 2, 1);
            controller.SetMissionDefinitions(new[] { mission });
            controller.SetMissionProgress("mission_claim", 2);

            MissionClaimResult firstClaim = controller.TryClaimMissionReward("mission_claim");
            MissionClaimResult secondClaim = controller.TryClaimMissionReward("mission_claim");

            Assert.IsTrue(firstClaim.Succeeded);
            Assert.AreEqual(MissionClaimStatus.Claimed, firstClaim.Status);
            Assert.AreEqual(10, economyController.Gold);
            Assert.AreEqual(MissionClaimStatus.AlreadyClaimed, secondClaim.Status);
            Assert.AreEqual(10, economyController.Gold);
            Assert.IsTrue(controller.TryGetMission("mission_claim", out MissionRuntimeState state));
            Assert.IsTrue(state.RewardClaimed);

            Object.DestroyImmediate(gameObject);
            Object.DestroyImmediate(mission);
        }

        [Test]
        public void TryClaimMissionReward_RejectsIncompleteMission()
        {
            GameObject gameObject = new GameObject("MissionControllerTests");
            gameObject.AddComponent<EconomyController>();
            MissionController controller = gameObject.AddComponent<MissionController>();
            MissionDefinition mission = CreateMission("mission_incomplete", 2, 1);
            controller.SetMissionDefinitions(new[] { mission });
            controller.SetMissionProgress("mission_incomplete", 1);

            MissionClaimResult result = controller.TryClaimMissionReward("mission_incomplete");

            Assert.AreEqual(MissionClaimStatus.NotComplete, result.Status);
            Assert.IsFalse(controller.Missions[0].RewardClaimed);

            Object.DestroyImmediate(gameObject);
            Object.DestroyImmediate(mission);
        }

        private static MissionDefinition CreateMission(string missionId, int requiredCount, int sortOrder)
        {
            return CreateMission(missionId, requiredCount, sortOrder, false);
        }

        private static MissionDefinition CreateMission(string missionId, int requiredCount, int sortOrder, bool isDaily)
        {
            return CreateMission(
                missionId,
                MissionType.Merge,
                "lotus",
                1,
                AbilityKind.Shovel,
                requiredCount,
                sortOrder,
                isDaily);
        }

        private static MissionDefinition CreateMission(
            string missionId,
            MissionType missionType,
            string targetFamilyId,
            int targetItemLevel,
            AbilityKind targetAbility,
            int requiredCount,
            int sortOrder,
            bool isDaily = false)
        {
            MissionDefinition mission = ScriptableObject.CreateInstance<MissionDefinition>();
            mission.EditorSetValues(
                missionId,
                missionId,
                string.Empty,
                missionType,
                DifficultyKind.Easy,
                targetFamilyId,
                targetItemLevel,
                targetAbility,
                requiredCount,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gold, 10) }, null),
                isDaily,
                sortOrder);
            return mission;
        }

        private static MissionDefinition CreateInvalidMission()
        {
            MissionDefinition mission = ScriptableObject.CreateInstance<MissionDefinition>();
            mission.EditorSetValues(
                string.Empty,
                string.Empty,
                string.Empty,
                MissionType.Merge,
                DifficultyKind.Easy,
                "lotus",
                1,
                AbilityKind.Shovel,
                1,
                null,
                false,
                0);
            return mission;
        }
    }
}
