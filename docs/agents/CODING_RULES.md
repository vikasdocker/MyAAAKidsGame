# Coding rules & anti-corruption law

**Doc type:** project knowledge (indexed into Dify)
**Purpose:** how to work in this repo without breaking design intent or trusting stale docs.
**Companion:** `AGENTS.md` (rules), `docs/project/DEVELOPMENT_RULES.md` (rendered conventions), `docs/project/CURRENT_STATE.md` (status).

## The one law: code is the source of truth, docs are the intent

- **For "what exists / what works":** believe the code. Verify by reading files and running tests.
- **For "what should exist":** believe `design/gdd/game-concept.md` + `AGENTS.md` §2/§3.
- **When code and a status document disagree:** do not silently "fix" either one. Record the mismatch in `docs/project/CURRENT_STATE.md` → "Known doc drift", then decide with the developer.

## Never do

1. **Never introduce a fail state, timer, energy, streak, currency, odds, rarity gate, purchase prompt, notification, ad, analytics SDK, or social feature.** Violations are defects, not preferences (`AGENTS.md` §2).
2. **Never make a drag the only path to an action** — every drag needs an equal-status tap (`AGENTS.md` §2.4).
3. **Never put load-bearing text in the UI.** Icons carry meaning; captions support (`AGENTS.md` §2.5).
4. **Never call legacy `UnityEngine.Input.*`.** Input System (New) only.
5. **Never enable URP Compatibility Mode** (removed in Unity 6.3).
6. **Never write to a full RenderTexture per frame for painting** — paint is mask/decal-based.
7. **Never add network I/O to the painting path.** Offline only.
8. **Never hand-author `.meta` files** — Unity generates them. (Note: the existing 31 `.cs` `.meta` files are already hand-authored 2-line stubs; do not "clean them up" without an ADR.)
9. **Never add a second runtime assembly** without an ADR.
10. **Never pass `-quit` with `-runTests`** or to a script that enters play mode.
11. **Never commit or "tidy up" the in-flight uncommitted feature** — the menu/playground/mouse-input/shader work is the developer's, not yours to touch.
12. **Never dump source code, the art bible (468 KB), or secrets into the knowledge base** — index curated summaries, keep git as the source of truth.

## Always do

1. **Read `docs/project/CURRENT_STATE.md` before claiming anything about status.**
2. **Cross-check every RAG/knowledge-base answer against the code** before acting on it (see below).
3. **Keep the knowledge base in sync** after doc edits: `python scripts/rag_sync.py --only docs/<path>`.
4. **Add a test when you add a behaviour.** The repo's own priorities are listed in `docs/roadmap/TODO.md`.
5. **Keep zero-allocation paths zero-allocation** — `CreatureStateAllocationTests` and `VisualBridgeTests` assert it.
6. **Follow the naming table** in `AGENTS.md` §6 (see `docs/project/DEVELOPMENT_RULES.md` for known deviations — do not replicate them).
7. **Respect the performance budget** (`AGENTS.md` §5): low-end Android is the floor, 60 fps target, never below 30, < 150 draw calls.

## Cross-check protocol (required for any status claim)

Knowledge-base retrieval returns *chunks of documents*, which can be stale. Before you act:

1. Run `python scripts/rag_query.py "<question>"` → get candidate claims + the document name they came from.
2. Open the cited file(s) in the repo and confirm the claim still holds (or is newer than the doc).
3. For "does X exist" questions, grep the code, not the docs: e.g. `grep -r "class SignatureMark" Assets/Scripts` will settle the Echo question faster than any document.
4. If the doc is wrong, fix the doc in the same change and re-sync it (`rag_sync.py --only …`).

Example that matters: RAG will tell you "the animation system is built and tested". That is true — **and** `PaintingRig.Build()` never instantiates it. Both halves are needed.

## When you change a document

1. Edit the file in the repo.
2. Record the change in `docs/changelog/DEVELOPMENT_LOG.md`.
3. Re-sync: `python scripts/rag_sync.py --only <path>`.
4. If the change invalidates an architectural decision, use the `/propagate-design-change` flow (ADR staleness check) rather than leaving ADRs silently wrong.

## Session hygiene

- Never end a session with modified game code and no `DEVELOPMENT_LOG.md` entry.
- Never leave `.env` staged — it is gitignored; `.env.example` is the committed file.
- Prefer a new commit over amending, and never force-push.
