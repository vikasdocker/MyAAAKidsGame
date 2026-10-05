# Tests — Dab.Tests

PlayMode + EditMode tests. Namespace root: Dab.Tests

Reference Dab.Runtime. llowUnsafeCode is false project-wide; do not request
it without an ADR.

## Highest-value targets

1. **Paint system** — stroke-to-surface mapping, undo, persistence round-trip
2. **Signature-mark → ability binding** — the MVP's fixed 1:1 Echo rule, including player-facing Adorn placement and visible mark persistence
3. **Save/load** — an authored design must survive a full app restart intact. MVP paint strokes and bound mark IDs have local persistence; crash/device-loss validation remains
4. **Non-negotiable constraint tests** — regression guards cover fail/loss state names, forbidden meta-system types, random gameplay decisions, and third-party ad/analytics SDK packages. Keep these checks current when adding runtime systems or dependencies.

> Loss of a child's authored creation is the most severe failure this game has.
> Test it hardest.
