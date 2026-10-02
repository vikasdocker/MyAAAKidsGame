using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Dab.Runtime.Input;

namespace Dab.Runtime.Painting
{
    /// <summary>
    /// Composites brush dabs into the creature's paint texture.
    ///
    /// Prototype scope — deliberately not the shipping architecture
    /// ---------------------------------------------------------
    /// This is the "does the mechanic feel good" build referenced by
    /// <c>design/gdd/game-concept.md</c> section 9, where freehand 3D painting is
    /// recorded as the project's highest technical risk. It answers that question
    /// in days rather than weeks, and deliberately avoids Render Graph entirely.
    ///
    /// A production version would stamp via a URP Render Graph raster pass so the
    /// dabs composite inside the render pipeline. That is ADR-0002 and it should
    /// not be written until this prototype proves the mechanic is worth the
    /// machinery. See the ADR's Alternatives section for why the simple path is
    /// defensible rather than merely expedient.
    ///
    /// What this prototype DOES establish:
    ///   - whether a child's finger feels good laying colour onto a curved surface
    ///   - whether the additive stamp avoids seams at stroke joins
    ///   - whether brush radius in UV space survives a creature's varying density
    ///   - whether the paint texture is legible at the 1024 default
    ///
    /// Design constraints honoured (AGENTS.md section 2):
    ///   - No precision is required. Dabs interpolate between the previous and
    ///     current sample, so a slow drag is still continuous and a fast one
    ///     still deposits a connected line.
    ///   - Nothing can be erased. Alpha only accumulates, so a child's work can
    ///     never be undone by them or by a stray tap.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CreaturePaintController : MonoBehaviour
    {
        #region Configuration

        [Header("Paint Texture")]
        [Tooltip("Resolution of the paint texture. 1024 is the prototype default; " +
                 "the production value is an open performance question.")]
        [SerializeField, Range(256, 2048)]
        private int _paintTextureSize = 1024;

        [Tooltip("Brush radius as a fraction of the texture. Small values keep " +
                 "detail; large values are more forgiving for young fingers.")]
        [SerializeField, Range(0.005f, 0.15f)]
        private float _brushRadiusUv = 0.045f;

        [Tooltip("0 = fully soft falloff, 1 = hard-edged dot. Soft is more " +
                 "forgiving and better suits crayon-like painting.")]
        [SerializeField, Range(0f, 0.9f)]
        private float _brushHardness = 0.25f;

        [Tooltip("Maximum UV distance between two interpolated dabs. Prevents a " +
                 "fast flick from leaving a dotted line.")]
        [SerializeField, Range(0.002f, 0.05f)]
        private float _maxDabSpacingUv = 0.008f;

        [Header("Surface")]
        [Tooltip("Renderer receiving the paint texture. Assigned to the creature " +
                 "mesh this controller paints onto.")]
        [SerializeField]
        private Renderer _targetRenderer;

        [Tooltip("Property name on the target material holding the paint texture. " +
                 "Must match CreatureCanvas.shader.")]
        [SerializeField]
        private string _paintTextureProperty = "_PaintMap";

        // Handlers are stored rather than passed as inline lambdas so
        // UnbindFromInput can actually detach them. A lambda passed directly to
        // += has no reference left to remove it with, which leaks this
        // controller into the input manager's invocation list for the lifetime
        // of the scene: a destroyed controller would still be invoked.
        private Action<Vector2> _onTouchBegan;
        private Action<Vector2> _onTouchMoved;
        private Action<Vector2, bool> _onTouchEnded;
        private FluidTouchInputManager _boundInput;

        #endregion

        #region Public State

        /// <summary>
        /// The accumulated paint texture. Assign to a material's
        /// <c>_PaintMap</c> slot, or leave to this controller to bind it.
        /// </summary>
        public RenderTexture PaintTexture => _paintTexture;

        /// <summary>
        /// The renderer this controller paints onto. Must be set before the
        /// component is enabled, because OnEnable allocates the paint texture and
        /// binds it to this renderer's material; without a renderer there is
        /// nothing to bind and the controller disables itself.
        /// </summary>
        public Renderer TargetRenderer
        {
            get => _targetRenderer;
            set => _targetRenderer = value;
        }

        /// <summary>Total dabs composited this session. Diagnostic only.</summary>
        public int DabCount { get; private set; }

        /// <summary>
        /// Raised after each dab. Carries the UV position so a future audio layer
        /// can play a stroke sound at the right pitch. Not used yet.
        /// </summary>
        public event Action<Vector2> DabComposited;

        #endregion

        #region Internal State

        private static readonly int BrushUvId = Shader.PropertyToID("_BrushUV");
        private static readonly int BrushColorId = Shader.PropertyToID("_BrushColor");
        private static readonly int BrushRadiusId = Shader.PropertyToID("_BrushRadius");
        private static readonly int BrushHardnessId = Shader.PropertyToID("_BrushHardness");

        private RenderTexture _paintTexture;
        private Material _stampMaterial;
        private Material _canvasMaterialInstance;
        private Mesh _stampMesh;
        private CommandBuffer _commandBuffer;

        private bool _hasLastSample;
        private Vector2 _lastSampleUv;
        private readonly List<Vector2> _pendingDabs = new List<Vector2>(32);

        private int _paintPropertyId;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            if (_targetRenderer == null)
            {
                Debug.LogError(
                    $"[{nameof(CreaturePaintController)}] No target renderer assigned. " +
                    "Assign the creature mesh this paints onto.", this);
                enabled = false;
                return;
            }

            _paintPropertyId = Shader.PropertyToID(_paintTextureProperty);

            CreatePaintTexture();
            CreateStampResources();
            BindToMaterial();
        }

        private void OnDisable()
        {
            // Deliberately not unbound here. A disabled controller still has its
            // input subscription, so re-enabling it resumes painting without the
            // caller having to rebind. Only destruction is terminal, so that is
            // the only point where the subscription must be broken.
            ReleaseResources();
        }

        private void OnDestroy()
        {
            UnbindFromInput();
            ReleaseResources();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Starts a stroke. Call on <c>TouchBegan</c>. Subsequent
        /// <see cref="PaintAt"/> calls interpolate from the last sample, so the
        /// child does not have to produce a dense stream of samples for a
        /// continuous line to appear.
        /// </summary>
        public void BeginStroke(Vector2 uv, Color color)
        {
            _hasLastSample = true;
            _lastSampleUv = uv;
            _pendingDabs.Clear();

            SetBrushColor(color);
            StampDab(uv);

            DabCount++;
            DabComposited?.Invoke(uv);
        }

        /// <summary>
        /// Extends the stroke to a new UV position, interpolating dabs across the
        /// gap so a fast flick still deposits a connected line rather than dots.
        /// Safe to call with any sample rate.
        /// </summary>
        public void PaintAt(Vector2 uv, Color color)
        {
            if (!_hasLastSample)
            {
                BeginStroke(uv, color);
                return;
            }

            SetBrushColor(color);

            var delta = uv - _lastSampleUv;
            var distance = delta.magnitude;

            if (distance < _maxDabSpacingUv)
            {
                return;
            }

            var steps = Mathf.Clamp(
                Mathf.CeilToInt(distance / _maxDabSpacingUv), 1, 64);

            for (var i = 1; i <= steps; i++)
            {
                var interpolated = _lastSampleUv + delta * (i / (float)steps);
                StampDab(interpolated);
                DabCount++;
            }

            _lastSampleUv = uv;
            DabComposited?.Invoke(uv);
        }

        /// <summary>
        /// Ends the stroke. Call on <c>TouchEnded</c> / <c>DragCompleted</c>.
        /// There is intentionally no clear, undo, or reset — a child cannot lose
        /// their work (Pillar 2, Nothing Can Go Wrong).
        /// </summary>
        public void EndStroke()
        {
            _hasLastSample = false;
            _lastSampleUv = Vector2.zero;
        }

        /// <summary>
        /// Subscribes to a <see cref="FluidTouchInputManager"/> and paints any
        /// touch that lands on this controller. Kept separate from the touch
        /// plumbing so the input layer stays testable in isolation.
        /// </summary>
        public void BindToInput(FluidTouchInputManager input, Func<Vector2, Vector2> screenToUv)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (screenToUv == null)
            {
                throw new ArgumentNullException(nameof(screenToUv));
            }

            // Rebinding without unbinding first would double-paint every touch.
            UnbindFromInput();

            _boundInput = input;
            _onTouchBegan = position => BeginStroke(screenToUv(position), Color.white);
            _onTouchMoved = position => PaintAt(screenToUv(position), Color.white);
            _onTouchEnded = (position, wasTap) => EndStroke();

            input.TouchBegan += _onTouchBegan;
            input.TouchMoved += _onTouchMoved;
            input.TouchEnded += _onTouchEnded;
        }

        /// <summary>
        /// Detaches from the input manager. Called automatically on destroy, and
        /// by <see cref="BindToInput"/> before it rebinds, so callers rarely need
        /// this directly. Not called on disable: a re-enabled controller should
        /// resume painting without needing to rebind.
        /// </summary>
        public void UnbindFromInput()
        {
            if (_boundInput == null)
            {
                return;
            }

            _boundInput.TouchBegan -= _onTouchBegan;
            _boundInput.TouchMoved -= _onTouchMoved;
            _boundInput.TouchEnded -= _onTouchEnded;

            _boundInput = null;
            _onTouchBegan = null;
            _onTouchMoved = null;
            _onTouchEnded = null;
        }

        #endregion

        #region Texture And Resources

        private void CreatePaintTexture()
        {
            var descriptor = new RenderTextureDescriptor(
                _paintTextureSize,
                _paintTextureSize,
                RenderTextureFormat.ARGB32,
                0)
            {
                // Start fully transparent: alpha 0 means unpainted, so the
                // creature's base appearance shows through until the child
                // draws. RenderTextureDescriptor has no clearBuffer flag, so the
                // clear is issued explicitly below.
                msaaSamples = 1,
                useMipMap = false,
                autoGenerateMips = false,
                sRGB = true
            };

            _paintTexture = new RenderTexture(descriptor)
            {
                name = $"CreaturePaint_{_paintTextureSize}",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            _paintTexture.Create();

            var clear = new CommandBuffer { name = "ClearPaintTexture" };
            clear.SetRenderTarget(_paintTexture);
            clear.ClearRenderTarget(true, true, Color.clear);
            Graphics.ExecuteCommandBuffer(clear);
            clear.Release();
        }

        private void CreateStampResources()
        {
            var shader = Shader.Find("Dab/Paint/PaintStamp");
            if (shader == null)
            {
                Debug.LogError(
                    $"[{nameof(CreaturePaintController)}] Shader 'Dab/Paint/PaintStamp' " +
                    "not found. Is it in Assets/Shaders/Paint/?", this);
                enabled = false;
                return;
            }

            _stampMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };

            // Fullscreen triangle in UV space. Three vertices rather than a quad
            // avoids the diagonal seam a two-triangle quad can produce on some
            // mobile rasterisers.
            _stampMesh = new Mesh { name = "PaintStampQuad", hideFlags = HideFlags.HideAndDontSave };
            _stampMesh.vertices = new[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(1f, 0f, 0f),
                new Vector3(0f, 1f, 0f),
                new Vector3(1f, 1f, 0f)
            };
            _stampMesh.uv = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f)
            };
            _stampMesh.triangles = new[] { 0, 1, 2, 2, 1, 3 };
            _stampMesh.UploadMeshData(true);

            _commandBuffer = new CommandBuffer { name = "CreaturePaint" };
        }

        private void BindToMaterial()
        {
            // Material instance rather than mutating the shared asset — the
            // creature mesh may be an addressable or prefab-instanced material.
            _canvasMaterialInstance = _targetRenderer.material;
            _canvasMaterialInstance.SetTexture(_paintPropertyId, _paintTexture);
        }

        private void ReleaseResources()
        {
            _hasLastSample = false;

            if (_commandBuffer != null)
            {
                _commandBuffer.Release();
                _commandBuffer = null;
            }

            if (_stampMaterial != null)
            {
                DestroySafely(_stampMaterial);
                _stampMaterial = null;
            }

            if (_stampMesh != null)
            {
                DestroySafely(_stampMesh);
                _stampMesh = null;
            }

            if (_canvasMaterialInstance != null)
            {
                DestroySafely(_canvasMaterialInstance);
                _canvasMaterialInstance = null;
            }

            if (_paintTexture != null)
            {
                _paintTexture.Release();
                DestroySafely(_paintTexture);
                _paintTexture = null;
            }
        }

        #endregion

        #region Compositing

        private void SetBrushColor(Color color)
        {
            _stampMaterial.SetColor(BrushColorId, color);
            _stampMaterial.SetFloat(BrushRadiusId, _brushRadiusUv);
            _stampMaterial.SetFloat(BrushHardnessId, _brushHardness);
        }

        private void StampDab(Vector2 uv)
        {
            _stampMaterial.SetVector(BrushUvId, new Vector4(uv.x, uv.y, 0f, 0f));

            _commandBuffer.Clear();
            _commandBuffer.SetRenderTarget(_paintTexture);
            _commandBuffer.DrawMesh(_stampMesh, Matrix4x4.identity, _stampMaterial, 0, 0);
            Graphics.ExecuteCommandBuffer(_commandBuffer);
        }

        #endregion

        #region Helpers

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