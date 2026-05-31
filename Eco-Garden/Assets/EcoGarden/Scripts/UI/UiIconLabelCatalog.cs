using EcoGarden.Abilities;
using EcoGarden.Economy;

namespace EcoGarden.UI
{
    public static class UiIconLabelCatalog
    {
        public const string Level = "Lv";
        public const string Mission = "Task";
        public const string Shop = "$";
        public const string Bag = "Bag";
        public const string Pause = "II";
        public const string Close = "X";
        public const string Restart = "R";
        public const string Next = ">";
        public const string Gold = "G";
        public const string Gem = "*";

        public static string Currency(CurrencyKind currencyKind)
        {
            return currencyKind == CurrencyKind.Gem ? Gem : Gold;
        }

        public static string Ability(AbilityKind abilityKind)
        {
            switch (abilityKind)
            {
                case AbilityKind.Shovel:
                    return "SH";
                case AbilityKind.MagicWand:
                    return "WD";
                case AbilityKind.SortingMagnet:
                    return "MG";
                default:
                    return "?";
            }
        }

        public static string AbilityWithCount(AbilityKind abilityKind, int count)
        {
            return Ability(abilityKind) + "\nx" + count;
        }

        public static string Count(int count)
        {
            return "x" + count;
        }
    }
}
