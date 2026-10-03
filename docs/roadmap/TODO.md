# TODO

**Doc type:** project knowledge (indexed into Dify)
**Last verified:** 2026-10-03
**Ordering:** by value to a child player, not by ease. No dates are promised.

## Immediate (do before adding anything new)

| # | Task | Evidence / why |
|---|---|---|
| 1 | **Land the in-flight feature deliberately** — commit the "make the loop playable" work, or stash it and fix HEAD first | HEAD (`8cef34e`) is likely red: `VisualBridgeTests` asserts the shader compiles and that shadow/depth passes displace, while the fix sits uncommitted; `GameplayLoopTests` needs `HandleMousePress/Hold/Release` which don't exist at HEAD |
| 2 | **Fix the broken PAINT button** — `MainMenuBootstrap.cs:188` loads `SCN_Playtest`, which does not exist (actual: `Playtest`) | Throws `ArgumentException` at runtime. Add the missing end-to-end test (only the Play button has one) |
| 3 | **Remove `[DBG-*]` debug logging** — 10 calls in `GameplayLoopTests.cs` (incl. per-frame loops), 2 in `FluidTouchInputManager.cs` | Debugging debris left in a finished feature |
| 4 | **Give CI something to compile** | `.github/workflows/verify-pipeline.yml` runs only the Python art gate; the repo can be entirely non-compiling and CI stays green |
| 5 | **Update stale docs** — `README.md` status table, `AGENTS.md` §9 status table, ADR-0001 status `Proposed`→`Accepted`, `Tests/README.md`, `Editor/README.md`, `production/README.md` | 12 tracked mismatches in `docs/project/CURRENT_STATE.md` → "Known doc drift" |

## Highest value to the game (the repo's own priorities, from `Assets/Scripts/Tests/README.md`)

> Its stated #1: *"Loss of a child's authored creation is the most severe failure this game has."*

| # | Task | Status |
|---|---|---|
| 1 | **Save / load + paint persistence round-trip** — `Runtime/Save/` is empty; zero `PlayerPrefs`, `JsonUtility`, `ISaveStore`, file I/O | **Not started.** No code, no test |
| 2 | **Paint undo** + stroke mapping tests | **Not started.** No undo exists in `CreaturePaintController` |
| 3 | **Signature-mark → ability binding** (`Assets/Scripts/Runtime/Abilities/` empty, no `SignatureMark` type, `AbilityUnlockState.AbilityId` is an unbacked `int`) | **Not started.** Blocked on design question 3 (1:1 vs combinations) |
| 4 | **Constraint tests** — enforce "no fail states / timers / odds / social / analytics" mechanically | **Not started** |

None of the four exist in code or in tests, despite being named the highest-value targets by the repo's own test README.

## High-value wiring work (code exists, needs connecting)

| Task | Evidence |
|---|---|
| **Wire `Animator` + `CreatureAnimationBridge` into `PaintingRig.Build()`** | ~1,100 lines built, generated, and covered by 9 tests but unreachable at runtime: the rig adds no `Animator` and no bridge, and no scene/prefab references `CreatureAnimatorController.controller` |
| **Add ADR-0001's 8 validation criteria as tests** | Sub-frame tap, DPI independence at 160/320/640, zero-GC stroke, `Screen.dpi==0` degradation, background-no-phantom, multi-touch safety — all testable, none covered |
| **Validate `Runtime/UI/Palette.cs` against `design/Art/palette.json`** | It is a hand-transcribed copy with no runtime/CI check that it still matches |
| **Drop dead asmdef references** | `Dab.Runtime` references `Unity.RenderPipelines.Universal.Runtime` and `Unity.TextMeshPro` with zero code using either |
| **Prune unused packages** | `ai.navigation`, `timeline`, `visualscripting`, `collab-proxy`, `multiplayer.center` all installed and unused — package weight on a mobile kids game |

## Systems to build next (design exists, code doesn't)

| System | Where | Note |
|---|---|---|
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
