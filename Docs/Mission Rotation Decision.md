# Eco Garden - Mission Rotation Decision

Date decided: 2026-05-25

## Decision

The first release uses static, one-time missions only.

Daily or rotating missions are deferred until after the first Android release candidate. `MissionDefinition.IsDaily` remains in the data model for future content, but `MissionController` skips daily missions by default so the first-release save model and UI do not imply a refresh cadence that does not exist yet.

## Rationale

- Static missions match the current save format: each mission stores `missionId`, `progress`, and `rewardClaimed`.
- No extra timestamp, timezone, server-clock, or refresh-window behavior is required for the first release.
- The current Mission UI already presents a fixed list and one-time claim state.
- Daily rotation would need additional testing for app restarts, offline clock changes, duplicate reward protection, stale mission rows, and partial progress reset rules.

## First-Release Behavior

- Mission progress persists until the mission is completed or claimed.
- Claimed rewards stay claimed across restarts.
- Mission rows are loaded from authored static mission assets in sort order.
- Assets marked `isDaily` are ignored by default and should not appear in the first-release Mission panel or compact tracker.

## Future Daily-Mission Requirements

Before enabling daily missions, add:

- Save fields for last mission refresh time and active rotation group.
- Clear rules for local time vs server time.
- Duplicate claim protection across refreshes.
- UI copy that distinguishes static missions from rotating missions.
- EditMode coverage for refresh, expired missions, restart persistence, and clock-change edge cases.
