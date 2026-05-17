using UnityEngine;

namespace EcoGarden.Config
{
    [CreateAssetMenu(menuName = "Eco Garden/Producers/Producer Definition", fileName = "ProducerDefinition")]
    public sealed class ProducerDefinition : ScriptableObject
    {
        [SerializeField] private string producerId;
        [SerializeField] private ItemDefinition spawnItem;
        [SerializeField] private float cooldownSeconds = 1f;
        [SerializeField] private int spawnCostGold;

        public string ProducerId { get { return producerId; } }
        public ItemDefinition SpawnItem { get { return spawnItem; } }
        public float CooldownSeconds { get { return cooldownSeconds; } }
        public int SpawnCostGold { get { return spawnCostGold; } }

#if UNITY_EDITOR
        public void EditorSetValues(string id, ItemDefinition item, float cooldown, int cost)
        {
            producerId = id;
            spawnItem = item;
            cooldownSeconds = cooldown;
            spawnCostGold = cost;
        }
#endif
    }
}
