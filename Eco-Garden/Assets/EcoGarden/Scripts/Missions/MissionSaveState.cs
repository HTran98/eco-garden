namespace EcoGarden.Missions
{
    public sealed class MissionSaveState
    {
        public MissionSaveState(string missionId, int progress, bool rewardClaimed)
        {
            MissionId = missionId;
            Progress = progress;
            RewardClaimed = rewardClaimed;
        }

        public string MissionId { get; }
        public int Progress { get; }
        public bool RewardClaimed { get; }
    }
}
