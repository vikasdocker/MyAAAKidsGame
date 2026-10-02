# Game Concept Document — *TBD (working title: "Dab")*

| Field | Value |
|---|---|
| **Project** | MyAAAKidsGame (working title — final TBD) |
| **Genre** | 3D Pet Simulator / Creative Sandbox |
| **Target Platform** | High-end mobile (iOS and Android) |
| **Target Audience** | Ages 6–9 (design intent covers 5–10; see Audience Reality Check) |
| **Primary Engine** | Unity — Universal Render Pipeline (URP) |
| **Camera / Presentation** | Third-person 3D, stylized bright, single hero creature |
| **Session Length** | 5–15 minutes |
| **Session Cadence** | Daily ritual, 1–3 short visits |
| **Orientation** | Portrait-first, one-thumb reachable |
| **Network** | Offline-first, no social layer, no trading, no chat |
| **Monetization** | One-time premium purchase. No IAP, no ads, no currency, no timers. |
| **Estimated Scope** | Large (9–14 months, solo first-time developer) |
| **Review Mode** | lean |
| **Document Status** | Concept approved — not yet design-reviewed |
| **Last Updated** | Initial concept pass |

---

## 1. Core Identity

### Elevator Pitch

> A plain fluffy hatchling learns everything from you. You paint it, adorn it, and
> every mark you make becomes something it can actually **do**.

**10-second test:** A child picks up the phone, paints stripes on a round fluffy
creature, and then the creature uses those stripes to do a special move. It has
never done that before because nobody else drew those stripes.

### Core Verb

**Adorn** — make it yours, then watch it use what you made.

### Core Fantasy

> *"I made this thing, and it can do this because of I made it."*

The emotional payload is **pride of authorship**, not power fantasy. The child
should want to show someone their creature specifically because it is *theirs* —
an artifact of their decisions, not a trophy they were issued.

### Unique Hook

> **Like a pet simulator, AND ALSO the customization you design becomes actual
> pet abilities and signature moves.**

Every other pet sim ends at decoration. This one converts authorship into
capability: a horn swirl you painted is what enables its charge. The creature's
moveset is a record of what its owner invented.

### One-Line Comparison

| Reference | What we take | What we deliberately reject |
|---|---|---|
| Toca Life World | Creative expression, zero failure, sandbox comfort | No pressure, no collection chase — we add stakes |
| Adopt Me! | The *pull* of wanting something badly | All rarity pressure, trading, social comparison, monetized scarcity |
| Nintendogs / Pou | Tactile caretaking loop, ambient companionship | The emptiness of a loop with no authored payoff |

---

## 2. Design Pillars

Four locked pillars. Each has a design test — a real decision it settles.

### Pillar 1 — Authored, Not Decorated

**Every choice the child makes is visible on the creature, does something, and is
kept permanently. Nothing is cosmetic-only. Nothing is temporary. Nothing is
resettable to a default the child didn't choose.**

- *Design test:* Debating whether a pattern can be pure decoration — **we ask
  what move it enables.** If it enables nothing, either give it a move or cut it.
- *Design test:* Debating whether to add seasonal/event skins — **we ask whether
  the event skin grants a bound move.** If not, it is a pillar violation.

### Pillar 2 — Nothing Can Go Wrong

**A 5-year-old can never fail, get stuck, lose progress, or be punished for
coming back. Precision is opt-in, never required.**

- *Design test:* Debating whether a minigame needs a fail state — **we replace
  the fail state with a retry that is faster and better than the first attempt.**
- *Design test:* Debating whether missing days should have consequences — **we
  make the return greeting scale with absence length, rewarding, never guilt-based.**

### Pillar 3 — Rich From Restraint

**Visual ambition comes from art direction, never from content volume. One
excellent creature framework; every additional creature is a low-cost variant.**

- *Design test:* Debating whether to model a new creature from scratch — **we ask
  whether the design can be a skinned variant of the existing mesh.** If yes, we
  do that instead.
- *Design test:* Debating texture resolution vs. shader complexity — **we buy
  quality with lighting and shading, not bytes.**

### Pillar 4 — Two Tools, Both Toys

**Freehand flow for the joyful part. Deliberate placement for the meaningful
part. Both are first-class; neither is a lesser mode.**

- *Design test:* Debating whether the paint tool needs precision modes — **we add
  forgiving hit volumes instead, keeping freehand freehand.**
- *Design test:* Debating whether signature marks should be freehand too — **we
  keep them as discrete placed choices, because the binding to a move must be
  legible and deliberate.**

### Anti-Pillars — What This Game Is NOT

- **We will NOT have trading, gifting, or any player-to-player exchange** — because
  it would compromise Pillar 1 (authorship becomes social currency) and would
  open a moderation, safety, and privacy surface that cannot be afforded within
  the timeline. Not now, not "later."
- **We will NOT have rarity tiers, loot boxes, odds, or pity timers** — because
  they compromise Pillar 2 (a child cannot lose) and Pillar 1 (the creature stops
  being the child's own creation). Rare variants are **earned by playing**, never
  by chance.
- **We will NOT have a room-decor meta in v1** — because it dilutes the Adorn verb,
  which is the entire game. It is a viable second product, not a feature of this one.
- **We will NOT have timers, streaks, energy, or currency** — they compromise
  Pillar 2 and turn a comfort object into a chore.
- **We will NOT have ads or in-app purchases** — Pillar 2 and child-directed
  standards are non-negotiable.

---

## 3. The Core Loop

### 30-Second Loop — moment to moment

1. **Touch the creature** — continuous stroke, creature leans into the finger,
   eyes track it, audio pitch rises with stroke speed.
2. **Open the paint palette** — large targets, icon-driven, read-free.
3. **Paint** — drag a stroke; color floods in behind the finger with no precision
   requirement. This is the "Creative Flow" peak.
4. **Watch it settle** — paint dries, the creature does something small with its
   new look.

*Payoff is instant and legible. Finger is input, creature is canvas. No loading,
no menu, no confirm.*

### 5-Minute Loop — the loop that matters

This is the loop that closes the sandbox gap. Without it, customization is a
sandbox with no consequence: the child paints, the game says "nice," nothing
happens.

1. **Paint** — freehand, no pressure, pure creative flow.
2. **Adorn** — choose a **signature mark** (horn swirl, wing tint, cheek spot).
3. **Echo** — that specific mark is now bound to a real ability the creature
   performs.
4. **Show** — the creature uses the ability in a short flourish and in minigames,
   visibly wearing the child's design.

The flourish is the *"and look what I made!"* moment. It is where "one more"
psychology kicks in, because designing a *new* mark is a genuinely open-ended itch.

### Session Loop — 5–15 minutes

**A 30–120 minute session is fiction for this audience.** Real sessions are short
and ritualistic:

- **Arrive** → creature recognizes you, greets enthusiastically
- **One substantial activity** → paint, or one minigame
- **Visible reward** → something new is authored and kept forever
- **Leave** → creature waits in visible unfinished-but-pleasant state

**Natural stopping point:** after the flourish. **Anti-cliff:** nothing is lost,
nothing is pending-failure.

**The return hook is anticipation, not obligation.** The pet is in a state of
gentle unfinished business. It is never sad, never sick, never neglected.

> **Critical rule:** never punish absence. No "you missed 3 days." A pet that
> gets sad when you leave teaches a 7-year-old that games are a chore, and is
> also a dark-pattern retention hook. Instead, **greeting intensity scales with
> time away** — returning after a week is the *best* greeting in the game.

### Progression Loop — weeks to months

**There is no levelling and no power unlock.** The child accumulates **a
vocabulary of their own inventions**: signature marks, bound moves, paint
patterns, minigame records.

- **Growth axis:** Expression, not power
- **Collectible:** Moves and marks *the child invented*
- **Long-term goal:** Design a creature they consider distinctly theirs
- **"Done" condition:** The child has authored something they love

This is a collection-completion game wearing a care-sim's clothes. The collection
is **of their own authorship** — which is the non-social, non-loot-box answer to
the pull we identified in Adopt Me.

### The Internal Tension — and Its Resolution

"Continuous and fluid" fights "creative self-expression" in one specific place.

- **Fluid** wants a *crayon* — drag fast, start point doesn't matter, the stroke
  is the reward.
- **Self-expression** wants *precision and consequence* — placed deliberately,
  because the child meant it.

A freehand-only painter is fluid but forgets intent. A placement-only painter is
expressive but feels rigid and fussy to a 6-year-old.

**Resolution — Pillar 4:** freehand painting is the fluid default (crayon energy,
zero precision demand, maximum Creative Flow); **signature marks are discrete
placed choices**, because binding to an ability must be legible and deliberate.

---

## 4. MDA Analysis

### Mechanics

- Continuous touch-and-stroke petting with reactive animation
- Freehand 3D surface painting with no precision requirement
- Discrete signature-mark placement
- Signature-mark → ability binding (the Echo rule)
- 3–5 minigames built on single gestures (timing, matching, pattern)
- Greeting intensity scaling with time away
- Offline save with permanent authoring record

### Dynamics

- The child strokes the creature and gets immediate, proportionate, expressive
  reaction; reaction intensity tracks input energy
- Painting produces immediate visible transformation behind the finger
- Placing a signature mark triggers a bound ability performance within seconds —
  authorship becomes causation
- Minigame difficulty plateaus rather than climbs; improvement is legible and
  failure is invisible
- Absence creates anticipation, and return creates delight

### Aesthetics

| Aesthetic | Dominant | How it's produced |
|---|---|---|
| **Expression** | ★★★ | Paint verb + permanent authorship + flourish |
| **Sensation** | ★★★ | Tactile stroke feel, reactive audio, bright juice |
| **Discovery** | ★★ | Echo's "what does this mark do?" moment |
| **Submission (relaxation)** | ★★ | Ambient caretaking, no pressure, ritual cadence |
| **Fellowship** | ✗ | **Absent by design.** No social layer. |
| **Challenge** | ★ | Minigames only, capped deliberately, never punitive |

**Primary: Expression.** Secondary: Sensation. Fellowship is deliberately absent —
the pet carries the relatedness load alone, which raises the bar on its animation
quality.

---

## 5. Player Motivation Profile (Self-Determination Theory)

### Autonomy — HIGH, and correct

Every expression choice is unconstrained. Nothing is gated behind currency,
unlocks, timers, or permission. The child has genuine authorship over their
creature's appearance, moveset, and name.

### Competence — the identified weak spot

Care sims give children nothing to get **better at**, which undermines competence
for an 8-year-old who needs to feel capable. Worse, in a no-fail game, "not being
bad at anything" can read as "not being good at anything either."

**Resolution:** minigames are the competence channel. Timing, matching, simple
pattern recognition — where improvement is legible and failure is invisible. This
is the primary justification for keeping minigames at all. Difficulty **plateaus**
rather than climbs.

> **Open question for `/design-system`:** how is improvement made *visible* in a
> game with no score and no fail state? A record of "your best run" is the
> starting hypothesis.

### Relatedness — the structurally hardest

You cannot build relatedness toward other players without a social layer, and the
social layer is correctly excluded. So relatedness must come entirely **from the
pet to the child**.

This raises the technical and art bar substantially: the creature's animation
quality, personality consistency, and memory of shared activity are carrying the
entire emotional weight. A lifeless animated pet makes this game fail regardless
of how good the paint tool is.

---

## 6. Audience Reality Check

### The 5–8 band is two cohorts, and drag-and-drop fails the younger one

- **Age 5:** lacks fine motor control and object persistence for drag-to-feed.
  They abandon mid-drag and tap instead. Icon-only UI can read as broken.
- **Age 8:** reads fluently, finds icon-only UI patronizing, and considers pet
  sims babyish.

**Design decisions taken:**

1. **Defensible band is 6–9, with reading assistance.** Text is supportive, never
   load-bearing. Every action is discoverable without reading.
2. **Every drag gesture has an equal-status tap fallback.** Not a degraded mode —
   a first-class parallel path. Direct manipulation is a shortcut, not a
   requirement.
3. **Reaction expressiveness is maximal.** A 5-year-old reads *large and obvious*
   reactions as responsive; subtlety reads as broken.
4. **Simultaneous local multiplayer / shared-device play** is an open
   consideration for a 9-year-old with a younger sibling.

---

## 7. Visual Identity Anchor

> **Status: provisional.** `AD-CONCEPT-VISUAL` was skipped under lean review mode.
> This section is the seed of the art bible, **not** a substitute for it. Run
> `/art-bible` before any asset production — this anchor governs, but does not
> replace, that specification.

### Visual Direction — **"Candy Sunrise"** *(working name)*

**The one-line visual rule:**

> **Everything is warm, rounded, and lit like morning — no sharp edges, no grey,
> no shadows that mean anything is wrong.**

### Supporting Principles

**1. Light comes from behind and above, always.**
Hair and fur rim-lit, creature always readable against any background.
- *Design test:* If an asset needs fill light to be visible, **we relight it.**

**2. Saturation is high but value range is narrow.**
Bright, candy-colored, but never harsh — mid-to-high key, soft terminator.
- *Design test:* If a color reads as "dark" or "muddy," **we lift its value and
  re-tune its hue toward warm.** No blacks in the palette except thin outlines.

**3. Every reaction overshoots.**
Squash, stretch, and anticipation frames beyond realism. A happy reaction must be
visible from across a room, because the target player may be 4 feet away.
- *Design test:* If a reaction could be mistaken for neutral at arm's length,
  **it is not exaggerated enough.**

### Colour Philosophy

- **Base palette:** warm cream, honey, peach, sky, mint — high-key, low-contrast
- **Accent:** saturated candy tones reserved for *rewards and the child's own
  paint* — so authorship is always the most colorful thing on screen
- **Forbidden:** pure black, desaturated grey, cold blues in shadow, any colour
  associated with threat or failure
- **Rule:** the creature's base colour stays neutral/cream **so that the child's
  paint is always the most vivid thing in frame** — this makes authorship visible
  at a glance, which is the whole point.

### Known Gap

**No reference games were supplied for visual touchstone analysis.** The palette
and lighting rules above are derived from the written brief and the "Candy
Sunrise" framing, not from art-direction analysis of comparable titles. Supply
visual references at `/art-bible` time — this is the highest-value missing input
to the project's art identity.

---

## 8. Flow State Design

The intended flow state is **Creative Flow** (per the reference analysis): total
immersion in painting and styling, where the child loses track of time.

**Entry conditions:** creature on screen, no modal UI, no pending reward, no
timer visible, no notification.

**Conditions to protect flow:**
- Palette opens **without** leaving the creature view — it never takes over the screen
- No dialog boxes during the paint interaction
- Audio is continuous and responsive, never a loop with a seam
- **No purchase prompts, ever, during play** (also a compliance requirement)
- No network calls on the paint path — offline-first guarantees no stutter

**Exit conditions (all voluntary):** the flourish completion, a parent gate, or
the child simply putting the device down. **There is no in-game reason to leave.**

**Anti-flow-killers:** notifications, ads, forced interstitial, error dialogs,
save-corruption risk on the paint path.

---

## 9. Scope and Feasibility

### The Central Constraint

**"AAA visuals + first game + months" is over-constrained.**

AAA-fidelity stylized rendering *is* reachable on URP — but only through
deliberate art-pipeline discipline: constrained palette, tight topology, heavy
reliance on shaders and lighting rather than polygon count or texture resolution.

What is **not** reachable in months by a first-time developer is **content
volume**: a creature roster, a minigame suite, and deep customization all scale
with labor, not cleverness.

> **The usual failure mode:** nine months spent on one hero creature that looks
> stunning, shipping an empty game.

**Resolution — Pillar 3:** one exceptionally polished creature framework, all
additional creatures as low-cost variant meshes/skins, visual quality bought with
art direction. Confirmed and accepted by the developer.

### Platform Implications

- **Mobile (iOS + Android) strongly favors Unity.** Chosen.
- **URP is the correct choice** over HDRP: HDRP is not viable for the target
  device range and would cost the stylized look nothing.
- **Thermal and memory budgets are the binding constraints**, not polygon counts.
  A 6-year-old's device is mid-range at best.
- **App Store / Play Store kids-category compliance is a hard requirement**, not
  polish: no third-party analytics SDKs, no behavioral ad targeting, parental gate
  on any external link or purchase.

### MVP Definition

**The absolute minimum build that tests "is the core loop fun?"**

1. One creature mesh with idle + 3 reactive animations (pet, happy, flourish)
2. Freehand painting working on that mesh
3. One signature mark + one bound ability + its flourish
4. Touch-petting with reactive audio/visual feedback
5. Save/load of the child's authored design

**If the MVP is fun, the concept is validated.** Everything else — minigames,
variants, extra marks — is content built on a proven loop.

**MVP explicitly excludes:** minigames, multiple creatures, room decor,
accessories, localization beyond English.

### Scope Tiers

| Tier | Contents | Timeline |
|---|---|---|
| **MVP** | One creature, painting, one mark + one bound ability, petting, saves | 3–5 months |
| **v1.0 (Target)** | 3–5 signature marks and bound abilities, 3 minigames, 2–3 creature variants, polish pass, store compliance | 9–14 months, solo |
| **Post-launch** | Additional creatures as variants, more minigames, localization, accessibility depth | Ongoing |

### Technical Risks

| Risk | Severity | Mitigation |
|---|---|---|
| **3D texture painting feels bad on mobile** — the single highest-risk mechanic | HIGH | Prototype this FIRST, before anything else. Freehand crayon approach; generous hit tolerance; immediate feedback |
| **Draw calls / shader variants blow mobile budget** as marks multiply | HIGH | Constrain palette to fixed slots; share materials; enforce batching from the first prototype |
| **Animating expressive emotion convincingly** — creature carries relatedness alone | MEDIUM | Prioritize animation quality over feature count; squash/stretch + overshoot carries emotion cheaply |
| **Painting performance on low-end mobile GPUs** | MEDIUM | Decal or mask-based approach; test on actual mid-range hardware early |
| **App store kids-category rejection** for SDK or data practices | MEDIUM | Zero third-party analytics; parent gate; audit before submission |
| **Content scope creep** — the standing risk of this genre | HIGH | Pillar 3 enforced at every review; template-based ADR for any new asset |
| **Thermal throttling in long sessions** | LOW | Sessions are 5–15 min; cap particle density |

> **First technical spike, before any other work:** prove freehand 3D painting is
> pleasant on a mid-range Android device. If it isn't, the entire concept changes
> and every other decision downstream is wasted.

### Design Risks

| Risk | Severity | Mitigation |
|---|---|---|
| **Child loses interest in 2 weeks** | HIGH | The Echo loop is the answer — novelty lives in designing new marks, not new pets |
| **Painting scares off players who don't want to draw** | MEDIUM | Signature-mark placement is the low-commitment path to the same reward |
| **8-year-olds find the game babyish** | MEDIUM | Competence channel via minigames; creation-with-agency framing over "cute pet" framing |
| **Relatedness gap** — no social layer means possible loneliness | MEDIUM | Pet personality consistency and memory-of-shared-activity; animation quality bar |

### Market Risks

| Risk | Severity | Mitigation |
|---|---|---|
| **Genre saturation** — pet sims are extremely crowded | HIGH | The Echo hook is the differentiator; it must be legible within 60 seconds or the market risk materializes |
| **Premium purchase vs. kids' market pricing** | MEDIUM | One-time purchase, no F2P; validate willingness to pay with a small test group |
| **Parents' perception of screen time** | MEDIUM | Session design is short and ritualistic; no compulsion mechanics is a genuine selling point |

---

## 10. Traceability

| Design Element | Source |
|---|---|
| Paint verb, creative flow peak | Reference analysis — Toca Life World |
| Collection pull without rarity/social pressure | Reference analysis — Adopt Me! (pull retained, pressure rejected) |
| Tactile caretaking loop, ambient companionship | Reference analysis — Nintendogs / Pou |
| Echo mechanic (authorship → ability) | Concept generation — Experience-First / MDA Backward |
| Signature-mark placement | Concept generation — Verb-First (PAINT), modified for legibility |
| "Authored, Not Decorated" | Pillar from sandbox-gap analysis |
| "Nothing Can Go Wrong" | Developer no-fail mandate |
| "Rich From Restraint" | Timeline/scope analysis — months + first project |
| "Two Tools, Both Toys" | Resolution of fluid-vs-precision internal tension |
| Exclusion of trading | Timeline + child-safety analysis |
| Exclusion of room-decor meta | Pillar 1 / Adorn-verb dilution analysis |
| Candy Sunrise visual anchor | Provisional — derived from brief, pending `/art-bible` |

---

## 11. Open Questions

To resolve in downstream skills:

1. **Competence without scores** — how is improvement visible in a no-fail,
   no-score game? (`/design-system` — Minigame System)
2. **Persistence of paint** — does a child's painted pattern survive indefinitely,
   and what happens when new body parts or creature variants are added?
3. **Signature-mark binding model** — is it strictly 1 mark = 1 ability, or can
   combinations grant emergent abilities? (Affects architecture and content cost)
4. **Reading assistance specifics** — voice-over for text? Text-to-speech? Icon-only?
   (Affects UX and localization scope)
5. **Parent gate design** — what does the parent-facing layer actually contain?
6. **Visual references** — not yet supplied. Blocking for art bible quality.
7. **Accessibility depth** — colorblind palettes interact with the "high saturation,
   no grey" rule and need explicit resolution.

---

## 12. Handoff

This document is the **source of truth** that all per-system GDDs trace back to.

**Required before asset production or GDD authoring:**

1. `/setup-engine` — pin Unity version, populate version-aware reference docs
2. `/art-bible` — formalize Candy Sunrise into a full art bible (gates all assets)
3. `/design-review design/gdd/game-concept.md` — validate concept completeness

**Then:** `/map-systems` → `/design-system` per system → `/review-all-gdds` → `/gate-check`