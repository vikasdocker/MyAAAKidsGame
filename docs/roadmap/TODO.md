# TODO

**Doc type:** project knowledge (indexed into Dify)
**Last verified:** 2026-10-03
**Ordering:** by value to a child player, not by ease. No dates are promised.

## Immediate (do before adding anything new)

| # | Task | Evidence / why |
|---|---|---|
| 1 | **Land the in-flight feature deliberately** — commit the "make the loop playable" work, or stash it and fix HEAD first | HEAD (`a610734`) is likely red: `VisualBridgeTests` asserts the shader compiles and that shadow/depth passes displace, while the fix sits uncommitted; `GameplayLoopTests` needs `HandleMousePress/Hold/Release` which don't exist at HEAD. The working tree plus Phase 3 is green: 59/59 PlayMode tests, 2026-10-05 |
| 2 | **(DONE 2026-10-05) Fix the broken PAINT button** — `MainMenuBootstrap.cs:188` loaded `SCN_Playtest`, which does not exist (actual: `Playtest`) | Fixed: the button now loads `Playtest`, and `PaintButton_LoadsPaintingDemoFromMenu` covers it end-to-end alongside the existing Play-button test |
| 3 | **(DONE 2026-10-05) Remove `[DBG-*]` debug logging** | Removed all 10 calls in `GameplayLoopTests.cs` (incl. per-frame loops) and both in `FluidTouchInputManager.cs`, plus the counters that existed only to feed them |
| 4 | **Give CI something to compile** | `.github/workflows/verify-pipeline.yml` runs only the Python art gate; the repo can be entirely non-compiling and CI stays green |
| 5 | **Update stale docs** — `README.md` status table, `AGENTS.md` §9 status table, ADR-0001 status `Proposed`→`Accepted`, `Tests/README.md`, `Editor/README.md`, `production/README.md` | 12 tracked mismatches in `docs/project/CURRENT_STATE.md` → "Known doc drift" |

Also fixed 2026-10-05 while running the suite: `GroundTap_SendsPetRunning_ThenFlourish` gave movement only 360 frames, which was not enough simulated time for the pet to turn and reach its target in fast batch runs. It now waits up to six real-time seconds for the expected run/flourish.

## Highest value to the game (the repo's own priorities, from `Assets/Scripts/Tests/README.md`)

> Its stated #1: *"Loss of a child's authored creation is the most severe failure this game has."*

| # | Task | Status |
|---|---|---|
| 1 | **Save / load + paint persistence round-trip** | **DONE 2026-10-05:** versioned local JSON-lines journal stores completed UV/color strokes, replays through the paint compositor, and recovers prior records if the final append is incomplete |
| 2 | **Paint undo** + stroke mapping tests | **Not started.** No undo exists in `CreaturePaintController` |
| 3 | **Signature-mark → ability binding** | **MVP DONE 2026-10-05:** fixed Horn Swirl → Playful Charge catalog, explicit typed binding, local persistence, and player-facing Adorn placement. Further mark content and device validation remain |
| 4 | **Constraint tests** — enforce "no fail states / timers / odds / social / analytics" mechanically | **PARTIAL:** regression tests cover fail/loss state names, forbidden meta-system types, random gameplay decisions, and third-party ad/analytics packages. Timer/social source-level guards and broader package identifiers remain |

Paint save/load, the Echo runtime foundation, and the initial player-facing Adorn/mark placement have code and regression tests. Paint undo, paint-color selection, richer Adorn feedback, and broader non-negotiable constraint coverage remain outstanding.

## High-value wiring work (code exists, needs connecting)

| Task | Evidence |
|---|---|
| **(DONE 2026-10-05) Wire `Animator` + `CreatureAnimationBridge` into `PaintingRig.Build()`** | Done: the rig now adds both and loads the generated controller from `Assets/Art/Animation/Resources/` via `Resources.Load`, asserted by `Playground_RigDrivesGeneratedAnimator`. Menu layout scale moved onto a `MenuPetSlot` parent because the breathing clip drives `localScale.y` on the creature root |
| **Add ADR-0001's 8 validation criteria as tests** | Sub-frame tap, DPI independence at 160/320/640, zero-GC stroke, `Screen.dpi==0` degradation, background-no-phantom, multi-touch safety — all testable, none covered |
| **Validate `Runtime/UI/Palette.cs` against `design/Art/palette.json`** | It is a hand-transcribed copy with no runtime/CI check that it still matches |
| **Drop dead asmdef references** | `Dab.Runtime` references `Unity.RenderPipelines.Universal.Runtime` and `Unity.TextMeshPro` with zero code using either |
| **Prune unused packages** | `ai.navigation`, `timeline`, `visualscripting`, `collab-proxy`, `multiplayer.center` all installed and unused — package weight on a mobile kids game |

## Systems to build next (design exists, code doesn't)

| System | Where | Note |
|---|---|---|
| **Paint save/load** | `Runtime/Save/` | **DONE 2026-10-05** for the single MVP creature; future-variant remapping is deferred |
| **(MVP DONE) Adorn UI and visible signature-mark placement** | `Runtime/UI/SignatureMarkAdornUI.cs`, `Runtime/Abilities/SignatureMarkPlacementController.cs` | Icon-first mode, cancellable placement, tap/drag parity, persistent visible Horn Swirl and fixed ability binding; see ADR-0005. Device validation and richer feedback remain |
| **Paint-color selection** | `Runtime/UI/`, `Runtime/Painting/` | Freehand input still uses white; the neutral cream base now makes Cocoa Ink marks legible, but child-selectable vivid paints remain |
| Audio | `Runtime/Audio/` empty, `Assets/Audio/` doesn't exist | Art bible mandates audio as one of three state-redundancy channels (shape / motion / audio) |
| Camera system | inline in 3 bootstraps | No follow, no portrait re-framing despite the "portrait-first" design |
| Parent gate + settings screen | — | Design question 5 unresolved |
| Localization hooks | `Localization/` empty | Zero code hits |
| Real minigame / competence channel | `Runtime/Minigames/` holds only a playground | Deliberately scoreless — see Pillar 2 |
| Portrait orientation lock | `defaultScreenOrientation: 4` (AutoRotation) | Design says portrait-first; settings allow landscape |

## Before any store build

- Change `applicationIdentifier` (still the URP template's `com.UnityTechnologies.com.unity.template.urpblank`) and `companyName` (`DefaultCompany`).
- Reconcile `Playtest.unity` scene name with the `SCN_Name` convention (it is registered first in build settings).
- Verify zero analytics/ad SDKs (currently clean) and kids-category compliance.
- Profile on real **low-end** Android against the AGENTS.md §5 budget table.
- Resolve accessibility vs. the "no grey / high saturation" colour rule (design question 7).

## Deliberately not doing

- No networking layer for gameplay (constraint 3).
- No Addressables until remote content is genuinely needed.
- No new creature models before asking "can this be a skinned variant of an existing mesh?" (Pillar 3).
