using System.Collections.Generic;
using EcoGarden.Utilities;
using UnityEngine;

namespace EcoGarden.Board
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private float cellSize = 0.82f;
        [SerializeField] private float cellGap = 0.06f;
        [SerializeField] private float itemSizeRatio = 0.78f;
        [SerializeField] private Transform cellRoot;
        [SerializeField] private Transform itemRoot;

        private readonly Dictionary<GridPosition, CellView> cellViews = new Dictionary<GridPosition, CellView>();
        private readonly Dictionary<GridPosition, ItemView> itemViews = new Dictionary<GridPosition, ItemView>();
        private readonly List<GridPosition> staleItemPositions = new List<GridPosition>();

        public float CellSize { get { return cellSize; } }
        public float CellGap { get { return cellGap; } }
        public float ItemWorldSize { get { return cellSize * itemSizeRatio; } }

        public float GetBoardWorldWidth(BoardState boardState)
        {
            return boardState.Width * cellSize + (boardState.Width - 1) * cellGap;
        }

        public float GetBoardWorldHeight(BoardState boardState)
        {
            return boardState.Height * cellSize + (boardState.Height - 1) * cellGap;
        }

        public bool TryGetItemView(GridPosition position, out ItemView itemView)
        {
            return itemViews.TryGetValue(position, out itemView) && itemView != null;
        }

        public void Render(BoardState boardState)
        {
            Clear();
            EnsureRoots();

            foreach (BoardCell cell in boardState.GetCells())
            {
                CreateCellView(boardState, cell);

                if (cell.Item != null)
                {
                    CreateItemView(boardState, cell);
                }
            }
        }

        public void Sync(BoardState boardState)
        {
            if (boardState == null)
            {
                Clear();
                return;
            }

            if (cellViews.Count == 0)
            {
                Render(boardState);
                return;
            }

            EnsureRoots();

            staleItemPositions.Clear();
            foreach (GridPosition position in itemViews.Keys)
            {
                staleItemPositions.Add(position);
            }

            foreach (BoardCell cell in boardState.GetCells())
            {
                SyncCellView(boardState, cell);
                SyncItemView(boardState, cell);
                staleItemPositions.Remove(cell.Position);
            }

            for (int i = 0; i < staleItemPositions.Count; i++)
            {
                RemoveItemView(staleItemPositions[i]);
            }

            staleItemPositions.Clear();
        }

        public Vector3 GridToWorld(BoardState boardState, GridPosition position)
        {
            float step = cellSize + cellGap;
            float originX = -((boardState.Width - 1) * step) * 0.5f;
            float originY = -((boardState.Height - 1) * step) * 0.5f;
            return transform.position + new Vector3(originX + position.X * step, originY + position.Y * step, 0f);
        }

        public bool TryWorldToGrid(BoardState boardState, Vector3 worldPosition, out GridPosition position)
        {
            float step = cellSize + cellGap;
            Vector3 local = worldPosition - transform.position;
            float originX = -((boardState.Width - 1) * step) * 0.5f;
            float originY = -((boardState.Height - 1) * step) * 0.5f;

            int x = Mathf.RoundToInt((local.x - originX) / step);
            int y = Mathf.RoundToInt((local.y - originY) / step);
            position = new GridPosition(x, y);

            if (!boardState.IsInBounds(position))
            {
                return false;
            }

            Vector3 cellCenter = GridToWorld(boardState, position);
            float halfSize = cellSize * 0.5f;
            return Mathf.Abs(worldPosition.x - cellCenter.x) <= halfSize &&
                   Mathf.Abs(worldPosition.y - cellCenter.y) <= halfSize;
        }

        public void Clear()
        {
            DestroyChildren(cellRoot);
            DestroyChildren(itemRoot);
            cellViews.Clear();
            itemViews.Clear();
        }

        private void CreateCellView(BoardState boardState, BoardCell cell)
        {
            GameObject cellObject = new GameObject("Cell");
            cellObject.transform.SetParent(cellRoot, false);

            CellView cellView = cellObject.AddComponent<CellView>();
            cellView.Initialize(
                cell,
                GetCellSprite(cell),
                GetCellColor(cell),
                GridToWorld(boardState, cell.Position),
                new Vector2(cellSize, cellSize));
            cellViews[cell.Position] = cellView;
        }

        private void CreateItemView(BoardState boardState, BoardCell cell)
        {
            GameObject itemObject = new GameObject("Item");
            itemObject.transform.SetParent(itemRoot, false);

            ItemView itemView = itemObject.AddComponent<ItemView>();
            itemView.Initialize(
                cell.Item,
                cell.Position,
                PlaceholderSpriteFactory.GetLotusSprite(cell.Item.Level),
                Color.white,
                GridToWorld(boardState, cell.Position),
                ItemWorldSize);
            itemViews[cell.Position] = itemView;
        }

        private void SyncCellView(BoardState boardState, BoardCell cell)
        {
            if (!cellViews.TryGetValue(cell.Position, out CellView cellView) || cellView == null)
            {
                CreateCellView(boardState, cell);
                return;
            }

            cellView.Refresh(cell, GetCellSprite(cell), GetCellColor(cell));
        }

        private void SyncItemView(BoardState boardState, BoardCell cell)
        {
            if (cell.Item == null)
            {
                RemoveItemView(cell.Position);
                return;
            }

            if (!itemViews.TryGetValue(cell.Position, out ItemView itemView) || itemView == null)
            {
                CreateItemView(boardState, cell);
                return;
            }

            itemView.Refresh(
                cell.Item,
                cell.Position,
                PlaceholderSpriteFactory.GetLotusSprite(cell.Item.Level),
                Color.white,
                GridToWorld(boardState, cell.Position),
                ItemWorldSize);
        }

        private void RemoveItemView(GridPosition position)
        {
            if (!itemViews.TryGetValue(position, out ItemView itemView))
            {
                return;
            }

            if (itemView != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(itemView.gameObject);
                }
                else
                {
                    DestroyImmediate(itemView.gameObject);
                }
            }

            itemViews.Remove(position);
        }

        private void EnsureRoots()
        {
            if (cellRoot == null)
            {
                GameObject root = new GameObject("Cells");
                root.transform.SetParent(transform, false);
                cellRoot = root.transform;
            }

            if (itemRoot == null)
            {
                GameObject root = new GameObject("Items");
                root.transform.SetParent(transform, false);
                itemRoot = root.transform;
            }
        }

        private static void DestroyChildren(Transform root)
        {
            if (root == null)
            {
                return;
            }

            for (int i = root.childCount - 1; i >= 0; i--)
            {
                Transform child = root.GetChild(i);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private static Color GetCellColor(BoardCell cell)
        {
            switch (cell.Kind)
            {
                case CellKind.Locked:
                    return new Color(0.17f, 0.19f, 0.23f, 1f);
                case CellKind.Obstacle:
                    return cell.ObstacleKind == ObstacleKind.Weed
                        ? new Color(0.28f, 0.48f, 0.27f, 1f)
                        : new Color(0.48f, 0.43f, 0.39f, 1f);
                case CellKind.Producer:
                    return new Color(0.25f, 0.56f, 0.76f, 1f);
                case CellKind.NpcOrderPoint:
                    return new Color(0.62f, 0.38f, 0.78f, 1f);
                default:
                    return new Color(0.60f, 0.78f, 0.74f, 1f);
            }
        }

        private static Sprite GetCellSprite(BoardCell cell)
        {
            switch (cell.Kind)
            {
                case CellKind.Locked:
                    return PlaceholderSpriteFactory.LockedTileSprite;
                case CellKind.Obstacle:
                    return cell.ObstacleKind == ObstacleKind.Weed
                        ? PlaceholderSpriteFactory.WeedSprite
                        : PlaceholderSpriteFactory.PebbleSprite;
                case CellKind.Producer:
                    return PlaceholderSpriteFactory.ProducerSprite;
                default:
                    return PlaceholderSpriteFactory.EmptyTileSprite;
            }
        }

        private static Color GetItemColor(int level)
        {
            switch (level)
            {
                case 1:
                    return new Color(0.78f, 0.66f, 0.45f, 1f);
                case 2:
                    return new Color(0.45f, 0.76f, 0.47f, 1f);
                case 3:
                    return new Color(0.37f, 0.70f, 0.58f, 1f);
                case 4:
                    return new Color(0.84f, 0.58f, 0.72f, 1f);
                default:
                    return new Color(0.95f, 0.74f, 0.88f, 1f);
            }
        }
    }
}
