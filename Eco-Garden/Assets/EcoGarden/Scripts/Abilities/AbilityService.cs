using EcoGarden.Board;

namespace EcoGarden.Abilities
{
    public sealed class AbilityService
    {
        private readonly BoardState boardState;
        private readonly AbilityInventory inventory;

        public AbilityService(BoardState boardState, AbilityInventory inventory)
        {
            this.boardState = boardState;
            this.inventory = inventory;
        }

        public bool TryUseShovel(GridPosition target)
        {
            if (inventory.GetCount(AbilityKind.Shovel) <= 0 || !boardState.TryRemoveObstacle(target))
            {
                return false;
            }

            inventory.TryConsume(AbilityKind.Shovel);
            return true;
        }

        public bool TryUseMagicWand(GridPosition target)
        {
            if (inventory.GetCount(AbilityKind.MagicWand) <= 0 || !boardState.TryUpgradeItem(target))
            {
                return false;
            }

            inventory.TryConsume(AbilityKind.MagicWand);
            return true;
        }

        public bool TryUseSortingMagnet(string familyId, out GridPosition movedFrom, out GridPosition movedTo)
        {
            movedFrom = default;
            movedTo = default;

            if (inventory.GetCount(AbilityKind.SortingMagnet) <= 0 ||
                !boardState.TryUseSortingMagnet(familyId, out movedFrom, out movedTo))
            {
                return false;
            }

            inventory.TryConsume(AbilityKind.SortingMagnet);
            return true;
        }
    }
}
