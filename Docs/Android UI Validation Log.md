# Eco Garden - Android UI Validation Log

Date created: 2026-05-26

Purpose: record the UI-R9 Android portrait device validation pass for the release-ready UI work from UI-R2 through UI-R8.

## Validation Scope

Use this log with `Docs/Android Portrait Layout Matrix.md`.

Required surfaces:

- HUD top bar: timer, Gold, Gem, Level, Mission, Shop, Pause.
- Objective panel.
- Ability bar.
- Delivery and Sell drop zones during drag.
- Compact mission tracker.
- Shop panel.
- Mission panel.
- Level Select panel.
- Win/fail Result panel.
- HUD feedback messages.

Required interactions:

- Tap producer.
- Drag item to empty cell.
- Drag merge pair.
- Drag requested item to Delivery.
- Drag any sellable item to Sell.
- Select each ability and tap a valid/invalid target.
- Open and close Shop, Mission, Level Select, and Result flow.
- Trigger purchase/claim/invalid-action feedback.

## Device Matrix

| Profile | Resolution / Device | Result | Screenshot / Evidence | Notes |
| --- | --- | --- | --- | --- |
| Small baseline | 720x1280 or equivalent emulator | Not run |  | Required before RC. |
| Standard baseline | 1080x1920 or equivalent emulator/device | Not run |  | Required before RC. |
| Tall phone | 1080x2400 or equivalent | Not run |  | Required before RC. |
| Notched phone | 1080x2340 or equivalent notch/cutout | Not run |  | Required before RC. |
| Gesture-nav phone | 1080x2400 with gesture navigation | Not run |  | Required before RC. |
| Small safe-area stress | 720x1560 or equivalent emulator | Not run |  | Should pass before RC. |

## Surface Results

| Surface | Small | Standard | Tall | Notch | Gesture Nav | Severity If Failed | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| HUD top bar | Not run | Not run | Not run | Not run | Not run | P0 | Confirm no notch/status-bar overlap. |
| Objective panel | Not run | Not run | Not run | Not run | Not run | P0 | Confirm it does not hide critical board cells. |
| Ability bar | Not run | Not run | Not run | Not run | Not run | P0 | Confirm all three buttons are reachable and readable. |
| Delivery zone | Not run | Not run | Not run | Not run | Not run | P0 | Confirm drag target highlight and acceptance. |
| Sell basket | Not run | Not run | Not run | Not run | Not run | P0 | Confirm drag target highlight and no gesture-nav conflict. |
| Compact mission tracker | Not run | Not run | Not run | Not run | Not run | P1 | Confirm it hides behind full panels. |
| Shop panel | Not run | Not run | Not run | Not run | Not run | P0 | Confirm tabs, rows, price badges, and states fit. |
| Mission panel | Not run | Not run | Not run | Not run | Not run | P1 | Confirm active/claim/claimed rows are distinct. |
| Level Select panel | Not run | Not run | Not run | Not run | Not run | P1 | Confirm Done/Current/Locked states are clear. |
| Result panel | Not run | Not run | Not run | Not run | Not run | P0 | Confirm Replay/Retry/Next actions fit and Next visibility is correct. |
| HUD feedback | Not run | Not run | Not run | Not run | Not run | P1 | Confirm success/warning/error colors are readable and non-blocking. |

## Code-Pass Baseline

2026-05-26:

- UI-R2 through UI-R8 are complete at code/metric level.
- Layout metrics cover top-bar action/stat widths, ability touch sizes, external drop-zone size/separation, shared panel headers/content, Shop row controls, Mission rows, Level Select rows, and Result actions.
- Runtime symbolic icon labels are accepted as first-release placeholders in `Docs/Asset Resource List.md`.
- Device validation remains required before closing RB-003 and RB-012.

2026-05-29:

- Added a Simulator-focused layout recovery pass after Game view validation showed broken HUD forms.
- `AndroidHudLayoutController` now reapplies layout on enable, rect-size changes, and screen/safe-area changes, and sets CanvasScaler match to portrait width or landscape height.
- Result flow now hides the compact Mission tracker while the Result panel is open and runtime-creates a missing Next button for old scenes.
- Re-run Unity Simulator profiles before RC: verify HUD forms remain visible outside the board on 720x1280, 1080x1920, 1080x2400, and one landscape/resize stress profile.

2026-05-29:

- Added a second Simulator cleanup pass based on portrait screenshot feedback.
- Verify the full board is visible after the updated camera padding.
- Verify Delivery and Sell are lower/smaller and do not cover important board cells.
- Verify compact Mission tracker no longer appears over the board; claimable missions should show a red badge on the Mission top-bar button and a HUD message.
- Verify Pause changes to a Play icon while paused and returns to Pause after resume.

2026-05-29:

- Mission panel validation target updated after UI polish.
- Verify Mission panel summary row reads Ready/Active/Claimed and does not overlap the scroll list.
- Verify claimable missions appear at the top of the Mission panel.
- Verify the Mission top-bar badge is a small fixed `!` badge, not a stretched red rectangle.

## Failure Recording

For every failed surface, record:

- Device/profile.
- Screenshot or short video path.
- Exact UI surface and interaction.
- Severity: P0 blocks release candidate, P1 blocks polish signoff, P2 can be deferred.
- Next action and owner.
