using System;
using UnityEngine;

namespace EcoGarden.Rewards
{
    [Serializable]
    public sealed class PlantTierUnlockReward
    {
        [SerializeField] private string familyId;
        [SerializeField] private int tier;

        public string FamilyId { get { return familyId; } }
        public int Tier { get { return tier; } }

        public PlantTierUnlockReward()
        {
        }

        public PlantTierUnlockReward(string familyId, int tier)
        {
            this.familyId = familyId;
            this.tier = tier;
        }
    }
}
