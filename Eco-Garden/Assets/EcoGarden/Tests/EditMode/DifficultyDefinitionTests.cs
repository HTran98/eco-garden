using EcoGarden.Config;
using EcoGarden.Tests;
using NUnit.Framework;

namespace EcoGarden.Tests.EditMode
{
    public sealed class DifficultyDefinitionTests
    {
        [Test]
        public void LevelDefinition_StoresDifficultyAndPressureData()
        {
            LevelDefinition level = TestLevelFactory.CreateLevel15();

            Assert.AreEqual(DifficultyKind.Hard, level.Difficulty.DifficultyKind);
            Assert.AreEqual(4, level.Difficulty.LockedCellCount);
            Assert.AreEqual(2, level.Difficulty.ObstacleCount);
            Assert.AreEqual(1, level.Difficulty.TemporaryLockCount);
            Assert.AreEqual(2f, level.Difficulty.RewardMultiplier);
            Assert.AreEqual(0.75f, level.Difficulty.TimerPressureMultiplier);
        }

        [Test]
        public void NpcOrderDefinition_ComputesOrderComplexity()
        {
            NpcOrderDefinition order = new NpcOrderDefinition(
                "mixed_order",
                "Mixed Order",
                new[]
                {
                    new OrderRequirementDefinition("lotus", 2, 2),
                    new OrderRequirementDefinition("lotus", 5, 1)
                });

            Assert.AreEqual(2, order.RequirementCount);
            Assert.AreEqual(3, order.TotalRequiredItems);
            Assert.AreEqual(5, order.HighestRequiredLevel);
            Assert.AreEqual(9, order.ComplexityScore);
        }
    }
}
