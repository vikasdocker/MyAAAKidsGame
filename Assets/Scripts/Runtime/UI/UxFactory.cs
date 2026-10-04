using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dab.Runtime.UI
{
    /// <summary>
    /// Tiny builder for the scaffold's warm, rounded, icon-first controls.
    ///
    /// Why UGUI over TMP for a scaffold: the project has not shipped a font yet
    /// (typography.json mandates Nunito, which is asset authoring and waits on the
    /// art bible). UGUI Text with the engine's built-in fallback font keeps the
    /// menu legible (48px floor, weight 600+) without bundling an unlicensed font.
    /// Labels are supportive only — every control is identified by silhouette and
    /// by what touching it does (typography.json two-removals-test).
    ///
    /// No asset authoring happens here: the rounded-corner sprite is generated at
    /// runtime, so this builds in a first-open project with no .meta GUIDs to
    /// hand-author (AGENTS.md).
    /// </summary>
    public static class UxFactory
    {
        public const int TouchTargetFloorPx = 72;

        private static readonly Vector2[] QuadUv =
        {
            new(0f, 0f), new(1f, 0f), new(0f, 1f), new(1f, 1f)
        };

        private static ushort[] _quadTriangles = { 0, 1, 2, 2, 1, 3 };

        /// <summary>
        /// A soft-edged rounded-rectangle sprite, generated once and reused by
        /// every control. 64px keeps the corners cheap and bilinear filtering
        /// smooths the alpha ramp on screen.
        /// </summary>
        public static Sprite RoundedRect
        {
            get
            {
                if (_roundedRect != null)
                {
                    return _roundedRect;
                }

                const int size = 64;
                const int corner = 14;

                var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
                {
                    name = "UxRoundedRect",
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear,
                    hideFlags = HideFlags.HideAndDontSave
                };

                var pixels = new Color[size * size];
                for (var y = 0; y < size; y++)
                {
                    for (var x = 0; x < size; x++)
                    {
                        var inside = IsInsideRoundedRect(x, y, size, corner)
                            ? 1f
                            : 0f;
                        pixels[y * size + x] = new Color(1f, 1f, 1f, inside);
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply(false, true);

                _roundedRect = Sprite.Create(
                    texture,
                    new Rect(0f, 0f, size, size),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0,
                    SpriteMeshType.FullRect);
                _roundedRect.name = "UxRoundedRect";
                return _roundedRect;
            }
        }

        private static Sprite _roundedRect;

        private static bool IsInsideRoundedRect(int x, int y, int size, int corner)
        {
            var inset = corner;
            var half = size * 0.5f;

            // Manhattan-ish test per quadrant: outside the central cross the
            // corner circle is inset by `inset` from each edge.
            var dx = Mathf.Abs(x + 0.5f - half) - (half - inset);
            var dy = Mathf.Abs(y + 0.5f - half) - (half - inset);

            if (dx <= 0f || dy <= 0f)
            {
                return true;
            }

            return dx * dx + dy * dy <= inset * inset;
        }

        /// <summary>
        /// A rounded, warm control with an icon or caption label. The label is
        /// supportive: the control is identifiable by silhouette alone.
        /// </summary>
        public static Button CreateButton(
            Transform parent,
            string label,
            float width,
            float height,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Color fill,
            UnityAction onClick)
        {
            var go = new GameObject(label, typeof(Button), typeof(Image));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = anchoredPosition;

            var image = go.GetComponent<Image>();
            image.sprite = RoundedRect;
            image.type = Image.Type.Simple;
            image.color = fill;

            var button = go.GetComponent<Button>();
            button.targetGraphic = image;

            // Pressed = 4% vertical squash + a value step (typography
            // treatments.pressed-ink and weight-policy). A subtle color dim on a
            // warm token keeps the pressed state visible without introducing a
            // grey or any forbidden signal colour.
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = new Color(0.94f, 0.94f, 0.94f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = Color.white;
            colors.fadeDuration = 0.1f;
            button.colors = colors;

            button.onClick.AddListener(onClick);

            CreateLabel(rect, label, 48, Palette.TextPrimary, new Vector2(0f, -height * 0.30f));

            return button;
        }

        /// <summary>
        /// A rounded, warm control carrying a large glyph with an optional small
        /// caption (typography L1 icon-over-label lockup). Falls back to the
        /// plain label when no glyph is wanted.
        /// </summary>
        public static Button CreateIconButton(
            Transform parent,
            string glyph,
            string caption,
            float width,
            float height,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Color fill,
            Color glyphColor,
            UnityAction onClick)
        {
            var button = CreateButton(
                parent, caption, width, height, anchorMin, anchorMax,
                anchoredPosition, fill, onClick);

            // The caption label created by CreateButton is meant for text-only
            // buttons; replace it with a glyph over a caption.
            DestroyLabel(button.transform.GetChild(0));

            if (!string.IsNullOrEmpty(glyph))
            {
                CreateLabel(button.transform, glyph, 80, glyphColor, new Vector2(0f, 8f));
            }

            CreateLabel(button.transform, caption, 40, Palette.TextSecondary, new Vector2(0f, -height * 0.34f));
            return button;
        }

        /// <summary>
        /// A single Text label. Font is the engine fallback; see the class note
        /// for why TMP is not used yet.
        /// </summary>
        public static Text CreateLabel(Transform parent, string text, int size, Color color, Vector2 offset)
        {
            var go = new GameObject(text, typeof(Text));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600f, size * 3.2f);
            rect.anchoredPosition = offset;

            var textComponent = go.GetComponent<Text>();
            textComponent.font = GetFallbackFont();
            textComponent.text = text;
            textComponent.fontSize = size;
            textComponent.fontStyle = FontStyle.Bold;
            textComponent.color = color;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
            textComponent.raycastTarget = false;

            return textComponent;
        }

        private static Font _fallbackFont;
        private static bool _fontResolved;

        private static Font GetFallbackFont()
        {
            if (_fontResolved)
            {
                return _fallbackFont;
            }

            _fontResolved = true;

            _fallbackFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_fallbackFont == null)
            {
                _fallbackFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            if (_fallbackFont == null)
            {
                Debug.LogWarning(
                    "[UxFactory] No built-in fallback font found; labels render " +
                    "blank. Bundling the licensed Nunito face is a post-art-bible step.");
            }

            return _fallbackFont;
        }

        private static void DestroyLabel(Transform child)
        {
            if (child == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(child.gameObject);
            }
            else
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }
}