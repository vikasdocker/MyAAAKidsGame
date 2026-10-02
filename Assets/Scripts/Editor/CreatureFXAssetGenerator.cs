using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dab.Editor
{
    /// <summary>
    /// Generates the three FX burst sprites consumed by
    /// <see cref="Dab.Runtime.FX.CreatureFXSpawner"/>.
    ///
    /// Palette and bible constraints the generator obeys
    /// ------------------------------------------------
    ///   - Every sprite is white with straight alpha, so any runtime tint
    ///     (material _BaseColor) multiplies in. No sprite carries baked colour;
    ///     colour comes from the runtime material, which is where palette.json
    ///     semantics live.
    ///   - The success burst is a FIXED 12-spoke radial (palette.json
    ///     MinigameSuccess / C7). It carries no fixed colour and must inherit the
    ///     child's paint hue; the texture only encodes shape and spokes.
    ///   - The heart is loaded through the Attention semantic (#FFE3B8), which
    ///     is applied at runtime by the spawner, never baked here.
    ///   - art-bible 2.x bans dripping/runs/splatter, so the wet-paint burst is
    ///     a soft absorb blot, not a splat.
    ///
    /// Output lives under Assets/Art/FX/Resources/ so <c>Resources.Load</c> can
    /// fetch it at runtime (no prefab wiring exists yet). The generated PNG is
    /// the runtime-included copy; the generator is the source of truth.
    ///
    /// Menu: Tools/Dab/Art/Generate FX Textures
    /// Headless: -executeMethod Dab.Editor.CreatureFXAssetGenerator.GenerateAll
    /// </summary>
    public static class CreatureFXAssetGenerator
    {
        public const string OutputFolder = "Assets/Art/FX/Resources/FX";
        public const string HeartFileName = "FX_FloatingHeart.png";
        public const string InkFileName = "FX_InkBloom.png";
        public const string RadialFileName = "FX_SuccessRadial.png";

        private const int Size = 256;
        private const int Spokes = 12;

        [MenuItem("Tools/Dab/Art/Generate FX Textures")]
        public static void GenerateAll()
        {
            var files = new List<string>
            {
                WritePng(HeartFileName, CreateHeart()),
                WritePng(InkFileName, CreateInkBloom()),
                WritePng(RadialFileName, CreateSuccessRadial())
            };

            AssetDatabase.Refresh();

            for (var i = 0; i < files.Count; i++)
            {
                ConfigureTextureImport(files[i]);
            }

            AssetDatabase.SaveAssets();

            Debug.Log(
                "[CreatureFXAssetGenerator] Generated " + files.Count +
                " FX sprites under " + OutputFolder + ".");
            foreach (var file in files)
            {
                Debug.Log("  " + file);
            }
        }

        #region PNG Writing

        private static string WritePng(string fileName, Color32[] pixels)
        {
            EnsureFolder(OutputFolder);

            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            texture.SetPixels32(pixels);
            texture.Apply(false, false);

            var path = Path.Combine(OutputFolder, fileName).Replace('\\', '/');
            File.WriteAllBytes(path, texture.EncodeToPNG());

            UnityEngine.Object.DestroyImmediate(texture);
            return path;
        }

        private static void ConfigureTextureImport(string assetPath)
        {
            if (AssetImporter.GetAtPath(assetPath) is not TextureImporter importer)
            {
                Debug.LogWarning(
                    "[CreatureFXAssetGenerator] No TextureImporter for " + assetPath);
                return;
            }

            importer.textureType = TextureImporterType.Default;
            importer.alphaIsTransparency = true;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.mipmapEnabled = false;
            importer.sRGBTexture = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }

        #endregion

        #region Shape Sampling

        private static byte Alpha(float a)
        {
            return (byte)Mathf.RoundToInt(Mathf.Clamp(a, 0f, 1f) * 255f);
        }

        private static float Smooth(float edge0, float edge1, float x)
        {
            var t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        /// <summary>
        /// Algebraic heart: (x^2 + y^2 - 1)^3 - x^2 y^3 &lt;= 0, scaled to fit the
        /// tile. Feathering is in curve-value space, which is a wide but stable
        /// soft edge for a 256px tile.
        /// </summary>
        private static Color32[] CreateHeart()
        {
            var pixels = new Color32[Size * Size];
            const float WorldWidth = 2.6f;
            const float WorldHeight = 2.2f;

            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var u = WorldWidth * (x / (Size - 1f) - 0.5f);
                    var v = WorldHeight * (0.5f - y / (Size - 1f));

                    var f = Mathf.Pow(u * u + v * v - 1f, 3f) - u * u * v * v * v;

                    // clamp(0.5 - F / width, 0, 1): inside the curve F < 0 gives
                    // alpha 1, outside it falls away over roughly 0.024 of curve
                    // value, which reads as a soft crayon edge rather than a hard
                    // cutout.
                    var alpha = Mathf.Clamp01(0.5f - f / 0.024f);
                    pixels[y * Size + x] = new Color32(255, 255, 255, Alpha(alpha));
                }
            }

            return pixels;
        }

        /// <summary>
        /// Soft absorb blot. The sum of a few offset radial falloffs makes an
        /// irregular, feathered wet-paint shape with no dripping, no runs, and
        /// no splatter points (art-bible ban).
        /// </summary>
        private static Color32[] CreateInkBloom()
        {
            var blobs = new[]
            {
                new Vector4(0.50f, 0.52f, 0.30f, 1f),
                new Vector4(0.43f, 0.44f, 0.20f, 1f),
                new Vector4(0.58f, 0.41f, 0.22f, 1f),
                new Vector4(0.53f, 0.63f, 0.18f, 1f)
            };

            var pixels = new Color32[Size * Size];

            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var px = (x + 0.5f) / Size;
                    var py = (y + 0.5f) / Size;

                    var value = 0f;
                    for (var b = 0; b < blobs.Length; b++)
                    {
                        var d = Mathf.Sqrt(
                            (px - blobs[b].x) * (px - blobs[b].x) +
                            (py - blobs[b].y) * (py - blobs[b].y));
                        value += Smooth(blobs[b].z, 0f, d);
                    }

                    value = Mathf.Clamp01(value);

                    // Soft fill between value 0.3 and 0.65, then a wide outer
                    // feather so the edge never reads as a drilled outline.
                    var fill = Smooth(0.3f, 0.65f, value);
                    var radial = Mathf.Sqrt((px - 0.5f) * (px - 0.5f) + (py - 0.5f) * (py - 0.5f));
                    var feather = 1f - Smooth(0.5f, 0.85f, radial);

                    pixels[y * Size + x] = new Color32(255, 255, 255, Alpha(fill * feather));
                }
            }

            return pixels;
        }

        /// <summary>
        /// Fixed 12-spoke radial (palette.json MinigameSuccess / C7). The
        /// spokes are tapered outward, terminate before the tile edge, and sit
        /// on a small solid core so the burst reads as a wheel even in a still
        /// texture.
        /// </summary>
        private static Color32[] CreateSuccessRadial()
        {
            var pixels = new Color32[Size * Size];
            var step = 2f * Mathf.PI / Spokes;

            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var px = (x + 0.5f) / Size - 0.5f;
                    var py = (y + 0.5f) / Size - 0.5f;

                    var radius = Mathf.Sqrt(px * px + py * py);
                    var theta = Mathf.Atan2(py, px);

                    // Distance to the nearest spoke centre, in radians.
                    var nearest = theta / step;
                    var k = Mathf.RoundToInt(nearest);
                    var delta = Mathf.Abs(theta - k * step);
                    delta = Mathf.Min(delta, step - delta);

                    // Spokes taper outward: narrow at the tip.
                    var halfWidth = 0.030f * (1f - 0.45f * radius);
                    var spoke = 1f - delta / halfWidth;

                    // Solid core so the centre of the burst does not read as a
                    // hole, and a soft termination before the tile edge.
                    var core = 1f - (radius - 0.10f) / 0.05f;
                    var outer = 1f - (radius - 0.85f) / 0.15f;

                    var alpha = Mathf.Max(spoke, core);
                    alpha *= Mathf.Clamp01(outer);

                    pixels[y * Size + x] = new Color32(255, 255, 255, Alpha(alpha));
                }
            }

            return pixels;
        }

        #endregion

        #region Folder Helpers

        private static void EnsureFolder(string path)
        {
            if (path == "Assets" || AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        #endregion
    }
}