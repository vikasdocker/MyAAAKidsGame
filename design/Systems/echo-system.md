# Echo System — Signature Marks and Bound Abilities

## Status

Approved MVP scope. The runtime binding/save foundation and first player-facing
Adorn placement path are implemented. Rich placement feedback and additional
catalogued marks remain future work.

## Player promise

A signature mark is an authored choice, not decoration: each mark has exactly
one fixed ability, and the creature performs that ability back. The child never
chooses an ability combination or receives a random result.

## MVP content

| Signature mark | Bound ability | MVP performance |
|---|---|---|
| Horn Swirl | Playful Charge | The creature runs toward the chosen point and celebrates on arrival |

The Horn Swirl → Playful Charge pairing follows the concrete example in
`design/gdd/game-concept.md`. The MVP contains this one pairing. The catalog is
the authority for the stable mark-to-ability mapping; callers cannot select or
override the ability for a mark.

## Runtime contract

- A mark is bound explicitly. A new creature does not receive a mark
  automatically.
- A bound mark can be queried by stable ID; a known mark always resolves to its
  one catalogued ability.
- Duplicate binding is idempotent. Unknown IDs are rejected and reported through
  the normal API result; they do not mutate the creature's saved design.
- Ability performance goes through `CreatureStateMachine` and the existing
  `AbilityUnlock` flourish state. The playground's movement remains
  deterministic and uses no random selection.
- Ability identity is a typed gameplay value, not an Animator state hash. The
  existing flourish animation remains shared in the MVP.
- Bound mark IDs are saved locally and restored before input is accepted. No
  cloud, account, or network service is involved.

## Interaction and presentation

- The gameplay scenes expose an icon-first Adorn control. Activating it enters a
  deliberate placement mode; activating it again cancels without changing the
  authored design.
- A tap on the creature places the selected Horn Swirl. A drag that begins on
  the creature has the same placement result at its initial contact; neither
  path requires fine motor precision.
- Placement draws the mark in Cocoa Ink through the existing paint compositor.
  The completed shape is stored by the local paint journal, and the stable mark
  ID remains stored by the signature-mark save. Both are restored by their
  existing local persistence systems.
- Placement does not erase or replace existing paint. Once bound, the MVP mark
  is permanent and cannot be placed again or removed.
- The Playground uses the existing ground-tap route to perform the bound
  Playful Charge. No default binding or random result is introduced.

## Design constraints

- No fail state, timer, cost, rarity, random roll, or loss of an existing mark.
- No mark combinations in the MVP; one mark always maps to one fixed ability.
- More marks may be added as authored catalog entries after design approval.
- Freehand painting is temporarily gated while the deliberate mark-placement
  mode is active, then resumes after the placing touch ends.

## Verification

- The catalog maps Horn Swirl to Playful Charge and has no combination path.
- Unknown marks cannot be bound; duplicate binding does not duplicate state.
- A bound Horn Swirl causes the typed Playful Charge to be performed through the
  state machine, while an unbound creature does not perform an ability.
- The Adorn control is cancellable; a surface tap creates one visible, persistent
  mark stroke and binds Horn Swirl; the placement touch does not enter Painting.
- Mark save/load round-trips stable IDs; corrupt or unsupported saves are
  reported and left intact.
- Existing freehand paint and gameplay tests continue to pass.
