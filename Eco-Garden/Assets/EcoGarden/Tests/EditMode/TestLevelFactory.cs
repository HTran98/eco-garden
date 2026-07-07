using System.Collections.Generic;
using EcoGarden.Abilities;
using EcoGarden.Config;
using UnityEngine;

namespace EcoGarden.Tests
{
    public static class TestLevelFactory
    {
        public static LevelDefinition CreateLevel15()
        {
            ItemDefinition lv5 = CreateItem("item_lotus_lv05_blooming_lotus", 5, "Blooming Lotus", 50, null);
            ItemDefinition lv4 = CreateItem("item_lotus_lv04_flower_bud", 4, "Flower Bud", 20, lv5);
            ItemDefinition lv3 = CreateItem("item_lotus_lv03_baby_leaf", 3, "Baby Leaf", 8, lv4);
            ItemDefinition lv2 = CreateItem("item_lotus_lv02_sprout", 2, "Sprout", 3, lv3);
            ItemDefinition lv1 = CreateItem("item_lotus_lv01_dried_seed", 1, "Dried Seed", 1, lv2);

            ProducerDefinition producer = ScriptableObject.CreateInstance<ProducerDefinition>();
            producer.EditorSetValues("producer_lotus_seed_01", lv1, 1f, 0);

            LevelDefinition level = ScriptableObject.CreateInstance<LevelDefinition>();
            level.EditorSetValues(
                15,
                "The Lotus Pond Corner",
                5,
                5,
                new[]
                {
                    "LL-LL",
                    "-221-",
                    "W-S-W",
                    "-111-",
                    "LL-LL"
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
                "pastel_zen",
                null,
                new DifficultyDefinition(DifficultyKind.Hard, 2, 8, 1, 5, 0.75f, 2f),
                new List<TemporaryLockDefinition>
                {
                    new TemporaryLockDefinition(1, 3, TemporaryLockUnlockTrigger.OrderCompleted, "lotus_lv05_x1")
                });

            return level;
        }

        public static LevelDefinition CreateLevelWithRows(string[] rows)
        {
            LevelDefinition level = CreateLevel15();
            level.EditorSetValues(
                level.LevelId,
                level.LevelName,
                rows != null && rows.Length > 0 ? rows[0].Length : 0,
                rows != null ? rows.Length : 0,
                rows,
                level.DefaultProducer,
                new List<ItemDefinition>
                {
                    level.GetItemDefinitionForLevel(1),
                    level.GetItemDefinitionForLevel(2),
                    level.GetItemDefinitionForLevel(3),
                    level.GetItemDefinitionForLevel(4),
                    level.GetItemDefinitionForLevel(5)
                },
                level.NpcOrder,
                new List<AbilityCountDefinition>
                {
                    new AbilityCountDefinition(AbilityKind.Shovel, 2),
                    new AbilityCountDefinition(AbilityKind.MagicWand, 1),
                    new AbilityCountDefinition(AbilityKind.SortingMagnet, 1)
                },
                level.TimerSeconds,
                level.ThemeId,
                null,
                level.Difficulty,
                new List<TemporaryLockDefinition>(level.TemporaryLocks));
            return level;
        }

        private static ItemDefinition CreateItem(string id, int level, string displayName, int sellValue, ItemDefinition next)
        {
            ItemDefinition item = ScriptableObject.CreateInstance<ItemDefinition>();
            item.EditorSetValues(id, "lotus", level, displayName, sellValue, next);
            return item;
        }
    }
}
