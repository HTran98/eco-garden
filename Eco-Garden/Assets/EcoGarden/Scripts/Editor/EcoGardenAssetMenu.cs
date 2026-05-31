using System.Collections.Generic;
using EcoGarden.Abilities;
using EcoGarden.Config;
using EcoGarden.Economy;
using EcoGarden.Missions;
using EcoGarden.Progression;
using EcoGarden.Rewards;
using EcoGarden.Shop;
using UnityEditor;
using UnityEngine;

namespace EcoGarden.Editor
{
    public static class EcoGardenAssetMenu
    {
        private const string ItemFolder = "Assets/EcoGarden/ScriptableObjects/Items";
        private const string ProducerFolder = "Assets/EcoGarden/ScriptableObjects/Producers";
        private const string LevelFolder = "Assets/EcoGarden/ScriptableObjects/Levels";
        private const string FirstReleaseLevelCatalogPath = LevelFolder + "/first_release_level_catalog.asset";
        private const string ShopFolder = "Assets/EcoGarden/ScriptableObjects/Shop";
        private const string MissionFolder = "Assets/EcoGarden/ScriptableObjects/Missions";

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
                    "S-PPPP--",
                    "--PPPP--",
                    "--W--W--",
                    "L--11--L",
                    "LL----LL"
                },
                producer,
                new List<ItemDefinition> { lv1, lv2, lv3, lv4, lv5 },
                new NpcOrderDefinition(
                    "lotus_lv05_order",
                    "Blooming Lotus",
                    new[] { new OrderRequirementDefinition("lotus", 5, 1) },
                    new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gold, 100) }, null)),
                new List<AbilityCountDefinition>
                {
                    new AbilityCountDefinition(AbilityKind.Shovel, 2),
                    new AbilityCountDefinition(AbilityKind.MagicWand, 1),
                    new AbilityCountDefinition(AbilityKind.SortingMagnet, 1)
                },
                180f,
                "pastel_zen",
                new List<PlantTierUnlockDefinition>
                {
                    new PlantTierUnlockDefinition("lotus", 4),
                    new PlantTierUnlockDefinition("lotus", 5)
                },
                new DifficultyDefinition(
                    DifficultyKind.Hard,
                    8,
                    4,
                    1,
                    5,
                    0.75f,
                    2f,
                    "Level 15 vertical slice: high-tier order, clustered blockers, and limited board space."),
                new List<TemporaryLockDefinition>
                {
                    new TemporaryLockDefinition(1, 6, TemporaryLockUnlockTrigger.OrderCompleted, "lotus_lv05_order")
                });

            EditorUtility.SetDirty(level);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = level;
        }

        [MenuItem("Eco Garden/Create Default Data/First Release Level Set")]
        public static void CreateFirstReleaseLevelSetData()
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
            List<ItemDefinition> lotusItems = new List<ItemDefinition> { lv1, lv2, lv3, lv4, lv5 };
            ProducerDefinition producer = CreateOrLoadProducer("producer_lotus_seed_01", lv1);

            CreateOrUpdateReleaseLevel(
                1,
                "First Sprouts",
                "level_001_first_sprouts.asset",
                new[]
                {
                    "--------",
                    "--PPPP--",
                    "--PPPP--",
                    "--PSPP--",
                    "--PPPP--",
                    "--PPPP--",
                    "--------",
                    "--------"
                },
                producer,
                lotusItems,
                CreateOrder("lotus_lv02_x1", "Sprout", new[] { new OrderRequirementDefinition("lotus", 2, 1) }, 25),
                CreateAbilities((AbilityKind.Shovel, 1)),
                240f,
                DifficultyKind.Easy,
                1f,
                1f,
                "Teach producer tap, drag, and merge.");

            CreateOrUpdateReleaseLevel(
                2,
                "Tidy Pond Edge",
                "level_002_tidy_pond_edge.asset",
                new[]
                {
                    "--------",
                    "--PPPP--",
                    "--P1PP--",
                    "--PSPP--",
                    "--PPWP--",
                    "--PPPP--",
                    "--------",
                    "--------"
                },
                producer,
                lotusItems,
                CreateOrder("lotus_lv02_x2", "Sprouts", new[] { new OrderRequirementDefinition("lotus", 2, 2) }, 35),
                CreateAbilities((AbilityKind.Shovel, 1)),
                230f,
                DifficultyKind.Easy,
                1f,
                1.1f,
                "Introduce selling spare items and light cleanup.");

            CreateOrUpdateReleaseLevel(
                3,
                "Young Leaves",
                "level_003_young_leaves.asset",
                new[]
                {
                    "--------",
                    "-PPPPP--",
                    "-P1PPP--",
                    "-PPSPP--",
                    "-PPP1P--",
                    "-PPPPP--",
                    "--------",
                    "--------"
                },
                producer,
                lotusItems,
                CreateOrder("lotus_lv03_x1", "Baby Leaf", new[] { new OrderRequirementDefinition("lotus", 3, 1) }, 45),
                CreateAbilities((AbilityKind.MagicWand, 1)),
                220f,
                DifficultyKind.Easy,
                1.05f,
                1.2f,
                "Teach reaching Lv3 and optional booster targeting.");

            CreateOrUpdateReleaseLevel(
                4,
                "Weed Patch",
                "level_004_weed_patch.asset",
                new[]
                {
                    "---LL---",
                    "--PPPP--",
                    "-PWPPW--",
                    "-PPSPP--",
                    "-PWPPW--",
                    "--PPPP--",
                    "---LL---",
                    "--------"
                },
                producer,
                lotusItems,
                CreateOrder("lotus_lv03_x2", "Baby Leaves", new[] { new OrderRequirementDefinition("lotus", 3, 2) }, 60),
                CreateAbilities((AbilityKind.Shovel, 2)),
                210f,
                DifficultyKind.Normal,
                1.15f,
                1.35f,
                "Add moderate obstacle pressure.");

            CreateOrUpdateReleaseLevel(
                5,
                "Visitor Request",
                "level_005_visitor_request.asset",
                new[]
                {
                    "--LLLL--",
                    "-PPPPPP-",
                    "-P1WPPP-",
                    "-PPSPPP-",
                    "-PPPW1P-",
                    "-PPPPPP-",
                    "--LLLL--",
                    "--------"
                },
                producer,
                lotusItems,
                CreateOrder(
                    "lotus_mixed_lv02_lv03",
                    "Sprout and Baby Leaf",
                    new[]
                    {
                        new OrderRequirementDefinition("lotus", 2, 1),
                        new OrderRequirementDefinition("lotus", 3, 1)
                    },
                    70),
                CreateAbilities((AbilityKind.SortingMagnet, 1)),
                210f,
                DifficultyKind.Normal,
                1.2f,
                1.45f,
                "Introduce multi-requirement order flow.");

            CreateOrUpdateReleaseLevel(
                6,
                "Narrow Channels",
                "level_006_narrow_channels.asset",
                new[]
                {
                    "LL----LL",
                    "L-PPPP-L",
                    "--PWWP--",
                    "--PSPP--",
                    "--PPWW--",
                    "L-PPPP-L",
                    "LL----LL",
                    "--------"
                },
                producer,
                lotusItems,
                CreateOrder("lotus_lv03_x2_narrow", "Baby Leaves", new[] { new OrderRequirementDefinition("lotus", 3, 2) }, 80),
                CreateAbilities((AbilityKind.Shovel, 1), (AbilityKind.MagicWand, 1)),
                200f,
                DifficultyKind.Normal,
                1.3f,
                1.55f,
                "Teach planning with fewer central cells.");

            CreateOrUpdateReleaseLevel(
                7,
                "Bud Unlock",
                "level_007_bud_unlock.asset",
                new[]
                {
                    "LL----LL",
                    "L-PPPP-L",
                    "--W11W--",
                    "--PSPP--",
                    "--PPPP--",
                    "--WPPW--",
                    "L-PPPP-L",
                    "LL----LL"
                },
                producer,
                lotusItems,
                CreateOrder("lotus_lv04_x1_intro", "Flower Bud", new[] { new OrderRequirementDefinition("lotus", 4, 1) }, 100),
                CreateAbilities(),
                200f,
                DifficultyKind.Normal,
                1.35f,
                1.75f,
                "First Lv4 order with a level-scoped tier unlock.",
                CreateTemporaryUnlocks(4));

            CreateOrUpdateReleaseLevel(
                8,
                "Busy Crossing",
                "level_008_busy_crossing.asset",
                new[]
                {
                    "LL----LL",
                    "L-PPPP-L",
                    "--W2PW--",
                    "--PSPP--",
                    "--PPW2--",
                    "--WPPP--",
                    "L-PPPP-L",
                    "LL----LL"
                },
                producer,
                lotusItems,
                CreateOrder(
                    "lotus_mixed_lv03_lv04",
                    "Baby Leaves and Bud",
                    new[]
                    {
                        new OrderRequirementDefinition("lotus", 3, 2),
                        new OrderRequirementDefinition("lotus", 4, 1)
                    },
                    125),
                CreateAbilities((AbilityKind.SortingMagnet, 1)),
                190f,
                DifficultyKind.Hard,
                1.5f,
                2f,
                "Mix low and high tier deliveries.",
                CreateTemporaryUnlocks(4));

            CreateOrUpdateReleaseLevel(
                9,
                "Bloom Prep",
                "level_009_bloom_prep.asset",
                new[]
                {
                    "LL----LL",
                    "L--PP--L",
                    "--W22W--",
                    "--PSPP--",
                    "--PPPW--",
                    "--WPPP--",
                    "L--PP--L",
                    "LL----LL"
                },
                producer,
                lotusItems,
                CreateOrder(
                    "lotus_lv04_x2",
                    "Flower Buds",
                    new[] { new OrderRequirementDefinition("lotus", 4, 2) },
                    new RewardDefinition(
                        new[] { new CurrencyReward(CurrencyKind.Gold, 145) },
                        new[] { new AbilityReward(AbilityKind.Shovel, 1) })),
                CreateAbilities(),
                185f,
                DifficultyKind.Hard,
                1.6f,
                2.15f,
                "High-tier quantity pressure.",
                CreateTemporaryUnlocks(4));

            CreateOrUpdateReleaseLevel(
                10,
                "First Bloom",
                "level_010_first_bloom.asset",
                new[]
                {
                    "LL----LL",
                    "L--21--L",
                    "--W--W--",
                    "S-PPPP--",
                    "--PPPP--",
                    "--W--W--",
                    "L--11--L",
                    "LL----LL"
                },
                producer,
                lotusItems,
                CreateOrder(
                    "lotus_lv05_x1_first_bloom",
                    "Blooming Lotus",
                    new[] { new OrderRequirementDefinition("lotus", 5, 1) },
                    new RewardDefinition(new[]
                    {
                        new CurrencyReward(CurrencyKind.Gold, 180),
                        new CurrencyReward(CurrencyKind.Gem, 3)
                    }, null)),
                CreateAbilities((AbilityKind.Shovel, 2), (AbilityKind.MagicWand, 1)),
                180f,
                DifficultyKind.Hard,
                1.7f,
                2.4f,
                "First Lv5 milestone and premium-currency teaser.",
                CreateTemporaryUnlocks(4, 5));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Eco Garden/Create Default Data/First Release Level Catalog")]
        public static void CreateFirstReleaseLevelCatalogData()
        {
            EnsureFolder("Assets/EcoGarden");
            EnsureFolder("Assets/EcoGarden/ScriptableObjects");
            EnsureFolder(LevelFolder);

            LevelCatalogDefinition catalog = CreateOrLoadAsset<LevelCatalogDefinition>(FirstReleaseLevelCatalogPath);
            catalog.EditorSetValues(
                "first_release_levels",
                "First Release Levels",
                LoadFirstReleaseLevels());

            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = catalog;
        }

        [MenuItem("Eco Garden/Create Default Data/Shop Catalog")]
        public static void CreateShopCatalogData()
        {
            EnsureFolder("Assets/EcoGarden");
            EnsureFolder("Assets/EcoGarden/ScriptableObjects");
            EnsureFolder(ShopFolder);

            CreateOrUpdateShopItem(
                "shop_booster_shovel_small",
                "Small Shovel Pack",
                "Adds shovel boosters.",
                ShopItemCategory.Booster,
                new ShopPriceDefinition(ShopPurchaseKind.Gold, 120),
                new RewardDefinition(null, new[] { new AbilityReward(AbilityKind.Shovel, 3) }),
                true);
            CreateOrUpdateShopItem(
                "shop_booster_wand_small",
                "Small Magic Wand Pack",
                "Adds magic wand boosters.",
                ShopItemCategory.Booster,
                new ShopPriceDefinition(ShopPurchaseKind.Gold, 160),
                new RewardDefinition(null, new[] { new AbilityReward(AbilityKind.MagicWand, 2) }),
                true);
            CreateOrUpdateShopItem(
                "shop_booster_magnet_small",
                "Small Sorting Magnet Pack",
                "Adds sorting magnet boosters.",
                ShopItemCategory.Booster,
                new ShopPriceDefinition(ShopPurchaseKind.Gold, 140),
                new RewardDefinition(null, new[] { new AbilityReward(AbilityKind.SortingMagnet, 2) }),
                true);
            CreateOrUpdateShopItem(
                "shop_bundle_boosters_premium",
                "Premium Booster Bundle",
                "Adds all booster types.",
                ShopItemCategory.Bundle,
                new ShopPriceDefinition(ShopPurchaseKind.Gem, 35),
                new RewardDefinition(null, new[]
                {
                    new AbilityReward(AbilityKind.Shovel, 5),
                    new AbilityReward(AbilityKind.MagicWand, 4),
                    new AbilityReward(AbilityKind.SortingMagnet, 4)
                }),
                true);
            CreateOrUpdateShopItem(
                "shop_deco_butterfly",
                "Butterfly Decoration",
                "Unlocks a butterfly cosmetic variant.",
                ShopItemCategory.Decoration,
                new ShopPriceDefinition(ShopPurchaseKind.Gold, 250),
                new RewardDefinition(null, null, new[] { "deco_butterfly_variant" }),
                false);
            CreateOrUpdateShopItem(
                "shop_deco_bird_visitor",
                "Bee Visitor Decoration",
                "Unlocks an ambient bee visitor.",
                ShopItemCategory.Decoration,
                new ShopPriceDefinition(ShopPurchaseKind.Gem, 20),
                new RewardDefinition(null, null, new[] { "deco_bee_visitor" }),
                false);
            CreateOrUpdateShopItem(
                "shop_deco_board_moss_stone",
                "Board Skin: Moss Stone",
                "Unlocks the Moss Stone board skin.",
                ShopItemCategory.Decoration,
                new ShopPriceDefinition(ShopPurchaseKind.Gem, 45),
                new RewardDefinition(null, null, new[] { "skin_board_moss_stone" }),
                false);
            CreateOrUpdateShopItem(
                "shop_deco_npc_traveler",
                "NPC Skin: Traveler",
                "Unlocks the Traveler NPC skin.",
                ShopItemCategory.Decoration,
                new ShopPriceDefinition(ShopPurchaseKind.Gem, 40),
                new RewardDefinition(null, null, new[] { "skin_npc_traveler" }),
                false);
            CreateOrUpdateShopItem(
                "shop_deco_background_lily_pond",
                "Background: Sunset Pond",
                "Unlocks a warm lily pond background.",
                ShopItemCategory.Decoration,
                new ShopPriceDefinition(ShopPurchaseKind.Gem, 35),
                new RewardDefinition(null, null, new[] { "skin_background_lily_pond" }),
                false);
            CreateOrUpdateShopItem(
                "shop_unlock_lotus_tier_4",
                "Unlock Lotus Tier 4",
                "Allows Lotus Lv4 creation and orders.",
                ShopItemCategory.Unlock,
                new ShopPriceDefinition(ShopPurchaseKind.Gold, 600),
                new RewardDefinition(null, null, null, new[] { new PlantTierUnlockReward("lotus", 4) }),
                false);
            CreateOrUpdateShopItem(
                "shop_unlock_lotus_tier_5",
                "Unlock Lotus Tier 5",
                "Allows Lotus Lv5 creation and orders.",
                ShopItemCategory.Unlock,
                new ShopPriceDefinition(ShopPurchaseKind.Gem, 60),
                new RewardDefinition(null, null, null, new[] { new PlantTierUnlockReward("lotus", 5) }),
                false);
            CreateOrUpdateShopItem(
                "shop_iap_gems_small",
                "Small Gem Pack",
                "Adds a small Gem pack.",
                ShopItemCategory.Currency,
                new ShopPriceDefinition(ShopPurchaseKind.Iap, 0, "eco_garden_gems_small"),
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 80) }, null),
                true);
            CreateOrUpdateShopItem(
                "shop_iap_gems_medium",
                "Medium Gem Pack",
                "Adds a medium Gem pack.",
                ShopItemCategory.Currency,
                new ShopPriceDefinition(ShopPurchaseKind.Iap, 0, "eco_garden_gems_medium"),
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gem, 220) }, null),
                true);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Eco Garden/Create Default Data/Missions")]
        public static void CreateMissionData()
        {
            EnsureFolder("Assets/EcoGarden");
            EnsureFolder("Assets/EcoGarden/ScriptableObjects");
            EnsureFolder(MissionFolder);

            CreateOrUpdateMission(
                "mission_merge_lotus",
                "Merge Lotus",
                "Merge lotus plants.",
                MissionType.Merge,
                DifficultyKind.Normal,
                "lotus",
                0,
                AbilityKind.Shovel,
                5,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gold, 80) }, null),
                false,
                10);
            CreateOrUpdateMission(
                "mission_grow_seeds",
                "Grow Seeds",
                "Create dried seed plants.",
                MissionType.Produce,
                DifficultyKind.Easy,
                "lotus",
                1,
                AbilityKind.Shovel,
                10,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gold, 60) }, null),
                false,
                20);
            CreateOrUpdateMission(
                "mission_clear_space",
                "Clear Space",
                "Sell plants to clear board space.",
                MissionType.Sell,
                DifficultyKind.Easy,
                string.Empty,
                0,
                AbilityKind.Shovel,
                3,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gold, 75) }, null),
                false,
                30);
            CreateOrUpdateMission(
                "mission_finish_order",
                "Finish Customer Order",
                "Deliver the requested lotus plants.",
                MissionType.Deliver,
                DifficultyKind.Normal,
                "lotus",
                2,
                AbilityKind.Shovel,
                2,
                new RewardDefinition(
                    new[] { new CurrencyReward(CurrencyKind.Gold, 120) },
                    new[] { new AbilityReward(AbilityKind.Shovel, 1) }),
                false,
                40);
            CreateOrUpdateMission(
                "mission_high_tier_order",
                "High-Tier Lotus Order",
                "Deliver a blooming lotus.",
                MissionType.Deliver,
                DifficultyKind.Hard,
                "lotus",
                5,
                AbilityKind.Shovel,
                1,
                new RewardDefinition(new[]
                {
                    new CurrencyReward(CurrencyKind.Gold, 250),
                    new CurrencyReward(CurrencyKind.Gem, 3)
                }, null),
                false,
                50);
            CreateOrUpdateMission(
                "mission_use_tools",
                "Use Garden Tools",
                "Use shovel boosters.",
                MissionType.UseAbility,
                DifficultyKind.Normal,
                string.Empty,
                0,
                AbilityKind.Shovel,
                2,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gold, 70) }, null),
                false,
                60);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateOrUpdateReleaseLevel(
            int levelId,
            string levelName,
            string fileName,
            string[] rows,
            ProducerDefinition producer,
            List<ItemDefinition> items,
            NpcOrderDefinition order,
            List<AbilityCountDefinition> abilities,
            float timerSeconds,
            DifficultyKind difficultyKind,
            float timerPressureMultiplier,
            float rewardMultiplier,
            string notes,
            List<PlantTierUnlockDefinition> temporaryUnlocks = null)
        {
            LevelDefinition level = CreateOrLoadAsset<LevelDefinition>(LevelFolder + "/" + fileName);
            level.EditorSetValues(
                levelId,
                levelName,
                8,
                8,
                rows,
                producer,
                new List<ItemDefinition>(items),
                order,
                abilities,
                timerSeconds,
                "pastel_zen",
                temporaryUnlocks,
                new DifficultyDefinition(
                    difficultyKind,
                    CountToken(rows, 'W'),
                    CountToken(rows, 'L'),
                    0,
                    order != null ? order.ComplexityScore : 0,
                    timerPressureMultiplier,
                    rewardMultiplier,
                    notes));
            EditorUtility.SetDirty(level);
        }

        private static List<LevelDefinition> LoadFirstReleaseLevels()
        {
            List<LevelDefinition> levels = new List<LevelDefinition>();
            string[] fileNames =
            {
                "level_001_first_sprouts.asset",
                "level_002_tidy_pond_edge.asset",
                "level_003_young_leaves.asset",
                "level_004_weed_patch.asset",
                "level_005_visitor_request.asset",
                "level_006_narrow_channels.asset",
                "level_007_bud_unlock.asset",
                "level_008_busy_crossing.asset",
                "level_009_bloom_prep.asset",
                "level_010_first_bloom.asset"
            };

            for (int i = 0; i < fileNames.Length; i++)
            {
                LevelDefinition level = AssetDatabase.LoadAssetAtPath<LevelDefinition>(LevelFolder + "/" + fileNames[i]);
                if (level != null)
                {
                    levels.Add(level);
                }
            }

            return levels;
        }

        private static NpcOrderDefinition CreateOrder(
            string orderId,
            string displayName,
            OrderRequirementDefinition[] requirements,
            int goldReward)
        {
            return CreateOrder(
                orderId,
                displayName,
                requirements,
                new RewardDefinition(new[] { new CurrencyReward(CurrencyKind.Gold, goldReward) }, null));
        }

        private static NpcOrderDefinition CreateOrder(
            string orderId,
            string displayName,
            OrderRequirementDefinition[] requirements,
            RewardDefinition reward)
        {
            return new NpcOrderDefinition(orderId, displayName, requirements, reward);
        }

        private static List<AbilityCountDefinition> CreateAbilities(params (AbilityKind abilityKind, int count)[] abilities)
        {
            List<AbilityCountDefinition> result = new List<AbilityCountDefinition>();
            for (int i = 0; i < abilities.Length; i++)
            {
                result.Add(new AbilityCountDefinition(abilities[i].abilityKind, abilities[i].count));
            }

            return result;
        }

        private static List<PlantTierUnlockDefinition> CreateTemporaryUnlocks(params int[] tiers)
        {
            List<PlantTierUnlockDefinition> result = new List<PlantTierUnlockDefinition>();
            for (int i = 0; i < tiers.Length; i++)
            {
                result.Add(new PlantTierUnlockDefinition("lotus", tiers[i]));
            }

            return result;
        }

        private static int CountToken(string[] rows, char token)
        {
            int count = 0;
            for (int y = 0; y < rows.Length; y++)
            {
                string row = rows[y];
                for (int x = 0; x < row.Length; x++)
                {
                    if (row[x] == token)
                    {
                        count++;
                    }
                }
            }

            return count;
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

        private static void CreateOrUpdateShopItem(
            string productId,
            string displayName,
            string description,
            ShopItemCategory category,
            ShopPriceDefinition price,
            RewardDefinition grant,
            bool repeatable)
        {
            string path = ShopFolder + "/" + productId + ".asset";
            ShopItemDefinition item = CreateOrLoadAsset<ShopItemDefinition>(path);
            item.EditorSetValues(productId, displayName, description, category, price, grant, repeatable);
            EditorUtility.SetDirty(item);
        }

        private static void CreateOrUpdateMission(
            string missionId,
            string displayName,
            string description,
            MissionType missionType,
            DifficultyKind difficulty,
            string targetFamilyId,
            int targetItemLevel,
            AbilityKind targetAbility,
            int requiredCount,
            RewardDefinition reward,
            bool isDaily,
            int sortOrder)
        {
            string path = MissionFolder + "/" + missionId + ".asset";
            MissionDefinition mission = CreateOrLoadAsset<MissionDefinition>(path);
            mission.EditorSetValues(
                missionId,
                displayName,
                description,
                missionType,
                difficulty,
                targetFamilyId,
                targetItemLevel,
                targetAbility,
                requiredCount,
                reward,
                isDaily,
                sortOrder);
            EditorUtility.SetDirty(mission);
        }
    }
}
