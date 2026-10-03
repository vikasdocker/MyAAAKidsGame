# Systems — per-system status index

**Doc type:** project knowledge (indexed into Dify)
**Last verified against code:** 2026-10-03
**Status values:** `COMPLETE` (works + wired + tested) · `PARTIAL` (exists but incomplete/unwired) · `MISSING` (documented, no code) · `BROKEN` (defective)

Narrative detail lives in `docs/project/CURRENT_STATE.md`. This file is the quick lookup table with file paths.

| System | Status | Files | Notes |
|---|---|---|---|
| Core / bootstrap | **COMPLETE** (prototype) | `Runtime/Core/PaintingRig.cs` (151, untracked), `Runtime/Playtest/CreaturePlaytestBootstrap.cs`, `Runtime/Minigames/PlaygroundBootstrap.cs`, `Runtime/UI/MainMenuBootstrap.cs` | `PaintingRig.Build()` is the single wiring point. Every scene = one GameObject + `Awake()` world build |
| Input | **COMPLETE** | `Runtime/Input/FluidTouchInputManager.cs` (714) | EnhancedTouch + DPI-normalized thresholds; `Tapped` first-class vs `DragCompleted`; 0.5 s boot grace; playable-area inset; 3 touch slots. Working tree adds a mouse-parity layer (`HandleMousePress/Hold/Release`, `VirtualMouseTouchId`). Zero legacy `UnityEngine.Input.*`. `InputSystem_Actions.inputactions` unreferenced |
| Painting | **COMPLETE** | `Runtime/Painting/CreaturePaintController.cs` (462), `Runtime/Painting/CreatureTestMeshGenerator.cs` (1341) | Screen→UV raycast, dab counting, mask/decal-based (no per-frame RenderTexture) |
| Creature state machine | **COMPLETE** | `Runtime/Creature/*` (9 files, 2,065 l): `CreatureStateMachine` (658), `CreatureState` (242), `CreatureStateId` (95), `Idle/Painting/Petting/Feeding` states, `AbilityUnlockState` (163) | 5 states + `None`; guarded transitions; zero-allocation path asserted by tests. `Feeding` only via explicit call |
| FX / particles | **COMPLETE** | `Runtime/FX/CreatureFXSpawner.cs` (788), `Assets/Art/FX/Resources/FX/*.png` ×3 | Fixed pool, round-robin, zero-allocation contract; ink / heart / 12-spoke radial bursts |
| Editor tooling | **COMPLETE** | `Editor/CreatureAnimatorControllerGenerator.cs` (288), `CreatureFXAssetGenerator.cs` (267), `DabArtGeneration.cs` (21), `Editor/Playtest/PlaytestSceneSetup.cs` (99), `Editor/Gameplay/GameplayScenesSetup.cs` (133, untracked) | 7 menu items + `-executeMethod` entry points; reproducible generation |
| Tests | **COMPLETE** (with gaps) | `Tests/` — `CreatureStateMachineTests` (9), `CreatureStateAllocationTests` (3), `VisualBridgeTests` (13), `GeneratedVisualAssetsTests` (9), `PlaytestRigTests` (3), `GameplayLoopTests` (6, untracked) = **43** | Not executed by any automation |
| Animation / Animator | **PARTIAL — built, not wired** | `Runtime/Creature/CreatureAnimationBridge.cs` (539), `Assets/Art/Animation/CreatureAnimatorController.controller`, 7 `zz_placeholder_*.anim`, `Editor/CreatureAnimatorControllerGenerator.cs` | `PaintingRig.Build()` adds no `Animator`/no bridge; no scene or prefab references the controller. ~1,100 lines reachable only from tests |
| Rendering / shaders | **PARTIAL** | `Assets/Shaders/Paint/CreatureCanvas.shader` (531, modified), `PaintStamp.shader` (95) | Only `Paint/` populated; `Fur/`, `Lit/`, `Unlit/`, `UI/`, `PostProcessing/` empty. No Shader Graph. Working tree fixes vertex `SAMPLE_TEXTURE2D`→`_LOD` and ShadowCaster CBUFFER |
| UI | **PARTIAL** (untracked) | `Runtime/UI/MainMenuBootstrap.cs` (210), `UxFactory.cs` (267), `Palette.cs` (58) | Runtime-built uGUI, icon-over-caption, `LegacyRuntime.ttf`, 1080×1920. No parent gate / settings / HUD / localization. **PAINT button broken** |
| Minigames | **PARTIAL** (untracked) | `Runtime/Minigames/PlaygroundBootstrap.cs` (90), `PlaygroundPetMotor.cs` (319) | A playground, not a minigame. Tap ground → run → flourish; deterministic golden-angle wander, zero `Random` |
| Camera | **PARTIAL** | inline in the 3 bootstraps | Static framing only; no follow/ortho/Cinemachine/portrait re-framing. Playtest background is cold grey-blue |
| Config / settings | **PARTIAL** | `Runtime/UI/Palette.cs` (58), `Assets/Settings/*.asset` | `Palette` is a hand copy of `palette.json`, unvalidated at runtime. No `ScriptableObject` configs |
| Abilities / Echo binding | **MISSING** | `Runtime/Abilities/` (empty) | No `SignatureMark`; `AbilityUnlockState.AbilityId` is an unbacked `int`. The game's central loop has no data model |
| Save / Load | **MISSING** | `Runtime/Save/` (empty) | Zero persistence. Stated as the most severe possible failure |
| Audio | **MISSING** | `Runtime/Audio/` (empty); `Assets/Audio/` does not exist | Zero `AudioSource`; art bible requires audio as a state-redundancy channel |
| Localization | **MISSING** | `Localization/` (empty root dir) | Zero code hits |
| AI / NPC | **MISSING** | — | `com.unity.ai.navigation` installed, unused |
| Inventory / quests / progression | **MISSING** | — | Nothing |
| Parent gate / settings screen | **MISSING** | — | Nothing |
| Scene naming / build settings | **BROKEN** | `MainMenuBootstrap.cs:188` | Loads `SCN_Playtest`, which does not exist. Actual scenes: `Playtest`, `SCN_MainMenu`, `SCN_Playground` |
| CI | **BROKEN** (as a gate) | `.github/workflows/verify-pipeline.yml`, `Tools/verify-art-metrics.py` | Gates art math only; never compiles Unity or runs tests |

## Empty folders worth knowing about

`Assets/Scripts/Runtime/{Abilities,Audio,Pet,Procedural,Save}/`, `Assets/Shaders/{Fur,Lit,PostProcessing,UI,Unlit}/`, `Assets/Art/{Characters,Environment,Materials,UI,VFX}/`, `Assets/Prefabs/**`, `Assets/{Animation,Materials,Textures,TextureSamples}/`, `design/Systems/`, `Documentation/`, `Localization/`, `Builds/`.

## asmdef health

- `Dab.Runtime` references `Unity.RenderPipelines.Universal.Runtime` and `Unity.TextMeshPro` but has **zero** code using either — dead references.
- `Dab.Tests` sets `includePlatforms: []` (all platforms) while mixing `#if UNITY_EDITOR`-guarded EditMode-style asset tests with PlayMode `[UnityTest]` scene tests in one assembly — worth confirming Unity classifies it as intended.
