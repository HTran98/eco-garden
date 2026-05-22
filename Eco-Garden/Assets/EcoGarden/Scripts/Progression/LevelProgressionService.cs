using EcoGarden.Config;
using EcoGarden.Save;
using UnityEngine;

namespace EcoGarden.Progression
{
    public static class LevelProgressionService
    {
        public static bool TryUnlockNextLevel(SaveData data, LevelDefinition completedLevel)
        {
            if (data == null || completedLevel == null || completedLevel.LevelId <= 0)
            {
                return false;
            }

            int nextLevelId = completedLevel.LevelId + 1;
            int currentHighest = Mathf.Max(1, data.highestUnlockedLevel);
            if (nextLevelId <= currentHighest)
            {
                data.highestUnlockedLevel = currentHighest;
                return false;
            }

            data.highestUnlockedLevel = nextLevelId;
            return true;
        }

        public static bool IsLevelUnlocked(SaveData data, LevelDefinition level)
        {
            if (level == null)
            {
                return false;
            }

            int highestUnlockedLevel = data != null ? Mathf.Max(1, data.highestUnlockedLevel) : 1;
            return level.LevelId > 0 && level.LevelId <= highestUnlockedLevel;
        }
    }
}
