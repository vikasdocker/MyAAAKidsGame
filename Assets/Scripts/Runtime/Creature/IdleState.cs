using System.Text;
using UnityEngine;

namespace Dab.Runtime.Creature
{
    /// <summary>
    /// The baseline. Every other state is defined as a measurable departure from
    /// this one (art-bible 2.3), which is why Idle accepts every transition: it
    /// holds no claim that needs protecting, so refusing would be arbitrary.
    ///
    /// art-bible 2.3: "I'm not going anywhere." Patient, breathing company. The
    /// rest state, the one moment that asks for nothing. 4 s breathing cycle
    /// with 6 s idle sway, deliberately offset so the two never sync.
    ///
    /// Note the art bible's own rule for this state: "light does not change.
    /// Only the creature changes". Nothing here touches lighting or camera.
    /// </summary>
    public sealed class IdleState : CreatureState
    {
        /// <summary>Art-bible 2.3: 4 s breathing cycle.</summary>
        private const float BreathingPeriodSeconds = 4f;

        /// <summary>Art-bible 2.3: 6 s idle sway, offset from the breath so they never sync.</summary>
        private const float SwayPeriodSeconds = 6f;

        /// <summary>Art-bible 2.3 energy 2/10, so a shallow breath.</summary>
        private const float BreathDepth = 0.04f;

        private const float SwayDepthDegrees = 1.5f;

        private const float TwoPi = 6.2831853f;

        private float _breathPhase;
        private float _swayPhase;

        public override CreatureStateId Id => CreatureStateId.Idle;

        public override string DisplayName => "Idle";

        public override void OnEnter()
        {
            // Phases are advanced rather than reset to zero on entry. Resetting
            // would snap the breath to its start every time the creature fell
            // back to idle, which is exactly the "puppet" read the art bible
            // tries to avoid. Continuity across a transition reads as a living
            // animal; a snap reads as a loop restarting.
            _breathPhase = (_breathPhase + 0.37f) % 1f;
            _swayPhase = (_swayPhase + 0.61f) % 1f;
        }

        public override void OnUpdate(float deltaTime)
        {
            _breathPhase = (_breathPhase + deltaTime / BreathingPeriodSeconds) % 1f;
            _swayPhase = (_swayPhase + deltaTime / SwayPeriodSeconds) % 1f;

            // Sine of a phase in [0,1) is one full cycle. Left unsmoothed: a sine
            // is already C1-continuous, so smoothing further would only add lag
            // between the child's action and the creature's response.
            OnAnimatorFloat("BreathDepth", Mathf.Sin(_breathPhase * TwoPi) * BreathDepth);
            OnAnimatorFloat("SwayDegrees", Mathf.Sin(_swayPhase * TwoPi) * SwayDepthDegrees);
        }

        public override void Describe(StringBuilder builder)
        {
            builder.Append(DisplayName)
                   .Append(" breath=")
                   .Append((Mathf.Sin(_breathPhase * TwoPi) * BreathDepth).ToString("F3"));
        }
    }
}
