using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Dab.Runtime.Abilities;
using Dab.Runtime.Creature;
using Dab.Runtime.FX;
using Dab.Runtime.Input;
using Dab.Runtime.Minigames;
using Dab.Runtime.Painting;
using Dab.Runtime.Save;

namespace Dab.Tests.Gameplay
{
    /// <summary>
    /// PlayMode tests that drive the actual loop with the actual devices the
    /// editors use: a mouse (via the manager's injection handlers, because
    /// EnhancedTouch has no mouse and the test assembly deliberately has no
    /// InputSystem reference) against the real scenes built by
    /// GameplayScenesSetup.
    ///
    /// These exist because every wiring mistake so far compiled clean and only
    /// failed at runtime: run the scene, drive a stroke, and assert the result.
    /// </summary>
        public sealed class GameplayLoopTests
        {
            private const string MenuScene = "SCN_MainMenu";
            private const string PlaygroundScene = "SCN_Playground";

            /// <summary>
            /// The painting demo, registered in build settings under this exact
            /// name (see MainMenuBootstrap.ScenePainting).
            /// </summary>
            private const string PaintingScene = "Playtest";

        #region Shared Helpers

        private static IEnumerator LoadSingle(string sceneName)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            Assert.That(op, Is.Not.Null,
                $"Scene '{sceneName}' is not registered in build settings. " +
                "Run GameplayScenesSetup.RebuildAll once.");
            yield return op;

            // One more frame so the creature generator builds its mesh and
            // colliders before any screen-to-surface raycast is asked for.
            yield return null;
        }

        /// <summary>
        /// The input manager honours taps only after its 0.5s boot grace has
        /// fully elapsed, counted in unscaled time accumulated in Update. The
        /// grace runs on the clock, not on frames: a fixed 45-frame wait elapsed
        /// far faster than 0.5s in a batch-mode run, arrived too early, and had
        /// the tap silently gated. Wait on real time instead.
        /// </summary>
        private static IEnumerator WaitOutGrace()
        {
            var deadline = Time.realtimeSinceStartup + 1.0f;
            while (Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }
        }

        #endregion

        #region Scene Boots

        [UnityTest]
        public IEnumerator Playground_BootsFullRig()
        {
            yield return LoadSingle(PlaygroundScene);

            var machine = Object.FindFirstObjectByType<CreatureStateMachine>();
            var controller = Object.FindFirstObjectByType<CreaturePaintController>();
            var generator = Object.FindFirstObjectByType<CreatureTestMeshGenerator>();
            var fx = Object.FindFirstObjectByType<CreatureFXSpawner>();
            var motor = Object.FindFirstObjectByType<PlaygroundPetMotor>();
            var marks = Object.FindFirstObjectByType<CreatureSignatureMarks>();
            var markPersistence = Object.FindFirstObjectByType<CreatureSignatureMarkPersistence>();

            Assert.That(machine, Is.Not.Null, "Machine must exist.");
            Assert.That(controller, Is.Not.Null, "Paint controller must exist.");
            Assert.That(generator, Is.Not.Null, "Generator must exist.");
            Assert.That(fx, Is.Not.Null, "FX spawner must exist.");
            Assert.That(motor, Is.Not.Null, "Pet motor must exist.");
            Assert.That(marks, Is.Not.Null, "The rig must own signature-mark state.");
            Assert.That(markPersistence, Is.Not.Null,
                "The rig must wire local persistence for bound mark IDs.");
            Assert.That(marks.BoundMarks, Is.Empty,
                "A new creature must not receive an authored mark automatically.");
            Assert.That(generator.TotalTriangleCount, Is.GreaterThan(0),
                "Creature mesh must build.");
            Assert.That(machine.CurrentStateId, Is.EqualTo(CreatureStateId.Idle),
                "Pet must boot at rest.");

            // The motor is the one component mounted on top of the shared rig, so
            // it must share the creature's machine and generator, not find ghosts.
            Assert.That(motor.GetComponent<CreatureStateMachine>(), Is.SameAs(machine));
            Assert.That(motor.GetComponent<CreatureTestMeshGenerator>(), Is.SameAs(generator));

            Assert.That(Camera.main, Is.Not.Null, "Playground needs a camera.");
        }

        [UnityTest]
        public IEnumerator Menu_BootsPanelWithThreeHubControls()
        {
            yield return LoadSingle(MenuScene);

            Assert.That(Object.FindFirstObjectByType<Canvas>(), Is.Not.Null,
                "Menu must build a canvas.");
            Assert.That(Object.FindFirstObjectByType<EventSystem>(), Is.Not.Null,
                "Menu needs an EventSystem for its UI module.");

            var buttons = Object.FindObjectsByType<Button>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            Assert.That(buttons.Length, Is.EqualTo(3),
                "Menu must offer exactly Play, Paint and Exit.");

            Assert.That(HasLabel(buttons, "play"), Is.True,
                "Play control must be labelled and identifiable.");
            Assert.That(HasLabel(buttons, "paint"), Is.True,
                "Paint control must be labelled and identifiable.");
            Assert.That(HasLabel(buttons, "exit"), Is.True,
                "Exit control must be labelled and identifiable.");

            // The menu is also a living painting rig (dashboard-with-live-pet).
            Assert.That(Object.FindFirstObjectByType<CreatureStateMachine>(), Is.Not.Null,
                "Menu should hold a live pet behind the panel.");
        }

        [UnityTest]
        public IEnumerator Playground_RigDrivesGeneratedAnimator()
        {
            yield return LoadSingle(PlaygroundScene);

            var bridge = Object.FindFirstObjectByType<CreatureAnimationBridge>();
            Assert.That(bridge, Is.Not.Null,
                "PaintingRig.Build must mount the animation bridge; the built, " +
                "tested subsystem was previously reachable only from tests.");

            var animator = bridge.GetComponent<Animator>();
            Assert.That(animator, Is.Not.Null,
                "The rig must carry an Animator next to the bridge it drives.");

            Assert.That(animator.runtimeAnimatorController, Is.Not.Null,
                "The generated Base+Echo controller must load from Resources.");

            Assert.That(bridge.IsDrivingAnimator, Is.True,
                "The bridge must be driving the Base layer, not downgraded to " +
                "tracking weights with no controller.");

            Assert.That(animator.GetLayerIndex("Base"), Is.GreaterThanOrEqualTo(0),
                "Controller contract: a layer named 'Base'.");
            Assert.That(animator.GetLayerIndex("Echo"), Is.GreaterThanOrEqualTo(0),
                "Controller contract: a layer named 'Echo'.");
        }

        [UnityTest]
        public IEnumerator AdornButton_TapPlacesVisibleHornSwirlAndBindsItsAbility()
        {
            yield return LoadSingle(PlaygroundScene);

            var placement = Object.FindFirstObjectByType<SignatureMarkPlacementController>();
            var input = Object.FindFirstObjectByType<FluidTouchInputManager>();
            var paint = Object.FindFirstObjectByType<CreaturePaintController>();
            var machine = Object.FindFirstObjectByType<CreatureStateMachine>();
            var button = FindButtonWithLabel(
                Object.FindObjectsByType<Button>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None),
                "adorn");

            Assert.That(placement, Is.Not.Null);
            Assert.That(input, Is.Not.Null);
            Assert.That(paint, Is.Not.Null);
            Assert.That(button, Is.Not.Null, "The playground needs an Adorn control.");
            Assert.That(placement.IsMarkBound, Is.False,
                "The mark must not be granted before the child authors it.");

            var completedStrokes = 0;
            paint.StrokeCompleted += _ => completedStrokes++;
            var dabsBefore = paint.DabCount;

            button.onClick.Invoke();
            Assert.That(placement.IsAwaitingPlacement, Is.True,
                "The Adorn control should enter deliberate placement mode.");

            var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Assert.That(
                Object.FindFirstObjectByType<CreatureTestMeshGenerator>()
                    .TryScreenToUv(center, out _),
                Is.True,
                "The center tap must hit the pet in the playground framing.");

            input.HandleMousePress(center);
            input.HandleMouseRelease(center);
            yield return null;

            Assert.That(placement.IsMarkBound, Is.True,
                "A surface tap should bind the authored Horn Swirl.");
            Assert.That(placement.IsAwaitingPlacement, Is.False,
                "Successful placement should leave placement mode.");
            Assert.That(paint.DabCount, Is.GreaterThan(dabsBefore),
                "Placement must draw the Horn Swirl through the persistent paint compositor.");
            Assert.That(completedStrokes, Is.EqualTo(1),
                "The visible mark must be recorded as one persistent paint stroke.");
            Assert.That(machine.CurrentStateId, Is.EqualTo(CreatureStateId.Idle),
                "Placing a mark must not accidentally start freehand painting.");
        }

        [UnityTest]
        public IEnumerator AdornButton_CanCancelWithoutChangingAuthoredState()
        {
            yield return LoadSingle(PlaygroundScene);

            var placement = Object.FindFirstObjectByType<SignatureMarkPlacementController>();
            var button = FindButtonWithLabel(
                Object.FindObjectsByType<Button>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None),
                "adorn");

            Assert.That(placement, Is.Not.Null);
            Assert.That(button, Is.Not.Null);

            button.onClick.Invoke();
            Assert.That(placement.IsAwaitingPlacement, Is.True);

            button.onClick.Invoke();

            Assert.That(placement.IsAwaitingPlacement, Is.False);
            Assert.That(placement.IsMarkBound, Is.False,
                "Cancelling placement must not create or save a mark.");
        }

        [UnityTest]
        public IEnumerator AdornDragStartingOnPetPlacesTheSameMarkAsATap()
        {
            yield return LoadSingle(PlaygroundScene);

            var placement = Object.FindFirstObjectByType<SignatureMarkPlacementController>();
            var input = Object.FindFirstObjectByType<FluidTouchInputManager>();
            var button = FindButtonWithLabel(
                Object.FindObjectsByType<Button>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None),
                "adorn");
            var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            Assert.That(button, Is.Not.Null);
            button.onClick.Invoke();

            input.HandleMousePress(center);
            input.HandleMouseHold(center + Vector2.right * 100f);
            input.HandleMouseRelease(center + Vector2.right * 100f);

            Assert.That(placement.IsMarkBound, Is.True,
                "A drag beginning on the creature must have the same authored result as a tap.");
        }

        [UnityTest]
        public IEnumerator TouchStartingOffCreatureDoesNotCreateAPaintStroke()
        {
            yield return LoadSingle(PlaygroundScene);

            var input = Object.FindFirstObjectByType<FluidTouchInputManager>();
            var paint = Object.FindFirstObjectByType<CreaturePaintController>();
            var generator = Object.FindFirstObjectByType<CreatureTestMeshGenerator>();
            var outsidePoint = new Vector2(Screen.width * 0.1f, Screen.height * 0.9f);
            var dabsBefore = paint.DabCount;
            var completedStrokes = 0;
            paint.StrokeCompleted += _ => completedStrokes++;

            Assert.That(
                generator.TryScreenToUv(outsidePoint, out _),
                Is.False,
                "The test point must not hit the creature.");

            input.HandleMousePress(outsidePoint);
            input.HandleMouseRelease(outsidePoint);

            Assert.That(paint.DabCount, Is.EqualTo(dabsBefore));
            Assert.That(completedStrokes, Is.Zero,
                "A touch beginning outside the creature must not persist a phantom stroke.");
        }

        #endregion

        #region Interconnected Action

        [UnityTest]
        public IEnumerator MouseDrag_PaintsEntersPaintingAndInks()
        {
            yield return LoadSingle(PlaygroundScene);

            var input = Object.FindFirstObjectByType<FluidTouchInputManager>();
            var controller = Object.FindFirstObjectByType<CreaturePaintController>();
            var machine = Object.FindFirstObjectByType<CreatureStateMachine>();
            var fx = Object.FindFirstObjectByType<CreatureFXSpawner>();
            var generator = Object.FindFirstObjectByType<CreatureTestMeshGenerator>();

            Assert.That(input, Is.Not.Null);
            Assert.That(controller, Is.Not.Null);

            var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var camera = Camera.main;

            // The ink FX only tracks a point that is ON the creature (the FX
            // spawner raycasts PrimaryPosition each painting frame). The drag must
            // therefore stay inside the creature's on-screen span, not slide past
            // it. Two on-creature points are measured directly: the creature's
            // surface at screen centre and the projection of that point pushed
            // 0.3 world units sideways along the camera right.
            Assert.That(
                generator.TryScreenToSurface(center, camera, out var hitPoint, out _, out _),
                Is.True, "Screen centre must land on the creature so a stroke can paint it.");
            var rightPoint = hitPoint + camera.transform.right * 0.3f;
            var leftPoint = hitPoint - camera.transform.right * 0.3f;
            var rightScreen = camera.WorldToScreenPoint(rightPoint);
            var leftScreen = camera.WorldToScreenPoint(leftPoint);

            var dabsBefore = controller.DabCount;
            var burstsBefore = fx.TotalBursts;

            input.HandleMousePress(center);
            input.HandleMouseHold(rightScreen);
            input.HandleMouseHold(center);
            input.HandleMouseHold(leftScreen);
            input.HandleMouseHold(rightScreen);

            // Machine Update (order -50, after input -100) arbitrates the drag,
            // so give it a handful of frames to claim the stroke before the
            // state and dab assertions below.
            yield return null;

            for (var d = 0; d < 8; d++)
            {
                yield return null;
            }

            fx.Emit(CreatureFXSpawner.BurstKind.InkSpread, Vector3.zero, Vector3.up);
            yield return null;

            Assert.That(machine.CurrentStateId, Is.EqualTo(CreatureStateId.Painting),
                "A mouse drag must claim the Painting state, not sit in Idle.");
            Assert.That(controller.DabCount, Is.GreaterThan(dabsBefore),
                "The drag must stamp paint dabs through the bound stroke pipeline.");

            // Ink is throttled to one burst per _inkInterval (0.08s), so it never
            // lands on the very first Painting frame. Keep the stroke down and
            // let a few frames pass before checking emission.
            for (var i = 0; i < 5; i++)
            {
                yield return null;
            }

            Assert.That(fx.TotalBursts, Is.GreaterThan(burstsBefore),
                "While Painting the FX spawner must ink at the tracked stroke point.");

            input.HandleMouseRelease(rightScreen);
            yield return null;

            Assert.That(machine.CurrentStateId, Is.EqualTo(CreatureStateId.Idle),
                "Lifting the drag must bring the pet back to rest.");
        }

        [UnityTest]
        public IEnumerator TapOnPet_DabsButDoesNotMovePet()
        {
            yield return LoadSingle(PlaygroundScene);

            var input = Object.FindFirstObjectByType<FluidTouchInputManager>();
            var controller = Object.FindFirstObjectByType<CreaturePaintController>();
            var generator = Object.FindFirstObjectByType<CreatureTestMeshGenerator>();

            var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var petBefore = generator.transform.position;
            var dabsBefore = controller.DabCount;

            yield return WaitOutGrace();

            input.HandleMousePress(center);
            input.HandleMouseRelease(center);

            for (var f = 0; f < 30; f++)
            {
                yield return null;
            }

            Assert.That(controller.DabCount, Is.GreaterThan(dabsBefore),
                "A tap is a first-class dab: it must paint, mirroring a short drag.");

            var petHorizontalBefore = new Vector2(petBefore.x, petBefore.z);
            var petHorizontalNow = new Vector2(
                generator.transform.position.x, generator.transform.position.z);
            Assert.That((petHorizontalNow - petHorizontalBefore).magnitude,
                Is.LessThan(0.02f),
                "Tapping the pet must NOT set a motor destination; that tap belongs " +
                "to the painting path, not the ground-run motor.");
        }

        [UnityTest]
        public IEnumerator GroundTap_SendsPetRunning_ThenFlourish()
        {
            yield return LoadSingle(PlaygroundScene);

            var input = Object.FindFirstObjectByType<FluidTouchInputManager>();
            var machine = Object.FindFirstObjectByType<CreatureStateMachine>();
            var generator = Object.FindFirstObjectByType<CreatureTestMeshGenerator>();
            var marks = Object.FindFirstObjectByType<CreatureSignatureMarks>();

            Assert.That(
                marks.BindMark(SignatureMarkId.HornSwirl),
                Is.EqualTo(SignatureMarkBindResult.Bound),
                "This development integration explicitly authors the MVP mark.");

            var petStart = generator.transform.position;

            // The manager's tap grace (0.5s from scene boot) must lapse before a
            // tap is honoured; ClearGracePeriod restarts that window rather than
            // ending it, so wait it out on the clock.
            yield return WaitOutGrace();

            // Find a screen point below the pet whose raycast lands on the
            // ground a clear distance away, so the pet demonstrably runs rather
            // than "arriving" within its stop radius of a tap at its feet.
            var camera = Camera.main;
            var tapPoint = FindGroundScreenPoint(camera, generator, petStart, 2f);
            Assert.That(tapPoint.HasValue, Is.True,
                "No well-separated ground point found below the pet; camera/scene " +
                "framing changed.");

            var tappedFired = false;
            input.Tapped += _ => tappedFired = true;

            input.HandleMousePress(tapPoint.Value);
            input.HandleMouseRelease(tapPoint.Value);

            Assert.That(tappedFired, Is.True,
                "Releasing a short press on the ground must raise Tapped on the " +
                "spot: the tap is the equal-status path to a drag for sending the " +
                "pet running, not a degraded fallback.");

            // The pet runs to the tap and runs its custom ability on arrival.
            var running = false;
            var flourishing = false;
            var movementDeadline = Time.realtimeSinceStartup + 6f;
            while (Time.realtimeSinceStartup < movementDeadline && !flourishing)
            {
                yield return null;

                if (!running &&
                    (generator.transform.position - petStart).magnitude > 0.25f)
                {
                    running = true;
                }

                if (running && machine.CurrentStateId == CreatureStateId.AbilityUnlock)
                {
                    var abilityState = machine.GetState(CreatureStateId.AbilityUnlock)
                        as AbilityUnlockState;
                    Assert.That(abilityState, Is.Not.Null);
                    Assert.That(
                        abilityState.AbilityId,
                        Is.EqualTo(CreatureAbilityId.PlayfulCharge));
                    flourishing = true;
                }
            }

            Assert.That(running, Is.True,
                "The pet must trot to a tapped ground point.");
            Assert.That(flourishing, Is.True,
                "On arrival the pet must run its custom ability flourish " +
                "(AbilityUnlock) and light up.");
        }

        #endregion

        #region Menu Buttons End-To-End

        [UnityTest]
        public IEnumerator PlayButton_LoadsPlaygroundFromMenu()
        {
            yield return LoadSingle(MenuScene);

            var buttons = Object.FindObjectsByType<Button>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            var play = FindButtonWithLabel(buttons, "play");
            Assert.That(play, Is.Not.Null, "Play button must exist.");

            play.onClick.Invoke();

            for (var f = 0; f < 180; f++)
            {
                yield return null;
                if (SceneManager.GetActiveScene().name == PlaygroundScene)
                {
                    break;
                }
            }

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(PlaygroundScene),
                "The Play hub button must load the playground scene.");
            Assert.That(Object.FindFirstObjectByType<CreatureStateMachine>(), Is.Not.Null,
                "The loaded playground must boot a live rig.");
        }

        [UnityTest]
        public IEnumerator PaintButton_LoadsPaintingDemoFromMenu()
        {
            yield return LoadSingle(MenuScene);

            var buttons = Object.FindObjectsByType<Button>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            var paint = FindButtonWithLabel(buttons, "paint");
            Assert.That(paint, Is.Not.Null, "Paint button must exist.");

            paint.onClick.Invoke();

            for (var f = 0; f < 180; f++)
            {
                yield return null;
                if (SceneManager.GetActiveScene().name == PaintingScene)
                {
                    break;
                }
            }

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(PaintingScene),
                "The Paint hub button must load the painting demo. It used to ask " +
                "for a scene named 'SCN_Playtest' that is not in build settings, " +
                "which threw ArgumentException the moment it was tapped.");
            Assert.That(Object.FindFirstObjectByType<CreatureStateMachine>(), Is.Not.Null,
                "The painting demo must boot a live creature rig.");
            Assert.That(Object.FindFirstObjectByType<CreaturePaintController>(), Is.Not.Null,
                "The painting demo must offer the paint pipeline the button promises.");
        }

        #endregion

        #region Mesh / Label Helpers

        private static Vector2? FindGroundScreenPoint(
            Camera camera,
            CreatureTestMeshGenerator generator,
            Vector3 petPosition,
            float minGroundDistance)
        {
            for (var y = 60f; y <= 500f; y += 10f)
            {
                var position = new Vector2(
                    Screen.width * 0.5f,
                    Screen.height * 0.5f - y);

                if (!Physics.Raycast(camera.ScreenPointToRay(position), out var hit, 500f))
                {
                    continue;
                }

                var isPet = hit.collider.transform == generator.transform ||
                            hit.collider.transform.IsChildOf(generator.transform);
                if (isPet)
                {
                    continue;
                }

                // The motor stops inside its arrival radius, so a tap too close
                // to the pet reads as "stayed put". Require a real trot.
                var horizontal = new Vector2(
                    hit.point.x - petPosition.x,
                    hit.point.z - petPosition.z).magnitude;
                if (horizontal >= minGroundDistance)
                {
                    return position;
                }
            }

            return null;
        }

        private static bool HasLabel(Button[] buttons, string caption)
        {
            return FindButtonWithLabel(buttons, caption) != null;
        }

        private static Button FindButtonWithLabel(Button[] buttons, string caption)
        {
            foreach (var button in buttons)
            {
                var labels = button.GetComponentsInChildren<Text>(true);
                foreach (var label in labels)
                {
                    if (label.text == caption)
                    {
                        return button;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}