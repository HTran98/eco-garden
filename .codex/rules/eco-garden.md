# Eco Garden Development Rules

Use these rules for every Eco Garden change.

## Scope First

- Read `Docs/Implementation Progress.md` before planning new work.
- Use `Docs/Implementation Task Breakdown.md` for dependency order.
- Use `Docs/Technical Implementation Plan.md` for architecture and system boundaries.
- Use `Docs/Execution Safety Checklist.md` before and after implementation.
- For IAP work, read `Docs/IAP Integration Decision.md` before touching shop or purchase code.

## Architecture Rules

- Put gameplay rules in runtime services/controllers first, not in UI code.
- Keep `BoardController` focused; extract a helper/service when logic grows beyond board orchestration.
- Keep data authoring in ScriptableObjects and runtime state in plain serializable/runtime classes.
- Preserve fallback behavior for old scenes, old saves, and missing ScriptableObject fields.
- Use explicit result/status types for economy, shop, mission, save, IAP, and delivery failure paths.

## Testing Rules

- Add or update EditMode tests for core rule changes.
- Include at least one negative test for every new success path.
- Save/load changes must test missing/default fields and persistence after reload.
- Shop, mission, and IAP changes must test failure states: insufficient currency, invalid product, cancel/fail purchase, duplicate claim, duplicate transaction.

## Unity and Android Rules

- Commit `.cs`, `.asset`, `.unity`, `.meta`, docs, and required ProjectSettings when they are part of the feature.
- Do not commit generated build artifacts, temp folders, Burst debug output, IL2CPP backup folders, or local IDE files.
- Check portrait Android layout impact for HUD, Shop, Mission, Sell Basket, Delivery, and result panels after UI changes.
- Do not enable production IAP until processed transaction ids persist across app restarts.

## Documentation Rules

- Update `Docs/Implementation Progress.md` after completed tasks.
- Document new setup/build requirements in `Docs`.
- Keep known limitations explicit instead of leaving them implied in code.
