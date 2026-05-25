# Eco Garden - Completion Task Breakdown

Planning date: 2026-05-21

Purpose: define the execution backlog from the current Phase 8 vertical-slice state toward a release-ready Eco Garden build.

Current baseline:

- Level 15 vertical slice is playable.
- Core board, producer, merge, abilities, sell basket, delivery, timer, save, HUD, Android layout, shop, missions, rewards, plant unlocks, difficulty data, and mock IAP are implemented at initial/vertical-slice quality.
- Production readiness is still blocked by save hardening, persistent IAP transaction ids, Unity IAP integration, Android UI verification, level/content expansion, balance, and QA.

## Milestone A - Stabilization Foundation

Goal: make the existing feature set reliable before adding more content.

| Task | Priority | Depends On | Output | Validation |
| --- | --- | --- | --- | --- |
| A1 - Full Core Loop Regression Checklist | P0 | Current build | Manual checklist covering produce, drag, merge, sell, deliver, order reward, mission progress, shop purchase, booster use, pause/restart, fail timer. | Clean-save and existing-save play sessions complete without blocking bugs. |
| A2 - Save Compatibility Audit | P0 | A1 | List every saved field and its safe default: currencies, boosters, board items, order progress, missions, shop inventory, decorations, unlocks, settings, processed purchases. | Old/partial save data loads without exceptions or invalid zeroed progression. |
| A3 - Save Migration and Defaults | P0 | A2 | Code updates for tolerant missing fields and stable fallback values. | EditMode save/load tests for absent fields and partial save JSON. |
| A4 - Persist Processed IAP Transaction IDs | P0 | A3 | Save data stores processed transaction ids; `IapPurchaseService` loads/saves them. | Duplicate transaction after simulated restart does not grant twice. |
| A5 - Negative Flow Test Pack | P0 | A3, A4 | Tests for insufficient currency, invalid product, duplicate mission claim, duplicate shop purchase, invalid delivery, locked tier merge/upgrade, duplicate IAP transaction. | Runtime/editor/EditMode assemblies build and targeted tests pass. |
| A6 - Scene and Reference Audit | P1 | A1 | Check generated scene and existing scene references: HUD, Shop, Mission, EventSystem, MockIAP, delivery/sell zones. | Play Mode starts with no missing-reference errors; all panels open/close. |

Exit criteria:

- Existing vertical slice can be restarted, replayed, and saved safely.
- Economy/progression failure paths are covered by tests.
- Real IAP cannot double-grant after app restart.

## Milestone B - Android and Production IAP

Goal: replace mock-only purchase readiness with a real Android store path behind the existing provider boundary.

| Task | Priority | Depends On | Output | Validation |
| --- | --- | --- | --- | --- |
| B1 - Install Unity IAP Package | P0 | A4 | Add `com.unity.purchasing` to Unity packages and lock files. | Unity import and assemblies compile. |
| B2 - Implement `UnityIapProvider` | P0 | B1 | Production `IIapProvider` implementation registering consumable products from shop catalog. | Mock provider path still works; Unity provider compiles without shop/UI changes. |
| B3 - Map Store Outcomes | P0 | B2 | Map success, cancel, failure, unavailable, duplicate into existing `IapPurchaseStatus`/shop messages. | Unit/EditMode tests cover outcome mapping where possible. |
| B4 - Product ID Verification | P0 | B2 | Confirm Google Play managed product ids: `eco_garden_gems_small`, `eco_garden_gems_medium`. | IDs match shop catalog and `Docs/IAP Integration Decision.md`. |
| B5 - Receipt Validation Decision | P1 | B2 | Document local prototype vs backend validation path and release blocker status. | `Docs/IAP Integration Decision.md` updated. |
| B6 - Android Internal Test Purchase | P0 | B3, B4 | Build and test through Google Play internal track. | Real device verifies success, cancel/fail, restart persistence, duplicate transaction protection. |

Exit criteria:

- Production IAP path exists but remains isolated behind `IIapProvider`.
- Android internal test confirms purchase grant and persistence behavior.

## Milestone C - Mobile UI and Interaction Readiness

Goal: ensure all current systems are usable on Android portrait devices.

| Task | Priority | Depends On | Output | Validation |
| --- | --- | --- | --- | --- |
| C1 - Portrait Layout Matrix | P0 | A6 | Define target profiles: 720x1280, 1080x1920, tall safe-area, small safe-area. | Checklist added to docs. |
| C2 - HUD Overlap Fix Pass | P0 | C1 | Adjust HUD, mission tracker, sell basket, delivery zone, timer/currency, pause/result panels. | No board-critical overlap on target profiles. |
| C3 - Shop UI Mobile Pass | P1 | C1 | Verify category tabs, product rows, Buy/Owned states, prices, feedback text. | Text remains readable and buttons reachable. |
| C4 - Mission UI Mobile Pass | P1 | C1 | Verify compact tracker and full mission panel behavior. | Tracker hides while panels are open and does not cover board interactions. |
| C5 - Touch Parity Pass | P1 | C2 | Verify tap producer, drag item, external drop, booster targeting, panel buttons. | Mouse/editor and touch/device behavior match. |

Exit criteria:

- Android portrait UX is usable without debug assumptions.
- Shop and mission UI do not interfere with core board play.

## Milestone D - Content and Progression Expansion

Goal: move from one vertical-slice level to a small first-release progression.

| Task | Priority | Depends On | Output | Validation |
| --- | --- | --- | --- | --- |
| D1 - First Release Level Set Spec | P1 | A1 | Define 5-10 levels with board layout, timer, difficulty, order requirements, locks, obstacles, rewards. | Level specs reviewed against difficulty progression. |
| D2 - Level Data Asset Creation | P1 | D1 | Create ScriptableObject assets or editor menu generation for first level set. | Parser loads all levels; tests cover representative rows/data. |
| D3 - Order Sequence Tuning | P1 | D1 | Define single/multi-order flows per level, including rewards and checkout/reset behavior. | Orders cannot request locked tiers unless explicitly allowed. |
| D4 - Economy Balance Sheet | P1 | D1 | Tune sell values, order rewards, mission rewards, booster prices, unlock prices, Gem grants. | Non-IAP progression is possible; premium currency is optional. |
| D5 - Difficulty Validation Playtest | P1 | D2, D4 | Playtest Easy/Normal/Hard/Expert levers: timer, obstacles, locks, order quantity/item level. | No dead-end boards; reward scale matches difficulty. |
| D6 - Mission Rotation Decision | P1 | D4 | Decide static, daily, or mixed missions for first release. | Save model and UI behavior align with chosen model. |
| D7 - Decoration Scope Decision | P2 | D4 | Either connect owned decorations to visible cosmetics or remove/defer decoration products. | Purchased decoration has visible effect or is out of release catalog. |

Exit criteria:

- A player can progress through multiple levels with coherent rewards and unlock pressure.
- Shop/mission economy has an initial balance target.

## Milestone E - Art, Audio, Feedback, and Tutorial

Goal: make the game understandable and presentable without relying on debug-like placeholders.

| Task | Priority | Depends On | Output | Validation |
| --- | --- | --- | --- | --- |
| E1 - Visual Asset Acceptance List | P1 | C2 | Mark every procedural/runtime asset as final, acceptable placeholder, or needs authored art. | `Docs/Asset Resource List.md` updated. |
| E2 - Final Gameplay Art Pass | P1 | E1 | Replace or approve board tiles, obstacles, producer, Lotus Lv1-Lv5, NPC, butterflies, icons, UI sprites. | Items remain readable on mobile. |
| E3 - SFX Asset Pass | P1 | A1 | Add/assign pickup/drop, merge, producer, ability, sell, delivery, reward, mission claim, shop purchase, win/fail, timer warning. | Sound settings save/load and mute behavior work. |
| E4 - VFX Polish Pass | P2 | E2 | Improve merge sparkle, coin feedback, delivery complete, ability burst, reward/claim feedback. | Feedback is clear but does not hide board state. |
| E5 - First-Time Guidance | P1 | D2, C5 | Lightweight tutorial prompts or guided first level for producer tap, drag merge, delivery, sell, boosters, shop/missions. | New player can complete first level without external docs. |

Exit criteria:

- Core actions have clear visual/audio feedback.
- First-time player path is understandable.

## Milestone F - QA, Release Build, and Documentation

Goal: turn the project into a reproducible release candidate.

| Task | Priority | Depends On | Output | Validation |
| --- | --- | --- | --- | --- |
| F1 - PlayMode Smoke Tests | P1 | A6, C5 | Tests for scene boot, generated level setup, producer path, panel open/close, basic result state. | Smoke tests catch scene/setup regressions. |
| F2 - Performance Profiling | P1 | C5, D5 | Profile drag, board sync, UI rebuilds, VFX, save writes on Android hardware. | No obvious interaction hitch or repeated GC spike in normal play. |
| F3 - Build Pipeline Documentation | P0 | B6 | Document Unity version, Android modules, package dependencies, build settings, signing/keystore, internal test steps. | Clean machine can reproduce build path. |
| F4 - Release Blocker Tracker | P0 | A1 | Maintain blocker table for IAP, save, Android build, UI overlap, compliance, critical bugs. | Every blocker has status and next action. |
| F5 - Release Candidate Build | P0 | B6, C5, D5, E5, F2, F3 | Build signed Android release candidate. | Install/run on target Android device; smoke checklist passes. |

Exit criteria:

- Release candidate build is reproducible.
- Critical blockers are closed or explicitly deferred.

## Recommended Sprint Order

### Sprint 1 - Reliability First

1. A2 - Save Compatibility Audit.
2. A3 - Save Migration and Defaults.
3. A4 - Persist Processed IAP Transaction IDs.
4. A5 - Negative Flow Test Pack.
5. A6 - Scene and Reference Audit.

Sprint exit: existing systems are safe to build on; duplicate rewards and save breakage risks are reduced.

### Sprint 2 - Android/IAP Gate

1. B1 - Install Unity IAP Package.
2. B2 - Implement `UnityIapProvider`.
3. B3 - Map Store Outcomes.
4. B4 - Product ID Verification.
5. C1 - Portrait Layout Matrix.
6. C2 - HUD Overlap Fix Pass.

Sprint exit: production IAP path compiles and major Android UI risks are known/fixed.

### Sprint 3 - Content and Balance

1. D1 - First Release Level Set Spec.
2. D2 - Level Data Asset Creation.
3. D3 - Order Sequence Tuning.
4. D4 - Economy Balance Sheet.
5. D5 - Difficulty Validation Playtest.

Sprint exit: game has a small progression arc beyond Level 15.

### Sprint 4 - Presentation and Onboarding

1. E1 - Visual Asset Acceptance List.
2. E2 - Final Gameplay Art Pass.
3. E3 - SFX Asset Pass.
4. E5 - First-Time Guidance.
5. C3/C4/C5 - Final UI/touch verification.

Sprint exit: new players can understand and play on Android without developer guidance.

### Sprint 5 - Release Candidate

1. F1 - PlayMode Smoke Tests.
2. F2 - Performance Profiling.
3. F3 - Build Pipeline Documentation.
4. F4 - Release Blocker Tracker.
5. F5 - Release Candidate Build.

Sprint exit: release candidate is buildable, testable, and has a documented blocker status.

## Immediate Next Implementation Task

Start with `A2 - Save Compatibility Audit`, then implement `A3 - Save Migration and Defaults`.

Reason: save compatibility is the dependency that protects shop, missions, unlocks, board state, and IAP transaction persistence. Fixing it first lowers risk for every later milestone.

## Execution Notes

2026-05-21:

- Started Milestone A.
- Implemented first pass of `A3 - Save Migration and Defaults` through `SaveService.Normalize`.
- Implemented `A4 - Persist Processed IAP Transaction IDs` for the mock/current IAP provider path.
- Added first-pass `A5` tests for save normalization and persisted duplicate IAP transaction rejection.
- Broadened `A5` with negative coverage for invalid delivery event safety, locked-tier Magic Wand behavior, and restored duplicate IAP transaction handling through `ShopController`.
- Added `Eco Garden/Validation/Audit Level 15 Scene` editor tool for `A6` scene/reference checks.
- Added `Docs/Milestone A Regression Checklist.md` for repeatable `A1` clean-save/existing-save regression and `A6` scene audit.
- Added `SceneAuditTests` so `A6` can run through Unity EditMode Test Runner after editor refresh.
- Ran `A6` batchmode scene audit successfully; `Eco Garden scene audit passed.`
- Unity batchmode EditMode runner exited successfully but did not emit XML results in this environment, so full Test Runner validation remains a manual/UI follow-up.
- Started `B1 - Install Unity IAP Package` by adding `com.unity.purchasing` 5.3.0 to `Packages/manifest.json`, matching the current Unity IAP v5 documentation stream.
- Completed package resolve for `B1`; `packages-lock.json` now includes Unity IAP 5.3.0 and Unity Services Core 1.16.0.
- Added first-pass `B2 - UnityIapProvider` using Unity IAP v5 `StoreController`, pending purchase event handling, provider completion events, and shared duplicate transaction grant logic.
- Added first-pass `B3` outcome mapping for pending, success, cancel, unavailable, duplicate, and generic failure.
- Added `B4 - Product ID Verification` automation through `Eco Garden/Validation/Audit IAP Catalog` and `IapCatalogAuditTests`, checking the shop catalog against `eco_garden_gems_small` and `eco_garden_gems_medium`.
- Added `C1 - Portrait Layout Matrix` in `Docs/Android Portrait Layout Matrix.md` with target profiles and HUD/panel/touch pass criteria.
- Started `C2 - HUD Overlap Fix Pass` by centralizing Android HUD anchors in `AndroidHudLayoutMetrics`, lowering the AbilityBar, moving Delivery/Sell above the bottom controls, aligning runtime/editor fallback UI to the same anchors, and adding portrait overlap tests.
- Started `C3 - Shop UI Mobile Pass` by centralizing shop row metrics and adding small-portrait width coverage for category tabs, product text, price badges, and Buy/Owned buttons.
- Started `C4 - Mission UI Mobile Pass` by widening the compact tracker on small portrait screens, limiting it to two active rows, and adding metric coverage for tracker/action button width.
- Started `C5 - Touch Parity Pass` with metric coverage that Delivery/Sell drop zones stay large enough and separated enough for drag/drop on portrait profiles.
- Started `D1 - First Release Level Set Spec` in `Docs/First Release Level Set Spec.md`, defining Levels 1-10 with board rows, timers, order requirements, rewards, unlock assumptions, and balance targets.
- Started `D2 - Level Data Asset Creation` by adding `Eco Garden/Create Default Data/First Release Level Set`, an editor generator that creates `level_001_first_sprouts.asset` through `level_010_first_bloom.asset` from the D1 spec.
- Verified `EcoGarden.Editor.csproj` builds after the D2 generator change; Unity batchmode asset generation was blocked locally by Unity Licensing Client disconnects before the execute method ran.
- Generated D2 assets from Unity Editor: `level_001_first_sprouts.asset` through `level_010_first_bloom.asset` plus `.meta` files now exist under `Assets/EcoGarden/ScriptableObjects/Levels`.
- Verified runtime, editor, and EditMode test assemblies build after the D2 assets were created.
- Added first-pass level progression rule: completing the active level order unlocks `currentLevelId + 1` in save data without lowering an already higher `highestUnlockedLevel`.
- Started D3 support work by adding `first_release_level_catalog.asset`, `LevelCatalogDefinition`, and `LevelCatalogService` so the first release level set has a single ordered source for future level selection/loading.
- Added `LevelCatalogController` and `Eco Garden/Fix Scene/Add First Release Level Loader` so a scene can assign the highest unlocked first-release level from save before `BoardController` loads.
- Completed first-pass finite level flow: `LevelStateController` now completes the level on order completion, generated result panels include Restart/Next buttons, and Next loads the next unlocked catalog level in-scene.
- Hardened Next Level against event ordering by reapplying the unlock rule before selecting the next level and testing Level 1 to Level 2 catalog progression.
- Added `Eco Garden/Create Scene/First Release Progression` and `Eco Garden/Validation/Audit First Release Scene` editor tools for generating and validating a playable Level 1-10 progression scene.
- Unity batchmode did not generate the first-release scene locally because the editor command returned without producing a log or asset; run the scene generator menu item from Unity Editor.
- Added Level Select UI to generated HUDs so unlocked catalog levels can be replayed/selected from a top-bar Level button; locked levels remain visible but disabled.
- Hardened Level Select integration by hiding the compact mission tracker while the Level panel is open and making first-release scene audit require the new Level Select UI objects.
- Started `D4 - Economy Balance Sheet` in `Docs/Economy Balance Sheet.md`, capturing Level 1-10 order rewards, mission rewards, shop sinks, IAP Gem grants, balance risks, and D5 playtest metrics.
- Started `D5 - Difficulty Validation Playtest` support by adding `LevelPlaytestMetricsController` and making first-release scene audit require it; completion/failure logs now capture level id/name, result, remaining time, Gold/Gem, and booster counts.
- Verified runtime, editor, and EditMode test assemblies build after Unity IAP import.
- Re-ran `A6` scene audit after Unity IAP import; third batchmode run passed after Unity finished package/domain refresh.
- Completed `D6 - Mission Rotation Decision`: first release uses static, one-time missions only; daily missions are deferred, documented in `Docs/Mission Rotation Decision.md`, and `MissionController` now skips `isDaily` assets by default.
- Completed `D7 - Decoration Scope Decision`: decoration purchases are deferred from the first release, documented in `Docs/Decoration Scope Decision.md`, and the runtime shop catalog excludes decoration items by default while preserving owned-decoration save compatibility.
- Completed `E1 - Visual Asset Acceptance List`: `Docs/Asset Resource List.md` now marks gameplay procedural sprites as first-release accepted placeholders, defers decoration art, and flags background, UI icons, VFX sprites, and SFX as needing authored assets before release.
- Completed `B5 - Receipt Validation Decision`: production Android IAP now explicitly requires backend-backed receipt validation; local/client-only validation is limited to prototype and internal-track testing, with blocker status documented in `Docs/IAP Integration Decision.md`.
- Completed `F4 - Release Blocker Tracker`: added `Docs/Release Blocker Tracker.md` with current release blockers for Play Mode regression, first-release scene regeneration/audit, Android UI/build validation, production IAP receipt validation/internal-track testing, playtest balance, authored presentation assets, and deferred mission/decoration scope.
- Closed release blocker `RB-002`: Unity batchmode `EcoGarden.Editor.EcoGardenSceneAudit.AuditFirstReleaseScene` opened `EcoGarden_FirstRelease_Progression.unity` and passed, confirming the first-release scene has the current required catalog, Level Select, metrics, HUD, shop, mission, save, and input references.
- Closed release blocker `RB-004`: Unity batchmode Android development build succeeded with current Unity IAP package state and produced `EcoGarden_Level15_VerticalSlice.apk`; details recorded in `Docs/Android Build Verification.md`.
- Started `F1 - PlayMode Smoke Tests`: added `EcoGarden.PlayModeTests` and `SceneSmokePlayModeTests` covering Level 15 scene boot references, producer spawn, Shop panel toggle, Mission panel toggle, and Playing state. `dotnet build Eco-Garden\EcoGarden.PlayModeTests.csproj /p:UseSharedCompilation=false` passed. Unity batchmode PlayMode runner compiled/imported the assembly but did not emit a results XML in this environment, so execution through Unity Test Runner remains a follow-up.
- Remaining Milestone A work: run the manual `A1` regression checklist in Play Mode.
