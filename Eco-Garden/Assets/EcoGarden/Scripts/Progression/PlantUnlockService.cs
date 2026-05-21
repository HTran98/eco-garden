using EcoGarden.Config;
using EcoGarden.Items;
using EcoGarden.Rewards;
using System;
using System.Collections.Generic;

namespace EcoGarden.Progression
{
    public sealed class PlantUnlockService
    {
        public const int DefaultUnlockedTier = 3;

        private readonly HashSet<string> unlockedTierKeys = new HashSet<string>();
        private readonly HashSet<string> temporaryAllowedTierKeys = new HashSet<string>();

        public event Action Changed;

        public bool UnlockTier(string familyId, int tier)
        {
            if (string.IsNullOrWhiteSpace(familyId) || tier <= 0)
            {
                return false;
            }

            bool changed = false;
            for (int currentTier = DefaultUnlockedTier + 1; currentTier <= tier; currentTier++)
            {
                changed |= unlockedTierKeys.Add(BuildKey(familyId, currentTier));
            }

            if (changed)
            {
                Changed?.Invoke();
            }

            return changed;
        }

        public bool IsTierUnlocked(string familyId, int tier)
        {
            if (string.IsNullOrWhiteSpace(familyId) || tier <= 0)
            {
                return false;
            }

            if (tier <= DefaultUnlockedTier)
            {
                return true;
            }

            string key = BuildKey(familyId, tier);
            return unlockedTierKeys.Contains(key) || temporaryAllowedTierKeys.Contains(key);
        }

        public bool IsMergeOutputAllowed(BoardItem sourceItem)
        {
            return sourceItem != null && IsTierUnlocked(sourceItem.FamilyId, sourceItem.Level + 1);
        }

        public bool IsRequirementAllowed(OrderRequirementDefinition requirement)
        {
            return requirement != null && IsTierUnlocked(requirement.FamilyId, requirement.Level);
        }

        public void SetTemporaryAllowedTiers(IEnumerable<PlantTierUnlockDefinition> temporaryUnlocks)
        {
            temporaryAllowedTierKeys.Clear();
            if (temporaryUnlocks == null)
            {
                return;
            }

            foreach (PlantTierUnlockDefinition unlock in temporaryUnlocks)
            {
                if (unlock != null &&
                    !string.IsNullOrWhiteSpace(unlock.FamilyId) &&
                    unlock.Tier > DefaultUnlockedTier)
                {
                    temporaryAllowedTierKeys.Add(BuildKey(unlock.FamilyId, unlock.Tier));
                }
            }
        }

        public void RestoreUnlockedTiers(IEnumerable<PlantTierUnlockDefinition> savedUnlocks)
        {
            unlockedTierKeys.Clear();
            if (savedUnlocks == null)
            {
                return;
            }

            foreach (PlantTierUnlockDefinition unlock in savedUnlocks)
            {
                if (unlock != null)
                {
                    UnlockTier(unlock.FamilyId, unlock.Tier);
                }
            }
        }

        public PlantTierUnlockDefinition[] GetSavedUnlocks()
        {
            List<PlantTierUnlockDefinition> result = new List<PlantTierUnlockDefinition>();
            foreach (string key in unlockedTierKeys)
            {
                if (TryParseKey(key, out string familyId, out int tier))
                {
                    result.Add(new PlantTierUnlockDefinition(familyId, tier));
                }
            }

            return result.ToArray();
        }

        public int GrantUnlocks(PlantTierUnlockReward[] rewards)
        {
            if (rewards == null)
            {
                return 0;
            }

            int grantCount = 0;
            for (int i = 0; i < rewards.Length; i++)
            {
                PlantTierUnlockReward reward = rewards[i];
                if (reward != null && UnlockTier(reward.FamilyId, reward.Tier))
                {
                    grantCount++;
                }
            }

            return grantCount;
        }

        private static string BuildKey(string familyId, int tier)
        {
            return familyId.Trim().ToLowerInvariant() + ":" + tier;
        }

        private static bool TryParseKey(string key, out string familyId, out int tier)
        {
            familyId = string.Empty;
            tier = 0;
            int separator = key.IndexOf(':');
            if (separator <= 0 || separator >= key.Length - 1)
            {
                return false;
            }

            familyId = key.Substring(0, separator);
            return int.TryParse(key.Substring(separator + 1), out tier);
        }
    }
}
