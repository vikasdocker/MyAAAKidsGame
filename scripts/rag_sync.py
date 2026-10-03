#!/usr/bin/env python3
"""Sync curated project documents into the MyAAAKidsGame Dify knowledge base.

The git repository is the source of truth; Dify holds a retrieval index of these
files. Run this after editing any of them:

    python scripts/rag_sync.py                          # sync everything changed
    python scripts/rag_sync.py --only docs/project/CURRENT_STATE.md
    python scripts/rag_sync.py --status                 # manifest vs disk
    python scripts/rag_sync.py --dry-run                # report, write nothing

State lives in `.rag/manifest.json` (path -> document id + content hash).
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import sys
import time
import urllib.error
import urllib.request
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
MANIFEST_PATH = REPO_ROOT / ".rag" / "manifest.json"

# The curated knowledge set. Paths are relative to the repository root and are
# used verbatim as the document name inside Dify so retrieval results cite the
# exact file to open.
DOCUMENT_PATHS = [
    "docs/project/PROJECT_OVERVIEW.md",
    "docs/project/GAME_DESIGN.md",
    "docs/project/ARCHITECTURE.md",
    "docs/project/TECH_STACK.md",
    "docs/project/DEVELOPMENT_RULES.md",
    "docs/project/CURRENT_STATE.md",
    "docs/architecture/systems.md",
    "docs/architecture/ADR_INDEX.md",
    "docs/roadmap/ROADMAP.md",
    "docs/roadmap/COMPLETED.md",
    "docs/roadmap/TODO.md",
    "docs/agents/OPENCODE_CONTEXT.md",
    "docs/agents/CODING_RULES.md",
    "docs/agents/RAG_GUIDE.md",
    "docs/changelog/DEVELOPMENT_LOG.md",
]

PROCESS_RULE = {
    "mode": "custom",
    "rules": {
        "pre_processing_rules": [
            {"id": "remove_extra_spaces", "enabled": True},
            {"id": "remove_urls_emails", "enabled": False},
        ],
        # Paragraph splitting keeps headings attached to real content instead of
        # producing heading-only chunks that rank well but say nothing.
        "segmentation": {"separator": "\n\n", "max_tokens": 1024, "chunk_overlap": 50},
    },
}
EMBEDDING_MODEL = "openai/text-embedding-3-small"
EMBEDDING_PROVIDER = "langgenius/openrouter/openrouter"
INDEX_POLL_SECONDS = 3
INDEX_POLL_TIMEOUT = 180


class SyncError(RuntimeError):
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
        raise SyncError("Missing configuration: " + ", ".join(missing) + ". Copy .env.example to .env first.")
    return values


def request_json(url: str, payload: dict | None, token: str, method: str = "POST") -> dict:
    headers = {"Authorization": f"Bearer {token}"}
    data = None
    if payload is not None:
        headers["Content-Type"] = "application/json"
        data = json.dumps(payload).encode("utf-8")
    request = urllib.request.Request(url, data=data, headers=headers, method=method)
    try:
        with urllib.request.urlopen(request, timeout=90) as response:
            body = response.read().decode("utf-8")
            return json.loads(body) if body else {}
    except urllib.error.HTTPError as exc:
        body = exc.read().decode("utf-8", "replace")
        raise SyncError(f"HTTP {exc.code} {method} {url}: {body[:600]}") from exc
    except urllib.error.URLError as exc:
        raise SyncError(f"Cannot reach {url}: {exc.reason}") from exc


def sha256_of(text: str) -> str:
    return hashlib.sha256(text.encode("utf-8")).hexdigest()


def load_manifest() -> dict:
    if MANIFEST_PATH.is_file():
        try:
            return json.loads(MANIFEST_PATH.read_text(encoding="utf-8"))
        except json.JSONDecodeError as exc:
            raise SyncError(f"Corrupt manifest {MANIFEST_PATH}: {exc}") from exc
    return {"documents": {}}


def save_manifest(manifest: dict) -> None:
    MANIFEST_PATH.parent.mkdir(parents=True, exist_ok=True)
    MANIFEST_PATH.write_text(json.dumps(manifest, indent=2, sort_keys=True) + "\n", encoding="utf-8")


def wait_for_indexing(config: dict[str, str], batch: str) -> str:
    base = config["DIFY_BASE_URL"].rstrip("/")
    deadline = time.time() + INDEX_POLL_TIMEOUT
    last = "unknown"
    while time.time() < deadline:
        result = request_json(
            f"{base}/v1/datasets/{config['DIFY_DATASET_ID']}/documents/{batch}/indexing-status",
            None,
            config["DIFY_API_KEY"],
            method="GET",
        )
        entries = result.get("data") or []
        if entries:
            statuses = {entry.get("indexing_status", "unknown") for entry in entries}
            last = ", ".join(sorted(statuses))
            if statuses <= {"completed"}:
                return "completed"
            if "failed" in statuses or "error" in statuses:
                detail = "; ".join(
                    str(entry.get("error") or "") for entry in entries if entry.get("error")
                )
                return f"failed: {detail}" if detail else "failed"
        time.sleep(INDEX_POLL_SECONDS)
    return f"timeout (last status: {last})"


def create_document(config: dict[str, str], name: str, text: str) -> str:
    payload = {
        "name": name,
        "text": text,
        "indexing_technique": "high_quality",
        "embedding_model": EMBEDDING_MODEL,
        "embedding_model_provider": EMBEDDING_PROVIDER,
        "doc_form": "text_model",
        "doc_language": "English",
        "process_rule": PROCESS_RULE,
    }
    base = config["DIFY_BASE_URL"].rstrip("/")
    result = request_json(f"{base}/v1/datasets/{config['DIFY_DATASET_ID']}/document/create-by-text",
                          payload, config["DIFY_API_KEY"])
    document_id = (result.get("document") or {}).get("id")
    batch = result.get("batch")
    if not document_id or not batch:
        raise SyncError(f"Unexpected create response for {name}: {json.dumps(result)[:400]}")
    print(f"  created {name} -> {document_id}")
    return f"{document_id}\x00{batch}"


def update_document(config: dict[str, str], document_id: str, name: str, text: str) -> str:
    payload = {
        "name": name,
        "text": text,
        "doc_form": "text_model",
        "doc_language": "English",
        "process_rule": PROCESS_RULE,
    }
    base = config["DIFY_BASE_URL"].rstrip("/")
    result = request_json(
        f"{base}/v1/datasets/{config['DIFY_DATASET_ID']}/documents/{document_id}/update-by-text",
        payload,
        config["DIFY_API_KEY"],
    )
    batch = result.get("batch")
    if not batch:
        raise SyncError(f"Unexpected update response for {name}: {json.dumps(result)[:400]}")
    print(f"  updated {name}")
    return batch


def plan(paths: list[str], manifest: dict, force: bool = False) -> list[tuple[str, str, str | None]]:
    """Return (action, path, document_id) triples: create | update | skip | missing."""
    entries = manifest.setdefault("documents", {})
    outcome: list[tuple[str, str, str | None]] = []
    for relative in paths:
        source = REPO_ROOT / relative
        if not source.is_file():
            outcome.append(("missing", relative, None))
            continue
        digest = sha256_of(source.read_text(encoding="utf-8"))
        entry = entries.get(relative)
        if entry is None:
            outcome.append(("create", relative, None))
        elif entry.get("sha256") != digest:
            outcome.append(("update", relative, entry.get("document_id")))
        elif force:
            outcome.append(("update", relative, entry.get("document_id")))
        else:
            outcome.append(("skip", relative, entry.get("document_id")))
    return outcome


def show_status(paths: list[str], manifest: dict) -> None:
    print(f"Manifest: {MANIFEST_PATH.relative_to(REPO_ROOT) if MANIFEST_PATH.exists() else '(not created)'}")
    print(f"{'action':8} {'path':48} document_id")
    for action, relative, document_id in plan(paths, manifest):
        print(f"{action:8} {relative:48} {document_id or '-'}")


def main(argv: list[str] | None = None) -> int:
    for stream in (sys.stdout, sys.stderr):
        try:
            stream.reconfigure(encoding="utf-8", errors="replace")
        except (AttributeError, OSError):
            pass

    parser = argparse.ArgumentParser(description="Sync docs/ into the Dify knowledge base.")
    parser.add_argument("--only", action="append", metavar="PATH",
                        help="sync only this repository-relative path (repeatable)")
    parser.add_argument("--status", action="store_true", help="show manifest vs disk and exit")
    parser.add_argument("--dry-run", action="store_true", help="report planned actions, write nothing")
    parser.add_argument("--force", action="store_true",
                        help="re-index even when the file hash is unchanged "
                             "(use after changing chunking rules)")
    args = parser.parse_args(argv)

    paths = args.only or DOCUMENT_PATHS
    unknown = [p for p in paths if p not in DOCUMENT_PATHS]
    if unknown:
        print("error: not part of the curated knowledge set: " + ", ".join(unknown), file=sys.stderr)
        print("Known paths:\n  " + "\n  ".join(DOCUMENT_PATHS), file=sys.stderr)
        return 2

    try:
        manifest = load_manifest()
    except SyncError as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 2

    if args.status:
        show_status(paths, manifest)
        return 0

    actions = plan(paths, manifest, force=args.force)
    if args.dry_run:
        show_status(paths, manifest)
        return 0

    try:
        config = load_config()
    except SyncError as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 2

    entries = manifest.setdefault("documents", {})
    counts = {"created": 0, "updated": 0, "skipped": 0, "missing": 0, "failed": 0}
    failures: list[str] = []

    for action, relative, document_id in actions:
        if action == "skip":
            counts["skipped"] += 1
            print(f"  unchanged {relative}")
            continue
        if action == "missing":
            counts["missing"] += 1
            print(f"  MISSING   {relative}")
            failures.append(relative)
            continue

        source = REPO_ROOT / relative
        text = source.read_text(encoding="utf-8")
        try:
            if action == "create":
                combined = create_document(config, relative, text)
                document_id, batch = combined.split("\x00", 1)
                counts["created"] += 1
            else:
                if not document_id:
                    counts["failed"] += 1
                    failures.append(f"{relative} (no document_id in manifest)")
                    continue
                batch = update_document(config, document_id, relative, text)
                counts["updated"] += 1
            status = wait_for_indexing(config, batch)
            if status != "completed":
                counts["failed"] += 1
                failures.append(f"{relative} (indexing: {status})")
                print(f"    indexing -> {status}")
            else:
                print("    indexing -> completed")
            entries[relative] = {"document_id": document_id, "sha256": sha256_of(text)}
        except SyncError as exc:
            counts["failed"] += 1
            failures.append(f"{relative} ({exc})")
            print(f"  FAILED    {relative}: {exc}", file=sys.stderr)

    save_manifest(manifest)
    print(
        "\nSummary: "
        + ", ".join(f"{counts[key]} {key}" for key in ("created", "updated", "skipped", "missing", "failed"))
    )
    if failures:
        print("Failures:")
        for item in failures:
            print(f"  - {item}")
        return 1
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
