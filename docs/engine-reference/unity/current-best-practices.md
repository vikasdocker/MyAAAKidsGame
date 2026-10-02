# Current Best Practices — Unity 6000.3.11f1

**Last verified: 2026-09-30**

Practices that are current as of Unity 6.3 LTS. Where these differ from older
guidance, the difference is called out.

---

## Project Settings

### Active Input Handling

Player Settings → **Active Input Handling: Input System Package (New)**.

The legacy Input Manager is marked for deprecation in 6.3. Setting it as active
produces warnings. This project is new-Input-System-only.

### Scripting Backend

IL2CPP on both iOS and Android. iOS requires it regardless; using one backend
across platforms avoids Mono/IL2CPP behavioural differences surfacing late.

---

## Input — Enhanced Touch

Enable once at boot, not lazily:

```csharp
// In boot, before any gesture handling
EnhancedTouchSupport.Enable();
```

**Why at boot:** `EnhancedTouch` must record touches as they occur to maintain
per-touch history. Enabling it mid-gesture loses the gesture already in progress,
and `Touch.activeTouches` will be empty or partial.

**Poll, don't read raw state.** Reading `Touchscreen.current.touches` inside
`Update` misses touch state changes. Use `Touch.activeTouches` and
`Touch.activeFingers`.

**Track touches by `touchId`, not by array index.** Platform touch IDs may be
reused after a touch finishes; uniqueness is only guaranteed among *active*
touches.

### Interpreting Phase

`Touch.activeTouches` includes `Stationary` touches. A touch that is merely
resting on screen stays in the active list. For "is the finger moving" logic,
compare position against the previous frame — do not rely on phase alone to
detect movement.

---

## URP — Mobile Configuration

### Render Graph only

Compatibility Mode is removed in 6.3. If a Render Graph-related error appears,
verify nothing is still attempting to enable it.

### Bloom

Set Bloom `Filter` appropriately for tile-based mobile GPUs:

- **Dual** — faster alternative for mobile devices; the recommended default here
- **Kawase** — fastest, best at low resolution

On-tile post-processing exists in 6.3 for Adreno/Mali-class GPUs and covers
vignetting, tonemapping, color grading, and dithering at better performance and
lower battery cost. Relevant because the visual identity relies on bright,
high-key rendering.

### Shader Graph

URP Unlit supports custom lighting models in Shader Graph as of 6.2/6.3. Worth
evaluating for the rim-lit fur requirement — a custom lighting model may express
"always lit from behind and above" more cheaply than a stack of passes.

---

## Custom Passes

URP and HDRP share the Render Graph compiler as of 6.3. Custom render passes
written against the pre-6.x URP API must be ported. Do not copy render-pass code
from pre-6.x tutorials.

---

## What Has NOT Changed

Useful to know, because much online material is still accurate on these:

- Assembly definitions, namespaces, and `.asmdef` structure — unchanged
- URP asset configuration in Project Settings — unchanged
- ScriptableObject patterns — unchanged
- `MonoBehaviour` lifecycle methods — unchanged
- IL2CPP build pipeline — unchanged

---

## Verification Steps

Run these on first project open rather than assuming:

1. Package Manager → confirm URP and Input System versions against
   `docs/engine-reference/unity/VERSION.md`
2. URP Asset → confirm Render Graph path, mobile bloom filter
3. Player Settings → confirm Input System active, IL2CPP selected
4. Build Settings → confirm iOS and Android only
5. **Zero third-party SDKs installed** — required for kids-category compliance