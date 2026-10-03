# Development log

**Doc type:** project knowledge (indexed into Dify)
**Purpose:** append-only record of what changed, what was verified, and what is left. Written per session so a fresh context can reconstruct state without re-auditing.
**Convention:** newest entry at the bottom. One entry per session. Never rewrite history here.

---

## 2026-10-02 — Project bootstrap (git history)

Commits `dbc8d48`, `92b9791`, `08e2c7d`, `c6e7b8d`, `8cef34e` — see `docs/roadmap/COMPLETED.md` for the per-commit breakdown. Everything committed so far landed on this single day.

---

## 2026-10-03 — In-flight (uncommitted) feature

**Changed (not committed):** `FluidTouchInputManager.cs` (+246, mouse-parity layer), `CreaturePlaytestBootstrap.cs` (−74, now calls `PaintingRig.Build`), `CreatureCanvas.shader` (+48, vertex `SAMPLE_TEXTURE2D_LOD` + ShadowCaster CBUFFER fix), `EditorBuildSettings.asset` (3 scenes); added `Runtime/Core/PaintingRig.cs`, `Runtime/Minigames/*`, `Runtime/UI/*`, `Editor/Gameplay/GameplayScenesSetup.cs`, `Assets/Scenes/SCN_MainMenu.unity`, `Assets/Scenes/SCN_Playground.unity`, `Tests/GameplayLoopTests.cs`.

**State:** complete and self-consistent. `HEAD` is probably red without it (the shader fix and `HandleMousePress` API are required by unmodified tests).

**Known defect carried in this work:** `MainMenuBootstrap.cs:188` → `LoadScene("SCN_Playtest")`, a scene that does not exist (actual: `Playtest`). Not covered by tests.

---

## 2026-10-03 — Full repository audit + Dify knowledge layer

**Audited (read-only):** all 31 `.cs` files (9,540 lines), 6 test files (43 tests), 3 scenes, 2 shaders, `ProjectSettings/`, `Packages/manifest.json`, git history, CI workflow, and every `.md` file. Nothing in the game was modified by the audit.

**Findings recorded** in `docs/project/CURRENT_STATE.md` (working-tree state, 4 broken/aspirational categories, 12 tracked doc-drift items) and `docs/architecture/systems.md` (per-system status table).

**Dify knowledge base created:**

| Item | Value |
|---|---|
| Knowledge base | `MyAAAKidsGame Project Knowledge` (`fae91071-9181-4415-9fe6-de47f871e5b7`) |
| Indexing | `high_quality`, embedding `openai/text-embedding-3-small` via `langgenius/openrouter/openrouter`, vector store Weaviate |
| API key | dataset-scoped `dataset-…` token, stored only in `.env` (gitignored) |
| Indexed docs | the 15 documents listed in `docs/agents/RAG_GUIDE.md` |
| Tooling | `scripts/rag_query.py`, `scripts/rag_sync.py`, `.rag/manifest.json` |
| OpenCode wiring | `.opencode/skills/project-knowledge/SKILL.md`, `opencode.json`, `AGENTS.md` §11 |

**Root cause found and worked around:** the pre-existing dataset `43a50efc…` was indexed as `economy` (keyword-only), so Weaviate holds no vectors and semantic search returns 0 records even though documents report `completed`. Worker log evidence: *"Summary index generation skipped for dataset …: indexing_technique=economy (not 'high_quality')"*. The new knowledge base was created `high_quality` from the start and verified end-to-end: document indexing creates the Weaviate class `Vector_index_<dataset>_Node`, and retrieval returns scored records.

**Verified:** Dify retrieval returns relevant chunks with scores; query script and sync script both run against the live instance.

**Validation evidence (2026-10-03):**
1. `"What is this game?"` → `PROJECT_OVERVIEW.md` chunk, score 0.70.
2. `"What is the current architecture?"` → `ARCHITECTURE.md` bootstrap-in-Awake section, score 0.43.
3. `"What systems are currently incomplete?"` → `docs/roadmap/TODO.md` + full `systems.md` status rows, score 0.51 / 0.39.
4. `"How should I implement a new creature ability?"` → `GAME_DESIGN.md` Echo section + implementation-status chunk, then cross-checked in code: `AbilityUnlockState.cs:68` is a bare `int AbilityId` and `class SignatureMark` has zero hits — RAG claim confirmed.
5. This file was edited, re-synced with `rag_sync.py --only`, and re-queried successfully.

Chunking was changed from `\n` to `\n\n` after the first sync: newline splitting produced heading-only chunks that ranked well but carried no content. All 15 documents were re-indexed with `--force`.

**Left undone:** the pre-existing stale dataset `43a50efc…` (AGENTS.md + README.md, no vectors) was left untouched.

---
