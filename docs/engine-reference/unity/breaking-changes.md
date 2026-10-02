# Breaking Changes — Unity 6000.0 → 6000.3

**Last verified: 2026-09-30** against Unity 6.3 LTS documentation.

Only changes with likely impact on this project are listed. This is not a
complete changelog.

---

## Unity 6.3 (6000.3)

### URP Compatibility Mode removed — CRITICAL

Deprecated in 6.0, removed in 6.3.

- `RenderGraphSettings.enableRenderCompatibilityMode` is **read-only**, always `false`
- Compatibility code is stripped by default to reduce compile time and build size
- The `URP_COMPATIBILITY_MODE` Player scripting define can re-add the mode **only
  to convert a project**; it **will not work in Unity 6.4**, and shipping with it
  is not supported

**Impact:** any tutorial, course, forum answer, or AI-generated snippet that
enables Compatibility Mode is invalid here. Custom render passes written for
pre-6.x URP must be ported to the Render Graph API.

**Action:** confirm the project uses URP Render Graph after the first import.

### URP Bloom mobile filtering

- New `Filter` options: **Kawase** (fastest, low resolution) and **Dual** (faster
  alternative for mobile), alongside default
- New **on-tile** post-processing renderer feature for tile-based GPUs
  (Adreno / Mali) — better performance and lower battery cost
- Vignetting, tonemapping, color grading, and film grain supported on on-tile path

**Impact:** directly relevant. Bloom is a core part of the bright "Candy Sunrise"
look, and this project targets mid-range Android.

**Action:** set Bloom `Filter` to Dual for mobile builds; profile on-tile effects
against the `AGENTS.md` performance budget.

---

## Unity 6.2 → 6.3

- Shared Render Graph compiler and API across URP and HDRP, laying groundwork for
  unified extensibility
- URP Unlit type now supports authoring custom lighting models in Shader Graph
- VFX Graph: new samples and templates; GPU event instancing support improved

**Impact:** relevant to the planned custom fur/paint shaders. Shader Graph custom
lighting is worth evaluating against the Candy Sunrise rim-light requirement.

---

## Unity 6.0 (6000.0)

### URP Compatibility Mode deprecated

- Compatibility Mode deprecated in favour of URP Render Graph

### Legacy Input path

- The old Input Manager is marked for deprecation; setting it as the active
  handler logs warnings in 6.x projects

**Impact:** this project uses the new Input System only, so this is a constraint
to enforce rather than a migration.

### Shader Graph / URP custom lighting foundations

- Initial custom lighting model authoring in URP Shader Graph

---

## Migration Checklist for This Project

- [x] Engine pinned to 6000.3.11f1 LTS
- [ ] Open project in Hub; verify URP import and Render Graph active
- [ ] Player Settings → Active Input Handling = **Input System Package (New)**
- [ ] Configure URP Asset for mobile: Bloom filter Dual, appropriate shadow
      distance, reduced resolution for post-processing
- [ ] Never enable Compatibility Mode
- [ ] Confirm no `UnityEngine.Input` usage in any script
- [ ] Verify installed URP and Input System package versions against `VERSION.md`