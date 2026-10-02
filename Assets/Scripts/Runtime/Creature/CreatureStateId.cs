namespace Dab.Runtime.Creature
{
    /// <summary>
    /// Every state the creature can be in, as a stable identity.
    ///
    /// Why an enum alongside the state objects
    /// ---------------------------------------
    /// The State Pattern normally keys transitions on the state object itself.
    /// That works, but it makes every comparison a reference equality check and
    /// leaves "which state is this" unanswerable from a save file, a log line, or
    /// an inspector field. This enum gives each state a stable serialisable
    /// identity, so a state can be named in data, compared by value, and restored
    /// after a scene load without holding a reference to a live object.
    ///
    /// Values are explicit and must not be renumbered: they are written into
    /// save data as integers.
    /// </summary>
    public enum CreatureStateId
    {
        /// <summary>
        /// No state. Never a legal transition target, and deliberately not a
        /// member of the runtime state table.
        ///
        /// This exists so "the creature has not entered a state yet" is
        /// representable without lying. A disabled or not-yet-booted machine
        /// has no current state, and previously the only way to say that was to
        /// claim Idle, which is a real behavioural claim. A sentinel that is
        /// explicitly illegal as a target keeps that distinction available to
        /// callers and to StateChanged subscribers.
        ///
        /// Placed last so it cannot be mistaken for an ordinal index into the
        /// table: valid table indices are 0 through 4 and this is 5, which is
        /// exactly out of range, so GetState returns null for it rather than
        /// aliasing a real state.
        /// </summary>
        None = 5,

        /// <summary>Resting baseline. See art-bible 2.3.</summary>
        Idle = 0,

        /// <summary>Absorbed, silky focus while a stroke is live. See art-bible 2.4.</summary>
        Painting = 1,

        /// <summary>Response to a gentle, sustained touch. See art-bible 2.3.</summary>
        Petting = 2,

        /// <summary>
        /// Drag-to-feed. Explicitly out of scope for the youngest cohort per
        /// game-concept.md ("lacks fine motor control and object persistence for
        /// drag-to-feed"), so it is opt-in and not reachable from the default
        /// input map.
        /// </summary>
        Feeding = 3,

        /// <summary>
        /// The signature mark has been bound to a real ability, and the creature
        /// performs it. See game-concept.md 5-minute loop, step 3 ("Echo").
        /// </summary>
        AbilityUnlock = 4
    }

    /// <summary>
    /// Why a state can decline a transition
    /// ------------------------------------
    /// <see cref="CreatureStateMachine"/> asks the current state whether a
    /// transition is allowed before performing it. A state returns one of these
    /// rather than a bare bool so a refusal can be logged with a reason that a
    /// human can act on, instead of a transition silently not happening.
    /// </summary>
    public enum TransitionResult
    {
        /// <summary>The transition may proceed.</summary>
        Allowed = 0,

        /// <summary>
        /// The state is mid-gesture and must finish what it is doing. Use for
        /// input that cannot be interrupted, such as a live paint stroke.
        /// </summary>
        BlockedBusy = 1,

        /// <summary>
        /// The transition is not meaningful from here, and the caller should not
        /// retry. Use for conditions that cannot become true mid-state without
        /// an intervening transition.
        /// </summary>
        BlockedInvalid = 2,

        /// <summary>
        /// The state requires something the scene has not supplied yet, such as
        /// a renderer or an audio source. Distinct from
        /// <see cref="BlockedInvalid"/> because it may resolve later.
        /// </summary>
        BlockedNotReady = 3
    }
}
