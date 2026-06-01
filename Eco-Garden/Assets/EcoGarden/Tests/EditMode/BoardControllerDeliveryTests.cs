using System.Collections.Generic;
using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Config;
using NUnit.Framework;
using UnityEngine;

namespace EcoGarden.Tests.EditMode
{
    public sealed class BoardControllerDeliveryTests
    {
        private GameObject gameObject;
        private BoardController boardController;

        [TearDown]
        public void TearDown()
        {
            if (gameObject != null)
            {
                Object.DestroyImmediate(gameObject);
            }
        }

        [Test]
        public void TryDeliverOrder_TracksPartialProgressBeforeCompletion()
        {
            CreateBoardControllerWithOrder(new NpcOrderDefinition(
                "two_sprouts",
                "Two Sprouts",
                new[] { new OrderRequirementDefinition("lotus", 1, 2) }));

            int orderCompletedCount = 0;
            int progressChangedCount = 0;
            boardController.OrderCompleted += () => orderCompletedCount++;
            boardController.OrderProgressChanged += () => progressChangedCount++;

            bool firstDelivered = boardController.TryDeliverOrder(new GridPosition(3, 1), false);
            bool secondDelivered = boardController.TryDeliverOrder(new GridPosition(4, 1), false);

            Assert.IsTrue(firstDelivered);
            Assert.IsTrue(secondDelivered);
            Assert.IsNull(boardController.BoardState.GetCell(new GridPosition(3, 1)).Item);
            Assert.IsNull(boardController.BoardState.GetCell(new GridPosition(4, 1)).Item);
            Assert.AreEqual(2, boardController.ActiveOrderRequirements[0].SubmittedCount);
            Assert.AreEqual(2, progressChangedCount);
            Assert.AreEqual(1, orderCompletedCount);
        }

        [Test]
        public void StartNextOrder_ResetsSubmittedProgressForNextCustomer()
        {
            CreateBoardControllerWithOrder(new NpcOrderDefinition(
                "one_sprout",
                "One Sprout",
                new[] { new OrderRequirementDefinition("lotus", 1, 1) }));

            Assert.IsTrue(boardController.TryDeliverOrder(new GridPosition(3, 1), false));
            Assert.AreEqual(1, boardController.ActiveOrderRequirements[0].SubmittedCount);

            boardController.StartNextOrder();

            Assert.AreEqual(0, boardController.ActiveOrderRequirements[0].SubmittedCount);
        }

        [Test]
        public void TryDeliverOrder_RejectsItemsThatDoNotMatchIncompleteRequirement()
        {
            CreateBoardControllerWithOrder(new NpcOrderDefinition(
                "two_sprouts",
                "Two Sprouts",
                new[] { new OrderRequirementDefinition("lotus", 2, 1) }));
            int boardChangedCount = 0;
            int deliveredCount = 0;
            int progressChangedCount = 0;
            int completedCount = 0;
            boardController.BoardChanged += () => boardChangedCount++;
            boardController.ItemDelivered += _ => deliveredCount++;
            boardController.OrderProgressChanged += () => progressChangedCount++;
            boardController.OrderCompleted += () => completedCount++;

            bool delivered = boardController.TryDeliverOrder(new GridPosition(3, 1), false);

            Assert.IsFalse(delivered);
            Assert.IsNotNull(boardController.BoardState.GetCell(new GridPosition(3, 1)).Item);
            Assert.AreEqual(0, boardController.ActiveOrderRequirements[0].SubmittedCount);
            Assert.AreEqual(0, boardChangedCount);
            Assert.AreEqual(0, deliveredCount);
            Assert.AreEqual(0, progressChangedCount);
            Assert.AreEqual(0, completedCount);
        }

        [Test]
        public void TryMoveOrMerge_DeliveryToNpcPointRaisesSingleBoardChanged()
        {
            CreateBoardControllerWithOrder(new NpcOrderDefinition(
                "one_sprout",
                "One Sprout",
                new[] { new OrderRequirementDefinition("lotus", 1, 1) }));
            GridPosition sourcePosition = new GridPosition(3, 1);
            GridPosition deliveryPosition = new GridPosition(0, 0);
            boardController.BoardState.GetCell(deliveryPosition).Kind = CellKind.NpcOrderPoint;
            int boardChangedCount = 0;
            int deliveredCount = 0;
            int completedCount = 0;
            boardController.BoardChanged += () => boardChangedCount++;
            boardController.ItemDelivered += _ => deliveredCount++;
            boardController.OrderCompleted += () => completedCount++;

            bool delivered = boardController.TryMoveOrMerge(sourcePosition, deliveryPosition, false);

            Assert.IsTrue(delivered);
            Assert.IsNull(boardController.BoardState.GetCell(sourcePosition).Item);
            Assert.IsNull(boardController.BoardState.GetCell(deliveryPosition).Item);
            Assert.AreEqual(1, boardChangedCount);
            Assert.AreEqual(1, deliveredCount);
            Assert.AreEqual(1, completedCount);
        }

        [Test]
        public void ClearedObstacles_CanBeCapturedAndRestoredAfterLevelReload()
        {
            CreateBoardControllerWithOrder(new NpcOrderDefinition(
                "one_sprout",
                "One Sprout",
                new[] { new OrderRequirementDefinition("lotus", 1, 1) }));
            GridPosition obstaclePosition = new GridPosition(2, 5);

            Assert.IsTrue(boardController.TryUseAbility(AbilityKind.Shovel, obstaclePosition));

            List<GridPosition> clearedObstacles = boardController.CaptureClearedObstaclePositions();
            CollectionAssert.Contains(clearedObstacles, obstaclePosition);

            boardController.LoadLevel();
            Assert.AreEqual(CellKind.Obstacle, boardController.BoardState.GetCell(obstaclePosition).Kind);

            boardController.RestoreClearedObstacles(clearedObstacles, false);

            Assert.AreEqual(CellKind.Empty, boardController.BoardState.GetCell(obstaclePosition).Kind);
            Assert.AreEqual(ObstacleKind.None, boardController.BoardState.GetCell(obstaclePosition).ObstacleKind);
        }

        private void CreateBoardControllerWithOrder(NpcOrderDefinition order)
        {
            LevelDefinition baseLevel = TestLevelFactory.CreateLevel15();
            LevelDefinition level = ScriptableObject.CreateInstance<LevelDefinition>();
            level.EditorSetValues(
                baseLevel.LevelId,
                baseLevel.LevelName,
                baseLevel.Width,
                baseLevel.Height,
                new List<string>(baseLevel.RowsTopToBottom).ToArray(),
                baseLevel.DefaultProducer,
                new List<ItemDefinition>
                {
                    baseLevel.GetItemDefinitionForLevel(1),
                    baseLevel.GetItemDefinitionForLevel(2),
                    baseLevel.GetItemDefinitionForLevel(3),
                    baseLevel.GetItemDefinitionForLevel(4),
                    baseLevel.GetItemDefinitionForLevel(5)
                },
                order,
                new List<AbilityCountDefinition>
                {
                    new AbilityCountDefinition(AbilityKind.Shovel, 2),
                    new AbilityCountDefinition(AbilityKind.MagicWand, 1),
                    new AbilityCountDefinition(AbilityKind.SortingMagnet, 1)
                },
                baseLevel.TimerSeconds,
                baseLevel.ThemeId);

            gameObject = new GameObject("BoardControllerDeliveryTests");
            gameObject.AddComponent<BoardView>();
            boardController = gameObject.AddComponent<BoardController>();
            boardController.SetLevelDefinition(level);
            boardController.LoadLevel();
        }
    }
}
