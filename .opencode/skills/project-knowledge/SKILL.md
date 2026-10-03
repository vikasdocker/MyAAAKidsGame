---
name: project-knowledge
description: Query the MyAAAKidsGame Dify knowledge base (persistent RAG) to recover project context across model/provider changes, cross-check retrieved claims against the code, and sync edited documents back into the index. Use when starting a session in this repo, when asked about project status/architecture/roadmap/rules, or after editing any file under docs/.
---

# Project knowledge (Dify RAG)

The git repository is the source of truth. Dify holds a retrieval index of 15
curated project documents so context survives a model or provider switch.
Retrieval is **raw chunk search with no LLM** — it costs nothing and cannot
hallucinate, but it can be *stale*.

## When to use

- Fresh session / lost context: run the query pack below before reading the whole repo.
- Questions about status, architecture, roadmap, rules, or known bugs.
- After you edit anything under `docs/` — sync it back (see below).

## Query

```bash
python scripts/rag_query.py "What is the current state of the game?"
python scripts/rag_query.py "What systems are incomplete or missing?"
python scripts/rag_query.py "What are the non-negotiable design constraints?"
python scripts/rag_query.py "What is the architecture and how are scenes wired?"
python scripts/rag_query.py "What is the highest priority next task?"
python scripts/rag_query.py "What bugs are currently known?"
python scripts/rag_query.py --top 8 --json "how does painting work"
```

Configuration comes from `.env` (gitignored; template in `.env.example`):
`DIFY_BASE_URL`, `DIFY_API_KEY`, `DIFY_DATASET_ID`.

## Cross-check before acting (mandatory)

Retrieved chunks cite the source document name — open that file and confirm the
claim still holds. For "does X exist" questions, grep the code rather than the docs.

Two traps that make naive trust wrong here:

1. The working tree holds a complete **uncommitted** feature (menu + playground +
   mouse input + `GameplayLoopTests` + a shader fix); HEAD is probably red
   without it. Never modify or commit it as a side effect.
2. Some systems are **built but unwired** — the animation subsystem is fully
   generated and tested yet `PaintingRig.Build()` never instantiates it.

Full rules: `docs/agents/CODING_RULES.md`. Status truth: `docs/project/CURRENT_STATE.md`.

## Sync after editing docs

```bash
python scripts/rag_sync.py --only docs/project/CURRENT_STATE.md
python scripts/rag_sync.py              # everything that changed
python scripts/rag_sync.py --status     # manifest vs disk
python scripts/rag_sync.py --dry-run
```

State lives in `.rag/manifest.json` (gitignored). If you rebuild the knowledge
base from scratch, clear it or you will get duplicate documents.

## Index

Knowledge base `MyAAAKidsGame Project Knowledge` on the local Dify instance
(`http://localhost`), `high_quality` indexing, embedding
`openai/text-embedding-3-small` via OpenRouter, Weaviate vector store, chunking
`\n` / 1024 tokens / 50 overlap.

Endpoint details, response shape, and failure modes: `docs/agents/RAG_GUIDE.md`.
