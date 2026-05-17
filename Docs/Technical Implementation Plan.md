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
      Input/
      Items/
      Level/
      UI/
      Save/
      Utilities/
    ScriptableObjects/
      Abilities/
      Items/
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
| Track required order | 1x Lotus Lv5 for Level 15 |
| Validate delivery | Item dragged to NPC order point |
| Complete level | Fires completion event |
| Fail level | Fires failure event when timer expires |

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
3. Idle until fulfilled or timer expires.
4. Exit after fulfillment.

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

## 15. Known Open Decisions

These should be resolved before production expansion, but are not blockers for the Level 15 vertical slice:

1. Whether selling items is an active player action.
2. Whether producer uses gold, energy, cooldown upgrades, or random spawn tables.
3. Whether obstacles can have HP greater than 1.
4. Whether levels beyond Level 15 use multiple NPC orders.
5. Whether monetization is handled through Unity LevelPlay, AdMob, Unity IAP, or another SDK.
6. Whether mid-level board state should persist after app close.

## 16. Recommended Build Order

Start with gameplay logic before art polish:

1. `LevelDefinition` and parser.
2. Plain board model and merge/move tests.
3. Board renderer with placeholder visuals.
4. Drag input and merge animation.
5. Producer and abilities.
6. NPC objective and timer.
7. Save, audio, VFX, Android polish.
