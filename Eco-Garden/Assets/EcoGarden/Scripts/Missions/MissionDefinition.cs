using EcoGarden.Abilities;
using EcoGarden.Config;
using EcoGarden.Rewards;
using UnityEngine;

namespace EcoGarden.Missions
{
    [CreateAssetMenu(menuName = "Eco Garden/Missions/Mission Definition", fileName = "MissionDefinition")]
    public sealed class MissionDefinition : ScriptableObject
    {
        [SerializeField] private string missionId;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private MissionType missionType;
        [SerializeField] private DifficultyKind difficulty;
        [SerializeField] private string targetFamilyId;
        [SerializeField] private int targetItemLevel;
        [SerializeField] private AbilityKind targetAbility;
        [SerializeField] private int requiredCount = 1;
        [SerializeField] private RewardDefinition reward;
        [SerializeField] private bool isDaily;
        [SerializeField] private int sortOrder;

        public string MissionId { get { return missionId; } }
        public string DisplayName { get { return displayName; } }
        public string Description { get { return description; } }
        public MissionType MissionType { get { return missionType; } }
        public DifficultyKind Difficulty { get { return difficulty; } }
        public string TargetFamilyId { get { return targetFamilyId; } }
        public int TargetItemLevel { get { return targetItemLevel; } }
        public AbilityKind TargetAbility { get { return targetAbility; } }
        public int RequiredCount { get { return requiredCount; } }
        public RewardDefinition Reward { get { return reward; } }
        public bool IsDaily { get { return isDaily; } }
        public int SortOrder { get { return sortOrder; } }
        public bool IsValid { get { return !string.IsNullOrWhiteSpace(missionId) && requiredCount > 0 && reward != null; } }

#if UNITY_EDITOR
        public void EditorSetValues(
            string missionId,
            string displayName,
            string description,
            MissionType missionType,
            DifficultyKind difficulty,
            string targetFamilyId,
            int targetItemLevel,
            AbilityKind targetAbility,
            int requiredCount,
            RewardDefinition reward,
            bool isDaily,
            int sortOrder)
        {
            this.missionId = missionId;
            this.displayName = displayName;
            this.description = description;
            this.missionType = missionType;
            this.difficulty = difficulty;
            this.targetFamilyId = targetFamilyId;
            this.targetItemLevel = Mathf.Max(0, targetItemLevel);
            this.targetAbility = targetAbility;
            this.requiredCount = Mathf.Max(1, requiredCount);
            this.reward = reward;
            this.isDaily = isDaily;
            this.sortOrder = sortOrder;
        }
#endif
    }
}
