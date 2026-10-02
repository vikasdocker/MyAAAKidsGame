using System.Text;
using UnityEngine;

namespace Dab.Runtime.Creature
{
    /// <summary>
    /// A live paint stroke. art-bible 2.4: "I made this." Absorbed, silky focus.
    ///
    /// Why this state owns an uninterruptible gesture
    /// -----------------------------------------------
    /// A paint stroke is the one interaction where a mid-gesture interruption
    /// destroys work rather than merely ending a reaction. A child who is halfway
    /// through a stroke, taps the creature's ear by accident, and has the stroke
    /// torn out from under them learns that painting is unreliable. That is the
    /// opposite of the confidence the input manager is built to create, so this
    /// state refuses to transition until the finger lifts.
    ///
    /// The refusal is <see cref="TransitionResult.BlockedBusy"/>, not
    /// <see cref="TransitionResult.BlockedInvalid"/>, because the gesture does
    /// end on its own. A caller that is willing to wait can retry.
    ///
    /// art-bible 2.4 also fixes the world response: environment value -1 step
    /// and saturation x0.75, "so the world steps back and the paint steps
    /// forward". That is the environment's business, not this state's, but the
    /// multiplier is recorded here so the value is not re-derived downstream.
    /// </summary>
    public sealed class PaintingState : CreatureState
    {
        /// <summary>art-bible 2.4: environment saturation x0.75 while painting.</summary>
        public const float EnvironmentSaturationMultiplier = 0.75f;

        /// <summary>art-bible 2.4: environment drops one value rung while painting.</summary>
        public const int EnvironmentValueRungDelta = -1;

        /// <summary>art-bible 5.x: Absorb_Painting is a 2.0 s animation.</summary>
        private const float AbsorbSeconds = 2f;

        private float _strokeSeconds;
        private float _peakSpeed;
        private bool _strokeActive;

        /// <summary>
        /// The owning machine, cached at Initialize. Held so the per-frame path
        /// can read smoothed touch speed without a GetComponent, and so the state
        /// agrees with the machine about what the gesture is doing rather than
        /// re-deriving it.
        /// </summary>
        private CreatureStateMachine _machine;

        public override CreatureStateId Id => CreatureStateId.Painting;

        public override string DisplayName => "Painting";

        public override void Initialize(CreatureStateMachine machine)
        {
            _machine = machine;
        }

        public override void OnEnter()
        {
            _strokeSeconds = 0f;
            _peakSpeed = 0f;
            _strokeActive = true;

            // art-bible 2.4 energy "spiking to 7 on each stroke start, never
            // pausing", so the entry is the spike.
            OnAnimatorTrigger("Absorb_Painting");
            OnAnimatorFloat("EnvironmentSaturation", EnvironmentSaturationMultiplier);
            OnAnimatorBool("IsWet", true);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!_strokeActive)
            {
                return;
            }

            // Null-tolerant so the state is unit-testable in isolation, with no
            // machine and therefore no touch-speed source. The machine always
            // calls Initialize, so this is a test-only path.
            if (_machine != null)
            {
                // Peak is sticky because the art bible wants the creature to
                // respond to how vigorous the stroke was, not to how it happens
                // to be moving at this instant. A fast flick that has already
                // slowed to a stop should still read as a confident stroke.
                _peakSpeed = Mathf.Max(_peakSpeed, _machine.TouchSpeedInchesPerSecond);
            }

            // Normalised against a brisk-but-plausible child stroke. Deliberately
            // not a physical maximum: the point is a legible 0-1 for the
            // Animator, not a measurement.
            var intensity = Mathf.Clamp01(_peakSpeed / 8f);
            OnAnimatorFloat("StrokeIntensity", intensity);

            // art-bible 2.4: "Paint should appear slightly before the finger; the
            // child is never asked to aim." Push the dab lead slightly ahead of
            // the reported position. A fixed small lead in screen space, not a
            // fraction of the stroke, so it stays constant at any finger speed.
            OnAnimatorFloat("PaintLeadPixels", 6f);

            if (_strokeSeconds >= AbsorbSeconds)
            {
                OnAnimatorTrigger("PaintSettle");
            }
        }

        public override void OnTouchEnded(Vector2 screenPosition, bool wasTap)
        {
            // The stroke is over, so this state no longer owns a gesture and can
            // be left. If the child lifted without this ever running, the machine
            // is still in Painting with a stuck stroke; this is the only exit.
            _strokeActive = false;
        }

        public override void OnExit()
        {
            _strokeActive = false;
            OnAnimatorBool("IsWet", false);
        }

        public override TransitionResult CanTransitionTo(CreatureStateId target, CreatureStateMachine machine)
        {
            // Petting is the one target that is not merely blocked but would be
            // actively wrong: a moving finger mid-stroke is painting, and reading
            // it as petting would make the creature respond to its own dabs.
            if (target == CreatureStateId.Petting)
            {
                return TransitionResult.BlockedBusy;
            }

            return _strokeActive
                ? TransitionResult.BlockedBusy
                : TransitionResult.Allowed;
        }

        public override void Describe(StringBuilder builder)
        {
            builder.Append(DisplayName)
                   .Append(_strokeActive ? " stroking" : " settling")
                   .Append(" peakSpeed=").Append(_peakSpeed.ToString("F2"));
        }
    }
}
