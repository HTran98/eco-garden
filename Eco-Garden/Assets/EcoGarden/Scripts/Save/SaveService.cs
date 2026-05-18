using UnityEngine;

namespace EcoGarden.Save
{
    public static class SaveService
    {
        private const string SaveKey = "EcoGarden.SaveData.v1";

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
                return data ?? CreateDefault();
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
            return new SaveData
            {
                highestUnlockedLevel = 1,
                shovelCount = -1,
                magicWandCount = -1,
                sortingMagnetCount = -1,
                soundEnabled = true,
                musicEnabled = true
            };
        }
    }
}
