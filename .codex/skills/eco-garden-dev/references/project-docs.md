# Project Docs Reference

Load these docs only when they are relevant to the task.

## Planning and Status

- `Docs/Implementation Progress.md`: source of truth for completed work, planned work, current risks, and next sprint.
- `Docs/Implementation Task Breakdown.md`: phase/task dependency order and definitions of done.
- `Docs/Technical Implementation Plan.md`: architecture, runtime components, data model, test plan, scene setup, performance notes.
- `Docs/Game Design Spec.md`: product/design behavior, economy and progression intent, player-facing systems.

## Implementation Guardrails

- `Docs/Execution Safety Checklist.md`: use for pre-dev, pre-merge, release gates, and weekly health checks.
- `Docs/Asset Resource List.md`: use before changing art/audio/resource naming.
- `Docs/Android Build Verification.md`: use before Android build work or build documentation updates.

## Current Architecture Summary

- Board and gameplay rules are mostly under `Eco-Garden/Assets/EcoGarden/Scripts/Board`, `Level`, `Abilities`, `Input`, and `AI`.
- Meta systems are under `Economy`, `Rewards`, `Shop`, `Missions`, `IAP`, `Progression`, and `Save`.
- UI wiring is under `Scripts/UI`; keep business rules in runtime services.
- Generated/default authoring tools are under `Scripts/Editor`.
- EditMode tests live under `Eco-Garden/Assets/EcoGarden/Tests/EditMode`.

## Verification Pattern

- Prefer targeted EditMode tests for pure logic.
- Build runtime, editor, and test assemblies when the task affects compile-time code.
- For Unity scenes/assets, perform a scene/reference sanity check when Unity is available.
- For Android/UI changes, check small and tall portrait layouts.
