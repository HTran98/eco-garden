# Eco Garden - Asset Resource List

Source task: `Docs/Implementation Task Breakdown.md` Task 0.3

## Naming Rules

Use lowercase snake case:

```text
<category>_<subject>_<variant>
```

Examples:

```text
bg_pond_foggy_01
item_lotus_lv01_dried_seed
decor_tree_01
icon_ability_shovel
sfx_merge_01
```

## Required Visual Assets

| Asset Id | Type | Target Folder | Status | Notes |
| --- | --- | --- | --- | --- |
| `bg_pond_foggy_01` | Background | `Assets/EcoGarden/Art/Backgrounds` | Placeholder needed | Main Level 15 background |
| `tile_empty_01` | Board tile | `Assets/EcoGarden/Art/Board` | Placeholder needed | Playable empty cell |
| `tile_locked_01` | Board tile | `Assets/EcoGarden/Art/Board` | Placeholder needed | Locked cell |
| `tile_highlight_valid_01` | Board tile | `Assets/EcoGarden/Art/Board` | Placeholder needed | Valid drag/ability target |
| `tile_highlight_invalid_01` | Board tile | `Assets/EcoGarden/Art/Board` | Placeholder needed | Invalid drag/ability target |
| `obs_weed_01` | Obstacle | `Assets/EcoGarden/Art/Obstacles` | Placeholder needed | Shovel-removable weed |
| `obs_pebble_01` | Obstacle | `Assets/EcoGarden/Art/Obstacles` | Placeholder needed | Shovel-removable pebble |
| `producer_lotus_seed_01` | Producer | `Assets/EcoGarden/Art/Producers` | Placeholder needed | Tappable lotus seed producer |
| `item_lotus_lv01_dried_seed` | Item | `Assets/EcoGarden/Art/Items` | Placeholder needed | Lotus Lv1 |
| `item_lotus_lv02_sprout` | Item | `Assets/EcoGarden/Art/Items` | Placeholder needed | Lotus Lv2 |
| `item_lotus_lv03_baby_leaf` | Item | `Assets/EcoGarden/Art/Items` | Placeholder needed | Lotus Lv3 |
| `item_lotus_lv04_flower_bud` | Item | `Assets/EcoGarden/Art/Items` | Placeholder needed | Lotus Lv4 |
| `item_lotus_lv05_blooming_lotus` | Item | `Assets/EcoGarden/Art/Items` | Placeholder needed | Lotus Lv5 objective item |
| `char_customer_01` | Character | `Assets/EcoGarden/Art/Characters` | Placeholder needed | NPC order customer |
| `char_butterfly_a_01` | Character | `Assets/EcoGarden/Art/Characters` | Placeholder needed | Cosmetic path butterfly |
| `char_butterfly_b_01` | Character | `Assets/EcoGarden/Art/Characters` | Placeholder needed | Cosmetic hover butterfly |
| `decor_tree_01` | Decor | `Assets/EcoGarden/Art/Decor` | Placeholder needed | Pastel Zen tree |
| `decor_pond_grass_01` | Decor | `Assets/EcoGarden/Art/Decor` | Placeholder needed | Pond edge decoration |
| `decor_stone_small_01` | Decor | `Assets/EcoGarden/Art/Decor` | Placeholder needed | Non-blocking decoration |
| `icon_ability_shovel` | UI icon | `Assets/EcoGarden/Art/UI` | Placeholder needed | Booster button |
| `icon_ability_magic_wand` | UI icon | `Assets/EcoGarden/Art/UI` | Placeholder needed | Booster button |
| `icon_ability_sorting_magnet` | UI icon | `Assets/EcoGarden/Art/UI` | Placeholder needed | Booster button |
| `icon_currency_gold` | UI icon | `Assets/EcoGarden/Art/UI` | Placeholder needed | Gold display |
| `icon_timer` | UI icon | `Assets/EcoGarden/Art/UI` | Placeholder needed | Timer display |
| `icon_pause` | UI icon | `Assets/EcoGarden/Art/UI` | Placeholder needed | Pause button |
| `icon_restart` | UI icon | `Assets/EcoGarden/Art/UI` | Placeholder needed | Restart button |
| `vfx_merge_sparkle_01` | VFX sprite | `Assets/EcoGarden/Art/VFX` | Placeholder needed | Merge feedback |
| `vfx_producer_pulse_01` | VFX sprite | `Assets/EcoGarden/Art/VFX` | Placeholder needed | Producer feedback |
| `vfx_ability_burst_01` | VFX sprite | `Assets/EcoGarden/Art/VFX` | Placeholder needed | Ability use feedback |
| `vfx_objective_complete_01` | VFX sprite | `Assets/EcoGarden/Art/VFX` | Placeholder needed | NPC fulfillment feedback |

## Required Audio Assets

| Asset Id | Type | Target Folder | Status | Notes |
| --- | --- | --- | --- | --- |
| `sfx_item_pickup_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Placeholder needed | Item pickup |
| `sfx_item_drop_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Placeholder needed | Item drop |
| `sfx_merge_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Placeholder needed | Merge chime |
| `sfx_producer_spawn_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Placeholder needed | Producer tap/spawn |
| `sfx_ability_use_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Placeholder needed | Booster activation |
| `sfx_objective_complete_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Placeholder needed | Level complete |
| `sfx_timer_warning_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Placeholder needed | Under 20 seconds |
| `music_level_pastel_zen_01` | Music | `Assets/EcoGarden/Audio/Music` | Deferred | Calm looping level music |

## Import Guidelines

| Asset Type | Guideline |
| --- | --- |
| Item sprites | Must remain readable at small mobile size |
| Board tiles | Keep cell edges clear and low noise |
| Background | Must not reduce board contrast |
| UI icons | Prefer strong silhouettes over detail |
| VFX | Short, readable, non-blocking |
| Audio | Soft, low-fatigue, mobile-friendly volume |

