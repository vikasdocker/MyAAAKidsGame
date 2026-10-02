# Technical Preferences — MyAAAKidsGame

> Framework-level technical defaults. **Specific values here are chosen by the
> team**, not imposed by the engine. Read `AGENTS.md` for project constraints
> first — this file assumes you have.

---

## Engine & Language

- **Engine**: Unity 6000.3.11f1 (Unity 6.3 LTS)
- **Language**: C#
- **Build System**: Unity Build Pipeline
- **Asset Pipeline**: Unity Asset Import Pipeline + Addressables *(Addressables installed later — not required for offline-first)*
- **Render Pipeline**: Universal Render Pipeline (URP), Render Graph mode only
- **Scripting Backend**: IL2CPP
- **Install Path**: `C:\Program Files\Unity\Hub\Editor\6000.3.11f1`

---

## Naming Conventions (Unity / C#)

- Classes: PascalCase (`FluidTouchInputManager`)
- Public fields & properties: PascalCase (`IsTouching`)
- Private fields: `_camelCase` (`_activeTouchId`)
- Methods: PascalCase (`TryGetStroke()`)
- Files: PascalCase matching class name exactly (`FluidTouchInputManager.cs`)
- Constants: PascalCase (`MaxTrackedTouches`)
- Namespaces: `Dab.Runtime.<Module>`
- Interfaces: `I` prefix (`ISaveStore`)

**Enforced:** file name must match the class name. Unity's MonoBehaviour lookup
fails confusingly when they diverge.

---

## Input & Platform

- **Target Platforms**: iOS, Android
- **Input Methods**: Touch
- **Primary Input**: Touch
- **Gamepad Support**: None
- **Touch Support**: Full
- **Platform Notes**:
  - Portrait-first, one-thumb reachable
  - **Every drag gesture must have an equal-status tap fallback** — the tap path
    is a first-class parallel route, not an accessibility afterthought
  - Nothing in the UI may require reading; text is never load-bearing
  - New Input System only — legacy `UnityEngine.Input` is deprecated in 6.3
  - `EnhancedTouchSupport.Enable()` at boot, never lazily mid-gesture
  - Touches tracked by `touchId`, not array index

---

## Performance Budgets

Target: **low-end Android is the floor.** Ratified by the Creative Director during
`/art-bible` authoring; this supersedes the earlier mid-range assumption. Budgets are
now authored to the low end of the install base, which is the correct posture for a
children's app: widening reach matters more than the last 30% of headroom. Hard
ceilings from `AGENTS.md` §5.

> Rationale recorded in `design/art/art-bible.md` §8.1, which carries the full peak-scene
> ledger. If a device is ever dropped from the support matrix, this floor may be revisited
> — but that is a product decision, not an engineering one.

| Metric | Target | Hard Ceiling |
|---|---|---|
| Frame rate | 60 fps | never below 30 |
| Frame time | 16.6 ms | 33.3 ms |
| Draw calls / frame | 150 | 250 |
| Triangles (visible) | 300k | 500k |
| Shader variants | 200 | 400 |
| Memory footprint | 500 MB | 800 MB |
| Cold start to interactive | 5 s | 8 s |

Enforce batching from the first prototype. Draw-call regressions found late are
architectural, not fixable.

---

## Testing

- **Framework**: NUnit, via the `Dab.Tests` assembly definition
- **Mode**: EditMode + PlayMode
- **Packages**: `com.unity.test-framework`

Highest-value targets, in order:

1. Paint system — stroke-to-surface mapping, persistence round-trip
2. Signature-mark → ability binding
3. Save/load — an authored design must survive a full restart intact
4. Constraint tests — no fail states, no timers, no random rewards

> Loss of a child's authored creation is the most severe failure this game has.
> Test it hardest.

---

## Engine Specialists

- **Primary**: unity-specialist
- **Language/Code Specialist**: unity-specialist (C# review — primary covers it)
- **Shader Specialist**: unity-shader-specialist (Shader Graph, HLSL, URP materials)
- **UI Specialist**: unity-ui-specialist (UI Toolkit UXML/USS, uGUI Canvas, runtime UI)
- **Additional Specialists**: unity-dots-specialist (ECS, Jobs, Burst — only if adopted), unity-addressables-specialist (asset loading, memory management, content catalogs — only once Addressables is installed)

**Routing Notes**: Invoke primary for architecture and general C# review. Invoke
shader specialist for rendering, materials, and the planned custom fur/paint
shaders. Invoke UI specialist for all interface implementation. Invoke DOTS
specialist only if ECS is actually adopted — this project is currently plain
MonoBehaviour. Invoke Addressables specialist when asset management work begins.

### File Extension Routing

| File Extension / Type | Specialist to Spawn |
|---|---|
| Game code (`.cs` files) | unity-specialist |
| Shader / material (`.shader`, `.shadergraph`, `.mat`) | unity-shader-specialist |
| UI / screen (`.uxml`, `.uss`, Canvas prefabs) | unity-ui-specialist |
| Scene / prefab / level (`.unity`, `.prefab`) | unity-specialist |
| Assembly definitions (`.asmdef`) | unity-specialist |
| Native extension / plugin (`.dll`, native plugins) | unity-specialist |
| General architecture review | unity-specialist |

---

## Forbidden Patterns

- ❌ URP Compatibility Mode — removed in 6.3, `enableRenderCompatibilityMode` is read-only `false`
- ❌ Legacy `UnityEngine.Input` API — deprecated in 6.3
- ❌ Polling `Touchscreen.current.touches` inside `Update` — misses state changes
- ❌ Enabling `EnhancedTouchSupport` lazily at first touch — loses in-progress gestures
- ❌ Tracking touches by array index — IDs may be reused
- ❌ Third-party analytics or advertising SDKs — kids-category store compliance
- ❌ Fail states, timers, streaks, energy, currency, ads, IAP, or any odds/rarity mechanic
- ❌ Player-to-player exchange of any kind (trading, gifting, chat)
- ❌ Network calls on the painting or input paths
- ❌ File names that diverge from class names

---

## Allowed Libraries

None beyond Unity packages currently integrated.

Do **not** speculatively add dependencies. Add a library here only when it is
actively being integrated in a session.

| Library | Status | Rationale |
|---|---|---|
| `com.unity.render-pipelines.universal` | Active | Core render pipeline |
| `com.unity.inputsystem` | Active | Touch input — new Input System only |
| `com.unity.ugui` | Active | UI foundation |
| `com.unity.test-framework` | Active | NUnit testing |
| `com.unity.addressables` | **Deferred** | Not installed. Offline-first game with no remote content catalogs; meaningful mobile package weight for no current benefit. Install when remote content is actually required. |
| Analytics / advertising SDKs | **Prohibited** | Kids-category compliance |

---

## Engine Version Awareness

**Pinned: Unity 6000.3.11f1 (Unity 6.3 LTS)** — supported to December 2027.

This version shipped in December 2025, **after** the May 2025 LLM knowledge
cutoff. Guidance from model memory may reference removed APIs. Verify against
the reference docs before writing engine-specific code.

| Reference | Contents |
|---|---|
| `docs/engine-reference/unity/VERSION.md` | Version pin, risk analysis, package versions |
| `docs/engine-reference/unity/breaking-changes.md` | 6.0 → 6.3 breaking changes |
| `docs/engine-reference/unity/deprecated-apis.md` | "Don't use X → Use Y" tables |
| `docs/engine-reference/unity/current-best-practices.md` | Current practices |

**Highest-risk trap:** URP Compatibility Mode was removed in 6.3. Nearly all
pre-2025 URP material assumes it exists.