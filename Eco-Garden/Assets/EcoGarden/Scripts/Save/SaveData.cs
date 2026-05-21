using System;

namespace EcoGarden.Save
{
    [Serializable]
    public sealed class SaveData
    {
        public int schemaVersion;
        public int gold;
        public int gem;
        public int highestUnlockedLevel;
        public int shovelCount = -1;
        public int magicWandCount = -1;
        public int sortingMagnetCount = -1;
        public bool hasBoardState;
        public int plantCount;
        public BoardItemSaveData[] boardItems;
        public string activeOrderId;
        public OrderRequirementSaveData[] orderRequirements;
        public PlantTierUnlockSaveData[] plantTierUnlocks;
        public string[] purchasedShopProductIds;
        public string[] ownedDecorationIds;
        public string[] processedIapTransactionIds;
        public MissionProgressSaveData[] missionProgress;
        public bool soundEnabled = true;
        public bool musicEnabled = true;
    }

    [Serializable]
    public sealed class BoardItemSaveData
    {
        public int x;
        public int y;
        public string familyId;
        public int level;
        public string itemId;
    }

    [Serializable]
    public sealed class OrderRequirementSaveData
    {
        public string familyId;
        public int level;
        public int requiredCount;
        public int submittedCount;
    }

    [Serializable]
    public sealed class PlantTierUnlockSaveData
    {
        public string familyId;
        public int tier;
    }

    [Serializable]
    public sealed class MissionProgressSaveData
    {
        public string missionId;
        public int progress;
        public bool rewardClaimed;
    }
}
