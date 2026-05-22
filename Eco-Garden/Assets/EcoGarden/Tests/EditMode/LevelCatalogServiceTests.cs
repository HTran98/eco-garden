using EcoGarden.Config;
using EcoGarden.Progression;
using EcoGarden.Save;
using NUnit.Framework;

namespace EcoGarden.Tests.EditMode
{
    public sealed class LevelCatalogServiceTests
    {
        [Test]
        public void Constructor_SortsLevelsAndSkipsDuplicateIds()
        {
            LevelDefinition level3 = CreateLevel(3);
            LevelDefinition level1 = CreateLevel(1);
            LevelDefinition duplicateLevel1 = CreateLevel(1);

            LevelCatalogService service = new LevelCatalogService(new[] { level3, null, level1, duplicateLevel1 });

            Assert.AreEqual(2, service.Levels.Count);
            Assert.AreEqual(1, service.Levels[0].LevelId);
            Assert.AreEqual(3, service.Levels[1].LevelId);
        }

        [Test]
        public void TryGetHighestUnlockedLevel_ReturnsHighestAvailableUnlockedLevel()
        {
            LevelCatalogService service = new LevelCatalogService(new[]
            {
                CreateLevel(1),
                CreateLevel(2),
                CreateLevel(3),
                CreateLevel(4)
            });

            bool found = service.TryGetHighestUnlockedLevel(new SaveData { highestUnlockedLevel = 3 }, out LevelDefinition level);

            Assert.IsTrue(found);
            Assert.AreEqual(3, level.LevelId);
        }

        [Test]
        public void TryGetLevel_ReturnsFalseForMissingLevel()
        {
            LevelCatalogService service = new LevelCatalogService(new[] { CreateLevel(1) });

            Assert.IsFalse(service.TryGetLevel(2, out LevelDefinition level));
            Assert.IsNull(level);
        }

        private static LevelDefinition CreateLevel(int levelId)
        {
            LevelDefinition level = TestLevelFactory.CreateLevel15();
            level.EditorSetValues(
                levelId,
                "Level " + levelId,
                level.Width,
                level.Height,
                ToArray(level.RowsTopToBottom),
                level.DefaultProducer,
                new System.Collections.Generic.List<ItemDefinition>
                {
                    level.GetItemDefinitionForLevel(1),
                    level.GetItemDefinitionForLevel(2),
                    level.GetItemDefinitionForLevel(3),
                    level.GetItemDefinitionForLevel(4),
                    level.GetItemDefinitionForLevel(5)
                },
                level.NpcOrder,
                new System.Collections.Generic.List<AbilityCountDefinition>(level.StartingAbilities),
                level.TimerSeconds,
                level.ThemeId,
                null,
                level.Difficulty,
                new System.Collections.Generic.List<TemporaryLockDefinition>(level.TemporaryLocks));

            return level;
        }

        private static string[] ToArray(System.Collections.Generic.IReadOnlyList<string> rows)
        {
            string[] result = new string[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                result[i] = rows[i];
            }

            return result;
        }
    }
}
