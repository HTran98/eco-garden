using EcoGarden.Abilities;
using EcoGarden.Economy;
using EcoGarden.Progression;

namespace EcoGarden.Rewards
{
    public static class RewardService
    {
        public static RewardGrantResult Grant(
            RewardDefinition reward,
            EconomyController economyController,
            AbilityInventory abilityInventory,
            PlantUnlockService plantUnlockService = null)
        {
            if (reward == null)
            {
                return default;
            }

            int currencyGrantCount = GrantCurrencies(reward.Currencies, economyController);
            int abilityGrantCount = GrantAbilities(reward.Abilities, abilityInventory);
            int decorationGrantCount = CountValidDecorationRewards(reward.DecorationIds);
            int plantTierUnlockGrantCount = GrantPlantTierUnlocks(reward.PlantTierUnlocks, plantUnlockService);

            return new RewardGrantResult(
                currencyGrantCount,
                abilityGrantCount,
                decorationGrantCount,
                plantTierUnlockGrantCount);
        }

        private static int GrantCurrencies(CurrencyReward[] currencies, EconomyController economyController)
        {
            if (currencies == null || economyController == null)
            {
                return 0;
            }

            int grantCount = 0;
            for (int i = 0; i < currencies.Length; i++)
            {
                CurrencyReward reward = currencies[i];
                if (reward == null || reward.Amount <= 0)
                {
                    continue;
                }

                economyController.AddCurrency(reward.CurrencyKind, reward.Amount);
                grantCount++;
            }

            return grantCount;
        }

        private static int GrantAbilities(AbilityReward[] abilities, AbilityInventory abilityInventory)
        {
            if (abilities == null || abilityInventory == null)
            {
                return 0;
            }

            int grantCount = 0;
            for (int i = 0; i < abilities.Length; i++)
            {
                AbilityReward reward = abilities[i];
                if (reward == null || reward.Count <= 0)
                {
                    continue;
                }

                int currentCount = abilityInventory.GetCount(reward.AbilityKind);
                abilityInventory.SetCount(reward.AbilityKind, currentCount + reward.Count);
                grantCount++;
            }

            return grantCount;
        }

        private static int CountValidDecorationRewards(string[] decorationIds)
        {
            if (decorationIds == null)
            {
                return 0;
            }

            int grantCount = 0;
            for (int i = 0; i < decorationIds.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(decorationIds[i]))
                {
                    grantCount++;
                }
            }

            return grantCount;
        }

        private static int GrantPlantTierUnlocks(
            PlantTierUnlockReward[] plantTierUnlocks,
            PlantUnlockService plantUnlockService)
        {
            if (plantTierUnlocks == null)
            {
                return 0;
            }

            if (plantUnlockService != null)
            {
                return plantUnlockService.GrantUnlocks(plantTierUnlocks);
            }

            int grantCount = 0;
            for (int i = 0; i < plantTierUnlocks.Length; i++)
            {
                PlantTierUnlockReward reward = plantTierUnlocks[i];
                if (reward != null &&
                    !string.IsNullOrWhiteSpace(reward.FamilyId) &&
                    reward.Tier > 0)
                {
                    grantCount++;
                }
            }

            return grantCount;
        }
    }
}
