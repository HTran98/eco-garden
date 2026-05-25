# Eco Garden - IAP Integration Decision

Date: 2026-05-20

## Decision

Use the current `IIapProvider` boundary for all shop purchase calls.

For the vertical slice and Editor testing, keep `MockIapProvider` as the active provider. For Android release builds, use Unity In-App Purchasing through `UnityIapProvider` behind the same interface.

The current product direction is client-only Unity IAP for this game. No custom backend/server will be created for the first release. This means real-money IAP grants are trusted from Unity IAP callbacks plus local processed-transaction persistence. The security tradeoff is accepted for this release scope.

Unity IAP is the preferred Android provider because Unity documents it as a unified purchasing system for multiple stores, including Google Play, and the package is installed through Package Manager as `com.unity.purchasing`.

References:

- Unity IAP overview: https://docs.unity.com/en-us/iap
- Unity IAP setup guide: https://docs.unity.com/en-us/iap/get-started

## Current Project State

| Area | Status |
| --- | --- |
| Mock IAP provider | Implemented |
| Shop UI purchase path | Connected to mock IAP |
| IAP product catalog rows | Implemented as shop products |
| Runtime grant path | Implemented through `IapPurchaseService` and `RewardService` |
| Duplicate transaction protection | Implemented in runtime service memory |
| Persistent processed transaction ids | Implemented in save data |
| Unity IAP package | Installed as `com.unity.purchasing` 5.3.0 |
| Android store provider | First-pass `UnityIapProvider` implemented |
| Receipt validation | Backend validation deferred; client receipt payload capture and optional validation hook are implemented |

`Eco-Garden/Packages/manifest.json` includes `com.unity.purchasing`; `packages-lock.json` resolves Unity IAP 5.3.0 and Unity Services Core 1.16.0.

## Required Store Product IDs

| Product ID | Type | Store Type | Grant |
| --- | --- | --- | --- |
| `eco_garden_gems_small` | Consumable | Google Play managed product | 80 Gem |
| `eco_garden_gems_medium` | Consumable | Google Play managed product | 220 Gem |

The shop catalog maps these store ids through `ShopPriceDefinition.iapProductId`.

## Android Production Setup Checklist

1. Install Unity IAP in Unity Package Manager:
   `Window > Package Manager > Unity Registry > In-App Purchasing`.
2. Confirm `Packages/manifest.json` contains `com.unity.purchasing`. Completed.
3. Link the Unity project to a Unity Project ID if Unity services, IAP analytics, or Google license key setup through the Editor is required.
4. Add Google Play managed products with the product ids listed above.
5. Add `UnityIapProvider` implementing `IIapProvider`. Completed first pass.
6. Register consumable products from the shop catalog at provider initialization. Current provider registers the two release product ids; next pass should derive them from catalog data or serialized release-scene config.
7. Map Unity purchase success, cancel, failure, and unavailable outcomes into `IapPurchaseStatus`. First-pass mapping exists for pending, success, cancel, unavailable, duplicate, and generic failure.
8. Persist processed transaction ids in save data before enabling real store purchases. Completed.
9. Keep backend receipt validation disabled unless a future release adds a server. Client-only Unity IAP is the accepted first-release path.
10. Build Android and run a Google Play internal test purchase.

## Receipt Validation Decision

Production Android releases can use backend-backed receipt validation when a server exists, but this game is currently not planning a backend. First release uses Unity IAP client-side purchase callbacks with local duplicate transaction protection.

Local/client-side IAP is not as secure as server validation. It can be affected by client tampering or save manipulation, and duplicate protection is limited to the local persisted processed transaction ids. The release owner accepts that tradeoff to avoid operating a backend for this game.

Selected release path:

1. Client starts purchase through `UnityIapProvider`.
2. Unity IAP returns store product id, transaction id, and receipt payload.
3. `IapPurchaseService` checks the local processed transaction id set to prevent duplicate grant in the current save.
4. Client applies the configured shop reward on successful Unity IAP completion.
5. Client persists processed transaction ids locally as duplicate protection across app restarts.
6. Google Play internal-track purchase testing must pass before public release.

Future backend option:

1. Add a backend validation endpoint and server-side transaction idempotency.
2. Wire `BackendIapReceiptValidator` or a replacement `IIapReceiptValidator` into the release scene.
3. Require backend approval before `IapPurchaseService` grants rewards.

Production blocker status:

| Blocker | Status | Release Requirement |
| --- | --- | --- |
| Backend receipt validation endpoint | Deferred | No custom backend/server is planned for this game. Revisit only if IAP fraud risk becomes unacceptable. |
| Client-side backend validation hook | Future Option | `IIapReceiptValidator` can block grants until a configured validator approves a receipt, but it should not be wired into first-release scenes without a real backend. |
| Receipt payload capture in `UnityIapProvider` result model | Closed | `IapPurchaseResult` and `IapProductPurchaseResult` expose receipt payloads; `UnityIapProvider` captures Unity IAP `order.Info.Receipt`. |
| Server-side transaction id idempotency | Deferred | Not available without a backend; local processed transaction ids remain the first-release duplicate protection. |
| Google Play internal-track purchase validation | Open | Required before release candidate. |
| Client-only Unity IAP purchase path | Accepted | First-release real-money IAP authority is Unity IAP callback plus local duplicate transaction persistence. |

## Product ID Verification

Automated checks:

- Unity menu: `Eco Garden/Validation/Audit IAP Catalog`
- EditMode test: `EcoGarden.Tests.EditMode.IapCatalogAuditTests.ShopCatalog_UsesRequiredGooglePlayProductIds`

The audit loads `Assets/EcoGarden/ScriptableObjects/Shop`, finds IAP shop items, and verifies:

- Required ids exist: `eco_garden_gems_small`, `eco_garden_gems_medium`.
- IAP ids are not empty.
- IAP ids are documented release ids.
- Duplicate IAP ids are reported.

## Build Check

Current code build check:

| Check | Result |
| --- | --- |
| Runtime assembly | Pass |
| Editor assembly | Pass |
| EditMode test assembly | Pass |

Android store build check:

| Check | Result |
| --- | --- |
| Existing Android build without Unity IAP | Previously confirmed manually in Unity Editor |
| Android build with Unity IAP package | Not run; package and assemblies compile, device/internal-track test remains required |

## Open Production Risks

1. Google Play Billing compliance depends on the installed Unity IAP package version at release time.
2. Client-only IAP is weaker than server validation; receipt fraud and save tampering risk are accepted for first release.
3. Backend validation hook exists but should remain unwired unless a future backend is built.
4. `UnityIapProvider` is not yet wired into the release scene; the vertical-slice scene still uses `MockIapProvider` for editor testing.
5. Non-consumable restore purchase flow is not implemented.
6. Store product ids must exactly match Google Play Console configuration before device testing.
7. Android internal-track purchase, cancel/fail, restart persistence, and duplicate transaction checks are not yet run on device.
