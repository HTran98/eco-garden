using System.Collections.Generic;

namespace EcoGarden.Shop
{
    public sealed class ShopCatalogService
    {
        private readonly List<ShopItemDefinition> items = new List<ShopItemDefinition>();
        private readonly Dictionary<string, ShopItemDefinition> itemsById = new Dictionary<string, ShopItemDefinition>();

        public IReadOnlyList<ShopItemDefinition> Items { get { return items; } }

        public ShopCatalogService(IEnumerable<ShopItemDefinition> definitions)
            : this(definitions, false)
        {
        }

        public ShopCatalogService(IEnumerable<ShopItemDefinition> definitions, bool includeDecorationItems)
        {
            if (definitions == null)
            {
                return;
            }

            foreach (ShopItemDefinition definition in definitions)
            {
                if (definition == null ||
                    !definition.IsValid ||
                    (!includeDecorationItems && definition.Category == ShopItemCategory.Decoration) ||
                    itemsById.ContainsKey(definition.ProductId))
                {
                    continue;
                }

                items.Add(definition);
                itemsById.Add(definition.ProductId, definition);
            }
        }

        public bool TryGetItem(string productId, out ShopItemDefinition definition)
        {
            definition = null;
            return !string.IsNullOrWhiteSpace(productId) &&
                   itemsById.TryGetValue(productId, out definition);
        }

        public List<ShopItemDefinition> GetItemsByCategory(ShopItemCategory category)
        {
            List<ShopItemDefinition> result = new List<ShopItemDefinition>();
            for (int i = 0; i < items.Count; i++)
            {
                ShopItemDefinition item = items[i];
                if (item != null && item.Category == category)
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
