# Current State — evidence-backed

**Doc type:** project knowledge (indexed into Dify) — the primary answer to "what works right now?"
**Last verified against code:** 2026-10-05
**Method:** full read-only audit on 2026-10-03, with phased changes since then cross-checked against code and Unity PlayMode tests.

---

## HEAD vs working tree (READ THIS FIRST)

Git HEAD = `a610734` (6 commits; the newest adds the Dify RAG knowledge layer).

**The working tree is ahead of HEAD by one complete, self-consistent feature that was never committed** ("make the loop playable"), plus the roadmap's three Immediate follow-ups applied to it on 2026-10-05:

```
 M Assets/Scripts/Runtime/Input/FluidTouchInputManager.cs        (+246) mouse-parity input layer
 M Assets/Scripts/Runtime/Playtest/CreaturePlaytestBootstrap.cs  (-74)  now calls PaintingRig.Build
 M Assets/Shaders/Paint/CreatureCanvas.shader                     (+48) vertex LOD + ShadowCaster CBUFFER fix
 M ProjectSettings/EditorBuildSettings.asset                             3 scenes registered
?? Assets/Scripts/Runtime/Core/PaintingRig.cs                     (151) shared rig builder
?? Assets/Scripts/Runtime/Minigames/                              (409) PlaygroundBootstrap + PlaygroundPetMotor
?? Assets/Scripts/Runtime/UI/                                     (535) MainMenuBootstrap, UxFactory, Palette
?? Assets/Scripts/Editor/Gameplay/GameplayScenesSetup.cs          (133)
?? Assets/Scenes/SCN_MainMenu.unity, SCN_Playground.unity
 ?? Assets/Scripts/Tests/GameplayLoopTests.cs                      (461, 12 tests)
```

**Why HEAD is probably red:** `VisualBridgeTests` (unmodified at HEAD) asserts `CreatureCanvasShaderCompilesWithoutErrors` and that shadow/depth passes displace like forward — while the fix for exactly those failures sits **uncommitted**. Likewise `GameplayLoopTests` needs `HandleMousePress/Hold/Release`, which do not exist at HEAD.

**Rule for agents:** do not modify, refactor, or commit this work as a side effect. It is the developer's in-flight feature. Roadmap "Immediate" items 2–4 (PAINT scene fix, `[DBG-*]` removal, animation wiring), local paint persistence, the Echo runtime foundation, and the initial Adorn interaction were applied deliberately on top of it at the developer's direction. The 59/59 full-suite pass predates the 3 latest constraint-guard tests; rerun when the Unity project is not in use by another session.

---

## CURRENT — works, wired, test-asserted

| System | Evidence |
|---|---|
| **Painting core** (the real heart) | `FluidTouchInputManager` → `CreaturePaintController` → `Dab/Paint/CreatureCanvas` → `_PaintMap`. Screen→UV raycast via generated `MeshCollider`; dab counting; stroke begin/end; 3 burst FX kinds; allocation-free paths |
| **Creature state machine** | `Runtime/Creature/*` (9 files, 2,065 l). 5 states + `None` sentinel, guarded transitions, `StateChanged` event, zero-allocation update path (12 tests) |
| **Procedural creature mesh** | `CreatureTestMeshGenerator.cs` (1,341 l) — explicit stand-in for a sculpted mesh |
| **FX pool** | `CreatureFXSpawner.cs` (788 l): fixed pool, round-robin reuse, `EmitParams` variation, zero-allocation contract asserted by tests; ink / heart / 12-spoke success bursts |
| **Runtime-built scenes** | 3 scenes, each a single bootstrap GameObject building the world in `Awake`. All scene script GUIDs resolve; no `.prefab` files exist at all |
| **Editor tooling** | 7 `MenuItem` entries + `-executeMethod` entry points: `CreatureAnimatorControllerGenerator`, `CreatureFXAssetGenerator`, `DabArtGeneration`, `PlaytestSceneSetup`, `GameplayScenesSetup`. Reproducible asset generation |
| **Tests** | 62 PlayMode tests across 10 files, including Adorn placement/cancellation/parity and safety-constraint regression guards. The latest 3 constraint tests have not yet been run; the other 59 passed in the last full run |
| **Animation** (wired 2026-10-05) | `PaintingRig.Build()` adds an `Animator` + `CreatureAnimationBridge` and loads the generated Base+Echo controller from `Resources` (`Assets/Art/Animation/Resources/`), mirroring the FX texture loading pattern. Asserted by `Playground_RigDrivesGeneratedAnimator` |
| **Paint persistence (MVP)** | Completed UV/color strokes are appended to a local versioned JSON-lines journal and restored through the paint compositor before first input. Writes flush on pause, quit, and rig destruction. Future-variant remapping is deferred |
| **Echo runtime + Adorn interaction (Phase 3)** | Typed catalog maps Horn Swirl → Playful Charge; icon-first Adorn mode places a visible Cocoa Ink spiral on the creature and binds the ability. Visible mark stroke and mark ID use the existing local paint and mark saves. Tap and drag-start are supported; placement mode gates freehand input |
| **Art bible + CI gate** | 468 KB art bible; `Tools/verify-art-metrics.py` (1,249 l) recomputes every numeric claim in `palette.json`/`typography.json`; CI green |
| **Main menu + playground loop** | Complete **in the working tree only** (uncommitted): play → run/paint/flourish → back. PAINT button loads `Playtest` correctly (fixed 2026-10-05; was `SCN_Playtest`, which threw `ArgumentException`) |

---

## PRESENT BUT UNWIRED (built, tested, unreachable at runtime)

| System | Evidence |
|---|---|
| **Rendering** | Only 2 of 7 shader subfolders are populated (`Paint/`). `Fur/`, `Lit/`, `Unlit/`, `UI/`, `PostProcessing/` are **empty**. No Shader Graph |
| **UI** | Runtime-built uGUI (untracked). Icon-over-caption buttons, generated rounded-rect sprite, `LegacyRuntime.ttf` fallback, 1080×1920 scaler, `InputSystemUIInputModule`. **No parent gate, no settings, no HUD, no localization hooks** |
| **Minigames** | `Runtime/Minigames/` contains a playground, not a minigame: tap ground → pet runs → flourish. Deterministic golden-angle wander, **zero `Random`**. No competence channel, no variety |
| **Camera** | No camera system — three inline `BuildCamera` implementations, static framing only, no follow/ortho/Cinemachine/portrait re-framing |
| **Config** | `Runtime/UI/Palette.cs` (58 l) is a hand-transcribed copy of `design/Art/palette.json` with **no runtime validation** that it matches (CI checks only the JSON). No `ScriptableObject` configs |

---

## ABSENT — documented but no code at all

| System | Evidence |
|---|---|
| **Audio** | `Runtime/Audio/` empty, **`Assets/Audio/` does not exist**, zero `AudioSource`/`AudioClip`. The art bible mandates audio as one of three state-redundancy channels (shape/motion/audio) |
| **Localization** | `Localization/` empty root dir; zero code hits |
| **AI / NPC** | No NavMesh/NavAgent usage despite `com.unity.ai.navigation` installed. Only "AI-like" behaviour is the playground's deterministic wander |
| **Inventory / quests / progression** | Nothing |
| **Parent gate, settings screen** | Nothing |
| **Portrait-first layout** | Orientation is AutoRotation with landscape allowed |

**Art assets:** 3 PNGs (31 KB total) are the entirety of the project's committed art. No audio, fonts, materials, textures, prefabs, or character models. Everything visual beyond those PNGs is generated at runtime by code.

---

## BROKEN

| # | Defect | Evidence |
|---|---|---|
| 1 | **HEAD is likely red.** | See "HEAD vs working tree" above. The working tree compiles clean and all 59/59 PlayMode tests pass (2026-10-05). |
| 2 | **CI never compiles Unity.** | `.github/workflows/verify-pipeline.yml` runs only the Python art gate. The repo can be entirely non-compiling and CI stays green. |
| 3 | **Application identifiers still the URP template's.** | `com.UnityTechnologies.com.unity.template.urpblank` (Android) etc.; `companyName: DefaultCompany`. |

Fixed 2026-10-05 (previously defect #1): the Paint button loaded the nonexistent scene `SCN_Playtest` and threw `ArgumentException` on tap; it now loads `Playtest`, covered by `PaintButton_LoadsPaintingDemoFromMenu`.

---

## EXPERIMENTAL / provisional

- `CreatureTestMeshGenerator` is explicitly a stand-in for a real sculpted mesh.
- 7 animation clips are self-declared `zz_placeholder_*` placeholders.
- The camera background in `Playtest` is a cold grey-blue (`0.24,0.26,0.30`), which arguably conflicts with art-bible "no cold blues / no grey"; menu and playground use `Palette.DawnCream`.
- `Assets/InputSystem_Actions.inputactions` (41 KB, URP template default) is **not referenced by any code**.

---

## KNOWN DOC DRIFT (tracked, not silently fixed)

1. `README.md:29-36` says "version not pinned", "Code: folder + assembly structure only", "This is a greenfield scaffold", "Art bible: not started", "ADRs: not started" — all false (9,540+ lines of C#, 50 tests, 468 KB art bible, ADR-0001 and ADR-0003).
2. `README.md:67-74` describes `Shaders/{Lit,Unlit,Fur,UI,PostProcessing}`, `Art/{Characters,Environment,VFX,UI}`, `Prefabs/*`, `Audio/*` — all empty; `Assets/Audio/` doesn't exist; neither `SCN_Boot` nor `SCN_Habitat` exists anywhere.
3. `README.md:111` rates the top risk against *mid-range* Android; AGENTS.md §5 superseded that with *low-end*.
4. `AGENTS.md` §9 status table says art bible / ADRs don't exist (they do) and "3 PlayMode tests" (there are 50).
5. `AGENTS.md` §7 still marks the visual anchor "PROVISIONAL — do not begin asset production"; production began.
6. `ADR-0001` status `Proposed` though fully implemented; `docs/registry/architecture.yaml` marks everything `proposed` and claims to be auto-generated (no automation exists).
7. `production/README.md` documents `stage.txt`, which does not exist (only `review-mode.txt` = `lean`).
8. `Assets/Scripts/Editor/README.md` lists 4 "planned tools not yet built" but never mentions the 5 tools that exist.
9. `Assets/Scripts/Tests/README.md` lists paint undo and non-negotiable constraint tests as targets; neither is implemented yet.
10. `docs/framework/technical-preferences.md:63` cites `design/art/art-bible.md` (lowercase) — actual is `design/Art/art-bible.md`; breaks on Linux CI if ever resolved path-wise.
11. All 31 `.cs` `.meta` files are 2-line hand-authored stubs, in violation of AGENTS.md's "do not hand-author `.meta` files".
12. `design/Art/art-bible.md` references `Assets/Fonts/` and `Assets/ThirdPartyLicenses/`, which do not exist.
