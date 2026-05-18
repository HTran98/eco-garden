# Eco Garden - Implementation Progress

Last updated: 2026-05-19

## Completed

| Task | Status | Notes |
| --- | --- | --- |
| Task 0.1 - Confirm Unity Project State | Partial | Unity version confirmed as `6000.4.7f1`; batchmode test was blocked because the project already has Unity processes open. |
| Task 0.2 - Create EcoGarden Folder Structure | Done | Added `Assets/EcoGarden` folders for scripts, prefabs, art, audio, ScriptableObjects, and tests. |
| Task 0.3 - Define Asset List and Naming Convention | Done | Added `Docs/Asset Resource List.md`. |
| Task 1.1 - Add Core Gameplay Types | Done | Added board/item/ability core runtime types. |
| Task 1.2 - Add ScriptableObject Definitions | Done | Added item, producer, level, NPC order, and ability-count definitions. |
| Task 1.3 - Implement Level Parser | Done | Added parser for top-to-bottom row data and Level 15 tokens. |
| Task 1.4 - Add EditMode Tests for Parser | Done | Added parser tests plus board rule and ability tests. |
| Task 1.5 - Create Lotus and Level 15 Assets | Tool Ready | Added Unity editor menu to create the data assets inside Unity. |
| Task 2.1 - Implement Board Runtime | Done | Added `BoardState` with cell lookup and board mutation rules. |
| Task 2.2 - Implement Move and Merge Rules | Done | Added move/merge methods and EditMode coverage. |
| Task 2.3 - Implement Producer Spawn Logic | Done | Added cooldown-aware producer spawn with nearest empty cell search. |
| Task 2.4 - Implement Ability Rules | Done | Added Shovel, Magic Wand, and Sorting Magnet service logic. |
| Task 2.5 - Add EditMode Tests for Board Rules | Done | Added board and ability rule tests. |
| Task 3.1 - Create Placeholder Visual Assets | Runtime Placeholder Done | Added runtime placeholder square sprite and color palette; final art remains future work. |
| Task 3.2 - Implement BoardView | Initial Done | Added `BoardController`, `BoardView`, `CellView`, `ItemView`, and scene creation tool. |
| Task 3.3 - Add Board Event Updates | Done | Added incremental `BoardView.Sync` updates for cell/item changes so actions no longer rebuild the whole board view every refresh. |
| Task 4.1 - Implement Pointer Input Abstraction | Initial Done | Added Input System based `BoardInputController` for mouse and touch press/release. |
| Task 4.2 - Implement Drag Move and Merge | Improved | Item now lifts from the cell, follows pointer while dragged, then animates into target or back to source. |
| Task 4.3 - Implement Producer Tap | Initial Done | Tap producer cell to spawn Lotus Lv1 using board producer rules. |
| Task 4.4 - Implement Ability Targeting | Initial Done | Added ability HUD buttons, selected ability state, target tap handling, counts, and board refresh on use. |
| Task 4.5 - Implement External Drop Zone Base | Initial Done | Added `ExternalDropZone` and drop-zone detection for UI overlay targets. |
| Task 4.6 - Implement Sell Basket | Improved | Added Sell Basket UI, sell logic, gold economy, coin feedback text, and `DraggedItemCanvasGhost` so sold items animate in front of UI. |
| Task 5.1 - Implement Game State Controller | Initial Done | Added `LevelStateController` with Playing, Completed, and Failed states. |
| Task 5.2 - Implement Timer | Initial Done | Timer counts down from LevelDefinition timer and fails the level at zero. |
| Task 5.3 - Implement NPC Objective | Improved | Drag Lotus Lv5 to the external Delivery drop zone to complete the level; delivery is no longer represented by a board cell. |
| Task 5.4 - Implement Basic HUD | Done | HUD now shows data-driven objective, timer, ability counts, gold, a working pause/resume button, and a Delivery drop zone. |
| Task 5.5 - Implement Win/Fail Panels | Initial Done | HUD tool creates result panel and restart button. |
| Task 6.1 - Add NPC Movement | Done | Added `NpcMovementController`; customer enters, idles at the Delivery drop zone, and exits after fulfillment. |
| Task 6.2 - Add Butterfly Cosmetic Movement | Done | Added `ButterflyMovementController`; Butterfly A loops around the board and Butterfly B hovers near the upper-left pond area without affecting gameplay. |
| Task 6.3 - Add Gameplay Feedback | Initial Done | Added `GameplayFeedbackController` for world popups and HUD messages on producer spawn/block, move, merge, invalid drops, ability use, sell, and delivery. |
| Task 6.3.1 - Add Sell Coin Feedback | Done | Sell Basket now shows coin burst text, pulses the gold counter on reward, and exposes a sell SFX hook through `BoardInputController`. |
| Task 6.4 - Replace Placeholder Art With Production Assets | Initial Runtime Art Done | Replaced square runtime placeholders with distinct procedural sprites for board tiles, obstacles, producer, Lotus Lv1-Lv5, customer NPC, and butterflies; final hand-authored art can still replace these sprite sources later. |
| Task 7.1 - Implement Basic Save Data | Improved | Added PlayerPrefs-backed JSON save for gold, highest unlocked level, booster counts, sound/music settings, plant count, and plant positions/levels/item ids; save auto-loads on scene start and auto-saves on gold, booster, board, and objective changes. |
| Task 7.2 - Android UI Pass | Initial Done | Added `AndroidHudLayoutController` for safe-area aware portrait HUD layout, text best-fit rules, drop-zone positioning, and extra camera bottom padding for Android-style screens. |
| Task 7.3 - Performance and Allocation Pass | Initial Done | Cached active external drop zones, reused board sync buffers, and pooled world feedback text to reduce per-drag scene searches and per-action allocations. |

## Added Editor Tools

| Menu | Purpose |
| --- | --- |
| `Eco Garden/Create Default Data/Level 15 Vertical Slice` | Creates Lotus Lv1-Lv5 item assets, producer asset, and Level 15 asset. |
| `Eco Garden/Create UI/Game HUD Skeleton` | Creates a Canvas with timer, gold, objective, ability buttons, feedback text, and EventSystem. |
| `Eco Garden/Fix UI/EventSystem Input System Module` | Replaces legacy `StandaloneInputModule` with `InputSystemUIInputModule`. |
| `Eco Garden/Create Scene/Level 15 Vertical Slice` | Creates a ready-to-play Level 15 scene with camera, board root, HUD, and default data. |

## Verification Notes

Unity import initially reported a compile error in `AbilityService` because out parameters were not assigned on failure paths. That issue has been fixed.

Batchmode EditMode tests did not produce a result file because the project already has active Unity processes. Run tests from Unity Test Runner after the editor refreshes, or close the open project instance before running batchmode.

2026-05-18: `dotnet build Eco-Garden/EcoGarden.Runtime.csproj --no-restore` and `dotnet build Eco-Garden/EcoGarden.EditModeTests.csproj --no-restore` both succeeded after local project restore.

2026-05-18: Delivery objective UX updated after playtest feedback. `dotnet build Eco-Garden/EcoGarden.Runtime.csproj --no-restore`, `dotnet build Eco-Garden/EcoGarden.Editor.csproj --no-restore`, and `dotnet build Eco-Garden/EcoGarden.EditModeTests.csproj --no-restore` succeeded.

2026-05-18: Task 6.1 NPC movement implemented. Runtime build succeeded; editor and EditMode test assemblies succeeded with `/p:UseSharedCompilation=false` to avoid transient compiler file locks.

2026-05-18: Delivery UX refined so the purple NPC order point is no longer part of the board. NPC now uses the external Delivery drop zone as the visual handoff location. Runtime, editor, and EditMode test assemblies built successfully.

2026-05-18: Task 6.2 butterfly cosmetic movement implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-18: Task 6.3 gameplay feedback implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-18: Task 6.3.1 sell coin feedback implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-18: Task 6.4 initial runtime art pass implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`; one parallel editor build hit a transient file lock and passed when rerun.

2026-05-18: Runtime art/UI pass refined after playtest: item sprites are larger, item level numbers are hidden by default, NPC is larger and offset above the Delivery zone, and HUD/drop-zone/button sprites are procedurally skinned. Runtime, editor, and EditMode test assemblies built successfully.

2026-05-18: Task 7.1 basic save data implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-18: Save data expanded to persist current plant count and every plant item's board position, family, level, and item id. Runtime, editor, and EditMode test assemblies built successfully.

2026-05-19: Task 7.2 Android UI pass implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-19: Task 7.3 performance/allocation pass implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.
