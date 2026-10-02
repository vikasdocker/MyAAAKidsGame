# Visual Anchor Prompt — Dab (working title)

> Generated: 2026-09-30
> Art Bible: `design/art/art-bible.md`
> Palette: `design/art/palette.json` · `design/art/palette.css`
> Usage: prefix for every asset generation in `/asset-spec`

---

## Style Anchor

Use this as the **prefix for every asset generation**. Never modify it per-asset —
if an asset seems to need a different anchor, the asset is wrong, not the anchor.

```
[style: soft-shaded 3D children's-toy aesthetic, matte clay-and-wax surface
finish with a faint hand-moulded irregularity; large simple rounded volumes,
no faceting, no hard edges, no corners; stylised non-photoreal proportions with
oversized heads and simplified anatomy; warm rounded-humanist charm, gentle
and unhurried, never manic or saccharine;
lighting: single warm directional key from behind and above (azimuth 130,
elevation 50), colour #FFE3B8, intensity 1.35, soft shadows; shadows tinted
peach-rose #F6B9A0 and NEVER black, grey, or blue; flat warm ambient #FFF0D8;
two-tone rim light — warm rose #FFB0D0 on the upper silhouette edge, pale
mint #BFF3E2 at the base of the silhouette; a visible emissive sun disc in the
sky dome at the key's azimuth so the light reads as a place; nothing in frame
is cold-lit and nothing falls below 25% of its base luminance;
colors: creature base coat #F7E9D2 Dawn Cream at 15% saturation — the coat is
neutral and unsaturated so the child's painted marks stay the most colourful
thing in frame; environment surfaces #E8C79B Honey Field at 33% saturation;
print masks in a single Cocoa hue family #33251B–#C0A584 only; ALL saturated
colour is reserved for painted marks drawn by the child, drawn from a
twelve-paint palette — Cherry Pop #E42C48, Splash #3FC4E0, Sunbeam #F5D33C,
Bluebell #4A7FE0, Tangerine #F28A2E, Leaf #4FA83C, Violet Pop #8B4FD6,
Grass Pop #6FBF3F, Petal #E87FB0, Splash Mint #3FC8A0, Indigo Pop #5B5BE0,
Cranberry #C4215E; no other hue may appear anywhere in the product;
composition: the creature is the single hero mass, centred or lower-third, and
the highest point in frame; only the creature casts a shadow; props ground
with soft peach contact occlusion at 0.15 opacity and never cast shadows;
one shallow continuous curved ground bowl, one vertical gradient sky dome
(#FFF6E2 zenith to #FFCFA0 horizon), no horizon seam; generous negative space;
reading order is creature, then painted mark, then reachable control, then
nothing;
detail level: confident and uncluttered — suggest texture, never describe it;
surface detail must survive reduction to 64 pixels tall and must remain fully
legible in greyscale;
camera: medium distance, eye-level or slightly below the subject so it looks up,
45mm-equivalent, no wide-angle distortion, no depth-of-field blur on the hero;
negative: no photorealism, no PBR metal, no specular chrome, no glass, no
mirror or reflective surfaces, no subsurface-skin shader, no rim-light bloom
that clips, no lens flare, no volumetric god-rays, no gritty or dark-fantasy
tone, no desaturated or greyed-out elements, no cold blue anywhere including
shadows, no red-as-error, no grey-as-unavailable, no gold-as-currency or
rarity, no green-as-success, no padlocks, no crosses-out or disabled states, no
sharp corners, no spikes, no cones, no blades, no pointed tails or horns, no
neon, no outline cel-shading, no anime eyes, no texture noise, no grunge, no
heavy vignette, no letterboxing, no watermark, no text anywhere in the image]
```

---

## Usage

For any asset spec, insert the subject **between** the style anchor and the
camera/detail instructions. The anchor is not edited — the subject is:

```
[style anchor, verbatim]

subject: a neutral cream creature 0.62 H wide with a 0.48 H head, no neck, one
rounded leaf-shaped ear rising above the crown, short rounded arm nubs, eyes
0.17 H with a small self-lit catchlight brighter than any other non-painted
element; coat is unpatterned; two wet painted strokes on its flank, Cherry Pop
and Sunbeam, each raised 0.03 H with directional streak texture and a soft
0.06 H feathered crayon edge

[camera/detail instructions]
```

### Interpolation markers

Where a spec needs to vary one axis, substitute from this table rather than
re-describing it:

| Marker | Substitute |
|---|---|
| `[coat-print]` | one of nine print classes: Solid, Stripe-Vertical, Stripe-Horizontal, Spot-Round, Spot-Angular, Spiral, Checker, Band, Ripple |
| `[hook]` | exactly one attached form — ear shape, horn crest, frill, fin, or tail. Never two. Never more than 12% of body silhouette area |
| `[tilt]` | proportion vector: head scale, body length, leg length, base mass |
| `[paint]` | one or more of the twelve Tier-0 paints, named |
| `[state]` | one of: Greeting, Idle Ambient, Painting, Flourish, Minigame, Results, Menus |
| `[prop]` | one of three base meshes only: pebble, arch, tuft |

---

## Per-State Variants

Each state is a **named delta from Idle Ambient**, expressed as deltas so a generator
can be given one state without being handed the whole rig.

| State | Anchor delta |
|---|---|
| **Idle Ambient** | *(the anchor as written — this is the baseline)* |
| **First Open / Greeting** | `[state: Greeting]` key intensity ×1.25; overall frame two value steps brighter, flooding down to baseline over 1.2 s; lowest contrast of any state, 1.9:1; 40 warm motes drifting; camera slightly below eyeline |
| **Painting** | `[state: Painting]` key ×0.85; rim warm-only with the mint half removed, ×1.15; environment value −1 rung and saturation ×0.75 so the world recedes; a low wide warm worklight band tracks the finger from above; wet strokes emissive-lifted and beaded for 0.8 s |
| **Signature Mark Flourish** | `[state: Flourish]` key rotated +15° to a three-quarter back-light, elevation 62°, ×1.1; rim ×2.0, strongest in the product; environment value −2 steps; a local point light **tinted to the mark's own paint colour** travels with the creature; 0.15 s light-source inversion where the mark is briefly the brightest object; spark-flecked radial halo |
| **Minigame** | `[state: Minigame]` key nearly frontal at azimuth 15°, elevation 60°, ×0.95; rim suppressed to 0.15 whisper; environment pushed back and depth-blurred with a bright circular stage pool; flat even 2.2:1 lighting; warm morning studio, **never an arena** |
| **Results / Reward** | `[state: Results]` base rig; a slow, wide, symmetrical radial warm sunburst behind the creature; centred symmetrical front-on framing; 2.0:1, soft and enveloping; reward objects grow in toward the light at 1.4× with overshoot |
| **Menus** | `[state: Menus]` key ×0.90; environment value ×0.85 and saturation ×0.80 — **dimmed, never greyed**; high three-quarter camera so the habitat reads as a diorama; creature still visible and breathing; a soft warm horizon band present, and **absent in every other state** |

---

## What This Style Is Not — the negative prompt, restated as instructions

The negative block in the anchor is a summary. These are the failures it is
preventing, in priority order. An asset exhibiting any of them is rejected.

| # | Failure | Why it is fatal |
|---|---|---|
| 1 | **Any cold hue, especially in shadows or ambient** | Breaks the one-rig rule. A blue shadow reads as cold, and cold reads as unwelcoming to the age group. |
| 2 | **Any environment element more saturated than the coat cap** | Steals saturation from the child's marks — the central Pillar 1 mechanism. |
| 3 | **Any sharp corner, spike, cone, blade, or pointed tip** | Breaks the 35%-radius law, breaks the 64 px silhouette test, and reads as a hazard. |
| 4 | **Any surface below 25% base luminance** | A dark surface reads as a problem. This is the visual form of Pillar 2. |
| 5 | **Anything that reads as locked, disabled, greyed, or unavailable** | Forbidden by name. Unavailable is 88% scale and one value step down, in warm cream. |
| 6 | **A second hero or a competing focal point** | Breaks the reading order: creature → painted mark → control → nothing. |
| 7 | **Legibility that depends on colour** | Fails the greyscale gate. Every state must survive desaturation. |
| 8 | **A detail below 0.08 H (5.1 px at 64 px render)** | Disappears under mipping and 6-bit mobile output. |
| 9 | **Reflective or metallic surface** | Cold by definition. Banned in §3.3. |
| 10 | **Text in the image** | Type is a separate production pass per §4c. Generators produce illegible glyphs. |

---

## Known Weakness of AI Generation Against This Style

Stated plainly so the art team knows which defects are the tool's fault and must be
corrected in-engine rather than re-prompted.

| Known failure | What to do |
|---|---|
| Generators reliably produce **blue or grey shadows** | Correct in-engine to Peach Shadow `#F6B9A0`. Do not spend prompt cycles on it. |
| Generators **add high-frequency texture noise** | Strip. Detail level is "suggest, never describe." |
| Generators **deform silhouettes asymmetrically** | Re-symmetry or reject. The silhouette must be a closed rounded mass. |
| Generators produce **sharp ear and horn tips** | Round the terminal to ≥40% of its own base width in DCC. |
| Generators **raise saturation globally** | Desaturate to the tier cap in §4.4 before it reaches the engine. |
| Generators **render the rim as bloom** | Rim is a two-tone silhouette edge, not a glow. Rim intensity 0.55, never clipped. |
| Generators **add a ground plane with a hard edge** | The ground bowl is continuous and has no seam with the sky. |
