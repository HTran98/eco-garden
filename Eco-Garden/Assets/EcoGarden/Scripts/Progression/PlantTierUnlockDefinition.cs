using System;
using UnityEngine;

namespace EcoGarden.Progression
{
    [Serializable]
    public sealed class PlantTierUnlockDefinition
    {
        [SerializeField] private string familyId;
        [SerializeField] private int tier;

        public string FamilyId { get { return familyId; } }
        public int Tier { get { return tier; } }

        public PlantTierUnlockDefinition()
        {
        }

        public PlantTierUnlockDefinition(string familyId, int tier)
        {
            this.familyId = familyId;
            this.tier = tier;
        }
    }
}
