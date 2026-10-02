using UnityEngine;

namespace Dab.Runtime.Creature
{
    /// <summary>
    /// Base class for every creature state.
    ///
    /// Allocation contract
    /// -------------------
    /// Instances are created once, in <see cref="CreatureStateMachine.Awake"/>,
    /// and reused for the lifetime of that machine. The machine never news up a
    /// state on a transition. This matters on the low-end Android floor device
    /// (art-bible 8.1): a state change that allocates produces a GC spike at
    /// exactly the moment the child is most likely to keep touching, which is
    /// where a dropped frame is most visible.
    ///
    /// Consequently every hook here is expected to be allocation-free in the
    /// steady state. In particular:
    ///   - do not use string concatenation or interpolation in <see cref="OnUpdate"/>,
    ///     it allocates every call;
    ///   - do not use LINQ, foreach over a List&lt;T&gt; (it boxes the enumerator),
    ///     or yield return;
    ///   - do not capture lambdas that outlive the call, they allocate a closure.
    /// Use the <c>StringBuilder</c> on the machine for any diagnostic text.
    ///
    /// Hook naming follows Unity's own convention: <c>On</c> for entry and exit,
    /// <see cref="OnUpdate"/> for the per-frame body.
    /// </summary>
    public abstract class CreatureState
    {
        #region Identity

        /// <summary>
        /// Stable identity of this state. Overridden by every concrete state and
        /// never inferred, so a subclass cannot accidentally report the wrong
        /// identity and make transitions unreachable.
        /// </summary>
        public abstract CreatureStateId Id { get; }

        /// <summary>
        /// Short human-readable name, used in diagnostics. Returned from a
        /// static string so this costs no allocation to read.
        /// </summary>
        public abstract string DisplayName { get; }

        #endregion

        #region Lifecycle

        /// <summary>
        /// Called once when the owning machine is enabled, before the first
        /// <see cref="OnEnter"/>. Use for one-time setup that does not depend on
        /// a particular transition, such as caching component references.
        /// </summary>
        public virtual void Initialize(CreatureStateMachine machine)
        {
        }

        /// <summary>
        /// Called when this state becomes current.
        ///
        /// Called exactly once per transition into this state, after the previous
        /// state's <see cref="OnExit"/> and before the first
        /// <see cref="OnUpdate"/>. This is where a state's entry animation and
        /// any one-shot audio should be triggered.
        /// </summary>
        public virtual void OnEnter()
        {
        }

        /// <summary>
        /// Called when this state stops being current, before the next state's
        /// <see cref="OnEnter"/>. Always paired with <see cref="OnEnter"/>, even
        /// when the machine is disabled, so a state can never leave a half-armed
        /// input subscription or a stuck timer behind.
        ///
        /// Not called when the machine is re-initialised without ever having
        /// entered this state.
        /// </summary>
        public virtual void OnExit()
        {
        }

        /// <summary>
        /// Per-frame body. Called only while this state is current.
        /// </summary>
        /// <param name="deltaTime">
        /// Unscaled seconds since the previous update. Unscaled deliberately:
        /// the creature keeps breathing while the game is paused mid-gesture, and
        /// art-bible 2.3 makes Idle Ambient a breathing baseline rather than a
        /// frozen one.
        /// </param>
        public virtual void OnUpdate(float deltaTime)
        {
        }

        #endregion

        #region Transitions

        /// <summary>
        /// Whether a transition to <paramref name="target"/> is currently legal.
        ///
        /// The machine calls this before every transition, including ones it
        /// requests itself, so an illegal transition is impossible to perform
        /// by accident. The default accepts everything, which is right for
        /// states with no exclusive claims (Idle) and wrong for states that own
        /// an uninterruptible gesture (Painting).
        /// </summary>
        /// <param name="target">The state being requested.</param>
        /// <param name="machine">The owning machine, for reading current context.</param>
        public virtual TransitionResult CanTransitionTo(CreatureStateId target, CreatureStateMachine machine)
        {
            return TransitionResult.Allowed;
        }

        /// <summary>
        /// Called immediately before the machine switches away from this state,
        /// after <see cref="CanTransitionTo"/> has approved. The last chance to
        /// flush anything the next state will need.
        /// </summary>
        /// <param name="next">The state about to become current.</param>
        public virtual void OnTransitioningTo(CreatureStateId next)
        {
        }

        #endregion

        #region Input Hooks

        // These forward the owning machine's input events. They exist so a state
        // can react to input without every state re-subscribing to
        // FluidTouchInputManager, which would multiply the number of delegates
        // in its invocation list and give each state its own idea of what counts
        // as a stroke.
        //
        // The machine routes input to the current state only, so a state never
        // sees an event while it is not current and needs no unsubscribe logic.

        /// <summary>A finger made contact, before any move or end.</summary>
        public virtual void OnTouchBegan(Vector2 screenPosition)
        {
        }

        /// <summary>A finger moved meaningfully while down.</summary>
        public virtual void OnTouchMoved(Vector2 screenPosition)
        {
        }

        /// <summary>A finger lifted.</summary>
        /// <param name="screenPosition">Where the finger left the screen.</param>
        /// <param name="wasTap">True for a short contact that stayed in the dead zone.</param>
        public virtual void OnTouchEnded(Vector2 screenPosition, bool wasTap)
        {
        }

        /// <summary>
        /// A tap was recognised. Fires in addition to
        /// <see cref="OnTouchEnded"/>, and is the hook to use when a state cares
        /// about the child's choice rather than about stroke bookkeeping.
        /// </summary>
        public virtual void OnTapped(Vector2 screenPosition)
        {
        }

        #endregion

        #region Animator Hooks

        // Animator-facing seams. There is no Animator dependency here on purpose.
        //
        // The art bible fixes animation *timing and targets* (art-bible 5.x gives
        // per-state durations such as Rest_Happy 0.9s, Focus_Petting 1.6s,
        // Absorb_Painting 2.0s, Proud_Flourish 2.5s) but not the controller names,
        // and no Animator exists in the scene yet. Coupling to one now would bake
        // in a guess.
        //
        // These virtual no-ops are the seam: a future Animator controller
        // overrides them, and no state needs editing. The parameters are passed
        // as plain floats and bools rather than an Animator instance so a state
        // can be tested headlessly with no Animator in the scene at all.

        /// <summary>
        /// Fires an Animator trigger for this state. Override to drive an
        /// Animator. <paramref name="triggerName"/> is a static string chosen by
        /// the override, so passing it costs no allocation.
        /// </summary>
        /// <param name="triggerName">
        /// Name the override should pass to <c>Animator.SetTrigger</c>. Supplied
        /// by the machine so a single override can map every state.
        /// </param>
        public virtual void OnAnimatorTrigger(string triggerName)
        {
        }

        /// <summary>
        /// Sets a normalised float on the Animator, for continuous parameters
        /// such as stroke speed or lean amount. Override to drive an Animator.
        /// </summary>
        public virtual void OnAnimatorFloat(string parameterName, float value)
        {
        }

        /// <summary>
        /// Sets a bool on the Animator, for latched flags such as "is wet".
        /// Override to drive an Animator.
        /// </summary>
        public virtual void OnAnimatorBool(string parameterName, bool value)
        {
        }

        /// <summary>
        /// Plays a state on the Animator by hash, with a normalised transition
        /// duration. Override to drive an Animator. <paramref name="stateHash"/>
        /// is precomputed so no string is hashed per transition.
        /// </summary>
        public virtual void OnAnimatorCrossFade(int stateHash, float normalizedTransitionDuration)
        {
        }

        #endregion

        #region Diagnostics

        /// <summary>
        /// Appends a one-line description of this state's live condition to
        /// <paramref name="builder"/>. Override to make a state debuggable
        /// without attaching a debugger.
        ///
        /// Takes a StringBuilder rather than returning a string on purpose:
        /// building a description string every frame would allocate every frame,
        /// and this hook is called from the diagnostic path where that cost is
        /// exactly what we are trying to avoid.
        /// </summary>
        public virtual void Describe(System.Text.StringBuilder builder)
        {
            builder.Append(DisplayName);
        }

        #endregion
    }
}
