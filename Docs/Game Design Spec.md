# Eco Garden - Game Design Spec

Source: `Docs/Unity 2D AI Architect-saved.html`

## 1. Vision

Eco Garden is a relaxing 2D grid-based puzzle merge game for Android, built in Unity 6. The core fantasy is tending a calm lotus pond by producing, merging, clearing, and delivering evolved lotus items in a pastel zen environment.

The first implementation target is a playable vertical slice centered on Level 15, "The Lotus Pond Corner".

## 2. Product Targets

| Area | Decision |
| --- | --- |
| Genre | 2D puzzle merge |
| Platform | Android |
| Engine | Unity 6 |
| Visual style | Pastel Zen, foggy pond background |
| Session length | 2-5 minutes per level |
| Monetization | Hybrid IAP and rewarded ads |
| Primary player action | Drag items on an 8x8 grid to merge and complete NPC orders |

## 3. Core Gameplay Loop

1. Player taps the Producer to create low-level lotus items.
2. Player drags items across valid cells.
3. Matching items merge into higher-level lotus items.
4. Player clears obstacles or manages limited space using abilities.
5. Player fulfills NPC orders with requested evolved items.
6. Level ends when all required orders are fulfilled.
7. Player earns gold and progression rewards.

## 4. Merge Rules

Initial implementation decisions:

| Rule | Decision |
| --- | --- |
| Merge count | 2 identical items merge into 1 next-level item |
| Input | Drag-and-drop item A onto item B |
| Valid merge | Same item family and same level |
| Output position | Target cell receives upgraded item |
| Source position | Source cell becomes empty |
| Max level | Level 5 item cannot merge further |
| Invalid drop | Item returns to source cell |
| Empty drop | Item moves to target cell |
| Locked cell | Cannot receive or contain movable items |
| Obstacle cell | Blocks movement unless cleared |

## 5. Item Evolution Chain

| Level | Item | Sell Value |
| --- | --- | --- |
| 1 | Dried Seed | 1 gold |
| 2 | Sprout | 3 gold |
| 3 | Baby Leaf | 8 gold |
| 4 | Flower Bud | 20 gold |
| 5 | Blooming Lotus | 50 gold |

Item family id: `lotus`

## 6. Grid Model

The board uses integer coordinates `(x, y)`:

| Property | Decision |
| --- | --- |
| Width | 8 |
| Height | 8 |
| Origin | Bottom-left cell is `(0, 0)` |
| Top row | `y = 7` |
| Right column | `x = 7` |
| Cell content | One board entity per cell |

Cell symbols:

| Symbol | Meaning |
| --- | --- |
| `-` | Empty playable cell |
| `L` | Locked cell |
| `W` | Weed obstacle |
| `P` | Pebble obstacle |
| `S` | Producer |
| `NPC` | Customer order point |
| `1` | Pre-placed Lotus level 1 |
| `2` | Pre-placed Lotus level 2 |

## 7. Level 15: The Lotus Pond Corner

Level identity:

| Field | Value |
| --- | --- |
| Level id | `15` |
| Name | The Lotus Pond Corner |
| Board size | 8x8 |
| Theme | Pastel Zen |
| Background | Foggy pond |
| Design intent | Spatial optimization challenge |

Grid layout, listed from top row `y=7` to bottom row `y=0`:

```text
y7: L L - - - - L L
y6: L - - 2 1 - - L
y5: - - W - - W - -
y4: S - P P P P - NPC
y3: - - P P P P - -
y2: - - W - - W - -
y1: L - - 1 1 - - L
y0: L L - - - - L L
```

Coordinates:

| Entity | Coordinates |
| --- | --- |
| Locked cells | `(0,7)`, `(1,7)`, `(6,7)`, `(7,7)`, `(0,6)`, `(7,6)`, `(0,1)`, `(7,1)`, `(0,0)`, `(1,0)`, `(6,0)`, `(7,0)` |
| Producer | `(0,4)` |
| NPC order point | `(7,4)` |
| Weed obstacles | `(2,5)`, `(5,5)`, `(2,2)`, `(5,2)` |
| Pebble obstacles | `(2,4)`, `(3,4)`, `(4,4)`, `(5,4)`, `(2,3)`, `(3,3)`, `(4,3)`, `(5,3)` |
| Pre-placed items | Lv2 at `(3,6)`, Lv1 at `(4,6)`, Lv1 at `(3,1)`, Lv1 at `(4,1)` |

## 8. Level 15 Objective

The first implementation target should use this explicit objective:

| Objective | Quantity |
| --- | --- |
| Deliver Blooming Lotus, Lv5 | 1 |

The NPC waits up to 180 seconds. If the timer expires, the level fails. If the player delivers the requested item before the timer expires, the level is completed.

## 9. Producer Rules

| Rule | Value |
| --- | --- |
| Producer id | `lotus_seed_producer` |
| Location | `(0,4)` |
| Spawn item | Lotus level 1 |
| Spawn trigger | Tap/click producer |
| Spawn cooldown | 1.0 second |
| Spawn placement | Nearest empty playable cell around producer using breadth-first search |
| Spawn blocked | If no empty playable cell exists, show blocked feedback and do not spawn |
| Spawn cost | 0 gold for vertical slice |

Later balance can add cost, energy, random spawn tables, and producer upgrades.

## 10. Obstacles

| Obstacle | Symbol | Behavior |
| --- | --- | --- |
| Weed | `W` | Blocks cell; removable with Shovel |
| Pebble | `P` | Blocks cell; removable with Shovel |

Initial vertical slice:

| Rule | Decision |
| --- | --- |
| Weed HP | 1 |
| Pebble HP | 1 |
| Merge interaction | Obstacles do not merge |
| Shovel interaction | Removes selected obstacle cell |
| Gold reward | 0 |

## 11. Abilities

### Shovel

Removes one obstacle from a selected board cell.

| Property | Value |
| --- | --- |
| Target | `W` or `P` cell |
| Consumed on use | Yes |
| Level 15 starting count | 2 |
| Required tutorial emphasis | High |

### Magic Wand

Upgrades one selected lotus item by one level.

| Property | Value |
| --- | --- |
| Target | Lotus level 1-4 |
| Invalid target | Lotus level 5, obstacle, locked, empty, producer, NPC |
| Consumed on use | Yes |
| Level 15 starting count | 1 |

### Sorting Magnet

Moves matching loose items closer together automatically.

| Property | Value |
| --- | --- |
| Target | Item family, default lotus |
| Behavior | Moves same-level pairs into adjacent empty cells when possible |
| Consumed on use | Yes |
| Level 15 starting count | 1 |

## 12. NPC and AI Behaviors

### Customer NPC

| Field | Value |
| --- | --- |
| Spawn position | `(-1,4)` world/grid-adjacent entry |
| Destination/order point | `(7,4)` |
| Wait time | 180 seconds |
| Order | 1x Lotus Lv5 |
| Fulfillment | Drag requested item onto NPC/order point |
| Completion behavior | NPC exits and level completes |

Future order flow should support multi-item NPC orders:

1. An NPC order can request one or more item requirements, such as `2x Lotus Lv2`.
2. Each valid delivered item is consumed immediately and increments submitted progress.
3. The objective panel shows requirement progress such as `Lotus Lv2 1/2`.
4. When all requirements are submitted, the NPC moves to a checkout point near the Sell Basket area.
5. Completion rewards are granted after the order completes.
6. The NPC returns to the delivery position or respawns from its entry path.
7. The next order for the current level appears.
8. The level completes when all level orders are fulfilled.

The checkout movement is visual and reward-driven. It must not reuse Sell Basket item-sale logic, because selling player items and fulfilling NPC orders are separate systems.

### Zen Butterflies

Butterflies are cosmetic AI and do not affect gameplay.

| Butterfly | Behavior |
| --- | --- |
| A | Loops through `(1,1) -> (2,5) -> (6,5) -> (5,1)` |
| B | Hovers around any existing Lv5 Blooming Lotus |

Movement style: float-weighted lerp with light sine offset for wing-like motion.

## 13. Economy

Initial vertical slice:

| Currency | Source | Use |
| --- | --- | --- |
| Gold | Selling plants, normal mission rewards, NPC order rewards, level completion | Booster purchases, selected unlocks, selected decorations |
| Gem | Rare missions, special rewards, IAP | Premium boosters, premium decorations, faster unlocks, bundles |

Gold is the normal earned currency. Selling plants always grants Gold, never Gem.

Gem is the premium currency. It should be scarce in regular play and primarily come from special missions/events or IAP. Core level completion must not require Gem.

Lotus sell values are defined in Gold.

### Sell Basket

The game should support an external drop zone outside the board where players can sell unwanted plant items to free board space.

| Rule | Decision |
| --- | --- |
| Zone name | Sell Basket |
| Location | Outside the 8x8 board, visually separated from playable cells |
| Accepted items | Movable plant/lotus items |
| Rejected items | Obstacles, producer, NPC/order point, locked cells |
| Result | Item is removed from board and gold is added |
| Gold value | Uses `ItemDefinition.sellValue` |
| Feedback | Item flies into basket, disappears, gold coins emit from basket, gold counter updates |

Design constraints:

1. The Sell Basket must not be visually confused with NPC delivery.
2. The Sell Basket should be far enough from the board to reduce accidental sales.
3. High-value item sale confirmation can be added later; prototype can sell immediately.
4. Selling must be optional and not required for Level 15 completion.

### External Drop Zones

External drop zones are non-board targets that can receive dragged items. They should be implemented as reusable interaction targets so future features can share the same drag/drop path.

| Zone | Purpose |
| --- | --- |
| Sell Basket | Sell unwanted items for gold |
| NPC Delivery Zone | Deliver requested order items |
| Future Storage Basket | Temporarily hold selected items outside board |
| Future Event Collector | Submit event-specific items |

Initial implementation should include Sell Basket as the first external drop zone. NPC delivery can remain on-board for Level 15, but the architecture should allow moving delivery outside the board later.

## 14. Monetization Boundaries

Monetization should not be required for Level 15 completion.

Acceptable later hooks:

| Hook | Notes |
| --- | --- |
| Rewarded ad | Gain 1 Shovel, Magic Wand, or Magnet |
| IAP | Booster packs |
| Soft currency | Buy boosters with earned gold |

Design constraint: the first playable implementation must be completable without ads or IAP.

## 15. Shop, Missions, and IAP Expansion

Status: In implementation. Shop, missions, mock IAP, and mission UI are implemented for the vertical slice; production Android IAP remains a setup task.

The next product expansion adds a lightweight meta layer around the Level 15 vertical slice. It should remain optional for level completion and should not block the core merge loop.

### Currency Rules

| Currency | Acquisition | Primary Uses | Constraints |
| --- | --- | --- | --- |
| Gold | Sell plants, complete normal NPC orders, complete normal missions, level rewards | Common boosters, some decorations, basic unlocks | Earnable through regular play |
| Gem | Rare/high-difficulty missions, events, IAP | Premium cosmetics, premium booster bundles, optional faster unlocks | Never earned from selling plants |

Currency design constraints:

1. Gold and Gem must be displayed separately in UI.
2. Every shop item must declare exactly one purchase currency or IAP product id.
3. Failed purchases never modify Gold, Gem, boosters, unlocks, or save data.
4. Level objectives must remain completable without Gem or IAP.

### Shop Items

The shop sells player-helping items, not required progression gates.

| Category | Example Items | Purchase Currency | Purpose |
| --- | --- | --- | --- |
| Booster items | Shovel pack, Magic Wand pack, Sorting Magnet pack | Gold, Gem, or IAP bundle | Helps complete levels |
| Decoration items | Butterfly skin, bird visitor, board skin, NPC skin, pond background | Gold or Gem | Cosmetic customization |
| Unlock items | Plant tier unlock, producer upgrade unlock | Gold or Gem | Progression acceleration |
| Currency packs | Small/medium Gem pack, optional Gold bundle | IAP | Monetization and convenience |
| Bundles | Starter bundle, booster bundle, decoration bundle | Gem or IAP | Combined value packs |

Initial shop catalog proposal:

| Shop Item | Category | Purchase Currency | Grant |
| --- | --- | --- | --- |
| Small Shovel Pack | Booster | Gold | Shovel count |
| Small Magic Wand Pack | Booster | Gold | Magic Wand count |
| Small Sorting Magnet Pack | Booster | Gold | Sorting Magnet count |
| Premium Booster Bundle | Booster | Gem or IAP | All booster counts |
| Butterfly Decoration | Decoration | Gold | Cosmetic butterfly variant |
| Bird Visitor Decoration | Decoration | Gem | Cosmetic ambient visitor |
| Board Skin: Moss Stone | Decoration | Gem | Board tile skin |
| NPC Skin: Traveler | Decoration | Gem | NPC appearance skin |
| Unlock Lotus Tier 4 | Unlock | Gold or Gem | Allows Lv4 Lotus creation/orders |
| Unlock Lotus Tier 5 | Unlock | Gold or Gem | Allows Lv5 Lotus creation/orders |
| Small Gem Pack | Currency | IAP | Gem |
| Medium Gem Pack | Currency | IAP | Gem |

Shop design constraints:

1. Shop purchases must be data-driven so item price, quantity, product id, and display name can be edited without code changes.
2. Shop UI must clearly distinguish Gold, Gem, and real-money purchases.
3. Failed purchases must not change save data.
4. Restore purchase support is required before production release for any non-consumable product.
5. The first implementation can use a mock purchase provider in Editor and a real IAP provider later.

### Plant Tier Unlocks

Higher-tier plants should unlock over progression instead of being fully available from the start.

| Unlock | Effect | Proposed Unlock Source |
| --- | --- | --- |
| Lotus Lv1-Lv3 | Basic merge chain | Available by default |
| Lotus Lv4 | Allows merging into Lv4 and receiving Lv4 orders | Level progression, Gold, or mission reward |
| Lotus Lv5 | Allows merging into Lv5 and receiving Lv5 orders | Later level progression, Gold/Gem, or special mission reward |

Rules:

1. Locked tiers cannot be produced by merge unless the level explicitly grants a temporary tutorial override.
2. NPC orders should not request locked tiers unless the level is designed to unlock that tier during the level.
3. Locked tier UI should explain the unlock requirement.
4. Unlock state is saved per item family and tier.

### Mission List and Mission Rewards

Missions provide short-term goals and rewards outside the single NPC order.

| Mission Type | Example | Progress Source | Difficulty | Reward |
| --- | --- | --- | --- | --- |
| Merge count | Merge 5 Lotus items | Board merge events | Easy | Gold |
| Produce count | Spawn 10 Lotus seeds | Producer spawn events | Easy | Gold |
| Sell count | Sell 3 items | Sell Basket events | Normal | Gold |
| Deliver order | Deliver 2 Lotus Lv2 | NPC order completion event | Normal | Gold or booster |
| Use ability | Use Shovel 2 times | Ability success events | Normal | Gold |
| High-tier delivery | Deliver Lotus Lv5 | NPC order completion event | Hard | Gold, Gem, or booster |

Mission rules:

1. Mission progress is event-driven from gameplay systems.
2. A mission can be `Locked`, `Active`, `Completed`, or `Claimed`.
3. Rewards are claimed manually from the mission list to keep reward feedback clear.
4. Claimed rewards are persisted in save data.
5. Initial implementation should support daily-style and static missions, but only static missions are required for the vertical slice.

### NPC Order Difficulty and Rewards

NPC orders should scale with level difficulty.

| Difficulty | Order Shape | Board Pressure | Reward |
| --- | --- | --- | --- |
| Easy | Low tier, low quantity, such as `2x Lotus Lv2` | Few obstacles, few locked cells | Gold |
| Normal | Mid tier or multiple requirements | Moderate obstacles and temporary locks | Gold plus small booster chance |
| Hard | High tier or higher quantity | More obstacles, fewer open cells, tighter timer | Gold plus booster or rare Gem |
| Expert | Multiple high-tier requirements | Many obstacles/locks, short timer | Gold plus Gem or premium reward |

Reward rules:

1. Reward value should scale with item level, quantity, board pressure, and timer pressure.
2. Gem rewards should be rare and reserved for high difficulty missions/orders.
3. Completing an NPC order grants rewards automatically after order completion feedback.
4. Mission rewards remain manually claimable from the mission list.

### Level Difficulty Scaling

Level data should scale difficulty through board layout and order requirements.

| Lever | Easy | Normal | Hard | Expert |
| --- | --- | --- | --- | --- |
| Obstacle count | Low | Moderate | High | High |
| Locked cells | Few | Some | Many | Many |
| Temporary locked cells | None | Few | Some | Many |
| Order count | 1 | 2-3 | 3-4 | 4+ |
| Requested item level | Lv2-Lv3 | Lv3-Lv4 | Lv4-Lv5 | Lv5+ future families |
| Requested quantity | 1-2 | 2-3 | 3-4 | 4+ |
| Timer pressure | Loose | Moderate | Tight | Very tight |

Temporary locked cells are cells that start unavailable but can unlock during the level through order completion, timer events, or paid/unpaid unlock mechanics.

### IAP Mechanism

IAP should be abstracted behind a provider interface so gameplay code never calls store SDK APIs directly.

| Layer | Responsibility |
| --- | --- |
| Product catalog | Defines product ids, type, price label, and grants |
| Purchase provider | Starts purchase, receives success/failure callbacks |
| Receipt validator | Validates receipts when production backend or SDK support is available |
| Grant service | Applies purchased currency/boosters once per successful transaction |
| Save service | Persists purchase grants and inventory changes |
| Shop UI | Shows products, purchase buttons, pending state, and errors |

Initial implementation decisions:

1. Editor uses a mock IAP provider that can simulate success, cancel, and failure.
2. Runtime Android implementation will target Unity IAP behind the existing `IIapProvider` boundary unless a different store SDK is chosen before production store integration.
3. Consumable products can grant Gem, Gold, boosters, or bundles.
4. Non-consumable products are not required for the first IAP pass.
5. Purchase grants must be idempotent by transaction id when transaction ids are available.
6. Real-money purchase support must remain optional; Level 15 and core objectives must be completable without IAP.

Initial Android store product ids:

| Store Product ID | Type | Grant |
| --- | --- | --- |
| `eco_garden_gems_small` | Consumable | 80 Gem |
| `eco_garden_gems_medium` | Consumable | 220 Gem |

## 16. UX Requirements

Required game screen elements:

| UI | Purpose |
| --- | --- |
| Board | 8x8 interactable grid |
| Timer | Shows NPC wait time |
| Objective panel | Shows required item and quantity |
| Booster bar | Shovel, Magic Wand, Sorting Magnet counts |
| Gold display | Shows current earned currency |
| Gem display | Shows current premium currency |
| Sell Basket | External drop target for selling unwanted items |
| Shop button | Opens shop item list |
| Mission button | Opens mission list and claimable rewards |
| Pause button | Opens pause menu |
| Feedback text/toast | Invalid move, board full, objective complete |

Required interaction feedback:

| Action | Feedback |
| --- | --- |
| Valid drag | Target cell highlight |
| Invalid drag | Return animation |
| Merge | Small scale pop and sparkle |
| Producer tap | Spawn pulse |
| Ability selected | Board target highlights |
| Objective delivered | NPC happy animation and completion panel |
| Partial NPC delivery | Objective panel updates submitted count |
| NPC order completed | NPC moves to checkout, reward is granted, next order appears |
| Item sold | Item flies to basket, coin burst appears, gold counter increments |
| Mission completed | Badge or toast appears on mission button |
| Reward claimed | Currency or booster count animates to HUD |
| Purchase completed | Shop closes or product row confirms grant |
| Purchase failed/cancelled | Non-blocking message, no inventory change |

## 17. Audio Requirements

Minimum audio set:

| Event | Audio |
| --- | --- |
| Item pickup/drop | Soft click |
| Merge | Gentle chime |
| Producer spawn | Water drop |
| Ability use | Magical soft burst |
| Objective complete | Warm success sting |
| Timer warning | Subtle tick or pulse under 20 seconds |
| Mission reward claim | Soft reward chime |
| Shop purchase success | Clear but calm confirmation |

## 18. Save Data Requirements

For the vertical slice, save:

| Field | Purpose |
| --- | --- |
| Highest unlocked level | Progression |
| Gold | Currency |
| Gem | Premium currency |
| Booster counts | Inventory |
| Settings | Audio and haptics preferences |
| Active missions | Mission progress, completion, and claimed state |
| Active NPC order | Current order id and submitted requirement counts |
| Plant tier unlocks | Unlocked item family/tier state |
| Decoration ownership | Owned board/NPC/ambient cosmetics |
| Shop inventory grants | Purchased/claimed booster, Gold, Gem, decoration, and unlock changes |
| Processed transactions | IAP transaction ids already granted when available |

Board-state resume can be deferred unless mid-level persistence is required.

## 19. Acceptance Criteria

Level 15 is accepted when:

1. The board loads exactly from the defined 8x8 layout.
2. Player can tap the producer to spawn Lotus Lv1 into valid empty cells.
3. Player can drag items to move and merge valid pairs.
4. Invalid moves return the item to its original cell.
5. Shovel removes `W` and `P` obstacle cells.
6. Magic Wand upgrades Lotus Lv1-4 by one level.
7. Sorting Magnet moves at least one matching pair closer when space exists.
8. NPC timer starts at 180 seconds.
9. Delivering 1 Lotus Lv5 to the NPC completes the level.
10. Timer reaching zero before delivery fails the level.
11. The level can be completed without ads or IAP.
12. The implementation runs on Android target settings without blocking editor errors.
