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
| Gold | Level completion, item sell value later | Future producer costs, boosters, unlocks |

Lotus sell values are defined, but selling can be deferred until after the vertical slice.

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

## 15. UX Requirements

Required game screen elements:

| UI | Purpose |
| --- | --- |
| Board | 8x8 interactable grid |
| Timer | Shows NPC wait time |
| Objective panel | Shows required item and quantity |
| Booster bar | Shovel, Magic Wand, Sorting Magnet counts |
| Currency display | Shows current gold |
| Sell Basket | External drop target for selling unwanted items |
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
| Item sold | Item flies to basket, coin burst appears, gold counter increments |

## 16. Audio Requirements

Minimum audio set:

| Event | Audio |
| --- | --- |
| Item pickup/drop | Soft click |
| Merge | Gentle chime |
| Producer spawn | Water drop |
| Ability use | Magical soft burst |
| Objective complete | Warm success sting |
| Timer warning | Subtle tick or pulse under 20 seconds |

## 17. Save Data Requirements

For the vertical slice, save:

| Field | Purpose |
| --- | --- |
| Highest unlocked level | Progression |
| Gold | Currency |
| Booster counts | Inventory |
| Settings | Audio and haptics preferences |

Board-state resume can be deferred unless mid-level persistence is required.

## 18. Acceptance Criteria

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
