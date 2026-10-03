# Project Overview — MyAAAKidsGame ("Dab")

**Doc type:** project knowledge (indexed into Dify)
**Last verified against code:** 2026-10-03
**Source of truth:** the git repository. If this document and the code disagree, verify the code and record the mismatch under "Known doc drift" in `docs/project/CURRENT_STATE.md`.

## What the game is

A **3D pet simulator for kids aged 6–9** on **high-end mobile (iOS + Android)**, built in Unity with the Universal Render Pipeline.

The child paints a plain fluffy hatchling, adorns it with signature marks, and **every mark they make becomes an ability the creature performs back** (the "Echo" loop). The child never fails, never loses, and is never punished for leaving.

- Working title: **"Dab"** (not final). Product name in `ProjectSettings.asset` is `DabAAAKidsGame`.
- Full concept: `design/gdd/game-concept.md` (24 KB, 529 lines — the design source of truth).
- Governing agent instructions: `AGENTS.md` (root).
- Audience: ages 6–9, plus a parent/guardian who sets up the device.

## Non-negotiable constraints (from AGENTS.md §2 — a violation is a defect, not a preference)

1. The child can never fail, get stuck, lose progress, or be punished for returning. No fail states, timers, energy, streaks, currency, ads, or IAP.
2. Nothing is rarity-gated, random, or loot-boxed. No odds mechanic anywhere.
3. No social features — no trading, gifting, chat, or player-to-player exchange. Do not add a networking layer for gameplay.
4. **Every drag gesture must have an equal-status tap fallback.** The tap path is first-class, not a degraded accessibility mode.
5. Nothing in the UI may require reading. Text is supportive, never load-bearing.
6. No purchase prompts, notifications, or interstitials during play.
7. Zero third-party analytics or ad SDKs (App Store / Play Store kids-category compliance). Verify before adding any package.

## Four pillars (AGENTS.md §3) — when two options conflict, the pillar wins

1. **Authored, Not Decorated.** Every choice the child makes is visible on the creature, does something, and is kept permanently. Test: "Does this pattern enable a move?" If not, give it a move or cut it.
2. **Nothing Can Go Wrong.** Test: "Can this fail state become a retry that's better than the first attempt?" If not, remove the fail state.
3. **Rich From Restraint.** Visual ambition comes from art direction, never content volume. Test: "Can this creature be a skinned variant of an existing mesh?" If yes, do that.
4. **Two Tools, Both Toys.** Freehand for the joyful part, deliberate placement for the meaningful part. Test: "Does this make freehand less freehand to satisfy precision?" If yes, use bigger hit volumes or forgiving snapping instead.

## Core loop (short form)

Paint → the marks become abilities → the creature performs them back → the child pets/feeds → the creature reacts → repeat. Session length target 5–15 minutes.

## Current one-line status

A **working prototype of the painting core and creature state machine**, plus a main-menu/playground loop that is complete in the working tree but **not yet committed**. The central "Echo" ability binding, save/load, and audio **do not exist in code yet**. See `docs/project/CURRENT_STATE.md` for the evidence-backed system-by-system table.

## Repository facts

| Item | Value |
|---|---|
| Path | `C:\Users\vikas\OneDrive\Desktop\MyAAAKidsGame` |
| Remote | `https://github.com/vikasdocker/MyAAAKidsGame.git` (branch `main`) |
| Engine | Unity 6000.3.11f1 (Unity 6.3 LTS) |
| Commits | 5, all on 2026-10-02 |
| Tracked C# | 31 files, 9,540 lines |
| Tests | 43 test methods across 6 files |
| Scenes | `Playtest.unity`, `SCN_MainMenu.unity`, `SCN_Playground.unity` |
| CI | `.github/workflows/verify-pipeline.yml` — runs `Tools/verify-art-metrics.py` only (no Unity compile, no tests) |

## Key documents

| Purpose | Path |
|---|---|
| Governing instructions | `AGENTS.md` |
| Design source of truth | `design/gdd/game-concept.md` |
| Art bible (468 KB) | `design/Art/art-bible.md` |
| Machine-readable palette | `design/Art/palette.json` (CI-gated) |
| Architecture decisions | `docs/architecture/adr-*.md` |
| Technical preferences | `docs/framework/technical-preferences.md` |
| Engine version risk notes | `docs/engine-reference/unity/VERSION.md` |
| Systems status | `docs/project/CURRENT_STATE.md`, `docs/architecture/systems.md` |
