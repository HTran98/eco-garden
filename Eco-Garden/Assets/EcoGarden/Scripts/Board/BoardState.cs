using System.Collections.Generic;
using EcoGarden.Config;
using EcoGarden.Items;

namespace EcoGarden.Board
{
    public sealed class BoardState
    {
        private static readonly GridPosition[] CardinalDirections =
        {
            new GridPosition(1, 0),
            new GridPosition(-1, 0),
            new GridPosition(0, 1),
            new GridPosition(0, -1)
        };

        private readonly BoardCell[,] cells;
        private readonly Dictionary<int, ItemDefinition> itemDefinitionsByLevel;

        public int Width { get; }
        public int Height { get; }
        public int MaxItemLevel { get; }

        public BoardState(int width, int height, Dictionary<int, ItemDefinition> itemDefinitionsByLevel)
        {
            Width = width;
            Height = height;
            this.itemDefinitionsByLevel = itemDefinitionsByLevel ?? new Dictionary<int, ItemDefinition>();
            MaxItemLevel = 0;

            foreach (int level in this.itemDefinitionsByLevel.Keys)
            {
                if (level > MaxItemLevel)
                {
                    MaxItemLevel = level;
                }
            }

            cells = new BoardCell[width, height];
        }

        public void SetCell(BoardCell cell)
        {
            cells[cell.Position.X, cell.Position.Y] = cell;
        }

        public bool IsInBounds(GridPosition position)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < Width && position.Y < Height;
        }

        public BoardCell GetCell(GridPosition position)
        {
            return IsInBounds(position) ? cells[position.X, position.Y] : null;
        }

        public IEnumerable<BoardCell> GetCells()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return cells[x, y];
                }
            }
        }

        public void ClearItems()
        {
            foreach (BoardCell cell in GetCells())
            {
                if (cell != null)
                {
                    cell.Item = null;
                }
            }
        }

        public bool TryPlaceItem(GridPosition position, BoardItem item)
        {
            BoardCell cell = GetCell(position);
            if (cell == null || item == null || cell.Kind != CellKind.Empty)
            {
                return false;
            }

            cell.Item = item;
            return true;
        }

        public bool TryMoveItem(GridPosition from, GridPosition to)
        {
            BoardCell source = GetCell(from);
            BoardCell target = GetCell(to);

            if (source == null || target == null || source.Item == null || !target.CanReceiveItem)
            {
                return false;
            }

            target.Item = source.Item;
            source.Item = null;
            return true;
        }

        public bool TryMergeItem(GridPosition from, GridPosition to)
        {
            BoardCell source = GetCell(from);
            BoardCell target = GetCell(to);

            if (source == null || target == null || source.Item == null || target.Item == null)
            {
                return false;
            }

            if (!source.Item.CanMergeWith(target.Item, MaxItemLevel))
            {
                return false;
            }

            int nextLevel = target.Item.Level + 1;
            string nextItemId = GetItemId(nextLevel, target.Item);
            target.Item = target.Item.CreateUpgraded(nextItemId);
            source.Item = null;
            return true;
        }

        public bool TryRemoveObstacle(GridPosition position)
        {
            BoardCell cell = GetCell(position);
            if (cell == null || cell.Kind != CellKind.Obstacle)
            {
                return false;
            }

            cell.Kind = CellKind.Empty;
            cell.ObstacleKind = ObstacleKind.None;
            return true;
        }

        public bool TryUpgradeItem(GridPosition position)
        {
            BoardCell cell = GetCell(position);
            if (cell == null || cell.Item == null || cell.Item.Level >= MaxItemLevel)
            {
                return false;
            }

            int nextLevel = cell.Item.Level + 1;
            string nextItemId = GetItemId(nextLevel, cell.Item);
            cell.Item = cell.Item.CreateUpgraded(nextItemId);
            return true;
        }

        public bool TrySpawnFromProducer(GridPosition producerPosition, float currentTime, out GridPosition spawnPosition)
        {
            spawnPosition = default;
            BoardCell producerCell = GetCell(producerPosition);

            if (producerCell == null ||
                producerCell.Kind != CellKind.Producer ||
                producerCell.Producer == null ||
                producerCell.Producer.Definition == null ||
                producerCell.Producer.Definition.SpawnItem == null ||
                !producerCell.Producer.IsReady(currentTime))
            {
                return false;
            }

            if (!TryFindNearestEmptyCell(producerPosition, out spawnPosition))
            {
                return false;
            }

            ItemDefinition item = producerCell.Producer.Definition.SpawnItem;
            BoardCell target = GetCell(spawnPosition);
            target.Item = new BoardItem(item.FamilyId, item.Level, item.ItemId);
            producerCell.Producer.StartCooldown(currentTime);
            return true;
        }

        public bool TryUseSortingMagnet(string familyId, out GridPosition movedFrom, out GridPosition movedTo)
        {
            movedFrom = default;
            movedTo = default;

            List<BoardCell> items = new List<BoardCell>();
            foreach (BoardCell cell in GetCells())
            {
                if (cell != null && cell.Item != null && cell.Item.FamilyId == familyId)
                {
                    items.Add(cell);
                }
            }

            BoardCell bestA = null;
            BoardCell bestB = null;
            int bestLevel = int.MaxValue;
            int bestDistance = int.MaxValue;

            for (int i = 0; i < items.Count; i++)
            {
                for (int j = i + 1; j < items.Count; j++)
                {
                    BoardCell a = items[i];
                    BoardCell b = items[j];
                    if (a.Item.Level != b.Item.Level)
                    {
                        continue;
                    }

                    int distance = a.Position.ManhattanDistance(b.Position);
                    if (a.Item.Level < bestLevel || (a.Item.Level == bestLevel && distance < bestDistance))
                    {
                        bestA = a;
                        bestB = b;
                        bestLevel = a.Item.Level;
                        bestDistance = distance;
                    }
                }
            }

            if (bestA == null || bestB == null)
            {
                return false;
            }

            if (TryMoveNextTo(bestB, bestA, out movedTo))
            {
                movedFrom = bestB.Position;
                return true;
            }

            if (TryMoveNextTo(bestA, bestB, out movedTo))
            {
                movedFrom = bestA.Position;
                return true;
            }

            return false;
        }

        public bool TrySellItem(GridPosition from, out int goldValue)
        {
            goldValue = 0;
            BoardCell source = GetCell(from);
            if (source == null || source.Item == null)
            {
                return false;
            }

            goldValue = GetSellValue(source.Item.Level);
            source.Item = null;
            return true;
        }

        private bool TryMoveNextTo(BoardCell movingCell, BoardCell anchorCell, out GridPosition destination)
        {
            for (int i = 0; i < CardinalDirections.Length; i++)
            {
                GridPosition direction = CardinalDirections[i];
                destination = new GridPosition(anchorCell.Position.X + direction.X, anchorCell.Position.Y + direction.Y);
                if (TryMoveItem(movingCell.Position, destination))
                {
                    return true;
                }
            }

            destination = default;
            return false;
        }

        private bool TryFindNearestEmptyCell(GridPosition origin, out GridPosition result)
        {
            Queue<GridPosition> open = new Queue<GridPosition>();
            HashSet<GridPosition> visited = new HashSet<GridPosition>();
            open.Enqueue(origin);
            visited.Add(origin);

            while (open.Count > 0)
            {
                GridPosition current = open.Dequeue();
                BoardCell currentCell = GetCell(current);

                if (current != origin && currentCell != null && currentCell.CanReceiveItem)
                {
                    result = current;
                    return true;
                }

                for (int i = 0; i < CardinalDirections.Length; i++)
                {
                    GridPosition direction = CardinalDirections[i];
                    GridPosition next = new GridPosition(current.X + direction.X, current.Y + direction.Y);
                    if (IsInBounds(next) && !visited.Contains(next))
                    {
                        visited.Add(next);
                        open.Enqueue(next);
                    }
                }
            }

            result = default;
            return false;
        }

        private string GetItemId(int level, BoardItem fallback)
        {
            if (itemDefinitionsByLevel.TryGetValue(level, out ItemDefinition definition) && definition != null)
            {
                return definition.ItemId;
            }

            return fallback.FamilyId + "_lv" + level.ToString("00");
        }

        private int GetSellValue(int level)
        {
            if (itemDefinitionsByLevel.TryGetValue(level, out ItemDefinition definition) && definition != null)
            {
                return definition.SellValue;
            }

            return 0;
        }
    }
}
