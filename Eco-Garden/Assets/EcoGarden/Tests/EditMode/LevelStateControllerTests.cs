using EcoGarden.Board;
using EcoGarden.Config;
using EcoGarden.Level;
using EcoGarden.Progression;
using EcoGarden.Save;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class LevelStateControllerTests
    {
        [Test]
        public void CompleteLevel_SetsCompletedStateFromPlaying()
        {
            GameObject boardObject = new GameObject("Board");
            GameObject levelObject = new GameObject("LevelStateController");
            try
            {
                BoardController boardController = boardObject.AddComponent<BoardController>();
                boardController.SetLevelDefinition(TestLevelFactory.CreateLevel15());

                LevelStateController levelStateController = levelObject.AddComponent<LevelStateController>();
                levelStateController.StartLevel();

                levelStateController.CompleteLevel();

                Assert.AreEqual(LevelPlayState.Completed, levelStateController.State);
                Assert.IsFalse(levelStateController.IsPlaying);
            }
            finally
            {
                Object.DestroyImmediate(levelObject);
                Object.DestroyImmediate(boardObject);
            }
        }

        [Test]
        public void CompleteLevel_RaisesCompletedEventOnce()
        {
            GameObject boardObject = new GameObject("Board");
            GameObject levelObject = new GameObject("LevelStateController");
            try
            {
                BoardController boardController = boardObject.AddComponent<BoardController>();
                boardController.SetLevelDefinition(TestLevelFactory.CreateLevel15());

                LevelStateController levelStateController = levelObject.AddComponent<LevelStateController>();
                int completedCount = 0;
                levelStateController.LevelCompleted += () => completedCount++;

                levelStateController.StartLevel();
                levelStateController.CompleteLevel();
                levelStateController.CompleteLevel();

                Assert.AreEqual(1, completedCount);
            }
            finally
            {
                Object.DestroyImmediate(levelObject);
                Object.DestroyImmediate(boardObject);
            }
        }

        [Test]
        public void StartNextLevel_SelectsNextCatalogLevel()
        {
            SaveService.Clear();

            GameObject boardObject = new GameObject("Board");
            GameObject catalogObject = new GameObject("LevelCatalogController");
            GameObject levelObject = new GameObject("LevelStateController");
            try
            {
                LevelDefinition level1 = CreateLevel(1);
                LevelDefinition level2 = CreateLevel(2);

                BoardController boardController = boardObject.AddComponent<BoardController>();
                boardController.SetLevelDefinition(level1);

                LevelCatalogController catalogController = catalogObject.AddComponent<LevelCatalogController>();
                catalogController.SetBoardController(boardController);
                catalogController.SetCatalog(CreateCatalog(level1, level2));

                LevelStateController levelStateController = levelObject.AddComponent<LevelStateController>();
                levelStateController.StartLevel();
                levelStateController.StartNextLevel();

                Assert.AreEqual(2, boardController.LevelDefinition.LevelId);
            }
            finally
            {
                SaveService.Clear();
                Object.DestroyImmediate(levelObject);
                Object.DestroyImmediate(catalogObject);
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
