using EcoGarden.Config;
using EcoGarden.Level;
using EcoGarden.Abilities;
using EcoGarden.Economy;
using EcoGarden.Items;
using EcoGarden.Progression;
using EcoGarden.Rewards;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EcoGarden.Board
{
    public sealed class BoardController : MonoBehaviour
    {
        [SerializeField] private LevelDefinition levelDefinition;
        [SerializeField] private BoardView boardView;
        [SerializeField] private bool loadOnStart = true;
        [SerializeField] private bool frameCameraOnLoad = true;
        [SerializeField] private float cameraPadding = 1.55f;
        [SerializeField] private float hudTopWorldPadding = 1.25f;
        [SerializeField] private float hudBottomWorldPadding = 1.25f;

        public BoardState BoardState { get; private set; }
        public LevelDefinition LevelDefinition { get { return levelDefinition; } }
        public BoardView BoardView { get { return boardView; } }
        public AbilityInventory AbilityInventory { get; private set; }
        public PlantUnlockService PlantUnlockService { get; private set; }
        public IReadOnlyList<OrderRequirementRuntimeState> ActiveOrderRequirements { get { return activeOrderRequirements; } }
        public string ActiveOrderId
        {
            get
            {
                return levelDefinition != null && levelDefinition.NpcOrder != null
                    ? levelDefinition.NpcOrder.OrderId
                    : string.Empty;
            }
        }

        public event Action BoardChanged;
        public event Action OrderProgressChanged;
        public event Action OrderCompleted;
        public event Action<BoardItem> ItemMerged;
        public event Action<BoardItem> ItemProduced;
        public event Action<BoardItem> ItemSold;
        public event Action<BoardItem> ItemDelivered;
        public event Action<AbilityKind> AbilityUsed;

        private AbilityService abilityService;
        private readonly List<OrderRequirementRuntimeState> activeOrderRequirements = new List<OrderRequirementRuntimeState>();

        private void Reset()
        {
            boardView = GetComponent<BoardView>();
        }

        private void Start()
        {
            if (loadOnStart)
            {
                LoadLevel();
            }
        }

        public void SetLevelDefinition(LevelDefinition definition)
        {
            levelDefinition = definition;
        }

        public void LoadLevel()
        {
            if (levelDefinition == null)
            {
                Debug.LogError("BoardController cannot load level because no LevelDefinition is assigned.", this);
                return;
            }

            if (boardView == null)
            {
                boardView = GetComponent<BoardView>();
            }

            if (boardView == null)
            {
                Debug.LogError("BoardController requires a BoardView.", this);
                return;
            }

            BuildPlantUnlockService();
            BoardState = LevelParser.Parse(levelDefinition, PlantUnlockService);
            BuildAbilityService();
            BuildOrderState();
            RefreshView();

            if (frameCameraOnLoad)
            {
                FrameCamera();
            }
        }

        public bool TryGetGridPosition(Vector3 worldPosition, out GridPosition position)
        {
            position = default;
            return BoardState != null &&
                   boardView != null &&
                   boardView.TryWorldToGrid(BoardState, worldPosition, out position);
        }

        public bool TryGetItemView(GridPosition position, out ItemView itemView)
        {
            itemView = null;
            return boardView != null && boardView.TryGetItemView(position, out itemView);
        }

        public Vector3 GetCellWorldPosition(GridPosition position)
        {
            return boardView.GridToWorld(BoardState, position);
        }

        public bool TryMoveOrMerge(GridPosition from, GridPosition to, bool refreshView = true)
        {
            if (BoardState == null)
            {
                return false;
            }

            BoardItem sourceItem = BoardState.GetCell(from) != null ? BoardState.GetCell(from).Item : null;
            bool willMerge = WillMerge(from, to);
            bool changed = TryDeliverToNpc(from, to) || BoardState.TryMergeItem(from, to) || BoardState.TryMoveItem(from, to);

            if (changed && refreshView)
            {
                RefreshView();
            }

            if (changed)
            {
                BoardChanged?.Invoke();
                if (willMerge)
                {
                    ItemMerged?.Invoke(sourceItem);
                }
            }

            return changed;
        }

        public bool TryDeliverOrder(GridPosition from, bool refreshView = true)
        {
            if (BoardState == null)
            {
                return false;
            }

            BoardCell source = BoardState.GetCell(from);
            if (source == null || source.Item == null || !TrySubmitOrderItem(source.Item))
            {
                return false;
            }

            BoardItem deliveredItem = source.Item;
            source.Item = null;
            if (refreshView)
            {
                RefreshView();
            }

            BoardChanged?.Invoke();
            OrderProgressChanged?.Invoke();
            ItemDelivered?.Invoke(deliveredItem);

            if (IsActiveOrderComplete())
            {
                GrantOrderReward();
                OrderCompleted?.Invoke();
            }

            return true;
        }

        public bool TrySpawnFromProducer(GridPosition producerPosition, float currentTime)
        {
            if (BoardState == null)
            {
                return false;
            }

            bool changed = BoardState.TrySpawnFromProducer(producerPosition, currentTime, out GridPosition spawnPosition);
            if (changed)
            {
                RefreshView();
                BoardChanged?.Invoke();
                BoardCell spawnedCell = BoardState.GetCell(spawnPosition);
                if (spawnedCell != null)
                {
                    ItemProduced?.Invoke(spawnedCell.Item);
                }
            }

            return changed;
        }

        public bool TryUseAbility(AbilityKind abilityKind, GridPosition targetPosition)
        {
            if (abilityService == null)
            {
                return false;
            }

            bool changed;
            switch (abilityKind)
            {
                case AbilityKind.Shovel:
                    changed = abilityService.TryUseShovel(targetPosition);
                    break;
                case AbilityKind.MagicWand:
                    changed = abilityService.TryUseMagicWand(targetPosition);
                    break;
                case AbilityKind.SortingMagnet:
                    changed = abilityService.TryUseSortingMagnet("lotus", out _, out _);
                    break;
                default:
                    changed = false;
                    break;
            }

            if (changed)
            {
                RefreshView();
                BoardChanged?.Invoke();
                AbilityUsed?.Invoke(abilityKind);
            }

            return changed;
        }

        public bool TrySellItem(GridPosition from, out int goldValue, bool refreshView = true)
        {
            goldValue = 0;
            if (BoardState == null)
            {
                return false;
            }

            BoardCell source = BoardState.GetCell(from);
            BoardItem soldItem = source != null ? source.Item : null;
            bool changed = BoardState.TrySellItem(from, out goldValue);
            if (changed && refreshView)
            {
                RefreshView();
            }

            if (changed)
            {
                BoardChanged?.Invoke();
                ItemSold?.Invoke(soldItem);
            }

            return changed;
        }

        public void SetAbilityCount(AbilityKind abilityKind, int count)
        {
            if (AbilityInventory != null)
            {
                AbilityInventory.SetCount(abilityKind, count);
            }
        }

        public void SetOrderSubmittedCount(string familyId, int level, int submittedCount)
        {
            for (int i = 0; i < activeOrderRequirements.Count; i++)
            {
                OrderRequirementRuntimeState requirement = activeOrderRequirements[i];
                if (requirement.FamilyId == familyId && requirement.Level == level)
                {
                    requirement.SetSubmittedCount(submittedCount);
                    OrderProgressChanged?.Invoke();
                    return;
                }
            }
        }

        public void RebuildOrderState()
        {
            BuildOrderState();
            OrderProgressChanged?.Invoke();
        }

        public void StartNextOrder()
        {
            BuildOrderState();
            OrderProgressChanged?.Invoke();
            BoardChanged?.Invoke();
        }

        public bool IsActiveOrderCompleteForSave()
        {
            return IsActiveOrderComplete();
        }

        public List<GridPosition> CaptureClearedObstaclePositions()
        {
            List<GridPosition> positions = new List<GridPosition>();
            if (BoardState == null || levelDefinition == null || levelDefinition.RowsTopToBottom == null)
            {
                return positions;
            }

            IReadOnlyList<string> rows = levelDefinition.RowsTopToBottom;
            if (rows.Count != levelDefinition.Height)
            {
                return positions;
            }

            for (int row = 0; row < rows.Count; row++)
            {
                string sourceRow = rows[row];
                if (string.IsNullOrEmpty(sourceRow) || sourceRow.Length != levelDefinition.Width)
                {
                    continue;
                }

                int y = levelDefinition.Height - 1 - row;
                for (int x = 0; x < levelDefinition.Width; x++)
                {
                    if (!IsObstacleToken(sourceRow[x]))
                    {
                        continue;
                    }

                    GridPosition position = new GridPosition(x, y);
                    BoardCell cell = BoardState.GetCell(position);
                    if (cell != null && cell.Kind != CellKind.Obstacle)
                    {
                        positions.Add(position);
                    }
                }
            }

            return positions;
        }

        public void RestoreClearedObstacles(IEnumerable<GridPosition> positions, bool refreshView = true)
        {
            if (BoardState == null || positions == null)
            {
                return;
            }

            bool changed = false;
            foreach (GridPosition position in positions)
            {
                changed |= BoardState.TryRemoveObstacle(position);
            }

            if (changed && refreshView)
            {
                RefreshView();
            }
        }

        public void RestoreBoardItems(System.Collections.Generic.IEnumerable<EcoGarden.Items.BoardItem> items, System.Collections.Generic.IEnumerable<GridPosition> positions)
        {
            if (BoardState == null || items == null || positions == null)
            {
                return;
            }

            BoardState.ClearItems();

            using (System.Collections.Generic.IEnumerator<EcoGarden.Items.BoardItem> itemEnumerator = items.GetEnumerator())
            using (System.Collections.Generic.IEnumerator<GridPosition> positionEnumerator = positions.GetEnumerator())
            {
                while (itemEnumerator.MoveNext() && positionEnumerator.MoveNext())
                {
                    BoardState.TryPlaceItem(positionEnumerator.Current, itemEnumerator.Current);
                }
            }

            RefreshView();
        }

        public void RefreshView()
        {
            if (BoardState != null && boardView != null)
            {
                boardView.Sync(BoardState);
            }
        }

        private void FrameCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null || !mainCamera.orthographic || BoardState == null)
            {
                return;
            }

            float boardHeight = boardView.GetBoardWorldHeight(BoardState);
            float boardWidth = boardView.GetBoardWorldWidth(BoardState);
            float aspect = mainCamera.aspect > 0f ? mainCamera.aspect : 1f;
            float safeCameraPadding = Mathf.Max(cameraPadding, 1.55f);
            float safeHudTopPadding = Mathf.Max(hudTopWorldPadding, 1.25f);
            float safeHudBottomPadding = Mathf.Max(hudBottomWorldPadding, 1.25f);
            float sizeByHeight = boardHeight * 0.5f + safeCameraPadding + (safeHudTopPadding + safeHudBottomPadding) * 0.5f;
            float sizeByWidth = boardWidth * 0.5f / aspect + safeCameraPadding;
            mainCamera.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);

            // Offset toward available play space between top and bottom HUD.
            mainCamera.transform.position = new Vector3(
                boardView.transform.position.x,
                boardView.transform.position.y + (safeHudTopPadding - safeHudBottomPadding) * 0.5f,
                mainCamera.transform.position.z);
        }

        private void BuildAbilityService()
        {
            AbilityInventory = new AbilityInventory();

            if (levelDefinition != null)
            {
                for (int i = 0; i < levelDefinition.StartingAbilities.Count; i++)
                {
                    AbilityCountDefinition count = levelDefinition.StartingAbilities[i];
                    if (count != null)
                    {
                        AbilityInventory.SetCount(count.AbilityKind, count.Count);
                    }
                }
            }

            abilityService = new AbilityService(BoardState, AbilityInventory);
        }

        private void BuildPlantUnlockService()
        {
            PlantUnlockService = new PlantUnlockService();
            if (levelDefinition != null)
            {
                PlantUnlockService.SetTemporaryAllowedTiers(levelDefinition.TemporaryAllowedPlantTiers);
            }
        }

        private void BuildOrderState()
        {
            activeOrderRequirements.Clear();
            if (levelDefinition == null || levelDefinition.NpcOrder == null)
            {
                return;
            }

            IReadOnlyList<OrderRequirementDefinition> requirements = levelDefinition.NpcOrder.Requirements;
            for (int i = 0; i < requirements.Count; i++)
            {
                OrderRequirementDefinition requirement = requirements[i];
                if (requirement != null)
                {
                    if (PlantUnlockService == null || PlantUnlockService.IsRequirementAllowed(requirement))
                    {
                        activeOrderRequirements.Add(new OrderRequirementRuntimeState(requirement));
                    }
                }
            }
        }

        private bool TryDeliverToNpc(GridPosition from, GridPosition to)
        {
            BoardCell source = BoardState.GetCell(from);
            BoardCell target = BoardState.GetCell(to);

            if (target == null ||
                target.Kind != CellKind.NpcOrderPoint ||
                source == null ||
                source.Item == null ||
                !TrySubmitOrderItem(source.Item))
            {
                return false;
            }

            BoardItem deliveredItem = source.Item;
            source.Item = null;
            BoardChanged?.Invoke();
            OrderProgressChanged?.Invoke();
            ItemDelivered?.Invoke(deliveredItem);

            if (IsActiveOrderComplete())
            {
                GrantOrderReward();
                OrderCompleted?.Invoke();
            }

            return true;
        }

        private static bool IsObstacleToken(char token)
        {
            return token == 'W' || token == 'P';
        }

        private bool WillMerge(GridPosition from, GridPosition to)
        {
            BoardCell source = BoardState.GetCell(from);
            BoardCell target = BoardState.GetCell(to);
            return source != null &&
                   target != null &&
                   source.Item != null &&
                   target.Item != null &&
                   source.Item.CanMergeWith(target.Item, BoardState.MaxItemLevel);
        }

        private bool CanDeliverOrder(BoardCell source)
        {
            return source != null &&
                   source.Item != null &&
                   levelDefinition != null &&
                   levelDefinition.NpcOrder != null &&
                   CanSubmitOrderItem(source.Item);
        }

        private bool CanSubmitOrderItem(EcoGarden.Items.BoardItem item)
        {
            if (item == null)
            {
                return false;
            }

            for (int i = 0; i < activeOrderRequirements.Count; i++)
            {
                OrderRequirementRuntimeState requirement = activeOrderRequirements[i];
                if (!requirement.IsComplete &&
                    requirement.FamilyId == item.FamilyId &&
                    requirement.Level == item.Level)
                {
                    return true;
                }
            }

            return false;
        }

        private bool TrySubmitOrderItem(EcoGarden.Items.BoardItem item)
        {
            if (item == null)
            {
                return false;
            }

            for (int i = 0; i < activeOrderRequirements.Count; i++)
            {
                if (activeOrderRequirements[i].TrySubmit(item))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsActiveOrderComplete()
        {
            if (activeOrderRequirements.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < activeOrderRequirements.Count; i++)
            {
                if (!activeOrderRequirements[i].IsComplete)
                {
                    return false;
                }
            }

            return true;
        }

        private void GrantOrderReward()
        {
            if (levelDefinition == null || levelDefinition.NpcOrder == null)
            {
                return;
            }

            EconomyController economyController = FindAnyObjectByType<EconomyController>();
            if (levelDefinition.NpcOrder.Reward != null)
            {
                RewardService.Grant(levelDefinition.NpcOrder.Reward, economyController, AbilityInventory, PlantUnlockService);
                return;
            }

            if (economyController != null)
            {
                economyController.AddGold(BuildFallbackOrderRewardGold());
            }
        }

        private int BuildFallbackOrderRewardGold()
        {
            int totalValue = 0;
            for (int i = 0; i < activeOrderRequirements.Count; i++)
            {
                OrderRequirementRuntimeState requirement = activeOrderRequirements[i];
                ItemDefinition itemDefinition = levelDefinition.GetItemDefinitionForLevel(requirement.Level);
                int itemValue = itemDefinition != null ? itemDefinition.SellValue : requirement.Level;
                totalValue += itemValue * requirement.RequiredCount;
            }

            return Mathf.Max(1, totalValue * 2);
        }
    }
}
