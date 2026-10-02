using System;
using System.Collections;
using NUnit.Framework;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.TestTools;
using Dab.Runtime.Creature;
using Dab.Runtime.FX;

namespace Dab.Tests.VisualBridge
{
    /// <summary>
    /// Covers the three visual-bridge components: the animation bridge, the FX
    /// spawner, and the shader's declared strip point.
    ///
    /// What is tested and what is not
    /// ------------------------------
    /// Behaviour and allocation are tested directly. Visual output is not, because
    /// a pixel assertion on a relief contour requires an authored mesh, an
    /// authored paint texture, a camera, and a reference image, and a test that
    /// invents its own expectations for all four would only assert that the
    /// shader matches the author of the test. The greyscale and value-carrier
    /// rules the relief implements are recorded in palette.json critical-pairs
    /// C3 and C4 and are checked by eye during art review.
    ///
    /// The shader is checked structurally instead: the material's keywords and
    /// the shader's property block are asserted to agree with the spec, so the
    /// strip point cannot silently disappear from a shader edit.
    /// </summary>
    public sealed class VisualBridgeTests
    {
        private GameObject _go;

        [TearDown]
        public void Teardown()
        {
            if (_go != null)
            {
                UnityEngine.Object.DestroyImmediate(_go);
            }
        }

        #region Animation Bridge

        [UnityTest]
        public IEnumerator BridgeStartsFullyOnIdle()
        {
            _go = new GameObject("BridgeTest");
            _go.AddComponent<CreatureStateMachine>();
            var bridge = _go.AddComponent<CreatureAnimationBridge>();
            yield return null;

            // Weights start on Idle rather than blending up from zero, so the
            // first frame is a valid pose instead of a fade-in from a collapsed
            // mesh.
            Assert.AreEqual(1f, bridge.GetWeight(CreatureStateId.Idle), 0.001f);
            Assert.AreEqual(CreatureStateId.Idle, bridge.DominantState);
        }

        [UnityTest]
        public IEnumerator BridgeDampsTowardNewStateWithoutSnapping()
        {
            _go = new GameObject("BridgeTest");
            var machine = _go.AddComponent<CreatureStateMachine>();
            var bridge = _go.AddComponent<CreatureAnimationBridge>();
            yield return null;

            // Stop the machine's own Update so the forced state sticks. This is
            // not a workaround: _idleReturnDelay is 4 s, and the machine
            // deliberately returns to Idle on that timer so a creature can
            // never be stranded in a reactive state with no finger nearby. Over
            // the 120 frames this test waits, that auto-return fires and the
            // bridge correctly follows it back to Idle. Disabling the machine
            // isolates the blend the test is actually about.
            machine.enabled = false;

            machine.ForceTransitionTo(CreatureStateId.Petting);

            // art-bible 2.1 hard law 6 requires a state change to animate over at
            // least 0.4 s, so the weight must NOT reach 1 on the frame of the
            // transition. This is the single most important assertion here: a
            // bridge that assigned the weight directly would pass every other
            // test in this file and violate the art bible.
            yield return null;
            var immediate = bridge.GetWeight(CreatureStateId.Petting);
            Assert.Less(immediate, 0.9f,
                "Petting weight snapped to " + immediate + " in one frame; " +
                "art-bible 2.1 law 6 forbids a sub-0.4s state change.");
            Assert.Greater(immediate, 0f,
                "Petting weight did not begin to rise; the bridge is not tracking.");

            // Wait on elapsed time, not frame count. The bridge blends on
            // Time.unscaledDeltaTime, and under -batchmode consecutive frames
            // are almost zero apart, so a frame-count loop advances the blend by
            // a rounding error. The sum of per-frame delta time is the real
            // elapsed time, so waiting on the realtime clock converges the blend
            // the way it would on a device.
            yield return WaitForWeight(bridge, CreatureStateId.Petting, 0.95f, 3f);

            Assert.Greater(bridge.GetWeight(CreatureStateId.Petting), 0.95f,
                "Petting weight never converged; the bridge is not animating.");
            Assert.AreEqual(CreatureStateId.Petting, bridge.DominantState,
                "DominantState should follow the damped blend, not the raw state.");
        }

        /// <summary>
        /// Spins frames until <paramref name="weight"/> reaches
        /// <paramref name="threshold"/>, or the realtime budget runs out.
        /// </summary>
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

        [UnityTest]
        public IEnumerator BridgeSurvivesMissingAnimatorAndController()
        {
            // No Animator and no controller exist in this project yet, so this is
            // the normal configuration today. The bridge must track weights
            // regardless and must not throw or log an error.
            _go = new GameObject("BridgeTest");
            var machine = _go.AddComponent<CreatureStateMachine>();
            var bridge = _go.AddComponent<CreatureAnimationBridge>();
            yield return null;

            Assert.IsFalse(bridge.IsDrivingAnimator,
                "No controller is assigned, so the bridge must report it is not driving.");
            Assert.IsNotNull(bridge, "Bridge must exist with no Animator present.");

            machine.enabled = false;
            machine.ForceTransitionTo(CreatureStateId.AbilityUnlock);
            yield return WaitForWeight(bridge, CreatureStateId.AbilityUnlock, 0.95f, 3f);

            Assert.Greater(bridge.GetWeight(CreatureStateId.AbilityUnlock), 0.95f,
                "Weights must be tracked even with no Animator, or the component " +
                "cannot be tested or debugged before art exists.");
        }

        [UnityTest]
        public IEnumerator BridgeIsAllocationFreePerFrame()
        {
            _go = new GameObject("BridgeTest");
            _go.AddComponent<CreatureStateMachine>();
            _go.AddComponent<CreatureAnimationBridge>();
            yield return null;

            for (var i = 0; i < 30; i++)
            {
                yield return null;
            }

            var recorder = ProfilerRecorder.StartNew(
                ProfilerCategory.Memory, "GC Allocated In Frame");
            if (!recorder.Valid)
            {
                recorder.Dispose();
                Assert.Inconclusive("GC counter unavailable.");
                yield break;
            }

            var total = 0L;
            for (var i = 0; i < 120; i++)
            {
                yield return null;
                total += recorder.LastValue;
            }

            recorder.Dispose();

            // Same caveat as the state machine allocation tests: this counts the
            // editor's whole frame, so the ceiling is a noise margin, not a
            // precise budget. A per-frame string concat for the trigger or
            // parameter names would be hundreds of bytes and fail loudly.
            Assert.Less(total / 120f, 512f,
                "Bridge update path allocated " + (total / 120f) + " bytes/frame.");
        }

        #endregion

        #region FX Spawner

        [UnityTest]
        public IEnumerator SpawnerBuildsPoolAndSurvivesEmitting()
        {
            _go = new GameObject("SpawnerTest");
            _go.AddComponent<CreatureStateMachine>();
            var spawner = _go.AddComponent<CreatureFXSpawner>();
            yield return null;

            Assert.IsTrue(spawner.IsReady, "Pool must be built on Awake.");
            Assert.AreEqual(0, spawner.TotalBursts);

            // Every kind, at a position with no collider under it. A burst must
            // still emit: the pool is positioned explicitly, so a miss against
            // the world must not be treated as a reason to drop the effect.
            spawner.Emit(CreatureFXSpawner.BurstKind.InkSpread, Vector3.zero, Vector3.up);
            spawner.Emit(CreatureFXSpawner.BurstKind.FloatingHeart, Vector3.one, Vector3.up);
            spawner.Emit(CreatureFXSpawner.BurstKind.SuccessFlourish, Vector3.up, Vector3.up);

            Assert.AreEqual(3, spawner.TotalBursts,
                "All three burst kinds must emit without a mesh hit.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator SpawnerEmissionIsAllocationFree()
        {
            _go = new GameObject("SpawnerTest");
            _go.AddComponent<CreatureStateMachine>();
            var spawner = _go.AddComponent<CreatureFXSpawner>();
            yield return null;

            for (var i = 0; i < 30; i++)
            {
                yield return null;
            }

            var recorder = ProfilerRecorder.StartNew(
                ProfilerCategory.Memory, "GC Allocated In Frame");
            if (!recorder.Valid)
            {
                recorder.Dispose();
                Assert.Inconclusive("GC counter unavailable.");
                yield break;
            }

            // Measure the burst path, not just the idle Update. This is where an
            // implementation typically breaks the contract: setting
            // main.startColor per burst, or calling GetComponent in the emit path,
            // allocates and would not show up in an idle-only measurement.
            var total = 0L;
            const int Bursts = 300;

            for (var i = 0; i < Bursts; i++)
            {
                spawner.Emit(CreatureFXSpawner.BurstKind.SuccessFlourish, Vector3.zero, Vector3.up);
                total += recorder.LastValue;
            }

            recorder.Dispose();

            Assert.Less(total / Bursts, 512f,
                "Burst path allocated " + (total / Bursts) + " bytes/burst. " +
                "The pool exists precisely so this is zero.");
        }

        [UnityTest]
        public IEnumerator SpawnerDoesNotGrowUnderSustainedEmission()
        {
            // The pool must be bounded. Round-robin over a fixed array means a
            // child scrubbing faster than bursts decay truncates old bursts
            // rather than growing the pool, so the child count of ParticleSystem
            // components must stay constant.
            _go = new GameObject("SpawnerTest");
            _go.AddComponent<CreatureStateMachine>();
            var spawner = _go.AddComponent<CreatureFXSpawner>();
            yield return null;

            var before = CountEmitters(_go);

            for (var i = 0; i < 500; i++)
            {
                spawner.Emit(CreatureFXSpawner.BurstKind.InkSpread, Vector3.zero, Vector3.up);
            }

            yield return null;

            Assert.AreEqual(before, CountEmitters(_go),
                "Emitter count changed under sustained emission; the pool is growing.");
        }

        [UnityTest]
        public IEnumerator SpawnerEmitsEveryBurstNotJustTheFirstPoolWorth()
        {
            // Regression guard. The pool arrays are fixed at the maximum size so
            // they are never resized, but only _poolSizePerKind entries are
            // actually built. Cycling on the array length therefore walked into
            // nulls and silently dropped a burst for every slot past the last
            // built one, which showed up as "the first few ink bursts work, then
            // the effect stops" rather than as an error.
            //
            // Emitting well past the pool size and checking every burst was
            // accepted catches the wraparound without depending on the private
            // cursor state.
            _go = new GameObject("SpawnerTest");
            _go.AddComponent<CreatureStateMachine>();
            var spawner = _go.AddComponent<CreatureFXSpawner>();
            yield return null;

            // The default pool is 6 per kind; 40 bursts cycles it several times
            // over, so a length-based cursor is guaranteed to hit nulls.
            const int Bursts = 40;

            for (var i = 0; i < Bursts; i++)
            {
                spawner.Emit(CreatureFXSpawner.BurstKind.InkSpread, Vector3.zero, Vector3.up);
            }

            Assert.AreEqual(Bursts, spawner.TotalBursts,
                "Some bursts were dropped. Every Emit on a built pool must be " +
                "accepted; the round-robin must stay inside the built prefix.");

            yield return null;
        }

        private static int CountEmitters(GameObject root)
        {
            var count = 0;
            var all = root.GetComponentsInChildren<ParticleSystem>(true);
            for (var i = 0; i < all.Length; i++)
            {
                count++;
            }

            return count;
        }

        #endregion

        #region Shader Contract

        [Test]
        public void CreatureCanvasShaderDeclaresReliefStripPointAndSpecDefaults()
        {
            // Structural check on the shader. The art bible and palette.json put
            // real numeric requirements on this shader, and those requirements are
            // invisible to a C# test otherwise. A shader edit that drops the strip
            // keyword or moves the relief outside the specified band would
            // otherwise pass the entire suite.
            var shader = Shader.Find("Dab/Paint/CreatureCanvas");
            Assert.IsNotNull(shader, "CreatureCanvas shader not found.");

            var names = new System.Collections.Generic.HashSet<string>();
            var count = shader.GetPropertyCount();
            for (var i = 0; i < count; i++)
            {
                names.Add(shader.GetPropertyName(i));
            }

            Assert.IsTrue(names.Contains("_ReliefHeight"),
                "_ReliefHeight missing; tactile relief is the accessibility carrier " +
                "for palette.json C3 and C4.");
            Assert.IsTrue(names.Contains("_ReliefFeather"),
                "_ReliefFeather missing; C4 specifies a 0.06 H feathered crayon edge.");
            Assert.IsTrue(names.Contains("_SheenColor") && names.Contains("_SheenStrength"),
                "Sheen properties missing; the sheen is the achromatic value channel " +
                "that survives desaturation.");
            Assert.IsTrue(names.Contains("_PaintMap"),
                "_PaintMap missing; CreaturePaintController binds the paint texture here.");
        }

        [Test]
        public void CreatureCanvasReliefDefaultSitsInsideTheSpecifiedBand()
        {
            // palette.json C3 and C4 both specify 0.02-0.04 H relief. The default
            // must sit inside that band: below it the relief is not a usable
            // carrier, above it the surface self-shadows on an unauthored curve.
            var shader = Shader.Find("Dab/Paint/CreatureCanvas");
            Assert.IsNotNull(shader);

            var index = -1;
            var count = shader.GetPropertyCount();
            for (var i = 0; i < count; i++)
            {
                if (shader.GetPropertyName(i) == "_ReliefHeight")
                {
                    index = i;
                    break;
                }
            }

            Assert.GreaterOrEqual(index, 0, "_ReliefHeight not found.");

            // The declared default lives in the shader source, not in
            // material.GetFloat, which would read the currently-assigned value
            // rather than the authored default. Read it from source.
            var path = System.IO.Path.Combine(
                System.IO.Directory.GetParent(Application.dataPath).FullName,
                "Assets", "Shaders", "Paint", "CreatureCanvas.shader");

            Assert.IsTrue(System.IO.File.Exists(path),
                "Could not locate CreatureCanvas.shader at " + path);

            var source = System.IO.File.ReadAllText(path);
            const string marker = "_ReliefHeight (\"Relief Height\"";
            var start = source.IndexOf(marker, System.StringComparison.Ordinal);
            Assert.Greater(start, 0, "_ReliefHeight declaration not found in source.");

            // Take the rest of the declaration's own line. Scanning forward for
            // the next ")" instead would run past the "= 0.035" into the
            // following property's Range(...) and parse garbage.
            var lineEnd = source.IndexOf('\n', start);
            if (lineEnd < 0)
            {
                lineEnd = source.Length;
            }

            var declaration = source.Substring(start, lineEnd - start);
            var equals = declaration.IndexOf("=");
            Assert.Greater(equals, 0, "No default assigned to _ReliefHeight.");

            var literal = declaration.Substring(equals + 1)
                .Trim()
                .TrimEnd(';')
                .Trim();

            float value;
            var parsed = float.TryParse(
                literal,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out value);

            Assert.IsTrue(parsed,
                "Could not parse _ReliefHeight default from: " + declaration);

            Assert.GreaterOrEqual(value, 0.02f,
                "Relief default " + value + " is below the 0.02-0.04 H band in " +
                "palette.json C3/C4.");
            Assert.LessOrEqual(value, 0.04f,
                "Relief default " + value + " exceeds the 0.02-0.04 H band in " +
                "palette.json C3/C4; it will self-shadow on an unauthored curve.");
        }

        [Test]
        public void CreatureCanvasShaderCompilesWithoutErrors()
        {
            // The definitive compile check. Everything else in this fixture reads
            // the shader as text, which cannot tell a compiling shader from a
            // broken one: a bad HLSL block still reports its properties, still
            // answers Shader.Find, and still passes a structural assertion.
            //
            // This has caught nothing so far, which is the point. It is here so
            // that the next person to add relief or a pass gets a red test
            // instead of a shader that silently renders magenta in the editor
            // and a black silhouette in a build.
#if UNITY_EDITOR
            var shader = Shader.Find("Dab/Paint/CreatureCanvas");
            Assert.IsNotNull(shader, "CreatureCanvas shader not found.");

            var messages = UnityEditor.ShaderUtil.GetShaderMessages(shader);
            var errors = new System.Text.StringBuilder();

            for (var i = 0; i < messages.Length; i++)
            {
                var message = messages[i];
                if (message.severity == UnityEditor.Rendering.ShaderCompilerMessageSeverity.Error)
                {
                    errors.AppendLine(
                        "  line " + message.line + ": " + message.message +
                        (string.IsNullOrEmpty(message.messageDetails)
                            ? ""
                            : " | " + message.messageDetails.Trim()));
                }
            }

            Assert.IsTrue(
                errors.Length == 0,
                "CreatureCanvas.shader failed to compile:" +
                Environment.NewLine + errors);
#endif
        }

        [Test]
        public void CreatureCanvasDeclaresTheReliefStripPointInEveryPassThatUsesIt()
        {
            // Every pass below branches on _CREATURE_RELIEF, but a
            // #if defined(KEYWORD) with no matching shader_feature pragma is
            // dead code: Unity never defines the keyword, so the branch is
            // always false and all the relief work silently disappears while
            // the shader still compiles and still reports its properties. A
            // Toggle property drawer does not create the variant.
            //
            // The keyword has to be declared in every pass that reads it, not
            // just the forward pass, or the shadow and depth passes would
            // displace along a different code path than the lit surface.
            var path = System.IO.Path.Combine(
                System.IO.Directory.GetParent(Application.dataPath).FullName,
                "Assets", "Shaders", "Paint", "CreatureCanvas.shader");

            var source = System.IO.File.ReadAllText(path);
            var passOpens = System.Text.RegularExpressions.Regex.Matches(source, @"\bPass\s*\{");
            Assert.GreaterOrEqual(passOpens.Count, 3, "Expected three passes.");

            var openings = new System.Collections.Generic.List<int>();
            foreach (System.Text.RegularExpressions.Match m in passOpens)
            {
                openings.Add(m.Index);
            }

            var declaresVariant = 0;
            var usesKeyword = 0;

            for (var i = 0; i < openings.Count; i++)
            {
                var start = openings[i];
                var end = i + 1 < openings.Count ? openings[i + 1] : source.Length;
                var body = source.Substring(start, end - start);

                var pragma = body.Contains(
                    "#pragma shader_feature_local _ _CREATURE_RELIEF");

                if (pragma)
                {
                    declaresVariant++;
                }

                // Ignore the property declaration, which lives in Properties
                // before the first pass and is not inside any pass body.
                if (body.Contains("#if defined(_CREATURE_RELIEF)"))
                {
                    usesKeyword++;

                    Assert.IsTrue(pragma,
                        "A pass reads _CREATURE_RELIEF but does not declare " +
                        "#pragma shader_feature_local _ _CREATURE_RELIEF, so the " +
                        "relief branch compiles out and never runs.");
                }
            }

            Assert.Greater(usesKeyword, 0, "No pass uses _CREATURE_RELIEF at all.");
            Assert.GreaterOrEqual(declaresVariant, 3,
                "Expected the relief strip point in all " + usesKeyword +
                " passes that branch on it; found " + declaresVariant + ".");
        }

        [Test]
        public void CreatureCanvasShadowAndDepthPassesDisplaceLikeTheForwardPass()
        {
            // A vertex-displacing forward pass with a non-displacing ShadowCaster
            // casts the shadow of the undisplaced silhouette, which reads as a
            // lighting bug rather than a missing pass. So the check is not just
            // that the passes exist, but that each one applies the same relief
            // displacement as the forward pass.
            //
            // This asserts on the shader source rather than Shader.passCount.
            // Under -batchmode -nographics the runtime only registers the first
            // pass (passCount reports 1 and LightMode comes back empty), so the
            // compiled-asset API cannot see these passes at all. The source is
            // also the stronger assertion: the numeric API can only confirm a
            // pass name, whereas the source shows whether the displacement is
            // actually wired into it.
            var path = System.IO.Path.Combine(
                System.IO.Directory.GetParent(Application.dataPath).FullName,
                "Assets", "Shaders", "Paint", "CreatureCanvas.shader");

            Assert.IsTrue(System.IO.File.Exists(path),
                "Could not locate CreatureCanvas.shader at " + path);

            var source = System.IO.File.ReadAllText(path);

            // Bound regions by the pass-open token itself, not by the substring
            // "Pass". A naive scan for "Pass" matches entry points such as
            // ShadowPassVertex, which would truncate the region before the
            // displacement and produce a false failure.
            var passOpens = System.Text.RegularExpressions.Regex.Matches(source, @"\bPass\s*\{");

            Assert.GreaterOrEqual(passOpens.Count, 3,
                "Expected UniversalForward, ShadowCaster and DepthOnly passes; " +
                "found " + passOpens.Count + " in CreatureCanvas.shader.");

            AssertPassDisplaces(source, passOpens, "UniversalForward", "forward");
            AssertPassDisplaces(source, passOpens, "ShadowCaster", "shadow");
            AssertPassDisplaces(source, passOpens, "DepthOnly", "depth");
        }

        private static void AssertPassDisplaces(
            string source,
            System.Collections.ICollection passOpens,
            string lightMode,
            string label)
        {
            var tag = "\"LightMode\" = \"" + lightMode + "\"";
            var openings = new System.Collections.Generic.List<int>();
            foreach (System.Text.RegularExpressions.Match m in passOpens)
            {
                openings.Add(m.Index);
            }

            for (var i = 0; i < openings.Count; i++)
            {
                var start = openings[i];
                var end = i + 1 < openings.Count ? openings[i + 1] : source.Length;

                var body = source.Substring(start, end - start);

                if (!body.Contains(tag))
                {
                    continue;
                }

                Assert.IsTrue(body.Contains("_ReliefHeight"),
                    "The " + label + " pass does not read _ReliefHeight, so it " +
                    "will not displace. The forward pass moves vertices, so a " +
                    "shadow or depth pass that does not will disagree with the " +
                    "lit silhouette.");

                return;
            }

            Assert.Fail("No " + lightMode + " pass declared in CreatureCanvas.shader.");
        }

        #endregion
    }
}
