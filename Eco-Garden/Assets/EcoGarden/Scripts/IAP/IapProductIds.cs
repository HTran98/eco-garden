namespace EcoGarden.IAP
{
    public static class IapProductIds
    {
        public const string GemsSmall = "eco_garden_gems_small";
        public const string GemsMedium = "eco_garden_gems_medium";

        public static string[] CreateRequiredConsumableIds()
        {
            return new[]
            {
                GemsSmall,
                GemsMedium
            };
        }
    }
}
