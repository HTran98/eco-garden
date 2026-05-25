using System.Collections;
using EcoGarden.Board;
using EcoGarden.Economy;
using EcoGarden.Input;
using EcoGarden.Level;
using EcoGarden.Missions;
using EcoGarden.Shop;
using EcoGarden.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace EcoGarden.Tests.PlayMode
{
    public sealed class SceneSmokePlayModeTests
    {
        private const string Level15SceneName = "EcoGarden_Level15_VerticalSlice";

        [UnityTest]
        public IEnumerator Level15Scene_BootsAndSupportsCoreSmokeActions()
        {
            yield return SceneManager.LoadSceneAsync(Level15SceneName, LoadSceneMode.Single);
            yield return null;
            yield return null;

            BoardController boardController = Object.FindAnyObjectByType<BoardController>();
            BoardView boardView = Object.FindAnyObjectByType<BoardView>();
            BoardInputController inputController = Object.FindAnyObjectByType<BoardInputController>();
            EconomyController economyController = Object.FindAnyObjectByType<EconomyController>();
            LevelStateController levelStateController = Object.FindAnyObjectByType<LevelStateController>();
            ShopController shopController = Object.FindAnyObjectByType<ShopController>();
            MissionController missionController = Object.FindAnyObjectByType<MissionController>();
            ShopUiController shopUiController = Object.FindAnyObjectByType<ShopUiController>();
            MissionUiController missionUiController = Object.FindAnyObjectByType<MissionUiController>();

            Assert.NotNull(boardController);
            Assert.NotNull(boardView);
            Assert.NotNull(inputController);
            Assert.NotNull(economyController);
            Assert.NotNull(levelStateController);
            Assert.NotNull(shopController);
            Assert.NotNull(missionController);
            Assert.NotNull(shopUiController);
            Assert.NotNull(missionUiController);
            Assert.NotNull(boardController.LevelDefinition);
            Assert.NotNull(boardController.BoardState);

            GridPosition producerPosition = FindProducerPosition(boardController);
            Assert.IsTrue(boardController.TrySpawnFromProducer(producerPosition, Time.time));
            Assert.IsTrue(CountBoardItems(boardController) > 0);

            shopUiController.ToggleShop();
            yield return null;
            Assert.IsTrue(FindSceneObject("ShopPanel").activeSelf);
            shopUiController.CloseShop();
            yield return null;
            Assert.IsFalse(FindSceneObject("ShopPanel").activeSelf);

            missionUiController.ToggleMissions();
            yield return null;
            Assert.IsTrue(FindSceneObject("MissionPanel").activeSelf);
            missionUiController.CloseMissions();
            yield return null;
            Assert.IsFalse(FindSceneObject("MissionPanel").activeSelf);

            Assert.AreEqual(LevelPlayState.Playing, levelStateController.State);
        }

        private static GridPosition FindProducerPosition(BoardController boardController)
        {
            foreach (BoardCell cell in boardController.BoardState.GetCells())
            {
                if (cell != null && cell.Kind == CellKind.Producer)
                {
                    return cell.Position;
                }
            }

            Assert.Fail("Scene has no producer cell.");
            return default;
        }

        private static int CountBoardItems(BoardController boardController)
        {
            int count = 0;
            foreach (BoardCell cell in boardController.BoardState.GetCells())
            {
                if (cell != null && cell.Item != null)
                {
                    count++;
                }
            }

            return count;
        }

        private static GameObject FindSceneObject(string objectName)
        {
            GameObject activeObject = GameObject.Find(objectName);
            if (activeObject != null)
            {
                return activeObject;
            }

            Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform candidate = transforms[i];
                if (candidate != null &&
                    candidate.gameObject.scene.IsValid() &&
                    candidate.name == objectName)
                {
                    return candidate.gameObject;
                }
            }

            Assert.Fail("Scene object not found: " + objectName);
            return null;
        }
    }
}
