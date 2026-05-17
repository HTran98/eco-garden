using System.Collections.Generic;
using EcoGarden.Board;
using EcoGarden.Config;
using EcoGarden.Items;

namespace EcoGarden.Level
{
    public static class LevelParser
    {
        public static BoardState Parse(LevelDefinition levelDefinition)
        {
            if (levelDefinition == null)
            {
                throw new LevelParseException("Level definition is null.");
            }

            IReadOnlyList<string> rows = levelDefinition.RowsTopToBottom;
            if (rows == null || rows.Count != levelDefinition.Height)
            {
                throw new LevelParseException("Level row count does not match board height.");
            }

            Dictionary<int, ItemDefinition> itemsByLevel = new Dictionary<int, ItemDefinition>();
            for (int i = 0; i < levelDefinition.ItemDefinitions.Count; i++)
            {
                ItemDefinition item = levelDefinition.ItemDefinitions[i];
                if (item != null)
                {
                    itemsByLevel[item.Level] = item;
                }
            }

            BoardState board = new BoardState(levelDefinition.Width, levelDefinition.Height, itemsByLevel);

            for (int row = 0; row < rows.Count; row++)
            {
                string sourceRow = rows[row];
                if (sourceRow == null || sourceRow.Length != levelDefinition.Width)
                {
                    throw new LevelParseException("Level row width is invalid at top-to-bottom row " + row + ".");
                }

                int y = levelDefinition.Height - 1 - row;
                for (int x = 0; x < levelDefinition.Width; x++)
                {
                    GridPosition position = new GridPosition(x, y);
                    BoardCell cell = BuildCellFromToken(position, sourceRow[x], levelDefinition, itemsByLevel);
                    board.SetCell(cell);
                }
            }

            return board;
        }

        private static BoardCell BuildCellFromToken(
            GridPosition position,
            char token,
            LevelDefinition levelDefinition,
            Dictionary<int, ItemDefinition> itemsByLevel)
        {
            switch (token)
            {
                case '-':
                    return new BoardCell(position, CellKind.Empty);
                case 'L':
                    return new BoardCell(position, CellKind.Locked);
                case 'W':
                    return new BoardCell(position, CellKind.Obstacle)
                    {
                        ObstacleKind = ObstacleKind.Weed
                    };
                case 'P':
                    return new BoardCell(position, CellKind.Obstacle)
                    {
                        ObstacleKind = ObstacleKind.Pebble
                    };
                case 'S':
                    return new BoardCell(position, CellKind.Producer)
                    {
                        Producer = new ProducerRuntime(levelDefinition.DefaultProducer)
                    };
                case 'N':
                    return new BoardCell(position, CellKind.NpcOrderPoint);
                default:
                    if (char.IsDigit(token))
                    {
                        int level = token - '0';
                        if (!itemsByLevel.TryGetValue(level, out ItemDefinition itemDefinition) || itemDefinition == null)
                        {
                            throw new LevelParseException("No item definition found for item level " + level + ".");
                        }

                        return new BoardCell(position, CellKind.Empty)
                        {
                            Item = new BoardItem(itemDefinition.FamilyId, itemDefinition.Level, itemDefinition.ItemId)
                        };
                    }

                    throw new LevelParseException("Invalid level token '" + token + "' at " + position + ".");
            }
        }
    }
}
