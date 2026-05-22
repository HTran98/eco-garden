# Eco Garden - First Release Level Set Spec

Date created: 2026-05-22

Purpose: define the first releasable progression set before creating additional `LevelDefinition` assets.

## Level Data Conventions

Board rows are 8 characters, top to bottom.

| Symbol | Meaning |
| --- | --- |
| `P` | Playable empty cell |
| `S` | Producer start cell |
| `1`-`5` | Starting Lotus item level |
| `W` | Weed/obstacle |
| `L` | Locked cell |
| `-` | Empty blocked/non-playable space |

Release constraints:

- Levels must remain completable without Gem or IAP.
- Completing the active level order unlocks the next level by saving `highestUnlockedLevel = max(highestUnlockedLevel, currentLevelId + 1)`.
- Lotus Lv1-Lv3 are available by default.
- Lotus Lv4 and Lv5 orders require either saved unlocks or level-scoped `temporaryAllowedPlantTiers`.
- Orders must not request a locked tier unless that level explicitly allows it.
- Gem rewards are rare and reserved for hard/expert content or mission rewards.
- Level 15 remains the current vertical-slice target and can be reused as a later challenge/reference level.

## Progression Arc

| Level | Name | Difficulty | Timer | Order Requirements | Reward Target | Unlocks/Tools | Design Goal |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | First Sprouts | Easy | 240s | Lotus Lv2 x1 | Gold 25 | Shovel x1 | Teach producer tap, drag, and merge. |
| 2 | Tidy Pond Edge | Easy | 230s | Lotus Lv2 x2 | Gold 35 | Shovel x1 | Introduce selling spare items and light cleanup. |
| 3 | Young Leaves | Easy | 220s | Lotus Lv3 x1 | Gold 45 | Magic Wand x1 | Teach reaching Lv3 and optional booster targeting. |
| 4 | Weed Patch | Normal | 210s | Lotus Lv3 x2 | Gold 60 | Shovel x2 | Add moderate obstacle pressure. |
| 5 | Visitor Request | Normal | 210s | Lotus Lv2 x1, Lotus Lv3 x1 | Gold 70 | Magnet x1 | Introduce multi-requirement order flow. |
| 6 | Narrow Channels | Normal | 200s | Lotus Lv3 x2 | Gold 80 | Shovel x1, Wand x1 | Teach planning with fewer central cells. |
| 7 | Bud Unlock | Normal | 200s | Lotus Lv4 x1 | Gold 100 | Temporary Lotus Lv4 | First Lv4 tutorial, no permanent Gem dependency. |
| 8 | Busy Crossing | Hard | 190s | Lotus Lv3 x2, Lotus Lv4 x1 | Gold 125 | Temporary Lotus Lv4, Magnet x1 | Mix low and high tier deliveries. |
| 9 | Bloom Prep | Hard | 185s | Lotus Lv4 x2 | Gold 145, Shovel x1 | Temporary Lotus Lv4 | Increase high-tier quantity pressure. |
| 10 | First Bloom | Hard | 180s | Lotus Lv5 x1 | Gold 180, Gem 3 | Temporary Lotus Lv5, Shovel x2, Wand x1 | First Lv5 milestone and premium-currency teaser. |

## Level Board Specs

### Level 1 - First Sprouts

Rows:

```text
--------
--PPPP--
--PPPP--
--PSPP--
--PPPP--
--PPPP--
--------
--------
```

Order: `lotus_lv02_x1`, Lotus Lv2 x1.

Difficulty notes: no obstacles, loose timer, large central board. The player only needs one merge.

### Level 2 - Tidy Pond Edge

Rows:

```text
--------
--PPPP--
--P1PP--
--PSPP--
--PPWP--
--PPPP--
--------
--------
```

Order: `lotus_lv02_x2`, Lotus Lv2 x2.

Difficulty notes: one weed blocks a convenient path. Selling spare Lv1 plants should be useful but not required.

### Level 3 - Young Leaves

Rows:

```text
--------
-PPPPP--
-P1PPP--
-PPSPP--
-PPP1P--
-PPPPP--
--------
--------
```

Order: `lotus_lv03_x1`, Lotus Lv3 x1.

Difficulty notes: two seeded Lv1 items shorten the path. Magic Wand is available as a recovery/helper action.

### Level 4 - Weed Patch

Rows:

```text
---LL---
--PPPP--
-PWPPW--
-PPSPP--
-PWPPW--
--PPPP--
---LL---
--------
```

Order: `lotus_lv03_x2`, Lotus Lv3 x2.

Difficulty notes: four weeds create routing pressure. Locked corners are visual bounds, not progression blockers.

### Level 5 - Visitor Request

Rows:

```text
--LLLL--
-PPPPPP-
-P1WPPP-
-PPSPPP-
-PPPW1P-
-PPPPPP-
--LLLL--
--------
```

Order: `lotus_mixed_lv02_lv03`, Lotus Lv2 x1 and Lotus Lv3 x1.

Difficulty notes: first multi-requirement order. Objective panel must clearly show partial progress.

### Level 6 - Narrow Channels

Rows:

```text
LL----LL
L-PPPP-L
--PWWP--
--PSPP--
--PPWW--
L-PPPP-L
LL----LL
--------
```

Order: `lotus_lv03_x2_narrow`, Lotus Lv3 x2.

Difficulty notes: fewer central cells and four weeds. Player should learn to sell low-value clutter.

### Level 7 - Bud Unlock

Rows:

```text
LL----LL
L-PPPP-L
--W11W--
--PSPP--
--PPPP--
--WPPW--
L-PPPP-L
LL----LL
```

Order: `lotus_lv04_x1_intro`, Lotus Lv4 x1.

Temporary unlocks: Lotus Lv4.

Difficulty notes: first Lv4 order. This level teaches the unlocked-tier rule without requiring a shop purchase.

### Level 8 - Busy Crossing

Rows:

```text
LL----LL
L-PPPP-L
--W2PW--
--PSPP--
--PPW2--
--WPPP--
L-PPPP-L
LL----LL
```

Order: `lotus_mixed_lv03_lv04`, Lotus Lv3 x2 and Lotus Lv4 x1.

Temporary unlocks: Lotus Lv4.

Difficulty notes: mixed low/high order and seeded Lv2 items. Magnet should help recover matching pairs.

### Level 9 - Bloom Prep

Rows:

```text
LL----LL
L--PP--L
--W22W--
--PSPP--
--PPPW--
--WPPP--
L--PP--L
LL----LL
```

Order: `lotus_lv04_x2`, Lotus Lv4 x2.

Temporary unlocks: Lotus Lv4.

Difficulty notes: high-tier quantity pressure. Timer remains hard but not expert.

### Level 10 - First Bloom

Rows:

```text
LL----LL
L--21--L
--W--W--
S-PPPP--
--PPPP--
--W--W--
L--11--L
LL----LL
```

Order: `lotus_lv05_x1_first_bloom`, Lotus Lv5 x1.

Temporary unlocks: Lotus Lv5.

Difficulty notes: simplified version of current Level 15 vertical slice with lower reward/timer pressure. This becomes the first release milestone; Level 15 can remain a later challenge with higher reward and tighter layout.

## Balance Targets

| Difficulty | Timer Range | Reward Range | Obstacle Count | Locked Count | Highest Tier |
| --- | --- | --- | --- | --- | --- |
| Easy | 220-240s | Gold 25-45 | 0-1 | 0 | Lv3 |
| Normal | 200-210s | Gold 60-100 | 2-4 | 4-8 | Lv4 |
| Hard | 180-190s | Gold 125-180, rare Gem 0-3 | 4-6 | 8+ | Lv5 |

## D2 Asset Creation Notes

- Create level assets as `level_001_first_sprouts.asset` through `level_010_first_bloom.asset`.
- Reuse existing Lotus Lv1-Lv5 item definitions and lotus producer.
- Use `NpcOrderDefinition` multi-requirements for Levels 5 and 8.
- Add `temporaryAllowedPlantTiers` only to Levels 7-10.
- Keep Level 15 asset unchanged until the first 10 levels are created and smoke-tested.

## D2 Generator Status

Editor generator added: `Eco Garden/Create Default Data/First Release Level Set`.

The generator writes `LevelDefinition` assets for Levels 1-10 using the rows, timers, rewards, difficulty labels, order requirements, starting abilities, and temporary tier unlocks defined above.

Current local blocker: Unity batchmode did not reach the execute method because the Unity Licensing Client repeatedly disconnected in this environment. Run the menu item from the Unity Editor, or rerun batchmode after licensing is stable:

```powershell
Unity.exe -batchmode -quit -projectPath D:\Project\Game\Eco-Garden -executeMethod EcoGarden.Editor.EcoGardenAssetMenu.CreateFirstReleaseLevelSetData -logFile D:\Project\Game\CreateFirstReleaseLevelSet.log
```

## D3 Catalog Status

Runtime catalog added: `first_release_level_catalog.asset`.

The catalog references Levels 1-10 in ascending `levelId` order and is backed by `LevelCatalogDefinition` plus `LevelCatalogService`. The service ignores null/duplicate level entries, keeps levels sorted by `LevelId`, and can resolve the highest unlocked level from save data.

Editor generator added: `Eco Garden/Create Default Data/First Release Level Catalog`.

Scene loader support added: `LevelCatalogController`.

Use `Eco Garden/Fix Scene/Add First Release Level Loader` on an existing scene to attach the controller to `GameRoot` and assign `first_release_level_catalog.asset`. On Awake, the controller reads save data and assigns the highest unlocked level to `BoardController` before the board loads.

## Level Completion Flow

- Delivering all required order items completes the active level.
- Completion opens the result panel, stops gameplay input/timer through `LevelPlayState.Completed`, and keeps the save-side next-level unlock rule active.
- The result panel includes Restart and Next buttons when generated from the HUD editor tool.
- Next is shown only when the next `levelId` exists in the catalog and is unlocked in save data.
- Next reapplies the next-level unlock rule before selecting the next level so UI event order cannot hide or block progression if save writing occurs later in the same completion event.

## First Release Scene Generator

Editor generator added: `Eco Garden/Create Scene/First Release Progression`.

The scene generator creates `Assets/EcoGarden/Scenes/EcoGarden_FirstRelease_Progression.unity` with:

- Level 1 as the starting board data.
- `first_release_level_catalog.asset` wired through `LevelCatalogController`.
- HUD, Level select, Delivery, Sell, Shop, Missions, SaveController, MockIapProvider, NPC, butterflies, and input root.
- Restart/Next result-panel controls from the generated HUD.
- `LevelPlaytestMetricsController` for D5 completion/failure metric logs.

Local note: Unity batchmode did not create the scene in this environment because the editor command returned before producing a log or asset. Run the menu item from the Unity Editor to generate the scene.

If the scene was generated before Level Select UI was added, rerun `Eco Garden/Create Scene/First Release Progression` so `LevelButton`, `LevelPanel`, and `LevelSelectUiController` are included.

If the scene was generated before playtest metrics were added, rerun the same menu item so `LevelPlaytestMetricsController` is attached.

## Level Select UI

Generated HUDs now include a `Level` top-bar button and `LevelPanel`.

- Unlocked levels show a Play button.
- Locked levels are visible but disabled.
- Selecting an unlocked level loads it through `LevelCatalogController` in the current scene.
- The panel uses the same Android safe-area panel anchors as Shop and Mission.
- Opening the Level panel hides the compact mission tracker, matching Shop and full Mission panel behavior.
- `Eco Garden/Validation/Audit First Release Scene` requires `LevelSelectUiController`, `LevelButton`, `LevelPanel`, and `LevelList` so stale generated scenes are caught.
