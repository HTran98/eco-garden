# Eco Garden - UI Release Audit

Date created: 2026-05-26

Purpose: break down the UI work needed to raise the current functional mobile UI to first-release quality.

## Release UI Target

The first release UI should feel like a complete Android portrait game UI, not a debug or prototype overlay. The target is:

- Readable at 720x1280 and 1080x1920 portrait.
- Safe on tall, notched, and gesture-navigation Android devices.
- Touch targets are reliable for repeated play.
- HUD and panels do not cover board-critical cells or drag paths.
- Shop, Mission, Level Select, Pause, and Result panels share one visual language.
- Purchase, claim, reward, invalid action, win, and fail feedback are clear.
- Any remaining placeholder visuals are intentional and documented.

## Current Baseline

| Area | Current State | Release Risk |
| --- | --- | --- |
| HUD layout | Code metrics exist for top bar, ability bar, Delivery, Sell, and compact mission tracker. | Device validation is still open; text hierarchy and visual polish are still prototype-like. |
| Shop | Mobile layout metrics exist for product row widths and buttons. | Needs final copy/state pass for IAP, prices, owned/unavailable states, and visual hierarchy. |
| Mission | Full panel and compact tracker exist; tracker hides while other panels open. | Full panel readability and claim-state polish need release pass. |
| Level Select | Panel exists for locked/unlocked levels. | Needs completed/locked/selected state clarity and better level summary layout. |
| Result/Pause | Basic result and restart/next controls exist. | Needs release-safe layout, clearer rewards/result messaging, and visual consistency. |
| Feedback | HUD messages and some reward/action feedback exist. | Needs standardized success/failure severity and non-blocking placement. |
| Visual skin | `HudSkinController` applies shared placeholder sprites and colors. | UI still relies on placeholder-style panels/buttons; icons and authored UI assets are open under RB-009. |

## Device Profiles

Use the profiles from `Docs/Android Portrait Layout Matrix.md`:

| Profile | Resolution | Required For Release |
| --- | --- | --- |
| Small baseline | 720x1280 | Yes |
| Standard baseline | 1080x1920 | Yes |
| Tall phone | 1080x2400 | Yes |
| Notched phone | 1080x2340 or equivalent safe area | Yes |
| Gesture-nav phone | 1080x2400 or equivalent bottom inset | Yes |
| Small safe-area stress | 720x1560 | Should pass before RC |

## Task Breakdown

### UI-R1 - Capture Baseline Screens

Priority: P0

Output:

- Screenshots or manual observations for HUD, Shop, Mission, Level Select, Pause, Result, and active drag/drop states.
- Record failures in the audit table below.

Validation:

- Every release UI surface has at least one entry for 720x1280 and 1080x1920.
- Notch and gesture-nav risks are marked as pass, fail, or needs-device.

### UI-R2 - HUD Gameplay Release Pass

Priority: P0

Scope:

- Top bar: timer, Gold, Gem, Level, Shop, Mission, Pause.
- Objective panel.
- Ability bar.
- Delivery and Sell drop zones.
- Compact mission tracker.
- HUD feedback message placement.

Acceptance criteria:

- No persistent HUD element covers a board-critical cell.
- Delivery and Sell are reachable while dragging.
- Ability buttons are finger-sized and do not shift layout.
- Timer/currency/objective are readable on 720x1280.
- Mission tracker remains useful but does not dominate the board.

### UI-R3 - Panel Framework Polish

Priority: P0

Scope:

- Shared panel background.
- Close button placement.
- Panel title hierarchy.
- Scroll viewport styling.
- Button disabled/pressed/selected states.

Acceptance criteria:

- Shop, Mission, Level Select, Pause, and Result panels look like the same product.
- Close/action buttons are consistently placed and reachable.
- Text does not clip or overlap at the smallest profile.

### UI-R4 - Shop Release Pass

Priority: P0

Scope:

- Category tabs.
- Product rows.
- Gold/Gem/IAP price badges.
- Buy, Owned, Unavailable, Pending, Cancelled, Failed, Duplicate states.
- IAP copy for client-only Unity IAP release path.

Acceptance criteria:

- Player can identify price type and purchase state at a glance.
- IAP rows do not imply backend account sync or restore behavior that is not implemented.
- Product row text fits on 720x1280.
- Purchase feedback is visible without blocking input after the panel closes.

### UI-R5 - Mission Release Pass

Priority: P1

Scope:

- Full mission panel rows.
- Compact tracker rows.
- Claim button state.
- Claimed/completed/not-complete copy.
- Reward text.

Acceptance criteria:

- Completed and claimed missions are visually distinct.
- Claim buttons are not enabled for incomplete or already claimed missions.
- Compact tracker shows only high-value active information.

### UI-R6 - Level Select and Result Flow Pass

Priority: P1

Scope:

- Level Select list rows.
- Locked/unlocked/completed/current states.
- Difficulty, timer, reward, and order summary text.
- Win/fail result panel.
- Restart and Next buttons.

Acceptance criteria:

- Player understands which levels are playable.
- Result panel clearly explains completion/failure and next action.
- Next button is only shown when meaningful and reachable.

### UI-R7 - Feedback and Motion Polish

Priority: P1

Scope:

- Button press feedback.
- Purchase success/fail feedback.
- Mission claim feedback.
- Reward gained feedback.
- Invalid action feedback.
- Timer warning and result transition.

Acceptance criteria:

- Feedback is brief, readable, and does not cover drag targets.
- Success, warning, and failure states are visually distinct.
- Repeated actions do not stack unreadable messages.

### UI-R8 - Icon and Asset Pass

Priority: P1

Scope:

- Shop, Mission, Level, Pause, Restart, Next icons.
- Currency icons for Gold/Gem.
- Ability icons.
- Delivery/Sell visual refinement.
- Panel/button sprite replacement or approval.

Acceptance criteria:

- Common actions use recognizable symbols where practical.
- Text labels are shortened where icons can carry the meaning.
- Any remaining placeholder sprite is accepted in `Docs/Asset Resource List.md`.

### UI-R9 - Android Device Validation

Priority: P0 before release candidate

Scope:

- Run `Docs/Android Portrait Layout Matrix.md`.
- Validate touch parity on Android hardware or equivalent device test.
- Record pass/fail and screenshots if available.

Acceptance criteria:

- RB-003 can move from `Needs Device` to `Closed` or a documented accepted risk.
- Any remaining failures have severity and next action.

## Audit Table

| Surface | 720x1280 | 1080x1920 | Tall | Notch | Gesture Nav | Severity | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| HUD top bar | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | UI-R2 code pass compacted top-bar actions and added width tests; visual/device pass still required. |
| Objective panel | Not run | Not run | Not run | Needs device | Needs device | P0 | Must not cover board or tracker. |
| Ability bar | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | Ability actions use compact two-line labels and touch-size tests; still needs visual/device pass. |
| Delivery zone | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | Existing metric coverage; needs drag test. |
| Sell basket | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | Existing metric coverage; needs drag test. |
| Compact mission tracker | Code pass | Code pass | Code pass | Needs device | Needs device | P1 | Existing metric coverage; needs readability pass. |
| Shop panel | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | Product state/copy polish still needed. |
| Mission panel | Code pass | Code pass | Code pass | Needs device | Needs device | P1 | UI-R3 shared panel header/content metrics applied; row readability still needs release pass. |
| Level Select panel | Code pass | Code pass | Code pass | Needs device | Needs device | P1 | UI-R3 shared panel header/content metrics applied; locked/current/completed states need clarity. |
| Pause panel | Not run | Not run | Not run | Needs device | Needs device | P1 | Needs consistent panel framework. |
| Win/fail result panel | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | UI-R3 result action metrics added; copy/state visual pass still needed. |
| HUD feedback messages | Not run | Not run | Not run | Needs device | Needs device | P1 | Must not cover drag/drop targets. |

## Recommended Execution Order

1. UI-R1 - Capture baseline screens.
2. UI-R2 - HUD gameplay release pass.
3. UI-R3 - Panel framework polish.
4. UI-R4 - Shop release pass.
5. UI-R5 - Mission release pass.
6. UI-R6 - Level Select and result flow pass.
7. UI-R7 - Feedback and motion polish.
8. UI-R8 - Icon and asset pass.
9. UI-R9 - Android device validation.

## First Implementation Slice

Start with UI-R2 plus the shared parts of UI-R3:

- Standardize HUD text sizes and button dimensions.
- Replace long top-bar text labels where possible with compact labels/icons.
- Make Result, Shop, Mission, and Level panels use one title/close/action layout.
- Add or update metric tests for any changed anchors/sizes.

This slice has the highest release impact because every play session depends on HUD readability and reliable drag/drop targets.

## Execution Notes

2026-05-26:

- Started UI-R2 HUD gameplay release pass.
- `AndroidHudLayoutMetrics` now defines top-bar text/action anchors and ability-button anchors, plus minimum width/touch-size constants.
- `AndroidHudLayoutController` applies top-bar and ability child rects at runtime so existing scenes receive the compact HUD pass without scene regeneration.
- Top-bar labels are compacted to `Lvl`, `Tasks`, `Shop`, and `II`; ability buttons now use two-line labels such as `Shovel` plus count to improve fit on small portrait.
- `EcoGardenUiMenu` scene generation was aligned with the same compact anchors and labels.
- Added EditMode metric coverage for small-portrait top-bar stat/action widths and ability button touch size.
- Runtime, editor, and EditMode test assemblies build after local generated `.csproj` stale references were removed for verification; Unity-generated project files are not tracked and were not committed.

2026-05-26:

- Started UI-R3 shared panel framework polish.
- Added `PanelUiLayoutMetrics` for common panel title, close, content, shop tab/content, and result action anchors.
- `AndroidHudLayoutController` now applies shared panel child rects and can find inactive scene objects, so existing hidden panels are normalized at runtime.
- HUD generator and runtime Shop/Mission fallback panels now use shared panel metrics instead of hard-coded header/content anchors.
- Added EditMode metric coverage for small-portrait panel title/close sizing, Shop header/tab/content separation, and Result action button touch size.
- Runtime, editor, and EditMode test assemblies build successfully after local generated `.csproj` verification updates; generated project files are not tracked and were not committed.
