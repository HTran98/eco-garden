using System.Collections.Generic;
using System;

namespace EcoGarden.Abilities
{
    public sealed class AbilityInventory
    {
        private readonly Dictionary<AbilityKind, int> counts = new Dictionary<AbilityKind, int>();

        public event Action<AbilityKind, int> CountChanged;

        public int GetCount(AbilityKind abilityKind)
        {
            return counts.TryGetValue(abilityKind, out int count) ? count : 0;
        }

        public void SetCount(AbilityKind abilityKind, int count)
        {
            int safeCount = count < 0 ? 0 : count;
            counts[abilityKind] = safeCount;
            CountChanged?.Invoke(abilityKind, safeCount);
        }

        public bool TryConsume(AbilityKind abilityKind)
        {
            int count = GetCount(abilityKind);
            if (count <= 0)
            {
                return false;
            }

            counts[abilityKind] = count - 1;
            CountChanged?.Invoke(abilityKind, counts[abilityKind]);
            return true;
        }
    }
}
