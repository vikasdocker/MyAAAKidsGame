using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Dab.Runtime.Creature;
using Dab.Runtime.FX;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Dab.Tests.VisualBridge
{
    /// <summary>
    /// Verifies the assets created by the two generators
    /// (<c>Dab.Editor.CreatureFXAssetGenerator</c> and
    /// <c>Dab.Editor.CreatureAnimatorControllerGenerator</c>) that this package
    /// now ships with.
    ///
    /// These tests exist so the generated assets cannot silently regress: a
    /// re-run of the generator must be reproducible, the success radial really
    /// has twelve spokes, and the bridge must bind the generated controller
    /// without running unbound.
    /// </summary>
    public sealed class GeneratedVisualAssetsTests
    {
        private GameObject _go;

        private static string FxFolder => Path.Combine(
            Path.GetDirectoryName(Application.dataPath),
            "Assets", "Art", "FX", "Resources", "FX");

        private static readonly string[] FxFiles =
        {
            "FX_FloatingHeart.png",
            "FX_InkBloom.png",
            "FX_SuccessRadial.png"
        };

        [TearDown]
        public void Teardown()
        {
            if (_go != null)
            {
                UnityEngine.Object.DestroyImmediate(_go);
                _go = null;
            }
        }

        #region Generated FX Sprites

        [Test]
        public void GeneratedFxTexturesExistAndArePowerOfTwo()
        {
            for (var i = 0; i < FxFiles.Length; i++)
            {
                var path = Path.Combine(FxFolder, FxFiles[i]);
                Assert.IsTrue(File.Exists(path),
                    "Missing generated sprite " + FxFiles[i] +
                    " — run Tools > Dab > Art > Generate FX Textures.");

                var texture = LoadPng(path);
                Assert.IsNotNull(texture, "Could not decode " + FxFiles[i]);
                Assert.IsTrue(IsPowerOfTwo(texture.width),
                    FxFiles[i] + " width " + texture.width + " is not a power of two.");
                Assert.IsTrue(IsPowerOfTwo(texture.height),
                    FxFiles[i] + " height " + texture.height + " is not a power of two.");
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        [Test]
        public void GeneratedSuccessRadialHasTwelveSpokes()
        {
            var texture = LoadPng(Path.Combine(FxFolder, "FX_SuccessRadial.png"));
            Assert.IsNotNull(texture,
                "Missing generated sprite FX_SuccessRadial.png.");

            // Sample alpha on a ring. If the sprite really is a 12-spoke radial,
            // the average alpha aligned with a spoke exceeds the average midway
            // between spokes. The difference must be unambiguous: palette.json
            // C7 makes the spoke pattern the readability carrier, so a 12-spoke
            // wheel that nearly reads as a disc would not survive greyscale.
            var steps = 48;
            var aligned = 0f;
            var midway = 0f;
            var alignedCount = 0;
            var midwayCount = 0;

            for (var s = 0; s < steps; s++)
            {
                var angle = 2f * Mathf.PI * s / steps;

                var x = Mathf.RoundToInt(0.5f * texture.width + Mathf.Cos(angle) * texture.width * 0.30f);
                var y = Mathf.RoundToInt(0.5f * texture.height + Mathf.Sin(angle) * texture.height * 0.30f);

                var step = 2f * Mathf.PI / 12f;
                var frac = Mathf.Abs((angle / step) - Mathf.Round(angle / step));
                var alpha = texture.GetPixel(x, y).a;

                if (frac <= 0.25f)
                {
                    aligned += alpha;
                    alignedCount++;
                }
                else
                {
                    midway += alpha;
                    midwayCount++;
                }
            }

            UnityEngine.Object.DestroyImmediate(texture);

            Assert.Greater(alignedCount, 0, "No aligned samples collected.");
            Assert.Greater(midwayCount, 0, "No midway samples collected.");

            var alignedAverage = aligned / alignedCount;
            var midwayAverage = midway / midwayCount;

            Assert.Greater(alignedAverage - midwayAverage, 0.05f,
                "Success radial does not read as twelve spokes: aligned " +
                alignedAverage.ToString("0.000") +
                " vs midway " + midwayAverage.ToString("0.000") + ".");
        }

        [Test]
        public void GeneratedHeartIsFilledAndLeftRightSymmetric()
        {
            var texture = LoadPng(Path.Combine(FxFolder, "FX_FloatingHeart.png"));
            Assert.IsNotNull(texture,
                "Missing generated sprite FX_FloatingHeart.png.");

            var left = 0f;
            var right = 0f;
            const int samples = 64;

            for (var i = 0; i < samples; i++)
            {
                var xLeft = Mathf.RoundToInt(0.5f * texture.width - i * 0.0045f * texture.width);
                var xRight = Mathf.RoundToInt(0.5f * texture.width + i * 0.0045f * texture.width);
                var y = Mathf.RoundToInt(0.45f * texture.height);

                left += texture.GetPixel(Mathf.Clamp(xLeft, 0, texture.width - 1), y).a;
                right += texture.GetPixel(Mathf.Clamp(xRight, 0, texture.width - 1), y).a;
            }

            // A filled heart: a horizontal cut through the lobes at 0.45 of the
            // height must hit some opaque pixels on both sides.
            Assert.Greater(left, 0f, "Heart has no filled opaque pixels on the left lobe.");
            Assert.Greater(right, 0f, "Heart has no filled opaque pixels on the right lobe.");
            Assert.AreEqual(left, right, 0.05f / samples,
                "Heart is not left/right symmetric, so it will not read as a heart.");

            UnityEngine.Object.DestroyImmediate(texture);
        }

        #endregion

        #region Generated Animator Controller

#if UNITY_EDITOR
        private const string ControllerPath =
            "Assets/Art/Animation/CreatureAnimatorController.controller";

        private static readonly string[] StateNames =
        {
            "Rest_Happy", "Absorb_Painting", "Focus_Petting", "Greet_Return", "Proud_Flourish"
        };

        private static readonly string[] StateIds =
        {
            "Idle", "Painting", "Petting", "Feeding", "AbilityUnlock"
        };

        private static AnimatorController LoadController()
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>(
                ControllerPath);
        }
#endif

        [Test]
        public void GeneratedControllerDeclaresBaseAndEchoLayers()
        {
#if UNITY_EDITOR
            var controller = LoadController();
            Assert.IsNotNull(controller,
                "Generated controller missing — run Tools > Dab > Art > " +
                "Generate Creature Animator.");

            Assert.AreEqual(2, controller.layers.Length,
                "Expected exactly two layers: Base + Echo (art-bible layer set).");
            Assert.AreEqual("Base", controller.layers[0].name,
                "First layer must be named 'Base' for the bridge contract.");
            Assert.AreEqual("Echo", controller.layers[1].name,
                "Second layer must be named 'Echo' for the bridge contract.");
            Assert.AreEqual(
                AnimatorLayerBlendingMode.Additive, controller.layers[1].blendingMode,
                "Echo must be additive, or it will shimmer against the base pose.");
            Assert.AreEqual(0f, controller.layers[1].defaultWeight, 1e-4f,
                "Echo must start silent and be driven up by the painting state.");
#else
            Assert.Inconclusive("This test requires the Unity Editor.");
#endif
        }

        [Test]
        public void GeneratedControllerDeclaresBridgeParameters()
        {
#if UNITY_EDITOR
            var controller = LoadController();
            Assert.IsNotNull(controller);

            var parameters = controller.parameters;
            var floats = new System.Collections.Generic.HashSet<string>();
            var triggers = new System.Collections.Generic.HashSet<string>();
            var hasTransitionSpeed = false;

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter.type == AnimatorControllerParameterType.Float)
                {
                    floats.Add(parameter.name);
                }
                else if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    triggers.Add(parameter.name);
                }
            }

            hasTransitionSpeed = floats.Contains("TransitionSpeed");

            for (var i = 0; i < StateIds.Length; i++)
            {
                Assert.IsTrue(floats.Contains("State_" + StateIds[i]),
                    "Missing float State_" + StateIds[i] + " the bridge writes.");
                Assert.IsTrue(triggers.Contains("Trigger_" + StateIds[i]),
                    "Missing trigger Trigger_" + StateIds[i] + " the bridge fires.");
            }

            Assert.IsTrue(hasTransitionSpeed,
                "Missing float TransitionSpeed the bridge writes for cross-fade pacing.");
#else
            Assert.Inconclusive("This test requires the Unity Editor.");
#endif
        }

        [Test]
        public void GeneratedControllerBaseHasFiveStatesWithBlendTreesAndFullConnections()
        {
#if UNITY_EDITOR
            var controller = LoadController();
            Assert.IsNotNull(controller);

            var baseMachine = controller.layers[0].stateMachine;
            Assert.AreEqual(5, baseMachine.states.Length,
                "Base layer must contain one state per CreatureStateId.");

            var states = new System.Collections.Generic.Dictionary<string, AnimatorState>();
            for (var i = 0; i < baseMachine.states.Length; i++)
            {
                var name = baseMachine.states[i].state.name;
                states[name] = baseMachine.states[i].state;
            }

            for (var i = 0; i < StateNames.Length; i++)
            {
                Assert.IsTrue(states.ContainsKey(StateNames[i]),
                    "Base layer state '" + StateNames[i] + "' missing.");
            }

            Assert.IsNotNull(baseMachine.defaultState,
                "Base layer must declare a default state.");
            Assert.AreEqual("Rest_Happy", baseMachine.defaultState.name,
                "Idle must be the default state.");

            // Every state is a 1D blend tree on its own State_<id> float.
            for (var i = 0; i < StateNames.Length; i++)
            {
                var motion = states[StateNames[i]].motion as BlendTree;
                Assert.IsNotNull(motion,
                    "State '" + StateNames[i] +
                    "' is not a blend tree; the bridge's float would have nothing to drive.");
                Assert.AreEqual(
                    "State_" + StateIds[i], motion.blendParameter,
                    "State '" + StateNames[i] + "' must be keyed on State_" + StateIds[i] + ".");
            }

            // Fully-connected trigger graph: every state can move to every other
            // state on that target's trigger (Mecanim forbids Trigger conditions
            // on AnyState transitions, so a generated hub is not an option).
            var transitionCount = 0;
            for (var i = 0; i < StateNames.Length; i++)
            {
                var transitions = states[StateNames[i]].transitions;
                Assert.AreEqual(StateNames.Length - 1, transitions.Length,
                    "State '" + StateNames[i] +
                    "' must connect to every other state.");

                for (var t = 0; t < transitions.Length; t++)
                {
                    Assert.IsFalse(transitions[t].hasExitTime,
                        "Mode transitions must be trigger-driven, not time-driven.");
                    var firedForTarget = false;
                    var conditions = transitions[t].conditions;
                    for (var c = 0; c < conditions.Length; c++)
                    {
                        if (conditions[c].mode == AnimatorConditionMode.If &&
                            conditions[c].parameter.StartsWith("Trigger_"))
                        {
                            firedForTarget = true;
                        }
                    }

                    Assert.IsTrue(firedForTarget,
                        "Transition out of '" + StateNames[i] +
                        "' has no trigger condition, so the bridge cannot fire it.");
                    transitionCount++;
                }
            }

            Assert.AreEqual(20, transitionCount,
                "Expected 5 states x 4 outgoing transitions = 20.");
#else
            Assert.Inconclusive("This test requires the Unity Editor.");
#endif
        }

        [Test]
        public void GeneratedControllerEchoLayerHasOneAdditiveState()
        {
#if UNITY_EDITOR
            var controller = LoadController();
            Assert.IsNotNull(controller);

            var echoMachine = controller.layers[1].stateMachine;
            Assert.AreEqual(1, echoMachine.states.Length,
                "Echo layer must contain a single additive state.");
            Assert.AreEqual("Echo_Painting", echoMachine.states[0].state.name);
            Assert.IsNotNull(echoMachine.defaultState);
            Assert.IsNotNull(echoMachine.states[0].state.motion,
                "Echo state must have motion so the additive layer is not empty.");
#else
            Assert.Inconclusive("This test requires the Unity Editor.");
#endif
        }

        #endregion

        #region Native Bridge Binding

        [UnityTest]
        public IEnumerator BridgeDrivesGeneratedControllerWithEcho()
        {
#if UNITY_EDITOR
            var controller = LoadController();
            if (controller == null)
            {
                Assert.Inconclusive("Generated controller missing; bridge binding cannot run.");
                yield break;
            }

            _go = new GameObject("BridgeNative");
            _go.AddComponent<Animator>().runtimeAnimatorController = controller;

            var machine = _go.AddComponent<CreatureStateMachine>();
            var bridge = _go.AddComponent<CreatureAnimationBridge>();
            yield return null;

            Assert.IsTrue(bridge.IsDrivingAnimator,
                "Bridge must drive the generated controller when both layers exist.");

            // Hold the transition like the other bridge tests: the machine's
            // own 4 s idle-return would otherwise drag the state back before
            // the echo weight is read.
            machine.enabled = false;
            machine.ForceTransitionTo(CreatureStateId.Painting);

            yield return WaitForWeight(bridge, CreatureStateId.Painting, 0.9f, 3f);

            var animator = _go.GetComponent<Animator>();
            var echoIndex = animator.GetLayerIndex("Echo");
            var baseIndex = animator.GetLayerIndex("Base");

            Assert.GreaterOrEqual(echoIndex, 0, "Generated controller has no Echo layer.");
            Assert.GreaterOrEqual(baseIndex, 0, "Generated controller has no Base layer.");

            Assert.AreEqual(1f, animator.GetLayerWeight(baseIndex), 0.001f,
                "Base layer must stay pinned at full weight.");

            Assert.AreEqual(
                bridge.GetWeight(CreatureStateId.Painting),
                animator.GetLayerWeight(echoIndex),
                0.05f,
                "Echo layer weight must mirror the damped painting weight.");
#else
            Assert.Inconclusive("This test requires the Unity Editor.");
            yield break;
#endif
        }

        private static IEnumerator WaitForWeight(
            CreatureAnimationBridge bridge,
            CreatureStateId state,
            float threshold,
            float timeoutSeconds)
        {
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (bridge.GetWeight(state) < threshold &&
                   Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }
        }

        #endregion

        #region Play-Mode Integration (Spawner)

        [UnityTest]
        public IEnumerator SpawnerAppliesFixedHeartColourFromResources()
        {
            // The heart's colour is palette.json Attention #FFE3B8 and must not
            // come from the inherited paint. The hue setter must only tint the
            // ink and success materials.
            _go = new GameObject("SpawnerTint");
            _go.AddComponent<CreatureStateMachine>();
            var spawner = _go.AddComponent<CreatureFXSpawner>();
            yield return null;

            Assert.IsTrue(spawner.IsReady);

            var expectedHeart = new Color(1f, 227f / 255f, 184f / 255f, 1f);

            var paint = new Color(0.4f, 0.78f, 0.95f, 1f);
            spawner.SetInheritedPaintHue(paint);
            Assert.AreEqual(paint, spawner.InheritedPaintHue);

            var inkMaterial = GetPoolMaterial(_go, "Pool_Ink");
            var heartMaterial = GetPoolMaterial(_go, "Pool_Heart");
            var successMaterial = GetPoolMaterial(_go, "Pool_Success");

            Assert.IsNotNull(inkMaterial, "Ink emitter missing its material.");
            Assert.IsNotNull(heartMaterial, "Heart emitter missing its material.");
            Assert.IsNotNull(successMaterial, "Success emitter missing its material.");

            // Material colors round-trip through the native layer with low-bit
            // float drift, so compare per-channel with a tolerance rather than
            // NUnit's default exact-equality on the struct.
            AssertColorEquals(paint, inkMaterial.GetColor("_BaseColor"), 0.0001f,
                "Ink must inherit the paint hue.");
            AssertColorEquals(paint, successMaterial.GetColor("_BaseColor"), 0.0001f,
                "Success radial must inherit the paint hue.");
            AssertColorEquals(expectedHeart, heartMaterial.GetColor("_BaseColor"), 0.0001f,
                "Heart must stay at the fixed Attention colour, not the paint hue.");

            yield return null;
        }

        private static Material GetPoolMaterial(GameObject root, string emitterName)
        {
            var renderers = root.GetComponentsInChildren<ParticleSystemRenderer>(true);
            for (var i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].name == emitterName)
                {
                    return renderers[i].sharedMaterial;
                }
            }

            return null;
        }

        private static void AssertColorEquals(
            Color expected, Color actual, float tolerance, string message)
        {
            var delta = Mathf.Max(
                Mathf.Abs(expected.r - actual.r),
                Mathf.Max(
                    Mathf.Abs(expected.g - actual.g),
                    Mathf.Max(Mathf.Abs(expected.b - actual.b), Mathf.Abs(expected.a - actual.a))));

            Assert.LessOrEqual(delta, tolerance,
                message + " Expected " + expected + " but was " + actual + " (delta " + delta + ").");
        }

        #endregion

        #region PNG Helpers

        private static Texture2D LoadPng(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(path)))
            {
                UnityEngine.Object.DestroyImmediate(texture);
                return null;
            }

            return texture;
        }

        private static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }

        #endregion
    }
}