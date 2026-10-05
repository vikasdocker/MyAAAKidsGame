# ADR-0004: Fixed Signature-Mark-to-Ability Binding

## Status

Accepted

## Date

2026-10-05

## Context

The game's core promise is that a child's authored mark causes the creature to
perform a move. `AbilityUnlockState` currently accepts a bare integer and the
playground can enter that state without any mark data. The design question was
whether one mark unlocks one fixed ability or whether combinations create
emergent abilities. The selected model affects the runtime data shape, save
format, and content cost.

## Decision

- Use a stable, typed `SignatureMarkId` and `CreatureAbilityId`.
- Keep the mapping in a deterministic catalog: each mark maps to exactly one
  fixed ability. Combinations have no meaning.
- The MVP catalog contains Horn Swirl → Playful Charge, based on the concrete
  example in the game concept.
- A creature's bound marks are explicit authored state; do not grant a default
  mark. Duplicate binding is idempotent, and unknown IDs are rejected without
  changing saved state.
- Persist bound mark IDs locally alongside the existing local-only design
  persistence. Resolve ability IDs from the catalog at runtime rather than
  persisting duplicate, potentially inconsistent mapping data.
- Route a performance request through the existing `AbilityUnlock` state. The
  MVP may share one flourish animation; gameplay ability identity must not be
  encoded as an Animator state hash.
- Defer the Adorn UI and visible surface placement to a subsequent phase. The
  runtime and save foundation in this phase is not a complete player-facing Echo
  loop and must not silently bind a mark for a new player.

## Alternatives considered

### Emergent abilities from combinations

Rejected for the MVP: it expands the content and test matrix, makes the result
harder to predict, and weakens the legibility of the child's authored choice.

### Store mark-to-ability pairs in each save

Rejected: the mapping is fixed content, not player-authored state. Saving both
IDs would allow a stale or corrupt save to contradict the catalog.

### Grant a default mark so the playground always performs an ability

Rejected: it would make the creature appear to have an authored choice the child
did not make, violating the authored-not-decorated pillar.

### Reuse raw ability IDs as Animator hashes

Rejected: gameplay identity and animation state identity are distinct
contracts. The MVP uses the existing shared flourish animation.

## Consequences

- More abilities require explicit catalog entries and authored design, but do
  not require an emergent-combination engine.
- A new creature has no bound mark until an explicit Adorn interaction is
  implemented and used.
- Save data contains only stable mark IDs; catalog changes that remove an ID
  require explicit migration or error handling rather than silent remapping.
- The current phase is infrastructure only. The mark-placement and visual
  presentation work remains necessary before the feature is considered
  player-ready.

## Verification

- Test the exact MVP catalog mapping and rejection of unknown IDs.
- Test binding idempotence and typed ability performance.
- Test local mark save/load and preserve corrupt or unsupported saves.
- Run the full PlayMode suite to detect regressions in the existing playground,
  painting, and state machine.
