# Eco Garden - Release Blocker Tracker

Date created: 2026-05-25

Purpose: track the blockers that must be closed, explicitly deferred, or accepted before an Android release candidate.

## Status Legend

| Status | Meaning |
| --- | --- |
| Open | Blocks release candidate or production release until resolved. |
| Needs Device | Code/docs exist, but real Android or Google Play validation is still required. |
| Needs Unity Editor | Requires manual Unity Editor action or Play Mode validation. |
| Accepted Risk | Explicit release-owner decision; does not block release under current scope. |
| Deferred | Not part of first release scope and documented as deferred. |
| Closed | Completed and validated for the current release scope. |

## Blockers

| ID | Area | Blocker | Status | Owner | Next Action | Release Impact |
| --- | --- | --- | --- | --- | --- | --- |
| RB-001 | Core Gameplay | Full A1 clean-save and existing-save regression has not been run in Play Mode. | Needs Unity Editor | User/Codex with Unity Editor | Run `Docs/Milestone A Regression Checklist.md` in Play Mode and execute `EcoGarden.PlayModeTests`; log results. | Blocks release candidate confidence. |
| RB-002 | Scene | First-release progression scene may be stale after Level Select and playtest metrics additions. | Closed | User/Codex with Unity Editor | Batchmode `EcoGarden.Editor.EcoGardenSceneAudit.AuditFirstReleaseScene` passed on 2026-05-25. Regenerate scene only after future scene-generator or HUD changes. | First-release scene references validated for current release scope. |
| RB-003 | Android UI | Portrait layout has code/metric coverage, but no device screenshot pass for notch/gesture-nav profiles. | Needs Device | User/device QA | Run `Docs/Android Portrait Layout Matrix.md` on target Android devices and record pass/fail. | Blocks mobile UX signoff. |
| RB-004 | Android Build | Android build with Unity IAP imported has not been rerun after package integration. | Closed | User/Codex with Android modules | Batchmode `EcoGarden.Editor.EcoGardenAndroidBuildVerification.BuildLevel15Android` succeeded on 2026-05-25 and produced `EcoGarden_Level15_VerticalSlice.apk`. Rebuild after future package/build-setting changes. | Android development build path validated for current project state. |
| RB-005 | IAP | Google Play internal-track purchase has not been tested for success, cancel/fail, restart persistence, and duplicate transaction behavior. | Needs Device | User/Google Play account | Configure managed products and run internal-track purchase tests. | Blocks production IAP and store readiness. |
| RB-006 | IAP | Backend receipt validation endpoint and server idempotency are not implemented. | Accepted Risk | Release owner | No backend/server will be created for this game. Use Unity IAP client-only with local processed transaction persistence; revisit backend only if fraud risk becomes unacceptable. | Does not block first release under the accepted Unity IAP client-only scope. |
| RB-007 | IAP | `UnityIapProvider` does not yet expose receipt payload data for backend validation. | Closed | Client engineering | `IapPurchaseResult` and `IapProductPurchaseResult` now expose receipt payloads; `UnityIapProvider` captures `order.Info.Receipt`; mock provider remains receipt-empty for Editor tests. | Client receipt handoff model is ready for future backend validation wiring. |
| RB-008 | Content/Balance | Level 1-10 difficulty and economy have instrumentation but no recorded playtest tuning pass. | Needs Unity Editor | Design/QA | Playtest Levels 1-10, record `LevelPlaytestMetricsController` output, update balance notes. | Blocks balance confidence. |
| RB-009 | Art/UI | Background, timer icon, VFX sprites, and SFX are still marked as needing authored assets; core UI icons now have accepted runtime symbolic placeholders. | Open | Art/audio owner | Decide whether remaining authored background/VFX/SFX/timer icon are required for first release or accepted as post-release polish. | Blocks presentation/audio signoff if final assets are required. |
| RB-012 | UI | UI-R2 through UI-R8 code passes are complete, but release UI has not been validated on Android portrait/notch/gesture-nav devices. | Needs Device | User/device QA | Run `Docs/Android UI Validation Log.md` and `Docs/Android Portrait Layout Matrix.md`; record screenshots and failures. | Blocks release-quality UX signoff. |
| RB-010 | Decorations | Decoration purchases have no visible cosmetic application path. | Deferred | Product/design | Keep decoration products hidden from first-release runtime catalog. Revisit after release. | Does not block first release while deferred. |
| RB-011 | Missions | Daily/rotating missions are not implemented. | Deferred | Product/design | Keep first release on static one-time missions. Revisit after release. | Does not block first release while deferred. |

## Current Release Gate Summary

| Gate | Current State | Minimum Next Action |
| --- | --- | --- |
| Core loop | Automated rule tests pass and PlayMode smoke tests compile; manual A1 pass and Unity Test Runner execution are still open. | Run Play Mode regression and `EcoGarden.PlayModeTests`. |
| First-release scene | Batchmode audit passed for the current `EcoGarden_FirstRelease_Progression.unity` scene. | Re-audit after future scene-generator or HUD changes. |
| Android UX | Metric tests pass for key layout risks; UI-R2 through UI-R8 code passes are complete, device validation is open. | Run `Docs/Android UI Validation Log.md` and portrait device matrix. |
| Android build | Current Unity IAP project builds a development Android APK in batchmode. | Rebuild after future package/build-setting changes; signed RC build remains future work. |
| Production IAP | Provider/package/product-id checks exist; backend validation is accepted as out of scope, internal-track tests are open. | Keep public IAP gated until RB-005 Google Play internal-track testing passes. |
| Presentation | Runtime gameplay sprites accepted; UI release polish and several authored assets are still open. | Execute `Docs/UI Release Audit.md` and decide minimum shippable asset bar. |

## Update Rules

- Update this tracker after every Play Mode regression, Android build, device UI pass, IAP test, or release-scope decision.
- Keep deferred items visible so they are not mistaken for implemented features.
- A release candidate should not be cut while any `Open`, `Needs Device`, or `Needs Unity Editor` blocker remains unless the release owner explicitly accepts that risk in this file.
- `Accepted Risk` items must state the tradeoff and the fallback plan.
