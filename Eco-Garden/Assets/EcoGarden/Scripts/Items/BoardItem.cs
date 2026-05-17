namespace EcoGarden.Items
{
    public sealed class BoardItem
    {
        public string FamilyId { get; private set; }
        public int Level { get; private set; }
        public string ItemId { get; private set; }

        public BoardItem(string familyId, int level, string itemId)
        {
            FamilyId = familyId;
            Level = level;
            ItemId = itemId;
        }

        public bool CanMergeWith(BoardItem other, int maxLevel)
        {
            return other != null &&
                   FamilyId == other.FamilyId &&
                   Level == other.Level &&
                   Level < maxLevel;
        }

        public BoardItem CreateUpgraded(string upgradedItemId)
        {
            return new BoardItem(FamilyId, Level + 1, upgradedItemId);
        }
    }
}
