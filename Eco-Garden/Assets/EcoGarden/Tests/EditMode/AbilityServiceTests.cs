using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Level;
using NUnit.Framework;

namespace EcoGarden.Tests
{
    public sealed class AbilityServiceTests
    {
        [Test]
        public void Shovel_ConsumesOnlyOnSuccess()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());
            AbilityInventory inventory = new AbilityInventory();
            inventory.SetCount(AbilityKind.Shovel, 1);
            AbilityService service = new AbilityService(board, inventory);

            Assert.IsFalse(service.TryUseShovel(new GridPosition(1, 1)));
            Assert.AreEqual(1, inventory.GetCount(AbilityKind.Shovel));

            Assert.IsTrue(service.TryUseShovel(new GridPosition(0, 2)));
            Assert.AreEqual(0, inventory.GetCount(AbilityKind.Shovel));
        }

        [Test]
        public void MagicWand_ConsumesOnlyOnSuccess()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());
            AbilityInventory inventory = new AbilityInventory();
            inventory.SetCount(AbilityKind.MagicWand, 1);
            AbilityService service = new AbilityService(board, inventory);

            Assert.IsFalse(service.TryUseMagicWand(new GridPosition(1, 1)));
            Assert.AreEqual(1, inventory.GetCount(AbilityKind.MagicWand));

            Assert.IsTrue(service.TryUseMagicWand(new GridPosition(1, 3)));
            Assert.AreEqual(0, inventory.GetCount(AbilityKind.MagicWand));
            Assert.AreEqual(3, board.GetCell(new GridPosition(1, 3)).Item.Level);
        }

        [Test]
        public void SortingMagnet_MovesMatchingPairAndConsumes()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());
            AbilityInventory inventory = new AbilityInventory();
            inventory.SetCount(AbilityKind.SortingMagnet, 1);
            AbilityService service = new AbilityService(board, inventory);

            bool used = service.TryUseSortingMagnet("lotus", out GridPosition movedFrom, out GridPosition movedTo);

            Assert.IsTrue(used);
            Assert.AreEqual(0, inventory.GetCount(AbilityKind.SortingMagnet));
            Assert.IsNull(board.GetCell(movedFrom).Item);
            Assert.IsNotNull(board.GetCell(movedTo).Item);
        }
    }
}
