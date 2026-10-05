using UnityEngine;
using Dab.Runtime.Abilities;
using Dab.Runtime.Creature;
using Dab.Runtime.FX;
using Dab.Runtime.Input;
using Dab.Runtime.Painting;
using Dab.Runtime.Save;
using Dab.Runtime.UI;

namespace Dab.Runtime.Core
{
    /// <summary>
    /// The connected result of building one creature rig.
    ///
    /// Every component resolves the others by <see cref="Component.GetComponent"/>
    /// on the same GameObject, so the wiring contract below lives in exactly one
    /// place (<see cref="Dab.Runtime.Core.PaintingRig.Build"/>) and each consumer
    /// that needs a peer finds it without a serialized reference that could drift.
    /// </summary>
    public sealed class PaintingRigNode
    {
        /// <summary>The creature GameObject carrying every rig component.</summary>
        public GameObject Creature { get; }

        /// <summary>The input manager on the creature. Drives everything.</summary>
        public FluidTouchInputManager Input { get; }

        /// <summary>Breeds the procedural surface and screen→UV/→world bridges.</summary>
        public CreatureTestMeshGenerator Generator { get; }

        /// <summary>Stamps paint into the creature's canvas. Bound to Input.</summary>
        public CreaturePaintController Paint { get; }

        /// <summary>Routes input to the current creature state (Idle/Painting/…).</summary>
        public CreatureStateMachine Machine { get; }

        /// <summary>Pools particle bursts for painting, petting and flourishes.</summary>
        public CreatureFXSpawner FX { get; }

        /// <summary>The creature's Animator, driving the generated controller.</summary>
        public Animator Animator { get; }

        /// <summary>State machine → Animator weight/trigger translator.</summary>
        public CreatureAnimationBridge AnimationBridge { get; }

        /// <summary>The creature's MeshRenderer hosting the canvas material.</summary>
        public Renderer Renderer { get; }

        internal PaintingRigNode(
            GameObject creature,
            FluidTouchInputManager input,
            CreatureTestMeshGenerator generator,
            CreaturePaintController paint,
            CreatureStateMachine machine,
            CreatureFXSpawner fx,
            Animator animator,
            CreatureAnimationBridge animationBridge,
            Renderer renderer)
        {
            Creature = creature;
            Input = input;
            Generator = generator;
            Paint = paint;
            Machine = machine;
            FX = fx;
            Animator = animator;
            AnimationBridge = animationBridge;
            Renderer = renderer;
        }
    }

    /// <summary>
    /// Builds one fully interconnected creature rig at runtime.
    ///
    /// Wiring performed (in one place, never re-derived):
    ///   - a single GameObject carries input, the procedural grid, the canvas
    ///     shader material, the paint controller, the state machine and the FX
    ///     spawner, so each finds its peers by GetComponent on self
    ///   - CreaturePaintController.BindToInput connects touch/mouse strokes to
    ///     dabs through the generator's screen-to-UV delegate
    ///   - the state machine subscribes to the same input manager, so a drag
    ///     becomes a Painting state and a gentle sustained hold becomes Petting
        ///   - the FX spawner subscribes to the state machine and ejects ink bursts
        ///     at the cursor while Painting, hearts on Petting, and the 12-spoke
        ///     success radial on an ability flourish
        ///   - the animation bridge subscribes to the same machine and drives the
        ///     generated Base+Echo controller (loaded from Resources), so a state
        ///     change is visible as motion without sound or colour
    ///
    /// Layout note: assets are built from code because a hand-authored scene
    /// would need .meta GUIDs generated on first open, which AGENTS.md forbids
    /// hand-authoring. This is the same decision the playtest scene makes.
    ///
    /// The rig is deliberately prefabrication: art-bible §7 gates asset
    /// production, so the surface here is the procedural CreatureTestMesh and the
    /// materials are runtime clones. When the sculpted creature arrives this
    /// class is replaced, it is not extended.
    /// </summary>
    public static class PaintingRig
    {
        /// <summary>
        /// Generated Base+Echo controller, reachable at runtime because the
        /// asset lives under a Resources folder. Same loading pattern the FX
        /// spawner uses for its generated textures.
        /// </summary>
        private const string AnimatorControllerResource = "CreatureAnimatorController";

        /// <summary>
        /// Builds a connected rig targeting <paramref name="name"/>, positioned at
        /// <paramref name="position"/> and scaled to <paramref name="scale"/>.
        ///
        /// Components are added while the GameObject is inactive and the entity is
        /// activated last, in the same order the playtest rig established:
        /// CreaturePaintController.OnEnable needs its TargetRenderer before it
        /// runs, and every consumer must exist before activation so the Awake-time
        /// GetComponent resolutions and input subscriptions all succeed.
        /// </summary>
        public static PaintingRigNode Build(string name, Vector3 position, Vector3 scale)
        {
            var creature = new GameObject(name);
            creature.transform.position = position;
            creature.transform.localScale = scale;

            // Deactivate before AddComponent. AddComponent runs OnEnable
            // synchronously only for an already-active object; on an inactive one
            // Awake/OnEnable are deferred until activation, in script-execution
            // order. Building cold lets every reference be in place first.
            creature.SetActive(false);

            // MeshFilter first so the generator's RequireComponent and the
            // collider setup have a filter to share even while inactive.
            creature.AddComponent<MeshFilter>();
            var renderer = creature.AddComponent<MeshRenderer>();

            var shader = Shader.Find("Dab/Paint/CreatureCanvas");
            if (shader == null)
            {
                Debug.LogError(
                    $"[{nameof(PaintingRig)}] Shader 'Dab/Paint/CreatureCanvas' " +
                    "not found. Painting will not display.", creature);
            }
            else
            {
                // Cloned per-instance: the paint controller writes _PaintMap into
                // this material, and a shared asset would leak paint between
                // creatures and survive scene reloads in the editor.
                var canvasMaterial = new Material(shader)
                {
                    name = name + " Canvas (Runtime)"
                };
                canvasMaterial.SetColor("_BaseColor", Palette.DawnCream);
                renderer.material = canvasMaterial;
            }

            var generator = creature.AddComponent<CreatureTestMeshGenerator>();
            var paint = creature.AddComponent<CreaturePaintController>();
            paint.TargetRenderer = renderer;

            var machine = creature.AddComponent<CreatureStateMachine>();
            creature.AddComponent<CreatureSignatureMarks>();
            creature.AddComponent<CreatureSignatureMarkPersistence>();
            var fx = creature.AddComponent<CreatureFXSpawner>();
            var input = creature.AddComponent<FluidTouchInputManager>();
            creature.AddComponent<SignatureMarkPlacementController>();

            // Animation: the bridge translates state changes into Animator
            // layer weights and triggers, which is the art bible's motion
            // channel of state redundancy. Both are added while still inactive
            // and the controller is assigned here, so the bridge's Awake finds
            // a fully-formed Animator — it must never see a controller-less
            // one and downgrade to "track weights only".
            var animator = creature.AddComponent<Animator>();
            var controller =
                Resources.Load<RuntimeAnimatorController>(AnimatorControllerResource);
            if (controller != null)
            {
                animator.runtimeAnimatorController = controller;
            }
            else
            {
                Debug.LogError(
                    $"[{nameof(PaintingRig)}] Resources/" +
                    $"{AnimatorControllerResource} not found; the creature will " +
                    "behave correctly but will not animate. Run Tools > Dab > Art " +
                    "> Generate Creature Animator.", creature);
            }

            var animationBridge = creature.AddComponent<CreatureAnimationBridge>();

            // Connects touch/mouse events to dab stamping through the
            // generator's raycast bridge. Done before activation so the first
            // frame of play can already paint.
            paint.BindToInput(
                input,
                generator.CreateScreenToUvDelegate(),
                position => generator.TryScreenToUv(position, out _));

            // Everything every OnEnable depends on is in place, so activation
            // lets each component initialise exactly once, in the right order.
            creature.SetActive(true);

            return new PaintingRigNode(
                creature, input, generator, paint, machine, fx, animator,
                animationBridge, renderer);
        }
    }
}