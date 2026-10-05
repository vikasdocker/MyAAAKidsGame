using System;
using UnityEngine;
using Dab.Runtime.Input;
using Dab.Runtime.Painting;
using Dab.Runtime.UI;

namespace Dab.Runtime.Abilities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CreatureSignatureMarks))]
    [RequireComponent(typeof(CreatureTestMeshGenerator))]
    [RequireComponent(typeof(CreaturePaintController))]
    [RequireComponent(typeof(FluidTouchInputManager))]
    public sealed class SignatureMarkPlacementController : MonoBehaviour
    {
        private const int SpiralSamples = 32;
        private const float SpiralTurns = 1.25f;
        private const float SpiralRadiusUv = 0.065f;
        private const float EdgeInsetUv = 0.01f;

        private CreatureSignatureMarks _marks;
        private CreatureTestMeshGenerator _generator;
        private CreaturePaintController _paint;
        private FluidTouchInputManager _input;
        private bool _isAwaitingPlacement;
        private bool _placementTouchActive;

        public event Action PlacementStateChanged;

        public bool IsAwaitingPlacement => _isAwaitingPlacement;
        public bool IsMarkBound => _marks != null && _marks.IsBound(SignatureMarkId.HornSwirl);

        private void Awake()
        {
            _marks = GetComponent<CreatureSignatureMarks>();
            _generator = GetComponent<CreatureTestMeshGenerator>();
            _paint = GetComponent<CreaturePaintController>();
            _input = GetComponent<FluidTouchInputManager>();

            _marks.StateChanged += HandleMarksChanged;
            _input.TouchBegan += HandleTouchBegan;
            _input.TouchEnded += HandleTouchEnded;
        }

        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.TouchBegan -= HandleTouchBegan;
                _input.TouchEnded -= HandleTouchEnded;
            }

            if (_marks != null)
            {
                _marks.StateChanged -= HandleMarksChanged;
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused && _placementTouchActive)
            {
                _placementTouchActive = false;
                _paint.SetPaintingInputEnabled(true);
            }
        }

        public void TogglePlacement()
        {
            if (IsMarkBound)
            {
                return;
            }

            _isAwaitingPlacement = !_isAwaitingPlacement;
            _paint.SetPaintingInputEnabled(!_isAwaitingPlacement);
            PlacementStateChanged?.Invoke();
        }

        private void HandleTouchBegan(Vector2 screenPosition)
        {
            if (!_isAwaitingPlacement ||
                !_generator.TryScreenToUv(screenPosition, out var uv))
            {
                return;
            }

            DrawHornSwirl(uv);

            var result = _marks.BindMark(SignatureMarkId.HornSwirl);
            if (result != SignatureMarkBindResult.Bound)
            {
                Debug.LogError(
                    $"[{nameof(SignatureMarkPlacementController)}] A surface mark " +
                    $"was drawn but could not be bound ({result}).", this);
                return;
            }

            _isAwaitingPlacement = false;
            _placementTouchActive = true;
            PlacementStateChanged?.Invoke();
        }

        private void HandleTouchEnded(Vector2 screenPosition, bool wasTap)
        {
            if (!_placementTouchActive || _input.IsTouching)
            {
                return;
            }

            _placementTouchActive = false;
            _paint.SetPaintingInputEnabled(true);
        }

        private void HandleMarksChanged()
        {
            PlacementStateChanged?.Invoke();
        }

        private void DrawHornSwirl(Vector2 center)
        {
            var radius = Mathf.Min(
                SpiralRadiusUv,
                center.x - EdgeInsetUv,
                1f - EdgeInsetUv - center.x,
                center.y - EdgeInsetUv,
                1f - EdgeInsetUv - center.y);
            radius = Mathf.Max(0f, radius);

            var start = SpiralPoint(center, radius, 0f);
            _paint.BeginStroke(start, Palette.Stroke);

            for (var i = 1; i <= SpiralSamples; i++)
            {
                var progress = i / (float)SpiralSamples;
                _paint.PaintAt(
                    SpiralPoint(center, radius, progress),
                    Palette.Stroke);
            }

            _paint.EndStroke();
        }

        private static Vector2 SpiralPoint(Vector2 center, float radius, float progress)
        {
            var angle = progress * SpiralTurns * Mathf.PI * 2f;
            var distance = radius * (0.12f + progress * 0.88f);
            return new Vector2(
                center.x + Mathf.Cos(angle) * distance,
                center.y + Mathf.Sin(angle) * distance);
        }
    }
}
