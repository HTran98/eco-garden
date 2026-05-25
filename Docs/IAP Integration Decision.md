# Eco Garden - IAP Integration Decision

Date: 2026-05-20

## Decision

Use the current `IIapProvider` boundary for all shop purchase calls.

For the vertical slice and Editor testing, keep `MockIapProvider` as the active provider. For Android production, use Unity In-App Purchasing through a future `UnityIapProvider` implementation behind the same interface.

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
| Receipt validation | Backend validation required before production release |

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
9. Add receipt validation path before production release. Backend validation is required for production.
10. Build Android and run a Google Play internal test purchase.

## Receipt Validation Decision

Production Android releases must use backend-backed receipt validation before any real-money IAP is enabled outside internal testing.

Local/client-side validation is acceptable only for prototype and Google Play internal-track testing because it can verify callback flow, transaction persistence, and duplicate-grant protection, but it cannot be treated as a secure production authority. The current first-pass `UnityIapProvider` may continue to pass successful purchase callbacks into `IapPurchaseService`, but a production build must not grant final currency solely from the client receipt path.

Selected release path:

1. Client starts purchase through `UnityIapProvider`.
2. Unity IAP returns store product id, transaction id, and receipt payload.
3. Client sends purchase payload to a backend validation endpoint.
4. Backend validates the receipt with the store authority and checks whether the transaction id was already granted.
5. Backend returns an allow/deny result and granted product payload.
6. Client applies rewards only after the backend approval path succeeds.
7. Client still persists processed transaction ids locally as a duplicate-protection fallback and offline safety measure.

Production blocker status:

| Blocker | Status | Release Requirement |
| --- | --- | --- |
| Backend receipt validation endpoint | Open | Required before production IAP release. |
| Receipt payload capture in `UnityIapProvider` result model | Open | Required before backend validation can be wired. |
| Server-side transaction id idempotency | Open | Required to prevent duplicate grants across reinstall/device changes. |
| Google Play internal-track purchase validation | Open | Required before release candidate. |
| Client-only/mock purchase path | Allowed for Editor/internal testing only | Must not be the production authority for real-money grants. |

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
2. Backend receipt validation is not implemented yet and blocks production IAP.
3. `UnityIapProvider` result data does not yet expose a validated receipt payload for backend handoff.
4. `UnityIapProvider` is not yet wired into the release scene; the vertical-slice scene still uses `MockIapProvider` for editor testing.
5. Non-consumable restore purchase flow is not implemented.
6. Store product ids must exactly match Google Play Console configuration before device testing.
7. Android internal-track purchase, cancel/fail, restart persistence, and duplicate transaction checks are not yet run on device.
