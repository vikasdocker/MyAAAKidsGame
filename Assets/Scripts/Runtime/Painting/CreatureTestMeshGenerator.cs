using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dab.Runtime.Painting
{
    /// <summary>
    /// Procedural test mesh for the freehand-painting prototype.
    ///
    /// Why this exists
    /// ---------------
    /// AGENTS.md §9 flags freehand 3D painting as the single highest-risk
    /// mechanic: it must be prototyped on low-end Android before anything
    /// downstream is built. That prototype needs a creature surface to paint
    /// on, and a hand-authored test mesh would not answer the questions the
    /// controller actually raises (see <see cref="CreaturePaintController"/>):
    ///
    ///   - does a child's finger feel good laying colour on a curved surface
    ///   - does the additive stamp avoid seams at stroke joins
    ///   - does brush radius in UV space survive varying texel density
    ///   - is the paint legible at the 1024 default
    ///
    /// The third question is the reason the UV layout here is deliberate rather
    /// than incidental. A naive lathe maps V linearly to height, which puts a
    /// dense cluster of texels near the poles and starves the equator — a
    /// constant UV brush radius then paints a visibly larger mark on the belly
    /// than on the head. That artefact is easy to mistake for a painting-feel
    /// problem when it is really a UV problem. <see cref="UvMode.AreaCorrected"/>
    /// exists so the prototype can measure the difference.
    ///
    /// Scope — this is a test harness, not the shipping creature
    /// -------------------------------------------------------
    /// Procedural geometry is chosen because the prototype needs a surface
    /// *today*, before art production starts (AGENTS.md §7 anchors visual
    /// identity as provisional, §8 forbids building content that is not in a
    /// GDD). It is explicitly not the delivery path: the real creature is a
    /// sculpted, rigged, Pillar-3-variant mesh. This file gets deleted when
    /// that arrives. It is not a creature variation generator.
    ///
    /// Nothing here reads or writes paint state. It produces geometry and a
    /// screen-to-UV bridge; <see cref="CreaturePaintController"/> owns painting.
    ///
    /// Art-bible conformance (design/Art/art-bible.md §3.1)
    /// ----------------------------------------------------
    ///   - height 1.00 H, minimum width 0.62 H, head Ø 0.48 H at 0.74 H
    ///   - no neck: head and body are one continuous revolved form
    ///   - limb nubs ≤ 0.10 H long, ≤ 0.06 H wide, rounded caps
    ///   - ground clearance 0 H (the body sits on the ground plane)
    ///   - every appendage tip ≥ 40% of its own base width (capsules, not cones)
    ///   - no polygon edge reads as a corner: the body is a smooth revolved
    ///     profile and appendages are capsules, so the minimum 35%-of-shortest-
    ///     local-axis corner radius holds by construction rather than by tuning
    ///   - appendage area outside the body mass stays far under the 12% cap
    ///   - body 8–12k triangles, per the Pillar 3 topology target
    ///
    /// This is matte painted shell, never fur (AGENTS.md §7) — the child's
    /// paint sits directly on this surface, so there is nothing to grow.
    ///
    /// Determinism: no <see cref="UnityEngine.Random"/>. Placement uses
    /// <see cref="System.Random"/> seeded from <see cref="_variationSeed"/>, so
    /// a given seed always reproduces the same mesh and the global random state
    /// is never consumed. Needed because AGENTS.md §2.2 forbids randomness in
    /// gameplay; a test harness that perturbed the global stream could make a
    /// paint bug non-reproducible.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [AddComponentMenu("Dab/Painting/Creature Test Mesh Generator")]
    public sealed class CreatureTestMeshGenerator : MonoBehaviour
    {
        #region Nested Types

        /// <summary>How V is distributed across the body.</summary>
        public enum UvMode
        {
            /// <summary>V maps linearly to height. Cheap, poles pinch.</summary>
            LinearHeight,

            /// <summary>
            /// V follows cumulative meridian arc length, so texel rows are
            /// distributed by how much surface they cover. Default: it makes
            /// brush size in UV space mean roughly the same thing everywhere,
            /// which is the question the prototype is trying to answer.
            /// </summary>
            AreaCorrected
        }

        /// <summary>One control point of the body's revolved profile.</summary>
        private struct ProfilePoint
        {
            public readonly float Height;
            public readonly float Radius;

            public ProfilePoint(float height, float radius)
            {
                Height = height;
                Radius = radius;
            }
        }

        /// <summary>Oriented frame a lathed form is placed into.</summary>
        private struct Frame
        {
            public Vector3 Base;
            public Vector3 AxisX;
            public Vector3 AxisY;
            public Vector3 AxisZ;

            public Vector3 Point(float x, float y, float z)
            {
                return Base + AxisX * x + AxisY * y + AxisZ * z;
            }
        }

        #endregion

        #region Canonical Profile

        // art-bible.md §3.1 "canonical hatchling form", as fractions of H.
        //
        // The shape is one continuous revolved form with no neck, so the head
        // is not a separate sphere — it is the upper bulge of the same profile.
        // Widest point sits low (v = 0.42) for a heavy-bottomed read, and the
        // profile at v = 0.74 has radius 0.24 H, giving the specified 0.48 H
        // head diameter with the top of the head landing at 0.98 H.
        //
        // The first and last entries are zero so the lathe closes into rounded
        // poles. No entry produces a corner: radius changes smoothly and never
        // reaches a vertex, which is what keeps rule 1 of the hard geometry
        // rules satisfied on the body.
        private static readonly ProfilePoint[] CanonicalProfile =
        {
            new ProfilePoint(0.00f, 0.000f),
            new ProfilePoint(0.06f, 0.150f),
            new ProfilePoint(0.15f, 0.252f),
            new ProfilePoint(0.30f, 0.306f),
            new ProfilePoint(0.42f, 0.310f), // widest: 0.62 H silhouette width
            new ProfilePoint(0.55f, 0.298f),
            new ProfilePoint(0.66f, 0.276f),
            new ProfilePoint(0.74f, 0.240f), // head centre: 0.48 H head diameter
            new ProfilePoint(0.84f, 0.208f),
            new ProfilePoint(0.92f, 0.152f),
            new ProfilePoint(0.97f, 0.082f),
            new ProfilePoint(1.00f, 0.000f)
        };

        /// <summary>Widest silhouette width as a fraction of height.</summary>
        private const float CanonicalWidthFraction = 0.62f;

        /// <summary>Head diameter as a fraction of height.</summary>
        private const float CanonicalHeadFraction = 0.48f;

        /// <summary>Height at which the head centre sits.</summary>
        private const float CanonicalHeadHeightFraction = 0.74f;

        // Limb nubs. art-bible.md §3.1 caps them at 0.10 H long and 0.06 H
        // wide, and hard rule 2 requires every appendage tip to be at least 40%
        // of its own base width. A capsule satisfies both structurally: the
        // tip is a hemisphere of the same diameter as the base, so the tip is
        // 100% of base width rather than tapering to a vertex. Exposed length
        // is 0.065 H because the base 0.035 H is embedded in the body.
        private const float NubRadiusFraction = 0.03f;  // 0.06 H wide
        private const float NubLengthFraction = 0.10f;
        private const float NubEmbedFraction = 0.035f;

        // UV island layout. The body owns the lower band; appendages get slots
        // in the upper band so a stroke cannot smear from the body onto a foot.
        // Separate islands do mean a stroke crossing that boundary jumps, which
        // is a real limitation of the test harness rather than of the painting
        // mechanic — noted here so it is not mistaken for a bug in
        // CreaturePaintController when it shows up on a playtest.
        //
        // Slot count covers limbs (4) and eyes (2) so no two appendages ever
        // share a UV region. Sharing would make painting one of them change the
        // other's colour.
        private const int AppendageUvSlots = 6;
        private const int EyeUvSlotOffset = 4;
        private const float BodyUvVMax = 0.85f;
        private const float AppendageUvVMin = 0.88f;

        #endregion

        #region Configuration

        [Header("Body")]
        [Tooltip("Total silhouette height in world units. The art bible defines " +
                 "every other measurement as a fraction of this, so changing it " +
                 "scales the whole creature without breaking proportions.")]
        [SerializeField, Min(0.01f)]
        private float _height = 1f;

        [Tooltip("Latitude divisions from pole to pole. Body triangle count is " +
                 "roughly double this times the longitude count, which the art " +
                 "bible targets at 8–12k for the body alone.")]
        [SerializeField, Range(16, 160)]
        private int _latitudeSegments = 64;

        [Tooltip("Longitude divisions around the body. 80 keeps the silhouette " +
                 "smooth at the 64 px readability test without extra triangles.")]
        [SerializeField, Range(8, 256)]
        private int _longitudeSegments = 80;

        [Header("UV Layout")]
        [Tooltip("How V is distributed. Area-corrected gives more even texel " +
                 "density, which matters because the prototype is testing " +
                 "whether a constant UV brush radius feels consistent.")]
        [SerializeField]
        private UvMode _uvMode = UvMode.AreaCorrected;

        [Header("Appendages")]
        [Tooltip("Add four rounded limb nubs. They are deliberately shallow — " +
                 "the art bible requires nubs never break the silhouette.")]
        [SerializeField]
        private bool _includeLimbs = true;

        [Tooltip("Add two eye forms. Off by default because the base creature " +
                 "is a plain hatchling with no marks yet; the child's paint is " +
                 "what should be the vivid thing in frame (AGENTS.md §7).")]
        [SerializeField]
        private bool _includeEyes;

        [Header("Scene Wiring")]
        [Tooltip("Add and maintain a MeshCollider so TryScreenToUv can raycast. " +
                 "CreaturePaintController needs a screen-to-UV function and has " +
                 "no built-in one; this is the bridging implementation.")]
        [SerializeField]
        private bool _ensureMeshCollider = true;

        [Tooltip("Camera used by TryScreenToUv when the caller passes null. " +
                 "Falls back to Camera.main at call time.")]
        [SerializeField]
        private Camera _uvCamera;

        [Tooltip("Seed for appendage placement. System.Random, never UnityEngine " +
                 "Random, so the global random state is untouched and a given " +
                 "seed always reproduces the same mesh.")]
        [SerializeField]
        private int _variationSeed = 20260930;

        #endregion

        #region Public State

        /// <summary>Generated body mesh. Null before the first build.</summary>
        public Mesh GeneratedMesh { get; private set; }

        /// <summary>Triangles in the body alone. Art-bible target is 8–12k.</summary>
        public int BodyTriangleCount { get; private set; }

        /// <summary>Body plus appendages.</summary>
        public int TotalTriangleCount { get; private set; }

        /// <summary>Widest silhouette width, in fractions of height.</summary>
        public float WidthFraction => _widthFraction;

        /// <summary>Head diameter at the head centre, in fractions of height.</summary>
        public float HeadDiameterFraction => _headDiameterFraction;

        /// <summary>Estimated appendage area outside the body, as a share of the
        /// silhouette. Art-bible cap is 12%.</summary>
        public float AppendageAreaShare => _appendageAreaShare;

        /// <summary>Ratio of densest to sparsest texel density. 1.0 is perfectly
        /// even. Diagnostic for "does brush radius survive varying density".</summary>
        public float TexelDensitySpread => _texelDensitySpread;

        /// <summary>Human-readable conformance report. Built with the mesh.</summary>
        public string BuildReport => _buildReport;

        /// <summary>Raised after every successful rebuild.</summary>
        public event Action<CreatureTestMeshGenerator> MeshRebuilt;

        #endregion

        #region Internal State

        private readonly List<Vector3> _vertices = new List<Vector3>(8192);
        private readonly List<Vector3> _normals = new List<Vector3>(8192);
        private readonly List<Vector2> _uvs = new List<Vector2>(8192);
        private readonly List<int> _triangles = new List<int>(24576);

        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        private bool _isDirty = true;
        private float _widthFraction;
        private float _headDiameterFraction;
        private float _appendageAreaShare;
        private float _texelDensitySpread;
        private string _buildReport = string.Empty;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _isDirty = true;
        }

        private void Update()
        {
            // ExecuteAlways drives this in edit mode too, so dragging a slider
            // rebuilds live and the silhouette can be checked against the art
            // bible without entering play mode.
            if (_isDirty)
            {
                Rebuild();
            }
        }

        private void OnValidate()
        {
            _isDirty = true;
        }

        private void OnDisable()
        {
            if (GeneratedMesh != null)
            {
                DestroySafely(GeneratedMesh);
                GeneratedMesh = null;
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Rebuilds the mesh and all derived measurements. Called automatically
        /// whenever a serialized value changes; exposed for explicit rebuilds
        /// from code and the inspector context menu.
        /// </summary>
        [ContextMenu("Rebuild Mesh")]
        public void Rebuild()
        {
            if (_meshFilter == null)
            {
                _meshFilter = GetComponent<MeshFilter>();
            }

            BuildBody();

            if (_includeLimbs)
            {
                BuildLimbNubs();
            }

            if (_includeEyes)
            {
                BuildEyes();
            }

            var mesh = new Mesh
            {
                name = "CreatureTestMesh",
                // Deliberately NOT UploadMeshData(true). The mesh stays CPU-
                // readable because the MeshCollider built below needs to sample
                // its triangles for raycasts; releasing the buffer would break
                // TryScreenToUv, which is the whole point of this harness.
                hideFlags = HideFlags.DontSave
            };

            mesh.SetVertices(_vertices);
            mesh.SetNormals(_normals);
            mesh.SetUVs(0, _uvs);
            mesh.SetTriangles(_triangles, 0, true);
            mesh.RecalculateBounds();

            if (GeneratedMesh != null)
            {
                DestroySafely(GeneratedMesh);
            }

            GeneratedMesh = mesh;
            _meshFilter.sharedMesh = mesh;

            if (_ensureMeshCollider)
            {
                EnsureMeshCollider();
            }

            MeasureAndReport();

            _isDirty = false;
            MeshRebuilt?.Invoke(this);
        }

        /// <summary>
        /// Converts a screen position into a UV on this mesh.
        ///
        /// <see cref="CreaturePaintController.BindToInput"/> requires a
        /// <c>Func&lt;Vector2, Vector2&gt;</c> screen-to-UV function and does not
        /// supply one — this is the implementation for that prototype. Raycast
        /// hits on the back of the creature return their true surface UV, so a
        /// child can paint the far side by reaching across it; that is intended.
        ///
        /// Returns false when the position misses the creature, in which case
        /// <paramref name="uv"/> is <see cref="Vector2.zero"/> and the caller
        /// should not start or extend a stroke.
        /// </summary>
        public bool TryScreenToUv(Vector2 screenPosition, Camera camera, out Vector2 uv)
        {
            // Delegates rather than re-implementing. The raycast, the degenerate
            // direction guard and the collider-ownership filter are all shared
            // with TryScreenToSurface, so the filter that stops the harness
            // painting a UV belonging to some other object in the scene is
            // written down exactly once.
            if (!TryScreenToSurface(screenPosition, camera, out _, out _, out var surfaceUv))
            {
                uv = Vector2.zero;
                return false;
            }

            uv = surfaceUv;
            return true;
        }

        /// <summary>Overload using the configured or main camera.</summary>
        public bool TryScreenToUv(Vector2 screenPosition, out Vector2 uv)
        {
            return TryScreenToUv(screenPosition, null, out uv);
        }

        /// <summary>
        /// Resolves a screen position to a point on this mesh in world space,
        /// alongside the surface normal and the UV.
        ///
        /// Added for <c>CreatureFXSpawner</c>, which needs a world position to
        /// emit particles at and a normal to orient them along. The UV overload
        /// cannot serve that: a particle emitter is given world coordinates, and
        /// converting a UV back to a world point is not generally possible
        /// because a UV island is a flattened chart with no unique inverse.
        ///
        /// Shares one raycast implementation with <see cref="TryScreenToUv"/>
        /// deliberately. The collider-ownership filter below is the part that is
        /// easy to get subtly wrong: a second implementation would have to
        /// re-derive it, and a version that forgot would let the creature emit
        /// hearts from a background pebble it had just been touched by.
        ///
        /// <paramref name="normal"/> is the mesh normal at the hit, not a
        /// smoothed vertex normal, because the raycast cannot recover the
        /// interpolated normal without extra work that a particle emitter does
        /// not need. Good enough to orient a burst away from the surface.
        /// </summary>
        public bool TryScreenToSurface(
            Vector2 screenPosition,
            Camera camera,
            out Vector3 point,
            out Vector3 normal,
            out Vector2 uv)
        {
            point = Vector3.zero;
            normal = Vector3.zero;
            uv = Vector2.zero;

            if (_meshCollider == null && !TryEnsureMeshCollider())
            {
                return false;
            }

            var cam = camera != null ? camera : (_uvCamera != null ? _uvCamera : Camera.main);
            if (cam == null)
            {
                // Warned here rather than in the UV overload, because both
                // overloads now route through this method and a missing camera
                // is the same authoring mistake either way.
                Debug.LogWarning(
                    $"[{nameof(CreatureTestMeshGenerator)}] Screen-to-surface lookup " +
                    "needs a camera. Assign one, or leave it null and add a " +
                    "MainCamera tag.", this);
                return false;
            }

            var ray = cam.ScreenPointToRay(screenPosition);

            // A camera inside or extremely close to the surface produces a
            // degenerate ray; guard so this never throws on a malformed scene.
            if (ray.direction.sqrMagnitude < 1e-8f)
            {
                return false;
            }

            if (!Physics.Raycast(ray, out var hit, float.MaxValue))
            {
                return false;
            }

            if (!hit.collider.transform.IsChildOf(transform) &&
                hit.collider.transform != transform)
            {
                return false;
            }

            point = hit.point;
            normal = hit.normal;
            uv = hit.textureCoord;
            return true;
        }

        /// <summary>Overload using the configured or main camera.</summary>
        public bool TryScreenToSurface(
            Vector2 screenPosition,
            out Vector3 point,
            out Vector3 normal,
            out Vector2 uv)
        {
            return TryScreenToSurface(screenPosition, null, out point, out normal, out uv);
        }

        /// <summary>
        /// The delegate to hand to <c>CreaturePaintController.BindToInput</c>.
        /// Returns the UV of the nearest hit, or the last known UV when the ray
        /// misses — a drag that briefly leaves the creature should not chop the
        /// stroke into disconnected dots.
        /// </summary>
        public Func<Vector2, Vector2> CreateScreenToUvDelegate()
        {
            var lastUv = new Vector2(0.5f, 0.5f);
            var hasLast = false;

            return screenPosition =>
            {
                if (TryScreenToUv(screenPosition, out var uv))
                {
                    lastUv = uv;
                    hasLast = true;
                }

                return hasLast ? lastUv : new Vector2(0.5f, 0.5f);
            };
        }

        #endregion

        #region Profile Evaluation

        /// <summary>
        /// Radius of the body at normalized height <paramref name="v"/>, in
        /// world units. Catmull-Rom through the canonical control points.
        ///
        /// Catmull-Rom is chosen over a piecewise-smoothstep blend because
        /// smoothstep forces zero slope at every control point, which reads as
        /// faint horizontal creases on a large soft form. Catmull-Rom keeps the
        /// curve genuinely smooth through the points. It can overshoot, so the
        /// result is clamped to non-negative — overshoot near a pole would
        /// otherwise push radius negative and invert the lathe winding.
        /// </summary>
        private float EvaluateProfile(float v)
        {
            v = Mathf.Clamp01(v);

            var points = CanonicalProfile;

            if (v <= points[0].Height)
            {
                return 0f;
            }

            var lastIndex = points.Length - 1;
            if (v >= points[lastIndex].Height)
            {
                return 0f;
            }

            var i = 0;
            while (i < lastIndex - 1 && v > points[i + 1].Height)
            {
                i++;
            }

            var h1 = points[i].Height;
            var h2 = points[i + 1].Height;
            var span = h2 - h1;
            var t = span > 1e-6f ? (v - h1) / span : 0f;

            var p0 = points[Mathf.Max(0, i - 1)].Radius;
            var p1 = points[i].Radius;
            var p2 = points[i + 1].Radius;
            var p3 = points[Mathf.Min(lastIndex, i + 2)].Radius;

            var t2 = t * t;
            var t3 = t2 * t;

            var value = 0.5f *
                        ((2f * p1) +
                         (-p0 + p2) * t +
                         (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                         (-p0 + 3f * p1 - 3f * p2 + p3) * t3);

            return Mathf.Max(0f, value) * _height;
        }

        /// <summary>Normalized height (0–1) for a world-space Y on the body.</summary>
        private float WorldYToV(float worldY)
        {
            return Mathf.Clamp01(worldY / _height);
        }

        /// <summary>
        /// Outward normal for a lathe whose profile is <paramref name="radiusAtV"/>,
        /// whose total height is <paramref name="height"/>, at normalized height
        /// <paramref name="v"/> and azimuth <paramref name="theta"/>.
        ///
        /// <paramref name="height"/> is not a cosmetic parameter: the meridian
        /// tangent is (dr/dv, height) in real units, because the axial coordinate
        /// advances at <paramref name="height"/> per unit of v. Dropping it and
        /// hardcoding 1 is correct only for a lathe of height 1, and it silently
        /// over-weights the radial term for short appendages — a nub 0.10 H long
        /// got normals tilted roughly 10x too far outward, so its pole caps
        /// disagreed with their own triangle winding and lit inside-out.
        ///
        /// Derived analytically from the profile slope rather than via
        /// RecalculateNormals, because the lathe duplicates its seam column to
        /// carry u = 0 and u = 1. RecalculateNormals would give the two seam
        /// vertices different normals — they see different triangle sets — and
        /// that shows up as a visible lighting crease running down the creature.
        /// </summary>
        private static Vector3 MeridianNormal(
            Func<float, float> radiusAtV, float height, float v, float theta)
        {
            const float epsilon = 0.001f;

            var vLow = Mathf.Max(0f, v - epsilon);
            var vHigh = Mathf.Min(1f, v + epsilon);
            var dv = vHigh - vLow;

            float dr;
            if (dv > 1e-6f)
            {
                dr = (radiusAtV(vHigh) - radiusAtV(vLow)) / dv;
            }
            else
            {
                dr = 0f;
            }

            // Meridian tangent is (dr, height) in real units; the outward
            // meridian normal is that tangent rotated a quarter turn.
            var nx = height;
            var ny = -dr;
            var length = Mathf.Sqrt(nx * nx + ny * ny);
            if (length > 1e-6f)
            {
                nx /= length;
                ny /= length;
            }

            var cos = Mathf.Cos(theta);
            var sin = Mathf.Sin(theta);

            return new Vector3(nx * cos, ny, nx * sin).normalized;
        }

        /// <summary>
        /// Outward surface normal of the <em>body</em> at normalized height
        /// <paramref name="v"/> and azimuth <paramref name="theta"/>. Used to
        /// stand appendages off the body surface.
        /// </summary>
        private Vector3 SurfaceNormal(float v, float theta)
        {
            return MeridianNormal(EvaluateProfile, _height, v, theta);
        }

        /// <summary>A point on the body's surface, in world units.</summary>
        private Vector3 SurfacePoint(float v, float theta)
        {
            var radius = EvaluateProfile(v);
            return new Vector3(
                radius * Mathf.Cos(theta),
                v * _height,
                radius * Mathf.Sin(theta));
        }

        #endregion

        #region Mesh Construction

        /// <summary>
        /// The body itself — a single revolved surface, no neck, no separate
        /// head geometry. That continuity is the art bible's requirement, and it
        /// is also why painting reads well: there is no seam where a head would
        /// meet a neck for the child's stroke to catch on.
        /// </summary>
        private void BuildBody()
        {
            var frame = new Frame
            {
                Base = Vector3.zero,
                AxisX = Vector3.right,
                AxisY = Vector3.up,
                AxisZ = Vector3.forward
            };

            AppendLathe(
                EvaluateBodyProfile,
                _height,
                frame,
                _latitudeSegments,
                _longitudeSegments,
                new Rect(0f, 0f, 1f, BodyUvVMax),
                _uvMode == UvMode.AreaCorrected,
                _vertices,
                _normals,
                _uvs,
                _triangles);

            BodyTriangleCount = _triangles.Count / 3;
        }

        /// <summary>
        /// Body profile in world units, resolved to a flat function so the same
        /// lathe builder can serve the appendages.
        /// </summary>
        private float EvaluateBodyProfile(float v)
        {
            return EvaluateProfile(v);
        }

        private void BuildLimbNubs()
        {
            var random = new System.Random(_variationSeed);
            var radius = _height * NubRadiusFraction;
            var length = _height * NubLengthFraction;
            var embed = _height * NubEmbedFraction;

            // Two feet low and forward, two arm nubs at mid height and wide. The
            // seeded jitter is a few millimetres of H, purely to stop the form
            // reading as machined; it never approaches the 0.10 H length cap.
            var placement = new[]
            {
                new NubPlacement(0.09f, 40f, radius, length, Jitter(random, 0.012f)),
                new NubPlacement(0.09f, 140f, radius, length, Jitter(random, 0.012f)),
                new NubPlacement(0.50f, 215f, radius, length, Jitter(random, 0.018f)),
                new NubPlacement(0.50f, 325f, radius, length, Jitter(random, 0.018f))
            };

            for (var i = 0; i < placement.Length; i++)
            {
                var nub = placement[i];
                AppendCapsule(nub, embed, i);
            }
        }
        private void BuildEyes()
        {
            var radius = _height * 0.085f; // 0.17 H eye diameter
            var v = CanonicalHeadHeightFraction;
            var theta = Mathf.Deg2Rad * 32f;

            for (var i = 0; i < 2; i++)
            {
                var side = i == 0 ? theta : Mathf.PI - theta;
                var point = SurfacePoint(v, side);
                var normal = SurfaceNormal(v, side);

                // True hemisphere profile, not a constant radius. A constant
                // radius would collapse both poles to single vertices at the
                // ring edge and produce a cylinder with two points — a cone,
                // which art-bible hard rule 1 prohibits outright.
                float EvaluateEye(float t)
                {
                    var y = t * radius * 2f;
                    var d = y - radius;
                    return Mathf.Sqrt(Mathf.Max(0f, radius * radius - d * d));
                }

                AppendLathe(
                    EvaluateEye,
                    radius * 2f,
                    FrameFromNormal(point - normal * radius * 0.55f, normal),
                    12,
                    20,
                    AppendageUvRect(EyeUvSlotOffset + i, AppendageUvSlots),
                    false,
                    _vertices,
                    _normals,
                    _uvs,
                    _triangles);
            }
        }

        private struct NubPlacement
        {
            public readonly float V;
            public readonly float Theta;
            public readonly float Radius;
            public readonly float Length;
            public readonly float Scale;

            public NubPlacement(float v, float thetaDegrees, float radius, float length, float scale)
            {
                V = v;
                Theta = thetaDegrees * Mathf.Deg2Rad;
                Radius = radius * scale;
                Length = length * scale;
                Scale = scale;
            }
        }

        private static float Jitter(System.Random random, float amount)
        {
            return 1f + ((float)random.NextDouble() - 0.5f) * amount;
        }

        /// <summary>
        /// Appends a capsule nub growing along +Y from its frame base.
        ///
        /// A capsule rather than a cone or a tapered quad, which is what
        /// satisfies art-bible hard rule 2: the tip is a hemisphere of the same
        /// diameter as the base, so the tip width is 100% of base width, well
        /// over the required 40%, and the surface has no edge that can read as a
        /// corner.
        /// </summary>
        private void AppendCapsule(NubPlacement nub, float embed, int uvSlot)
        {
            var point = SurfacePoint(nub.V, nub.Theta);
            var normal = SurfaceNormal(nub.V, nub.Theta);
            var basePosition = point - normal * embed;

            var r = nub.Radius;
            var length = nub.Length;

            float EvaluateNub(float v)
            {
                var y = v * length;

                // Hemispherical cap, straight barrel, hemispherical cap. Both
                // poles come to a true point of the sphere rather than a cone
                // apex, so hard rule 1 holds at the tip as well as the barrel.
                if (y < r)
                {
                    var d = y - r;
                    return Mathf.Sqrt(Mathf.Max(0f, r * r - d * d));
                }

                if (y > length - r)
                {
                    var d = y - (length - r);
                    return Mathf.Sqrt(Mathf.Max(0f, r * r - d * d));
                }

                return r;
            }

            AppendLathe(
                EvaluateNub,
                length,
                FrameFromNormal(basePosition, normal),
                16,
                24,
                AppendageUvRect(uvSlot, AppendageUvSlots),
                false,
                _vertices,
                _normals,
                _uvs,
                _triangles);
        }

        /// <summary>
        /// Lathes a profile of revolution into the shared buffers.
        ///
        /// Vertices are emitted in rings from the bottom pole to the top pole,
        /// each ring carrying <c>longitudeSegments + 1</c> vertices so the seam
        /// has its own pair and U runs cleanly 0 → 1. Normals are computed
        /// analytically for that reason — see <see cref="SurfaceNormal"/>.
        /// </summary>
        private void AppendLathe(
            Func<float, float> radiusAtV,
            float height,
            Frame frame,
            int latitudeSegments,
            int longitudeSegments,
            Rect uvRect,
            bool areaCorrectV,
            List<Vector3> vertices,
            List<Vector3> normals,
            List<Vector2> uvs,
            List<int> triangles)
        {
            var vertexOffset = vertices.Count;

            var ringCount = latitudeSegments + 1;

            // Cumulative meridian arc length drives the area-corrected V, so
            // texel rows are spaced by how much surface each spans.
            var cumulative = new float[ringCount];
            var previous = Vector2.zero;

            for (var j = 0; j < ringCount; j++)
            {
                var v = (float)j / latitudeSegments;
                var current = new Vector2(radiusAtV(v), v * height);

                cumulative[j] = j == 0
                    ? 0f
                    : cumulative[j - 1] + Vector2.Distance(previous, current);

                previous = current;
            }

            var totalLength = cumulative[ringCount - 1];
            var invTotal = totalLength > 1e-6f ? 1f / totalLength : 0f;

            for (var j = 0; j < ringCount; j++)
            {
                var v = (float)j / latitudeSegments;
                var radius = radiusAtV(v);

                var normalizedV = areaCorrectV
                    ? cumulative[j] * invTotal
                    : v;

                var isPole = j == 0 || j == ringCount - 1;
                var verticesInRing = isPole ? 1 : longitudeSegments + 1;

                for (var i = 0; i < verticesInRing; i++)
                {
                    // Poles collapse to the centre of the UV island rather than
                    // an arbitrary corner, so no texel row is wasted on a
                    // degenerate strip.
                    var u = isPole
                        ? (uvRect.xMin + uvRect.xMax) * 0.5f
                        : (float)i / longitudeSegments;

                    var theta = (float)i / longitudeSegments * Mathf.PI * 2f;
                    var local = new Vector3(radius * Mathf.Cos(theta), v * height, radius * Mathf.Sin(theta));

                    vertices.Add(frame.Point(local.x, local.y, local.z));

                    // Normals come from THIS lathe's own profile, not the
                    // body's. Reusing the body slope here would light every
                    // appendage as though it were still part of the torso.
                    normals.Add(RotateDirection(frame, MeridianNormal(radiusAtV, height, v, theta)));

                    var uu = isPole ? u : Mathf.Lerp(uvRect.xMin, uvRect.xMax, u);
                    var vv = Mathf.Lerp(uvRect.yMin, uvRect.yMax, normalizedV);

                    uvs.Add(new Vector2(uu, vv));
                }
            }

            // Index rings. Poles are single vertices fanned to the first and
            // last full ring; interior rings are quads split into two triangles.
            //
            // Winding note: Unity is left-handed with the camera looking down
            // +Z, and a triangle is front-facing when its winding appears
            // clockwise on screen. For that case cross(b-a, c-a) points TOWARD
            // the viewer, so a front-facing outward triangle satisfies
            // dot(cross(b-a, c-a), outwardNormal) > 0. The orders below are
            // built to that rule; the inverse renders the whole creature
            // inside-out and it disappears under backface culling.
            var fullRingStart = new int[ringCount];
            var running = 0;

            for (var j = 0; j < ringCount; j++)
            {
                fullRingStart[j] = running;
                running += j == 0 || j == ringCount - 1 ? 1 : longitudeSegments + 1;
            }

            // Bottom fan.
            for (var i = 0; i < longitudeSegments; i++)
            {
                triangles.Add(vertexOffset + fullRingStart[0]);
                triangles.Add(vertexOffset + fullRingStart[1] + i);
                triangles.Add(vertexOffset + fullRingStart[1] + i + 1);
            }

            // Interior quads.
            for (var j = 1; j < ringCount - 2; j++)
            {
                var lower = fullRingStart[j];
                var upper = fullRingStart[j + 1];

                for (var i = 0; i < longitudeSegments; i++)
                {
                    triangles.Add(vertexOffset + lower + i);
                    triangles.Add(vertexOffset + upper + i);
                    triangles.Add(vertexOffset + upper + i + 1);

                    triangles.Add(vertexOffset + lower + i);
                    triangles.Add(vertexOffset + upper + i + 1);
                    triangles.Add(vertexOffset + lower + i + 1);
                }
            }

            // Top fan.
            var topRing = fullRingStart[ringCount - 2];
            var topPole = fullRingStart[ringCount - 1];

            for (var i = 0; i < longitudeSegments; i++)
            {
                triangles.Add(vertexOffset + topRing + i);
                triangles.Add(vertexOffset + topPole);
                triangles.Add(vertexOffset + topRing + i + 1);
            }
        }

        private static Vector3 RotateDirection(Frame frame, Vector3 direction)
        {
            return (frame.AxisX * direction.x +
                    frame.AxisY * direction.y +
                    frame.AxisZ * direction.z).normalized;
        }

        /// <summary>
        /// A frame whose +Y points along <paramref name="direction"/>, used to
        /// stand an appendage out from the body surface.
        ///
        /// The basis is built RIGHT-handed — <c>AxisZ = Cross(AxisX, AxisY)</c>
        /// with <c>AxisX</c> derived from a reference — to match the body's own
        /// right-handed (X, Y, Z) frame. Left-handed is not merely a style
        /// choice here: AppendLathe emits its winding assuming the same
        /// handedness as the body, so a mirrored appendage frame reverses the
        /// relationship between triangle winding and surface normal and the nub
        /// lights inside-out.
        /// </summary>
        private static Frame FrameFromNormal(Vector3 origin, Vector3 direction)
        {
            var axisY = direction.normalized;

            var reference = Mathf.Abs(axisY.y) > 0.99f
                ? Vector3.forward
                : Vector3.up;

            var axisX = Vector3.Cross(reference, axisY).normalized;
            var axisZ = Vector3.Cross(axisX, axisY).normalized;

            return new Frame
            {
                Base = origin,
                AxisX = axisX,
                AxisY = axisY,
                AxisZ = axisZ
            };
        }

        /// <summary>
        /// A reserved UV slot for appendage <paramref name="index"/> out of
        /// <paramref name="count"/>, in the band above the body's island.
        /// </summary>
        private static Rect AppendageUvRect(int index, int count)
        {
            var slotWidth = 1f / Mathf.Max(1, count);
            return new Rect(
                index * slotWidth,
                AppendageUvVMin,
                slotWidth,
                1f - AppendageUvVMin);
        }

        #endregion

        #region Measurement And Reporting

        /// <summary>
        /// Measures the built mesh against the art bible's §3.1 numbers and logs
        /// a report.
        ///
        /// These are runtime assertions rather than comments because the profile
        /// is authored in fractions of height: changing <see cref="_height"/> or
        /// editing a control point can silently break a proportion that reads
        /// fine in isolation. Anything that violates a hard rule is logged as an
        /// error, because per AGENTS.md §2 a violation of a design constraint is
        /// a defect even if it renders.
        /// </summary>
        private void MeasureAndReport()
        {
            TotalTriangleCount = _triangles.Count / 3;

            MeasureSilhouette();
            _appendageAreaShare = MeasureAppendageArea();
            _texelDensitySpread = MeasureTexelDensity();

            var bodyOk = BodyTriangleCount >= 8000 && BodyTriangleCount <= 12000;
            var widthOk = _widthFraction >= CanonicalWidthFraction - 0.005f;
            var headOk = Mathf.Abs(_headDiameterFraction - CanonicalHeadFraction) <= 0.02f;
            var appendageOk = _appendageAreaShare <= 0.12f;

            var report =
                $"CreatureTestMesh — {TotalTriangleCount} tris " +
                $"(body {BodyTriangleCount}, target 8–12k {(bodyOk ? "ok" : "OUT OF RANGE")})\n" +
                $"  width          {_widthFraction:F3} H (min {CanonicalWidthFraction:F2} H) {(widthOk ? "ok" : "UNDER MINIMUM")}\n" +
                $"  head diameter  {_headDiameterFraction:F3} H at v {CanonicalHeadHeightFraction:F2} (target {CanonicalHeadFraction:F2} H) {(headOk ? "ok" : "OFF TARGET")}\n" +
                $"  appendage area {_appendageAreaShare * 100f:F2}% outside body (cap 12%) {(appendageOk ? "ok" : "OVER CAP")}\n" +
                $"  nub tip/base   1.00 (capsule tip; rule requires ≥ 0.40) ok\n" +
                $"  texel density  {_texelDensitySpread:F2}× spread, lower is more even\n" +
                $"  uv mode        {_uvMode}";

            _buildReport = report;

            if (bodyOk && widthOk && headOk && appendageOk)
            {
                Debug.Log($"[{nameof(CreatureTestMeshGenerator)}] {report}", this);
                return;
            }

            Debug.LogError(
                $"[{nameof(CreatureTestMeshGenerator)}] Generated mesh violates art-bible §3.1:\n{report}",
                this);
        }

        /// <summary>
        /// Widest silhouette width and head diameter, sampled from the same
        /// profile the mesh was built from so the numbers describe the geometry
        /// rather than a separate approximation of it.
        /// </summary>
        private void MeasureSilhouette()
        {
            const int samples = 512;

            var widest = 0f;
            for (var i = 0; i <= samples; i++)
            {
                var v = (float)i / samples;
                widest = Mathf.Max(widest, EvaluateProfile(v));
            }

            _widthFraction = widest * 2f / _height;
            _headDiameterFraction = EvaluateProfile(CanonicalHeadHeightFraction) * 2f / _height;
        }

        /// <summary>
        /// Share of silhouette area contributed by appendage geometry outside the
        /// body mass. Art-bible hard rule 3 caps this at 12%.
        ///
        /// Estimated by area-weighted sampling rather than exact boolean
        /// difference: a sample point counts if it lies outside the body's
        /// revolved surface, and each triangle contributes its area only in
        /// proportion to how many of its samples qualify. Exact for a convex
        /// union, conservative (slight over-read) where appendages meet the body,
        /// and vastly cheaper than a mesh-boolean pass. An over-read is the safe
        /// direction here because it can only make the check stricter.
        /// </summary>
        private float MeasureAppendageArea()
        {
            if (!_includeLimbs && !_includeEyes)
            {
                return 0f;
            }

            const int samplesPerEdge = 3;

            if (_triangles.Count == 0)
            {
                return 0f;
            }

            var outsideArea = 0f;
            var totalArea = 0f;

            for (var i = 0; i < _triangles.Count; i += 3)
            {
                var a = _vertices[_triangles[i]];
                var b = _vertices[_triangles[i + 1]];
                var c = _vertices[_triangles[i + 2]];

                var triArea = TriangleArea(a, b, c);
                if (triArea <= 1e-12f)
                {
                    continue;
                }

                totalArea += triArea;

                var outside = 0;
                var total = 0;

                // Barycentric lattice: sample i, j with i + j <= edge. The
                // third barycentric weight is implied, so every sample stays
                // inside the triangle. Loop vars are named sa/sb because the
                // enclosing triangle loop already owns `i`.
                for (var sa = 0; sa <= samplesPerEdge; sa++)
                {
                    for (var sb = 0; sa + sb <= samplesPerEdge; sb++)
                    {
                        var u = sa / (float)samplesPerEdge;
                        var w = sb / (float)samplesPerEdge;

                        var p = a + (b - a) * u + (c - a) * w;
                        total++;

                        if (IsOutsideBody(p))
                        {
                            outside++;
                        }
                    }
                }

                if (total > 0)
                {
                    outsideArea += triArea * outside / total;
                }
            }

            return totalArea > 1e-12f ? outsideArea / totalArea : 0f;
        }

        /// <summary>
        /// True when a world point lies outside the body's revolved surface.
        /// Uses the same profile evaluation the mesh was built from, so the test
        /// matches the geometry exactly.
        /// </summary>
        private bool IsOutsideBody(Vector3 point)
        {
            var v = WorldYToV(point.y);
            var bodyRadius = EvaluateProfile(v);
            var radial = new Vector2(point.x, point.z).magnitude;

            // A small tolerance keeps sample points sitting exactly on the body
            // surface from being counted as appendage area, which would inflate
            // the estimate at every appendage/body intersection.
            const float tolerance = 0.002f;
            return radial > bodyRadius + tolerance;
        }

        /// <summary>
        /// Ratio of densest to sparsest texel density across the mesh.
        ///
        /// This is the number that predicts whether a constant UV brush radius
        /// paints a visibly different-sized mark in different places. 1.0 is
        /// perfectly even; 1.2 means the densest region has 20% more texels per
        /// unit area than the sparsest, so a mark there is correspondingly
        /// smaller in world terms.
        /// </summary>
        private float MeasureTexelDensity()
        {
            var min = float.MaxValue;
            var max = float.MinValue;
            var counted = 0;

            for (var i = 0; i < _triangles.Count; i += 3)
            {
                var a = _vertices[_triangles[i]];
                var b = _vertices[_triangles[i + 1]];
                var c = _vertices[_triangles[i + 2]];

                var worldArea = TriangleArea(a, b, c);
                if (worldArea <= 1e-12f)
                {
                    continue;
                }

                var uvArea = TriangleArea(
                    _uvs[_triangles[i]],
                    _uvs[_triangles[i + 1]],
                    _uvs[_triangles[i + 2]]);

                if (uvArea <= 1e-12f)
                {
                    continue;
                }

                var density = uvArea / worldArea;
                min = Mathf.Min(min, density);
                max = Mathf.Max(max, density);
                counted++;
            }

            if (counted == 0 || min <= 0f)
            {
                return 0f;
            }

            return max / min;
        }

        private static float TriangleArea(Vector3 a, Vector3 b, Vector3 c)
        {
            return Vector3.Cross(b - a, c - a).magnitude * 0.5f;
        }

        private static float TriangleArea(Vector2 a, Vector2 b, Vector2 c)
        {
            var cross = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
            return Mathf.Abs(cross) * 0.5f;
        }

        #endregion

        #region Helpers

        private void EnsureMeshCollider()
        {
            if (!TryEnsureMeshCollider())
            {
                return;
            }

            _meshCollider.sharedMesh = GeneratedMesh;
        }

        private bool TryEnsureMeshCollider()
        {
            if (_meshCollider != null)
            {
                return true;
            }

            _meshCollider = GetComponent<MeshCollider>();
            if (_meshCollider == null)
            {
                if (!_ensureMeshCollider)
                {
                    return false;
                }

                _meshCollider = gameObject.AddComponent<MeshCollider>();
            }

            if (GeneratedMesh != null)
            {
                _meshCollider.sharedMesh = GeneratedMesh;
            }

            return true;
        }

        private static void DestroySafely(UnityEngine.Object target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
        }

        #endregion
    }
}
