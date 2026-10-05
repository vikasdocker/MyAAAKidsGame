using System;
using UnityEngine;
using Dab.Runtime.Abilities;
using Dab.Runtime.Core;
using Dab.Runtime.Painting;
using Dab.Runtime.UI;

namespace Dab.Runtime.Playtest
{
    /// <summary>
    /// Builds the painting playtest rig at runtime so the scene file itself
    /// stays trivial (a single empty GameObject with this component).
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
    ///   - one creature GameObject (built by <see cref="PaintingRig"/>) carrying
    ///     input, procedural mesh, canvas material, paint controller, state
    ///     machine and FX spawner, fully interconnected: a drag paints the
    ///     creature, moves it into the Painting state and ejects ink bursts at
    ///     the cursor; a gentle sustained hold is Petting; a flourish fires the
    ///     12-spoke success radial
    ///   - directional light and camera framed on the creature
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
            var rig = PaintingRig.Build(
                "Creature",
                Vector3.zero,
                Vector3.one * _creatureScale);

            SignatureMarkAdornUI.Build(
                rig.Creature.GetComponent<SignatureMarkPlacementController>());

            BuildLighting();
            BuildCamera(rig.Generator);

            Debug.Log(
                $"[{nameof(CreaturePlaytestBootstrap)}] Playtest rig ready. " +
                $"Creature triangles: {rig.Generator.TotalTriangleCount}. " +
                "Drag anywhere on the creature to paint it; it reacts to its new " +
                "look and ink follows the cursor.",
                this);
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

            // The zero-texture shell policy means an unpainted creature is a solid
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
