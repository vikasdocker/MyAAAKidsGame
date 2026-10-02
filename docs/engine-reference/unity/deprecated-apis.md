# Deprecated APIs — Unity 6000.3

**Last verified: 2026-09-30**

"Don't use X → Use Y" reference for this project.

---

## Input

| Don't Use | Use Instead | Notes |
|---|---|---|
| `UnityEngine.Input.touches` | `Touch.activeTouches` | Requires `EnhancedTouchSupport.Enable()` |
| `UnityEngine.Input.touchCount` | `Touch.activeTouches.Count` | Requires `EnhancedTouchSupport.Enable()` |
| `UnityEngine.Input.GetTouch(i)` | `Touch.activeTouches[i]` | Requires `EnhancedTouchSupport.Enable()` |
| `UnityEngine.Input.touchSupported` | `Touchscreen.current != null` | No enable needed |
| `OnMouseDown` / `OnMouseUp` / `OnMouseEnter` | `IPointerDownHandler` / `IPointerUpHandler` / `IPointerEnterHandler` | `IPointer*` also needs a `PhysicsRaycaster` on the camera and an EventSystem in the scene |
| Polling `Touchscreen.current.touches` in `Update` | `Touch.activeTouches` | Reading raw `Touchscreen` state in `Update` **misses state changes** — the docs are explicit about this |
| The legacy Input Manager as active handler | New Input System | Deprecated in 6.3; logs warnings |

---

## URP

| Don't Use | Use Instead | Notes |
|---|---|---|
| URP Compatibility Mode | URP Render Graph | **Removed** in 6.3; `enableRenderCompatibilityMode` is read-only `false` |
| `URP_COMPATIBILITY_MODE` define as a permanent solution | Port to Render Graph | Migration-only; stops working in 6.4 |
| Legacy custom render pass/pass data APIs | Render Graph pass API | URP and HDRP now share the Render Graph compiler |

---

## Package Compile-Out Defines (Input System 1.18+)

These defines no longer exist — the functionality can no longer be removed:

| Removed Define | Consequence |
|---|---|
| `UNITY_INPUT_SYSTEM_PROJECT_WIDE_ACTIONS` | Project-wide Input Actions are always compiled in |
| `USE_IMGUI_EDITOR_FOR_ASSETS` | **Deprecated** feature option |
| Auto-save-on-focus-lost compile-out | Can no longer be stripped |

Also removed in 1.18: all code supporting Unity versions older than **2022.3 LTS**.

---

## Notes on Enhanced Touch

Two distinctions the docs draw explicitly, and which matter for input code:

- **Finger** = the Nth contact point on a screen. Query with `Touch.activeFingers`.
- **Touch** = a complete contact from `Began` through `Ended`/`Canceled`. Query
  with `Touch.activeTouches`.

`EnhancedTouch` costs some overhead because it must record touches as they occur,
which is why `EnhancedTouchSupport.Enable()` is required — it cannot be enabled
lazily mid-gesture. Enable it during app boot, not on first touch.

`EnhancedTouch` also protects against a specific failure mode `Touchscreen` has:
a touch shorter-lived than a single input update can be overwritten by a new touch
in that same update. `EnhancedTouch` retains all records for the update. For a
game where a child's tap must always register, this matters.

Touch IDs are unique only with respect to *active* touches — platforms may reuse
IDs after touches finish.

---

## Version Caveat

`OnMouse` MonoBehaviour events gain support in the Input System package only on
Unity **6000.4+**. This project is on 6000.3, so those events remain unsupported.
Use the `IPointer*` interfaces instead.