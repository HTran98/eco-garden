using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Economy;
using EcoGarden.Items;
using System.Collections.Generic;
using UnityEngine;

namespace EcoGarden.Save
{
    public sealed class SaveController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private EconomyController economyController;

        private SaveData data;
        private bool isApplying;
        private bool isSubscribed;

        public SaveData Data { get { return data; } }

        private void Awake()
        {
            ResolveReferences();
            data = SaveService.Load();
        }

        private void Start()
        {
            ApplyLoadedData();
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
            SaveCurrentState();
        }

        public void SaveCurrentState()
        {
            if (data == null)
            {
                data = SaveService.Load();
            }

            CaptureCurrentState();
            SaveService.Save(data);
        }

        private void ApplyLoadedData()
        {
            if (data == null)
            {
                return;
            }

            isApplying = true;

            if (economyController != null)
            {
                economyController.SetGold(data.gold);
            }

            if (boardController != null && boardController.AbilityInventory != null)
            {
                if (data.shovelCount >= 0)
                {
                    boardController.SetAbilityCount(AbilityKind.Shovel, data.shovelCount);
                }

                if (data.magicWandCount >= 0)
                {
                    boardController.SetAbilityCount(AbilityKind.MagicWand, data.magicWandCount);
                }

                if (data.sortingMagnetCount >= 0)
                {
                    boardController.SetAbilityCount(AbilityKind.SortingMagnet, data.sortingMagnetCount);
                }
            }

            ApplyBoardItems();

            isApplying = false;
        }

        private void CaptureCurrentState()
        {
            ResolveReferences();

            if (economyController != null)
            {
                data.gold = economyController.Gold;
            }

            if (boardController != null && boardController.LevelDefinition != null)
            {
                if (data.highestUnlockedLevel <= 0)
                {
                    data.highestUnlockedLevel = boardController.LevelDefinition.LevelId;
                }
            }

            if (boardController != null && boardController.AbilityInventory != null)
            {
                data.shovelCount = boardController.AbilityInventory.GetCount(AbilityKind.Shovel);
                data.magicWandCount = boardController.AbilityInventory.GetCount(AbilityKind.MagicWand);
                data.sortingMagnetCount = boardController.AbilityInventory.GetCount(AbilityKind.SortingMagnet);
            }

            CaptureBoardItems();
        }

        private void Subscribe()
        {
            if (isSubscribed)
            {
                return;
            }

            ResolveReferences();

            if (economyController != null)
            {
                economyController.GoldChanged += OnGoldChanged;
            }

            if (boardController != null)
            {
                boardController.BoardChanged += OnBoardChanged;
                boardController.ObjectiveCompleted += OnObjectiveCompleted;

                if (boardController.AbilityInventory != null)
                {
                    boardController.AbilityInventory.CountChanged += OnAbilityCountChanged;
                }
            }

            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed)
            {
                return;
            }

            if (economyController != null)
            {
                economyController.GoldChanged -= OnGoldChanged;
            }

            if (boardController != null)
            {
                boardController.BoardChanged -= OnBoardChanged;
                boardController.ObjectiveCompleted -= OnObjectiveCompleted;

                if (boardController.AbilityInventory != null)
                {
                    boardController.AbilityInventory.CountChanged -= OnAbilityCountChanged;
                }
            }

            isSubscribed = false;
        }

        private void OnGoldChanged(int gold)
        {
            if (isApplying)
            {
                return;
            }

            data.gold = gold;
            SaveService.Save(data);
        }

        private void OnBoardChanged()
        {
            if (isApplying)
            {
                return;
            }

            CaptureBoardItems();
            SaveService.Save(data);
        }

        private void OnAbilityCountChanged(AbilityKind abilityKind, int count)
        {
            if (isApplying)
            {
                return;
            }

            switch (abilityKind)
            {
                case AbilityKind.Shovel:
                    data.shovelCount = count;
                    break;
                case AbilityKind.MagicWand:
                    data.magicWandCount = count;
                    break;
                case AbilityKind.SortingMagnet:
                    data.sortingMagnetCount = count;
                    break;
            }

            SaveService.Save(data);
        }

        private void OnObjectiveCompleted()
        {
            if (boardController != null && boardController.LevelDefinition != null)
            {
                int nextLevel = boardController.LevelDefinition.LevelId + 1;
                if (nextLevel > data.highestUnlockedLevel)
                {
                    data.highestUnlockedLevel = nextLevel;
                }
            }

            SaveCurrentState();
        }

        private void CaptureBoardItems()
        {
            if (boardController == null || boardController.BoardState == null)
            {
                return;
            }

            List<BoardItemSaveData> savedItems = new List<BoardItemSaveData>();
            foreach (BoardCell cell in boardController.BoardState.GetCells())
            {
                if (cell == null || cell.Item == null)
                {
                    continue;
                }

                savedItems.Add(new BoardItemSaveData
                {
                    x = cell.Position.X,
                    y = cell.Position.Y,
                    familyId = cell.Item.FamilyId,
                    level = cell.Item.Level,
                    itemId = cell.Item.ItemId
                });
            }

            data.hasBoardState = true;
            data.plantCount = savedItems.Count;
            data.boardItems = savedItems.ToArray();
        }

        private void ApplyBoardItems()
        {
            if (!data.hasBoardState ||
                data.boardItems == null ||
                boardController == null ||
                boardController.BoardState == null)
            {
                return;
            }

            List<BoardItem> items = new List<BoardItem>();
            List<GridPosition> positions = new List<GridPosition>();
            for (int i = 0; i < data.boardItems.Length; i++)
            {
                BoardItemSaveData savedItem = data.boardItems[i];
                if (savedItem == null || string.IsNullOrEmpty(savedItem.familyId) || savedItem.level <= 0)
                {
                    continue;
                }

                string itemId = string.IsNullOrEmpty(savedItem.itemId)
                    ? savedItem.familyId + "_lv" + savedItem.level.ToString("00")
                    : savedItem.itemId;
                items.Add(new BoardItem(savedItem.familyId, savedItem.level, itemId));
                positions.Add(new GridPosition(savedItem.x, savedItem.y));
            }

            boardController.RestoreBoardItems(items, positions);
        }

        private void ResolveReferences()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (economyController == null)
            {
                economyController = FindAnyObjectByType<EconomyController>();
            }
        }
    }
}
