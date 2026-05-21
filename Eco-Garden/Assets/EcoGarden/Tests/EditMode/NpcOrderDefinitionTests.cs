using EcoGarden.Config;
using EcoGarden.Items;
using NUnit.Framework;

namespace EcoGarden.Tests.EditMode
{
    public sealed class NpcOrderDefinitionTests
    {
        [Test]
        public void LegacyOrder_ExposesSingleRequirementAndMatchesItem()
        {
            NpcOrderDefinition order = new NpcOrderDefinition("lotus", 5, 1);
            BoardItem matchingItem = new BoardItem("lotus", 5, "item_lotus_lv05");
            BoardItem wrongLevelItem = new BoardItem("lotus", 4, "item_lotus_lv04");

            Assert.AreEqual(1, order.Requirements.Count);
            Assert.AreEqual("lotus", order.Requirements[0].FamilyId);
            Assert.AreEqual(5, order.Requirements[0].Level);
            Assert.AreEqual(1, order.Requirements[0].Quantity);
            Assert.IsTrue(order.Matches(matchingItem));
            Assert.IsFalse(order.Matches(wrongLevelItem));
        }

        [Test]
        public void MultiRequirementOrder_MatchesAnyRequirement()
        {
            NpcOrderDefinition order = new NpcOrderDefinition(
                "mixed_lotus_order",
                "Mixed Lotus Order",
                new[]
                {
                    new OrderRequirementDefinition("lotus", 2, 2),
                    new OrderRequirementDefinition("lotus", 4, 1)
                });

            Assert.AreEqual(2, order.Requirements.Count);
            Assert.IsTrue(order.Matches(new BoardItem("lotus", 2, "item_lotus_lv02")));
            Assert.IsTrue(order.Matches(new BoardItem("lotus", 4, "item_lotus_lv04")));
            Assert.IsFalse(order.Matches(new BoardItem("lotus", 5, "item_lotus_lv05")));
        }

        [Test]
        public void RequirementRuntimeState_TracksSubmittedProgress()
        {
            OrderRequirementRuntimeState state = new OrderRequirementRuntimeState(
                new OrderRequirementDefinition("lotus", 2, 2));

            Assert.IsFalse(state.IsComplete);
            Assert.IsTrue(state.TrySubmit(new BoardItem("lotus", 2, "item_lotus_lv02_a")));
            Assert.AreEqual(1, state.SubmittedCount);
            Assert.IsFalse(state.IsComplete);
            Assert.IsFalse(state.TrySubmit(new BoardItem("lotus", 3, "item_lotus_lv03")));
            Assert.IsTrue(state.TrySubmit(new BoardItem("lotus", 2, "item_lotus_lv02_b")));
            Assert.IsTrue(state.IsComplete);
            Assert.IsFalse(state.TrySubmit(new BoardItem("lotus", 2, "item_lotus_lv02_c")));
            Assert.AreEqual(2, state.SubmittedCount);
        }
    }
}
