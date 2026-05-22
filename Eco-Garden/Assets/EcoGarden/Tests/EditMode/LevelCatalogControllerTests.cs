using EcoGarden.Board;
using EcoGarden.Config;
using EcoGarden.Progression;
using EcoGarden.Save;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class LevelCatalogControllerTests
    {
        [Test]
        public void SelectHighestUnlockedLevel_AssignsBoardLevel()
        {
            GameObject boardObject = new GameObject("Board");
            GameObject controllerObject = new GameObject("LevelCatalogController");
            try
            {
                BoardController boardController = boardObject.AddComponent<BoardController>();
                LevelCatalogController catalogController = controllerObject.AddComponent<LevelCatalogController>();
                catalogController.SetBoardController(boardController);
                catalogController.SetCatalog(CreateCatalog(CreateLevel(1), CreateLevel(2), CreateLevel(3)));

                bool selected = catalogController.SelectHighestUnlockedLevel(new SaveData { highestUnlockedLevel = 2 });

                Assert.IsTrue(selected);
                Assert.AreEqual(2, boardController.LevelDefinition.LevelId);
                Assert.AreEqual(2, catalogController.SelectedLevel.LevelId);
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
                Object.DestroyImmediate(boardObject);
            }
        }

        [Test]
        public void SelectLevel_RejectsLockedLevel()
        {
            GameObject boardObject = new GameObject("Board");
            GameObject controllerObject = new GameObject("LevelCatalogController");
            try
            {
                BoardController boardController = boardObject.AddComponent<BoardController>();
                LevelCatalogController catalogController = controllerObject.AddComponent<LevelCatalogController>();
                catalogController.SetBoardController(boardController);
                catalogController.SetCatalog(CreateCatalog(CreateLevel(1), CreateLevel(2)));

                bool selected = catalogController.SelectLevel(2, new SaveData { highestUnlockedLevel = 1 });

                Assert.IsFalse(selected);
                Assert.IsNull(boardController.LevelDefinition);
                Assert.IsNull(catalogController.SelectedLevel);
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
                Object.DestroyImmediate(boardObject);
            }
        }

        private static LevelCatalogDefinition CreateCatalog(params LevelDefinition[] levels)
        {
            LevelCatalogDefinition catalog = ScriptableObject.CreateInstance<LevelCatalogDefinition>();
            catalog.EditorSetValues("test_catalog", "Test Catalog", new System.Collections.Generic.List<LevelDefinition>(levels));
            return catalog;
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
