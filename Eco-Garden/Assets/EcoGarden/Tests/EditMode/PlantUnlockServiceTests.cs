using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Items;
using EcoGarden.Progression;
using NUnit.Framework;
using System.Collections.Generic;

namespace EcoGarden.Tests.EditMode
{
    public sealed class PlantUnlockServiceTests
    {
        [Test]
        public void IsTierUnlocked_DefaultsLowTiersOnly()
        {
            PlantUnlockService service = new PlantUnlockService();

            Assert.IsTrue(service.IsTierUnlocked("lotus", 1));
            Assert.IsTrue(service.IsTierUnlocked("lotus", 3));
            Assert.IsFalse(service.IsTierUnlocked("lotus", 4));
        }

        [Test]
        public void TryMergeItem_BlocksLockedOutputTier()
        {
            BoardState boardState = CreateBoardState(new PlantUnlockService());
            BoardItem item = new BoardItem("lotus", 3, "lotus_lv03");
            boardState.TryPlaceItem(new GridPosition(0, 0), item);
            boardState.TryPlaceItem(new GridPosition(1, 0), new BoardItem("lotus", 3, "lotus_lv03"));

            bool merged = boardState.TryMergeItem(new GridPosition(0, 0), new GridPosition(1, 0));

            Assert.IsFalse(merged);
            Assert.AreSame(item, boardState.GetCell(new GridPosition(0, 0)).Item);
        }

        [Test]
        public void TryMergeItem_AllowsUnlockedOutputTier()
        {
            PlantUnlockService unlockService = new PlantUnlockService();
            unlockService.UnlockTier("lotus", 4);
            BoardState boardState = CreateBoardState(unlockService);
            boardState.TryPlaceItem(new GridPosition(0, 0), new BoardItem("lotus", 3, "lotus_lv03"));
            boardState.TryPlaceItem(new GridPosition(1, 0), new BoardItem("lotus", 3, "lotus_lv03"));

            bool merged = boardState.TryMergeItem(new GridPosition(0, 0), new GridPosition(1, 0));

            Assert.IsTrue(merged);
            Assert.AreEqual(4, boardState.GetCell(new GridPosition(1, 0)).Item.Level);
        }

        [Test]
        public void MagicWand_BlocksLockedOutputTierAndDoesNotConsume()
        {
            BoardState boardState = CreateBoardState(new PlantUnlockService());
            boardState.TryPlaceItem(new GridPosition(0, 0), new BoardItem("lotus", 3, "lotus_lv03"));
            AbilityInventory inventory = new AbilityInventory();
            inventory.SetCount(AbilityKind.MagicWand, 1);
            AbilityService service = new AbilityService(boardState, inventory);

            bool upgraded = service.TryUseMagicWand(new GridPosition(0, 0));

            Assert.IsFalse(upgraded);
            Assert.AreEqual(1, inventory.GetCount(AbilityKind.MagicWand));
            Assert.AreEqual(3, boardState.GetCell(new GridPosition(0, 0)).Item.Level);
        }

        private static BoardState CreateBoardState(PlantUnlockService unlockService)
        {
            BoardState boardState = new BoardState(2, 1, new Dictionary<int, EcoGarden.Config.ItemDefinition>
            {
                { 1, null },
                { 2, null },
                { 3, null },
                { 4, null }
            }, unlockService);

            boardState.SetCell(new BoardCell(new GridPosition(0, 0), CellKind.Empty));
            boardState.SetCell(new BoardCell(new GridPosition(1, 0), CellKind.Empty));
            return boardState;
        }
    }
}
