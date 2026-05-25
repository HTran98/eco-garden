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

Acceptance status meanings:

- `Runtime placeholder accepted for first release`: procedural runtime art is readable enough to ship as a temporary first-release asset.
- `Needs authored art before release`: current implementation is missing, too generic, or not visually specific enough for release.
- `Deferred from first release`: related feature or cosmetic is not part of the first release scope.

| Asset Id | Type | Target Folder | Status | Notes |
| --- | --- | --- | --- | --- |
| `bg_pond_foggy_01` | Background | `Assets/EcoGarden/Art/Backgrounds` | Needs authored art before release | No authored or procedural full-scene background exists; board contrast must remain clear. |
| `tile_empty_01` | Board tile | `Assets/EcoGarden/Art/Board` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.EmptyTileSprite` is readable and low-noise. |
| `tile_locked_01` | Board tile | `Assets/EcoGarden/Art/Board` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.LockedTileSprite` clearly separates blocked cells. |
| `tile_highlight_valid_01` | Board tile | `Assets/EcoGarden/Art/Board` | Needs authored art before release | Current interaction feedback is functional but lacks a distinct authored valid-target marker. |
| `tile_highlight_invalid_01` | Board tile | `Assets/EcoGarden/Art/Board` | Needs authored art before release | Current interaction feedback is functional but lacks a distinct authored invalid-target marker. |
| `obs_weed_01` | Obstacle | `Assets/EcoGarden/Art/Obstacles` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.WeedSprite` is recognizable at board size. |
| `obs_pebble_01` | Obstacle | `Assets/EcoGarden/Art/Obstacles` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.PebbleSprite` is readable and can represent secondary shovel targets. |
| `producer_lotus_seed_01` | Producer | `Assets/EcoGarden/Art/Producers` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.ProducerSprite` communicates a pond/seed source. |
| `item_lotus_lv01_dried_seed` | Item | `Assets/EcoGarden/Art/Items` | Runtime placeholder accepted for first release | Runtime Lotus Lv1 sprite is distinct from higher tiers. |
| `item_lotus_lv02_sprout` | Item | `Assets/EcoGarden/Art/Items` | Runtime placeholder accepted for first release | Runtime Lotus Lv2 sprite is distinct from Lv1/Lv3. |
| `item_lotus_lv03_baby_leaf` | Item | `Assets/EcoGarden/Art/Items` | Runtime placeholder accepted for first release | Runtime Lotus Lv3 sprite is distinct enough for merge progression. |
| `item_lotus_lv04_flower_bud` | Item | `Assets/EcoGarden/Art/Items` | Runtime placeholder accepted for first release | Runtime Lotus Lv4 bud reads as the pre-bloom tier. |
| `item_lotus_lv05_blooming_lotus` | Item | `Assets/EcoGarden/Art/Items` | Runtime placeholder accepted for first release | Runtime Lotus Lv5 bloom is the clearest objective item in the current set. |
| `char_customer_01` | Character | `Assets/EcoGarden/Art/Characters` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.NpcSprite` is simple but usable for order/customer movement. |
| `char_butterfly_a_01` | Character | `Assets/EcoGarden/Art/Characters` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.ButterflySprite` supports current ambient pathing. |
| `char_butterfly_b_01` | Character | `Assets/EcoGarden/Art/Characters` | Runtime placeholder accepted for first release | Shares the accepted butterfly runtime sprite until variants are authored. |
| `decor_tree_01` | Decor | `Assets/EcoGarden/Art/Decor` | Deferred from first release | Decoration purchases are hidden until visible cosmetic application exists. |
| `decor_pond_grass_01` | Decor | `Assets/EcoGarden/Art/Decor` | Deferred from first release | Decoration purchases are hidden until visible cosmetic application exists. |
| `decor_stone_small_01` | Decor | `Assets/EcoGarden/Art/Decor` | Deferred from first release | Decoration purchases are hidden until visible cosmetic application exists. |
| `icon_ability_shovel` | UI icon | `Assets/EcoGarden/Art/UI` | Needs authored art before release | Ability buttons currently rely on text/button styling. |
| `icon_ability_magic_wand` | UI icon | `Assets/EcoGarden/Art/UI` | Needs authored art before release | Ability buttons currently rely on text/button styling. |
| `icon_ability_sorting_magnet` | UI icon | `Assets/EcoGarden/Art/UI` | Needs authored art before release | Ability buttons currently rely on text/button styling. |
| `icon_currency_gold` | UI icon | `Assets/EcoGarden/Art/UI` | Needs authored art before release | Currency display works through text but lacks an icon. |
| `icon_timer` | UI icon | `Assets/EcoGarden/Art/UI` | Needs authored art before release | Timer display works through text but lacks an icon. |
| `icon_pause` | UI icon | `Assets/EcoGarden/Art/UI` | Needs authored art before release | Pause uses styled button text; final mobile UI should use a clear icon. |
| `icon_restart` | UI icon | `Assets/EcoGarden/Art/UI` | Needs authored art before release | Result panel restart uses styled button text; final mobile UI should use a clear icon. |
| `vfx_merge_sparkle_01` | VFX sprite | `Assets/EcoGarden/Art/VFX` | Needs authored art before release | Merge feedback exists through runtime feedback hooks, but no dedicated sparkle sprite is authored. |
| `vfx_producer_pulse_01` | VFX sprite | `Assets/EcoGarden/Art/VFX` | Needs authored art before release | Producer feedback is functional but lacks a dedicated pulse sprite. |
| `vfx_ability_burst_01` | VFX sprite | `Assets/EcoGarden/Art/VFX` | Needs authored art before release | Ability feedback is functional but lacks a dedicated burst sprite. |
| `vfx_objective_complete_01` | VFX sprite | `Assets/EcoGarden/Art/VFX` | Needs authored art before release | Delivery/order completion feedback needs final non-blocking VFX. |

## Required Audio Assets

Audio is not release-ready yet. Current gameplay has saved sound/music settings and some SFX hook points, but no committed production audio files in the target folders.

| Asset Id | Type | Target Folder | Status | Notes |
| --- | --- | --- | --- | --- |
| `sfx_item_pickup_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Needs authored audio before release | Item pickup |
| `sfx_item_drop_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Needs authored audio before release | Item drop |
| `sfx_merge_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Needs authored audio before release | Merge chime |
| `sfx_producer_spawn_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Needs authored audio before release | Producer tap/spawn |
| `sfx_ability_use_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Needs authored audio before release | Booster activation |
| `sfx_objective_complete_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Needs authored audio before release | Level complete |
| `sfx_timer_warning_01` | SFX | `Assets/EcoGarden/Audio/SFX` | Needs authored audio before release | Under 20 seconds |
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
