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
| Visual skin | `HudSkinController` applies shared placeholder sprites/colors and `UiIconLabelCatalog` provides compact runtime symbolic labels. | Authored UI icon art can still replace the accepted runtime symbolic placeholders after first release. |

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
- Results are recorded in `Docs/Android UI Validation Log.md`.

## Audit Table

| Surface | 720x1280 | 1080x1920 | Tall | Notch | Gesture Nav | Severity | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| HUD top bar | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | UI-R2 code pass compacted top-bar actions and added width tests; visual/device pass still required. |
| Objective panel | Not run | Not run | Not run | Needs device | Needs device | P0 | Must not cover board or tracker. |
| Ability bar | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | Ability actions use compact two-line labels and touch-size tests; still needs visual/device pass. |
| Delivery zone | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | Existing metric coverage; needs drag test. |
| Sell basket | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | Existing metric coverage; needs drag test. |
| Compact mission tracker | Code pass | Code pass | Code pass | Needs device | Needs device | P1 | Existing metric coverage; needs readability pass. |
| Shop panel | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | UI-R4 code pass added release copy/state polish; visual/device pass still required. |
| Mission panel | Code pass | Code pass | Code pass | Needs device | Needs device | P1 | UI-R5 code pass added mission row/claim/tracker polish; visual/device pass still required. |
| Level Select panel | Code pass | Code pass | Code pass | Needs device | Needs device | P1 | UI-R6 code pass added locked/current/done row states and metric coverage; visual/device pass still required. |
| Pause panel | Not run | Not run | Not run | Needs device | Needs device | P1 | Needs consistent panel framework. |
| Win/fail result panel | Code pass | Code pass | Code pass | Needs device | Needs device | P0 | UI-R6 code pass clarified complete/fail copy and replay/retry/next actions; visual/device pass still required. |
| HUD feedback messages | Code pass | Code pass | Code pass | Needs device | Needs device | P1 | UI-R7 code pass added severity color/duration and duplicate suppression; placement still needs device validation. |

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

2026-05-26:

- Started UI-R4 Shop release pass.
- Shop product rows now present invalid products as unavailable, show pending IAP purchases as non-interactable `Pending`, and refresh from `ShopController.IapPurchaseCompleted` callbacks.
- IAP button/price copy now uses `Store` and `Store unavailable` language, matching the client-only Unity IAP release path without implying custom backend account sync or restore behavior.
- Empty shop categories are disabled and the selected tab falls back to the first populated release category, avoiding an empty Decoration tab after decoration purchases were deferred.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-26:

- Started UI-R5 Mission release pass.
- Added `MissionUiLayoutMetrics` for full mission rows and compact tracker rows, with EditMode metric coverage for small-portrait text and claim-button widths.
- Mission rows now use distinct visual states for active, ready-to-claim, and claimed missions; incomplete and claimed missions remain non-interactable.
- Compact tracker now prioritizes claimable missions before active progress rows, keeping high-value actions visible without opening the full panel.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false` after local generated `.csproj` verification updates; generated project files are not tracked.

2026-05-26:

- Started UI-R6 Level Select and Result flow pass.
- Added `LevelSelectUiLayoutMetrics` and EditMode metric coverage for small-portrait level row title/action widths.
- Level Select rows now show `Done`, `Current`, or `Locked`, include difficulty, timer, and order summary copy, and use disabled button styling for locked levels.
- Result flow now uses clearer complete/fail messages, labels the restart action as `Replay` or `Retry`, and resolves inactive result UI objects during wiring.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false` after local generated `.csproj` verification updates; generated project files are not tracked.

2026-05-26:

- Started UI-R7 Feedback and Motion polish.
- Added `FeedbackMessageSeverity` and `FeedbackMessagePresentation` to classify common HUD messages into info, success, warning, and error states.
- `GameplayFeedbackController` now applies severity colors and longer durations for warning/error messages, while suppressing immediate duplicate HUD messages to avoid unreadable stacking during repeated taps or invalid actions.
- Added EditMode coverage for common release message classification and readable warning/error durations.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false` after local generated `.csproj` verification updates; generated project files are not tracked.

2026-05-26:

- Started UI-R8 Icon and Asset pass.
- Added `UiIconLabelCatalog` as the shared source for compact runtime symbolic labels for top-bar actions, ability buttons, result actions, and Gold/Gem displays.
- HUD generator, Android runtime label normalization, Ability HUD refresh, and Economy currency text now use the shared catalog instead of scattered long labels.
- Updated `Docs/Asset Resource List.md` to mark accepted runtime symbolic placeholders for ability, currency, pause, restart, and next icons.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false` after local generated `.csproj` verification updates; generated project files are not tracked.

2026-05-26:

- Started UI-R9 Android device validation prep.
- Added `Docs/Android UI Validation Log.md` with required profiles, release UI surfaces, interaction checks, evidence fields, and failure-recording rules.
- Updated `Docs/Android Portrait Layout Matrix.md` with the latest UI-R2 through UI-R8 code-pass baseline.
- RB-003 and RB-012 remain open as device-validation gates; no device screenshots have been captured in this environment.

2026-05-28:

- Started UI visual upgrade pass 1.
- Added `UiThemePalette` as the shared cozy-garden palette for light panels, green primary actions, warm accent states, Gold/Gem/Store colors, Delivery/Sell highlights, and readable text choices.
- `HudSkinController` now applies the palette across existing HUD, panel, button, viewport, and drop-zone objects at runtime while keeping existing Android layout metrics unchanged.
- Shop, Mission, and Level Select runtime rows now reference the shared palette for disabled, selected, claimable, completed, locked, currency, Gem, and Store states.
- Added EditMode coverage for core palette contrast and text-color selection.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Unity-generated `.csproj` files were updated locally only for verification and remain untracked.
- Authored PNG/icon replacement remains the next visual upgrade slice; Android device validation is still required before closing RB-003/RB-012.

2026-05-28:

- Continued UI visual upgrade with first-pass transparent PNG icons under `Assets/EcoGarden/Art/UI/Resources/UiIcons`.
- Added PNGs for Gold, Gem, Timer, Pause, Restart, Next, Shop, Mission, Level, Close, Shovel, Magic Wand, and Sorting Magnet.
- `HudSkinController` now creates a non-raycast `RuntimeIcon` child for Pause, Level, Mission, Shop, Close, Restart, and Next buttons when the resource sprite is available, while keeping text fallback behavior if the asset is missing.
- Ability, currency, and timer PNGs are now wired as icon+number layouts: Economy shows numeric balances, timer keeps time text, and ability buttons keep `xN` counts beside the icon.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Android visual/device validation is still required.

2026-05-28:

- Continued UI visual upgrade with first-pass surface skins under `Assets/EcoGarden/Art/UI/Resources/UiSkins`.
- Added PNG skins for light/strong/overlay panels, row surfaces, primary/secondary/disabled buttons, Gold/Gem/Store price badges, Delivery, and Sell.
- `HudSkinController` now loads surface skins for HUD panels, top bar, result panel, tracker, buttons, and drop zones while preserving procedural fallback sprites.
- Shop product rows, price badges, category tabs, runtime Shop panels/buttons, Mission rows/buttons, and Level Select rows/buttons now use the skin sprites when available.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Unity import plus Game view/device visual validation remain required.

2026-05-28:

- Continued UI visual upgrade with interaction/readability polish.
- Added `UiButtonFeedback`, a lightweight pointer press scale effect for HUD, Shop, Mission, and Level Select buttons.
- `HudSkinController` now applies subtle runtime text shadow to skinned text, improving contrast on the new panel/button PNG surfaces.
- Runtime, editor, and EditMode test assemblies build successfully after local restore regenerated missing `Temp/obj/*/project.assets.json`.
- Play Mode validation is required to judge button feel and confirm text shadows do not look too heavy on device.

2026-05-28:

- Continued visual upgrade with first-pass full-screen background art.
- Added `bg_pond_foggy_01.png` under `Assets/EcoGarden/Art/Backgrounds/Resources/Backgrounds`.
- Added `EcoGardenBackgroundController`, which loads the background sprite from Resources, follows/scales to the orthographic camera, and renders behind board sprites.
- `GameBootstrapper` now ensures a background exists for old scenes, and scene generation adds the background for new generated scenes.
- Runtime, editor, and EditMode test assemblies build successfully. Play Mode/Game view validation is required to confirm board contrast and HUD readability.

2026-05-28:

- Continued visual upgrade with board readability backing.
- Added `ui_board_backdrop.png` under `Assets/EcoGarden/Art/UI/Resources/UiSkins`.
- Added `BoardBackdropController`, which loads the backdrop sprite, tracks the active `BoardController`, and scales the backdrop from `BoardView.GetBoardWorldWidth/Height` so it fits each level.
- `GameBootstrapper` now ensures old scenes receive the board backdrop, and scene generation creates it after board load for new generated scenes.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Unity import plus portrait Game view/device validation remain required.

2026-05-28:

- Continued UI feel polish for drag/drop affordance.
- Added `UiAmbientPulse`, which creates a non-raycast runtime glow child and animates alpha/scale without changing the parent RectTransform or touch target.
- `HudSkinController` now applies the pulse to Delivery and Sell drop zones using their themed highlight colors.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Play Mode validation is required to confirm the pulse is noticeable without feeling distracting.

2026-05-28:

- Continued board visual readability polish.
- `CellView` now creates a subtle `CellShadow` child `SpriteRenderer` under each tile, using the same sprite with low alpha and lower sorting order.
- `ItemView` now creates a subtle `ItemShadow` child `SpriteRenderer` under each item, follows refresh/drag alpha, and raises sorting order while dragging.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Play Mode validation is required to confirm the extra depth does not make small boards feel busy.

2026-05-28:

- Continued ability HUD affordance polish.
- `AbilityHudController` now adds a `RuntimeSelectionGlow` child image to ability buttons and toggles it with the selected ability state.
- The selected booster now has both the existing selected color and a visible glow while preserving the same touch target and icon/count layout.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Play Mode validation is required to confirm the selected state reads clearly on device.

2026-05-28:

- Continued panel feel polish.
- Added `UiPanelTransition`, a lightweight CanvasGroup/scale entrance animation that runs from `OnEnable` using unscaled time.
- `HudSkinController` now attaches the transition to Result, Level, Shop, and Mission panels so existing `SetActive` open flows get the animation without changing panel controllers.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Play Mode validation is required to confirm the motion feels responsive on Android.

2026-05-28:

- Continued row hierarchy polish for panel scanability.
- Added `UiRowAccent`, a shared non-raycast runtime accent strip for repeated rows.
- Shop product rows now use category/Store/disabled accent colors; Mission rows use active/claimable/claimed accents; Level rows use current/done/locked accents.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Play Mode validation is required to confirm the accent strip helps scanning without crowding text.

2026-05-28:

- Continued timer urgency polish.
- `LevelStateController` now caches the Timer text presentation and applies a warning color/scale pulse under 20 seconds, with a stronger critical color under 10 seconds.
- Timer presentation restores to its base color/scale when the level is restarted, paused/completed/failed, or above the warning threshold.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Play Mode validation is required to judge whether the pulse is noticeable but not distracting.

2026-05-28:

- Continued objective readability polish.
- `LevelStateController` now computes aggregate order submitted/required progress during objective refresh.
- `ObjectivePanel` receives the shared `UiRowAccent` strip, shifting from primary to warm accent near completion and success when the order is complete.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Play Mode validation is required to confirm the objective accent helps without competing with the timer.

2026-05-29:

- Completed finite-level order handoff polish.
- `LevelStateController` now auto-advances to the next catalog level shortly after `OrderCompleted` when a next level exists, while standalone scenes without a catalog keep the existing result panel behavior.
- `StartNextLevel` still unlocks/saves/selects the next level and calls `StartLevel`, so the next level's order objective is refreshed immediately after the board loads.
- Added EditMode coverage that `StartNextLevel` selects level 2, returns to Playing, and refreshes the objective text for the new order. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-29:

- Fixed completion result obstruction seen in Play Mode.
- `LevelStateController.ShowResult` now hides Shop, Mission, and Level panels before showing the completion result, so open side panels cannot cover the Next action.
- Next-level lookup now asks the catalog for the next higher level instead of only checking `currentLevelId + 1`, with a fallback to the first catalog level if the current scene level is not part of the catalog.
- Added EditMode coverage that completing a level hides an open Mission panel. Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-29:

- Fixed the remaining completion/result handoff issues from Play Mode screenshots.
- Completion now hides every Shop/Mission/MissionTracker/Level panel instance before showing result or advancing, and the compact Mission tracker no longer reopens while `ResultPanel` is active.
- Old scenes that have a `ResultPanel` but no `NextLevelButton` now receive a runtime Next button with the shared result-action anchors and skin fallback.
- Next-level selection now force-unlocks the selected next catalog level before calling `LevelCatalogController.SelectLevel`, so skipped/non-sequential first-release catalog ids do not leave the game stuck on the completed level.
- `AndroidHudLayoutController` now reapplies layout during Unity Simulator screen/safe-area changes and sets CanvasScaler match dynamically for portrait vs landscape simulator profiles.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Unity Simulator/Game view visual validation is still required.

2026-05-29:

- Continued Simulator portrait cleanup after review screenshot.
- Board framing now uses more conservative camera padding so the full grid remains visible on narrow mobile Simulator profiles.
- Delivery and Sell drop zones are smaller and placed lower, reducing board obstruction while keeping minimum touch-size coverage.
- Compact Mission tracker is disabled during gameplay; mission readiness is now communicated by a red badge on the Mission top-bar button and a one-time HUD message when a mission becomes claimable.
- Pause button now switches to a runtime Play icon while paused, then restores the Pause icon when resumed.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Simulator visual validation is required to tune final drop-zone placement if needed.

2026-05-29:

- Hardened Level Complete panel behavior after Play Mode showed the panel could remain without an obvious Next action.
- The Next result action is now visible for every completed level result, even when catalog lookup is not yet resolved.
- Pressing Next closes the panel immediately; it advances to the next catalog level when available or reloads the current level as a no-catalog fallback.
- Auto-advance uses the same path, and catalog lookup now includes inactive scene controllers.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-29:

- Tuned Level Complete auto-advance timing.
- The completion panel now stays visible for at least 5 seconds before auto-advance, leaving time to read the reward text or tap Next manually.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-29:

- Continued result-panel release polish.
- Added `ResultCountdownText` so players can see the remaining automatic handoff time on completion.
- Replay/Next result actions now show icon plus readable text instead of icon-only buttons.
- Added layout coverage to ensure the result message, countdown, and actions do not overlap on the 720x1280 portrait profile.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-29:

- Continued Mission UI release polish.
- Full Mission panel now has a summary row for Ready/Active/Claimed counts and a shorter content area beneath it.
- Claimable missions are sorted above active and claimed rows so reward actions are easier to find.
- Mission top-bar alert badge is fixed-size with `!`, avoiding the stretched red mark seen in Simulator.
- Compact mission tracker remains disabled in gameplay to keep the board clear.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-29:

- Continued Level Select UI release polish.
- Level panel now has a summary row showing unlocked progress and current level.
- Level list content is anchored below the summary row to avoid crowding the panel header.
- Level row action labels now distinguish Current, Replay, Play, and Locked states.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-29:

- Continued Shop UI release polish.
- Shop panel now has a summary row below category tabs showing selected category, item count, Store count, and owned count.
- Product list content is anchored below the summary row to avoid crowding category tabs.
- Runtime fallback Shop panels create the same summary row.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-29:

- Continued HUD feedback and drop-zone polish.
- HUD feedback messages now get a severity-colored background surface for readability over the pond/board art.
- Delivery and Sell labels receive text shadows, and highlighted drop zones scale up slightly while dragging over them.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-29:

- Continued ability HUD polish.
- Booster buttons now disable when their count reaches zero and use disabled styling for the count label.
- Selecting a booster now sends the target prompt through the HUD feedback presentation, making the next action clearer.
- Runtime, editor, and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-31:

- Continued Pause UI release polish.
- Added a real `PausePanel` with Resume and Restart actions, shared Result-size portrait anchors, runtime fallback creation for old scenes, generated HUD support, skin/transition wiring, and blocking-panel cleanup when pause opens.
- Added EditMode coverage for Pause panel touch sizes/message separation on the 720x1280 portrait profile and for pause toggle show/hide behavior.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; the first parallel runtime build hit a temporary file lock from another build process and passed when rerun separately.

2026-05-31:

- Continued modal interaction polish.
- Added `UiModalBackdropController`, a full-screen dim/raycast backdrop that activates behind Result, Pause, Level, Shop, and Mission panels so modal UI clearly blocks board input outside the panel bounds.
- `HudSkinController` ensures old scenes receive the backdrop at runtime, and generated HUDs now include the controller directly.
- Added EditMode coverage for backdrop visibility, raycast blocking, dim opacity, and sibling order behind visible modal panels.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false` after local Unity-generated `.csproj` includes were refreshed for the new files; generated project files are not tracked.

2026-05-31:

- Continued modal stacking cleanup.
- Added `UiModalPanelUtility` so opening Shop, Mission, or Level Select closes sibling modal panels instead of allowing multiple full panels to stack under the same backdrop.
- `UiModalBackdropController` now uses the shared modal panel list, keeping the backdrop behavior aligned with the panel coordinator.
- Added EditMode coverage that the requested modal remains visible while sibling panels are closed.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false` after local Unity-generated `.csproj` verification includes were refreshed; generated project files are not tracked.

2026-05-31:

- Continued backdrop interaction polish.
- `ModalBackdrop` is now a dim `Button` with no transition, so tapping outside Shop, Mission, or Level Select closes those navigation panels.
- Backdrop dismiss intentionally leaves Pause and Result panels active, preserving explicit Resume/Restart/Replay/Next decisions.
- Added EditMode coverage for backdrop button setup and dismiss behavior.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-31:

- Continued modal z-order polish.
- Opening Shop, Mission, Level Select, Pause, or Result now raises that modal panel to the top sibling so old scene hierarchy order cannot leave a panel partially covered by other HUD elements.
- `ModalBackdrop` now positions itself behind the topmost active modal instead of the first modal found by name order.
- Added EditMode coverage for raising modal panels and keeping the backdrop behind the actual topmost visible modal.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`.

2026-05-31:

- Continued Shop/decor release polish.
- Decoration category products are active by default and Shop rows now expose clearer type, status, and effect copy, including `ON`/`OWN` states for purchased one-time items.
- Added `DecorationController` so purchased decor is visibly applied: board/background tint, butterfly variant, bee ambient visitor, and Traveler NPC color.
- Saved decoration ownership now re-applies on restore through `ShopInventory.Changed`.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Play Mode/device validation is still required to judge final art quality and whether placeholder bee/butterfly/NPC variants are shippable.

2026-05-31:

- Continued Shop/inventory UI completion.
- Added a top-bar Bag entry and `InventoryPanel` with booster rows, owned decor rows, count display for boosters, and `Use`/`Using` actions for decor.
- Decor is now applied from active decoration ids, so purchased decor can sit in the bag until the player uses it.
- Added first-pass PNG resources for Bag/shop/decor inventory icons and runtime skin wiring for the new panel and close button.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Unity import and Android device validation remain required for final icon/art readability.

2026-06-01:

- Continued Shop decor content and icon clarity.
- Added `Background: Sunset Pond` as a Decoration shop item and included it in both current scene shop catalogs.
- Added `skin_background_lily_pond`; using it from the bag swaps the gameplay background to `bg_lily_pond_sunset_01`.
- Replaced the copied placeholder decor icons with clearer first-pass PNG silhouettes for board, butterfly, bee, NPC, background, and generic decor.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Unity import/Play Mode validation remains required for final visual approval.

2026-06-01:

- Continued Level Select and first-level play flow polish.
- Level row actions now open a preview panel instead of immediately switching levels.
- The preview shows level name, difficulty/timer, objective, order reward, and starting boosters before the player taps Play.
- Generated HUDs, runtime fallback UI, Android layout, and skin pass all know the new Level preview panel/buttons.
- Added first-start HUD hints for levels 1-3 covering producer/merge, selling spare plants, and Magic Wand usage.
- Runtime and EditMode test assemblies build successfully with `/p:UseSharedCompilation=false`; Play Mode validation remains required for the full level 1-10 handoff.
