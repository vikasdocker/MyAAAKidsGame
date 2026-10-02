using System.Text;
using UnityEngine;

namespace Dab.Runtime.Creature
{
    /// <summary>
    /// Response to a gentle, sustained touch. art-bible 2.3: a lean into the
    /// finger, squash proportional to stroke speed, ears flattening, eyes
    /// tracking, with the light explicitly unchanged so "the child's touch is
    /// the only event in frame".
    ///
    /// Why this is a state and not a per-frame reaction
    /// --------------------------------------------------
    /// art-bible R3 records the failure this design avoids: "Response
    /// proportional to input, not switched by a state machine" was an anti-pattern
    /// in a reference game, where response quality came from how hard you pushed
    /// rather than from attention. This is the opposite: the creature is
    /// *attentive* to petting, and the amount of lean tracks the gesture so the
    /// child can read their own influence on it.
    ///
    /// Why petting is interruptible and painting is not
    /// --------------------------------------------------
    /// Petting owns no persistent work. Interrupting it loses nothing, so it
    /// yields immediately. Painting owns a stroke, so it refuses. This asymmetry
    /// is the reason the two states behave so differently on the same touch.
    /// </summary>
    public sealed class PettingState : CreatureState
    {
        /// <summary>art-bible 5.x: Focus_Petting is a 1.6 s animation.</summary>
        private const float FocusSeconds = 1.6f;

        /// <summary>art-bible 5.x: Settle_Any is 0.32 s, the return to rest.</summary>
        private const float SettleSeconds = 0.32f;

        /// <summary>Lean in degrees at full stroke speed. art-bible 5.3 caps lean at 7 degrees.</summary>
        private const float MaxLeanDegrees = 7f;

        /// <summary>
        /// Speed that maps to a full lean. Above the petting threshold, so a
        /// gentle touch produces a visible but partial lean rather than nothing.
        /// </summary>
        private const float LeanSpeedReference = 1.6f;

        private const float EarsFlattenAmount = 0.5f;

        private float _elapsed;
        private float _settleElapsed;
        private float _leanDegrees;
        private float _squash;
        private bool _isSettling;

        private CreatureStateMachine _machine;

        public override CreatureStateId Id => CreatureStateId.Petting;

        public override string DisplayName => "Petting";

        public override void Initialize(CreatureStateMachine machine)
        {
            _machine = machine;
        }

        public override void OnEnter()
        {
            _elapsed = 0f;
            _isSettling = false;

            OnAnimatorTrigger("Focus_Petting");

            // art-bible 2.3 is explicit that petting does not move the world.
            // Asserted as a float so a future lighting layer has the value and
            // cannot quietly desaturate the scene during petting.
            OnAnimatorFloat("EnvironmentSaturation", 1f);
        }

        public override void OnUpdate(float deltaTime)
        {
            _elapsed += deltaTime;

            if (_isSettling)
            {
                UpdateSettle(deltaTime);
                return;
            }

            var speed = _machine != null ? _machine.TouchSpeedInchesPerSecond : 0f;

            // art-bible 2.3: "squash proportional to stroke speed". Squashing
            // *up* with speed rather than down, because a fast stroke is
            // excitement, and a creature that compresses when excited reads as
            // a landing preparation, not a happy one.
            var normalized = Mathf.Clamp01(speed / LeanSpeedReference);
            _leanDegrees = normalized * MaxLeanDegrees;
            _squash = normalized * 0.06f;

            OnAnimatorFloat("LeanDegrees", _leanDegrees);
            OnAnimatorFloat("Squash", _squash);
            OnAnimatorFloat("EarsFlatten", EarsFlattenAmount * normalized);

            if (_elapsed >= FocusSeconds)
            {
                OnAnimatorTrigger("Focus_Hold");
            }
        }

        public override void OnTouchEnded(Vector2 screenPosition, bool wasTap)
        {
            // The finger is gone, so the creature relaxes rather than snapping
            // upright. Settle_Any is 0.32 s per art-bible 5.x.
            _isSettling = true;
            _settleElapsed = 0f;
            OnAnimatorTrigger("Settle_Any");
        }

        private void UpdateSettle(float deltaTime)
        {
            _settleElapsed += deltaTime;

            var t = Mathf.Clamp01(_settleElapsed / SettleSeconds);

            // Ease back to rest rather than cutting. Linear decay to zero reads
            // as the animation being switched off; an ease-out reads as relaxing.
            var eased = 1f - (1f - t) * (1f - t);

            _leanDegrees = Mathf.Lerp(_leanDegrees, 0f, eased);
            _squash = Mathf.Lerp(_squash, 0f, eased);

            OnAnimatorFloat("LeanDegrees", _leanDegrees);
            OnAnimatorFloat("Squash", _squash);
            OnAnimatorFloat("EarsFlatten", EarsFlattenAmount * (1f - eased));
        }

        public override void OnExit()
        {
            // Guarantee a clean rest pose even if the state is left mid-settle,
            // so a rapid pet-then-paint does not strand the creature leaning.
            _isSettling = false;
            _leanDegrees = 0f;
            _squash = 0f;
        }

        public override TransitionResult CanTransitionTo(CreatureStateId target, CreatureStateMachine machine)
        {
            // Everything is permitted. Petting owns no work, so it yields rather
            // than blocking. Blocking here would make the creature feel sticky to
            // a child who is simply moving on to the next thing.
            return TransitionResult.Allowed;
        }

        public override void Describe(StringBuilder builder)
        {
            builder.Append(DisplayName)
                   .Append(_isSettling ? " settling" : " leaning")
                   .Append(" lean=").Append(_leanDegrees.ToString("F1"))
                   .Append("deg");
        }
    }
}
