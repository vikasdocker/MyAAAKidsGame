# ADR-0003: Local Paint Persistence as a Versioned Stroke Journal

## Status

Accepted

## Date

2026-10-05

## Context

The MVP must retain a child's authored paint across scene changes and app
restarts. The paint currently exists only in a GPU `RenderTexture`; the design
requires local-only storage, and the painting interaction must not incur
readback stalls or network latency.

The product scope is one MVP creature. The save decision for future variants is
deferred; this record must not introduce a variant-mapping layer. Signature-mark
IDs are persisted separately under ADR-0004; they are not paint-journal records.

## Decision

Persist each completed paint stroke as an ordered sequence of normalized UV
samples and brush colors in a versioned JSON-lines journal under
`Application.persistentDataPath`.

- Restore strokes through the existing `CreaturePaintController` before input
  can be accepted.
- Append after a stroke ends. Serialize each stroke on the main thread and
  perform append I/O on a worker thread. Read and replay the journal during
  startup before the first input frame; flush pending writes on app pause, app
  quit, and rig destruction.
- Keep committed lines if the final line is incomplete; report and repair only
  that tail. Treat interior corruption and unsupported versions as explicit
  errors and never silently replace them with an empty save.
- Keep persistence local and offline. Do not add cloud sync, accounts, or
  third-party services.
- Defer variant remapping. Signature-mark persistence is handled by the
  separate versioned save document approved in ADR-0004.

The journal captures the paint inputs, not a GPU texture snapshot. Replaying the
same UV/color sequence through the current brush compositor avoids GPU
readback and preserves the authored design on the current MVP surface.

## Alternatives considered

### Read back and save the `RenderTexture`

Rejected for the prototype: synchronous readback can stall the painting path;
asynchronous readback needs additional lifecycle handling to guarantee the most
recent design is flushed when the app backgrounds. A raster checkpoint can be
reconsidered after low-end Android profiling.

### Save only when the app exits

Rejected: mobile apps may be suspended or terminated without a reliable exit
callback, risking loss of a child's authored work.

### Cloud/account persistence

Rejected: conflicts with the offline-first and local-save constraints and adds
unnecessary networking and account scope.

## Consequences

- Save size grows with authored stroke history; the MVP has no erase or reset
  path, so completed records are append-only.
- Brush-compositor changes may require a future journal migration or a raster
  checkpoint. The schema version makes incompatible changes explicit.
- A stroke is durable after its completion write is flushed. An interrupted
  final line is recoverable without discarding earlier strokes.
- `ADR-0002` remains the separately planned production painting-render-pass
  decision referenced by the painting prototype; this ADR does not change that
  rendering approach.

## Verification

- Round-trip UV positions, colors, stroke boundaries, and sample ordering.
- Restore through the paint controller without recursively saving restored
  strokes.
- Recover a truncated final record while preserving earlier complete strokes.
- Confirm save writes are flushed on lifecycle/scene teardown and do not perform
  GPU readback.
