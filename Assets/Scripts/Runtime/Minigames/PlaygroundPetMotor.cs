using UnityEngine;
using Dab.Runtime.Creature;
using Dab.Runtime.Input;
using Dab.Runtime.Painting;

namespace Dab.Runtime.Minigames
{
    /// <summary>
    /// The pet's free-run behaviour for the playground scene: tap anywhere on the
    /// ground and the painted pet trots over, then runs through its custom
    /// ability flourish; when the flourish ends it keeps wandering the field on a
    /// deterministic golden-angle path until you call it again.
    ///
    /// Gameplay contract
    /// ----------------
    ///   - Taps that land on the pet itself belong to the painting path (the
    ///     rig's input + <c>CreatureTestMeshGenerator.TryScreenToSurface</c>
    ///     decide), never to this motor. Ground taps set a destination only.
    ///   - The motor never fights the state machine: it only moves while the
    ///     machine is Idle, so a painting drag, a petting hold or a flourish are
    ///     never interrupted.
    ///   - Wandering is deterministic (fixed step sequence, no backend): this rig
    ///     honours the project's no-randomness policy, and the golden-angle walk
    ///     covers the arena without any loot-like odds in the loop.
    ///   - The next wander target is picked after the flourish returns to Idle,
    ///     so the celebration has room to breathe.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlaygroundPetMotor : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Trot speed in world units per second.")]
        [SerializeField] private float _walkSpeed = 2f;

        [Tooltip("Turn rate in degrees per second while rotating toward the goal.")]
        [SerializeField] private float _turnSpeed = 240f;

        [Tooltip("Distance from the goal at which the pet counts as arrived.")]
        [SerializeField] private float _arriveRadius = 1.1f;

        [Tooltip("Vertical bob of the steps while trotting, world units.")]
        [SerializeField] private float _stepBob = 0.06f;

        [Tooltip("Rise of the flourish hop, world units.")]
        [SerializeField] private float _flourishHopHeight = 0.45f;

        [Tooltip("Radius of the deterministic walk around the field centre.")]
        [SerializeField] private float _wanderRadius = 3.6f;

        [Header("Dependencies")]
        [Tooltip("Ground collider. Null falls back to 'any hit that is not the pet'.")]
        [SerializeField] private GameObject _ground;

        private CreatureStateMachine _machine;
        private CreatureTestMeshGenerator _generator;
        private FluidTouchInputManager _input;

        private Vector3 _basePosition;
        private Vector3 _goal;
        private bool _hasGoal;
        private float _rotationAngle;
        private float _stepCycle;
        private float _flourishHopTime = -1f;
        private uint _wanderStep;

        private void Start()
        {
            _machine = GetComponent<CreatureStateMachine>();
            _generator = GetComponent<CreatureTestMeshGenerator>();
            _input = GetComponent<FluidTouchInputManager>();

            _basePosition = new Vector3(transform.position.x, 0f, transform.position.z);

            if (_input == null)
            {
                Debug.LogError(
                    $"[{nameof(PlaygroundPetMotor)}] No FluidTouchInputManager on " +
                    "this GameObject; taps will not set destinations.", this);
            }
            else
            {
                _input.Tapped += HandleTapped;
            }

            if (_machine == null)
            {
                Debug.LogError(
                    $"[{nameof(PlaygroundPetMotor)}] No CreatureStateMachine on " +
                    "this GameObject; the pet cannot flourish on arrival.", this);
            }
            else
            {
                _machine.StateChanged += HandleStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.Tapped -= HandleTapped;
            }

            if (_machine != null)
            {
                _machine.StateChanged -= HandleStateChanged;
            }
        }

        private void HandleTapped(Vector2 screenPosition)
        {
            if (_generator == null)
            {
                return;
            }

            // Taps on the pet go to the painting path; only ground taps drive the
            // motor.
            if (_generator.TryScreenToSurface(screenPosition, out _, out _, out _))
            {
                return;
            }

            var cam = Camera.main;
            if (cam == null)
            {
                return;
            }

            var ray = cam.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, 500f))
            {
                return;
            }

            if (hit.collider == null || hit.collider.transform == null)
            {
                return;
            }

            var isCreature = _generator.transform == hit.collider.transform ||
                             hit.collider.transform.IsChildOf(_generator.transform);
            if (isCreature)
            {
                return;
            }

            if (_ground != null)
            {
                var isGround = hit.collider.transform == _ground.transform ||
                               hit.collider.transform.IsChildOf(_ground.transform);
                if (!isGround)
                {
                    return;
                }
            }

            _goal = new Vector3(hit.point.x, 0f, hit.point.z);
            _hasGoal = true;
        }

        private void HandleStateChanged(CreatureStateId previous, CreatureStateId current)
        {
            if (current == CreatureStateId.AbilityUnlock)
            {
                _flourishHopTime = 0f;
            }

            if (current == CreatureStateId.Idle && previous == CreatureStateId.AbilityUnlock)
            {
                _flourishHopTime = -1f;
                PickNextWanderTarget();
            }
        }

        private void Update()
        {
            if (_machine == null)
            {
                return;
            }

            UpdateFlourishHop();

            // Only move while the pet is free. During Painting, Petting or a
            // flourish the motor holds still no matter where the goal is.
            if (_machine.CurrentStateId != CreatureStateId.Idle)
            {
                return;
            }

            if (_hasGoal)
            {
                MoveTowardGoal();
            }
            else
            {
                StepInPlace();
            }
        }

        private void MoveTowardGoal()
        {
            var offset = _goal - _basePosition;
            var distance = offset.magnitude;
            if (distance <= _arriveRadius)
            {
                ArriveAtGoal();
                return;
            }

            var desiredAngle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
            _rotationAngle = Mathf.MoveTowardsAngle(_rotationAngle, desiredAngle, _turnSpeed * Time.deltaTime);

            var forward = Quaternion.Euler(0f, _rotationAngle, 0f) * Vector3.forward;
            var turnDot = Vector3.Dot(forward, offset.normalized);

            if (turnDot > 0.5f)
            {
                _basePosition += forward * (_walkSpeed * Time.deltaTime);
            }

            _creatureTransform.rotation = Quaternion.Euler(0f, _rotationAngle, 0f);
            _stepCycle += _walkSpeed * Time.deltaTime;
            _creatureTransform.position = new Vector3(
                _basePosition.x,
                Mathf.Abs(Mathf.Sin(_stepCycle * 5f)) * _stepBob,
                _basePosition.z);
        }

        private void ArriveAtGoal()
        {
            _hasGoal = false;
            _basePosition = new Vector3(_goal.x, 0f, _goal.z);

            // The pet runs through its custom ability flourish on arrival. If a
            // gesture had the machine busy, the next arrival retries.
            if (_machine.CurrentStateId == CreatureStateId.Idle)
            {
                _machine.TransitionTo(CreatureStateId.AbilityUnlock);
            }
        }

        private void StepInPlace()
        {
            _stepCycle += Time.deltaTime;
            _creatureTransform.position = new Vector3(
                _basePosition.x,
                Mathf.Abs(Mathf.Sin(_stepCycle * 4f)) * _stepBob * 0.5f,
                _basePosition.z);
        }

        private void UpdateFlourishHop()
        {
            if (_flourishHopTime < 0f)
            {
                return;
            }

            if (_machine.CurrentStateId != CreatureStateId.AbilityUnlock)
            {
                _flourishHopTime = -1f;
                _creatureTransform.localScale = Vector3.one;
                return;
            }

            _flourishHopTime += Time.deltaTime;
            var t = Mathf.Clamp01(_flourishHopTime / 1.1f);
            var y = Mathf.Sin(t * Mathf.PI) * _flourishHopHeight;
            var squash = 1f + Mathf.Sin(t * Mathf.PI * 2f) * 0.04f;
            _creatureTransform.localScale = Vector3.one * squash;

            var position = _creatureTransform.position;
            _creatureTransform.position = new Vector3(position.x, y, position.z);
        }

        private Transform _creatureTransform
        {
            get
            {
                if (_cachedTransform == null)
                {
                    _cachedTransform = transform;
                }

                return _cachedTransform;
            }
        }

        private Transform _cachedTransform;

        /// <summary>
        /// Wires the ground collider reference. Called by the playground bootstrap
        /// after it builds the ground plane; kept internal so no other scene can
        /// re-point the motor at a different surface behind the bootstrap's back.
        /// </summary>
        internal void SetGround(GameObject ground)
        {
            _ground = ground;
        }

        /// <summary>
        /// Deterministic golden-angle walk around the field centre. No randomness
        /// anywhere in this rig: the same seed replays the exact same trail.
        /// </summary>
        private void PickNextWanderTarget()
        {
            const float goldenAngle = 2.399963f;
            var angle = _wanderStep * goldenAngle;
            _wanderStep++;

            _goal = new Vector3(
                Mathf.Cos(angle) * _wanderRadius,
                0f,
                Mathf.Sin(angle) * _wanderRadius);
            _hasGoal = true;
        }
    }
}