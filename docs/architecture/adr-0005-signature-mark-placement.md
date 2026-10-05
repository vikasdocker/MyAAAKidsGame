# ADR-0005: Deliberate Signature-Mark Placement

## Status

Accepted

## Date

2026-10-05

## Context

ADR-0004 established the stable Horn Swirl → Playful Charge binding and local
mark-ID save, but deferred the child-facing Adorn interaction. Without visible
placement the authored mark cannot be made by a player, and the saved mark ID
alone cannot show where the mark was placed.

## Decision

- Add an icon-first Adorn control to the Playground and painting demo.
- The control toggles a placement mode that can be cancelled without changing
  saved state. The only MVP choice is Horn Swirl.
- The first touch on the creature places a single Horn Swirl. A tap and a drag
  beginning on the creature have the same result at the initial contact point.
- Render the mark as a Cocoa Ink spiral by submitting it as a completed stroke
  to the existing paint compositor. This reuses existing UV mapping, visible
  paint texture, and local paint journal rather than creating a second decal
  renderer or expanding the mark-save schema.
- Persist the stable Horn Swirl ID through the existing signature-mark store.
  The visible stroke and gameplay identity are restored locally by the existing
  save systems.
- Gate freehand touch painting for the duration of placement mode, then restore
  it after the placing touch ends. Programmatic compositor calls remain enabled.
- Give every rig a neutral Dawn Cream base so Cocoa Ink authored marks have
  contrast and the creature's unpainted surface remains visually quiet.

## Alternatives considered

### Add a new decal/mesh renderer and save UV coordinates in the mark file

Rejected for this MVP: it duplicates the existing mask-based paint path and
changes the stable mark-save format. The current paint journal already records
UV/color samples needed to reconstruct the placed shape.

### Bind Horn Swirl from the Adorn button alone

Rejected: it would create an ability without a deliberate visible mark on the
creature, violating the authored-not-decorated pillar.

### Make the mark freehand-drawn

Rejected: signature marks are discrete choices whose binding must be legible
and deliberate; the freehand tool remains a separate creative path.

## Consequences

- The MVP placement is permanent and has no erase, undo, or reposition path.
- The placement shape is a procedural prototype, not a final art asset.
- The existing two local stores persist the visible mark stroke and bound mark
  ID independently. Either store can report a write failure; store unification
  and crash/device-loss testing remain future persistence work.
- The existing tap and drag painting tests must continue to pass, and a
  player-facing placement-to-binding regression test is required.

## Verification

- Enter and cancel placement without binding a mark.
- Tap the creature to draw and persist one Horn Swirl stroke, bind its fixed
  typed ability, and return to freehand mode after release.
- Verify that the placement touch does not enter the Painting state.
- Run the complete Unity PlayMode test suite.
