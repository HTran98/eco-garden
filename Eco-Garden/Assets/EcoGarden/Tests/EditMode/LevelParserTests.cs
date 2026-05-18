using EcoGarden.Board;
using EcoGarden.Level;
using NUnit.Framework;

namespace EcoGarden.Tests
{
    public sealed class LevelParserTests
    {
        [Test]
        public void ParseLevel15_BuildsEightByEightBoard()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            Assert.AreEqual(8, board.Width);
            Assert.AreEqual(8, board.Height);
            Assert.AreEqual(CellKind.Locked, board.GetCell(new GridPosition(0, 7)).Kind);
            Assert.AreEqual(CellKind.Producer, board.GetCell(new GridPosition(0, 4)).Kind);
            Assert.AreEqual(CellKind.Empty, board.GetCell(new GridPosition(7, 4)).Kind);
        }

        [Test]
        public void ParseLevel15_MapsTopRowToYSeven()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            Assert.AreEqual(CellKind.Locked, board.GetCell(new GridPosition(0, 7)).Kind);
            Assert.AreEqual(CellKind.Locked, board.GetCell(new GridPosition(1, 7)).Kind);
            Assert.AreEqual(CellKind.Empty, board.GetCell(new GridPosition(2, 7)).Kind);
            Assert.AreEqual(CellKind.Locked, board.GetCell(new GridPosition(0, 0)).Kind);
        }

        [Test]
        public void ParseLevel15_CreatesPrePlacedItems()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            Assert.AreEqual(2, board.GetCell(new GridPosition(3, 6)).Item.Level);
            Assert.AreEqual(1, board.GetCell(new GridPosition(4, 6)).Item.Level);
            Assert.AreEqual(1, board.GetCell(new GridPosition(3, 1)).Item.Level);
            Assert.AreEqual(1, board.GetCell(new GridPosition(4, 1)).Item.Level);
        }

        [Test]
        public void ParseLevel15_CreatesObstacles()
        {
            BoardState board = LevelParser.Parse(TestLevelFactory.CreateLevel15());

            Assert.AreEqual(ObstacleKind.Weed, board.GetCell(new GridPosition(2, 5)).ObstacleKind);
            Assert.AreEqual(ObstacleKind.Pebble, board.GetCell(new GridPosition(3, 4)).ObstacleKind);
        }

        [Test]
        public void Parse_RejectsInvalidRowWidth()
        {
            Assert.Throws<LevelParseException>(() =>
            {
                LevelParser.Parse(TestLevelFactory.CreateLevelWithRows(new[]
                {
                    "LL----LL",
                    "L--21--L",
                    "--W--W--",
                    "S-PPPP-N",
                    "--PPPP--",
                    "--W--W--",
                    "L--11--L",
                    "LL---"
                }));
            });
        }

        [Test]
        public void Parse_RejectsInvalidToken()
        {
            Assert.Throws<LevelParseException>(() =>
            {
                LevelParser.Parse(TestLevelFactory.CreateLevelWithRows(new[]
                {
                    "LL----LL",
                    "L--21--L",
                    "--W--W--",
                    "S-PPPP-N",
                    "--PPPP--",
                    "--W--W--",
                    "L--11--L",
                    "LL---XLL"
                }));
            });
        }
    }
}
