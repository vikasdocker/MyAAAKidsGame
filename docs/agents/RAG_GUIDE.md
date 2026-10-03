# RAG guide — query & sync

**Doc type:** project knowledge (indexed into Dify)
**Last verified:** 2026-10-03
**Rule:** this document must never contain the API key. Keys live in `.env` (gitignored); `.env.example` holds placeholders.

## What this is

The repository is the **source of truth**. Dify holds a **retrieval index** of curated project documents so an agent can recover context across model/provider changes without re-reading the whole repo.

- **No LLM is involved in retrieval** — raw chunk retrieval only, so it costs nothing, works with any model provider, and cannot hallucinate an answer.
- **Git remains authoritative.** If the index and the repo disagree, the repo wins and the index needs a sync.

## Configuration

`.env` (gitignored) / `.env.example` (committed placeholders):

| Variable | Meaning |
|---|---|
| `DIFY_BASE_URL` | Dify origin, `http://localhost` (nginx on port 80) |
| `DIFY_API_KEY` | Dataset service-API token, prefix `dataset-` |
| `DIFY_DATASET_ID` | Knowledge base id |

Scripts read these from the environment first, then from a `.env` file in the repo root.

**Knowledge base:** `MyAAAKidsGame Project Knowledge` — `indexing_technique: high_quality`, embedding `openai/text-embedding-3-small` on provider `langgenius/openrouter/openrouter`, vector store Weaviate (`VECTOR_INDEX_NAME_PREFIX=Vector_index`), chunking: separator `\n\n` (paragraphs), 1024 max tokens, 50 overlap.

## Commands

```bash
# Query — prints chunks, scores, source document names
python scripts/rag_query.py "What systems are incomplete?"
python scripts/rag_query.py --top 8 --json "how does painting work"
python scripts/rag_query.py --markdown "what is the architecture"

# Sync — create/update index documents from docs/
python scripts/rag_sync.py                 # sync everything changed
python scripts/rag_sync.py --only docs/project/CURRENT_STATE.md
python scripts/rag_sync.py --status        # show manifest vs disk
python scripts/rag_sync.py --dry-run       # report planned actions, write nothing
```

Query length is capped at **250 characters** by Dify's retrieve endpoint; the script validates and trims.

## Endpoints used

| Purpose | Request |
|---|---|
| Retrieve | `POST /v1/datasets/{id}/retrieve` with `Authorization: Bearer <DIFY_API_KEY>`. Body may be just `{"query": "<=250 chars"}` (settings fall back to the dataset default). If `retrieval_model` is included it must be **complete** — Pydantic requires `search_method`, `reranking_enable` and `score_threshold_enabled` alongside `top_k`, otherwise HTTP 400 |
| Create document | `POST /v1/datasets/{id}/document/create-by-text` → `{document, batch}` |
| Update document | `POST /v1/datasets/{id}/documents/{doc_id}/update-by-text` |
| Indexing status | `GET /v1/datasets/{id}/documents/{batch}/indexing-status` — note the path segment is the **batch** id, not the document id |
| Response shape | `{query:{content}, records:[{score, segment:{content, document:{name,id}}, child_chunks[], files[], summary}]}` |

Console-side operations (creating the KB and its API key) need a logged-in console session with the `X-CSRF-Token` header; those are one-time setup steps, not part of daily use.

## What is indexed

15 curated documents, chunked by newline:

```
docs/project/        PROJECT_OVERVIEW, GAME_DESIGN, ARCHITECTURE, TECH_STACK,
                     DEVELOPMENT_RULES, CURRENT_STATE
docs/architecture/   systems, ADR_INDEX
docs/roadmap/        ROADMAP, COMPLETED, TODO
docs/agents/         OPENCODE_CONTEXT, CODING_RULES, RAG_GUIDE
docs/changelog/      DEVELOPMENT_LOG
```

**Deliberately not indexed:** the 468 KB `design/Art/art-bible.md` (summarized instead), all C# source (git is the index), `Library/`, binary assets, and anything containing secrets.

## Manifest

`.rag/manifest.json` (gitignored) maps each source path to its Dify document id and content hash. It lets the sync skip unchanged files, update changed ones, and avoid duplicates.

Delete it and the next sync will create fresh documents — so treat it as valuable local state. If the knowledge base is rebuilt from scratch, clear it first or you will get duplicate documents.

## Failure modes

| Symptom | Cause | Action |
|---|---|---|
| `records: []` with HTTP 200 | Index has no vectors — the dataset was indexed as `economy` (keyword-only) and later flipped to `high_quality` without backfill | Re-create documents with explicit `indexing_technique: high_quality` + embedding model, and confirm a Weaviate class `Vector_index_<dataset>_Node` appears |
| HTTP 402 `openrouter_credits` | OpenRouter LLM credits exhausted | Does **not** affect retrieval; only affects Dify LLM apps/workflows. Use Groq for LLM work |
| HTTP 404 `Documents not found` on indexing-status | Passed the document id where the **batch** id is expected | Use the `batch` returned by create/update |
| Console `GET /console/api/datasets/{id}/api-keys` → 500 | Dify bug (`ApiToken has no attribute 'dataset_id'`) | Use the workspace route `POST/GET /console/api/datasets/api-keys` |
| Retrieval returns results but they contradict the code | The document is stale | Fix the doc, then `rag_sync.py --only <path>` |
