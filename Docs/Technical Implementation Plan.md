# Eco Garden - Technical Implementation Plan

Source design: `Docs/Game Design Spec.md`

## 1. Implementation Goal

Build a Unity 6 vertical slice of Eco Garden centered on Level 15. The goal is a playable, data-driven 8x8 merge board with producer, obstacles, boosters, NPC objective, timer, and completion/failure states.

## 2. Architecture Principles

| Principle | Application |
| --- | --- |
| Data-driven levels | Store level layout and objective outside scene logic |
| Small gameplay services | Keep board, merge, abilities, and objective systems separate |
| Unity-friendly authoring | Use ScriptableObjects for config and prefabs for views |
| Testable rules | Keep core board logic in plain C# classes where possible |
| Mobile-first | Avoid per-frame allocations in board logic and item lookup |

## 3. Recommended Folder Structure

Create these folders under `Eco-Garden/Assets`:

```text
Assets/
  EcoGarden/
    Art/
    Audio/
    Prefabs/
      Board/
      Items/
      UI/
      Characters/
    Scenes/
    Scripts/
      Abilities/
      AI/
      Board/
      Config/
      Economy/
      Missions/
      Shop/
      IAP/
      Input/
      Items/
      Level/
      UI/
      Save/
      Utilities/
    ScriptableObjects/
      Abilities/
      Items/
      Shop/
      Missions/
      Levels/
      Producers/
    Tests/
      EditMode/
      PlayMode/
```

## 4. Data Model

### Core Enums

```csharp
public enum CellKind
{
    Empty,
    Locked,
    Obstacle,
    Producer,
    NpcOrderPoint
}

public enum ObstacleKind
{
    None,
    Weed,
    Pebble
}

public enum AbilityKind
{
    Shovel,
    MagicWand,
    SortingMagnet
}
```

### Runtime Board Types

```csharp
public readonly struct GridPosition
{
    public readonly int X;
    public readonly int Y;
}

public sealed class BoardCell
{
    public GridPosition Position;
    public CellKind Kind;
    public ObstacleKind ObstacleKind;
    public BoardItem Item;
    public ProducerRuntime Producer;
}

public sealed class BoardItem
{
    public string FamilyId;
    public int Level;
    public string ItemId;
}
```

## 5. ScriptableObject Configs

### ItemDefinition

Fields:

| Field | Type |
| --- | --- |
| `itemId` | string |
| `familyId` | string |
| `level` | int |
| `displayName` | string |
| `sellValue` | int |
| `sprite` | Sprite |
| `nextItem` | ItemDefinition |

### ProducerDefinition

Fields:

| Field | Type |
| --- | --- |
| `producerId` | string |
| `spawnItem` | ItemDefinition |
| `cooldownSeconds` | float |
| `spawnCostGold` | int |

### LevelDefinition

Fields:

| Field | Type |
| --- | --- |
| `levelId` | int |
| `levelName` | string |
| `width` | int |
| `height` | int |
| `rowsTopToBottom` | string[] |
| `initialItems` | list of item placements |
| `producerPlacements` | list of producer placements |
| `npcOrder` | order definition |
| `startingAbilities` | list of ability counts |
| `timerSeconds` | float |
| `themeId` | string |

Level 15 should be represented as a `LevelDefinition` asset first. JSON import can be added later if level authoring expands.

## 6. Level 15 Data

Use top-to-bottom rows exactly as follows:

```text
LL----LL
L--21--L
--W--W--
S-PPPP-N
--PPPP--
--W--W--
L--11--L
LL----LL
```

Token mapping:

| Token | Meaning |
| --- | --- |
| `L` | Locked cell |
| `-` | Empty playable cell |
| `W` | Weed obstacle |
| `P` | Pebble obstacle |
| `S` | Producer |
| `N` | NPC order point |
| `1` | Lotus level 1 |
| `2` | Lotus level 2 |

Parser rule: row index `0` in `rowsTopToBottom` maps to board `y = height - 1`.

## 7. Main Runtime Components

### BoardController

Responsibilities:

| Responsibility | Notes |
| --- | --- |
| Load level | Builds runtime board from `LevelDefinition` |
| Coordinate conversion | Grid to world and world to grid |
| Cell lookup | Bounds checks and occupancy checks |
| Move item | Move if target cell is empty and playable |
| Merge item | Merge identical family and level |
| Sell item | Remove item from board and return sell value |
| Notify views | Emits events for spawn, move, merge, remove |

Public methods:

```csharp
bool TryMoveItem(GridPosition from, GridPosition to);
bool TryMergeItem(GridPosition from, GridPosition to);
bool TrySpawnFromProducer(GridPosition producerPosition);
bool TryRemoveObstacle(GridPosition position);
bool TryUpgradeItem(GridPosition position);
bool TrySellItem(GridPosition from, out int goldValue);
BoardCell GetCell(GridPosition position);
```

### BoardView

Responsibilities:

| Responsibility | Notes |
| --- | --- |
| Instantiate cell views | One view per board cell |
| Instantiate item views | One view per board item |
| Animate board events | Move, merge, pop, invalid return |
| Highlight targets | Drag and ability targeting |

### InputController

Responsibilities:

| Responsibility | Notes |
| --- | --- |
| Drag handling | Pick up, drag, drop |
| Tap handling | Producer taps and ability target taps |
| Pointer abstraction | Support mouse in editor and touch on Android |

Use Unity Input System if already enabled in the project. Otherwise, a small pointer abstraction around `Input.mousePosition` and touch can be used for the vertical slice.

### AbilityController

Responsibilities:

| Ability | Implementation |
| --- | --- |
| Shovel | Calls `TryRemoveObstacle` |
| Magic Wand | Calls `TryUpgradeItem` |
| Sorting Magnet | Finds nearest matching pair and moves them adjacent when possible |

### ObjectiveController

Responsibilities:

| Responsibility | Notes |
| --- | --- |
| Track required orders | Supports one or more order requirements per NPC order |
| Validate delivery | Item dragged to Delivery zone is consumed only if it matches an unfilled requirement |
| Track submitted counts | Stores partial progress such as `Lotus Lv2 1/2` |
| Complete order | Fires order completion event and reward grant |
| Advance order sequence | Shows next order after NPC checkout/return flow |
| Complete level | Fires completion event when all level orders are complete |
| Fail level | Fires failure event when timer expires |

### NPC Order Data

Future order definitions should support multi-item requirements and rewards.

Recommended data types:

| Type | Responsibility |
| --- | --- |
| `NpcOrderDefinition` | Defines one NPC order inside a level or order catalog |
| `OrderRequirementDefinition` | Defines required item family, level, and quantity |
| `OrderRuntimeState` | Tracks submitted count for each requirement |
| `OrderRewardDefinition` | Defines automatic rewards for completing the order |
| `LevelOrderSequence` | Holds ordered NPC orders for a level |

Required fields:

| Field | Type |
| --- | --- |
| `orderId` | string |
| `displayName` | string |
| `difficulty` | enum: Easy, Normal, Hard, Expert |
| `requirements` | list of item family/level/count requirements |
| `reward` | reward definition |
| `timerSecondsOverride` | optional float |

Delivery behavior:

1. Matching item delivery removes the item from the board immediately.
2. Submitted counts are persisted so restart does not lose delivery progress.
3. Order completion triggers NPC checkout movement, reward grant, and next-order reveal.
4. NPC checkout is a visual target near Sell Basket, but it must not call Sell Basket sale logic.

### ExternalDropZone

Reusable MonoBehaviour for non-board drag targets.

Responsibilities:

| Responsibility | Notes |
| --- | --- |
| Detect pointer overlap | Supports mouse/touch release over UI or world zone |
| Identify zone type | Sell, delivery, storage, event collector |
| Route item drop | Calls the correct gameplay service |
| Provide feedback anchor | Supplies world/screen position for item fly-in and VFX |

Initial subclasses or modes:

| Zone | Behavior |
| --- | --- |
| SellZone | Accepts movable plant items, removes item, awards gold |
| DeliveryZone | Accepts objective item, completes order |

### EconomyController

Tracks gold and exposes events for UI and VFX.

```csharp
int Gold { get; }
void AddGold(int amount);
bool TrySpendGold(int amount);
```

### Wallet and Currency

Gold remains the normal earned currency. Gem is the premium currency.

Recommended types:

| Type | Responsibility |
| --- | --- |
| `CurrencyKind` | Enum with `Gold` and `Gem` |
| `WalletService` or expanded `EconomyController` | Stores balances and emits balance change events |
| `CurrencyAmount` | Serializable pair of currency kind and amount |
| `RewardDefinition` | Grants Gold, Gem, boosters, decorations, or unlocks |

Rules:

1. Selling board items grants Gold only.
2. Gem comes from rare/high-difficulty rewards, events, or IAP.
3. Shop prices must specify Gold, Gem, or IAP product id explicitly.
4. UI should display Gold and Gem separately.

### Shop System

Status: proposed, awaiting approval before implementation.

The shop should be split into catalog data, purchase flow, and reward granting. Gameplay systems should only receive final granted inventory changes.

Recommended runtime components:

| Component | Folder | Responsibility |
| --- | --- | --- |
| `ShopItemDefinition` | `Scripts/Config` or `Scripts/Shop` | ScriptableObject catalog entry for a shop row |
| `ShopCatalog` | `Scripts/Shop` | Loads and indexes available shop items |
| `ShopController` | `Scripts/Shop` | Handles buy button requests and UI state |
| `PurchaseService` | `Scripts/IAP` | Coordinates provider callbacks and grant application |
| `IIapProvider` | `Scripts/IAP` | Interface for mock and platform purchase providers |
| `MockIapProvider` | `Scripts/IAP` | Editor/test provider for success/cancel/failure |
| `PurchaseGrantService` | `Scripts/IAP` | Adds gold/boosters after successful purchase |
| `ShopView` | `Scripts/UI` | Product list, price labels, pending/error/success states |

`ShopItemDefinition` fields:

| Field | Type |
| --- | --- |
| `shopItemId` | string |
| `displayName` | string |
| `description` | string |
| `category` | enum: Booster, Decoration, Unlock, Currency, Bundle |
| `purchaseKind` | enum: Gold, Gem, IAP |
| `priceCurrency` | CurrencyKind |
| `priceAmount` | int |
| `iapProductId` | string |
| `grantGold` | int |
| `grantGem` | int |
| `grantAbilities` | list of ability/count pairs |
| `grantDecorations` | list of decoration ids |
| `grantPlantTierUnlocks` | list of family/tier pairs |
| `isRepeatable` | bool |
| `sortOrder` | int |

### Mission System

Missions should listen to gameplay events and update persistent progress. Mission rewards are granted only when the player claims them.

Recommended runtime components:

| Component | Folder | Responsibility |
| --- | --- | --- |
| `MissionDefinition` | `Scripts/Config` or `Scripts/Missions` | ScriptableObject mission config |
| `MissionRuntimeState` | `Scripts/Missions` | Current progress/status for one mission |
| `MissionController` | `Scripts/Missions` | Subscribes to gameplay events and updates progress |
| `MissionRewardService` | `Scripts/Missions` | Applies claimed rewards through economy/ability inventory |
| `MissionListView` | `Scripts/UI` | Lists active/completed missions and claim buttons |

`MissionDefinition` fields:

| Field | Type |
| --- | --- |
| `missionId` | string |
| `displayName` | string |
| `description` | string |
| `missionType` | enum: Merge, Produce, Sell, Deliver, UseAbility |
| `targetFamilyId` | string |
| `targetItemLevel` | int |
| `targetAbility` | AbilityKind |
| `requiredCount` | int |
| `rewardGold` | int |
| `rewardGem` | int |
| `rewardAbilities` | list of ability/count pairs |
| `rewardDecorations` | list of decoration ids |
| `rewardPlantTierUnlocks` | list of family/tier pairs |
| `isDaily` | bool |
| `sortOrder` | int |

Gameplay event sources:

| Event | Source |
| --- | --- |
| Item merged | `BoardController` |
| Item produced | `BoardController` producer spawn |
| Item sold | `BoardController` or `BoardInputController` sell path |
| Objective delivered | `BoardController`/`LevelStateController` |
| Ability used | `AbilityHudController` or ability service success path |

### Plant Tier Unlocks

Higher-tier plant availability should be explicit instead of implied by item data.

Recommended components:

| Component | Responsibility |
| --- | --- |
| `PlantTierUnlockDefinition` | Defines unlockable family/tier and display data |
| `PlantUnlockService` | Stores unlocked tiers and answers merge/order availability checks |
| `PlantUnlockView` | Shows locked tier state in shop or progression UI |

Integration points:

1. Merge rules check whether the output tier is unlocked.
2. Order generation/selection does not request locked tiers unless the level grants a temporary override.
3. Shop unlock products can grant family/tier unlocks.
4. Save data stores unlocked family/tier pairs.

### Difficulty Config

Level difficulty should be data-driven so obstacle count, locked cells, temporary locks, order complexity, timer, and rewards can scale predictably.

Recommended types:

| Type | Responsibility |
| --- | --- |
| `DifficultyKind` | Easy, Normal, Hard, Expert |
| `DifficultyDefinition` | Balancing values for board pressure, order complexity, timer, reward multiplier |
| `TemporaryLockDefinition` | Cells that start locked and unlock through level/order events |

Difficulty levers:

| Lever | Controlled By |
| --- | --- |
| Obstacle count | Level layout or generator |
| Locked cells | Level layout |
| Temporary locked cells | Level runtime events |
| Order count | Level order sequence |
| Requested item level and quantity | NPC order definitions |
| Reward value | Reward definition and difficulty multiplier |
| Timer pressure | Level or order timer |

### IAP Provider Boundary

The IAP system must be isolated from gameplay and UI through interfaces.

```csharp
public interface IIapProvider
{
    bool IsProductAvailable(string storeProductId);
    IapPurchaseResult Purchase(string storeProductId);
}
```

Implementation notes:

1. `MockIapProvider` is required first so shop flow can be tested without store setup.
2. Android production provider decision: use Unity IAP through a future `UnityIapProvider` behind this interface.
3. `PurchaseService` validates product id against `ShopCatalog` before purchase.
4. `PurchaseGrantService` should grant once per transaction id when one is provided.
5. All purchase outcomes must be surfaced to UI without blocking gameplay.
6. `com.unity.purchasing` is not installed yet; package installation and Google Play product configuration are production setup tasks.

### SellZoneController

Responsibilities:

| Responsibility | Notes |
| --- | --- |
| Accept sold item | Validates dragged board item |
| Award gold | Uses item sell value |
| Trigger VFX | Coin burst from sell zone |
| Update UI | Gold counter reflects new value |

### TimerController

Counts down from `LevelDefinition.timerSeconds`. Emits warning events below 20 seconds and fail event at zero.

### NpcController

For vertical slice, pathing can be deterministic:

1. Spawn at world position corresponding to grid-adjacent `(-1,4)`.
2. Move to NPC order point `(7,4)`.
3. Idle until the active order is fulfilled or timer expires.
4. Move to checkout point near the Sell Basket after all requirements are submitted.
5. Grant the order reward.
6. Return to delivery position or respawn for the next order.
7. Exit after the final level order is fulfilled.

### ButterflyController

Cosmetic only. It should not depend on board simulation.

## 8. Scene Setup

Create or update a gameplay scene with:

| GameObject | Components |
| --- | --- |
| `GameRoot` | Game bootstrapper, references Level 15 asset |
| `BoardRoot` | BoardController, BoardView |
| `InputRoot` | InputController |
| `UIRoot` | HUDController, ObjectivePanel, AbilityBar, TimerView |
| `AudioRoot` | AudioManager |
| `VfxRoot` | Optional pooled effects |

The scene should not manually place level cells. Board contents must come from `LevelDefinition`.

## 9. Prefabs

Minimum prefabs:

| Prefab | Purpose |
| --- | --- |
| `CellView` | Background tile and highlights |
| `LockedCellView` | Visual for locked cells, or a state on `CellView` |
| `ObstacleView` | Weed/Pebble visual |
| `ItemView` | Sprite renderer and drag visuals |
| `ProducerView` | Producer visual and cooldown feedback |
| `NpcView` | Customer character |
| `ButterflyView` | Cosmetic moving character |
| `FloatingTextView` | Feedback text |

Use placeholder sprites first if final art is unavailable.

## 10. Implementation Milestones

### Milestone 1: Project Structure and Data

Deliverables:

1. Create `Assets/EcoGarden` folder structure.
2. Add core enums and runtime board model.
3. Add item, producer, and level ScriptableObject definitions.
4. Create Lotus Lv1-Lv5 item assets.
5. Create Level 15 asset.

Acceptance:

1. Level 15 data can be parsed into an 8x8 runtime board.
2. Parser rejects rows with invalid width or invalid tokens.

### Milestone 2: Board Rendering

Deliverables:

1. Board scene root.
2. Cell and item view prefabs.
3. BoardView renders Level 15 from data.
4. Grid-to-world coordinate conversion.

Acceptance:

1. Editor play mode shows the exact Level 15 layout.
2. Locked, empty, obstacle, producer, NPC, and item cells are visually distinct.

### Milestone 3: Drag, Move, and Merge

Deliverables:

1. Pointer input controller.
2. Item drag behavior.
3. Move-to-empty-cell rule.
4. Pair merge rule.
5. Invalid drop return animation.

Acceptance:

1. Dragging item to empty playable cell moves it.
2. Dragging Lv1 onto Lv1 creates Lv2.
3. Dragging different levels does not merge.
4. Dragging onto locked/obstacle/producer cells fails cleanly.

### Milestone 4: Producer

Deliverables:

1. Producer tap handling.
2. Cooldown tracking.
3. Breadth-first nearest empty cell spawn.
4. Board-full feedback.

Acceptance:

1. Tapping producer creates Lotus Lv1 near `(0,4)`.
2. Cooldown prevents rapid duplicate spawning.
3. Producer does nothing when no empty playable cell exists.

### Milestone 4.5: External Drop Zones and Sell Basket

Deliverables:

1. External drop zone abstraction.
2. Sell Basket visual placeholder outside the board.
3. Sell item logic in board/economy services.
4. Gold counter update.
5. Basic coin burst placeholder.

Acceptance:

1. Dragging an item into Sell Basket removes it from the board.
2. Gold increases by the sold item's sell value.
3. Dragging non-item board cells cannot trigger sale.
4. Invalid external drops return item to original cell.
5. Sell Basket is visually distinct from NPC delivery.

### Milestone 5: Abilities

Deliverables:

1. Ability selection UI.
2. Shovel removal.
3. Magic Wand upgrade.
4. Sorting Magnet pair movement.
5. Ability inventory counts.

Acceptance:

1. Shovel removes `W` and `P`.
2. Magic Wand upgrades Lotus Lv1-4 only.
3. Sorting Magnet changes board state only when a valid matching pair and destination exist.
4. Ability count decreases only after successful use.

### Milestone 6: NPC Objective and Timer

Deliverables:

1. NPC spawn and movement.
2. Objective panel.
3. Timer countdown.
4. Delivery validation.
5. Win/fail panels.

Acceptance:

1. Timer starts at 180 seconds.
2. Dragging Lotus Lv5 to NPC completes the level.
3. Timer reaching zero fails the level.

### Milestone 7: Polish and Mobile Readiness

Deliverables:

1. Placeholder VFX and SFX.
2. Android aspect ratio checks.
3. Basic save for gold, level unlock, settings, boosters.
4. Performance pass for allocations.

Acceptance:

1. Game is playable with touch input.
2. UI fits common Android portrait resolutions.
3. No recurring GC spikes from board interaction.

### Milestone 8: Shop, Missions, and IAP Planning

Status: documentation only until approved.

Deliverables:

1. Shop item data model and catalog plan.
2. Gold/Gem currency rules and wallet plan.
3. Multi-item NPC order and reward plan.
4. Mission definition, progress tracking, and reward claim plan.
5. Plant tier unlock plan.
6. Difficulty scaling plan.
7. IAP provider abstraction and mock provider plan.
8. Save data extension plan for missions and processed purchases.
9. UI entry point plan for Shop and Missions.

Acceptance:

1. Product/design rules are documented.
2. Architecture boundaries are documented.
3. Implementation tasks are broken down and ready for approval.
4. No runtime implementation starts until approval is given.

## 11. Test Plan

### EditMode Tests

| Test | Expected |
| --- | --- |
| Parse Level 15 rows | Board width 8, height 8 |
| Parse top row | Top row maps to `y=7` |
| Invalid token | Parser returns error |
| Merge same level | Output level increments by 1 |
| Merge max level | Merge rejected |
| Move to empty | Source empty, target occupied |
| Move to locked | Rejected |
| Remove obstacle | Cell becomes empty |
| Upgrade item | Lv1 becomes Lv2 |

### PlayMode Tests

| Test | Expected |
| --- | --- |
| Scene loads Level 15 | All expected cells visible |
| Producer tap | Spawns item |
| Drag merge | Item view updates |
| Deliver Lv5 | Completion panel appears |
| Timer expiry | Failure panel appears |

## 12. Level Parser Pseudocode

```csharp
for (int row = 0; row < rowsTopToBottom.Length; row++)
{
    int y = height - 1 - row;
    string sourceRow = rowsTopToBottom[row];

    for (int x = 0; x < width; x++)
    {
        char token = sourceRow[x];
        GridPosition position = new GridPosition(x, y);
        BuildCellFromToken(position, token);
    }
}
```

## 13. Sorting Magnet Algorithm

Initial simple algorithm:

1. Collect all movable lotus items.
2. Group by item level.
3. Pick the lowest-level group with at least two items.
4. Find the pair with the shortest Manhattan distance.
5. Find an empty playable cell adjacent to the first item.
6. Move the second item to that adjacent cell.
7. If no adjacent empty cell exists, try the reverse direction.
8. If no valid move exists, ability use fails and count is not consumed.

## 14. Performance Notes

| Area | Requirement |
| --- | --- |
| Board size | Small now, but code should support larger boards |
| Cell lookup | Use 2D array or flat array, not scene search |
| Views | Pool item and VFX views if repeated spawning becomes frequent |
| Input | Avoid allocations during drag |
| UI updates | Event-driven, not every-frame text rebuilds except timer |
| Android | Test portrait layout and touch target sizes early |

## 15. Save Extensions for Meta Systems

Additional save fields for the proposed meta systems:

| Field | Purpose |
| --- | --- |
| `missionStates` | Stores mission id, progress, status, and claimed state |
| `processedPurchaseTransactions` | Prevents duplicate IAP grants when transaction ids exist |
| `ownedShopFlags` | Tracks one-time shop items or future non-consumables |
| `lastMissionRefreshUtc` | Supports daily mission refresh later |
| `gem` | Stores premium currency balance |
| `activeOrderState` | Stores current NPC order id and submitted counts |
| `plantTierUnlocks` | Stores unlocked family/tier pairs |
| `ownedDecorations` | Stores cosmetic ownership |

## 16. Known Open Decisions

These should be resolved before production expansion, but are not blockers for the Level 15 vertical slice:

1. Whether selling items is an active player action.
2. Whether producer uses gold, energy, cooldown upgrades, or random spawn tables.
3. Whether obstacles can have HP greater than 1.
4. Whether levels beyond Level 15 use multiple NPC orders.
5. Resolved for IAP: Android real-money purchases should use Unity IAP behind `IIapProvider`; rewarded ads remain a separate later decision.
6. Whether mid-level board state should persist after app close.
7. Whether missions are static, daily rotating, or both for the first release.
8. Which products are consumable versus non-consumable.
9. Whether purchase receipt validation is local-only for prototype or backend-backed for production.
10. Whether Gem can be earned from hard missions only, events only, or both.
11. Whether high-tier plant unlocks are level-gated, shop-gated, mission-gated, or mixed.
12. Whether temporary locked cells unlock from order completion, timer milestones, paid unlocks, or all of these.

## 17. Recommended Build Order

Start with gameplay logic before art polish:

1. `LevelDefinition` and parser.
2. Plain board model and merge/move tests.
3. Board renderer with placeholder visuals.
4. Drag input and merge animation.
5. Producer and abilities.
6. NPC objective and timer.
7. Save, audio, VFX, Android polish.
8. Shop catalog and mock purchase flow after approval.
9. Mission progress and reward claim flow after approval.
10. Platform IAP provider after shop flow is validated.
