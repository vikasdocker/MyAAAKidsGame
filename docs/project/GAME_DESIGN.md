# Game Design — condensed summary

**Doc type:** project knowledge (indexed into Dify)
**Last verified:** 2026-10-03
**Full design (source of truth):** `design/gdd/game-concept.md` (529 lines). This file is a retrieval-friendly summary, not a replacement. When designing a system, read the GDD.

## Pitch

Paint a fluffy hatchling; every mark you make becomes an ability it performs back. A no-fail, no-pressure pet game for 6–9 year-olds.

## Loops

| Loop | Duration | Content |
|---|---|---|
| 30-second | moment | One paint stroke → immediate creature reaction |
| 5-minute | sitting | Paint a set of marks → trigger an ability → pet/feeed → reaction |
| Session | 5–15 min | Complete a creature look → see it perform → leave freely |
| Progression | days | Accumulated authored marks + creature variants; **no currency, no timers, no odds** |

Design intent (from the GDD's MDA / SDT sections): the child is motivated by **autonomy** (their marks are chosen), **competence** (the creature visibly responds), and **relatedness** (the pet reacts to them personally). Anything that introduces a score, a timer, or a failure readout breaks all three.

## The Echo mechanic (central hook — NOT IMPLEMENTED)

A "signature mark" painted by the child is bound to an ability the creature can perform. This is the whole reason the game exists.

**Implementation status:** no data model exists. `Assets/Scripts/Runtime/Abilities/` is empty. `AbilityUnlockState.AbilityId` is a bare `int` with the code comment *"left as a plain field so the flourish can be triggered in a prototype without that system existing."* There is no `SignatureMark` type and no mark→ability binding table anywhere in the repo.

## Visual identity — "Candy Sunrise" (AGENTS.md §7)

1. Light comes from behind and above. Soft warm rim on top/back edge. The creature reads as a **matte, painted shell — never fur**, because the child's paint sits directly on that surface.
2. High saturation, narrow value range, mid-to-high key, soft terminator. No blacks except thin outlines.
3. Every reaction overshoots — visible from across the room.
4. **The creature's base colour stays neutral/cream so the child's paint is always the most vivid thing in frame.** Do not break this.
5. Forbidden: pure black, desaturated grey, cold blues in shadow, any colour associated with threat or failure.

The full art bible (`design/Art/art-bible.md`, 6,036 lines) has since been written and covers mood per state, shape language, colour system, typography, character/environment/UI direction and asset standards. **KNOWN DOC DRIFT:** `AGENTS.md` §7 still says the anchor is "PROVISIONAL" and "do not begin asset production" — asset production has in fact begun (committed 2026-10-02: 3 FX PNGs, 1 animator controller, 7 placeholder clips).

## Access rules

- Every drag has an equal-status tap.
- No reading required; icons carry meaning, captions are support.
- No fail, no timer, no score, no odds, no purchases, no social, no analytics.

## Open design questions (AGENTS.md §10)

1. How is skill improvement made visible without a score or fail state?
2. Do authored paint patterns survive permanently, and what happens when creature variants are added?
3. Is signature-mark binding strictly 1:1, or do combinations grant emergent abilities? (Affects architecture and content cost.)
4. What form does reading assistance take (voice-over, TTS, icon-only)?
5. What does the parent gate contain?
6. Visual reference games were never supplied (blocking quality review of the art bible).
7. Accessibility vs. the "no grey / high saturation" rule — needs explicit resolution, especially colourblind palettes.

## Design files inventory

| File | Size | Contents |
|---|---|---|
| `design/gdd/game-concept.md` | 24 KB | Identity, pillars + anti-pillars, loops, MDA, SDT profile, audience reality check, flow, scope tiers, risk register |
| `design/Art/art-bible.md` | 468 KB | 9 sections: identity, mood per state, shape language, colour, typography, style anchor, characters, environment, UI, asset standards |
| `design/Art/reference-catalog.md` | 14.7 KB | 3 reference games (Toca Life World, Adopt Me!, Nintendogs) + coverage map |
| `design/Art/style-anchor-prompt.md` | 9.5 KB | AI image-gen style anchor + negative prompt |
| `design/Art/palette.json` / `.css` | 35 / 13 KB | Machine-readable palette incl. colourblind safety and build-time checks — **gated by CI** |
| `design/Art/typography.json` | 24 KB | Typography spec — **gated by CI** |
| `design/Systems/` | — | **Empty.** No per-system GDDs have been written |
