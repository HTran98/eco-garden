using EcoGarden.Board;
using EcoGarden.Level;
using NUnit.Framework;

namespace EcoGarden.Tests
{
    public sealed class BoardRuleTests
    {
        [Test]
        public void TryMoveItem_MovesItemToEmptyCell()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool moved = board.TryMoveItem(new GridPosition(4, 1), new GridPosition(4, 2));

            Assert.IsTrue(moved);
            Assert.IsNull(board.GetCell(new GridPosition(4, 1)).Item);
            Assert.AreEqual(1, board.GetCell(new GridPosition(4, 2)).Item.Level);
        }

        [Test]
        public void TryMoveItem_RejectsLockedCell()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool moved = board.TryMoveItem(new GridPosition(4, 1), new GridPosition(0, 0));

            Assert.IsFalse(moved);
            Assert.IsNotNull(board.GetCell(new GridPosition(4, 1)).Item);
        }

        [Test]
        public void TryMergeItem_MergesIdenticalLevels()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool merged = board.TryMergeItem(new GridPosition(4, 1), new GridPosition(3, 1));

            Assert.IsTrue(merged);
            Assert.IsNull(board.GetCell(new GridPosition(4, 1)).Item);
            Assert.AreEqual(2, board.GetCell(new GridPosition(3, 1)).Item.Level);
        }

        [Test]
        public void TryMergeItem_RejectsDifferentLevels()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool merged = board.TryMergeItem(new GridPosition(4, 6), new GridPosition(3, 6));

            Assert.IsFalse(merged);
            Assert.AreEqual(1, board.GetCell(new GridPosition(4, 6)).Item.Level);
            Assert.AreEqual(2, board.GetCell(new GridPosition(3, 6)).Item.Level);
        }

        [Test]
        public void TryRemoveObstacle_ClearsObstacleCell()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool removed = board.TryRemoveObstacle(new GridPosition(2, 5));

            Assert.IsTrue(removed);
            Assert.AreEqual(CellKind.Empty, board.GetCell(new GridPosition(2, 5)).Kind);
            Assert.AreEqual(ObstacleKind.None, board.GetCell(new GridPosition(2, 5)).ObstacleKind);
        }

        [Test]
        public void TryUpgradeItem_UpgradesNonMaxItem()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool upgraded = board.TryUpgradeItem(new GridPosition(3, 6));

            Assert.IsTrue(upgraded);
            Assert.AreEqual(3, board.GetCell(new GridPosition(3, 6)).Item.Level);
        }

        [Test]
        public void TrySpawnFromProducer_SpawnsNearProducer()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool spawned = board.TrySpawnFromProducer(new GridPosition(0, 4), 0f, out GridPosition spawnPosition);

            Assert.IsTrue(spawned);
            Assert.IsNotNull(board.GetCell(spawnPosition).Item);
            Assert.AreEqual(1, board.GetCell(spawnPosition).Item.Level);
        }
    }
}
