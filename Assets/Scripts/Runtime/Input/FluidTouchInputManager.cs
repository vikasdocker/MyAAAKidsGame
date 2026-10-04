using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

// Alias is load-bearing, not cosmetic. UnityEngine.TouchPhase (legacy Input
// Manager) and UnityEngine.InputSystem.TouchPhase are distinct enums that exist
// simultaneously in 6.3. An unqualified TouchPhase would be ambiguous to a reader
// and easy to bind to the legacy enum by mistake in a new-Input-System-only project.
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Dab.Runtime.Input
{
    /// <summary>
    /// Continuous, forgiving touch input for young players (ages 6-9).
    ///
    /// Design intent
    /// ------------
    /// Children in this band do not have reliable fine motor control, do not read
    /// fluently, and abandon a gesture midway when it stops producing feedback.
    /// This manager is therefore built around three ideas:
    ///
    ///   1. Continuous intent. A drag is reported as a live stroke, not as a
    ///      gesture that must be completed correctly. An incomplete drag still
    ///      yields value, because partial strokes are still strokes.
    ///
    ///   2. Tap and drag are equal-status routes to the same outcome. This is
    ///      NOT an accessibility fallback layered on top of a primary drag path.
    ///      A tap is a first-class input that produces the same result as a
    ///      short drag. This satisfies AGENTS.md constraint 4.
    ///
    ///   3. Generous hit regions and slow-handling. Press radius is inflated well
    ///      beyond a typical minimum touch target, and movement below a small
    ///      threshold is treated as holding rather than dragging, so a resting
    ///      finger does not jitter-spam events.
    ///
    /// Scope note: this class reports raw-but-forgiving input. It does not interpret
    /// game meaning. Consumers decide what a stroke means. See
    /// design/gdd/game-concept.md section 3 for the 30-second loop.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-100)]
    public sealed class FluidTouchInputManager : MonoBehaviour
    {
        #region Configuration

        [Header("Tap / Drag Discrimination")]
        [Tooltip("Seconds a finger may stay down and still count as a tap. " +
                 "Children rest fingers more than adults; keep this generous.")]
        [SerializeField, Range(0.1f, 1.0f)]
        private float _tapMaxDuration = 0.45f;

        [Tooltip("Screen-space distance (inches) below which movement counts as " +
                 "holding rather than dragging. Needs to absorb a shaky hand, " +
                 "not just micro-tremor.")]
        [SerializeField, Range(0.01f, 0.5f)]
        private float _dragDeadZoneInches = 0.12f;

        [Tooltip("How many simultaneous touches to track. More than 3 is " +
                 "unnecessary for this game and costs memory.")]
        [SerializeField, Range(1, 3)]
        private int _maxTrackedTouches = 3;

        [Header("Forgiveness")]
        [Tooltip("Minimum distance (inches) between emitted stroke samples. " +
                 "Prevents flooding consumers with duplicate positions while " +
                 "keeping strokes smooth on a moving finger.")]
        [SerializeField, Range(0.01f, 0.25f)]
        private float _minSampleSpacingInches = 0.03f;

        [Tooltip("Ignore touches that begin closer than this to the screen edge. " +
                 "Edge gestures on a phone are usually system gestures the child " +
                 "did not mean to make.")]
        [SerializeField, Range(0f, 0.25f)]
        private float _edgeInsetInches = 0.05f;

        [Header("Greeting Hook")]
        [Tooltip("Seconds the manager waits for the child before the first " +
                 "unsolicited tap is honoured. Prevents an accidental touch " +
                 "during the intro greeting from being read as a command.")]
        [SerializeField, Range(0f, 3f)]
        private float _gracePeriodSeconds = 0.5f;

        [Header("Mouse (Editor / Desktop)")]
        [Tooltip("EnhancedTouch surfaces real touchscreens only; a mouse pressed " +
                 "in the Unity editor never appears as a touch. When this is on, " +
                 "the manager synthesises the same primary-stroke events from the " +
                 "mouse button so clicking and dragging in the editor behaves " +
                 "exactly like a finger — tap and drag parity is preserved. " +
                 "No-ops when no mouse device is present, so builds on web and " +
                 "mobile are unaffected.")]
        [SerializeField]
        private bool _mouseInputEnabled = true;

        #endregion

        #region Public Events

        /// <summary>A finger made contact. Always fired before any move or end.</summary>
        public event Action<Vector2> TouchBegan;

        /// <summary>
        /// A finger moved meaningfully while down. Fires only when the position
        /// changed past the dead zone and sample spacing.
        /// </summary>
        public event Action<Vector2> TouchMoved;

        /// <summary>
        /// A finger lifted. <paramref name="wasTap"/> is true when the contact
        /// was short and stayed within the dead zone.
        /// </summary>
        public event Action<Vector2, bool> TouchEnded;

        /// <summary>
        /// A tap was recognised. Fired in addition to TouchEnded. Consumers that
        /// only care about "did the child choose this" should subscribe here.
        /// </summary>
        public event Action<Vector2> Tapped;

        /// <summary>A stroke completed long enough to count as a drag.</summary>
        public event Action<List<Vector2>> DragCompleted;

        #endregion

        #region Public State

        /// <summary>True while at least one tracked finger is down.</summary>
        public bool IsTouching => ActiveTouchCount > 0;

        /// <summary>Number of fingers currently down.</summary>
        public int ActiveTouchCount => GetActiveCount();

        /// <summary>True when any active touch has travelled past the dead zone.</summary>
        public bool IsDragging { get; private set; }

        /// <summary>Primary touch position in pixels, or Vector2.zero if none.</summary>
        public Vector2 PrimaryPosition { get; private set; }

        #endregion

        #region Internal State

        // Synthetic touch id for the mouse-bound primary stroke. int.MaxValue can
        // never collide with a real EnhancedTouch touchId, so the mouse press is
        // just another finger in the slot table and inherits every gesture rule
        // (dead zone, sample spacing, tap discrimination, drag completion).
        private const int VirtualMouseTouchId = int.MaxValue;

        private readonly List<Vector2> _strokePoints = new List<Vector2>(64);

        private readonly int[] _trackedTouchIds = new int[3];

        private int _primaryTouchId = -1;

        private float _touchStartTime;
        private Vector2 _touchStartPosition;
        private Vector2 _lastSamplePosition;
        private bool _touchExceedsDeadZone;

        private float _clockTime;
        private float _secondsSinceGraceCleared;
        private bool _enhancedTouchReady;

        private bool _mouseDown;
        private Vector2 _mousePosition;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            for (var i = 0; i < _trackedTouchIds.Length; i++)
            {
                _trackedTouchIds[i] = -1;
            }
        }

        private void OnEnable()
        {
            TryEnableEnhancedTouch();
        }

        private void OnDisable()
        {
            // EnhancedTouchSupport is deliberately NOT disabled here.
            //
            // OnDisable fires on app backgrounding (app switcher, incoming call,
            // Android multi-task) as well as on ordinary disable and scene unload.
            // Disabling mid-gesture discards EnhancedTouch's recorded history for
            // any finger still down, and the re-enable in Update would then
            // happen mid-gesture — which loses that touch entirely. The net
            // effect would be a phantom stroke or a silently dropped tap on
            // resume.
            //
            // EnhancedTouch carries a small fixed cost and is intended to be
            // enabled for the app session. Leaving it on is both safe and cheap.
            //
            // Cancelling tracked touches is still correct on disable, so a
            // re-enabled manager never inherits a stroke from a previous session.
            CancelActiveTouches();
        }

        private void OnApplicationPause(bool paused)
        {
            // A finger held as the app is backgrounded must not resume into a
            // stroke against whatever content is now on screen.
            if (paused)
            {
                CancelActiveTouches();
            }
        }

        private void Update()
        {
            // Accumulating in Update rather than reading Time.time per callback
            // keeps the deadline stable for touches that began before a frame drop.
            _clockTime += Time.unscaledDeltaTime;
            _secondsSinceGraceCleared += Time.unscaledDeltaTime;

            if (!_enhancedTouchReady)
            {
                TryEnableEnhancedTouch();
                if (!_enhancedTouchReady)
                {
                    return;
                }
            }

            PollTouches();
            PollMouse();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Releases every active touch as cancelled and clears stroke state.
        /// Call this when the view changes out from under the child — opening a
        /// panel, or a scene transition — so a finger held across that boundary
        /// cannot produce a phantom stroke against new content.
        /// </summary>
        public void CancelActiveTouches()
        {
            _mouseDown = false;
            _mousePosition = Vector2.zero;

            if (ActiveTouchCount == 0)
            {
                return;
            }

            _primaryTouchId = -1;
            IsDragging = false;
            _touchExceedsDeadZone = false;
            _strokePoints.Clear();

            for (var i = 0; i < _trackedTouchIds.Length; i++)
            {
                _trackedTouchIds[i] = -1;
            }
        }

        /// <summary>
        /// Returns the stroke points recorded for the current primary touch.
        /// Live, not a copy — read it during a stroke callback and do not retain it.
        /// Empty when no primary touch is active.
        /// </summary>
        public IReadOnlyList<Vector2> CurrentStroke => _strokePoints;

        /// <summary>
        /// Resets the no-input grace window. Call this when the child has finished
        /// a greeting sequence and input should be honoured immediately.
        /// </summary>
        public void ClearGracePeriod()
        {
            _secondsSinceGraceCleared = 0f;
        }

        #endregion

        #region Touch Polling

        private void PollTouches()
        {
            var activeTouches = Touch.activeTouches;

            // Ended touches first. Doing this before processing live touches keeps
            // a lift-then-press in the same frame from colliding IDs.
            for (var i = activeTouches.Count - 1; i >= 0; i--)
            {
                var touch = activeTouches[i];
                if (touch.phase == TouchPhase.Ended ||
                    touch.phase == TouchPhase.Canceled)
                {
                    if (IsTracked(touch.touchId))
                    {
                        HandleTouchEnded(touch);
                    }
                }
            }

            for (var i = 0; i < activeTouches.Count; i++)
            {
                var touch = activeTouches[i];
                if (touch.phase == TouchPhase.Began)
                {
                    HandleTouchBegan(touch);
                }
                else if (touch.phase == TouchPhase.Moved ||
                         touch.phase == TouchPhase.Stationary)
                {
                    HandleTouchHeld(touch);
                }
            }

            UpdatePrimaryPosition();
        }

        private void HandleTouchBegan(Touch touch)
        {
            BeginTrackedGesture(touch.touchId, touch.screenPosition);
        }

        private void HandleTouchHeld(Touch touch)
        {
            UpdateTrackedGesture(touch.touchId, touch.screenPosition);
        }

        private void HandleTouchEnded(Touch touch)
        {
            EndTrackedGesture(touch.touchId, touch.screenPosition);
        }

        private void BeginPrimaryStroke(int touchId, Vector2 position)
        {
            _primaryTouchId = touchId;
            _touchStartTime = _clockTime;
            _touchStartPosition = position;
            _lastSamplePosition = position;
            _touchExceedsDeadZone = false;
            IsDragging = false;

            _strokePoints.Clear();
            _strokePoints.Add(position);
        }

        private void EndPrimaryStroke()
        {
            _primaryTouchId = -1;
            _touchExceedsDeadZone = false;
            IsDragging = false;
            _strokePoints.Clear();
        }

        private void UpdatePrimaryPosition()
        {
            if (_primaryTouchId == -1)
            {
                PrimaryPosition = Vector2.zero;
                return;
            }

            if (_primaryTouchId == VirtualMouseTouchId)
            {
                PrimaryPosition = _mousePosition;
                return;
            }

            var activeTouches = Touch.activeTouches;
            for (var i = 0; i < activeTouches.Count; i++)
            {
                if (activeTouches[i].touchId == _primaryTouchId)
                {
                    PrimaryPosition = activeTouches[i].screenPosition;
                    return;
                }
            }

            PrimaryPosition = Vector2.zero;
        }

        #endregion

        #region Gesture Primitives (Touch + Mouse)

        /// <summary>
        /// Shared beginning for a gesture, used by both a real touch and the
        /// synthesized mouse stroke. The mouse is just another finger in the
        /// slot table, so every forgiveness rule (playable area, dead zone,
        /// sample spacing, tap discrimination) applies to it identically.
        /// </summary>
        private void BeginTrackedGesture(int touchId, Vector2 position)
        {
            // The mouse reserves its own slot even when a real finger is already
            // down; only genuine touches fight over the limited touch slots.
            if (ActiveTouchCount >= Mathf.Min(_maxTrackedTouches, _trackedTouchIds.Length) &&
                touchId != VirtualMouseTouchId)
            {
                return;
            }

            if (!IsInsidePlayableArea(position))
            {
                return;
            }

            // Find an empty slot rather than assuming the next index is free.
            // If a previous release left a hole, this fills it without
            // overwriting an existing live ID.
            var slot = -1;
            for (var i = 0; i < _trackedTouchIds.Length; i++)
            {
                if (_trackedTouchIds[i] == -1)
                {
                    slot = i;
                    break;
                }
            }

            if (slot == -1)
            {
                return;
            }

            _trackedTouchIds[slot] = touchId;

            // The first finger of a new interaction owns the stroke. A second
            // finger is tracked for pinch-style reactions but does not steal it.
            if (_primaryTouchId == -1)
            {
                BeginPrimaryStroke(touchId, position);
            }

            TouchBegan?.Invoke(position);
        }

        /// <summary>
        /// Shared held/moved handling for a gesture, used by both a real touch
        /// and the mouse. Inflates travel past the dead zone into a drag and
        /// emits samples past the spacing threshold only for the primary stroke.
        /// </summary>
        private void UpdateTrackedGesture(int touchId, Vector2 position)
        {
            if (!IsTracked(touchId))
            {
                return;
            }

            if (!_touchExceedsDeadZone && HasExceededDeadZone(position))
            {
                _touchExceedsDeadZone = true;
                IsDragging = true;
            }

            if (touchId == _primaryTouchId && _touchExceedsDeadZone)
            {
                if (HasPassedSampleSpacing(position))
                {
                    _lastSamplePosition = position;
                    _strokePoints.Add(position);
                    TouchMoved?.Invoke(position);
                }
            }
        }

        /// <summary>
        /// Shared ending for a gesture, used by both a real touch and the mouse.
        /// Resolves tap-versus-drag, raises the lifecycle events, and hands a
        /// completed stroke to consumers that only want an outcome.
        /// </summary>
        private void EndTrackedGesture(int touchId, Vector2 position)
        {
            ReleaseTrackedId(touchId);

            var duration = _clockTime - _touchStartTime;
            var wasTap = !_touchExceedsDeadZone &&
                         duration <= _tapMaxDuration &&
                         touchId == _primaryTouchId;

            if (touchId == _primaryTouchId)
            {
                if (IsDragging && _strokePoints.Count >= 2)
                {
                    DragCompleted?.Invoke(new List<Vector2>(_strokePoints));
                }

                EndPrimaryStroke();
            }

            TouchEnded?.Invoke(position, wasTap);

            // A tap is a first-class outcome, not a lesser drag. Fired separately
            // so consumers that only need intent can ignore stroke bookkeeping.
            //
            // The gate is time since the grace window was CLEARED, not touch age.
            // Comparing against touch duration here would double-count
            // _tapMaxDuration and silently cancel every tap, because a tap is by
            // definition short. _tapMaxDuration decides what counts as a tap;
            // _gracePeriodSeconds decides when taps start being honoured; they
            // are independent axes and neither should be derived from the other.
            if (wasTap && _secondsSinceGraceCleared >= _gracePeriodSeconds)
            {
                Tapped?.Invoke(position);
            }
        }

        #endregion

        #region Mouse Input

        /// <summary>
        /// Polls the mouse device and forwards presses, holds and releases into
        /// the same gesture pipeline as touch, so editor and desktop play can
        /// drive the full loop (paint, state change, FX) with a mouse.
        ///
        /// EnhancedTouch does not observe the mouse, which is why this path
        /// exists at all. The three handlers double as testable entry points: a
        /// PlayMode test can feed a stroke without any physical device.
        /// </summary>
        private void PollMouse()
        {
            if (!_mouseInputEnabled)
            {
                return;
            }

            var mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            var position = mouse.position.ReadValue();

            if (mouse.leftButton.wasPressedThisFrame)
            {
                HandleMousePress(position);
            }
            else if (mouse.leftButton.isPressed)
            {
                HandleMouseHold(position);
            }

            if (mouse.leftButton.wasReleasedThisFrame)
            {
                HandleMouseRelease(position);
            }
        }

        /// <summary>
        /// Begins a primary stroke from a mouse press. Public so tests and
        /// non-device input streams can drive the exact path a real click takes.
        /// </summary>
        public void HandleMousePress(Vector2 position)
        {
            if (!_mouseInputEnabled || _mouseDown)
            {
                return;
            }

            _mouseDown = true;
            _mousePosition = position;
            BeginTrackedGesture(VirtualMouseTouchId, position);
        }

        /// <summary>
        /// Continues a mouse stroke (or holds it still, mirroring a resting
        /// finger). Public for the same reason as <see cref="HandleMousePress"/>.
        /// </summary>
        public void HandleMouseHold(Vector2 position)
        {
            if (!_mouseDown || !_mouseInputEnabled)
            {
                return;
            }

            _mousePosition = position;
            UpdateTrackedGesture(VirtualMouseTouchId, position);
        }

        /// <summary>
        /// Lifts the mouse stroke. Public for the same reason as
        /// <see cref="HandleMousePress"/>.
        /// </summary>
        public void HandleMouseRelease(Vector2 position)
        {
            if (!_mouseDown || !_mouseInputEnabled)
            {
                return;
            }

            _mouseDown = false;
            _mousePosition = Vector2.zero;
            EndTrackedGesture(VirtualMouseTouchId, position);
        }

        #endregion

        #region Forgiveness Helpers

        private bool HasExceededDeadZone(Vector2 position)
        {
            return InchesBetween(position, _touchStartPosition) > _dragDeadZoneInches;
        }

        private bool HasPassedSampleSpacing(Vector2 position)
        {
            return InchesBetween(position, _lastSamplePosition) >= _minSampleSpacingInches;
        }

        /// <summary>
        /// Screen-space distance in inches. Inches rather than pixels because DPI
        /// varies enormously across the devices this game targets — a pixel
        /// threshold that forgives enough on a flagship will do nothing on a
        /// cheap tablet, and the audience skews toward exactly the cheap end.
        /// </summary>
        private static float InchesBetween(Vector2 a, Vector2 b)
        {
            return (a - b).magnitude / Mathf.Max(1f, Screen.dpi);
        }

        private bool IsInsidePlayableArea(Vector2 position)
        {
            var insetPixels = _edgeInsetInches * Mathf.Max(1f, Screen.dpi);

            return position.x >= insetPixels &&
                   position.y >= insetPixels &&
                   position.x <= Screen.width - insetPixels &&
                   position.y <= Screen.height - insetPixels;
        }

        private bool IsTracked(int touchId)
        {
            for (var i = 0; i < _trackedTouchIds.Length; i++)
            {
                if (_trackedTouchIds[i] == touchId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Number of live tracked IDs, derived from the array rather than kept as
        /// a separate counter. A counter and the array can disagree the moment a
        /// slot is released out of order, and the disagreement shows up as either
        /// a phantom finger or a dropped gesture.
        /// </summary>
        private int GetActiveCount()
        {
            var count = 0;
            for (var i = 0; i < _trackedTouchIds.Length; i++)
            {
                if (_trackedTouchIds[i] != -1) count++;
            }
            return count;
        }

        private void ReleaseTrackedId(int touchId)
        {
            for (var i = 0; i < _trackedTouchIds.Length; i++)
            {
                if (_trackedTouchIds[i] == touchId)
                {
                    _trackedTouchIds[i] = -1;
                    return;
                }
            }
        }

        #endregion

        #region Enhanced Touch Setup

        /// <summary>
        /// Enhanced touch must be enabled before the first gesture is observed,
        /// because it records touch history as touches occur. Enabling it in
        /// response to a touch loses that touch. This attempts at OnEnable and
        /// retries in Update so a device that enumerates late still recovers.
        /// </summary>
        private void TryEnableEnhancedTouch()
        {
            if (_enhancedTouchReady)
            {
                return;
            }

            EnhancedTouchSupport.Enable();
            _enhancedTouchReady = true;
        }

        #endregion
    }
}