using UnityEngine;

namespace Dab.Runtime.UI
{
    /// <summary>
    /// Colour tokens lifted verbatim from design/Art/palette.json. Names match
    /// the spec's tokens so a future UI pass can diff against the source of truth
    /// instead of re-deriving hex values. No UI colour may exist outside the world
    /// palette (palette.json "ui.divergence"); everything here is a listed token.
    /// </summary>
    public static class Palette
    {
        /// <summary>--ui-shell: page ground, menu dim.</summary>
        public static readonly Color Shell = ColorFromHex(0xBEA88B);

        /// <summary>--ui-surface: card, panel, palette tray.</summary>
        public static readonly Color Surface = ColorFromHex(0xDABE98);

        /// <summary>--ui-surface-raised: raised card, tooltip.</summary>
        public static readonly Color SurfaceRaised = ColorFromHex(0xE8D5B8);

        /// <summary>--ui-active: active control, pressed card.</summary>
        public static readonly Color Active = ColorFromHex(0xF7E9D2);

        /// <summary>--ui-text-primary: all body and label text (9.28:1 on Active).</summary>
        public static readonly Color TextPrimary = ColorFromHex(0x4A382A);

        /// <summary>--ui-text-secondary: supporting caption (4.21:1 on Surface, large-text only).</summary>
        public static readonly Color TextSecondary = ColorFromHex(0x6B4F3A);

        /// <summary>--ui-keyline: 4px border on every colour swatch.</summary>
        public static readonly Color Keyline = ColorFromHex(0xF7E9D2);

        /// <summary>--ui-stroke / --ui-focus-outer: icon strokes, outlines.</summary>
        public static readonly Color Stroke = ColorFromHex(0x4A382A);

        /// <summary>Honey Field: the world, sunlit and unremarkable (ground bowl).</summary>
        public static readonly Color HoneyField = ColorFromHex(0xE8C79B);

        /// <summary>Dawn Cream: the world's brightest representative surface.</summary>
        public static readonly Color DawnCream = ColorFromHex(0xF7E9D2);

        /// <summary>Peach Shadow: cast shadows — always a grounding cue, never a problem.</summary>
        public static readonly Color PeachShadow = ColorFromHex(0xF6B9A0);

        /// <summary>Attention / celebration glow. Declared NON-INFORMATIONAL vs Dawn Cream (C2).</summary>
        public static readonly Color AttentionGlow = ColorFromHex(0xFFE3B8);

        private static Color ColorFromHex(uint rgb)
        {
            return new Color(
                ((rgb >> 16) & 0xFF) / 255f,
                ((rgb >> 8) & 0xFF) / 255f,
                (rgb & 0xFF) / 255f,
                1f);
        }
    }
}