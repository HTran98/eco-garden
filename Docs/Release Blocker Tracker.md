# Eco Garden - Release Blocker Tracker

Date created: 2026-05-25

Purpose: track the blockers that must be closed, explicitly deferred, or accepted before an Android release candidate.

## Status Legend

| Status | Meaning |
| --- | --- |
| Open | Blocks release candidate or production release until resolved. |
| Needs Device | Code/docs exist, but real Android or Google Play validation is still required. |
| Needs Unity Editor | Requires manual Unity Editor action or Play Mode validation. |
| Deferred | Not part of first release scope and documented as deferred. |
| Closed | Completed and validated for the current release scope. |

## Blockers

| ID | Area | Blocker | Status | Owner | Next Action | Release Impact |
| --- | --- | --- | --- | --- | --- | --- |
| RB-001 | Core Gameplay | Full A1 clean-save and existing-save regression has not been run in Play Mode. | Needs Unity Editor | User/Codex with Unity Editor | Run `Docs/Milestone A Regression Checklist.md` in Play Mode and log result. | Blocks release candidate confidence. |
| RB-002 | Scene | First-release progression scene may be stale after Level Select and playtest metrics additions. | Needs Unity Editor | User/Codex with Unity Editor | Run `Eco Garden/Create Scene/First Release Progression`, then `Eco Garden/Validation/Audit First Release Scene`. | Blocks first-release level flow validation. |
| RB-003 | Android UI | Portrait layout has code/metric coverage, but no device screenshot pass for notch/gesture-nav profiles. | Needs Device | User/device QA | Run `Docs/Android Portrait Layout Matrix.md` on target Android devices and record pass/fail. | Blocks mobile UX signoff. |
| RB-004 | Android Build | Android build with Unity IAP imported has not been rerun after package integration. | Needs Unity Editor | User/Codex with Android modules | Build Android from Unity after confirming Android modules and Unity licensing are stable. | Blocks Android release candidate. |
| RB-005 | IAP | Google Play internal-track purchase has not been tested for success, cancel/fail, restart persistence, and duplicate transaction behavior. | Needs Device | User/Google Play account | Configure managed products and run internal-track purchase tests. | Blocks production IAP and store readiness. |
| RB-006 | IAP | Backend receipt validation is not implemented. | Open | Backend/release owner | Add backend validation endpoint, receipt payload handoff, and server-side transaction idempotency. | Blocks production real-money IAP. |
| RB-007 | IAP | `UnityIapProvider` does not yet expose receipt payload data for backend validation. | Open | Client engineering | Extend IAP result model/provider to capture receipt payload without breaking mock provider tests. | Blocks backend receipt validation wiring. |
| RB-008 | Content/Balance | Level 1-10 difficulty and economy have instrumentation but no recorded playtest tuning pass. | Needs Unity Editor | Design/QA | Playtest Levels 1-10, record `LevelPlaytestMetricsController` output, update balance notes. | Blocks balance confidence. |
| RB-009 | Art/UI | Background, UI icons, VFX sprites, and SFX are still marked as needing authored assets. | Open | Art/audio owner | Prioritize minimum release asset set from `Docs/Asset Resource List.md`. | Blocks presentation/audio signoff if final assets are required. |
| RB-010 | Decorations | Decoration purchases have no visible cosmetic application path. | Deferred | Product/design | Keep decoration products hidden from first-release runtime catalog. Revisit after release. | Does not block first release while deferred. |
| RB-011 | Missions | Daily/rotating missions are not implemented. | Deferred | Product/design | Keep first release on static one-time missions. Revisit after release. | Does not block first release while deferred. |

## Current Release Gate Summary

| Gate | Current State | Minimum Next Action |
| --- | --- | --- |
| Core loop | Automated rule tests pass; manual A1 pass is still open. | Run Play Mode regression. |
| First-release scene | Tooling exists; scene regeneration/audit still needs Editor confirmation. | Regenerate and audit scene. |
| Android UX | Metric tests pass for key layout risks; device validation is open. | Run portrait device matrix. |
| Android build | Previous manual Android build succeeded before final IAP/package state. | Rebuild Android with current project. |
| Production IAP | Provider/package/product-id checks exist; receipt validation/internal-track tests are open. | Keep IAP non-production until RB-005/RB-006/RB-007 close. |
| Presentation | Runtime gameplay sprites accepted; several authored assets still open. | Decide minimum shippable asset bar. |

## Update Rules

- Update this tracker after every Play Mode regression, Android build, device UI pass, IAP test, or release-scope decision.
- Keep deferred items visible so they are not mistaken for implemented features.
- A release candidate should not be cut while any `Open`, `Needs Device`, or `Needs Unity Editor` blocker remains unless the release owner explicitly accepts that risk in this file.
