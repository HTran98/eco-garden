using UnityEngine;

namespace EcoGarden.Save
{
    public static class SaveService
    {
        private const string SaveKey = "EcoGarden.SaveData.v1";
        public const int CurrentSchemaVersion = 4;

        public static SaveData Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                return CreateDefault();
            }

            string json = PlayerPrefs.GetString(SaveKey);
            if (string.IsNullOrEmpty(json))
            {
                return CreateDefault();
            }

            try
            {
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                return Normalize(data);
            }
            catch
            {
                return CreateDefault();
            }
        }

        public static void Save(SaveData data)
        {
            if (data == null)
            {
                return;
            }

            PreserveCompletedTutorialFlag(data);
            data = Normalize(data);
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        public static void Clear()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }

        private static SaveData CreateDefault()
        {
            return Normalize(new SaveData
            {
                schemaVersion = CurrentSchemaVersion,
                highestUnlockedLevel = 1,
                shovelCount = -1,
                magicWandCount = -1,
                sortingMagnetCount = -1,
                tutorialCompleted = false,
                soundEnabled = true,
                musicEnabled = true
            });
        }

        public static SaveData Normalize(SaveData data)
        {
            if (data == null)
            {
                data = new SaveData();
            }

            int loadedVersion = data.schemaVersion;
            if (loadedVersion <= 0)
            {
                data.soundEnabled = true;
                data.musicEnabled = true;
            }

            if (loadedVersion > 0 && loadedVersion < 4 && HasMeaningfulProgress(data))
            {
                data.tutorialCompleted = true;
            }

            data.schemaVersion = CurrentSchemaVersion;
            if (data.highestUnlockedLevel <= 0)
            {
                data.highestUnlockedLevel = 1;
            }

            data.boardItems = data.boardItems ?? new BoardItemSaveData[0];
            data.clearedObstacles = data.clearedObstacles ?? new ClearedObstacleSaveData[0];
            data.orderRequirements = data.orderRequirements ?? new OrderRequirementSaveData[0];
            data.plantTierUnlocks = data.plantTierUnlocks ?? new PlantTierUnlockSaveData[0];
            data.purchasedShopProductIds = data.purchasedShopProductIds ?? new string[0];
            data.ownedDecorationIds = data.ownedDecorationIds ?? new string[0];
            data.processedIapTransactionIds = data.processedIapTransactionIds ?? new string[0];
            data.missionProgress = data.missionProgress ?? new MissionProgressSaveData[0];

            return data;
        }

        private static bool HasMeaningfulProgress(SaveData data)
        {
            return data.highestUnlockedLevel > 1 ||
                   data.gold > 0 ||
                   data.gem > 0 ||
                   data.hasBoardState ||
                   data.plantCount > 0 ||
                   (data.boardItems != null && data.boardItems.Length > 0) ||
                   (data.orderRequirements != null && data.orderRequirements.Length > 0) ||
                   (data.plantTierUnlocks != null && data.plantTierUnlocks.Length > 0) ||
                   (data.purchasedShopProductIds != null && data.purchasedShopProductIds.Length > 0) ||
                   (data.ownedDecorationIds != null && data.ownedDecorationIds.Length > 0) ||
                   (data.activeDecorationIds != null && data.activeDecorationIds.Length > 0) ||
                   (data.missionProgress != null && data.missionProgress.Length > 0);
        }

        private static void PreserveCompletedTutorialFlag(SaveData data)
        {
            if (data.tutorialCompleted || !PlayerPrefs.HasKey(SaveKey))
            {
                return;
            }

            string json = PlayerPrefs.GetString(SaveKey);
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            try
            {
                SaveData existing = JsonUtility.FromJson<SaveData>(json);
                if (existing != null && existing.tutorialCompleted)
                {
                    data.tutorialCompleted = true;
                }
            }
            catch
            {
                // Ignore corrupt existing data; Normalize handles the snapshot being saved.
            }
        }
    }
}
