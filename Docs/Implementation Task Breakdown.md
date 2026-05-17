# Eco Garden - Implementation Task Breakdown

Source plan: `Docs/Technical Implementation Plan.md`

## Execution Strategy

Build the vertical slice in thin, verifiable layers:

1. Data and pure gameplay logic first.
2. Board rendering second.
3. Input and interaction third.
4. Objective, timer, and win/fail loop fourth.
5. Polish and Android readiness last.

Each task should end with a visible result, a passing test, or both.

## Phase 0: Project Baseline

### Task 0.1 - Confirm Unity Project State

| Field | Detail |
| --- | --- |
| Goal | Verify the Unity project opens and compiles before new work starts. |
| Files/Area | `Eco-Garden/Assets`, `Eco-Garden/Packages`, `Eco-Garden/ProjectSettings` |
| Dependencies | None |
| Output | Known-good baseline before gameplay implementation. |
| Done When | Unity opens without compile errors and SampleScene can enter Play Mode. |

### Task 0.2 - Create EcoGarden Folder Structure

| Field | Detail |
| --- | --- |
| Goal | Add the planned project folders under `Assets/EcoGarden`. |
| Files/Area | `Eco-Garden/Assets/EcoGarden/...` |
| Dependencies | Task 0.1 |
| Output | Folder structure for scripts, prefabs, configs, tests, art, audio. |
| Done When | All planned folders exist and Unity imports them cleanly. |

### Task 0.3 - Define Asset List and Naming Convention

| Field | Detail |
| --- | --- |
| Goal | Define the required visual/audio resources before placeholder or final asset work begins. |
| Files/Area | `Docs`, `Eco-Garden/Assets/EcoGarden/Art`, `Audio`, `Prefabs` |
| Dependencies | Task 0.2 |
| Output | Asset list grouped by gameplay use, naming convention, import rules, and placeholder/final status. |
| Done When | Every required background, board tile, item, obstacle, character, UI icon, VFX, and SFX has an asset id and target folder. |

Minimum asset list:

| Category | Required Assets |
| --- | --- |
| Background | Foggy pond background, optional parallax pond layer, soft vignette overlay |
| Board tiles | Empty cell, locked cell, valid target highlight, invalid target highlight |
| Obstacles | Weed, pebble |
| Producer | Lotus seed producer idle sprite, cooldown/blocked state if needed |
| Items | Lotus Lv1 Dried Seed, Lv2 Sprout, Lv3 Baby Leaf, Lv4 Flower Bud, Lv5 Blooming Lotus |
| Characters | Customer NPC, Butterfly A, Butterfly B |
| Environment decor | Tree, pond grass, small stones, optional lotus leaves |
| Ability icons | Shovel, Magic Wand, Sorting Magnet |
| UI icons | Gold, timer, pause, restart, objective marker |
| VFX | Merge sparkle, producer pulse, ability burst, objective complete |
| Audio | Pickup/drop, merge, producer spawn, ability use, objective complete, timer warning |

Naming convention:

```text
bg_pond_foggy_01
tile_empty_01
tile_locked_01
tile_highlight_valid_01
tile_highlight_invalid_01
obs_weed_01
obs_pebble_01
producer_lotus_seed_01
item_lotus_lv01_dried_seed
item_lotus_lv02_sprout
item_lotus_lv03_baby_leaf
item_lotus_lv04_flower_bud
item_lotus_lv05_blooming_lotus
char_customer_01
char_butterfly_a_01
char_butterfly_b_01
decor_tree_01
icon_ability_shovel
icon_ability_magic_wand
icon_ability_sorting_magnet
icon_currency_gold
vfx_merge_sparkle_01
sfx_merge_01
```

## Phase 1: Data Model and Level Loading

### Task 1.1 - Add Core Gameplay Types

| Field | Detail |
| --- | --- |
| Goal | Create core enums and plain runtime data structures. |
| Files/Area | `Assets/EcoGarden/Scripts/Board`, `Items`, `Abilities` |
| Dependencies | Task 0.2 |
| Output | `GridPosition`, `CellKind`, `ObstacleKind`, `AbilityKind`, `BoardCell`, `BoardItem`. |
| Done When | Code compiles with no scene dependencies. |

### Task 1.2 - Add ScriptableObject Definitions

| Field | Detail |
| --- | --- |
| Goal | Create authoring assets for items, producers, abilities, and levels. |
| Files/Area | `Assets/EcoGarden/Scripts/Config` |
| Dependencies | Task 1.1 |
| Output | `ItemDefinition`, `ProducerDefinition`, `LevelDefinition`, supporting serializable placement classes. |
| Done When | Unity can create these assets from the Create Asset menu. |

### Task 1.3 - Implement Level Parser

| Field | Detail |
| --- | --- |
| Goal | Convert `LevelDefinition.rowsTopToBottom` into a runtime board. |
| Files/Area | `Assets/EcoGarden/Scripts/Level`, `Board` |
| Dependencies | Task 1.1, Task 1.2 |
| Output | Parser that maps top row to `y = height - 1`, validates width and tokens. |
| Done When | Parser can load Level 15 data and reject malformed data. |

### Task 1.4 - Add EditMode Tests for Parser

| Field | Detail |
| --- | --- |
| Goal | Lock down coordinate and token behavior. |
| Files/Area | `Assets/EcoGarden/Tests/EditMode` |
| Dependencies | Task 1.3 |
| Output | Tests for valid Level 15, invalid token, invalid row width, top-row coordinate mapping. |
| Done When | EditMode tests pass. |

### Task 1.5 - Create Lotus and Level 15 Assets

| Field | Detail |
| --- | --- |
| Goal | Author the first real data assets. |
| Files/Area | `Assets/EcoGarden/ScriptableObjects` |
| Dependencies | Task 1.2, Task 1.3 |
| Output | Lotus Lv1-Lv5 items, producer config, Level 15 config. |
| Done When | Level 15 asset references item and producer definitions correctly. |

## Phase 2: Board Simulation

### Task 2.1 - Implement Board Runtime

| Field | Detail |
| --- | --- |
| Goal | Store board cells and provide fast cell lookup. |
| Files/Area | `Assets/EcoGarden/Scripts/Board` |
| Dependencies | Task 1.3 |
| Output | `BoardState` or equivalent runtime model. |
| Done When | Code can query cells, bounds, occupancy, and playable state. |

### Task 2.2 - Implement Move and Merge Rules

| Field | Detail |
| --- | --- |
| Goal | Support moving items and merging identical pairs. |
| Files/Area | `Assets/EcoGarden/Scripts/Board`, `Items` |
| Dependencies | Task 2.1 |
| Output | `TryMoveItem`, `TryMergeItem`, max-level rejection, invalid target rejection. |
| Done When | Same-level lotus items merge into next level; invalid moves fail without changing state. |

### Task 2.3 - Implement Producer Spawn Logic

| Field | Detail |
| --- | --- |
| Goal | Tap producer to create Lotus Lv1 near the producer. |
| Files/Area | `Assets/EcoGarden/Scripts/Board`, `Level` |
| Dependencies | Task 2.1 |
| Output | `TrySpawnFromProducer`, cooldown state, nearest-empty-cell search. |
| Done When | Spawn succeeds when space exists and fails when the board is full. |

### Task 2.4 - Implement Ability Rules

| Field | Detail |
| --- | --- |
| Goal | Add pure logic for Shovel, Magic Wand, and Sorting Magnet. |
| Files/Area | `Assets/EcoGarden/Scripts/Abilities`, `Board` |
| Dependencies | Task 2.2 |
| Output | `TryUseShovel`, `TryUseMagicWand`, `TryUseSortingMagnet`. |
| Done When | Ability count is consumed only on successful use. |

### Task 2.5 - Add EditMode Tests for Board Rules

| Field | Detail |
| --- | --- |
| Goal | Verify gameplay rules independently of Unity scene rendering. |
| Files/Area | `Assets/EcoGarden/Tests/EditMode` |
| Dependencies | Task 2.2, Task 2.3, Task 2.4 |
| Output | Tests for move, merge, max level, obstacle removal, wand upgrade, magnet movement. |
| Done When | Board rule tests pass. |

## Phase 3: Board Rendering

### Task 3.1 - Create Placeholder Visual Assets

| Field | Detail |
| --- | --- |
| Goal | Provide simple placeholder sprites/colors for all board entities and required UI icons. |
| Files/Area | `Assets/EcoGarden/Art`, `Prefabs` |
| Dependencies | Task 0.3, Task 1.5 |
| Output | Placeholder visuals for background, empty, locked, weed, pebble, producer, NPC, Lotus Lv1-Lv5, abilities, gold, timer, pause/restart. |
| Done When | Every Level 15 token and required HUD icon has a distinct visible representation. |

Placeholder requirements:

| Asset Type | Requirement |
| --- | --- |
| Background | Simple pastel pond image or flat color panel with pond-like shapes |
| Items | Different silhouette/color per lotus level |
| Obstacles | Clearly distinguish weed from pebble |
| Producer | Visually distinct from regular items |
| NPC | Clearly marks the order/delivery point |
| Abilities | Usable temporary icons for Shovel, Magic Wand, Sorting Magnet |
| UI | Gold, timer, pause, restart icons can be simple shapes/text-backed sprites |

Do not block gameplay implementation waiting for final art. Placeholder assets must be easy to replace through sprite references in ScriptableObjects and prefabs.

### Task 3.2 - Implement BoardView

| Field | Detail |
| --- | --- |
| Goal | Render board state into scene objects. |
| Files/Area | `Assets/EcoGarden/Scripts/Board`, `Prefabs/Board`, `Prefabs/Items` |
| Dependencies | Task 2.1, Task 3.1 |
| Output | `BoardView`, `CellView`, `ItemView`, grid-to-world placement. |
| Done When | Level 15 appears as an 8x8 board in Play Mode. |

### Task 3.3 - Add Board Event Updates

| Field | Detail |
| --- | --- |
| Goal | Keep visuals in sync when board state changes. |
| Files/Area | `Assets/EcoGarden/Scripts/Board` |
| Dependencies | Task 3.2, Task 2.2 |
| Output | Spawn, move, merge, remove, and upgrade visual updates. |
| Done When | Board visuals update without rebuilding the whole board every action. |

## Phase 4: Input and Interaction

### Task 4.1 - Implement Pointer Input Abstraction

| Field | Detail |
| --- | --- |
| Goal | Support mouse in editor and touch on Android. |
| Files/Area | `Assets/EcoGarden/Scripts/Input` |
| Dependencies | Task 3.2 |
| Output | Pointer press, drag, release, tap events with world position. |
| Done When | Editor mouse input can select board cells. |

### Task 4.2 - Implement Drag Move and Merge

| Field | Detail |
| --- | --- |
| Goal | Let player drag items between cells. |
| Files/Area | `Assets/EcoGarden/Scripts/Input`, `Board` |
| Dependencies | Task 4.1, Task 3.3, Task 2.2 |
| Output | Drag item view, target detection, valid move, valid merge, invalid return. |
| Done When | Player can move and merge items on Level 15. |

### Task 4.3 - Implement Producer Tap

| Field | Detail |
| --- | --- |
| Goal | Let player tap the producer to spawn Lotus Lv1. |
| Files/Area | `Assets/EcoGarden/Scripts/Input`, `Board` |
| Dependencies | Task 4.1, Task 2.3, Task 3.3 |
| Output | Producer tap handling and cooldown feedback. |
| Done When | Tapping producer spawns a visible item near `(0,4)`. |

### Task 4.4 - Implement Ability Targeting

| Field | Detail |
| --- | --- |
| Goal | Connect booster UI selection to board target actions. |
| Files/Area | `Assets/EcoGarden/Scripts/Abilities`, `Input`, `UI` |
| Dependencies | Task 2.4, Task 4.1 |
| Output | Ability selection state, valid target highlights, ability execution. |
| Done When | Shovel, Magic Wand, and Sorting Magnet can be used from UI. |

### Task 4.5 - Implement External Drop Zone Base

| Field | Detail |
| --- | --- |
| Goal | Add reusable drop targets outside the board for selling, delivery, storage, and future event collection. |
| Files/Area | `Assets/EcoGarden/Scripts/Input`, `Board`, `Economy`, `UI` |
| Dependencies | Task 4.2 |
| Output | External drop zone component, drop type enum, pointer-over-zone detection, drop routing hook. |
| Done When | Drag controller can distinguish board drops from external zone drops. |

### Task 4.6 - Implement Sell Basket

| Field | Detail |
| --- | --- |
| Goal | Let players sell unwanted plant items by dragging them outside the board into a basket. |
| Files/Area | `Assets/EcoGarden/Scripts/Economy`, `Input`, `UI`, `Prefabs/UI` |
| Dependencies | Task 4.5 |
| Output | Sell Basket visual placeholder, sell validation, gold reward, gold UI update. |
| Done When | Dragging an item into Sell Basket removes it from board and increases gold by item sell value. |

Sell Basket behavior:

| Rule | Decision |
| --- | --- |
| Accepted target | Movable plant/lotus item |
| Rejected target | Empty cell, obstacle, producer, NPC, locked cell |
| Reward | `ItemDefinition.sellValue` |
| Invalid drop | Item returns to original board cell |
| Visual feedback | Item flies into basket, gold coin burst plays |

## Phase 5: Objective, Timer, and Game State

### Task 5.1 - Implement Game State Controller

| Field | Detail |
| --- | --- |
| Goal | Coordinate level start, active play, win, fail, and pause states. |
| Files/Area | `Assets/EcoGarden/Scripts/Level` |
| Dependencies | Task 3.2 |
| Output | `GameStateController` or `LevelController`. |
| Done When | Systems can subscribe to level start, complete, fail, and pause events. |

### Task 5.2 - Implement Timer

| Field | Detail |
| --- | --- |
| Goal | Count down Level 15's 180-second limit. |
| Files/Area | `Assets/EcoGarden/Scripts/Level`, `UI` |
| Dependencies | Task 5.1 |
| Output | Timer model and UI display. |
| Done When | Timer reaches zero and triggers fail state. |

### Task 5.3 - Implement NPC Objective

| Field | Detail |
| --- | --- |
| Goal | Complete level when player delivers 1 Lotus Lv5 to NPC. |
| Files/Area | `Assets/EcoGarden/Scripts/AI`, `Level`, `Board` |
| Dependencies | Task 4.2, Task 5.1 |
| Output | NPC order validation and delivery behavior. |
| Done When | Dragging Lotus Lv5 to NPC completes the level. |

### Task 5.4 - Implement Basic HUD

| Field | Detail |
| --- | --- |
| Goal | Show objective, timer, booster counts, gold, pause. |
| Files/Area | `Assets/EcoGarden/Scripts/UI`, `Prefabs/UI` |
| Dependencies | Task 5.2, Task 5.3, Task 4.4 |
| Output | Functional gameplay HUD. |
| Done When | Player can understand objective, remaining time, and available abilities. |

### Task 5.5 - Implement Win/Fail Panels

| Field | Detail |
| --- | --- |
| Goal | Show result state and allow restart. |
| Files/Area | `Assets/EcoGarden/Scripts/UI`, `Prefabs/UI` |
| Dependencies | Task 5.1, Task 5.2, Task 5.3 |
| Output | Completion panel, failure panel, restart button. |
| Done When | Win and fail states are visible and restart works. |

## Phase 6: Cosmetic AI and Feedback

### Task 6.1 - Add NPC Movement

| Field | Detail |
| --- | --- |
| Goal | Move NPC from `(-1,4)` to `(7,4)`, then idle. |
| Files/Area | `Assets/EcoGarden/Scripts/AI`, `Prefabs/Characters` |
| Dependencies | Task 5.3 |
| Output | Simple deterministic NPC movement. |
| Done When | NPC enters, waits, and exits after fulfillment. |

### Task 6.2 - Add Butterfly Cosmetic Movement

| Field | Detail |
| --- | --- |
| Goal | Add non-gameplay ambient motion. |
| Files/Area | `Assets/EcoGarden/Scripts/AI`, `Prefabs/Characters` |
| Dependencies | Task 3.2 |
| Output | Butterfly A loop path and Butterfly B hover behavior. |
| Done When | Butterflies move without affecting board logic. |

### Task 6.3 - Add Gameplay Feedback

| Field | Detail |
| --- | --- |
| Goal | Add visual/audio feedback for core actions. |
| Files/Area | `Assets/EcoGarden/Scripts/UI`, `Audio`, `Vfx` |
| Dependencies | Task 4.2, Task 4.3, Task 4.4 |
| Output | Merge pop, invalid move feedback, producer pulse, ability effect, objective success. |
| Done When | Core actions are readable without debug logs. |

### Task 6.3.1 - Add Sell Coin Feedback

| Field | Detail |
| --- | --- |
| Goal | Add gold coin feedback when an item is sold through Sell Basket. |
| Files/Area | `Assets/EcoGarden/Scripts/UI`, `Vfx`, `Audio`, `Prefabs/UI` |
| Dependencies | Task 4.6, Task 6.3 |
| Output | Coin burst placeholder, gold counter pop, sell SFX hook. |
| Done When | Selling an item clearly communicates gold reward from the basket area. |

### Task 6.4 - Replace Placeholder Art With Production Assets

| Field | Detail |
| --- | --- |
| Goal | Replace temporary visuals with production-ready art while preserving gameplay references and prefab contracts. |
| Files/Area | `Assets/EcoGarden/Art`, `Prefabs`, `ScriptableObjects` |
| Dependencies | Task 6.3 |
| Output | Final or near-final background, tree/decor, lotus items, obstacles, producer, NPC, butterflies, ability icons, UI icons, VFX sprites. |
| Done When | All placeholder art used in the Level 15 vertical slice is replaced or explicitly accepted as final. |

Production asset checklist:

| Category | Done When |
| --- | --- |
| Background | Foggy pond background fits the board and Android portrait framing |
| Tree/decor | Decor supports the Pastel Zen theme without obscuring gameplay |
| Items | Lotus Lv1-Lv5 remain readable at mobile size |
| Obstacles | Weed and pebble are visually distinct and match targeting rules |
| Producer | Producer reads as tappable and different from board items |
| NPC | NPC/order point reads as the delivery target |
| Butterflies | Cosmetic motion assets do not distract from board interactions |
| Ability icons | Icons are recognizable in the booster bar |
| VFX | Merge and ability effects are clear but not visually noisy |

## Phase 7: Persistence and Android Readiness

### Task 7.1 - Implement Basic Save Data

| Field | Detail |
| --- | --- |
| Goal | Save player progression and inventory. |
| Files/Area | `Assets/EcoGarden/Scripts/Save` |
| Dependencies | Task 5.5 |
| Output | Save/load for gold, highest unlocked level, booster counts, settings. |
| Done When | Restarting Play Mode can restore saved test data. |

### Task 7.2 - Android UI Pass

| Field | Detail |
| --- | --- |
| Goal | Ensure portrait mobile layout is usable. |
| Files/Area | `Assets/EcoGarden/Scripts/UI`, `Prefabs/UI` |
| Dependencies | Task 5.4 |
| Output | Responsive HUD for common Android portrait sizes. |
| Done When | UI does not overlap at 720x1280 and 1080x1920. |

### Task 7.3 - Performance and Allocation Pass

| Field | Detail |
| --- | --- |
| Goal | Remove obvious per-interaction allocation and scene searches. |
| Files/Area | Gameplay scripts |
| Dependencies | Task 6.3 |
| Output | Cleaner board lookup, pooled effects if needed, no repeated `FindObjectOfType` in gameplay path. |
| Done When | Gameplay interactions are smooth in editor profiling and no obvious GC spikes occur from dragging. |

### Task 7.4 - Build Verification

| Field | Detail |
| --- | --- |
| Goal | Confirm the vertical slice can build for Android. |
| Files/Area | Unity Build Settings |
| Dependencies | Task 7.2, Task 7.3 |
| Output | Android build attempt and issue list. |
| Done When | Build succeeds or all blocking build errors are documented with fixes. |

## Suggested First Sprint

Start with these tasks only:

1. Task 0.1 - Confirm Unity Project State.
2. Task 0.2 - Create EcoGarden Folder Structure.
3. Task 0.3 - Define Asset List and Naming Convention.
4. Task 1.1 - Add Core Gameplay Types.
5. Task 1.2 - Add ScriptableObject Definitions.
6. Task 1.3 - Implement Level Parser.
7. Task 1.4 - Add EditMode Tests for Parser.
8. Task 1.5 - Create Lotus and Level 15 Assets.

Sprint 1 is complete when Level 15 exists as data and can be parsed into a validated runtime board.

## Task Dependency Summary

```text
0.1 -> 0.2 -> 0.3
0.2 -> 1.1 -> 1.2 -> 1.3 -> 1.4 -> 1.5
1.3 -> 2.1 -> 2.2 -> 2.3 -> 2.4 -> 2.5
0.3 + 1.5 -> 3.1
2.1 + 3.1 -> 3.2 -> 3.3
3.2 -> 4.1 -> 4.2 -> 4.3 -> 4.4
3.2 -> 5.1 -> 5.2 -> 5.3 -> 5.4 -> 5.5
5.3 -> 6.1
3.2 -> 6.2
4.x -> 6.3 -> 6.4
5.5 -> 7.1
5.4 -> 7.2
6.3 -> 7.3 -> 7.4
```

## Definition of Done for the Vertical Slice

The vertical slice is done when:

1. Level 15 loads from data.
2. Board renders correctly.
3. Player can spawn, move, merge, and use abilities.
4. Player can sell unwanted items through Sell Basket.
5. NPC order requires 1 Lotus Lv5.
6. Timer can complete or fail the level.
7. HUD communicates the objective and state.
8. The game can be played with touch-style input.
9. Android build blockers are resolved or explicitly documented.
