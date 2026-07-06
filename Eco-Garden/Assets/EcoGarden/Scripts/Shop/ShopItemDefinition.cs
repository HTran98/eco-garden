using EcoGarden.Rewards;
using UnityEngine;

namespace EcoGarden.Shop
{
    [CreateAssetMenu(menuName = "Eco Garden/Shop/Shop Item", fileName = "ShopItemDefinition")]
    public sealed class ShopItemDefinition : ScriptableObject
    {
        [SerializeField] private string productId;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private ShopItemCategory category;
        [SerializeField] private Sprite icon;
        [SerializeField] private ShopPriceDefinition price;
        [SerializeField] private RewardDefinition grant;
        [SerializeField] private bool repeatable = true;

        public string ProductId { get { return productId; } }
        public string DisplayName { get { return displayName; } }
        public string Description { get { return description; } }
        public ShopItemCategory Category { get { return category; } }
        public Sprite Icon { get { return icon; } }
        public ShopPriceDefinition Price { get { return price; } }
        public RewardDefinition Grant { get { return grant; } }
        public bool Repeatable { get { return repeatable; } }
        public bool IsValid { get { return !string.IsNullOrWhiteSpace(productId) && price != null && grant != null; } }

#if UNITY_EDITOR
        public void EditorSetValues(
            string productId,
            string displayName,
            string description,
            ShopItemCategory category,
            ShopPriceDefinition price,
            RewardDefinition grant,
            bool repeatable,
            Sprite icon = null)
        {
            this.productId = productId;
            this.displayName = displayName;
            this.description = description;
            this.category = category;
            this.icon = icon;
            this.price = price;
            this.grant = grant;
            this.repeatable = repeatable;
        }
#endif
    }
}
