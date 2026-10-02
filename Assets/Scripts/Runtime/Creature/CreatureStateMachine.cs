using System;
using System.Text;
using Dab.Runtime.Input;
using UnityEngine;

namespace Dab.Runtime.Creature
{
    /// <summary>
    /// Owns the creature's current state and routes input to it.
    ///
    /// Design
    /// ------
    /// A single owner, one current state, and a fixed table of states created
    /// once at Awake. Transitions are value-based on <see cref="CreatureStateId"/>
    /// rather than object-identity based, so they can be expressed in data and
    /// restored from a save.
    ///
    /// Why input is routed rather than subscribed per state
    /// ---------------------------------------------------
    /// Each state could subscribe to <see cref="FluidTouchInputManager"/> itself.
    /// That was rejected: it multiplies delegates in the input manager's
    /// invocation list, gives every state an independent copy of gesture logic,
    /// and leaves five states each needing correct unsubscribe-on-exit or they
    /// leak. Instead the machine holds the only subscription and forwards to the
    /// current state, so a state cannot receive input while inactive and needs no
    /// subscription logic at all.
    ///
    /// Allocation behaviour
    /// --------------------
    ///   - States are constructed once in Awake, never on transition.
    ///   - TransitionTo is allocation-free on the success path: no boxing, no
    ///     lambdas, no LINQ, no string formatting.
    ///   - The events below are plain C# events, not UnityEvents, so raising
    ///     them does not allocate an argument array.
    ///   - The diagnostic string is built into a reused StringBuilder, allocated
    ///     once, and only when Describe() is called.
    ///
    /// The one deliberate exception is the <c>TransitionRefused</c> event, which
    /// carries a <see cref="TransitionResult"/> and a static state name. It is
    /// raised only on refusal, never per frame.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-50)]
    public sealed class CreatureStateMachine : MonoBehaviour
    {
        #region Configuration

        [Header("Input")]
        [Tooltip("Input source. If left empty the machine looks for one on this " +
                 "GameObject, then for one in the scene.")]
        [SerializeField]
        private FluidTouchInputManager _input;

        [Header("Startup")]
        [Tooltip("State the creature begins in. Idle is the baseline every other " +
                 "state is defined as a delta from (art-bible 2.3).")]
        [SerializeField]
        private CreatureStateId _initialState = CreatureStateId.Idle;

        [Tooltip("Seconds without input before the creature returns to Idle. " +
                 "Prevents the creature getting stranded in a reactive state " +
                 "after the child stops touching. Zero disables the timeout.")]
        [SerializeField, Min(0f)]
        private float _idleReturnDelay = 4f;

        [Header("Petting Detection")]
        [Tooltip("Seconds a touch must stay down and slow before it reads as " +
                 "petting rather than painting. Painting starts on a moving " +
                 "finger, petting on a sustained gentle one.")]
        [SerializeField, Range(0.1f, 3f)]
        private float _pettingHoldSeconds = 0.6f;

        [Tooltip("Inches per second below which a moving finger still counts as " +
                 "gentle. Inches not pixels, because DPI varies enormously " +
                 "across the target devices and this audience skews cheap.")]
        [SerializeField, Range(0.05f, 8f)]
        private float _pettingMaxSpeedInchesPerSecond = 1.6f;

        [Header("Diagnostics")]
        [Tooltip("Logs every accepted and refused transition. Off by default; " +
                 "string formatting on a transition is fine, per frame is not.")]
        [SerializeField]
        private bool _logTransitions;

        #endregion

        #region Public Events

        /// <summary>
        /// Raised after a transition completes, once both exit and enter hooks
        /// have run. Carries value-typed arguments only, so raising it does not
        /// allocate.
        /// </summary>
        public event Action<CreatureStateId, CreatureStateId> StateChanged;

        /// <summary>
        /// Raised when a transition was asked for and declined. Carries the
        /// reason and the two static state names, so no string is built on the
        /// refusal path either. Only raised if something is subscribed.
        /// </summary>
        public event Action<CreatureStateId, CreatureStateId, TransitionResult> TransitionRefused;

        #endregion

        #region Public State

        /// <summary>The current state, or null before Awake has run.</summary>
        public CreatureState CurrentState { get; private set; }

        /// <summary>Stable identity of the current state.</summary>
        public CreatureStateId CurrentStateId { get; private set; } = CreatureStateId.Idle;

        /// <summary>Seconds the current state has been active.</summary>
        public float TimeInState { get; private set; }

        /// <summary>Total accepted transitions. Diagnostic only.</summary>
        public int TransitionCount { get; private set; }

        #endregion

        #region Internal State

        // Fixed-size table. Five states is a compile-time fact, not a runtime
        // one, so the table is a plain array with a constant size and a
        // parallel id lookup. No dictionary: a Dictionary<enum, CreatureState>
        // would hash on every lookup and would need a non-zero-based array
        // internally anyway.
        /// <summary>
        /// Number of states in the table. Exposed so tooling and tests can walk
        /// the whole table without hard-coding a count that would then drift out
        /// of step with <see cref="CreatureStateId"/>.
        /// </summary>
        public const int StateCount = 5;

        private readonly CreatureState[] _states = new CreatureState[StateCount];
        private readonly StringBuilder _describeBuilder = new StringBuilder(128);

        /// <summary>
        /// Read-only lookup of a constructed state, or null if the id is out of
        /// range. Diagnostic and test surface only: gameplay should go through
        /// <see cref="TransitionTo"/> so guards still run. Returns the live
        /// instance, so callers must not mutate it.
        /// </summary>
        public CreatureState GetState(CreatureStateId id)
        {
            var index = (int)id;
            return index >= 0 && index < StateCount ? _states[index] : null;
        }

        // Petting detection. Tracked here rather than in PettingState because the
        // decision "is this a gentle touch or a painting stroke" has to be made
        // before we know which state to enter.
        private float _touchDownSeconds;
        private Vector2 _lastTouchPosition;
        private float _smoothedSpeedInchesPerSecond;
        private bool _strokeInProgress;

        // Reused so OnAnimatorCrossFade can be called with a precomputed hash
        // and the log can name a state without allocating.
        private bool _subscribed;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            BuildStateTable();
        }

        private void OnEnable()
        {
            SubscribeInput();

            if (CurrentState == null)
            {
                // Force-enter the initial state without asking the (nonexistent)
                // current state for permission.
                EnterInitialState();
            }
        }

        private void OnDisable()
        {
            // Exit before unsubscribing so a state's OnExit sees a live machine
            // and cannot leave a timer armed against a disabled component.
            if (CurrentState != null)
            {
                CurrentState.OnExit();
            }

            UnsubscribeInput();
        }

        private void Update()
        {
            var deltaTime = Time.unscaledDeltaTime;
            TimeInState += deltaTime;
            TickTouchClock(deltaTime);

            // Input is consumed by the input manager in its own Update. The
            // manager runs at execution order -100, this at -50, so every event
            // raised during a frame has already been delivered by the time this
            // body runs. The current state therefore sees a consistent frame
            // rather than a half-updated one.
            CurrentState?.OnUpdate(deltaTime);

            ArbitrateHeldGesture();
            UpdateIdleTimeout(deltaTime);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Requests a transition. The current state has the right to refuse it.
        /// </summary>
        /// <returns>
        /// True if the transition happened. False if the current state refused,
        /// in which case <see cref="TransitionRefused"/> has been raised.
        /// </returns>
        public bool TransitionTo(CreatureStateId target)
        {
            if (CurrentState == null)
            {
                EnterState(target);
                return true;
            }

            if (target == CurrentStateId)
            {
                // Re-entering the current state would run OnExit/OnEnter for no
                // reason and restart every timer in it. Treat it as a no-op so
                // callers can be sloppy about it.
                return false;
            }

            var verdict = CurrentState.CanTransitionTo(target, this);
            if (verdict != TransitionResult.Allowed)
            {
                RaiseRefused(target, verdict);
                return false;
            }

            EnterState(target);
            return true;
        }

        /// <summary>
        /// Forces a transition, bypassing <see cref="CreatureState.CanTransitionTo"/>.
        ///
        /// For scripted moments that must not be vetoable, such as a scene load
        /// or a scripted flourish. Because it bypasses the check, a state that
        /// owns an uninterruptible gesture will have that gesture torn down
        /// mid-flight, so prefer <see cref="TransitionTo"/> unless you know why.
        /// </summary>
        public void ForceTransitionTo(CreatureStateId target)
        {
            EnterState(target);
        }

        /// <summary>
        /// Returns to Idle. The natural way for a reactive state to finish.
        /// </summary>
        public bool ReturnToIdle()
        {
            return TransitionTo(CreatureStateId.Idle);
        }

        /// <summary>
        /// A one-line description of the current state, for logs and the
        /// inspector. Allocates only when called, because it returns a string.
        /// Do not call it per frame.
        /// </summary>
        public string DescribeCurrentState()
        {
            _describeBuilder.Length = 0;
            _describeBuilder.Append(CurrentStateId).Append(" for ")
                            .Append(TimeInState.ToString("F2")).Append("s: ");
            CurrentState?.Describe(_describeBuilder);
            return _describeBuilder.ToString();
        }

        #endregion

        #region State Table

        private void BuildStateTable()
        {
            // Constructed here, once. See the allocation contract on
            // CreatureState: a transition must never allocate a state.
            _states[(int)CreatureStateId.Idle] = new IdleState();
            _states[(int)CreatureStateId.Painting] = new PaintingState();
            _states[(int)CreatureStateId.Petting] = new PettingState();
            _states[(int)CreatureStateId.Feeding] = new FeedingState();
            _states[(int)CreatureStateId.AbilityUnlock] = new AbilityUnlockState();

            for (var i = 0; i < StateCount; i++)
            {
                _states[i]?.Initialize(this);
            }
        }

        private void EnterInitialState()
        {
            var initial = _initialState;

            // Clamp rather than throw. A save file or an inspector typo naming a
            // state that was never built should land on Idle, which every other
            // state is defined against, not leave the creature with no state.
            if ((int)initial < 0 || (int)initial >= StateCount || _states[(int)initial] == null)
            {
                initial = CreatureStateId.Idle;
            }

            EnterState(initial);
        }

        private void EnterState(CreatureStateId target)
        {
            var index = (int)target;
            if (index < 0 || index >= StateCount)
            {
                return;
            }

            var next = _states[index];
            if (next == null)
            {
                return;
            }

            var previousId = CurrentStateId;

            if (CurrentState != null)
            {
                CurrentState.OnTransitioningTo(target);
                CurrentState.OnExit();
            }

            CurrentState = next;
            CurrentStateId = target;
            TimeInState = 0f;
            _touchDownSeconds = 0f;
            _smoothedSpeedInchesPerSecond = 0f;
            _strokeInProgress = false;
            TransitionCount++;

            CurrentState.OnEnter();

            if (_logTransitions)
            {
                Debug.Log($"[CreatureStateMachine] {previousId} -> {target}", this);
            }

            StateChanged?.Invoke(previousId, target);
        }

        private void RaiseRefused(CreatureStateId target, TransitionResult verdict)
        {
            if (_logTransitions)
            {
                Debug.Log($"[CreatureStateMachine] {CurrentStateId} -> {target} refused: {verdict}", this);
            }

            // Guarded so the common case, nothing listening, costs one null check
            // and builds nothing.
            if (TransitionRefused != null)
            {
                TransitionRefused(CurrentStateId, target, verdict);
            }
        }

        private void UpdateIdleTimeout(float deltaTime)
        {
            if (_idleReturnDelay <= 0f || CurrentStateId == CreatureStateId.Idle)
            {
                return;
            }

            // Deliberately keyed on time-in-state rather than on "no input seen".
            // A creature that misses its own return to Idle would sit in the last
            // reactive state forever, and the child would be talking to a creature
            // stuck in Focus_Petting with no finger anywhere near it.
            if (TimeInState >= _idleReturnDelay)
            {
                ReturnToIdle();
            }
        }

        #endregion

        #region Input Routing

        private void SubscribeInput()
        {
            if (_subscribed)
            {
                return;
            }

            ResolveInput();

            if (_input == null)
            {
                Debug.LogWarning(
                    "[CreatureStateMachine] No FluidTouchInputManager found. The " +
                    "creature will sit in its initial state and never react to " +
                    "touch. Add the manager to the scene, or assign it here.", this);
                return;
            }

            // Named handlers stored in fields, not inline lambdas, so
            // UnsubscribeInput can actually detach them. The same reasoning as
            // CreaturePaintController: a lambda passed straight to += leaves no
            // reference to remove, which leaks the machine for the scene's life.
            _input.TouchBegan += HandleTouchBegan;
            _input.TouchMoved += HandleTouchMoved;
            _input.TouchEnded += HandleTouchEnded;
            _input.Tapped += HandleTapped;
            _subscribed = true;
        }

        private void UnsubscribeInput()
        {
            if (!_subscribed || _input == null)
            {
                return;
            }

            _input.TouchBegan -= HandleTouchBegan;
            _input.TouchMoved -= HandleTouchMoved;
            _input.TouchEnded -= HandleTouchEnded;
            _input.Tapped -= HandleTapped;
            _subscribed = false;
        }

        private void ResolveInput()
        {
            if (_input != null)
            {
                return;
            }

            _input = GetComponent<FluidTouchInputManager>();

            if (_input == null)
            {
                // GetComponentInChildren before falling back to a scene-wide
                // search: a per-creature manager on a child is the intended setup
                // once there is more than one creature, and a scene-wide search
                // would silently bind every creature to whichever manager it found
                // first.
                _input = GetComponentInChildren<FluidTouchInputManager>();
            }
        }

        private void HandleTouchBegan(Vector2 position)
        {
            _touchDownSeconds = 0f;
            _lastTouchPosition = position;
            _smoothedSpeedInchesPerSecond = 0f;

            CurrentState?.OnTouchBegan(position);
        }

        private void HandleTouchMoved(Vector2 position)
        {
            // Speed is measured here rather than inside a state because the
            // gentle-versus-brisk decision gates which state we enter. It has to
            // be known before PettingState exists as the current state.
            var dpi = Mathf.Max(1f, Screen.dpi);
            var inches = (position - _lastTouchPosition).magnitude / dpi;
            _lastTouchPosition = position;

            var instantaneous = inches / Mathf.Max(1e-5f, Time.unscaledDeltaTime);

            // Smoothed rather than instantaneous. A single frame of finger
            // jitter, or a frame drop that stretches one deltaTime, would
            // otherwise spike the reading and make a gentle stroke look brisk.
            // 0.35 keeps recent history without adding perceptible lag to a
            // six-year-old's intent.
            _smoothedSpeedInchesPerSecond = Mathf.Lerp(
                _smoothedSpeedInchesPerSecond, instantaneous, 0.35f);

            CurrentState?.OnTouchMoved(position);

            ArbitrateGesture();
        }

        /// <summary>
        /// Decides whether the live gesture means painting or petting.
        ///
        /// This is the core arbitration between the two continuous verbs, and it
        /// lives on the machine rather than inside either state because neither
        /// state can be current before the decision is made. A state that could
        /// observe the gesture first and then claim it would have to be entered
        /// speculatively and then possibly transition away on the same frame.
        ///
        /// The rule, in priority order:
        ///
        ///   1. A finger that has travelled past the input manager's dead zone is
        ///      painting. Painting wins ties on purpose. A child drawing a slow
        ///      careful line and a child stroking the creature produce a
        ///      similar signal, and guessing wrong in the petting direction is
        ///      much worse: the creature would react to being petted while the
        ///      child thinks they are painting, and the dabs would go
        ///      unacknowledged at the exact moment they are trying hardest.
        ///
        ///   2. Otherwise, a finger held down and still for long enough is
        ///      petting. This is the "gentle, targeted" case: no travel, so no
        ///      stroke, but deliberate and sustained, which is what separates it
        ///      from a finger resting on the screen.
        ///
        ///   3. Anything else is not a gesture this machine claims. The creature
        ///      stays Idle and the input falls through to other consumers, which
        ///      is what lets the paint controller receive the same touch.
        ///
        /// Note this runs off <see cref="FluidTouchInputManager.IsDragging"/>
        /// rather than re-deriving travel, so the dead zone stays defined in
        /// exactly one place. Duplicating that threshold here would be how the
        /// two classes slowly disagree about what counts as a drag.
        /// </summary>
        private void ArbitrateGesture()
        {
            if (CurrentState == null || _input == null)
            {
                return;
            }

            // Only arbitrate out of Idle. From any other state the current
            // state's own CanTransitionTo governs, and re-asserting a competing
            // claim here would fight the state machine's own rules.
            if (CurrentStateId != CreatureStateId.Idle)
            {
                return;
            }

            if (_input.IsDragging)
            {
                // Painting is entered through TransitionTo, not forced, so the
                // refusal path stays meaningful if a subclass of PaintingState
                // ever declines to start mid-gesture.
                TransitionTo(CreatureStateId.Painting);
                _strokeInProgress = true;
                return;
            }

            if (IsGentleSustainedTouch)
            {
                TransitionTo(CreatureStateId.Petting);
            }
        }

        /// <summary>
        /// The held-still case, evaluated every frame rather than on move events.
        ///
        /// This has to be separate from <see cref="ArbitrateGesture"/> because a
        /// finger resting on the creature produces no movement at all, and
        /// <c>TouchMoved</c> only fires once travel passes the input manager's
        /// dead zone and sample spacing. Waiting for a move event to detect
        /// petting would mean a perfectly still child is never petted, which is
        /// the single most likely way a child pets something.
        ///
        /// Costs one branch per frame in the common case: the cheap
        /// <c>IsTouching</c> test rejects almost every frame.
        /// </summary>
        private void ArbitrateHeldGesture()
        {
            if (CurrentStateId != CreatureStateId.Idle || _input == null)
            {
                return;
            }

            if (!_input.IsTouching)
            {
                return;
            }

            // IsDragging is re-checked so a finger that started as a hold and
            // then travelled is claimed by painting on the same frame it crosses
            // the dead zone, rather than sitting in a pet that was never asked
            // for. HandleTouchMoved usually gets there first; this closes the gap.
            if (_input.IsDragging)
            {
                return;
            }

            if (IsGentleSustainedTouch)
            {
                TransitionTo(CreatureStateId.Petting);
            }
        }

        private void HandleTouchEnded(Vector2 position, bool wasTap)
        {
            CurrentState?.OnTouchEnded(position, wasTap);

            // A stroke that ends returns the creature to rest. Done here rather
            // than relying on the idle timeout so the settle begins the moment
            // the finger lifts, which is what makes the response feel connected
            // to the touch. A tap is excluded: a tap carries no gesture, and
            // bouncing through Painting on a tap would make the creature twitch
            // at every accidental brush of the screen.
            if (_strokeInProgress && !wasTap)
            {
                _strokeInProgress = false;
                TransitionTo(CreatureStateId.Idle);
            }
        }

        private void HandleTapped(Vector2 position)
        {
            CurrentState?.OnTapped(position);
        }

        /// <summary>
        /// Feeds the machine's own per-frame bookkeeping to the current state.
        ///
        /// States that need to know how long a finger has been down, or how fast
        /// it is moving, read it from here rather than each re-deriving it from
        /// the touch events. Exposed on the machine so a state cannot disagree
        /// with the machine about the gesture.
        /// </summary>
        public float TouchHeldSeconds => _touchDownSeconds;

        /// <summary>Smoothed finger speed in inches per second. Zero when still.</summary>
        public float TouchSpeedInchesPerSecond => _smoothedSpeedInchesPerSecond;

        /// <summary>
        /// True when the current gesture has been slow and sustained long enough
        /// to read as petting. The machine owns the definition so "gentle" means
        /// one thing across every state.
        /// </summary>
        public bool IsGentleSustainedTouch =>
            _touchDownSeconds >= _pettingHoldSeconds &&
            _smoothedSpeedInchesPerSecond <= _pettingMaxSpeedInchesPerSecond;

        /// <summary>
        /// Advances the touch-duration clock. Called once per frame from Update.
        ///
        /// Accumulated from deltaTime rather than sampled from a start timestamp
        /// so a frame drop does not retroactively extend a gesture that the child
        /// has already finished, and so a touch that began before a long frame
        /// still measures correctly.
        /// </summary>
        private void TickTouchClock(float deltaTime)
        {
            if (_input != null && _input.IsTouching)
            {
                _touchDownSeconds += deltaTime;
            }
        }

        #endregion
    }
}
