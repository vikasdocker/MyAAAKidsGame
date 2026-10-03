# OpenCode context recovery

**Doc type:** project knowledge (indexed into Dify)
**Purpose:** the answer to *"I just opened this repo with a fresh model/context — what do I need to know, and how do I recover state?"*
**Last verified:** 2026-10-03

## 30-second orientation

You are working on **"Dab" (`MyAAAKidsGame`)** — a no-fail pet-painting game for ages 6–9, Unity 6000.3.11f1 + URP 17.3, one runtime assembly `Dab.Runtime`. The heart of the game (paint → creature reacts) **works**; the hook (marks become abilities), save/load, and audio **do not exist yet**.

Read in this order:

1. `AGENTS.md` — operating rules (but its §9 status table is stale; see below).
2. `docs/project/CURRENT_STATE.md` — what actually works, what's broken, what's missing. **This is the most reliable status document in the repo.**
3. `design/gdd/game-concept.md` — design source of truth.
4. `docs/architecture/ADR_INDEX.md` — decisions already made.

## Never assume — always check

| Question | Where the answer lives |
|---|---|
| What works right now? | `docs/project/CURRENT_STATE.md`, `docs/architecture/systems.md` |
| What should I build next? | `docs/roadmap/TODO.md` |
| Is this doc trustworthy? | `docs/project/CURRENT_STATE.md` → "Known doc drift" |
| What must never change? | `AGENTS.md` §2 (7 non-negotiable constraints) + §3 (4 pillars) |
| What does the code actually do? | The code. Always. Docs are stale-prone |

## Fresh-session query pack (run via the Dify RAG)

```bash
python scripts/rag_query.py "What is the current state of the game?"
python scripts/rag_query.py "What systems are incomplete or missing?"
python scripts/rag_query.py "What are the non-negotiable design constraints?"
python scripts/rag_query.py "What is the architecture and how are scenes wired?"
python scripts/rag_query.py "What is the highest priority next task?"
python scripts/rag_query.py "What bugs are currently known?"
python scripts/rag_query.py "Which documents are stale and untrustworthy?"
```

Then **cross-check the top hits against the code** before acting on them — see `docs/agents/CODING_RULES.md`.

## Hard traps in this repo

1. **The working tree contains one complete uncommitted feature** (menu + playground + mouse input + `GameplayLoopTests` + shader fix). Do not modify, revert, or commit it as a side effect. See `docs/project/CURRENT_STATE.md` → "HEAD vs working tree".
2. **HEAD is probably red.** The shader compile fix exists only uncommitted.
3. **`README.md` is badly stale** — it claims the repo is a greenfield scaffold with no code. Ignore its status table entirely.
4. **`AGENTS.md` claims to be the winning document in any conflict, but its own status table says the art bible and ADRs don't exist** when both do. Treat AGENTS.md as authoritative for *rules*, not for *status*.
5. **The animation subsystem is invisible** — built, generated, tested, and never instantiated. If you grep for `Animator` in runtime code you will conclude it doesn't exist; it does, it's just unwired.
6. **CI is not a safety net.** It runs one Python art-metrics script. It never compiles Unity.
7. **The PAINT button is broken** (`MainMenuBootstrap.cs:188` → `SCN_Playtest`, a scene that doesn't exist).

## Session-continuation checklist

Before ending a session, record in `docs/changelog/DEVELOPMENT_LOG.md`:
- what changed (files + one line each),
- what was verified (command + result),
- what is left undone and why,
- any doc/code mismatch discovered (add it to `CURRENT_STATE.md`).
