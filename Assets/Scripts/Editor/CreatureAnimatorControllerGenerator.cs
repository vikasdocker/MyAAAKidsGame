using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Dab.Editor
{
    /// <summary>
    /// Generates the master creature AnimatorController plus clearly-marked
    /// placeholder clips, so <see cref="Dab.Runtime.Creature.CreatureAnimationBridge"/>
    /// can bind natively instead of running unbound.
    ///
    /// Spec the generator obeys (art-bible 2.x / palette.json)
    /// --------------------------------------------------------
    ///   - One rig, one layer set: "Base" + an additive "Echo" layer. Not five
    ///     per-state layers (art-bible animation agreement).
    ///   - Base layer carries five state nodes, one per CreatureStateId:
    ///     Idle -&gt; Rest_Happy, Painting -&gt; Absorb_Painting,
    ///     Petting -&gt; Focus_Petting, Feeding -&gt; Greet_Return,
    ///     AbilityUnlock -&gt; Proud_Flourish.
    ///   - Each state's Motion is a 1D blend tree keyed on that state's own
    ///     <c>State_&lt;id&gt;</c> float: threshold 0 is the neutral baseline,
    ///     threshold 1 is the state's signature clip. The bridge's damped float
    ///     therefore drives per-state expression, not just the mode switch.
    ///   - Mode switching uses a fully-connected set of transitions, each gated
    ///     on <c>Trigger_&lt;id&gt;</c>. Fully-connected rather than AnyState
    ///     because Mecanim does not allow Trigger conditions on AnyState
    ///     transitions, and the bridge may jump any state to any state.
    ///   - Echo is an additive layer whose weight the bridge drives from the
    ///     painting state weight; it has a single additive placeholder state.
    ///
    /// Placeholder clips
    /// -----------------
    /// Real animation is a closed set of 20 authored clips (art-bible). These
    /// generated clips are temporary stand-ins so states have motion now; every
    /// one is prefixed <c>zz_placeholder_</c> so it can never be mistaken for a
    /// real clip, and the controller is regenerated wholesale when the real set
    /// lands.
    ///
    /// Menu: Tools/Dab/Art/Generate Creature Animator
    /// Headless: -executeMethod Dab.Editor.CreatureAnimatorControllerGenerator.GenerateAll
    /// </summary>
    public static class CreatureAnimatorControllerGenerator
    {
        // Both live under a Resources folder so the runtime rig can reach the
        // controller with Resources.Load and no serialized reference (no
        // prefabs exist in this project to carry one).
        private const string ControllerPath =
            "Assets/Art/Animation/Resources/CreatureAnimatorController.controller";

        private const string ClipDirectory = "Assets/Art/Animation/Resources/Placeholders";
        private const string NeutralClipName = "zz_placeholder_Neutral.anim";
        private const string EchoClipName = "zz_placeholder_Echo.anim";

        /// <summary>Parallel to CreatureStateId: Idle, Painting, Petting, Feeding, AbilityUnlock.</summary>
        private static readonly string[] StateIds =
            { "Idle", "Painting", "Petting", "Feeding", "AbilityUnlock" };

        // Placeholder names, not the real closed-set clip names.
        private static readonly string[] StateClipNames =
        {
            "zz_placeholder_Rest_Happy.anim",
            "zz_placeholder_Absorb_Painting.anim",
            "zz_placeholder_Focus_Petting.anim",
            "zz_placeholder_Greet_Return.anim",
            "zz_placeholder_Proud_Flourish.anim"
        };

        // Loop period and scale-pulse amplitude per placeholder, roughly in the
        // spirit of the closed set's stated durations.
        private static readonly float[] StatePeriods = { 4.0f, 2.0f, 1.6f, 1.2f, 2.5f };
        private static readonly float[] StateAmplitudes = { 0.010f, 0.014f, 0.010f, 0.012f, 0.020f };

        [MenuItem("Tools/Dab/Art/Generate Creature Animator")]
        public static void GenerateAll()
        {
            EnsureFolder("Assets/Art/Animation/Resources");
            EnsureFolder(ClipDirectory);

            var neutral = CreatePlaceholderClip(
                ClipDirectory, NeutralClipName, 4.0f, 0.015f);
            var echoClip = CreatePlaceholderClip(
                ClipDirectory, EchoClipName, 1.6f, 0.005f);

            var stateClips = new AnimationClip[StateClipNames.Length];
            for (var i = 0; i < StateClipNames.Length; i++)
            {
                stateClips[i] = CreatePlaceholderClip(
                    ClipDirectory, StateClipNames[i], StatePeriods[i], StateAmplitudes[i]);
            }

            var controller = BuildController(stateClips, neutral, echoClip);

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                "[CreatureAnimatorControllerGenerator] Generated '" + ControllerPath +
                "' with Base + Echo layers and " + stateClips.Length + " state nodes.");
        }

        private static AnimatorController BuildController(
            AnimationClip[] stateClips, AnimationClip neutral, AnimationClip echoClip)
        {
            var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);

            // Parameters the bridge writes. Names must match the bridge's
            // constants verbatim: State_<id> floats, Trigger_<id> triggers,
            // TransitionSpeed float.
            controller.AddParameter("TransitionSpeed", AnimatorControllerParameterType.Float);
            for (var i = 0; i < StateIds.Length; i++)
            {
                controller.AddParameter(
                    "State_" + StateIds[i], AnimatorControllerParameterType.Float);
            }
            for (var i = 0; i < StateIds.Length; i++)
            {
                controller.AddParameter(
                    "Trigger_" + StateIds[i], AnimatorControllerParameterType.Trigger);
            }

            // Base layer.
            var layers = controller.layers;
            layers[0].name = "Base";
            layers[0].defaultWeight = 1f;
            controller.layers = layers;

            var baseMachine = controller.layers[0].stateMachine;
            baseMachine.name = "Base";
            baseMachine.RemoveUnexpectedStates(StateClipNames);

            var states = new AnimatorState[StateIds.Length];
            for (var i = 0; i < StateIds.Length; i++)
            {
                var stateName = StateClipNames[i]
                    .Replace(".anim", string.Empty)
                    .Replace("zz_placeholder_", string.Empty);

                var state = baseMachine.AddState(stateName);
                state.name = stateName;

                // A BlendTree created with `new` is a freestanding object; it
                // must live inside the controller asset or it is dropped on
                // save and the state silently loses its motion.
                var stateTree = CreateStateBlendTree(
                    stateName, stateClips[i], neutral, StateIds[i]);
                AssetDatabase.AddObjectToAsset(stateTree, controller);
                state.motion = stateTree;

                states[i] = state;
            }

            baseMachine.defaultState = states[0];

            // Fully-connected trigger graph: every state can transition to every
            // other state on that target's trigger.
            for (var s = 0; s < states.Length; s++)
            {
                for (var t = 0; t < states.Length; t++)
                {
                    if (s == t)
                    {
                        continue;
                    }

                    var transition = states[s].AddTransition(states[t]);
                    transition.hasExitTime = false;
                    transition.canTransitionToSelf = false;
                    transition.duration = 0.45f;
                    transition.AddCondition(
                        AnimatorConditionMode.If, 0f, "Trigger_" + StateIds[t]);
                }
            }

            // Echo additive layer. AddLayer must receive a ready state machine; unlike
            // the seeded layer 0, a layer added via the struct API has no
            // state machine of its own and must own one before it is attached.
            var echoMachine = new AnimatorStateMachine { name = "Echo" };
            AssetDatabase.AddObjectToAsset(echoMachine, controller);

            controller.AddLayer(new AnimatorControllerLayer
            {
                name = "Echo",
                defaultWeight = 0f,
                blendingMode = AnimatorLayerBlendingMode.Additive,
                stateMachine = echoMachine
            });

            echoMachine.RemoveUnexpectedStates(new[] { EchoClipName });

            var echoState = echoMachine.AddState("Echo_Painting");
            echoState.name = "Echo_Painting";
            echoState.motion = echoClip;
            echoMachine.defaultState = echoState;

            return controller;
        }

        private static BlendTree CreateStateBlendTree(
            string stateName, AnimationClip signature, AnimationClip neutral, string stateId)
        {
            var tree = new BlendTree
            {
                name = "Blend_" + stateName,
                blendParameter = "State_" + stateId
            };

            // threshold 0 = neutral baseline (calm), threshold 1 = the signature
            // clip at full strength. The bridge's damped State_<id> float drives
            // the same cross-fade the layer blend would, so state changes inside
            // the tree can never pop.
            tree.AddChild(neutral, 0f);
            tree.AddChild(signature, 1f);
            return tree;
        }

        private static AnimationClip CreatePlaceholderClip(
            string directory, string fileName, float period, float amplitude)
        {
            var clip = new AnimationClip();
            clip.name = Path.GetFileNameWithoutExtension(fileName);
            clip.frameRate = 30;

            // One subtle localScale.y pulse so the state visibly breathes. The
            // path is the root: the Animator drives the creature's root object,
            // so an empty path keeps the placeholder valid for any hierarchy.
            var curve = new AnimationCurve(
                new Keyframe(0f, 1f - amplitude),
                new Keyframe(period * 0.5f, 1f + amplitude),
                new Keyframe(period, 1f - amplitude));
            curve.preWrapMode = WrapMode.Loop;
            curve.postWrapMode = WrapMode.Loop;
            clip.SetCurve("", typeof(Transform), "m_LocalScale.y", curve);

            var settings = new AnimationClipSettings { loopTime = true };
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            var path = Path.Combine(directory, fileName).Replace('\\', '/');
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(clip, path);
            return clip;
        }

        private static void EnsureFolder(string path)
        {
            if (path == "Assets" || AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }

    internal static class AnimatorStateMachineExtensions
    {
        /// <summary>
        /// CreateAnimatorControllerAtPath seeds a machine with a default state
        /// ("New State"); the generator must not ship a junk state with no
        /// motion. Removes every state whose name is not in <paramref name="allowed"/>
        /// before callers add theirs.
        /// </summary>
        public static void RemoveUnexpectedStates(
            this AnimatorStateMachine machine, string[] allowed)
        {
            var states = machine.states;
            for (var i = 0; i < states.Length; i++)
            {
                var name = states[i].state.name;
                var keep = false;
                for (var a = 0; a < allowed.Length; a++)
                {
                    var expected = Path.GetFileNameWithoutExtension(allowed[a]);
                    if (expected == name)
                    {
                        keep = true;
                        break;
                    }
                }

                if (!keep)
                {
                    machine.RemoveState(states[i].state);
                }
            }
        }
    }
}