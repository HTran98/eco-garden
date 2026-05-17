using System;
using System.Collections.Generic;
using EcoGarden.Abilities;
using UnityEngine;

namespace EcoGarden.Config
{
    [CreateAssetMenu(menuName = "Eco Garden/Levels/Level Definition", fileName = "LevelDefinition")]
    public sealed class LevelDefinition : ScriptableObject
    {
        [SerializeField] private int levelId;
        [SerializeField] private string levelName;
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;
        [SerializeField] private string[] rowsTopToBottom;
        [SerializeField] private ProducerDefinition defaultProducer;
        [SerializeField] private List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();
        [SerializeField] private NpcOrderDefinition npcOrder;
        [SerializeField] private List<AbilityCountDefinition> startingAbilities = new List<AbilityCountDefinition>();
        [SerializeField] private float timerSeconds = 180f;
        [SerializeField] private string themeId;

        public int LevelId { get { return levelId; } }
        public string LevelName { get { return levelName; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public IReadOnlyList<string> RowsTopToBottom { get { return rowsTopToBottom; } }
        public ProducerDefinition DefaultProducer { get { return defaultProducer; } }
        public IReadOnlyList<ItemDefinition> ItemDefinitions { get { return itemDefinitions; } }
        public NpcOrderDefinition NpcOrder { get { return npcOrder; } }
        public IReadOnlyList<AbilityCountDefinition> StartingAbilities { get { return startingAbilities; } }
        public float TimerSeconds { get { return timerSeconds; } }
        public string ThemeId { get { return themeId; } }

        public ItemDefinition GetItemDefinitionForLevel(int level)
        {
            for (int i = 0; i < itemDefinitions.Count; i++)
            {
                if (itemDefinitions[i] != null && itemDefinitions[i].Level == level)
                {
                    return itemDefinitions[i];
                }
            }

            return null;
        }

#if UNITY_EDITOR
        public void EditorSetValues(
            int id,
            string name,
            int boardWidth,
            int boardHeight,
            string[] rows,
            ProducerDefinition producer,
            List<ItemDefinition> items,
            NpcOrderDefinition order,
            List<AbilityCountDefinition> abilities,
            float timer,
            string theme)
        {
            levelId = id;
            levelName = name;
            width = boardWidth;
            height = boardHeight;
            rowsTopToBottom = rows;
            defaultProducer = producer;
            itemDefinitions = items;
            npcOrder = order;
            startingAbilities = abilities;
            timerSeconds = timer;
            themeId = theme;
        }
#endif
    }

    [Serializable]
    public sealed class NpcOrderDefinition
    {
        [SerializeField] private string familyId;
        [SerializeField] private int level;
        [SerializeField] private int quantity;

        public string FamilyId { get { return familyId; } }
        public int Level { get { return level; } }
        public int Quantity { get { return quantity; } }

        public NpcOrderDefinition()
        {
        }

        public NpcOrderDefinition(string familyId, int level, int quantity)
        {
            this.familyId = familyId;
            this.level = level;
            this.quantity = quantity;
        }
    }

    [Serializable]
    public sealed class AbilityCountDefinition
    {
        [SerializeField] private AbilityKind abilityKind;
        [SerializeField] private int count;

        public AbilityKind AbilityKind { get { return abilityKind; } }
        public int Count { get { return count; } }

        public AbilityCountDefinition()
        {
        }

        public AbilityCountDefinition(AbilityKind abilityKind, int count)
        {
            this.abilityKind = abilityKind;
            this.count = count;
        }
    }
}
