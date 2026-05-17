using UnityEngine;

namespace EcoGarden.Config
{
    [CreateAssetMenu(menuName = "Eco Garden/Items/Item Definition", fileName = "ItemDefinition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string familyId;
        [SerializeField] private int level;
        [SerializeField] private string displayName;
        [SerializeField] private int sellValue;
        [SerializeField] private Sprite sprite;
        [SerializeField] private ItemDefinition nextItem;

        public string ItemId { get { return itemId; } }
        public string FamilyId { get { return familyId; } }
        public int Level { get { return level; } }
        public string DisplayName { get { return displayName; } }
        public int SellValue { get { return sellValue; } }
        public Sprite Sprite { get { return sprite; } }
        public ItemDefinition NextItem { get { return nextItem; } }

#if UNITY_EDITOR
        public void EditorSetValues(string id, string family, int itemLevel, string itemName, int value, ItemDefinition next)
        {
            itemId = id;
            familyId = family;
            level = itemLevel;
            displayName = itemName;
            sellValue = value;
            nextItem = next;
        }
#endif
    }
}
