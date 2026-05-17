using System.Collections.Generic;

namespace EcoGarden.Abilities
{
    public sealed class AbilityInventory
    {
        private readonly Dictionary<AbilityKind, int> counts = new Dictionary<AbilityKind, int>();

        public int GetCount(AbilityKind abilityKind)
        {
            return counts.TryGetValue(abilityKind, out int count) ? count : 0;
        }

        public void SetCount(AbilityKind abilityKind, int count)
        {
            counts[abilityKind] = count < 0 ? 0 : count;
        }

        public bool TryConsume(AbilityKind abilityKind)
        {
            int count = GetCount(abilityKind);
            if (count <= 0)
            {
                return false;
            }

            counts[abilityKind] = count - 1;
            return true;
        }
    }
}
