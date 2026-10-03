# Development Rules

**Doc type:** project knowledge (indexed into Dify)
**Last verified:** 2026-10-03
**Authoritative source:** `AGENTS.md` (root). This is a retrieval-friendly rendering of it plus what the code actually does. Where AGENTS.md and reality disagree, both are listed.

## Priority order when rules conflict

1. `design/gdd/game-concept.md` — design source of truth. If code contradicts it, the code is wrong.
2. `AGENTS.md` — operating rules for this repo. It declares itself the winner in a conflict, but its own status table is stale (see "Known drift" below), so always confirm status against the code.
3. `docs/framework/technical-preferences.md` — engine/naming/input/perf/testing preferences.
4. ADRs in `docs/architecture/adr-*.md` — before implementing a technical decision, write one.

## Do not

- Do not add content that isn't in a GDD. If it isn't specified, it doesn't get built.
- Do not add a second runtime assembly without an ADR.
- Do not enable URP Compatibility Mode (removed in Unity 6.3).
- Do not write to a full RenderTexture per frame for painting — paint is mask/decal-based.
- Do not add network calls to the painting path. Offline only.
- Do not add cloud saves, accounts, analytics, ads, IAP, or social features.
- Do not add an addressables/content-catalog layer until remote content is genuinely needed.
- Do not hand-author `.meta` files — Unity generates them on first open.
- Do not add fail states, timers, energy, streaks, currency, or odds mechanics.
- Do not make a drag-only interaction: every drag needs an equal-status tap.
- Do not put load-bearing text in the UI.
- Do not add a legacy `UnityEngine.Input.*` call — Input System (New) only.
- Do not pass `-quit` together with `-runTests`.

## Naming conventions (AGENTS.md §6)

| Type | Convention | Example |
|---|---|---|
| Namespaces | `Dab.Runtime.<Module>` | `Dab.Runtime.Painting` |
| Classes | PascalCase | `SignatureMark` |
| Files | match class name exactly | `SignatureMark.cs` |
| Private fields | `_camelCase` | `_lastStrokeTime` |
| Public fields | PascalCase | `ReactionIntensity` |
| Constants / enums | PascalCase | `StrokeTool` |
| Interfaces | `I` prefix | `ISaveStore` |
| Shaders | `Dab/Category` | `Dab/Paint/CreatureCanvas` |
| Prefabs | `PREFIX_Type_Name` | `PFX_Pet_Hero`, `UI_Palette` |
| Scenes | `SCN_Name` | `SCN_MainMenu`, `SCN_Playground` |
| ScriptableObjects | PascalCase + `SO` | `CreatureConfigSO` |

**Known deviations in code (do not replicate, do not "fix" without an ADR):**
- Editor namespaces: `Dab.EditorTools.Playtest` and `Dab.EditorTools.Gameplay` exist alongside the documented `Dab.Editor`.
- Scene `Playtest.unity` violates `SCN_Name` and is registered first in build settings.
- Test namespaces are `Dab.Tests.Creature`, `.Gameplay`, `.VisualBridge`, `.Playtest` — undocumented but harmless.
- No `ScriptableObject` subclass exists anywhere yet.

## Code layout (runtime folders, from AGENTS.md §5)

```
Assets/Scripts/Runtime/Core/        # Boot, DI, app state, scene flow   [PaintingRig lives here]
Assets/Scripts/Runtime/Creature/    # state machine, animation bridge   [ACTUAL code — not in AGENTS.md's list]
Assets/Scripts/Runtime/Painting/    # paint controller, procedural mesh
Assets/Scripts/Runtime/Abilities/   # Echo binding                      [EMPTY]
Assets/Scripts/Runtime/Minigames/   # competence channel                [playground only]
Assets/Scripts/Runtime/Save/        # persistence                       [EMPTY]
Assets/Scripts/Runtime/Input/       # FluidTouchInputManager
Assets/Scripts/Runtime/UI/          # icon-first UI, palette, UX factory
Assets/Scripts/Runtime/FX/          # pooled FX spawner                 [not in AGENTS.md's list]
Assets/Scripts/Runtime/Audio/       # reactive audio                    [EMPTY]
Assets/Scripts/Runtime/Procedural/  # cheap creature variation          [EMPTY]
Assets/Scripts/Runtime/Pet/         # AGENTS.md lists this; it is EMPTY
Assets/Scripts/Editor/              # editor tooling (Dab.Editor / Dab.EditorTools)
Assets/Scripts/Tests/               # PlayMode + EditMode tests (Dab.Tests)
```

## Workflow rules (AGENTS.md §8)

- Per-system GDDs are authored with `/design-system` **before** code exists for that system.
- Technical decisions are recorded with `/architecture-decision` **before** implementation. Anything affecting the runtime asmdef, asset budget, rendering approach, or persistence model requires an ADR.
- Scope is protected by Pillar 3: a new creature starts as a "can this be a skinned variant?" question.

## Testing expectations

- PlayMode tests run with `-runTests -batchmode` and **no `-quit`**.
- Zero-allocation contracts are asserted in `CreatureStateAllocationTests` and `VisualBridgeTests` — keep them passing.
- Existing gap: `Assets/Scripts/Tests/README.md` names 4 highest-value targets (paint undo + persistence round-trip, signature-mark binding, save/load, constraint tests). **None exist.** Read `docs/roadmap/TODO.md`.

## Known drift (AGENTS.md status table vs reality)

| AGENTS.md claim | Reality |
|---|---|
| "Art bible: Not created (`/art-bible` pending)" | `design/Art/art-bible.md` = 468 KB / 6,036 lines |
| "Architecture / ADRs: None" | `docs/architecture/adr-0001-touch-input-abstraction.md` exists (428 lines); `docs/registry/architecture.yaml` populated |
| "3 PlayMode tests green" | 43 test methods across 6 files |
| §7 "anchor PROVISIONAL, do not begin asset production" | asset production began; committed 2026-10-02 |
| layout lists `Runtime/Pet/` | `Pet/` empty; code is in `Runtime/Creature/` (unlisted) |
| Shaders `Dab/Paint/FurPaint` | actual: `Dab/Paint/CreatureCanvas`, `Dab/Paint/PaintStamp` |
