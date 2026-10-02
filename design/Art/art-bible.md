# Art Bible — "Dab" (working title)

> **Status**: COMPLETE — Sections 1–9 authored and approved. Companion production files: palette.json, palette.css, typography.json, style-anchor-prompt.md, reference-catalog.md.
> **Source**: `design/gdd/game-concept.md` (Visual Identity Anchor: "Candy Sunrise")
> **Game**: 3D pet simulator for children aged 6–9 — iOS + Android, Unity 6.3 URP
> **Last Updated**: 2026-09-30

---

## Section 1 — Visual Identity Statement

### 1.1 The Rule

> **Candy Sunrise: everything is warm, rounded, and lit like morning; the
> creature's body is neutral cream so the child's own marks are always the
> loudest thing in frame. When two directions are both defensible, choose the one
> that makes the child's authorship more visible and the interface more readable
> — never the one that adds more content.**

This single rule governs the three axes on which this project drifts:

| Axis | Forced resolution |
|---|---|
| Amplitude vs. clarity | Clarity wins. A vague effect is worse than no effect. |
| Saturation vs. authorship | Authorship wins. The world loses saturation before the child's paint does. |
| New content vs. one thing finished | One thing finished wins. Pillar 3 as a visual law. |

### 1.2 The Binding Encoding Law

**Non-negotiable. Overrides every other section.**

> No state, mark, status, or affordance is ever communicated by colour alone.
> Every piece of information travels on at least two channels: one of
> {shape, silhouette, pattern, motion, size, value} **plus** colour.

Required redundant channel, chosen to suit the information:

- **State change** (wet → dry, available → spent): shape change + motion change +
  value change. A wet bead flattens, a settle animation plays, emissive drops to
  matte.
- **Tool type** (freehand vs. placement): icon silhouette differs — a crayon-lozenge
  vs. a pin-badge — and trailing behaviour differs (ribbon vs. snapping ghost). Not
  a hue swap on the same icon.
- **Minigame success**: creature body shape change (a hop) plus a particle burst in
  a **fixed radial pattern**, independent of colour. Success is legible in greyscale.
- **Painted mark in greyscale**: every mark carries (a) raised bead of 0.02–0.04 H
  relief, (b) directional streak texture aligned to stroke vector, (c) colour.
  Remove colour and the mark still reads as the child's work.

**Design test:** desaturate any screenshot to greyscale. Every state, tool, and
result must still be distinguishable. **If greyscale collapses two things into one,
that is a defect, not a palette limitation.**

> **Note on scope of this law:** it governs *state changes*, so it must apply across
> animation and audio channels too — not only static art. A child who cannot
> distinguish a state needs it to be *loud*, not merely differently shaped. See
> §1.6.

### 1.3 Ambiguity Resolver — the six forced calls

Any future art or design argument not settled by this document is settled here:

| When the question is… | Choose |
|---|---|
| "Rounded or articulated?" | **Rounded.** Corner radius never below 35% of a form's shortest local axis. |
| "Contrast or cohesion?" | **Cohesion.** No state exceeds a 3.5:1 value ratio across visible surfaces. |
| "Cool or warm shadow?" | **Warm.** Shadows tint toward peach-rose. Neutral or blue is a bug. |
| "Where does the key come from?" | **Behind and above**, always, from the visible sun disc. Never front, never below. |
| "Bigger reaction or more accurate reaction?" | **Bigger.** Overshoot past realism, then settle slow. Neutral is a failure state. |
| "New asset class or variant of an existing one?" | **Variant.** If it needs new geometry, the answer is no. |

### 1.4 Supporting Principles

#### Principle 1 — The child is the most colourful thing on screen
*Serves Pillar 1 — Authored, Not Decorated.*

Creature base coat held at **≤ 25% saturation** across every variant, state, and
frame. Every environment surface capped at **≤ 40% saturation**. Saturated candy
tones are a budgeted resource; the child's paint and signature marks receive **100%
of it**. No other consumer.

*Design test:* screenshot any frame, eyedrop the five most saturated pixels. If any
is not the child's paint, a signature mark, or a flourish reward, that is a
violation — desaturate until the child owns the top of the list.

#### Principle 2 — Nothing in frame can read as a problem
*Serves Pillar 2 — Nothing Can Go Wrong.*

No asset may convey threat, error, urgency, scarcity, or neglect. Specifically: no
red-as-error, no darkening-as-consequence, no shake-as-penalty, no
countdown-as-pressure, no desaturated-as-unavailable, no locked-padlock glyph, no
greyed-out control.

Inability and blocked states are expressed by **scale and softness** — a control
that cannot be used is 88% size and one step lower in value. Never crossed out,
never locked.

*Design test:* point at any screen and ask a child "does anything here look like it
went wrong, or like you can't do that?" If yes, the asset is redesigned — enlarge
the positive element, soften the negative one. **Never remove a function the child
has already used.**

#### Principle 3 — One creature, restated — never multiplied
*Serves Pillar 3 — Rich From Restraint.*

Visual ambition is bought with lighting, shading, and silhouette proportion, never
geometry, texture resolution, or asset count.

A new creature is legally defined as: **one base mesh + one rig + one silhouette
hook + a proportion vector + a print class.** Anything beyond that is a new asset
and out of scope by default.

*Design test:* if a proposed creature needs more than a texture set, a rig
retarget, and exactly one attached form, reject it and find the variant. If it
cannot be instanced into the existing batch, it does not ship.

### 1.5 Reviewer Checklist

- [ ] The child's paint is the most saturated thing in frame
- [ ] Greyscale test passes — every state distinguishable without colour
- [ ] Nothing in frame reads as a failure, a lock, or a warning
- [ ] Every new creature resolves to a variant, not a new mesh
- [ ] Light comes from behind and above, source visible in-world
- [ ] No shape in any asset reads as a sharp corner

### 1.6 Accessibility Depth Note

The Binding Encoding Law (§1.2) governs **state changes**, so its redundant channel
must be audible and animated, not only differently-shaped. Concretely, every state
transition carries at minimum:

1. A **shape or silhouette** delta (required by §1.2)
2. A **motion** delta — anticipation, overshoot, or settle
3. An **audio** delta — pitch, timbre, or register shift

A child who cannot distinguish a state by sight must still distinguish it by ear.
This resolves open question 7 in `AGENTS.md`: the colourblind rule is satisfied by
structural redundancy across three channels, not by palette adjustment.

---

## Section 2 — Mood & Atmosphere

### 2.1 One Rig, Seven Modulations

The game contains **exactly one lighting rig**. States do not author new lights;
they modulate the shared rig along named axes. A draw-call and consistency decision
— and what lets one developer hold a look across seven contexts.

#### Candy Sunrise Base Rig

| Element | Specification |
|---|---|
| **Key** | Directional. Azimuth 130° from camera-forward, elevation 50°. Colour `#FFE3B8`. Intensity 1.35. Soft shadows. |
| **Shadow colour** | Tinted `#F6B9A0` at 0.28 opacity. **Never black, never grey, never blue.** |
| **Sky fill** | Gradient dome: zenith `#FFF6E2` → horizon `#FFCFA0` |
| **Ground bounce** | `#F3B98F`, low intensity, warm |
| **Rim** | Directional, azimuth 180°, elevation 22°, two-tone: warm `#FFB0D0` upper silhouette, mint `#BFF3E2` at base. Intensity 0.55. |
| **Ambient** | Flat `#FFF0D8`. No grey, no cold. |
| **Sun disc** | Visible emissive disc in the sky dome at the key's azimuth, so light direction reads as a *place*. |

#### Hard Lighting Laws (all states, no exceptions)

1. **No surface in any state falls below 25% of its base luminance.** Shadow never
   reaches darkness. This is the literal implementation of "no shadows that mean
   anything is wrong."
2. **No state exceeds a 3.5:1 value ratio** across visible surfaces.
3. **The rim light is never off in any state except Minigame**, where it is
   suppressed to a whisper (0.15). It guarantees the creature reads against any
   background.
4. **No light source in the world is coloured cold.** Warm-to-neutral only.
5. **The creature is never fully frontally lit except in Minigame.** Every other
   state is rim-dominant.
6. **Every state change animates over ≥ 0.4s.** No lighting pops, except the single
   authored 0.15s light-pop in the Flourish (§2.5) — deliberate theatre, not error.

### 2.2 State: First Open / Greeting

**Emotional target:** *"You're here. I noticed."* Recognition and delight,
delivered instantly and as a gift, with **zero reference to elapsed time.** The
Pillar 2 anti-guilt mechanism executed as lighting: absence is never punished,
only rewarded.

| Property | Value |
|---|---|
| **Key** | Base rig, intensity ×1.25 |
| **Environment** | Fog to 85% near-white; slow drift of 40 warm motes |
| **Value ratio** | 1.9:1 — lowest contrast of any state |
| **Direction** | Base rig (behind and above) |
| **Adjectives** | radiant, eager, washed-in-light, buoyant, welcoming |
| **Energy** | 8/10 — opens at 6, peaks at 8 on recognition, settles to 4 over 1.2 s |
| **Camera** | Creature fills lower two-thirds; horizon low; camera slightly below eyeline so it looks *up* at the child |

**Exclusive signature variable: absolute brightness.** The Greeting is one full stop
brighter than every other state, and *starts* two stops brighter, flooding down over
1.2 seconds. A child returning after a week gets the longest, brightest version —
**greeting intensity scales with absence length, and the child sees the scaling
without reading a word.**

### 2.3 State: Idle Ambient (Petting)

**Emotional target:** *"I'm not going anywhere."* Patient, breathing company. The
rest state. The one moment that asks for nothing.

| Property | Value |
|---|---|
| **Key** | Base rig, **unmodified** |
| **Environment** | Base rig, unmodified |
| **Value ratio** | 2.4:1 |
| **Direction** | Behind and above, at rest |
| **Adjectives** | warm, still, soft, patient, breathing |
| **Energy** | 2/10 — 4 s breathing cycle with 6 s idle sway, offset so they never sync |
| **Camera** | Static. **Does not move, breathe, or drift.** |

**Exclusive signature variable: nothing — Ambient is the baseline.** This is the
load-bearing decision of the section. Every other state is defined as a measurable
departure from Idle Ambient. **If a proposed state cannot be described as a delta
from Ambient, it is not a state — it is a new scene, and it will not be built.**

Petting response: light does not change. **Only the creature changes** — a lean into
the finger, squash proportional to stroke speed, ears flattening, eyes tracking.
The world stays absolutely still so the child's touch is the only event in frame.

### 2.4 State: Painting

**Emotional target:** *"I made this."* Absorbed, silky focus. The Creative Flow
peak. Paint should appear slightly before the finger; the child is never asked to
aim.

| Property | Value |
|---|---|
| **Key** | Base rig, intensity ×0.85 |
| **Rim** | Warm-shift only (mint half removed), intensity ×1.15 — silhouette brightens as body dims |
| **Environment** | Value −1 step, **saturation ×0.75**. The world steps back so the paint steps forward. |
| **Value ratio** | 2.9:1 |
| **Direction** | Behind and above, plus a low wide warm "worklight" band tracking the finger from above |
| **Adjectives** | focused, silky, wet, luminous, absorbed |
| **Energy** | 5/10 sustained, spiking to 7 on each stroke start, never pausing |
| **Camera** | Pushes in 12% over 0.6 s on palette open, then holds perfectly still. **The camera is a fixed easel.** |

**Exclusive signature variable: environment saturation drop.** No other state
desaturates the world. The world dropping back is the signal that the child's work
is now the subject.

**Wet/dry encoding** (satisfies §1.2): a stroke is emissive-lifted and beaded for
0.8 s, then settles to matte and flat over 0.6 s. Shape + value + motion change.
In greyscale, wet paint is brighter and raised; dry paint is flat and matte. Colour
is the third channel, not the first.

### 2.5 State: Signature Mark Flourish

**Emotional target:** *"Watch what I gave you."* Triumph, ceremony — the single most
important 2.5 seconds in the game. The Echo pillar made visible. Must be legible
from across a room, to a child holding the device four feet from their face.

| Property | Value |
|---|---|
| **Key** | Rotates +15° to three-quarter back-light; intensity ×1.1 |
| **Rim** | Intensity ×2.0 — strongest in the game |
| **Environment** | Value −2 steps. The room drops away. |
| **Mark light** | Local point light **tinted to the mark's own colour** travels with the creature |
| **Value ratio** | 3.5:1 — maximum permitted, reached only here |
| **Direction** | Three-quarter back, elevation 62° — most dramatic angle the rig allows |
| **Adjectives** | triumphant, radiant, ceremonial, spark-flecked, golden |
| **Energy** | 10/10 — one 2.5 s peak, then 3.0 s slow decay to Ambient. Never a second peak without a new mark. |
| **Camera** | Slow 8° orbit toward the mark's side, easing out. The only camera move that tracks a subject. |

**Exclusive signature variable: light-source inversion.** For the first 0.15 s, **the
creature's mark is the brightest object in frame and briefly the apparent light
source of its own halo**, before the rig floods back in. Nothing else in the game
inverts. This is the moment the child is meant to film.

### 2.6 State: Minigame

**Emotional target:** *"I can do this."* Clean, honest, focused competence. The only
channel through which improvement is legible, so it reads instantly.

| Property | Value |
|---|---|
| **Key** | Azimuth 15° — **nearly frontal.** Elevation 60°, intensity 0.95. |
| **Rim** | Suppressed to 0.15 (whisper, silhouette only) |
| **Environment** | Pushed back and depth-blurred; bright circular stage pool under the creature |
| **Value ratio** | 2.2:1 — flat and even |
| **Direction** | **Frontal.** The only state where the creature is not rim-dominant. |
| **Adjectives** | bright, clean, focused, buoyant, honest |
| **Energy** | 6/10, rhythmic — matched to gesture cadence, **never to a timer** |
| **Camera** | Locked flat three-quarter, maximising visible painted surface area |

**The art-direction call:** here, and only here, mood is sacrificed to legibility.
The patterns on the creature's body *are* the game — the child is matching what they
painted. Frontal even light and a flattened environment are correct, and the warm
palette is retained so the state never leaves Candy Sunrise. The framing is a
**morning-lit studio, not an arena.**

**Success encoding** (satisfies §1.2): body-shape hop (squash 82% → stretch 112%
overshoot) + fixed 12-spoke radial particle burst + score-free record stamp.
**Identical in greyscale every time.** The burst pattern is fixed and never varies
by colour or performance quality, because Pillar 2 forbids visually grading the
child.

**There is no fail lighting state.** A miss produces a 0.3 s dim-to-Ambient, and the
next prompt appears at **higher** brightness than the previous one, so the visual
trend across attempts is always upward.

### 2.7 State: Results / Reward

**Emotional target:** *"This is mine, and it's staying."* Warm, held, satisfied.
The punctuation mark at the end of the session. **Permanence is the feeling.**

| Property | Value |
|---|---|
| **Key** | Base rig, unmodified |
| **Environment** | Slow, wide, **radial warm sunburst** behind the creature — soft, symmetrical, non-rotating |
| **Value ratio** | 2.0:1 — soft and enveloping |
| **Adjectives** | warm-hollowed, still, satisfied, glowing, held |
| **Energy** | 4/10, decaying. Nothing new arrives during Results. |
| **Camera** | **Centred, symmetrical, front-on.** |

**Exclusive signature variable: compositional symmetry.** Every other state is
bottom-weighted or off-centre — creature sits low, camera off-axis, world leaning.
Results is the one centred, symmetrical frame in the product, and it lasts 3–4
seconds. Symmetry means "settled," and it appears only here, which is exactly what
makes it read as finality.

**Reward objects are earned by growing into frame**, always toward the light source,
at 1.4× scale, with overshoot. Never awarded by popup, never slide in, never drop
from above, never appear with confetti that could read as threat. Once looked at,
it settles into the world as scenery.

### 2.8 State: Menus

**Emotional target:** *"Everything is here, nothing is urgent."* Calm, orderly,
unhurried. The menu is a pause button, not a form.

| Property | Value |
|---|---|
| **Key** | Base rig, intensity ×0.9 |
| **Environment** | Value ×0.85, **saturation ×0.8** — the room rests. **Never greys out; it dims.** |
| **Value ratio** | 2.1:1 |
| **Adjectives** | calm, orderly, hushed, tactile, unhurried |
| **Energy** | 1/10 — lowest in the game |
| **Camera** | Lifts to a **high three-quarter** so the habitat reads as a diorama under glass; creature visible but no longer the subject |

**Exclusive signature variable: camera angle.** Menus are the only state where the
camera does not point at the creature. The creature stays in frame, visible and
breathing, so a child who opens a menu never feels they left the pet — the
anti-guilt rule holds in the UI layer too.

**Menus are the same room, held still.** Quiet is achieved through **lower value and
lower saturation, never through grey.** Menus use the habitat palette at 85% value
and 80% saturation — still warm, still candy, just speaking more softly. Any menu
trending toward neutral grey violates the identity rule.

A **visible horizon band** — a soft warm curved light strip at the far edge of the
ground bowl — is the one element present in Menus and absent everywhere else. It
gives the child a stable reference line saying *this is the same place, arranged.*

### 2.9 State Distinctness Matrix

**No two states share a signature variable.** This is the proof that all seven are
separable in greyscale — the §1.2 law applied to atmosphere.

| State | Signature variable | Non-colour channel |
|---|---|---|
| First Open / Greeting | Absolute brightness | Overall frame lightness |
| Idle Ambient | *(baseline — no delta)* | Stillness, slow breathing cycle |
| Painting | Environment saturation drop | Background receding; raised wet beads |
| Signature Mark Flourish | Light-source inversion | Halo position, spark burst, motion |
| Minigame | Light direction (frontal) | Silhouette shading direction |
| Results / Reward | Compositional symmetry | Object position in frame |
| Menus | Camera angle | Subject size and frame composition |

### 2.10 Reviewer Checklist

- [ ] Every state reads as a delta from Idle Ambient
- [ ] No state exceeds a 3.5:1 value ratio
- [ ] No surface falls below 25% base luminance
- [ ] Warm shadows only — no grey, no black, no cold blue
- [ ] Minigame is the only frontally-lit state, and it is warm anyway
- [ ] Flourishes legible in greyscale from four feet
- [ ] No state has a "fail" lighting variant
- [ ] Sun disc visible in-world at the key light's azimuth

---

## Section 3 — Shape Language

### 3.1 Character Silhouette Philosophy

**Governing statement (§1.1):** *"choose the one that makes the child's authorship
more visible."* In shape terms: **the creature is a single legible mass that the
child's marks ride on.** The body is the canvas; it never competes with the canvas.

#### The canonical hatchling form

All proportions are fractions of total silhouette height **H**.

| Measurement | Value | Reason |
|---|---|---|
| Total silhouette height | 1.00 H | — |
| Minimum silhouette width | 0.62 H | Rounded mass, not a lollipop |
| Head diameter | 0.48 H | Big-head hatchling read; appeals to the 5–8 band |
| Head centre height | 0.74 H | Top of head at 0.98 H |
| Narrowest neck constriction | ≥ 0.70 × head width | **No neck.** Head and body are one continuous form. |
| Eye diameter | 0.17 H | Reads at 64 px; primary expression channel |
| Limb nubs (feet, arm nubs) | ≤ 0.10 H long, ≤ 0.06 H wide | Never break the silhouette |
| Ground clearance | ≤ 0.04 H | Grounded, never floating (except Flourish, by 0.25 H) |
| Pattern feature, minimum | 0.08 H | No stripe thinner than 8% of body height; prevents aliasing, survives 64 px |

#### Hard geometry rules

1. **No polygon edge may read as a corner.** Minimum corner radius = **35% of the
   form's shortest local axis**; the remaining 65% is chamfer, never a point. No
   cones, no spikes, no blades, no hard beaks, no angular talons.
2. **Every appendage tip is at least 40% of its own base width.** Pointed tails,
   tapered horns, and needle fins are prohibited. Taper to a rounded cap, never a
   vertex.
3. **Total silhouette area from appendages outside the body mass is capped at 12%.**
   Ears, horns, frills, tufts, and tails together. This one number protects
   thumbnail readability, the 300k triangle budget, and the "one mass" identity
   simultaneously. **Note that this cap governs only the area a hook adds
   *outside* the core mass** — it is not in conflict with the hook footprint
   minimum in the 64 px test below, because a hook that overlaps the body
   spends none of this budget. The two rules together are what make a crest,
   frill, or floppy ear the legal hook vocabulary and a detached fin, floating
   tail, or wing the illegal one.
4. **Topology targets** (Pillar 3 — buy quality with shading, not bytes): base body
   8–12k tris, rig under 45 bones, no LOD swap below 0.5 H on screen. The creature
   is the one object that gets full geometry budget because it is the hero.
5. **The creature casts the only real shadow in the game** (§3.5).

#### Thumbnail and notification-size readability

**The 64-pixel test.** Every variant, state, and pose must be identifiable at
**64 px tall, front-on, in greyscale, with no motion.** Three steps:

1. **Silhouette** — render at 64 px, three value steps from background. The outline
   must be a closed rounded mass. If the shape breaks into two lobes or a limb
   pierces the outline, the appendage budget is exceeded.
2. **Variant hook** — the hook's **footprint** (its area within the silhouette,
   counting the part that overlaps the body mass) must occupy at least **25% of
   the body's core silhouette area**. The core is the egg inscribed in the
   bounding box: `0.487 H²` at canonical proportions, so the hook's minimum
   footprint is **`0.122 H²`**. A smaller hook is a detail, not an identity, and
   fails.

   **At least 40% of that footprint must overlap the body mass** (`≥ 0.049 H²`),
   because the 12% cap in rule 3 only allows the hook to contribute `0.073 H²`
   of *new* outline. This single derived constraint is what legally eliminates
   detached fins, floating tails, and wings from the hook vocabulary without
   anyone having to forbid them by name — they have nowhere to put their area.
   **Corrections to the 64 px test:** the original wording read this against the
   bounding box, which is arithmetically impossible (`0.155 H²` needed vs
   `0.058 H²` permitted). It can only be read against area.
3. **Print legibility** — the print class must be distinguishable from other
   variants in greyscale at 64 px. This is why §3.2 fixes print classes to a closed
   set of value-distinct patterns.

**This is not academic: the Android notification icon for this game *is* the
creature** — a 24–48 px silhouette with no detail and, at OS level, no colour
differentiation available at all. Any variant identified only by colour is
unshippable.

### 3.2 Variant Identity — Hook, Tilt, Print

One base mesh. A variant is legally defined by exactly **three** channels. No
variant may introduce a fourth.

| Channel | What it is | Budget | Carries |
|---|---|---|---|
| **HOOK** | Exactly **one** attached form on the silhouette — ear shape, horn crest, frill, fin, tail. One mesh, one material, one draw. | 1 mesh, ≤ 3k tris | **Identity.** What the child names their creature by. Must pass the 64 px test. |
| **TILT** | A 4-value proportion vector: head scale, body length, leg length, base mass. | Free (vertex weights) | **Archetype read** — "this one is small", "this one is long" |
| **PRINT** | One surface pattern class from the closed set below. | Free (mask slot) | **Flavour and texture.** Never identity on its own. |

**The rule that makes this work:** **Hook carries identity, Print carries flavour,
Tilt carries archetype.** If a variant can be told apart only by its Print, it is not
a variant — it is a recolour, and it fails the 64 px test.

#### Closed print class set (nine, fixed, no additions)

Each class defined by a **distinct value character and distinct edge character**, so
all nine remain separable in greyscale at 64 px. Direct implementation of §1.2 at
the texture level.

| # | Class | Form | Greyscale character |
|---|---|---|---|
| 1 | **Solid** | Unbroken fill | Flat, no internal edges |
| 2 | **Stripe — Vertical** | Parallel bars, head-to-toe | Hard alternating value, vertical rhythm |
| 3 | **Stripe — Horizontal** | Bands ringing the body | Hard alternating value, horizontal rhythm |
| 4 | **Spot — Round** | Discrete circles | Isolated blobs, soft edges, high local contrast |
| 5 | **Spot — Angular** | Diamonds / wedges | Isolated blobs, sharp corners, mid local contrast |
| 6 | **Spiral** | One continuous swirl | One unbroken curved line, highest-contrast feature |
| 7 | **Checker** | Alternating squares | Dithered, mid-frequency, no dominant edge direction |
| 8 | **Band** | Wide gradient stripe, soft edges | Smooth value ramp, no hard edge |
| 9 | **Ripple** | Nested arcs | Curved, concentric, low frequency |

> **Adding a tenth print class requires an ADR and a Pillar 3 review.** The value of
> this set is that it closes the question permanently: nine greyscale-separable
> classes multiplied across H/TILT combinations yield the visual variety of dozens
> of creatures from one mesh. **Pillar 3 producing *ambition* rather than austerity.**

#### Painted marks

- A stroke is bounded by a **0.06 H feathered edge** — a crayon mark, never a
  hard-edged decal.
- A wet stroke carries **0.02–0.04 H of relief** above the coat, plus emissive lift,
  plus streak texture aligned to the stroke vector. Settles to flat and matte over
  0.6 s.
- Marks live in **six fixed mask slots** (Pillar 3 + the shader-variant ceiling in
  `AGENTS.md`). A stroke lands in the least-occupied valid slot. Slot exhaustion is
  resolved by visible pattern density, **never by discarding the child's work.**
- The child's mark always renders in the saturated budget reserved by §1.4
  Principle 1, and is never re-tinted to fit a variant's palette.

### 3.3 Environment Geometry

**Governing statement:** *"the child's authorship more visible."* The environment's
only jobs are to give the creature a stage, give the silhouette a value field to
read against, and reassure through scale. **It competes with nothing.**

#### What dominates, and why

**A single shallow ground bowl and a gradient sky dome. Props are subordinate to
both.**

| Element | Specification | Why it dominates |
|---|---|---|
| **Ground bowl** | One wide, shallow, continuous curved mass in the lower third. Radius 6 H. Max elevation change across it: 8% of radius. No holes, no cliffs, no steps. | Grounds the creature and provides the value field for the rim-lit silhouette. A single continuous curve can never read as an obstacle. |
| **Sky dome** | Vertical gradient, zenith `#FFF6E2` → horizon `#FFCFA0` | Fills the upper two-thirds with a warm, low-contrast, untextured field. Nothing competes with the creature. |
| **Sun disc** | One emissive disc at the key's azimuth and elevation | Makes light direction a *place*. Gives a stable orientation cue in a world with no other landmarks. |
| **Props** | Instanced from exactly **three** base meshes in three silhouette families: **pebble, arch, tuft** | Three meshes cover the entire prop vocabulary. Pillars, grass tufts, and stepping stones are the same three objects at different scales. Pillar 3 at the environment level. |

**The environment contains four geometry classes in total: ground bowl, sky dome, sun
disc, rounded props.** That is the whole world — a 3-draw base plus instanced props,
well inside the 150 draw-call ceiling.

#### Environment prohibitions

1. **Nothing may suggest a barrier, hazard, or a place the child can get stuck.** No
   doors, gates, fences, walls, mazes, pits, ladders, stairs, or cliffs. There is
   nowhere the creature can be blocked, because there is nothing to be blocked by.
   *(Pillar 2, expressed in geometry.)*
2. **No hard horizon line.** The ground bowl's far edge and the sky's horizon meet in
   a soft gradient band with no defined seam. A hard line reads as a wall, and a
   wall reads as a limit.
3. **No environment surface exceeds 40% saturation**, and none may exceed the value
   contrast of the creature's rim. The environment may never be the
   second-most-interesting thing on screen.
4. **No prop casts a real shadow.** Props ground with soft contact-occlusion
   darkening in warm peach at 0.15 opacity. A hard prop shadow competes with the
   creature for the eye and breaks the "nothing is wrong" read. (§3.5)
5. **No reflective surfaces.** A mirror in a Candy Sunrise world is cold by
   definition.
6. **No prop is taller than 0.6 H.** The creature's head is always the highest point
   in frame, in every state. A composition guarantee, not a suggestion.

### 3.4 UI Shape Grammar

#### The decision: **echo the world, flattened.**

Not a distinct HUD language. The argument, and it is binding:

`AGENTS.md` §2.5 — *nothing in the UI may require reading* — forces icon-first UI. A
child navigates by **shape recognition**, not text. If the UI introduced a second,
unrelated shape vocabulary, every object would have to be **learned twice**: once as
a world object, once as a UI symbol. A child who learned that the round pebble in
the world means *rest* would not recognise a UI button expressing the same idea with
a different silhouette. At six years old, a second visual language is not a nicety —
it is a retraining tax, paid on every screen, every session, for the life of the
product.

**One shape vocabulary. One learning event.** The UI is the world's vocabulary,
rendered flat and calm.

#### Grammar rules

| Rule | Specification |
|---|---|
| **Container set** | Three types only: **pebble** (buttons, toggles, targets), **card** (grouped panels, palette), **ribbon** (banners, record stamps, confirmations). Nothing else. |
| **Corner radius scale** | One shared scale, identical to the world's: pebbles 100% (fully round), cards 40%, ribbons 20%. A radius used in the world is the same radius in the UI. **Zero rectangles. Zero sharp corners. Zero underlines. Zero dividers below 4 px.** |
| **Touch targets** | ≥ 72 px on a 1080×1920 portrait screen, all inside the **bottom 40%** of the display. One-thumb reachable, one-handed, no reach. |
| **Icon weight** | 8 px minimum stroke at 1×, rounded caps, rounded joins, no interior detail below 12 px. Icons are pictograms, not illustrations. |
| **Pressed state** | **Shape change, never colour change.** Control scales to 88% with 4% vertical squash and lifts one value step. The `AGENTS.md` drag-parity rule made visual: **a tap is a shape, not a tint.** |
| **Drag/tap parity** | Every drag interaction has a tap target with the **same silhouette** as the drag region. Placing a signature mark by dragging or tapping produces an identical result. |
| **Text** | Supportive only. Maximum five words. Never the sole content of a control. A button with icon and text shows the icon first and larger. **If the icon is removed, the button must still be understandable by shape.** |
| **Disabled / unavailable** | 88% scale, one value step down. **Never greyed, never locked, never crossed out, never behind a padlock.** *(Pillar 2, in UI geometry.)* |

#### Recognition check for UI

Every UI element must pass this before shipping: **cover all text, remove 60%
saturation, show it to a five-year-old who has never seen it. Every control must
still be identifiable by shape and by what happens when touched.** If a control
becomes unidentifiable, it is redesigned — not relabelled.

### 3.5 Hero vs. Supporting Shapes

**Governing statement:** *"the child's authorship more visible."* The eye lands on
the creature, then the child's marks, then the reachable control, then nowhere. That
order is fixed and is the composition brief.

#### The hierarchy

| Tier | Elements | Granted | Denied |
|---|---|---|---|
| **Hero** | The creature and the child's marks on it | The only cast shadow. The only rim light. The only motion overshoot. The only saturated colour. Highest value contrast. Full geometry budget. | — defines itself |
| **Hero (transient)** | A reward object, 1.5 s only | Hero-tier rim and overshoot at the flourish, then demoted to supporting | Never becomes a permanent second focal point |
| **Supporting** | Props, palette, records, saved marks | Contact grounding only. 2% idle sway. Max 60% of mark saturation. | Cast shadow, rim light, overshoot, high value contrast |
| **Receding** | Sky dome, ground bowl, sun disc | Nothing. Static, untextured, gradient-only. | Motion, shadow, saturation, detail, silhouette interest |

#### The five rules that enforce it

1. **The creature is the only object permitted to cast a shadow.** Everything else
   grounds with warm contact occlusion at 0.15 opacity. This single rule does more
   for visual hierarchy than any amount of composition tuning — the eye follows
   shadows, and there is exactly one.
2. **Reward objects earn hero status transiently and lose it permanently.** 1.5 s of
   rim and overshoot at the flourish, then they settle into the world as scenery. The
   child feels authorship without the frame developing a second centre of gravity.
3. **The creature is the tallest thing in frame, always.** Enforced by the 0.6 H prop
   ceiling (§3.3). No camera angle, state, or minigame layout may put a prop above
   the creature's head.
4. **The creature's eyes are the brightest non-painted element in any frame.** They
   carry a small self-lit term, always. This makes the creature read as *alive*
   rather than merely *present* at 64 px, and is the anchor the eye returns to in
   every state.
5. **Only the creature overshoots.** Squash, stretch, and anticipation past realism
   are reserved for the creature and its reward flash. Props and UI use linear,
   damped easing with zero overshoot. A hard split: **the living world is bouncy,
   the made world is calm.**

#### The reading order

**Creature (value contrast) → painted mark (saturation) → active UI target (size, in
the thumb zone) → the child's saved works on record → props (lowest contrast) →
nothing.**

Any screen breaking this order is wrong regardless of layout quality. The most
common violation to watch for: a UI panel with stronger value contrast than the
creature, or a reward popup brighter than the flourish. Both are hierarchy failures,
not layout failures.

### 3.6 Reviewer Checklist

- [ ] Every form passes 35% minimum corner radius. No vertex reads as a corner.
- [ ] Appendage silhouette area ≤ 12% of body area
- [ ] Hook footprint ≥ 0.122 H² (25% of the 0.487 H² core silhouette area)
- [ ] ≥ 40% of the hook footprint overlaps the body mass — no detached fins, floating tails, or wings
- [ ] Every appendage tip ≥ 40% of its own base width
- [ ] The 64 px greyscale test passes for every variant, state, and pose
- [ ] Every variant identifies by **Hook** — not Print, not colour
- [ ] All nine print classes remain separable in greyscale at 64 px
- [ ] No painted mark feature smaller than 0.08 H
- [ ] Environment contains only four geometry classes
- [ ] No barrier, hazard, door, pit, or edge anywhere in the world
- [ ] Nothing in the world is taller than 0.6 H
- [ ] The creature casts the only real shadow in the game
- [ ] Only the creature overshoots; props and UI do not
- [ ] No UI control has a sharp corner, grey disabled state, or lock glyph
- [ ] The reading order holds on every screen
- [ ] UI and world share one corner-radius scale and one silhouette vocabulary

---

## Section 4 — Colour System

### 4.0 Canonical Metrics

Every number in this section is computed from the sRGB triple and is checkable in a
spreadsheet. Four metrics are used, and each for exactly one job. Using one metric
for two jobs is how palettes drift.

| Metric | Formula | Range | **What it is the gate for** |
|---|---|---|---|
| **Luma `L`** | Rec. 709, gamma-encoded: `0.2126R + 0.7152G + 0.0722B`, R/G/B normalised 0–1 | 0–100% | **The greyscale test** (§1.2). All greyscale separations below are differences in `L`, because this is what a desaturate-to-grey toggle actually produces. |
| **Saturation `S`** | **HSV (HSB)**, `(max − min) / max` | 0–100% | **The §1.4 Principle 1 budget.** Denominated on HSV, not HSL — see below. |
| **HSL (H/S/L)** | Standard HSL | — | Spec completeness and translation into `palette.css`. **Not a gate.** |
| **WCAG `Y`** | Linearised sRGB relative luminance | 0–1 | **UI contrast ratios only** (§4.7), where it is the correct standard. |

> **Why the budget is denominated in HSV `S`, not HSL `S`.** This is a definition,
> not a relaxation. HSL `S` is a function of lightness, so two colours of
> identical colourfulness report wildly different HSL `S` — Dawn Cream is 69.8%
> HSL `S` at only 15.0% HSV `S`, and Peach Shadow is 82.7% HSL `S` at 35.0% HSV
> `S`. The §1.4 ceilings — **coat ≤25%, environment ≤40%** — land exactly on the
> HSV numbers and are not satisfiable on the HSL numbers. HSV `S` is therefore the
> budget's denominator, and **HSV `S` is what every art, shader, and review tool
> in this project must measure.**

**The entire colour surface of the game derives from three sources.** There is no
fourth: the coat (§4.1 #1), the Cocoa print-mask family (§4.5 — seven values, one
hue family, one material, one texture slot), and the twelve-paint Tier-0 palette
(§4.4.3).

---

### 4.1 Primary Palette

Seven colours. **Five are surfaces** — they may appear on geometry. **Two are light
terms** — they may *never* appear on a surface; they exist so the shared rig (§2.1)
and the UI can reference them without anyone inventing a new value.

| # | Name | Hex | sRGB 0–255 | HSL (H / S / L) | HSV S | **Luma L** | **What it MEANS in this world** | Budget share |
|---|---|---|---|---|---|---|---|---|
| 1 | **Dawn Cream** | `#F7E9D2` | 247, 233, 210 | 37.30° / 69.81% / 89.61% | **15.0%** | **91.9%** | **The blank page, and the creature's own untouched self.** The only large light surface in the game. Because it is the highest-value, lowest-chroma thing in frame it reads as *quiet* — and a saturated mark on it reads as *new*. Meaning: *"nothing here has been decided yet, and that is a good state."* Also the game's paper: menus, cards, and the Results page are this colour under different light. | 15.0% — Tier 1, coat (**cap 25%**) |
| 2 | **Honey Field** | `#E8C79B` | 232, 199, 155 | 34.29° / 62.60% / 75.88% | **33.2%** | **79.5%** | **Sunlight that has landed on something.** The ground bowl, the props, the sunlit side of every surface. It is the colour of *the room being warm*, not of the room being big. Meaning: *"this is the world, and it is pleased to be here."* | 33.2% — Tier 3, environment (**cap 40%**) |
| 3 | **Sunlight Amber** | `#FFE3B8` | 255, 227, 184 | 36.34° / 100% / 86.08% | **27.8%** | **90.1%** | **Light itself.** The key colour, the sun disc, the Flourish's travelling halo, the focus glow. It never tints a surface — it *is* the glow. Meaning: *"this is attention, and attention is warm."* | Light term — **exempt** from surface caps; warm-only by §2.1 law 4 |
| 4 | **Peach Shadow** | `#F6B9A0` | 246, 185, 160 | 17.44° / 82.69% / 79.61% | **35.0%** | **76.9%** | **The colour of not-wrong.** Every shadow, every contact-occlusion, every "that did not happen" is tinted with this. The single most important negative colour in the product: it is what a shadow looks like when nothing is wrong. Also the Flourish halo's warm floor and the unavailable-control's warm floor. | 35.0% — light/contact term (**cap 40%**, never a hero surface) |
| 5 | **Rim Rose** | `#FFB0D0` | 255, 176, 208 | 335.70° / 100% / 84.51% | **31.0%** | **76.5%** | **The creature is alive.** The upper-silhouette half of the two-tone rim, and nothing else in the world. The only colour permitted to sit on a silhouette edge — because an edge glow means *life here*, full stop. | Light term — exempt; **upper rim only** |
| 6 | **Cocoa Ink** | `#4A382A` | 74, 56, 42 | 26.25° / 27.59% / 22.75% | **43.2%** | **23.1%** | **The child's authored line.** Every print mask, record-stamp outline, UI label, icon stroke, and the wet stroke's shadow side. It is the *drawing* colour — intent made permanent. **The only colour in the product permitted below 25% luma.** | 43.2% — Tier 2, print/line (**cap 48.2%**) |
| 7 | **Mint Halo** | `#BFF3E2` | 191, 243, 226 | 160.38° / 68.42% / 85.10% | **21.4%** | **90.5%** | **The creature is grounded.** The base-of-silhouette half of the rim, and the only cool-permitted tone in the product. Bound to *position* (the feet) and *stillness*, never to an event. | Light term — exempt; **no surface use, no UI token** |

#### Binding notes on the primary palette

- **Mint Halo is a hard-limited token.** It appears **only** as the lower rim term,
  at the intensities fixed in §2.1. It may not be used on a surface, in any UI
  token, in any environment material, or in any particle. It is the only hue in the
  product between 150° and 200°, and it is *rationed*, because §2.1 law 4 forbids
  cold light. A pale mint at 85% value and 21% HSV S survives that law; a mint
  *surface* would not.
- **Rim Rose (76.5% L) and Mint Halo (90.5% L) are 14.0% apart in luma and 164.7°
  apart in hue** — the closest-value pair in the palette. This is a defect risk and
  §4.3 rules it out of carrying information: the two rim halves are separated by
  **vertical position on the silhouette**, not by colour.
- **Dawn Cream (91.9% L) and Sunlight Amber (90.1% L) are 1.8% apart.** They are one
  perceptual value band, and **no semantic in this product may depend on telling
  them apart.** This is a deliberate consequence of "one rig, seven modulations"
  (§2.1): the world's lightest surface and the key light are intentionally the same
  value, so nothing is gained by confusing them.
- **"One value step" means one rung of the L0–L8 ladder** defined in §4.5 — not a
  percentage. The machine-readable state rules in §4.6 use the ladder.

---

### 4.2 Semantic Colour Vocabulary

Every row carries a **named redundant non-colour channel**. A row with no backup is
a defect and does not ship (§1.2). Legal backup channels are drawn from
{shape, silhouette, pattern, motion, size, value, position, audio}.

#### 4.2.1 Assigned semantics

| Colour | Hex | **Meaning** | **Redundant non-colour backup channel** | Context |
|---|---|---|---|---|
| **Dawn Cream** | `#F7E9D2` | Unauthored / untouched / resting surface. Also the default page. | **Value** — highest rung, L 91.9%. Always reads as the brightest large field; nothing else sits at this value on a surface. | World + UI |
| **Honey Field** | `#E8C79B` | The world, sunlit and unremarkable. | **Position** (lower third, continuous curve) + **pattern** (untextured, no internal edges) | World |
| **Sunlight Amber** | `#FFE3B8` | Attention / celebration / focus glow. | **Motion** (emissive rise, never a static tint) + **value** (additive luma ≥8% over the local field) | World |
| **Peach Shadow** | `#F6B9A0` | Not-wrong. Shadows, contact occlusion, settled strokes, unavailable controls. | **Value** (L 76.9%) + **softness** (gradient falloff, never a hard edge) | World + UI |
| **Rim Rose** | `#FFB0D0` | Alive. Upper silhouette edge, and nothing else. | **Position** (upper 55% of the silhouette edge) + **value** (additive luma rise) | World |
| **Mint Halo** | `#BFF3E2` | Grounded. Base of the silhouette edge only. | **Position** (lower 45% of the silhouette edge, below the knee) + **stillness** (removed entirely in Painting, whisper in Minigame) | World |
| **Cocoa Ink** | `#4A382A` | The authored line. The drawing itself. | **Shape** — strokes are drawn, never filled regions + **weight** (≥8 px, rounded caps) | World + UI |
| **Tier-0 paint** (12) | see §4.4.3 | *The child chose this.* The only colours with authorship meaning. | **Size** (1.10× when selected) + **value** (one full ladder rung when selected) + **keyline** (4 px Dawn Cream on every swatch face) | World + UI |

#### 4.2.2 The colour vocabulary this game refuses

This game has **no fail state, no health, no danger, no threat, no countdown, no
scarcity, no lock, and no error** (§1.4 Principle 2). Therefore the standard
colour semantics that ship with those systems are **unavailable**. They are listed
here so that no one reintroduces them by muscle memory.

| Refused assignment | Why refused | What this colour is *actually* permitted to mean here |
|---|---|---|
| **Red = error / danger / damage / low health** | There is nothing to be in danger from. A red pixel reads as a problem to a six-year-old, and Pillar 2 forbids showing problems. | **Authorship only.** Cranberry and Cherry Pop are Tier-0 paints. **Red may therefore never appear on any system UI surface, environment material, creature base coat, or light term** — it exists solely as a colour the child can pick. |
| **Gold = currency / rarity / premium / locked** | There is no currency and no rarity. Gold-as-value is the most manipulative visual grammar in games and it is banned outright. | **Warmth and attention.** Sunbeam is a Tier-0 paint; the only gold-family *light* term is Sunlight Amber, meaning attention. |
| **Grey = unavailable / disabled / inactive** | Explicitly forbidden by §1.4 Principle 2 and §3.4. | **Nothing.** Grey has no meaning in this product. Unavailable is 88% scale + one rung down, in Dawn Cream, warm. |
| **Blue = cold / unfriendly / "other"** | §2.1 law 4 forbids cold light, and a cold hue on a creature reads as unapproachable. | **Authorship only.** Bluebell, Splash, Indigo Pop, and Violet Pop are Tier-0 paints. No system surface uses blue. |
| **Green = "go" / success / correct** | Success must be legible in greyscale (§2.6) and must not be graded by performance quality. | **Authorship only.** Grass Pop and Leaf are Tier-0 paints. **Success is carried by shape + motion + a fixed 12-spoke radial burst**, never by a green flash. |
| **Black = premium / depth / danger / "pro"** | A black surface drops below the 25% luminance floor (§2.1 law 1) and reads as a hole. | **Cocoa Ink only** — the authored line. It is a drawing colour, not a surface colour. |
| **Darkened = consequence** | Darkening-as-punishment is the oldest guilt mechanic in games. | **Peach Shadow only** — the colour of *not-wrong*. Shadow means "the light ended here," never "you did badly." |
| **Saturation = quality / performance grade** | Grading the child's work by colour would rank children. | **Saturation is reserved for the child** (§1.4). No system asset may be more saturated than the coat cap. |

---

### 4.3 Colourblind Safety Matrix

#### 4.3.0 The governing result

> **There is no pair of things in this product whose sole differentiator is hue.**

This is not an aspiration; it is a consequence of three structural decisions:

1. **The nine print classes share one hue family** (§4.3.3), so colour vision
   deficiency has no hue signal available to destroy.
2. **Every state, tool, and status in §4.2 is bound to shape, motion, size, value,
   or position** (§1.2).
3. **The disabled, pressed, and selected states all use hue-preserving rules**, so
   none of them introduces a colour that a sibling state lacks.

The residual risk is not in the print system or in state — it is in the
**twelve-paint palette**, where twelve hues are inherently in competition. That
residual is named, quantified, and given backups in §4.3.2.

#### 4.3.0.1 The collapse test — run it on every pair

| ΔLuma | Verdict | Consequence |
|---|---|---|
| **≥ 16.0%** | Colour-independent | No further work required |
| **5.0 – 15.9%** | Colour-weak | Must carry a redundant channel. Value alone is insufficient |
| **2.0 – 4.9%** | **Colour-void** | Must carry a **shape, motion, size, or position** channel. **Value is not a legal carrier** |
| **< 2.0%** | **Non-informational** | Must be **declared as such in writing**. No semantic may depend on it |

#### 4.3.1 Critical pairs

| # | Pair | Luma A / B / **Δ** | Protanopia | Deuteranopia | Tritanopia | **Exact channel that carries it** | Verdict |
|---|---|---|---|---|---|---|---|
| **C1** | **Rim Rose `#FFB0D0` vs Mint Halo `#BFF3E2`** — upper rim vs base rim | 76.5 / 90.5 / **14.0** | Rose loses its red axis, drifts to a neutral grey-violet. The rose/mint *identity* is void. | Identical collapse to protanopia. | Rose largely preserved; mint shifts toward cyan. Hue survives better here than in C2, but the **roles** (alive vs grounded) are still carried positionally. | **Vertical position on the silhouette** — Rim Rose occupies the upper 55% of the silhouette edge, Mint Halo the lower 45%, with a 10% blend band at the knee. *Secondary:* **motion** — the mint half is removed entirely in Painting (§2.4) and the whole rim drops to 0.15 in Minigame (§2.6), so the two terms are never required to be told apart at equal strength in any state that depends on it. *Tertiary:* **audio register** — the idle purr is an octave higher at the crown than at the base. | **HARD BAR: the two rim terms may never be the sole evidence for any state, tool, or result.** C1's 14.0% ΔL is *not* sufficient to lift that bar. |
| **C2** | **Dawn Cream `#F7E9D2` vs Sunlight Amber `#FFE3B8`** — world surface vs key light / halo / focus glow | 91.9 / 90.1 / **1.8** | No separation. | No separation. | No separation. | **Nothing — by design.** | **DECLARED NON-INFORMATIONAL.** The world's lightest surface and the key light share a value band because §2.1 permits exactly one rig. Therefore **Sunlight Amber may not mark a boundary, a target, a selection, a state, or a hit area.** Its only permitted appearances are (a) as a light/emissive term, (b) as the Flourish halo tint, (c) as the **inner 2 px** of the UI focus ring, where an outer Cocoa Ink ring already carries the contrast. **If a reviewer can identify anything in a screenshot *only* by "it is Amber and not Cream," that screenshot fails.** |
| **C3** | **Wet paint vs dry paint** — measured on Cherry Pop `#E42C48` | 33.4 / 39.4 / **6.0** | Impossible to fail — identical hue by construction. | Same. | Same. | **Shape (primary):** a wet stroke carries **0.02–0.04 H of relief**; a dry stroke is flat. **Motion (primary):** wet holds an emissive lift for 0.8 s, then settles to flat matte over 0.6 s. **Value (tertiary):** HSL L ×1.18 while wet, decaying to ×1.00. | PASS. The 6.0% ΔL is **deliberately small** — value is the *third* channel here, not the first (§1.2). |
| **C4** | **Print mask vs painted mark on the same body** — worst case Splash Mint 65.9 vs Sand M5 66.0 | Six paint/mask pairs fall inside the < 2.0% NON-INFORMATIONAL band and are **declared non-informational in value**: Splash Mint/Sand M5 **0.1**; Splash/Sand M5 **0.6**; Cherry Pop/Cocoa M2 **0.7**; Leaf/Tan M4 **1.0**; Grass Pop/Sand M5 **1.4**; Violet Pop/Umber M3 **1.8** | Prints and paint may share hue and may match in value. Both hue and value are unavailable as carriers in the adversarial case. | Same. | Same. | **Shape (primary):** a mark has 0.02–0.04 H relief and a 0.06 H feathered crayon edge; a print is flat texture on the coat. **Pattern (primary):** the mark carries directional streak texture aligned to the stroke vector; a print carries none. **Motion (primary):** the mark runs the wet→dry lifecycle; a print is inert. | PASS on three channels. **MANDATORY:** the 0.02–0.04 H relief must hold on the *darkest* Tier-0 paints (Cranberry 28.3, Cherry Pop 33.4) or they will read flat against a print. **Relief is a shader requirement, not an art request** — carry to §5 and §8. |
| **C5** | **Active UI target vs its card** | 91.9 / 75.8 / **16.1** | No hue change between the two — impossible to fail. | Same. | Same. | **Size (primary):** 1.4× scale. **Value (primary):** one ladder rung. **Position (primary):** the thumb zone, bottom 40% of the display. | PASS, colour-independent. |
| **C6** | **Available vs unavailable control** | 91.9 / 79.3 / **12.6** | Impossible to fail — §1.4 Principle 2 and §3.4 both mandate **no hue change at all**. Unavailable is 88% scale + one ladder rung down. Never greyed, never locked. | Same. | Same. | **Size (primary):** 88%. **Value (primary):** one ladder rung. | PASS, colour-independent. The safest row in the matrix, and safe *because* Pillar 2 removed the semantic that colour would otherwise have had to encode. |
| **C7** | **Minigame success burst** | n/a — the burst is the child's own paint hue and **varies per child** | Nothing to fail. | Same. | Same. | **Pattern (primary):** a **fixed 12-spoke radial**, identical every time, never scaled or re-tinted by performance. **Shape:** body squash 82% → stretch 112% overshoot. | PASS. Because the pattern is **fixed**, there is no hue variance for CVD to destroy. This is why §2.6 forbids colour-varying the burst. |
| **C8** | **Creature vs ground bowl** | 91.9 / 79.5 / **12.4** | Body/ground separation survives on value. The Rim Rose edge loses hue but keeps its additive luma rise. | Same. | Same. | **Value (primary):** the coat sits 12.4% luma above Honey Field. **Additive luma (primary):** the rim must raise the silhouette edge by **≥8.0% luma in the additive buffer** over the adjacent unlit surface. | PASS. |
| **C9** | **The creature's cast shadow vs the ground** — recorded so it is not misread | 76.9 / 79.5 / **2.6** | n/a | n/a | n/a | **Nothing. This pair is deliberately near-invisible.** | **NON-INFORMATIONAL, by design.** The cast shadow's job is to *ground* the creature, not to be read. **No semantic may ever attach to the shadow, and a reviewer must not treat it as a readability channel.** It is also the only real shadow in the product (§3.5), so its low contrast costs nothing. |

#### 4.3.2 The twelve-paint palette — residual-risk disclosure

The Tier-0 palette is the only place in the product where hue carries anything, and
it carries it only *between menu entries*. §3.4's shape grammar gives every swatch
the same pebble silhouette, so a per-swatch silhouette is **not available** as a
backup and **must not be invented** — it would be a second shape vocabulary, which
is precisely the retraining tax §3.4 forbids. The legal backups are therefore
**size, value, and a keyline.**

**Adjacency rule (build-enforced).** Reading the shipped palette in traversal order
(3 columns × 4 rows, row-major), **any two adjacent swatches must satisfy
Δhue ≥ 25.0° OR Δluma ≥ 5.0%.** All seventeen adjacencies pass; the single pair that
fails on hue (Petal ↔ Cranberry, Δhue 5.5°) passes on **Δluma 31.6%**.

**No residual is currently required.** Two pairs were candidates; both clear.

- **Petal / Cranberry** is the tightest *hue* pair in the set at **5.5°** (332.0°
  vs 337.5°), below the 12° hue floor of check 5. It is separated by **value
  instead: 31.6% luma**, which falls in the ">= 16.0% luma delta / colour-independent,
  no further work required" band of the collapse test. It therefore passes check 5 on
  the value alternative and requires no declaration.
- **Tangerine / Grass Pop** was declared a residual here at 2.3% luma. Corrected, they
  are **4.4% apart in value but 69.3° apart in hue**, so they clear check 6 on the
  hue channel outright. **The earlier declaration is withdrawn.**

A residual would be required only if a pair failed *both* channels. No pair in the
current palette does. If a future retune creates one, the remedy below stands, because a
palette entry's *selected* state is a state and §1.2 requires a redundant channel
for every state, tool, and result:

- **1.10° scale when selected, one full ladder rung of value when selected, and a
  4 px Dawn Cream keyline on every swatch face.**
- Tangential colour *choice* — "do I like coral or tangerine?" — is a **preference,
  not information**, and §1.2 does not govern preference.

**Minimum hue separation across the whole Tier-0 palette: 5.5°** (Petal 332.0° /
Cranberry 337.5°). This is *below* the 12° hue floor of check 5 in §4.4.5.
Check 5 now carries a value alternative (≥ 15.0% luma delta); this pair is 31.6%
apart in luma and passes on that channel. 16 of the 17 grid adjacencies clear hue on
hue alone. The previously claimed 15.1° minimum (Cranberry/Cherry Pop) was wrong on
both figure and pair: corrected, Cranberry/Cherry Pop are 13.4° apart, and they are
not the closest pair in the set.

#### 4.3.3 The nine print classes — the greyscale contract

**The finding that resolves this subsection:** the print system uses **one hue
family** — seven values, all between **H 25.00° and H 33.00°** across M0-M5, an 8.0° spread,
rendered from a single material and a single texture slot. **Hue is therefore
constant across all nine print classes**, and protanopia, deuteranopia, and
tritanopia have **zero effect** on print-class separability: there is no hue signal
available to lose.

Separability is carried by exactly three keys:

- **Key A — Local contrast (per class).** ΔLuma between mask and coat. This is what
  makes a 0.08 H feature (5.1 px at a 64 px render) survive mipping,
  anti-aliasing, and 6-bit output.
- **Key B — Integrated luma (per class).** Mean luma of the body at 64 px after the
  pattern integrates at viewing distance. This is what separates classes *from each
  other*.
- **Key C — Edge character.** Already fixed and pairwise-unique by §3.2.

#### 4.3.3.1 The two numeric rules

> **Rule P1 — Feature legibility floor: mask-to-coat local contrast ≥ 25.0% luma.**
> Applied to the 0.08 H minimum feature from §3.1, which is 5.1 px at a 64 px
> front-on render. The lightest mask permitted is therefore **M5 Sand (66.0%)**
> against **Dawn Cream (91.9%) = 25.9%** — inside tolerance, with 0.9 points of
> margin. Any lighter mask fails P1 and is not authored.

> **Rule P2 — Class-to-class separation: integrated luma gap ≥ 5.0%, with a
> categorical override.** Any two classes below 5.0% apart on Key B **must** differ
> categorically on Key C — hard vs. soft edge, or discrete vs. continuous
> connectivity. The override exists because P2 is a *gallery* rule, not a *gameplay*
> rule: print classes are never seen side-by-side during play, because §3.2 fixes
> **Hook carries identity**. P2 governs only the record screen, where a child may
> flip between two saved creatures.

#### 4.3.3.2 Print class assignment and verification

Coat = **Dawn Cream, 91.9% luma**. Mask family = Cocoa, H 25.00–33.00°.

| # | Class | Mask | Mask Luma | **Key A** local contrast | Coverage | **Key B** integrated luma | Key C — edge character |
|---|---|---|---|---|---|---|---|
| 1 | **Solid** | none — coat only | — | 0.0 | 0% | **91.9** | zero internal edges |
| 2 | **Spot-Round** | M0 Deep Cocoa `#33251B` | 15.4 | **76.5** | 8% | **85.8** | isolated blobs, soft edges, ≥5 blobs |
| 3 | **Spot-Angular** | M2 Cocoa `#6B4F3A` | 32.7 | **59.2** | 14% | **83.6** | isolated blobs, 45° sharp corners, ≥5 |
| 4 | **Ripple** | M3 Umber `#876546` | 41.6 | **50.3** | 22% | **80.8** | nested concentric arcs, ≥3 rings |
| 5 | **Checker** | M1 Cocoa Ink `#4A382A` | 23.1 | **68.8** | 30% | **71.3** | hard dither, two orthogonal directions, no dominant direction |
| 6 | **Spiral** | M0 Deep Cocoa `#33251B` | 15.4 | **76.5** | 30% | **69.0** | one unbroken continuous curve, min 0.08 H wide |
| 7 | **Band** | ramp M0 → M2, mean 24.1 | **24.1** *(ramp mean)* | **67.8** *(at ramp mean)* | 42% | **63.4** | smooth ramp, **zero hard edges** |
| 8 | **Stripe-Vertical** | M1 Cocoa Ink `#4A382A` | 23.1 | **68.8** | 50% | **57.5** | hard alternating, **vertical** rhythm |
| 9 | **Stripe-Horizontal** | M0 Deep Cocoa `#33251B` | 15.4 | **76.5** | 62% | **44.5** | hard alternating, **horizontal** rhythm |

> **Ramp rows are stated at the ramp's mean, not across its endpoint range.** Band
> spans M0 Deep Cocoa (luma 15.4) to M2 Cocoa (luma 32.7), but its **Mask Luma** and
> **Key A** columns record the value at the **mean** mask luma, **24.1**, which is how
> `print-classes` records the same class in `palette.json`:
> local-contrast = 91.9 − 24.1 = **67.8**. The endpoint figures (15.4 / 32.7 luma,
> 76.5 / 59.2 contrast) are the ramp's extremes, not the class's identity — 76.5 is
> Spot-Round's and Spiral's Key A, and 59.2 is Spot-Angular's, so quoting the range
> attributed another class's separability to Band. Key B is the column that
> distinguishes Band from its neighbours, and Key B is already a single mean value.
> Both columns are re-derived from this palette by
> `Tools/verify-art-metrics.py`, which fails if this note and `palette.json` diverge.

**Key B, sorted, with gaps:** 91.9 → 85.8 (**6.1**) → 83.6 (**2.2**) → 80.8 (**2.8**)
→ 71.3 (**9.5**) → 69.0 (**2.3**) → 63.4 (**5.6**) → 57.5 (**5.9**) → 44.5 (**13.0**).

**Pairs below the 5.0% P2 floor, and what carries each:**

| Pair | ΔKey B | Categorical override on Key C | Second independent discriminator | Result |
|---|---|---|---|---|
| Spot-Round / Spot-Angular | **2.2** | Soft-edged circle vs. 45° sharp corner | Key A **76.5 vs. 59.2 = 17.3% apart** | **PASS** — two independent keys |
| Spot-Angular / Ripple | **2.8** | Discrete convex blobs vs. nested concentric arcs | Key A **59.2 vs. 50.3 = 8.9% apart** | **PASS** — two independent keys |
| Checker / Spiral | **2.3** | Many small hard shapes in two directions vs. **one continuous curve** | Key A **68.8 vs. 76.5 = 7.7% apart** | **PASS** — two independent keys |

**Minimum required luma delta, stated plainly:**

| Quantity | Required | Achieved minimum | Status |
|---|---|---|---|
| **P1** mask-to-coat local contrast | **≥ 25.0%** | **25.9%** (Sand 66.0 vs coat 91.9) | PASS, +0.9 margin |
| **P2** class-to-class integrated luma | **≥ 5.0%**, categorical override permitted | **2.2%** (Spot-Round / Spot-Angular) | PASS via override |
| **P3** absolute 6-bit floor — no two mask values within one quantisation step | **≥ 1.6%** | **7.7%** (M0 15.4 → M1 23.1) | PASS, 4.8× margin |

Mask ramp luma, verified: **15.4 → 23.1 → 32.7 → 41.6 → 56.4 → 66.0 → 91.9**.
Adjacent gaps: **7.7 / 9.6 / 8.9 / 14.8 / 9.6 / 25.9.** Minimum adjacent ramp gap
**7.7%**, comfortably above the 1.6% 6-bit step. Hue spread across the entire ramp:
**8.0°** — one family, one material, one texture slot, and **zero shader variants
consumed by the print system**, which is what protects the 200-variant ceiling in
`AGENTS.md`.

---

### 4.4 Saturation Budget Allocation

#### 4.4.1 Metric, scope, and tolerance

| | |
|---|---|
| **Denomination** | **HSV (HSB) saturation**, `S = (max − min) / max`, 0–100%. Not HSL — see §4.0. |
| **What the budget is** | A **frame-level resource**, not a per-asset licence. At any instant the frame has 100% of the saturation available to it, and **the child's paint gets all of it.** |
| **Stated tolerance** | **±2.0% absolute HSV S** on every surface tier. **±1.0% absolute luma** on the §4.5 ladder. **±0.5%** on Tier-0 authored values — these are exact, and a swatch 2% off is a different colour. |
| **Measurement procedure** | Eyedrop the frame, sort by HSV S descending, read the top five. **Any pixel above 40.0% HSV S that is not Tier 0 is a defect.** |

#### 4.4.2 Tier definitions and allocation

| Tier | Consumer | **Ceiling (HSV S)** | Peak claim on the 100% budget | Enforced by |
|---|---|---|---|---|
| **Tier 0** | The child's twelve paints | **100%** — no ceiling | **100%** | §4.4.3 palette; of the tiers, only Tier 0 paints and the line-art/line-work tiers 2 and 6 may appear above 40% |
| **Tier 1** | Creature base coat | **25.0%** ±2.0 | ~15% | Dawn Cream at 15.0%. Coat variants may reach 25% but must stay ≤ the cap. |
| **Tier 2** | Print masks (Cocoa family) | **48.2%** ±2.0 | ~48% | Umber 48.1% is the family max. Masks are line-art, not surfaces, so they sit above the surface caps. |
| **Tier 3** | Environment surfaces | **40.0%** ±2.0 | ~35% | Honey Field 33.2%; Peach Shadow 35.0% is the tier's maximum. |
| **Tier 4** | Light terms (no surface use) | **exempt** — but **never above 35.0%** | ~35% | Rim Rose 31.0%, Sunlight Amber 27.8%, Mint Halo 21.4%, Peach Shadow 35.0% |
| **Tier 5** | UI surfaces | **33.2%** ±2.0 — matches Tier 3 | ~33% | §4.7. UI is held at the environment cap, never above it. |
| **Tier 6** | UI text and line work | **48.2%** ±2.0 — matches Tier 2 | ~46% | Cocoa 45.8% (`#6B4F3A`, the caption colour). Raised from 43.2%: Ink is ink; a lower-saturation ink reads muddy, not calm. |

**The budget in one sentence:** *every non-Tier-0 **surface** lives at or below 40%
HSV S (environment 40.0, UI surfaces 33.2); above 40% the only things permitted are
Tier-0 paint, print masks (line art, 48.2), and UI text/line work (48.2) — and none
of those three is a filled surface.*

#### 4.4.3 Tier-0 — the twelve-paint palette

The only colours in the product that are allowed to be loud, because they are the
only ones the child made. Shipped layout is **3 columns × 4 rows, row-major**, and
that order is load-bearing — it is the order the §4.3.2 adjacency rule was solved
against.

| Grid | Paint | Hex | Hue ° | HSV S | **Luma L** | Notes |
|---|---|---|---|---|---|---|
| R1C1 | **Cherry Pop** | `#E42C48` | 350.9 | 80.7% | **33.4%** | Default first pick - highest recognition for the youngest players |
| R1C2 | **Splash** | `#3FC4E0` | 190.4 | 71.9% | **66.6%** | |
| R1C3 | **Sunbeam** | `#F5D33C` | 49.0 | 75.5% | **81.3%** | Highest-luma paint by a wide margin (81.3%; next is Splash at 66.6%) |
| R2C1 | **Bluebell** | `#4A7FE0` | 218.8 | 67.0% | **48.1%** | |
| R2C2 | **Tangerine** | `#F28A2E` | 28.2 | 81.0% | **60.2%** | 69.3° from Grass Pop - clears the hue channel outright, no residual declared |
| R2C3 | **Leaf** | `#4FA83C` | 109.4 | 64.3% | **55.4%** | 1.0% from the Tan M4 mask - value-invisible, see C4 |
| R3C1 | **Violet Pop** | `#8B4FD6` | 266.7 | 63.1% | **39.8%** | Third-darkest paint - **relief shader requirement from C4/R5 applies** |
| R3C2 | **Grass Pop** | `#6FBF3F` | 97.5 | 67.0% | **64.6%** | 69.3° from Tangerine - clears hue outright. 1.4% from the Sand M5 mask - value-invisible, see C4 |
| R3C3 | **Petal** | `#E87FB0` | 332.0 | 45.3% | **59.9%** | Highest-luma of the pinks. Tightest hue pair in the grid with Cranberry (5.5°), separated by value instead - 31.6% |
| R4C1 | **Splash Mint** | `#3FC8A0` | 162.5 | 68.5% | **65.9%** | Distinct from Mint Halo by 47.1% HSV S and 24.6% luma - **may not be confused with the rim term**. 0.1% from the Sand M5 mask - value-invisible, see C4 |
| R4C2 | **Indigo Pop** | `#5B5BE0` | 240.0 | 59.4% | **39.5%** | Least-saturated Tier-0 entry. 1.8% from the Umber M3 mask - value-invisible, see C4 |
| R4C3 | **Cranberry** | `#C4215E` | 337.5 | 83.2% | **28.3%** | **LOWEST-luma paint** - relief requirement applies per C4/R5 |

**Adjacency verification, all 17 pairs in traversal order.** A 3x4 grid has 17
edges (8 horizontal + 9 vertical). The first draft of this section verified only
14 and left three unproven; all 17 are proven below.

| Pair | dHue | dLuma | Pass condition | Verdict |
|---|---|---|---|---|
| Cherry Pop — Splash | 160.5° | 33.2% | both | PASS |
| Splash — Sunbeam | 141.4° | 14.7% | both | PASS |
| Bluebell — Tangerine | 169.4° | 12.1% | both | PASS |
| Tangerine — Leaf | 81.2° | 4.8% | hue only | PASS |
| Violet Pop — Grass Pop | 169.2° | 24.8% | both | PASS |
| Grass Pop — Petal | 125.5° | 4.7% | hue only | PASS |
| Splash Mint — Indigo Pop | 77.5° | 26.4% | both | PASS |
| Indigo Pop — Cranberry | 97.5° | 11.2% | both | PASS |
| Cherry Pop — Bluebell | 132.1° | 14.7% | both | PASS |
| Splash — Tangerine | 162.2° | 6.4% | both | PASS |
| Sunbeam — Leaf | 60.4° | 25.9% | both | PASS |
| Bluebell — Violet Pop | 47.9° | 8.3% | both | PASS |
| Tangerine — Grass Pop | 69.3° | 4.4% | hue only | PASS |
| Leaf — Petal | 137.4° | 4.5% | hue only | PASS |
| Violet Pop — Splash Mint | 104.2° | 26.1% | both | PASS |
| Grass Pop — Indigo Pop | 142.5° | 25.1% | both | PASS |
| **Petal — Cranberry** | **5.5°** | **31.6%** | **luma only** | **PASS** - tightest hue pair in the grid |

**All 17 adjacencies pass.** 16 clear the hue channel (≥ 25.0°); the single
exception, Petal/Cranberry, clears only the luma channel at 31.6%, well above the
15.0% value alternative added to check 5 in §4.4.5. Minimum dHue across the grid
is 5.5° (Petal/Cranberry); minimum dLuma is 4.4% (Tangerine/Grass Pop), a pair that
clears hue instead. No adjacency fails on both channels.

#### 4.4.4 Enforcement rules

1. **The 40.0% HSV S wall is absolute.** No system asset — creature, environment,
   UI, particle, light term — may exceed it. Only Tier-0 paints may.
2. **The 25.0% coat cap cannot be traded.** Raising it to make a creature prettier
   is a design regression, not a quality improvement: it takes saturation away from
   the child's marks.
3. **A new colour is a Pillar 1 and Pillar 3 event.** Adding a colour not derived
   from coat, mask family, or Tier-0 requires an ADR.
4. **Every swatch face carries a 4 px Dawn Cream keyline** so that hue is never the
   boundary between two palette entries.

#### 4.4.5 Build-time checks

| # | Check | Threshold | Failure means |
|---|---|---|---|
| **1** | Coat HSV S | ≤ **25.0%** (+/-2.0 tol) | Coat too saturated — child loses budget |
| **2** | Environment HSV S | ≤ **42.0%** (40.0 + 2.0 tol) | Environment competing with paint |
| **3** | Any non-Tier-0 pixel HSV S | ≤ **40.0%** | §1.4 Principle 1 breach |
| **4** | Tier-0 swatch HSV S | authored ± **0.5%** | Wrong colour |
| **5** | Minimum hue separation, whole Tier-0 set | ≥ **12.0°** OR ≥ **15.0% luma** | CVD adjacency risk |
| **6** | Adjacent-swatch Δhue OR Δluma | ≥ **25.0°** OR ≥ **5.0%** | Two swatches merge under CVD |
| **7** | Any surface below §2.1 law 1 floor | luma ≥ **25.0%** of base | Shadow reads as damage |
| **8** | Any state above §2.1 law 2 ceiling | ratio ≤ **3.5:1** | State becomes visually graded |

---

### 4.5 Value & Contrast Structure

#### 4.5.1 The L0–L8 luma ladder

Nine rungs. **"One value step" means one rung** — every state rule in §2 and §4.6 is
expressed in rungs, never in percentages, so that the whole document moves together
when the ladder is tuned.

| Rung | Name | Hex | **Luma L** | Role |
|---|---|---|---|---|
| **L0** | Deep Cocoa `#33251B` | `#33251B` | **15.4%** | Mask darkest. **Line art only — never a surface.** |
| **L1** | Cocoa Ink | `#4A382A` | **23.1%** | The authored line. **The only rung below the 25% luminance floor, permitted because it is a stroke, not a surface.** |
| **L2** | Cocoa | `#6B4F3A` | **32.7%** | Mask mid-dark. Deep UI text on light surfaces. |
| **L3** | Umber | `#876546` | **41.6%** | Mask mid. The lightest value a *shadow* may reach. |
| **L4** | Tan | `#A98C6B` | **56.4%** | Mask light-mid. Body text on light surfaces. |
| **L5** | Sand | `#C0A584` | **66.0%** | **Lightest mask permitted** (Rule P1, 25.9% over coat). |
| **L6** | Honey Field | `#E8C79B` | **79.5%** | **The environment rung.** Ground bowl, props, UI card surface. |
| **L7** | Dawn Cream | `#F7E9D2` | **91.9%** | **The surface rung.** Coat, page, active control. |
| **L8** | Highlight | `#FFFBF4` | **98.6%** | **Reserved for specular and additive light only.** Never a diffuse surface - it would blow out on 6-bit mobile. |

**The mask family is L0–L5 + L7** — seven values, hue H 25.00–33.00° (8.0° spread),
**one material and one texture slot.** L6 and L8 are world/UI terms, not mask terms.
This is the single most important line in §4.5: it is why the print system costs
**zero shader variants** against the 200-variant ceiling.

#### 4.5.2 The definition of "one value step"

| Rung transition | Ratio | Machine constant |
|---|---|---|
| L7 → L6 | 79.5 / 91.9 = ×0.865 | |
| L6 → L5 | 66.0 / 79.5 = x0.830 | |
| L5 → L4 | 56.4 / 66.0 = x0.855 | |
| L4 → L3 | 41.6 / 56.4 = x0.738 | |

> **Machine constant for all state rules: multiply HSL `L` by 0.85 per value step.**

**Why 0.85 and not the local rung ratio.** The local ratio in the L4–L6 band is
≈ ×0.82, which is *more* contrast than one rung when applied to already-light
values. Since the §2.1 law 2 ceiling (3.5:1) is a **maximum**, the safe direction is
to dim *less* than a full rung. **×0.85 is deliberately conservative: no number of
consecutive steps can accidentally breach the ceiling.** When the ladder is retuned,
re-derive this constant — do not leave it stale.

#### 4.5.3 The luminance floor and the ratio ceiling, as enforceable rules

> **Rule V1 — Luminance floor (§2.1 law 1).** No surface in any state may fall below
> **25.0% of its base rung's luma.** The darkest permitted environment value is
> therefore **L3 Umber, 41.6%** under Ambient. **No asset may ever reach L0–L1 as a
> diffuse surface.** L0–L1 exist exclusively for strokes, print masks, and UI line
> work, all of which are permitted to be dark *because they are lines*.

> **Rule V2 — Ratio ceiling (§2.1 law 2).** No state may exceed **3.5:1** between its
> lightest and darkest visible surface.

**Every §2 state ratio is reachable. This is the derivation, recorded because a
reviewer previously flagged it as impossible and it is not:**

WCAG contrast ratio `= (Lmax + 0.05) / (Lmin + 0.05)`, with `L` as a 0–1 fraction.

| §2 state | Target ratio | Required `Lmin` at `Lmax` = L7 91.9% | Ladder rung that delivers it | Achievable? |
|---|---|---|---|---|
| Menus | 2.1:1 | `0.8271/2.1 - 0.05` = **34.4% Y** | **L3 Umber 41.6%** → **4.09:1** | **Yes** |
| Results | 2.0:1 | `0.8271/2.0 - 0.05` = **36.4% Y** | **L4 Tan 56.4%** → **2.64:1** — clears 2.0:1 with no trim | **Yes** |
| Greeting | 1.9:1 | `0.8271/1.9 - 0.05` = **38.5% Y** | **L4 Tan 56.4%** → **2.64:1** — clears 1.9:1 unaided | **Yes** |
| Minigame | 2.2:1 | `0.8271/2.2 - 0.05` = **32.6% Y** | **L4 Tan 56.4%** → **2.64:1**; L3 Umber → **4.09:1** | **Yes** |
| Ambient | 2.4:1 | `0.8271/2.4 - 0.05` = **29.5% Y** | **L4 Tan 56.4%** → **2.64:1**; L3 Umber → **4.09:1** | **Yes** |
| Painting | 2.9:1 | `0.8271/2.9 - 0.05` = **23.5% Y** | **L2 Cocoa 32.7%** → **6.25:1** | **Yes** |
| Flourish | **3.5:1** *(ceiling)* | `0.8271/3.5 - 0.05` = **18.6% Y** | **L2 Cocoa 32.7%** → **6.25:1**; L1 Cocoa Ink 23.1% → **9.28:1** | **Yes, at the ceiling** |

**Conclusion: no §2 numeral is unachievable. The low-ratio states are simply the
low-rung pairings (L7 with L3/L4), and the Flourish is the only state that reaches
L1 — as an intentional peak, and the only place in the game where a *surface* gets
that dark.** Nothing in §2 or §4 needs amendment.

#### 4.5.4 Greyscale separation contract for the nine print classes

Full verification is in §4.3.3. The contract this section owns:

| Contract | Value | Consequence |
|---|---|---|
| **P1** mask-to-coat local contrast | ≥ **25.0%** luma | No mask lighter than M5 Sand. |
| **P2** class-to-class integrated luma | ≥ **5.0%**, categorical Key-C override permitted | Three pairs use the override; each has a second independent discriminator. |
| **P3** 6-bit quantisation floor | ≥ **1.6%** between adjacent mask rungs | Achieved **7.7%** — 4.8× margin. Safe on mobile output. |
| **P4** hue constancy across the mask family | **≤ 10°** spread | Achieved **8.0°** across M0-M5. M6 Dawn Cream is the coat, not a mask, and is excluded from the spread. CVD cannot affect print separability. |
| **P5** 64 px feature survival | minimum feature **0.08 H** = **5.1 px** | Every class authored above this. Nothing thinner is permitted. |

---

### 4.6 Area & State Colour Variants

Machine-applicable rules over the primary palette. Each state in §2 is a **named
delta from Ambient**, and each delta is expressed in ladder rungs and HSV S so it
can be implemented as a lighting parameter set rather than re-authored per scene.

| State | Key intensity | Rim | Environment value | **Environment HSV S** | Accent / override |
|---|---|---|---|---|---|
| **Idle Ambient** *(baseline)* | ×1.00 | ×1.00, full two-tone | L6 L7 | **33.2%** | — |
| **First Open / Greeting** | ×1.25 | ×1.00 | L7 | 33.2% | **Absolute brightness: +2 steps on entry, flooding −2 steps over 1.2 s to Ambient.** Greeting intensity scales with absence length. |
| **Painting** | ×0.85 | **Warm-only** (mint removed) ×1.15 | **−1 rung** | **×0.75 → 24.9%** | Wet strokes: HSL L ×1.18 + emissive, settling to ×1.00 over 0.6 s |
| **Signature Mark Flourish** | ×1.10, azimuth +15°, elev 62° | **×2.00** | **−2 rungs** | 33.2% | **Mark point light tinted to the mark's own Tier-0 hue**, travelling with the creature. 0.15 s light-source inversion. |
| **Minigame** | ×0.95, azimuth 15°, elev 60° | **0.15** (whisper) | L7, depth-blurred | 33.2% | Success: fixed 12-spoke radial, **never re-tinted by performance** |
| **Results / Reward** | ×1.00 | ×1.00 | L6 L7 + radial sunburst | 33.2% | Symmetrical, centred composition. Rewards grow in at 1.4× with overshoot. |
| **Menus** | ×0.90 | ×1.00 | **×0.85** | **×0.80 → 26.6%** | Horizon band present (only state where it appears). **Never greyed — dimmed.** |

**Universal state invariants, applied on top of every row above:**

- No state exceeds **3.5:1** (§4.5 Rule V2).
- No surface falls below **L3 41.6%** (§4.5 Rule V1).
- No environment surface exceeds **40.0% HSV S** (§4.4.4).
- Warm only — no cold light term is ever introduced by a state (§2.1 law 4).
- **Mint Halo is never re-tinted, only re-scaled.** It is rationed to the base rim.
- Every state transition animates over **≥ 0.4 s** (§2.1 law 6).

---

### 4.7 UI Palette & Divergence

#### 4.7.1 The divergence, stated explicitly

**The UI palette diverges from the world palette in exactly two ways, and no more.**

| | World | UI | Divergence | Justification |
|---|---|---|---|---|
| **Page / card ground** | Honey Field `#E8C79B` (L6) | **Warm Shell `#BEA88B`** | −1 rung (L6 → between L5 and L6) | A held panel must sit *below* the world it floats over, or it competes with the creature (§3.5 reading order). One rung of dimming is the minimum that achieves this without greying — and §2.8 forbids grey. |
| **Active surface** | Dawn Cream `#F7E9D2` (L7) | Dawn Cream `#F7E9D2` (L7) | **None** | The active control is the same paper as the world's brightest surface. This keeps the world's vocabulary intact (§3.4). |

**Everything else is identical.** The UI uses the same coat, the same mask family,
the same ladder, the same light terms, and the same Tier-0 paints. **No UI colour
exists that is not in the world palette** — this is the direct enforcement of §3.4's
"one shape vocabulary, one learning event" rule extended to colour.

#### 4.7.2 UI token table

| Token | Hex | Luma L | HSV S | Role | Redundant channel |
|---|---|---|---|---|---|
| `--ui-shell` | `#BEA88B` | 66.9% | 26.8% | Page ground, menu dim | Value (below the world it floats over) |
| `--ui-surface` | `#DABE98` | 75.8% | 30.3% | Card, panel, palette tray | Value + position |
| `--ui-surface-raised` | `#E8D5B8` | 84.3% | 20.7% | Raised card, tooltip | Value + shadow offset |
| `--ui-active` | `#F7E9D2` | 91.9% | 15.0% | Active control, pressed card | **Size 1.4×** + value + thumb-zone position |
| `--ui-text-primary` | `#4A382A` | 23.1% | 43.2% | All body and label text | Weight ≥ 600, size ≥ 28 px |
| `--ui-text-secondary` | `#6B4F3A` | 32.7% | 45.8% | Supporting caption | Value + weight |
| `--ui-stroke` | `#4A382A` | 23.1% | 43.2% | Icon strokes, control outlines | **Shape** — ≥ 8 px, rounded caps |
| `--ui-keyline` | `#F7E9D2` | 91.9% | 15.0% | 4 px border on every palette swatch | Shape (the border is the boundary) |
| `--ui-focus-outer` | `#4A382A` | 23.1% | 43.2% | Focus ring outer, ≥ 3:1 | Shape (4 px ring) + position |
| `--ui-focus-inner` | `#FFE3B8` | 90.1% | 27.8% | Focus ring inner 2 px only | Shape — **carries nothing; §4.3 C2 forbids it** |
| `--ui-unavailable` | `#DABE98` | 75.8% | 30.3% | Unavailable control fill | **Size 88% + −1 rung. No hue change at all.** |
| `--ui-reward` | Tier-0 dynamic | varies | ≤100% | Earned reward display | Size + growth overshoot + keyline |

#### 4.7.3 Contrast verification (WCAG, §4.0 metric `Y`)

| Pair | Ratio | Verdict |
|---|---|---|
| `--ui-text-primary` on `--ui-active` | **9.28:1** | PASS - AAA at all sizes |
| `--ui-text-primary` on `--ui-surface` | **6.24:1** | PASS - AA at all sizes (≥ 4.5:1, below the 7.0 AAA line) |
| `--ui-text-secondary` on `--ui-surface` | **4.21:1** | PASS - **AA large-text only.** 4.21 is below the 4.5:1 normal-text line, so this pair is legal at Caption 48px weight ≥ 700, or at Body 60px and above — never at Caption weight 600 |
| `--ui-stroke` on `--ui-shell` | **4.85:1** | PASS - AA for non-text UI (≥ 3:1) |
| `--ui-active` on `--ui-shell` | **1.91:1** | **DELIBERATELY LOW.** An active control is distinguished by **1.4x scale and -1 rung of elevation shadow**, never by fill contrast. *If this ratio is ever raised, the UI has started competing with the creature for attention (§3.5).* |
| `--ui-focus-outer` on any surface | **≥ 4.85:1** — exact worst case **4.848:1** on `--ui-shell`; 6.24:1 on `--ui-surface`, 9.28:1 on `--ui-active` | PASS - focus is always visible. Clears WCAG 1.4.11 (≥ 3:1) on every surface, and the project's own 4.5:1 bar. **4.85 is the rounded display value, not the floor** — the floor is 4.848, and a lower-bound claim must not round up |

#### 4.7.4 Greyscale verification of the whole UI

Desaturate any screen to greyscale. Every one of these must remain distinguishable,
and each does so on a non-colour channel:

| Element | Distinguishable in greyscale by |
|---|---|
| Active vs inactive control | **1.4× size** + one rung elevation shadow |
| Available vs unavailable | **88% size** — never colour |
| Pressed | **4% vertical squash** + one rung lighter |
| Selected paint swatch | **1.10× scale** + one rung + 4 px keyline |
| Icon vs container | **Silhouette** — the entire §3.4 shape grammar |
| Focus | **4 px ring** — geometry, not tint |

---

### 4.8 Reviewer Checklist

**Greyscale first — every item is checked with colour removed:**

- [ ] Desaturated to greyscale, every state, tool, and result is still distinguishable
- [ ] No information anywhere is carried by hue alone
- [ ] No rim term, or pair of rim terms, is the sole evidence for a state (C1 hard bar)
- [ ] Nothing is identifiable only by "it is Amber and not Cream" (C2)
- [ ] The cast shadow carries no semantic and is not used as a readability channel (C9)

**Budget:**

- [ ] Coat HSV S ≤ **25.0%** (+/-2.0 tolerance)
- [ ] Environment HSV S ≤ **42.0%** (40.0 + tolerance)
- [ ] Top-five most saturated pixels in any frame are **all** Tier-0 paint
- [ ] No non-Tier-0 pixel exceeds **40.0%** HSV S
- [ ] No new colour exists that is not derived from coat, mask family, or Tier-0

**Value:**

- [ ] Every surface sits at **L3 or above** (Rule V1); L0–L1 appear only as strokes
- [ ] No state exceeds **3.5:1** (Rule V2)
- [ ] Every "one value step" in the scene is one **ladder rung**, not a percentage
- [ ] The ×0.85 machine constant has been re-derived if the ladder was retuned

**Print system:**

- [ ] Mask-to-coat local contrast ≥ **25.0%** on all nine classes (P1)
- [ ] Any class pair under 5.0% integrated luma has a documented Key-C override **and** a second independent discriminator (P2)
- [ ] Adjacent mask rungs ≥ **1.6%** apart for 6-bit output (P3)
- [ ] Mask hue spread ≤ **10°** — one family, one material, one texture slot (P4)
- [ ] No print feature thinner than **0.08 H** (P5)
- [ ] The print system consumes **zero** shader variants

**Palette grid:**

- [ ] All 17 adjacent-swatch pairs satisfy Δhue ≥ 25.0° **or** Δluma ≥ 5.0%
- [ ] Every swatch face carries a 4 px Dawn Cream keyline
- [ ] **Splash Mint is never confused with the Mint Halo rim term** by any reader
- [ ] The two relief-critical paints (Cranberry 28.3%, Cherry Pop 33.4%) hold 0.02–0.04 H relief against a print — *carry to shader verification*

**Refusals:**

- [ ] No red-as-error, no grey-as-unavailable, no gold-as-currency, no green-as-success
- [ ] No darkened-as-consequence; all shadows are Peach Shadow
- [ ] No colour is used to grade the quality of the child's work

---

## Section 4c — Typography Spec

> **Scope statement, binding.** This is a **small, bold, legible** type system, not a
> rich-text system. There are no paragraphs, no lore, no dialogue, no quests, no
> help text, and no body copy anywhere in the shipped product — the sole exceptions
> are the adult parent gate and platform-legal boilerplate, both of which use the
> platform system font. §3.4's five-word cap means **the overwhelming majority of
> every string in this game is one or two words.** Nothing below specifies
> justified text, hyphenation, drop caps, multi-column layout, small caps, inline
> emphasis, footnotes, or rich text, because the product has none of them and
> specifying them invites their invention.

### 4c.1 Type Roles

Thirteen roles. This list is **closed** — a fourteenth role requires an ADR.

| # | Role | Purpose | **Max words** | Required? | Non-text channel that carries meaning when text is removed |
|---|---|---|---|---|---|
| 1 | **Idle control label** | The resting name of a button, tab, or tool | **3** | **Optional — by law** (§3.4 / `AGENTS.md` §2.5) | Icon silhouette + pebble/card/ribbon container + the physical response on touch |
| 2 | **Active control label** | Names the currently selected tool or swatch | **2** | Optional | Icon silhouette + **1.4× scale** + one ladder rung + thumb-zone position (C5) |
| 3 | **Dialogue-free feedback stamp** | The word that names what just happened (Results, Flourish, minigame) | **3** | Optional | Creature body shape + fixed 12-spoke radial burst + audio register shift (§1.2, §2.6) |
| 4 | **Record item title** | The name of a saved mark or creature on the record card | **3** | **Required** — it is the child's authorship and has no icon; the thumbnail *is* the identity | Saved-artwork thumbnail + record-stamp ribbon shape + position in the child-authored grid |
| 5 | **Child-entered name** | The name the child gives their creature | **1 word, 10 characters** | Required *if* text entry ships (gated on an ADR) | The creature's own silhouette + its Hook — the child recognises their pet's shape, not its letters |
| 6 | **Serial / count** | Marks made, strokes laid down | **0 words** (≤4 characters) | Optional | **The count is duplicated as N filled pebble segments** in the counter pill — a count of shapes, not a number. Also position in a row |
| 7 | **Caption** | One warm line explaining a gesture the icon cannot show alone (`hold to fill`) | **5** | Optional; **max 2 per screen** | Icon + the live gesture-preview animation running inside the icon |
| 8 | **System banner headline** | Names the screen or moment (menus, parent gate) | **3** | Optional | The horizon band + camera angle (§2.8) + the container ribbon shape |
| 9 | **Parent-gate heading** | The one place reading *is* required — adult-only | **8** | Required (adult surface) | Single pebble + hold gesture; §1.2 still binds the affirmative action to shape |
| 10 | **Parent-gate body** | Adult instruction copy | **40 words total for the entire gate** | Required (adult surface) | Adult-only by policy; no child ever sees it |
| 11 | **OS notification payload** | Rendered by the platform in the **system font**, at the OS's chosen size and weight | **3 words, 24 characters** | Required | **The notification icon *is* the creature silhouette** (§3.1) — 24–48 px, no detail, zero colour differentiation available |
| 12 | **Store marketing headline** | App Store / Play screenshots and promo art — *outside* the app | **4** | Required | Creature silhouette + flourish halo + Tier-0 palette swatch |
| 13 | **Store marketing subhead** | One line under the store headline | **8** | Required | Three-swatch palette + app icon + creature Hook |
| — | *Store/legal boilerplate* | *Platform-rendered system font. Not authored by this spec.* | — | — | — |

**Global cap: 5 words per control, everywhere, including roles whose column says
less.** Where a role's cap is lower, that lower number is the derived *layout*
limit (§4c.3).

### 4c.2 Font Families

**The load-bearing decision: the product ships ONE text family.** §3.4's "one shape
vocabulary, one learning event" argument applies with full force to type. A second
rounded family — even a beautiful one — reintroduces the retraining tax on every
screen, and Nunito at weight 900 with negative tracking is already visually
distinct enough to carry display. This is Pillar 3 (rich from restraint) expressed
as a font decision.

| Slot | Family | **Licence** | Weights shipped | **Fallback chain (first hit wins)** | What it communicates about Candy Sunrise |
|---|---|---|---|---|---|
| **Display** | **Nunito** (variable 200–1000; static instances generated at build) | **SIL OFL 1.1** — permits commercial use, bundling, and embedding in a shipped binary; **no per-seat fee, no seat tracking, no network, no runtime telemetry**. No Reserved Font Names declared. | **700 / 800 / 900** | `Nunito` → **SF Pro Rounded** (iOS 16+ system) → **Roboto** (`sans-serif`, Android) → `sans-serif` | Rounded terminals and a tall, generous ascender frame reproduce §3.4's "echo the world, flattened" in the one medium that can carry it. Humanist skeleton, not geometric: Candy Sunrise is a **warm room with a living thing in it**, not a candy-factory assembly line. |
| **Body / UI-label** | **Nunito** *(same family — deliberately)* | SIL OFL 1.1 | **600 / 700** | identical chain | High x-height (≈0.55 em) and a large aperture are what make a 48 px label legible to a six-year-old at 240 mm. One family means the letterforms a child meets on the Results screen are the same ones they met on the palette. |
| **Numeric** | **Nunito** with OpenType **`tnum`** (tabular figures) enabled on the count role | SIL OFL 1.1 | **700** | identical chain | Counts appear **once** and settle (§2.7: nothing new arrives during Results). Tabular figures prevent the icon row from re-flowing when a count increments — a 2 px jitter in a pebble is a hierarchy violation per §3.5. |
| **Monospace** | **DM Mono** — *shipped only if the record-serial stamp requires it* | SIL OFL 1.1 | **400 only** | `DM Mono` → `monospace` | **This product does not need a monospace face.** There is no code, no rapidly-changing readout, and nothing the child must transcribe character-by-character. It is named and licensed here so the fallback chain is complete and so the one fixed-pitch job (the record serial) has a sanctioned answer, but it ships in a single weight and is **fenced off from the child's play surface.** |
| **Child's hand** *(conditional)* | **Patrick Hand** | SIL OFL 1.1 | **400 only** | `Patrick Hand` → `cursive` → system | **The only handwriting in the game is the child's own name, rendered on the record card.** §1.4 Principle 1 reserves "the authored line" for the child; if the *system* wrote in a hand, it would steal the single channel that says "the child made this." This is a build-time selection from three fixed hands at onboarding, so a hand can never appear on a system control. |

**Rejected alternatives, recorded so they are not re-litigated:**

- **Fredoka** (OFL) as display — more explicitly "candy," but its weight range tops out short of 900 and its numerals are less separable at 48 px. Rejected.
- **Lexend** (OFL) as body — genuinely excellent readability research, but its angled terminals read slightly clinical against §3.4's no-sharp-corners law, and its x-height advantage is not material at our sizes. Rejected.
- **Any slab serif, script face, Cooper-style retro face, or blackletter** — rejected on §3.4 (second visual language) and, for scripts, because it collides with the child's authorship channel.

**Fallback discipline (Unity 6.3 / TMP):**

1. Fonts are **bundled** — `Assets/Fonts/*.ttf`, statically imported, `Include In Build`. **`Font.CreateDynamicFontFromOSFont` is forbidden**: it makes the shipped layout depend on device state and gives no fallback warning.
2. Build **static** TTF instances at 600/700/800/900. The variable font is a source file, not a build artefact.
3. **Subset to ~200 glyphs** (Latin basic, the handful of typographic marks actually used, digits, and the exact product string table). OFL expressly permits subsetting. Budget: **≤ 28 KB per weight → ≤ 112 KB for four weights.**
4. The mandatory OFL licence text ships in `Assets/ThirdPartyLicenses/`. Bundling the licence file is an OFL obligation, not a formality.
5. The fallback chain is **geometric grotesque → rounded → grotesque**. It is **never permitted to terminate in a serif.** A serif fallback would put a Times-like face in a Candy Sunrise UI, break the radius language, and produce a screen that fails §3.4's recognition check.
6. **TMP SDF, not MSDF.** MSDF exists to recover *sharp corners* at small sizes — this product has none (§4c.5) and it costs fill rate on the 6-bit floor. Use SDF with **12 px glyph padding** to protect the rounded terminals against atlas bleed. One dynamic atlas per weight; `Multi Atlas` **off**.
7. If Addressables is still not installed (`AGENTS.md` §4), fonts must **not** be routed through it.

### 4c.3 Size Scale

**Reference resolution: 1080 × 1920, portrait.** All sizes below are **design px at
that reference**. The density bucket for a 1080p Android device is `xxhdpi`, so
**1 design px = 1/3 dp = 1/3 sp**; the `sp` column is what a UI programmer types.

**Base size: 48 px (16 sp) at 1080 × 1920.** This ladder is **not** a geometric
series and has **no single step ratio**. The step ratios are
**1.25 / 1.40 / 1.3333 / 1.2857 / 1.3333**, and every tier size is a fixed integer.
A new tier is added by choosing one of those steps deliberately, never by
multiplying the tier below it by a mean. See the note under the table.

| Tier | **px @1080×1920** | **sp** | Weight | **Tracking (1/1000 em)** | **Max words** *(derived layout limit)* | Used for |
|---|---|---|---|---|---|---|
| **Caption** | **48** | 16 | **700** | **+8** | 5 | Caption role; small labels in dense controls; counters. **The absolute floor of the product.** |
| **Body** | **60** | 20 | **600** | **0** | 5 | Idle control labels, record item titles, banner headlines |
| **Lead** | **84** | 28 | **700** | **0** | 4 | Feedback stamps, Results heading, palette tab names |
| **Subhead** | **112** | 37 | **800** | **−10** | 3 | Card titles, ability-slot names |
| **Headline** | **144** | 48 | **800** | **−20** | 2 | Flourish stamp, single reward name |
| **Display** | **192** | 64 | **900** | **−30** | 1 | Store art, parent-gate heading, and the **only** in-world display string (the Flourish banner name) |

*Ladder: 48 / 60 / 84 / 112 / 144 / 192 — a rounded major third. Widest step 60 → 84 at **1.40**; tightest 48 → 60 at **1.25**. A "scale ratio of 1.32" used to be quoted here; that was the geometric mean of the whole ladder, not a step anyone multiplies by. Applying it yields 63.4 / 83.6 / 110.3 / 145.6, none of which are tiers. The gate in `Tools/verify-art-metrics.py` re-derives each step ratio from the tier sizes and fails if this sentence disagrees.*

**Word caps are derived, not invented.** Content width is **936 px** (1080 − 2 × 72 px
side margins, matching the §3.4 minimum touch target). At an average
rounded-humanist advance of ≈0.52 em: Display admits ≈9 characters (1 word), Headline
≈12 (2), Subhead ≈16 (3), Lead ≈21 (4), Body ≈30 and Caption ≈37 (5 — §3.4's cap
binds before layout does). **The five-word cap is therefore never a layout
constraint below Headline.**

#### The 48 px minimum — defended

**48 px at 1080 × 1920 is 16 sp. It is 1.45× the iOS/HWC minimum legible size
(11 pt = 33 px at 1080p) and it clears the worst-case device, not the flagship.**

Worst case modelled: **5.0″ 720 × 1280, ≈256 ppi**, `CanvasScaler` reference
1080×1920 → scale 0.667.

| Step | Value |
|---|---|
| Caption at 48 design px on a 720p device | **32 device px em** → **3.17 mm em** |
| Cap-height at 0.70 em | 22.4 device px → **2.22 mm** |
| Viewing distance, 6-year-old holding a phone | **240 mm** (child range 200–300 mm; adult range 350–400 mm) |
| Angular subtense of cap-height | **0.53° = 31.8 arcmin** |
| Child acuity floor for the same task (6/9 equivalent) | **≈40 arcmin** |
| Margin | **1.26×** — clear, with real headroom |
| Stem thickness, weight 700 (≈0.14 em) | **4.5 device px = 0.44 mm** |
| One 6-bit quantisation step at 256 ppi | ≈0.10 mm → **4.4× margin** |

**The size floor and the weight floor are one rule, not two.** At weight 400 the same
cap-height has ≈2.5 px stems, which fail both the 6-bit floor and §3.4's "no
interior detail below 12 px." **The 48 px minimum is only safe because §4c.4 sets
the weight floor at 600.** A reviewer who lowers one must lower neither; a reviewer
who lowers both ships an unreadable game.

**Corollary, and the reason 48 px is the floor rather than 40:** at 48 px, a
two-word caption is ≈150 px wide on a 936 px content width — 16% of the line. Type
is never the thing that fills a screen in this product. That is the §1.1 rule
expressed in a metric.

### 4c.4 Weight Hierarchy

**Shipped weights: 600 / 700 / 800 / 900. That is the entire set.**

| Context | **Exact weight** | Numerals | Never used |
|---|---|---|---|
| Caption, counters, serial | **700** | `tnum` | 600 |
| Idle control label, record item title | **600** | — | 500 |
| Active control label, feedback stamp, banner headline | **700** | `tnum` | 600 at Caption size |
| Subhead, Headline | **800** | — | 700 |
| Display | **900** | — | 800 |
| Parent gate (adult surface, platform font) | 400–700 | — | — |

#### What weight does **not** do

> **Weight is a role channel only. Weight never expresses state.**

- It does not express **available vs. unavailable.** That is 88% scale + one ladder rung, with **no hue change at all** (§4.7.2, C6). A label never goes light because it cannot be used.
- It does not express **pressed.** Pressed is 4% vertical squash + one value step (§3.4).
- It does not express **selected.** Selection is 1.4× scale + one rung + thumb-zone position (C5).
- It does not express **success, failure, error, warning, urgency, newness, or rarity.** Every one of those is refused by §1.4 Principle 2 or §4.2.2, and none may be smuggled in through stroke modulation. There is no bold-means-good convention in this game, because there is nothing that is good or bad.
- A label that changes state may gain 100 weight **only** as reinforcement of a change already carried by ≥1.4× scale or a full ladder rung. **Weight alone is never a legal state channel.**

#### Why nothing below 400 — and why 400 is still never authored

1. **The 6-bit / low-DPI floor.** Weights 100–300 in a rounded face put stems below 3 px at 48 design px on the 720p worst case, and below §4.3.3's **P3 6-bit quantisation floor** (1.6% luma between adjacent values; the P3 margin is 4.8× and a hairline stem consumes it in one pixel).
2. **Hairline terminals are the *inhabited* strokes.** The thinnest parts of `l`, `t`, `f`, `r`, `i`, `j` and the dot on `i` are their terminals. Those are precisely the glyphs that spell the words a six-year-old is still learning. A 250-weight round face erases its own legibility at exactly the ages we ship to.
3. **The forbidden-grey argument, and this is the decisive one.** §1.4 Principle 2 forbids grey-as-unavailable, and §4.2.2 gives grey **no meaning in this product**. A thin weight *is* a soft grey — it is greyness in the one channel where the document has deliberately left the semantic empty. Authoring thin text would make "faded" mean "quiet," and then the next designer would use it for "unavailable." **Grey has no meaning; therefore no weight may approximate grey.**
4. **400 is a fallback default, never an authored value.** If Nunito fails to load, the fallback (SF Pro Rounded / Roboto) arrives at its own default weight. That degraded path is allowed to be whatever the fallback gives; it is **not** a design option and may never be authored into a spec, a token, or a mock.

#### The one 400 that is legal

**Patrick Hand at 400, role 5 (child-entered name), and nowhere else.** Its single
weight is the minimum viable weight of a marker hand, not a thin weight — and it is
the child's voice, not the system's. It is carved out of rule 3 explicitly:
*handwriting is authorship, so the system may borrow it only from the child, only as
the child's own name.*

### 4c.5 Line Height, Tracking, Alignment

#### Line height

| Tier | **Ratio** | Justification |
|---|---|---|
| Caption, Body | **1.20** | Applied only when a caption wraps to a second line. A rounded face has tall ascenders and short descenders; below 1.20 the `l` in the second line crowds the cap above it. |
| Lead, Subhead, Headline, Display | **1.10** | These are 1–3 word settings in a fixed geometry. At 1.20 a two-line Headline would occupy 346 px — a third of a 1920 px screen — to hold one word pair. 1.10 exists only to prevent diacritic clipping and descender collision. |

**Hard line rule: 2 lines maximum, anywhere in the product.** Nothing is 3 lines.
Nothing is a paragraph. A control's label is **always 1 line** — it is set on a
single line box and vertically centred in its container, and the container is sized
from the *reserved* box (§4c.7), never from the measured text.

**Not specified, deliberately, because the product has none:** hyphenation,
justified or ragged-right multi-column body text, drop caps, hanging punctuation,
ligature-driven justification, first-line indents, widow/orphan control, hyphen
dictionaries, inline emphasis, footnote markers. Their absence here is a *decision*,
and adding one is an ADR event.

#### Tracking

Values are in **1/1000 em**, at the reference resolution, and apply **after** kerning:

| Tier | Tracking | Why |
|---|---|---|
| Caption (48) | **+8** | Optical compensation. Small rounded faces have proportionally large sidebearings; a slight *positive* track tightens the word's colour and helps a non-fluent reader segment it as one shape rather than as separate letters. |
| Body (60) | **0** | The neutral point. Nunito's spacing is authored well here and any correction is a liability. |
| Lead (84) | **0** | Tracking corrections below ~90 px are below the perceptual threshold on a 6-bit screen. |
| Subhead (112) | **−10** | Tracking begins to matter above 90 px. This is the point where the word shape starts to read as a *form* — the same reason §3.1 cares about silhouette. |
| Headline (144) | **−20** | Rounded terminals already carry generous air. |
| Display (192) | **−30** | Compensates for the roundness. A rectangular face needs roughly half this; Nunito needs the full value or the Display word opens visible holes. |

**Hand-set kerning pairs are forbidden.** Tracking comes from the token table above,
full stop. Hand kerning is the fastest way for eight screens to drift into four
different type styles.

#### Alignment

- **Left-aligned, always.** A six-year-old re-reads the same control many times; a left edge gives a fixed return target every time. Centred text gives a different origin on every re-read and is the single most common cause of a UI that "feels slippery."
- **The reserved box.** Each control carries a **reserved label box**, sized at build time to the **widest string in that control's full state set** (every label it can ever show). Alignment is left *inside* the box; the box is centred in the control. One line therefore *appears* centred, and a label swap from 3 words to 1 word **cannot re-centre or reflow the pebble.** This is the anti-jiggle rule and it is a build-time constant, not a runtime measurement.
- **Tabular figures (`tnum`) on every count**, for the same reason.

#### The icon-to-label ratio and the lockup rule

**Ratio: the icon's bounding height is 1.6 × the label's cap-height, floored at
72 px** (the §3.4 minimum touch target) **and capped at 2.4 × the label's em.**

| Tier | Label px | Cap-height | Icon px |
|---|---|---|---|
| Caption | 48 | 33.6 | **72** (floor) |
| Body | 60 | 42 | **72** (floor) |
| Lead | 84 | 58.8 | **96** |
| Subhead | 112 | 78.4 | **128** |
| Headline | 144 | 100.8 | **168** |
| Display | 192 | 134.4 | **216** |

The 1.6 ratio is the §3.4 rule "shows the icon first and larger" in a number: the
icon's height is **1.6× the letter's height**, and the icon's bounding box — not the
text — is what the child is aiming at. The icon is never permitted to be smaller
than the label's em box in any dimension.

**Two lockups only:**

- **L1 — Icon over label.** The default. Vertical stack: icon, then a gap of **0.25 × the label's em**, then the label centred beneath it. Used by every in-world control. Container height = icon + gap + cap-height + 2 × 24 px padding. The label's width is capped at the container's inner width − 32 px and **may never exceed the icon's inner 60% width** — which is why L1 stacks rather than nests.
- **L2 — Icon inline.** Records and ribbons only. Icon left, label right, **max 2 words**, optical vertical alignment (label baseline = icon centre + 0.30 × the label's em, not the icon's geometric centre).

**Optical nudges (the only two permitted):** the icon artboard may sit up to **3% of
its own height** above geometric centre to compensate for descender mass; an L2
label box may be shifted left by **4% of the icon width** to compensate for the
icon's visual mass. Nothing else may be nudged, and no nudge may exceed these
values.

**The two-removals test, which is the enforceable form of §1.2 applied to type:**

1. **Text removed** → every control must still be identifiable by icon shape and by what happens when touched. **Binding on 100% of controls.**
2. **Text *and* icon removed** → required only for the **four hub controls** (paint, place, play, records), which must be separable by container silhouette alone. **Binding on those four. Not required elsewhere**, because inventing a unique silhouette per control would be a second shape vocabulary — the exact retraining tax §3.4 exists to prevent.

#### Glyphs are exempt from the corner-radius rule — stated, and justified

> **Exemption.** §3.4 mandates "zero sharp corners." **That rule binds type
> *containers* — the card, the ribbon, the label plate — and binds nothing about the
> glyphs inside them.** Glyph corner radius is **not** constrained by this document.
> The exemption is not a loosening; it is the rule read correctly.

Three reasons, in order of force:

1. **The rule's own text scopes it to geometry.** §3.1 reads "No **polygon** edge may read as a corner," and §3.4's rule sits in a table of *container* rules. A glyph outline is a rendered curve, not a polygon edge, and it is not a shape in the world. Applying the rule to letters would be an extension of its scope, which is what §1.3 forbids when it says any question "not settled by this document is settled here."
2. **A glyph corner is not a physical hazard, and that is the entire purpose of the rule.** §3.3 forbids doors, gates, walls, pits, and stairs because *the child could get stuck or hurt*. The counter of an `A` cannot be touched, cannot snag, and cannot be mistaken for a ledge. The rule protects a six-year-old's hand and eye from hazards in the world. Letters are not in the world.
3. **Enforcing it would destroy legibility, which is the higher-priority harm.** §3.1's 35%-of-shortest-axis radius applied to a 48 px round-face glyph set rounds the counters of `e`, `a`, `s`, and `g` shut and collapses the letterforms. That is a direct violation of `AGENTS.md` §2.5 — nothing in the UI may require reading — and a *worse* outcome than the corner rule protects against, because one is cosmetic and the other is unreadable.

**The one shape guarantee that keeps the exemption honest:** **the glyph is inset
from its container by at least 12 px, so the rounded container is always the
silhouette the child sees first.** A rounded pebble with a flat letter inside still
reads as a rounded pebble. The child's shape-recognition system — the thing §3.4 is
actually protecting — never sees the glyph's corners at all.

**Typeface-level consequence:** the family choice itself enforces this. We select a
font with **round terminals and no sharp-cut variants**, so no letterform in the
product has a corner to begin with.

### 4c.6 Special Treatments

#### The one permitted treatment: **Pressed Ink**

**Cocoa Ink fill + a 1.5 px Dawn Cream highlight at 2 px offset up-left, 55%
opacity.** Applied at Caption and above.

This is the "warm paper" treatment, and it is worth having because it costs nothing
and it is *correct*: §1.3 fixes the key light as **behind and above**, so an inset
bevel catching light on the upper-left face is the physically honest read for
something lying flat on a lit page. The label then reads as **stamped into the
paper** rather than floating above it — which is exactly what §4.1 already says
Cocoa Ink is: *the authored line, intent made permanent.*

**It introduces no colour.** It is a two-token composite of the two text tokens
already in §4.7.2 (`--ui-text-primary` + `--ui-active`). The composite's effective
luma sits between L1 and L7, but it is a 1.5 px edge treatment on the glyph
boundary, never the glyph body — **the readable contrast is unchanged at the
verified 9.28:1.** No new token, no new hue, no new entry in `palette.json`.

**Cap: one treatment per string. Never combined with a drop shadow or an outline.**

#### Permitted effects and hard limits

| Effect | Limit | Legality |
|---|---|---|
| **Pressed Ink highlight** | 1.5 px, 2 px up-left, Dawn Cream @ 55% | Above. The default at every tier |
| **Outline / stroke** | **Max 2.0 px (0.67 dp)**, Cocoa Ink, **Display tier only**, and only when the string overlaps the world (Flourish banner, record card over the creature) | §4.3.3 P3 — 2 px survives mip downsampling and 6-bit output where 1 px would not. Above 2 px it reads as a different, "sticker" typeface |
| **Drop shadow** | **One per string.** Offset **y +3 px**, blur **4 px**, Peach Shadow `#F6B9A0` @ **0.30 opacity** | Warm, never grey or black (§2.1 law 3). Restricted to **Ribbon containers and Display tier over the world** — banned on all in-container text, where a shadow can enter a glyph counter and eat the margin the 9.28:1 ratio depends on |
| **Dark outer glow** | Cocoa Ink, radius **6 px**, **35% opacity**, offset **2 px down**, Display/Headline only, world overlays only | Legal because it is a *stroke* token (§4.5 Rule V1 exception) and it adds luma **drop**, which is a value carrier, not a hue one |

#### Glow — and the C2 constraint, enforced

> **Sunlight Amber glow on text is FORBIDDEN. Unconditionally. It is not a matter of
> taste.**

Two independent reasons, either sufficient:

1. **It is invisible.** §4.1 records Dawn Cream at 91.9% luma and Sunlight Amber at 90.1% — a **1.8% luma delta**. An amber glow on a cream field does not glow; it does nothing. Worse than nothing, because an artist will author it, believe it works, and not check.
2. **It is illegal.** §4.3 **C2** declares the Amber/Cream pair **NON-INFORMATIONAL** and states that Amber "may not mark a boundary, a target, a selection, a state, or a hit area." A glow is a marker. Amber glow on text is therefore not merely ineffective — it is a §1.2 breach, because a reviewer could in principle be asked to identify something *by the glow*.

**If a glow is required, it is a Cocoa Ink dark outer glow**, per the table above.
Cocoa Ink on Dawn Cream is **9.28:1** — verified, AAA at all sizes (§4.7.3) — and it
is a value carrier in a product whose greyscale test is the primary review gate.

#### Additive light terms on text — forbidden, with the arithmetic

No Highlight L8 `#FFFBF4` additive term, no lightening screen, no bloom on a text
layer, at any opacity, including the 0.15 s Flourish light-inversion.

**Reason.** Any additive light term applied to Cocoa Ink drives its luma up from L1
23.1% toward L8 99.0%, and contrast against Dawn Cream falls as the denominator's
`+0.05` term dominates. At the *lowest* additive opacity anyone would propose —
**12%** — Ink moves to ≈32% luma, dropping the verified **9.28:1** to roughly
**2.7:1**, which fails even large-text AA. **There is no opacity at which additive
light on Ink is both visible and legal.** So: **text is excluded from every additive
and bloom pass in the project.** A bloom that catches the icon is fine; a bloom that
catches the glyph is a bug.

#### Forbidden entirely

| Forbidden | Why |
|---|---|
| **Any glow in Sunlight Amber** | C2. Above |
| **Any additive / lightening term on text** | Contrast arithmetic. Above |
| **Italic, oblique, or synthetic slanting** | A slant shears counters and thins the joins on a round face; Nunito's oblique at 600 closes the aperture of `a` and `e`. Also: slant is a *mood* channel, and mood in this product is carried by lighting (§2), never by type |
| **Underline, strike-through, dashed emphasis** | §3.4: "Zero rectangles. Zero sharp corners. **Zero underlines.**" An underline is also a rectangle |
| **All-caps, small caps, caps-for-emphasis** | Caps reduce the effective word shape and destroy the "one word, one form" read that helps a pre-reader. Nothing in this product is set in caps except three legal proper nouns |
| **Gradient fills, multi-colour glyphs, two-tone text** | Two colours in one glyph reintroduces hue as a carrier and breaks Tier 6's 48.2% saturation claim |
| **Text shadows of more than one colour, or shadow + outline stacked** | Two effects on one string is decoration, and §1.1 resolves decoration-vs-clarity as *clarity wins* |
| **Blur, bevel, emboss, inner shadow, chrome, neon** | Same |
| **Typewriter, scramble, reveal, marquee, ticker, wave, bounce, pulse, breathing scale** | Type motion that reads as urgency is banned by §1.4 Principle 2, and §3.5 rule 5 is a hard split: *only the creature overshoots.* Type uses **linear, damped easing, zero overshoot, max 200 ms.** No word-by-word stagger. No per-character animation |
| **Ellipsis / silent truncation** | A child cannot tell a truncated word from a complete one. Truncation is forbidden; overflow is a layout bug (§4c.7) |
| **Hand-set kerning pairs** | Drift. Tracking comes from the token table only |
| **Any text on the creature's body** | The body is the child's canvas. Type there would out-shout the child's mark and violate §1.4 Principle 1's "the child's marks are always the loudest thing in frame" |
| **Any new colour token for type** | §4.4.4 rule 3 — a new colour is an ADR event. Every glyph in this product is Cocoa Ink, Cocoa, or Dawn Cream |

### 4c.7 Accessibility

#### Minimum size

**48 design px (16 sp) at 1080 × 1920, at reference resolution, everywhere — with no
exceptions.** Not in tooltips, not in notifications (OS-controlled anyway), not in
debug overlays, not in the parent gate. The derivation is §4c.3.

**Tooltips and captions are the temptation and are banned from shrinking further.**
If a caption does not fit, it wraps to two lines and its container grows. It never
goes to 40 px.

#### Minimum contrast — citing §4.7.3 as verified

| Pair | **Verified ratio** | Rule |
|---|---|---|
| `--ui-text-primary #4A382A` on `--ui-active #F7E9D2` | **9.28:1** - AAA at all sizes | The default for every label on a control |
| `--ui-text-primary` on `--ui-surface #DABE98` | **6.24:1** - AA at all sizes (≥ 4.5:1) | Legal for record titles and card titles |
| `--ui-text-secondary #6B4F3A` on `--ui-surface` | **4.21:1** - AA large-text only (< 4.5:1, so it is size- and weight-bound) | See the placement rule and the size-coupling rule below |

**Placement rule (binding, and it closes a real gap):** `--ui-text-secondary` is
permitted **only** on `--ui-surface`, `--ui-surface-raised`, or `--ui-shell`. **It is
never placed on `--ui-active`,** because §4.7.3 verifies no ratio for that pair and
this document does not invent numbers it cannot verify. A caption needing to sit on
an active control uses `--ui-text-primary` instead. One token, one background, one
verified ratio — no surprises at review.

**Size coupling for secondary text:** §4.7.3's "≥ 24 px caption minimum" is the
**WCAG large-text contrast threshold**, quoted in CSS/dp units (18 pt) — not a
design-canvas figure. Converted to this scale it binds as:

> `--ui-text-secondary` is legal **only at Caption (48 px) with weight ≥ 700** —
> 16 dp bold, which clears WCAG's 14 pt-bold large-text threshold — **or at Body
> (60 px) and above at any permitted weight.** Below those two conditions it is not
> used on any screen.

Both cases also satisfy §4.7.2's redundant channel for the token ("value + weight"):
secondary text is always **one rung lighter and ≥100 weight heavier** than the
primary text it sits under.

#### Behaviour at 200% text scale

**We do not consume full OS text scale, and this is a decision, not an omission.**

*Why not.* Our layout geometry is fixed by non-negotiables: 72 px minimum touch
targets (§3.4), controls inside the bottom 40% of the display (§3.4), and
**drag/tap parity with identical silhouettes** (`AGENTS.md` §4). A 200% text scale
multiplies label boxes by 2 and pushes most labels past the container, breaking the
pebble geometry that makes the UI icon-first in the first place. Honouring it would
mean destroying §3.4 to serve a setting that `AGENTS.md` §2.5 says is not
load-bearing.

*What we do instead.*

1. **Read the OS scale and clamp it to 1.00–1.40.** The child's need for larger type is real and is served — just at 1.4×, which our geometry survives.
2. **The Large setting is a parent-gated tier promotion: every role moves up exactly one tier (×1.40)** — Caption 48 → 66, Body 60 → 84, Lead 84 → 112, and so on. Only the Caption↔Body promotion is a nudge; from Lead upward it lands on an existing tier, so **no new token is created.**
3. **At Large, no control grows in width.** The label grows, the icon grows to hold the 1.6:1 lockup, and if the label would exceed the container's inner 60% width it **wraps to a second line and the container's height grows by one line.** The width is fixed; the pebble never reflows horizontally, so the thumb-zone grid never moves.
4. **Truncation stays forbidden at Large.** If a label still does not fit after 2 lines, the **string is wrong**, and the fix is the string table — never ellipsis, never auto-shrink, never clipping. Auto-shrink would reintroduce a size below the §4c.3 floor, and an ellipsis is unreadable by a pre-reader by construction.
5. **We still surface the OS setting.** If the OS scale is above 1.40, a Warm Shell ribbon offers the parent-gated Large setting rather than silently ignoring the system.

#### The greyscale check for text

Desaturate any screen. Every label must still be a **readable letterform**, and every
text role must still be **rankable**.

| Check | Result |
|---|---|
| `--ui-text-primary` on `--ui-active`, greyscale | Luma gap **68.8%** — §4.3.0 "colour-independent" band (≥16%) with **4.3× margin** |
| `--ui-text-secondary` on `--ui-surface`, greyscale | Luma gap **43.05%** — same band, **2.69× margin**. This is the **weakest legal text pair in the product** and is declared here so a reviewer tests it first. Note this is a luma gap and is unrelated to the 4.21:1 WCAG ratio on the same pair, which is a relative-luminance measure |
| Role ranking in greyscale | Carried by **weight and size only**: adjacent text roles differ by **≥100 weight** or by **≥1.28× size**. Never by colour |
| Any two adjacent labels | Must differ in **word shape, tier, or container** — never by tone alone |

**Text-specific greyscale test:** render a screen in greyscale at **1280 × 720**.
Every string must be readable without knowing what colour it was, and no two labels
on the same screen may become indistinguishable. **A label that requires colour to
be read is a defect, not a colour limitation.**

#### §1.6 — state changes when the *text* is what changed

This is the hard case. §1.6 requires every state transition to carry a shape delta, a
motion delta, **and** an audio delta. **A label swap, on its own, fails that test**,
because a word changing is not a shape, a motion, and a sound.

> **Rule L1 — A text-bearing control may never change state by text alone.** Any
> label swap is accompanied by all four of the following:

| # | Requirement | Detail |
|---|---|---|
| 1 | **Icon silhouette changes** | Not a hue swap, not a tint, not a fill flip on the same outline — the outline itself changes shape (e.g. outlined pebble → filled pebble). §1.2's tool-type rule, applied to type |
| 2 | **Motion delta** | 200 ms cross-fade + 4 px rise, **linear and damped, zero overshoot** (§3.5 rule 5: only the creature overshoots). Never a bounce, never a scale pop, never a typewriter |
| 3 | **Audio delta** | The control **reuses an existing state's pitch / timbre / register shift.** A label swap **may never introduce a new sound.** If no existing state owns the sound, the swap is not shipped |
| 4 | **Size or value delta** | The tier changes by ≥1 step, **or** the container changes by 1.4× scale or one ladder rung. Weight alone is not legal (§4c.4) |

**Rule L2 — The control never resizes.** The reserved label box (§4c.5) is sized to
the **widest string in the control's entire state set** and fixed at build time. A
3-word → 1-word swap changes the ink inside a stationary box. **A control that
changes width when its label changes is a defect.**

**Rule L3 — No label may encode a number that moves.** A changing count is a state,
and a changing glyph inside a static box is the *worst* possible carrier — the
child's eye is on the number, not the container. All counts live in role 6, in a
separate counter pill whose **pebble-segment count duplicates the number as shapes**
(§4c.1 role 6).

**Rule L4 — The closed set of permitted label swaps.** Four pairs. Adding a fifth is
an ADR event.

| Pair | Label A → B | Icon silhouette change | Audio | The state it reuses |
|---|---|---|---|---|
| 1 | `empty` → `your mark` (first signature mark saved) | outline pebble → pebble with a painted dot | single soft click, up a fifth | §2.5 Flourish settle |
| 2 | `sleepy` → `ready` (ability slot armed) | closed frill → open frill | idle purr shifts up an octave | C1's existing crown/base register split |
| 3 | `save` → `saved` | notched pebble outline → same pebble filled | one soft click, up a fifth | §2.7 Results settle |
| 4 | `look` → `match` (minigame prompt change) | eye → two-tone pattern swatch | prompt chime, +3 semitones | §2.6 minigame register |

**Forbidden label swaps, explicitly:** any string containing a number that
increments; any string containing a time, timer, or countdown; `best`, `score`,
`level`, `win`, `wrong`, `try again`, `failed`, `locked`, `unlocked`, `new!`, or any
exclamation mark. **Exclamation marks are banned in all product strings** — urgency
punctuation is a problem signal by definition, and §1.4 Principle 2 has no semantic
for it.

### 4c.8 Reviewer Checklist

**Roles and scope**

- [ ] The string count is verifiable: `rg` the product string table and confirm **every** string is ≤5 words and ≤2 lines
- [ ] Only the 13 §4c.1 roles exist; any 14th has an ADR
- [ ] No paragraph, lore text, dialogue, quest, tutorial prose, or help text exists anywhere in the build
- [ ] `--ui-text-secondary` appears **only** on `--ui-surface`, `--ui-surface-raised`, or `--ui-shell` — never on `--ui-active`
- [ ] Secondary text is at Caption@700 or Body+ and nowhere else

**Sizes**

- [ ] No string anywhere renders below **48 design px at 1080×1920**
- [ ] Minimum sizes were checked on a **5″ 720×1280 (256 ppi)** render, not a flagship
- [ ] Every tier in §4c.3 is used at least once; no ad-hoc sizes exist outside the ladder
- [ ] Display is 1 word, Headline 2, Subhead 3 — verified against the 936 px content width

**Weights**

- [ ] Shipped weights are exactly **600 / 700 / 800 / 900**
- [ ] Nothing is authored at 400 or below in Nunito
- [ ] **No weight change anywhere encodes a state** — press, disable, select, success, warning
- [ ] Weight never changes without an accompanying ≥1.4× size or one-rung value change

**Families and licensing**

- [ ] Nunito and Patrick Hand carry **SIL OFL 1.1**; the licence text is bundled in `Assets/ThirdPartyLicenses/`
- [ ] No font in the project carries a per-seat fee, a seat-tracking clause, or an app-embedding prohibition
- [ ] Fonts are **bundled**; `Font.CreateDynamicFontFromOSFont` appears nowhere
- [ ] Font files are subsetted to the product string table; total ≤ **112 KB**
- [ ] Fallback chain terminates in a **grotesque, never a serif**
- [ ] `Multi Atlas` is off; SDF (not MSDF) is used with 12 px glyph padding

**Shape conformance**

- [ ] Every control's **container** obeys the §3.4 corner-radius scale
- [ ] Every glyph is inset **≥12 px** from its container
- [ ] No typeface in the project has square-cut terminals or a sharp-corner variant
- [ ] Glyph corner-radius exemption (§4c.5) is applied as *scope reading*, not as a relaxation

**Lockups**

- [ ] Icon height = **1.6 × label cap-height**, floored at 72 px, in every lockup
- [ ] L1 stacks vertically with a 0.25 em gap; L2 inline is capped at 2 words
- [ ] Labels never exceed the icon's inner 60% width in L1
- [ ] Labels are left-aligned inside a reserved box; the box is centred
- [ ] **The two-removals test passes:** text removed → all controls identifiable; text + icon removed → the four hub controls identifiable

**Tracking, leading, alignment**

- [ ] Tracking values match §4c.5 per tier; no hand-set kern pairs exist
- [ ] Line height 1.20 ≤ Body, 1.10 ≥ Lead
- [ ] Nothing is 3 lines; no justified text, hyphenation, or drop caps anywhere

**Treatments**

- [ ] **No Sunlight Amber glow on text** — C2
- [ ] **No additive light term on text** at any opacity, including in the Flourish
- [ ] Outline ≤ 2 px, Display tier only
- [ ] One drop shadow per string maximum; Peach Shadow @ 0.30, 3 px / 4 px, ribbon or world-overlay only
- [ ] No italic, underline, strike, all-caps, gradient fill, blur, bevel, or two-tone glyph
- [ ] No type animation longer than 200 ms, and zero overshoot
- [ ] No ellipsis or truncation anywhere
- [ ] No text on the creature's body

**Accessibility**

- [ ] Greyscale render at 1280×720: every string readable, no two adjacent labels confusable
- [ ] Primary-on-active is the default pairing on every control
- [ ] OS text scale is read, clamped to 1.00–1.40, and surfaced to the parent gate
- [ ] At Large, no control changes width; overflow wraps to a 2nd line and the container grows in height
- [ ] Every label swap satisfies L1 (icon + motion + audio + size/value) using the closed set in L4
- [ ] **Every control with a label swap has a fixed reserved box sized to its widest state** — L2 verified by resizing every state in the editor
- [ ] No exclamation mark, no timer, no count, no score word in any product string
- [ ] Notification payload ≤3 words / 24 characters; notification icon is the creature silhouette

---

## Section 4d — Visual Anchor Prompt

The full anchor lives in **`design/art/style-anchor-prompt.md`** because it is
consumed by tooling (`/asset-spec`) and needs to be editable without touching this
document. Its contract with this section:

| Element | Requirement |
|---|---|
| **Modularity** | The anchor is a single prefix block. Assets interpolate a `subject:` fragment after it. The anchor itself is **never edited per-asset** — an asset that seems to need a different anchor is wrong, not the anchor. |
| **Palette anchoring** | The anchor names palette colours by **both** name and hex, cross-referenced to `palette.json`. A generator never has to infer a hue. |
| **Interpolation markers** | `[coat-print]`, `[hook]`, `[tilt]`, `[paint]`, `[state]`, `[prop]` — one closed substitution table instead of re-describing axes. |
| **Negative prompts** | Strong and specific, covering all ten forbidden categories from §4.2.2 and §3.3, plus photorealism, cel-shading, and text-in-image. |
| **Per-state variants** | All seven §2 states as named deltas from Idle Ambient. |
| **Known generator weaknesses** | A published list of the failures AI generators reliably produce against this style — blue shadows, high-frequency noise, sharp ear tips, saturated output, bloom-as-rim — with the instruction to **correct in-engine rather than re-prompt**. This exists so the team does not waste cycles on defects that are the tool's, not the prompt's. |

---

## Section 5 — Character Design Direction

> **Scope note, binding.** This game has **no enemies, no NPCs, no allies, and no
> combat cast.** The assigned brief's "distinguishing feature rules per character
> type (enemies/NPCs/allies)" has **nothing to design against**, and inventing a
> cast would violate §1.4 Principle 3 and Pillar 3 in a single stroke. What exists
> is: **one player creature** (varianted), **six painted-mark Echo ability
> manifestations** (transient, non-creature), and **reward objects** (1.5 s
> transient hero). That is the entire character population. This section is written
> against it.

> **Surface ruling, binding.** The creature is a **matte painted shell**. It reads
> as a soft warm rim along the top and back edge; it is **never fur, never fibre,
> never fibre-cards, never a shag silhouette**. The reason is mechanical, not
> stylistic: **the child's paint sits on this surface**, and a fibrous or furred
> surface destroys a flat canvas. Every reference in this section to the coat is a
> reference to a smooth, matte, painted shell.

---

### 5.1 The Player Creature — Archetype

**The archetype: a plump, limbless-seeming hatchling that is more surface than
animal — a soft rounded volume with a large head, no neck, stub feet, and a blank
cream shell that behaves like a sheet of good paper.** The read is *"something that
has just been made and is waiting to be drawn on."* Not a mascot (mascots are
pre-animated and pre-decorated), not a pet (pets are finished objects), and
definitely not a plush (plush implies fur, and fur cannot be painted on). The
archetype's job is to be **legible, receptive, and unfinished.** Every proportion
below exists to serve that: a big head reads as young and inviting, no neck reads as
one soft mass rather than a body with a head on it, and a *flatter-than-expected
shell* reads as a surface waiting to receive something. The creature must never be
the most interesting thing on its own body — that job belongs to the child's marks.

#### Canonical hatchling form

All values are fractions of total silhouette height **H**. **No §3.1 number is
changed.** Three are *added* (mouth, shell flatness, shell gloss), and the §3.1
step-2 hook test is applied in its **ratified** form (core-silhouette footprint).

| Measurement | Value | Status |
|---|---|---|
| Total silhouette height | 1.00 H | restated, unchanged |
| Minimum silhouette width | 0.62 H | restated, unchanged |
| Head diameter | 0.48 H | restated, unchanged |
| Head centre height | 0.74 H | restated, unchanged |
| Narrowest neck constriction | ≥ 0.70 × head width — **no neck** | restated, unchanged |
| Eye diameter | 0.17 H | restated; **spec detail added:** no pupil, no iris, no lashes — a single self-lit lens + one 1.5 px L8 specular dot. At 64 px the eye is 10.9 px; a pupil is sub-pixel detail that costs 3 px and reads as dirt. |
| Limb nubs (feet, arm nubs) | ≤ 0.10 H long, ≤ 0.06 H wide | restated, unchanged |
| Ground clearance | ≤ 0.04 H | restated, unchanged |
| Pattern / mark feature, minimum | 0.08 H | restated, unchanged (matches P5, §4.3.3) |
| **Mouth** | **0.09 H wide × 0.02 H deep value crease.** Not an opening — no dark interior, ever. | **ADDED** |
| **Shell albedo flatness** | **± 1.0% luma across the entire body** | **ADDED** — see §5.5. A mottled or occluded shell makes every mark edge ambiguous. |
| **Shell gloss (specular)** | **0.02** | **ADDED** — the shell is the flattest, dullest surface in the game. Wet paint is 0.35. That 17× ratio is the entire wet/dry read. |
| **Appendage silhouette area outside body mass** | **≤ 12%** = **0.058 H²** | restated, unchanged |
| Base body triangles | 8–12k (author at **10.0k**) | restated, unchanged |
| Rig | < 45 bones | restated, unchanged |
| Cast shadow | exists, **0 triangles** | **RESTATED AS ZERO-GEOMETRY.** See §5.7. |

#### The 64 px hook test, as ratified

> **A hook's footprint must be ≥ 25% of the body's CORE SILHOUETTE AREA — the egg
> inscribed in the 0.62 × 1.00 H bounding box, ≈ 0.487 H². Minimum footprint
> therefore 0.122 H². And at least 40% of that footprint (≥ 0.049 H²) must overlap
> the body mass, because the 12% cap only permits 0.073 H² of *new outline*.**

The 12% appendage cap governs **new outline**, and the 40% overlap clause is what
forces every hook to be **structurally attached**: a hook that floats clear of the
body spends its whole budget on new outline and fails at the threshold. A detached
fin, a floating tail, or a wing cannot clear 0.049 H² of body overlap without
either enlarging past the cap or reading as a second object. This is why
**detached fins, floating tails, and wings are structurally excluded** from the
vocabulary, not merely discouraged.

#### The load-bearing base-mesh decision

> **The base mesh ships with zero appendages.** No ears, no crest, no frill, no fin,
> no tail. Feet and arm nubs stay (they are ≤ 0.10 H and break the silhouette —
> that is what they are for).

This is not a preference, it is what makes the system legal. §3.2 permits
**exactly one** attached form per variant. If ears were in the base, a
`Floppy-Ear` hook would produce a three-eared creature and a `Crest` hook a
two-eared one, and the "one hook carries identity" rule would have failed on the
first variant shipped. With a bare base, every hook is genuinely the *only*
attached form, the neutral silhouette is a pure closed egg, and that egg is exactly
what the 24–48 px OS notification icon needs (§3.1) and what the core-silhouette
arithmetic above assumes. **The default creature ships as `Floppy-Ear` / TILT
(1.00, 1.00, 1.00, 1.00) / `Solid`.**

---

### 5.2 Variant System — Authoring Rules

A variant is legally **one base mesh + one rig + one HOOK + one TILT vector + one
PRINT.** No fourth channel. No exceptions.

#### Procedure

| Step | Action | Gate |
|---|---|---|
| 1 | Pick **one** hook from the closed library | Footprint **≥ 0.122 H²**; body overlap **≥ 0.049 H²**; new outline **≤ 0.073 H²** |
| 2 | Set the **4-value TILT vector** | All four inside the legal range; none outside |
| 3 | Pick **one** PRINT from the nine (§3.2) | Zero shader variants consumed |
| 4 | Assign the variant's **shell** | Dawn Cream only. The shell is never re-tinted to make a variant "prettier" (§4.4.4 rule 2) |
| 5 | **Run the 64 px gate** | Below. Reject and re-author on any failure |
| 6 | **Run the three-channel check** | Greyscale, 64 px, front-on, no motion: hook readable, print readable, and the two do not collapse into each other |

#### HOOK library — closed set of three

**All three are crown- or nape-anchored and structurally fused to the body mass.**
There is no rear-anchored or flank-anchored hook, because a hook that sits behind
the body cannot reach the 0.049 H² overlap clause without becoming a floating tail.

| ID | Type | Form | Footprint | Body overlap | New outline | Anchored at | 64 px read |
|---|---|---|---|---|---|---|---|
| **H1** | **Crest** | single centre fan, 3–5 rounded lobes, fused to the crown | **0.140 H²** | **0.104 H²** (74%) | **0.036 H²** (4.9% of body area) | crown centre | a raised fan clearly above the head-line |
| **H2** | **Frill** | single continuous nape ridge, fused, wrapping the back of the head | **0.128 H²** | **0.092 H²** (72%) | **0.036 H²** (4.9%) | nape | a raised collar behind the head; widest, lowest, softest |
| **H3** | **Floppy-Ear** | matched pair of soft teardrops, fused at the base, hanging down and out | **0.134 H²** | **0.061 H²** (46%) | **0.073 H²** (9.9% of body area) | crown, front-biased | two lobes hanging past the head-line at the sides |

Every hook obeys the 35% minimum corner radius and every tip is **≥ 40% of its own
base width.** Note the budget spread: **Crest** is the cheapest hook and buys the
most height; **Floppy-Ear** consumes exactly the full 0.073 H² new-outline
allowance and is therefore the *only* hook that could not be enlarged. All three sit
at or near the 0.122 H² floor — a fourth hook would need either a wider silhouette
or a new body form.

**A fourth hook is an ADR plus a Pillar 3 review**, exactly as §3.2 requires for a
tenth print class. **Structurally excluded, permanently:** detached fins, floating
tails, wings, horns on a stalk, and any hook that does not clear 0.049 H² of body
overlap.

**Second discriminator axis — hook *position* and *extent above the head-line*.**
Two positions (crown / nape) × three forms gives three pairwise-distinct
silhouettes at 32 px, and the three deliberately occupy **different vertical
bands**: Crest reads **above** the head, Frill reads **at** the head-line and
behind it, Floppy-Ear reads **below and beside** it. A child who cannot tell Crest
from Frill by form will tell them apart by whether the shape rises or falls from
the skull.

#### TILT — the 4-value vector

| Value | Neutral | **Legal range** | Derived §3.1 limit | Why the range stops where it does |
|---|---|---|---|---|
| **Head scale** | 1.00 | **0.86 – 1.16** | head Ø **0.41 – 0.56 H** | Outside, head:body stops reading as a hatchling and the 0.48 H canonical value becomes unrecoverable |
| **Body length** | 1.00 | **0.88 – 1.14** | min width **0.55 – 0.71 H** | Below 0.55 H the mass reads as a lollipop (§3.1's own stated reason); above 0.71 H the ground-clearance and rim proportions break |
| **Leg length** | 1.00 | **0.80 – 1.20** | clearance ≤ **0.048 H**, nubs ≤ **0.12 H** | Nubs that reach 0.12 H begin to break the closed-mass silhouette at 64 px |
| **Base mass** | 1.00 | **0.88 – 1.14** | — | Past 0.14 the centre of gravity drops so far that the §5.6 bounce stops reading as a *hop* and starts reading as a *roll* |

**TILT is quantised to 0.02 steps** — it is stored in the save file and must be
byte-reproducible. The legal envelope is therefore **16 × 14 × 21 × 14 = 65,856
TILT vectors**, and the closed variant space is **3 hooks × 65,856 × 9 prints =
1,778,112 distinct creatures from one mesh, one rig, one material, and two draw
calls.** That number is the Pillar 3 argument, entire.

**The TILT-after-hook rule:** hook footprint and overlap are measured on the
**final TILT-applied silhouette**, not on the neutral one. TILT may not push a
hook below 0.122 H² footprint, below 0.049 H² body overlap, or above 0.073 H² new
outline.

**Declined:** this section does **not** specify how many variants ship, how they are
unlocked, or which are named. That is content, owned by the producer and the
Abilities GDD, and specifying it here would be exactly the "new content" the §1.1
axis table refuses.

#### Hook carries identity. Print carries flavour. Tilt carries archetype.

This is §3.2's rule and this section enforces it as an **authoring permission
table**, not a slogan:

| Channel | May a reviewer reject a variant on this alone? | Rationale |
|---|---|---|
| **HOOK** | **Yes — instant reject** | The only channel that survives the 24 px OS notification icon, where zero colour and zero detail are available (§3.1) |
| **TILT** | Yes | "This one is small," "this one is long" — the archetype read, checkable at 64 px as proportion |
| **PRINT** | **No — never, alone** | A variant tellable only by Print is a recolour. It fails the 64 px test and it is unshippable to the notification icon |
| **Shell colour** | **No — never, alone** | Shell is Dawn Cream on every variant. There is no per-variant shell colour |

#### Worked example — three contrasting variants, one mesh

| | **"Lump"** | **"Ribbon"** | **"Comet"** |
|---|---|---|---|
| Hook | **H3 Floppy-Ear** | **H2 Frill** | **H1 Crest** |
| TILT (head, length, leg, mass) | 1.16 / 0.88 / 0.80 / 1.14 | 0.86 / 1.14 / 1.20 / 0.88 | 0.94 / 1.06 / 1.10 / 0.94 |
| Print | **Solid** (integrated luma 91.9) | **Stripe-Horizontal** (44.5) | **Spiral** (69.0) |
| Reads as | small, heavy, wide, planted | long, light, in motion, in a hurry | tallest crown, forward energy |
| 64 px silhouette | wide low dome, **two lobes hanging below the head-line** | tall oval, **raised collar at and behind the head-line**, **3–4 horizontal bands** | **fan above the head-line**, **one unbroken curve** |
| Hook footprint ✓ (≥ 0.122 H²) | **0.134** | **0.128** | **0.140** |
| Body overlap ✓ (≥ 0.049 H²) | **0.061** (46%) | **0.092** (72%) | **0.104** (74%) |
| New outline ✓ (≤ 0.073 H²) | **0.073** (at cap) | **0.036** | **0.036** |
| Print pair gap ✓ (P2 ≥ 5%) | 91.9 → 44.5 = **47.4** | 91.9 → 69.0 = **22.9** | 69.0 → 44.5 = **24.5** |

All three clear §4.3.3's P2 with **no categorical override needed** — a rare and
desirable outcome, achieved by choosing three prints from opposite ends of the
integrated-luma ladder.

#### The 64 px gate — explicit, four steps, all four mandatory

Render front-on, greyscale, 64 px tall, no motion, three value steps from
background.

1. **Silhouette.** Closed rounded mass. Two lobes, or any hook element piercing the
   outline, = **fail** (appendage budget exceeded).
2. **Hook footprint.** Footprint **≥ 0.122 H²**, body overlap **≥ 0.049 H²**,
   new outline **≤ 0.073 H²**. At 64 px the hook is **8–9 px** of recognisable
   shape across at least one axis. Smaller than that and it is a detail, not an
   identity. **Fail.**
3. **Print.** The print class is separable from every other print class in
   greyscale at 64 px, with its §4.3.3 override documented if integrated luma is
   under 5%. **Fail** if undeclared.
4. **Cross-channel.** The hook is *still* identifiable with the print masked out,
   and the print is *still* identifiable with the hook masked out. **This step is
   new to this section** and is the one that catches real defects: a crest whose
   lobes merge into a checkerboard, or a spiral whose curve reads as a collar.
   **Fail** if either masking breaks legibility.

---

### 5.3 Distinguishing Feature Rules

Adapted to what exists. Each row is separable at **64 px in greyscale**, and each
names the channel that survives colour removal.

#### (a) Three hook types, apart

**Primary: hook silhouette — form + position + vertical band.** All three hooks sit
at or above 0.122 H² footprint, so at 64 px each is **8–9 px** of readable shape.
The three are separable on **vertical band** alone: Crest rises **above** the
head-line, Frill sits **at** it, Floppy-Ear hangs **below** it. That single
orthogonal axis carries all three pairs.
**Secondary: integrated luma of the print**, available but never load-bearing.
**Tertiary: audio.** Each hook carries one purr resonance (§5.6) — not a different
pitch per hook, but a different **harmonic emphasis**: crown hooks ring the octave,
nape hooks ring the fundamental. Optional, decorative, never required.

#### (b) The six Echo manifestations, apart

An Echo is **not a creature, not a mesh, and not a variant.** It is a transient
manifestation bound to one painted mark. Six families, closed set, keyed on
**growth axis** so that no two share a motion signature.

| ID | Family | Growth axis | Motion signature | Caps |
|---|---|---|---|---|
| **E1** | **Column** | +Y from the mark site | grows up, holds, collapses inward | ≤ 0.55 H tall, 0.18 H wide |
| **E2** | **Drape** | −Y from the hook line | falls and hangs, settles with 1 bounce | ≤ 0.34 H drop, 3–5 strands |
| **E3** | **Sweep** | −X → +X across the flank | **one** lateral pass, no repeat | 0.8–1.0 H wide, 0.08 H thick |
| **E4** | **Orbit** | none — no growth | continuous closed elliptical travel | 2–3 nodes, 0.90 × 0.55 H path |
| **E5** | **Halo** | +Y above the crown | static arc, 15° rotation over 2.0 s | 0.42 H radius, 0.06 H tube |
| **E6** | **Buds** | radial from the mark site | 3–5 discrete buds, **staggered 80 ms** scale-in, then hold | 0.06–0.12 H each |

**Greyscale separability:** all six are separable on **silhouette + motion** with
colour fully removed, because the six growth axes (up, down, lateral-pass, orbit,
static-arc, staggered-radial) are pairwise distinct. A seventh family is an ADR.

**Explicitly refused:** no Echo family is radially symmetric. §2.6 has already
claimed the **fixed 12-spoke radial** for minigame success, and a second
radially-symmetric family would make success ambiguous in exactly the frame where
it matters most. E6 is radial but **staggered and non-uniform** — 3–5 unequal buds
on 80 ms offsets — which is a different read at a glance and in greyscale.

**Echo geometry may not rise above 1.25 H total silhouette.** §3.5 rule 3 makes the
creature the highest point in frame; an Echo that climbs above it becomes a second
subject and the frame loses its single centre.

**Echo geometry is exempt from the 12% hook cap.** The cap governs *identity
silhouette*, and an Echo is transient, non-identity, and present for under 3.5 s.
The binding constraints on Echo geometry are instead §3.1's hard geometry rules —
**35% minimum corner radius, tips ≥ 40% of base width, no cone / spike / blade** —
plus the caps in the table above. An Echo with a sharp tip is a defect even though
an Echo tip is not part of the silhouette budget.

**Declined:** the six **abilities themselves** are a game-design decision owned by
the Abilities GDD. This section specifies only the *visual encoding envelope* — six
growth axes, caps, greyscale discriminators, and a six-step audio register set
spanning a major sixth (§5.6). Which mark binds to which family is not this
section's to fix.

#### (c) Own creature vs. a saved record

**Primary: the eye.** §3.5 rule 4 makes live eyes the brightest non-painted element
in frame, carrying a small self-lift always. So:

| | Live creature | Record / saved print |
|---|---|---|
| Eye self-lift | **≥ 8% additive luma** + L8 specular dot | **0% — flat.** No specular dot |
| Cast shadow | yes (§3.5 rule 1) | none |
| Lower rim (Mint Halo) | present | removed |
| On-screen size | 1.0× (menu) to 1.4× (flourish) | 0.62 × its thumbnail cell |
| Container | none | record card + 4 px Dawn Cream keyline |

In greyscale the live creature's eyes are **the two brightest pixels in the frame**;
a record print has no such pixels. That is a one-glance, 64 px, colour-free
discrimination, and it costs nothing because §3.5 rule 4 already mandates the
self-lift. **The eye is doing three jobs: aliveness, expression, and
live-vs-printed.** A record print is a *drawing of your pet*, not a second pet, and
it should never look alive — which is also why it carries no shadow and no rim.

#### (d) Reward object vs. world prop

Reward objects hold hero tier for **1.5 s** (§3.5 rule 2) and then become scenery.
So the hero window is the **acquisition**, not the discriminator. The permanent
discriminators are:

| Channel | Prop | Reward (after demotion) |
|---|---|---|
| **Scale** | 1.00 × its prop family's base | **1.08 ×** — never returns to 1.00 × |
| **Value** | Honey Field, **L6 79.5%** | one rung up, **L5 Sand 66.0%** — **13.5% ΔL, colour-independent** |
| **Position** | anywhere | always adjacent to its record card, at all times |
| **Geometry family** | pebble / arch / tuft (§3.3) | **a fourth family, `keystone`, which the world never spawns as scenery** |

At 64 px, **1.08× is 5.1 px** and 13.5% ΔL sits above §4.3's 5.0–15.9% "colour-weak
— must carry a redundant channel" band, meaning the size delta is the redundant
channel and it is doing the work. Combined with a unique silhouette family, this is
separable in greyscale with no colour at all. Cost: one prop family, one extra draw
in the instanced batch.

---

### 5.4 Expression and Pose Direction

#### The style call, stated once

> **Exaggerate amplitude, keep timing honest.**

| Axis | Target | Justification |
|---|---|---|
| **Shape amplitude** | **Exaggerated.** Eye scale 0.78–1.35, squash 0.82, stretch 1.12, rise 0.25 H | §1.3 forces it: "bigger reaction, then settle slow. Neutral is a failure state." |
| **Timing** | **Naturalistic.** Asymmetric anticipation, ease-out settle, no bounce-timing gimmickry, no rubber-hold | A music-hall bow is loud *because* the timing is honest. Looping bouncy timing is fatiguing on a 4 s idle loop that runs all session |
| **Vocabulary size** | **Three mouth forms, two eye states, two ear states.** Extremely small | A tiny vocabulary is what survives 64 px. Nine eye shapes is zero eye shapes |
| **Realism** | **Refused entirely** | Nothing in this product may read as an animal having a mood |

The whole system rests on one structural decision, and it is the answer to the
hardest constraint in this section.

#### Pillar 2 — the Warm Settle Law

> **A negative expression is not *banned* in this game. It is
> *unauthorable*.**

The creature's expression vocabulary is **closed and small enough that distress has
no representation.** This is a mechanism, not a prohibition: a reviewer cannot
find a rule to break, because the asset that would express the feeling does not
exist.

| Organ | Legal forms | Count | Consequence |
|---|---|---|---|
| **Mouth** | **Level** (rest) · **Arc-up** (convex, apex +0.035 H) · **O** (small circle, wonder) | **3** | **A downturned arc is not an asset.** There is no mesh, no blend shape, no parameter. The mouth is *structurally incapable* of expressing unhappiness, and no animator can reach one |
| **Eye** | **Open** (1.00) · **Soft-narrow** (0.78) · **Flourish** (1.35, legal only in the 2.5 s flourish) | **2 + 1 gated** | Eye-**widen** past 1.35 is impossible, and it is permitted only at the moment of maximum success. A widened eye elsewhere would read as alarm; here it reads as delight |
| **Ear** | **Forward** (+0 to +38°) · **Relaxed-back** (−0 to −18°) | **2** | **Pinned-flat is capped at 22° from vertical and is legal only as a petting response**, never as an expression. Ears cannot be pulled back hard enough to read as fear |
| **Body** | Rise 0–0.25 H · lean 0–7° · squash 0.82–1.00 · stretch 1.00–1.12 | bounded | No cower-arc, no arch-back, no recoil translate, no shrink below 0.90 |

**And the substitute, named.** When a system would signal distress, disappointment,
fear, confusion, or failure, it emits **Warm Settle** instead — and the
substitutions are specific, not vague:

| Would-be negative | **What replaces it** | Motion & shape substitute | Why it works |
|---|---|---|---|
| **Distress** (discomfort, too much petting) | **Lean-in** | 4° lean **toward** the stimulus, 1.04 volume, 320 ms settle, 2 bounces | Turning away reads as rejection. Turning *toward* reads as trust. The direction is the message |
| **Disappointment** (nothing happened, no reward) | **Return to Ambient** | Nothing at all. The creature simply resumes the 4.0 s breathe | Ambient is the safest state in the game (§2.3). **The absence of a reaction is the only "nothing" the child ever sees** — and it is indistinguishable from a pet just being a pet |
| **Fear / alarm** (loud input, sudden touch) | **Interest** | Head-tilt 9° toward the sound + ear +26° + eye tracking | Curiosity and alarm share a head-tilt; the version authored here has the ears forward. **Head-tilt is authored as interest and can only be reached via interest** |
| **Confusion** (input not understood) | **Curiosity** | 9° side-tilt, one ear +30° / one ear +8° held for 0.9 s | Asymmetry is the *only* asymmetric ear legal expression, and it always resolves forward. There is no head-shake and no "?" — and no text, ever (§4c.6) |
| **Failure** (minigame miss) | **Anticipation** (clip 10) | 0.3 s squash to 0.90, eyes 1.10, the exact "about to" tell used before every ability | The miss plays the *same* clip as the try. Nothing in the frame changes, because the next prompt arrives at higher brightness (§2.6). **Visual trend across attempts is always upward** |

**Two supporting clauses, for completeness:**

- **Downward-substitution law.** Any downward *intent* is expressed as a lateral or
  upward motion. The creature never moves down-and-away. It moves down-and-toward.
- **Amplitude floor.** No readable expression may land at less than **70% of its
  target amplitude**. A mouth arc at 30% is not "subtle" — low-amplitude mouth
  curvature is precisely the animal signal for a negative, so a small expression is
  a *worse* expression. Either it commits or it is not authored.

**Authoring-time greyscale pre-check:** the six expression targets are rendered at
64 px, greyscale, before any animation is cut. If two collapse into one, the fix is
**more amplitude** (mouth arc, ear degrees) — never colour, never a new form.

#### Expression targets

`Eye` is scale relative to rest (0.17 H). `Ear` is degrees from rest. `Mouth` is the
§5.1 form. Every row is greyscale-checkable at 64 px.

| Target | Mouth | Eye | Ear | Body | Audio | Duration |
|---|---|---|---|---|---|---|
| **Rest / neutral** | Level, 1.00× width | 1.00, both open, centred | 0° | 4-contact, 0° pitch, spine straight, ground clearance 0.04 H | idle purr, fundamental, no gap | 4.0 s breathe + 6.0 s sway, unsynced |
| **Happy recognition** | **Arc-up**, apex **+0.035 H**, width 1.5× | **1.22**, tracking camera | **+26°** forward | rise **0.06 H**, chest lift, stretch **1.09** | purr **+2 semitones**, one chime above it | 0.9 s in, hold, 1.2 s decay |
| **Focused / petting** | Level, narrowed to **0.70×** — *narrow, never down* | **0.86** half-lidded, **0 ms** tracking lag | **−18°** back, relaxed | lean **7°** into the finger, squash to **0.94** proportional to stroke speed, capped 1.0 strokes/s | purr **−1 octave**, continuous, no gaps | 1.6 s loop, 1:1 with input |
| **Absorbed / painting** | Level, **0.60×** width | 1.00, gaze **locked** to mark site, **saccades disabled** | **+12°**, both ears **equal** — asymmetry is illegal here | breathe slows to **5.2 s**, **sway stops**, micro-nod **0.01 H** at 2.0 s, lean 3° to the work | one sustained tone, **−3 semitones**, **no rhythm** | 2.0 s loop |
| **Proud / flourish** | **O** at 1.4× width, closes to Arc-up | **1.35** (cap), self-lift **+14%** for 2.5 s | **+38°** (cap), fully forward | rise **0.25 H** — the only ground departure outside a minigame hop — stretch **1.12**, 2-bounce settle | open fifth, **+5 semitones**, a **fixed 12-note figure, identical every time** | 2.5 s, then 3.0 s decay. No second peak without a new mark |
| **Warm Settle** (universal) | Level | **0.78** soft | **−8°** | lean **4° toward**, 1.04 volume, 320 ms settle, bounces 0.35 / 0.12 | one soft chime, **no pitch fall** | 0.6 s |

The Flourish audio figure is **fixed, never re-composed, never re-tuned by
performance** — for the same reason the 12-spoke burst is fixed (§2.6, C7):
variation there would grade the child.

#### Pose targets

| Pose | Posture | Silhouette consequence | Legal in |
|---|---|---|---|
| **Grounded** | 4 contacts, 0.02 H compression | closed mass, contact shadow intact | every state |
| **Lean** | 4–7°, weight shifted to the front contacts | silhouette still closed; **no contact lifts** | petting, painting, Warm Settle |
| **Rise** | both rear contacts lift ≤ 0.04 H | still a closed mass — this is a pose, not an appendage | recognition, flourish |
| **Reach** | one arm nub extends ≤ **1.6×** its rest length | **nub never exceeds 0.12 H** and never pierces the outline | painting, flourish |
| **Settle** | 1.04 volume, 320 ms, 2 bounces | returns to grounded | any transition |

**Declined:** no sitting, no lying, no sleeping, no side poses. A reclining hatchling
halves the paintable frontal area and §2.6 requires "maximising visible painted
surface area." Pose variety is bought from TILT, not from new poses.

#### Overshoot budget

| Actor | Overshoot | Settle | Bounces | Ceiling |
|---|---|---|---|---|
| **Creature — scale** | 0.82 → **1.12** on Y or XZ, **never both beyond 1.10** | 320 ms | 2 (0.35, 0.12) | 3 amplitude events / 2 s |
| **Creature — travel** | ≤ **0.08 H** past target, **Y axis only** | 320 ms | — | 1 grounded bounce / 2 s |
| **Creature — rotation** | ≤ **6°** | 320 ms | — | 1 per 2 s |
| **Props, UI, Echo geometry** | **0.00** — critically damped | 400 ms | **0** | — |
| **Reward flash (1.5 s)** | 1.40× growth, 1.12 overshoot | 500 ms | 1 | once per object, ever |

**Forbidden in motion, with reasons:** **shake** (§1.4 P2 — shake is the universal
penalty signal); **lateral overshoot across the ground plane** (reads as being
thrown, i.e. a loss); **rotation beyond 12°** (reads as dizziness, and dizziness is
confusion); **any negative-Y traverse that does not end adjacent to its origin**
(the creature never leaves, so a departure it does not return from is a lie).

---

### 5.5 The Body as Canvas

**This is the mechanic.** Everything below is load-bearing for the game existing.

#### The shell is authored to receive colour without muddying

| Rule | Value | Why |
|---|---|---|
| **Shell albedo flatness** | **± 1.0% luma across the entire body** | A mottled, ambient-occluded, or fur-shaded shell makes every mark's edge ambiguous. **A canvas must be blank.** This is the strictest art rule in the section, and it is the mechanical reason the surface is a matte painted shell and never fur |
| **Shell gloss** | **0.02** specular | 17× duller than wet paint (0.35). If the shell has any specular, wetness is invisible |
| **Shell roughness** | 0.85, no anisotropic highlight | Same reason |
| **Shell value** | **Dawn Cream, L7 91.9%** — the highest rung in the product | §4.1: the world's brightest surface, so it reads *quiet* and a saturated mark on it reads *new* |
| **Shell saturation** | **15.0% HSV S** against a **25.0%** cap | The headroom is not spent on shell prettiness (§4.4.4 rule 2) |
| **Paint composite** | **Over-composite at 100% pigment load. Multiply is FORBIDDEN.** | Multiplication by a cream base is *exactly how paint muds*. An 80.7%-S Cherry Pop multiplied by a 15.0%-S shell does not arrive at 80.7% S |
| **Mark over print** | The mark **occludes** the print mask. The mask is never composited under paint | The child's authorship outranks the variant's flavour (§1.4 P1). Print is Tier 2, 48.2% S; the child's mark wins locally, always |
| **Under the shell** | **Nothing.** No dark interior, no AO below L3 (41.6%), no subsurface tint | A shadowed crevice under a mark edge makes the mark's own relief ambiguous |
| **No fibre, no fur shells, no strand cards, no hair cards** | **0** | The child's paint sits on this surface. A fibrous surface is not a paintable surface. This is a hard exclusion, not a preference |

#### Mask slot layout — six fixed slots

Slots are **UV regions on the base mesh's single 0–1 layout**, each a wrapped 3D
shell region so a stroke wraps the body rather than sticking to a face.

| Slot | Region | Anchor | Size at §2.6 minigame framing | Restriction |
|---|---|---|---|---|
| **1** | **Crown** | top 0.10 H of the skull | 0.26 H wide | **Always available. Never restricted, never deprioritised** |
| **2** | **Cheek-L** | front-left of the head | 0.18 H | — |
| **3** | **Cheek-R** | front-right of the head | 0.18 H | — |
| **4** | **Flank-L** | mid-body, left | 0.30 × 0.24 H | largest body slot |
| **5** | **Flank-R** | mid-body, right | 0.30 × 0.24 H | largest body slot |
| **6** | **Front plate** | centre of the chest and belly | 0.34 × 0.30 H | **The only slot visible in Menus and Results** — the one slot guaranteed on-screen in every state |

No slot is smaller than 0.18 H, and the smallest is **11.5 px at 64 px** —
comfortably above the 5.1 px P5 floor. The layout is deliberately
**anterior-weighted** because §2.6 and §2.4 both frame the front or three-quarter
front: four of six slots are on the camera-facing half, and slot 6 is the fallback
precisely because it is never occluded.

**Hook interaction with slots.** A crest occupies crown silhouette, not crown
*surface*; the Crown slot's UV region is authored to wrap the front-top of the skull
and is unaffected by which hook is fitted. **A hook may not occlude a slot.** The
Floppy-Ear hook's overlap is measured at the head's side; Cheek-L and Cheek-R are
authored to clear the fitted hook's silhouette at maximum ear spread, verified once
per hook type at build time.

#### Placement and the six-slot exhaustion policy

| Condition | Action |
|---|---|
| Any slot < **35%** covered | Stroke lands in the least-occupied valid slot. Ties break 1 → 6 |
| Any slot **35–55%** covered | Same. Stroke width **tapers 1.00 → 0.75×** over 0.5 H of travel — the stroke thins as the slot fills, which is legible, is not a warning, and needs no text |
| All six > **55%** | **Compaction.** The oldest mark in the most-occupied slot contracts toward its own centroid over **250 ms**, freeing space. New stroke lands immediately |
| Compaction would push any mark below **0.08 H** | **Refused.** The stroke goes to the next slot. A mark is never shrunk out of existence |
| All six refuse compaction | **Overlay.** The stroke composites onto the least-occupied slot as an additive second layer. It may overlap. It is never discarded, never re-tinted, never moved off the body, never rejected |

> **The invariant, in one sentence: visible pattern density is the pressure valve,
> and the child never loses a mark.** Every step above is a way of making room.
> There is no path through this policy that ends in a lost stroke — because "your
> mark didn't fit" is a fail state, and Pillar 2 forbids fail states. **The only two
> terminal outcomes are compaction and overlay, and both are invisible to the child
> as a problem.**

Slot order is fixed and never randomised. A child who learns "the crown fills
first" learns one predictable thing.

#### Relief — §4.3 C4 carried forward as an *art* obligation

§4.3 C4 is explicit: relief "is a shader requirement, not an art request." This
section treats it as art anyway, because **three of the four channels C4 relies on
are authored decisions, not shader decisions.**

| # | Obligation | Value | Owner |
|---|---|---|---|
| **R1** | **Relief depth** | **0.02 – 0.04 H** above the shell. At 64 px that is **1.3 – 2.6 px** — the primary greyscale channel for a mark on any print | Art + shader |
| **R2** | **Directional streak texture, authored into the decal** | **≥ 3 streaks per 0.08 H**, aligned to the stroke vector, internal contrast **≥ 12% luma**, hue held within the stroke's own Tier-0 entry (± 8% HSV S) | **Art — authored, not generated** |
| **R3** | **Feathered edge is a value ramp, not a colour ramp** | The 0.06 H feather spans **≥ 2 ladder rungs** of value | Art + shader |
| **R4** | **Raised shoulder catches the rim and casts a contact line** | The mark's upper shoulder receives the Rim Rose rim; its lower edge carries a **0.010 H** Peach Shadow contact line | Shader |
| **R5** | **Relief must hold on the two dark Tier-0 paints** | **Cranberry (luma 28.3) and Cherry Pop (33.4)** — the relief silhouette and R2 streaks must both remain visible against **any** of the nine print classes | Shader — **§8 verification item** |

**The reasoning that makes R2 an art rule, not a shader note.** C4's adversarial
case is Splash Mint (65.9) against Sand M5 (66.0) — **Δluma 0.1%**, effectively zero.
Violet Pop (39.8) against M3 Umber (41.6) is a 1.8% gap, well inside
§4.3's 2.0–4.9% "colour-void" and 5.0–15.9% "colour-weak" bands. So on the dark
paints a mark has **no value advantage and possibly no value disadvantage** against
a print mask. The only surviving channels are **geometry** (R1), **internal
structure** (R2), and **pattern orientation** (R2 — a print is radial or gridded; a
stroke is directional). A mark with no streaks and no relief is, on those two
paints, *invisible as the child's work*. **The streak texture is the authorship
signature. It is authored art, and it is the most important pixel in the game.**

#### Wet/dry lifecycle, as art direction

Three stages, each with shape, value, surface, pattern, motion, and audio — §1.2
satisfied at the pixel level.

| Stage | Duration | Relief | Value | Surface | Streak | Audio |
|---|---|---|---|---|---|---|
| **Wet** | **0.8 s** hold | **0.04 H**, still swelling | HSL **L ×1.18** + additive **≥ 8%** | gloss **0.35** | maximum internal contrast, streaks spreading | stroke-creak, register rising |
| **Settle** | **0.6 s** | 0.04 → **0.02 H** | ×1.18 → ×1.00 | 0.35 → **0.05** | contrast fixed | one soft chime |
| **Dry** | permanent | **0.02 H** floor | ×1.00 | matte, gloss ≤ 0.05 | fixed | purr layer returns |

**In greyscale: wet is brighter and raised, dry is flat and matte.** Colour is the
third channel, not the first (§4.3 C3).

> **A dry mark re-wets when touched.** Petting a stroke you painted re-runs the
> 0.8 s wet lifecycle at 0.6× intensity — a loop the child can trigger at will,
> with no UI. This is why the lifecycle is authored as a cycle rather than a
> one-shot.

**Declined:** no dripping, no runs, no splatter, no splatter-as-wetness. A drip
reads as a smudge or a mess, and smudges are mistakes.

#### No text on the body — §4c.6

No glyphs, numerals, stamps, letterforms, or handwriting-like marks on the creature,
ever. Restated as art direction: **the nine print classes deliberately contain no
glyph class** (§3.2), and a tenth class shaped like handwriting is forbidden by
§4c.6 and by this section. There is no caption, name, score, or stamp printed on the
body at any size. The child's name lives on the **record card** (§4c role 5), and
only the child's own hand is permitted there.

---

### 5.6 Animation Direction

#### Clip budget — closed set of 20

| Group | Clips | Loops | Total |
|---|---|---|---|
| **Baseline (idle set)** | Breathe (4.0 s) · Sway (6.0 s) · Blink (0.30 s) · Gaze (**procedural, not a clip**) | 3 | 3 |
| **State** | Rest_Happy 0.9 s · Focus_Petting 1.6 s · Absorb_Painting 2.0 s · Proud_Flourish 2.5 s · Greet_Return 1.2 s · Settle_Any 0.32 s | 2 | 6 |
| **Event** | Anticipate_Ability 0.30 s · Minigame_Hop 0.45 s · Pet_Land 0.60 s | 0 | 3 |
| **Echo** | Echo_Base (0.8 s in / 2.0 s hold / 0.6 s out) + **6 family masks** | 0 | 7 |
| **Hook** | Hook_Spring (**6 frames**, shared by all three hooks) | 1 | 1 |
| | | | **20** |

**Retargeting rules — three, all binding:**

1. **No clip is ever authored per variant.** TILT changes vertex weights, not the
   rig, so all 20 clips retarget to all 65,856 TILT vectors for free. A per-variant
   clip set would multiply 20 by a six-figure variant count.
2. **One shared 6-frame hook spring** serves all three hook types: two delayed
   cycles, 60 ms per lobe offset. Hooks differ in mesh, never in animation.
3. **Squash, stretch, and overshoot are procedural, in code, not baked.** Baking
   them multiplies clip count by the number of overshoot events in §5.4 and buys
   nothing. Code-side squash is bounded by the §5.4 overshoot budget, which is the
   real control.

No IK solvers. No constraints. No spring bones. No clip events. Target: **≤ 32 MB
animation data, one rig, one Animator layer set (Base + Echo additive).**

#### "Only the creature overshoots," in motion terms

| Actor | Easing | Overshoot | Anticipation |
|---|---|---|---|
| **Creature** | ease-out with 2 bounces (0.35, 0.12), 320 ms | **per §5.4 budget** | 0.25 amplitude, 120 ms, on every state entry |
| **Echo geometry** | ease-in-out, symmetric, 0 bounces | **0.00** | none |
| **Props** | critically damped, 400 ms | **0.00** | none |
| **UI** | linear / damped, 200 ms, 4% vertical squash on press | **0.00** | none |

**The split, in one line: the living world is bouncy, the made world is calm**
(§3.5 rule 5). Anticipation is creature-only, and **1 per 2 s maximum** — more and
the frame stops reading as an event.

#### Idle baselines

| Baseline | Value | Constraint |
|---|---|---|
| Breathe | **4.0 s** (0.25 Hz), spine scale Y 1.00 → 1.03 → 1.00, **ears and hook lag the body by 200 ms** | The 200 ms lag is the whole "alive" read. Without it, the creature is a breathing balloon |
| Sway | **6.0 s**, body lean ±3° | **Different period from breathe, phase-locked at 0** — they never visibly sync. Sway is also the one baseline that **stops** in Painting (§2.4: the world goes still so the child's touch is the only event) |
| Blink | close **0.12 s**, open **0.18 s**; random **4–9 s**; **both eyes, always** | One eye winking reads as a wink. A wink is a social signal this product does not make |
| Gaze | **procedural**, 120 ms saccade lerp, 0.15 max angular step per saccade | Saccades **disabled** in Absorb/Painting. A creature that glances away mid-stroke is a creature that lost interest in the child's work |
| Painting override | breathe **5.2 s**, sway **off**, no blink for 0.8 s after each stroke | Slower breathing = absorbed. This is the mechanism, and it is a rate, not a mood |
| Minigame override | breathe **2.0 s**, matched to gesture cadence, **never to a timer** | §2.6 is explicit |

#### §1.6 — every state change carries shape + motion + audio

The creature's contribution to all seven §2 states. A row with an empty cell is a
defect in this section, not in the lighting.

| §2 state | **Shape delta** | **Motion delta** | **Audio delta** |
|---|---|---|---|
| **First Open / Greeting** | Eye 1.22, ear +26°, mouth Arc-up | 0.06 H rise, stretch 1.09, 0.9 s in | Purr +2 semitones, one chime |
| **Idle Ambient** | Level / 1.00 / 0° — *the baseline* | 4.0 s breathe + 6.0 s sway | Continuous purr, fundamental, no gap |
| **Painting** | Eye 0.86 → 1.00, ear +12°, mouth 0.60× width | Breathe 5.2 s, **sway off**, gaze locked, no saccades | Sustained tone −3 semitones, **no rhythm** |
| **Signature Mark Flourish** | Eye **1.35** (cap), self-lift +14%, ear +38° (cap), mouth **O** | 0.25 H rise, stretch 1.12, 2-bounce settle, 2.5 s peak then 3.0 s decay | Open fifth, +5 semitones, **fixed 12-note figure, identical every time** |
| **Minigame** | Squash **0.82** → stretch **1.12** (fixed by §2.6) | 0.45 s hop, breathe 2.0 s at gesture cadence | Purr steady; success = one fixed chime + purr swell |
| **Results / Reward** | Rest form, 0.78 soft eye — *nothing new* | Settle 0.32 s, then full stillness | Purr decays to silence over 3.0 s. **Nothing new arrives** |
| **Menus** | Level, 1.00, 0° | Breathe and sway continue — **the creature is never paused in a menu** | Purr continues at −4 dB. A child who opens a menu never left the pet |

**Six audio registers for the six Echo families**, one per family, spanning a major
sixth: E1 Column highest, E2 Drape second, E3 Sweep third, E4 Orbit fourth,
E5 Halo fifth, E6 Buds lowest. A child who cannot see an Echo clearly can still
name it by ear, and two Echoes never share a register.

---

### 5.7 LOD & Budget Philosophy

> **The creature has no LOD. One mesh, one material, two draw calls, at every
> distance, forever.**

**Declined, with reasons.** A second LOD for the creature is not specified because:

1. **It would save nothing.** §1.4 Principle 3 forbids multiplying the creature, so
   exactly **one** creature is on screen in any scene. A 10k-tri mesh is **3.3% of
   the 300k visible-triangle budget.** LOD exists to amortise cost across many
   instances; there is one instance.
2. **It would split the canvas.** The six mask slots are UV regions on **this**
   mesh. A LOD swap with a different UV layout can shear a child's painted mark
   across two meshes — and the mark **is** the save data and the ability binding. A
   mark that renders differently at distance is a bug in the game's core loop, not
   a performance feature.
3. **It would break the 64 px contract.** §3.1 forbids an LOD swap below 0.5 H
   precisely because at 0.5 H (32 px) the hook footprint and print class are the
   only surviving evidence. A swapped mesh that dropped the crest lobes would fail
   §3.1 step 2.

#### The no-LOD-below-0.5 H rule, stated as it actually works

§3.1's rule is an **anti-pop** rule, and its real content is: **LOD0 must survive
down to 0.5 H and must never be swapped out under it.** 0.5 H is 32 px — the size
the OS notification icon and every menu thumbnail uses. Therefore the effective LOD
contract is:

> **LOD0 is the whole contract. It must be correct at 32 px.**

#### The LOD that actually happens: shading LOD, not geometry LOD

Detail is shed by **shading layers turning off**, never by geometry disappearing.
Every item below is already sub-pixel at the distance that sheds it, so the loss is
zero by construction.

| Dropped at | What | Why it is free to drop |
|---|---|---|
| **< 0.50 H** (32 px) | **Streak micro-contrast** (R2) above 12% → 6% | At 32 px a stroke is 2 px wide; the streaks are sub-pixel. R1's 1.3 px step carries the mark alone |
| **< 0.35 H** (22 px) | **Eye specular dot** (1.5 px) | Sub-pixel. The eye's self-lift survives and is the aliveness anchor |
| **< 0.35 H** | **Shell surface micro-variation** (all of it — the shell is flat by rule) | Zero loss. This is why the shell is authored flat |
| **< 0.25 H** (16 px) | **Echo streak and Echo micro-shadow** | Only ever seen in Flourish at close range anyway |

**Never dropped, at any distance, ever:** the HOOK mesh · the 12% appendage
silhouette · the print mask and its nine classes · the mark relief (R1) · the mark's
position · the eye's self-lift.

#### Triangle and draw budget

| Element | Tris | Draws | Notes |
|---|---|---|---|
| Base body | **10,000** (8–12k band) | **1** | One material, one texture set |
| HOOK mesh | **2,400** (≤ 3k cap) | **1** | One material; one of three types fitted per instance, single creature on screen |
| Print mask | **0** | 0 | One texture slot, one material — **zero shader variants** (§4.5) |
| Paint (6 mask slots) | **0** | 0 | Six channels in the base material — **zero shader variants** |
| **Cast shadow** | **0** | **0** | **Projected soft blob, composited in the lighting pass, tinted Peach Shadow at 0.28 (§2.1).** |
| **Total on screen** | **12,400** | **2** | **4.1% of the 300k tri budget, 1.3% of the 150 draw-call budget** |

> **The shadow is the single biggest budget decision in this section.** A shadow
> mesh on a rim-lit, always-rim-lit creature would cost triangles to produce an
> object that carries **no information at all** — §4.3 C9 explicitly declares it
> non-informational (ΔL 2.6%), and §1.2 permits no semantic to attach to it. A
> reviewer who upgrades the shadow to geometry has spent budget on a thing the
> document says means nothing. **§3.5 rule 1 is preserved — the creature still
> casts the only shadow in the game. It just costs zero triangles.**

#### Hand-off to the technical artist — §8 must pick up these eight

| # | Item | Obligation |
|---|---|---|
| **1** | **Relief must hold on the dark paints** | 0.02–0.04 H relief plus directional streaks must read against **any** print class for **Cranberry (28.3)** and **Cherry Pop (33.4)**. This is C4's mandatory carry and it is the section's highest-priority shader item |
| **2** | Shell gloss 0.02 vs wet-paint gloss 0.35 | 17× ratio. **Raise shell specular and the wet/dry lifecycle dies** |
| **3** | Over-composite, never multiply | Paint must arrive at full Tier-0 saturation. Multiplication by a cream base mutes it |
| **4** | Mark occludes print, locally | Not composited, not multiplied |
| **5** | Six mask slots, one material, **zero shader variants** | Against the 200-variant ceiling |
| **6** | Rim must run over the mark's raised shoulder + 0.010 H contact line (R4) | This is what makes relief read on a dark paint — a highlight is not enough; it needs a contact |
| **7** | Eye self-lift is the live-vs-record discriminator | ≥ 8% additive live, exactly 0% on any record print |
| **8** | Zero-triangle cast shadow; verify Peak < 300k on mid-range Android | Soft projected blob, Peach Shadow 0.28 opacity |

**A ninth item, carried to §8 Asset Standards (Materials), not §8 Hand-off:** the creature material is a
**matte painted shell with a soft warm rim along the top and back edge. It is
**never fur**, and the material may not implement fur, fibre, strand, hair-card,
or shell-layer effects of any kind. This is a material-level exclusion, not a
modelling note, because a shell-layer fur implementation would visually destroy
the flat canvas the paint system depends on.

---

### 5.8 Reviewer Checklist

**Greyscale first — every item is checked with colour fully removed, at 64 px,
front-on, no motion:**

**Silhouette & variant**
- [ ] Hook footprint **≥ 0.122 H²**, body overlap **≥ 0.049 H²**, new outline **≤ 0.073 H²** — measured on the **final TILT-applied** silhouette
- [ ] Hook is still identifiable with the print masked out, and the print still identifiable with the hook masked out
- [ ] No variant is identifiable by print or colour alone
- [ ] Base mesh carries **zero appendages** — every hook is the only attached form
- [ ] No detached fin, floating tail, wing, or stalked horn exists in the vocabulary
- [ ] All three hooks are separable in greyscale at 32 px by **vertical band** (above / at / below the head-line)
- [ ] All nine print classes remain separable in greyscale at 64 px
- [ ] Appendage tips **≥ 40%** of their own base width; minimum corner radius **35%**

**Surface**
- [ ] The creature reads as a **matte painted shell** with a soft warm rim on the top and back edge
- [ ] **No fur, no fibre, no hair cards, no strand cards, no shell layers** in the mesh, the rig, or the material
- [ ] **No fur-based surface shading, no fur-driven AO, no fibre-scale variation** anywhere on the body

**Expression — the Warm Settle Law**
- [ ] The mouth vocabulary contains **exactly three forms: Level, Arc-up, O.** A downturned arc does not exist as an asset
- [ ] Eye scale never leaves **0.78 – 1.35**, and 1.35 appears only inside the 2.5 s flourish
- [ ] Ear rotation never leaves **−18° to +38°**; no pinned-flat expression exists
- [ ] No clip contains a shake, a lateral overshoot, rotation beyond 12°, or a negative-Y departure
- [ ] Every expression lands at **≥ 70%** of its target amplitude, or is not authored
- [ ] Every §2 state has a filled shape + motion + audio row (§5.6)
- [ ] All six Echo families are separable in greyscale by growth axis, and none is radially symmetric
- [ ] No expression is readable **only** as a colour change

**The canvas**
- [ ] Shell albedo is flat to **± 1.0% luma** across the whole body
- [ ] Shell gloss **0.02**; wet-paint gloss **0.35**
- [ ] Every mark carries **0.02–0.04 H relief**, a **0.06 H** feathered edge ramping **≥ 2 rungs**, and **≥ 3 directional streaks per 0.08 H** at **≥ 12%** internal luma contrast
- [ ] **Violet Pop and Cranberry hold relief and streaks against all nine print classes** — C4 verified
- [ ] No slot is smaller than **0.18 H**; the Front Plate is visible in Menus and Results
- [ ] No fitted hook occludes any mask slot, verified once per hook type at build time
- [ ] Slot exhaustion resolves by compaction or overlay — **never by discarding a mark**
- [ ] No mark is ever compacted below **0.08 H**
- [ ] Paint composites as **over, never multiply**; marks occlude prints, locally
- [ ] Wet/dry reads in greyscale: wet is brighter and raised, dry is flat and matte
- [ ] **No text, glyph, numeral, stamp, or letterform anywhere on the body**

**Budget**
- [ ] Creature on screen: **≤ 12,400 tris, 2 draw calls**, at every distance
- [ ] **No LOD mesh exists**, and no LOD swap occurs below **0.5 H**
- [ ] Cast shadow is **zero triangles** and carries no semantic (C9)
- [ ] Shell HSV S **≤ 25.0%**; top-five most saturated pixels in any frame are **all** Tier-0
- [ ] Creature is the **only** cast shadow, the **only** rim-lit thing, the **only** overshoot, and the **highest point** in frame
- [ ] Eyes are the **brightest non-painted element** in every frame
- [ ] Total clips = **20**; no clip is authored per variant
- [ ] No surface anywhere in a creature material falls below **L3 (41.6%)**

---

## Section 6 — Environment Design Language

This section governs the world the creature lives in. It has exactly three jobs, and
they were fixed before anything else was decided:

1. **Give the creature a stage** — one continuous warm field it can be read against
   from any camera angle the seven §2 states allow.
2. **Give the rim-lit silhouette a value field to sit on** — one uninterrupted,
   low-contrast mass, never a busy one.
3. **Reassure through scale** — the creature is the tallest thing in frame, always.

**Governing statement (inherited from §3.3, restated because it is the whole
section):** *"the child's authorship more visible."* The environment competes with
nothing. It is the paper the creature stands on, not a place the creature visits.

**What this section is not.** Visual language only. It specifies no locations, no
zones, no level progression, no unlocks, no reward economics, and no gameplay logic.
Where the world meets those questions, this section states the visual envelope and
hands the decision on. Every refusal is recorded as a **Declined** block inside the
section that owns it, and consolidated in the register at **6.8**.

**The load-bearing canon, restated so no reader has to go looking:**

| Canon | Value | Source |
|---|---|---|
| Geometry classes | **4 total** — ground bowl, sky dome, sun disc, rounded props. Prop families: **pebble, arch, tuft** (scenery) + **`keystone`** (reward-only) | §3.3, §5.3(d) |
| Barriers / hazards | **Zero.** No doors, gates, fences, walls, mazes, pits, ladders, stairs, cliffs, edges | §3.3 prohibition 1 |
| Height ceiling | **0.6 H** on every prop in every state. Creature = 1.00 H, and is the highest point in frame | §3.3 prohibition 6, §3.5 rule 3 |
| Creatures on screen | **Exactly one, ever** | §1.4 Principle 3 |
| Cast shadows | **One in the game** — the creature's. **0 triangles.** Everything else grounds with contact occlusion at 0.15 | §3.5 rule 1, §5.7 |
| Light direction | **Behind and above, always.** Rim-dominant in six of seven states | §2.1, `AGENTS.md` §7 |
| Surface material | **Matte painted.** Never fur, fibre, hair cards, strand cards, or shell layers — **world surfaces included** | `AGENTS.md` §7, §5.7 item 9 |
| Environment saturation | **≤ 40.0% HSV S.** The world loses saturation before the child's paint does | §1.4 Principle 1, §4.4.2 Tier-3 |
| Value ladder | Nine rungs **L0–L8**. One "value step" = one rung, or ×0.85 on HSL `L` | §4.5.1, §4.5.2 |

**Scale unit.** Every dimension in this section is a fraction of **H**, the
creature's total silhouette height at canonical proportions (1.00 H tall, 0.62 H
minimum silhouette width, per §5.1). `H` is the only unit used. No metres, no
arbitrary world scale — a prop dimension that cannot be written as a fraction of `H`
is not specified.

---

### 6.1 The world premise

#### 6.1.1 What kind of place this is

> **A shallow warm dish at sunrise. One continuous field of ground, one gradient of
> sky, one sun, and a scatter of small soft objects that were clearly put there by
> somebody who was not in a hurry. Nothing is out of reach, because there is no
> "reach" — there is only here.**

That is the entire premise. It is deliberately smaller than it sounds, and the
smallness is the design.

#### 6.1.2 The five decisions that make it that

| # | Decision | The rule | Why — and which pillar it serves |
|---|---|---|---|
| **1** | **A dish, not a room** | No walls, no ceiling, no corners, no verticals. One open shallow bowl under open sky | A room is a container, and a container has an inside that implies an outside — which implies somewhere else to be, which implies progression. `game-concept.md` §2 already refuses a room-decor meta for v1 because it dilutes the Adorn verb. A dish has no inside, so there is nowhere else, and the child can never want to go there. **Pillars 1 and 2** |
| **2** | **One place, held forever** | Exactly one location in the product, and it is the same location in all seven §2 states. States change the **light**, never the **place** | §2.8 already commits to this for Menus — *"the same room, held still."* Generalised: a state that changed location would be a new scene, and §2.3's rule — *"if a proposed state cannot be described as a delta from Ambient, it is not a state"* — kills it. A child who paints a pink stripe and comes back to a green room learns the world is a slideshow. **Pillar 2** |
| **3** | **The sun never moves** | Sun disc fixed at the key's azimuth 130° / elevation 50°, present, constant, in every state. No arc, no day cycle, no time-of-day variation, no seasonal tint | **A moving sun is a clock.** A clock is time pressure, and `game-concept.md` §2 lists timers, streaks, and any elapsed-time mechanic as a Pillar 2 violation. A sun frozen at 50° elevation reads as *morning, held* — it is permanently the hour the child arrived, and it never suggests an hour has passed. §2.2 delivers the product's only legitimate time signal — greeting intensity scaling with **absence length** — and that is a light value, not a sky state. **Pillar 2, the load-bearing decision of this section** |
| **4** | **Handmade through placement, never through surface** | The world's "someone made this" quality is carried entirely by **prop placement rhythm** (6.2.6) and the **clay-and-wax finish** (6.4.5). It is carried by **nothing else**. No albedo variation on the ground, no mottling, no weathering, no hand-painted texture, no noise, no tiling detail of any kind | Two reasons, both load-bearing. First, the ground is the value field the rim-lit silhouette reads against; any albedo variation on it becomes false information in the silhouette's contrast, and §3.5 rule 1 depends on the creature being unambiguously the highest-contrast thing in frame. Second, the creature's own shell is held at **±1.0% luma across the entire body** (§5.1) so every painted mark edge is unambiguous. A world with surface noise beside a body held flat reads as *dirty*. The handmade quality therefore lives in **where things are**, not **what things are made of** |
| **5** | **The world asks for nothing** | The environment never animates toward a goal, never draws the eye, never pulses, never indicates, never rewards. It is weather, not scenery to explore | §3.5's tier table puts the ground bowl, sky dome, and sun disc in the **Receding** tier with the standing denial of *"Motion, shadow, saturation, detail, silhouette interest."* The only world motion permitted at all is 2% prop idle sway, and it is not directional. **Pillar 1 — the frame's only event is the child's authorship** |

#### 6.1.3 What the world is *for*, as the child experiences it

| The child should feel… | Delivered by | Not delivered by |
|---|---|---|
| *"This is a place, and it is nice to be in."* | One continuous warm field, wide and shallow, no seams, no edges | Any landmark, any boundary, any "you have arrived here" moment |
| *"Nothing is out of reach, and nothing is in the way."* | 0.6 H height ceiling, zero barriers, one unbroken curve | A wide-open field that implies the child *could* go somewhere |
| *"Someone was here before me, and they were gentle."* | 2% prop sway, 6.2.6 placement rhythm, clay-and-wax finish | Any decal, any graffiti, any authored "clever" prop |
| *"The room gets out of the way when I'm making something."* | §2.4 world stillness — sway off, gradient static, value −1 rung, saturation ×0.75 | Dimming toward grey, blurring, or hiding the world |
| *"I know where I am without reading anything."* | The **visible horizon band**, the one stable reference line, Menus only (§2.8) | A minimap, a location name, any text |

#### 6.1.4 Four things this world will never be

Stated up front, because each has been proposed in conversation and each is refused
with a reason rather than a preference.

| Refused | Why |
|---|---|
| **A habitat the child arranges** | `game-concept.md` §2 anti-pillar: room decor dilutes the Adorn verb and is a separate product. A world the child fills is a world with an inventory, and an inventory is a collection with a completion state — a pressure mechanic wearing a costume |
| **A landscape with landmarks** | A landmark is a destination. A destination implies somewhere the child is not, and *not being somewhere* is the exact feeling Pillar 2 forbids. The one orientation cue in the world is the sun disc, and it exists because §2.1 requires light to read as a *place*, not because the child must navigate |
| **A season, weather, or time-of-day system** | Any cycle is a clock (6.1.2 decision 3). A weather system additionally risks low-visibility states, and a low-visibility state reads to a child as *something is wrong with the picture* |
| **Anything the child can enter, stand on top of, or be blocked by** | 6.2.4 and §3.3 prohibition 1. A raised surface is a step, a step is a level, a level is an order. This world is one elevation. Permanently |

---

### 6.2 The four geometry classes

#### 6.2.1 The set is closed

> **Four classes. Three scenery prop families plus one reward-only family. A fifth
> class is an ADR, not a suggestion.**

The same closed-set logic that governs the six Echo families (§5.3(b)) and the three
hooks (§5.2) governs the world. §1.4 Principle 3 and §1.3's forced call — *"New asset
class or variant of an existing one? Variant. If it needs new geometry, the answer is
no"* — both apply to environments. **Pillar 3 at the environment level:** the world
earns its richness from three 320–560 triangle meshes and one lighting rig, not from
asset count.

**Why three families, and not two or four.** §3.3's claim is that pebble, arch, and
tuft cover the whole prop vocabulary — pillars, grass tufts, and stepping stones are
the same three objects at different scales. The load-bearing reason is **silhouette
count**: three distinct silhouettes is the most a child can hold as "the world's
vocabulary" without it becoming a set to recognise. One fewer reads as empty; one more
reads as a collection. The reason it is not two is that two silhouettes cannot produce
a foreground/midground read without a height difference, and height difference at the
0.6 H ceiling starts competing with the creature's silhouette. **The world has exactly
three prop silhouettes and the child is never asked to learn one of them by name.**

#### 6.2.2 The world surfaces, and then the prop families

**World surfaces — three, and they are the whole stage:**

| | **`ground bowl`** | **`sky dome`** | **`sun disc`** |
|---|---|---|---|
| **Role** | The value field the creature reads against | The warm low-contrast field above the horizon | Makes light direction a *place* (§2.1) |
| **Form** | One wide, shallow, continuous curved mass, lower third of frame | Vertical-gradient hemisphere, open bottom | Flat emissive disc |
| **Dimensions** | Radius **6 H**; max elevation change **8% of radius** = **0.48 H** across the whole span | Radius **9 H** | 48-segment fan, at azimuth **130°**, elevation **50°** |
| **Triangles** | **4,680** | **1,280** | **96** |
| **Shading** | Lit, wrapped terminator, hemispheric fill, ground bounce | **Unlit.** It *is* the ambient source, evaluated analytically as a two-colour hemisphere term | **Emissive, unlit** |
| **Ladder rung** | **L6** Honey Field `#E8C79B` near field → **L7** Dawn Cream `#F7E9D2` outer 30% | `#FFF6E2` zenith → `#FFCFA0` horizon (§2.1 tokens) | **Sunlight Amber `#FFE3B8`** |
| **Textures** | **Zero.** No albedo, normal, roughness, detail, splat, or decal | **Zero.** Gradient computed per-vertex from world-space Y | None |
| **Shadow** | Receives the creature's blob. **Casts nothing** | Neither casts nor receives | Casts nothing |
| **Motion** | **None** | **None** | **None, in every state, ever** |
| **May not** | Be taller than 0.48 H of relief, be stepped on, be subdivided, be tiled, or show any hard edge | Carry cloud, ray, flare, banding, star, or any second bright thing | Move, pulse, flare, rotate, or change state |

**Prop families — three scenery, plus the reward-only fourth:**

| | **`pebble`** | **`arch`** | **`tuft`** | **`keystone`** |
|---|---|---|---|---|
| **Role** | Grounding. The most-scattered family; present in every frame | Vertical accent. The only family that breaks the ground plane | Soft mass. The only family with a lobed top | **Reward-only. Never spawned as scenery** |
| **Height (H)** | **0.06 – 0.18** | **0.22 – 0.42** | **0.14 – 0.34** | **0.30 – 0.44** |
| **Width (H)** | 0.10 – 0.26 | span 0.24 – 0.44; thickness 0.05 – 0.09 | 0.12 – 0.30 across | 0.18 – 0.30 |
| **Lobes** | 1, smooth | 1, arched crown, **solid, no negative space** | **3 – 5, unequal** | 1, rounded crown |
| **Base triangles** | **320** (icosphere, 2 subdivisions) | **480** | **400** (5 lobes × 80) | **560** |
| **Max instances on screen** | **24** | **8** | **16** | **4** |
| **Max triangles on screen** | 7,680 | 3,840 | 6,400 | 2,240 |
| **Shadow** | **No.** Contact occlusion only, Peach Shadow 0.15 | No | No | No |
| **Rim term** | **0%** | **0%** | **0%** | **0%** |
| **Ladder rung** | **L6** Honey Field `#E8C79B` | **L6** Honey Field `#E8C79B` | **L6** Honey Field `#E8C79B` | **L5** Sand `#C0A584` — one rung up, per §5.3(d) |
| **Darkest permitted turn** | **L5** Sand `#C0A584` 66.0% | **L5** Sand 66.0% | **L5** Sand 66.0% | **L5** Sand 66.0% |
| **Gloss / metalness** | 0.02 / 0.0 | 0.02 / 0.0 | 0.02 / 0.0 | 0.02 / 0.0 |
| **Instancing strategy** | GPU-instanced batch, one draw for all 24 | GPU-instanced batch, one draw for all 8 | GPU-instanced batch, one draw for all 16 | GPU-instanced batch, one draw for all 4 |
| **Material** | *One shared matte-clay material for all four families* | ← | ← | ← |
| **May be used for** | Ground contact, depth cue, filling the lower third, making the field feel *placed* rather than *default* | Giving the horizon a rhythm so the field is not a bare plane; a scale reference against the creature's 0.62 H width | Soft organic breakup; carrying the 2% sway read that makes the world feel warm rather than static | A settled reward, permanently adjacent to its record card (§5.3(d)) |
| **May NOT be used for** | Anything the child can touch, collect, count, or move. No collectible, no currency, no resource — **there is no economy in this world** | A doorway, gate, archway, portal, or any traversable opening — see 6.2.4 | A grass blade, a hair, a fibre cluster, a fur tuft, a strand bundle — **never** — see 6.2.5 | **Scenery.** The world never places a keystone. It appears only as a reward, and only where a record card is (§5.3(d)) |
| **May NOT, any of them** | Exceed 0.6 H · cast a shadow · carry a rim term · overshoot · exceed 40.0% HSV S · frame the creature (6.2.7) | ← | ← | ← |

#### 6.2.3 The prop budget, derived

| Family | Base tris | Cap | Max tris on screen | Draws |
|---|---|---|---|---|
| `pebble` | 320 | 24 | 7,680 | 1 |
| `arch` | 480 | 8 | 3,840 | 1 |
| `tuft` | 400 | 16 | 6,400 | 1 |
| `keystone` | 560 | 4 | 2,240 | 1 |
| **Prop total** | — | — | **20,160** | **4** |

**Why low-poly is correct here and not a compromise.** §1.4 Principle 3 and
`AGENTS.md` §5 both say the same thing: *visual ambition is bought with lighting,
shading, and silhouette proportion, never geometry.* A pebble at 320 triangles and a
pebble at 3,200 triangles are the same object in this world, because a pebble is read
by its **silhouette and its lit crown**, and both of those are fully resolved at 320
triangles under a 0.45 wrap with a 0.02 gloss. There is no surface detail to spend
triangles on — §6.3.4 removes every texture in the environment. **The environment's
entire detail budget is spent on three silhouettes and one curve, and both are
exhausted by ~26k triangles.**

#### 6.2.4 The arch problem, and its resolution

The `arch` family is the one place where §3.3's own vocabulary creates a hazard, and
it must be resolved here rather than discovered by a modeller.

**The hazard:** an arched silhouette is the universal shape language for a *passage*.
The moment a child sees a curved form with space under it, the form reads as a door —
which is §3.3 prohibition 1, and which under Pillar 2 reads as *a place where you are
not allowed to go*. A world that contains a door is a world that contains a limit.

**The resolution, three binding rules:**

1. **No negative space.** The `arch` is a **solid, closed, ground-contacting mass**
   with an arched *crown*. No hole through it, no undercut, no void beneath it. The
   base is a continuous curve in contact with the ground across its full span. A form
   that cannot be seen through is not a door.
2. **It cannot contain the creature.** Maximum span **0.44 H**. The creature's minimum
   silhouette width is **0.62 H** (§5.1). An object narrower than the creature cannot
   be something the creature could pass through or stand inside, so it cannot read as
   a passage at any distance, at any camera angle, at any scale in frame. This
   arithmetic is why the span cap is **0.44 H and not 0.8 H**.
3. **It never frames the creature.** No arch may be positioned such that the creature
   sits within its silhouette from any camera angle in any of the seven states. A prop
   that frames the creature reads as a portal, a target reticle, or a picture frame —
   all three make the creature a *specimen* rather than a *friend*.

**Additional standing prohibition:** an arch may not stand in for a doorway, gate,
tunnel, window, or frame *even at other scales*, per §3.3's "pillars, grass tufts, and
stepping stones are the same three objects at different scales" — scale variation is
licensed, **function change is not**.

**Declined: a hollow arch with a walk-through opening, as a shortcut to "ancient
ruin" reading.** Refused on the arithmetic above and on Pillar 2. A walk-through
opening is a place the creature can enter, which means a place it can be inside and
not in — the first spatial concept in the product that implies a *wrong* place to be.

#### 6.2.5 The tuft problem, and its resolution

**The hazard:** the name is a trap. A "tuft" in every art pipeline ever written
defaults to a cluster of thin blades, and a cluster of thin blades is a **fibre
system** — the exact thing §5.7 item 9 and `AGENTS.md` §7 rule 1 forbid. The product
would then contain a hair system in the world, standing next to a body that is
explicitly not allowed to be made of hair.

**The resolution:** `tuft` is a **solid, soft-clay, lobed mass**, not a bundle of
strands. Its 3–5 lobes are **geometry**, 80 triangles each, merged into one watertight
shell with no gaps between them and no alpha, no cutout, and no alpha-tested billboard
anywhere in the prop set.

| Forbidden in the `tuft`, always | Enforced by |
|---|---|
| Grass blades, stalks, reeds, fronds | Lobe geometry only; no individual strand may be resolvable at 64 px |
| Hair cards, strand cards, fur shells, shell layers, alpha cutouts | **Zero alpha-tested surfaces in the entire prop set.** Props are opaque geometry, full stop |
| A cone, spike, blade, or any tip sharper than a 35% corner radius (§1.3) | Lobe caps are hemispherical; no lobe terminates in a point |
| Motion that reads as wind in fur | 2% amplitude sway only, critically damped, 0 overshoot (§5.6) |
| Any use of Rim Rose `#FFB0D0` or Mint Halo `#BFF3E2` | §4.1: Rim Rose is "the creature is alive … and nothing else in the world"; Mint Halo is a light term with "no surface use." Both are hard-limited and masked out of every environment material |

**The general rule this establishes, and it applies to all four families:** the world
is made of **the same material as the creature — a matte painted shell** — because
the world is part of the same handmade object as the pet. A child holding a clay
creature in a clay dish reads as one thing. A fur tuft in a clay field reads as two
things from two different products, and the one that is *not* allowed to be fur is
the one that will look wrong.

#### 6.2.6 Placement rhythm — how the world is made by hand

Handmade quality is carried by **where things are** (6.1.2 decision 4), so the
placement rules are a real art specification, not a level-design convenience.

| Rule | Value | Reason |
|---|---|---|
| **Cluster rule** | Props occur in clusters of **3 – 7**. Never an even scatter, never a pair. A cluster has **one dominant member** (largest in the family); the remainder sit at 0.4 – 0.8× its scale | An even scatter is a *spawned* pattern and children read spawned patterns as machine-made. Irregular clusters with a dominant member are the universal signature of hand placement |
| **Minimum separation** | No two props of the same family within **0.30 H** centre-to-centre | Prevents a cluster reading as one lumpy mass, and keeps the three silhouettes individually legible |
| **Sweep rule** | At least **one empty arc of ≥ 90°** within every 3 H radius of the creature's standing position | Guarantees the ground plane always has a clean region. A field with no empty arc reads as *full*, and a full field reads as a space with a limit |
| **Exclusion zone** | **No prop within 0.90 H** of the creature's centre | Protects the silhouette, the cast-shadow blob, and the painting hit volume. This is a **composition** rule, not a gameplay rule — nothing depends on it and it is not a safe area |
| **No prop between camera and creature** | Within the creature's silhouette footprint plus a **0.20 H** margin, in all seven states | A prop crossing the creature's silhouette breaks the rim read that §3.5 rule 3 and §2.1 law 3 depend on. The creature must never be described by an outline belonging to something else |
| **Sway** | **2% amplitude**, period 6.0 s, per-prop phase offset, no two props within 15° of phase | §3.5 grants props 2% idle sway. It is the only motion in the world, and the phase scatter is what stops 24 instances reading as one breathing object |
| **Population is not this section's decision** | Family caps are ceilings (24 / 8 / 16 / 4). Actual counts are not specified here | How many props exist is a framing-density question per state, and framing is not this section's to fix. The caps are ceilings; a section that needs a specific number asks for it |

#### 6.2.7 What may not be built, in total

The complete standing prohibition list for the environment. Every item is a reviewer
rejection, stated so it can be cited without re-argument.

| # | Prohibition | Source |
|---|---|---|
| 1 | A fifth geometry class or prop family | §1.4 Principle 3, §1.3; 6.2.1 |
| 2 | Any door, gate, fence, wall, maze, pit, ladder, stair, cliff, ledge, terrace, or boundary of any kind | §3.3 prohibition 1 |
| 3 | Any prop taller than **0.6 H**, in any state, at any camera angle, including a minigame layout | §3.3 prohibition 6, §3.5 rule 3 |
| 4 | Any prop that casts a real shadow | §3.5 rule 1, §3.3 prohibition 4 |
| 5 | Any reflective surface, cubemap probe, screen-space reflection, or gloss above **0.02** | §3.3 prohibition 5, 6.4.5 |
| 6 | Any fur, fibre, hair card, strand card, shell layer, alpha cutout, or grass-blade system anywhere in the world | `AGENTS.md` §7, §5.7 item 9, 6.2.5 |
| 7 | Any use of Rim Rose `#FFB0D0` or Mint Halo `#BFF3E2` on a world surface | §4.1 — both hard-limited light terms |
| 8 | Any world surface above **40.0% HSV S** | §1.4 Principle 1, §4.4.4 rule 1 |
| 9 | Any world surface below **L3 Umber 41.6% luma**, or any pure black | §4.5 Rule V1, `game-concept.md` §7 |
| 10 | Any hue outside the **17° – 42°** world band | 6.5.4 — makes "no cold blues in shadow" machine-checkable |
| 11 | Any desaturated neutral — anything at or below **5.0% HSV S** | 6.5.4; "no grey" is otherwise unenforceable as a number |
| 12 | Any tiled or repeating texture on the ground, the dome, or a prop — albedo, normal, roughness, or detail | 6.3.4; a visible repeat is a legible *pattern*, and a legible pattern in the world's value field is a hierarchy violation |
| 13 | Any hard horizon line, visible world edge, or seam between ground and sky | §3.3 prohibition 2, 6.3.3 |
| 14 | Any environmental animation with intent — pulsing, drawing the eye, indicating, highlighting, rewarding, or reacting to a game event | §3.5 Receding-tier denial of motion and silhouette interest |
| 15 | Any world motion that survives into Painting | §2.4, §5.6, 6.3.6 |
| 16 | Any environmental change between states other than **light, value, saturation, and the two named set-dressing terms** (Greeting motes, Menus horizon band) | §2.3, 6.1.2 decision 2 |
| 17 | Any second sun, second key, or second light source in the world. **The environment has no lights of its own** | §2.1 — one rig, states modulate it, states do not author lights |
| 18 | A collectible, resource, currency, or countable object. **Nothing in this world can be picked up** | `game-concept.md` §2 — no currency, no rarity, no collection pressure |

---

### 6.3 Ground and horizon

#### 6.3.1 The ground bowl

| Property | Value |
|---|---|
| **Form** | One wide, shallow, continuous curved mass in the lower third of frame |
| **Radius** | **6 H** |
| **Max elevation change** | **8% of radius** = **0.48 H** across the entire span |
| **Holes, cliffs, steps, terraces, subdivisions** | **None.** One uninterrupted surface, one elevation, no seams between regions |
| **Surface normal variation** | < **4°** across the full field — which is *why* 4,680 triangles is sufficient |
| **Rungs** | **L6** Honey Field `#E8C79B` (79.5% luma) in the near field, ramping to **L7** Dawn Cream `#F7E9D2` (91.9% luma) across the outer 30% of radius |
| **Gloss / metalness** | 0.02 / 0.0 — the same gloss as the creature's blank shell. The ground is the flattest, dullest surface in the product |
| **Textures** | **Zero.** No albedo map, no normal map, no roughness map, no detail map, no splat, no vertex-colour painting, no decal, no LOD dither (§6.3.4) |

**Why the near-to-far ramp goes *lighter*.** The creature's shell is L7 at 91.9%, the
lightest surface in the game. If the ground beneath the creature were also L7, the
creature's *lower half* would sit on a field of identical value and the silhouette
would dissolve exactly where the rim light is weakest. Putting **L6 beneath the
creature** buys the most important contrast in the composition: a **12.4% luma
separation between the creature's feet and the ground it stands on**, at 0.04 H
ground clearance, on a warm-to-warm hue pair. That separation is what makes the
creature *stand* rather than *float*, and it is a value decision, not an outline.

The ramp then lightens toward the horizon so the ground **recedes** as it goes away.
Value rising with distance is the correct cue for a surface curving away from the
viewer, and it is also what makes the far edge a soft transition rather than a band.

#### 6.3.2 The sky dome

| Property | Value |
|---|---|
| **Form** | Vertical-gradient hemisphere, radius **9 H**, 1,280 tris, open bottom |
| **Gradient** | **Zenith `#FFF6E2`** → **Horizon `#FFCFA0`**, per §2.1. Unchanged |
| **Shading** | **Unlit.** No lighting term, no shadow, no authored ambient response on the surface — the dome *is* the ambient source, evaluated analytically as a two-colour hemisphere term |
| **Texture** | **Zero.** Computed per-vertex from world-space Y. No gradient texture, no cubemap, no LUT |
| **Edge** | Never in frame. 9 H dome against a 6 H bowl, plus the §2 camera framings that keep the horizon low |
| **Peak HSV S** | `#FFCFA0` = **37.3%** — inside the 40.0% wall, with **2.7 points** of headroom |

**Declined: a sky with weather in it.** No clouds, no sun rays, no lens flare, no
god-rays, no aurora, no stars, no gradient-banding overlay. Each is a light effect
that would put a **second bright thing** in frame competing with the creature's rim,
and §2.1 law 2 caps the frame at **3.5:1** value ratio. A god-ray through a sunrise sky
is a 6:1 event. The sky's entire job is to be a warm, low-contrast, untextured field
that the rim-lit silhouette can be read against.

#### 6.3.3 The horizon: seam-free, and proven so

This is the requirement §3.3 prohibition 2 makes load-bearing — *"no hard horizon
line … a hard line reads as a wall, and a wall reads as a limit."* The treatment is
specified, and then **the seam is computed**, so a reviewer can verify it rather than
argue about it.

**The treatment, from near field to zenith:**

| Zone | Extent | Treatment |
|---|---|---|
| **Near field** | Inner 70% of bowl radius | L6 Honey Field `#E8C79B`, full key light, no fog |
| **Ramp** | 70 – 92% of radius | Rises one rung to L7 Dawn Cream `#F7E9D2`. Smooth, monotonic, no banding |
| **Dissolve** | 92 – 100% of radius | Linear fog toward the sky's horizon colour, reaching **85%** |
| **Join** | Bowl edge ↔ dome base | Fog at 85% against dome horizon `#FFCFA0`. **No geometry seam, no edge, no lip, no skirt** |
| **Glow strip** | Just above the join, dome only | Sunlight Amber `#FFE3B8` band, **0.06 H** tall, additive, intensity **0.18**, soft-edged top and bottom. This is what makes the field read as *sunrise* rather than as *a bowl under a gradient* |
| **Sky** | Above the glow strip | `#FFCFA0` → `#FFF6E2`, 1,280-tri gradient |

**The seam, computed.** Fog at 85% from a ground of L7 `#F7E9D2` toward `#FFCFA0`:

| Channel | 0.15 × `#F7E9D2` | 0.85 × `#FFCFA0` | Result | 8-bit |
|---|---|---|---|---|
| R | 37.1 | 216.8 | 253.8 | **254** |
| G | 35.0 | 176.0 | 211.0 | **211** |
| B | 31.5 | 136.0 | 167.5 | **168** |
| | | | | **`#FED3A8`** |

> **`#FED3A8` is not a palette entry and is not a new colour.** It is a *computed
> intermediate* — the arithmetic result of blending two existing tokens (L7 Dawn
> Cream and the §2.1 sky horizon) at 85/15, shown here only so the 1.03:1 seam figure
> is reproducible by hand. It is never authored into a material, a swatch, a token,
> or a spec. A reviewer running the §6.5.3 rule-14 check should skip it for that
> reason. The two colours it is made *of* are the only colours present at the join.

| | Value |
|---|---|
| Sky horizon `#FFCFA0` luma | **68.4%** |
| Fogged ground edge `#FED3A8` luma | **70.5%** |
| **Luma separation** | **2.1 points** |
| **WCAG contrast ratio across the join** | **1.03 : 1** |
| Threshold for "not a line" | ≤ 1.20 : 1 |
| **Verdict** | **PASS** — 1.03:1 is below the threshold and at the 8-bit quantisation floor. The join is not visible, and it is not visible *by calculation*, not by taste |

**No visible edge of the world, as three specific guarantees:**

1. **The bowl's silhouette is never resolvable.** At 1.03:1 the far edge is
   indistinguishable from the sky behind it. The child cannot point at where the world
   stops, because there is nowhere it stops.
2. **The dome's rim is never in frame.** 9 H against a 6 H bowl radius, plus §2's
   camera framings.
3. **The camera never pulls back far enough to see the bowl as an object.** Already
   fixed by §2: every state holds the creature in the lower two-thirds or centred, and
   Menus — the only state that lifts the camera — lifts to a **high three-quarter where
   the habitat reads as a diorama under glass**, which is a framing device, not a
   zoom-out. A view that showed the whole bowl as a bowl would turn a place into a
   model, and a model is an object on a shelf.

**The one exception, and it is a named one.** §2.8 defines a **visible horizon band**
— a soft warm curved light strip at the far edge of the ground bowl — as **present in
Menus and absent everywhere else**. It is the product's only stable reference line
saying *this is the same place, arranged.* It is specified and budgeted here as a
**shader term inside the ground material**, not a mesh:

| Property | Value |
|---|---|
| **Geometry** | 0 triangles |
| **Draw calls** | 0 — computed in the ground material, driven by a state uniform |
| **Form** | Soft warm curved strip following the bowl's far edge, **0.10 H** tall at centre, tapering to **0.05 H** at the frame's left and right extremes |
| **Colour** | **Sunlight Amber `#FFE3B8`**, additive, **0.22** intensity — a Tier-4 light term, exempt by function, and at 27.8% HSV S under the wall regardless |
| **Softness** | Feathered top and bottom over 0.04 H. **No defined edge, ever** — an edge would make it a line, and a line is what this element exists to avoid being |
| **Present in** | **Menus only.** Masked to 0 in all six other states, per §2.8 |
| **Non-colour channel (§1.2)** | Its *presence* is the signal, and presence is not colour. A reviewer verifies the rule by toggling the uniform, not by eyedropping |

**Declined: a permanent horizon line.** Making the band always-on would give the child
a permanent orientation cue, which sounds helpful. Refused because it would also give
Menus no signature of its own — §2.9's whole purpose is proving the seven states
separable, and Menus' signature variable is **camera angle**. A second exclusive
element competing for that role weakens the proof. It is present in exactly one state
so that a child who opens a menu and sees the band is seeing a *change of
arrangement*, not a permanent feature they stopped noticing.

#### 6.3.4 Tiling-free, and why there is no ground texture at all

| Rule | Value | Reason |
|---|---|---|
| **Tiled albedo** | **None, anywhere.** Not the ground, not a prop, not the dome | A tile is a visible repeat. A visible repeat in the world's value field is a hierarchically legible pattern, and a legible pattern competes. §3.5 makes the environment the *lowest-contrast* element; a repeat is contrast |
| **Normal / detail maps** | **None** | The surface normal varies by <4° across the whole field, so there is no structure for a normal map to describe. A normal map here would invent detail the geometry does not have, and would break the matte finish at 0.02 gloss |
| **Roughness / gloss maps** | **None.** Flat 0.02 on all world materials | A gloss variation is a value variation. See 6.3.1 |
| **Vertex-colour hand-painting** | **None** | The specific temptation when a world has "handmade" qualities. Refused by 6.1.2 decision 4 — handmade lives in **placement**, not surface. A painted gradient is a texture, and it will not survive a 6-bit mobile target cleanly |
| **Decals** | **None** | A decal is a placed texture. Same objection, plus it is a fourth thing that could drift from the palette |
| **Noise, dither, LOD transitions** | **None** | At 8.7% of the triangle budget (§6.6) the world never needs a dither pattern, and a dither is a texture |

**The consequence, stated plainly so it is not a surprise at production:** the world's
entire material budget is **one untextured matte clay material with a world-space
vertical value ramp**. That is the whole thing. It is also why §6.6's numbers are
achievable on a mid-range Android, and it is the direct implementation of
`AGENTS.md` §5 — *"buy visual quality with shading and lighting, not polygon count or
texture resolution."* The world is the proof of that sentence.

#### 6.3.5 Motes

§2.2 specifies a slow drift of **40 warm motes** in First Open / Greeting. That is a
particle system and therefore a VFX asset, so this section specifies only its visual
envelope and hands the build to §8 and the technical artist.

| Property | Value | Note |
|---|---|---|
| **Count** | **40** maximum, all states that have them | Fixed by §2.2 |
| **States present** | **First Open / Greeting only.** Masked to 0 in all six others | They are the Greeting's warmth cue and belong to the Greeting |
| **Size** | **0.012 – 0.02 H** each | Never larger than §4.5.4 P5's 0.08 H minimum-feature floor, and never approaching a 0.05 H screen-readable speck that would read as debris |
| **Colour** | **Sunlight Amber `#FFE3B8`** at 0.20 – 0.45 alpha, additive | A light term, exempt by function; at 27.8% HSV S it is under the wall anyway. **No mote may use a Tier-0 paint hue** — a red mote reads as an error pixel (§4.2), and a mote is a light, not a paint |
| **Motion** | Slow vertical drift, **0.15 H per 6 s**. No lateral velocity, no twinkle, no flicker | A twinkling mote is a sparkle at 20 Hz, and 20 Hz flicker on a mid-range panel under 60 fps produces a visible stutter. **No flicker above 3 Hz anywhere in the world** |
| **Depth** | Soft-particle, no hard intersection with the ground | A mote clipping through the ground is a hard edge, and a hard edge in a field with no other hard edges reads as a defect |
| **Shadow** | None. Motes cast nothing and receive nothing | §3.5 rule 1 |
| **During Painting** | **Absent**, with the rest of the state's set-dressing | 6.3.6 |

#### 6.3.6 How the world goes still during Painting

This is the mechanism behind §2.4 and §5.6's stillness, and it is specified here in
full because the world is what has to stop.

> **The rule: during Painting, the environment contains exactly one source of change
> in the entire frame, and it is the child's finger.** The world does not move, does
> not drift, does not pulse, does not breathe, and does not react.

| Element | Ambient | **During Painting** | Transition |
|---|---|---|---|
| **Prop sway** (all 24 pebbles, 8 arches, 16 tufts) | 2% amplitude, 6.0 s period, per-prop phase | **0% — off** | Critically damped to zero over **400 ms**, **0 overshoot** (§5.6: props get 400 ms damped, 0.00 overshoot) |
| **Sky gradient** | Static | **Static** — no drift, no scroll, no noise, no shimmer | No transition; it never animates in any state |
| **Ground value ramp** | Static | **Static in shape**, value **−1 rung** (§4.6) | Ladder step over **≥ 0.4 s** (§2.1 law 6) |
| **Environment saturation** | 33.2% | **×0.75 → 24.9%** (§4.6) | ≥ 0.4 s |
| **Sun disc** | Static | **Static.** No pulse, no flare, no brightness change | No transition; it never animates in any state |
| **Horizon band** | Absent | **Still absent** — Menus-only (§2.8) | n/a |
| **Motes** | Absent | **Still absent** | n/a |
| **Reward sunburst** | Absent | **Still absent** | n/a |
| **Contact occlusion** under the creature | Tracks the creature | **Tracks the creature** | Continuous; it follows the body, which is the creature's motion, not the world's |
| **Creature cast-shadow blob** | Tracks the creature | **Tracks the creature** | Continuous |

**What is left moving in frame during Painting, in full:**

| Source | Motion | Period | Why it is permitted |
|---|---|---|---|
| **The child's finger** | The stroke | — | The event. §2.4's whole design |
| **The wet paint bead** | Beads, settles, flattens | 0.8 s bead → 0.6 s settle | §2.4's wet/dry lifecycle; the paint is the child's, and its settling is the only motion that reads as *consequence* |
| **The creature's breathing** | Spine scale 1.00 → 1.03 → 1.00 | **5.2 s** (up from 4.0 s) | §5.6: slower breathing = absorbed. It is the creature, not the world |
| **The creature's ears and hook** | Lag the body by 200 ms | 5.2 s | §5.6 — the 200 ms lag is the aliveness read |
| **The creature's blink** | Suppressed for **0.8 s** after each stroke | — | §5.6 — the creature does not look away from the child's work |

**One moving thing would be a design failure. Three moving things — finger, paint,
creature — is the correct number: an event, its consequence, and a living witness.**

**Declined: keeping a little motion "so the world doesn't feel dead."** This is the
temptation, and it is refused. §2.4 names the environment-saturation drop as the
**exclusive signature variable** of Painting — *"No other state desaturates the world.
The world dropping back is the signal that the child's work is now the subject."* That
signal is a **stillness** signal. A world with residual drift during Painting is a
world that has not fully stepped back, and the child reads *"the world is still
happening"* instead of *"I am making something."* The stillness is not an absence of
effort; **it is the effect**.

---

### 6.4 The single lighting rig

#### 6.4.1 One rig, and the environment has no lights of its own

**§2.1 governs. Nothing below adds a light.**

> **The environment authors zero lights. It receives the one shared rig, and every
> state modulates that rig along the named axes already fixed in §4.6.** A new
> environment light is an ADR, on the same terms as a new colour (§4.4.4 rule 3) and
> a new geometry class (6.2.1) — because all three are the same mistake: a new
> variable in a system whose entire value is that it has none.

| Rig element | §2.1 specification | **The environment's relationship to it** |
|---|---|---|
| **Key** | Directional. Azimuth **130°** from camera-forward, elevation **50°**. Colour `#FFE3B8`. Intensity **1.35** | **Full participation.** The ground bowl and all three scenery prop families receive the key normally |
| **Shadow colour** | `#F6B9A0` at **0.28** opacity | Applies to the creature's blob only. **The environment casts nothing** — 6.4.6 |
| **Sky fill** | Gradient dome: zenith `#FFF6E2` → horizon `#FFCFA0` | **The dome is the source.** Evaluated analytically as a two-colour hemisphere term. The environment's brightest contributor, and *free* — no texture, no probe, no draw |
| **Ground bounce** | `#F3B98F`, low intensity, warm | Hemisphere term from below. Gives the underside of each arch and the far side of each pebble a warm lift instead of a dark one. **This term is why no world surface approaches the L3 floor** |
| **Rim** | Directional, azimuth **180°**, elevation **22°**, two-tone: warm `#FFB0D0` upper silhouette, mint `#BFF3E2` at base. Intensity **0.55** | **MASKED OUT OF THE ENVIRONMENT ENTIRELY. 0%.** See 6.4.3 — the most counter-intuitive line in the section, and not negotiable |
| **Ambient** | Flat `#FFF0D8`. No grey, no cold | Full participation, uniform, flat |
| **Sun disc** | Visible emissive disc at the key's azimuth | Emissive, unlit, **static in every state** (6.1.2 decision 3) |
| **Horizon glow strip** | *Not* in §2.1 — specified at 6.3.3 | Sunlight Amber additive term in the dome material, **Menus only** |
| **Radial warm sunburst** | *Not* in §2.1 — §2.7 specifies it | Sunlight Amber additive term in the dome material, **Results only**. Non-rotating, soft, symmetrical |

**The two set-dressing terms — glow strip and sunburst — are shader terms inside
materials that already exist. They add 0 lights, 0 triangles, and 0 draw calls.** They
are listed separately here only so a reviewer can see the environment's total light
count: one rig, plus one emissive disc, plus two material-level additive terms, of
which neither is a light.

#### 6.4.2 Key-to-fill ratios on world surfaces

| Surface | Key | Sky fill | Ground bounce | Ambient | **Resulting value spread on the surface** |
|---|---|---|---|---|---|
| **Ground bowl** | 1.35, full | Hemisphere, zenith→horizon | `#F3B98F`, low | `#FFF0D8`, flat | Roughly **L6 → L7** end to end, with <4° of normal variation keeping the whole ramp inside one narrow band |
| **`pebble`** | 1.35, full | Hemisphere | Bounce, low | Flat | **L6** on the lit side → **L5** Sand at the darkest turn of the form. A **13.5%** intra-object range, which is the *entire* dynamic range a prop is allowed |
| **`arch`** | 1.35, full — the crown catches the key, the span's underside does not | Hemisphere | **Bounce does most of the work here** — the underside is lit by the ground, not the sky | Flat | **L6** crown → **L5** underside. The underside never goes darker than L5, because the bounce term is warm and non-zero |
| **`tuft`** | 1.35, full | Hemisphere | Bounce, low | Flat | **L6** → **L5** in the lobe valleys. Valleys are the darkest world feature and they still clear **L5 Sand at 66.0%** — five rungs above the L1 limit |
| **Sky dome** | None — unlit | *Is* the fill | None | None | **#FFF6E2 92.7% → #FFCFA0 68.4%**, a 24.3-point vertical ramp |
| **Sun disc** | None — emissive | — | — | — | **Sunlight Amber `#FFE3B8`**, unlit, static |
| **`keystone`** | 1.35, full | Hemisphere | Bounce, low | Flat | **L5 Sand 66.0%** — one rung up from a scenery prop, per §5.3(d). The permanent value discriminator between a reward and a prop |

**The load-bearing number.** §4.5.3 Rule V2 caps any state at **3.5:1** and Rule V1
floors any surface at **25% of its base rung**. The environment's actual worst case:

| | Luma | |
|---|---|---|
| Lightest world surface in any state | **#FFF6E2** 92.7% (sky zenith) | |
| Darkest world surface in any state | **#FFCFA0** 68.4% (fogged sky horizon) | Never a ground or prop surface — the ground's darkest is L6 79.5% and a prop's darkest turn is L5 66.0% |
| **Environment-internal ratio** | **1.35 : 1** | **Nowhere near any state's ceiling.** §4.5.3's tightest state (Menus, 2.1:1) is satisfied by the environment alone; the state's ratio is carried by the creature and the UI, not the world |
| Headroom to the 3.5:1 ceiling | **2.15 : 1 of unused range** | The world is *designed* to be low-contrast. That headroom is deliberate and it is what §6.7 spends |

#### 6.4.3 The rim is masked out of the environment

**This is the most counter-intuitive decision in Section 6, so it gets its reasoning
in full rather than a line in a table.**

§2.1 gives the rig a rim light, and §2.1 law 3 says *"The rim light is never off in
any state except Minigame … It guarantees the creature reads against any
background."* The obvious reading is that the rim lights the whole scene. It does not,
and it must not.

| Reason | Detail |
|---|---|
| **The tokens forbid it** | §4.1: **Rim Rose `#FFB0D0`** is *"the creature is alive"* and *"**nothing else in the world**"* — it is "the only colour permitted to sit on a silhouette edge." **Mint Halo `#BFF3E2`** is explicitly a light term with *"**no surface use, no UI token**"*, "no environment material," and is "the only hue in the product between 150° and 200°," rationed precisely because §2.1 law 4 forbids cold light. Both are hard-limited and masked out of every environment material |
| **The tier table forbids it** | §3.5 denies rim light to **Supporting** (props) and to **Receding** (ground, dome, disc) as a *standing* denial, not a per-state choice |
| **The composition forbids it** | A rim on the world means every pebble has a glowing pink edge. §3.5 rule 1's whole argument is *"the eye follows shadows, and there is exactly one."* The eye follows rims the same way. A world of 48 pink-edged pebbles is a frame with 48 miniature focal points, and the creature — supposed to be the *only* silhouette carrying the aliveness signal — is one of a crowd |
| **The mood forbids it** | Rim light is a **dawn** phenomenon, and dawn rim reads on *subjects*, not on *terrain*. A lit horizon and a lit creature are the two things a sunrise is for. Spending the effect on pebbles spends the strongest lighting idea in the product on its least important content |

**How it is implemented, and the one place the environment does get an edge lift:**

| Surface | Rim term | Edge lift received from |
|---|---|---|
| `pebble`, `arch`, `tuft`, `keystone` | **0%** | The **key's** soft terminator wrap (6.4.4) only |
| **Ground bowl** | **0%** | Hemisphere fill + key, both wrapped |
| **Sky dome** | **0%** — unlit anyway | — |
| **The creature** | **0.55** in five states, **×2.0** at Flourish, **×1.15** warm-only in Painting, **0.15** whisper in Minigame | §2.1, §4.6 |

**The one concession, and its size.** A prop's top edge does receive a faint lift —
but **not from a rim light.** It is the key light's wrap (6.4.4) doing its job on a
rounded crown, which is ordinary diffuse shading, not a special effect. A pebble's lit
crown reaches roughly **7–9% of its own luma** across 0.06 H of curvature, and it is
**one rung at most** above the prop's base rung. It is not a glow, it has no colour
distinct from the material, and it does not appear on the prop's *silhouette edge*
against the background — it appears on the surface facing the sun. That distinction is
the entire difference between a rounded object catching morning light and an object
with life drawn around it, and §4.1's binding note requires it be kept: **the two rim
halves are separated by vertical position on the silhouette, and nothing else in the
world may appear on a silhouette edge.**

**Declined: a cool world rim in Mint Halo at low intensity, to keep the ground from
going flat.** Refused for a specific reason. §4.1 explains Mint Halo is "rationed" and
is the product's *only* permitted hue between 150° and 200°. A cyan rim on 48 pebbles
would consume the ration across the world's largest surfaces and leave none for the
creature's base — the one place §4.1 says it belongs, bound to **position (the
feet)** and **stillness**. The ground does not go flat, because the **ground bounce**
term (`#F3B98F`) lifts every underside in the frame. Warm is also the correct answer:
§1.3's forced call is *"Cool or warm shadow? Warm. Shadows tint toward peach-rose.
Neutral or blue is a bug."*

#### 6.4.4 Terminator softness

| Property | Value | Reason |
|---|---|---|
| **Diffuse wrap** | **0.45 half-Lambert wrap** on all world materials | A wrap of 0.45 pushes the terminator past the geometric 90° point, so the shadow-side transition begins *before* the surface turns away from the key. This is how the world gets its "soft terminator" from `game-concept.md` §7 |
| **Terminator band width** | **≥ 0.35 of the lit hemisphere** | Long enough that the transition is never resolvable as an edge at any camera distance. `game-concept.md` §7 requires "soft terminator"; this is the number |
| **Hard terminator** | **Prohibited** on every world material | A hard terminator is a hard line. §3.3 prohibition 2's reasoning — *"a hard line reads as a wall"* — applies to shading edges as much as to horizon edges |
| **Specular** | **0.02** on all world materials, matching the creature's blank shell | §3.3 prohibition 5 bans reflective surfaces. At 0.02 the only specular is a broad dim sheen that follows the form and carries no information. Raising it produces a moving highlight — a second bright thing, at camera-motion frequency |
| **Fresnel / edge light** | **None** | This is how world rims get built by accident, and 6.4.3 forbids it |
| **Normal perturbation** | **None** — no normal maps (§6.3.4), no triplanar detail, no vertex noise | Every world normal is the true geometric normal. This is what guarantees §3.1's 35% minimum corner radius reads as *roundness* rather than as a lighting artefact |

#### 6.4.5 Surface finish — clay and wax

The world's material finish, named so it can be reviewed rather than interpreted.

| Property | Specification |
|---|---|
| **Base** | Matte. Gloss **0.02** |
| **Diffuse** | Full, wrapped, **metalness 0.0 on every world material.** A metallic world term produces a cold hard reflection by definition |
| **Micro-variation** | **None.** No roughness variation, no albedo variation, no normal variation. The world's "handmade" quality is placement and silhouette only (6.1.2 decision 4) |
| **Cavity / AO** | **Contact occlusion only** — a 0.15-opacity Peach Shadow smear directly under each prop's footprint. Never a baked cavity map, never a screen-space AO pass, never an SSAO composite on world geometry |
| **Read at 64 px** | Every prop family must be identifiable by silhouette alone at 64 px, in greyscale, with no lighting help. §1.2 applied to the environment: the three silhouettes are the world's only vocabulary, and the child meets them constantly |

**Contact occlusion in full, because it is the environment's only grounding device:**

| Property | Value |
|---|---|
| **Purpose** | Grounds a prop. **Not** a cast shadow (§3.5 rule 1) |
| **Colour** | **Peach Shadow `#F6B9A0`**, the product's "colour of not-wrong" (§4.1) |
| **Opacity** | **0.15** — exactly half the creature's 0.28, and the number is load-bearing: at 0.28 a prop's contact patch would read as a cast shadow, and the creature would stop being the only shadow in the game |
| **Form** | A tight elliptical smear **under the footprint only**, extending **0.03 H** beyond the base. **Not** a directional projection, **not** offset toward the anti-key direction, **not** elongated |
| **Geometry** | **0 triangles** — projected and composited in the lighting pass, exactly as the creature's blob is (§5.7) |
| **Greyscale separation from the creature's shadow** | **0.13 opacity** of difference, plus a **form** difference (tight ellipse vs. directional projection), plus a **size** difference. Three independent channels — §1.2 satisfied, and the check is: *desaturate the frame, confirm two shadow weights that are visibly different and neither resembling damage* |
| **Fade** | Beyond **3.5 H** from camera the contact occlusion fades out. It is sub-pixel at that distance, so the loss is zero by construction — the same shading-LOD argument as §5.7 |
| **During Painting** | Present, and follows the creature. It is the creature's motion, not the world's (6.3.6) |

#### 6.4.6 The zero-triangle shadow, and the fact that nothing else casts one

> **There is exactly one cast shadow in this product. It is the creature's. It costs
> zero triangles. And the environment has no shadow map at all.**

This is restated in full because it is the section's largest and most counterintuitive
performance and art decision at once.

| Element | Shadow behaviour | Triangles | Draw calls |
|---|---|---|---|
| **Creature** | **The only cast shadow.** Soft projected blob, composited in the lighting pass, **Peach Shadow `#F6B9A0` at 0.28 opacity**, offset away from the key, soft edge over 0.06 H | **0** | **0** |
| **`pebble`** | Contact occlusion 0.15 | **0** | **0** |
| **`arch`** | Contact occlusion 0.15 | **0** | **0** |
| **`tuft`** | Contact occlusion 0.15 | **0** | **0** |
| **`keystone`** | Contact occlusion 0.15 | **0** | **0** |
| **Ground bowl** | Receives the creature's blob. **Casts nothing** | 0 | 0 |
| **Sky dome** | Unlit. **Casts nothing, receives nothing** | 0 | 0 |
| **Sun disc** | Emissive. Casts nothing | 0 | 0 |
| **Motes** | Cast nothing, receive nothing | 0 | 0 |
| **Total shadow geometry in the product** | | **0** | **0** |

**Consequences, and they are large:**

1. **No shadow map is required for the environment at all.** If the creature's blob is
   a projected decal rather than a real shadow, then nothing in the scene casts a real
   shadow, and the environment's shadow-casting pass is empty by construction. That
   removes the environment's entire shadow cost — no cascade setup, no depth pass
   allocation, no shadow-map render target, no per-frame shadow draw.
2. **A prop shadow would be a hierarchy defect even if it were free.** §3.5 rule 1:
   *"the eye follows shadows, and there is exactly one."* Two shadows, or forty-eight,
   and the composition gains focal points that compete with the creature.
3. **A prop shadow would be a mood defect.** §3.3 prohibition 4 gives the direct
   reason: *"A hard prop shadow competes with the creature for the eye and breaks the
   'nothing is wrong' read."* Softness does not rescue it, because even a soft
   directional shadow is a *directional* event in a world whose only other event is
   the child's finger.
4. **The creature still casts the only shadow in the game. §3.5 rule 1 is preserved
   intact.** It simply costs 0 triangles — the §5.7 argument, applied here without
   modification: a shadow mesh on an always-rim-lit creature would spend triangles
   producing an object that carries no information (§4.3 C9 declares it
   non-informational), and it would cast a visible self-shadow across a painted mark,
   which is a data-loss bug on the surface that stores the child's save file.

**Declined: enabling the environment's shadow-casting "for future flexibility."** The
capability does not exist. Adding it later means the environment starts casting
shadows that nothing has art-directed, on a mid-range Android, in a product whose
entire composition argument rests on there being exactly one. If a future feature
needs contact darkening it uses the contact-occlusion path, which is already
specified, already zero-geometry, and already on the correct colour.

---

### 6.5 Materials and palette use

#### 6.5.1 Every world surface, on the ladder

The complete inventory. **No world surface exists that is not in this table.**

| World surface | Hex | **Ladder rung** | Luma L | HSV S | **Tier** | 6-bit check |
|---|---|---|---|---|---|---|
| **Sky dome, zenith** | `#FFF6E2` | above L7 — light term | 92.7% | **11.4%** | Tier-4 gradient | Fine |
| **Sky dome, horizon** | `#FFCFA0` | between L5 and L6 | 68.4% | **37.3%** | Tier-4 gradient | Fine — **the highest HSV S in the world** |
| **Sun disc** | `#FFE3B8` | L7 band — light term | 90.1% | **27.8%** | Tier-4 emissive | Fine |
| **Horizon glow strip** *(Menus)* | `#FFE3B8` additive 0.22 | L7 band — light term | — | **27.8%** | Tier-4 additive | N/A — additive |
| **Radial sunburst** *(Results)* | `#FFE3B8` additive | L7 band — light term | — | **27.8%** | Tier-4 additive | N/A — additive |
| **Ground bowl, near field** | `#E8C79B` | **L6** Honey Field | 79.5% | **33.2%** | Tier-3 | Fine — §4.5.4 P3 needs ≥1.6% between adjacent rungs |
| **Ground bowl, outer 30%** | `#F7E9D2` | **L7** Dawn Cream | 91.9% | **15.0%** | Tier-3 | Fine |
| **`pebble` / `arch` / `tuft`** | `#E8C79B` | **L6** Honey Field | 79.5% | **33.2%** | Tier-3 | Fine |
| **Prop darkest turn** (lit → far side) | `#C0A584` | **L5** Sand | 66.0% | **31.2%** | Tier-3 | Fine — **the floor for prop geometry, 4 rungs above the L1 limit** |
| **Contact occlusion** *(all props)* | `#F6B9A0` @ 0.15 | light/contact term | 76.9% | **35.0%** | Tier-4 | N/A — alpha-blended |
| **Creature cast-shadow blob** | `#F6B9A0` @ 0.28 | light/contact term | 76.9% | **35.0%** | Tier-4 | N/A |
| **`keystone`** *(post-demotion)* | `#C0A584` | **L5** Sand | 66.0% | **31.2%** | Tier-3 | Fine — one rung up from L6, per §5.3(d) |
| **Motes** *(Greeting)* | `#FFE3B8` @ 0.20–0.45 additive | L7 band — light term | — | **27.8%** | Tier-4 | N/A — additive |
| **All world materials** | *base* | — | — | — | — | **Metalness 0.0 · gloss 0.02 · alpha 1.0 · zero textures** |

**Two rows in that table are the only world surfaces permitted to reach 35.0% HSV S,
and they are the shadow terms** — which is why §4.4.2 names Peach Shadow 35.0% as
"the tier's maximum." The world runs out of saturation exactly where the product needs
it, and not one point further.

#### 6.5.2 The world saturation ceiling, per state

| State | Ground / props | Sky horizon | Contact occlusion | **World's peak HSV S in frame** | §4.4.5 build check |
|---|---|---|---|---|---|
| **Idle Ambient** *(baseline)* | 33.2% | 37.3% | 35.0% | **37.3%** | ≤ 42.0% — PASS |
| **First Open / Greeting** | 33.2% | 37.3% | 35.0% | **37.3%** | ≤ 42.0% — PASS |
| **Painting** | **24.9%** (×0.75) | **28.0%** (×0.75) | **26.3%** (×0.75) | **28.0%** | ≤ 42.0% — PASS |
| **Signature Mark Flourish** | 33.2% | 37.3% | 35.0% | **37.3%** | ≤ 42.0% — PASS |
| **Minigame** | 33.2% | 37.3% | 35.0% | **37.3%** | ≤ 42.0% — PASS |
| **Results / Reward** | 33.2% | 37.3% | 35.0% | **37.3%** | ≤ 42.0% — PASS |
| **Menus** | **26.6%** (×0.80) | **29.8%** (×0.80) | **28.0%** (×0.80) | **29.8%** | ≤ 42.0% — PASS |

| | Value |
|---|---|
| **Absolute world ceiling** | **40.0% HSV S ± 2.0** (§4.4.2 Tier-3) |
| **Build-check threshold** | **≤ 42.0%** (§4.4.5 check 2) |
| **Highest value the world ever reaches** | **37.3%** — sky horizon, Ambient |
| **Headroom** | **2.7 points** to the wall, **4.7** to the build-check threshold |
| **Result** | The world **cannot** breach the saturation wall in any state, at any lighting value, because its peak is a *fixed authored colour* and the world's two modulating states modulate **down** in both cases |

**The consequence for the frame.** `game-concept.md` §7 requires *"accent: saturated
candy tones reserved for rewards and the child's own paint — so authorship is always
the most colorful thing on screen."* §1.4 Principle 1's test is: *"eyedrop the five
most saturated pixels. If any is not the child's paint, a signature mark, or a
flourish reward, that is a violation."*

| Frame | The five most saturated pixels are | Verdict |
|---|---|---|
| Any state with the creature's paint on it | Tier-0 paints, **45.3% – 83.2%** HSV S | **PASS** — nothing environmental comes within 10.3 points of the lowest Tier-0 entry (Petal, 45.3%), measured against the authored environment max of 35.0% (Peach Shadow) |
| Any state with no paint on the creature (Results, Menus) | The world peaks at 37.3%; the creature at **Dawn Cream 15.0%** | **PASS, with a stated condition** — below |

**The stated condition, and it is a real one.** §1.4 Principle 1's test presupposes
paint on the creature. In Results and Menus the tiered hierarchy has no saturated
pixel at all, so the test has nothing to rank. This is **correct and intended**:
Results is *"this is mine, and it's staying"* and Menus is *"everything is here,
nothing is urgent."* A loud frame would contradict both. The environment is still
fully inside its ceiling in those states, and the test is re-run the instant paint
returns — which is why a flourished reward object carries hero-tier rim and its 1.4×
growth, so a reward frame is never a quiet frame.

#### 6.5.3 The explicit forbidden list

| # | Forbidden | Threshold | Enforced by |
|---|---|---|---|
| 1 | **Pure black** | `#000000`, anywhere, on any surface, at any opacity | `game-concept.md` §7, §4.1 |
| 2 | **Any diffuse surface below L1** | < **23.1% luma** | §4.5 Rule V1 |
| 3 | **Any world surface below L3** | < **41.6% luma.** The world's actual floor is **L5 Sand at 66.0%** — 4 rungs of margin | §4.5 Rule V1 |
| 4 | **Desaturated grey or neutral** | ≤ **5.0% HSV S** on any world surface | `game-concept.md` §7, `AGENTS.md` §7 |
| 5 | **Cold blue in shadow** | Any world surface hue outside **17° – 42°** | §2.1 law 4, 6.5.4 |
| 6 | **Any non-Tier-0 pixel above 40.0% HSV S** | Hard wall. No exceptions, no state | §4.4.4 rule 1, §1.4 Principle 1 |
| 7 | **Red on any world surface** | No world material may use Cherry Pop `#E42C48`, Cranberry `#C4215E`, or any hue within 15° of either | §4.2 — red means error, *"and there is nothing to be in danger from."* Those two colours exist **only** as something the child can pick |
| 8 | **Gold as value** | No world surface may use Sunbeam `#F5D33C`. Sunlight Amber `#FFE3B8` is permitted **only** as a light term — sun disc, glow strip, sunburst, motes | §4.2 — *"Gold-as-value is the most manipulative visual grammar in games and it is banned outright."* Our gold is *attention*, never *price* |
| 9 | **Blue or violet on any world surface** | No world material may use Splash `#3FC4E0`, Bluebell `#4A7FE0`, Indigo Pop `#5B5BE0`, or Violet Pop `#8B4FD6`. **Note:** the dome's `#FFF6E2` zenith is 41.4° hue, inside the band — the cool end of the *world* is a warm off-white, never a blue | §4.2 — *"Blue = cold / unfriendly / 'other'."* No system surface uses blue |
| 10 | **Green as "go" or "success"** | No world material may use Leaf `#4FA83C` or Grass Pop `#6FBF3F`, and **no world state may signal success by a colour change** | §2.6, §4.2 — success is carried by shape + motion + a fixed 12-spoke radial burst |
| 11 | **Rim Rose on a world surface** | `#FFB0D0` — hard-limited to the creature's upper rim | §4.1, 6.4.3 |
| 12 | **Mint Halo on a world surface** | `#BFF3E2` — hard-limited to the creature's base rim, and the only permitted hue in 150°–200° | §4.1, 6.4.3 |
| 13 | **Quiet achieved by desaturating toward grey** | Any state going below the ×0.75 / ×0.80 floors is a defect | §2.8 — *"Quiet is achieved through lower value and lower saturation, never through grey"* |
| 14 | **Any new colour** | Not derivable from an existing rung by 6.5.5's three operations | §4.4.4 rule 3 → **ADR** |

**On #13, stated as a warning because it is the likeliest failure.** A menu that dims
by desaturating toward neutral will pass every numeric check except this one, because
a low-saturation warm colour and a neutral colour are the same number in a
screenshot's average. The reviewer test is not a threshold — it is **"is it still
obviously warm?"** A menu a child describes as *grey* fails, whatever the eyedropper
says.

#### 6.5.4 The world hue band — making "no cold blues" a number

`game-concept.md` §7 lists *"cold blues in shadow"* as forbidden, but a prohibition
with no threshold is unenforceable. Every colour the world may use falls inside a
**17° – 42°** hue band, and the band is **derived from the palette, not chosen**:

| World token | Hue | Inside 17° – 42°? |
|---|---|---|
| **Peach Shadow `#F6B9A0`** | **17.44°** | Yes — *this sets the lower bound* |
| Ground bounce `#F3B98F` | 25.20° | Yes |
| Sky horizon `#FFCFA0` | 29.68° | Yes |
| **Sand `#C0A584`** (L5) | 33.0° | Yes |
| **Honey Field `#E8C79B`** (L6) | 34.29° | Yes |
| **Sunlight Amber `#FFE3B8`** | 36.34° | Yes |
| **Dawn Cream `#F7E9D2`** (L7) | 37.30° | Yes |
| L8 Highlight `#FFFBF4` | 40.00° | Yes |
| Sky zenith `#FFF6E2` | 41.38° | Yes — *this sets the upper bound* |

| | Value |
|---|---|
| **World hue band** | **17.00° – 42.00°** — warm, through amber, to warm off-white |
| **Band width** | **25.0°** — no cold, no green, no violet, no blue, ever, on any world surface |
| **Justification** | §1.3: *"Cool or warm shadow? Warm. Shadows tint toward peach-rose. Neutral or blue is a bug."* The lower bound is set by the product's single most important negative colour; the upper bound is set by the sky, the product's lightest field |
| **Rim Rose 335.70° and Mint Halo 160.38°** | **Both outside the band, and both forbidden on surfaces** — which is the point. The band is what guarantees the two light terms are *only ever light* |
| **Build check** | Any world material with hue < 17.0° or > 42.0° → **reject** |

#### 6.5.5 Tier-3 derivation rule — deriving a new world tint from an existing rung

> **There are seven colours in this product. A "new world tint" is not a new colour:
> it is an existing rung with a state parameter applied. Anything that cannot be
> produced by the three operations below is a new colour, and a new colour is an
> ADR.**

This is what makes §4.4.4 rule 3 enforceable rather than aspirational, and it is the
mechanism by which a state gets a look without anyone hex-picking a value in a shader
inspector.

**The three legal operations, and only these three:**

| # | Operation | Permitted range | Machine constant | Example, and who uses it |
|---|---|---|---|---|
| **1** | **Ladder step** — select an adjacent named rung, or step one rung by scaling HSL `L` | **Down only** for world surfaces, unless a §4.6 state table explicitly authorises a step (Flourish: −2 rungs) | **× 0.85 per step on HSL `L`** (§4.5.2). Re-derive when the ladder is retuned — do not leave it stale | Ground near field **L6** → outer ramp **L7** by ladder select. Menus **×0.85** value. Painting **−1 rung** |
| **2** | **Saturation scale** — multiply HSV `S` | **Down only.** Never up, never above 1.00, never above 40.0% absolute | **×1.00 / ×0.85 / ×0.80 / ×0.75**, as fixed in §4.6 | Painting **×0.75** → 24.9%. Menus **×0.80** → 26.6% |
| **3** | **Warm-hue clamp** — rotate H toward the band | **Into** 17° – 42°. Never out of it | Linear rotation, clamped | Any new world material's authored hue is clamped into the band at import |

**Worked example, so the rule is demonstrable rather than theoretical.** A request
arrives: *"the arches need to read a little differently from the pebbles."*

| Step | Operation | Result | Verdict |
|---|---|---|---|
| 1 | Author the arches at **L6 Honey Field** `#E8C79B` — the same rung as every scenery prop | 33.2% HSV S, 79.5% luma | Legal — an existing rung |
| 2 | Step **down one rung** (operation 1), arch family only | **L5 Sand `#C0A584`**, 31.2% HSV S, 66.0% luma | Legal — one ladder step, ×0.85 |
| 3 | No saturation change (operation 2) | Peak stays 31.2% | Legal — never raised |
| 4 | Hue 33.0°, already inside 17° – 42° | No clamp needed | Legal |
| | | **Result: arches at L5, pebbles at L6. One rung of value separation between two families. Zero new colours.** | **APPROVED** |

**And the same request answered wrongly:**

| Wrong approach | Why it is rejected |
|---|---|
| Authoring a new hex, e.g. `#D9B48C` "a warmer sand" | A new colour. §4.4.4 rule 3 → **ADR**, Pillar 1 and Pillar 3 review |
| Raising the arch saturation to 38% "so it stands out" | Operation 2 is **down only.** A prop that stands out by saturation competes with the paint, and it would be a *new* peak in the §6.5.2 table |
| Giving the arches a **Rim Rose** edge "so they read" | §4.1: Rim Rose is the creature-is-alive token and nothing else in the world. 6.4.3 |
| Making the arches **taller** to read | §3.3 prohibition 6, 0.6 H ceiling — and a taller arch approaches the creature's silhouette |
| Giving the arches their own **material** | 6.1.2 decision 4 — handmade lives in placement and silhouette, not surface. One world material, and that is load-bearing for §6.6's draw count |

**The gate, for a reviewer.** A proposed world tint may be approved if and only if all
four are true —

1. It names an **existing rung or token** as its base.
2. It is produced by **operations 1, 2, 3 only**, subject to their restrictions.
3. Its resulting absolute values sit inside **≤ 40.0% HSV S**, **≥ L3 / 41.6% luma**,
   and **17° – 42° hue**.
4. Its §6.5.2 per-state row has been recomputed and the world's peak is still under
   the wall.

**If any of the four fails, it is a new colour and it is an ADR.** No exceptions for
"it's only a small prop," and no exceptions for "it's only in one state."

#### 6.5.6 The world has one material

| Property | Value | Reason |
|---|---|---|
| **Distinct world materials** | **One.** `Dab/Env/ClayMatte` — matte, gloss 0.02, metalness 0.0, untextured, world-space value ramp. All four prop families share it, and so does the ground bowl | §6.3.4 |
| **Dome material** | `Dab/Env/GradientDome` — unlit, two-colour world-Y gradient, plus the two additive terms. Its own material because it is **unlit**, which is a different shader. Still **one draw** | §3.5 puts the dome in the Receding tier: untextured, gradient-only |
| **Shader variants consumed by the environment** | **2 base materials.** All state modulation is **uniform-driven**, not variant-generating | §5.7 hand-off item 5's discipline applied to the world: parameters are uniforms, not variants |
| **Total environment shader-variant cost** | **2 materials** against a **200-variant ceiling** | The environment costs **1%** of the shader budget. `AGENTS.md` §5 is satisfied by a wide margin |

---

### 6.6 Environment budget

#### 6.6.1 The environment inside the global ceilings

`AGENTS.md` §5: **60 fps, never below 30. Triangles < 300k, ceiling 500k. Draw calls
< 150, ceiling 250.**

| Item | Triangles | Draws | % of 300k pref. | % of 500k ceiling | % of 150 pref. | % of 250 ceiling |
|---|---|---|---|---|---|---|
| **Creature** (§5.7: base 10,000 + HOOK 2,400) | **12,400** | **2** | 4.1% | 2.5% | 1.3% | 0.8% |
| **Ground bowl** | 4,680 | 1 | 1.6% | 0.9% | 0.7% | 0.4% |
| **Sky dome** | 1,280 | 1 | 0.4% | 0.3% | 0.7% | 0.4% |
| **Sun disc** | 96 | 1 | 0.0% | 0.0% | 0.7% | 0.4% |
| **`pebble` batch** (max 24 × 320) | 7,680 | 1 | 2.6% | 1.5% | 0.7% | 0.4% |
| **`arch` batch** (max 8 × 480) | 3,840 | 1 | 1.3% | 0.8% | 0.7% | 0.4% |
| **`tuft` batch** (max 16 × 400) | 6,400 | 1 | 2.1% | 1.3% | 0.7% | 0.4% |
| **`keystone` batch** (max 4 × 560) | 2,240 | 1 | 0.7% | 0.4% | 0.7% | 0.4% |
| **Creature cast shadow** | **0** | **0** | — | — | — | — |
| **All prop contact occlusion** | **0** | **0** | — | — | — | — |
| **Horizon glow strip** *(Menus)* | **0** | **0** | — | — | — | — |
| **Radial sunburst** *(Results)* | **0** | **0** | — | — | — | — |
| **Shadow map / shadow pass** | **0** | **0** | — | — | — | — |
| | | | | | | |
| **ENVIRONMENT TOTAL** | **26,216** | **7** | **8.7%** | **5.2%** | **4.7%** | **2.8%** |
| **CREATURE + ENVIRONMENT** | **38,616** | **9** | **12.9%** | **7.7%** | **6.0%** | **3.6%** |
| **REMAINING HEADROOM** | **261,384** | **141** | **87.1%** | **84.6%** | **94.0%** | **92.4%** |

**That headroom is deliberate and it is not slack.** The world and the creature
together are **12.9%** of the preferred triangle budget and **6.0%** of the preferred
draw budget. The remaining 87% of triangles and 94% of draws are reserved for the
things that actually scale with a session: the six Echo families (§5.3(b)), the
Painting worklight band (§2.4), the 12-spoke success burst (§2.6), wet-paint beads, UI
compositing, and menu transitions. `AGENTS.md` §5 warns that *"draw-call regressions
discovered late are architectural, not fixable"* — and the only way to guarantee that
is to leave the world's cost provably, verifiably small and fixed at design time, which
is what this table does. **The world cannot become the reason a frame drops, because
the world is not a variable.**

#### 6.6.2 Draw-call derivation, itemised

| # | Draw | Present in | Notes |
|---|---|---|---|
| 1 | Ground bowl + value ramp + Menus glow strip | All states | One material; the glow strip is a uniform inside it |
| 2 | Sky dome + Results sunburst | All states | Unlit; the sunburst is a uniform inside it |
| 3 | Sun disc | All states | Emissive, unlit. May batch with #2 under the SRP Batcher; counted separately here to be conservative |
| 4 | `pebble` instanced batch, ≤ 24 | All states | GPU instancing, one material |
| 5 | `arch` instanced batch, ≤ 8 | All states | ← |
| 6 | `tuft` instanced batch, ≤ 16 | All states | ← |
| 7 | `keystone` instanced batch, ≤ 4 | **Results and after** | Reward-only family. §5.3(d): *"Cost: one prop family, one extra draw in the instanced batch"* |
| | **Total** | **7** | The four prop families share one material, so it is 3 world surfaces + 4 batches = **7 draws, not 11** |

**Why four batches and not one.** The four families are **four distinct meshes**, and
GPU instancing batches per mesh. They share one *material*, so they cost one shader
binding but four draw submissions. Merging them into a single mesh with per-instance
family selection is a different prop system, a different authoring pipeline, and a
shader variant — to save **3 draw calls out of a 150 budget we are using 6% of.**
Declined as a bad trade.

#### 6.6.3 The world has no geometry LOD — and it is the same argument as the creature's

§5.7's reasoning applies to the environment, for a different reason:

| Reason | Detail |
|---|---|
| **It would save almost nothing** | The world is **26,216 triangles = 8.7%** of the 300k preferred budget. An LOD system costs engineering, one or two shader variants, authoring time per prop family, and a pop-analysis pass — to amortise 8.7% of nothing |
| **The world has 3 unique meshes** | LOD exists to amortise cost across many *instances*. 24 pebbles at 320 tris are already 7,680 tris, and popping between two LODs of a 320-triangle pebble is a pop of an object smaller than the pixel it happens on |
| **A pop is a hard line, and hard lines are forbidden** | §3.3 prohibition 2's reasoning — *"a hard line reads as a wall"* — applies to a one-frame silhouette change exactly as it applies to a horizon. §5.7's own framing: **"§3.1's rule is an anti-pop rule"** |
| **The world's extents are bounded and known** | A 6 H bowl under a 9 H dome, viewed at the §2 camera framings, gives a **fixed** visible-triangle count. There is no roaming camera and no long view distance |

**The one shading LOD that does exist, and it is the same species as §5.7's:**

| Dropped beyond | What | Why it is free to drop |
|---|---|---|
| **3.5 H** from camera | **Contact occlusion** on props | Sub-pixel at that distance. Zero loss by construction |
| **4.5 H** from camera | **Tuft lobe interiors** — 3–5 lobes collapse to 2 silhouette lobes | At that range a tuft is 4–6 px; the lobe count is not resolvable, and the family still reads as a soft mass |
| **5.0 H** from camera | **Arch crown curvature**, flattening toward a rounded slab | Same. The family reads from silhouette at 4 px, which is all the ground bowl's ramp requires |

**Never dropped, at any distance, in any state:** the three family silhouettes
themselves · the **0.6 H height ceiling** · the value-rung separation between families
· the **absence** of a cast shadow · the **absence** of a rim term · the prop's gloss
and metalness. A prop that drops its material identity at distance has become a
different object, and the world has only three.

#### 6.6.4 Asset specification hand-off

Naming per `AGENTS.md` §6 and the project convention
`[category]_[name]_[variant]_[size].[ext]`.

| Asset | Type | Spec | Name |
|---|---|---|---|
| Ground bowl | Mesh, untextured, no UV2 | 4,680 tris, 72 × 32 + centre fan, radius 6 H, ≤ 8% elevation | `env_groundbowl_shallow_large.fbx` |
| Sky dome | Mesh, untextured, no UV | 1,280 tris, 40 × 16 hemisphere, radius 9 H, normals irrelevant (unlit) | `env_skydome_gradient_large.fbx` |
| Sun disc | Mesh, unlit emissive | 96 tris, 48-segment fan, azimuth 130° / elevation 50° | `env_sun_disc_medium.fbx` |
| `pebble` | Mesh, untextured | **320 tris**, icosphere 2 subdivisions, 0.06 – 0.18 H | `env_pebble_soft_small.fbx` |
| `arch` | Mesh, untextured, **solid — no negative space** | **480 tris**, closed solid, 0.22 – 0.42 H tall, span ≤ 0.44 H | `env_arch_solid_medium.fbx` |
| `tuft` | Mesh, untextured, **no alpha, no cutouts** | **400 tris**, 3 – 5 lobes × 80, 0.14 – 0.34 H | `env_tuft_lobed_small.fbx` |
| `keystone` | Mesh, untextured | **560 tris**, 0.30 – 0.44 H | `env_keystone_crown_small.fbx` |
| World material | Shader | `Dab/Env/ClayMatte` — gloss 0.02, metalness 0.0, world-Y value ramp | — |
| Dome material | Shader | `Dab/Env/GradientDome` — unlit, `#FFF6E2` → `#FFCFA0`, 2 additive uniform terms | — |
| Prefabs | Prefab | `PFX_Env_Pebble`, `PFX_Env_Arch`, `PFX_Env_Tuft`, `PFX_Env_Keystone` | — |
| Scene | Scene | `SCN_Habitat` — **one scene for the product.** Seven states, one place | — |

**Modelling requirements, binding on whoever builds these:**

| # | Requirement | Threshold |
|---|---|---|
| 1 | **Minimum corner radius** | **35% of the shortest local axis** (§1.3). In practice 100% — every prop is a superellipsoid or a merged set of hemispheres. **No vertex anywhere may read as a corner** (§1.5) |
| 2 | **No cone, spike, blade, or point** | Tips, where present, are **≥ 40% of base width** (§3.1) |
| 3 | **No alpha, no cutout, no transparent surface** on any prop | Fully opaque geometry. A prop with an alpha-tested silhouette is a fur or foliage system by another name (6.2.5) |
| 4 | **No LOD group** on any prop | 6.6.3. Any imported LOD group is to be stripped, not merged |
| 5 | **No lightmap UV, no light probes, no reflection probes** | The world is lit by one analytic rig; baked GI would break when the rig modulates per state, and a baked probe cannot follow §4.6's seven state tables |
| 6 | **No collider beyond a single ground-plane primitive** | Nothing in the world is solid. This is a visual world, and a collider hierarchy here would be a gameplay affordance nobody asked for |
| 7 | **Static batching enabled on all prop prefabs** | Draw calls are already at 7; static batching is a free fallback if a mid-range device disagrees about instancing |
| 8 | **Verify peak < 300k visible triangles on mid-range Android** | `AGENTS.md` §5; carried from §5.7 hand-off item 8, which this section extends to the world |

**Declined: an environment LOD group, and a lightmapped scene.** Both are declined for
the same underlying reason — they would make the world a *variable* when its entire
value proposition is that it is not. An LOD group is a pop waiting for a camera
angle; a lightmap is a baked rig that cannot respond to the seven state tables in
§4.6. The world is one unlit-gradient dome, one lit ground bowl, one emissive disc,
and four instanced batches, and it stays that way.

---

### 6.7 World treatment per state

**No new locations, no new zones.** The world is the same dish in all seven states.
Every row below is a **lighting, value, saturation, and set-dressing** change to one
place, per §2.3's rule: *"If a proposed state cannot be described as a delta from
Ambient, it is not a state — and it will not be built."*

**One derived principle that governs the whole table.** A uniform ladder step changes
a surface's *absolute* value but not its *ratio* to another surface stepped by the
same amount. Therefore **the world's internal contrast ratio is 1.35:1 in every one of
the seven states**, and every §2 state ratio — 1.9:1, 2.4:1, 2.9:1, 3.5:1, 2.2:1,
2.0:1, 2.1:1 — is produced entirely by the **creature and the UI** against the world,
never by the world against itself. The world is a flat, low-contrast field in all seven
states; it is the reference the other elements are measured against. This is the
numerical form of §3.5's reading order.

#### 6.7.1 The master table

| State | **Key the world receives** | **What the world DOES (motion)** | **What the world IS (value / saturation)** | **Set-dressing present** | **Prop behaviour** |
|---|---|---|---|---|---|
| **First Open / Greeting** | Base rig × **1.25** | Fog to **85% near-white**; **40 warm motes** drifting at 0.15 H per 6 s | **L7** · **33.2%** · frame is **+2 steps** brighter on entry, flooding **−2 steps over 1.2 s** to Ambient | Motes **only** | 2% sway, 6.0 s, phase-scattered. Nothing else moves |
| **Idle Ambient** *(baseline)* | Base rig **unmodified** | **Nothing.** 2% prop sway is the only motion in the world | **L6 → L7** ramp · **33.2%** | *None* | 2% sway, 6.0 s. The world's rest condition |
| **Painting** | Base rig × **0.85** | **Nothing. Sway OFF.** Gradient static, sun disc static, motes absent, no drift | **−1 rung** → **L5 → L6** · **×0.75 → 24.9%** | *None* | Sway damped to **0% over 400 ms**, 0 overshoot |
| **Signature Mark Flourish** | Base rig × **1.10**, azimuth **+15°**, elevation **62°** (three-quarter back) | Nothing. Props hold; the key's rotation sweeps its highlight across the ground once | **−2 rungs** · **33.2%** — *the world drops away* | *None* | Sway continues, and is the only thing moving in the world |
| **Minigame** | Azimuth **15°** (nearly frontal), elevation **60°**, intensity **0.95** | Nothing | **Pushed back and softened**: ground value approaches sky value by 2 rungs across the mid-field, leaving prop **silhouettes on a matched field** · **33.2%** | **Circular stage pool** under the creature, in the ground material | Sway **off** — the state is a locked, flat, working frame |
| **Results / Reward** | Base rig **unmodified** | **Radial warm sunburst**, slow and wide, **symmetrical and non-rotating** | **L6 → L7** ramp + sunburst · **33.2%** | Sunburst **only** | Sway continues. Reward objects grow in at **1.4×** with overshoot, then **settle as scenery** (→ `keystone`) |
| **Menus** | Base rig × **0.90** | Nothing | **×0.85 value** · **×0.80 saturation → 26.6%** — *the room rests. Never greys* | **Visible horizon band** — the one state where it appears | Sway continues. The creature breathes; a child who opens a menu never left the pet |

#### 6.7.2 Per-state notes on what the world is *for* in that state

**First Open / Greeting — the world is a floodlight.**
§2.2's exclusive signature variable is **absolute brightness**, and it is the one
state where the world is the brightest thing in frame on purpose. Fog to 85%
near-white erases the ground ramp almost entirely, so the child sees a warm white
wash with a sun in it and a creature in front of it. The horizon is low and the
camera sits slightly below the creature's eyeline, so the world **looks up at the
child** and the child looks down into a place that is bigger than them. The 40 motes
are the only thing in the entire product that is *in the air*, and they exist for
2 seconds. **Greeting intensity scales with absence length** — the longest, brightest
version goes to the child who has been away longest, and no word is read.

**Idle Ambient — the world does nothing, and that is its function.**
The baseline. 2.4:1 is the most legible *normal*, and the world's job is to be the
thing the child's return looks the same as. The camera does not move, breathe, or
drift, and the light does not change. **Only the creature changes** — a lean into the
finger, squash, ears flattening, eyes tracking. §2.3: *"The world stays absolutely
still so the child's touch is the only event in frame."* This is the state a child
spends the most time in, and it is the one state in which the world is *provably*
silent.

**Painting — the world steps back so the paint steps forward.**
§2.4's exclusive signature variable is the **environment saturation drop** — *no other
state desaturates the world.* Three things happen at once: value steps **−1 rung**,
saturation steps **×0.75 → 24.9%**, and **every prop sway stops.** The value step
pushes the ground's L6 toward L5 and the sky's warmth comes forward, so the frame's
depth flattens; the saturation step is the literal statement that the paint is now
the most colourful thing on screen. Alongside it runs a low wide warm **worklight band
tracking the finger from above** — that is a rig light (§2.1), not an environment
light, and the world does not author it. The camera pushes in 12% over 0.6 s and then
holds perfectly still: **the camera is a fixed easel.** Full mechanism at 6.3.6.

**Signature Mark Flourish — the world drops away.**
Value **−2 rungs**, the largest environment step in the product, and it is the correct
size: the Flourish is the one moment where the child's authorship must be the
brightest, largest, and most ceremonial object in frame, and the only way to achieve
that is to remove the room. The world's own ratio stays 1.35:1 — a uniform step
preserves it — so the 3.5:1 the state achieves is **creature and mark against world**,
which is exactly where §2.1's ceiling should be spent. The key rotates +15° to a
three-quarter back-light, sweeping one highlight across the ground. The rim is ×2.0,
**on the creature only.** The world's role in the 2.5 seconds the child is meant to
film is to be a warm, dim, soft thing the halo reads against.

**Minigame — the world is a studio backdrop, and this is the one state that
flattens it.**
§2.6 makes the explicit art-direction call: *"here, and only here, mood is sacrificed
to legibility. The patterns on the creature's body **are** the game … The framing is a
**morning-lit studio, not an arena.**"* The world's response is a **value push** — the
ground's value rises toward the sky's across the mid-field, so the props are left as
**silhouettes on a matched field** and stop competing with the body patterns. The
optional **radial softening** is applied to the ground's value ramp only; **prop
silhouette edges stay hard.** That constraint matters, and here is why: the prop
silhouettes are the world's only *information* (§6.2.1 — the child learns none of them
by name, but recognises all three instantly), while the ground's value ramp is
*decoration*. **Blurring decoration is free; blurring information is a defect.** A
bright circular **stage pool** — **radius 1.4 H, +1 rung at centre, tapering to 0 at
the rim, in the ground material** — puts an even field of light under the creature so
the whole painted surface is readable. The stage pool is **not a spotlight**: it is a
value term in the ground shader, 0 triangles, 0 draws, and it does not count as a
light. Value ratio **2.2:1** — flat and even, the honestest light in the product. The
world is warm, and it is quiet, and it is not looking.

**Results / Reward — the world is symmetry.**
§2.7's exclusive signature variable is **compositional symmetry**, and Results is the
only centred, symmetrical frame in the product. The world's contribution is a **slow,
wide, radial warm sunburst behind the creature** — soft, symmetrical,
**non-rotating**, in the dome material, 0 triangles, 0 draws. Symmetry means "settled,"
and it appears only here, which is what makes it read as finality. Value ratio
**2.0:1** — soft and enveloping, the second-softest frame in the game. The world is
allowed one arrival in this state and one only: a reward object **growing into frame
at 1.4× with overshoot, always toward the light source**, which then **settles into
the world as scenery** — at which point it is a **`keystone`** at **L5 Sand, 1.08× its
family base, adjacent to its record card**, per §5.3(d). Nothing else arrives. The
world's other job here is to **hold absolutely still** while that happens, so the one
new object is unmistakably the event. Camera centred, symmetrical, front-on.

**Menus — the world is a diorama under glass, and it dims without ever greying.**
§2.8's exclusive signature variable is **camera angle**: Menus are the only state where
the camera does not point at the creature. It lifts to a **high three-quarter** so the
habitat reads as a diorama, and the creature stays in frame, visible and breathing.
The world's values: **×0.85 value, ×0.80 saturation → 26.6%** — the room rests, and
*the correct way to make a menu quiet is lower value and lower saturation, never
grey.* A menu trending toward neutral violates the identity rule, and §2.8 says so
explicitly. The **visible horizon band** appears here and **only here** — a soft warm
curved strip at the far edge of the ground bowl, Sunlight Amber additive at 0.22, a
shader term in the ground material, **0 triangles, 0 draws**, tapered 0.10 H at centre
to 0.05 H at the frame edges, feathered over 0.04 H, **with no defined edge ever.** It
is the child's stable reference line saying *this is the same place, arranged.* Energy
**1/10** — the lowest in the game.

---

### 6.8 Declined and deferred — the consolidated register

Every refusal in this section, gathered so a reviewer or a later section can check
the whole list at once rather than hunting for the paragraph that owns it.

| # | Declined | Section | Why, in one line |
|---|---|---|---|
| 1 | A room, walls, a ceiling, corners, or verticals | 6.1.2, 6.1.4 | A container implies an outside, and an outside implies somewhere else to be |
| 2 | More than one location, or any location change between states | 6.1.2 | §2.3 — a state that changes the place is a new scene, and new scenes are not built |
| 3 | A day cycle, season, weather, or time-of-day system | 6.1.2, 6.1.4 | A moving sun is a clock, and a clock is time pressure — Pillar 2 |
| 4 | Handmade character carried by albedo, texture, mottling, or decals | 6.1.2, 6.3.4 | It would inject false information into the value field the silhouette reads against |
| 5 | A habitat the child arranges, a room-decor meta, or any inventory | 6.1.4 | `game-concept.md` §2 anti-pillar — it dilutes the Adorn verb |
| 6 | Landmarks, a navigable layout, a minimap, or a place name | 6.1.4 | A landmark is a destination, and *not being somewhere* is the feeling Pillar 2 forbids |
| 7 | A fifth geometry class or prop family | 6.2.1 | §1.3 / §1.4 Principle 3 — new geometry means the answer is no |
| 8 | A hollow arch, or any arch with a walk-through opening | 6.2.4 | A visible passage is a limit, and a limit reads as *not allowed* |
| 9 | Arch span above 0.44 H | 6.2.4 | Must stay narrower than the creature's own 0.62 H width, or it can be read as a doorway |
| 10 | An arch, or any prop, that frames the creature | 6.2.4, 6.2.6 | A frame makes the creature a specimen rather than a friend |
| 11 | A `tuft` built from blades, strands, cards, or alpha cutouts | 6.2.5 | A fibre system is fur, and fur is forbidden everywhere including the world |
| 12 | An even scatter, or props in pairs | 6.2.6 | An even scatter is machine-made; children read spawned patterns as such |
| 13 | A fully-populated field with no empty arc | 6.2.6 | A full field reads as a space with a limit |
| 14 | Any prop within 0.90 H of the creature, or between camera and creature | 6.2.6 | Breaks the silhouette read that §3.5 rule 3 and §2.1 law 3 depend on |
| 15 | Clouds, sun rays, god-rays, lens flare, aurora, stars, or banding on the dome | 6.3.2 | Each is a second bright thing, and §2.1 law 2 caps the frame at 3.5:1 |
| 16 | A permanent horizon band | 6.3.3 | It would rob Menus of a signature and weaken §2.9's separability proof |
| 17 | Any tiled, repeated, or decal texture anywhere in the world | 6.3.4 | A visible repeat is a legible pattern, and a legible pattern competes |
| 18 | Vertex-colour hand-painting of the ground | 6.3.4 | 6.1.2 decision 4 — handmade lives in placement, and this would not survive 6-bit output |
| 19 | Any mote using a Tier-0 paint hue, or any mote flicker above 3 Hz | 6.3.5 | A red mote reads as an error pixel; 20 Hz flicker stutters on a mid-range panel |
| 20 | Any residual world motion during Painting | 6.3.6 | §2.4 — the stillness *is* the effect, not an absence of effort |
| 21 | A second light, a second sun, or any environment-authored light | 6.4.1 | §2.1 — one rig, states modulate it, states do not author lights |
| 22 | A rim term on any world surface, or Rim Rose / Mint Halo as a world colour | 6.4.3 | §4.1 hard-limits both; a rimmed pebble field puts 48 focal points in the frame |
| 23 | A cool Mint Halo world rim, at any intensity | 6.4.3 | It would spend the product's only permitted 150°–200° hue on terrain instead of the creature's feet |
| 24 | A hard terminator, gloss above 0.02, metalness above 0.0, or a Fresnel edge | 6.4.4, 6.4.5 | Hard lines read as walls; a moving highlight is a second bright thing |
| 25 | A prop cast shadow, or enabling the environment's shadow-casting "for flexibility" | 6.4.6 | §3.5 rule 1 — the eye follows shadows, and there is exactly one |
| 26 | A new world colour; a new hex; raising a prop's saturation to stand out | 6.5.5 | §4.4.4 rule 3 → **ADR**. A tint is an existing rung plus operations 1–3 |
| 27 | A second world material, a lightmapped scene, light probes, or reflection probes | 6.5.6, 6.6.4 | A baked rig cannot follow §4.6's seven state tables, and it would break 6.3.4's zero-texture rule |
| 28 | An environment LOD group | 6.6.3 | A pop is a hard line, and the world is 8.7% of the budget — it would save nothing |
| 29 | Merging the four prop families into one mesh to save 3 draw calls | 6.6.2 | Costs a prop system, a pipeline, and a shader variant to save 2% of a budget we use 6% of |
| 30 | A collider hierarchy or any solid world object | 6.6.4 | The world is visual. A collider is a gameplay affordance nobody asked for |
| 31 | A collectible, resource, currency, or countable object | 6.2.7 | `game-concept.md` §2 — no currency, no rarity, no collection pressure |

**Three things this section deliberately does not specify, and who owns each:**

| Not specified here | Owned by | Why it is not an art-bible decision |
|---|---|---|
| **Level progression, unlocks, zone gating, or anything about where the child is in a journey** | Level design / systems GDDs | Visual language only. §2 already forbids locked areas outright, so there is nothing to art-direct |
| **Reward economics, how many rewards exist, or what triggers one** | Reward GDD | This section specifies the keystone's *visual envelope* only (1.08×, L5 Sand, one instanced batch, adjacent to its record card) per §5.3(d) |
| **The creature's own behaviour in any state** | §5.6, already approved | Referenced throughout so the world's deltas can be stated as deltas. No §5 number is changed here |

---

### 6.9 Reviewer checklist

**Open the file, screenshot the game, and work down this list. A checked box is a
verified observation, not an opinion.**

#### 6.9.1 The closed set

- [ ] Exactly **four** geometry classes are present: ground bowl, sky dome, sun disc, rounded props
- [ ] Exactly **three** prop silhouettes exist in the world, plus `keystone` — and `keystone` **never** appears as scenery
- [ ] No prop exceeds **0.6 H** in any state, at any camera angle — including a minigame layout
- [ ] The **creature's head is the highest point in frame** in all seven states, in every framing
- [ ] Exactly **one creature** is on screen

#### 6.9.2 Nothing can go wrong

- [ ] There is **no door, gate, fence, wall, maze, pit, ladder, stair, cliff, ledge, terrace, or boundary** anywhere
- [ ] No arch has a walk-through opening, and no arch span exceeds **0.44 H**
- [ ] No prop sits between the camera and the creature's silhouette, and none is within **0.90 H** of the creature's centre
- [ ] No prop is collectible, countable, movable, or lockable
- [ ] Point at the screen and ask a child *"does anything here look like a place I can't go?"* — the answer is no

#### 6.9.3 The child's paint owns the frame

- [ ] Eyedrop the frame, sort by HSV S descending, read the top five: **all are Tier-0 paint, a signature mark, or a flourish reward**
- [ ] No non-Tier-0 pixel exceeds **40.0% HSV S**; the environment's own peak is **37.3%**
- [ ] The environment in **Painting** is at **24.9%** — visibly quieter than the paint on the creature beside it
- [ ] Remove all colour (greyscale): the creature, its painted marks, and the three prop silhouettes are **all still distinguishable**
- [ ] The creature's **eyes are the brightest non-painted pixels** in the frame
- [ ] Reading order holds: **creature → painted mark → active control → saved records → props → nothing**

#### 6.9.4 Light

- [ ] **One rig.** No state, prop, or material has introduced a second light source
- [ ] Light comes from **behind and above**, and the **sun disc is visible in-world** at the key's azimuth 130° / elevation 50°
- [ ] **No world surface carries a rim term.** Rim Rose `#FFB0D0` and Mint Halo `#BFF3E2` appear on the creature's silhouette and **nowhere else**
- [ ] Terminator is **soft** on every world surface — 0.45 wrap, ≥ 0.35 band, no resolvable edge
- [ ] Every shadow in frame is **warm**. No black, no grey, no cold blue
- [ ] No world surface falls below **25% of its base rung's luma**; the world's actual floor is **L5 Sand at 66.0%**
- [ ] No state exceeds a **3.5:1** value ratio; the world's own internal ratio is **1.35:1** in all seven
- [ ] Gloss is **0.02** and metalness **0.0** on every world material. Nothing in the world is reflective

#### 6.9.5 Shadow

- [ ] There is **exactly one cast shadow** in the game and it is the creature's
- [ ] That shadow is a **soft projected blob** at **0 triangles** and **0 draw calls**, tinted **Peach Shadow `#F6B9A0` at 0.28**
- [ ] Props ground with **contact occlusion at 0.15**, and it is visibly lighter than the creature's
- [ ] **No prop casts a directional shadow.** If any does, it is a reject
- [ ] The environment's **shadow pass is empty** — no shadow map is rendered for world geometry
- [ ] Contact occlusion and the creature's blob are distinguishable in **greyscale** (0.13 opacity apart, plus different form and size)

#### 6.9.6 Ground, horizon, and the absence of an edge

- [ ] **The horizon has no hard line.** The join measures **≤ 1.20:1** contrast; spec says **1.03:1**
- [ ] There is **no visible edge of the world** — the child cannot point at where the ground stops
- [ ] The sky dome's rim is never in frame
- [ ] The camera **never pulls back** far enough to see the bowl as an object
- [ ] The world is **not tiled** — no repeat, no decal, no vertex-colour painting, no normal map
- [ ] The ground is **L6 beneath the creature**, giving a **12.4% luma separation** between the creature's feet and the ground
- [ ] The Menus horizon band appears **only** in Menus, and has **no defined edge**

#### 6.9.7 Stillness during Painting

- [ ] **All prop sway is off** in Painting — no residual drift on any prop
- [ ] Sky gradient, ground ramp, and sun disc are **static** in Painting
- [ ] **Exactly three** things move in frame during Painting: the finger, the wet paint settling, and the creature breathing at **5.2 s**
- [ ] Nothing in the world is animated in **any** state for the purpose of indicating, highlighting, or rewarding

#### 6.9.8 State separation

- [ ] Each state's world treatment matches the §6.7.1 table exactly — no ad-hoc world value in any state
- [ ] Greeting is **one full stop brighter** than every other state
- [ ] **Minigame** is the only frontally-lit state, and it is warm anyway
- [ ] **Menus** dims via lower value and lower saturation and is **obviously still warm** — not grey
- [ ] **Results** is the only centred, symmetrical frame in the product
- [ ] Desaturate any two states to greyscale: they remain distinguishable

#### 6.9.9 Material and shape discipline

- [ ] **No fur, fibre, hair cards, strand cards, shell layers, or alpha cutouts** anywhere in the world — including the tufts
- [ ] No vertex in any prop reads as a **corner**; all corner radii ≥ 35% of the shortest local axis
- [ ] No prop has a **cone, spike, blade, or point** tip
- [ ] Every prop is identifiable by **silhouette alone at 64 px in greyscale**
- [ ] No world material introduces a **new colour**. Any tint is an existing rung plus operations 1–3 in §6.5.5, or it is an **ADR**
- [ ] No world surface hue falls outside **17° – 42°**; no world surface is at or below **5.0% HSV S**

#### 6.9.10 Budget

- [ ] Environment totals **26,216 triangles** and **7 draw calls**
- [ ] Creature + environment ≤ **12.9%** of the 300k preferred triangle budget and **≤ 6.0%** of the 150 preferred draw budget
- [ ] No prop prefab carries an **LOD group**; the world has no geometry LOD
- [ ] No lightmap UVs, light probes, or reflection probes in the environment
- [ ] The world uses **2 shader materials** against the 200-variant ceiling
- [ ] **Peak visible triangle count < 300k verified on mid-range Android**, per `AGENTS.md` §5
- [ ] **60 fps** sustained, never below 30

---

## Section 7 — UI/HUD Visual Direction

**Candy Sunrise, expressed flat.** This section is the bridge between §3's shape
language and §4c's type system. It adds no new colour, no new font, no new shape,
no new verb, and no new state. It states, in numbers a reviewer can check, what
those four sections look like when they are combined into seven screens and one OS
notification.

**Canonical inputs this section is bound by:** §1.2 Binding Encoding Law · §2.1–§2.10
the seven states and their signature variables · §3.4 UI Shape Grammar · §3.5 Hero
vs. Supporting · §4.5 Value & Contrast · §4.7 UI Palette & Divergence (the two
divergences, the eleven tokens) · §4c.3–§4c.8 Typography (frozen) · §5.4 Warm Settle
Law · `design/gdd/game-concept.md` §2 Pillar 4 *Two Tools, Both Toys* and Pillar 2
*Nothing Can Go Wrong*.

**Scope note.** This section governs *visual and interaction language*: what a
control looks like, what it says, how big it is, what it does when touched, and what
it may never look like. It does **not** specify a UI framework, a layout engine, an
anchoring scheme, a canvas-scaler configuration, an event graph, a state machine,
or a code structure. Those are the `technical-artist`'s and `ui-programmer`'s
decisions. Where this section needs a number to be *checkable*, it states the
number and the visual consequence, and stops there.

---

### 7.1 UI stance

#### The stance, in one sentence

> **The UI is the world's own vocabulary, rendered flat and calm, and it is never
> louder than the child.** A control is a world object that has been pressed into a
> sheet of paper. A label is a caption someone wrote next to it. Nothing in the UI
> is a second idea about what a thing is.

This is §3.4's argument ("echo the world, flattened") applied at the screen level,
and §3.5's "the child's authorship more visible" applied as composition. It is not
a HUD in the sense that word is usually meant — there is no persistent readout
strip, no resource bar, no objective marker, no minimap, no quick-select rail. What
persists across play is **the creature, one card, and a world that is doing nothing.**

#### 7.1.1 The reading order, as visual law

The order is fixed and it is the composition brief for every screen in this product.

| Rung | Element | Granted | The measurable that puts it here |
|---|---|---|---|
| **1** | **The creature** | The frame's only cast shadow, only rim light, only overshoot, highest value contrast. §3.5 rule 4: its eyes are the brightest non-painted element in frame. | Mean luma of the creature region exceeds every other region by **≥ 16%** (§4.3 "colour-independent" band). C8: additive luma **≥ 8%** on the silhouette edge. |
| **2** | **The flourish** — the child's marks, and the reward flash at its 2.5 s peak | Tier-0 saturation, hero-tier rim and overshoot **transiently only** (§3.5 rule 2) | The paint is the only thing in frame permitted above **40% HSV S** (§4.4 tier 0). At the Flourish it is the brightest object in frame (§2.5 light-source inversion). |
| **3** | **The active control** | 1.4× scale, one rung of elevation, thumb-zone position. **Fill contrast: none.** | C5: luma delta **16.1%**, carrier is size (primary) + value (primary) + position (primary). §4.7.3 holds `--ui-active` on `--ui-shell` at **1.91:1** *deliberately low*. |
| **4** | **The world** | Props, palette ground, ground bowl, horizon band. Lowest contrast in the frame. | Environment ceiling **40% HSV S** (§4.4 tier 3, actual 33.2%). No surface below **L3 41.6%**. |
| **5** | **The inactive control** — records, menu, the parent's knot | Drawn, quiet, reachable, never competing. | No elevation rung above the card it sits on. Never 1.4× scale. |
| **6** | **Nothing** | — | There is no sixth element. A screen that needs one is a screen with a feature the game does not have. |

**The mapping, stated once so §3.5's six-rung chain and this five-term spine cannot
drift.** §3.5 reads `creature → painted mark → active UI target → saved works on
record → props → nothing`. This section folds *"saved works on record"* into
**rung 5, inactive control**, because a record is a control you open, not something
the eye should land on. Nothing else is re-ordered. Where the two lists appear to
disagree, this table is the tie-break and §3.5 rule 5 ("the eye lands on the
creature, then the child's marks, then the reachable control, then nowhere") is the
sentence both are quoting.

#### 7.1.2 The two named violations

These are the two failures this section exists to prevent. Both are **hierarchy
failures, not layout failures**, and both are measured, not argued.

| # | The violation | Why it is a hierarchy failure | The numeric test that catches it |
|---|---|---|---|
| **V‑A** | **A UI panel with stronger value contrast than the creature.** | The panel becomes the subject. The child looks at the game instead of at their pet. It is the single most common way a warm, calm UI goes wrong: panels get raised because they are "hard to see", and every raise is a demotion of the creature. | **(a)** Any UI fill against the field immediately behind it must be **≤ 1.91:1** — the figure §4.7.3 already declared deliberately low. **(b)** No UI surface may sit below **L3 Umber, 41.6%** (§4.5 Rule V1). **(c)** Measure the frame's three regions (creature / world / UI): the creature must hold the **maximum** local value contrast in every frame, in every state. A card that fails (a) or (b) is a defect regardless of how good it looks. |
| **V‑B** | **A reward popup brighter than the flourish.** | Two centres of gravity in one frame. The frame stops being a moment and becomes an interface. It is also a Pillar 1 violation: the reward was authored, and an authored thing should not need a popup to announce itself. | **There are no reward popups.** §2.7: rewards are objects that *grow into frame*, toward the light, at 1.4× with overshoot. The only card permitted at a reward moment is a **ribbon stamp on `--ui-surface` (75.8% luma)** — which is **16.1% luma below** Dawn Cream, so the creature out-reads the stamp by construction. **Greyscale screenshot at the Flourish peak: the mark must be the brightest object in frame. Any UI pixel brighter than the mark's local luma is V‑B.** |

A third, quieter failure is worth naming because it is the one that actually shows
up in review: **V‑C, the dock that grows.** A card that gains a row, a badge, a
count, or a notification dot because a feature was added has pushed rung 3 above
rung 2. The hub card is **624 px tall at both type tiers** (§7.3.4). It does not grow.
If a feature needs more room than 560 px, the feature is cut, not the card.

#### 7.1.3 Three standing commitments

1. **Icon-first, text-supportive.** Text is never the sole content of a control, and
   never load-bearing (§3.4, `AGENTS.md` §2.5). Remove all text from any screen and
   every control remains identifiable by silhouette and by what happens when touched.
2. **One shape vocabulary, one learning event.** Every UI silhouette is a silhouette
   that already exists in the world — a pebble, a card, a ribbon, a shell, a frill.
   No chevron, no arrow, no gear, no hamburger, no X, no checkmark, no magnifier.
3. **The child never sees an adult surface, and never sees a problem.** No lock
   glyph, no grey, no warning, no exclamation mark, no timer, no count, no score,
   and no control that has ever been unavailable for a reason the child could
   understand (§7.7, §7.10).

---

### 7.2 Screen inventory

One row per §2 state, plus the OS notification. **"What's deliberately absent" is
the load-bearing column** — it is where Pillar 2 stops being a principle and
becomes a layout.

| Screen / state | Purpose | On it | **Deliberately absent** | Primary child action |
|---|---|---|---|---|
| **First Open / Greeting** (§2.2) | *"You're here. I noticed."* Recognition as a gift, with zero reference to elapsed time. | Creature filling the lower two-thirds, the hub card, 40 warm motes | No "welcome back" copy, no day count, no streak, no "while you were away" ribbon, no modal, no tutorial overlay, no achievement | **Touch the creature** — the petting gesture, which is also the only thing the Greeting asks for |
| **Idle Ambient** (§2.3) | *"I'm not going anywhere."* The rest state. The one moment that asks for nothing. | Creature, world, hub card, nothing else | No badges, no "new" dots, no prompts, no notifications, no ambient music loop with a seam, no idle animation on the card | **Touch the creature**, or open PAINT |
| **Painting** (§2.4) | *"I made this."* Creative Flow peak. Paint appears slightly **before** the finger. | Creature pushed in 12%, palette card, 12 swatch faces, the selected swatch raised, PLACE, UNDO, one caption plate | No numeric colour picker, no hex field, no eyedropper, no brush-size slider, no opacity slider, no symmetry tool, no layers panel, no modal dialog, no confirm step, no filename, no autosave indicator with a timestamp | **Drag a stroke on the creature** — and tap works identically (§3.4 drag/tap parity) |
| **Signature Mark Flourish** (§2.5) | *"Watch what I gave you."* The 2.5 s the game is built around. | The mark, its travelling mark-light, the fixed spark burst, one Display-tier ribbon stamp, the camera's 8° orbit | No unlock banner, no "ability learned" popup, no XP, no stat card, no streak counter, no count of flourishes, no camera cut, no slow-motion on a tap | **Watch** — 2.5 s, then the stamp settles and the flourish decays over 3.0 s |
| **Minigame** (§2.6) | *"I can do this."* The competence channel. | Creature frontally lit, the prompt pebble (`Look` → `Match`), the pattern to match, the fixed 12-spoke burst on success | **No timer, no countdown, no progress bar, no lives, no score, no combo, no streak, no "wrong", no red, no buzzer, no decreasing speed, no leaderboard, no attempt counter** | **Tap or drag to match** — timing is generous and the cadence is matched to the gesture, never to a clock |
| **Results / Reward** (§2.7) | *"This is mine, and it's staying."* Permanence is the feeling. | Centred symmetrical frame, radial warm sunburst, the reward object **grown into frame** at 1.4× with overshoot, the record entry, one ribbon stamp | No score, no stars, no rating, no "new record", no "better than last time", no comparison to a previous run, no share button, no confetti that could read as threat, no popup | **Look at it** — 3–4 s, then it settles into the world as scenery |
| **Menus** (§2.8) | *"Everything is here, nothing is urgent."* A pause button, not a form. | High three-quarter diorama, the horizon band, BACK, the MENU pebble held for the parent gate | No settings list, no tabs, no scroll, no fourteen-option grid, no "About", no version string, no social, no account, no news feed — a menu with more than four items is a different product | **Nothing.** It is a pause. The child can leave by tapping the creature |
| **OS Notification** (§7.9) | Tell an adult the app exists **without creating obligation**. | The creature's silhouette as the OS icon, a payload of ≤3 words / ≤24 characters | **No badge count, no "!", no image of the mark, no "your pet is waiting for you", no streak reminder, no action label promising a reward, no red, no urgency of any kind** | **None required.** Tapping opens the app, which greets brighter the longer they were away — so the notification is a gift and never a debt |

#### 7.2.1 The seven screens are one screen

A state change is a **re-composition**, never a transition. The card's left edge,
its width, and its distance from the bottom safe edge are identical in all seven
states. Only its height, its contents, and the creature's framing change.

| Law | The rule | Why it is a hierarchy rule |
|---|---|---|
| **T‑1** | **Nothing slides, wipes, or flies in.** No element enters from off-frame. | An arriving element is a demand on the eye, and the eye belongs to the creature. |
| **T‑2** | **At most one card at a time.** Records, palette, and menus are the same card at different heights. Two cards is a dialog, and a dialog is a different product. | Keeps rung 3 singular. Two cards create two competing "active" reads. |
| **T‑3** | **The creature never leaves the frame.** Only its framing changes (§7.6.3). | The creature is rung 1 by definition; a creature that can be off-screen is not. |
| **T‑4** | **Every state change completes in ≤ 400 ms, ease-out, no overshoot.** The one exception is the flourish orbit (§2.5). | A slow transition is a transition the child waits through. Waiting is a problem. |
| **T‑5** | **No element appears except in direct response to a touch.** PAINT's touch brings the palette; the palette's arrival brings UNDO and PAGE with it. Nothing appears on a timer, on a threshold, or on returning to the frame. | An element that appears unprompted is a notification, and notifications are §7.10's business, not the child's. |
| **T‑6** | **The card's footprint is declared before its contents are designed.** Every state has a declared height at both text tiers (§7.3.4). Contents that do not fit are cut, not shrunk below 48 px, and not scrolled. | A card that grows to fit its contents is a card that has been given a badge. That is V‑C. |
| **T‑7** | **Transitions may not scale the card or the camera.** Only the creature's framing (§7.6.3) and the flourish orbit (§2.5) move the frame. | Zooming the UI is the same gesture as emphasising the UI. |

---

### 7.3 The card, and only the card

#### 7.3.1 The container vocabulary

Three containers, and no fourth. Every one is a shape that already exists in the
world (§3.4), every one has **zero sharp corners**, and every one honours §1.3's
**minimum 35% corner radius** — which is §3.4's stricter floor, not a softening of it.

| Container | Size (px @ 1080×1920) | Radius (px) | Radius rule satisfied | Used for |
|---|---|---|---|---|
| **Pebble** | 216 × 216 | 108 | fully round (50%, the target, not the floor) | every child-touchable control |
| **Pebble, active** | 302 × 302 | 151 | fully round | the one selected control |
| **Swatch pebble** | 144 × 144 | 72 | fully round | the twelve colour faces |
| **Card** — hub | 936 × 624 | 219 | 35.1% of 624 | PAINT · PLACE · PLAY · RECORDS |
| **Card** — palette | 936 × 648 | 227 | 35.0% of 648 | swatches · UNDO · PAGE · caption plate |
| **Card** — records | 936 × 1296 | 328 | 35.0% of 936 | twelve record faces · BACK |
| **Card** — menus | 936 × 552 | 194 | 35.1% of 552 | BACK · the parent's held knot |
| **Ribbon** — flourish | 936 × 168 | 59 | 35.0% of 168 | the one Display-tier stamp |
| **Ribbon** — results | 936 × 144 | 51 | 35.4% of 144 | the one stamp in Results |
| **Caption plate** | 640 × 96 | 34 | 35.4% of 96 | the one sentence of running text |
| **Record face** | 264 × 264 | 93 | 35.2% of 264 | one saved mark |

**The palette card is the only surface permitted to be taller than the hub card,
and it grows *upward into sky*** — never downward over the thumb, never upward over
the creature (§7.6.3). If a future feature needs a fourth card shape, the shape is
refused; the feature is cut.

**Grid, global.** Content width **936 px**, side margins **72 px** (72 + 936 + 72 =
1080). **Padding inside every card: 48 px.** **Gap between sibling pebbles inside a
card: 24 px minimum, and never less than 0.4 × the pebble diameter** — see §7.3.4
for why that number exists.

#### 7.3.2 The hub card — the only four-way choice the child ever makes

Interior 840 × 528 at (120, 1268). Two rows, two columns, of 216 px pebbles.

```
  PAINT        PLACE          ← row 1   y 1268–1484
  PLAY      RECORDS           ← row 2   y 1580–1796
  col 1 x 204–420   col 2 x 660–876     ← horizontal gap 240
                                        ← vertical gap 96
```

| Position | Control | Why it is in that cell |
|---|---|---|
| top-left | **PAINT** | The default verb. Top-left is the first cell a scanning eye lands on in a Latin layout, and PAINT is the verb everything else modifies. |
| top-right | **PLACE** | PAINT's second mode. Adjacent, so the relationship is spatial before it is semantic. |
| bottom-left | **PLAY** | The third thing. Bottom-left, above the thumb's rest. |
| bottom-right | **RECORDS** | The child-made object. Furthest from the eye's entry, which is correct: the child looks at the creature, not at their archive. |

**Every pebble is in the thumb zone.** All four occupy y ≥ 1268, below the y = 1152
line that separates the thumb zone (bottom 40% of the frame) from the adult zone.
There is no fifth cell, no centre cell, and no cell in the bottom 40% corner that a
resting thumb can cover by accident.

#### 7.3.3 The palette card — six colours, one sentence, two controls

Interior 840 × 552 at (120, 1244). Card top y = 1196; bottom edge y = 1844.

```
  ┌─ 936 × 648 · radius 227 ───────────────────────────────────────┐
  │  swatch col 1   col 2   col 3        │  UNDO   216              │
  │  120–264       336–480  552–696      │  744–960 · y 1244–1460   │
  │  ─────────────────────────────────    │                        │
  │  row 1  y 1244–1388                  │  PAGE   216              │
  │  row 2  y 1460–1604                  │  744–960 · y 1484–1700   │
  │                                       │                        │
  │            caption plate 640 × 96     │                        │
  │            220–860 · y 1664–1760      │                        │
  └───────────────────────────────────────┴────────────────────────┘
```

| Element | Placement | The number that makes it legal |
|---|---|---|
| **Swatch pebbles ×6** | 3 columns × 2 rows, **144 px** diameter, **72 px** gaps | 144 px = **exactly 48 dp** at the reference resolution — the absolute floor, never below it. |
| **Selected swatch** | rises to **202 px** (1.4×), radius 101 | Overflows its slot by **29 px per side**; the 72 px gap leaves **43 px of clear space** to the neighbour. **Minimum legal gap for a 1.4× rise on a 144 px pebble is 58 px.** If a future grid drops below that, the swatch loses the rise — never the gap. |
| **UNDO** | right column, y 1244–1460 | Icon-only (§7.8.1). No label. |
| **PAGE** | right column, y 1484–1700 | Icon-only, and **one** pebble, not two — see D5. |
| **Caption plate** | 640 × 96, centred, y 1664–1760 | The only running text in Painting. The raised swatch in row 2 reaches y 1633; the plate starts at y 1664 — **31 px of clearance**, so a selected swatch never touches the sentence. |
| **Colour faces** | 12 across two pages, 6 per page | Every face carries a **4 px `--ui-keyline`** ring. The keyline *is* the boundary; nothing else defines the swatch's edge. |

**Every swatch face is a Tier-0 paint at full strength.** The card is the one place
where saturation above 40% HSV S is not an exception but the point (§7.5.3).

**No slider, no picker, no field, no dialog.** The card has room for a colour
*identity*, not a colour *parameter*. A child who wants a different colour taps a
different pebble. A child who wants the one they were using taps the raised one again.

#### 7.3.4 The card does not grow

This is the rule V‑C is built on, stated as a testable constraint.

| Constraint | Value |
|---|---|
| Hub card height at type tier 1.00 | **624 px** |
| Hub card height at type tier 1.40 | **624 px** |
| Palette card height at either tier | **648 px** |
| Records card height at either tier | **1296 px** |
| Menus card height at either tier | **552 px** |
| Label box behaviour at tier 1.40 | **width fixed; overflow wraps to a second line; the box grows; the card does not** |
| Rows the hub card may hold | **exactly 2** |
| Rows the palette card may hold | **exactly 2** |
| Record faces the records card may hold | **exactly 12** (3 × 4 — see §7.6.4) |
| Badges, dots, counts, ribbons inside a card | **0, always** |

A card is a fixed rectangle the way a sheet of paper is a fixed rectangle. If a
feature needs a thirteenth record face, a fifth hub control, or a fifth palette row,
**the feature is cut or the record moves into the world as scenery** — the card is
never the thing that gives. This is §3.5 rule 4 (*the child's authorship more
visible*) enforced as geometry: the world is where authored things accumulate.

---

### 7.4 Records, ribbons, and the two stamps

#### 7.4.1 The records card

Interior 840 × 1200 at (120, 596). Card spans y 548–1844.

```
  col 1  x 120–384      col 2  x 408–672      col 3  x 696–960     (gaps 24)
  row 1  y 596–860      row 2  y 908–1172     row 3  y 1220–1484   row 4  y 1532–1796   (gaps 48)
  BACK   x 432–648 · y 1628–1844                                            bottom band
```

- **Twelve record faces**, 264 px square, radius 93, laid out **3 × 4** with 24 px
  column gaps and 48 px row gaps. 264 px = **88 dp** at reference, 58.7 dp at
  720 × 1280 — comfortably above the floor.
- **A record face shows one saved mark, and nothing else.** No date. No index
  number. No "new". No frame, no mat, no border beyond the mark itself. A record is
  the child's drawing; a card around it would be an inventory.
- **The card's only text is `Your marks`**, centred, in the top 96 px band
  (y 596–692) in `--ui-text-primary` at 48 px, weight 700. It is a caption someone
  wrote, not a title card.
- **BACK** is a 216 px pebble centred in the band below the fourth row, icon-only
  (§7.8.1), at the thumb-zone line.
- **No scroll.** Twelve is what this card draws. Records beyond twelve are a
  `game-designer` decision; §7.10.1 records it as open, and this section does not
  invent a thirteenth slot, a page, or a scroll bar.

#### 7.4.2 The flourish stamp

- **Ribbon, 936 × 168, radius 59.** x 72–1008, **top-aligned at y 1220** — the hub
  card's reserved top edge, which the flourish **replaces**.
- During the 2.5 s flourish and its 3.0 s decay, **the hub card is absent.** The
  mark is the only thing in the lower frame. This is V‑B made structural: there is
  no card on screen for the stamp to out-shout.
- The stamp carries **the child's own words if the parent recorded any, otherwise
  nothing at all.** An empty ribbon is legal. A ribbon that must be filled with a
  congratulation is a different product.
- Type tier 1.40: the ribbon's reserved text box is 792 × 120, sized to the widest
  five-word phrase permitted, wrapping to a second line inside the ribbon. **The
  ribbon does not grow and the stamp never covers the mark** (mark and ribbon occupy
  disjoint regions; the ribbon's bottom edge is the creature's ground line).

#### 7.4.3 The results stamp

- **Ribbon, 936 × 144, radius 51.** x 72–1008, **bottom edge on the creature's ground
  line (y 1160)**, sitting above the reward object.
- The reward object **grows into frame** at 1.4× with overshoot. It does not pop, it
  does not slide, and it is not in a box.
- **No score, no stars, no rating, no comparison, no "new record", no confetti.**
  The frame is symmetrical and radial — a sunburst, not a leaderboard.
- Type tier 1.40: reserved text box 792 × 96, five-word cap, one line preferred,
  wraps to two inside the ribbon. **The ribbon does not grow.**

---

### 7.5 UI colour, value, and elevation

#### 7.5.1 The eleven tokens, and nothing else

| # | Token | Hex | Luma | HSV S | Used in this UI for |
|---|---|---|---|---|---|
| 1 | `--ui-shell` | `#BEA88B` | 66.9% | 26.8% | the page ground behind a card; the world's own shadow under a card |
| 2 | `--ui-surface` | `#DABE98` | 75.8% | 30.3% | every card face — the loudest UI surface in the product |
| 3 | `--ui-surface-raised` | `#E8D5B8` | 84.3% | 20.7% | a card that is itself raised (the parent frill's inner face) |
| 4 | `--ui-active` | `#F7E9D2` | 91.9% | 15.0% | the active pebble's fill; a pressed card face |
| 5 | `--ui-text-primary` | `#4A382A` | 23.1% | 43.2% | every label and body word |
| 6 | `--ui-text-secondary` | `#6B4F3A` | 32.7% | 45.8% | the one supporting caption, only |
| 7 | `--ui-stroke` | `#4A382A` | 23.1% | 43.2% | every icon stroke and control outline |
| 8 | `--ui-keyline` | `#F7E9D2` | 91.9% | 15.0% | the 4 px ring on every swatch |
| 9 | `--ui-focus-outer` | `#4A382A` | 23.1% | 43.2% | the 4 px focus ring |
| 10 | `--ui-focus-inner` | `#FFE3B8` | 90.1% | 27.8% | the 2 px inner focus ring — **carries nothing** |
| 11 | `--ui-unavailable` | `#DABE98` | 75.8% | 30.3% | the transient settle fill (§7.5.4) |

**No twelfth token is authorised here.** `--ui-reward` is declared in art-bible
§4.7.2 as a Tier-0 exception but is **absent from `palette.json`'s eleven-token UI
list**; this section therefore specifies rewards as *Tier-0 paint, at full strength,
drawn as the reward object itself* and creates no token. That discrepancy is logged
as **D6** (§7.10.1) for `technical-artist` and `producer` to reconcile in the machine
palette — it is not resolved by inventing a colour.

#### 7.5.2 The saturation and value budget, as arithmetic

| Test | Number | Consequence if broken |
|---|---|---|
| Max HSV S of any UI **surface** | **33.2%** ceiling; authored max **30.3%** | the UI is competing with the world |
| Max HSV S of any UI **text, stroke, or focus ring** | **48.2%** | the UI is competing with the child's paint |
| Max HSV S of any non-Tier-0 **surface or fill** | **40%** — Tier 2 print masks and Tier 6 UI line work are line art, not surfaces, and are excepted at 48.2% | the paint stops being the loudest thing |
| Least-saturated Tier-0 paint (Petal, 45.3%) × loudest UI surface (30.3%) | **1.49×** | — |
| Least-saturated Tier-0 paint × world ceiling (33.2%) | **1.36×** | — |
| Least-saturated Tier-0 paint (45.3%) × darkest UI ink (45.8%) | **0.99×** | **INVERTED** — Cocoa Ink is *more* saturated than the least-saturated Tier-0 paint. Permitted because ink is line work carrying text, not a filled area, and it stays under the Tier 6 cap; on filled areas the paint still wins (1.49× over UI surface, 1.36× over the world ceiling) |
| UI fill luma band | **66.9% – 91.9%** | a UI fill outside this band is either a shadow or a light leak |
| No UI surface below L3 Umber | **41.6%** | V‑A, clause (b) |
| UI fill vs. field behind it | **≤ 1.91:1** | V‑A, clause (a) |

Declared contrast figures, cited rather than recomputed: **`--ui-text-primary` on
`--ui-active` = 9.28:1**; **`--ui-text-secondary` on `--ui-surface` = 4.21:1**;
**focus ring = ≥ 4.85:1**; **`--ui-active` on `--ui-surface` = 1.49:1, deliberately
low**; **UI fill vs. backdrop = ≤ 1.91:1**.

#### 7.5.3 Where saturation is allowed to break the rules

Exactly three places, and all three are the child's or the game's celebration:

1. **Swatch faces** — Tier-0 paint at full strength. The palette card exists to hold
   saturated colour; a desaturated swatch would be a swatch with no reason to exist.
2. **The reward object** — Tier-0 paint at full strength, during its 2.5 s.
3. **The wet stroke** — Tier-0 paint at full strength, until it dries.

There is no fourth. In particular there is **no saturated UI chrome**: no coloured
button face, no coloured tab, no coloured header bar, no coloured badge. A coloured
control is the most common way a warm UI starts shouting, and it is refused here.

#### 7.5.4 Elevation and the three steps a control can take

| Step | Read | Fill | Size | Ladder rung | Shadow |
|---|---|---|---|---|---|
| **Resting** | present, available | `--ui-surface` | 216 | 0 | none |
| **Active** | *this is the one you chose* | `--ui-active` | **302 (1.4×)** | **+1** | one, soft, below |
| **Pressed** | *you are touching it right now* | `--ui-active` | 302 | +1 | one, tightened |
| **Settling** | *this one is briefly smaller* | `--ui-unavailable` | **190 (0.88×)** | **−1** | none |

**The active state is carried by size, ladder rung, and position — and explicitly
not by fill contrast.** `--ui-active` on `--ui-surface` is 1.49:1, which would be
invisible if it were the only channel. It is not the only channel: the pebble is
**86 px wider**, sits **one rung higher**, and is **where the thumb already is**.

**The settle state has no grey and no hue shift.** `--ui-unavailable` is
**value-identical to `--ui-surface`** (`#DABE98`, 75.8% luma). That collision is the
design: a settling pebble does not *change colour*, it **sinks into the card it
sits on** and becomes a dimple. Two hard rules follow, and both are load-bearing:

- **A settle state may only ever be drawn on a card.** Against the world, a
  `#DABE98` pebble on a `#E8C79B` ground is not a state, it is a mistake.
- **A settle state may never mean *unavailable*.** It means *for a moment, this is
  not the thing to touch*. If a control is unavailable for a reason the child could
  name, it does not settle — **it does not exist** (§7.1.3, §7.8.2).

---

### 7.6 Resolution, framing, and safe areas

#### 7.6.1 The authoring grid and the one scale factor

Every number in this section is authored at **1080 × 1920** and means that many
pixels at that resolution.

| Quantity | Rule |
|---|---|
| `k` | `k = W_logical ÷ 1080` |
| 1 design px | `1 ÷ 3` dp at the reference resolution |
| Everything else | Scales by `k`. **No other scale factor exists.** No per-screen factors, no density tiers, no art-direction-by-device variants. |
| Non-uniform scaling | **Forbidden.** `k` applies to both axes. A pebble is a circle at every width or it is a defect. |
| Corner radius | Scales by `k`. The **ratio** to the container's short edge is what is fixed. |

#### 7.6.2 Hit targets

| Target | Design px | At 1080 × 1920 | At 720 × 1280 | Floor |
|---|---|---|---|---|
| Every child-touchable control | **216** | **72 dp** | **48 dp exactly** | 48 dp — met |
| Swatch pebble | **144** | **48 dp exactly** | **48 dp exactly** | 48 dp — met |
| Record face | 264 | 88 dp | 58.7 dp | — |
| Icon stroke width | **≥ 8** | ≥ 8 px | ≥ 5.3 px | — |
| Focus ring outer / inner | 4 / 2 | — | — | — |
| Smallest legible type | **48** | **16 sp** | 32 px / 10.7 sp | §4c |
| Smallest drawn detail | **12** | — | — | nothing below 12 px is drawn at all |

Art-bible §3.4's nominal **72 px / 24 dp** touch floor is **documented and never
used as an actual target** — it exists as a legacy figure from before the 48 dp
rule was adopted. Any asset delivered at 72 px for a child target is rejected,
not accepted as conservative.

**Every hit area extends to the full 216 px circle**, including the transparent
margin around a smaller drawn glyph. No control's hit area is ever smaller than its
pebble.

#### 7.6.3 Framing: the camera is locked, the sky is not

The creature is the subject, so the camera may not be re-authored per screen. The
lock is **horizontal FOV = 32°**, held constant. Vertical FOV is *not* the locked
quantity — it varies with aspect ratio, and the variation is absorbed entirely by
sky.

| Aspect | Logical frame | Sky band (top of frame → creature) | Creature region |
|---|---|---|---|
| 16:9 | 1080 × 1920 | **524 px** | y 440 – 1160 |
| 19.5:9 | 1080 × 2340 | **944 px** | y 760 – 1480 |
| 20:9 | 1080 × 2400 | **1004 px** | y 820 – 1540 |

**Extra height becomes sky, never scale.** A taller phone sees more dawn and the
same creature at the same size. The creature's **ground line sits 760 px above the
bottom safe edge** and its **projected height is 720 px** in Framing A; the bottom
76 px below the card is world.

**Framing A — rest, greeting, idle, minigame, results.** Ground line 760, projected
height 720.

**Framing B — Painting only.** The creature is **pushed in 12%**: projected height
**806 px** (720 × 1.12), ground line unchanged at 760. It grows upward into the sky,
so the palette card can rise without ever covering the creature's feet, its Mint
Halo grounding rim, or one stroke of the area the child is drawing on. Silhouette
spans y 354 – 1160 in the 16:9 frame; the palette card's top edge is y 1196 —
**36 px of clearance below the lowest point of the creature.**

**Framing C — menus.** A raised three-quarter diorama. The creature is scenery and
may be partly out of frame. The card is the frame's subject but never rises more
than one rung above the horizon band, and the creature is never fully occluded.

**Framing D — the flourish.** Framing A plus the §2.5 8° orbit. The orbit is the
only camera movement in the product.

#### 7.6.4 Safe areas and the reserved bands

| Band | y (16:9 frame) | What may live there |
|---|---|---|
| Top safe edge → +24 | 0 – 24 | **nothing** |
| Adult zone | 24 – 1152 | **MENU only** (D3). This is the top 40% of the frame and it is deliberately hard for a resting thumb to reach. |
| Thumb zone | 1152 – 1920 | every child-touchable control, every card, every ribbon |
| Bottom inset | 1920 → platform inset | **nothing** — cards anchor 76 px above it |

**MENU — the one deliberate exception (D3).** 216 px pebble, radius 108, at
**x 72–288, y 24–240**, icon-only. It is outside the thumb zone because the parent
gate must be *hard to reach by accident*: a five-year-old should not open the
adult surface by drumming the corner of the screen. Every other control obeys the
thumb-zone rule; MENU is the only one that inverts it, and it inverts it for a
safety reason, not a layout reason.

**Bottom anchor.** Every card's bottom edge sits **76 px above the bottom safe
edge** (y 1844 in the 16:9 frame). Card tops therefore follow from height alone:
hub y 1220 · palette y 1196 · menus y 1292 · records y 548.

**The records grid holds twelve faces** — 3 × 4 (§7.4.1). Twelve is this section's ceiling;
overflow is a `game-designer` decision and is logged open in §7.10.1.

---

### 7.7 Typography and labels

#### 7.7.1 The frozen faces

| Face | Licence | Permitted use |
|---|---|---|
| **Nunito** | SIL Open Font Licence 1.1 | **every word in the product's UI**, at weights 400 / 600 / 700 |
| **Patrick Hand** | SIL Open Font Licence 1.1 | **the child's own name, and nothing else.** Where the child wrote their name, this is the hand they wrote in. |

No third face. No icon font. No platform system face inside the child's UI — the
platform system face appears **only** inside the parent gate (§7.10.3), because an
adult surface should look like an adult surface and a child should never learn to
read the OS.

#### 7.7.2 The complete set of words in the child's UI

Six strings. That is the entire vocabulary.

| Where | String | Face / weight | Size @1080 | Reserved box |
|---|---|---|---|---|
| PAINT pebble | `Paint` | Nunito 700 | 48 px / 16 sp | 216 × 264 (label sits **below** the pebble) |
| PLACE pebble | `Place` | Nunito 700 | 48 px / 16 sp | 216 × 264 |
| PLAY pebble | `Play` | Nunito 700 | 48 px / 16 sp | 216 × 264 |
| RECORDS pebble | `Your marks` | Nunito 700 | 48 px / 16 sp | 216 × 264 — the **widest state**, and the box every other pebble's box is sized to |
| BACK pebble | `Back` | Nunito 700 | 48 px / 16 sp | 216 × 264 |
| Records card | `Your marks` | Nunito 700 | 48 px / 16 sp | 936 × 96 |
| Minigame prompt | `Look` → `Match` | Nunito 700 | 56 px / 18.7 sp | 264 × 96, inside a 264 prompt pebble |
| Caption plate | ≤ 5 words | Nunito 600 | 48 px / 16 sp | 640 × 96 (wraps to 2 lines inside the plate) |
| Flourish ribbon | ≤ 5 words | Nunito 700 | 48 px / 16 sp | 792 × 120 |
| Results ribbon | ≤ 5 words | Nunito 700 | 48 px / 16 sp | 792 × 96 |

**Icon-only controls: MENU, UNDO, PAGE, and all twelve swatches.** These carry no
text at all, at any type tier, at any accessibility setting. Their meaning is
carried by silhouette and by what happens when touched — which is the test that
commitment 1 in §7.1.3 sets.

#### 7.7.3 Words that may never appear

| Banned | Why |
|---|---|
| any **count** of anything — strokes, marks, records, colours used | a count is a score the child cannot interpret yet |
| any **timer**, clock face, countdown, or "in 30 seconds" | Pillar 2 — the game never hurries the child |
| any **score**, rating, stars, rank, tier, level, "best" | Pillar 1 — the child authored it; it does not need a number on it |
| **"Try again"**, "wrong", "incorrect", "fail", "oops" | the game has no failure state to describe |
| **"Are you sure?"**, "Cancel", "Confirm", "Delete" | there is nothing in this product to confirm or delete |
| **"Level up"**, "Achievement", "Reward earned", "New!" | rewards grow in; they are not announced |
| **"Loading"**, "Please wait" | §2.3 promises the game is doing nothing |
| **any exclamation mark** | punctuation that raises volume; the volume belongs to the flourish |
| **"Welcome back"**, "while you were away", "days in a row" | §2.2 — the Greeting recognises the child, it does not measure their absence |
| **the OS's own vocabulary inside the child UI** | `Settings`, `Back`, `Home`, `Search`, `OK` — except the single word `Back`, which is the one platform convention we keep because a five-year-old has already met it on a television |

#### 7.7.4 Type tier, and what happens at Large

| Tier | Trigger | Effect |
|---|---|---|
| 1.00 | default | every size in §7.7.2 |
| 1.40 | OS text size set to its largest supported value | **type scales to 1.40. Control width does not change. The label overflows its reserved box, wraps to a second line, and the box grows taller. The card does not grow (§7.3.4).** |

**Clamp: 1.00 – 1.40.** The game reads the platform text size and uses it, bounded.
Above 1.40 the layout would stop being a 2 × 2 grid, and a 2 × 2 grid of 48 dp
targets is the minimum this product can honour. The clamp is surfaced to the parent
in the parent gate (§7.10.3) — the parent sets the size and is told what the child
will see, rather than discovering it.

**Widest-state reservation.** `Your marks` is nine characters; `Place` is five. If
the reserved box were sized to `Place`, the label would re-flow at tier 1.40 and
the pebble's text would appear to move. Every label box in the UI is therefore sized
to **its own widest legal state at tier 1.40**, and the text is bottom-anchored
inside it so that a one-word label sits on the same baseline as a two-word label.

---

### 7.8 Iconography, encoding, and touch response

#### 7.8.1 The closed control set

| Control | Where | Label | Silhouette | Redundant channels beyond shape |
|---|---|---|---|---|
| **PET** | the creature itself — **not a control** | none | the creature | position (centre of frame), value (rung 1) |
| **PAINT** | hub, top-left | `Paint` | a **crayon-lozenge** — a tapered capsule at 20° with a nib notch | size, position, label, the sound it makes |
| **PLACE** | hub, top-right; palette top-left | `Place` | a **pin-badge** — a pebble with one soft nub on its upper edge | size, position, label |
| **PLAY** | hub, bottom-left | `Play` | a **creature silhouette with a forward hook** — the creature's profile leaning into a direction | size, position, label |
| **RECORDS** | hub, bottom-right; records card | `Your marks` | a **ribbon carrying three stacked pebbles** | size, position, label |
| **MENU** | top-left, adult zone — **D3** | none | a **tiered stack of three pebbles**, widest at the base | position (outside the thumb zone), the frill that opens on hold |
| **UNDO** | palette, right column top | none | an **inward curl** — a stroke that turns back on itself | position (right column), the stroke it removes |
| **BACK** | records card, menus card | `Back` | an **open shell** — a pebble with one edge lifted | position, label |
| **PAGE** | palette, right column bottom — **D5** | none | a pebble with a **nub on the leading edge** — nub right on page 1, nub left on page 2 | the nub's direction *is* the page state; **no separate page indicator exists** (D7) |
| **SWATCH ×12** | palette grid | none | a Tier-0 colour face inside a **4 px `--ui-keyline`** ring | the ring, the 1.4× rise, and the paint colour itself |

**No chevron, no arrow, no gear, no hamburger, no X, no checkmark, no magnifier, no
lock, no padlock, no gear, no tab, no toggle, no slider, no dropdown, no badge.**
Every one of those is a control from a different product.

#### 7.8.2 What each control means with no words at all

The Binding Encoding Law (§1.2) requires that colour, shape, motion, size, value,
and label each carry the same information. For this UI that means:

| Control | Shape | Size | Value | Position | Motion | Label |
|---|---|---|---|---|---|---|
| PAINT | crayon | 216 / 302 | surface / active | top-left | the 8° flourish orbit on the mark it makes | `Paint` |
| PLACE | pin-badge | 216 / 302 | surface / active | top-right | a pebble *lands* rather than draws | `Place` |
| PLAY | forward hook | 216 / 302 | surface / active | bottom-left | the creature leans into it | `Play` |
| RECORDS | ribbon + 3 pebbles | 216 / 302 | surface / active | bottom-right | twelve faces, still, until tapped | `Your marks` |
| MENU | tiered stack | 216 only | surface | **top-left, outside the thumb zone** | the frill opens after 0.6 s of holding | *none* |
| UNDO | inward curl | 216 only | surface | right column, top | a stroke curls back and is gone | *none* |
| BACK | open shell | 216 only | surface | centred below content | the previous card re-composes in | `Back` |
| PAGE | leading nub | 216 only | surface | right column, bottom | **the nub rotates 180° when the page turns** | *none* |
| SWATCH | ringed face | 144 / 202 | surface / active | grid position | the selected swatch rises 1.4× and stays | *none* |

**Colour is never a channel in this table, because in this UI there is no colour to
carry meaning.** All twelve UI tokens sit at or below 48.2% HSV S; the loudest one
(`--ui-surface`, 30.3%) is still **1.49× less saturated** than the quietest paint.
The UI is loud in *shape*, and the child's paint is loud in *colour*, and the two
never compete. This is the whole reason §4.7's divergence works.

#### 7.8.3 Tap and drag are the same gesture

§3.4's **drag/tap parity**: a tap on the creature and a drag across it are the same
act of authorship, and they produce the same kind of mark. There is no "hold to
draw", no "drag to draw, tap to select", and no mode in which a tap does nothing.

- **Tap** on the creature → one mark, the size of a fingertip.
- **Drag** across the creature → one continuous stroke.
- **Tap** a control → the control activates. **Drag** a control → the control does
  not drag. This is the only place the two gestures differ, and it differs so that a
  child's enthusiastic sweeping thumb cannot drag the palette off the screen.

**No double-tap, no long-press on any child control, no swipe, no pinch, no
drag-and-drop, no context menu.** The full gesture vocabulary of the child's UI is
**tap, drag, and hold** — and hold does exactly one thing (§7.10.3).

#### 7.8.4 Pressed, and the 4 px ring

| Moment | Visual |
|---|---|
| Finger down | fill → `--ui-active`; **4 px `--ui-focus-outer` ring** at ≥ 4.85:1; a **2 px `--ui-focus-inner`** ring inside it; shadow tightens by 1 rung |
| Finger held | no change. **Holding does not animate a child control.** |
| Finger up | fill → `--ui-surface`; ring gone; shadow returns |

The **2 px inner ring carries nothing** — it is decoration, and it must never be
the only thing distinguishing pressed from unpressed. The outer ring is the channel,
and it is a *shape* channel, not a colour channel.

---

### 7.9 OS surfaces: the app icon and the notification

#### 7.9.1 The app icon — 1024 × 1024

| Rule | Value |
|---|---|
| Master resolution | **1024 × 1024**, square, no transparency in the delivered file |
| Subject | **the creature, unpainted, standing** — no child paint, ever, because the app icon is not the child's |
| Print | **no print.** The icon is a creature on a warm ground, full stop. |
| Text | **none.** No name, no initial, no badge, no count. |
| Safe zone | the creature occupies the **central 66%** of the canvas; the outer 17% on every side is ground only |
| Ground | `--ui-shell` → `--ui-surface` warm field, single soft light from the upper left |
| Rim | **Rim Rose permitted on the upper 55% of the silhouette edge only.** Consistent with §1.2: position + additive rise, never colour alone. |
| **Mint Halo** | **banned from the icon entirely.** A green halo on a launcher is a system-level reading we do not want, and it is a value that reads as an affordance elsewhere. |
| Legibility ladder | 1024 → 60 → 40 → 29 px. At 29 px the creature reads as a **silhouette with two eye marks**. Below 29 px the two eye marks are the minimum and nothing else survives. |

**Legibility ladder test.** If the icon does not read as "a small warm creature" at
29 px, it does not pass at 1024 px either — a launcher shows it at 60 px most of the
time and a child will recognise it there long before they can read the name.

#### 7.9.2 The OS notification — 24 to 48 px

The notification is an **adult surface**: it appears in the adult's lock screen or
notification shade, at a size the game does not control.

| Rule | Value |
|---|---|
| Icon | the creature's **solid monochrome silhouette** in `--ui-text-primary` on a `--ui-surface` rounded square. **No mark, no paint, no flourish, no spark.** |
| Interior detail | the eye and mouth marks are **omitted entirely below 32 px**. Below 32 px they are 2 px noise that reads as dirt. |
| Payload | **≤ 3 words and ≤ 24 characters.** Set in Nunito 600, 24 px, `--ui-text-primary`. |
| Payload examples that pass | `Dab is here` · `Your marks are ready` · `Hello from Dab` · `Dab remembered you` |
| Payload examples that fail | `Your pet has been waiting 3 days` (count + obligation) · `Come collect your reward` (debt) · `Streak: 5 days` (count) · `Dab is sad` (a problem) · `Open now to unlock a new mark` (promise + count) |
| Badge | **never.** No badge count, no dot, no number on the app icon. A badge is a debt. |
| Tone | **no urgency of any kind.** No `!`, no red, no "now", no action label that promises anything. |
| Action label | if the OS offers one, it reads **`Open`** — never `Play`, never `Collect`, never `Continue`. |
| Frequency | **not this section's decision.** Wording and payload are; cadence is a `producer` and `game-designer` question and is logged open in §7.10.1. |

**The notification's promise is that nothing is owed.** It may say the game exists.
It may not say the game is waiting, because a game that waits is a debt, and the
whole product is built so the child never feels one. §2.2's Greeting pays this off
from the other side: the longer they were away, the **warmer** the greeting, with
**no number anywhere** to tell them the game noticed.

---

### 7.10 The parent gate, the declines, and the open questions

#### 7.10.1 The parent gate

The adult surface is **not a menu**. It is reached by **holding MENU for 1.5
seconds**, and it looks like the world closing a frill, not like a settings screen
appearing.

| Moment | What happens |
|---|---|
| 0 ms | MENU's tiered-pebble silhouette begins to press into the card |
| 0.6 s | the creature's frill **opens** — a world event, in `--ui-surface-raised`, not a UI transition |
| 1.5 s | the gate is open. Release to enter it; release earlier and nothing happened. |
| Inside | ≤ 6 rows, platform system face, weight 400–700 |

| Row | What it does | Type budget |
|---|---|---|
| **Text size** | steps the child's UI between tier 1.00 and 1.40 | ≤ 8 words |
| **Sound** | the child's audio on or off | ≤ 6 words |
| **Restore purchase** | the store's own restore flow, if the platform has one | ≤ 6 words |
| **Legal / privacy** | the required notices, in the platform's own layout | ≤ 12 words of summary |
| **Local data** | one plain sentence: what is stored, and where | ≤ 8 words |

**Heading: ≤ 8 words. Total body copy across all rows: ≤ 40 words.** These are the
whole budget. A parent gate that needs a paragraph is a privacy policy wearing a
costume, and a privacy policy belongs on a web page that the parent chose to visit.

**Banned inside the gate, without exception:** anything destructive; an ads
toggle; currency, purchases beyond the platform's own restore; progression, levels,
or streaks; **any setting addressed to or affecting the child**; any external link
that leaves the app; any account, login, or profile; any analytics, any telemetry
readout, any identifier; any "are you sure?".

**The gate has no exit button.** It closes when the parent closes it, and when it
closes the game returns to the Greeting — not to a menu, not to a hub, and not to
a paused state with a resume button. The child cannot arrive at a screen that says
something went wrong, because there is no such screen.

#### 7.10.2 Declines

| Declined | Because |
|---|---|
| Reward popups, badges, "new" dots, achievement toasts | V‑B; rewards grow in |
| Scroll bars, tabs, carousels, dropdowns, sliders, toggles | a control the child can change by accident is a control the game cannot predict |
| Lock glyphs, padlocks, greys, crosses | Pillar 2 — nothing is ever forbidden |
| A settings grid of any kind in the child's UI | the adult surface exists; it is 1.5 s away behind MENU |
| A HUD strip, resource bar, objective marker, minimap | rung 3 must stay singular; a strip is a second subject |
| Coloured UI chrome | §7.5.3 — no saturated UI surface except swatches, rewards, wet strokes |
| A separate page indicator beside PAGE | the nub's direction already says it (D7) |
| A thirteenth record slot, or a records scroll | §7.4.1 — twelve is what this card draws |
| Any second hand-authored camera angle | Framing A–D only |

#### 7.10.3 Declared divergences from the sections above

| # | Divergence | Decision |
|---|---|---|
| **D1** | Art-bible §3.4 gives the ribbon a **nominal 20% radius**. §1.3 and §3.6 require a **35% minimum** on every UI container. | **35% governs.** The §3.4 figure is treated as superseded. Ribbons, cards, plates and record faces all use ≥ 35% (§7.3.1). |
| **D2** | §5.4's Warm Settle is described as the mark leaning *away*. Read literally, "away" invites translation away from the child. | **The mark never withdraws.** It recedes: scale 0.88, rung 1 → 0, and a **4 px translation *toward* the last touch** — 320 ms out, 600 ms hold, 400 ms return, zero overshoot, fill and hue unchanged. "Leans away" means *recedes*, not *leaves*. |
| **D3** | Art-bible §3.4's touch-target placement would put MENU in the thumb zone. | **MENU is inverted** to x 72–288, y 24–240, outside the bottom 40%. The parent gate must be hard to open by accident. It is the only control that inverts the rule, and for a safety reason. |
| **D4** | Art-bible §3.4 states a **72 px / 24 dp** touch floor. | **Superseded by 216 px = 72 dp at reference = 48 dp at 720 × 1280**, applied to every child target (§7.6.2). The 72 px figure is documentation, not a target. |
| **D5** | The closed control set listed **PAGE ×2** (previous + next). A 936 × 648 card has room for **one** 216 px pebble in the right column once UNDO is placed. | **PAGE is one pebble, not two.** Its nub points at the page that has more swatches: right on page 1, left on page 2. At two pages "the other page" is unambiguous, and one control is one fewer thing to learn. **This removes a control.** |
| **D6** | Art-bible §4.7.2 declares **`--ui-reward`**; `palette.json`'s UI list has **eleven tokens and does not include it.** | **Unresolved here, and deliberately so.** This section creates no twelfth token and no twelfth colour. Rewards are specified as Tier-0 paint at full strength. Reconciliation belongs to `technical-artist` and `producer` in the machine palette. |
| **D7** | §4c's colourblind matrix treats a standalone page indicator as a redundant channel. | **Declined.** The PAGE nub's direction carries the same information in the same shape language, so a separate indicator would be a second element saying one thing (§7.1.2 V‑C). |

#### 7.10.4 Open questions this section cannot answer

These are logged rather than guessed. Each names the owner.

| Question | Owner |
|---|---|
| What happens to the **thirteenth mark** the child makes? This card draws twelve and does not scroll. | `game-designer` |
| How often does the **OS notification** fire, and at what times of day? Wording is fixed here; cadence is not. | `producer`, `game-designer` |
| Is the **records card** reachable before the child has made any marks, and what does an empty grid look like? An empty 3 × 3 grid may read as a problem, which §2.8 forbids. | `game-designer`, `ux-designer` |
| Does **PLACE** sit on the hub card, or only inside Painting? This section places it in both (§7.8.1). | `game-designer` |
| What is the exact **relief requirement** for the two lowest-luma paints (Violet Pop, Cranberry) on a `--ui-surface` card face? | `technical-artist` |

---

### 7.11 Reviewer checklist

Every line is a test someone can run on a frame, a spec, or a screenshot and mark
pass or fail. **Any FAIL is a defect; "it looks good" is not an answer.**

**Hierarchy**
- [ ] 1 The creature's mean region luma exceeds every other region by **≥ 16%**.
- [ ] 2 At the flourish peak, a greyscale capture shows **the mark as the brightest object in frame**.
- [ ] 3 Exactly **one** control is at 1.4× scale. Not two.
- [ ] 4 Exactly **one** card is on screen. Not two.
- [ ] 5 The active control's read comes from **size + rung + position**, and would still read if its fill were `--ui-surface`.

**V‑A and V‑B**
- [ ] 6 Every UI fill against the field behind it is **≤ 1.91:1**.
- [ ] 7 No UI surface sits below **L3 Umber, 41.6%**.
- [ ] 8 No UI fill falls outside the **66.9% – 91.9%** luma band.
- [ ] 9 No reward popup, badge, dot, or toast exists anywhere in the build.
- [ ] 10 The hub card is **624 px** at type tier 1.00 **and** at tier 1.40.

**Colour**
- [ ] 11 No UI surface exceeds **33.2%** HSV S; authored maximum is **30.3%**.
- [ ] 12 No UI text, stroke, or ring exceeds **48.2%** HSV S.
- [ ] 13 The only pixels above **40%** HSV S are swatch faces, rewards, and wet strokes.
- [ ] 14 Exactly the **eleven** declared tokens appear. No twelfth colour exists.

**Shape**
- [ ] 15 Every container is a pebble, a card, a ribbon, a plate, or a record face. **No fifth shape.**
- [ ] 16 Every corner radius is **≥ 35%** of its container's short edge (pebbles fully round).
- [ ] 17 **Zero** sharp corners. Every stroke has rounded caps and joins.
- [ ] 18 No chevron, arrow, gear, hamburger, X, checkmark, magnifier, lock, tab, slider, toggle, or dropdown appears.

**Targets**
- [ ] 19 Every child target is **216 px** (or **144 px** for swatches) — **72 dp / 48 dp** at 1080 × 1920 and **exactly 48 dp** at 720 × 1280.
- [ ] 20 No control was delivered at §3.4's legacy 72 px / 24 dp figure.
- [ ] 21 Every hit area fills its pebble. No transparent-margin hits.
- [ ] 22 Icon strokes are **≥ 8 px**; nothing is drawn below **12 px**.

**Layout**
- [ ] 23 Content width **936 px**, side margins **72 px**, card padding **48 px**.
- [ ] 24 Every card's bottom edge is **76 px** above the bottom safe edge.
- [ ] 25 Every child control is **below y = 1152**. The only exception is MENU (D3).
- [ ] 26 The selected swatch's 1.4× rise clears its neighbours by **≥ 43 px** and never touches the caption plate.
- [ ] 27 The hub card is exactly **2 × 2**; the palette card exactly **2 rows**; the records card exactly **12** faces.

**Type**
- [ ] 28 Every word is **Nunito**, except the child's own name, which is **Patrick Hand**.
- [ ] 29 Minimum type is **48 px / 16 sp** at 1080 × 1920.
- [ ] 30 Every label box is sized to its **widest state at tier 1.40**; at 1.40, labels wrap and boxes grow — and **no card grows**.
- [ ] 31 No word exceeds **five words**. No exclamation mark appears.
- [ ] 32 No count, timer, score, rank, level, "try again", "new", "loading", or "welcome back" appears.
- [ ] 33 MENU, UNDO, PAGE and the swatches carry **no text at all**.

**Gesture and motion**
- [ ] 34 Tap and drag produce the same kind of mark (§3.4 parity).
- [ ] 35 No double-tap, no long-press on a child control, no swipe, no pinch, no drag-and-drop.
- [ ] 36 Pressed state shows the **4 px outer focus ring**; the 2 px inner ring carries nothing.
- [ ] 37 A settling control shows **scale 0.88, rung −1, fill unchanged** — no grey, no hue shift, and **never on the world**, only on a card.
- [ ] 38 A settling control **never translates away from the child** — it moves 4 px *toward* the last touch.
- [ ] 39 State changes complete in **≤ 400 ms**, ease-out, and nothing slides in from off-frame.

**Camera**
- [ ] 40 Horizontal FOV is locked at **32°**; vertical FOV varies and the difference is absorbed entirely by sky.
- [ ] 41 The creature's ground line is **760 px** above the bottom safe edge, projected height **720 px** (806 px in Painting).
- [ ] 42 On 19.5:9 and 20:9, sky grows to **944 px** and **1004 px** while the creature's size is unchanged.

**OS surfaces**
- [ ] 43 App icon: creature unpainted, **no text, no print, no Mint Halo**, inside the central **66%**.
- [ ] 44 App icon is legible as a small warm creature at **29 px**.
- [ ] 45 Notification icon is a **solid monochrome silhouette**; interior detail is **omitted below 32 px**.
- [ ] 46 Notification payload is **≤ 3 words / ≤ 24 characters**; no badge, no `!`, no red, no obligation.
- [ ] 47 The parent gate is reached only by **holding MENU 1.5 s**, and shows **≤ 6 rows**, heading **≤ 8 words**, body **≤ 40 words**.
- [ ] 48 No destructive action, ads toggle, currency, progression, child setting, analytics readout, or external link exists inside the gate.

**Bookkeeping**
- [ ] 49 D1 through D7 are either honoured above or explicitly overridden in writing by the `creative-director`.
- [ ] 50 Every asset in the build matches `design/Art/asset-manifest.md` and is named `[category]_[name]_[variant]_[size].[ext]`.

---

## Section 8 — Asset Standards

**Owning role:** Art Director (authoring), technical-artist (import presets,
compression, atlas and variant implementation), unity-specialist (build-side
enforcement). Every number below is either a canon value carried forward or a
budget this section derives from one. Nothing here weakens §5.5, §5.6, or §5.7.

**Governing principle — Pillar 3.** *One thing finished beats more content.*
Every rule in this section is a **subtraction**. A budget that cannot hold is a
budget that has been spent on a second thing. Where a rule looks
over-restrictive — one mesh, one 0–1 UV layout, one print texture slot, six
stroke targets, twelve shader variants against a 200 ceiling — that is not
poverty. That is the entire point: the headroom is deliberately unspent so the
creature's *flat canvas* stays flat and one authored mark survives a save/load
round trip byte-identically.

---

### 8.1 Global budgets

#### The ceiling table — carried, not re-derived

| Metric | Preferred | Hard ceiling | Source |
|---|---|---|---|
| Frame rate | **60 fps** (16.6 ms) | never below **30 fps** (33.3 ms) | `AGENTS.md` A5 |
| Draw calls / frame | **< 150** | **250** | A5 |
| Visible triangles | **< 300k** | **500k** | A5 |
| Shader variants | **< 200** | **400** | A5 |
| Memory footprint | **< 500 MB** | **800 MB** | A5 |
| Cold start to interactive | **< 5 s** | **8 s** | A5 |

Engine **Unity 6000.3.11f1 (6.3 LTS)**, URP, **Render Graph mode only**, IL2CPP,
New Input System only. `Addressables` is **deferred / not installed** per
`docs/framework/technical-preferences.md` — this section does not assume it (§8.10).

#### The explicit device floor assumption

> **All asset budgets in this section are authored and profiled against LOW-END
> Android as the floor.**

| Axis | Floor assumption |
|---|---|
| SoC class | 2019–2021 budget Android, 4-core, **ARM64 only** (IL2CPP, Unity 6 has no ARMv7 player) |
| RAM | 4 GB device / **2 GB usable app heap** |
| GPU | Vulkan preferred, **GLES3.1 mandatory fallback** — every asset must be correct under both |
| Screen | **1080 × 1920 portrait**, 8-bit, 420 ppi-class density |
| Thermal | Sustained play; **no burst-credit assumption.** A budget met only in the first 90 s is not met |
| Input | Touch only. No gamepad, no hardware keyboard assumption |

**Divergence, flagged not hidden.** `technical-preferences.md` §Performance
Budgets names **mid-range** Android as the floor. This section profiles to
**LOW-END**, which is the stricter of the two and therefore safe under both
readings. **This divergence is now resolved.** The Creative Director ratified
**LOW-END** as the standing device floor, and `technical-preferences.md` §Performance
Budgets has been updated to match, so the two documents no longer name different
floors.

#### The peak-scene ledger — why the ceilings are not actually tight

| Element | Tris | Draws |
|---|---|---|
| Creature base body | 10,000 | 1 |
| Creature HOOK mesh | 2,400 | 1 |
| Print mask (9 classes) | **0** | **0** — one texture slot in the base material |
| Paint, 6 mask slots | **0** | **0** — six channels in the base material |
| Creature cast shadow | **0** | **0** — projected soft blob, Peach Shadow 0.28 |
| Mark relief, all slots | **0** | **0** — authored in the base material |
| Echo geometry, 6 concurrent | 3,600 | 1 (instanced, one family per register) |
| Ground bowl | 800 | 1 |
| Sky dome | 200 | 1 |
| Sun disc | 64 | 1 |
| Props, 3 base meshes × ≤ 24 instances | 15,360 | 3 (GPU instanced, one per family) |
| **Peak total** | **32,424** | **9** |

**32,424 tris is 10.8% of the 300k preferred budget. 9 draws is 6.0% of the 150
preferred budget.** Not one of these numbers was reached by cutting a corner on
the creature. The creature is **4.1%** of the triangle budget (§5.7) and it is
the only thing that must be perfect. Everything else is scenery and is sized so
it can never crowd the canvas.

#### Texture memory — not a constraint, and that is a designed outcome

| Class | Asset | Compressed size |
|---|---|---|
| Print mask | 1 × Texture2DArray, 1024², **9 slices** | 4.2 MB |
| Hook masks | 1024² → 512², **3 slices** | 0.35 MB |
| Streak patterns | 4 × 512² single-channel | 0.47 MB |
| Feather ramp | 64 × 4 single-channel | 0.01 MB |
| Shell albedo / roughness | **ZERO TEXTURE** (§8.5.2) | 0.00 MB |
| Environment | **ZERO TEXTURE** (§3.3, §8.5.2) | 0.00 MB |
| UI atlas | 1 × 2048² RGBA | 1.86 MB |
| **Authored texture total** | | **≈ 6.9 MB** |

1.4% of the 500 MB preferred memory budget. Memory pressure in this project
comes from the engine, the save layer, and runtime stroke targets — **not from
art assets.** Which means the two numbers that actually need defending are draw
calls and shader variants, and those are where §8.2 and §8.6 put their weight.

---

### 8.2 The nine hand-off items — as verifiable acceptance items

§5.7 declared nine obligations to this section. Each is restated below as a
**binary acceptance item**: a named test, a threshold, and a PASS/FAIL with no
judgement call in it. "Looks right" is not a result. An item that cannot be
measured is not an item, and an unmeasurable obligation in a hand-off list is an
obligation nobody discharges.

Ownership column: **TA** = technical-artist · **AS** = art director · **US** =
unity-specialist · **SH** = unity-shader-specialist.

| # | Obligation | Owner | Test method | PASS | FAIL |
|---|---|---|---|---|---|
| **1** | **R5 — relief + streaks hold on the two dark Tier-0 paints, against all nine print classes** | SH, verified by AS | Build the **R5 verification board**: 9 print classes × 2 paints (**Cranberry luma 28.3**, **Cherry Pop 33.4**) = **18 cells**. Each cell renders the class as mask over the shell, plus one mark at relief **0.02 H** and one at **0.04 H**, each carrying **≥ 3 streaks per 0.08 H**. Capture in **greyscale**, colour fully removed, creature height = **64 px**, front-on, no motion. Sample method: 3 px median luma inside the mark body vs 3 px median luma in the adjacent print region; streak method: adjacent streak-band median luma across the band's normal direction. | **All 18 cells** pass both: **(a)** relief-shoulder edge separation **≥ 3.0% luma**, and **(b)** **≥ 3 resolvable streak bands per 0.08 H** with adjacent-band separation **≥ 3.0% luma** | Any one of 18 cells misses 3.0% on either channel, or fewer than 3 streak bands resolve. No partial pass. One cell is a defect in the game's core loop |
| **2** | Shell gloss **0.02** vs wet-paint gloss **0.35** — a **17× ratio** | SH | Greyscale capture at 64 px of one wet stroke (t = 0.4 s into the 0.8 s wet hold) beside its settled sibling on the same shell | Both gloss values match §5.5 to **± 0.01**; the wet stroke's specular lobe is **visibly separate** in the greyscale capture and survives **−60% saturation** (the §3.4 recognition test) | Wet and dry are indistinguishable in greyscale. The wet/dry lifecycle is the product's reward signal; if it dies, the child gets no confirmation their work landed |
| **3** | **Over-composite, never multiply** | SH | Sample the final composited pixel of a **full-load Cherry Pop** stroke on the shell. §5.5 names Cherry Pop at **80.7% HSV S**; the shell is **15.0% S** | Arrives **≥ 75% HSV S** (± 2) — i.e. pigment load survives; multiply would land it below 20% | HSV S below 60%. Paint is muddied. Multiply is **FORBIDDEN** by §5.5, not merely discouraged |
| **4** | Mark **occludes** print, locally | SH | Render a mark overlapping a print class boundary, greyscale | Inside the mark silhouette, the print mask contributes **exactly 0** — measured as an unchanged pixel when the print texture is swapped for flat black | Any print contribution inside the mark. §1.4 P1: the child's authorship outranks the variant's flavour, always |
| **5** | Six mask slots, one material, **ZERO shader variants** | SH, counted by US | Build-time variant enumeration for the creature material, cross-referenced against the variant ledger in §8.6 | Creature material variant count = **1**. Adding slots 7 or 8, or a per-slot keyword, is a **fail** of this item, not an extension of it | ≥ 2 variants. Every slot must remain a texture *channel*, never a keyword — the whole reason the paint system costs nothing against the 200 ceiling |
| **6** | Rim over the mark's raised shoulder + **0.010 H** contact line (R4) | SH, verified by AS | Greyscale capture at 64 px, mark at relief **0.02 H**, flat front lighting | Upper shoulder carries the **Rim Rose** rim; a **contact line** is measurable along the lower edge. **Screen-space floor:** at 64 px, 0.010 H = **0.64 px**, which is sub-pixel — the contact line **must** be rasterised as a soft screen-space ramp of **≥ 1.0 px** width, not a hard edge, or it aliases out of existence | Contact line absent, or rendered as a hard sub-pixel edge that disappears. Note: a hard highlight is not sufficient; §5.7 item 6 is explicit that relief on a dark paint needs a **contact**, not just a rim |
| **7** | Eye self-lift is the live-vs-record discriminator | SH, verified by AS | Greyscale capture, live creature vs the same creature from a loaded save | Live eye: additive self-lift **≥ 8% luma**. Record-print eye: additive self-lift **exactly 0.0%** | Either value inverted. If a record shows a live eye, the child believes the pet they left is still waiting — that is the one lie this product must not tell |
| **8** | **Zero-triangle** cast shadow; Peak < 300k on **low-end** Android | TA, verified by US | Profiler on the floor device from §8.1, every §2 state, 90 s sustained | Peak visible tris **< 300,000** (measured peak **32,424**, §8.1); shadow cost **0 tris, 0 draws**; blob opacity **0.28**, tint **Peach Shadow** | Any real shadow mesh appears on the creature or on any prop. §3.3 prohibition 4 and §5.7 both forbid it: the cast shadow is the game's **only** shadow and it must cost nothing |
| **9** | **MATERIAL NEVER-FUR EXCLUSION** — the creature material may not implement fur, fibre, strand, hair-card, or shell-layer effects of any kind | SH (implement), AS (own), US (audit) | **Three-part audit, all three must pass.** **(a) Source audit:** the creature shader source and every material assigned to the creature prefab contain **zero** fur / fibre / strand / haircard / haircard / shelllayer / shell-layer keywords, `#pragma multi_compile` or Shader Feature declarations, and zero references to fur, hair, strand, fibre, furShell, or shell-layer assets. **(b) Feature audit:** the build's shader-variant report shows the creature material declares **0 keywords**, so no fur toggle can exist at runtime even by accident. **(c) Render audit:** greyscale capture of the shell, 64 px and at 1.0 H, with **UV0 sampling every 64×64 texel grid point** — albedo luma deviation across the entire body **≤ ±1.0%** | All three parts pass | Any shell-layer fur implementation — because it visually destroys the flat canvas the paint system depends on, and §5.5 states it is a **hard exclusion, not a preference**. This is a material-level exclusion, not a modelling note: a fur-free *mesh* with a fur *shader* is the failure case this item exists to catch |

**Discharge rule.** Items 1–8 are §5.7's obligation and were previously
described as "eight". They are **eight items plus item 9 = nine**, and all nine
are closed. **No item may be re-scoped, deferred, or partially passed.** An item
that cannot pass is an ADR request (§8.6.3), not a lowered threshold. Threshold
changes require the **creative-director** and are recorded against the canon
value they replace.

---

### 8.3 Naming conventions

**Closed, machine-checkable, no judgement.** The pattern is:

```
[category]_[name]_[variant]_[size].[ext]
```

Lowercase `snake_case` for **every** asset. One global rule set, then a worked
example per category. Assets failing 8.3 fail the definition of done (§8.11)
with no reviewer discretion.

#### The four global rules

| Rule | Statement | Machine check |
|---|---|---|
| **R1 Case** | **Lowercase `snake_case`, always.** Lowercase with underscores is the *only* permitted form. No `PascalCase` (`BodyMesh`), no `camelCase` (`bodyMesh`), no `kebab-case` (`body-mesh`) | Regex `^[a-z0-9]+(_[a-z0-9]+)*$` on the filename stem |
| **R2 Separators** | **Exactly one underscore between tokens.** No spaces, no hyphens, no dots, no consecutive underscores. The single permitted dot is the extension separator | Regex rejects `\s`, `-`, `\.` (before the last dot), `__` |
| **R3 Reserved tokens** | `low`, `high`, `mask`, `a`, `b`, `alpha`, `01`…`99` are **reserved** and mean only what §8.3 says they mean. An author may not repurpose them | Enum check against the reserved list |
| **R4 Truncation** | **No truncated or abbreviated stems.** `bdy`, `creat`, `tex`, `matl`, `anim` are all forbidden. Full words only. A name that does not fit is a naming bug, not a length bug | Reviewer checklist item; not automatable |

> **The C# exception.** `technical-preferences.md` mandates PascalCase for
> classes and a filename that matches the class name exactly. That rule governs
> `.cs` files and **only** `.cs` files. It does not license PascalCase anywhere
> else in `Assets/`. This is the single most common Unity-project naming drift
> and it is why R1 exists.

#### Reserved token semantics

| Token | Means | Never means |
|---|---|---|
| `low` / `high` | The **same map** at two resolutions (a resolution ladder pair, §8.5.1) | A lower-quality variant of the art |
| `mask` | A **data** map. Always `sRGB OFF`, always `Wrap Clamp`, always a single-channel or class-index map (§8.7) | A greyscale version of a colour map |
| `alpha` | The **coverage channel** of the immediately preceding map | A standalone opacity map |
| `a` / `b` | Two LOD levels of one asset, or variant A/B of one hook. Declared in the asset manifest | A revision, an iteration, or a "second try" |
| `01`–`99` | A two-digit frame or sequence index, **zero-padded** (`tex_petal_01.png`, never `tex_petal_1.png`) | A version number |

#### Worked example per category

| Category | Pattern | Worked example | Notes |
|---|---|---|---|
| **Mesh** | `env_[object]_[descriptor]_large.fbx` | `env_bowl_ground_large.fbx` | `large` is the world scale class, not a size guess. Scale classes: `small` < 0.6 H · `medium` 0.6–3 H · `large` > 3 H |
| **Creature mesh** | `chr_dab_body_base.fbx` | `chr_dab_body_base.fbx` | One creature mesh exists. If a second appears, Pillar 3 and §5.7 both fail — this is a **stop-ship**, not a naming fix |
| **Hook mesh** | `chr_hook_[type]_medium.fbx` | `chr_hook_floppyear_medium.fbx` | `type` ∈ `curl` · `crest` · `floppyear` — the three hooks of §5.2 |
| **Material** | `mat_[subject]_[role].mat` | `mat_dab_shell_body.mat` | `role` ∈ `shell` · `hook` · `ground` · `sky` · `sun` · `prop_pebble` · `echo` |
| **Shader** | `shd_[subject]_[role].shadergraph` | `shd_dab_shell.shadergraph` | Custom shaders are **approved per §8.6.2**. A new `.shadergraph` is not an artist-side decision |
| **Texture — colour** | `tex_[subject]_[descriptor].png` | `tex_pebble_peach.png` | `sRGB ON` (§8.7) |
| **Texture — data/mask** | `tex_[subject]_[descriptor]_mask.png` | `tex_dab_print_spots_mask.png` | `sRGB OFF`, `Wrap Clamp`, `_mask` is mandatory |
| **Texture array** | `arr_[subject]_[descriptor]_Nslice.asset` | `arr_dab_print_class_9slice.asset` | N is the **exact** slice count, written in the name. `.asset` because it is a Texture2DArray importer asset |
| **Render target (runtime)** | `rt_[slot]_paint_[res].rendertexture` | `rt_slot06_frontplate_paint_512.rendertexture` | Six of these exist by §5.5. They are **runtime-authored**, never committed as art |
| **Animation clip** | `anim_[subject]_[clipname].anim` | `anim_dab_absorbpainting.anim` | `clipname` ∈ the **closed 20-clip set** of §5.6. A 21st clip name is a fail |
| **Controller** | `ctl_[subject]_[purpose].controller` | `ctl_dab_creature.controller` | One controller per rig. Layers are fixed: **Base + Echo additive** (§5.6) |
| **Audio clip** | `aud_[event]_[register].wav` | `aud_echo_colume1.wav` | `register` ∈ `colume1`…`colume6` for Echo, or `fx` / `loop` / `voice` elsewhere |
| **Prefab** | `pfb_[system]_[subject].prefab` | `pfb_creature_dab.prefab` | System ∈ `creature` · `env` · `prop` · `ui` · `fx` |
| **ScriptableObject** | `so_[domain]_[subject].asset` | `so_print_class_variant.asset` | Domain ∈ `print` · `hook` · `paint` · `palette` · `audio` · `ui` |
| **UI atlas** | `atl_ui_[scope].png` + `atl_ui_[scope]_sprite_02d.asset` | `atl_ui_core.png` | One atlas per scope. `_02d` = a Sprite (2D and UI) importer asset |
| **UI icon** | `ui_icon_[subject]_[descriptor].png` | `ui_icon_palette_brush.png` | Authored at **96 px minimum**, 128 px default (§3.4 icon weight) |

---

### 8.4 Mesh standards

#### 8.4.1 Triangle budgets by class

| Class | Tris | Verts | UV pins | Faces | Absolute cap |
|---|---|---|---|---|---|
| **Creature base body** | **10,000** (8–12k band) | ≤ 5,200 | ≤ 24 | ≤ 3,400 | **12,000** |
| **HOOK mesh** (per type) | **2,400** | ≤ 1,600 | ≤ 8 | ≤ 800 | **3,000** |
| **Ground bowl** | 800 | ≤ 500 | ≤ 8 | ≤ 300 | 1,200 |
| **Sky dome** | 200 | ≤ 130 | 0 | ≤ 100 | 300 |
| **Sun disc** | 64 | ≤ 40 | 0 | 1 | 96 |
| **Prop — pebble** | 320 | ≤ 200 | ≤ 4 | ≤ 120 | 480 |
| **Prop — arch** | 640 | ≤ 400 | ≤ 4 | ≤ 240 | 900 |
| **Prop — tuft** | 480 | ≤ 300 | ≤ 4 | ≤ 180 | 700 |
| **Echo geometry** (per family) | 600 | ≤ 400 | ≤ 8 | ≤ 220 | **1,000** |

The body and hook numbers are **carried from §5.7 and are not negotiable
here.** They total **12,400 tris in 2 draw calls**. Every other class is sized
against §3.3's four geometry classes and Pillar 3.

**Face rule, all classes:** quads and triangles only. **Zero n-gons with 5 or
more vertices** on the creature, hook, or props. A single n-gon is a triangulate
ambiguity waiting to shear a child's mark. Triangles are permitted and expected
at creases, poles, and caps.

#### 8.4.2 Normal smoothing

| Rule | Value |
|---|---|
| Shading | **Smooth** across the body and hook |
| Auto-smooth / split angle | **60°** |
| Authored custom split normals | **Permitted only on named, listed edges**, **≤ 12 per mesh** |
| Unnamed hard edges | **Defect.** A hard edge that is not in the mesh spec will be re-smoothed by the next artist's export settings and the silhouette will change under nobody's supervision |

The 12-edge ceiling is the whole rule. Named edges are declared in the mesh
spec; everything else is smooth. On a rim-lit creature an accidental hard edge
breaks the rim into two and is immediately visible.

#### 8.4.3 UV0 — the single 0–1 layout

> **There is exactly one UV channel: UV0. One 0–1 layout. On every mesh in the
> game. No exceptions, no second channel, no tiling UV, no lightmap UV.**

| Rule | Value | Why |
|---|---|---|
| Channels | **UV0 only.** A second UV channel is a fail | §5.5 makes the six mask slots *regions on the base mesh's single 0–1 layout*. A second channel splits the canvas |
| Layout | Single 0–1 square. Islands packed by the priority in the table below | One atlas, one import, one predictable seam |
| Bleed border | **16 texels** of bleed around every island | A stroke crossing a seam must never sample its neighbour |
| Wrap mode | **Clamp** on every creature map | See the disambiguation below |
| Non-overlap | **Islands may not overlap.** Overlap is a defect, not a packing shortcut | Overlapping islands make a mark ambiguous against two classes at once |
| **Wrapping, disambiguated** | §5.5's "wrapped 3D shell region" means the **region wraps around the body's 3D form**. It does **not** mean `Wrap Repeat` | A `Repeat` island reads its own opposite edge at the seam. On a paintable surface that is a bug that only shows up when a child paints across it |

**Print-class island layout, nine islands in one 0–1 square, in packing
priority order.** The nine print classes are laid out left-to-right, top-to-band,
largest class first, because the largest class must never be the one that fails
to fit:

| Band | Islands | Constraint |
|---|---|---|
| Band A | 3 largest classes | Each ≥ 512 × 512 texels at 1024² |
| Band B | 4 mid classes | Each ≥ 384 × 384 texels |
| Band C | 2 smallest classes | Each ≥ 256 × 256 texels |

Every island, at every size, must still resolve the **P5 minimum feature of
0.08 H = 5.1 px at 64 px** (§4.5.4). At 1,024 px/H authoring resolution that
feature is **82 texels** — the smallest permitted island (256²) resolves it at
3.1×. Binary test: sample the feature's texel span in the importer; **< 82
texels is a fail.**

#### 8.4.4 The six paint mask slots as UV regions

The six slots of §5.5 are **sub-rectangles inside the print-class layout's
reserved slot band**, not separate meshes. No slot geometry exists — §5.7 sets
mark relief at **0 tris**.

| Slot | Region size (§5.5) | Required UV sub-rect | Stroke target | Guard border |
|---|---|---|---|---|
| 1 Crown | 0.26 H wide, top 0.10 H of skull | ≥ 0.30 × 0.14 H | 512² | 32 texels |
| 2 Cheek-L | 0.18 H | ≥ 0.22 × 0.22 H | 512² | 32 texels |
| 3 Cheek-R | 0.18 H | ≥ 0.22 × 0.22 H | 512² | 32 texels |
| 4 Flank-L | 0.30 × 0.24 H | ≥ 0.34 × 0.28 H | 512² | 32 texels |
| 5 Flank-R | 0.30 × 0.24 H | ≥ 0.34 × 0.28 H | 512² | 32 texels |
| 6 Front plate | 0.34 × 0.30 H | ≥ 0.38 × 0.34 H | 512² | 32 texels |

Each sub-rect is sized to the §5.5 region **plus a one-stroke-width margin**,
because a stroke is allowed to land at the very edge of a slot. Every stroke
target is **512 × 512 RGBAHalf**, `Wrap Clamp`, with a **32-texel guard border**
of zeroed colour and zeroed coverage so bilinear filtering can never wrap a
neighbouring slot's pigment into this one.

**Why 512² is enough.** Authoring resolution is **1,024 px/H**. The widest
permitted stroke is slot 6 at **0.34 H = 348 px**. A 512² target oversamples
that by **1.47×**. The thinnest permitted feature, **0.08 H = 82 px**, resolves
to **120 px**. Six targets × 512² × 8 bytes = **12.6 MB**, which is 2.5% of the
500 MB preferred memory budget. No memory ADR is required.

**Hook/occlusion rule (carried from §5.5, and an asset obligation).** The Crown
slot's UV region is authored to wrap the **front-top of the skull** and is
unaffected by which hook is fitted. **A hook may not occlude a slot.** Cheek-L
and Cheek-R clear all three hook silhouettes at maximum ear spread, verified
once per hook type at build time — **three checks, one per hook type.**

#### 8.4.5 Tangents

| Rule | Value |
|---|---|
| Creature body, hook | **Tangents not generated.** The creature ships **no normal map** — the shell is flat by rule (§5.5) |
| Props, Echo geometry | Tangents not generated. No normal maps in this project as shipped |
| If a relief normal map is ever adopted | **MikkTSpace**, tangents written (4-component, W = 1), `sRGB OFF`, and this creates a **new asset class requiring an ADR** (US + creative-director). It also interacts with item 9's audit, because a fur-style detail normal on the shell is exactly the failure case item 9 catches |

The *implementation* of R1 relief — parallax offset, detail normal, or shell
offset — is the **shader specialist's** call. The **art** obligation is that the
authored relief silhouette and the streak map exist as greyscale data maps at
the §8.5.1 ladder. Art supplies the map; TA/SH choose the technique.

#### 8.4.6 Mesh compression

| Mesh | Compression |
|---|---|
| **Creature body, HOOK** | **OFF.** Unconditional. Vertex quantisation moves the surface, and the surface *is* the canvas: it would break the ±1.0% shell flatness measurement, the 0.02 H relief floor, and the three-hook separability test at **32 px** |
| Ground bowl, sky dome, sun disc | **OFF** — under 1,200 tris; compression saves nothing measurable |
| Props | **Medium** permitted. Nothing reads a prop's silhouette at finer than 0.6 H (§3.3) |
| Echo geometry | **Off → Medium** permitted only in the Flourish state, verified against the 0.25 H shed rule in §5.7 |

#### 8.4.7 Vertex colour

| Mesh | Permitted use |
|---|---|
| **Creature body, HOOK** | **FORBIDDEN as data.** The mesh must bake to **uniform (1, 1, 1, 1)** — no tinting, no mask, no paint ID, no per-vertex variation of any kind. A baked check that fails this is a fail. §5.5's canvas carries its data in **texture channels**, and a second data path is a second thing to get wrong |
| Props | **Alpha only**, as a contact-occlusion weight, clamped **[0, 1]**. RGB must be **(1, 1, 1)** always |
| Echo geometry | **Alpha only**, as the per-vertex Echo mask. RGB must be **(1, 1, 1)** always |

RGB vertex colour on any mesh is a palette-override escape hatch. It is how a
creature ends up accidentally Cherry Pop in one lighting state. Not permitted.

#### 8.4.8 Colliders

**Primitive colliders only.** Sphere, capsule, box. **No imported
`MeshCollider`** on any asset. Consequence, and it is enforced: **Read/Write is
OFF on every mesh in the game** (§8.7), because nothing needs CPU mesh access.
If a mesh collider is ever required — for a child dragging a prop, say — that is
an **ADR** with a named memory cost, not an import-setting flip.

---

### 8.5 Texture standards

#### 8.5.1 Resolution ladder

Authoring base unit for the creature: **1,024 px per H** (body height = 1024
px in UV0 space). All resolutions are **power-of-two**.

| Class | Resolution | Format | sRGB | Notes |
|---|---|---|---|---|
| **Shell albedo** | **NONE — 0 × 0** | — | — | **A flat colour material constant.** §8.5.2 |
| **Shell roughness** | **NONE — 0 × 0** | — | — | Constant **0.85**. Constant **0.02** gloss |
| **Print mask** | **1024² × 9 slices** (Texture2DArray) | R8 single-channel | **OFF** | One sampler binding = "one texture slot" per §4.5. Layer index is a uniform, never a keyword |
| **HOOK masks** | **512² × 3 slices** | R8 | **OFF** | One array, three layers, one per hook type |
| **Streak patterns** | **512² × 4** | R8 | **OFF** | The authored R2 streak library |
| **Feather ramp** | **64 × 4** | R8 | **OFF** | R3's value ramp. Never a colour ramp |
| **Ground / prop / sky / sun** | **NONE — 0 × 0** | — | — | §3.3: no reflective surfaces, no texture detail. The sky's vertical gradient is two material constants evaluated from local Y |
| **Echo geometry** | **NONE — 0 × 0** | — | — | Geometry carries the read |
| **UI atlas** | **2048²**, one per scope | RGBA8 | **ON** | ≤ 32 sprite entries. 9-slice for pebbles/cards/ribbons |
| **UI icon (single)** | **≥ 96²**, default **128²** | RGBA8 | **ON** | §3.4: 8 px minimum stroke at 1×, so 4× authoring = 32 px minimum stroke in source |
| **Colour texture (props, if ever)** | **512²** | RGBA8 | **ON** | Standby class; unused as shipped |

#### 8.5.2 The creature shell needs almost no texture — and must have none

> **The shell carries no albedo texture and no roughness texture. It is two
> material constants: albedo **Dawn Cream `#F7E9D2`** (L7, luma 91.9%, HSV S
> 15.0%), roughness **0.85**, specular gloss **0.02**.**

This is not a compression saving. It is the mechanism.

§5.5 requires shell albedo flat to **±1.0% luma across the entire body** and
calls that *"the strictest art rule in the section."* A texture map cannot
hold ±1.0% flatness through an ASTC or ETC2 codec. ASTC 6×6 is a lossy
block codec; its block-level error on a smooth field is on the order of ±1–2%
luma **before** the panel's own 8-bit quantisation and the shader's lighting
maths. Shipping a shell albedo texture would mean shipping a shell that is
out of specification by construction, and the failure would be invisible in
every interior lighting state and visible only on a dark paint in Outdoor
lighting — the exact case item 1 tests for. **Flatness is a property of the
number of texels in the mesh. Zero texels means zero deviation.**

The same logic applies to the environment (§3.3: no reflective surfaces, no
texture detail, one untextured field behind the creature) and to Echo geometry,
which is form, not paint.

#### 8.5.3 The packing solution — 9 print classes + 6 paint slots, at zero shader variants

**Decision: a Texture2DArray for the print classes; texture *channels* for the
paint slots; no atlas for either.**

| Content | Mechanism | Samplers | Shader variants | Why this and not the alternative |
|---|---|---|---|---|
| **9 print classes** | **Texture2DArray, 1024² × 9 slices**, class selected by a **float layer index uniform** | 1 | **0** | An **atlas** would need a per-class UV rect (another uniform — workable) *and* would make all nine classes share one mip chain. At 64 px a stroke is 2 px wide; a shared mip chain is exactly where cross-class bleed happens, and the P2 class-to-class separation of **≥ 5.0% luma** is the contract being defended. An array gives each class its own mip chain for free. A **multi-channel** packing is impossible — nine classes exceed four channels — and splitting into two RGBA textures costs a second sampler and a blend between them |
| **6 paint slots** | **Six channels in the base material** — six runtime 512² RGBAHalf render targets (§8.4), RGB = pigment at Tier-0 load, A = coverage | 6 | **0** | Slots are **not** a choice at draw time — a creature can be carrying marks in several slots simultaneously (§5.5's compaction and overlay rules). A layer index would have to be per-slot per-fragment. **Channels are the only mechanism that composites six simultaneous layers in one pass**, and it is why §5.7 records paint as **0 tris, 0 draws** |
| **Streak patterns (R2)** | 4 single-channel 512² maps in the array of the base material | 1 | **0** | Variant index is a **uniform**. R2 streaks are *authored art*, but the choice between four authored variants is data, not a shader permutation |
| **Feather ramp (R3)** | 64 × 4 single-channel | 1 | **0** | A ramp, not a colour lookup |

**The invariant:** *every* selector in the creature material is a **uniform or a
channel**. Nothing is a keyword. That single discipline is the entire reason the
paint and print systems cost **zero of the 200-variant preferred budget**, and it
is the test for item 5 (§8.2).

**Determinism, and it is a save-data requirement.** The streak variant index is
derived from the mark's **stored stroke index**. **Never randomised at runtime.**
A mark that reloads with different streaks is a mark that did not survive
save/load — which `technical-preferences.md` names as the most severe failure in
this game.

#### 8.5.4 Compression — ASTC for Android, ETC2 as fallback

| Platform | Format | Permitted quality |
|---|---|---|
| iOS | **ASTC** | Per §8.5.5 ladder |
| Android (primary) | **ASTC** | Per §8.5.5 ladder |
| Android (fallback) | **ETC2_RGBA8** | Only where ASTC is unsupported; must be verified on one ETC2 device before ship |

**ASTC quality ladder, by map character:**

| Map character | ASTC | Why |
|---|---|---|
| **Soft-feathered or gradient** — the R3 **0.06 H** feather ramp, the R2 streak maps, the UI atlas's shadows | **4 × 4** (highest) | A 0.06 H feather is **3.84 px at 64 px** and must span **≥ 2 ladder rungs** (R3). Low-rate ASTC blocks that ramp into visible steps, and a stepped feather is a *hard-edged* mark — the opposite of what the child painted |
| **Print class masks, hook masks** — hard-edged class boundaries | **6 × 6** (default) | Class edges are the P2 contract and need edge fidelity, but the interiors are flat and compress to nothing |
| **Large flat UI fills** | 8 × 8 permitted | Nothing to preserve inside a solid pebble |

> **FORBIDDEN: Crunch (DXT) and DXT1/DXT5 on any gradient, ramp, feather, or
> soft-feathered map.**
>
> This is absolute. Crunch is a lossy DXT5 wrapper with block-level DXT1 colour,
> and its block grid lands directly on the R3 feather: at 64 px that feather is
> **3.84 px wide against a 4 × 4 texel block** — one block per feather. The
> output is not a soft edge, it is a **staircase**, and it re-introduces
> precisely the hard mark edge §5.5's placement policy spends three tiers
> avoiding. It also blocks on the L0–L5 print-mask value rungs and breaks P3's
> **≥ 1.6%** adjacent-rung separation by banded quantisation.
>
> DXT is additionally banned project-wide because it is not a target platform
> format for iOS or Android 6000.3, and a DXT asset in the project is a
> transcoded intermediate that will eventually be shipped by accident.

**Texture filtering, all maps:** bilinear, mipmaps **ON** for print, hook, and
UI maps; **mipmaps OFF** for the 64 × 4 feather ramp and for every stroke target
(there is no smaller-than-one view of a ramp that means anything).

#### 8.5.5 Text, glyphs, and numerals — forbidden in every texture

> **No texture in this project may contain text, a glyph, a numeral, a stamp, a
> letterform, or handwriting-like marks.** No exceptions, at any resolution, on
> any asset class, including UI atlases.

**The mechanism is threefold, and all three must hold:**

1. **§5.5 / §4c.6 — on the body.** The nine print classes deliberately contain
   **no glyph class** (§3.2), and a tenth class shaped like handwriting is
   forbidden. There is no caption, name, score, or stamp on the creature at any
   size. The child's name lives on the **record card** (§4c role 5), and only
   the child's own hand appears there.
2. **§4c — in UI.** Text is rendered by the text system at runtime from the
   §4c type stack. Nothing is rasterised into an atlas. An atlas entry that
   looks like a letter is a defect.
3. **Hand-off — the record card's handwriting.** If the record card renders the
   child's hand, it must be **runtime stroke data (vector polylines) rendered by
   the UI layer**, never a baked glyph texture and never a hand-authored
   imitation of handwriting. **Owner: `ui-programmer`, with `ux-designer`** for
   the stroke-data format. This is outside an art bible's scope and is named here
   so it is not lost.

**Test, binary:** open every PNG in `Assets/` at 200% and scan for any connected
component whose aspect ratio, stroke rhythm, or glyph-like topology suggests a
letterform. Any hit is a fail. This is a **§4c.6 legal-and-pedagogical**
requirement, not an aesthetic preference — the record card is the one place the
child's own authorship may be recorded, and only their hand may appear on it.

---

### 8.6 Material and shader variant budget

#### 8.6.1 The rules that hold variants ≤ 200

| # | Rule | Enforcement |
|---|---|---|
| **V1** | **The creature material declares ZERO keywords.** No `multi_compile`, no `shader_feature`, no Shader Feature toggles, no `ShaderGUI` keyword writes in `OnValidate` | Build variant report. Creature material = **1 variant** |
| **V2** | **Every toggle is a uniform.** Layer index, class index, streak variant, gloss, relief depth, streak strength, live/record toggle, slot occupancy ×6 | Source review. A toggle that appears as a keyword is a V2 fail |
| **V3** | **No fog variant on the creature.** The creature shader declares no `multi_compile_fog`. Scene fog would modulate albedo across the body and break the ±1.0% flatness contract | Source review |
| **V4** | **SRP Batcher compatibility is mandatory.** All material properties in a single `UnityPerMaterial` CBUFFER, all instances in the same shader | Batcher compatibility report must be 100% for the creature |
| **V5** | **No new shader without approval.** A `.shadergraph` is created by the **shader specialist** against a written spec from the **art director**, and is not an artist-side decision | PR check |
| **V6** | **No `ShaderVariantCollection`** without an ADR. This project has **12 variants against a 200 ceiling.** A variant collection at 12 variants is machinery built for a problem that does not exist — the definition of Pillar 3 leakage | PR check |
| **V7** | **Render Graph mode only.** No Compatibility Mode path (removed in 6.3 per `technical-preferences.md` forbidden patterns) | Build config |
| **V8** | **The variant ledger is a committed file.** Every material declares its expected variant count. A drift between declared and actual is a **build failure**, not a warning | Build check |

#### 8.6.2 The variant ledger — the whole project, today

| Material | Shader | Keywords | Variants |
|---|---|---|---|
| `mat_dab_shell_body` | `shd_dab_shell` | **0** | **1** |
| `mat_hook_*` (3 materials, 1 shader) | `shd_dab_shell` | **0** | **1** |
| `mat_env_ground_bowl` | `shd_dab_ground` | **0** | **1** |
| `mat_env_sky_dome` | `shd_dab_sky` | **0** | **1** |
| `mat_env_sun_disc` | `shd_dab_sun` | **0** | **1** |
| `mat_prop_pebble` / `_arch` / `_tuft` | `shd_dab_prop` | **0** | **3** |
| `mat_echo_family` (6 instances) | `shd_dab_echo` | **0** | **1** |
| `mat_ui_worldflat` | `shd_dab_uiflat` | **0** | **1** |
| `mat_ui_core` | URP Unlit | **0** | **1** |
| `mat_fx_*` (marks, blob, burst) | `shd_dab_fx` | **0** | **3** |
| **Declared total** | | | **12** |

**12 of 200. 6% of the preferred ceiling. 188 remain unspent, deliberately.**

That headroom is not an invitation. §5.5's six slots and §4.5's nine print
classes are *already* paid for at zero variants, and the correct response to
"we have 188 spare variants" is to ship nothing further. Every variant spent
past this ledger is content bought with the budget that should have gone to
finishing the one thing.

#### 8.6.3 The escalation path — an ADR, not a keyword

> **If the variant ledger is ever exceeded, the response is an Architecture
> Decision Record. It is never a keyword.**

The failure mode this rule exists to prevent is specific and common: an artist
or programmer hits an inconvenient shader state, reaches for `_FEATURE_ON`, gets
moving in ninety seconds, and the cost surfaces six weeks later as a
three-minute shader-compile stall on a low-end Android. The cost was paid in
warm-up time on the exact device class this project is floored to (§8.1).

**Escalation, in order, all four steps required:**

| Step | Action | Owner |
|---|---|---|
| **1** | **Prove it is a defect, not a habit.** Grep the project for the keyword. If the demand is *"I need a bool"*, it is a **uniform** (V2) and the ADR is rejected at this step. An ADR is only for a demand that is genuinely a compile-time branch | Requester |
| **2** | **File `ADR-NNN-variant-budget-exception`.** Must state: the exact keyword; the consuming material; the measured variant cost; **the specific removal trigger and the condition that would let it be deleted**; and what art outcome fails without it | **technical-artist** drafts |
| **3** | **Review.** Feasibility and render-pipeline correctness: **unity-specialist**. Vision cost — is this the *one thing finished* or a second thing? **creative-director** | US + CD |
| **4** | **Register.** Add the row to the §8.6.2 ledger with its ADR ID. The build check (V8) is updated in the same commit. No keyword ships unregistered | US |

**Explicitly not an escalation path:** lowering the threshold to make a test
pass; adding the keyword "temporarily"; merging with no ledger row. Item 5 in
§8.2 is the worked example — a per-slot keyword would be the *entire* reason
the paint system stops costing zero variants, and it would be worth about
fourteen variants, which is cheap, and it would break the design. Budget
headroom is not permission.

---

### 8.7 Import settings — Unity 6.3 URP presets

**One-pass checklist. Every row is OFF or ON. No row has a judgement state.**
Applied as Unity **AssetPostprocessor** presets (owned by **technical-artist**)
so that a bad export cannot reach `Assets/` unflagged.

#### Model importer

| Setting | Creature body / HOOK | Ground / sky / sun | Props / Echo |
|---|---|---|---|
| **Scale Factor** | **1.0** | **1.0** | **1.0** |
| **Use File Scale** | **OFF** — file scale silently rescales the canvas | OFF | OFF |
| **Convert Units** | **OFF** — §4c/§5 measure everything in **H** (body height), not metres | OFF | OFF |
| **Import Normals** | **ON**, `Import` | ON | ON |
| **Normals — Method** | **Angle Weighted** | Angle Weighted | Angle Weighted |
| **Normals — Angle** | **60°** | 60° | 60° |
| **Import Tangents** | **OFF** (§8.4 — no normal maps ship) | OFF | OFF |
| **Generate Lightmap UVs** | **OFF** — no baked lighting in this project | OFF | OFF |
| **Mesh Compression** | **OFF** (§8.4) | **OFF** | **Medium** |
| **Optimize Mesh** | **OFF** — it reorders vertices and can invert the mark UVs | OFF | OFF |
| **Read/Write** | **OFF** — no mesh colliders (§8.4) | OFF | OFF |
| **Index Format** | **32-bit** — 12,400 tris is trivial, but pinning it prevents a future silent promotion that doubles vertex buffers | 32-bit | 16-bit |
| **Weld Vertices** | **OFF** — welding across a seam fuses two UV islands | OFF | OFF |

#### Texture importer

| Setting | **Colour maps** (UI atlas, icons, any albedo) | **Data / mask maps** (print array, hook masks, streaks, feather ramp, roughness, normal) | **Stroke targets** (runtime `RenderTexture`) |
|---|---|---|---|
| **sRGB (Color)** | **ON** | **OFF** — mandatory. A mask sampled as sRGB shifts every rung and breaks P3's ≥ 1.6% adjacent-rung separation | **OFF** |
| **Texture Type** | `Default` / `Sprite (2D and UI)` | **Default** — never `Sprite` | `Render Texture` |
| **Mip Maps** | **ON** | **ON** for print/hook/streak · **OFF** for the 64 × 4 feather ramp | **OFF** |
| **Wrap Mode** | **Clamp** | **Clamp** — mandatory (§8.4). No `Repeat` anywhere in this project | **Clamp** |
| **Filter Mode** | `Bilinear` | `Bilinear` | `Bilinear` |
| **Aniso** | **1** — creatures are near-frontal; higher is cost without read | **1** | 0 |
| **Compression — Android** | **ASTC**, per §8.5.5 ladder | **ASTC** | n/a — runtime format `ARGBHalf` |
| **Compression — iOS** | **ASTC**, same ladder | **ASTC** | n/a |
| **Compression Quality** | **50 (Normal)** on gradients and soft-feathered maps · **75 (High)** on hard-edged class masks | **75 (High)** | n/a |
| **Crunch** | **FORBIDDEN** (§8.5.5) | **FORBIDDEN** | n/a |
| **Max Size** | **2048** (UI atlas only); **128** for icons | **1024** (print array) · **512** (hook, streak) · **64** (feather ramp) | **512** |
| **Resizing Algorithm** | `Mitchell` on soft maps — it preserves smooth ramps better than bilinear | `Mitchell` | n/a |
| **NPOT Scale** | **None** — every asset is power-of-two | **None** | n/a |

**Texture2DArray import specifics (print mask, hook masks):** `Texture Shape`
= **2D Array**; `sRGB` = **OFF**; `Enable Mip Maps` = **ON**; `Wrap Mode` =
**Clamp**; `Array Size` written in the filename per §8.3 so the slice count is
visible without opening the asset.

**Enforcement, binary:** a build-time importer validator fails the build if any
texture contradicts this table. It is a table, so it is a test — there is no
"it looked fine in the inspector" state. Owner: **technical-artist** implements;
**unity-specialist** owns the build check.

---

### 8.8 Audio standards

Audio is the child's second channel (§4.3 C3 — colour is third, not first), and
per §5.6 a child who cannot see an Echo clearly can still name it by ear. These
are asset standards, not mix standards.

#### Format

| Property | Value | Why |
|---|---|---|
| **Sample rate** | **48,000 Hz**, single rate project-wide | The Unity default and the video rate. A 44.1 kHz asset resamples on load and shifts loop phase |
| **Bit depth (source)** | **24-bit PCM** in the repo | Headroom for the ≥ 12% streak-contrast-style headroom of the wet-paint transient without clipping |
| **Channels** | **Mono for all SFX, loops, and event one-shots.** The world is a single continuous space; a stereo bed would place a sound "over there", which §3.3's geometry deliberately denies exists | |
| **Platform encoding** | **ADPCM** for one-shots under **2.0 s** · **Vorbis** for loops and anything **≥ 2.0 s** | Both are Unity built-in importers, not third-party dependencies |
| **Loudness target** | Per-clip normalisation to **−18 dBFS RMS** for SFX; the ambience bed **−24 dBFS RMS** | A fixed reference so two artists' assets sit at the same loudness on first import |
| **True-peak ceiling** | **−1.0 dBTP on every clip. Hard limit. Zero clipped samples permitted** | Clipping on the wet-stroke creak — the confirmation that the child's mark landed — is the one artifact that would make the reward feel broken |
| **Normalisation** | **Per-clip, never per-bus, never per-frame.** No adaptive or ducking logic in the asset domain | Loudness that changes by itself is the adult world leaking in (§8.8.4) |
| **Loop format** | Exact integer sample counts, crossfade **0** | See §8.8.3 |

#### 8.8.1 The six Echo registers

| Register | Family | Note | Role |
|---|---|---|---|
| **E1** | **Column** | **Highest** | |
| **E2** | **Drape** | Second | |
| **E3** | **Sweep** | Third | |
| **E4** | **Orbit** | Fourth | |
| **E5** | **Halo** | Fifth | |
| **E6** | **Buds** | **Lowest** | |

**Spans a major sixth, E1 down to E6.** Two Echoes never share a register — a
child who cannot see an Echo clearly can still name it by ear. One clip per
register, six clips, no overlap. Asset names carry the register (§8.3), so a
duplicate register is visible in a directory listing.

#### 8.8.2 The fixed figures — immutable, not re-tuned per performance

| Figure | Spec | Rule |
|---|---|---|
| **Flourish tone figure** | **12 notes, FIXED** | **Never re-composed, never re-transposed, never re-tuned, never re-timed by performance, per asset.** One asset exists. Every Flourish plays that asset |
| **Success burst** | **12 spokes, FIXED** | One asset. §2.6 |
| **Purr under Menus** | **−4 dB** relative to Ambient | A fixed gain on a fixed asset, not a second asset. The creature is never paused in a menu (§5.6) |

**Why the figures are immutable at the asset level, not just at the code
level.** A figure that may be re-tuned per performance is a figure with more
than one correct value, and the second performer to re-tune it differently is
the last one. §5.6's 12-note figure is *the sound of the child's work being
celebrated* — it is the reward, and a reward that varies is a reward the child
cannot predict and therefore cannot anticipate. **Immutable in the asset.
Owner: sound-designer authors, art director owns the rule, QA verifies
byte-identity** (hash the asset against the manifest; any drift is a build
failure).

#### 8.8.3 Loop durations — exact sample counts

| Loop | Duration | Samples @ 48 kHz | Constraint |
|---|---|---|---|
| **Breathe** | **4.0 s** | **192,000** | Paired with spine scale Y 1.00 → 1.03 → 1.00, **ears and hook lag 200 ms** |
| **Sway** | **6.0 s** | **288,000** | **Different period from breathe.** Body lean ±3° |
| **Painting breathe** | **5.2 s** | **249,600** | Sway **off**. Slower = absorbed |
| **Minigame** | **2.0 s** | **96,000** | **Matched to gesture cadence, never to a timer** (§2.6) |
| **Echo_Base** | **0.8 s in / 2.0 s hold / 0.6 s out** | 38,400 / 96,000 / 28,800 | Three segments, one asset |

**Seamless-loop test, binary.** Two conditions, both required:

1. **Zero discontinuity:** `|s[0] − s[N−1]| ≤ 0.5 LSB`. Any click at the loop
   point is a fail. Measured on the *source* WAV, before encoding.
2. **No DC offset:** mean absolute amplitude over one period ≤ **0.5% of peak**.
   A DC offset in a 4.0 s loop is an inaudible thump every 4 seconds.

**The 4.0 s / 6.0 s pairing.** Their shared period is `lcm(4, 6) = 12.0 s`, so
the two loops become phase-aligned every twelve seconds. §5.6 requires them to
"never visibly sync." The audio-side obligation that makes that true: the two
loops' **amplitude envelopes must not be the same shape**. If breathe is a
symmetric swell, sway must not be. Verify on a 12.0 s render: the combined
envelope must show **no repeating maximum**. A visible every-12-second pulse is
a fail.

**Painting breathe 5.2 s.** Deliberately coprime with nothing that shares its
period, and paired with **sway off** — the world goes still so the child's touch
is the only event (§2.4). Any asset that re-introduces motion audio during
Painting is a fail.

#### 8.8.4 What no audio asset may ever do

> **No audio asset in this game grades the child.**

| Forbidden | Because |
|---|---|
| Stingers on failure, error, or loss | Pillar 2. There is no failure state, so there is no sound for one |
| Rising tension, urgency, or "hurry" cues | The product has no timer, no streak, no fail — urgency is an adult mechanic |
| Random or procedural variation in a fixed figure | §5.6. The 12-note Flourish is one asset |
| Adaptive loudness, per-frame normalisation, or ducking | A child must be able to predict the volume of every sound |
| Any sound that plays *before* the child acts | Anticipation is shape and motion. Audio confirms, it does not anticipate |
| Anything resembling applause, crowd, or judgement | The pet's response is the only audience |

Enforcement: the sound library is a **closed manifest**. Any new clip requires a
row in that manifest with a stated §2 state and a written reason it cannot be
misread as a grade. **No row, no clip.**

---

### 8.9 Font asset rules

Two families. Both **SIL Open Font License 1.1**.

| Family | Role | Licence | Source of weights and sizes |
|---|---|---|---|
| **Nunito** | Primary UI family | **SIL OFL 1.1** | §4c.2, §4c.3, §4c.4 |
| **Patrick Hand** | Child-adjacent / handwriting-adjacent family | **SIL OFL 1.1** | §4c.2, §4c.3, §4c.4 |

**The exact weights, sizes, and role assignments are pinned in §4c.2–§4c.4 and
are not restated or reinterpreted here.** This section governs only the *asset
rules* for shipping those two families.

| # | Rule | Test |
|---|---|---|
| **F1** | **The OFL 1.1 licence text ships in the app's third-party notices.** Both families, both notices, reachable from the title screen without a network call | Notices panel contains `OFL.txt` for Nunito **and** Patrick Hand. Missing either is a **licence-compliance fail** and a ship blocker |
| **F2** | **Exact weights pinned.** One TMP font asset per pinned weight. No weight range, no variable-font axis in a text component | Every TMP font asset's weight matches a §4c.4 pinned value. Any unpinned weight is a fail |
| **F3** | **Subsetting is permitted** under OFL 1.1 — with conditions | A subset font is a *modified* font. Conditions: (a) it is **not sold alone**; (b) **if the upstream OFL declares a Reserved Font Name, a modified font must not use that name** — Nunito and Patrick Hand must be checked against their own `OFL.txt` before any subset ships; (c) the OFL notice ships with it (F1). **Owner: art director** to confirm RFN status from upstream `OFL.txt`; **legal/store** review before ship |
| **F4** | **No substitution. No metric-changing fallback.** | Every TMP font asset has an **empty fallback list**. If a glyph is missing, it must be a **build failure**, never a runtime fallback to a different family — a fallback changes advance widths, which changes line breaks, which changes where a label sits relative to its icon. §3.4 requires icon-first layout; a shifted label is a broken layout |
| **F5** | **Character set is closed and deliberate.** The atlas is generated at build time from the localisation string table — not hand-populated | Atlas glyph count == the string table's character set. A stale hand-populated atlas is a fail |
| **F6** | **No baked text in any texture** (§8.5.5). All text is rendered at runtime by the text system | No texture contains a glyph (§8.5.5 test) |
| **F7** | **No third-party font asset may enter `Assets/` without an ADR.** This project has two families. Two is the whole list | Directory listing of `Assets/` fonts contains exactly the §4c families plus documented subsets |

**Hand-off:** F3's Reserved Font Name determination and F1's notices-panel
placement are **not** art-bible decisions. **Owner: art director** to determine
RFN status; **ui-programmer** to build the notices panel and the build-time
atlas generation; **producer** to confirm store-side licence disclosure.

---

### 8.10 Loading strategy

**Target: cold start to interactive < 5 s preferred, 8 s hard ceiling** on the
low-end Android floor device (§8.1). **Preferred budget: 4.6 s, holding 0.4 s
of headroom.** Headroom is not waste — a low-end device that hits 4.6 s on a
bench may hit 5.4 s on a shelf with a warm-but-not-hot SoC.

#### The timing budget

| Phase | Target | Contents |
|---|---|---|
| Engine boot + IL2CPP init + URP init | **1.8 s** | Not art-controllable |
| First scene load | **0.6 s** | The scene below, all synchronous |
| **First paint-ready** | **2.4 s** | Print mask array + hook mask array resident; stroke targets allocated. **The child must be able to paint the instant the first frame appears** |
| First frame presented | **4.6 s** | Idle Ambient live, 60 fps |

#### What loads in the first scene — everything the child can see or hear in Idle Ambient

| Group | Contents | Cost |
|---|---|---|
| Creature | Body mesh, HOOK mesh (default type), the creature material, `mat_hook_*` × 3 | 12,400 tris, 2 draws |
| Print + hook masks | `arr_dab_print_class_9slice` (1024² × 9, 4.2 MB), `arr_hook_mask_3slice` (512² × 3, 0.35 MB) | **Blocking.** These are required for the child's first mark. There is no streaming fallback for a texture the canvas needs |
| Streaks + feather | 4 × 512² streaks, 64 × 4 feather ramp | 0.48 MB. Blocking — R2 is the authorship signature |
| Environment | Ground bowl, sky dome, sun disc, 3 prop base meshes | 1,064 + 3,200 tris, 6 draws |
| Stroke targets | **6 × 512² RGBAHalf** | 12.6 MB, allocated at first scene. **This is the single largest art-side memory allocation and it is up front on purpose** — a stroke that lands in an unallocated target loses the mark, which is the game's most severe failure |
| UI | `atl_ui_core` 2048², `atl_ui_common`, both font atlases (F5) | 1.86 MB + fonts |
| Audio | Purr fundamental loop, the four idle baselines' clips (breathe 4.0 s, sway 6.0 s), one chime, the seven menu clips | Mono, small |
| Shaders | **All 12 variants**, warmed | Warmed in first scene because §8.6.3 makes shader-compile stall an explicit failure mode on this device class |

**Everything above is in the first scene.** Nothing above is optional at first
frame. The reason: a child's first five seconds is the entire first impression of
the product, and a first frame that pops in a mask array is a first frame that
teaches the child the canvas can be blank sometimes.

#### What streams, and when

| Content | Loads at | Justification |
|---|---|---|
| Remaining **2** HOOK types' mesh data | On hook change (§5.2), ≤ 200 ms hitch absorbed by the 0.9 s `Greet_Return` | The child is never mid-stroke while a hook changes |
| **Alternative TILT vectors** | Never streamed. **Computed, not stored** | §5.6 rule 1: one clip set retargets to all 65,856 TILT vectors. TILT is vertex weights, not assets |
| Minigame-specific audio | On §2.6 entry, ≤ 300 ms, during the `Anticipate_Ability` 0.30 s | Anticipation covers the load |
| Remaining **22 print-class-aware** material property sets | **Not loaded.** Print class is a **uniform** on one already-resident material (V2) | Zero variants means zero per-class assets |
| Any **new** variant | **Never** (§8.6.3) | |

**The load-time rule that follows from §5.5:** *if a child can reach it within
one tap of first frame, it is in the first scene.* The six stroke targets exist
because slot 6 (Front plate) is the slot guaranteed on-screen in every state
(§5.5) — so all six must be ready, immediately, because a child can paint on
the front plate in Idle Ambient.

#### Addressables — an explicit constraint, not an omission

`technical-preferences.md` records `com.unity.addressables` as **Deferred, not
installed**: *"Offline-first game with no remote content catalogs; meaningful
mobile package weight for no current benefit."* **This section does not assume
Addressables.**

| Question | Answer under the current constraint |
|---|---|
| What is "addressable"? | Everything required for first-frame interactive is in **Build Settings scene 0** as synchronous references. There is no catalog, no remote bundle, and no dynamic download |
| Second scene | Loaded **synchronously** on transition, budgeted **< 400 ms**. There is one scene per §2 state cluster at most, and the design's Pillar-3 shape means there are very few |
| Remote content | **None. The game is fully offline.** No catalog is permitted to exist, because a catalog implies a network path and `technical-preferences.md` forbids network calls on the painting or input paths |
| **When Addressables is adopted** | **Hand-off: `unity-addressables-specialist`** (already routed for in `technical-preferences.md`) to re-derive this section's loading table against real bundles. Until that hand-off is accepted, this table stands and the measured 4.6 s is the contract |

---

### 8.11 Definition of done — the artist's binary pre-submit checklist

**An artist runs this before submitting any asset. Every box is a fact, not a
judgement. Any unchecked box is a rejected submission — there is no partial
submission and no "reviewer will notice".**

#### A. Naming (§8.3)

- [ ] Filename matches `^[a-z0-9]+(_[a-z0-9]+)*$` — lowercase, single underscores, no spaces, no hyphens, no truncation
- [ ] Every token is a full word — no `bdy`, `creat`, `tex`, `matl`, `anim`
- [ ] Filename follows this category's pattern in §8.3, including the worked example
- [ ] Any reserved token used (`low`/`high`/`mask`/`alpha`/`a`/`b`/frame index) means exactly what §8.3 says
- [ ] If it is a `.cs` file: PascalCase matching the class name exactly (`technical-preferences.md`)

#### B. Mesh (§8.4)

- [ ] Triangle count within its class budget **and** its absolute cap in §8.4.1
- [ ] Vert count ≤ its cap; faces ≤ its cap
- [ ] **Zero n-gons with 5+ vertices** (creature, hook, props)
- [ ] **UV0 only.** One 0–1 layout. No second channel, no tiling UV, no lightmap UV
- [ ] All UV islands non-overlapping, each with a **16-texel bleed border**
- [ ] `Wrap Clamp` on every map — **no `Repeat` anywhere**
- [ ] P5 feature check: thinnest mask feature resolves at **≥ 82 texels** (0.08 H at 1,024 px/H)
- [ ] Six paint slot sub-rects present, each ≥ its §8.4.4 minimum, each clear of the other five
- [ ] Cheek-L and Cheek-R clear **all three** hook silhouettes at maximum ear spread
- [ ] Auto-smooth angle **60°**; custom hard edges **named and ≤ 12**; zero unnamed hard edges
- [ ] Tangents **OFF** (no normal map ships)
- [ ] Mesh compression **OFF** on creature, hook, ground, sky, sun
- [ ] Vertex colour bakes to uniform **(1,1,1,1)** on creature/hook; props and Echo are **(1,1,1,α)**, α ∈ [0,1]
- [ ] Primitive collider only. **No `MeshCollider`**
- [ ] The base mesh carries **zero appendages** — the HOOK is the only attached form (§5.8)

#### C. Texture (§8.5)

- [ ] Resolution matches §8.5.1 exactly; power-of-two
- [ ] `sRGB` **ON** for colour, **OFF** for every mask/data map
- [ ] **No text, glyph, numeral, stamp, letterform, or handwriting-like mark** in this texture — §8.5.5
- [ ] Print array slice count in the filename matches the asset's actual slice count
- [ ] Print islands laid out Band A / B / C per §8.4.3, largest class first
- [ ] ASTC, quality per the §8.5.5 ladder. **No Crunch. No DXT1/DXT5**
- [ ] Mipmaps ON for print/hook/streak/UI; **OFF** for the feather ramp
- [ ] `Wrap Clamp`. `Filter Bilinear`. `Aniso 1`
- [ ] `Max Size` set to the class value; `Resizing Algorithm` = Mitchell on soft maps
- [ ] Compression **Quality 50 on gradients and soft feathers, 75 on hard-edged masks**
- [ ] Shell / environment / Echo submissions are correctly **zero-texture** — I did not add a texture to a surface §8.5.2 says must be flat

#### D. Material and shader (§8.6)

- [ ] Expected variant count is **registered in the §8.6.2 ledger**
- [ ] Built variant count **matches** the ledger. Any drift is a build failure, not a warning (V8)
- [ ] **Zero keywords** added by me. Every toggle I needed is a uniform (V2)
- [ ] No new `.shadergraph` was created by me — shaders are the **shader specialist's** (V5)
- [ ] No `ShaderVariantCollection` added (V6)
- [ ] SRP Batcher compatibility: **all** properties in one `UnityPerMaterial` CBUFFER (V4)
- [ ] Render Graph mode only; no Compatibility Mode path (V7)
- [ ] **No fur, fibre, strand, hair-card, or shell-layer anything** — item 9. I did not add one, and I did not add one "temporarily"
- [ ] Shell albedo is a flat colour constant, flatness **±1.0% luma** verified across UV0
- [ ] No AO, no mottling, no baked shadow, no vertex tint on the shell

#### E. Animation (§5.6)

- [ ] Clip is one of the **20** in the closed set. A 21st name is a fail
- [ ] Duration is exactly as specified in §5.6
- [ ] **No squash, stretch, or overshoot baked.** Those are procedural, in code (§5.6 rule 3)
- [ ] No IK, no constraints, no spring bones, no clip events
- [ ] Retargets to **all** TILT vectors. No per-variant clip was authored (§5.6 rule 1)
- [ ] One rig, **< 45 bones**, Base + Echo additive layers only
- [ ] Hook clips: shared **6-frame** spring only

#### F. Audio (§8.8)

- [ ] **48,000 Hz** mono, 24-bit PCM source
- [ ] Added as a row to the **closed manifest**, with a §2 state and a written reason it cannot be misread as grading the child
- [ ] Loop: exact integer sample count; `|s[0] − s[N−1]| ≤ 0.5 LSB`; DC offset ≤ 0.5% of peak
- [ ] **True peak ≤ −1.0 dBTP.** Zero clipped samples
- [ ] Echo clip: register is correct and **unique** across the six
- [ ] Fixed figures (12-note Flourish, 12-spoke burst) are **untouched byte-identical** assets
- [ ] No stinger, no urgency cue, no random variation, no adaptive loudness

#### G. Font (§8.9)

- [ ] Font asset is **Nunito** or **Patrick Hand** — there is no third family (F7)
- [ ] Weight is pinned in §4c.4
- [ ] Fallback list is **empty** (F4)
- [ ] OFL 1.1 notice is shipped in third-party notices (F1)
- [ ] If subset: Reserved Font Name status confirmed against upstream `OFL.txt`, and the subset is not sold alone (F3)

#### H. Budget self-check (§8.1)

- [ ] I have measured this asset's triangle, draw-call, variant, and memory contribution
- [ ] Nothing I added pushed the **peak scene above 32,424 tris or 9 draws** for its class
- [ ] Nothing I added pushed the **declared variant total above 12**
- [ ] This asset does not make a *second thing* where the design intends *one finished thing* — **Pillar 3**

---

### 8.12 Reviewer checklist

**Run this on every submission. Greyscale first — colour fully removed, at 64 px,
front-on, no motion (§5.8 protocol). An unchecked box is a rejection.**

#### Gate 1 — naming and structure (§8.3, §8.4)

- [ ] Filename passes the §8.3 regex with **zero** judgement calls
- [ ] Category pattern and worked example match
- [ ] No truncation, no reserved-token misuse, no PascalCase outside `.cs`
- [ ] Triangles, verts, and faces are all within cap — **numbers, not adjectives**
- [ ] UV0 only; one 0–1 layout; islands non-overlapping with 16-texel bleed
- [ ] `Wrap Clamp` on everything; **no `Repeat`**
- [ ] Mesh compression OFF on creature, hook, ground, sky, sun
- [ ] Vertex colour is uniform (1,1,1,1) on the creature
- [ ] Zero n-gons ≥ 5 verts; unnamed hard edges = 0

#### Gate 2 — the canvas is flat (§5.5, §8.2 items 1 and 9)

- [ ] Shell albedo flat to **±1.0% luma** across the entire body, sampled on a 64 × 64 UV grid — **measured, not eyeballed**
- [ ] **Zero** fur, fibre, strand, hair-card, shell-layer implementation — **item 9, all three audit parts**
- [ ] No AO, no mottle, no baked shadow, no vertex tint on the shell
- [ ] **Item 1 R5 board: all 18 cells pass** — 9 print classes × Cranberry (28.3) and Cherry Pop (33.4), at relief 0.02 H and 0.04 H, relief shoulder **and** ≥ 3 streak bands per 0.08 H both **≥ 3.0% luma** separation, greyscale, 64 px. **One failing cell rejects the asset**
- [ ] Item 6: **0.010 H** contact line present as a **≥ 1.0 px screen-space ramp**, not a hard sub-pixel edge
- [ ] Item 7: live eye additive **≥ 8%**, record-print eye additive **exactly 0.0%**
- [ ] Zero text, glyph, numeral, or handwriting-like mark anywhere in any texture (§8.5.5)

#### Gate 3 — texture and compression (§8.5, §8.7)

- [ ] Resolutions match the §8.5.1 ladder; all power-of-two
- [ ] `sRGB` **ON** for colour, **OFF** for every mask and data map
- [ ] **No Crunch. No DXT.** ASTC at the §8.5.5 quality tier
- [ ] Gradient and soft-feathered maps are **ASTC 4 × 4** — the 0.06 H feather is 3.84 px at 64 px and must not block
- [ ] Hard-edged class masks are **ASTC 6 × 6** or better
- [ ] Mipmaps correct per map type; feather ramp has **no** mips
- [ ] Print array slice count matches the filename; slice order matches Band A/B/C
- [ ] P5 minimum feature resolves at **≥ 82 texels**
- [ ] The six paint slot sub-rects are present, sized, and mutually clear
- [ ] Cheek-L/Cheek-R clear **all three** hooks at maximum ear spread

#### Gate 4 — variants and budget (§8.6, §8.1)

- [ ] Declared variant count in the ledger **equals** built variant count (V8)
- [ ] Creature material variant count is **1**; zero keywords (items 5, V1, V2)
- [ ] **No new shader** was introduced without a written spec (V5)
- [ ] **No `ShaderVariantCollection`** (V6)
- [ ] SRP Batcher compatibility is 100% for the creature (V4)
- [ ] Variant total is **≤ 12 declared**; any excess is an **ADR**, never a keyword
- [ ] Peak scene stays **≤ 32,424 tris and 9 draws** for the class
- [ ] Cast shadow cost is **0 tris, 0 draws**, Peach Shadow **0.28** (item 8)
- [ ] Authored texture total still within the **≈ 6.9 MB** declaration

#### Gate 5 — animation and audio (§5.6, §8.8)

- [ ] Clip is one of the **20**; duration exact; **no baked squash/stretch/overshoot**
- [ ] Retargets to all TILT vectors; one rig, **< 45 bones**; Base + Echo layers only
- [ ] Audio is **48 kHz mono 24-bit**; true peak **≤ −1.0 dBTP**; zero clipping
- [ ] Loop sample counts exact; `|s[0] − s[N−1]| ≤ 0.5 LSB`; DC ≤ 0.5% of peak
- [ ] Breathe 4.0 s / sway 6.0 s render to **12.0 s with no repeating maximum**
- [ ] Six Echo registers are unique; E1 Column highest → E6 Buds lowest, one major sixth
- [ ] **12-note Flourish figure and 12-spoke burst are byte-identical to the manifest** (hash-checked)
- [ ] Purr **−4 dB** under menus; painting breathe **5.2 s**; minigame **2.0 s**
- [ ] No audio asset grades the child (§8.8.4); every clip is a manifest row

#### Gate 6 — fonts and licence (§8.9)

- [ ] Only Nunito and Patrick Hand; weights pinned; fallback lists **empty**
- [ ] OFL 1.1 notices ship in third-party notices for **both** families
- [ ] Any subset has documented Reserved Font Name compliance

#### Gate 7 — the Pillar 3 gate

> **Read this one aloud. It is the only item on the list that cannot be measured,
> and it is the one that matters most.**

- [ ] **Does this asset make the pet better, or does it make the list longer?**
- [ ] Is every triangle, variant, and megabyte here serving *one* thing finished — the child's mark surviving intact, on a flat canvas, at 32 px, forever?
- [ ] If removing this asset would leave the child's experience unchanged, **reject it.**

---

## Section 9 — Reference Direction

This section closes the last gap named in the source documents. `AGENTS.md` §10
question 6 lists *"Visual reference games — not supplied. Blocking art bible
quality,"* and `design/gdd/game-concept.md` §7 records the same gap as the *"highest-value
missing input to the project's art identity."* Three references have now been supplied and
fixed:

| ID | Reference | It is the reference FOR |
|---|---|---|
| **R1** | **Toca Life World** | The world/room framing — warm domestic surfaces, a single continuous field, the feeling of a safe handmade place |
| **R2** | **Adopt Me!** | The pet-as-object read — one closed mass, one identifying feature, legible at thumbnail size |
| **R3** | **Nintendogs** | The living-pet read — lagged secondary motion, asymmetric idle timing, affection carried by the body |

The set is **closed at three.** Provenance, official sources, verification status, and the
legal terms for all three live in the standalone catalog at
**`design/art/reference-catalog.md`**. This section holds only the art direction.

Two structural notes before any reference is discussed.

**First — Sections 1 through 5 are frozen and outrank everything below.** This section
cannot loosen a number. Where a reference disagrees with the canon, the canon wins and the
reference is re-read for a different property. §9.3 states the rule in full; §9.2 exists
because the most likely failure in a reference-driven section is a reference slowly
acquiring authority it was never given.

**Second — two of these three are not 3D-realtime-rendered products, and pretending
otherwise is the section's biggest trap.** Toca Life World is 2D art with parallax; Adopt
Me! is Roblox geometry under a platform avatar economy; Nintendogs is a Nintendo DS title
targeting hardware eighteen years old. All three are cited for *framing, proportion,
timing, and readability* — the properties that survive a technology change. **None of them
is a rendering reference.** Our rendering is specified by §2.1, §3.1, §4 and §5, and it is
specified as URP on a mid-range Android phone. A reference that shows a technique we cannot
afford is not a shortcut; it is a budget error (§9.1, closing note).

---

### Reference 1 — Toca Life World

| Reference | What to take (specific, actionable, exact property to copy) | What to avoid (specific, with WHY it fails) | Canon link |
|---|---|---|---|
| **R1 — Toca Life World** | **1. One uninterrupted domestic floor.** Its rooms read as a single warm surface with objects *placed on* it, never walled off, and you can never lose your place. Copy the *property*: the world is one continuous curved field. **Ground bowl, radius 6 H, maximum 8% of radius in elevation change across the entire span. No holes, no cliffs, no steps, no partitions.** A single unbroken curve can never read as an obstacle, which is the whole point — the child must never meet a boundary in this world. | **The room as a container to fill.** Toca's Home Designer is an inventory problem: 90+ locations, hundreds of items, a catalogue that grows. We are taking the *framing* and refusing the *inventory*. `game-concept.md` §2 lists room decor as an explicit anti-pillar for v1 because it dilutes the Adorn verb. | §3.3 ground bowl; §3.3 environment prohibition 1 |
| **R1 — Toca Life World** | **2. Warmth carried by hue temperature, not by brightness range.** The shadow side of a cabinet in a Toca room is a warmer, slightly darker version of the same hue — never a desaturated grey, never a cool neutral. Copy: **shadow tint is fixed to Peach Shadow `#F6B9A0` at 0.28 opacity and never leaves its hue band; the darkest permitted diffuse surface anywhere is L3 Umber at 41.6% luma.** Depth in Dab comes from hue temperature moving warmer and value dropping one rung — never from value dropping toward black. | **The 2D-as-3D shortcut.** Toca's readability comes from having *no real lighting at all* — flat art plus parallax. Copying that look means copying the absence of a lighting rig, and §2.1 is categorical: there is **exactly one lighting rig**, states modulate it along named axes, and states do not author new lights. Take the framing, refuse the technique. | §2.1 shadow colour; §2.1 hard lighting law 1; §4.5 Rule V1; §4.1 Peach Shadow entry |
| **R1 — Toca Life World** | **3. "Put down by a person," not "staged by a set designer."** Toca furniture sits slightly imperfectly, which is what makes it read as a real room rather than a display case. Copy the *read*, and in Dab it is delivered by **placement rhythm and surface finish, never by albedo variation.** Concretely: props carry 2% idle sway (§3.5); prop families are instanced from three base meshes — pebble, arch, tuft (§3.3); and the handmade quality lives in the clay-and-wax *finish* of the style anchor, with a soft terminator and a faint hand-moulded irregularity in the silhouette. **The creature's shell is exempt and stays blank: ± 1.0% luma across the entire body.** | **Per-room palettes.** Each Toca location reads as its own colour scheme. We have **seven colours in the entire product** and they do not change by location or by state. A per-room palette is a new colour, and a new colour is an ADR plus a Pillar 1 and Pillar 3 review. Rooms in Dab are the same room under different light — which is the same claim §2.8 makes about menus. | §3.5 rule 5 / 2% sway; §3.3 three prop families; §5.5 shell albedo flatness; §4.4.4 rule 3 |
| **R1 — Toca Life World** | **4. Scale reassurance built into the furniture.** The reference figure in a Toca room is small relative to the space: the floor is wide, the objects are large, nothing towers. Copy as a composition guarantee: **nothing in the world is taller than 0.6 H, and the creature's head is the highest point in frame in every state, every camera angle, every minigame layout.** | **A front door, a gate, a wall, a cliff.** Toca's rooms have doors and boundaries because Toca has errands and destinations. We have neither. A door or an edge reads as a limit, and a limit reads as *a place where you are not allowed to go* — which to a six-year-old is a problem, and §1.4 Principle 2 forbids showing problems. Zero barriers, zero hazards, zero places the creature can be blocked. | §3.3 environment prohibition 1; §3.3 rule 6; §3.5 rule 3; §1.4 Principle 2 |
| **R1 — Toca Life World** | **5. Nothing in the room is urgent.** No badge, no alert dot on a door, no countdown over a task, no red mark anywhere on the architecture. Copy as an asset rule: **no asset in this product may convey threat, error, urgency, scarcity, or neglect — and no string carries an exclamation mark, a timer, a count, or a score word.** Unavailable is 88% scale and one rung down, in warm cream. Never greyed, never locked, never crossed out. | **The weekly-gift cadence.** *"Gifts to be collected every Friday"* is a retention calendar wearing an art-bible hat. It is a cadence decision, not an art decision, and it collides with §2.7: nothing new arrives during Results, and the Results frame is the punctuation mark at the end of the session. A recurring reward tick converts a comfort object into a chore, which `AGENTS.md` §2.1 forbids outright. | §1.4 Principle 2; §3.4 disabled state; §2.7; §4c.6 and §4c.8 string bans; `AGENTS.md` §2.1 |

---

### Reference 2 — Adopt Me!

| Reference | What to take (specific, actionable, exact property to copy) | What to avoid (specific, with WHY it fails) | Canon link |
|---|---|---|---|
| **R2 — Adopt Me!** | **1. Silhouette first, everything else second.** Its pets are readable as 48 px inventory thumbnails because the silhouette is a **single closed mass carrying one dominant identifying feature** — and because the identifying feature is deliberately large relative to the creature, not a detail. Copy the property in our units: **closed rounded mass at 64 px, front-on, greyscale; hook footprint ≥ 0.122 H²; body overlap ≥ 0.049 H²; new outline ≤ 0.073 H², all measured on the final TILT-applied silhouette.** | **Faceted, blocky, low-poly construction as a style.** That silhouette language is a Roblox engine and avatar-economy constraint, not an art direction. Our canon is the opposite: base body authored at **10.0k triangles** in the 8–12k band, minimum corner radius **35%** of a form's shortest local axis, no polygon edge may read as a corner, no faceting. | §3.1 hard geometry rules 1 and 4; §5.1 canonical form table; §5.7 triangle budget |
| **R2 — Adopt Me!** | **2. Exactly one identifying feature, and it is the thing you name the creature by.** Copy as a hard system rule: **one base mesh + one rig + exactly one HOOK + a TILT vector + a PRINT. No fourth channel, ever.** The hook is the *only* channel a reviewer may reject a variant on, and it is the only one that survives the OS notification icon. | **Rarity, obvious, golden, neon, mega, ride, fly, legendary.** Adopt Me's entire visual grammar is *value and saturation as desirability* — the whole design says "you are lucky" or "you have less than me." Two of those are refused by name in our canon: **gold-as-currency/rarity is banned outright**, and **saturation is never a quality or performance grade**, because grading the child's work by colour would rank children. This is the most dangerous row in the whole section: it converts the child's authorship into a lottery ticket. | §4.2.2 (gold, saturation-as-grade); §4.4.4 rule 2; §1.4 Principle 1; `AGENTS.md` §2.2; `game-concept.md` §2 anti-pillars |
| **R2 — Adopt Me!** | **3. Variants separated by where they sit relative to the head.** Different creatures occupy different vertical bands — above the head, at the head-line, hanging beside it — so they are told apart even when the form is ambiguous. Copy directly, because our canon already solves it this way: **Crest reads above the head-line, Frill reads at and behind it, Floppy-Ear reads below and beside it. Three hooks, three vertical bands, one orthogonal axis carrying all three pairs at 32 px.** | **More pets as the answer to variety.** R2's answer to "we need more creatures" is 200+ of them. Ours is **3 hooks × 65,856 TILT vectors × 9 print classes = 1,778,112 distinct creatures from one mesh, one rig, one material, and two draw calls.** Copying R2's answer to variety is copying the exact failure Pillar 3 exists to prevent: nine months spent on content volume. | §5.2 hook library and second discriminator axis; §5.2 variant space; §1.4 Principle 3; `AGENTS.md` Pillar 3 |
| **R2 — Adopt Me!** | **4. The held-in-the-hand object read.** An R2 pet reads as a thing you could pick up: one soft mass, weight low, feet planted. Copy as three numbers: **ground clearance ≤ 0.04 H, four-contact grounded pose, and no sitting, lying, sleeping, or side poses at all** — because a reclining creature halves the paintable frontal area, and Minigame requires maximising visible painted surface. Pose variety is bought from TILT, never from new poses. | **A finished, fully decorated pet.** An R2 pet arrives as a product: outfit, furniture, accessories, a name. Ours is the opposite by definition — *"something that has just been made and is waiting to be drawn on."* §5.1 rules out the mascot (pre-animated and pre-decorated), the pet (a finished object), and the plush (implying fur, which cannot be painted on). The shell is Dawn Cream on every variant and is never re-tinted to make a variant prettier. | §5.1 archetype; §5.1 load-bearing base-mesh decision; §5.2 step 4; §4.4.4 rule 2 |
| **R2 — Adopt Me!** | **5. Strong separation between the pet and the ground it stands on.** Copy as a **value** relationship, never a glow or an outline: the coat sits **12.4% luma above** the ground bowl, and the rim must raise the silhouette edge by **≥ 8.0% additive luma** over the adjacent unlit surface. That pair is what keeps the creature readable against any background, in every state, at any distance. | **The egg-hatch-reveal acquisition beat.** A cracking shell, a beam of light from above, a silhouette inside — this is slot-machine grammar with a visual surface. It encodes *chance* before it encodes *value*, and chance is the one mechanic this codebase refuses in writing. It also inverts our whole emotional claim: a reveal says *"look what you got,"* and the entire game needs to say *"look what I made."* | §4.3 C8; §2.1 rim specification; `AGENTS.md` §2.2; `game-concept.md` §2 anti-pillars; §1.1 |

---

### Reference 3 — Nintendogs

| Reference | What to take (specific, actionable, exact property to copy) | What to avoid (specific, with WHY it fails) | Canon link |
|---|---|---|---|
| **R3 — Nintendogs** | **1. The 200 ms lag — secondary motion always trails primary.** The puppy is not one rigid object: ears, tail, and head arrive a beat after the body. This is the cheapest aliveness trick in games and it is already canon. Copy: **ears and hook lag the body by 200 ms on the 4.0 s breathe.** Take the *principle*, not just the ear case — every soft element on the creature trails its parent, never leads it and never moves independently. | **Neorealistic breed modelling.** R3 is built on real-breed reference: muzzle length, ear set, coat length, markings per breed. We have an invented archetype with no breed, no muzzle, and a fixed closed-egg silhouette. Copying breeds means copying anatomy we cannot afford at 10k tris and that would break the single-mass read at 64 px. | §5.6 idle baselines; §3.1 canonical hatchling form; §5.1 archetype; §5.7 budget |
| **R3 — Nintendogs** | **2. Asymmetric idle timing — hold, then something small, then hold.** The puppy's idle is not metronomic. Copy as a rule about *cycles*, not about one animation: **breathe runs on 4.0 s and sway runs on 6.0 s, phase-locked at 0 so the two never visibly sync.** Any two periodic behaviours running on this creature must sit on different periods. Two loops at the same rate read as a machine; two loops at different rates read as an animal. | **A dark apartment with a cool television glow.** R3's room is dim and its key light is cool. Our shadow colour *is a semantic* — Peach Shadow is **"the colour of not-wrong,"** the single most important negative colour in the product — and a cold or dim shadow takes that token away from its only job and puts a cold hue in the frame, which at 6–9 reads as unapproachable. Hard law: no state falls below 25% of base luminance, and no light source is ever cold. | §2.1 hard lighting laws 1 and 4; §1.3 forced call on warm shadow; §4.1 Peach Shadow entry; §4.5 Rule V1 |
| **R3 — Nintendogs** | **3. Response proportional to input, not switched by a state machine.** How hard you stroke determines how continuously it reacts; there is no *petted* flag playing a canned animation. Copy as a continuous function: **lean up to 7° into the finger, squash to 0.94 proportional to stroke speed, capped at 1.0 strokes/s.** Reaction energy tracks input energy, continuously, with no threshold and no discrete trigger. | **Needs, and their visible absence.** Feed, brush, clean, toilet — the needs grammar. A visible unmet need is a visible problem, and a problem is what §1.4 Principle 2 forbids in every visual channel. The structural answer is already canon: **Idle Ambient is the baseline with a zero signature delta, and any proposed state that cannot be described as a measurable delta from Ambient is not a state — it is a new scene, and it will not be built.** | §5.4 Focus/Petting expression target; §2.3 exclusive signature variable; §2.9 state distinctness matrix; §1.4 Principle 2 |
| **R3 — Nintendogs** | **4. Affection is carried by the body, not the face.** In R3 a dog shows love by leaning, turning toward you, closing distance. Copy as the **Warm Settle Law**: when a system would signal distress, disappointment, fear, confusion, or failure, it emits Warm Settle instead — a **4° lean *toward*** the stimulus, 1.04 volume, 320 ms settle. Turning away reads as rejection; turning *toward* reads as trust. The direction is the message. And the amplitude floor: **nothing readable lands below 70% of its target amplitude**, because low-amplitude mouth curvature is the animal signal for a negative, so a small expression is a *worse* expression. | **The articulated animal jaw.** R3 dogs have a real mouth with a dark interior, because the model is a dog. Ours is a **0.09 H × 0.02 H value crease with no dark interior, ever.** Copying a jaw would also reopen the expressive vocabulary §5.4 closed on purpose — three mouth forms (Level, Arc-up, O), two eye states, two ear states — where a downturned arc is *structurally absent as an asset*, so no animator can reach unhappiness and distress has no representation to leak through. | §5.1 mouth row; §5.4 Warm Settle Law; §5.4 amplitude floor; §5.4 expression targets |
| **R3 — Nintendogs** | **5. Nothing punishes you for going away.** The dog is simply there when you return. Copy as the **brightest** rule in this section: **greeting intensity scales with absence length, so returning after a week produces the brightest, longest greeting in the game — one full stop brighter than every other state, flooding down over 1.2 s.** The child sees the scaling without reading a word. This is the anti-guilt mechanism executed as lighting. | **Obedience trials, cash, cash-to-breeds, unlock ladders.** R3 grades the animal and grades the owner, and both ladders are visual. Ours grades nobody: **there is no fail lighting state, a miss produces a 0.3 s dim-to-Ambient, and the next prompt arrives at *higher* brightness than the previous one, so the visual trend across attempts is always upward.** Competence is made visible by a record, never by a comparison against a standard. | §2.2 Greeting; §2.6 no fail lighting state; `AGENTS.md` §2.1; `game-concept.md` §5 competence |

---

### 9.1 How to use a reference without copying it

> **A reference is a constraint solver, not a mood board.**

**The mood-board failure.** Someone opens a game, feels something about it, describes the
feeling in adjectives — *warm, safe, handmade, cute, appealing* — and that description
becomes the guidance. Two artists read the same sentence and produce two unrelated assets.
Both get approved, because the approval criterion was *"does it feel right,"* which cannot
be falsified and therefore cannot be reviewed. Six months later there is no way to know
whether an asset is on-brief, because there was never a brief.

**The constraint-solver method.** A reference is a body of solved problems. For each solved
problem, extract the **measurable property** the reference achieves — the number, the
ratio, the closed set, the timing rule — and re-express it in *our own units*. Then the
number is the guidance, and the reference has done its work: it justified the decision at
authoring time, it predicts failure at review time, and it does not require anyone to have
the same memory you do.

**The specificity test.**

> **If two artists read the same reference guidance and produce different results, the
> guidance was not specific enough.**

Every "what to take" row in the three tables above must be checkable with a **ruler**, an
**eyedropper**, or a **render at 64 px**. Adjectives may appear inside a row only as the
name of a property that a number then pins down. *"Warm domestic surfaces"* is not
guidance. *"Shadow tint fixed to Peach Shadow `#F6B9A0` at 0.28 opacity; darkest diffuse
surface is L3 Umber at 41.6% luma"* is guidance.

**The substitution test.** Cover the reference names and read only our canon. If a row
stops meaning anything without knowing which game it came from, that row is describing the
reference rather than constraining our product, and it is deleted. Every row above is
written so that it survives being read blind.

**The cost statement.** A reference is not free. Every constraint above is paid for in
somewhere else: cycles on one mesh, a closed set of 20 clips, six mask slots, three hooks,
nine print classes, zero shader variants for the print system, one material for the
creature. Pillar 3 buys ambition with restraint, and a reference is one of the things
restraint is spent on. **When a reference demands a fourth hook, a second print family, or
a per-state palette, the negotiation is not "how do we get closer to the reference." It is
"which of our numbers are we spending to get there, and who approved it."** §5.2 and §3.2
already answer for their own axes: a fourth hook is an ADR plus a Pillar 3 review, and a
tenth print class is an ADR plus a Pillar 3 review.

#### The three references restated as measurable constraints on our canon

This is the operative form of the section. The impression is what the reference gives; the
constraint is what we author against; the check is how a reviewer proves it.

| Reference | The impression it gives | **The measurable constraint it actually licenses** | Lives in | How it is checked at review |
|---|---|---|---|---|
| **R1** Toca Life World | *warm domestic surfaces; a safe handmade place* | **Zero vertical partitions and zero barriers. One continuous ground bowl, radius 6 H, ≤ 8% elevation change across the whole span. Nothing in the world taller than 0.6 H. Shadow tint locked to Peach Shadow H 17.44° at 0.28 opacity. No diffuse surface below L3 Umber, 41.6% luma. Zero urgent markers on any surface.** | §3.3, §2.1, §4.5 V1, §1.4 P2 | Prop-height pass; eyedrop any shadow pixel and confirm hue band and luma; confirm no barrier geometry exists in the scene at all |
| **R2** Adopt Me! | *pet-as-object; readable silhouette at thumbnail size* | **Closed rounded mass at 64 px front-on greyscale. Hook footprint ≥ 0.122 H²; body overlap ≥ 0.049 H²; new outline ≤ 0.073 H² — measured on the final TILT-applied silhouette. Exactly one hook per variant, and it is the only channel that may reject a variant. Three hooks separable at 32 px by vertical band: above / at / below the head-line. Coat-to-ground Δl ≥ 12.4%. No variant identifiable by print or colour alone.** | §3.1, §3.2, §5.1, §5.2, §4.3 C8 | The four-step 64 px gate; the OS notification icon test at 24–48 px with colour removed |
| **R3** Nintendogs | *living-pet read; idle personality; affection loop* | **Secondary motion lags primary by 200 ms. Breathe 4.0 s and sway 6.0 s on different periods, phase-locked so they never visibly sync. Reaction is a continuous function of input — lean ≤ 7°, squash to 0.94 ∝ stroke speed, capped 1.0 strokes/s. Every expression lands at ≥ 70% of its target amplitude. Idle Ambient is a zero-delta baseline and every other state is a measurable delta from it. Greeting brightness scales *positively* with absence length. Nothing punishes absence.** | §5.6, §5.4, §2.3, §2.2, §2.9 | 64 px expression pre-check before any clip is cut; cycle-period inspection on the idle set; state matrix audit for an empty delta cell |

**What the constraints are *not*.** None of the three licenses a look. R1 licenses a
framing, R2 a proportion, R3 a timing. The look is ours and is specified by §2.1, §3.1,
§4 and §5. Any sentence of the form *"this should feel more like [reference]"* is, in
this project, an incomplete sentence — it is missing the number.

---

### 9.2 Anti-references

The failure modes this project is **most at risk of drifting into**, each with the concrete
tell you will actually see in a render, the pillar it breaks, and the canon number it
violates. These are not general badness. Each one is a plausible outcome of an ordinary
good-faith decision made by someone who has not read Sections 1–5.

| # | Anti-reference | The concrete tell | Why it fails against our pillars | **Canon number violated** | Do this instead |
|---|---|---|---|---|---|
| **AR-1** | **The generic 3D mobile-pet look** | A broad specular sheen that stays put on the body as the creature moves (clearcoat / plastic read); a background of saturated teal, hot pink, or lime at 60–80% HSV S; soft ambient-occlusion darkening under the belly and between the legs; a blurred floating drop shadow; a dozen props at similar value competing with the creature | The plastic sheen spends the saturation budget on the world and the gloss, so the child's paint has nothing to be loud against — Pillar 1 dies. Worse, a clearcoat **kills the wet/dry read entirely**: you cannot see a wet bead on a surface that is already shiny, so §5.5's 17× gloss ratio collapses to nothing and the entire paint feedback loop goes silent | §4.4.4 rule 1 (40.0% HSV S wall); §1.4 Principle 1 (coat ≤ 25.0%, environment ≤ 40.0%); §5.5 (shell gloss **0.02**, roughness **0.85**, no anisotropic highlight); §5.5 shell albedo flatness **± 1.0% luma** (AO breaks this); §3.1 ground clearance **≤ 0.04 H** | Shell gloss exactly **0.02**. No pixel above **40.0% HSV S** that is not Tier-0 paint. Ground with Peach Shadow contact occlusion at **0.15**, never a blurred drop shadow. Creature grounded, always. |
| **AR-2** | **Plush / velvet / fur collectible** | Fibre cards or strand shells; a nap that catches the rim into a fuzzy halo; a fabric seam line down the body; visible stitching; a visible weave in the texture; a fuzzy silhouette edge that eats the hook's outline | This one is **mechanical, not stylistic.** The child's paint sits directly on this surface, and a fibrous surface cannot hold a flat mask decal — so the core mechanic degrades. It also destroys the single thing §3.1 is buying: a **closed, countable outline at 64 px**. A fuzzy edge has no measurable boundary, so the hook footprint cannot be measured and the 12% appendage cap cannot be enforced. And a plush is a finished object with a maker in its history, which is precisely what §5.1 says our creature is not | §5.5 **("No fibre, no fur shells, no strand cards, no hair cards — 0. This is a hard exclusion, not a preference")**; §5.7 hand-off item 9 (material-level exclusion, not a modelling note); §3.1 rule 2 and the hook-footprint test; §5.5 shell albedo flatness | Matte painted shell. Gloss **0.02**, roughness **0.85**. No fibre, no fur, no shell-layer shading in the mesh, the rig, **or the material**. Every hook tip rounded to **≥ 40% of its own base width** so the outline stays countable. |
| **AR-3** | **The "cute but grimy" pet / rescue-puppy aesthetic** | Scuff and dirt texture on the coat; matted or uneven surface direction; a dulled, darker snout area; water or stain marks; a worn patch; visible tear tracks; a soft vignette of grime around the edges of the frame | A grimy surface is a **state**, and it reads as a condition — specifically *this creature is in a condition that someone should fix.* That is the visual grammar of neglect, and Pillar 2 forbids showing problems. It also directly attacks the canvas: §5.5's reason for a blank shell is that **mottle makes every mark edge ambiguous**, so grime reduces the visibility of the child's authorship, which is the Pillar 1 mechanism rather than a side effect of it | §5.5 (**shell albedo flat to ± 1.0% luma; "under the shell: nothing — no dark interior, no AO below L3, no subsurface tint"**); §1.4 Principle 2 (nothing may convey neglect); §4.5 Rule V1 (darkest diffuse surface **L3, 41.6%**) | The shell is blank on every variant, permanently. When marks need space, they are **compacted toward their own centroid or overlaid** — never smudged, never faded, never dirtied. There is no path through §5.5's slot policy that ends in a lost stroke. |
| **AR-4** | **Dark / moody lighting conventions** | A teal-and-orange grade; a rim that goes cold at the base; the frame bottoming out into near-black at the edges; volumetric haze or god-rays; a heavy vignette; a cinematic contrast curve that crushes the shadow side; a lens flare at the sun disc | Our shadow colour is not a colour, it is **a semantic**: Peach Shadow is *the colour of not-wrong.* A cool or crushing shadow takes that token away from its only job and puts a cold hue in frame, which at 6–9 reads as unapproachable. The entire rig is built on the claim that shadow never reaches darkness, because that is the literal implementation of *"no shadows that mean anything is wrong"* | §2.1 **hard lighting law 1** (no surface below 25% of base luminance); §2.1 **law 4** (no light source is ever cold); §2.1 shadow colour `#F6B9A0` — **never black, grey, or blue**; §1.3 forced call: *"Cool or warm shadow? Warm. Neutral or blue is a bug"*; §4.5 Rule V1 (L3 floor); style-anchor negative block (*no gritty or dark-fantasy tone, no heavy vignette, no volumetric god-rays, no lens flare*) | Crush **value** only as far as **L3 Umber, 41.6%**, and spend the rest of the depth budget on **warmth**. Shadow tints toward peach-rose, so the frame stays high-key and gets its depth from hue temperature rather than from darkness. |
| **AR-5** | **Reward-screen confetti and burst spam** | 60+ particle sprites; several simultaneous bursts; randomised colours sampled from a wide hue wheel; screen shake; a chromatic or white flash; coins, stars, or a **"+1"** numeral; a banner sliding down from the top; four layers of audio stinger | **Three distinct pillar failures in one habit.** (1) *Randomised burst colour grades the child's performance* — §2.6 and matrix row C7 fix the burst to a **fixed 12-spoke radial, identical every time, never re-tinted by performance**, precisely so no child is visually ranked. (2) *Screen shake is the universal penalty signal* and §5.4 lists shake as forbidden in motion, alongside lateral overshoot and rotation beyond 12°. (3) *A top-down banner re-enters from outside the frame* and reads as an interruption, where §2.7 says reward objects are earned by **growing into frame**, toward the light, at 1.4× with overshoot, then settling into the world as scenery | §2.6 + §4.3 **C7** (fixed 12-spoke radial); §5.4 forbidden-in-motion (**shake**); §2.7 (**grow in at 1.4× with overshoot — never popup, never slide in, never drop from above, never confetti**); §3.5 rule 5 (**only the creature overshoots** — props, UI, and Echo geometry are 0.00); §4.4.4 rule 1 (particles may not exceed 40.0% HSV S); §4c (no numerals, no count, no score word) | One 2.5 s flourish: fixed 12-spoke radial, fixed 12-note audio figure, then a 3.0 s decay. **One event per session earns this treatment — there is no second peak without a new mark.** The reward object grows in at 1.4× and becomes scenery permanently. |
| **AR-6** | **Cel-shaded / outline / anime-eye pet** | A uniform dark contour around the whole silhouette; hard-edged shadow terminators; a body-wide specular "shine" star; eyes with a sclera, iris, pupil, multi-stop gradient, lashes, and a double highlight | An outline is a **second value channel competing with the real lighting.** On a rim-dominant creature the rim already does the silhouette work; a contour creates two competing edges and a value-contrast bump that pulls the eye off the painted mark — a §3.5 hierarchy failure wearing a shading costume. Anime eyes are built on a large sclera-and-pupil hierarchy, and our eye is **a single self-lit lens with no pupil, no iris, and no lashes**, because at 64 px the eye is 10.9 px and a pupil is sub-pixel detail that costs 3 px and **reads as dirt** | style-anchor negative block (*no outline cel-shading, no anime eyes*); §5.1 eye row (**single self-lit lens + one 1.5 px L8 specular dot; no pupil, no iris, no lashes**); §3.5 rule 4 (eyes are the brightest non-painted element, carried by **self-lift**, not by drawn highlights); §4.7.3 (`--ui-active` on `--ui-shell` is **deliberately 1.91:1** — we hold fill contrast low so the UI never competes with the creature; an outline is the same mistake in a different medium) | Silhouette is carried by the **two-tone rim** — Rim Rose across the upper 55% of the silhouette edge, Mint Halo across the lower 45% — and by value contrast against the ground bowl. **Never by a drawn contour.** |
| **AR-7** | **Egg / hatch / reveal as the acquisition beat** | A closed container that cracks or shakes; a shaft of light from above; a silhouette revealed inside; a gold, prismatic, or neon rarity flash; an egg icon on a progress bar; a "new!" marker | This is **slot-machine grammar with a visual surface.** It encodes *chance* before it encodes *value*, and chance is the one mechanic this codebase refuses in writing. It also inverts the emotional claim of the entire product: a reveal says *"look what you got,"* and the game needs to say *"look what I made."* Pillar 1 is authorship; a reveal is acquisition | `AGENTS.md` §2.2 (*"Nothing is rarity-gated, random, or loot-boxed… There is no odds mechanic anywhere in this codebase"*); `game-concept.md` §2 anti-pillars (no rarity tiers, loot boxes, odds, or pity timers); §4.2.2 (**gold = currency/rarity banned outright**; a prismatic hue set would need an ADR under §4.4.4 rule 3); §2.7 (growth into frame, not reveal); §3.5 rule 2 (reward objects hold hero tier for **1.5 s**, then permanently demote) | The child makes the mark, the mark is already there, and the creature performs it. **Acquisition is authored, not won.** There is no container, no reveal, and no rarity tier anywhere in the product. |
| **AR-8** | **Modal-chrome and dialog-spam "kids" UI** | A full-screen modal for every confirmation; a speech-bubble tutorial chain; a close "✕" glyph; a titled window with a border and a header bar; icons with hard drop shadows; a grid of 12+ evenly-weighted buttons at equal value; a tooltip on hover | It **multiplies learning events and it requires reading.** Our whole shape argument is that the child learns *one* vocabulary — the world's — and the UI is that same vocabulary flattened, so every object is learned once, for the life of the product. A modal chain re-teaches the same button in a new frame a dozen times, and at six years old that is a retraining tax paid on every screen, every session. It also smuggles in a semantic we do not have: a "✕" is *dismiss or cancel*, and nothing in this product can be refused, and nothing here is ever wrong | §3.4 (**three container types only: pebble, card, ribbon. Zero rectangles, zero sharp corners, zero underlines, zero dividers below 4 px**); §3.4 *"one shape vocabulary, one learning event"* — the retraining-tax argument; `AGENTS.md` §2.5 (*nothing in the UI may require reading*); §4c.1 role caps (**5 words maximum per control**); §4c.5 (**2 lines maximum anywhere**; left-aligned always); §4c.6 forbidden table (no underline, strike-through, or emphasis marks); §1.4 Principle 2 (never locked, never crossed out, never greyed) | **Pebble, card, ribbon only**, sharing one corner-radius scale with the world (pebbles 100%, cards 40%, ribbons 20%). Icon-first, then the mandatory recognition check: cover all text, remove 60% saturation, show it to a five-year-old who has never seen it. If a control stops being identifiable it is **redesigned, not relabelled**. |

**How the anti-references were chosen.** Not by asking what is ugly. Each one is the
**natural outcome of a specific reasonable decision**: AR-1 is what happens if someone
raises shell gloss to "make it look nicer"; AR-2 is what happens if someone adds velvety
ambient occlusion to make the creature feel soft; AR-3 is what happens if someone decides
the creature needs a backstory; AR-4 is what happens if someone ports a lighting
presets menu from another project; AR-5 is what happens if someone treats the Flourish as
a reward and rewards get confetti; AR-6 is what happens if someone imports a toon shader;
AR-7 is what happens if someone models the content economy on R2 instead of on Pillars 1–3;
AR-8 is what happens if someone ships a tutorial. That is the test for this table: **every
row names a decision someone will actually want to make.**

---

### 9.3 Reference-mixing rules

> **When two references conflict, our own canon is the tiebreaker. Never a third
> reference.**

Six rules. The first is the whole policy; the rest are how it is applied without argument.

**1 — Sections 1 through 5 outrank every reference. Always.** A reference never
outranks a numbered rule, and there is no negotiation and no *"inspired by both."* If R1's
look and §2.1's rig disagree, **§2.1 wins** and R1 is re-read for a different property —
in that case, R1's warm continuous floor, not R1's flat-shaded technique. References are
evidence for decisions that Sections 1–5 have left open. They are not authority over
decisions Sections 1–5 have closed.

**2 — Never introduce a third reference to break a tie.** This rule exists to prevent one
specific and common failure: a two-way constraint problem gets "solved" by importing
*Animal Crossing* (for calm), *The Legend of Zelda* (for warmth), and *Journey* (for
scale), and the result is a blend with no numbers and therefore no test — a mood board
wearing a constraint solver's clothes. **Our reference set is closed at three.** Adding a
fourth requires the same authority as a new colour: an ADR plus a Pillar 1 and Pillar 3
review (§4.4.4 rule 3). A fourth reference is a fourth set of numbers, and four sets of
numbers will disagree.

**3 — Take properties, never combinations.** The unit of adoption is a single measurable
property (§9.1), extracted from whichever reference owns it and re-expressed in our
units. We never adopt a *look* that is a blend of two references, because a blend has no
numbers, and a thing with no numbers cannot be reviewed, tested, or defended.

**4 — Write the conflict down before resolving it.** When two references pull in different
directions, the argument is recorded with both canon links and settled by §1.3's six
forced calls. Unwritten, it becomes a contest between two people's memories of two games,
and the more senior memory wins, which is not a design process.

**5 — A reference may never be used to justify new content.** §1.1's axis table is
unambiguous: *"New content vs. one thing finished — one thing finished wins."* If
honouring a reference appears to require a new mesh, a new print class, a new colour, or a
new prop family, the reference is asking for something out of scope. Say so, cite §3.2 and
§5.2, and find the variant instead.

**6 — Where all three agree, the agreement is ours to keep — and it is the only thing we
inherit in common.** All three references are warm, rounded, child-facing, and
low-pressure. That shared quality is precisely why these three were chosen, and it is
already ours: §1.1, §1.4, §2.1, and §3.1 say it in numbers. Nothing else is taken from
all three at once.

#### The live conflicts, already settled

Recorded so these five arguments are never re-run.

| # | The conflict | Canonical answer | Where it is written |
|---|---|---|---|
| **M1** | **R1 shows flat-shaded art with no real lighting; §2.1 requires exactly one lighting rig.** | **§2.1 wins.** Seven states modulate one rig along named axes. A state that cannot be described as a measurable delta from Idle Ambient is not a state. | §2.1, §2.3, §2.9 |
| **M2** | **R2 answers "we need more creatures" with 200+; Pillar 3 answers it with one mesh and 1,778,112 variants.** | **Pillar 3 wins.** 3 hooks × 65,856 TILT × 9 prints, one mesh, one rig, one material, two draw calls. A fourth hook is an ADR + Pillar 3 review. | §1.4 Principle 3; §5.2 |
| **M3** | **R3's room is dim with cool television light; §2.1 forbids both.** | **§2.1 wins.** No surface below 25% of base luminance; no cold light source; the darkest diffuse surface is L3 Umber at 41.6%. Depth is bought with **hue temperature**, not darkness. | §2.1 laws 1 and 4; §4.5 V1; §1.3 |
| **M4** | **R1 has a Friday-gift cadence; §2.7 says nothing new arrives during Results.** | **§2.7 wins.** Results is 3–4 seconds, centred, symmetrical, decaying, and permanently final. No recurring reward tick, no calendar, no cadence anywhere in the product. | §2.7; `AGENTS.md` §2.1 |
| **M5** | **R2 flashes gold, neon, and prismatic for rarity; §4.2.2 refuses gold outright and refuses saturation-as-grade.** | **§4.2.2 wins.** Gold means warmth and attention only. Saturation is reserved for the child, and a system asset may never exceed 40.0% HSV S. | §4.2.2; §4.4.4 rules 1 and 2 |

---

### 9.4 The three hard visual tests

Every reference, every extracted constraint, and every asset authored from one must
survive all three. These are the enforcement mechanism for the entire section: the
references supply the constraints in §9.1, and these three tests are what decide whether
a constraint was actually honoured.

#### Test 1 — The 64 px greyscale test (§3.1, ratified §5.1 and §5.2)

> Render **front-on, greyscale, 64 px tall, no motion, three value steps from
> background.** All four steps are mandatory; any failure rejects the asset.

| Step | What is checked | Threshold |
|---|---|---|
| 1 · **Silhouette** | Closed rounded mass. Two lobes, or any hook element piercing the outline | Appendage silhouette area ≤ 12% = **0.058 H²** |
| 2 · **Hook footprint** | Footprint, body overlap, new outline — **measured on the final TILT-applied silhouette** | ≥ **0.122 H²** footprint · ≥ **0.049 H²** overlap · ≤ **0.073 H²** new outline · **8–9 px** of recognisable shape at 64 px |
| 3 · **Print** | Class separable from every other class in greyscale | P2 ≥ 5.0% integrated luma, **or** a documented Key-C override with a second independent discriminator |
| 4 · **Cross-channel** | Hook still identifiable with the print masked out; print still identifiable with the hook masked out | Both, or reject. This is the step that catches a crest whose lobes merge into a checkerboard |

**Why it is in this section at all.** The Android notification icon for this game *is* the
creature — a **24–48 px silhouette with no detail and, at OS level, no colour
differentiation available whatsoever.** Any variant identified only by colour is
unshippable. This test is not academic; it is a shipping constraint, and it is the direct
mechanisation of R2's most valuable property.

#### Test 2 — Greyscale with colour removed (§1.2, extended by §1.6)

> Desaturate any frame to greyscale. **Every state, tool, status, and affordance must still
> be distinguishable.**

The Binding Encoding Law: *no state, mark, status, or affordance is ever communicated by
colour alone.* Every piece of information travels on at least two channels — one of
{shape, silhouette, pattern, motion, size, value} **plus** colour. Colour is the *extra*
channel, never the first. **If greyscale collapses two things into one, that is a defect,
not a palette limitation.**

§1.6 extends the law to motion and audio, because it governs *state changes*: every state
transition carries a **shape or silhouette delta**, a **motion delta** (anticipation,
overshoot, or settle), and an **audio delta** (pitch, timbre, or register). A child who
cannot distinguish a state by sight must still distinguish it by ear. **A row with an empty
cell in §5.6's state table is a defect in that section, not in the lighting.**

This is also the standing resolution of `AGENTS.md` §10 question 7: the colourblind rule is
satisfied by structural redundancy across three channels, not by palette adjustment.

#### Test 3 — Is the child's mark the most saturated thing in frame? (§1.4 Principle 1 / §4.4.4)

> Eyedrop the frame, sort by HSV `S` descending, read the **top five**. **All five must be
> the child's paint, a signature mark, or a flourish reward.** Any other pixel above
> **40.0% HSV S** is a defect.

| Tier | Consumer | Ceiling (HSV S) |
|---|---|---|
| **Tier 0** | The child's twelve paints | **100%** — no ceiling | **100%** | §4.4.3 palette; of the tiers, only Tier 0 paints and the line-art/line-work tiers 2 and 6 may appear above 40% |
| Tier 1 | Creature base coat | **25.0%** ± 2.0 |
| Tier 2 | Print masks (Cocoa family) | **48.2%** ± 2.0 — line art, not a surface |
| Tier 3 | Environment surfaces | **40.0%** ± 2.0 |
| Tier 4 | Light terms (no surface use) | exempt, **never above 35.0%** |
| Tier 5 / 6 | UI surfaces / UI line work | **33.2%** / **48.2%** ± 2.0 |

Denominated on **HSV `S`**, never HSL — because the ceilings land exactly on the HSV
numbers and are not satisfiable on the HSL numbers. **No colour exists in this product
that is not derived from the coat, the Cocoa print-mask family, or the Tier-0 palette**; a
new colour is an ADR event.

This is Pillar 1 as a pixel measurement, and it is the single test that catches **AR-1,
AR-3, and AR-5** in one eyedrop.

#### How the three tests are run

- **All three must pass. Two of three is a fail.** There is no partial credit and no
  "acceptable for a prototype" — the third one costs one desaturate.
- **Run them in cost order, cheapest first:** Test 3 is one eyedropper, Test 1 is one
  render, Test 2 is one desaturate. Most defects die at Test 3, before anyone has to
  argue about a hook.
- **A reference cannot vote.** If someone brings a screenshot from any reference game to
  argue for a look, the argument ends at whichever test that look fails. This is the
  operational meaning of *"a reference is a constraint solver"* — the solver has an exit
  condition.
- **Re-shoot before review.** An asset authored from a reference must be rendered and run
  against all three tests before it reaches a reviewer. A screenshot *of the reference* is
  never evidence about our asset.
- **Record the smallest case, not the hero case.** Tests run on the worst frame: the
  smallest on-screen size (0.5 H = 32 px), the lowest-value state (Menus, 2.1:1), the
  busiest state (Flourish, 3.5:1, environment at −2 rungs), and the two relief-critical
  paints (**Cranberry 28.3%** and **Cherry Pop 33.4%**, which must hold relief and streaks
  against all nine print classes).

---

### 9.5 Reviewer checklist

**Provenance and legal**

- [ ] No asset, mesh, texture, material, animation, audio, font, or data in the repository
      derives from, was traced from, or was extracted from any of the three references
- [ ] **No reference screenshot, capture, still, promotional image, or ripped asset is
      stored in this repository or shipped in the application** — nothing visual from a
      reference is vendored anywhere
- [ ] Every decision traceable to a reference cites a **numbered canon section**, not a
      game name
- [ ] `design/art/reference-catalog.md` is the **only** file where a reference is named;
      art direction cites canon, not titles
- [ ] The reference set is still exactly three, or a fourth is covered by an ADR plus a
      Pillar 1 and Pillar 3 review

**Guidance quality — the §9.1 tests, applied to the guidance itself**

- [ ] Every "what to take" row is measurable with a ruler, an eyedropper, or a 64 px
      render — **no adjective standing alone**
- [ ] **Substitution test:** the row still means something with the reference name covered
- [ ] Each constraint is written in **our units** — H², H, HSV S, luma, ladder rungs,
      percent, px — and never in the reference's units
- [ ] Where a constraint cannot be expressed as a number, it was **deleted**, not softened

**The three hard tests (§9.4)**

- [ ] **64 px gate:** all four steps pass, with step 2 measured on the final TILT-applied
      silhouette
- [ ] **Greyscale:** every state, tool, status, and result is still distinguishable with
      colour fully removed
- [ ] **Top five most saturated pixels** in the frame are **all** Tier-0 paint, signature
      mark, or flourish reward; no non-Tier-0 pixel exceeds **40.0% HSV S**
- [ ] Every §2 state has a **filled** shape + motion + audio row
- [ ] Tests were run on the **worst** frame — smallest size, lowest value, busiest state,
      and both relief-critical paints

**Anti-references (§9.2) — screen the whole frame for each of these eight**

- [ ] **AR-1** no plastic or clearcoat sheen — shell gloss **0.02**, roughness **0.85**;
      no blurred floating drop shadow
- [ ] **AR-2** no fur, fibre, hair cards, strand cards, or shell-layer effects in the mesh,
      the rig, **or** the material
- [ ] **AR-3** no grime, scuff, mottle, stain, or worn patch on the shell — flat to
      **± 1.0% luma**
- [ ] **AR-4** no cool hue anywhere, no near-black corner, no heavy vignette, no volumetric
      haze, no god-rays, no lens flare; darkest diffuse surface is **L3, 41.6%**
- [ ] **AR-5** no confetti, no burst spam, no screen shake, no sliding banner, no "+1"
      numeral, no randomised burst colour
- [ ] **AR-6** no cel outline, no anime eye construction, no body-wide specular shine star
- [ ] **AR-7** no egg, hatch, reveal, rarity-tier flash, or progress-bar container
- [ ] **AR-8** no modal chain, no speech-bubble tutorial, no close "✕", no window title
      bar, no 12-button equal-weight grid

**Reference-mixing (§9.3)**

- [ ] Where two references conflicted, the **canon section is cited** and §1.3's forced
      call was applied and recorded
- [ ] Nothing was blended into a single look — every adopted element is a single measurable
      property
- [ ] No reference was used to justify a new mesh, print class, colour, prop family, or
      asset class

**Hand-off**

- [ ] §5.7's eight shader obligations and §4.3 C4's mandatory carry (relief holding on
      Violet Pop and Cranberry) are unaffected by anything taken from a reference
- [ ] §6–§8, when authored, inherit the same saturation budget, value ladder, and
      three-channel encoding — **a reference never gets to be the reason a later section
      relaxes a number**

---

*Sections 1–9 authored and approved. Companion production files: palette.json, palette.css, typography.json, style-anchor-prompt.md, reference-catalog.md.*