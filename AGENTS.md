# AGENTS.md — MyAAAKidsGame

**Read this before writing any code, creating any asset, or answering any question in this repository.**

This file is the single source of truth for *how work gets done here*. It is
generated from `design/gdd/game-concept.md` — if code and this document ever
disagree, the document wins and the code is the bug.

---

## 1. What This Project Is

A **3D pet simulator for kids aged 6–9** on **high-end mobile (iOS + Android)**,
built in **Unity with the Universal Render Pipeline**.

The child paints a plain fluffy hatchling, adorns it with signature marks, and
**every mark they make becomes an ability the creature performs back**. They never
fail, never lose, and never get punished for leaving.

**Working title:** "Dab" — not final.

**Full concept:** `design/gdd/game-concept.md`

---

## 2. Non-Negotiable Design Constraints

These are not preferences. A change that violates one of these is a **defect**,
even if it passes review.

1. **The child can never fail, get stuck, lose progress, or be punished for
   coming back.** No fail states. No timers. No energy. No streaks. No currency.
   No ads. No IAP.
2. **Nothing is rarity-gated, random, or loot-boxed.** Rare things are earned by
   playing. There is no odds mechanic anywhere in this codebase.
3. **No social features.** No trading, gifting, chat, or player-to-player
   exchange of any kind. Do not add a networking layer for gameplay.
4. **Every drag gesture must have an equal-status tap fallback.** The tap path is
   not a degraded accessibility mode — it is a first-class parallel path.
5. **Nothing in the UI may require reading.** Text is supportive, never
   load-bearing. Every action is discoverable without reading text.
6. **No purchase prompts, notifications, or interstitials during play.**
7. **Zero third-party analytics or ad SDKs.** Required for App Store / Play
   Store kids-category compliance. Verify before adding any package dependency.

---

## 3. Four Pillars — With Their Design Tests

When two options are in tension, the pillar wins. Apply the design test literally.

### Pillar 1 — Authored, Not Decorated
Every choice the child makes is visible on the creature, **does something**, and
is kept permanently.
- *Test:* "Does this pattern enable a move?" — If not, give it a move or cut it.

### Pillar 2 — Nothing Can Go Wrong
A 5-year-old can never fail or lose anything.
- *Test:* "Can this fail state become a retry that's *better* than the first
  attempt?" — If not, remove the fail state.

### Pillar 3 — Rich From Restraint
Visual ambition comes from art direction, never content volume.
- *Test:* "Can this creature be a skinned variant of an existing mesh?" — If yes,
  do that instead of modelling a new one.

### Pillar 4 — Two Tools, Both Toys
Freehand for the joyful part; deliberate placement for the meaningful part.
- *Test:* "Does this make freehand *less* freehand to satisfy precision?" — If
  yes, find another solution (bigger hit volumes, forgiving snapping).

---

## 4. Technology Stack

- **Engine**: Unity 6000.3.11f1 (Unity 6.3 LTS)
- **Language**: C#
- **Build System**: Unity Build Pipeline
- **Asset Pipeline**: Unity Asset Import Pipeline + Addressables
- **Render Pipeline**: Universal Render Pipeline (URP), **Render Graph mode only**
- **Input**: Unity Input System (touch-first)
- **Scripting Backend**: IL2CPP (iOS + Android)
- **Target Platforms**: iOS, Android
- **Primary Input**: Touch

> **URP Compatibility Mode is removed in Unity 6.3.** This is not a style
> preference — `RenderGraphSettings.enableRenderCompatibilityMode` is read-only
> `false`. If you are following any tutorial, course, or AI-generated snippet that
> enables Compatibility Mode, it is wrong for this project.
>
> **Addressables is listed as the studio default but is NOT installed yet.**
> Do not add it until remote content is actually needed — it is meaningful
> package weight for mobile, and this game is offline-first with no content
> catalogs.

---

## 5. Hard Technical Constraints

### Performance Budget — Low-End Android Is the Floor

A 6-year-old's phone is **not** a flagship, and it is often the cheapest Android
device in the house. Treat the budget below as a hard ceiling, verified on real
**low-end** hardware before release. Ratified during `/art-bible` authoring; this
supersedes the earlier mid-range assumption, and `technical-preferences.md` has been
aligned to match.

| Metric | Target | Hard Ceiling |
|---|---|---|
| Frame rate | 60 fps | **Never drop below 30** |
| Draw calls per frame | < 150 | 250 absolute |
| Triangle count (visible) | < 300k | 500k absolute |
| Shader variants | < 200 | 400 absolute |
| Memory footprint | < 500 MB | 800 MB absolute |
| Cold start to interactive | < 5 s | 8 s |
| Session length | 5–15 min | n/a |

**Enforce batching from the first prototype.** Draw-call regressions discovered
late are architectural, not fixable.

### Technical Constraints

- **Rendering:** URP, **not** HDRP. HDRP is not viable for the target device
  range and costs this stylized look nothing.
- **Asset budget:** Buy visual quality with **shading and lighting**, not
  polygon count or texture resolution. This is the core art-pipeline strategy.
- **Painting:** mask/decal-based. Do not write to a full RenderTexture per frame.
- **Painting path must be fully offline** — no network calls, no stutter, no
  latency on the core interaction.
- **Persistence:** local saves only. No cloud. No account system.
- **Input:** Unity Input System, touch-first, one-thumb reachable in portrait.

### Namespace & Code Layout

Runtime code lives in **one assembly definition** — do not add more without an ADR.

```
Assets/Scripts/Runtime/Core/        # Boot, DI, app state, scene flow
Assets/Scripts/Runtime/Pet/         # Creature state machine, reaction system
Assets/Scripts/Runtime/Painting/    # Painting system, signature marks
Assets/Scripts/Runtime/Abilities/   # Signature-mark → ability binding (Echo)
Assets/Scripts/Runtime/Minigames/   # Competence channel
Assets/Scripts/Runtime/Save/        # Authored-design persistence
Assets/Scripts/Runtime/Input/       # Touch abstraction, gesture recognition
Assets/Scripts/Runtime/UI/          # Icon-first UI, palette, parent gate
Assets/Scripts/Runtime/Audio/       # Reactive audio
Assets/Scripts/Runtime/Procedural/  # Cheap creature variation generation
Assets/Scripts/Editor/              # Editor tooling
Assets/Scripts/Tests/               # PlayMode + EditMode tests
```

**Namespace root:** `Dab.Runtime.*` and `Dab.Editor.*`

---

## 6. Naming Conventions

| Type | Convention | Example |
|---|---|---|
| Namespaces | `Dab.Runtime.<Module>` | `Dab.Runtime.Painting` |
| Classes | PascalCase | `SignatureMark` |
| Files | Match class name exactly | `SignatureMark.cs` |
| Private fields | `_camelCase` | `_lastStrokeTime` |
| Public fields | PascalCase | `ReactionIntensity` |
| Constants / enums | PascalCase | `StrokeTool` |
| Suffix interfaces | `I` prefix | `ISaveStore` |
| Shaders | `Dab/Category` | `Dab/Paint/FurPaint` |
| Prefabs | `PREFIX_Type_Name` | `PFX_Pet_Hero`, `UI_Palette` |
| Scenes | `SCN_Name` | `SCN_Boot`, `SCN_Habitat` |
| ScriptableObjects | PascalCase + `SO` in name | `CreatureConfigSO` |

---

## 7. Visual Identity — "Candy Sunrise"

**The rule:** everything is warm, rounded, and lit like morning. No sharp edges,
no grey, no shadows that mean anything is wrong.

1. **Light comes from behind and above, always.** A soft warm rim along the top
   and back edge; the creature reads as a matte, painted shell — **never fur**,
   because the child's paint sits directly on this surface. The creature must
   stay readable against any background.
2. **High saturation, narrow value range.** Mid-to-high key, soft terminator. No
   blacks except thin outlines.
3. **Every reaction overshoots.** Must be visible from across a room.

**Critical colour rule:** the creature's **base colour stays neutral/cream so the
child's paint is always the most vivid thing in frame.** This is how authorship
becomes visible at a glance — do not break this.

**Forbidden:** pure black, desaturated grey, cold blues in shadow, any colour
associated with threat or failure.

> ⚠️ **This anchor is PROVISIONAL.** `/art-bible` has not yet formalized it, and
> no visual reference games were supplied. Do not begin asset production.

---

## 8. Workflow Rules

- **`design/gdd/game-concept.md` is the source of truth.** All per-system GDDs
  trace back to it. If code contradicts it, the code is wrong.
- **Per-system GDDs are authored with `/design-system`** before code exists for
  that system.
- **Technical decisions are recorded with `/architecture-decision`** before they
  are implemented. Any decision affecting the runtime asmdef, asset budget,
  rendering approach, or persistence model requires an ADR.
- **Do not add content that isn't in a GDD.** If it isn't specified, it doesn't
  get built.
- **Scope is protected by Pillar 3.** New creature = variant question first.

---

## 9. Environment Status

| Item | State |
|---|---|
| Engine | **Unity 6000.3.11f1** (Unity 6.3 LTS), URP, Render Graph only |
| Installed locally | ✅ **Installed.** `C:\Program Files\Unity\Hub\Editor\6000.3.11f1\Editor\Unity.exe` reports `6000.3.11f1_3000ef702840`. Installed via `unity install 6000.3.11f1`. |
| Unity Hub CLI | ✅ `C:\Users\vikas\AppData\Local\Unity\bin\unity.exe` (not on `PATH`). Use for `projects new`, `run`, `templates list`. |
| Engine reference docs | ✅ `docs/engine-reference/unity/VERSION.md` |
| Technical preferences | ✅ `docs/framework/technical-preferences.md` |
| Art bible | Not created (`/art-bible` pending) |
| Per-system GDDs | None (`/map-systems` pending) |
| Architecture / ADRs | None (`/create-architecture` pending) |
| Project metadata | ✅ CLI-generated. `unity projects new` with the `com.unity.template.urp-blank` template, merged into the existing folder. Editor opens and imports clean. |
| Scene files | ✅ `Assets/Scenes/Playtest.unity`, generated by `PlaytestSceneSetup.Rebuild`. Rig hierarchy is built at runtime by `CreaturePlaytestBootstrap` — see below. |
| Code | `FluidTouchInputManager`, `CreatureTestMeshGenerator`, `CreaturePaintController`, `CreaturePlaytestBootstrap` compiling clean. 3 PlayMode tests green. |

Folders are structured. Unity generates `.meta` files on first project open — do
not hand-author them.

### Before First Build

1. ✅ Pin the Unity version — **done: 6000.3.11f1**
2. ✅ Install the editor — **done**, via `unity install 6000.3.11f1`
3. ✅ Generate project metadata with the CLI URP template — **done**.
   `.meta` files are generated; do not hand-author them.
4. In Project Settings → Player → **Active Input Handling: Input System Package
   (New)**. The legacy Input Manager is marked for deprecation in 6.3 and will
   log warnings if it is the active handler.
5. ✅ Create the playtest scene — **done** via
   `PlaytestSceneSetup.Rebuild` (`Dab/Playtest/Rebuild Playtest Scene`). It holds
   a single GameObject carrying `CreaturePlaytestBootstrap`, which builds the
   camera, light, creature, material, and input bindings at runtime.
6. `/art-bible` before any asset production
7. **Prototype freehand 3D painting on LOW-END Android FIRST** — it is the
   single highest-risk mechanic. If it isn't pleasant on a real device, the
   concept changes and every downstream decision was premature.

### Running things

```powershell
$cli = "C:\Users\vikas\AppData\Local\Unity\bin\unity.exe"
$ed  = "C:\Program Files\Unity\Hub\Editor\6000.3.11f1\Editor\Unity.exe"

# Import + compile check
& $cli run "C:\Users\vikas\OneDrive\Desktop\MyAAAKidsGame"

# Rebuild the playtest scene
& $ed -batchmode -nographics -quit -projectPath <repo> `
    -executeMethod Dab.EditorTools.Playtest.PlaytestSceneSetup.Rebuild -logFile <log>

# PlayMode tests
& $ed -runTests -batchmode -projectPath <repo> `
    -testPlatform PlayMode -testResults <results.xml> -logFile <log>
```

Do **not** pass `-quit` to a script that enters play mode: the editor exits
before play mode starts and reports success regardless. PlayMode test runs and
`-runTests` must also omit it.

---

## 10. Open Questions Blocking Downstream Work

1. How is skill improvement made **visible without a score or fail state**?
2. Do authored paint patterns survive permanently, and what happens when creature
   variants are added?
3. Is signature-mark binding strictly 1:1, or do combinations grant emergent
   abilities? — **Affects architecture and content cost.**
4. What form does reading assistance take (voice-over, TTS, icon-only)?
5. What does the parent gate contain?
6. Visual reference games — not supplied. **Blocking art bible quality.**
7. Accessibility vs. the "no grey / high saturation" rule — needs explicit
   resolution, especially colorblind palettes.

---

## 11. Project Knowledge — Dify RAG

This repo has a persistent knowledge index so agents keep context across model
and provider changes. **Git stays the source of truth**; Dify only stores a
retrieval index of curated documents.

**Index:** knowledge base `MyAAAKidsGame Project Knowledge` on the local Dify
instance (`http://localhost`), high-quality indexing, 15 documents from
`docs/`. Credentials live in `.env` (gitignored — copy `.env.example`).

```bash
# Recover context in a fresh session
python scripts/rag_query.py "What is the current state of the game?"
python scripts/rag_query.py "What systems are incomplete or missing?"
python scripts/rag_query.py "What are the non-negotiable design constraints?"
python scripts/rag_query.py "What is the highest priority next task?"

# After editing anything under docs/
python scripts/rag_sync.py --only docs/project/CURRENT_STATE.md
```

**Retrieval is raw chunk search with no LLM** — cheap and model-independent,
but it can be stale. Therefore:

1. **Cross-check every retrieved claim against the code before acting on it.**
   Chunks cite their source document; open it, then grep the code for anything
   about what exists or works.
2. **For status questions, `docs/project/CURRENT_STATE.md` + the code beat this
   file's §9 table**, which is known-stale (it still says the art bible and ADRs
   do not exist). §2 and §3 above — the constraints and pillars — remain
   non-negotiable.
3. **Never index source code, the 468 KB art bible, or secrets.** Summaries only.
4. **Record doc/code mismatches** in `docs/project/CURRENT_STATE.md` →
   "Known doc drift" rather than silently editing either side.
5. Full operating rules: `docs/agents/CODING_RULES.md`; setup and failure modes:
   `docs/agents/RAG_GUIDE.md`; session checklist: `docs/agents/OPENCODE_CONTEXT.md`.
