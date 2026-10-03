#!/usr/bin/env python3
"""Query the MyAAAKidsGame Dify knowledge base (raw chunk retrieval, no LLM).

Usage:
    python scripts/rag_query.py "What systems are incomplete?"
    python scripts/rag_query.py --top 8 --json "how does painting work"
    python scripts/rag_query.py --markdown "what is the architecture"

Configuration comes from the environment, falling back to a `.env` file in the
repository root (see `.env.example`). The API key never appears in output.
"""

from __future__ import annotations

import argparse
import json
import os
import sys
import urllib.error
import urllib.request
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
# Dify's retrieve endpoint rejects queries longer than 250 characters.
QUERY_MAX = 250


class RagError(RuntimeError):
    """Raised when Dify is unreachable or answers with an error."""


def load_config() -> dict[str, str]:
    env_file = REPO_ROOT / ".env"
    values: dict[str, str] = {}
    if env_file.is_file():
        for raw_line in env_file.read_text(encoding="utf-8").splitlines():
            line = raw_line.strip()
            if not line or line.startswith("#") or "=" not in line:
                continue
            key, _, value = line.partition("=")
            values[key.strip()] = value.strip().strip('"').strip("'")
    for key in ("DIFY_BASE_URL", "DIFY_API_KEY", "DIFY_DATASET_ID"):
        if os.environ.get(key):
            values[key] = os.environ[key].strip()
    missing = [k for k in ("DIFY_BASE_URL", "DIFY_API_KEY", "DIFY_DATASET_ID") if not values.get(k)]
    if missing:
        raise RagError(
            "Missing configuration: " + ", ".join(missing) + ". Set them in the environment or in .env "
            f"(copy .env.example from {REPO_ROOT})."
        )
    return values


def post_json(url: str, payload: dict, token: str) -> dict:
    request = urllib.request.Request(
        url,
        data=json.dumps(payload).encode("utf-8"),
        headers={"Content-Type": "application/json", "Authorization": f"Bearer {token}"},
        method="POST",
    )
    try:
        with urllib.request.urlopen(request, timeout=60) as response:
            return json.loads(response.read().decode("utf-8"))
    except urllib.error.HTTPError as exc:
        body = exc.read().decode("utf-8", "replace")
        raise RagError(f"HTTP {exc.code} from {url}: {body[:600]}") from exc
    except urllib.error.URLError as exc:
        raise RagError(f"Cannot reach {url}: {exc.reason}") from exc


def query(config: dict[str, str], text: str, top_k: int) -> dict:
    url = f"{config['DIFY_BASE_URL'].rstrip('/')}/v1/datasets/{config['DIFY_DATASET_ID']}/retrieve"
    # Dify validates retrieval_model as a whole object: when present it requires
    # search_method, reranking_enable and score_threshold_enabled.
    payload = {
        "query": text,
        "retrieval_model": {
            "search_method": "semantic_search",
            "reranking_enable": False,
            "top_k": top_k,
            "score_threshold_enabled": False,
        },
    }
    return post_json(url, payload, config["DIFY_API_KEY"])


def summarize(content: str, limit: int = 220) -> str:
    flat = " ".join(content.split())
    return flat if len(flat) <= limit else flat[: limit - 1] + "\u2026"


def main(argv: list[str] | None = None) -> int:
    # Windows consoles default to cp1252, which cannot encode arrows/quotes
    # that appear in the knowledge base.
    for stream in (sys.stdout, sys.stderr):
        try:
            stream.reconfigure(encoding="utf-8", errors="replace")
        except (AttributeError, OSError):  # non-reconfigurable stream
            pass

    parser = argparse.ArgumentParser(description="Retrieve project-knowledge chunks from Dify.")
    parser.add_argument("query", help="question to ask (max 250 characters)")
    parser.add_argument("--top", type=int, default=5, help="number of chunks to return (default 5)")
    parser.add_argument("--json", action="store_true", help="print raw JSON")
    parser.add_argument("--markdown", action="store_true", help="print markdown")
    args = parser.parse_args(argv)

    text = " ".join(args.query.split())
    if len(text) > QUERY_MAX:
        print(f"warning: query truncated from {len(text)} to {QUERY_MAX} characters", file=sys.stderr)
        text = text[:QUERY_MAX]

    try:
        config = load_config()
        result = query(config, text, args.top)
    except RagError as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 2

    records = result.get("records", [])
    if args.json:
        print(json.dumps(result, indent=2, ensure_ascii=False))
        return 0

    if args.markdown:
        print(f"## Retrieved {len(records)} chunk(s)\n")
        print(f"**Query:** {result.get('query', {}).get('content', text)}\n")
        for index, record in enumerate(records, start=1):
            segment = record.get("segment", {})
            document = segment.get("document", {})
            score = record.get("score")
            print(f"### {index}. `{document.get('name', 'unknown')}` (score {score})\n")
            print(segment.get("content", "").strip() + "\n")
        if not records:
            print("_No matching chunks. The index may be empty or the document may be out of sync._")
        return 0

    print(f"Query:  {text}")
    print(f"Dataset: {config['DIFY_DATASET_ID']}  records: {len(records)}")
    if not records:
        print("  (no matching chunks - run `python scripts/rag_sync.py --status` to inspect the index)")
        return 0
    for index, record in enumerate(records, start=1):
        segment = record.get("segment", {})
        document = segment.get("document", {})
        print(f"\n[{index}] {document.get('name', 'unknown')}  score={record.get('score')}")
        print(f"    {summarize(segment.get('content', ''))}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
