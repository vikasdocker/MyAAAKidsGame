using System.Text;
using Dab.Runtime.Abilities;
using UnityEngine;

namespace Dab.Runtime.Creature
{
    /// <summary>
    /// The signature mark has been bound to a real ability and the creature
    /// performs it. game-concept.md 5-minute loop step 3, "Echo", and step 4,
    /// "Show".
    ///
    /// This is the state the whole loop exists to reach. Steps 1 and 2 (paint,
    /// then choose a mark) are only worth doing because this fires, and the
    /// concept doc is blunt that without it the game is "a sandbox with no
    /// consequence". So the behaviour here is treated as load-bearing rather
    /// than as a flourish.
    ///
    /// Timing is taken from the art bible rather than invented:
    ///   art-bible 2.5   one 2.5 s peak, then 3.0 s slow decay to Ambient
    ///   art-bible 5.x   Proud_Flourish is 2.5 s
    ///
    /// The two rules that matter most here, both from art-bible 2.5:
    ///   - "one 2.5 s peak, then 3.0 s slow decay. Never a second peak without a
    ///     new mark." So this state refuses to re-trigger while it is running,
    ///     and the machine will not let it interrupt itself.
    ///   - the 0.15 s light-pop is "deliberate theatre, not error", so the
    ///     brightness spike below is intentional and must not be smoothed away.
    ///
    /// Why it is a state and not a one-shot animation event: it is the only state
    /// that represents the creature *actively using* something the child made, so
    /// it has to be able to block competing input for its duration. A child who
    /// pets the creature mid-flourish and cancels the payoff has lost the moment
    /// the 5-minute loop is built to deliver.
    /// </summary>
    public sealed class AbilityUnlockState : CreatureState
    {
        /// <summary>art-bible 2.5 / 5.x: Proud_Flourish is 2.5 s.</summary>
        public const float FlourishSeconds = 2.5f;

        /// <summary>art-bible 2.5: 3.0 s slow decay back to Ambient.</summary>
        public const float DecaySeconds = 3f;

        /// <summary>art-bible 2.5: the 0.15 s light-pop is deliberate theatre.</summary>
        private const float LightPopSeconds = 0.15f;

        /// <summary>Multiplier at the peak of the pop. Above 1.0, on purpose.</summary>
        private const float LightPopIntensity = 1.35f;

        private float _elapsed;
        private bool _popFired;
        private bool _flourishComplete;
        private CreatureStateMachine _machine;

        public override CreatureStateId Id => CreatureStateId.AbilityUnlock;

        public override string DisplayName => "AbilityUnlock";

        public override void Initialize(CreatureStateMachine machine)
        {
            _machine = machine;
        }

        /// <summary>
        /// The ability being performed. Gameplay identity is separate from
        /// Animator state identity; the MVP shares the Proud_Flourish animation.
        /// </summary>
        public CreatureAbilityId AbilityId { get; internal set; }

        public override void OnEnter()
        {
            _elapsed = 0f;
            _popFired = false;
            _flourishComplete = false;

            OnAnimatorTrigger("Proud_Flourish");

        }

        public override void OnUpdate(float deltaTime)
        {
            _elapsed += deltaTime;

            // The light pop fires once, at the top of the peak. Guarded rather
            // than scheduled by a timer so a frame drop cannot fire it twice or
            // skip it entirely.
            if (!_popFired && _elapsed >= LightPopSeconds)
            {
                _popFired = true;
                OnAnimatorFloat("FlourishLightPop", LightPopIntensity);
            }

            if (!_flourishComplete && _elapsed >= FlourishSeconds)
            {
                _flourishComplete = true;
                OnAnimatorTrigger("Flourish_Decay");
            }

            if (_elapsed >= FlourishSeconds + DecaySeconds)
            {
                // Self-terminate rather than relying on the machine's idle
                // timeout. art-bible 2.5 specifies a 3.0 s decay to Ambient, so
                // the return is part of this state's spec, not an afterthought.
                //
                // Routed through the machine rather than transitioned locally,
                // because a state cannot change which state is current without
                // going through the owner. Guarded for a null machine so the
                // state is still unit-testable in isolation.
                if (_machine != null)
                {
                    _machine.ReturnToIdle();
                }

                return;
            }

            // Decay curve. Held at peak for the flourish, then eased home over
            // the decay. The ease is on the decay leg only: art-bible 2.5 calls
            // the peak a deliberate 0.15 s pop, so smoothing across it would
            // soften exactly the moment that is supposed to land.
            var t = _elapsed <= FlourishSeconds
                ? 1f
                : 1f - Mathf.Clamp01((_elapsed - FlourishSeconds) / DecaySeconds);

            var eased = t * t * (3f - 2f * t); // smoothstep
            OnAnimatorFloat("FlourishIntensity", eased);
            OnAnimatorFloat("FlourishLightPop", _popFired ? Mathf.Lerp(LightPopIntensity, 0f, eased) : 0f);
        }

        public override TransitionResult CanTransitionTo(CreatureStateId target, CreatureStateMachine machine)
        {
            // art-bible 2.5: "Never a second peak without a new mark." While the
            // flourish is running, nothing may interrupt it, including another
            // request to perform an ability.
            return _flourishComplete
                ? TransitionResult.Allowed
                : TransitionResult.BlockedBusy;
        }

        public override void OnExit()
        {
            // Leave the creature fully at rest. A flourish that ends with a
            // residual light pop would make the world look like it is still
            // celebrating on the next frame.
            OnAnimatorFloat("FlourishIntensity", 0f);
            OnAnimatorFloat("FlourishLightPop", 0f);
        }

        public override void Describe(StringBuilder builder)
        {
            builder.Append(DisplayName)
                   .Append(" ability=").Append(AbilityId)
                   .Append(_flourishComplete ? " decaying" : " peak")
                   .Append(" t=").Append(_elapsed.ToString("F2"));
        }
    }
}
