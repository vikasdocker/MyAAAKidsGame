# Roadmap

**Doc type:** project knowledge (indexed into Dify)
**Last verified:** 2026-10-03
**Companion files:** `docs/roadmap/COMPLETED.md`, `docs/roadmap/TODO.md`

This is a **state-of-the-repo roadmap**, derived from code evidence — not an aspirational marketing plan. Nothing here is promised to a date.

## COMPLETED

Prototype foundations built and committed on 2026-10-02: project settings + package set, the painting core (input → paint controller → shader), the procedural creature mesh, the 5-state creature state machine, the visual bridge (animation bridge + pooled FX + paint relief shader), generated FX sprites and animator controller, the art bible + machine-readable palette/typography, and a CI gate that verifies every numeric art claim.

Detail: `docs/roadmap/COMPLETED.md`.

## IN PROGRESS (uncommitted, in the working tree)

The "make the loop playable" feature: shared `PaintingRig` builder, main menu scene, playground scene + pet motor, mouse-parity input layer, scene-registration editor tooling, `GameplayLoopTests`, and a shader compile fix. It is complete and self-consistent but **not committed**, and HEAD is likely red without it.

Detail and hazards: `docs/roadmap/TODO.md` → "Immediate".

## BLOCKED

- **Signature-mark → ability binding ("Echo"):** blocked on design question 3 in `AGENTS.md` §10 — is binding strictly 1:1, or do combinations grant emergent abilities? That answer changes the data model and content cost.
- **Save/load schema:** blocked on design question 2 — do authored paint patterns survive permanently, and what happens when creature variants arrive?
- **Reading assistance (voice-over / TTS / icon-only):** design question 4, unresolved.
- **Parent gate contents:** design question 5, unresolved.
- **Art-bible quality review:** design question 6 — visual reference games were catalogued (`design/Art/reference-catalog.md`) but never formally supplied for review.

## NEXT (highest value, unblocked now)

1. Commit or stash the in-flight feature deliberately; decide whether to land it on top of a green HEAD.
2. Fix `MainMenuBootstrap.cs:188` → load `Playtest` (or rename the scene) and add the missing test.
3. Remove `[DBG-*]` debug logging from `GameplayLoopTests.cs` and `FluidTouchInputManager.cs`.
4. Wire `Animator` + `CreatureAnimationBridge` into `PaintingRig.Build()` — the subsystem already exists and is tested.
5. Introduce save/load (`Runtime/Save/`) — the repo's own stated highest-severity risk.
6. Start an audio system — the art bible already requires audio as a state-redundancy channel.

Detail: `docs/roadmap/TODO.md`.

## FUTURE

- Echo ability binding (once design question 3 is answered).
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
| No persistence | Critical | A child's authored creation can be lost — described in-repo as "the most severe failure this game has" |
| CI does not compile Unity | High | Non-compiling code ships green |
| HEAD likely red | High | Shader/input fixes live only in the working tree |
| Package weight (nav, timeline, visualscripting, collab-proxy installed but unused) | Medium | Kids-category/mobile budgets |
