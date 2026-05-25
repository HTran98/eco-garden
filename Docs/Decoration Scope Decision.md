# Eco Garden - Decoration Scope Decision

Date decided: 2026-05-25

## Decision

Decoration purchases are deferred from the first release.

Decoration `ShopItemDefinition` assets remain in the project as future content, and owned decoration ids remain supported in save data for compatibility. For the first release, `ShopCatalogService` excludes `ShopItemCategory.Decoration` items by default so players cannot spend Gold or Gem on cosmetics that do not yet change the visible game.

## Rationale

- Current decoration ownership is saved, but there is no complete runtime cosmetic application path for board skins, NPC skins, ambient visitors, or butterfly variants.
- Selling invisible cosmetics would create a real economy sink without player-facing value.
- Removing save support would risk old test saves and future content work, so the safer first-release choice is to keep ownership persistence but hide the products from the active catalog.

## First-Release Behavior

- Decoration products do not appear in the runtime shop catalog by default.
- Direct purchase attempts through `ShopController.TryPurchase` return product not found for deferred decoration product ids.
- Existing saves with `ownedDecorationIds` still load and re-save safely.
- Decoration category UI may remain as a reserved empty category until the final shop UI pass removes or relabels it.

## Re-Enable Requirements

Before enabling decoration catalog items, add:

- A runtime cosmetic application path for each purchasable decoration type.
- UI state that shows equipped/owned cosmetics separately from one-time purchase state.
- Save fields for equipped decoration choices if ownership alone is not enough.
- EditMode or PlayMode coverage proving purchased cosmetics are visible after restart.
