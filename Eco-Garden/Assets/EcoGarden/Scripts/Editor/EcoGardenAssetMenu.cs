using System.Collections.Generic;
using EcoGarden.Abilities;
using EcoGarden.Config;
using UnityEditor;
using UnityEngine;

namespace EcoGarden.Editor
{
    public static class EcoGardenAssetMenu
    {
        private const string ItemFolder = "Assets/EcoGarden/ScriptableObjects/Items";
        private const string ProducerFolder = "Assets/EcoGarden/ScriptableObjects/Producers";
        private const string LevelFolder = "Assets/EcoGarden/ScriptableObjects/Levels";

        [MenuItem("Eco Garden/Create Default Data/Level 15 Vertical Slice")]
        public static void CreateLevel15Data()
        {
            EnsureFolder("Assets/EcoGarden");
            EnsureFolder("Assets/EcoGarden/ScriptableObjects");
            EnsureFolder(ItemFolder);
            EnsureFolder(ProducerFolder);
            EnsureFolder(LevelFolder);

            ItemDefinition lv5 = CreateOrLoadItem(
                "item_lotus_lv05_blooming_lotus",
                "lotus",
                5,
                "Blooming Lotus",
                50,
                null);
            ItemDefinition lv4 = CreateOrLoadItem("item_lotus_lv04_flower_bud", "lotus", 4, "Flower Bud", 20, lv5);
            ItemDefinition lv3 = CreateOrLoadItem("item_lotus_lv03_baby_leaf", "lotus", 3, "Baby Leaf", 8, lv4);
            ItemDefinition lv2 = CreateOrLoadItem("item_lotus_lv02_sprout", "lotus", 2, "Sprout", 3, lv3);
            ItemDefinition lv1 = CreateOrLoadItem("item_lotus_lv01_dried_seed", "lotus", 1, "Dried Seed", 1, lv2);

            ProducerDefinition producer = CreateOrLoadProducer("producer_lotus_seed_01", lv1);

            LevelDefinition level = CreateOrLoadAsset<LevelDefinition>(LevelFolder + "/level_015_lotus_pond_corner.asset");
            level.EditorSetValues(
                15,
                "The Lotus Pond Corner",
                8,
                8,
                new[]
                {
                    "LL----LL",
                    "L--21--L",
                    "--W--W--",
                    "S-PPPP-N",
                    "--PPPP--",
                    "--W--W--",
                    "L--11--L",
                    "LL----LL"
                },
                producer,
                new List<ItemDefinition> { lv1, lv2, lv3, lv4, lv5 },
                new NpcOrderDefinition("lotus", 5, 1),
                new List<AbilityCountDefinition>
                {
                    new AbilityCountDefinition(AbilityKind.Shovel, 2),
                    new AbilityCountDefinition(AbilityKind.MagicWand, 1),
                    new AbilityCountDefinition(AbilityKind.SortingMagnet, 1)
                },
                180f,
                "pastel_zen");

            EditorUtility.SetDirty(level);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = level;
        }

        private static ItemDefinition CreateOrLoadItem(
            string itemId,
            string familyId,
            int level,
            string displayName,
            int sellValue,
            ItemDefinition nextItem)
        {
            string path = ItemFolder + "/" + itemId + ".asset";
            ItemDefinition item = CreateOrLoadAsset<ItemDefinition>(path);
            item.EditorSetValues(itemId, familyId, level, displayName, sellValue, nextItem);
            EditorUtility.SetDirty(item);
            return item;
        }

        private static ProducerDefinition CreateOrLoadProducer(string producerId, ItemDefinition spawnItem)
        {
            string path = ProducerFolder + "/" + producerId + ".asset";
            ProducerDefinition producer = CreateOrLoadAsset<ProducerDefinition>(path);
            producer.EditorSetValues(producerId, spawnItem, 1f, 0);
            EditorUtility.SetDirty(producer);
            return producer;
        }

        private static T CreateOrLoadAsset<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            int slashIndex = path.LastIndexOf('/');
            string parent = path.Substring(0, slashIndex);
            string folderName = path.Substring(slashIndex + 1);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
