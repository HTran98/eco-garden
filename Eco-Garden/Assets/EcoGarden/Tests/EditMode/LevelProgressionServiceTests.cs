using EcoGarden.Config;
using EcoGarden.Progression;
using EcoGarden.Save;
using NUnit.Framework;

namespace EcoGarden.Tests.EditMode
{
    public sealed class LevelProgressionServiceTests
    {
        [Test]
        public void TryUnlockNextLevel_UnlocksLevelAfterCompletedLevel()
        {
            SaveData data = new SaveData { highestUnlockedLevel = 1 };
            LevelDefinition level = TestLevelFactory.CreateLevel15();
            level.EditorSetValues(
                3,
                level.LevelName,
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

            bool changed = LevelProgressionService.TryUnlockNextLevel(data, level);

            Assert.IsTrue(changed);
            Assert.AreEqual(4, data.highestUnlockedLevel);
        }

        [Test]
        public void TryUnlockNextLevel_DoesNotLowerExistingProgress()
        {
            SaveData data = new SaveData { highestUnlockedLevel = 20 };
            LevelDefinition level = TestLevelFactory.CreateLevel15();

            bool changed = LevelProgressionService.TryUnlockNextLevel(data, level);

            Assert.IsFalse(changed);
            Assert.AreEqual(20, data.highestUnlockedLevel);
        }

        [Test]
        public void IsLevelUnlocked_UsesHighestUnlockedLevel()
        {
            SaveData data = new SaveData { highestUnlockedLevel = 3 };
            LevelDefinition level = TestLevelFactory.CreateLevel15();
            level.EditorSetValues(
                4,
                level.LevelName,
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

            Assert.IsFalse(LevelProgressionService.IsLevelUnlocked(data, level));
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
