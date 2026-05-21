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

            bool delivered = boardController.TryDeliverOrder(new GridPosition(3, 1), false);

            Assert.IsFalse(delivered);
            Assert.IsNotNull(boardController.BoardState.GetCell(new GridPosition(3, 1)).Item);
            Assert.AreEqual(0, boardController.ActiveOrderRequirements[0].SubmittedCount);
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
