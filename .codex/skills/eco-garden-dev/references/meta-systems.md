# Eco Garden Meta Systems Reference

Use this reference for Economy, Rewards, Shop, Missions, IAP, Save, Progression, and Android release work.

## Economy and Rewards

- Gold is earned currency from selling plants, normal orders, and normal missions.
- Gem is premium currency from rare rewards, events, or IAP.
- Reward data can grant currency, abilities, decorations, and plant tier unlocks.
- Spend/earn paths must not allow negative balances.
- Rewards must be applied through shared services, not duplicated in UI controllers.

## Orders and Missions

- NPC orders can have multiple item requirements with submitted counts.
- Delivery consumes matching requested items one at a time.
- Order completion grants configured rewards and advances NPC checkout/return behavior.
- Missions progress from successful gameplay events only: merge, produce, sell, deliver, and ability use.
- Mission rewards must be claimable once and persisted.

## Shop

- Shop catalog is data-driven through shop item ScriptableObjects.
- Shop purchases validate price, purchase kind, repeatability, inventory, and reward grant.
- Gold/Gem purchase failures must return explicit statuses.
- Non-repeatable products and owned decorations must persist.

## IAP

- Keep all store-specific logic behind `IIapProvider`.
- Editor and vertical-slice flow uses `MockIapProvider`.
- Android production should use Unity IAP through a future `UnityIapProvider`.
- Required product ids:
  - `eco_garden_gems_small`
  - `eco_garden_gems_medium`
- Persist processed transaction ids before enabling real purchases.
- Receipt validation remains a production release decision; document the selected approach before release.

## Save Compatibility

- New fields require safe defaults when absent.
- Old save data must not block startup or zero out critical player inventory unexpectedly.
- Critical state includes Gold, Gem, boosters, board items, order progress, missions, shop inventory, owned decorations, plant tier unlocks, settings, and processed purchase ids.

## Android Release Gates

- Portrait layout must avoid overlap with board, sell basket, delivery zone, shop, missions, and result panels.
- Product ids must match Google Play Console exactly.
- Internal test purchase must verify success, cancel/fail, app restart, and duplicate transaction behavior.
