# Save System GDD

**Status:** MVP scope resolved
**Source:** `design/gdd/game-concept.md` — offline save and permanent authorship
**Architecture:** `docs/architecture/adr-0003-local-paint-persistence.md` and
`docs/architecture/adr-0004-echo-signature-mark-binding.md`

## Player promise

Paint the creature, leave, and come back to the same authored design. Saving is
automatic and local. The child never has to remember a save action or confirm
that their work was kept.

## MVP scope

- Persist completed freehand paint strokes for the single MVP creature.
- Restore the same paint on the same creature after scene changes and app
  restarts.
- Save locally on the device; no account, cloud, network, analytics, or
  purchase dependency.
- Save each completed stroke automatically. An in-progress stroke is committed
  when the gesture ends.
- Keep this save format specific to the current creature. Mapping existing paint
  onto future creature variants is deferred.
- Bound signature-mark IDs are saved locally after the Echo data model is
  approved. Their fixed abilities are resolved from the runtime catalog rather
  than duplicated in the save. Visible mark placement remains future work.

## Player experience

There is no save button, save prompt, confirmation dialog, timer, or failure
state. The existing paint interaction stays offline and does not wait for
storage I/O. On boot, saved strokes and bound mark IDs are restored before the child can
interact.
Storage problems are logged explicitly for development; they never prevent the
child from entering the scene and painting.

## Data and recovery

Each stroke is stored as its ordered normalized surface UV samples and brush
colors. A versioned JSON-lines journal is appended locally, then replayed through
the existing paint controller on startup. This avoids GPU texture readback and
keeps persisted paint on the same mask/decal path as live paint.

If the final journal record is incomplete, keep all preceding complete strokes,
report the recovery, and remove only the incomplete tail before appending again.
Corruption before the final record or an unsupported schema version is an
explicit load error; do not silently replace that save with an empty design.

## Acceptance criteria

1. A fresh install starts with a blank paint mask and no save file.
2. Completed strokes are appended locally without network access or render
   texture readback.
3. Reloading the journal reproduces the same ordered UV/color inputs and visible
   paint on the MVP creature.
4. A partially written final record does not discard earlier complete strokes.
5. Unsupported or unreadable save data is reported and is not overwritten as a
   successful empty save.
6. Scene transitions and app background/quit flush pending local writes.
7. Bound mark IDs round-trip through their versioned local save document;
   unsupported or corrupt data is reported and left unchanged.
