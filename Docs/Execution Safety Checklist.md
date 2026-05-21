# Eco Garden - Execution Safety Checklist

Purpose: keep delivery speed high while reducing architecture drift and regression risk.
Scope: use this checklist for every new feature and every bugfix that touches gameplay, economy, missions, shop, save, or IAP.

## How to use

- Before implementation: complete `Pre-Dev`.
- Before merge/acceptance: complete `Pre-Merge`.
- End of week: complete `Weekly Health`.

---

## 1) Pre-Dev Checklist

### 1.1 Feature framing

- [ ] Feature has a one-paragraph goal and explicit out-of-scope.
- [ ] Impacted systems are listed (Board, Input, Missions, Shop, Save, IAP, UI, Android).
- [ ] Backward compatibility is identified (old saves, old scenes, old ScriptableObjects).
- [ ] Fallback behavior is defined when new data is missing.

### 1.2 Design and ownership

- [ ] New rules are added in service/runtime classes first, not in UI code.
- [ ] `BoardController` change is minimized; if large, extract helper/service instead of growing controller logic.
- [ ] Public API changes are documented in code comments or implementation notes.

### 1.3 Scene and reference safety

- [ ] Prefer serialized references; avoid new broad runtime lookups unless necessary.
- [ ] Any runtime lookup has a null-safe fallback and clear user feedback.
- [ ] No per-frame search calls in `Update` unless temporary and justified.

---

## 2) Pre-Merge Checklist

### 2.1 Coding quality

- [ ] No duplicated business logic across controllers.
- [ ] No hidden side effects in simple getters or UI refresh methods.
- [ ] Error paths return explicit status/result enums.
- [ ] New enums and result types include default/unknown handling.

### 2.2 Test coverage

- [ ] EditMode tests added/updated for the core rule changes.
- [ ] At least one negative test exists for each new success path.
- [ ] Save/load behavior tested for new fields and version tolerance.
- [ ] Shop/Mission/IAP flow tests include failure states (cancel, duplicate, insufficient currency, invalid product).

### 2.3 Integration sanity

- [ ] Scene opens and Play Mode starts without missing references.
- [ ] Core loop still works end-to-end: produce -> move/merge -> sell/deliver -> reward/progress.
- [ ] HUD entry points open/close correctly (Shop, Mission, Results).
- [ ] Existing scene fallback behavior still works for missing optional UI objects.

### 2.4 Performance and allocation

- [ ] No new avoidable allocations in frequent input/gameplay paths.
- [ ] No repeated object creation for stable UI rows without cleanup/pooling strategy.
- [ ] Any new pooling/caching has clear reset behavior.

### 2.5 Android readiness

- [ ] Portrait layout checked on at least one small and one tall safe-area profile.
- [ ] Text remains readable; no overlap with board-critical areas.
- [ ] Input interactions tested for touch drag/tap parity.

---

## 3) Release Gates (Phase 8+)

### 3.1 Economy and progression gates

- [ ] Gold/Gem spend and earn are balanced and cannot go negative.
- [ ] Mission rewards cannot be claimed twice.
- [ ] Non-repeatable shop products cannot be purchased twice.
- [ ] Plant tier unlock restrictions are enforced for merge and upgrade flows.

### 3.2 IAP production gates

- [ ] `IIapProvider` production implementation added (Unity IAP provider).
- [ ] Product IDs match documented list.
- [ ] Duplicate transaction protection verified with provider callbacks.
- [ ] Restore purchases path tested.
- [ ] Failure and cancellation messaging validated in UI.

### 3.3 Save compatibility gates

- [ ] New save fields have safe defaults when absent.
- [ ] Old save data does not block startup.
- [ ] Critical progression state (currency, missions, unlocks, owned products) survives app restart.

---

## 4) Done Definition Per Task

A task is considered done only when all items are true:

- [ ] Feature works in gameplay.
- [ ] Relevant tests pass locally.
- [ ] `Docs/Implementation Progress.md` updated with status + validation note.
- [ ] Any new setup steps are documented in Docs.
- [ ] Known limitations are explicitly listed (if any).

---

## 5) Weekly Health Checklist

- [ ] Review top 3 highest-risk files by change frequency (for example: BoardController, ShopUiController, MissionUiController).
- [ ] Identify one extraction/refactor candidate to reduce controller growth.
- [ ] Remove one temporary workaround if no longer needed.
- [ ] Verify no stale TODO/FIXME blocks critical paths.
- [ ] Re-check Android build path and blocker list.

---

## 6) Optional Fast Score (for planning)

Score each area from 1-5, then prioritize the lowest:

- Architecture stability: [ ]
- Test confidence: [ ]
- UI robustness: [ ]
- Save/IAP reliability: [ ]
- Android release readiness: [ ]
