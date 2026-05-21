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
| Persistent processed transaction ids | Not implemented yet |
| Unity IAP package | Not installed |
| Android store provider | Deferred |
| Receipt validation | Deferred |

`Eco-Garden/Packages/manifest.json` does not currently include `com.unity.purchasing`.

## Required Store Product IDs

| Product ID | Type | Store Type | Grant |
| --- | --- | --- | --- |
| `eco_garden_gems_small` | Consumable | Google Play managed product | 80 Gem |
| `eco_garden_gems_medium` | Consumable | Google Play managed product | 220 Gem |

The shop catalog maps these store ids through `ShopPriceDefinition.iapProductId`.

## Android Production Setup Checklist

1. Install Unity IAP in Unity Package Manager:
   `Window > Package Manager > Unity Registry > In-App Purchasing`.
2. Confirm `Packages/manifest.json` contains `com.unity.purchasing`.
3. Link the Unity project to a Unity Project ID if Unity services, IAP analytics, or Google license key setup through the Editor is required.
4. Add Google Play managed products with the product ids listed above.
5. Add `UnityIapProvider` implementing `IIapProvider`.
6. Register consumable products from the shop catalog at provider initialization.
7. Map Unity purchase success, cancel, failure, and unavailable outcomes into `IapPurchaseStatus`.
8. Persist processed transaction ids in save data before enabling real store purchases.
9. Add receipt validation path before production release. Backend validation is preferred for production.
10. Build Android and run a Google Play internal test purchase.

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
| Android build with Unity IAP package | Not run because `com.unity.purchasing` is not installed |

## Open Production Risks

1. Google Play Billing compliance depends on the installed Unity IAP package version at release time.
2. Receipt validation is currently not production-safe.
3. Processed IAP transaction ids are not yet saved across app restarts.
4. Non-consumable restore purchase flow is not implemented.
5. Store product ids must exactly match Google Play Console configuration before device testing.
