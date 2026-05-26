# Eco Garden - Implementation Progress

Last updated: 2026-05-25

## Current Completion Roadmap

Updated planning date: 2026-05-21

The current project state is past the Level 15 vertical slice and has initial implementations for the Phase 8 meta systems: Gold/Gem economy, rewards, multi-item NPC orders, plant tier unlocks, difficulty data, shop, missions, mock IAP, and Android layout support. The remaining work should now shift from feature scaffolding to production hardening, content expansion, balancing, and release validation.

### Priority 1 - Stabilize Core Gameplay and Save Reliability

| Task | Priority | Contribution / Output | Done When |
| --- | --- | --- | --- |
| Task 9.1 - Run Full Regression Pass | P0 | Verify produce, drag, merge, sell, deliver, booster use, order reward, mission progress, shop purchase, pause/restart, and fail timer in one continuous play session. | All core loops work from a clean save and from an existing save. Issues are logged with reproduction steps. |
| Task 9.2 - Harden Save Compatibility | P0 | Add/verify safe defaults for Gold, Gem, boosters, plant positions, order progress, missions, owned shop products, decorations, plant tier unlocks, and processed purchase ids. | Old or partial save data loads without blocking Play Mode, and critical state survives app restart. |
| Task 9.3 - Persist Processed IAP Transactions | P0 | Move duplicate transaction protection from runtime memory into save data before real store purchases are enabled. | Replaying the same store transaction after restart does not grant rewards twice. |
| Task 9.4 - Add Negative Flow Coverage | P0 | Add focused EditMode tests for insufficient currency, locked plant tier merge/upgrade, invalid delivery, duplicate mission claim, duplicate non-repeatable shop purchase, and duplicate IAP transaction. | New tests fail before the relevant guard and pass after implementation. |
| Task 9.5 - Scene Reference Audit | P1 | Open generated and existing scenes, check missing references, inactive panel wiring, fallback-created UI, and EventSystem/Input System setup. | Scene starts without missing reference errors and all HUD entry points open/close correctly. |

### Priority 2 - Production IAP and Android Store Readiness

| Task | Priority | Contribution / Output | Done When |
| --- | --- | --- | --- |
| Task 10.1 - Install and Lock Unity IAP Package | P0 | Add `com.unity.purchasing` through Unity Package Manager and commit the manifest/package lock changes. | Project imports and assemblies compile with Unity IAP installed. |
| Task 10.2 - Implement `UnityIapProvider` | P0 | Add a production provider behind `IIapProvider`, register catalog products, and map Unity purchase outcomes to existing `IapPurchaseStatus`. | Shop IAP products can route through Unity IAP without changing shop UI or reward logic. |
| Task 10.3 - Configure Google Play Product IDs | P0 | Create/verify Google Play managed products: `eco_garden_gems_small` and `eco_garden_gems_medium`. | Store ids match the shop catalog and documented IAP decision. |
| Task 10.4 - Add Receipt Validation Plan | P1 | Decide prototype local validation vs backend validation; document the release requirement. | Production release path has an explicit validation decision and blocker list. |
| Task 10.5 - Android Internal Test Purchase | P0 | Build Android with Unity IAP and test success, cancel/fail, app restart, and duplicate transaction behavior through Google Play internal testing. | Real device purchase grants once, persists, and failure states do not alter inventory. |

### Priority 3 - Content, Progression, and Balance

| Task | Priority | Contribution / Output | Done When |
| --- | --- | --- | --- |
| Task 11.1 - Define First Release Level Set | P1 | Expand from Level 15 vertical slice into a small playable level set with difficulty labels, order requirements, timer values, obstacles, locks, and rewards. | At least 5-10 levels are represented as data assets or documented level specs. |
| Task 11.2 - Balance Economy Sources and Sinks | P1 | Tune sell values, order rewards, mission rewards, shop prices, booster prices, and plant tier unlock costs. | A player can progress without IAP, while Gem/IAP remains optional acceleration or cosmetic purchase. |
| Task 11.3 - Mission Rotation Decision | P1 | Decide whether missions are static, daily rotating, or mixed for the first release. | Mission save fields and UI behavior match the selected model. |
| Task 11.4 - Decoration Ownership Usage | P2 | Connect owned decoration ids to visible cosmetic changes or explicitly defer decorations from release scope. | Purchased decorations are either visible in-game or removed/deferred from the initial catalog. |
| Task 11.5 - Difficulty Scaling Validation | P1 | Playtest Easy/Normal/Hard definitions against order complexity, locked cells, obstacles, timer, and rewards. | Each difficulty has a clear gameplay difference without causing dead-end boards. |

### Priority 4 - UI, Art, Audio, and Feel

| Task | Priority | Contribution / Output | Done When |
| --- | --- | --- | --- |
| Task 12.1 - Final Mobile UI Pass | P0 | Check 720x1280, 1080x1920, tall safe-area screens, and small font edge cases for HUD, shop, missions, result panels, sell basket, and delivery zone. | No critical text overlap, blocked board cells, or unreachable buttons on target Android layouts. |
| Task 12.2 - Replace or Approve Procedural Runtime Art | P1 | Decide which procedural sprites are acceptable and which need final authored art. | Every visible gameplay object has either final art or an approved placeholder status. |
| Task 12.3 - Add Production SFX Hooks and Assets | P1 | Fill pickup/drop, merge, producer spawn, ability use, sell, reward, mission claim, shop purchase, win/fail, and timer warning audio. | Core actions are readable through sound, with settings respecting saved sound/music preferences. |
| Task 12.4 - VFX Polish Pass | P2 | Improve merge sparkle, ability burst, coin feedback, delivery completion, shop/mission claim feedback, and timer warning effects. | Feedback is clear on mobile without covering important board interactions. |
| Task 12.5 - Tutorial / First-Time Guidance | P1 | Add lightweight first-use guidance for producer tap, drag merge, delivery, sell basket, boosters, shop, and missions. | A new player can complete the first playable level without reading external instructions. |

### Priority 5 - Release QA and Documentation

| Task | Priority | Contribution / Output | Done When |
| --- | --- | --- | --- |
| Task 13.1 - Add PlayMode Smoke Tests | P1 | Cover scene boot, generated Level 15 setup, producer tap path, drag/merge path where feasible, and UI panel open/close. | Smoke tests catch scene/setup regressions beyond pure EditMode logic. |
| Task 13.2 - Performance Profiling Pass | P1 | Profile drag, board sync, shop/mission panel rebuilds, VFX pooling, and save writes on Android hardware. | No obvious interaction hitch or repeated GC spike in normal play. |
| Task 13.3 - Build Pipeline Checklist | P0 | Document exact Unity version, Android modules, build settings, package dependencies, keystore/signing requirement, and internal test steps. | A clean machine can reproduce the Android build path from docs. |
| Task 13.4 - Release Blocker List | P0 | Maintain a short blocker table for IAP, save, Android build, UI overlap, compliance, and critical gameplay bugs. | Every release blocker has owner/status/next action before submission. |
| Task 13.5 - Update Progress After Each Completed Task | P1 | Keep this document current with status and validation commands/results. | Progress notes match the current code state and do not require archaeology through commit history. |

### Recommended Next Sprint

| Sprint Task | Why Now | Validation |
| --- | --- | --- |
| Task 9.2 - Harden Save Compatibility | Save corruption or missing defaults can invalidate shop, missions, unlocks, and IAP work. | EditMode save/load tests plus manual restart from old and clean saves. |
| Task 9.3 - Persist Processed IAP Transactions | Required before any production IAP testing. | Duplicate transaction test across simulated restart. |
| Task 9.4 - Add Negative Flow Coverage | The project now has many economy/progression failure paths that must not silently grant rewards. | Runtime/editor/test assemblies build and targeted EditMode tests pass. |
| Task 10.1 - Install and Lock Unity IAP Package | Production store work is currently blocked by package absence. | Unity import and assembly build pass with `com.unity.purchasing`. |
| Task 12.1 - Final Mobile UI Pass | Recent shop/mission tracker changes were playtest-driven and need device-size verification. | Screenshot/manual checklist for small and tall portrait Android layouts. |

### Current Highest Risks

| Risk | Impact | Mitigation |
| --- | --- | --- |
| IAP duplicate grant after restart | Real-money purchases could grant twice. | Persist processed transaction ids before Unity IAP provider testing. |
| Old save data missing new Phase 8 fields | Existing testers may hit zero boosters, missing panels, locked tiers, or lost progress. | Add tolerant save defaults and a save migration/clear-save workflow for test builds. |
| UI overlap on Android portrait | Shop/mission tracker/sell basket/delivery zone can obstruct board interactions. | Run layout checks on multiple portrait aspect ratios after every UI change. |
| Feature scaffolding without balance | Systems exist but may not form a satisfying progression curve. | Create a small level set and economy tuning sheet before adding more features. |
| Mock IAP masking store integration issues | Editor path passes while Android store callbacks fail. | Keep provider boundary, but test Unity IAP on Google Play internal track early. |

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
| Task 7.4 - Build Verification | Done | Added Android batchmode build utility and documented attempts in `Docs/Android Build Verification.md`; manual Android build completed successfully in Unity after Android build settings were available. |

## Planned / Awaiting Approval

| Task | Status | Notes |
| --- | --- | --- |
| Phase 8 - Shop, Missions, and IAP Expansion | In Progress | Scope approved and implementation has started for Gold/Gem currency, rewards, multi-item NPC orders, delivery progress, and plant tier unlock rules. |
| Task 8.0 - Approve Shop, Mission, and IAP Scope | Done | Scope approved by user request to continue implementation after docs update. |
| Task 8.1 - Add Gold and Gem Currency Model | Initial Done | Added `CurrencyKind`, Gem balance support in `EconomyController`, Gold/Gem save/load, Gem HUD text in generated HUD plus runtime fallback for existing scenes, and EditMode coverage for currency balance/spend behavior. |
| Task 8.2 - Add Reward Data Model | Initial Done | Added reusable reward data for currencies, abilities, decoration ids, and plant tier unlocks, plus `RewardService` for applying Gold/Gem and booster grants with EditMode coverage. |
| Task 8.3 - Add Multi-Item NPC Order Model | Initial Done | Extended NPC order data with multi-requirement support, reward reference, item matching helpers, runtime submitted-count state, save DTOs for order progress, objective text formatting for multiple requirements, and EditMode coverage. |
| Task 8.4 - Implement NPC Delivery Progress and Checkout Flow | Initial Done | Delivery now consumes matching requested items one at a time, updates submitted progress, completes only when all requirements are fulfilled, grants configured order rewards, persists order progress, updates objective text as `submitted/required`, and moves NPC through a checkout point before exit. |
| Task 8.5 - Add Plant Tier Unlock Rules | Initial Done | Added `PlantUnlockService`, save/load for unlocked plant tiers, reward grants for plant tier unlocks, merge/magic-wand output validation, temporary level-allowed tiers, Level 15 data support for Lv4/Lv5 tutorial override, and EditMode coverage. |
| Task 8.6 - Add Difficulty and Reward Scaling Data | Initial Done | Added `DifficultyKind`, `DifficultyDefinition`, `TemporaryLockDefinition`, order complexity metrics, Level 15 Hard difficulty metadata, temporary lock sample data, and EditMode coverage for difficulty/order scaling data. |
| Task 8.7 - Add Shop Data Model | Initial Done | Added shop category/purchase enums, price data, `ShopItemDefinition`, scene-independent `ShopCatalogService`, default shop catalog editor data for boosters/decorations/unlocks/Gem packs, and EditMode coverage for lookup/filter behavior. |
| Task 8.8 - Implement Shop Purchase Flow With Gold and Gem | Initial Done | Added `ShopController`, purchase status/result data, shop inventory for non-repeatable products and owned decorations, Gold/Gem spend validation, reward granting, save/load of shop inventory, scene generator wiring, and EditMode coverage. |
| Task 8.9 - Add Shop UI | Initial Done | Added HUD Shop button, shop panel, category tabs, scrollable product list, Buy buttons wired to `ShopController`, purchase feedback messages, owned state display, and Android layout/skin support. |
| Task 8.10 - Add Mission Data Model | Initial Done | Added mission type/data assets, runtime mission state/controller, save/load progress DTOs, default mission editor data, scene generator wiring, and EditMode coverage for mission state loading/progress. |
| Task 8.11 - Track Mission Progress From Gameplay Events | Initial Done | Added typed board gameplay events for merge, produce, sell, deliver, and ability use, connected `MissionController` progress tracking to those events, and covered mission progress updates from board gameplay and ability usage in EditMode tests. |
| Task 8.12 - Implement Mission Rewards and Claim Flow | Initial Done | Added mission reward claim status/result data, one-time reward claiming through `RewardService`, claimed-state persistence through existing mission save data, and EditMode coverage for successful and rejected claims. |
| Task 8.13 - Add Mission List UI | Initial Done | Added HUD Mission button, mission panel, scrollable mission rows, progress/reward text, Claim buttons wired to `MissionController`, claim feedback messages, and Android layout/skin support with runtime fallback for existing scenes. |
| Task 8.14 - Add IAP Provider Abstraction and Mock Provider | Initial Done | Added `IIapProvider`, mock IAP provider, purchase status/result models, `IapPurchaseService` reward grant path, duplicate transaction protection, and EditMode coverage for success, cancel, duplicate, and non-repeatable IAP purchases. |
| Task 8.15 - Connect IAP Products to Shop UI | Initial Done | Connected IAP-priced shop products to `MockIapProvider` through `ShopController`, added IAP purchase status mapping and UI messages, scene generator wiring for the mock provider, and EditMode coverage for successful and cancelled IAP shop purchases. |
| Task 8.16 - Platform IAP Integration Decision and Build Check | Decision Done | Chose Unity IAP as the Android production provider behind `IIapProvider`, documented required product ids and setup checklist, confirmed `com.unity.purchasing` is not installed yet, and documented current build status/production blockers in `Docs/IAP Integration Decision.md` and Android build notes. |

## Added Editor Tools

| Menu | Purpose |
| --- | --- |
| `Eco Garden/Create Default Data/Level 15 Vertical Slice` | Creates Lotus Lv1-Lv5 item assets, producer asset, and Level 15 asset. |
| `Eco Garden/Create Default Data/Shop Catalog` | Creates the initial shop product assets for booster packs, decorations, plant tier unlocks, and Gem IAP packs. |
| `Eco Garden/Create Default Data/Missions` | Creates the initial mission definition assets for merge, produce, sell, deliver, high-tier order, and tool-use missions. |
| `Eco Garden/Create UI/Game HUD Skeleton` | Creates a Canvas with timer, gold, objective, ability buttons, feedback text, and EventSystem. |
| `Eco Garden/Fix UI/EventSystem Input System Module` | Replaces legacy `StandaloneInputModule` with `InputSystemUIInputModule`. |
| `Eco Garden/Create Scene/Level 15 Vertical Slice` | Creates a ready-to-play Level 15 scene with camera, board root, HUD, and default data. |
| `EcoGarden.Editor.EcoGardenAndroidBuildVerification.BuildLevel15Android` | Batchmode method for Android APK verification of the Level 15 scene. |

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

2026-05-19: Proposed Phase 8 planning added for Shop, Missions, and IAP. Awaiting approval before implementation.

2026-05-19: Phase 8 planning revised to keep Gold as the earned currency, add Gem as premium currency, define multi-item NPC order delivery/reward flow, list shop categories/items, add plant tier unlocks, and document difficulty/reward scaling. Runtime implementation remains paused pending approval.

2026-05-19: Task 7.4 Android build verification attempted. Editor assembly build succeeded. First attempt was blocked by an active Unity editor lock; second attempt after closing Unity reached project import but was blocked by Android platform support/licensing (`UnityEditor.Android.Extensions` missing and `Switching to AndroidPlayer is disabled`). See `Docs/Android Build Verification.md`.

2026-05-19: User confirmed Android build completed successfully from Unity Editor. Task 7.4 marked Done.

2026-05-19: Task 8.0 approved and Task 8.1 Gold/Gem currency model implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-19: Task 8.2 reward data model implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-19: Task 8.3 multi-item NPC order model implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-19: Task 8.4 NPC delivery progress and checkout flow implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-19: Task 8.5 plant tier unlock rules implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`; initial parallel build attempt hit a transient `EcoGarden.Runtime.dll` file lock and passed when rerun sequentially.

2026-05-19: NPC order completion flow fixed after playtest. Completing an order now grants reward, keeps the level in Playing state so timer/input continue, sends the NPC to checkout and back to Delivery, then resets order progress for the next customer. Level 15 generated data now includes a Gold reward for the default order, with a fallback reward for older assets. Runtime, editor, and EditMode test assemblies built successfully.

2026-05-19: Task 8.6 difficulty and reward scaling data implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`; a parallel build attempt hit a transient runtime DLL file lock and passed when rerun sequentially.

2026-05-19: Task 8.7 shop data model implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-19: Task 8.8 shop purchase flow implemented for Gold/Gem products. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`; IAP products are detected but return unsupported until the IAP provider task is implemented.

2026-05-19: Task 8.9 shop UI implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`; the first parallel EditMode build hit a transient runtime DLL file lock and passed when rerun sequentially.

2026-05-20: Shop panel open bug fixed. `ShopUiController` now resolves inactive HUD objects such as `ShopPanel`, close button, category buttons, and product list before wiring button events. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-20: Shop UI fallback improved after the Shop button still did not open the panel in an existing scene. `ShopUiController` now rewires buttons again on `Start`/`OnEnable`, retries wiring while missing, and can create a runtime `ShopPanel` fallback if the scene reference is missing. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-20: Task 8.10 mission data model implemented. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`; initial parallel editor build hit a transient runtime DLL file lock and passed when rerun sequentially.

2026-05-20: Task 8.11 mission progress tracking implemented. Successful merge, producer spawn, sell, delivery, and booster-use actions now emit gameplay events that increment matching missions and persist through the existing save flow. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-20: Task 8.12 mission reward claim flow implemented. Completed missions can claim configured rewards once, incomplete or already-claimed missions are rejected, and claimed state is saved through `MissionsChanged`. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-20: Task 8.13 mission list UI implemented. HUD now has a Mission entry point, mission panel rows show progress and rewards, and claimable missions can grant rewards through the UI. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`; an initial parallel editor build hit a transient runtime DLL file lock and passed when rerun sequentially.

2026-05-20: Mission UI refined after UX feedback. A compact always-visible mission tracker now appears on the right side above the Sell area, showing active mission progress and claim buttons without requiring the player to open the full Mission panel. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`; an initial parallel test build hit a transient runtime DLL file lock and passed when rerun sequentially.

2026-05-20: Mission tracker size reduced after playtest feedback. The always-visible tracker now occupies a smaller right-side area above Sell, shows two active missions, and uses tighter row/text sizing to avoid covering the board. Runtime and editor assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-20: Mission tracker placement refined after playtest feedback. The tracker right edge remains compact, its top is raised to align with the board area, height is increased, and mission row text is larger for readability. Runtime and editor assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-20: Mission tracker text size increased again after playtest feedback. Tracker title, mission names, and progress text are larger, with taller rows to avoid cramped text. Runtime and editor assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-20: Task 8.14 IAP provider abstraction and mock provider implemented. Mock purchases can simulate success, cancellation, and failure; successful IAP grants configured rewards once and repeated transaction ids do not double-grant. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`; an initial parallel runtime build hit a transient DLL file lock and passed when rerun sequentially.

2026-05-20: Task 8.15 IAP shop connection implemented. IAP catalog rows now route through the mock provider when present, grant configured rewards on success, report cancel/fail/duplicate states through shop feedback, and the generated scene includes a `MockIapProvider` on `GameRoot`. Runtime, editor, and EditMode test assemblies built successfully with `/p:UseSharedCompilation=false`; an initial parallel test build hit a transient DLL file lock and passed when rerun sequentially.

2026-05-20: Mission tracker/shop overlap fixed after playtest feedback. The always-visible mission tracker now hides while the Shop panel or full Mission panel is open, then returns when those panels close. Runtime and editor assemblies built successfully with `/p:UseSharedCompilation=false`.

2026-05-20: Task 8.16 platform IAP decision and build check completed. Unity IAP is selected for future Android production IAP behind the existing provider boundary; current project remains on `MockIapProvider` because `com.unity.purchasing` is not installed. Required store product ids, Android setup checklist, and blockers are documented in `Docs/IAP Integration Decision.md` and `Docs/Android Build Verification.md`.

2026-05-19: Booster targeting UX fixed. Re-selecting the same booster now cancels selection, and tapping an invalid booster target cancels the booster selection while allowing the same tap to continue through normal board actions such as producer spawn or item drag. Runtime, editor, and EditMode test assemblies built successfully.

2026-05-19: Booster `No uses left` recovery improved. Ability HUD now ensures the board ability inventory is loaded before checking counts, and an editor menu `Eco Garden/Save/Clear Local Save Data` was added to reset stale local saves that may have persisted booster counts at zero during testing. Runtime, editor, and EditMode test assemblies built successfully.

2026-05-20: Shop UI visual refresh implemented. Shop rows now use procedural card sprites instead of temporary square blocks, include category badges, descriptions, price badges, clearer Buy/Owned states, and highlighted category tabs. Runtime, editor, and EditMode test assemblies built successfully.

2026-05-20: Added delivery safety checklist for ongoing development in `Docs/Execution Safety Checklist.md`, covering coding guardrails, test minimums, scene/reference stability, Android checks, and Phase 8+ release gates.

2026-05-21: Completion execution started from `Docs/Completion Task Breakdown.md`. Save hardening and IAP transaction persistence were implemented: `SaveData` now has a schema version and `processedIapTransactionIds`, `SaveService.Normalize` supplies safe defaults for old/partial saves, `ShopController` restores/captures processed IAP transaction ids, and `IapPurchaseService` can seed duplicate protection from saved transaction ids. Added EditMode coverage for save normalization and persisted duplicate IAP transaction behavior. `dotnet build Eco-Garden/EcoGarden.Runtime.csproj --no-restore /p:UseSharedCompilation=false` and `dotnet build Eco-Garden/EcoGarden.EditModeTests.csproj --no-restore /p:UseSharedCompilation=false` passed. A parallel build attempt hit the known transient runtime DLL file lock and passed when rerun sequentially.

2026-05-21: Milestone A negative-flow coverage expanded. Added tests proving invalid delivery does not mutate board/order state or fire delivery/completion events, Magic Wand cannot upgrade into a locked plant tier and does not consume a charge, and `ShopController` rejects an IAP transaction id restored from save without granting Gem. `dotnet build Eco-Garden/EcoGarden.Runtime.csproj --no-restore /p:UseSharedCompilation=false` and `dotnet build Eco-Garden/EcoGarden.EditModeTests.csproj --no-restore /p:UseSharedCompilation=false` passed.

2026-05-21: Added `EcoGarden.Editor.EcoGardenSceneAudit.AuditLevel15Scene` and menu item `Eco Garden/Validation/Audit Level 15 Scene` to support Milestone A scene/reference validation. The audit checks required gameplay/meta controllers, LevelDefinition assignment, Input System UI module, Sell/Delivery drop zones, and missing scripts. Added `Docs/Milestone A Regression Checklist.md` for clean-save/existing-save core loop regression and scene audit instructions. `dotnet build Eco-Garden/EcoGarden.Editor.csproj --no-restore /p:UseSharedCompilation=false` passed. Unity batchmode audit was not executed because Unity editor processes were already open for the project and did not produce a test result/log.

2026-05-21: Added `SceneAuditTests` so the Level 15 scene audit can run through Unity EditMode Test Runner, and updated the EditMode test assembly to reference `EcoGarden.Editor`. `dotnet build Eco-Garden/EcoGarden.EditModeTests.csproj /p:UseSharedCompilation=false` and `dotnet build Eco-Garden/EcoGarden.Editor.csproj /p:UseSharedCompilation=false` passed after restore regenerated local project assets.

2026-05-21: Started Milestone B Android/IAP gate. Installed Unity IAP package `com.unity.purchasing` 5.3.0, resolved `packages-lock.json`, added first-pass `UnityIapProvider` behind `IIapProvider`, and updated the shop/IAP service flow so Unity IAP v5 pending purchases can complete asynchronously through the same reward grant and duplicate transaction protection path. Runtime, editor, and EditMode test assemblies build successfully, and the Level 15 scene audit passed after Unity package/domain refresh. Android internal-track purchase testing is still required.

2026-05-22: Added Milestone B product ID verification. `IapProductIds` centralizes required Google Play ids, `UnityIapProvider` uses that source for its default consumable list, and `EcoGardenIapCatalogAudit` plus `IapCatalogAuditTests` validate shop IAP assets against `eco_garden_gems_small` and `eco_garden_gems_medium`. Unity batchmode `EcoGarden.Editor.EcoGardenIapCatalogAudit.AuditIapCatalog` passed, and runtime/editor/EditMode test assemblies build successfully.

2026-05-22: Started Milestone C mobile UI readiness with `Docs/Android Portrait Layout Matrix.md`. The matrix defines 720x1280, 1080x1920, tall, notch, gesture-nav, and small safe-area stress profiles plus HUD, panel, and touch parity pass criteria for the next HUD overlap and mobile interaction passes.

2026-05-22: Started `C2 - HUD Overlap Fix Pass`. Added `AndroidHudLayoutMetrics` as the shared source for portrait HUD anchors, lowered the AbilityBar inside the safe-area root, moved Delivery/Sell drop zones above bottom controls, aligned runtime UI fallback and editor HUD generation with those anchors, and added `AndroidHudLayoutMetricsTests` for core portrait profiles. Unity batchmode script compilation passed, and runtime/editor/EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-22: Started `C4 - Mission UI Mobile Pass`. The compact mission tracker is wider on small portrait screens, limited to two visible active missions, and covered by metric tests for minimum tracker/action-button width while still avoiding Delivery/Sell drop zones.

2026-05-22: Started `C3 - Shop UI Mobile Pass`. Added `ShopUiLayoutMetrics`, routed shop product rows through shared anchors, and added `ShopUiLayoutMetricsTests` so category tabs, product text, price badges, and Buy/Owned buttons keep minimum usable widths on 720x1280 portrait.

2026-05-22: Started `C5 - Touch Parity Pass`. Extended `AndroidHudLayoutMetricsTests` to verify Delivery/Sell external drop zones keep minimum touch size and horizontal separation across portrait profiles, reducing risk that drag/drop targets conflict with each other or bottom controls.

2026-05-22: Started `D1 - First Release Level Set Spec`. Added `Docs/First Release Level Set Spec.md` with a 10-level release progression, board row drafts, order requirements, reward targets, tier unlock assumptions, difficulty notes, and D2 asset creation guidance.

2026-05-22: Started `D2 - Level Data Asset Creation`. Added `Eco Garden/Create Default Data/First Release Level Set` to generate LevelDefinition assets for levels 1-10 from the release spec while reusing existing Lotus item and producer assets. `dotnet build Eco-Garden/EcoGarden.Editor.csproj /p:UseSharedCompilation=false` passed. Unity batchmode asset generation did not complete locally because the Unity Licensing Client disconnected before the execute method ran; rerun the menu item in Unity after licensing is stable.

2026-05-22: `D2 - Level Data Asset Creation` assets generated from Unity Editor. Added `level_001_first_sprouts.asset` through `level_010_first_bloom.asset` and matching `.meta` files under `Assets/EcoGarden/ScriptableObjects/Levels`. Unity IAP also generated `Assets/Resources/BillingMode.json` with Google Play store mode. `dotnet build Eco-Garden/EcoGarden.Runtime.csproj /p:UseSharedCompilation=false`, `dotnet build Eco-Garden/EcoGarden.Editor.csproj /p:UseSharedCompilation=false`, and `dotnet build Eco-Garden/EcoGarden.EditModeTests.csproj /p:UseSharedCompilation=false` passed.

2026-05-22: Added first-pass level progression unlock rule. `LevelProgressionService` now opens the next level after the active level order is completed by updating `highestUnlockedLevel` to at least `currentLevelId + 1`, and `SaveController` applies that rule on `OrderCompleted` before saving. Added EditMode coverage for unlocking the next level, preserving already higher progress, and checking whether a level is unlocked. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-22: Started D3 level set catalog support. Added `LevelCatalogDefinition`, `LevelCatalogService`, `first_release_level_catalog.asset`, and editor menu `Eco Garden/Create Default Data/First Release Level Catalog` so Levels 1-10 are available through one ordered catalog for level selection/loading work. Added EditMode coverage for sorted catalog construction, duplicate skipping, missing lookup failure, and highest-unlocked-level resolution. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-22: Added first-pass level catalog scene loader. `LevelCatalogController` can select the highest unlocked level from `first_release_level_catalog.asset` and assign it to `BoardController` before board load, while rejecting locked level selections. Added editor menu `Eco Garden/Fix Scene/Add First Release Level Loader` for wiring existing scenes without replacing the Level 15 scene generator. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-22: Added first-pass finite level completion flow. `LevelStateController` now listens for order completion, sets `LevelPlayState.Completed`, shows the result panel, and exposes `StartNextLevel` for catalog-backed in-scene progression. Generated HUD result panels now include Restart and Next buttons, with Next visible only when the next level exists and is unlocked. Added EditMode coverage for completed-state transition. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-22: Hardened Next Level flow against order-completion event ordering. The result panel now shows Next whenever the catalog contains the next level after completion, and `StartNextLevel` reapplies/saves the unlock rule before selecting that next level. Added EditMode coverage that `StartNextLevel` moves the board from Level 1 to Level 2 through a catalog-backed controller. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-22: Added first-release progression scene tooling. `Eco Garden/Create Scene/First Release Progression` creates a playable `EcoGarden_FirstRelease_Progression.unity` scene starting from Level 1 with `LevelCatalogController`, `first_release_level_catalog.asset`, HUD, Shop, Missions, SaveController, MockIapProvider, NPC, butterflies, and input wiring. `Eco Garden/Validation/Audit First Release Scene` validates required scene references and catalog presence. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`. Unity batchmode scene generation did not produce a scene/log locally, so run the scene generator menu item from Unity Editor.

2026-05-22: Added first-pass Level Select UI. Generated HUDs now include a Level top-bar button and Level panel that lists catalog levels, enables Play for unlocked levels, disables locked entries, and loads selected unlocked levels through `LevelCatalogController`. `AndroidHudLayoutController` applies the standard panel anchors to the Level panel, and `HudSkinController` skins the new Level controls. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-22: Checked the generated `EcoGarden_FirstRelease_Progression.unity` scene after adding Level Select UI. The scene currently has `LevelCatalogController` and `NextLevelButton`, but it was generated before `LevelButton`/`LevelPanel` were added. Rerun `Eco Garden/Create Scene/First Release Progression` from Unity Editor to regenerate the scene with the new Level Select UI.

2026-05-22: Hardened Level Select UI integration. The compact mission tracker now hides while the Level panel is open, and `Eco Garden/Validation/Audit First Release Scene` now requires `LevelSelectUiController`, `LevelButton`, `LevelPanel`, and `LevelList` so stale first-release scenes fail validation instead of silently missing level selection. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-22: Started `D4 - Economy Balance Sheet`. Added `Docs/Economy Balance Sheet.md` covering first-release Gold/Gem assumptions, Level 1-10 order rewards, mission rewards, shop sinks, IAP Gem grants, current balance risks, and D5 playtest metrics to capture before numeric tuning. Current recommendation is to playtest before changing prices because Levels 1-10 remain non-IAP-safe through temporary tier unlocks, while stacked mission claims may make Gold too generous.

2026-05-22: Started `D5 - Difficulty Validation Playtest` instrumentation. `LevelStateController` now exposes remaining time and completion/failure events, and `LevelPlaytestMetricsController` logs level id/name, result, remaining timer seconds, Gold/Gem, and booster counts on complete/fail. The first-release scene generator and audit now include this metrics controller. Added EditMode coverage that `CompleteLevel` raises its completion event once. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-25: Completed `D6 - Mission Rotation Decision`. First release missions are static, one-time missions; daily rotation is deferred until after release-candidate work. Added `Docs/Mission Rotation Decision.md`, made `MissionController` skip `isDaily` assets by default so unsupported rotating content does not appear in the first-release UI, and added EditMode coverage for that guard.

2026-05-25: Completed `D7 - Decoration Scope Decision`. Decoration purchases are deferred from the first release because owned decoration ids currently have no visible cosmetic application path. Added `Docs/Decoration Scope Decision.md`, made `ShopCatalogService` exclude decoration items by default while keeping an opt-in path for future cosmetic builds, and added EditMode coverage for default exclusion and future inclusion.

2026-05-25: Completed `E1 - Visual Asset Acceptance List`. Updated `Docs/Asset Resource List.md` so current runtime procedural gameplay sprites are explicitly accepted as first-release placeholders, decoration art is deferred with decoration purchases, and background, UI icons, VFX sprites, and SFX are flagged as needing authored assets before release.

2026-05-25: Completed `B5 - Receipt Validation Decision`. Updated `Docs/IAP Integration Decision.md` to require backend-backed receipt validation before production Android IAP, limit local/client-only validation to prototype and Google Play internal-track testing, and list open production blockers for receipt payload capture, server idempotency, backend validation, and internal purchase validation.

2026-05-25: Completed `F4 - Release Blocker Tracker`. Added `Docs/Release Blocker Tracker.md` with current blockers and next actions for A1 Play Mode regression, first-release scene regeneration/audit, Android portrait/device validation, Android build with Unity IAP, Google Play internal-track purchase testing, backend receipt validation, playtest balance, authored presentation assets, and deferred decoration/mission scope.

2026-05-25: Closed release blocker `RB-002` for first-release scene validation. Unity batchmode `EcoGarden.Editor.EcoGardenSceneAudit.AuditFirstReleaseScene` passed against `EcoGarden_FirstRelease_Progression.unity`; the log confirmed the scene opened and the first-release scene audit passed after script compilation.

2026-05-25: Closed release blocker `RB-004` for Android build validation. Unity batchmode `EcoGarden.Editor.EcoGardenAndroidBuildVerification.BuildLevel15Android` succeeded with the current Unity IAP package state, producing `Builds/Android/EcoGarden_Level15_VerticalSlice.apk`; build result, output path, size, and duration are recorded in `Docs/Android Build Verification.md`.

2026-05-25: Started `F1 - PlayMode Smoke Tests`. Added `EcoGarden.PlayModeTests` and `SceneSmokePlayModeTests` for Level 15 scene boot references, producer spawn, Shop panel toggle, Mission panel toggle, and active Playing state. `dotnet build Eco-Garden\EcoGarden.PlayModeTests.csproj /p:UseSharedCompilation=false` passed; Unity batchmode PlayMode runner compiled/imported the assembly but did not emit a results XML in this environment, so Test Runner execution remains a follow-up.

2026-05-25: Closed release blocker `RB-007` for client receipt payload capture. `IapPurchaseResult` and `IapProductPurchaseResult` now expose receipt payloads, `UnityIapProvider` captures Unity IAP `order.Info.Receipt` on pending purchase completion/failure callbacks, and mock IAP remains receipt-empty for Editor/test flows. `dotnet build Eco-Garden/EcoGarden.Runtime.csproj --no-restore /p:UseSharedCompilation=false` and `dotnet build Eco-Garden/EcoGarden.EditModeTests.csproj /p:UseSharedCompilation=false` passed; backend validation remains open under `RB-006`.

2026-05-25: Started `RB-006` client/backend receipt validation framework. Added `IIapReceiptValidator`, receipt validation request/result/status models, and a fail-closed `BackendIapReceiptValidator` placeholder; `IapPurchaseService` now blocks reward grants and processed transaction persistence when a configured validator rejects, reports invalid payload, or has no backend transport available. Existing mock/editor IAP behavior remains unchanged when no validator is configured. Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; production backend endpoint, HTTP transport, and server-side idempotency remain open.

2026-05-25: Updated the IAP release decision after product direction clarification. This game will not create or operate a custom backend/server for first release; Unity IAP client-only purchase callbacks plus local processed transaction persistence are the accepted release path. `RB-006` is now marked as an accepted risk instead of a blocking backend task, while `RB-005` Google Play internal-track purchase validation remains required before public IAP.

2026-05-26: Started the release UI improvement track. Added `Docs/UI Release Audit.md` with a baseline UI risk summary, Android portrait target profiles, audit table, and task breakdown from UI-R1 baseline capture through HUD, panel framework, Shop, Mission, Level Select, feedback, icons/assets, and Android device validation. Added `RB-012` to the release blocker tracker for release-ready UI polish, with UI-R1/UI-R2 as the next actions.

2026-05-26: Started UI-R2 HUD gameplay release pass. Added compact top-bar and ability-button anchors to `AndroidHudLayoutMetrics`, made `AndroidHudLayoutController` apply those child rects and compact top-bar labels at runtime, changed ability buttons to two-line labels for small portrait fit, aligned the HUD generator with those anchors, and added EditMode metric coverage for small-portrait top-bar/action widths and ability touch size. Runtime, editor, and EditMode test assemblies build successfully after removing stale generated `.csproj` references locally for verification; generated project files are not tracked.

2026-05-26: Started UI-R3 shared panel framework polish. Added `PanelUiLayoutMetrics` for shared panel title, close button, content, Shop tab/content, and Result action anchors; `AndroidHudLayoutController` now applies panel child rects to active and inactive scene objects. The HUD generator and runtime Shop/Mission fallback panels now use the shared metrics, and new EditMode tests cover small-portrait panel header, Shop content separation, and Result action touch sizes. Runtime, editor, and EditMode test assemblies build successfully after local generated `.csproj` verification updates; generated project files are not tracked.

2026-05-26: Started UI-R4 Shop release pass. `ShopUiController` now disables empty release categories, falls back to a populated category when Decoration content is deferred, uses `Store` copy for Unity IAP rows, blocks invalid/pending/owned rows from repeated taps, and refreshes pending IAP state from `ShopController.IapPurchaseCompleted`. Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-26: Started UI-R5 Mission release pass. Added `MissionUiLayoutMetrics` and EditMode coverage for full mission row and compact tracker touch/text widths. `MissionUiController` now gives active, ready-to-claim, and claimed missions distinct row/button states, uses shorter claim labels, prefixes reward text, and prioritizes claimable missions in the compact tracker. Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false` after local generated `.csproj` verification updates; generated project files are not tracked.

2026-05-26: Started UI-R6 Level Select and Result flow pass. Added `LevelSelectUiLayoutMetrics` and EditMode coverage for level row title/action widths on small portrait. `LevelSelectUiController` now shows Done/Current/Locked row states, difficulty/timer metadata, order summary copy, and disabled styling for locked levels. `LevelStateController` now clarifies complete/fail result messages, labels restart as Replay or Retry, keeps Next visible only when a catalog next level exists, and resolves inactive result UI objects during wiring. Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false` after local generated `.csproj` verification updates; generated project files are not tracked.
