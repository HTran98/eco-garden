namespace EcoGarden.IAP
{
    public static class IapProductIds
    {
        public const string Gems010 = "eco_garden_gems_010";
        public const string Gems020 = "eco_garden_gems_020";
        public const string Gems030 = "eco_garden_gems_030";
        public const string Gems040 = "eco_garden_gems_040";
        public const string Gems050 = "eco_garden_gems_050";
        public const string Gems060 = "eco_garden_gems_060";
        public const string Gems070 = "eco_garden_gems_070";
        public const string Gems080 = "eco_garden_gems_080";
        public const string Gems090 = "eco_garden_gems_090";
        public const string Gems100 = "eco_garden_gems_100";
        public const string Gems150 = "eco_garden_gems_150";
        public const string Gems200 = "eco_garden_gems_200";
        public const string Gems250 = "eco_garden_gems_250";
        public const string GemsSmall = "eco_garden_gems_small";
        public const string GemsMedium = "eco_garden_gems_medium";

        public static string[] CreateRequiredConsumableIds()
        {
            return new[]
            {
                Gems010,
                Gems020,
                Gems030,
                Gems040,
                Gems050,
                Gems060,
                Gems070,
                Gems080,
                Gems090,
                Gems100,
                Gems150,
                Gems200,
                Gems250,
                GemsSmall,
                GemsMedium
            };
        }
    }
}
