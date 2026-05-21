using UnityEngine;

namespace EcoGarden.Missions
{
    public sealed class MissionRuntimeState
    {
        public MissionRuntimeState(MissionDefinition definition)
        {
            Definition = definition;
        }

        public MissionDefinition Definition { get; }
        public string MissionId { get { return Definition != null ? Definition.MissionId : string.Empty; } }
        public int Progress { get; private set; }
        public bool RewardClaimed { get; private set; }
        public int RequiredCount { get { return Definition != null ? Definition.RequiredCount : 0; } }
        public bool IsComplete { get { return RequiredCount > 0 && Progress >= RequiredCount; } }
        public bool CanClaim { get { return IsComplete && !RewardClaimed; } }

        public void SetProgress(int progress)
        {
            Progress = Mathf.Clamp(progress, 0, RequiredCount);
        }

        public void AddProgress(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            SetProgress(Progress + amount);
        }

        public void SetRewardClaimed(bool claimed)
        {
            RewardClaimed = claimed;
        }
    }
}
