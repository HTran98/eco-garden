# Eco Garden - Audio Generation Prompts

Use this file to generate first-release audio assets with Gemini, Stable Audio, or another audio generator.

## Import Targets

| Type | Unity Folder | Import Notes |
| --- | --- | --- |
| SFX | `Assets/EcoGarden/Audio/SFX` | WAV, 48 kHz, 16-bit or 24-bit, mono or narrow stereo, no silence at the start |
| Music | `Assets/EcoGarden/Audio/Music` | WAV or OGG source, 48 kHz, stereo, seamless loop |

## Creative Direction

Eco Garden is a cozy mobile merge game set around a calm lotus pond. Audio should feel natural, warm, small, and low-fatigue on phone speakers.

Use the prompt briefs as natural creative direction instead of strict rule blocks. If a generator adds unwanted voice or musical material to SFX, add a short negative line such as:

```text
Avoid voices, singing, humming, spoken words, animal calls, harsh alarms, casino sounds, clipping, distortion.
```

Recommended loudness targets:

- SFX: peak around -3 dB, consistent perceived volume, not overly compressed.
- Music: soft background level, seamless loop, no dominant melody that becomes tiring.

## Prompt Format

Each prompt uses this structure:

```text
Style:
Mood:
Main sounds / instruments:
Duration:
Use case:
Technical notes:
```

Keep the filename outside the prompt unless the tool has a filename field.

## Required SFX

| Filename | Event | Prompt Brief |
| --- | --- | --- |
| `sfx_item_pickup_01.wav` | Player picks up a board item | Style: cozy mobile game foley, tiny tactile UI sound.<br>Mood: light, clean, responsive.<br>Main sounds: soft wooden tap, tiny lotus leaf rustle.<br>Duration: 0.15-0.25 seconds.<br>Use case: item pickup on a merge board.<br>Technical notes: very short transient, no melody, no voice, no harsh click. |
| `sfx_item_drop_valid_01.wav` | Item drops onto a valid cell | Style: soft garden board placement foley.<br>Mood: gentle, valid, satisfying.<br>Main sounds: clay and wood contact, subtle water ripple tail.<br>Duration: 0.15-0.30 seconds.<br>Use case: placing a plant token on a valid board cell.<br>Technical notes: warm low-volume finish, no voice, no heavy impact. |
| `sfx_item_drop_invalid_01.wav` | Item returns after invalid drop | Style: polite invalid-action UI foley.<br>Mood: soft, non-punishing, slightly downward.<br>Main sounds: muted hollow wood tap, tiny leaf flutter.<br>Duration: 0.20-0.35 seconds.<br>Use case: item returns after invalid placement.<br>Technical notes: no buzzer, no alarm, no voice, no harsh error tone. |
| `sfx_merge_01.wav` | Two matching items merge | Style: cozy magical merge chime.<br>Mood: satisfying, calm, upward.<br>Main sounds: soft bell sparkle, small water shimmer.<br>Duration: 0.35-0.60 seconds.<br>Use case: two lotus items combine into a higher tier.<br>Technical notes: gentle mobile-friendly sparkle, no arcade laser, no voice. |
| `sfx_producer_spawn_01.wav` | Producer creates a new lotus seed | Style: fresh pond spawn foley.<br>Mood: light, organic, repeatable.<br>Main sounds: single water droplet, tiny bubble, soft leaf pop.<br>Duration: 0.25-0.45 seconds.<br>Use case: producer creates a new lotus item.<br>Technical notes: suitable for frequent tapping, no voice, no loud pop. |
| `sfx_sell_item_01.wav` | Item sold in Sell basket | Style: calm reward confirmation.<br>Mood: friendly, useful, not flashy.<br>Main sounds: small coin sparkle, soft leaf and wood tap.<br>Duration: 0.30-0.50 seconds.<br>Use case: item sold in the flower shop sell area.<br>Technical notes: avoid casino slot-machine feel, no voice, no loud fanfare. |
| `sfx_delivery_submit_01.wav` | Requested item delivered to customer | Style: warm delivery handoff foley.<br>Mood: helpful, cozy, confirmed.<br>Main sounds: soft whoosh into basket, woven basket touch, gentle chime.<br>Duration: 0.30-0.55 seconds.<br>Use case: requested lotus item delivered to customer NPC.<br>Technical notes: no character voice, no spoken reaction, no big celebration. |
| `sfx_order_complete_01.wav` | NPC order completes | Style: cozy order-complete sting.<br>Mood: pleased, complete, gentle celebration.<br>Main sounds: warm two-note chime, water shimmer, small sparkle.<br>Duration: 0.60-1.00 seconds.<br>Use case: all requested order items are fulfilled.<br>Technical notes: mobile-friendly, no voice, no triumphant fanfare. |
| `sfx_level_complete_01.wav` | Level complete panel appears | Style: peaceful success sting.<br>Mood: resolved, bright, calm.<br>Main sounds: warm marimba or bell notes, airy sparkle.<br>Duration: 0.90-1.40 seconds.<br>Use case: level complete panel appears.<br>Technical notes: short, not loud, no voice, no copyrighted melody. |
| `sfx_level_failed_01.wav` | Timer runs out / level fails | Style: gentle failure feedback.<br>Mood: mildly disappointed, soft, safe.<br>Main sounds: descending wooden bell, muted water ripple.<br>Duration: 0.70-1.10 seconds.<br>Use case: timer reaches zero or level fails.<br>Technical notes: no scary tone, no alarm, no voice. |
| `sfx_timer_warning_01.wav` | Timer warning under 20 seconds | Style: subtle timer tick.<br>Mood: noticeable but not stressful.<br>Main sounds: soft wooden tick, faint water pulse.<br>Duration: 0.18-0.30 seconds.<br>Use case: repeated once per second when timer is low.<br>Technical notes: low fatigue, consistent, no voice, no alarm. |

## Ability SFX

| Filename | Event | Prompt Brief |
| --- | --- | --- |
| `sfx_ability_shovel_01.wav` | Shovel clears weed/pebble | Style: tiny garden tool foley.<br>Mood: practical, clean, satisfying.<br>Main sounds: soft garden scrape, pebble and leaf movement, small clean pop.<br>Duration: 0.30-0.50 seconds.<br>Use case: shovel ability clears an obstacle.<br>Technical notes: no harsh metal, no voice, no heavy impact. |
| `sfx_ability_magic_wand_01.wav` | Magic Wand upgrades an item | Style: delicate cozy magic.<br>Mood: bright, upward, soft.<br>Main sounds: airy chime, tiny sparkles, light shimmer.<br>Duration: 0.45-0.75 seconds.<br>Use case: magic wand upgrades a lotus item.<br>Technical notes: no combat magic, no voice, no aggressive whoosh. |
| `sfx_ability_sorting_magnet_01.wav` | Sorting Magnet moves matching items | Style: soft playful magnet effect.<br>Mood: clever, gentle, organized.<br>Main sounds: quiet magnetic hum, sliding leaf motion, light end chime.<br>Duration: 0.45-0.75 seconds.<br>Use case: sorting magnet gathers matching items.<br>Technical notes: no sci-fi laser, no voice, no harsh sweep. |
| `sfx_ability_unavailable_01.wav` | Ability has zero count or invalid target | Style: polite unavailable UI feedback.<br>Mood: clear, soft, non-punishing.<br>Main sounds: muted soft tap, tiny dull chime.<br>Duration: 0.18-0.30 seconds.<br>Use case: ability cannot be used.<br>Technical notes: no buzzer, no alarm, no voice. |

## Economy, Shop, Mission, and UI SFX

| Filename | Event | Prompt Brief |
| --- | --- | --- |
| `sfx_gold_gain_01.wav` | Gold balance increases | Style: small cozy currency reward.<br>Mood: warm, useful, light.<br>Main sounds: two tiny coin chimes, warm wooden resonance.<br>Duration: 0.30-0.50 seconds.<br>Use case: gold balance increases.<br>Technical notes: no casino style, no voice, no loud sparkle. |
| `sfx_gem_gain_01.wav` | Gem balance increases | Style: premium crystal reward.<br>Mood: bright but still cozy.<br>Main sounds: crystal sparkle, soft water shimmer.<br>Duration: 0.35-0.60 seconds.<br>Use case: gem balance increases.<br>Technical notes: slightly brighter than gold, no casino style, no voice. |
| `sfx_reward_claim_01.wav` | Generic reward claimed | Style: warm reward claim flourish.<br>Mood: satisfying, calm, positive.<br>Main sounds: leaf chimes, soft bell sparkle.<br>Duration: 0.45-0.75 seconds.<br>Use case: generic reward claim.<br>Technical notes: no voice, no huge fanfare, no harsh highs. |
| `sfx_mission_claim_01.wav` | Mission reward claimed | Style: mission-complete reward chime.<br>Mood: friendly, accomplished, short.<br>Main sounds: bell flourish, tiny sparkle.<br>Duration: 0.45-0.80 seconds.<br>Use case: mission reward is claimed.<br>Technical notes: no voice, no victory shout, no big orchestral hit. |
| `sfx_shop_purchase_success_01.wav` | Shop purchase succeeds | Style: calm shop confirmation.<br>Mood: premium, clean, friendly.<br>Main sounds: soft coin tap, warm chime, small leaf rustle.<br>Duration: 0.40-0.70 seconds.<br>Use case: successful shop purchase.<br>Technical notes: no casino sound, no voice, no loud cash register. |
| `sfx_shop_purchase_failed_01.wav` | Shop purchase fails/cancelled | Style: gentle cancelled-purchase feedback.<br>Mood: polite, unobtrusive, slightly downward.<br>Main sounds: muted wooden tick, soft downward chime.<br>Duration: 0.25-0.45 seconds.<br>Use case: shop purchase fails or is cancelled.<br>Technical notes: no buzzer, no alarm, no voice. |
| `sfx_iap_pending_01.wav` | Store purchase enters pending state | Style: neutral waiting feedback.<br>Mood: calm, pending, unresolved.<br>Main sounds: soft two-step pulse, quiet UI tone.<br>Duration: 0.35-0.60 seconds.<br>Use case: store purchase enters pending state.<br>Technical notes: no success flourish, no error tone, no voice. |
| `sfx_button_tap_01.wav` | Standard UI button tap | Style: tiny cozy UI click.<br>Mood: responsive, soft, low-fatigue.<br>Main sounds: soft wood or leaf click.<br>Duration: 0.08-0.16 seconds.<br>Use case: standard button tap.<br>Technical notes: no high-frequency snap, no voice, no tail. |
| `sfx_panel_open_01.wav` | Shop/Mission/Bag/Level panel opens | Style: soft UI panel motion.<br>Mood: smooth, clean, light.<br>Main sounds: cloth and leaf whoosh, tiny chime.<br>Duration: 0.20-0.35 seconds.<br>Use case: panel opens.<br>Technical notes: no voice, no sharp sweep, no loud impact. |
| `sfx_panel_close_01.wav` | Panel closes or backdrop dismissed | Style: soft UI panel close.<br>Mood: calm, tidy, short.<br>Main sounds: reverse leaf whoosh, muted tap.<br>Duration: 0.18-0.30 seconds.<br>Use case: panel closes or backdrop dismissed.<br>Technical notes: no voice, no clicky snap, no long tail. |
| `sfx_pause_open_01.wav` | Pause panel opens | Style: quiet pause UI feedback.<br>Mood: calm, clear, low energy.<br>Main sounds: muted wooden tap, soft low chime.<br>Duration: 0.20-0.35 seconds.<br>Use case: pause panel opens.<br>Technical notes: no voice, no alarm, no dramatic stop. |
| `sfx_decoration_apply_01.wav` | Decoration activated from Bag | Style: cozy cosmetic apply shimmer.<br>Mood: fresh, pretty, gentle.<br>Main sounds: soft shimmer, leaf swirl, warm sparkle.<br>Duration: 0.45-0.75 seconds.<br>Use case: decoration activated from Bag.<br>Technical notes: no voice, no big magic blast, no harsh highs. |

## Ambient and Music

| Filename | Event | Prompt Brief |
| --- | --- | --- |
| `amb_pond_day_loop_01.wav` | Optional low pond ambience | Style: natural field recording, small calm lotus pond during daytime.<br>Mood: quiet, relaxed, low-fatigue.<br>Main sounds: very soft water movement, tiny ripples, subtle water bubbles, light outdoor air, extremely faint leaf rustling.<br>Duration: seamless 30-second loop.<br>Use case: low background ambience for a casual mobile merge game.<br>Technical notes: pure environmental recording, non-musical, narrow stereo image, centered sound field, consistent texture, no sudden events, no birds, frogs, insects, animals, voices, singing, humming, whispering, drones, pads, or fantasy elements. |
| `music_level_pastel_zen_01.wav` | Main gameplay music | Style: cozy pastel zen instrumental, soft acoustic-electronic hybrid.<br>Mood: peaceful, focused, lightly optimistic, not sleepy.<br>Main sounds / instruments: gentle marimba, soft kalimba, warm pads, light water ambience, very soft percussion taps.<br>Duration: seamless 60-90 second loop.<br>Use case: main gameplay background music for repeated merge-board play.<br>Technical notes: minimal melody, no vocals, no speech, no choir, no copyrighted style, no dominant drums. |
| `music_menu_garden_01.wav` | Optional menu/level select music | Style: cozy garden menu instrumental.<br>Mood: calm, welcoming, optimistic.<br>Main sounds / instruments: light plucked notes, soft bells, warm pad, subtle garden air.<br>Duration: seamless 45-75 second loop.<br>Use case: menu and level select background music.<br>Technical notes: sparse arrangement, no strong melody, no vocals, no speech, no choir, no copyrighted style. |

## Priority Order

Generate in this order if time is limited:

1. `sfx_item_pickup_01.wav`
2. `sfx_item_drop_valid_01.wav`
3. `sfx_merge_01.wav`
4. `sfx_producer_spawn_01.wav`
5. `sfx_ability_shovel_01.wav`
6. `sfx_ability_magic_wand_01.wav`
7. `sfx_sell_item_01.wav`
8. `sfx_delivery_submit_01.wav`
9. `sfx_order_complete_01.wav`
10. `sfx_level_complete_01.wav`
11. `sfx_timer_warning_01.wav`
12. `sfx_button_tap_01.wav`
13. `music_level_pastel_zen_01.wav`

## Unity Import Checklist

After generating files:

1. Put SFX files in `Assets/EcoGarden/Audio/SFX`.
2. Put music and ambience loops in `Assets/EcoGarden/Audio/Music`.
3. In Unity, set SFX load type to `Decompress On Load` or `Compressed In Memory` depending on final memory profile.
4. Set music load type to streaming or compressed in memory after Android testing.
5. Verify loops have no click at the seam.
6. Keep generated `.meta` files with the project after Unity imports the audio.
