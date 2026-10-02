# ADR-0001: Touch Input Abstraction — EnhancedTouch, DPI-Normalized Thresholds, First-Class Tap

## Status

Proposed

## Date

2026-09-30

## Engine Compatibility

| Field | Value |
|---|---|
| **Engine** | Unity 6000.3.11f1 (Unity 6.3 LTS) |
| **Domain** | Input |
| **Knowledge Risk** | HIGH — version shipped December 2025, after May 2025 LLM cutoff |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md`, `breaking-changes.md`, `deprecated-apis.md`, `current-best-practices.md` |
| **Post-Cutoff APIs Used** | `EnhancedTouchSupport.Enable()` + `Touch.activeTouches` (Input System 1.18+); `Touchscreen` avoided; `UnityEngine.Input` prohibited |
| **Verification Required** | Tap recognition under 60 ms contact; `Screen.dpi == 0` fallback; background/resume with finger held; 5-year-old tap-and-hold jitter behaviour |

### ⚠ Post-Cutoff Knowledge Warning

Unity 6000.3 shipped after the LLM knowledge cutoff. The following were verified
against the engine reference library rather than training data:

- **EnhancedTouch requires explicit `Enable()`.** It cannot be enabled lazily on
  first touch — the API must record touch history as touches occur, so enabling
  mid-gesture loses the in-progress gesture entirely.
- **`Touch.activeTouches` includes `Stationary` touches.** A finger merely resting
  on screen stays in the active list. Phase alone cannot detect movement.
- **Touch IDs are unique only among *active* touches.** Platforms may reuse IDs
  after a touch finishes. Array-index tracking is therefore incorrect.
- **Legacy `UnityEngine.Input` is deprecated in 6.3** and logs warnings when set
  as the active input handler. `UnityEngine.TouchPhase` and
  `UnityEngine.InputSystem.TouchPhase` are **distinct enums that coexist**.
- **`OnMouse*` MonoBehaviour events are unsupported on 6000.3** with the Input
  System package (added in 6000.4+). `IPointer*` interfaces are the route.

---

## ADR Dependencies

| Field | Value |
|---|---|
| **Depends On** | None — this is the first ADR |
| **Enables** | ADR-0002 (painting system — consumes stroke events), Paint system GDD, Minigame system GDD |
| **Blocks** | Epic: Input & Interaction. Any story implementing petting, painting input, or minigame gesture handling |
| **Ordering Note** | Must be Accepted before `FluidTouchInputManager` is consumed by any other system. The painting render pass (ADR-0002) depends on the stroke events defined here. |

---

## Context

### Problem Statement

This game's target player is 6 to 9 years old. Children in this band do not have
reliable fine motor control, do not read fluently, and abandon gestures midway when
feedback stops arriving. A conventional input layer — one built around precise,
completing gestures and pixel-space thresholds tuned on the developer's machine —
would fail this audience in three specific ways:

1. **Drag-to-interact fails the youngest players.** A 5-year-old lacks both the
   motor control and the object persistence to complete a drag. They lift
   mid-gesture and tap instead.
2. **Pixel-tuned thresholds fail across the device range.** DPI varies by an
   order of magnitude between a flagship phone and a cheap tablet. A pixel
   dead-zone that forgives enough on the former does nothing on the latter — and
   the audience skews toward the cheap end.
3. **A tap treated as a degenerate drag is a second-class path.** Design
   constraint AGENTS.md §2.4 requires tap and drag to be *equal-status* routes to
   the same outcome. That cannot be true if a tap is structurally "a drag that
   didn't finish."

### Constraints

**Technical**
- Unity 6000.3.11f1, URP, IL2CPP, iOS + Android only
- New Input System mandatory; legacy `UnityEngine.Input` deprecated in 6.3
- 60 fps target, mid-range Android is the floor
- No allocation per frame on the input path (would create GC spikes mid-gesture —
  exactly when visual feedback matters most)

**Design**
- Nothing in the UI may require reading (AGENTS.md §2.5)
- Every drag gesture must have an equal-status tap fallback (§2.4)
- No precision may be *required* (§2.2, Pillar 2 — Nothing Can Go Wrong)
- Must produce continuous stroke data for the painting system (GDD §3, 30-second loop)

**Timeline**
- Months-scale, first-time developer, single developer
- Cannot afford a bespoke gesture-recognition framework

### Requirements

1. Report continuous stroke position for the freehand painting path
2. Recognise a tap with no precision requirement, as a first-class outcome
3. Tolerate device DPI variance across the full target range
4. Not allocate per frame
5. Survive app backgrounding without phantom or dropped touches
6. Report intents, not game meaning — consumers interpret

---

## Decision

A single MonoBehaviour, `FluidTouchInputManager`, owns all touch interpretation for
the project. Three decisions follow from this.

### Decision 1 — EnhancedTouch always-on, never raw `Touchscreen` polling

`EnhancedTouchSupport.Enable()` is called in `OnEnable` and retained for the app
session. All reading goes through `Touch.activeTouches` and `Touch.activeFingers`.

**`EnhancedTouchSupport.Disable()` is never called** — not on disable, not on
scene unload, not on pause. Tracked touches are cancelled instead.

**Rationale.** EnhancedTouch retains full per-touch history for touches
shorter-lived than a single input update. A child's quick tap is frequently
shorter than one frame, and the legacy `Touchscreen` path can overwrite it with a
new touch in that same update. For an audience whose taps must *always* register,
that guarantee is not optional. The API's own guidance is to enable at boot; a
persistent manager that never disables is the straightforward way to comply.

**Rejected alternative — event-driven `Finger.onFingerDown` callbacks.** Equally
idiomatic in 6.3 and avoids per-frame list scanning. Polling was chosen for
determinism and debuggability: with ≤3 tracked touches the scan cost is
unmeasurable at 60 fps, and a single polling loop makes phase ordering explicit.
**This is a revisitable decision** — revisit if profiling shows input cost matters,
or if multi-touch gestures beyond 3 contacts are later required.

### Decision 2 — All thresholds normalized to inches via `Screen.dpi`

Drag dead zone, sample spacing, and screen-edge inset are authored in **inches** and
converted to pixels at use.

| Threshold | Default | Purpose |
|---|---|---|
| `_dragDeadZoneInches` | 0.12" | Below this, movement is "holding", not dragging |
| `_minSampleSpacingInches` | 0.03" | Minimum distance between emitted samples |
| `_edgeInsetInches` | 0.05" | Ignore touches beginning near screen edges |

**Rationale.** A 0.12" movement is meaningful precision to an adult and noise to a
child. Expressing it in device-independent units means one tuning session
transfers across every device. `Screen.dpi` is clamped to a floor of 1 to prevent
division by zero on devices or emulators that report 0.

**Rejected alternative — pixel thresholds with a quality setting.** Rejected
because it makes forgiveness a device-tier decision, which means the least
forgiving configuration ships to exactly the players who can least tolerate it.

### Decision 3 — Tap is a distinct first-class event, not a short-drag case

Four separate C# events. `Tapped` fires **in addition to** `TouchEnded`, not as a
subcase of it.

```csharp
event Action<Vector2>       TouchBegan;
event Action<Vector2>       TouchMoved;
event Action<Vector2, bool> TouchEnded;      // (position, wasTap)
event Action<Vector2>       Tapped;          // tap only — no stroke bookkeeping
event Action<List<Vector2>> DragCompleted;   // drag only
```

**Rationale.** Consumers that only need "did the child choose this" — UI buttons,
palette selections — subscribe to `Tapped` and never touch stroke lists. Consumers
doing continuous work — painting, petting — subscribe to `TouchMoved` and
`DragCompleted`. Neither path is aware the other exists.

Had tap been modelled as "a drag that stayed inside the dead zone," every consumer
would have had to re-derive that distinction, and the temptation would be to
deprioritise the tap branch as an edge case. Separate events make the equal-status
guarantee structural rather than documentary.

### Additional Behaviours

**Ended-before-live ordering.** Each frame processes `Ended`/`Canceled` touches
before `Began`/`Moved` touches. A child who lifts and re-presses within one frame —
common at this age — cannot collide touch IDs.

**First-finger stroke ownership.** The first finger of a new interaction owns the
stroke. A second finger is tracked but does not steal it. This prevents a
sibling's incidental touch from hijacking an in-progress painting stroke.

**Grace period.** A configurable window suppresses `Tapped` immediately after boot
so an accidental touch during the greeting sequence is not read as a command.
`TouchBegan` is deliberately *not* suppressed — blocking it would make the creature
feel unresponsive during the greeting, which is worse.

**CancelActiveTouches().** Public. Releases all active touches and clears stroke
state. Called on `OnDisable`, on `OnApplicationPause(true)`, and intended for panel
opens and scene transitions, so a finger held across a boundary cannot produce a
phantom stroke against new content.

**TouchPhase alias.** `using TouchPhase = UnityEngine.InputSystem.TouchPhase;` is
declared explicitly. `UnityEngine.TouchPhase` (legacy) and
`UnityEngine.InputSystem.TouchPhase` coexist in 6.3; an unqualified reference is
ambiguous and easy to bind to the wrong enum in a new-Input-System-only project.

### Architecture Diagram

```
        Finger on glass
                │
                ▼
   ┌────────────────────────┐
   │  InputSystem (package) │   EnhancedTouchSupport.Enable() at OnEnable
   │  records touch history │   NEVER disabled — see Decision 1
   └───────────┬────────────┘
               │  Touch.activeTouches  (polled once per frame in Update)
               ▼
   ┌────────────────────────┐
   │ FluidTouchInputManager │
   │                        │
   │  1. Ended/Canceled     │  ← processed FIRST (ID-collision safety)
   │  2. Began/Moved/Stat.  │
   │                        │
   │  Filter: edge inset    │
   │  Filter: max 3 touches │
   │  Filter: dead zone     │  ← inches via Screen.dpi
   │  Filter: sample spacing│  ← inches via Screen.dpi
   └───────────┬────────────┘
               │
     ┌─────────┴─────────┬──────────────┬───────────────┐
     ▼                   ▼              ▼               ▼
 TouchBegan          TouchMoved       Tapped      DragCompleted
     │                   │              │               │
     ▼                   ▼              │               │
 Pet reaction      Paint system       │               │
 (30s loop)      (ADR-0002)           │               │
                    Minigames        │               │
                                        │               │
                              UI buttons, palette   │
                              (no stroke lists)      │
```

### Key Interfaces

```csharp
namespace Dab.Runtime.Input
{
    public sealed class FluidTouchInputManager : MonoBehaviour
    {
        // Intents only — consumers assign game meaning.
        event Action<Vector2>       TouchBegan;
        event Action<Vector2>       TouchMoved;
        event Action<Vector2, bool> TouchEnded;     // (position, wasTap)
        event Action<Vector2>       Tapped;
        event Action<List<Vector2>> DragCompleted;

        bool                 IsTouching { get; }
        int                  ActiveTouchCount { get; }
        bool                 IsDragging { get; }
        Vector2              PrimaryPosition { get; }
        IReadOnlyList<Vector2> CurrentStroke { get; }   // live, do not retain

        void CancelActiveTouches();
        void ClearGracePeriod();
    }
}
```

**Consumption rule:** `CurrentStroke` returns the live internal list. Read it
inside a stroke callback; never retain the reference. `DragCompleted` passes a
copy, so it is safe to hold.

---

## Alternatives Considered

### Alternative A: Input Actions with Interaction Assets

- **Description**: Declare an `InputAction` per gesture and drive behaviour from
  Interaction assets (tap, drag, hold) configured in the editor.
- **Pros**: Designer-editable without code; the system's intended path; strong
  tooling and rebinding support.
- **Cons**: Interaction assets encode thresholds in **pixels**, making the DPI
  problem unsolved. Tap-vs-drag collapses into a single action's internal
  interaction state, so the equal-status guarantee (§2.4) must be re-derived by
  every consumer. Tuning requires opening the editor.
- **Rejection Reason**: Cannot express inch-normalized forgiveness, which is the
  central requirement. The DPI problem is not solvable within this API.
  Actions remain appropriate for *gameplay button bindings* if a gamepad or
  accessibility remapping feature is ever added, and are not thereby rejected as
  a technology.

### Alternative B: Raw `Touchscreen` polling, custom history tracking

- **Description**: Read `Touchscreen.current.touches` directly and maintain touch
  IDs manually.
- **Pros**: Lowest overhead; no enable/disable lifecycle to manage.
- **Cons**: Unity's own documentation states reading raw touch state inside
  `Update` **misses state changes**. Manual ID tracking must reimplement
  liveness handling that EnhancedTouch provides. Sub-frame taps — the most common
  case for a 6-year-old — are exactly what gets dropped.
- **Rejection Reason**: Unreliable for short contacts. The failure mode is silent
  and age-specific, which makes it the worst possible defect here.

### Alternative C: Separate `TapDetector` and `StrokeDetector` components

- **Description**: Two components, each polling touches, each deciding intent.
- **Pros**: Clean single responsibility.
- **Cons**: Both would poll and both would compete for primary-touch ownership.
  Coordination between them is more complex than the problem warrants, and the
  equal-status guarantee would depend on arbitration between two independent
  components rather than being structural.
- **Rejection Reason**: Complexity without benefit at this scope. Worth revisiting
  if painting and UI ever need genuinely independent consumption of the same touch.

### Alternative D: Third-party gesture recognition library

- **Pros**: Mature, feature-rich.
- **Cons**: Adds a dependency and a supply-chain surface to a **kids-directed**
  product, where every third-party SDK requires compliance review. Most such
  libraries target adult touch precision and would need retuning to this extent.
- **Rejection Reason**: Violates the zero-third-party-SDK constraint and buys
  capability this game does not need. YAGNI against a hard compliance constraint.

---

## Consequences

### Positive

- Forgiving behaviour is identical across every device in the target range,
  verified by construction rather than by testing each device
- Tap and drag are structurally equal; a consumer cannot accidentally implement
  one as a degraded path
- No per-frame allocation on the input path — lists are pre-sized to 64
- Consumers depend on one component, not on Input System internals, so an
  engine-version migration touches one file
- Sub-frame taps always register, which matters disproportionately for this age
  band
- Backgrounding with a finger held produces no phantom stroke

### Negative

- Polling cost is paid every frame even when idle (~negligible at ≤3 touches, but
  non-zero and not event-driven)
- `EnhancedTouchSupport` is never disabled, so it persists for the app session
  including scenes that need no touch input at all
- Touch interpretation is coupled into one MonoBehaviour; a stateful consumer
  mistake (retaining `CurrentStroke`) causes subtle bugs
- Inch normalization depends on `Screen.dpi`, which some low-end Android devices
  and all emulators report inaccurately
- The polling-vs-event decision is baked in and would need revisiting if multi-touch
  beyond 3 contacts or a gamepad is ever required

### Risks

| Risk | Severity | Mitigation |
|---|---|---|
| `Screen.dpi` misreported on low-end Android or emulators | MEDIUM | Clamped to 1 to prevent divide-by-zero; thresholds authored generously (0.12" / 0.03") so a modest DPI misreport is absorbed rather than fatal. **Verify on physical low-end hardware.** |
| Finger held during app switch resumes into a stroke | MEDIUM | `OnApplicationPause` cancels tracked touches; `CancelActiveTouches()` is public for panel opens and scene loads |
| 5-year-old tap-and-hold jitter misread as a drag | MEDIUM | Dead zone is 0.12" (3 mm) and drag state requires *exceeding* it; a genuine 6-year-old tremor is smaller. **Verify with real 5-year-olds — this is a genuine unknown** |
| Per-frame polling shows up in profiling | LOW | ≤3 touches, no allocation, `ReadOnlyArray` iteration. Revisit to `Finger` callbacks if it appears |
| Legacy `UnityEngine.TouchPhase` bound by mistake | LOW | Explicit alias in the file; compile error rather than silent wrong-enum behaviour |
| Consumers treat `CurrentStroke` as a stable snapshot | LOW | Documented as live; `DragCompleted` passes a copy |
| EnhancedTouch left enabled in scenes that need no input | LOW | Small fixed cost; correctness benefit outweighs it |

---

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|---|---|---|
| `game-concept.md` §6 Audience Reality Check | "Every feeding/petting mechanic needs a tap-path fallback that is equally first-class, not a degraded mode" | `Tapped` is a distinct event, not a drag subcase. Consumers cannot implement one as a degraded path (Decision 3) |
| `game-concept.md` §6 | "Reaction expressiveness is maximal. A 5-year-old reads large and obvious reactions as responsive; subtlety reads as broken" | 0.12" dead zone ensures a resting or tremor-affected finger still registers as a deliberate touch (Decision 2) |
| `game-concept.md` §3 30-Second Loop | "Touch pet (continuous stroke → it leans into your finger)" | `TouchMoved` + `DragCompleted` supply continuous stroke data at 0.03" sample spacing |
| `game-concept.md` §3 30-Second Loop | "No loading, no menu, no confirm" | Single component, zero allocation, no gesture-completion requirement — an abandoned drag still yields a partial stroke |
| `game-concept.md` §5 Competence | "Difficulty plateaus rather than climbs" | Fixed forgiving thresholds mean difficulty cannot vary by player skill |
| `game-concept.md` §8 Flow State | "No dialog boxes during the paint interaction" | Input reports intent only; all modal decisions are consumer-side and can be suppressed |
| `AGENTS.md` §2.4 | "Every drag gesture has an equal-status tap fallback" | Structural, enforced by event separation rather than convention |
| `AGENTS.md` §2.5 | "Nothing in the UI may require reading" | Consumer-side, but input reports position not intent, so icon-only UI is unconstrained |

---

## Performance Implications

- **CPU**: One `Update` iteration over ≤3 touches per frame. No allocation, no
  boxing, no LINQ. Measured cost is negligible; **profile to confirm**.
- **Memory**: Two `List<Vector2>` pre-sized to 64 (≈1 KB total), one
  `int[3]`, one `List<Vector2>` allocated per completed drag (the copy handed to
  `DragCompleted`). Long-tail GC pressure only on drag completion, which is
  user-paced and infrequent.
- **Load Time**: Negligible. Input System package is already required.
- **Network**: None. No network calls on the input path, per AGENTS.md.

---

## Migration Plan

Not applicable — greenfield. `FluidTouchInputManager` is the first runtime code in
the project and has no consumers.

---

## Validation Criteria

This ADR is validated when all of the following hold:

1. **Sub-frame tap registers.** A contact shorter than one frame produces exactly
   one `Tapped`. Testable via Input System Test Fixtures.
2. **DPI independence.** Identical threshold behaviour at `Screen.dpi` values of
   160, 320, and 640. Automated via a screen-parameter override.
3. **No allocation per frame.** Zero GC bytes during a continuous stroke across
   600 frames. Measured via `ProfilerRecorder` on GC Allocated In Frame.
4. **`Screen.dpi == 0` does not throw and degrades to a 1-pixel floor.** Unit test.
5. **Background with finger held produces no phantom stroke.** `OnApplicationPause`
   then resume — no `DragCompleted` fires.
6. **Multi-touch safety.** Second finger does not steal the primary stroke. Third
   finger is rejected without corrupting tracked state.
7. **Real-child playtest.** A 5-year-old and a 9-year-old each complete the
   petting and painting interactions without adult coaching. **This is the only
   criterion that cannot be automated**, and it is the one that determines whether
   the thresholds are right.
8. **No legacy Input usage.** Zero references to `UnityEngine.Input` in project
   code.

---

## Related Decisions

- `design/gdd/game-concept.md` — source of truth; §§3, 5, 6, 8 traced above
- `AGENTS.md` — §2 non-negotiable constraints, §5 performance budgets
- `docs/engine-reference/unity/deprecated-apis.md` — Input migration table
- **ADR-0002 (pending)** — freehand painting render pass; consumes the stroke
  events defined here