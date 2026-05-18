using System;

namespace EcoGarden.Save
{
    [Serializable]
    public sealed class SaveData
    {
        public int gold;
        public int highestUnlockedLevel;
        public int shovelCount = -1;
        public int magicWandCount = -1;
        public int sortingMagnetCount = -1;
        public bool hasBoardState;
        public int plantCount;
        public BoardItemSaveData[] boardItems;
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
}
