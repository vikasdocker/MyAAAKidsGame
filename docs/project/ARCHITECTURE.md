# Architecture

**Doc type:** project knowledge (indexed into Dify)
**Last verified against code:** 2026-10-03
**Decision records:** `docs/architecture/adr-*.md` · **Registry:** `docs/registry/architecture.yaml`

## Shape of the runtime

There is **one runtime assembly**: `Assets/Scripts/Runtime/Dab.Runtime.asmdef` (root namespace `Dab.Runtime`), referencing `Unity.InputSystem`, `Unity.RenderPipelines.Universal.Runtime`, `Unity.TextMeshPro`.

| Assembly | Refs | Platforms |
|---|---|---|
| `Dab.Runtime` | InputSystem, URP Runtime, TextMeshPro | all |
| `Dab.Editor` | `Dab.Runtime` | editor only |
| `Dab.Tests` | `Dab.Runtime`, both TestRunners, `nunit.framework.dll` (`UNITY_INCLUDE_TESTS`) | all (not editor-only) |

Adding a second runtime assembly requires an ADR (AGENTS.md §5). Note: `URP Runtime` and `TextMeshPro` are referenced by `Dab.Runtime` but **never used in code** (zero hits) — dead references.

## Scene architecture: bootstrap-in-Awake

Every scene contains **exactly one GameObject** carrying a bootstrap script, and builds its entire world in `Awake()`:

| Scene | Bootstrap | Builds |
|---|---|---|
| `Playtest.unity` | `CreaturePlaytestBootstrap` | camera, light, creature, material, input bindings |
| `SCN_MainMenu.unity` | `MainMenuBootstrap` (untracked) | camera, light, live pet, Canvas + EventSystem, 3 icon buttons |
| `SCN_Playground.unity` | `PlaygroundBootstrap` (untracked) | camera, lighting, ground plane, one `PaintingRig` pet, `PlaygroundPetMotor` |

Rationale (in code comments): runtime construction dodgs hand-authored `.meta` GUIDs, which AGENTS.md forbids authoring manually.

`ProjectSettings/EditorBuildSettings.asset` registers, in order: `Playtest`, `SCN_MainMenu`, `SCN_Playground`.

## The single wiring point: `PaintingRig.Build()`

`Assets/Scripts/Runtime/Core/PaintingRig.cs` (151 lines, untracked) is the one place a playable creature is assembled:

```
PaintingRig.Build(name, pos, scale)
  → new GameObject (inactive)
  → MeshFilter + MeshRenderer
  → Shader.Find("Dab/Paint/CreatureCanvas") + cloned material   (error-logs, does not throw, if shader missing)
  → CreatureTestMeshGenerator      (procedural stand-in mesh + MeshCollider)
  → CreaturePaintController        (TargetRenderer assigned)
  → CreatureStateMachine
  → CreatureFXSpawner
  → FluidTouchInputManager
  → paint.BindToInput(...)
  → SetActive(true)
```

Returns an immutable `PaintingRigNode` handle. `CreaturePlaytestBootstrap` was refactored (uncommitted) to call this instead of repeating the wiring inline.

**KNOWN gap:** `PaintingRig.Build()` adds **no `Animator` and no `CreatureAnimationBridge`** — so the entire animation subsystem is unreachable at runtime (see `docs/architecture/systems.md`).

## Data / input flow

```
touch or mouse
  → FluidTouchInputManager        (EnhancedTouch + mouse-parity layer, DPI-normalized thresholds)
       Tapped (first-class) / DragCompleted
  → CreaturePaintController       (screen→UV raycast via generated MeshCollider, dab counting)
  → Dab/Paint/CreatureCanvas.shader → _PaintMap texture
  → CreatureFXSpawner             (pooled bursts: ink, heart, success radial)
  → CreatureStateMachine          (Idle / Painting / Petting / Feeding / AbilityUnlock)
```

Painting is **mask/decal-based — never a per-frame RenderTexture write**, and the whole path is offline (no network calls).

## State machine

`Runtime/Creature/` — 9 files, 2,065 lines. State pattern:

- `CreatureStateId`: `None 0?` → `Idle 0, Painting 1, Petting 2, Feeding 3, AbilityUnlock 4` + `None` sentinel (values as defined in `CreatureStateId.cs`).
- `CreatureStateMachine` (658 l): guarded transitions (`Allowed`, `BlockedBusy`, `BlockedInvalid`, `BlockedNotReady`), `StateChanged` event, `ForceTransitionTo` bypass for tests, zero-allocation update path asserted by `CreatureStateAllocationTests`.
- `Feeding` is reachable **only** by explicit call, never auto-entered (test-asserted).

## Rendering

- URP 17.3.0, **Render Graph mode only** — Compatibility Mode is removed in Unity 6.3 and `m_EnableRenderCompatibilityMode: 0` in `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset`.
- Two hand-written URP shaders (no Shader Graph anywhere):
  - `Dab/Paint/CreatureCanvas` (531 l): ForwardLit + ShadowCaster + DepthOnly, rim/wrap/sheen, paint-as-height relief gated by `_CREATURE_RELIEF`.
  - `Dab/Paint/PaintStamp` (95 l): `SRPDefaultUnlit` brush compositor.
- Quality levels: `Mobile` → `Mobile_RPAsset`, `PC` → `PC_RPAsset`; active RP asset `Assets/Settings/PC_RPAsset.asset`.

## Persistence

**None.** `Runtime/Save/` is empty; zero `PlayerPrefs`, `JsonUtility`, file I/O, or `ISaveStore`. Local-only saves are the stated constraint (no cloud, no account) — the system simply does not exist yet.

## Persistence of build/domain-reload state

`EditorSettings.asset`: **domain reload disabled** (`m_EnterPlayModeOptionsEnabled: 1`, `m_EnterPlayModeOptions: 0`). Static caches (e.g. `UxFactory._roundedRect`, `Palette` readonly fields) survive play sessions. Be careful with static mutable state.

## Performance budget (AGENTS.md §5 — hard ceilings, low-end Android is the floor)

| Metric | Target | Hard ceiling |
|---|---|---|
| Frame rate | 60 fps | never below 30 |
| Draw calls | < 150 | 250 |
| Visible triangles | < 300k | 500k |
| Shader variants | < 200 | 400 |
| Memory | < 500 MB | 800 MB |
| Cold start → interactive | < 5 s | 8 s |

## KNOWN DOC DRIFT (architecture)

- `docs/architecture/adr-0001-touch-input-abstraction.md` declares `Status: **Proposed**` but the decision is **fully implemented** in `FluidTouchInputManager`; `docs/registry/architecture.yaml` likewise marks its interface and all 5 forbidden patterns `proposed`.
- The ADR's 8 validation criteria have **no tests** — there is no `FluidTouchInputManager` test file at all.
- AGENTS.md §5 lists `Runtime/Pet/` in the layout; that folder is **empty**, and the real code lives in the **unlisted** `Runtime/Creature/`. `Runtime/FX/` and `Runtime/Playtest/` are also unlisted.
- AGENTS.md mandates namespace root `Dab.Editor.*`, but `PlaytestSceneSetup` and `GameplayScenesSetup` use `Dab.EditorTools.*` — and AGENTS.md's own command snippet calls `Dab.EditorTools.…`, so the doc contradicts itself.
