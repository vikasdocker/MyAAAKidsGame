using System;
using UnityEngine;
using Dab.Runtime.Creature;
using Dab.Runtime.Input;
using Dab.Runtime.Painting;

namespace Dab.Runtime.FX
{
    /// <summary>
    /// Spawns pooled particle bursts at the point on the creature the child
    /// actually touched.
    ///
    /// Zero-allocation contract
    /// ------------------------
    /// Nothing in this component's per-frame or per-burst path allocates. Every
    /// ParticleSystem, Material, and Mesh is created once in <see cref="Awake"/>
    /// and reused forever. The two costs that normally break this guarantee in
    /// particle code are both avoided explicitly:
    ///
    ///   - Instantiate/Destroy per burst. Replaced by a fixed pool with a
    ///     round-robin cursor. A pool slot that is still busy is reused anyway,
    ///     which truncates the older burst rather than growing the pool, so the
    ///     particle count is bounded no matter how fast a child scrubs.
    ///   - main.startColor / startSize per burst. Setting these goes through the
    ///     managed wrapper and is per-emitter state. They are set once at build
    ///     time on the pool's shared material instead, and per-particle variation
    ///     uses <c>ParticleSystem.EmitParams</c>, which is a struct and is
    ///     therefore free.
    ///
    /// Mobile budget
    /// -------------
    /// The pool is deliberately small and each burst is a handful of particles,
    /// because art-bible 2.1 fixes one lighting rig and warns against spending
    /// frame budget on decoration. Emission rates are low, particles are
    /// short-lived, and the whole system is one extra draw call per material
    /// rather than one per burst. On the lowest tier the spawner can be
    /// disabled outright without affecting gameplay: nothing here is load-bearing
    /// (Pillar 2, Nothing Can Go Wrong).
    ///
    /// Colour is never the only channel
    /// --------------------------------
    /// palette.json critical-pairs C7 and the art-bible greyscale gate both
    /// require effects to survive desaturation. Each burst type therefore
    /// carries shape and motion as well as hue: hearts rise and wobble, the
    /// success flourish blooms on a fixed 12-spoke radial, and ink absorbs into
    /// the surface as a soft blot. A child who cannot separate the hues still
    /// sees all three.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CreatureFXSpawner : MonoBehaviour
    {
        #region Types

        /// <summary>
        /// The three burst families. Deliberately an enum over a string key: the
        /// routing table below is a switch, so a typo is a compile error rather
        /// than a silently dead branch.
        /// </summary>
        public enum BurstKind
        {
            /// <summary>Wet paint absorbing into the surface. Fires while painting.</summary>
            InkSpread = 0,

            /// <summary>Affection. Fires when the child pets the creature.</summary>
            FloatingHeart = 1,

            /// <summary>
            /// Success, on an ability unlock flourish. palette.json MinigameSuccess
            /// fixes this as a 12-spoke radial that inherits the child's paint hue;
            /// it must never carry a fixed colour of its own.
            /// </summary>
            SuccessFlourish = 2
        }

        #endregion

        #region Configuration

        [Header("Scene References")]
        [Tooltip("State machine whose transitions drive the effects. " +
                 "Found on this object if left empty.")]
        [SerializeField]
        private CreatureStateMachine _machine;

        [Tooltip("Mesh provider, used to resolve a touch into a world point. " +
                 "Found on this object if left empty.")]
        [SerializeField]
        private CreatureTestMeshGenerator _meshGenerator;

        [Tooltip("Input source used to find the touch position. " +
                 "Found on this object if left empty.")]
        [SerializeField]
        private FluidTouchInputManager _input;

        [Header("Pooling")]
        [Tooltip("ParticleSystems per burst kind. Sized so a child scrubbing " +
                 "fast cannot exhaust it; exceeding it reuses the oldest slot " +
                 "rather than growing.")]
        [SerializeField, Range(2, 16)]
        private int _poolSizePerKind = 6;

        [Header("Burst Counts")]
        [SerializeField, Range(1, 12)] private int _inkCount = 4;
        [SerializeField, Range(1, 12)] private int _heartCount = 3;

        // No count for SuccessFlourish: the burst is one growing 12-spoke
        // radial sprite, so emitting a fixed single particle is palette.json
        // C7 rather than a configurable knob.

        [Header("Throttling")]
        [Tooltip("Minimum seconds between ink bursts during a continuous stroke. " +
                 "A fast drag emits many strokes per second and an unthrottled " +
                 "response would look like a firehose, not feedback.")]
        [SerializeField, Range(0.02f, 0.5f)]
        private float _inkInterval = 0.08f;

        [Tooltip("Minimum seconds between heart bursts while petting.")]
        [SerializeField, Range(0.1f, 1.5f)]
        private float _heartInterval = 0.45f;

        [Tooltip("Minimum seconds between success flourishes. The flourish is the " +
                 "single most important moment in the 5-minute loop, so it is not " +
                 "throttled tightly and may repeat freely.")]
        [SerializeField, Range(0.1f, 2f)]
        private float _successInterval = 0.6f;

        [Header("Colours (from design/Art/palette.json)")]
        [Tooltip("Heart tint. Fixed at the Attention semantic #FFE3B8. The heart is " +
                 "the child's own affect rendered outward, so it never inherits the " +
                 "paint hue.")]
        [SerializeField] private Color _heartColor = new Color(1f, 0.890196f, 0.721569f, 1f);

        [Header("Inherited Paint Hue")]
        [Tooltip("Hue applied to ink blooms and the success radial. palette.json " +
                 "MinigameSuccess requires the success radial to inherit the child's " +
                 "paint hue and carry no fixed colour of its own, and wet paint is " +
                 "the child's paint too. White until a paint-selection system exists " +
                 "to call SetInheritedPaintHue.")]
        [SerializeField] private Color _paintHue = Color.white;

        [Header("Textures")]
        [Tooltip("Optional overrides for the generated FX sprites. When left empty " +
                 "the spawner falls back to Resources.Load under FX/ (generated by " +
                 "Tools > Dab > Art > Generate FX Textures).")]
        [SerializeField] private Texture2D _inkTexture;
        [SerializeField] private Texture2D _heartTexture;
        [SerializeField] private Texture2D _successTexture;

        [Header("Lifetime")]
        [SerializeField, Range(0.2f, 2f)] private float _inkLifetime = 0.45f;
        [SerializeField, Range(0.3f, 2.5f)] private float _heartLifetime = 1.1f;
        [SerializeField, Range(0.3f, 3f)] private float _successLifetime = 0.9f;

        #endregion

        #region Internal State

        private const int KindCount = 3;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");

        // Fixed-size pools, one flat array per kind. Flat rather than jagged so the
        // whole set is three contiguous allocations made once, and so the cursor
        // maths is a modulo rather than a double indirection.
        private readonly ParticleSystem[] _inkPool = new ParticleSystem[16];
        private readonly ParticleSystem[] _heartPool = new ParticleSystem[16];
        private readonly ParticleSystem[] _successPool = new ParticleSystem[16];

        private int _inkCursor;
        private int _heartCursor;
        private int _successCursor;

        private Transform _poolRoot;
        private Material _inkMaterial;
        private Material _heartMaterial;
        private Material _successMaterial;

        // Unscaled time accumulators. Time.unscaledTime, not Time.time, so a
        // paused game still settles its effects rather than freezing a burst
        // mid-air, which reads as a bug.
        private float _lastInkTime = -99f;
        private float _lastHeartTime = -99f;
        private float _lastSuccessTime = -99f;

        private Camera _camera;
        private bool _subscribed;
        private bool _poolBuilt;

        // Last resolved world point, so a transition-triggered burst has a
        // sensible position even when the finger has already lifted.
        private Vector3 _lastPoint;
        private Vector3 _lastNormal = Vector3.up;
        private bool _hasPoint;

        // Reused raycast target. Physics.Raycast writes into this rather than
        // allocating a fresh out variable per call, which keeps the intent
        // explicit: one hit buffer, no garbage.
        private RaycastHit _hit;

        #endregion

        #region Public State

        /// <summary>Total bursts emitted this session. Diagnostic only.</summary>
        public int TotalBursts { get; private set; }

        /// <summary>
        /// True once the pool is built and the component can emit. False when no
        /// particle shader is available, in which case the component disables
        /// itself rather than throwing every frame.
        /// </summary>
        public bool IsReady => _poolBuilt;

        /// <summary>
        /// Current hue inherited by the paint-tinted bursts (ink and the success
        /// radial). White until a paint-selection system calls
        /// <see cref="SetInheritedPaintHue"/>.
        /// </summary>
        public Color InheritedPaintHue => _paintHue;

        /// <summary>
        /// Sets the hue the ink and success bursts inherit from the child's
        /// current paint. This is not a decoration knob: palette.json
        /// MinigameSuccess requires the success radial to take its colour from
        /// the paint the child chose, and reading that hue is the future paint
        /// system's job. The value is cached so repeated identical sets are free,
        /// and materials are only touched when the hue actually changes.
        /// </summary>
        public void SetInheritedPaintHue(Color hue)
        {
            if (hue == _paintHue)
            {
                return;
            }

            _paintHue = hue;

            if (_inkMaterial != null)
            {
                _inkMaterial.SetColor(BaseColorId, hue);
            }

            if (_successMaterial != null)
            {
                _successMaterial.SetColor(BaseColorId, hue);
            }
        }

        /// <summary>
        /// Emits a burst of a given kind at a world position and direction.
        /// Public so a non-state-machine event, an audio cue, or a playtest tool
        /// can trigger the same effects without duplicating the pooling logic.
        /// </summary>
        public void Emit(BurstKind kind, Vector3 position, Vector3 normal)
        {
            if (!_poolBuilt)
            {
                return;
            }

            var emit = new ParticleSystem.EmitParams
            {
                // applyShapeToPosition is required for the per-emit position to be
                // honoured. Without it Unity uses the system's transform for
                // placement and every particle appears at the pool root.
                applyShapeToPosition = true,
                position = position
            };

            // Counts bursts that actually emitted. The per-kind helpers return
            // false when the round-robin finds no free slot, and counting those
            // anyway would make this a request counter dressed up as a result
            // counter: it would report every burst as accepted while the effect
            // was silently not appearing. That is exactly the failure this
            // number exists to expose.
            var emitted = false;

            switch (kind)
            {
                case BurstKind.InkSpread:
                    emitted = EmitInk(position, normal, emit);
                    break;
                case BurstKind.FloatingHeart:
                    emitted = EmitHeart(position, normal, emit);
                    break;
                case BurstKind.SuccessFlourish:
                    emitted = EmitSuccess(position, normal, emit);
                    break;
            }

            if (emitted)
            {
                TotalBursts++;
            }
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _machine = _machine != null ? _machine : GetComponent<CreatureStateMachine>();
            _meshGenerator = _meshGenerator != null
                ? _meshGenerator
                : GetComponent<CreatureTestMeshGenerator>();
            _input = _input != null ? _input : GetComponent<FluidTouchInputManager>();

            BuildPool();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        /// <summary>
        /// Releases the runtime-created materials and the pool root. The
        /// emitter GameObjects are children of the creature and would go with it,
        /// but the materials are standalone objects and would outlive it.
        /// </summary>
        private void OnDestroy()
        {
            Unsubscribe();

            for (var i = 0; i < _inkPool.Length; i++)
            {
                _inkPool[i] = null;
                _heartPool[i] = null;
                _successPool[i] = null;
            }

            DestroyRuntimeObject(_inkMaterial);
            DestroyRuntimeObject(_heartMaterial);
            DestroyRuntimeObject(_successMaterial);

            _inkMaterial = null;
            _heartMaterial = null;
            _successMaterial = null;

            if (_poolRoot != null)
            {
                DestroyRuntimeObject(_poolRoot.gameObject);
                _poolRoot = null;
            }

            _poolBuilt = false;
        }

        private static void DestroyRuntimeObject(UnityEngine.Object target)
        {
            if (target == null)
            {
                return;
            }

            // DestroyImmediate is illegal during play, and Destroy is deferred
            // and warned about in edit mode. Pick by mode rather than branching on
            // the test, which runs in both.
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(target);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(target);
            }
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Update()
        {
            // A splatter follows the finger while a stroke is live, so it needs
            // the touch position each frame. The other two bursts are transition
            // driven and need no polling at all. One branch per frame for the
            // common case.
            if (_machine != null && _machine.CurrentStateId == CreatureStateId.Painting)
            {
                TryTrackTouch();
                TryEmitThrottled(BurstKind.InkSpread, _inkInterval, ref _lastInkTime);
            }
        }

        #endregion

        #region Pool Construction

        private void BuildPool()
        {
            var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
            {
                Debug.LogWarning(
                    $"[{nameof(CreatureFXSpawner)}] No particle shader found. " +
                    "Effects are disabled; gameplay is unaffected.", this);
                enabled = false;
                return;
            }

            // Children of a dedicated root, so the whole effect set can be
            // toggled or destroyed as one unit and cannot leak into the creature's
            // own transform hierarchy.
            var rootGo = new GameObject("CreatureFX");
            rootGo.hideFlags = HideFlags.HideAndDontSave;
            _poolRoot = rootGo.transform;
            _poolRoot.SetParent(transform, false);

            // One shared material per kind. Sharing is what keeps this to three
            // draw calls instead of one per pooled emitter, and it means the
            // colour is uniform state set once rather than per-emission state
            // set repeatedly.
            //
            // Ink and the success radial share the inherited paint hue; the
            // heart is the fixed Attention colour. Materials are created with the
            // current _paintHue, so a SetInheritedPaintHue call before Awake
            // still colours them here.
            var inkMat = CreateMaterial(shader, _paintHue);
            var heartMat = CreateMaterial(shader, _heartColor);
            var successMat = CreateMaterial(shader, _paintHue);

            AssignTexture(inkMat, LoadTexture("InkBloom", _inkTexture));
            AssignTexture(heartMat, LoadTexture("FloatingHeart", _heartTexture));
            AssignTexture(successMat, LoadTexture("SuccessRadial", _successTexture));

            // Held so OnDestroy can release them. A Material created here is not
            // a child of anything, so destroying the creature does not take it
            // with it, and HideFlags.HideAndDontSave keeps it out of a save
            // rather than out of memory. Every scene reload or playtest restart
            // would otherwise leave three more orphaned materials behind.
            _inkMaterial = inkMat;
            _heartMaterial = heartMat;
            _successMaterial = successMat;

            for (var i = 0; i < _poolSizePerKind && i < _inkPool.Length; i++)
            {
                _inkPool[i] = CreateEmitter("Ink", inkMat, _inkLifetime, 0.05f);
                _heartPool[i] = CreateEmitter("Heart", heartMat, _heartLifetime, 0.075f);
                _successPool[i] = CreateEmitter(
                    "Success", successMat, _successLifetime, 0.05f, grow: true);
            }

            _poolBuilt = true;
        }

        private static Material CreateMaterial(Shader shader, Color color)
        {
            // One material per kind, created once. hideFlags so a playtest scene
            // does not leak these into a save or a build.
            var material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            material.SetColor(BaseColorId, color);
            return material;
        }

        /// <summary>
        /// Binds a generated sprite to a material, preferring the explicitly
        /// assigned editor override and falling back to the asset generated under
        /// Resources/FX by Tools > Dab > Art > Generate FX Textures. A missing
        /// texture degrades to the shader default quad — noisy at worst, never an
        /// error that breaks gameplay.
        /// </summary>
        private static void AssignTexture(Material material, Texture2D texture)
        {
            if (material == null || texture == null || !material.HasProperty(BaseMapId))
            {
                return;
            }

            material.SetTexture(BaseMapId, texture);
        }

        private static Texture2D LoadTexture(string resourceName, Texture2D overrideTexture)
        {
            if (overrideTexture != null)
            {
                return overrideTexture;
            }

            var loaded = Resources.Load<Texture2D>("FX/" + resourceName);
            if (loaded == null)
            {
                Debug.LogWarning(
                    "[CreatureFXSpawner] Texture 'FX/" + resourceName + "' not found; " +
                    "run Tools > Dab > Art > Generate FX Textures. Falling back to the " +
                    "shader default.");
            }

            return loaded;
        }

        private ParticleSystem CreateEmitter(
            string name, Material material, float lifetime, float size, bool grow = false)
        {
            var go = new GameObject("Pool_" + name);
            go.hideFlags = HideFlags.HideAndDontSave;

            // AddComponent starts the system immediately, so it must be stopped
            // before any module is reconfigured. Unity refuses to change
            // duration or loop on a system that is still playing, and doing so
            // logs an assert. This is not cosmetic: a pooled emitter left playing
            // with rateOverTime edits would accumulate particles on its own.
            var ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 1f;
            main.loop = false;
            main.playOnAwake = false;
            main.startLifetime = lifetime;
            main.startSpeed = 0f;
            main.startSize = size;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 24;

            // The success radial blooms: a single particle that grows from a dot
            // to a full wheel over its lifetime, which is what carries the
            // 12-spoke pattern without a wall of overlapping particles.
            if (grow)
            {
                var sizeOverLifetime = ps.sizeOverLifetime;
                sizeOverLifetime.enabled = true;
                sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
                    1f, new AnimationCurve(
                        new Keyframe(0f, 0.05f),
                        new Keyframe(1f, 1.6f)));
            }

            // Emission rate is zero on purpose: particles are added only through
            // Emit, so a pooled emitter never accumulates background particles
            // between bursts.
            var emission = ps.emission;
            emission.rateOverTime = 0f;

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.01f;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = material;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;

            // Lights off. art-bible 2.1 states there is exactly one lighting rig
            // and that states do not author new lights; an emissive particle is
            // a material property, not a light, and adding one would be a
            // per-frame light cost on the lowest tier.
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            go.transform.SetParent(_poolRoot, false);
            return ps;
        }

        #endregion

        #region Event Plumbing

        private void Subscribe()
        {
            if (_subscribed || _machine == null)
            {
                return;
            }

            // Method group rather than a lambda: a lambda has no reference left
            // to remove it with, so the machine would keep invoking a destroyed
            // spawner.
            _machine.StateChanged += HandleStateChanged;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed || _machine == null)
            {
                return;
            }

            _machine.StateChanged -= HandleStateChanged;
            _subscribed = false;
        }

        private void HandleStateChanged(CreatureStateId from, CreatureStateId to)
        {
            // Routed to the position the child last touched. Reading the live
            // touch here instead would be wrong: the transition to Idle is
            // triggered by the finger lifting, so by the time this runs the touch
            // has already ended and the live position is stale or absent.
            if (!_hasPoint)
            {
                return;
            }

            switch (to)
            {
                case CreatureStateId.Petting:
                    // art-bible 2.3: petting does not move the world, but a heart
                    // is the child's own affect rendered outward, not a change to
                    // the environment. Throttled, so a long pet does not fill the
                    // screen with hearts and bury the creature.
                    if (Time.unscaledTime - _lastHeartTime >= _heartInterval)
                    {
                        _lastHeartTime = Time.unscaledTime;
                        Emit(BurstKind.FloatingHeart, _lastPoint, _lastNormal);
                    }

                    break;

                case CreatureStateId.AbilityUnlock:
                    // The flourish is the payoff of the 5-minute loop
                    // (game-concept.md step 3, "Echo"). It gets the largest burst
                    // and the least throttling, because this is the moment the loop
                    // exists to deliver.
                    if (Time.unscaledTime - _lastSuccessTime >= _successInterval)
                    {
                        _lastSuccessTime = Time.unscaledTime;
                        Emit(BurstKind.SuccessFlourish, _lastPoint, _lastNormal);
                    }

                    break;
            }
        }

        #endregion

        #region Touch Tracking

        private void TryTrackTouch()
        {
            if (_input == null || _meshGenerator == null || !_input.IsTouching)
            {
                return;
            }

            if (_camera == null)
            {
                _camera = Camera.main;
            }

            var position = _input.PrimaryPosition;

            if (_meshGenerator.TryScreenToSurface(
                position, _camera, out var point, out var normal, out _))
            {
                _lastPoint = point;
                _lastNormal = normal;
                _hasPoint = true;
            }
        }

        private void TryEmitThrottled(BurstKind kind, float interval, ref float lastTime)
        {
            if (!_hasPoint)
            {
                return;
            }

            var now = Time.unscaledTime;
            if (now - lastTime < interval)
            {
                return;
            }

            lastTime = now;
            Emit(kind, _lastPoint, _lastNormal);
        }

        #endregion

        #region Emission

        private bool EmitInk(
            Vector3 position, Vector3 normal, ParticleSystem.EmitParams emit)
        {
            var ps = NextSlot(_inkPool, ref _inkCursor);
            if (ps == null)
            {
                return false;
            }

            var direction = normal.sqrMagnitude > 1e-6f ? normal : Vector3.up;

            // Wet paint absorbs: a slow, short-lived drift that stays where the
            // stroke was, reading as the surface soaking in paint rather than
            // paint flicking off. art-bible explicitly bans dripping, runs, and
            // splatter, so a soft settling blot is the on-spec behaviour.
            // UnityEngine.Random is qualified explicitly: this file has
            // `using System;` for Action, which makes the bare name ambiguous.
            emit.velocity = direction * 0.45f + UnityEngine.Random.insideUnitSphere * 0.25f;
            emit.startLifetime = _inkLifetime;
            ps.Emit(emit, _inkCount);
            return true;
        }

        private bool EmitHeart(
            Vector3 position, Vector3 normal, ParticleSystem.EmitParams emit)
        {
            var ps = NextSlot(_heartPool, ref _heartCursor);
            if (ps == null)
            {
                return false;
            }

            // Upward drift with a lateral wobble. The wobble is what carries the
            // motion channel for colour-blind legibility: a rising static dot is
            // ambiguous, a rising wobbling one is unambiguously "rising".
            var wobble = new Vector3(
                Mathf.Sin(Time.unscaledTime * 5f) * 0.35f,
                1f,
                Mathf.Cos(Time.unscaledTime * 4f) * 0.25f);

            emit.velocity = wobble * 0.55f;
            emit.startLifetime = _heartLifetime;
            ps.Emit(emit, _heartCount);
            return true;
        }

        private bool EmitSuccess(
            Vector3 position, Vector3 normal, ParticleSystem.EmitParams emit)
        {
            var ps = NextSlot(_successPool, ref _successCursor);
            if (ps == null)
            {
                return false;
            }

            // palette.json MinigameSuccess/C7 specifies a fixed 12-spoke radial
            // for a success burst and calls the pattern itself the carrier. The
            // pattern lives in the generated sprite and in the growth the
            // emitter's sizeOverLifetime curve applies, so one particle per burst
            // is the correct, spec-compliant emission: a single 12-spoke sprite
            // that blooms instead of a cloud of scatter. It carries no fixed
            // colour here — the material inherits the child's paint hue.
            emit.velocity = Vector3.zero;
            emit.startLifetime = _successLifetime;
            ps.Emit(emit, 1);
            return true;
        }

        /// <summary>
        /// Returns the next pool slot, round-robin.
        ///
        /// Round-robin rather than "find a free slot" is a deliberate trade: a
        /// scan would prefer a fully drained emitter, but it also costs a loop
        /// whose length is the pool size on every burst, and it would still have
        /// to fall back to stealing when the pool is saturated. The cursor is
        /// O(1) and the failure mode when saturated is that the oldest burst is
        /// overwritten, which is the correct visual outcome anyway because a
        /// faster-than-recycled stream of bursts is already too dense to read.
        /// </summary>
        private static ParticleSystem NextSlot(ParticleSystem[] pool, ref int cursor)
        {
            if (pool == null || pool.Length == 0)
            {
                return null;
            }

            // Bound the scan to the populated prefix, not pool.Length. The arrays
            // are fixed at MaxPoolSizePerKind so they are never resized, but only
            // _poolSizePerKind entries are built, so cycling on Length would walk
            // into nulls and silently drop every burst after the first few until
            // the cursor wrapped back to zero.
            var count = 0;
            while (count < pool.Length && pool[count] != null)
            {
                count++;
            }

            if (count == 0)
            {
                return null;
            }

            var ps = pool[cursor];
            cursor = (cursor + 1) % count;
            return ps;
        }

        #endregion
    }
}
