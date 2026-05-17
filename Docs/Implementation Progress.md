# Eco Garden - Implementation Progress

Last updated: 2026-05-17

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
| Task 4.1 - Implement Pointer Input Abstraction | Initial Done | Added Input System based `BoardInputController` for mouse and touch press/release. |
| Task 4.2 - Implement Drag Move and Merge | Improved | Item now lifts from the cell, follows pointer while dragged, then animates into target or back to source. |
| Task 4.3 - Implement Producer Tap | Initial Done | Tap producer cell to spawn Lotus Lv1 using board producer rules. |
| Task 4.4 - Implement Ability Targeting | Initial Done | Added ability HUD buttons, selected ability state, target tap handling, counts, and board refresh on use. |
| Task 4.5 - Implement External Drop Zone Base | Initial Done | Added `ExternalDropZone` and drop-zone detection for UI overlay targets. |
| Task 4.6 - Implement Sell Basket | Improved | Added Sell Basket UI, sell logic, gold economy, coin feedback text, and `DraggedItemCanvasGhost` so sold items animate in front of UI. |
| Task 5.1 - Implement Game State Controller | Initial Done | Added `LevelStateController` with Playing, Completed, and Failed states. |
| Task 5.2 - Implement Timer | Initial Done | Timer counts down from LevelDefinition timer and fails the level at zero. |
| Task 5.3 - Implement NPC Objective | Initial Done | Drag Lotus Lv5 to NPC order point to complete the level. |
| Task 5.5 - Implement Win/Fail Panels | Initial Done | HUD tool creates result panel and restart button. |

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
