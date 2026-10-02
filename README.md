# MyAAAKidsGame

**3D pet simulator for kids 6–9** — high-end mobile (iOS + Android), Unity + URP.

A child paints a plain fluffy hatchling, adorns it with signature marks, and every
mark they make becomes an ability the creature performs back. No failing, no
losing, no punishing the player for leaving.

---

## Start Here

**Read [`AGENTS.md`](./AGENTS.md) before writing any code.** It contains the
non-negotiable design constraints, performance budgets, naming conventions, and
the four pillars.

The full design concept — including the MDA analysis, motivation profile, core
loops, scope tiers, and risk register — is in
**[`design/gdd/game-concept.md`](./design/gdd/game-concept.md)**.

---

## Status

| Item | State |
|---|---|
| Concept | ✅ Approved |
| Pillars | ✅ Four locked + five anti-pillars |
| Engine | ⏳ Unity + URP — **version not pinned** |
| Art bible | ⏳ Not started |
| Per-system GDDs | ⏳ Not started |
| Architecture / ADRs | ⏳ Not started |
| Code | ✅ Folder + assembly structure only |

**This is a greenfield scaffold.** Folders are structured but empty. Unity
generates `.meta` files on first project open — do not hand-author them.

---

## Project Layout

```
MyAAAKidsGame/
├── AGENTS.md                  # ← read first. Constraints & conventions.
├── .gitignore
├── design/
│   └── gdd/game-concept.md    # ← the concept. Source of truth.
├── docs/                      # engine reference docs (/setup-engine)
├── production/                # stage.txt, review-mode.txt
│
└── Assets/
    ├── Scripts/
    │   ├── Runtime/           # Dab.Runtime.asmdef — one runtime assembly
    │   │   ├── Core/          #   boot, DI, app state, scene flow
    │   │   ├── Pet/           #   creature state machine, reactions
    │   │   ├── Painting/      #   paint system, signature marks
    │   │   ├── Abilities/     #   signature-mark → ability binding ("Echo")
    │   │   ├── Minigames/     #   the competence channel
    │   │   ├── Save/          #   authored-design persistence
    │   │   ├── Input/         #   touch abstraction, gestures
    │   │   ├── UI/            #   icon-first UI, palette, parent gate
    │   │   ├── Audio/         #   reactive audio
    │   │   └── Procedural/    #   cheap creature variation
    │   ├── Editor/            # Dab.Editor.asmdef — editor tooling
    │   └── Tests/             # Dab.Tests.asmdef — PlayMode + EditMode
    │
    ├── Shaders/               # Lit / Unlit / Paint / Fur / UI / PostProcessing
    ├── Art/                   # Characters/{Base,Variants}, Environment, VFX, UI
    ├── Prefabs/               # Pet, Environment, UI, VFX
    ├── Animations/            # Creature, Player, UI
    ├── Audio/                 # Music, SFX, Voice, Ambience
    ├── Materials/  Textures/  TextureSamples/
    ├── Settings/              # URP, Input, Quality
    └── Scenes/                # SCN_Boot, SCN_Habitat — not yet created
```

---

## Four Pillars

1. **Authored, Not Decorated** — every choice the child makes is visible on the
   creature, *does something*, and is kept forever.
2. **Nothing Can Go Wrong** — a 5-year-old can never fail or lose anything.
3. **Rich From Restraint** — visual ambition from art direction, never content volume.
4. **Two Tools, Both Toys** — freehand for the joyful part, deliberate placement
   for the meaningful part.

Each pillar has a design test that settles real arguments. See `AGENTS.md` §3.

---

## Critical Paths

**Recommended order — do not skip ahead:**

1. `/setup-engine` — pin the Unity version, populate reference docs
2. `/art-bible` — formalize the visual identity *(gates all asset production)*
3. `/design-review design/gdd/game-concept.md`
4. `/map-systems` — decompose into systems with dependencies
5. `/design-system <system>` — one GDD per system
6. `/review-all-gdds` → `/gate-check`
7. `/create-architecture` → `/architecture-decision` (×N)
8. `/prototype freehand-painting` — **highest technical risk, do this first**

---

## Biggest Risks

| Risk | Severity |
|---|---|
| Freehand 3D painting doesn't feel good on mid-range mobile | **HIGH** |
| Content scope creep in a saturated genre | **HIGH** |
| Draw calls / shader variants blow the mobile budget as marks multiply | **HIGH** |
| Child loses interest in ~2 weeks (needs the "Echo" loop to land) | **HIGH** |

> **Before any further feature work:** prove freehand 3D painting is pleasant on
> a real mid-range Android device. If it isn't, the concept changes and every
> downstream decision was premature.

---

## Hard Constraints (summary)

- **Never:** fail states, timers, streaks, currency, ads, IAP, rarity/odds,
  trading, chat, any social feature, purchase prompts during play, third-party
  analytics SDKs.
- **Always:** every drag gesture has an equal-status tap fallback; nothing in the
  UI requires reading; the creature's base colour stays neutral so the child's
  paint is the most vivid thing on screen.