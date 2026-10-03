# Tech Stack

**Doc type:** project knowledge (indexed into Dify)
**Last verified:** 2026-10-03 (from `ProjectSettings/`, `Packages/manifest.json`)
**Engine reference:** `docs/engine-reference/unity/VERSION.md`, `breaking-changes.md`, `deprecated-apis.md`, `current-best-practices.md`

## Engine

| Item | Value |
|---|---|
| Editor | **Unity 6000.3.11f1** (Unity 6.3 LTS), revision `3000ef702840` |
| Installed at | `C:\Program Files\Unity\Hub\Editor\6000.3.11f1\Editor\Unity.exe` |
| Hub CLI | `C:\Users\vikas\AppData\Local\Unity\bin\unity.exe` (not on `PATH`) |
| Language | C# |
| Scripting backend | IL2CPP (Android `scriptingBackend: 1`) |
| Color space | Linear |
| Min Android SDK | 25, target arch ARM64 (`AndroidTargetArchitectures: 2`) |
| iOS min | 15.0 |
| Graphics APIs | Android: Vulkan + GLES3 (manual); iOS: automatic |
| Active input handling | `1` = **Input System (New) only** ✓ |
| Orientation | `4` = AutoRotation, all 4 autorotate flags on → **NOT portrait-locked** |

**KNOWN DOC DRIFT:** design says "portrait-first, one-thumb", but `defaultScreenOrientation: 4` allows landscape. Not yet reconciled.

**KNOWN DOC DRIFT:** all three `applicationIdentifier` values are still the URP template's `com.UnityTechnologies.com.unity.template.urpblank` / `com.Unity-Technologies.com.unity.template.urp-blank`. `companyName` is `DefaultCompany`. Must be changed before any store build.

## Packages (`Packages/manifest.json`, 46 dependencies)

| Package | Version | Note |
|---|---|---|
| `com.unity.render-pipelines.universal` | 17.3.0 | **URP only** — no HDRP anywhere |
| `com.unity.inputsystem` | 1.19.0 | New Input System — the only input path |
| `com.unity.test-framework` | 1.6.0 | NUnit |
| `com.unity.ugui` | 2.0.0 | uGUI (brings `Unity.TextMeshPro`) |
| `com.unity.ai.navigation` | 2.0.11 | installed but **zero code uses NavMesh/NavAgent** |
| `com.unity.timeline` | 1.8.11 | installed, unused |
| `com.unity.visualscripting` | 1.9.10 | installed, unused |
| `com.unity.collab-proxy`, `com.unity.multiplayer.center` | 2.11.4 / 1.0.1 | template defaults, unused |
| **`com.unity.addressables`** | **NOT installed** | deliberately deferred — offline-first game, no content catalogs, meaningful package weight for mobile |
| analytics / ad SDKs | **none** ✓ | kids-category constraint respected |

## Render pipeline configuration

- Active RP: `Assets/Settings/PC_RPAsset.asset` (GraphicsSettings) — quality levels `Mobile`→`Mobile_RPAsset`, `PC`→`PC_RPAsset`.
- `UniversalRenderPipelineGlobalSettings.asset`: `m_EnableRenderCompatibilityMode: 0` ✓ (Render Graph only; Compatibility Mode is removed in 6.3).
- Volume profiles: `DefaultVolumeProfile`, `SampleSceneProfile`.
- **URP Compatibility Mode is forbidden.** Any tutorial/AI snippet that enables `RenderGraphSettings.enableRenderCompatibilityMode` is wrong for this project.

## Editor / project settings

| Setting | Value | Note |
|---|---|---|
| Serialization | Force Text (`m_SerializationMode: 2`) | required for readable YAML diffs |
| Enter Play Mode Options | enabled, options `0` | **domain reload disabled** — static state persists |
| Custom tags / layers | none | only built-in |
| `ForceInternetPermission` | 0 ✓ | offline-first |

## Build & verification commands

```powershell
$cli = "C:\Users\vikas\AppData\Local\Unity\bin\unity.exe"
$ed  = "C:\Program Files\Unity\Hub\Editor\6000.3.11f1\Editor\Unity.exe"

# Import + compile check
& $cli run "C:\Users\vikas\OneDrive\Desktop\MyAAAKidsGame"

# Rebuild the playtest scene
& $ed -batchmode -nographics -quit -projectPath <repo> `
    -executeMethod Dab.EditorTools.Playtest.PlaytestSceneSetup.Rebuild -logFile <log>

# PlayMode tests  (never pass -quit with -runTests)
& $ed -runTests -batchmode -projectPath <repo> `
    -testPlatform PlayMode -testResults <results.xml> -logFile <log>
```

Do **not** pass `-quit` to a script that enters play mode — the editor exits before play mode starts and reports success regardless.

## CI

`.github/workflows/verify-pipeline.yml`: checkout → setup-python 3.12 → `python Tools/verify-art-metrics.py` (with `set -o pipefail`) → publish summary → upload artifact on failure. Runs on `push` + `workflow_dispatch`, `contents: read`, concurrency-cancels superseded runs.

**What CI gates:** numeric claims in `design/Art/palette.json` + `typography.json` (WCAG contrast, colourblind luma deltas, saturation budgets). Born from 8 real defects found 2026-10-02.

**What CI does NOT gate:** Unity compilation, EditMode/PlayMode tests, shader compilation, scene/prefab reference validation, lint, build. **The CI never touches a line of C#.**

## Local tooling

- `Tools/verify-art-metrics.py` — 1,249-line stdlib-only Python gate.
- Python 3.14 is available locally (`python`, `py`, `python3`).
- Unity project has `Library/` present (~29k files) — Unity cache, gitignored.
