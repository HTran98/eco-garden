---
name: eco-garden-dev
description: Develop the Eco Garden Unity project using its existing roadmap, architecture, rules, and validation workflow. Use when Codex is asked to plan, implement, review, test, document, commit, or continue work on Eco Garden gameplay, board logic, UI, save data, shop, missions, rewards, IAP, Android readiness, or release hardening.
---

# Eco Garden Dev

## Start Every Task

1. Read `.codex/rules/eco-garden.md`.
2. Read `Docs/Implementation Progress.md` for current status and next priorities.
3. Read only the project docs needed for the requested area:
   - Architecture or task ordering: `references/project-docs.md`.
   - Shop, missions, rewards, IAP, or Android release: `references/meta-systems.md`.
4. Inspect code before deciding implementation details.
5. Keep changes scoped to the requested task and existing code patterns.

## Development Workflow

1. Frame the feature or fix in one paragraph.
2. Identify impacted systems: Board, Input, Level, AI, Economy, Rewards, Shop, Missions, IAP, Save, UI, Android, Docs.
3. Check compatibility with old saves, old scenes, and old ScriptableObjects.
4. Implement runtime/service logic before UI wiring.
5. Add or update focused EditMode tests for rule changes.
6. Update docs when behavior, setup, release risk, or task status changes.
7. Verify with the narrowest useful build/test command available.

## Local Code Rules

- Prefer existing service boundaries over new abstractions.
- Keep UI controllers as presenters/wiring; do not duplicate business rules there.
- Keep board mutations in board/runtime services.
- Use ScriptableObjects for authored data and serializable DTOs for saved state.
- Preserve null-safe runtime fallbacks for generated/existing scenes.
- Avoid per-frame scene searches and repeated allocations in drag/gameplay paths.

## Validation Minimums

- Runtime/editor/test assemblies should build after code changes when feasible.
- EditMode tests should cover both success and failure for new gameplay/economy/progression rules.
- Save changes must prove absent fields load safely.
- UI changes must be checked against portrait Android layout risk.
- IAP changes must preserve the `IIapProvider` boundary and duplicate grant protection.

## Commit Hygiene

- Stage only intentional files.
- Include Unity `.meta` files for new Unity assets/scripts.
- Exclude generated artifacts such as `.utmp`, `*_BurstDebugInformation_DoNotShip`, `*_BackUpThisFolder_ButDontShipItWithYourGame`, `Library`, `Temp`, `Build`, `Builds`, APK/AAB output, and IDE files.
- Treat unexpected dirty files as user/Unity changes; do not revert them unless explicitly requested.

## Key Next Priorities

Use `Docs/Implementation Progress.md` as the source of truth. Current high-priority themes are:

- Save compatibility and persistent processed IAP transactions.
- Negative coverage for economy, missions, shop, delivery, unlocks, and IAP.
- Unity IAP integration behind `IIapProvider`.
- Android portrait UI verification.
- First release content, balance, and release blocker tracking.
