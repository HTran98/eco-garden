using System;
using EcoGarden.Abilities;
using UnityEngine;

namespace EcoGarden.Rewards
{
    [Serializable]
    public sealed class AbilityReward
    {
        [SerializeField] private AbilityKind abilityKind;
        [SerializeField] private int count;

        public AbilityKind AbilityKind { get { return abilityKind; } }
        public int Count { get { return count; } }

        public AbilityReward()
        {
        }

        public AbilityReward(AbilityKind abilityKind, int count)
        {
            this.abilityKind = abilityKind;
            this.count = count;
        }
    }
}
