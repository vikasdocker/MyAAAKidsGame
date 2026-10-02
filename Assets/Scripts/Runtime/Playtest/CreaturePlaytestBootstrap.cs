using System;
using UnityEngine;
using Dab.Runtime.Input;
using Dab.Runtime.Painting;

namespace Dab.Runtime.Playtest
{
    /// <summary>
    /// Builds the finger-painting playtest rig at runtime so the scene file
    /// itself stays trivial (a single empty GameObject with this component).
    ///
    /// Why this exists rather than authored scene objects
    /// ---------------------------------------------------
    /// A hand-authored .unity scene would have to reference every script by the
    /// GUID Unity assigns when it generates .meta files on first project open.
    /// Those GUIDs do not exist until then, and AGENTS.md is explicit that .meta
    /// files must not be hand-authored. Building the hierarchy in code sidesteps
    /// the GUID problem entirely and keeps the whole rig in one reviewable file.
    ///
    /// Wiring performed on Awake:
    ///   - creature mesh (generated procedurally) + MeshRenderer + canvas material
    ///   - directional light and camera framed on the creature
    ///   - CreaturePaintController bound to FluidTouchInputManager, with the
    ///     generator's own screen-to-UV raycast as the mapping function
    ///
    /// This is playtest scaffolding for the painting prototype, not production
    /// scene flow. Scene loading and game-state transitions are out of scope here.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CreaturePlaytestBootstrap : MonoBehaviour
    {
        [Header("Camera")]
        [Tooltip("Distance the camera sits in front of the creature, in world units. " +
                 "The creature is 1 unit tall, so ~3 units frames it with margin.")]
        [SerializeField, Range(1f, 10f)]
        private float _cameraDistance = 3f;

        [Tooltip("Vertical offset of the camera from the creature's mid-height. " +
                 "Positive looks slightly down at the creature.")]
        [SerializeField, Range(-1f, 2f)]
        private float _cameraHeight = 0.15f;

        [Header("Creature")]
        [Tooltip("Scale applied to the generated creature. 1.0 uses the " +
                 "generator's own 1-unit-tall proportions unchanged.")]
        [SerializeField, Range(0.5f, 5f)]
        private float _creatureScale = 1f;

        private void Awake()
        {
            var input = BuildInput();
            var (generator, controller) = BuildCreature(input);

            BuildLighting();
            BuildCamera(generator);

            Debug.Log(
                $"[{nameof(CreaturePlaytestBootstrap)}] Playtest rig ready. " +
                $"Creature triangles: {generator.TotalTriangleCount}. " +
                $"Paint with a finger anywhere on the creature.",
                this);
        }

        /// <summary>
        /// Touch input lives on its own object so the creature's transform stays
        /// clean and the input manager keeps its single responsibility.
        /// </summary>
        private static FluidTouchInputManager BuildInput()
        {
            var go = new GameObject("FluidTouchInput");
            return go.AddComponent<FluidTouchInputManager>();
        }

        /// <summary>
        /// Creates the creature, gives it a renderer with a material using
        /// CreatureCanvas.shader, and wires the paint controller to touch input.
        /// </summary>
        private (CreatureTestMeshGenerator, CreaturePaintController) BuildCreature(
            FluidTouchInputManager input)
        {
            var go = new GameObject("Creature");
            go.transform.localScale = Vector3.one * _creatureScale;

            // Deactivate before AddComponent. AddComponent runs OnEnable
            // synchronously, and the paint controller's OnEnable allocates the
            // paint texture and binds it to the target renderer's material.
            // Activating the object first would make OnEnable fire against a
            // half-configured controller: no renderer, no material, no mapping
            // delegate. It would log an error and disable itself, and the rig
            // would come up already broken.
            go.SetActive(false);

            var renderer = go.AddComponent<MeshRenderer>();

            var shader = Shader.Find("Dab/Paint/CreatureCanvas");
            if (shader == null)
            {
                Debug.LogError(
                    $"[{nameof(CreaturePlaytestBootstrap)}] Shader 'Dab/Paint/CreatureCanvas' " +
                    "not found. Painting will not display.", this);
            }
            else
            {
                // Cloned per-instance: the paint controller writes _PaintMap into
                // this material, and a shared asset would leak paint between
                // creatures and survive scene reloads in the editor.
                renderer.material = new Material(shader)
                {
                    name = "CreatureCanvas (Playtest)"
                };
            }

            // The generator must exist and have built its mesh before the paint
            // controller is enabled: the controller's OnEnable allocates the paint
            // texture and binds it to the renderer's material, and the screen-to-UV
            // delegate it is given raycasts against the generated MeshCollider.
            var generator = go.AddComponent<CreatureTestMeshGenerator>();

            var controller = go.AddComponent<CreaturePaintController>();
            controller.TargetRenderer = renderer;
            controller.BindToInput(input, generator.CreateScreenToUvDelegate());

            // Everything the controller's OnEnable depends on is in place, so
            // activation lets it initialise exactly once, in the right order.
            go.SetActive(true);

            return (generator, controller);
        }

        private static void BuildLighting()
        {
            var go = new GameObject("Directional Light");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;

            // Angled from the front-left and above so the creature's rounded form
            // reads with a visible light-to-shadow falloff. A head-on light flattens
            // the silhouette and hides exactly the shading cues that make a form
            // look painted rather than like flat vector art.
            go.transform.rotation = Quaternion.Euler(35f, -30f, 0f);
            light.intensity = 1.1f;
            light.color = Color.white;
        }

        private void BuildCamera(CreatureTestMeshGenerator generator)
        {
            var existing = Camera.main;
            if (existing != null)
            {
                return;
            }

            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";

            var camera = go.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;

            // The zero-texture shell policy means unpainted creature is a solid
            // colour; a neutral mid-tone background keeps it readable without
            // implying a finished environment.
            camera.backgroundColor = new Color(0.24f, 0.26f, 0.30f);
            camera.fieldOfView = 40f;

            var focus = generator.transform.position + Vector3.up * (0.5f * _cameraHeight + 0.5f);
            go.transform.position = new Vector3(0f, focus.y, -_cameraDistance);
            go.transform.rotation = Quaternion.LookRotation(focus - go.transform.position);
        }
    }
}
