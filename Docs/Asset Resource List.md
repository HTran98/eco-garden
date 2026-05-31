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
- `Runtime symbolic placeholder accepted for first release`: compact text/icon labels are readable enough to ship temporarily until authored UI icons replace them.
- `Needs authored art before release`: current implementation is missing, too generic, or not visually specific enough for release.
- `Deferred from first release`: related feature or cosmetic is not part of the first release scope.

| Asset Id | Type | Target Folder | Status | Notes |
| --- | --- | --- | --- | --- |
| `bg_pond_foggy_01` | Background | `Assets/EcoGarden/Art/Backgrounds/Resources/Backgrounds` | First-pass PNG wired | `EcoGardenBackgroundController` loads the pond/garden background, scales it to the orthographic camera, and keeps it behind gameplay. Needs device contrast validation. |
| `ui_board_backdrop` | UI/world skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | `BoardBackdropController` scales this soft panel behind the board so the pond background does not reduce tile/item contrast. Needs portrait Game view validation. |
| `tile_empty_01` | Board tile | `Assets/EcoGarden/Art/Board` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.EmptyTileSprite` is readable and low-noise; `CellView` adds a subtle runtime shadow for board depth. |
| `tile_locked_01` | Board tile | `Assets/EcoGarden/Art/Board` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.LockedTileSprite` clearly separates blocked cells; `CellView` adds a subtle runtime shadow for board depth. |
| `tile_highlight_valid_01` | Board tile | `Assets/EcoGarden/Art/Board` | Needs authored art before release | Current interaction feedback is functional but lacks a distinct authored valid-target marker. |
| `tile_highlight_invalid_01` | Board tile | `Assets/EcoGarden/Art/Board` | Needs authored art before release | Current interaction feedback is functional but lacks a distinct authored invalid-target marker. |
| `obs_weed_01` | Obstacle | `Assets/EcoGarden/Art/Obstacles` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.WeedSprite` is recognizable at board size. |
| `obs_pebble_01` | Obstacle | `Assets/EcoGarden/Art/Obstacles` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.PebbleSprite` is readable and can represent secondary shovel targets. |
| `producer_lotus_seed_01` | Producer | `Assets/EcoGarden/Art/Producers` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.ProducerSprite` communicates a pond/seed source. |
| `item_lotus_lv01_dried_seed` | Item | `Assets/EcoGarden/Art/Items` | Runtime placeholder accepted for first release | Runtime Lotus Lv1 sprite is distinct from higher tiers; `ItemView` adds a subtle runtime shadow for readability. |
| `item_lotus_lv02_sprout` | Item | `Assets/EcoGarden/Art/Items` | Runtime placeholder accepted for first release | Runtime Lotus Lv2 sprite is distinct from Lv1/Lv3; `ItemView` adds a subtle runtime shadow for readability. |
| `item_lotus_lv03_baby_leaf` | Item | `Assets/EcoGarden/Art/Items` | Runtime placeholder accepted for first release | Runtime Lotus Lv3 sprite is distinct enough for merge progression; `ItemView` adds a subtle runtime shadow for readability. |
| `item_lotus_lv04_flower_bud` | Item | `Assets/EcoGarden/Art/Items` | Runtime placeholder accepted for first release | Runtime Lotus Lv4 bud reads as the pre-bloom tier; `ItemView` adds a subtle runtime shadow for readability. |
| `item_lotus_lv05_blooming_lotus` | Item | `Assets/EcoGarden/Art/Items` | Runtime placeholder accepted for first release | Runtime Lotus Lv5 bloom is the clearest objective item in the current set; `ItemView` adds a subtle runtime shadow for readability. |
| `char_customer_01` | Character | `Assets/EcoGarden/Art/Characters` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.NpcSprite` is simple but usable for order/customer movement. |
| `char_butterfly_a_01` | Character | `Assets/EcoGarden/Art/Characters` | Runtime placeholder accepted for first release | `PlaceholderSpriteFactory.ButterflySprite` supports current ambient pathing. |
| `char_butterfly_b_01` | Character | `Assets/EcoGarden/Art/Characters` | Runtime placeholder accepted for first release | Shares the accepted butterfly runtime sprite until variants are authored. |
| `skin_board_moss_stone` | Decor / board skin | Runtime tint via `DecorationController` | Runtime placeholder accepted for first release | Buying the Moss Stone board skin applies a board/backdrop/background tint. Authored board skin art can replace the tint later. |
| `skin_background_lily_pond` | Decor / background skin | `Assets/EcoGarden/Art/Backgrounds/Resources/Backgrounds/bg_lily_pond_sunset_01.png` | First-pass transparent PNG wired | Buying Sunset Pond changes the gameplay background through the inventory bag. |
| `deco_butterfly_variant` | Decor / butterfly | Runtime tint plus extra ambient butterfly via `DecorationController` | Runtime placeholder accepted for first release | Buying Butterfly Decoration recolors existing butterflies and adds one extra ambient butterfly. |
| `deco_bee_visitor` | Decor / ambient visitor | Runtime placeholder via `DecorationController` | Runtime placeholder accepted for first release | Buying Bee Visitor adds a small ambient bee-style visitor using the current butterfly sprite/tint placeholder. |
| `skin_npc_traveler` | Decor / NPC skin | Runtime tint via `DecorationController` | Runtime placeholder accepted for first release | Buying NPC Traveler changes the customer NPC color. Authored NPC costume art can replace the tint later. |
| `icon_ability_shovel` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon and `AbilityHudController` keeps the count as `xN`. |
| `icon_ability_magic_wand` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon and `AbilityHudController` keeps the count as `xN`. |
| `icon_ability_sorting_magnet` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon and `AbilityHudController` keeps the count as `xN`. |
| `icon_currency_gold` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon and `EconomyController` keeps the numeric value as text. |
| `icon_currency_gem` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon and `EconomyController` keeps the numeric value as text. |
| `icon_timer` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon and `LevelStateController` keeps the timer value as text. |
| `icon_pause` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon at runtime when available. |
| `icon_restart` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon at runtime when available. |
| `icon_next` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon at runtime when available. |
| `icon_nav_shop` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon at runtime when available. |
| `icon_nav_bag` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon for the top-bar Bag button. |
| `icon_nav_mission` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon at runtime when available. |
| `icon_nav_level` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon at runtime when available. |
| `icon_close` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | `HudSkinController` loads the icon for Shop, Mission, and Level close buttons when available. |
| `icon_shop_booster` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | Reserved for shop/inventory booster presentation. |
| `icon_shop_decor` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | Used as the empty-state decor icon in the inventory bag. |
| `icon_decor_board` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | Inventory icon for the board skin decor. |
| `icon_decor_butterfly` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | Inventory icon for the butterfly decor. |
| `icon_decor_bee` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | Inventory icon for the Bee Visitor decor. |
| `icon_decor_npc` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | Inventory icon for the Traveler NPC decor. |
| `icon_decor_background` | UI icon | `Assets/EcoGarden/Art/UI/Resources/UiIcons` | First-pass transparent PNG wired | Inventory/shop icon for the Sunset Pond background decor. |
| `ui_panel_light` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by HUD/panel skin pass when imported; fallback remains procedural panel sprite. |
| `ui_panel_strong` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by AbilityBar and strong panel surfaces. |
| `ui_panel_overlay` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by TopBar, Result, and compact tracker overlay surfaces. |
| `ui_row_light` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by Shop, Mission, Level, and viewport row surfaces. |
| `ui_button_primary` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by runtime/generated buttons. |
| `ui_button_secondary` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by selected Shop category tabs. |
| `ui_button_disabled` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by disabled Shop category tabs and unavailable price fallback. |
| `ui_badge_gold` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by Gold price badges. |
| `ui_badge_gem` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by Gem price badges. |
| `ui_badge_store` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by Store/IAP price badges. |
| `ui_drop_delivery` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by Delivery drop zone when imported. |
| `ui_drop_sell` | UI skin | `Assets/EcoGarden/Art/UI/Resources/UiSkins` | First-pass transparent PNG wired | Used by Sell drop zone when imported. |
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

## UI Visual Direction

First-release UI should use the existing responsive layout and replace only the visual layer where possible.

Style target:

- Cozy garden mobile UI with light leaf-panel surfaces, saturated green primary actions, warm gold secondary accents, and clear purple/blue Store or Gem accents.
- Panels and repeated rows should stay calm and readable; visual detail belongs in icons, badges, drop zones, and reward feedback.
- Runtime symbolic labels remain acceptable during implementation, but authored transparent PNG icons should replace the most visible symbols before final presentation signoff.

Runtime skin baseline:

- `UiThemePalette` centralizes the first visual-skin palette for HUD, panels, buttons, Shop rows, Mission rows, Level Select rows, Delivery, and Sell.
- `HudSkinController` applies the shared palette at runtime so existing scenes can receive the pass without scene regeneration.
- PNG replacement should prefer non-layout assets first: currency icons, top-bar icons, ability icons, result action icons, Delivery/Sell art, and 9-slice panel/button sprites.
- First-pass icon PNGs live under `Assets/EcoGarden/Art/UI/Resources/UiIcons` so Android runtime can load them through `Resources.Load<Sprite>` without scene-specific serialized references.
- First-pass panel/button/row/badge/drop-zone PNGs live under `Assets/EcoGarden/Art/UI/Resources/UiSkins`; runtime code loads them opportunistically and falls back to procedural sprites if Unity has not imported them yet.
- `ui_board_backdrop` uses the same `UiSkins` resource path and is rendered in world space behind the board to preserve board readability over the new background.
