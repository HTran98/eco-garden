# Eco Garden - Audio Generation Prompts

Use this file to generate first-release audio assets with Gemini or another audio generator.

## Import Targets

| Type | Unity Folder | Import Notes |
| --- | --- | --- |
| SFX | `Assets/EcoGarden/Audio/SFX` | WAV, 48 kHz, 16-bit or 24-bit, mono or narrow stereo, no silence at the start |
| Music | `Assets/EcoGarden/Audio/Music` | WAV or OGG source, 48 kHz, stereo, seamless loop |

## Global Audio Direction

Eco Garden is a cozy mobile merge game set around a calm lotus pond. Audio should be soft, warm, short, and low-fatigue for repeated play on phone speakers.

Use these rules for every prompt:

- No vocals, no spoken words, no copyrighted melodies.
- Avoid harsh highs, loud impacts, scary tones, arcade lasers, or casino-style reward sounds.
- Keep SFX short and responsive. Most gameplay SFX should be 0.15-0.7 seconds.
- Use natural garden materials where possible: water drops, soft leaves, tiny wood taps, clay/stone clicks, gentle bells, airy chimes.
- Leave moderate headroom; avoid clipping or aggressive compression.
- Export each file with exactly the filename listed below.

Recommended loudness targets:

- SFX: peak around -3 dB, consistent perceived volume, not overly compressed.
- Music: soft background level, seamless loop, no dominant melody that becomes tiring.

## Required SFX

| Filename | Event | Duration | Gemini Prompt |
| --- | --- | --- | --- |
| `sfx_item_pickup_01.wav` | Player picks up a board item | 0.15-0.25s | Create a very short cozy mobile game sound for picking up a small lotus item: soft wooden tap plus tiny leaf rustle, light and tactile, no harsh click, no melody, no voice. |
| `sfx_item_drop_valid_01.wav` | Item drops onto a valid cell | 0.15-0.30s | Create a short gentle drop sound for placing a small plant token on a pond board: soft clay/wood contact with a subtle water ripple tail, warm, clean, low volume, no voice. |
| `sfx_item_drop_invalid_01.wav` | Item returns after invalid drop | 0.20-0.35s | Create a soft invalid placement sound for a cozy garden puzzle game: muted hollow tap with a tiny downward leaf flutter, polite and non-alarming, no buzzer, no harsh error tone. |
| `sfx_merge_01.wav` | Two matching items merge | 0.35-0.60s | Create a gentle merge chime for two lotus items combining: soft bell sparkle, small water shimmer, warm upward motion, satisfying but calm, no arcade sound, no voice. |
| `sfx_producer_spawn_01.wav` | Producer creates a new lotus seed | 0.25-0.45s | Create a cozy pond producer spawn sound: single water droplet, tiny bubble, soft leaf pop, light and fresh, suitable for frequent tapping, no voice. |
| `sfx_sell_item_01.wav` | Item sold in Sell basket | 0.30-0.50s | Create a calm sell confirmation sound: small coin sparkle mixed with soft leaf/wood tap, friendly reward feel, not casino-like, not loud, no voice. |
| `sfx_delivery_submit_01.wav` | Requested item delivered to customer | 0.30-0.55s | Create a warm delivery submit sound for handing a lotus item to a customer: soft whoosh into basket, gentle chime, subtle happy confirmation, no voice. |
| `sfx_order_complete_01.wav` | NPC order completes | 0.60-1.00s | Create a cozy order complete sting: warm two-note chime, soft water shimmer, small celebratory sparkle, gentle and mobile-friendly, no voice. |
| `sfx_level_complete_01.wav` | Level complete panel appears | 0.90-1.40s | Create a short level complete success sting for a cozy lotus pond merge game: warm marimba or bell notes, airy sparkle, peaceful resolution, not triumphant or loud, no voice. |
| `sfx_level_failed_01.wav` | Timer runs out / level fails | 0.70-1.10s | Create a gentle level failed sound: soft descending wooden bell and muted water ripple, mildly disappointed but not scary, no alarm, no voice. |
| `sfx_timer_warning_01.wav` | Timer warning under 20 seconds | 0.18-0.30s | Create a subtle timer warning tick for a cozy puzzle game: soft wooden tick with faint water pulse, noticeable but not stressful, suitable to repeat every second, no voice. |

## Ability SFX

| Filename | Event | Duration | Gemini Prompt |
| --- | --- | --- | --- |
| `sfx_ability_shovel_01.wav` | Shovel clears weed/pebble | 0.30-0.50s | Create a soft shovel ability sound: tiny garden scrape, pebble/leaf movement, clean pop at the end, satisfying but gentle, no harsh metal, no voice. |
| `sfx_ability_magic_wand_01.wav` | Magic Wand upgrades an item | 0.45-0.75s | Create a magical soft burst for upgrading a lotus item: airy chime, tiny sparkles, upward shimmer, cozy and delicate, no fantasy combat feel, no voice. |
| `sfx_ability_sorting_magnet_01.wav` | Sorting Magnet moves matching items | 0.45-0.75s | Create a soft magnet sorting sound: gentle magnetic hum, tiny sliding leaf movement, light chime at the end, playful but calm, no sci-fi laser, no voice. |
| `sfx_ability_unavailable_01.wav` | Ability has zero count or invalid target | 0.18-0.30s | Create a polite unavailable action sound: muted soft tap with tiny dull chime, clear but non-punishing, no buzzer, no voice. |

## Economy, Shop, Mission, and UI SFX

| Filename | Event | Duration | Gemini Prompt |
| --- | --- | --- | --- |
| `sfx_gold_gain_01.wav` | Gold balance increases | 0.30-0.50s | Create a soft gold gain sound: two tiny coin chimes with warm wooden resonance, light reward feel, not flashy, no voice. |
| `sfx_gem_gain_01.wav` | Gem balance increases | 0.35-0.60s | Create a gentle premium gem gain sound: crystal sparkle, soft water shimmer, slightly brighter than gold but still cozy, no casino style, no voice. |
| `sfx_reward_claim_01.wav` | Generic reward claimed | 0.45-0.75s | Create a warm reward claim sound: small bundle of leaf chimes and soft bell sparkle, satisfying, calm, no voice. |
| `sfx_mission_claim_01.wav` | Mission reward claimed | 0.45-0.80s | Create a mission complete reward chime for a cozy garden mobile game: friendly bell flourish, tiny sparkle, short and positive, no voice. |
| `sfx_shop_purchase_success_01.wav` | Shop purchase succeeds | 0.40-0.70s | Create a clear calm shop purchase confirmation sound: soft coin tap, warm chime, small leaf rustle, premium but not flashy, no voice. |
| `sfx_shop_purchase_failed_01.wav` | Shop purchase fails/cancelled | 0.25-0.45s | Create a gentle purchase failed or cancelled sound: muted wooden tick with soft downward chime, polite and unobtrusive, no buzzer, no voice. |
| `sfx_iap_pending_01.wav` | Store purchase enters pending state | 0.35-0.60s | Create a neutral pending purchase sound: soft two-step pulse, calm waiting feel, no success flourish, no error, no voice. |
| `sfx_button_tap_01.wav` | Standard UI button tap | 0.08-0.16s | Create a tiny cozy UI button tap sound: soft wood or leaf click, very short, low fatigue, no high-frequency snap, no voice. |
| `sfx_panel_open_01.wav` | Shop/Mission/Bag/Level panel opens | 0.20-0.35s | Create a soft panel open sound: light cloth/leaf whoosh with tiny chime, subtle and clean, no voice. |
| `sfx_panel_close_01.wav` | Panel closes or backdrop dismissed | 0.18-0.30s | Create a soft panel close sound: gentle reverse leaf whoosh and muted tap, calm, short, no voice. |
| `sfx_pause_open_01.wav` | Pause panel opens | 0.20-0.35s | Create a quiet pause sound: muted wooden tap and soft low chime, calm and clear, no voice. |
| `sfx_decoration_apply_01.wav` | Decoration activated from Bag | 0.45-0.75s | Create a cozy cosmetic apply sound: soft shimmer, leaf swirl, warm sparkle, feeling like changing garden decor, no voice. |

## Ambient and Music

| Filename | Event | Duration | Gemini Prompt |
| --- | --- | --- | --- |
| `amb_pond_day_loop_01.wav` | Optional low pond ambience | 20-40s seamless loop | Create a seamless calm lotus pond ambience loop: very soft water movement, distant gentle garden air, tiny occasional leaf movement, no birds dominating, no insects too loud, no melody, no voice. |
| `music_level_pastel_zen_01.wav` | Main gameplay music | 60-90s seamless loop | Create a seamless looping background music track for a cozy lotus pond mobile merge puzzle game: gentle marimba, soft kalimba, warm pads, light water ambience, peaceful but not sleepy, minimal melody, no drums louder than soft taps, no voice, no copyrighted style. |
| `music_menu_garden_01.wav` | Optional menu/level select music | 45-75s seamless loop | Create a seamless cozy garden menu loop: light plucked notes, soft bells, warm pad, calm optimistic mood, sparse arrangement, no strong melody, no voice. |

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
