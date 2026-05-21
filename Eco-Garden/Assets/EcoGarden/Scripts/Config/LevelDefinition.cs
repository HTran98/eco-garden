using System;
using System.Collections.Generic;
using EcoGarden.Abilities;
using EcoGarden.Items;
using EcoGarden.Progression;
using EcoGarden.Rewards;
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
        [SerializeField] private List<PlantTierUnlockDefinition> temporaryAllowedPlantTiers = new List<PlantTierUnlockDefinition>();
        [SerializeField] private DifficultyDefinition difficulty = DifficultyDefinition.NormalDefault;
        [SerializeField] private List<TemporaryLockDefinition> temporaryLocks = new List<TemporaryLockDefinition>();
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
        public IReadOnlyList<PlantTierUnlockDefinition> TemporaryAllowedPlantTiers { get { return temporaryAllowedPlantTiers; } }
        public DifficultyDefinition Difficulty { get { return difficulty ?? DifficultyDefinition.NormalDefault; } }
        public IReadOnlyList<TemporaryLockDefinition> TemporaryLocks { get { return temporaryLocks; } }
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
            string theme,
            List<PlantTierUnlockDefinition> temporaryUnlocks = null,
            DifficultyDefinition difficultyDefinition = null,
            List<TemporaryLockDefinition> temporaryLockDefinitions = null)
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
            temporaryAllowedPlantTiers = temporaryUnlocks ?? new List<PlantTierUnlockDefinition>();
            difficulty = difficultyDefinition ?? DifficultyDefinition.NormalDefault;
            temporaryLocks = temporaryLockDefinitions ?? new List<TemporaryLockDefinition>();
        }
#endif
    }

    [Serializable]
    public sealed class DifficultyDefinition
    {
        [SerializeField] private DifficultyKind difficultyKind;
        [SerializeField] private int obstacleCount;
        [SerializeField] private int lockedCellCount;
        [SerializeField] private int temporaryLockCount;
        [SerializeField] private int orderComplexityScore;
        [SerializeField] private float timerPressureMultiplier = 1f;
        [SerializeField] private float rewardMultiplier = 1f;
        [SerializeField] private string notes;

        public DifficultyKind DifficultyKind { get { return difficultyKind; } }
        public int ObstacleCount { get { return obstacleCount; } }
        public int LockedCellCount { get { return lockedCellCount; } }
        public int TemporaryLockCount { get { return temporaryLockCount; } }
        public int OrderComplexityScore { get { return orderComplexityScore; } }
        public float TimerPressureMultiplier { get { return timerPressureMultiplier; } }
        public float RewardMultiplier { get { return rewardMultiplier; } }
        public string Notes { get { return notes; } }

        public static DifficultyDefinition NormalDefault
        {
            get
            {
                return new DifficultyDefinition(
                    DifficultyKind.Normal,
                    0,
                    0,
                    0,
                    0,
                    1f,
                    1f,
                    string.Empty);
            }
        }

        public DifficultyDefinition()
        {
        }

        public DifficultyDefinition(
            DifficultyKind difficultyKind,
            int obstacleCount,
            int lockedCellCount,
            int temporaryLockCount,
            int orderComplexityScore,
            float timerPressureMultiplier,
            float rewardMultiplier,
            string notes = "")
        {
            this.difficultyKind = difficultyKind;
            this.obstacleCount = Mathf.Max(0, obstacleCount);
            this.lockedCellCount = Mathf.Max(0, lockedCellCount);
            this.temporaryLockCount = Mathf.Max(0, temporaryLockCount);
            this.orderComplexityScore = Mathf.Max(0, orderComplexityScore);
            this.timerPressureMultiplier = Mathf.Max(0f, timerPressureMultiplier);
            this.rewardMultiplier = Mathf.Max(0f, rewardMultiplier);
            this.notes = notes ?? string.Empty;
        }
    }

    [Serializable]
    public sealed class TemporaryLockDefinition
    {
        [SerializeField] private int x;
        [SerializeField] private int y;
        [SerializeField] private TemporaryLockUnlockTrigger unlockTrigger;
        [SerializeField] private string unlockSourceId;

        public int X { get { return x; } }
        public int Y { get { return y; } }
        public TemporaryLockUnlockTrigger UnlockTrigger { get { return unlockTrigger; } }
        public string UnlockSourceId { get { return unlockSourceId; } }

        public TemporaryLockDefinition()
        {
        }

        public TemporaryLockDefinition(
            int x,
            int y,
            TemporaryLockUnlockTrigger unlockTrigger,
            string unlockSourceId = "")
        {
            this.x = x;
            this.y = y;
            this.unlockTrigger = unlockTrigger;
            this.unlockSourceId = unlockSourceId ?? string.Empty;
        }
    }

    public enum TemporaryLockUnlockTrigger
    {
        None,
        OrderCompleted,
        TimerElapsed,
        Purchase
    }

    [Serializable]
    public sealed class NpcOrderDefinition
    {
        [SerializeField] private string orderId;
        [SerializeField] private string displayName;
        [SerializeField] private OrderRequirementDefinition[] requirements;
        [SerializeField] private RewardDefinition reward;
        [SerializeField] private string familyId;
        [SerializeField] private int level;
        [SerializeField] private int quantity;

        public string OrderId { get { return orderId; } }
        public string DisplayName { get { return displayName; } }
        public IReadOnlyList<OrderRequirementDefinition> Requirements
        {
            get
            {
                if (requirements != null && requirements.Length > 0)
                {
                    return requirements;
                }

                return LegacyRequirementCache.Get(familyId, level, quantity);
            }
        }

        public RewardDefinition Reward { get { return reward; } }
        public string FamilyId { get { return familyId; } }
        public int Level { get { return level; } }
        public int Quantity { get { return quantity; } }
        public int RequirementCount
        {
            get
            {
                int count = 0;
                IReadOnlyList<OrderRequirementDefinition> activeRequirements = Requirements;
                for (int i = 0; i < activeRequirements.Count; i++)
                {
                    if (activeRequirements[i] != null)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public int TotalRequiredItems
        {
            get
            {
                int total = 0;
                IReadOnlyList<OrderRequirementDefinition> activeRequirements = Requirements;
                for (int i = 0; i < activeRequirements.Count; i++)
                {
                    OrderRequirementDefinition requirement = activeRequirements[i];
                    if (requirement != null)
                    {
                        total += Mathf.Max(0, requirement.Quantity);
                    }
                }

                return total;
            }
        }

        public int HighestRequiredLevel
        {
            get
            {
                int highestLevel = 0;
                IReadOnlyList<OrderRequirementDefinition> activeRequirements = Requirements;
                for (int i = 0; i < activeRequirements.Count; i++)
                {
                    OrderRequirementDefinition requirement = activeRequirements[i];
                    if (requirement != null && requirement.Level > highestLevel)
                    {
                        highestLevel = requirement.Level;
                    }
                }

                return highestLevel;
            }
        }

        public int ComplexityScore
        {
            get
            {
                int score = 0;
                IReadOnlyList<OrderRequirementDefinition> activeRequirements = Requirements;
                for (int i = 0; i < activeRequirements.Count; i++)
                {
                    OrderRequirementDefinition requirement = activeRequirements[i];
                    if (requirement != null)
                    {
                        score += Mathf.Max(0, requirement.Level) * Mathf.Max(0, requirement.Quantity);
                    }
                }

                return score;
            }
        }

        public NpcOrderDefinition()
        {
        }

        public NpcOrderDefinition(string familyId, int level, int quantity)
        {
            orderId = familyId + "_lv" + level.ToString("00") + "_x" + quantity;
            displayName = string.Empty;
            this.familyId = familyId;
            this.level = level;
            this.quantity = quantity;
            requirements = new[] { new OrderRequirementDefinition(familyId, level, quantity) };
        }

        public NpcOrderDefinition(
            string orderId,
            string displayName,
            OrderRequirementDefinition[] requirements,
            RewardDefinition reward = null)
        {
            this.orderId = orderId;
            this.displayName = displayName;
            this.requirements = requirements;
            this.reward = reward;

            if (requirements != null && requirements.Length > 0 && requirements[0] != null)
            {
                familyId = requirements[0].FamilyId;
                level = requirements[0].Level;
                quantity = requirements[0].Quantity;
            }
        }

        public bool Matches(BoardItem item)
        {
            if (item == null)
            {
                return false;
            }

            IReadOnlyList<OrderRequirementDefinition> activeRequirements = Requirements;
            for (int i = 0; i < activeRequirements.Count; i++)
            {
                OrderRequirementDefinition requirement = activeRequirements[i];
                if (requirement != null && requirement.Matches(item))
                {
                    return true;
                }
            }

            return false;
        }

        private static class LegacyRequirementCache
        {
            private static readonly List<OrderRequirementDefinition> Cache = new List<OrderRequirementDefinition>(1);

            public static IReadOnlyList<OrderRequirementDefinition> Get(string familyId, int level, int quantity)
            {
                Cache.Clear();
                Cache.Add(new OrderRequirementDefinition(familyId, level, quantity));
                return Cache;
            }
        }
    }

    [Serializable]
    public sealed class OrderRequirementDefinition
    {
        [SerializeField] private string familyId;
        [SerializeField] private int level;
        [SerializeField] private int quantity;

        public string FamilyId { get { return familyId; } }
        public int Level { get { return level; } }
        public int Quantity { get { return quantity; } }

        public OrderRequirementDefinition()
        {
        }

        public OrderRequirementDefinition(string familyId, int level, int quantity)
        {
            this.familyId = familyId;
            this.level = level;
            this.quantity = quantity;
        }

        public bool Matches(BoardItem item)
        {
            return item != null &&
                   item.FamilyId == familyId &&
                   item.Level == level;
        }
    }

    [Serializable]
    public sealed class OrderRequirementRuntimeState
    {
        [SerializeField] private string familyId;
        [SerializeField] private int level;
        [SerializeField] private int requiredCount;
        [SerializeField] private int submittedCount;

        public string FamilyId { get { return familyId; } }
        public int Level { get { return level; } }
        public int RequiredCount { get { return requiredCount; } }
        public int SubmittedCount { get { return submittedCount; } }
        public bool IsComplete { get { return submittedCount >= requiredCount; } }

        public OrderRequirementRuntimeState()
        {
        }

        public OrderRequirementRuntimeState(OrderRequirementDefinition definition)
        {
            if (definition == null)
            {
                return;
            }

            familyId = definition.FamilyId;
            level = definition.Level;
            requiredCount = Mathf.Max(0, definition.Quantity);
            submittedCount = 0;
        }

        public bool TrySubmit(BoardItem item)
        {
            if (IsComplete ||
                item == null ||
                item.FamilyId != familyId ||
                item.Level != level)
            {
                return false;
            }

            submittedCount++;
            return true;
        }

        public void SetSubmittedCount(int count)
        {
            submittedCount = Mathf.Clamp(count, 0, requiredCount);
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
