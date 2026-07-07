using System.Collections.Generic;
using EcoGarden.Utilities;
using UnityEngine;

namespace EcoGarden.Board
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private float cellSize = 0.74f;
        [SerializeField] private float cellGap = 0.055f;
        [SerializeField] private float itemSizeRatio = 0.82f;
        [SerializeField] private bool useDiamondLayout;
        [SerializeField] private float diamondVerticalRatio = 0.58f;
        [SerializeField] private Transform cellRoot;
        [SerializeField] private Transform itemRoot;

        private readonly Dictionary<GridPosition, CellView> cellViews = new Dictionary<GridPosition, CellView>();
        private readonly Dictionary<GridPosition, ItemView> itemViews = new Dictionary<GridPosition, ItemView>();
        private readonly List<GridPosition> staleItemPositions = new List<GridPosition>();
        private BoardState lastBoardState;
        private Color emptyEdgeColor = new Color(0.42f, 0.68f, 0.66f, 1f);
        private Color emptyCenterColor = new Color(0.58f, 0.78f, 0.70f, 1f);
        private Color producerColor = new Color(0.20f, 0.49f, 0.62f, 1f);

        public float CellSize { get { return cellSize; } }
        public float CellGap { get { return cellGap; } }
        public float ItemWorldSize { get { return cellSize * itemSizeRatio; } }

        public float GetBoardWorldWidth(BoardState boardState)
        {
            if (useDiamondLayout)
            {
                return (boardState.Width + boardState.Height - 2) * GetDiamondStepX() * 0.5f + cellSize;
            }

            return boardState.Width * cellSize + (boardState.Width - 1) * cellGap;
        }

        public float GetBoardWorldHeight(BoardState boardState)
        {
            if (useDiamondLayout)
            {
                return (boardState.Width + boardState.Height - 2) * GetDiamondStepY() * 0.5f + cellSize * diamondVerticalRatio;
            }

            return boardState.Height * cellSize + (boardState.Height - 1) * cellGap;
        }

        public bool TryGetItemView(GridPosition position, out ItemView itemView)
        {
            return itemViews.TryGetValue(position, out itemView) && itemView != null;
        }

        public void Render(BoardState boardState)
        {
            lastBoardState = boardState;
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
            lastBoardState = boardState;
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
            if (useDiamondLayout)
            {
                float centerX = (boardState.Width - 1) * 0.5f;
                float centerY = (boardState.Height - 1) * 0.5f;
                float dx = position.X - centerX;
                float dy = position.Y - centerY;
                float worldX = (dx - dy) * GetDiamondStepX() * 0.5f;
                float worldY = (dx + dy) * GetDiamondStepY() * 0.5f;
                return transform.position + new Vector3(worldX, worldY, 0f);
            }

            float step = cellSize + cellGap;
            float originX = -((boardState.Width - 1) * step) * 0.5f;
            float originY = -((boardState.Height - 1) * step) * 0.5f;
            return transform.position + new Vector3(originX + position.X * step, originY + position.Y * step, 0f);
        }

        public bool TryWorldToGrid(BoardState boardState, Vector3 worldPosition, out GridPosition position)
        {
            if (useDiamondLayout)
            {
                Vector3 diamondLocal = worldPosition - transform.position;
                float stepX = GetDiamondStepX();
                float stepY = GetDiamondStepY();
                float diff = stepX > 0f ? diamondLocal.x * 2f / stepX : 0f;
                float sum = stepY > 0f ? diamondLocal.y * 2f / stepY : 0f;
                float dx = (sum + diff) * 0.5f;
                float dy = (sum - diff) * 0.5f;
                float centerX = (boardState.Width - 1) * 0.5f;
                float centerY = (boardState.Height - 1) * 0.5f;

                position = new GridPosition(
                    Mathf.RoundToInt(dx + centerX),
                    Mathf.RoundToInt(dy + centerY));

                if (!boardState.IsInBounds(position))
                {
                    return false;
                }

                Vector3 diamondCellCenter = GridToWorld(boardState, position);
                float localDeltaX = Mathf.Abs(worldPosition.x - diamondCellCenter.x);
                float localDeltaY = Mathf.Abs(worldPosition.y - diamondCellCenter.y);
                float halfWidth = cellSize * 0.5f;
                float halfHeight = cellSize * diamondVerticalRatio * 0.5f;
                return halfWidth > 0f &&
                       halfHeight > 0f &&
                       localDeltaX / halfWidth + localDeltaY / halfHeight <= 1.12f;
            }

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
            Vector2 visualSize = GetCellVisualSize();
            cellView.Initialize(
                cell,
                GetCellSprite(cell),
                GetCellColor(boardState, cell),
                GridToWorld(boardState, cell.Position),
                visualSize,
                GetCellRotation());
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

            cellView.Refresh(cell, GetCellSprite(cell), GetCellColor(boardState, cell), GetCellRotation());
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

        public void SetCosmeticTilePalette(Color edgeColor, Color centerColor, Color producerAccentColor)
        {
            emptyEdgeColor = edgeColor;
            emptyCenterColor = centerColor;
            producerColor = producerAccentColor;
            RefreshCellColors();
        }

        public void ResetCosmeticTilePalette()
        {
            SetCosmeticTilePalette(
                new Color(0.42f, 0.68f, 0.66f, 1f),
                new Color(0.58f, 0.78f, 0.70f, 1f),
                new Color(0.20f, 0.49f, 0.62f, 1f));
        }

        private void RefreshCellColors()
        {
            if (lastBoardState == null)
            {
                return;
            }

            foreach (KeyValuePair<GridPosition, CellView> entry in cellViews)
            {
                CellView cellView = entry.Value;
                BoardCell cell = cellView != null ? cellView.Cell : null;
                if (cell != null)
                {
                    cellView.Refresh(cell, GetCellSprite(cell), GetCellColor(lastBoardState, cell), GetCellRotation());
                }
            }
        }

        private Color GetCellColor(BoardState boardState, BoardCell cell)
        {
            float centerWeight = GetDiamondCenterWeight(boardState, cell.Position);

            switch (cell.Kind)
            {
                case CellKind.Locked:
                    return new Color(0.13f, 0.17f, 0.19f, Mathf.Lerp(0.52f, 0.72f, centerWeight));
                case CellKind.Obstacle:
                    return cell.ObstacleKind == ObstacleKind.Weed
                        ? new Color(0.22f, 0.40f, 0.26f, Mathf.Lerp(0.58f, 0.78f, centerWeight))
                        : new Color(0.38f, 0.35f, 0.32f, Mathf.Lerp(0.58f, 0.78f, centerWeight));
                case CellKind.Producer:
                    return WithAlpha(producerColor, Mathf.Lerp(0.52f, 0.70f, centerWeight));
                case CellKind.NpcOrderPoint:
                    return new Color(0.52f, 0.36f, 0.66f, Mathf.Lerp(0.48f, 0.64f, centerWeight));
                default:
                    return WithAlpha(Color.Lerp(emptyEdgeColor, emptyCenterColor, centerWeight), Mathf.Lerp(0.38f, 0.64f, centerWeight));
            }
        }

        private static float GetDiamondCenterWeight(BoardState boardState, GridPosition position)
        {
            if (boardState == null || boardState.Width <= 1 || boardState.Height <= 1)
            {
                return 1f;
            }

            float centerX = (boardState.Width - 1) * 0.5f;
            float centerY = (boardState.Height - 1) * 0.5f;
            float normalizedX = Mathf.Abs(position.X - centerX) / Mathf.Max(1f, centerX);
            float normalizedY = Mathf.Abs(position.Y - centerY) / Mathf.Max(1f, centerY);
            float diamondDistance = (normalizedX + normalizedY) * 0.5f;
            return Mathf.Clamp01(1.08f - diamondDistance);
        }

        private float GetDiamondStepX()
        {
            return cellSize + cellGap;
        }

        private float GetDiamondStepY()
        {
            return (cellSize + cellGap) * Mathf.Max(0.2f, diamondVerticalRatio);
        }

        private Vector2 GetCellVisualSize()
        {
            return useDiamondLayout
                ? new Vector2(cellSize, cellSize * Mathf.Max(0.2f, diamondVerticalRatio))
                : new Vector2(cellSize, cellSize);
        }

        private float GetCellRotation()
        {
            return useDiamondLayout ? 45f : 0f;
        }

        private static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
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
