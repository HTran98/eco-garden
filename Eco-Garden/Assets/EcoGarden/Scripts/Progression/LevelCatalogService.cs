using System.Collections.Generic;
using EcoGarden.Config;
using EcoGarden.Save;

namespace EcoGarden.Progression
{
    public sealed class LevelCatalogService
    {
        private readonly List<LevelDefinition> levels = new List<LevelDefinition>();
        private readonly Dictionary<int, LevelDefinition> levelsById = new Dictionary<int, LevelDefinition>();

        public IReadOnlyList<LevelDefinition> Levels { get { return levels; } }

        public LevelCatalogService(IEnumerable<LevelDefinition> definitions)
        {
            if (definitions == null)
            {
                return;
            }

            foreach (LevelDefinition definition in definitions)
            {
                if (definition == null || definition.LevelId <= 0 || levelsById.ContainsKey(definition.LevelId))
                {
                    continue;
                }

                levelsById.Add(definition.LevelId, definition);
                InsertSorted(definition);
            }
        }

        public LevelCatalogService(LevelCatalogDefinition catalog)
            : this(catalog != null ? catalog.Levels : null)
        {
        }

        public bool TryGetLevel(int levelId, out LevelDefinition level)
        {
            return levelsById.TryGetValue(levelId, out level);
        }

        public bool TryGetFirstUnlockedLevel(SaveData data, out LevelDefinition level)
        {
            level = null;
            for (int i = 0; i < levels.Count; i++)
            {
                if (LevelProgressionService.IsLevelUnlocked(data, levels[i]))
                {
                    level = levels[i];
                    return true;
                }
            }

            return false;
        }

        public bool TryGetHighestUnlockedLevel(SaveData data, out LevelDefinition level)
        {
            level = null;
            for (int i = levels.Count - 1; i >= 0; i--)
            {
                if (LevelProgressionService.IsLevelUnlocked(data, levels[i]))
                {
                    level = levels[i];
                    return true;
                }
            }

            return false;
        }

        private void InsertSorted(LevelDefinition definition)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (definition.LevelId < levels[i].LevelId)
                {
                    levels.Insert(i, definition);
                    return;
                }
            }

            levels.Add(definition);
        }
    }
}
