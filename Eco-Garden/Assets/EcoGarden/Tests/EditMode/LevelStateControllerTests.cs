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
        public void CompleteLevel_HidesBlockingPanels()
        {
            GameObject boardObject = new GameObject("Board");
            GameObject levelObject = new GameObject("LevelStateController");
            GameObject missionPanel = new GameObject("MissionPanel");
            GameObject missionTrackerPanel = new GameObject("MissionTrackerPanel");
            try
            {
                BoardController boardController = boardObject.AddComponent<BoardController>();
                boardController.SetLevelDefinition(TestLevelFactory.CreateLevel15());
                missionPanel.SetActive(true);
                missionTrackerPanel.SetActive(true);

                LevelStateController levelStateController = levelObject.AddComponent<LevelStateController>();
                levelStateController.StartLevel();
                levelStateController.CompleteLevel();

                Assert.IsFalse(missionPanel.activeSelf);
                Assert.IsFalse(missionTrackerPanel.activeSelf);
            }
            finally
            {
                Object.DestroyImmediate(missionTrackerPanel);
                Object.DestroyImmediate(missionPanel);
                Object.DestroyImmediate(levelObject);
                Object.DestroyImmediate(boardObject);
            }
        }

        [Test]
        public void CompleteLevel_CreatesRuntimeNextButtonWhenMissing()
        {
            SaveService.Clear();

            GameObject boardObject = new GameObject("Board");
            GameObject catalogObject = new GameObject("LevelCatalogController");
            GameObject levelObject = new GameObject("LevelStateController");
            GameObject resultPanel = new GameObject("ResultPanel", typeof(RectTransform));
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
                levelStateController.CompleteLevel();

                Transform nextButton = resultPanel.transform.Find("NextLevelButton");
                Assert.IsNotNull(nextButton);
                Assert.IsTrue(nextButton.gameObject.activeSelf);
                Assert.IsTrue(nextButton.GetComponent<UnityEngine.UI.Button>().interactable);
            }
            finally
            {
                SaveService.Clear();
                Object.DestroyImmediate(resultPanel);
                Object.DestroyImmediate(levelObject);
                Object.DestroyImmediate(catalogObject);
                Object.DestroyImmediate(boardObject);
            }
        }

        [Test]
        public void StartNextLevelWithoutCatalog_ClosesResultPanelAndReplaysCurrentLevel()
        {
            GameObject boardObject = new GameObject("Board");
            GameObject levelObject = new GameObject("LevelStateController");
            GameObject resultPanel = new GameObject("ResultPanel", typeof(RectTransform));
            try
            {
                BoardController boardController = boardObject.AddComponent<BoardController>();
                boardObject.AddComponent<BoardView>();
                boardController.SetLevelDefinition(CreateLevel(1));

                LevelStateController levelStateController = levelObject.AddComponent<LevelStateController>();
                levelStateController.StartLevel();
                levelStateController.CompleteLevel();

                Assert.IsTrue(resultPanel.activeSelf);

                levelStateController.StartNextLevel();

                Assert.IsFalse(resultPanel.activeSelf);
                Assert.AreEqual(LevelPlayState.Playing, levelStateController.State);
                Assert.AreEqual(1, boardController.LevelDefinition.LevelId);
            }
            finally
            {
                Object.DestroyImmediate(resultPanel);
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
            GameObject objectiveObject = new GameObject("ObjectiveText");
            try
            {
                LevelDefinition level1 = CreateLevel(1);
                LevelDefinition level2 = CreateLevelWithOrder(2, new NpcOrderDefinition(
                    "level_2_order",
                    "Level 2 Order",
                    new[] { new OrderRequirementDefinition("lotus", 2, 1) }));

                BoardController boardController = boardObject.AddComponent<BoardController>();
                boardController.SetLevelDefinition(level1);
                objectiveObject.AddComponent<UnityEngine.UI.Text>();

                LevelCatalogController catalogController = catalogObject.AddComponent<LevelCatalogController>();
                catalogController.SetBoardController(boardController);
                catalogController.SetCatalog(CreateCatalog(level1, level2));

                LevelStateController levelStateController = levelObject.AddComponent<LevelStateController>();
                levelStateController.StartLevel();
                levelStateController.StartNextLevel();

                Assert.AreEqual(2, boardController.LevelDefinition.LevelId);
                Assert.AreEqual(LevelPlayState.Playing, levelStateController.State);
                StringAssert.Contains("0/1", objectiveObject.GetComponent<UnityEngine.UI.Text>().text);
            }
            finally
            {
                SaveService.Clear();
                Object.DestroyImmediate(objectiveObject);
                Object.DestroyImmediate(levelObject);
                Object.DestroyImmediate(catalogObject);
                Object.DestroyImmediate(boardObject);
            }
        }

        [Test]
        public void TogglePause_ShowsAndHidesPausePanel()
        {
            GameObject boardObject = new GameObject("Board");
            GameObject levelObject = new GameObject("LevelStateController");
            GameObject pausePanel = new GameObject("PausePanel", typeof(RectTransform));
            try
            {
                BoardController boardController = boardObject.AddComponent<BoardController>();
                boardController.SetLevelDefinition(TestLevelFactory.CreateLevel15());
                pausePanel.SetActive(false);

                LevelStateController levelStateController = levelObject.AddComponent<LevelStateController>();
                levelStateController.StartLevel();

                levelStateController.TogglePause();

                Assert.AreEqual(LevelPlayState.Paused, levelStateController.State);
                Assert.IsTrue(pausePanel.activeSelf);

                levelStateController.TogglePause();

                Assert.AreEqual(LevelPlayState.Playing, levelStateController.State);
                Assert.IsFalse(pausePanel.activeSelf);
            }
            finally
            {
                Object.DestroyImmediate(pausePanel);
                Object.DestroyImmediate(levelObject);
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
            LevelDefinition template = TestLevelFactory.CreateLevel15();
            return CreateLevelWithOrder(levelId, template.NpcOrder);
        }

        private static LevelDefinition CreateLevelWithOrder(int levelId, NpcOrderDefinition order)
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
                order,
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
