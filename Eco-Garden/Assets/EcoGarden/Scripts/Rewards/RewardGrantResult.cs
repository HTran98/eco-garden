namespace EcoGarden.Rewards
{
    public readonly struct RewardGrantResult
    {
        public RewardGrantResult(
            int currencyGrantCount,
            int abilityGrantCount,
            int decorationGrantCount,
            int plantTierUnlockGrantCount)
        {
            CurrencyGrantCount = currencyGrantCount;
            AbilityGrantCount = abilityGrantCount;
            DecorationGrantCount = decorationGrantCount;
            PlantTierUnlockGrantCount = plantTierUnlockGrantCount;
        }

        public int CurrencyGrantCount { get; }
        public int AbilityGrantCount { get; }
        public int DecorationGrantCount { get; }
        public int PlantTierUnlockGrantCount { get; }

        public bool HasAnyGrant
        {
            get
            {
                return CurrencyGrantCount > 0 ||
                       AbilityGrantCount > 0 ||
                       DecorationGrantCount > 0 ||
                       PlantTierUnlockGrantCount > 0;
            }
        }
    }
}
