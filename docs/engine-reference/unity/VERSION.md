# Unity 6000.3.11f1 (Unity 6.3 LTS) — Version Reference

| Field | Value |
|---|---|
| **Engine** | Unity |
| **Engine Version** | 6000.3.11f1 (Unity 6.3 LTS) |
| **Install Path** | `C:\Program Files\Unity\Hub\Editor\6000.3.11f1` |
| **Project Pinned** | 2026-09-30 |
| **LLM Knowledge Cutoff** | May 2025 |
| **Likely LLM Coverage** | Unity through ~2023.x / early 6000.x |
| **Risk Level** | **HIGH** — version shipped after training cutoff |
| **Support Window** | To December 2027 |
| **Last Docs Verified** | 2026-09-30 |

---

## ⚠ Why This Version Is High Risk

Unity 6.3 LTS released **December 2025**, roughly seven months after the May 2025
LLM knowledge cutoff. Any Unity guidance produced from model memory predates this
release and may reference APIs that are now removed.

**Practical consequence:** most tutorials, courses, and AI-generated Unity snippets
targeting URP are written for pre-6.x versions and assume **URP Compatibility
Mode**. That mode no longer exists here. See `breaking-changes.md`.

---

## Critical Changes Affecting This Project

### 1. URP Compatibility Mode — REMOVED (highest impact)

Deprecated in Unity 6.0, **removed in 6.3**.

- `RenderGraphSettings.enableRenderCompatibilityMode` is now **read-only** and
  always returns `false`.
- The `URP_COMPATIBILITY_MODE` scripting define still works **only to migrate** an
  existing project. It will stop working in Unity 6.4 and shipping with it is
  not supported.

**This project must use URP Render Graph.** Anything enabling Compatibility Mode
is wrong here.

### 2. Shared Render Graph across URP and HDRP

URP and HDRP now compile through the same Render Graph backend. Custom render
features and passes written for legacy URP need review against the 6.x Render
Graph API — this is relevant to the planned custom paint and fur shaders.

### 3. Legacy Input Manager — deprecated, not removed

The old `UnityEngine.Input` API is **marked for deprecation** in 6.3 and logs
warnings when it is the active input handler. It still functions during the 6.3
cycle but will be removed in a future version.

**This project uses the new Input System exclusively.**

### 4. URP mobile bloom filtering

URP Bloom gained **Kawase** and **Dual** filter options, including on-tile
post-processing for tile-based mobile GPUs (Adreno, Mali). Set Bloom `Filter` to
**Dual** for mobile, or Kawase at low resolution — materially cheaper than the
default on the target device class.

### 5. Custom lighting via Shader Graph

URP Unlit type now supports authoring custom lighting models in Shader Graph.
Potentially useful for the non-photorealistic "Candy Sunrise" fur/rim-light look.

---

## Package Versions to Verify

Do not assume these — check Package Manager on first project open.

| Package | Expected | Notes |
|---|---|---|
| `com.unity.render-pipelines.universal` | 17.x | Ships with 6.3 |
| `com.unity.inputsystem` | 1.18+ | Enhanced Touch API; 1.18 is Jan 2026 |
| `com.unity.ugui` | 2.x | Ships with 6.x |

Input System 1.18 notes relevant here:
- Project-wide Input Actions can no longer be compiled out (removed the
  `UNITY_INPUT_SYSTEM_PROJECT_WIDE_ACTIONS` define).
- Code supporting Unity versions older than 2022.3 LTS was removed.
- `OnMouse` MonoBehaviour events gain support on Unity 6000.4+ (**not** 6.3).

---

## Version Decision Rationale

Candidates considered:

| Version | Status | Decision |
|---|---|---|
| 6000.3.12f1 LTS | Latest 6.3 patch, shipped Mar 2026 | Not installed — would require download |
| **6000.3.11f1 LTS** | **Installed** | **PINNED** — same 6.3 LTS feature set, start immediately |
| 6000.4.10f1 | Installed, newer editor | Not LTS — wrong support track for production |
| 2022.3.20f1 LTS | Installed, legacy | Predates Render Graph transition entirely |

6000.3.11f1 was chosen because it is already installed, is on the current LTS
feature set, and is supported until December 2027. Patch-level differences within
6.3 are bug fixes, not features — the 12f1 upgrade can happen later without
architectural impact.

---

## Project-Specific Constraints

- **URP Render Graph only.** No Compatibility Mode.
- **New Input System only.** Legacy `UnityEngine.Input` is off-limits.
- **IL2CPP** on both iOS and Android (iOS requires it).
- **No third-party analytics or ad SDKs** — kids-category store compliance.
- Offline-first. No network calls on the painting or input paths.

---

## Reference Files

| File | Contents |
|---|---|
| `breaking-changes.md` | Version-by-version breaking changes 6.0 → 6.3 |
| `deprecated-apis.md` | "Don't use X → Use Y" tables |
| `current-best-practices.md` | Practices current as of 6000.3.11f1 |