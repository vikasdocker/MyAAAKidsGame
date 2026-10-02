# Reference Catalog

> **Purpose.** Provenance, official sources, verification status, and usage terms for the
> three visual references that seed the art direction of "Dab."
> **Art direction lives elsewhere.** The guidance — what to take, what to avoid, the
> anti-references, the mixing rules, and the three hard tests — is
> **`design/art/art-bible.md` → Section 9**. This file records *who the references are and
> where they came from*. It is the only file in the repository where a reference is named;
> every art decision cites a numbered canon section instead.
> **Reference set: CLOSED AT THREE.** Adding a fourth requires an ADR plus a Pillar 1 and
> Pillar 3 review (§4.4.4 rule 3) — the same authority a new colour requires.
> **Last verified:** 2026-10-01.

---

## How this catalog works

| Question | Answer |
|---|---|
| Who chose these? | The creative director. Non-negotiable inputs to Section 9. |
| What may be taken? | **Measurable visual properties only**, re-expressed in our own units. See Section 9 §9.1. |
| What may be copied? | **Nothing.** See "Legal and usage" below. |
| Why three and not one? | Each owns a different axis: R1 owns the **world framing**, R2 owns **silhouette legibility**, R3 owns **the living-pet read**. One reference per axis, so no axis has two authorities fighting over it. |
| What closes the gap? | `AGENTS.md` §10 question 6 and `game-concept.md` §7 *"Known Gap"* — *"No reference games were supplied for visual touchstone analysis."* |

**Verification method.** Official domains were confirmed by web search on **2026-10-01**,
resolving the publisher or platform domain first and only then the product page. Where a
specific deep link could not be confirmed, the top-level official domain is given and
marked `official domain — deep link unverified`. **No deep link in this file is inferred,
guessed, or constructed.** A wrong URL in a provenance record is worse than no URL.

> ⚠️ **Look-alike domains exist and are not official.** Searching these titles surfaces
> fan wikis, APK mirrors, "MOD" repackaging sites, and unrelated businesses with similar
> names. None of them is a publisher, and none may be cited. Use only the domains in the
> table below.

---

## Legal and usage — applies to every entry without exception

> **These are inspiration references only. No assets, art, code, audio, models, textures,
> fonts, or data may be copied, traced, extracted, sampled, or derived from any of them.**

1. **No copying.** No mesh, texture, material, rig, animation clip, UI element, sound,
   font, or any other asset from any reference may be copied, traced onto, approximated
   by tracing, or derived from any reference. This includes the *silhouettes* of individual
   pets, characters, props, or icons. Similarity of a general silhouette is not
   permission to reproduce a specific one.

2. **No images ship.** **No reference screenshot, capture, still, promotional image, video
   frame, or ripped asset may be stored in this repository, committed to version control,
   bundled into the build, or shipped in the application.** There is no `references/`
   folder and there will not be one. Screenshots are for the reviewing artist's own eyes
   only — they are never an artefact. If an argument about a reference needs a picture,
   the reviewer opens the official source themselves.

3. **Why properties may be taken but not assets.** Style, technique, proportion, timing, and
   general aesthetic direction are not protectable in the way a specific expression is —
   which is precisely why Section 9 §9.1 requires **every takeaway to be restated as a
   number or a closed set in our own units.** That restatement is both the legal margin
   and the quality mechanism. **A takeaway that cannot be expressed as a number is not a
   takeaway, and it is deleted.** The measure of how well this project used a reference is
   not how closely the result resembles it; it is how little resemblance the result needs
   in order to pass Section 9's three tests.

4. **Attribution and criticism.** These titles are referenced by name for the purpose of
   art-direction analysis and criticism, which is what this catalog is. Names and titles
   remain the property of their respective owners.

5. **Third-party IP.** The works listed below are the property of their publishers. They are
   listed here for provenance. No endorsement, partnership, or licence is implied or
   claimed, and none exists.

---

## R1 — Toca Life World

| Field | Value |
|---|---|
| **Source type** | Official publisher site (first-party). Publisher: **Toca Boca AB**, a subsidiary of **Spin Master Corp** (Stockholm, Sweden). |
| **URL** | `https://www.tocaboca.com` — product page at `https://www.tocaboca.com/toca-boca-world` |
| **Confidence** | ✅ **Verified** — publisher's own domain and product page confirmed by web search, 2026-10-01. |
| **What it's a reference FOR** | **The world/room framing: warm domestic surfaces, one continuous unbroken field, the feeling of a safe handmade place, and scale reassurance.** |
| **Canon sections it supports** | §3.3 (ground bowl; four geometry classes; no barriers), §2.1 (shadow colour; 25% luminance floor), §4.1 (Peach Shadow), §4.5 Rule V1, §1.4 Principle 2, §3.5 rule 3, §2.7 |
| **Legal** | Inspiration only. Nothing copied, nothing shipped. See "Legal and usage" above. |

**Take, in one line.** The framing of a safe place — a single warm continuous surface with
objects placed on it, sized so the creature is small inside it, and with nothing urgent
anywhere in it.

**Avoid, in one line.** The inventory (90+ locations, hundreds of items, a catalogue that
grows), the per-room palette, and the 2D-parallax-as-3D shortcut — which would mean
copying the *absence* of a lighting rig, and §2.1 permits exactly one rig.

**Verification note.** The publisher has recently rebranded the product as **"Toca Boca
World"** on its own site; "Toca Life World" remains the title the concept document and
this project reference it by. Both names resolve to the same publisher page.

---

## R2 — Adopt Me!

| Field | Value |
|---|---|
| **Source type** | Official game site + official publisher site + platform listing. Developer/publisher: **Uplift Games** (Fort Lauderdale, FL, USA; remote-first, North America & UK). |
| **URL** | `https://www.playadopt.me` (official game site) · `https://www.uplift.games` (official studio) · `https://www.roblox.com/games/920587237/Adopt-Me` (official platform listing) |
| **Confidence** | ✅ **Verified** — all three domains confirmed by web search, 2026-10-01. |
| **What it's a reference FOR** | **The pet-as-object read: one closed mass carrying one large identifying feature, readable instantly at thumbnail size with no detail available.** |
| **Canon sections it supports** | §3.1 (64 px test; hook footprint ≥ 0.122 H²; 10k tris), §3.2 (hook carries identity; one hook per variant), §5.1 (archetype; base mesh ships with zero appendages), §5.2 (closed hook set of three; vertical-band separation; variant space), §4.2.2 (gold and saturation-as-grade refusals), §2.7, §4.3 C8, §1.4 Principle 3 |
| **Legal** | Inspiration only. Nothing copied, nothing shipped. **"Adopt Me!" is a trademark of Uplift Games.** See "Legal and usage" above. |

**Take, in one line.** Silhouette-first readability — a single closed mass with exactly one
identifying feature large enough to survive being scaled down to nothing.

**Avoid, in one line.** The rarity vocabulary (golden / neon / mega / ride / fly / legendary)
and everything it implies, the egg-hatch-reveal acquisition beat, the answer to variety that
is *more pets*, and the finished-and-decorated pet that arrives as a product. The first
would convert authorship into a lottery ticket; the third is Pillar 3's failure mode
verbatim.

**Verification note.** Frequently confused with unrelated businesses operating under similar
names, and with fan wikis and repackaging sites. Neither the developer nor the platform
domain is to be cited from any of them.

---

## R3 — Nintendogs

| Field | Value |
|---|---|
| **Source type** | Official publisher site (first-party). Publisher: **Nintendo**. Series entries: *Nintendogs* (Nintendo DS, 2005) and *Nintendogs + Cats* (Nintendo 3DS, 2011). |
| **URL** | `https://www.nintendo.com` — regional product page at `https://www.nintendo.com/en-gb/Games/Nintendo-DS/Nintendogs-Chihuahua-Friends-272024.html` |
| **Confidence** | 🟡 **Partially verified** — `nintendo.com` confirmed as the official publisher domain by web search, 2026-10-01. The deep link above is a **confirmed en-GB regional page**; the **US-market deep link is unverified** and is deliberately not written down rather than guessed. |
| **What it's a reference FOR** | **The living-pet read: lagged secondary motion, asymmetric idle timing, response proportional to input, and affection carried by the body rather than the face.** |
| **Canon sections it supports** | §5.6 (idle baselines: 4.0 s breathe + 6.0 s sway, 200 ms ear/hook lag, 20-clip budget, per-state shape/motion/audio rows), §5.4 (Warm Settle Law; amplitude floor; overshoot budget; forbidden-in-motion), §2.2 (greeting intensity scales with absence length), §2.3 (Idle Ambient as zero-delta baseline), §2.1 laws 1 and 4, §1.4 Principle 2, §4.2.2 |
| **Legal** | Inspiration only. Nothing copied, nothing shipped. **Nintendogs is a trademark of Nintendo.** See "Legal and usage" above. |

**Take, in one line.** The living-pet read at almost no cost: secondary motion always trails
primary, any two cycles on the creature sit on different periods, and the reaction is a
continuous function of input energy rather than a triggered clip.

**Avoid, in one line.** Real-breed anatomy, the articulated jaw, the needs grammar and its
visible absence, obedience trials and value ladders, the cool dim room, and anything
requiring a microphone.

**Verification note.** Cited as the **series**, not as a single SKU, because the living-pet
read is a property of the whole franchise and not of one breed or one regional release.

---

## Coverage map — the three axes, one owner each

The set is chosen so that **no axis has two authorities**. If a fourth reference were added
to cover a fourth axis, that axis would have no prior owner — which is exactly when this
rule should be re-examined, at ADR level, and not before.

| Axis | Owned by | The single measurable property taken | Checked by |
|---|---|---|---|
| **World framing** | R1 | One continuous ground bowl, radius 6 H, ≤ 8% elevation change; nothing taller than 0.6 H; shadow locked to Peach Shadow at 0.28; no diffuse surface below L3 41.6% | Prop-height pass; eyedrop any shadow pixel |
| **Silhouette legibility** | R2 | Closed rounded mass at 64 px; hook footprint ≥ 0.122 H² with ≥ 0.049 H² body overlap and ≤ 0.073 H² new outline; exactly one hook per variant; three hooks separable at 32 px by vertical band | The four-step 64 px gate; the 24–48 px notification icon test |
| **The living-pet read** | R3 | 200 ms secondary-motion lag; 4.0 s breathe on a different period from 6.0 s sway; reaction continuous in input; every expression ≥ 70% of target amplitude; Ambient is the zero-delta baseline | 64 px expression pre-check; idle cycle-period inspection |

**Not covered by any reference, and therefore owned entirely by the canon:** the lighting
rig (§2.1), the print system (§3.2, §4.3.3), the mask slots and relief obligations (§5.5),
the palette and saturation budget (§4.1, §4.4), typography (§4c), and all three hard tests
(§9.4). **A reference supplies a constraint; the canon supplies the answer.**

---

## Verification log

| Reference | Domain checked | Method | Result | Date |
|---|---|---|---|---|
| R1 Toca Life World | `tocaboca.com` | Web search for official publisher site; resolved first-party product page and confirmed publisher identity (Toca Boca AB, Spin Master subsidiary) | ✅ Verified | 2026-10-01 |
| R2 Adopt Me! | `playadopt.me`, `uplift.games`, `roblox.com` | Web search for official game site and studio; cross-checked the official game page against the platform listing | ✅ Verified | 2026-10-01 |
| R3 Nintendogs | `nintendo.com` | Web search for official publisher domain; confirmed a regional product page | 🟡 Domain verified; US deep link unverified and **not written down** | 2026-10-01 |

**Rule for future maintenance.** Re-verify a domain before citing it, and record the date
here. If a domain stops resolving, mark the entry unverified rather than substituting a
replacement from memory or from search results. **A provenance record that guesses is worth
less than one that admits a gap.**

---

## Reviewer checklist

**Provenance**

- [ ] No reference screenshot, capture, still, video frame, promotional image, or ripped
      asset exists anywhere in this repository or in the build
- [ ] No asset in the project derives from, was traced from, or was extracted from any
      reference
- [ ] Every decision traceable to a reference cites a **numbered canon section**, not a
      game name
- [ ] Every URL used to check a reference is one of the verified domains in this file, and
      this file is the only place a reference is named
- [ ] The reference set is still exactly three, or a fourth is covered by an ADR plus a
      Pillar 1 and Pillar 3 review
- [ ] Verification dates are current and no entry silently cites an unverified deep link

**Usage discipline**

- [ ] Every takeaway is expressed as a number or a closed set in **our** units — H², H, HSV
      S, luma, ladder rungs, percent, px — and never as an adjective or in the reference's
      own units
- [ ] Takeaways survive the **substitution test**: read with the reference name covered,
      the row still means something
- [ ] Takeaways survive the **specificity test**: two artists given the same row produce
      the same result
- [ ] Nothing was taken from all three references in common beyond the single agreed
      quality — warm, rounded, low-pressure — which is already the canon in numbers
- [ ] No reference was used to justify a new mesh, print class, colour, prop family, or
      asset class
- [ ] Where references conflicted, the canon settled it by §1.3's forced calls, and the
      conflict is recorded in §9.3's table rather than re-litigated

**Output**

- [ ] Every asset authored with a reference in view has been re-rendered and passed all
      three of §9.4's tests — **a screenshot of the reference is never evidence about our
      asset**
- [ ] No asset on screen resembles a specific named character, pet, prop, or icon from any
      reference
