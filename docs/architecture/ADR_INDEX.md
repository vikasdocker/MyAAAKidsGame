# ADR Index

**Doc type:** project knowledge (indexed into Dify)
**Last verified:** 2026-10-03
**Rule (AGENTS.md §8):** any decision affecting the runtime asmdef, asset budget, rendering approach, or persistence model requires an ADR, written **before** implementation.

## Decision records

| ID | Title | Status in file | Actual implementation state | Date |
|---|---|---|---|---|
| [ADR-0001](adr-0001-touch-input-abstraction.md) | Touch input abstraction | **Proposed** | **Implemented** — `FluidTouchInputManager` uses EnhancedTouch, DPI-normalized thresholds, `Tapped` as a first-class event, zero legacy `UnityEngine.Input.*` | 2026-09-30 |

**KNOWN DOC DRIFT:** ADR-0001's `Status: **Proposed**` is stale — the decision is fully implemented and in use. It should read *Accepted*. Its 8 validation criteria (sub-frame tap, DPI independence at 160/320/640, zero-GC stroke, `Screen.dpi == 0` degradation, background-no-phantom, multi-touch safety, plus "no legacy Input usage") have **no tests**; criteria 1–6 are all testable and none are covered.

## Registry

`docs/registry/architecture.yaml` mirrors ADR-0001:
- interface `touch_input` → `status: proposed` (actually implemented)
- state ownership → "None yet"
- 5 forbidden patterns → all `status: proposed`
- header claims it is "populated **automatically** from `docs/architecture/adr-*.md`" — **no such automation exists**. CI runs only `Tools/verify-art-metrics.py`.

## Related decisions recorded outside ADRs

| Decision | Where recorded | Note |
|---|---|---|
| Unity 6000.3.11f1 pinned | `AGENTS.md` §4, `docs/engine-reference/unity/VERSION.md` | No ADR; version-risk analysis flags post-cutoff HIGH risk |
| Addressables deferred | `AGENTS.md` §4 note, `docs/framework/technical-preferences.md` (allowed-libraries table) | Justified: offline-first, no content catalogs, package weight |
| URP Render Graph only | `AGENTS.md` §4 callout | Compatibility Mode removed in 6.3 — enforced by `m_EnableRenderCompatibilityMode: 0` |
| One runtime assembly | `AGENTS.md` §5 | Currently true: only `Dab.Runtime` |
| Paint is mask/decal-based | `AGENTS.md` §5 | No per-frame RenderTexture writes |
| Performance budget = low-end Android | `AGENTS.md` §5 table | Supersedes the earlier mid-range assumption (README still says mid-range) |
| Scene = single bootstrap GameObject in `Awake` | Code comments in the bootstraps | No ADR — rationale: avoids hand-authoring `.meta` GUIDs |

## ADR backlog (decisions made without a record)

1. `PaintingRig` as the single wiring point (uncommitted).
2. Runtime-built uGUI with generated sprites + `LegacyRuntime.ttf` instead of TextMeshPro (uncommitted, `UxFactory` comments).
3. Domain reload disabled in Enter Play Mode options (`EditorSettings.asset`) — static caches survive play sessions.
4. Hand-authored 2-line `.meta` stubs for all 31 `.cs` files — **contradicts** AGENTS.md's "do not hand-author `.meta` files".
5. Editor namespace `Dab.EditorTools.*` alongside the documented `Dab.Editor.*`.
6. `Dab.Tests` assembly configured for all platforms while mixing EditMode and PlayMode tests.
