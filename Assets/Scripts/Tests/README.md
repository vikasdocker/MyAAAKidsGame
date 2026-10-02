# Tests — Dab.Tests

PlayMode + EditMode tests. Namespace root: Dab.Tests

Reference Dab.Runtime. llowUnsafeCode is false project-wide; do not request
it without an ADR.

## Highest-value targets

1. **Paint system** — stroke-to-surface mapping, undo, persistence round-trip
2. **Signature-mark → ability binding** — the "Echo" rule, including combination cases
3. **Save/load** — an authored design must survive a full app restart intact
4. **Non-negotiable constraint tests** — no fail states, no timers, no random rewards

> Loss of a child's authored creation is the most severe failure this game has.
> Test it hardest.
