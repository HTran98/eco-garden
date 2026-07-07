using EcoGarden.Board;
using EcoGarden.Level;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests
{
    public sealed class BoardRuleTests
    {
        [Test]
        public void TryMoveItem_MovesItemToEmptyCell()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool moved = board.TryMoveItem(new GridPosition(3, 1), new GridPosition(4, 2));

            Assert.IsTrue(moved);
            Assert.IsNull(board.GetCell(new GridPosition(3, 1)).Item);
            Assert.AreEqual(1, board.GetCell(new GridPosition(4, 2)).Item.Level);
        }

        [Test]
        public void TryMoveItem_RejectsLockedCell()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool moved = board.TryMoveItem(new GridPosition(3, 1), new GridPosition(0, 0));

            Assert.IsFalse(moved);
            Assert.IsNotNull(board.GetCell(new GridPosition(3, 1)).Item);
        }

        [Test]
        public void TryMergeItem_MergesIdenticalLevels()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool merged = board.TryMergeItem(new GridPosition(3, 1), new GridPosition(2, 1));

            Assert.IsTrue(merged);
            Assert.IsNull(board.GetCell(new GridPosition(3, 1)).Item);
            Assert.AreEqual(2, board.GetCell(new GridPosition(2, 1)).Item.Level);
        }

        [Test]
        public void TryMergeItem_RejectsDifferentLevels()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool merged = board.TryMergeItem(new GridPosition(3, 3), new GridPosition(1, 3));

            Assert.IsFalse(merged);
            Assert.AreEqual(1, board.GetCell(new GridPosition(3, 3)).Item.Level);
            Assert.AreEqual(2, board.GetCell(new GridPosition(1, 3)).Item.Level);
        }

        [Test]
        public void TryRemoveObstacle_ClearsObstacleCell()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool removed = board.TryRemoveObstacle(new GridPosition(0, 2));

            Assert.IsTrue(removed);
            Assert.AreEqual(CellKind.Empty, board.GetCell(new GridPosition(0, 2)).Kind);
            Assert.AreEqual(ObstacleKind.None, board.GetCell(new GridPosition(0, 2)).ObstacleKind);
        }

        [Test]
        public void TryUpgradeItem_UpgradesNonMaxItem()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool upgraded = board.TryUpgradeItem(new GridPosition(1, 3));

            Assert.IsTrue(upgraded);
            Assert.AreEqual(3, board.GetCell(new GridPosition(1, 3)).Item.Level);
        }

        [Test]
        public void TrySpawnFromProducer_SpawnsNearProducer()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            bool spawned = board.TrySpawnFromProducer(new GridPosition(2, 2), 0f, out GridPosition spawnPosition);

            Assert.IsTrue(spawned);
            Assert.IsNotNull(board.GetCell(spawnPosition).Item);
            Assert.AreEqual(1, board.GetCell(spawnPosition).Item.Level);
        }

        [Test]
        public void BoardView_SquareLayoutKeepsGridCenteredInsidePlatform()
        {
            GameObject viewObject = new GameObject("BoardView");
            try
            {
                BoardView boardView = viewObject.AddComponent<BoardView>();
                BoardState board = new BoardState(3, 3, null);
                for (int y = 0; y < board.Height; y++)
                {
                    for (int x = 0; x < board.Width; x++)
                    {
                        board.SetCell(new BoardCell(new GridPosition(x, y), CellKind.Empty));
                    }
                }

                boardView.Render(board);

                Vector3 center = boardView.GridToWorld(board, new GridPosition(1, 1));
                Vector3 right = boardView.GridToWorld(board, new GridPosition(2, 1));
                Vector3 top = boardView.GridToWorld(board, new GridPosition(1, 2));

                Assert.AreEqual(boardView.GetBoardWorldWidth(board), boardView.GetBoardWorldHeight(board), 0.001f);
                Assert.Greater(right.x, center.x);
                Assert.AreEqual(center.y, right.y, 0.001f);
                Assert.AreEqual(center.x, top.x, 0.001f);
                Assert.Greater(top.y, center.y);
                Assert.IsTrue(boardView.TryWorldToGrid(board, center, out GridPosition mapped));
                Assert.AreEqual(new GridPosition(1, 1), mapped);
            }
            finally
            {
                Object.DestroyImmediate(viewObject);
            }
        }
    }
}
