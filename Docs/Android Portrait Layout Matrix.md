# Eco Garden - Android Portrait Layout Matrix

Date created: 2026-05-22

Purpose: define the target portrait profiles and pass criteria for Milestone C mobile UI validation.

## Target Profiles

| Profile | Resolution | Aspect | Safe Area Risk | Primary Concern |
| --- | --- | --- | --- | --- |
| Small baseline | 720x1280 | 9:16 | Low | Minimum readable HUD and reachable board controls. |
| Standard baseline | 1080x1920 | 9:16 | Low | Main Android reference layout. |
| Tall phone | 1080x2400 | 9:20 | Medium | Top/bottom spacing, timer/currency placement, panel height. |
| Notched phone | 1080x2340 | 9:19.5 | High top inset | HUD must avoid camera/notch and status bar. |
| Gesture-nav phone | 1080x2400 | 9:20 | High bottom inset | Sell basket, delivery area, and bottom buttons must stay above gesture area. |
| Small safe-area stress | 720x1560 | 6:13 | High top/bottom inset | Text truncation, compact tracker, and touch target spacing. |

## Global Pass Criteria

- Board cells remain visible and usable; no persistent HUD/panel element covers board-critical cells.
- Timer, Gold, Gem, Shop, Mission, Pause, Sell basket, and Delivery zone remain readable.
- Touch targets for primary actions are at least visually finger-sized and not stacked too closely.
- Panels fit within the safe area and can be closed without reaching hidden controls.
- Text does not overlap, clip awkwardly, or shrink below readable size on the smallest profile.
- Mission tracker hides or relocates while Shop/Mission/Pause/Result panels are open.
- External drop zones remain reachable while dragging board items.

## HUD Checklist

| Area | 720x1280 | 1080x1920 | Tall | Notch | Gesture Nav | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| Timer/currency row |  |  |  |  |  | Top row stays inside safe area. |
| Pause/restart controls |  |  |  |  |  | Buttons remain reachable and do not overlap currency. |
| Shop entry |  |  |  |  |  | Button remains visible without covering board. |
| Mission entry/tracker | Code pass | Code pass | Code pass | Pending device | Pending device | Tracker avoids Delivery/Sell, keeps a readable minimum width, and shows two compact rows. |
| Sell basket | Code pass | Code pass | Code pass | Pending device | Pending device | Drop target moved above AbilityBar and bottom safe-area root. |
| Delivery zone | Code pass | Code pass | Code pass | Pending device | Pending device | Drop target moved above AbilityBar and kept separate from Sell basket. |

## Panel Checklist

| Panel | 720x1280 | 1080x1920 | Tall | Notch | Gesture Nav | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| Shop | Code pass | Code pass | Code pass | Pending device | Pending device | Metric tests cover category tab, product text, price badge, and Buy/Owned button width. |
| Mission |  |  |  |  |  | Claim buttons stay reachable and reward text fits. |
| Pause |  |  |  |  |  | Resume/restart controls fit without clipping. |
| Win/fail result |  |  |  |  |  | Rewards and action buttons stay visible. |

## Touch Parity Checklist

| Action | Expected Result |
| --- | --- |
| Tap producer | Spawns item without hitting HUD or panel controls. |
| Drag item to empty cell | Item moves and drag preview stays under finger. |
| Drag merge pair | Merge completes without external zones stealing input. |
| Drag to Sell basket | Sell target highlights/reaches reliably. |
| Drag to Delivery zone | Delivery target accepts only requested item. |
| Select booster then tap item | Targeting works on all target profiles. |
| Open/close Shop | Board input is blocked while panel is open and restored after close. |
| Open/close Mission | Tracker and full panel do not overlap. |

## Execution Notes

- Run this matrix after any HUD, panel, safe-area, board sizing, or input-layer change.
- Record failures in `Docs/Completion Task Breakdown.md` under Milestone C execution notes.
- Android device validation takes priority over Game view simulation when results differ.

2026-05-22:

- First C2 code pass moved the AbilityBar lower inside the safe-area root and moved Delivery/Sell drop zones above it.
- `AndroidHudLayoutMetrics` now centralizes HUD anchors used by runtime layout, runtime UI fallbacks, and editor UI generation.
- `AndroidHudLayoutMetricsTests` verifies AbilityBar does not overlap Delivery/Sell and MissionTracker does not overlap drop zones for 720x1280, 1080x1920, 1080x2400, and 720x1560 profiles.
- C4 code pass widened the compact mission tracker on small portrait screens and limited it to two rows to avoid cramped claim buttons.
- C3 code pass added shop row layout metrics and tests for category tab, price badge, Buy/Owned button, and product text width on 720x1280.
- C5 code pass added touch-size and separation tests for Delivery/Sell drop zones across portrait profiles.
