using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Dab.Runtime.Core;

namespace Dab.Runtime.UI
{
    /// <summary>
    /// Builds the main-menu panel and its living pet preview at runtime so the
    /// scene file stays a single empty GameObject with this component. Mirrors
    /// PlaytestSceneSetup's "bootstrap in code" rule: no hand-authored .meta, no
    /// authored scene objects, and every connection lives in this file.
    ///
    /// Panel content (icon-first, read-free; typography two-removals-test):
    ///   - title wordmark (supportive label only)
    ///   - big PLAY hub button -> SCN_Playground
    ///   - big PAINT hub button -> Playtest (the painting demo)
    ///   - small EXIT button -> Application.Quit
    ///
    /// Behind the panel sits a full PaintingRig pet, dragged right through the
    /// same interconnected path as the playtest scene: drag to paint it, hold to
    /// pet, watch it react. Painting in the menu is the game's opening beat, not
    /// a shortcut to it, so the panel deliberately stays out of the creature's
    /// lower third on portrait layouts.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MainMenuBootstrap : MonoBehaviour
    {
        /// <summary>Build-settings scene names, referenced here exactly once.</summary>
        private const string ScenePlayground = "SCN_Playground";

        /// <summary>
        /// The painting demo. Registered in build settings as `Playtest`
        /// (first in the list); the SCN_ rename is tracked separately in
        /// docs/roadmap/TODO.md so this constant keeps matching reality.
        /// </summary>
        private const string ScenePainting = "Playtest";

        private void Awake()
        {
            BuildCameraAndLighting();
            BuildPet();

            var canvas = BuildCanvas();
            var root = BuildRoot(canvas);
            var hub = BuildHub(root);

            BuildTitle(root, hub);
            BuildPlayButton(hub);
            BuildPaintButton(hub);
            BuildExitButton(hub);

            Debug.Log(
                $"[{nameof(MainMenuBootstrap)}] Main menu ready. Drag the pet on " +
                "the right to paint it; the buttons below-open take you to the " +
                "playground or the painting demo.", this);
        }

        private void BuildCameraAndLighting()
        {
            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Palette.DawnCream;
            camera.fieldOfView = 40f;
            cameraGo.transform.position = new Vector3(0f, 1.6f, -5.2f);
            cameraGo.transform.rotation = Quaternion.LookRotation(
                new Vector3(0f, 0.6f, 0f) - cameraGo.transform.position);

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            light.intensity = 1.05f;
            light.color = new Color(1f, 0.96f, 0.88f);
        }

        /// <summary>
        /// A living pet behind the panel, painted with the same rig the playtest
        /// scene uses. It reads so the menu reads; a static panel against a bare
        /// gradient would fail the "pet is the point" beat before the first touch.
        ///
        /// Layout scale lives on a slot parent, not on the creature: the
        /// creature's root localScale.y is driven by the breathing animation
        /// clip, so authoring 0.85 on the creature itself would be overridden
        /// on the first animated frame and leave X/Z at 0.85 against Y at 1 —
        /// an off-uniform stretch. The slot keeps the authored size uniform.
        /// </summary>
        private void BuildPet()
        {
            var slot = new GameObject("MenuPetSlot");
            slot.transform.position = Vector3.zero;
            slot.transform.localScale = Vector3.one * 0.85f;

            var rig = PaintingRig.Build("MenuPet", Vector3.zero, Vector3.one);
            rig.Creature.transform.SetParent(slot.transform, true);
        }

        private static Canvas BuildCanvas()
        {
            var canvasGo = new GameObject("Menus Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            var eventsGo = new GameObject("EventSystem");
            eventsGo.AddComponent<EventSystem>();
            eventsGo.AddComponent<InputSystemUIInputModule>();

            return canvas;
        }

        private static RectTransform BuildRoot(Canvas canvas)
        {
            var rootGo = new GameObject("Menu Root", typeof(RectTransform));
            rootGo.transform.SetParent(canvas.transform, false);

            var root = rootGo.GetComponent<RectTransform>();
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;

            var shell = rootGo.AddComponent<Image>();
            shell.sprite = null;
            shell.color = Palette.Shell;

            return root;
        }

        /// <summary>
        /// A full-width padding panel that frames the hub cluster; helps taps
        /// land on the warm surface instead of a raw graphic edge.
        /// </summary>
        private static RectTransform BuildHub(RectTransform root)
        {
            var hubGo = new GameObject("Hub", typeof(RectTransform));
            hubGo.transform.SetParent(root, false);

            var hub = hubGo.GetComponent<RectTransform>();
            hub.anchorMin = new Vector2(0f, 0f);
            hub.anchorMax = new Vector2(1f, 1f);
            hub.pivot = new Vector2(0.5f, 0.5f);
            hub.sizeDelta = new Vector2(0f, -360f); // keep lower third free for the pet
            hub.anchoredPosition = new Vector2(0f, -120f);

            var panel = hubGo.AddComponent<Image>();
            panel.sprite = UxFactory.RoundedRect;
            panel.color = Palette.SurfaceRaised;

            return hub;
        }

        private void BuildTitle(RectTransform root, RectTransform hub)
        {
            UxFactory.CreateLabel(root, "Dab", 192, Palette.TextPrimary, new Vector2(0f, 620f));
            UxFactory.CreateLabel(root, "Paint · Play · Belong", 56, Palette.TextSecondary, new Vector2(0f, 470f));

            // Fresh-paint cue above the hub: the palette splash that teases the
            // painting beat. Purely supportive.
            var splash = UxFactory.CreateLabel(hub, "touch to paint", 48, Palette.TextPrimary, new Vector2(0f, 200f));
            splash.color = Palette.DawnCream;
        }

        private void BuildPlayButton(RectTransform hub)
        {
            UxFactory.CreateIconButton(
                hub,
                "\u25B6",           // ▶ black right-pointing triangle
                "play",
                560f, 320f,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, 20f),
                Palette.Active,
                Palette.TextPrimary,
                () =>
                {
                    Debug.Log("[MainMenuBootstrap] Play -> " + ScenePlayground + ".", this);
                    SceneManager.LoadScene(ScenePlayground);
                });
        }

        private void BuildPaintButton(RectTransform hub)
        {
            UxFactory.CreateIconButton(
                hub,
                "\u2716",           // ✖ heavy multiplication x (paint dab)
                "paint",
                400f, 240f,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, -320f),
                Palette.Surface,
                Palette.TextPrimary,
                () =>
                {
                    Debug.Log("[MainMenuBootstrap] Paint -> " + ScenePainting + ".", this);
                    SceneManager.LoadScene(ScenePainting);
                });
        }

        private void BuildExitButton(RectTransform hub)
        {
            UxFactory.CreateIconButton(
                hub,
                "\u2715",           // ✕ multiplication x
                "exit",
                220f, 180f,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, -620f),
                Palette.SurfaceRaised,
                Palette.TextSecondary,
                () =>
                {
                    Debug.Log("[MainMenuBootstrap] Exit requested.");
                    Application.Quit();
                });
        }
    }
}