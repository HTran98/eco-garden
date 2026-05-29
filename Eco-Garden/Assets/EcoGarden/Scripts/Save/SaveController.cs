using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Economy;
using EcoGarden.Items;
using EcoGarden.Missions;
using EcoGarden.Progression;
using EcoGarden.Shop;
using System.Collections.Generic;
using UnityEngine;

namespace EcoGarden.Save
{
    public sealed class SaveController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private EconomyController economyController;
        [SerializeField] private ShopController shopController;
        [SerializeField] private MissionController missionController;

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
                economyController.SetGem(data.gem);
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

            ApplyClearedObstacles();
            ApplyBoardItems();
            ApplyPlantTierUnlocks();
            ApplyShopInventory();
            ApplyMissionProgress();
            ApplyOrderProgress();

            isApplying = false;
        }

        private void CaptureCurrentState()
        {
            ResolveReferences();

            if (economyController != null)
            {
                data.gold = economyController.Gold;
                data.gem = economyController.Gem;
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
            CaptureClearedObstacles();
            CapturePlantTierUnlocks();
            CaptureShopInventory();
            CaptureMissionProgress();
            CaptureOrderProgress();
        }

        private void UnlockNextLevelForCompletedOrder()
        {
            if (data == null || boardController == null || boardController.LevelDefinition == null)
            {
                return;
            }

            LevelProgressionService.TryUnlockNextLevel(data, boardController.LevelDefinition);
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
                economyController.GemChanged += OnGemChanged;
            }

            if (boardController != null)
            {
                boardController.BoardChanged += OnBoardChanged;
                boardController.OrderProgressChanged += OnOrderProgressChanged;
                boardController.OrderCompleted += OnOrderCompleted;

                if (boardController.AbilityInventory != null)
                {
                    boardController.AbilityInventory.CountChanged += OnAbilityCountChanged;
                }

                if (boardController.PlantUnlockService != null)
                {
                    boardController.PlantUnlockService.Changed += OnPlantUnlockChanged;
                }
            }

            if (shopController != null && shopController.Inventory != null)
            {
                shopController.Inventory.Changed += OnShopInventoryChanged;
                shopController.ProcessedIapTransactionsChanged += OnProcessedIapTransactionsChanged;
            }

            if (missionController != null)
            {
                missionController.MissionsChanged += OnMissionsChanged;
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
                economyController.GemChanged -= OnGemChanged;
            }

            if (boardController != null)
            {
                boardController.BoardChanged -= OnBoardChanged;
                boardController.OrderProgressChanged -= OnOrderProgressChanged;
                boardController.OrderCompleted -= OnOrderCompleted;

                if (boardController.AbilityInventory != null)
                {
                    boardController.AbilityInventory.CountChanged -= OnAbilityCountChanged;
                }

                if (boardController.PlantUnlockService != null)
                {
                    boardController.PlantUnlockService.Changed -= OnPlantUnlockChanged;
                }
            }

            if (shopController != null && shopController.Inventory != null)
            {
                shopController.Inventory.Changed -= OnShopInventoryChanged;
                shopController.ProcessedIapTransactionsChanged -= OnProcessedIapTransactionsChanged;
            }

            if (missionController != null)
            {
                missionController.MissionsChanged -= OnMissionsChanged;
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
            CaptureShopInventory();
            SaveService.Save(data);
        }

        private void OnGemChanged(int gem)
        {
            if (isApplying)
            {
                return;
            }

            data.gem = gem;
            CaptureShopInventory();
            SaveService.Save(data);
        }

        private void OnBoardChanged()
        {
            if (isApplying)
            {
                return;
            }

            CaptureBoardItems();
            CaptureClearedObstacles();
            CaptureOrderProgress();
            SaveService.Save(data);
        }

        private void OnOrderProgressChanged()
        {
            if (isApplying)
            {
                return;
            }

            CaptureOrderProgress();
            SaveService.Save(data);
        }

        private void OnOrderCompleted()
        {
            if (isApplying)
            {
                return;
            }

            UnlockNextLevelForCompletedOrder();
            SaveCurrentState();
        }

        private void OnPlantUnlockChanged()
        {
            if (isApplying)
            {
                return;
            }

            CapturePlantTierUnlocks();
            SaveService.Save(data);
        }

        private void OnShopInventoryChanged()
        {
            if (isApplying)
            {
                return;
            }

            CaptureShopInventory();
            SaveService.Save(data);
        }

        private void OnProcessedIapTransactionsChanged()
        {
            if (isApplying)
            {
                return;
            }

            CaptureShopInventory();
            SaveService.Save(data);
        }

        private void OnMissionsChanged()
        {
            if (isApplying)
            {
                return;
            }

            CaptureMissionProgress();
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

        private void CaptureClearedObstacles()
        {
            if (boardController == null || boardController.BoardState == null || boardController.LevelDefinition == null)
            {
                return;
            }

            List<GridPosition> clearedPositions = boardController.CaptureClearedObstaclePositions();
            List<ClearedObstacleSaveData> savedObstacles = new List<ClearedObstacleSaveData>();
            int levelId = boardController.LevelDefinition.LevelId;
            if (data.clearedObstacles != null)
            {
                for (int i = 0; i < data.clearedObstacles.Length; i++)
                {
                    ClearedObstacleSaveData obstacle = data.clearedObstacles[i];
                    if (obstacle != null && obstacle.levelId != levelId)
                    {
                        savedObstacles.Add(obstacle);
                    }
                }
            }

            for (int i = 0; i < clearedPositions.Count; i++)
            {
                GridPosition position = clearedPositions[i];
                savedObstacles.Add(new ClearedObstacleSaveData
                {
                    levelId = levelId,
                    x = position.X,
                    y = position.Y
                });
            }

            data.clearedObstacles = savedObstacles.ToArray();
        }

        private void CaptureOrderProgress()
        {
            if (boardController == null || boardController.ActiveOrderRequirements == null)
            {
                return;
            }

            data.activeOrderId = boardController.ActiveOrderId;
            List<OrderRequirementSaveData> savedRequirements = new List<OrderRequirementSaveData>();
            var requirements = boardController.ActiveOrderRequirements;
            for (int i = 0; i < requirements.Count; i++)
            {
                var requirement = requirements[i];
                if (requirement == null)
                {
                    continue;
                }

                savedRequirements.Add(new OrderRequirementSaveData
                {
                    familyId = requirement.FamilyId,
                    level = requirement.Level,
                    requiredCount = requirement.RequiredCount,
                    submittedCount = requirement.SubmittedCount
                });
            }

            data.orderRequirements = savedRequirements.ToArray();
        }

        private void CapturePlantTierUnlocks()
        {
            if (boardController == null || boardController.PlantUnlockService == null)
            {
                return;
            }

            PlantTierUnlockDefinition[] unlocks = boardController.PlantUnlockService.GetSavedUnlocks();
            List<PlantTierUnlockSaveData> savedUnlocks = new List<PlantTierUnlockSaveData>();
            for (int i = 0; i < unlocks.Length; i++)
            {
                PlantTierUnlockDefinition unlock = unlocks[i];
                if (unlock == null || string.IsNullOrEmpty(unlock.FamilyId))
                {
                    continue;
                }

                savedUnlocks.Add(new PlantTierUnlockSaveData
                {
                    familyId = unlock.FamilyId,
                    tier = unlock.Tier
                });
            }

            data.plantTierUnlocks = savedUnlocks.ToArray();
        }

        private void CaptureShopInventory()
        {
            if (shopController == null || shopController.Inventory == null)
            {
                return;
            }

            data.purchasedShopProductIds = shopController.Inventory.GetPurchasedProductIds();
            data.ownedDecorationIds = shopController.Inventory.GetOwnedDecorationIds();
            data.processedIapTransactionIds = shopController.GetProcessedIapTransactionIds();
        }

        private void CaptureMissionProgress()
        {
            if (missionController == null)
            {
                return;
            }

            MissionSaveState[] missionStates = missionController.CaptureMissionStates();
            List<MissionProgressSaveData> savedMissions = new List<MissionProgressSaveData>();
            for (int i = 0; i < missionStates.Length; i++)
            {
                MissionSaveState state = missionStates[i];
                if (state == null || string.IsNullOrEmpty(state.MissionId))
                {
                    continue;
                }

                savedMissions.Add(new MissionProgressSaveData
                {
                    missionId = state.MissionId,
                    progress = state.Progress,
                    rewardClaimed = state.RewardClaimed
                });
            }

            data.missionProgress = savedMissions.ToArray();
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

        private void ApplyClearedObstacles()
        {
            if (data.clearedObstacles == null ||
                boardController == null ||
                boardController.BoardState == null ||
                boardController.LevelDefinition == null)
            {
                return;
            }

            List<GridPosition> positions = new List<GridPosition>();
            int levelId = boardController.LevelDefinition.LevelId;
            for (int i = 0; i < data.clearedObstacles.Length; i++)
            {
                ClearedObstacleSaveData obstacle = data.clearedObstacles[i];
                if (obstacle == null || obstacle.levelId != levelId)
                {
                    continue;
                }

                positions.Add(new GridPosition(obstacle.x, obstacle.y));
            }

            boardController.RestoreClearedObstacles(positions);
        }

        private void ApplyOrderProgress()
        {
            if (data.orderRequirements == null ||
                boardController == null ||
                data.activeOrderId != boardController.ActiveOrderId)
            {
                return;
            }

            for (int i = 0; i < data.orderRequirements.Length; i++)
            {
                OrderRequirementSaveData requirement = data.orderRequirements[i];
                if (requirement == null || string.IsNullOrEmpty(requirement.familyId))
                {
                    continue;
                }

                boardController.SetOrderSubmittedCount(requirement.familyId, requirement.level, requirement.submittedCount);
            }

            if (boardController.IsActiveOrderCompleteForSave())
            {
                boardController.StartNextOrder();
            }
        }

        private void ApplyPlantTierUnlocks()
        {
            if (data.plantTierUnlocks == null ||
                boardController == null ||
                boardController.PlantUnlockService == null)
            {
                return;
            }

            List<PlantTierUnlockDefinition> unlocks = new List<PlantTierUnlockDefinition>();
            for (int i = 0; i < data.plantTierUnlocks.Length; i++)
            {
                PlantTierUnlockSaveData unlock = data.plantTierUnlocks[i];
                if (unlock == null || string.IsNullOrEmpty(unlock.familyId))
                {
                    continue;
                }

                unlocks.Add(new PlantTierUnlockDefinition(unlock.familyId, unlock.tier));
            }

            boardController.PlantUnlockService.RestoreUnlockedTiers(unlocks);
            boardController.RebuildOrderState();
        }

        private void ApplyShopInventory()
        {
            if (shopController == null || shopController.Inventory == null)
            {
                return;
            }

            shopController.RestoreInventory(data.purchasedShopProductIds, data.ownedDecorationIds);
            shopController.RestoreProcessedIapTransactionIds(data.processedIapTransactionIds);
        }

        private void ApplyMissionProgress()
        {
            if (missionController == null || data.missionProgress == null)
            {
                return;
            }

            List<MissionSaveState> missionStates = new List<MissionSaveState>();
            for (int i = 0; i < data.missionProgress.Length; i++)
            {
                MissionProgressSaveData savedMission = data.missionProgress[i];
                if (savedMission == null || string.IsNullOrEmpty(savedMission.missionId))
                {
                    continue;
                }

                missionStates.Add(new MissionSaveState(
                    savedMission.missionId,
                    savedMission.progress,
                    savedMission.rewardClaimed));
            }

            missionController.RestoreMissionStates(missionStates);
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

            if (shopController == null)
            {
                shopController = FindAnyObjectByType<ShopController>();
            }

            if (missionController == null)
            {
                missionController = FindAnyObjectByType<MissionController>();
            }
        }
    }
}
