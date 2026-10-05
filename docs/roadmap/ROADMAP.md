# Roadmap

**Doc type:** project knowledge (indexed into Dify)
**Last verified:** 2026-10-05
**Companion files:** `docs/roadmap/COMPLETED.md`, `docs/roadmap/TODO.md`

This is a **state-of-the-repo roadmap**, derived from code evidence — not an aspirational marketing plan. Nothing here is promised to a date.

## COMPLETED

Prototype foundations built and committed on 2026-10-02: project settings + package set, the painting core (input → paint controller → shader), the procedural creature mesh, the 5-state creature state machine, the visual bridge (animation bridge + pooled FX + paint relief shader), generated FX sprites and animator controller, the art bible + machine-readable palette/typography, and a CI gate that verifies every numeric art claim.

Detail: `docs/roadmap/COMPLETED.md`.

## IN PROGRESS (uncommitted, in the working tree)

The "make the loop playable" feature, local paint persistence, and the Phase 3 Echo runtime foundation remain **uncommitted**. The Echo slice adds the fixed Horn Swirl → Playful Charge catalog, explicit typed binding, typed performance, local mark-ID persistence, and an icon-first Adorn interaction that paints a persistent visible mark. The roadmap's Immediate items 2–4 (PAINT scene fix, `[DBG-*]` removal, animation wiring) were applied on 2026-10-05.

Detail and hazards: `docs/roadmap/TODO.md` → "Immediate".

## BLOCKED

- **Reading assistance (voice-over / TTS / icon-only):** design question 4, unresolved.
- **Parent gate contents:** design question 5, unresolved.
- **Art-bible quality review:** design question 6 — visual reference games were catalogued (`design/Art/reference-catalog.md`) but never formally supplied for review.

## NEXT (highest value, unblocked now)

1. **Polish and validate Echo's player-facing loop:** validate Adorn/placement on-device and strengthen save recovery for the independently stored mark stroke and binding.
2. Start an audio system — the art bible already requires audio as a state-redundancy channel.

Done 2026-10-05 (previously 2–4): the PAINT button's bogus scene name, the `[DBG-*]` debris, and the unwired animation subsystem. A 5th pre-existing test failure (tap gated by a frame-count grace wait) was fixed alongside.

Detail: `docs/roadmap/TODO.md`.

## FUTURE

- Paint-color selection, richer Adorn feedback, and additional authored marks.
- A real minigame / competence channel (`Runtime/Minigames/` currently holds only a playground).
- Camera system (currently three inline static implementations).
- Parent gate, settings screen, localization hooks.
- Real character art to replace `CreatureTestMeshGenerator` (Pillar 3: skinned variants first).
- CI that actually compiles Unity and runs the test suite.
- Application identifiers / company name before any store build.

## Risks being tracked

| Risk | Severity | Note |
|---|---|---|
| Freehand 3D painting doesn't feel good on **low-end** Android | **Critical** | The single highest-risk mechanic. AGENTS.md: prototype it on real low-end hardware first; if it isn't pleasant, the concept changes and every downstream decision was premature |
| Persistence recovery | Reduced for MVP | Completed paint strokes are saved locally; future variant mapping and crash/device-loss validation remain |
| CI does not compile Unity | High | Non-compiling code ships green |
| HEAD likely red | High | Shader/input fixes live only in the working tree |
| Package weight (nav, timeline, visualscripting, collab-proxy installed but unused) | Medium | Kids-category/mobile budgets |
