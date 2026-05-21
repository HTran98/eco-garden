using EcoGarden.Rewards;

namespace EcoGarden.Missions
{
    public readonly struct MissionClaimResult
    {
        public MissionClaimResult(MissionClaimStatus status, RewardGrantResult rewardResult)
        {
            Status = status;
            RewardResult = rewardResult;
        }

        public MissionClaimStatus Status { get; }
        public RewardGrantResult RewardResult { get; }
        public bool Succeeded { get { return Status == MissionClaimStatus.Claimed; } }
    }
}
