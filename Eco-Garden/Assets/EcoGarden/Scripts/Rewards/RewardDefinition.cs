using System;
using UnityEngine;

namespace EcoGarden.Rewards
{
    [Serializable]
    public sealed class RewardDefinition
    {
        [SerializeField] private CurrencyReward[] currencies;
        [SerializeField] private AbilityReward[] abilities;
        [SerializeField] private string[] decorationIds;
        [SerializeField] private PlantTierUnlockReward[] plantTierUnlocks;

        public CurrencyReward[] Currencies { get { return currencies; } }
        public AbilityReward[] Abilities { get { return abilities; } }
        public string[] DecorationIds { get { return decorationIds; } }
        public PlantTierUnlockReward[] PlantTierUnlocks { get { return plantTierUnlocks; } }

        public RewardDefinition()
        {
        }

        public RewardDefinition(
            CurrencyReward[] currencies,
            AbilityReward[] abilities,
            string[] decorationIds = null,
            PlantTierUnlockReward[] plantTierUnlocks = null)
        {
            this.currencies = currencies;
            this.abilities = abilities;
            this.decorationIds = decorationIds;
            this.plantTierUnlocks = plantTierUnlocks;
        }
    }
}
