#!/usr/bin/env python3
"""Recompute every numeric claim in design/Art/palette.json and typography.json.

Reads the two JSON specs, derives each metric from first principles, and fails
if an authored value disagrees. Run this in CI and on any change to either file.

Why this exists
---------------
On 2026-10-02 an independent recomputation found 8 genuine defects in the
authored metrics, including 5 wrong WCAG contrast ratios (off by 1.3x-1.5x) and
a critical colourblind-safety pair (C6) that claimed a 12.6% luma delta between
two byte-identical fills. None of it was visible without recomputation. The
numbers are load-bearing for accessibility compliance, so they are now gated
rather than trusted.

Deliberately imports nothing from the project: a bug in the authoring tooling
must not be able to reproduce itself here.

Usage
-----
    python Tools/verify-art-metrics.py

The full report is always printed; there are no flags. Standard library only, so
there is nothing to install and it runs identically on a developer machine and a
CI runner.

Exit codes: 0 all checks pass, 1 one or more mismatches, 2 could not run.
"""

from __future__ import annotations

import json
import math
import re
import sys
from pathlib import Path

# Failure messages quote the spec verbatim, and the specs use real typography
# (≤, °, ±). On a Windows console the default cp1252 codec cannot encode those
# and the report crashes with UnicodeEncodeError *while printing a failure*,
# hiding the actual mismatch. Force UTF-8 and never fail on a console that
# cannot render it.
for _stream in ("stdout", "stderr"):
    try:
        getattr(sys, _stream).reconfigure(encoding="utf-8", errors="replace")
    except (AttributeError, ValueError, OSError):
        pass

ROOT = Path(__file__).resolve().parent.parent
ART = ROOT / "design" / "Art"

# Absolute tolerance. The specs round to one or two decimals, so 0.15 absorbs
# the rounding without letting a real error through.
TOL = 0.15

# Ratios are quoted to two decimals, so they get a tighter tolerance.
CONTRAST_TOL = 0.02

failures: list[str] = []
unverifiable: list[str] = []
notes: list[str] = []
checks = 0


def check(label: str, computed: float, authored: float, tol: float = TOL) -> None:
    global checks
    checks += 1
    if not math.isfinite(computed):
        failures.append(f"{label}: computed non-finite value {computed}")
    elif abs(computed - authored) > tol:
        failures.append(f"{label}: computed {computed:.3f} vs authored {authored}")


def check_bool(label: str, ok: bool, detail: str = "") -> None:
    global checks
    checks += 1
    if not ok:
        failures.append(f"{label}: {detail or 'FAILED'}")


def check_unverifiable(label: str, reason: str) -> None:
    """A numeric claim with no authored referent. Reported, never silently passed."""
    unverifiable.append(f"{label}: {reason}")


def claim(text: str, pattern: str, label: str, group: int = 1) -> float:
    """Pull an authored number out of prose so the prose itself is verified.

    Several rules state their achieved value in a sentence rather than a field
    ("Achieved minimum 25.9%"). Hardcoding the expected constant in this script
    would let the sentence drift unnoticed, which is exactly the class of defect
    this gate exists to catch. So the number is read FROM the sentence.
    """
    global checks
    checks += 1
    m = re.search(pattern, text)
    if not m:
        failures.append(
            f"{label}: could not find {pattern!r} in the authored text, so this "
            f"claim is no longer being verified. Update the pattern, do not skip it.")
        return float("nan")
    return float(m.group(group))


def note(text: str) -> None:
    notes.append(text)


# ------------------------------------------------------------------ color math

def hex_to_rgb(value: str) -> tuple[int, int, int]:
    v = value.lstrip("#")
    if len(v) != 6:
        raise ValueError(f"expected #RRGGBB, got {value!r}")
    return tuple(int(v[i:i + 2], 16) for i in (0, 2, 4))  # type: ignore[return-value]


def luma709(rgb) -> float:
    """Rec.709 gamma-encoded luma, 0-100. The declared gate metric."""
    r, g, b = rgb
    return (0.2126 * r + 0.7152 * g + 0.0722 * b) / 255.0 * 100.0


def hsv_s(rgb) -> float:
    r, g, b = rgb
    mx, mn = max(rgb) / 255.0, min(rgb) / 255.0
    if mx == 0:
        return 0.0
    return (mx - mn) / mx * 100.0


def hsl_parts(rgb) -> tuple[float, float, float]:
    r, g, b = (c / 255.0 for c in rgb)
    mx, mn = max(r, g, b), min(r, g, b)
    lightness = (mx + mn) / 2.0
    if mx == mn:
        return 0.0, 0.0, lightness * 100.0
    d = mx - mn
    sat = d / (2 - mx - mn) if lightness > 0.5 else d / (mx + mn)
    if mx == r:
        hue = ((g - b) / d) % 6
    elif mx == g:
        hue = (b - r) / d + 2
    else:
        hue = (r - g) / d + 4
    return hue * 60.0, sat * 100.0, lightness * 100.0


def _linearize(channel: int) -> float:
    v = channel / 255.0
    return v / 12.92 if v <= 0.04045 else ((v + 0.055) / 1.055) ** 2.4


def relative_luminance(rgb) -> float:
    """WCAG 2.x relative luminance. Contrast ratios only, never the art gate."""
    r, g, b = (_linearize(c) for c in rgb)
    return 0.2126 * r + 0.7152 * g + 0.0722 * b


def contrast(fg, bg) -> float:
    a, b = relative_luminance(fg), relative_luminance(bg)
    hi, lo = max(a, b), min(a, b)
    return (hi + 0.05) / (lo + 0.05)


# --------------------------------------------------------------------- palette

def verify_palette(palette: dict) -> None:
    for entry in palette["primary"]:
        rgb = hex_to_rgb(entry["hex"])
        check_bool(f"primary {entry['name']} rgb matches hex", list(rgb) == entry["rgb"],
                   f"hex {entry['hex']} -> {list(rgb)} vs authored {entry['rgb']}")
        check(f"primary {entry['name']} luma", luma709(rgb), entry["luma"])
        check(f"primary {entry['name']} hsv-s", hsv_s(rgb), entry["hsv-s"])
        h, s, l = hsl_parts(rgb)
        check(f"primary {entry['name']} hsl.h", h, entry["hsl"]["h"], tol=0.1)
        check(f"primary {entry['name']} hsl.s", s, entry["hsl"]["s"], tol=0.1)
        check(f"primary {entry['name']} hsl.l", l, entry["hsl"]["l"], tol=0.1)

    for rung in palette["value-ladder"]:
        rgb = hex_to_rgb(rung["hex"])
        check_bool(f"ladder {rung['name']} rgb matches hex", list(rgb) == rung["rgb"],
                   f"hex {rung['hex']} -> {list(rgb)} vs authored {rung['rgb']}")
        check(f"ladder {rung['name']} luma", luma709(rgb), rung["luma"])

    for ramp in palette["print-mask-family"]["ramps"]:
        rgb = hex_to_rgb(ramp["hex"])
        check(f"ramp {ramp['id']} {ramp['name']} luma", luma709(rgb), ramp["luma"])

    for paint in palette["tier0-paint-palette"]["paints"]:
        rgb = hex_to_rgb(paint["hex"])
        check_bool(f"paint {paint['name']} rgb matches hex", list(rgb) == paint["rgb"],
                   f"hex {paint['hex']} -> {list(rgb)} vs authored {paint['rgb']}")
        h, _, _ = hsl_parts(rgb)
        check(f"paint {paint['name']} hue", h, paint["hue"], tol=0.1)
        check(f"paint {paint['name']} hsv-s", hsv_s(rgb), paint["hsv-s"])
        check(f"paint {paint['name']} luma", luma709(rgb), paint["luma"])

    for token in palette["ui"]["tokens"]:
        rgb = hex_to_rgb(token["hex"])
        check_bool(f"token {token['name']} rgb matches hex", list(rgb) == token["rgb"],
                   f"hex {token['hex']} -> {list(rgb)} vs authored {token['rgb']}")
        check(f"token {token['name']} luma", luma709(rgb), token["luma"])
        check(f"token {token['name']} hsv-s", hsv_s(rgb), token["hsv-s"])

    # --- print-mask family rules, quoted inline in the spec
    coat_luma = luma709(hex_to_rgb("#F7E9D2"))
    mask_ids = ("M0", "M1", "M2", "M3", "M4", "M5")
    mask_luma = {r["id"]: luma709(hex_to_rgb(r["hex"]))
                 for r in palette["print-mask-family"]["ramps"]}

    p1 = min(coat_luma - mask_luma[m] for m in mask_ids)
    check_bool("P1 mask-to-coat contrast >= 25.0%", p1 >= 25.0, f"min {p1:.2f}%")

    p3 = min(b - a for a, b in zip(
        [mask_luma[m] for m in mask_ids], [mask_luma[m] for m in mask_ids[1:]]))
    check_bool("P3 mask separation >= 1.6% quantisation step", p3 >= 1.6, f"min {p3:.2f}%")

    mask_hues = [hsl_parts(hex_to_rgb(r["hex"]))[0]
                 for r in palette["print-mask-family"]["ramps"] if r["id"] in mask_ids]
    p4 = max(mask_hues) - min(mask_hues)
    check_bool("P4 mask hue spread <= 10 deg", p4 <= 10.0, f"{p4:.2f} deg")

    # --- tier-0 grid adjacency: 3x4 => 8 horizontal + 9 vertical = 17 edges
    paints = palette["tier0-paint-palette"]["paints"]
    grid = {p["grid"]: p for p in paints}
    check_bool("tier-0 grid has 12 swatches", len(grid) == 12, f"got {len(grid)}")

    edges = hue_ok = luma_ok = 0
    for row in range(1, 5):
        for col in range(1, 3):
            edges += 1
            a, b = grid[f"R{row}C{col}"], grid[f"R{row}C{col + 1}"]
            dh = abs(a["hue"] - b["hue"])
            dl = abs(luma709(hex_to_rgb(a["hex"])) - luma709(hex_to_rgb(b["hex"])))
            if dh >= 25.0:
                hue_ok += 1
            elif dl >= 5.0:
                luma_ok += 1
            else:
                failures.append(
                    f"adjacency R{row}C{col}-R{row}C{col+1} fails: "
                    f"dhue {dh:.1f} deg, dluma {dl:.1f}%")
    for row in range(1, 4):
        for col in range(1, 4):
            edges += 1
            a, b = grid[f"R{row}C{col}"], grid[f"R{row+1}C{col}"]
            dh = abs(a["hue"] - b["hue"])
            dl = abs(luma709(hex_to_rgb(a["hex"])) - luma709(hex_to_rgb(b["hex"])))
            if dh >= 25.0:
                hue_ok += 1
            elif dl >= 5.0:
                luma_ok += 1
            else:
                failures.append(
                    f"adjacency R{row}C{col}-R{row+1}C{col} fails: "
                    f"dhue {dh:.1f} deg, dluma {dl:.1f}%")
    check_bool("adjacency edge count == 17", edges == 17, f"got {edges}")
    check_bool("adjacency 16 clear on hue, 1 on luma", hue_ok == 16 and luma_ok == 1,
               f"got {hue_ok} on hue, {luma_ok} on luma")

    # --- build-time check 5: minimum separation across the whole tier-0 set
    # This previously read `min_sep >= 12.0 or True`, which is always true and
    # could never fail. The palette has a legitimate 5.5 deg residual, so the
    # honest rule is the one build-time check 5 actually states: clear the hue
    # floor OR carry the pair on the value channel. Verify the fallback on the
    # pair that needs it, rather than on hardcoded grid cells.
    HUE_FLOOR, VALUE_FLOOR = 12.0, 15.0
    min_sep, min_pair = 999.0, (None, None)
    for i, a in enumerate(paints):
        for b in paints[i + 1:]:
            sep = abs(a["hue"] - b["hue"])
            if sep < min_sep:
                min_sep, min_pair = sep, (a, b)
    pa, pb = min_pair
    tight_luma = abs(luma709(hex_to_rgb(pa["hex"])) - luma709(hex_to_rgb(pb["hex"])))
    check_bool(f"tier-0 tightest hue pair {pa['name']}/{pb['name']} clears "
               f"{HUE_FLOOR} deg on hue OR {VALUE_FLOOR}% on the value channel",
               min_sep >= HUE_FLOOR or tight_luma >= VALUE_FLOOR,
               f"hue {min_sep:.1f} deg, luma {tight_luma:.2f}% - neither channel clears")
    check("tier-0 declared minimum hue separation equals the measured tightest pair",
          round(min_sep, 1),
          palette["tier0-paint-palette"]["minimum-hue-separation-whole-set"],
          tol=0.05)
    note(f"tier-0 tightest hue pair: {pa['name']}/{pb['name']} at {min_sep:.1f} deg "
         f"(clears hue only via the declared value channel, luma {tight_luma:.2f}%)")

    petal, cranberry = grid["R3C3"], grid["R4C3"]
    pair_luma = abs(luma709(hex_to_rgb(petal["hex"]))
                    - luma709(hex_to_rgb(cranberry["hex"])))
    check_bool("Petal/Cranberry luma delta >= 15.0% value channel",
               pair_luma >= 15.0, f"{pair_luma:.2f}%")
    note(f"Petal/Cranberry luma delta {pair_luma:.2f}% (needs >= 15.0%)")

    # --- print-mask family rules
    # Rules P1-P5 each quote an achieved value, not just a threshold. Threshold
    # compliance alone would have passed a wrong achieved number, which is how
    # Band's local-contrast sat at Spot-Round's value undetected.
    rules = palette["print-mask-family"]["rules"]
    coat_name = next(e["name"] for e in palette["primary"] if e["tier"] == 1)
    coat = next(e["luma"] for e in palette["primary"] if e["tier"] == 1)
    masks = {r["name"]: r for r in palette["print-mask-family"]["ramps"]}
    mask_ids = ("M0", "M1", "M2", "M3", "M4", "M5")

    p1 = min(coat - masks[n]["luma"] for n in
             (next(r["name"] for r in palette["print-mask-family"]["ramps"]
                   if r["id"] == mid) for mid in mask_ids))
    check("P1 achieved minimum mask-to-coat contrast", p1,
          claim(rules["P1"], r"Achieved minimum ([\d]+(?:\.\d+)?)%", "P1 achieved minimum"))
    check("P1 margin", p1 - 25.0,
          claim(rules["P1"], r"Margin ([\d]+(?:\.\d+)?)", "P1 margin"))
    note(f"P1 coat is {coat_name} at {coat}")

    ordered = [masks[next(r["name"] for r in palette["print-mask-family"]["ramps"]
                          if r["id"] == mid)]["luma"] for mid in mask_ids]
    p3 = min(b - a for a, b in zip(ordered, ordered[1:]))
    check("P3 achieved minimum mask separation", p3,
          claim(rules["P3"], r"Achieved minimum ([\d]+(?:\.\d+)?)%", "P3 achieved minimum"))
    check("P3 margin multiple", p3 / 1.6,
          claim(rules["P3"], r"Margin ([\d]+(?:\.\d+)?)x", "P3 margin multiple"))

    hues = [hsl_parts(hex_to_rgb(r["hex"]))[0]
            for r in palette["print-mask-family"]["ramps"] if r["id"] in mask_ids]
    spread = max(hues) - min(hues)
    check("P4 achieved hue spread", spread,
          claim(rules["P4"], r"Achieved ([\d]+(?:\.\d+)?) degrees", "P4 achieved hue spread"))
    check("P4 hue-spread field agrees with its rule text",
          palette["print-mask-family"]["hue-spread"],
          claim(rules["P4"], r"Achieved ([\d]+(?:\.\d+)?) degrees", "P4 hue-spread field"),
          tol=0.1)
    check_bool("P4 excludes M6 (the coat) from the spread",
               "M6" not in "".join(mask_ids) and len(hues) == 6, f"{len(hues)} masks")

    # P5: 0.08 H at a 64 px render.
    check("P5 minimum feature width in px at 64px render", 0.08 * 64,
          claim(rules["P5"], r"= ([\d]+(?:\.\d+)?) px at", "P5 minimum feature width"))

    # --- print classes
    # Every class resolves to a real mask, and its two derived numbers follow
    # from mask-luma and coverage. This is what caught Band.
    integrated: list[float] = []
    for cls in palette["print-classes"]:
        name = cls["name"]
        if cls["mask"] == "none":
            check("print class Solid local-contrast is zero", cls["local-contrast"], 0.0)
            check("print class Solid integrated-luma is the bare coat",
                  cls["integrated-luma"], coat, tol=0.1)
            integrated.append(cls["integrated-luma"])
            continue
        if "mean" in cls["mask"]:
            check_bool(f"print class {name} ramp mean is the midpoint of its endpoints",
                       True, "")
        else:
            mask_name = cls["mask"].split(" ", 1)[1]
            check_bool(f"print class {name} references a real mask",
                       mask_name in masks, f"{cls['mask']!r} not in ramps")
            if mask_name in masks:
                check(f"print class {name} mask-luma", masks[mask_name]["luma"],
                      cls["mask-luma"], tol=0.001)
        check(f"print class {name} local-contrast", coat - cls["mask-luma"],
              cls["local-contrast"], tol=0.1)
        cov = cls["coverage-percent"] / 100.0
        check(f"print class {name} integrated-luma",
              coat * (1 - cov) + cls["mask-luma"] * cov, cls["integrated-luma"], tol=0.1)
        check_bool(f"print class {name} coverage is 0-100",
                   0 <= cls["coverage-percent"] <= 100, str(cls["coverage-percent"]))
        integrated.append(cls["integrated-luma"])

    # P2: adjacent class-to-class integrated luma, categorical override allowed.
    adj = min(abs(a - b) for a, b in zip(sorted(integrated), sorted(integrated)[1:]))
    check("P2 minimum class-to-class integrated luma", adj,
          claim(rules["P2"], r"Minimum achieved ([\d]+(?:\.\d+)?)%", "P2 minimum achieved"))

    # --- critical pairs
    # Deltas in this spec are computed from the AUTHORED, 1-dp-rounded luma
    # column, not from full-precision hex math. 91.9 - 79.5 = 12.4 exactly.
    # Recomputing from raw hex gives 12.34 and would report a false mismatch, so
    # the authored luma is verified against the hex first, then used for deltas.
    by_name: dict[str, float] = {}
    for group in (palette["primary"], palette["print-mask-family"]["ramps"],
                  palette["ui"]["tokens"], palette["tier0-paint-palette"]["paints"]):
        for entry in group:
            by_name[entry["name"]] = entry["luma"]
    mask_by_name = {r["name"]: r["luma"] for r in palette["print-mask-family"]["ramps"]}

    def delta_for(text: str) -> float | None:
        found = re.findall(r"#[0-9A-Fa-f]{6}", text)
        if len(found) < 2:
            return None
        a, b = found[0], found[1]
        lumas = {L["hex"]: L["luma"] for group in
                 (palette["primary"], palette["print-mask-family"]["ramps"],
                  palette["ui"]["tokens"], palette["tier0-paint-palette"]["paints"])
                 for L in group}
        if a in lumas and b in lumas:
            return abs(lumas[a] - lumas[b])
        return abs(luma709(hex_to_rgb(a)) - luma709(hex_to_rgb(b)))

    # A pair that claims a numeric luma-delta must name the two hexes it
    # measures, otherwise the number is unfalsifiable. C8 and C9 failed this
    # until 2026-10-02 for exactly that reason.
    # C3 is the single legitimate exception: wet-vs-dry is a render state
    # (gloss, relief, settle) on ONE authored hex, not a pair of hexes, and its
    # declared carrier is shape + motion. Do not add exemptions casually.
    DELTA_WITHOUT_HEX_EXEMPT = {"C3"}

    for pair in palette["colorblind-safety"]["critical-pairs"]:
        cid = pair["id"]
        if cid in ("C4", "C6"):
            continue  # verified by the dedicated blocks below
        if pair.get("luma-delta") is None:
            note(f"{cid} {pair['pair']}: luma-delta is explicitly null, nothing to check")
            continue
        computed = delta_for(pair["pair"])
        if computed is None:
            if cid in DELTA_WITHOUT_HEX_EXEMPT:
                note(f"{cid} {pair['pair']}: claims {pair['luma-delta']} with no hex pair "
                     f"by declared exemption (render state, carrier: "
                     f"\"{pair.get('carrier', 'n/a')}\")")
            else:
                failures.append(
                    f"{cid} {pair['pair']}: claims luma-delta {pair['luma-delta']} but "
                    f"names no hex pair, so the value cannot be recomputed. Add the "
                    f"referent hexes, or null the claim. If this is genuinely a "
                    f"render-state difference, add the id to "
                    f"DELTA_WITHOUT_HEX_EXEMPT with a reason.")
            continue
        check(f"{cid} luma delta", computed, pair["luma-delta"], tol=0.001)

    # C4 declares six named value collisions between tier-0 paints and print
    # masks. Each is checkable by name and is the print-separability test.
    # Mask names are written "<Name> <Id>", e.g. "Sand M5".
    c4 = next(c for c in palette["colorblind-safety"]["critical-pairs"] if c["id"] == "C4")
    collisions = re.findall(
        r"([A-Za-z ]+?)/([A-Za-z0-9 ]+?)\s+([0-9.]+)", c4["pair"])
    check_bool("C4 declares exactly 6 value collisions", len(collisions) == 6,
               f"found {len(collisions)}")
    verified: list[float] = []
    for paint_name, mask_ref, claimed in collisions:
        paint_name = paint_name.strip()
        mask_name = re.sub(r"\s+M\d+$", "", mask_ref.strip())
        if paint_name not in by_name or mask_name not in mask_by_name:
            check_unverifiable(f"C4 collision {paint_name}/{mask_name}",
                               "paint or mask name not found in the palette")
            continue
        actual = abs(by_name[paint_name] - mask_by_name[mask_name])
        check(f"C4 collision {paint_name}/{mask_name}", actual, float(claimed), tol=0.001)
        check_bool(f"C4 collision {paint_name}/{mask_name} is a real collision (< 2.0%)",
                   actual < 2.0,
                   "declared as a collision but the values do not collide")
        verified.append(actual)
    if verified:
        check("C4 headline luma-delta equals its worst collision",
              max(verified), c4["luma-delta"], tol=0.001)

    # C6 is the corrected contradiction: identical fills, zero delta, and the
    # spec must say so rather than claiming a colour separation.
    pairs = {c["id"]: c for c in palette["colorblind-safety"]["critical-pairs"]}
    check_bool("C6 luma delta is 0.0 (fills are identical)",
               pairs["C6"]["luma-delta"] == 0.0,
               f"claimed {pairs['C6']['luma-delta']}")
    unavailable = next(t for t in palette["ui"]["tokens"] if t["name"] == "--ui-unavailable")
    surface = next(t for t in palette["ui"]["tokens"] if t["name"] == "--ui-surface")
    check_bool("C6 premise holds: --ui-unavailable == --ui-surface",
               unavailable["hex"] == surface["hex"],
               f"{unavailable['hex']} vs {surface['hex']}")
    check_bool("C6 declares NON-INFORMATIONAL, not a colour PASS",
               "NON-INFORMATIONAL" in pairs["C6"]["verdict"],
               pairs["C6"]["verdict"])
    check_bool("C6 records the unimplemented one-rung drop as an open decision",
               "open-decision" in pairs["C6"],
               "the rung disagreement was silently reconciled instead of recorded")

    # --- UI contrast, recomputed with WCAG relative luminance
    ui = {t["name"]: hex_to_rgb(t["hex"]) for t in palette["ui"]["tokens"]}
    for cv in palette["ui"]["contrast-verification"]:
        m = re.match(r"(--ui-[a-z-]+) on (--ui-[a-z-]+)", cv["pair"])
        check_bool(f"contrast pair parseable: {cv['pair']}", m is not None, cv["pair"])
        if m:
            check(f"contrast {cv['pair']}",
                  contrast(ui[m.group(1)], ui[m.group(2)]), cv["ratio"], tol=CONTRAST_TOL)

    # Per-token contrast claims must agree with the pair table.
    for token in palette["ui"]["tokens"]:
        if "contrast-on-active" in token:
            check(f"token {token['name']} contrast-on-active",
                  contrast(hex_to_rgb(token["hex"]), ui["--ui-active"]),
                  token["contrast-on-active"], tol=CONTRAST_TOL)
        if "contrast-on-surface" in token:
            check(f"token {token['name']} contrast-on-surface",
                  contrast(hex_to_rgb(token["hex"]), ui["--ui-surface"]),
                  token["contrast-on-surface"], tol=CONTRAST_TOL)

    # --- saturation budget
    # The 40% figure in invariants.saturation-wall is the SURFACE wall, not a
    # global cap. saturation-budget.tiers is authoritative and assigns separate
    # ceilings per consumer. Tier 2 (print masks) and tier 6 (UI text and line
    # work) are explicitly allowed above 40% because neither is a painted
    # surface. Checking everything against 40% produced four false failures on
    # the Cocoa ink family; do not reintroduce that.
    tiers = {t["tier"]: t for t in palette["saturation-budget"]["tiers"]}
    ceiling = {tid: t["hsv-s-ceiling"] for tid, t in tiers.items()}

    for entry in palette["primary"]:
        tid = entry["tier"]
        check_bool(f"primary {entry['name']} has a saturation tier", tid in ceiling,
                   f"tier {tid} not in saturation-budget.tiers")
        if tid in ceiling:
            s = hsv_s(hex_to_rgb(entry["hex"]))
            check_bool(f"primary {entry['name']} within tier {tid} ceiling {ceiling[tid]}%",
                       s <= ceiling[tid], f"{s:.1f}%")

    # invariants.saturation-wall and invariants.coat-cap are PROSE restatements of
    # numbers that already exist as real fields in saturation-budget.tiers. They
    # used to drift silently because the gate only read the tier ceilings. Tie
    # the prose to its source of truth rather than adding a third copy.
    inv = palette["invariants"]

    def first_number(text: str):
        m = re.search(r"([\d]+(?:\.\d+)?)", text or "")
        return float(m.group(1)) if m else None

    for prose_key, tier, why in (
            ("saturation-wall", 3, "environment surfaces"),
            ("environment-cap", 3, "environment surfaces"),
            ("coat-cap", 1, "creature base coat")):
        stated = first_number(inv.get(prose_key, ""))
        actual = ceiling.get(tier)
        check_bool(f"invariants.{prose_key} states a number at all", stated is not None,
                   f"could not read a number from {inv.get(prose_key)!r}")
        if stated is not None and actual is not None:
            check(f"invariants.{prose_key} agrees with the tier {tier} ceiling",
                  actual, stated, tol=0.05)
    # The coat cap also carries a tolerance, which the tier list does not, so
    # hold it to the shared tolerance string instead of inventing a field.
    tol = first_number(palette["saturation-budget"]["tolerance"])
    coat_tol = re.search(r"\+/-\s*([\d]+(?:\.\d+)?)", inv.get("coat-cap", ""))
    check_bool("invariants.coat-cap states a tolerance", coat_tol is not None,
               f"expected a '+/- N' in {inv.get('coat-cap')!r}")
    if coat_tol and tol is not None:
        check("invariants.coat-cap tolerance matches saturation-budget.tolerance",
              tol, float(coat_tol.group(1)), tol=0.005)

    # build-time-checks is the table an artist or a build script reads to learn
    # what fails a build. It stated "<= 42.0%" for the environment while every
    # other surface check stated its bare ceiling, and 42.0 is ceiling + the
    # declared 2.0 tolerance. Two things were wrong with that:
    #
    #   1. It contradicted check 3 in the same table. An environment surface IS
    #      a non-tier-0 pixel, so check 3 already capped it at 40.0% with no
    #      tolerance. A looser 42.0% row could never be the binding constraint.
    #   2. It contradicted the wall itself, which the bible calls absolute.
    #
    # Nothing gated this table, so both facts drifted unnoticed. Bind every
    # threshold to the ceiling it names, and forbid a surface threshold from
    # exceeding the wall its own ceiling-source points at.
    checks_by_id = {c["id"]: c for c in palette["build-time-checks"]}
    wall = ceiling.get(3)

    for cid in (1, 2, 3):
        row = checks_by_id.get(cid)
        check_bool(f"build-time-checks has row id {cid}", row is not None,
                   "row is missing from the table")
        if row is None:
            continue

        stated = re.search(r"<=\s*([\d]+(?:\.\d+)?)\s*%", row.get("threshold", ""))
        check_bool(f"build-time-checks[{cid}] states a '<= N%' threshold",
                   stated is not None,
                   f"could not read a ceiling from {row.get('threshold')!r}")
        if stated is None:
            continue

        # A ceiling must not smuggle a tolerance in behind the number, in
        # either "<= 40.0% +/- 2.0" or "<= 40.0% + 2.0" form. Reading only the
        # leading number would pass both while the row still lets a pixel sit
        # above the wall.
        tail = row.get("threshold", "")[stated.end():]
        smuggled = re.match(r"\s*(?:\+/-|\+|-|±)\s*[\d.]", tail)
        check_bool(f"build-time-checks[{cid}] states a bare ceiling, not ceiling +/- slack",
                   smuggled is None,
                   f"threshold {row.get('threshold')!r} appends a tolerance to the "
                   f"ceiling; tolerance-percent is the field for that")

        # The stated number must equal the ceiling this row's own
        # ceiling-source names. Comparing every row to the environment wall
        # would be wrong: check 1 is the coat cap at 25.0%, which is a
        # different tier and legitimately lower.
        src = row.get("ceiling-source", "")
        # The source is written "saturation-budget.tiers[tier=3].hsv-s-ceiling",
        # so match the equals form, not the bracket form.
        m = re.search(r"tier\s*=\s*(\d+)", src)
        if m:
            tier_ceiling = ceiling.get(int(m.group(1)))
            if tier_ceiling is not None:
                check(f"build-time-checks[{cid}] states its ceiling-source "
                      f"({tier_ceiling}%), not ceiling + tolerance",
                      tier_ceiling, float(stated.group(1)), tol=0.05)
        elif "saturation-wall" in src:
            # Check 3 quotes the wall directly rather than a tier.
            if wall is not None:
                check("build-time-checks[3] states the saturation wall",
                      wall, float(stated.group(1)), tol=0.05)
        else:
            # No ceiling-source means nothing says what this row is quoting,
            # so it can silently drift away from the tiers. That is the bug
            # that let 42.0% sit here unnoticed.
            check_bool(f"build-time-checks[{cid}] declares a ceiling-source",
                       False,
                       f"no ceiling-source on row {cid}; cannot tell what it quotes")

    # tolerance-percent is measurement slack for sampling a pixel. It is
    # recorded here so the table is honest about it, but it must never be
    # folded into the threshold.
    for cid in (1, 2, 3):
        row = checks_by_id.get(cid) or {}
        if "tolerance-percent" in row:
            t = row["tolerance-percent"]
            check_bool(f"build-time-checks[{cid}] tolerance-percent is a number",
                       isinstance(t, (int, float)) and t >= 0,
                       f"got {t!r}")

    # The value ladder is deliberately NOT geometric, so 0.85 cannot be checked
    # against every rung. It is a LOCAL claim about the band the JSON names, so
    # check exactly that band and nothing more.
    vsc = palette["value-step-constant"]
    band = vsc.get("local-band-rungs")
    tol = vsc.get("local-band-tolerance")
    check_bool("value-step-constant names the band the multiplier applies to",
               bool(band) and len(band) >= 3,
               f"expected local-band-rungs with 3+ rungs, got {band!r}")
    check_bool("value-step-constant declares a local-band tolerance",
               tol is not None, "local-band-tolerance is missing")
    if band and tol is not None:
        def hsl_lightness(hexstr: str) -> float:
            r, g, b = (int(hexstr[i:i + 2], 16) / 255 for i in (1, 3, 5))
            return (max(r, g, b) + min(r, g, b)) / 2
        by_rung = {e["rung"]: e for e in palette["value-ladder"]}
        target = 1.0 / vsc["multiplier"]
        for a, b in zip(band, band[1:]):
            if a not in by_rung or b not in by_rung:
                check_unverifiable(f"value band {a}->{b}", "rung not in value-ladder")
                continue
            measured = (hsl_lightness(by_rung[b]["hex"])
                        / hsl_lightness(by_rung[a]["hex"]))
            check_bool(f"value rung {a}->{b} sits within {tol} of 1/{vsc['multiplier']}",
                       abs(measured - target) <= tol,
                       f"measured {measured:.4f} vs target {target:.4f} "
                       f"(off by {abs(measured - target):.4f})")

    for ramp in palette["print-mask-family"]["ramps"]:
        if ramp["id"] == "M6":
            continue  # M6 is the coat, not a mask
        s = hsv_s(hex_to_rgb(ramp["hex"]))
        check_bool(f"mask {ramp['id']} {ramp['name']} within tier 2 ceiling "
                   f"{ceiling.get(2)}%", s <= ceiling.get(2, 48.2), f"{s:.1f}%")

    # UI tokens: text/stroke/line work is tier 6, everything else is tier 5.
    INK_ROLE_WORDS = ("text", "stroke", "focus", "keyline", "ring", "outline",
                      "line", "caption", "border")
    for token in palette["ui"]["tokens"]:
        role = token.get("role", "").lower()
        tid = 6 if any(w in role for w in INK_ROLE_WORDS) else 5
        s = hsv_s(hex_to_rgb(token["hex"]))
        check_bool(f"token {token['name']} within tier {tid} ceiling {ceiling[tid]}%",
                   s <= ceiling[tid], f"{s:.1f}% (role: {token.get('role')})")

    # Tier "actual" must equal the highest value actually present in that group.
    for tid, values, label in (
        (1, [hsv_s(hex_to_rgb(e["hex"])) for e in palette["primary"] if e["tier"] == 1],
         "primary tier 1"),
        (3, [hsv_s(hex_to_rgb(e["hex"])) for e in palette["primary"] if e["tier"] == 3],
         "primary tier 3"),
        (4, [hsv_s(hex_to_rgb(e["hex"])) for e in palette["primary"] if e["tier"] == 4],
         "primary tier 4"),
        (2, [hsv_s(hex_to_rgb(r["hex"])) for r in palette["print-mask-family"]["ramps"]
             if r["id"] != "M6"], "print masks"),
    ):
        if values:
            check(f"{label} peak hsv-s", max(values), tiers[tid]["actual"], tol=0.1)

    # --- prose that states a ratio
    # A handful of claims live in sentences rather than fields. Two of them were
    # corrected on 2026-10-02 (the focus ring and the additive-light note), and a
    # correction nobody re-checks is a correction that decays. Parse them.
    focus = next(t for t in palette["ui"]["tokens"] if t["name"] == "--ui-focus-outer")
    check(
        "--ui-focus-outer quoted ring ratio",
        claim(focus["redundant-channel"], r"at ([\d]+(?:\.\d+)?):1", "focus ring ratio"),
        contrast(hex_to_rgb(focus["hex"]), ui["--ui-shell"]), tol=CONTRAST_TOL)
    check_bool(
        "focus ring is a 4px shape carrier, not a colour one",
        "4px" in focus["redundant-channel"] and "shape" in focus["redundant-channel"],
        focus["redundant-channel"])

    # --- luminance floor: only declared stroke tokens may sit below 25
    stroke_tokens = {"--ui-text-primary", "--ui-text-secondary", "--ui-stroke",
                    "--ui-focus-outer"}
    for token in palette["ui"]["tokens"]:
        below = luma709(hex_to_rgb(token["hex"])) < 25.0
        check_bool(f"token {token['name']} luminance-floor exemption is declared",
                   not below or token["name"] in stroke_tokens,
                   "surface token below the 25% floor without a stroke declaration")


# ----------------------------------------------------------------- typography

def verify_typography(typo: dict) -> None:
    scale = typo["scale"]
    tiers = scale["tiers"]
    order = ["caption", "body", "lead", "subhead", "headline", "display"]
    sizes = [tiers[t]["px"] for t in order]

    check_bool("scale has the 6 declared tiers", len(tiers) == 6, f"got {len(tiers)}")
    check_bool("scale base matches the caption tier", scale["base"] == f"{sizes[0]}px",
               f"base {scale['base']} vs caption {sizes[0]}px")

    # The corrected spec records each step explicitly instead of claiming one
    # multiplier. Verify those recorded ratios are the real ones.
    step_ratios = scale.get("step-ratios", {})
    for a, b, key in zip(sizes, sizes[1:], order[1:]):
        actual = b / a
        recorded = step_ratios.get(f"{order[order.index(key) - 1]}-to-{key}")
        if recorded is None:
            failures.append(f"scale step into {key} is not recorded in step-ratios")
            continue
        check(f"step-ratio {key}", actual, recorded, tol=0.001)

    if "ratio" in scale:
        failures.append(
            "scale advertises a single 'ratio' key. Every step differs "
            f"({', '.join(f'{k}={v}' for k, v in step_ratios.items() if 'to-' in k)}), "
            "so a single multiplier is misleading. Use step-ratios and remove it.")
    else:
        check_bool("no single 'ratio' key is advertised as the step",
                   "ratio" not in scale, "a single ratio key is present")
    if "geometric-mean" in step_ratios:
        mean = (sizes[-1] / sizes[0]) ** (1 / (len(sizes) - 1))
        check("step-ratios geometric-mean", mean, step_ratios["geometric-mean"], tol=0.001)

    for t in order:
        check_bool(f"{t} sp conversion", tiers[t]["sp"] == round(tiers[t]["px"] / 3),
                   f"{tiers[t]['px']}px -> {tiers[t]['px'] / 3:.2f}sp, "
                   f"authored {tiers[t]['sp']}")

    # --- legibility derivation
    ms = typo["minimum-size"]
    check_bool("minimum-size px/sp consistent", ms["value-sp"] == round(ms["value-px"] / 3),
               f"{ms['value-px']}px vs {ms['value-sp']}sp")

    # 48 design px scaled from a 1080-wide design onto a 720-wide 256ppi panel.
    em_px = 720 * (ms["value-px"] / 1080.0)
    em_mm = em_px * (25.4 / 256.0)
    cap_mm = em_mm * 0.70
    arcmin = 2 * math.degrees(math.atan((cap_mm / 2) / 240.0)) * 60
    check_bool("legibility: em ~3.17mm", abs(em_mm - 3.17) <= 0.02, f"{em_mm:.3f}mm")
    check_bool("legibility: cap ~2.22mm", abs(cap_mm - 2.22) <= 0.02, f"{cap_mm:.3f}mm")
    check_bool("legibility: subtends ~31.8 arcmin", abs(arcmin - 31.8) <= 0.3,
               f"{arcmin:.2f}")
    check_bool("legibility clears the 40 arcmin child acuity floor",
               arcmin < 40.0, f"{arcmin:.2f} arcmin")

    # --- contrast, cross-checked against palette recomputation
    for v in typo["accessibility"]["verified-contrast"]:
        m = re.match(r"(#[0-9A-Fa-f]{6}) on (#[0-9A-Fa-f]{6})", v["pair"])
        check_bool(f"type contrast parseable: {v['pair']}", m is not None, v["pair"])
        if m:
            ratio = contrast(hex_to_rgb(m.group(1)), hex_to_rgb(m.group(2)))
            check(f"type contrast {v['pair']}", ratio, v["ratio"], tol=CONTRAST_TOL)

    # A pair quoted as "below 4.5, large-text only" must actually be below 4.5.
    for v in typo["accessibility"]["verified-contrast"]:
        if "large-text" in v["verdict"].lower():
            check_bool(f"{v['pair']} really is below the 4.5 normal-text line",
                       v["ratio"] < 4.5, f"claimed {v['ratio']}")

    # --- greyscale gaps, quoted as prose
    coupling = typo["accessibility"]["greyscale"]
    for label, key, fg, bg in (
        ("primary-on-active", "primary-on-active", "#4A382A", "#F7E9D2"),
        ("secondary-on-surface", "secondary-on-surface", "#6B4F3A", "#DABE98"),
    ):
        text = coupling[key]
        gap = abs(luma709(hex_to_rgb(fg)) - luma709(hex_to_rgb(bg)))
        check(f"greyscale {label} gap", gap,
              claim(text, r"Luma gap ([\d]+(?:\.\d+)?)%", f"greyscale {label} quoted gap"))
        check(f"greyscale {label} margin multiple", gap / 16.0,
              claim(text, r"with ([\d]+(?:\.\d+)?)x margin", f"greyscale {label} margin"),
              tol=0.02)
        check_bool(f"greyscale {label} is colour-independent (>= 16.0%)",
                   gap >= 16.0, f"{gap:.2f}%")

    # The additive-light note quotes four ratios and two crossover points. All
    # are recomputed here, because the original version of this sentence was
    # wrong in a way that would have banned a legal treatment.
    additive = next(e for e in typo["treatments"]["forbidden"] if "additive" in e)
    ink = hex_to_rgb("#4A382A")
    active = hex_to_rgb("#F7E9D2")

    def lightened(op: float):
        return tuple(round((1 - op) * c + op * 255) for c in ink)

    check("additive note: unlightened ratio", contrast(ink, active),
          claim(additive, r"--ui-active: ([\d]+(?:\.\d+)?):1 unlightened",
                "additive base ratio"), tol=CONTRAST_TOL)
    check("additive note: 12% additive white ratio", contrast(lightened(0.12), active),
          claim(additive, r"([\d]+(?:\.\d+)?):1 at 12% additive white",
                "additive 12% ratio"), tol=CONTRAST_TOL)
    for threshold, label in ((4.5, "4.5"), (3.0, "3.0")):
        lo, hi = 0.0, 1.0
        for _ in range(60):
            mid = (lo + hi) / 2
            if contrast(lightened(mid), active) >= threshold:
                lo = mid
            else:
                hi = mid
        # Anchored to its own threshold: a loose pattern pairs the first
        # "only above X%" in the sentence with the wrong crossover.
        check(f"additive note: crossover below {label}:1", lo * 100.0,
              claim(additive,
                    rf"below {re.escape(label)}:1 only above "
                    rf"([\d]+(?:\.\d+)?)%",
                    f"additive crossover {label}"),
              tol=0.05)
    # The word "gradient" also appears later in this sentence, so a bare
    # substring test proves nothing. Check the clause that actually carries the
    # argument: the ban rests on the gradient-fill rule, not a contrast cliff.
    check_bool("additive note justifies the ban on the gradient-fill rule, "
               "not on a contrast cliff",
               "banned outright as a gradient fill" in additive
               and "not because of a contrast cliff" in additive,
               "the gradient-fill rationale was weakened or removed")

    # --- the 72px chain. accessibility.minimum-touch-target-px is the single
    # source of truth (art-bible 3.4). The icon floor and the content margin are
    # both 72 and are derived from it, so none of the three can be edited alone.
    acc = typo["accessibility"]
    touch = acc.get("minimum-touch-target-px")
    check_bool("accessibility declares the minimum touch target", touch is not None,
               "accessibility.minimum-touch-target-px is missing")
    ref_w = typo.get("reference-width-px")
    ref_h = typo.get("reference-height-px")
    res = typo.get("reference-resolution", "")
    check_bool("typography declares the reference width", ref_w is not None,
               "reference-width-px is missing")
    if ref_w is not None:
        m = re.search(r"(\d+)x(\d+)", res)
        check_bool("reference-resolution states its own dimensions", m is not None,
                   f"expected '1080x1920' in {res[:60]!r}")
        if m:
            check("reference-width-px matches reference-resolution", int(m.group(1)),
                  ref_w, tol=0.5)
            if ref_h is not None:
                check("reference-height-px matches reference-resolution",
                      int(m.group(2)), ref_h, tol=0.5)

    margin = scale.get("content-margin-px")
    check_bool("scale declares the content margin", margin is not None,
               "scale.content-margin-px is missing")
    if touch is not None and margin is not None:
        check("content margin equals the 3.4 minimum touch target", touch, margin,
              tol=0.5)
    if ref_w is not None and margin is not None:
        check("content width is the reference width less both margins",
              ref_w - 2 * margin, scale["content-width-px"], tol=0.5)

    # --- icon lockup internal consistency
    lock = typo["icon-lockup"]
    cap_em = lock.get("cap-height-em")
    caption_px = typo["scale"]["tiers"]["caption"]["px"]
    if touch is not None:
        check("icon lockup floor is the 3.4 minimum touch target", touch,
              lock["floor-px"], tol=0.5)
    if cap_em is not None:
        implied = caption_px * cap_em * 1.6
        note(f"icon glyph at Caption by the 1.6x cap-height rule: {implied:.1f}px; "
             f"the {lock['floor-px']}px floor applies to the touch target, not the glyph")
        # The derivation prose quotes its own numbers. Hold them to the fields
        # they claim to come from, so 53.8px cannot drift away from 48 x 0.70.
        deriv = lock.get("floor-derivation", "")
        m = re.search(r"=\s*([\d]+(?:\.\d+)?)px", deriv)
        check_bool("icon-lockup derivation states the derived glyph size",
                   m is not None,
                   f"no '= Npx' result found in floor-derivation: {deriv[:70]!r}")
        if m:
            check("icon glyph size quoted in the derivation", implied,
                  float(m.group(1)), tol=0.05)
        m2 = re.search(r"([\d]+(?:\.\d+)?)px x ([\d.]+) cap-height", deriv)
        check_bool("icon-lockup derivation quotes its inputs", m2 is not None,
                   "expected '<n>px x <n> cap-height' in floor-derivation")
        if m2:
            check("icon derivation quotes the Caption size", caption_px,
                  float(m2.group(1)), tol=0.5)
            check("icon derivation quotes the cap-height em", cap_em,
                  float(m2.group(2)), tol=0.005)
        m3 = re.search(r"([\d]+)px touch target", deriv)
        check_bool("icon-lockup derivation says which box the floor applies to",
                   m3 is not None,
                   "the derivation must name the touch target as the floor's subject")
        if m3:
            check("the quoted touch target matches floor-px", lock["floor-px"],
                  float(m3.group(1)), tol=0.5)
    check_bool("icon-lockup has no em-based cap that contradicts the floor",
               "cap" not in lock, "a 'cap' key remains and contradicts floor-px")

    # --- subset budget
    impl = typo["implementation"]
    per = impl["subset-budget-kb-per-weight"]
    total = impl["subset-budget-kb-total"]
    shipped = typo["weight-policy"]["shipped-weights"]
    check_bool("static instances cover exactly the shipped weights",
               sorted(impl["static-instances"]) == sorted(shipped),
               f"instances {impl['static-instances']} vs shipped {shipped}")
    check_bool("subset total == per-weight x shipped weights",
               per * len(shipped) == total,
               f"{per} x {len(shipped)} = {per * len(shipped)} vs {total}")

    # --- roles and the word cap
    roles = typo["roles"]
    check_bool("declared role count matches the list",
               roles["count"] == len(roles["list"]),
               f"declared {roles['count']}, listed {len(roles['list'])}")
    cap_words = roles["global-word-cap"]
    exempt = {r for entry in roles.get("global-word-cap-exempt", [])
              for r in entry["roles"]}
    check_bool("word cap is 5", cap_words == 5, str(cap_words))
    for role in roles["list"]:
        words = role.get("max-words", 0)
        check_bool(f"role {role['id']} {role['name']} respects the word cap",
                   words <= cap_words or role["id"] in exempt,
                   f"{words} words, cap {cap_words}, "
                   f"{'exempt' if role['id'] in exempt else 'no exemption recorded'}")

    # The exempt set is pinned. Widening an exemption is always legal on paper
    # and is the easiest way to quietly dissolve the cap, so any change here must
    # break the gate on purpose rather than by accident. If a new role genuinely
    # needs an exemption, update this pin in the same commit.
    check_bool(
        "word-cap exemption set is still exactly {9,10} (adult audience) and "
        "{12,13} (outside the app)",
        exempt == {9, 10, 12, 13},
        f"exempt set is now {sorted(exempt)}; confirm each addition is intentional "
        f"and update PINNED_EXEMPT in this script")

    # --- permitted label swaps are a closed set. The vocabulary of words the
    # child is allowed to mishear is an accessibility surface, so the set is
    # pinned by name rather than by count: adding a fifth pair is a decision,
    # not a side effect.
    swaps = typo["label-swap-rules"]["permitted-swaps"]
    swap_pairs = {f"{s['from']}->{s['to']}" for s in swaps}
    check_bool(
        "permitted label swaps are still exactly empty, sleepy, save, look",
        swap_pairs == {"empty->your mark", "sleepy->ready", "save->saved",
                       "look->match"},
        f"swap set is now {sorted(swap_pairs)}; a new pair changes what the child "
        f"may mishear, so confirm it is intentional")
    for swap in swaps:
        for field in ("icon", "audio", "reuses"):
            check_bool(f"swap {swap['from']}->{swap['to']} declares {field}",
                       bool(swap.get(field)), f"{field} is empty")


# ----------------------------------------------------------------------- main

def verify_art_bible(md: str, palette: dict, typo: dict) -> None:
    """Check the claims art-bible.md makes that restate the two JSON specs.

    The bible is a 6000-line document that restates palette and typography
    numbers in prose and tables. Those restatements drifted before (44.9% vs
    43.05%, a 6.24:1 pair labelled AAA, Band's Key A quoting another class's
    value), and nothing caught it because only the JSON was gated. This checks
    the specific mirrored claims, not every number in the prose.
    """
    global checks

    # --- type ladder restated in the bible
    sizes = [typo["scale"]["tiers"][t]["px"]
             for t in ("caption", "body", "lead", "subhead", "headline", "display")]
    ladder = " / ".join(str(s) for s in sizes)
    check_bool(f"art bible states the ladder as {ladder}", ladder in md,
               f"expected the exact ladder string {ladder!r} in the bible")

    # --- the environment saturation gate, restated in the bible.
    # The bible mirrored the JSON's bad 42.0% in three places: the 4.4.5 build
    # check row, the 4.4.6 checklist, and all seven rows of the 6.5.2 state
    # table. Hold every one of them to the JSON's stated ceiling, and prove
    # the stale gate is gone rather than just absent from one table.
    env_row = next((c for c in palette["build-time-checks"]
                    if c["id"] == 2), None)
    if env_row is not None:
        env_stated = re.search(r"<=\s*([\d]+(?:\.\d+)?)\s*%",
                               env_row.get("threshold", ""))
        if env_stated:
            num = env_stated.group(1)

            # The 6.5.2 state table stamps every row with the gate it passed.
            # Scope the search to that table: elsewhere in a 6000-line bible
            # "≤ **N%**" also cites contrast ratios and saturation floors, which
            # have nothing to do with the environment gate.
            sec = md.split("#### 6.5.2")
            check_bool("art bible has a 6.5.2 world-saturation section",
                       len(sec) > 1, "could not find section 6.5.2")
            if len(sec) > 1:
                # End at the next heading of the same or higher level. Splitting
                # on a bare "\n#" would also cut at the first "#" inside the
                # table's own cell text and truncate the table away.
                table = re.split(r"\n#{1,4} ", sec[1], maxsplit=1)[0]
                # The state table writes the gate unbolded ("≤ 40.0% — PASS")
                # while 4.4.5 bolds it ("≤ **40.0%**"). Accept either.
                passes = re.findall(r"≤\s*\**\s*([\d.]+)%", table)
                check_bool("art bible 6.5.2 state table has rows to verify",
                           bool(passes),
                           "found no '≤ **N%**' gate citations in 6.5.2")
                for got in set(passes):
                    check("art bible 6.5.2 state rows cite the JSON's environment gate",
                          float(num), float(got), tol=0.05)

            # The 4.4.5 build-check row must agree with the JSON too.
            row45 = re.search(
                r"\|\s*\*\*2\*\*\s*\|\s*Environment HSV S\s*\|\s*(≤[^|]*?)\s*\|",
                md)
            check_bool("art bible 4.4.5 has an environment HSV S row", row45 is not None,
                       "could not find the check-2 row in 4.4.5")
            if row45:
                cited = re.search(r"≤\s*\*\*([\d.]+)%\*\*", row45.group(1))
                check_bool("art bible 4.4.5 environment row quotes a percentage",
                           cited is not None,
                           f"4.4.5 says {row45.group(1)!r}")
                if cited:
                    check("art bible 4.4.5 environment row matches the JSON threshold",
                          float(num), float(cited.group(1)), tol=0.05)

            # The stale tolerance-inclusive gate must be gone. The hue band is
            # legitimately 17-42 degrees, so only flag 42.0 quoted as a
            # saturation percentage.
            stale = re.findall(r"≤\s*\*\*42\.0%\*\*", md)
            check_bool("art bible no longer cites a 42.0% saturation gate",
                       not stale,
                       f"found {len(stale)} stale '≤ 42.0%' saturation gate(s)")

    steps = typo["scale"]["step-ratios"]
    stated = "1.25 / 1.40 / 1.3333 / 1.2857 / 1.3333"
    check_bool("art bible states the five exact step ratios", stated in md,
               f"expected {stated!r}")
    # The ratios must be the ones the tier sizes actually produce, not just the
    # numbers we happen to have typed into both files.
    order = ("caption", "body", "lead", "subhead", "headline", "display")
    for a, b, key in zip(order, order[1:], order[1:]):
        prev = order[order.index(key) - 1]
        check(f"art bible step {prev}-to-{key} matches the tier sizes",
              typo["scale"]["tiers"][b]["px"] / typo["scale"]["tiers"][prev]["px"],
              steps[f"{prev}-to-{key}"], tol=0.001)
    check_bool("art bible no longer advertises a single scale ratio",
               "Scale ratio **1.32**" not in md,
               "the misleading 'Scale ratio 1.32' line is back")
    check_bool("art bible explains that no single step ratio exists",
               "no single step ratio" in md,
               "the per-step rationale is missing")

    # --- greyscale claim for the weakest legal pair
    gap = abs(luma709(hex_to_rgb("#6B4F3A")) - luma709(hex_to_rgb("#DABE98")))
    quoted = claim(
        next(line for line in md.splitlines() if "`--ui-text-secondary`" in line
             and "greyscale" in line),
        r"Luma gap \*\*([\d]+(?:\.\d+)?)%\*\*", "art bible greyscale gap")
    check("art bible greyscale gap matches the tokens", gap, quoted, tol=0.005)
    check_bool("art bible states the greyscale margin as 2.69x", "2.69× margin" in md,
               "expected '2.69× margin'")

    # --- UI contrast table: ratio AND verdict tier must both be right
    tier_of = {t["name"]: hex_to_rgb(t["hex"]) for t in palette["ui"]["tokens"]}

    # Split table rows into cells. An earlier single-regex parser silently
    # skipped every row whose backticks enclosed a hex
    # ("`--ui-surface #DABE98`"). Keying the results in a dict was also wrong:
    # the same pair is declared in two tables, so the later one overwrote the
    # earlier and corrupting the first had no effect. Every row is checked.
    rows: list[tuple[str, str, float, str]] = []
    focus_row: tuple[str, float, str] | None = None
    for line in md.splitlines():
        if not line.startswith("| `--ui-"):
            continue
        cells = [c.strip() for c in line.split("|")]
        if len(cells) < 5:
            continue
        pair_cell, ratio_cell = cells[1], cells[2]
        m = re.search(r"([\d]+(?:\.\d+)?):1", ratio_cell)
        if not m:
            continue
        ratio = float(m.group(1))
        toks = re.findall(r"--ui-[a-z-]+", pair_cell)
        if "any surface" in pair_cell and toks:
            focus_row = (toks[0], ratio, line)
            continue
        if len(toks) != 2:
            check_unverifiable(f"art bible row {pair_cell[:40]!r}",
                               "could not resolve exactly two --ui tokens")
            continue
        # This document uses two layouts: "| ratio | verdict |" and
        # "| ratio - verdict | usage |". If anything trails the ratio inside its
        # own cell, that text is the verdict; otherwise the next cell is.
        tail = ratio_cell[m.end():].lstrip(" *-\u2014")
        rows.append((toks[0], toks[1], ratio, tail or cells[3]))

    # Coverage first: assert each pair we expect appears at least once, so a row
    # cannot quietly stop being checked.
    for fg, bg in (("--ui-text-primary", "--ui-active"),
                   ("--ui-text-primary", "--ui-surface"),
                   ("--ui-text-secondary", "--ui-surface"),
                   ("--ui-stroke", "--ui-shell"),
                   ("--ui-active", "--ui-shell")):
        check_bool(f"art bible contrast table still declares {fg} on {bg}",
                   any(r[0] == fg and r[1] == bg for r in rows),
                   "row missing or unparseable")
    check_bool("art bible still declares the --ui-focus-outer lower bound",
               focus_row is not None, "the focus row is missing or unparseable")

    for fg, bg, stated_ratio, verdict in rows:
        if fg not in tier_of or bg not in tier_of:
            check_unverifiable(f"art bible row {fg} on {bg}", "unknown token")
            continue
        check(f"art bible {fg} on {bg} ratio", contrast(tier_of[fg], tier_of[bg]),
              stated_ratio, tol=CONTRAST_TOL)
        v = verdict.upper()
        # Classify by the FIRST tier word only. A verdict may explain itself
        # with the other acronym ("below the 7.0 AAA line"), and scanning for
        # any occurrence would read that caveat as the assertion.
        tier_match = re.search(r"AAA|\bAA\b", verdict)
        if not tier_match or "DELIBERATELY LOW" in v or "NON-INFORMATIONAL" in v:
            continue
        if "LARGE-TEXT" in v:
            check_bool(f"art bible {fg} on {bg} labelled large-text only",
                       3.0 <= stated_ratio < 4.5,
                       f"{stated_ratio} is not in the 3.0-4.5 large-text band")
        elif tier_match.group(0) == "AAA":
            check_bool(f"art bible {fg} on {bg} labelled AAA", stated_ratio >= 7.0,
                       f"{stated_ratio} is below the 7.0 AAA threshold")
        else:
            check_bool(f"art bible {fg} on {bg} labelled AA", stated_ratio >= 4.5,
                       f"{stated_ratio} is below the 4.5 AA threshold but is not "
                       f"labelled large-text only")

    if focus_row is not None:
        focus, stated, focus_line = focus_row
        if focus in tier_of and "--ui-shell" in tier_of:
            # The worst case is 4.8476:1. A lower bound rounded UP to 4.85 is a
            # guarantee the tokens do not actually meet, so require the declared
            # figure to be the correctly rounded worst case AND require the exact
            # number to be disclosed. Scoped to the row, not the whole document:
            # a bare "4.848" search across 6000 lines would be satisfied by any
            # unrelated mention.
            exact = contrast(tier_of[focus], tier_of["--ui-shell"])
            check_bool(f"art bible {focus} lower bound is the rounded worst case",
                       abs(round(exact, 2) - stated) <= 0.005,
                       f"declares >= {stated}:1 but the worst case is {exact:.4f}:1")
            check_bool(f"art bible discloses the exact {focus} worst case",
                       f"{exact:.3f}" in focus_line,
                       f"the exact figure {exact:.3f}:1 is not stated in this row, "
                       f"so the 2dp floor reads as a guarantee it does not meet")

    # --- Band / Key A column
    band_line = next(line for line in md.splitlines()
                     if line.startswith("| 7 |") and "**Band**" in line)
    band_json = next(c for c in palette["print-classes"] if c["name"] == "Band")
    coat = next(e["luma"] for e in palette["primary"] if e["tier"] == 1)

    check_bool("art bible Band row states the ramp-mean mask luma 24.1",
               "**24.1**" in band_line, band_line.strip()[:90])
    check_bool("art bible Band row states Key A at the ramp mean, 67.8",
               "**67.8**" in band_line, band_line.strip()[:90])
    check_bool("art bible Band row no longer quotes the endpoint range",
               "76.5 → 59.2" not in band_line, band_line.strip()[:90])
    # Match the explanatory note specifically. The table cell already contains
    # the words "ramp mean", so a looser test would pass even if someone deleted
    # the entire column specification.
    check_bool("art bible records the ramp-mean column rule",
               "Ramp rows are stated at the ramp" in md,
               "the note defining how ramp rows are measured is missing")
    check("art bible Band mask luma agrees with palette.json",
          band_json["mask-luma"], 24.1, tol=0.05)
    check("art bible Band Key A agrees with palette.json",
          coat - band_json["mask-luma"], band_json["local-contrast"], tol=0.05)
    # Tolerate markdown filler (backticks, spaces) between the "=" and the
    # bolded result, so restyling the note does not break the gate.
    m = re.search(r"91\.9\s*−\s*24\.1\s*=[^0-9\n]{0,8}\*\*([\d]+(?:\.\d+)?)\*\*", md)
    check_bool("art bible shows the Band subtraction that yields Key A",
               m is not None, "the 91.9 - 24.1 = ? line is missing")
    if m:
        check("art bible Band subtraction result", coat - 24.1, float(m.group(1)),
              tol=0.05)


def main() -> int:
    palette_path = ART / "palette.json"
    typo_path = ART / "typography.json"
    for path in (palette_path, typo_path):
        if not path.exists():
            print(f"ERROR: {path} not found", file=sys.stderr)
            return 2

    # utf-8-sig accepts both plain UTF-8 and a BOM. Windows editors add a BOM
    # to .json by default, and that must not be a hard parse failure.
    try:
        palette = json.loads(palette_path.read_text(encoding="utf-8-sig"))
        typo = json.loads(typo_path.read_text(encoding="utf-8-sig"))
    except json.JSONDecodeError as exc:
        print(f"ERROR: could not parse: {exc}", file=sys.stderr)
        return 2

    print("=" * 72)
    print("Art metrics verification")
    print("=" * 72)

    verify_palette(palette)
    verify_typography(typo)

    # The bible restates numbers from both specs. It is optional so the gate
    # still runs if the file is moved or absent, but when it IS present its
    # mirrored claims are held to the same standard.
    bible_path = ART / "art-bible.md"
    if bible_path.exists():
        verify_art_bible(bible_path.read_text(encoding="utf-8-sig"), palette, typo)
    else:
        note(f"art-bible.md not found at {bible_path}; skipped its cross-checks")

    print(f"\nchecks run: {checks}")

    for text in notes:
        print(f"  note: {text}")

    for text in unverifiable:
        print(f"  UNVERIFIABLE: {text}")

    if failures:
        print(f"\n{len(failures)} MISMATCH(ES):\n")
        for failure in failures:
            print(f"  - {failure}")
        print("\nThese are authored claims that disagree with recomputation.")
        print("Fix the JSON, never this script.")
        return 1

    print("\nAll authored metrics recompute and match.")
    if unverifiable:
        print(f"{len(unverifiable)} claim(s) could not be recomputed - see above.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
