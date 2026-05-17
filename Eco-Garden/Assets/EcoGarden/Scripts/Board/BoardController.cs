using EcoGarden.Config;
using EcoGarden.Level;
using EcoGarden.Abilities;
using System;
using UnityEngine;

namespace EcoGarden.Board
{
    public sealed class BoardController : MonoBehaviour
    {
        [SerializeField] private LevelDefinition levelDefinition;
        [SerializeField] private BoardView boardView;
        [SerializeField] private bool loadOnStart = true;
        [SerializeField] private bool frameCameraOnLoad = true;
        [SerializeField] private float cameraPadding = 1.25f;
        [SerializeField] private float hudTopWorldPadding = 1.15f;

        public BoardState BoardState { get; private set; }
        public LevelDefinition LevelDefinition { get { return levelDefinition; } }
        public BoardView BoardView { get { return boardView; } }
        public AbilityInventory AbilityInventory { get; private set; }

        public event Action ObjectiveCompleted;

        private AbilityService abilityService;

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

            BoardState = LevelParser.Parse(levelDefinition);
            BuildAbilityService();
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

            bool changed = BoardState.TryMergeItem(from, to) || BoardState.TryMoveItem(from, to);
            if (!changed)
            {
                changed = TryDeliverToNpc(from, to);
            }

            if (changed && refreshView)
            {
                RefreshView();
            }

            return changed;
        }

        public bool TrySpawnFromProducer(GridPosition producerPosition, float currentTime)
        {
            if (BoardState == null)
            {
                return false;
            }

            bool changed = BoardState.TrySpawnFromProducer(producerPosition, currentTime, out _);
            if (changed)
            {
                RefreshView();
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

            bool changed = BoardState.TrySellItem(from, out goldValue);
            if (changed && refreshView)
            {
                RefreshView();
            }

            return changed;
        }

        public void RefreshView()
        {
            if (BoardState != null && boardView != null)
            {
                boardView.Render(BoardState);
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
            float sizeByHeight = boardHeight * 0.5f + cameraPadding + hudTopWorldPadding * 0.5f;
            float sizeByWidth = boardWidth * 0.5f / aspect + cameraPadding;
            mainCamera.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);

            // Offset upward so the board sits slightly below center, leaving room for overlay HUD.
            mainCamera.transform.position = new Vector3(
                boardView.transform.position.x,
                boardView.transform.position.y + hudTopWorldPadding * 0.5f,
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

        private bool TryDeliverToNpc(GridPosition from, GridPosition to)
        {
            BoardCell source = BoardState.GetCell(from);
            BoardCell target = BoardState.GetCell(to);

            if (source == null ||
                target == null ||
                source.Item == null ||
                target.Kind != CellKind.NpcOrderPoint ||
                levelDefinition == null ||
                levelDefinition.NpcOrder == null)
            {
                return false;
            }

            if (source.Item.FamilyId != levelDefinition.NpcOrder.FamilyId ||
                source.Item.Level != levelDefinition.NpcOrder.Level)
            {
                return false;
            }

            source.Item = null;
            ObjectiveCompleted?.Invoke();
            return true;
        }
    }
}
