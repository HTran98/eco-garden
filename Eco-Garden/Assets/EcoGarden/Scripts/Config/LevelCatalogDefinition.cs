using System.Collections.Generic;
using UnityEngine;

namespace EcoGarden.Config
{
    [CreateAssetMenu(menuName = "Eco Garden/Levels/Level Catalog", fileName = "LevelCatalogDefinition")]
    public sealed class LevelCatalogDefinition : ScriptableObject
    {
        [SerializeField] private string catalogId;
        [SerializeField] private string displayName;
        [SerializeField] private List<LevelDefinition> levels = new List<LevelDefinition>();

        public string CatalogId { get { return catalogId; } }
        public string DisplayName { get { return displayName; } }
        public IReadOnlyList<LevelDefinition> Levels { get { return levels; } }

#if UNITY_EDITOR
        public void EditorSetValues(string id, string name, List<LevelDefinition> levelDefinitions)
        {
            catalogId = id;
            displayName = name;
            levels = levelDefinitions ?? new List<LevelDefinition>();
        }
#endif
    }
}
