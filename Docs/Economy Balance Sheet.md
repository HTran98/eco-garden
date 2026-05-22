# Eco Garden - Economy Balance Sheet

Date created: 2026-05-22

Purpose: track first-release economy sources, sinks, and balance risks for Levels 1-10.

## Current Assumptions

- Gold is the primary earned currency.
- Gem is premium/rare reward currency and should remain optional for first release progression.
- Levels 1-10 must be completable without Gem or IAP.
- Lotus Lv1-Lv3 are default-unlocked.
- Levels 7-9 temporarily allow Lotus Lv4.
- Level 10 temporarily allows Lotus Lv4-Lv5.
- Permanent Lv4/Lv5 unlocks remain shop/meta progression, not required for completing Levels 1-10.

## Level Order Rewards

| Level | Name | Difficulty | Timer | Order | Reward | Starting Tools | Temporary Unlock |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | First Sprouts | Easy | 240s | Lotus Lv2 x1 | Gold 25 | Shovel x1 | None |
| 2 | Tidy Pond Edge | Easy | 230s | Lotus Lv2 x2 | Gold 35 | Shovel x1 | None |
| 3 | Young Leaves | Easy | 220s | Lotus Lv3 x1 | Gold 45 | Magic Wand x1 | None |
| 4 | Weed Patch | Normal | 210s | Lotus Lv3 x2 | Gold 60 | Shovel x2 | None |
| 5 | Visitor Request | Normal | 210s | Lotus Lv2 x1, Lv3 x1 | Gold 70 | Magnet x1 | None |
| 6 | Narrow Channels | Normal | 200s | Lotus Lv3 x2 | Gold 80 | Shovel x1, Wand x1 | None |
| 7 | Bud Unlock | Normal | 200s | Lotus Lv4 x1 | Gold 100 | None | Lotus Lv4 |
| 8 | Busy Crossing | Hard | 190s | Lotus Lv3 x2, Lv4 x1 | Gold 125 | Magnet x1 | Lotus Lv4 |
| 9 | Bloom Prep | Hard | 185s | Lotus Lv4 x2 | Gold 145, Shovel x1 | None | Lotus Lv4 |
| 10 | First Bloom | Hard | 180s | Lotus Lv5 x1 | Gold 180, Gem 3 | Shovel x2, Wand x1 | Lotus Lv4-Lv5 |

Total first-clear order rewards: Gold 865, Gem 3, Shovel x1.

## Mission Rewards

| Mission | Trigger | Target | Reward |
| --- | --- | --- | --- |
| Merge Lotus | Merge | 5 merges | Gold 80 |
| Grow Seeds | Produce | 10 Lotus Lv1 | Gold 60 |
| Clear Space | Sell | 3 sells | Gold 75 |
| Finish Customer Order | Deliver | Lotus Lv2 deliveries x2 | Gold 120, Shovel x1 |
| High-Tier Lotus Order | Deliver | Lotus Lv5 x1 | Gold 250, Gem 3 |
| Use Garden Tools | Use ability | Shovel x2 | Gold 70 |

Potential mission reward total if all current missions are claimed once: Gold 655, Gem 3, Shovel x1.

## Shop Sinks

| Product | Price | Grant | Repeatable | Balance Role |
| --- | --- | --- | --- | --- |
| Small Shovel Pack | Gold 120 | Shovel x3 | Yes | Early earned-currency booster sink. |
| Small Magic Wand Pack | Gold 160 | Magic Wand x2 | Yes | Strong recovery tool; should cost more than shovel. |
| Small Sorting Magnet Pack | Gold 140 | Magnet x2 | Yes | Mid-value organization helper. |
| Butterfly Decoration | Gold 250 | Decoration | No | Optional earned cosmetic sink. |
| Unlock Lotus Tier 4 | Gold 600 | Permanent Lotus Lv4 | No | Long-term progression sink after first-release arc. |
| Premium Booster Bundle | Gem 35 | Shovel x5, Wand x4, Magnet x4 | Yes | Premium acceleration; should not be required. |
| Bird Visitor Decoration | Gem 20 | Decoration | No | Low-cost premium cosmetic. |
| NPC Skin: Traveler | Gem 40 | Decoration | No | Premium cosmetic. |
| Board Skin: Moss Stone | Gem 45 | Decoration | No | Premium cosmetic. |
| Unlock Lotus Tier 5 | Gem 60 | Permanent Lotus Lv5 | No | Premium/meta unlock; not required by Levels 1-10. |
| Small Gem Pack | IAP | Gem 80 | Yes | Store product `eco_garden_gems_small`. |
| Medium Gem Pack | IAP | Gem 220 | Yes | Store product `eco_garden_gems_medium`. |

## First-Pass Balance Read

| Area | Current Read | Risk | Next Action |
| --- | --- | --- | --- |
| Gold income | First-clear levels give 865 Gold; missions add up to 655 Gold. | Early economy may be generous if missions complete naturally during Levels 1-10. | During D5 playtest, record Gold after each level and after mission claims. |
| Booster prices | Shovel 120, Magnet 140, Wand 160 Gold. | Prices are reachable after a few level clears plus missions; boosters may reduce level pressure too early. | Check whether player can buy boosters before Levels 4-6 and whether that trivializes obstacle pressure. |
| Lv4 unlock | Permanent Lv4 costs 600 Gold, while Levels 7-10 use temporary unlocks. | Player may afford permanent Lv4 near the end of arc, but it is not needed for release levels. | Keep temporary unlocks for authored level progression; treat permanent Lv4 as post-arc convenience. |
| Lv5 unlock | Permanent Lv5 costs 60 Gem; Level 10 gives only Gem 3. | Permanent Lv5 is unreachable without IAP or future Gem rewards. | Keep as optional premium/meta sink; document as not required for first release. |
| Decorations | Gold cosmetic 250; Gem cosmetics 20-45. | Cosmetic purchases have no visible application yet. | D/E milestone must either connect decoration ownership to visuals or defer/remove decoration products. |
| IAP | Small/Medium Gem packs grant 80/220 Gem. | Production validation still required; purchases must never be required for Levels 1-10. | Keep IAP as optional acceleration until Google Play internal test passes. |

## D5 Playtest Metrics To Capture

For each Level 1-10 run:

| Metric | Why |
| --- | --- |
| Attempts to complete | Detect difficulty spikes. |
| Remaining timer seconds | Tune timer pressure. |
| Gold before/after | Tune source/sink pacing. |
| Boosters used | Detect required tools or overpowered grants. |
| Mission claims triggered | Measure accidental Gold injection. |
| Board dead-end moments | Identify layouts with too little playable space. |

## Playtest Metric Log

`LevelPlaytestMetricsController` logs a one-line metric when a level completes or fails:

```text
EcoGarden Playtest: result=Completed levelId=1 levelName="First Sprouts" remainingSeconds=210 gold=25 gem=0 shovel=1 wand=0 magnet=0
```

Use the Unity Console to copy these lines into the playtest notes. The first-release progression scene generator attaches this component automatically.

If `Eco Garden/Validation/Audit First Release Scene` reports that `LevelPlaytestMetricsController` is missing, regenerate the scene with `Eco Garden/Create Scene/First Release Progression`.

## Current Recommendation

Do not tune numeric prices yet. First run D5 playtest on the generated first-release scene and record the metrics above. The current economy is structurally safe because first-release levels use temporary tier unlocks and do not require Gem/IAP, but Gold/missions may be too generous once claims stack.
