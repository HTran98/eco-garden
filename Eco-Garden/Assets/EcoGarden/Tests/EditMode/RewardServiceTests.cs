using EcoGarden.Abilities;
using EcoGarden.Economy;
using EcoGarden.Progression;
using EcoGarden.Rewards;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class RewardServiceTests
    {
        private GameObject gameObject;
        private EconomyController economyController;
        private AbilityInventory abilityInventory;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject("RewardServiceTests");
            economyController = gameObject.AddComponent<EconomyController>();
            abilityInventory = new AbilityInventory();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void Grant_AppliesCurrencyAndAbilityRewards()
        {
            abilityInventory.SetCount(AbilityKind.Shovel, 1);
            RewardDefinition reward = new RewardDefinition(
                new[]
                {
                    new CurrencyReward(CurrencyKind.Gold, 25),
                    new CurrencyReward(CurrencyKind.Gem, 2)
                },
                new[]
                {
                    new AbilityReward(AbilityKind.Shovel, 3),
                    new AbilityReward(AbilityKind.MagicWand, 1)
                });

            RewardGrantResult result = RewardService.Grant(reward, economyController, abilityInventory);

            Assert.IsTrue(result.HasAnyGrant);
            Assert.AreEqual(2, result.CurrencyGrantCount);
            Assert.AreEqual(2, result.AbilityGrantCount);
            Assert.AreEqual(25, economyController.Gold);
            Assert.AreEqual(2, economyController.Gem);
            Assert.AreEqual(4, abilityInventory.GetCount(AbilityKind.Shovel));
            Assert.AreEqual(1, abilityInventory.GetCount(AbilityKind.MagicWand));
        }

        [Test]
        public void Grant_IgnoresInvalidAmounts()
        {
            RewardDefinition reward = new RewardDefinition(
                new[]
                {
                    new CurrencyReward(CurrencyKind.Gold, 0),
                    new CurrencyReward(CurrencyKind.Gem, -1)
                },
                new[]
                {
                    new AbilityReward(AbilityKind.SortingMagnet, 0)
                });

            RewardGrantResult result = RewardService.Grant(reward, economyController, abilityInventory);

            Assert.IsFalse(result.HasAnyGrant);
            Assert.AreEqual(0, economyController.Gold);
            Assert.AreEqual(0, economyController.Gem);
            Assert.AreEqual(0, abilityInventory.GetCount(AbilityKind.SortingMagnet));
        }

        [Test]
        public void Grant_CountsFutureDecorationAndUnlockRewards()
        {
            RewardDefinition reward = new RewardDefinition(
                null,
                null,
                new[] { "butterfly_blue", string.Empty, "npc_traveler" },
                new[]
                {
                    new PlantTierUnlockReward("lotus", 4),
                    new PlantTierUnlockReward("", 5),
                    new PlantTierUnlockReward("lotus", 0)
                });

            RewardGrantResult result = RewardService.Grant(reward, economyController, abilityInventory);

            Assert.IsTrue(result.HasAnyGrant);
            Assert.AreEqual(2, result.DecorationGrantCount);
            Assert.AreEqual(1, result.PlantTierUnlockGrantCount);
        }

        [Test]
        public void Grant_AppliesPlantTierUnlockRewardsWhenServiceIsProvided()
        {
            PlantUnlockService unlockService = new PlantUnlockService();
            RewardDefinition reward = new RewardDefinition(
                null,
                null,
                null,
                new[] { new PlantTierUnlockReward("lotus", 4) });

            RewardGrantResult result = RewardService.Grant(
                reward,
                economyController,
                abilityInventory,
                unlockService);

            Assert.AreEqual(1, result.PlantTierUnlockGrantCount);
            Assert.IsTrue(unlockService.IsTierUnlocked("lotus", 4));
        }
    }
}
