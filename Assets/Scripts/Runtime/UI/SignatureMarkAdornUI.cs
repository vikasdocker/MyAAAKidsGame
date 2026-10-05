using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Dab.Runtime.Abilities;

namespace Dab.Runtime.UI
{
    [DisallowMultipleComponent]
    public sealed class SignatureMarkAdornUI : MonoBehaviour
    {
        private SignatureMarkPlacementController _placement;
        private Button _button;
        private Text _glyph;
        private Text _caption;
        private Image _background;

        public static SignatureMarkAdornUI Build(
            SignatureMarkPlacementController placement)
        {
            if (placement == null)
            {
                Debug.LogError(
                    $"[{nameof(SignatureMarkAdornUI)}] A placement controller is required.");
                return null;
            }

            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var canvasObject = new GameObject("Adorn Canvas");
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                var scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080f, 1920f);
                scaler.matchWidthOrHeight = 0.5f;
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("Adorn EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<InputSystemUIInputModule>();
            }

            var button = UxFactory.CreateIconButton(
                canvas.transform,
                "\u2736",
                "adorn",
                240f,
                190f,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 155f),
                Palette.SurfaceRaised,
                Palette.TextPrimary,
                placement.TogglePlacement);
            button.name = "AdornButton";

            var ui = button.gameObject.AddComponent<SignatureMarkAdornUI>();
            ui.Initialize(placement, button);
            return ui;
        }

        private void OnDestroy()
        {
            if (_placement != null)
            {
                _placement.PlacementStateChanged -= Refresh;
            }
        }

        private void Initialize(
            SignatureMarkPlacementController placement,
            Button button)
        {
            _placement = placement;
            _button = button;
            var labels = button.GetComponentsInChildren<Text>(true);
            if (labels.Length >= 2)
            {
                _glyph = labels[0];
                _caption = labels[1];
            }

            _background = button.GetComponent<Image>();
            _placement.PlacementStateChanged += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            var bound = _placement.IsMarkBound;
            var placing = _placement.IsAwaitingPlacement;
            _button.interactable = !bound;

            if (_glyph != null)
            {
                _glyph.text = bound ? "\u2713" : "\u2736";
            }

            if (_caption != null)
            {
                _caption.text = bound ? "made" : placing ? "place" : "adorn";
            }

            if (_background != null)
            {
                _background.color = placing ? Palette.Active : Palette.SurfaceRaised;
            }
        }
    }
}
