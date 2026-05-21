# Eco Garden - Milestone A Regression Checklist

Date created: 2026-05-21

Purpose: complete Milestone A manual validation after save hardening and persistent IAP transaction protection.

## Preconditions

- Open Unity `6000.4.7f1`.
- Open project `Eco-Garden`.
- Use scene `Assets/EcoGarden/Scenes/EcoGarden_Level15_VerticalSlice.unity`.
- For clean-save checks, run `Eco Garden/Save/Clear Local Save Data` before entering Play Mode.
- For existing-save checks, run once, make progress, exit Play Mode, then re-enter Play Mode.

## A6 - Scene Reference Audit

Run from Unity menu:

```text
Eco Garden/Validation/Audit Level 15 Scene
```

Or run the EditMode test:

```text
EcoGarden.Tests.EditMode.SceneAuditTests.Level15Scene_HasRequiredReferences
```

Or from batchmode when no Unity editor process has the project open:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe' `
  -batchmode `
  -quit `
  -projectPath 'D:\Project\Game\Eco-Garden' `
  -executeMethod EcoGarden.Editor.EcoGardenSceneAudit.AuditLevel15Scene `
  -logFile 'D:\Project\Game\Eco-Garden\Logs\SceneAudit.log'
```

Pass criteria:

- `BoardController`, `BoardView`, `BoardInputController`, `EconomyController`, `SaveController`, `ShopController`, `MissionController`, and `MockIapProvider` exist.
- `BoardController` has a LevelDefinition.
- `EventSystem` uses `InputSystemUIInputModule`.
- Sell and Delivery drop zones exist.
- No missing scripts are present in the scene.

## A1 - Core Loop Regression

### Clean Save

- [ ] Clear local save data.
- [ ] Enter Play Mode without console errors.
- [ ] Timer starts and counts down.
- [ ] Producer tap creates Lotus Lv1 near the producer.
- [ ] Dragging an item to an empty playable cell moves it.
- [ ] Dragging two same-level lotus items merges into the next tier when unlocked/temporarily allowed.
- [ ] Invalid drag returns item to source.
- [ ] Shovel removes an obstacle and decrements count.
- [ ] Magic Wand upgrades a valid unlocked/temporarily allowed item and decrements count.
- [ ] Magic Wand on locked-tier output fails without decrementing count.
- [ ] Sorting Magnet moves a valid matching pair and decrements count.
- [ ] Selling a plant removes it and increases Gold.
- [ ] Invalid delivery keeps item on board and does not progress order.
- [ ] Valid delivery consumes requested item and updates objective progress.
- [ ] Completed order grants reward once.
- [ ] NPC checkout/return flow runs after order completion.
- [ ] Mission progress updates from produce, merge, sell, deliver, and ability use.
- [ ] Completed mission claim grants reward once.
- [ ] Shop Gold/Gem purchase spends correct currency and grants reward.
- [ ] IAP mock success grants configured reward.
- [ ] IAP mock cancel/fail does not grant reward.
- [ ] Pause/resume button works.
- [ ] Restart button reloads playable state.
- [ ] Timer reaching zero shows fail state.

### Existing Save

- [ ] Exit Play Mode after changing Gold/Gem, booster counts, board items, order progress, missions, shop inventory, plant unlocks, and processed IAP transactions.
- [ ] Re-enter Play Mode.
- [ ] Gold/Gem restore correctly.
- [ ] Booster counts restore correctly.
- [ ] Board items restore correctly.
- [ ] Partial order progress restores correctly.
- [ ] Mission progress and claimed state restore correctly.
- [ ] Purchased non-repeatable shop products remain owned.
- [ ] Owned decorations remain saved even if cosmetic usage is deferred.
- [ ] Plant tier unlocks restore correctly.
- [ ] Replayed mock transaction id is rejected as duplicate and does not grant twice.

## Result Log

| Date | Tester | Build/Commit | Clean Save | Existing Save | Scene Audit | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| 2026-05-21 | Codex | Uncommitted workspace | Not run - manual Play Mode required | Not run - manual Play Mode required | Pass - batchmode `AuditLevel15Scene` | Unity batchmode EditMode runner exited 0 but did not create XML results; use menu/test runner UI for A1/A6 if XML is required. |
| 2026-05-21 | Codex | Uncommitted workspace after Unity IAP import | Not run - manual Play Mode required | Not run - manual Play Mode required | Pass - `SceneAuditAfterIap3.log` | First two post-IAP audit attempts were consumed by package/domain refresh; third run executed audit and passed. |
