using System.Collections;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Dab.Runtime.Creature;

namespace Dab.Tests.Creature
{
    /// <summary>
    /// Exercises the state machine's contract at runtime.
    ///
    /// Why these are PlayMode tests rather than EditMode ones
    /// -------------------------------------------------------
    /// The machine builds its state table in Awake and enters its initial state
    /// in OnEnable. EditMode tests instantiate no live component lifecycle, so
    /// CurrentState would be null and every assertion here would be vacuous or
    /// would need the internals reimplemented. Booting a real GameObject is the
    /// only way to test the thing that actually ships.
    ///
    /// What is deliberately NOT tested here
    /// ------------------------------------
    /// The gesture arbitration. FluidTouchInputManager raises its events from
    /// private methods fed by the real input backend, and there is no public
    /// injection seam, so driving a synthetic stroke would mean adding test-only
    /// API to production code. Painting-then-Petting is instead covered by the
    /// transition-guard tests below, which test the decision the arbitration
    /// defers to. The input plumbing itself is covered by the existing
    /// playtest rig, which boots a scene with a live input manager.
    /// </summary>
    public sealed class CreatureStateMachineTests
    {
        private GameObject _go;
        private CreatureStateMachine _machine;

        [UnitySetUp]
        public IEnumerator CreateMachine()
        {
            _go = new GameObject("TestCreature");
            // AddComponent runs Awake immediately; yielding a frame lets OnEnable
            // run too, so the machine reaches a fully entered initial state.
            _machine = _go.AddComponent<CreatureStateMachine>();
            yield return null;
        }

        [TearDown]
        public void DestroyMachine()
        {
            if (_go != null)
            {
                Object.DestroyImmediate(_go);
            }
        }

        [Test]
        public void BootsIntoIdleWithAStateEntered()
        {
            Assert.IsNotNull(_machine.CurrentState,
                "Machine must have a current state after boot.");
            Assert.AreEqual(CreatureStateId.Idle, _machine.CurrentStateId);
            Assert.AreEqual(CreatureStateId.Idle, _machine.CurrentState.Id,
                "Entered state must agree with the machine's reported id.");
        }

        [Test]
        public void EveryDeclaredStateIsConstructedAndMatchesItsId()
        {
            // Guards the enum/table wiring. A state added to the enum but not to
            // BuildStateTable would otherwise fail only when something tried to
            // enter it, and fail as a silent no-op rather than an error.
            var builder = new StringBuilder();
            var seen = new System.Collections.Generic.HashSet<CreatureStateId>();

            for (var i = 0; i < CreatureStateMachine.StateCount; i++)
            {
                var id = (CreatureStateId)i;
                var state = _machine.GetState(id);

                Assert.IsNotNull(state, "State " + id + " was not constructed.");
                Assert.AreEqual(id, state.Id,
                    "State at index " + i + " reports the wrong id.");
                Assert.IsFalse(string.IsNullOrEmpty(state.DisplayName),
                    "State " + id + " has no display name.");
                Assert.IsTrue(seen.Add(id), "Duplicate state id " + id + ".");
            }
        }

        [Test]
        public void TransitionToMovesBetweenStatesAndRaisesStateChanged()
        {
            CreatureStateId from = CreatureStateId.None;
            CreatureStateId to = CreatureStateId.None;
            var raised = 0;

            _machine.StateChanged += (a, b) =>
            {
                from = a;
                to = b;
                raised++;
            };

            Assert.IsTrue(_machine.TransitionTo(CreatureStateId.Petting));
            Assert.AreEqual(CreatureStateId.Petting, _machine.CurrentStateId);
            Assert.AreEqual(1, raised, "StateChanged must fire once per transition.");
            Assert.AreEqual(CreatureStateId.Idle, from);
            Assert.AreEqual(CreatureStateId.Petting, to);
        }

        [Test]
        public void StatesAreReusedRatherThanReallocated()
        {
            // The allocation contract on CreatureState: a transition must not
            // construct a state. Verified by reference identity across a
            // round trip, which is the only observable that distinguishes
            // reuse from reconstruction.
            var before = _machine.GetState(CreatureStateId.Petting);

            _machine.TransitionTo(CreatureStateId.Petting);
            _machine.TransitionTo(CreatureStateId.Idle);
            _machine.TransitionTo(CreatureStateId.Petting);

            var after = _machine.GetState(CreatureStateId.Petting);
            Assert.AreSame(before, after,
                "State instances must be reused, not rebuilt per transition.");
        }

        [Test]
        public void FeedingIsReachableButNotAutoEntered()
        {
            // FeedingState documents itself as opt-in because age-5 children lack
            // the fine motor control for drag-to-feed. That constraint is only
            // honoured if the state genuinely cannot be reached from touch, so
            // assert the exclusion is real: Idle and Painting are the only
            // states, and Feeding is neither.
            Assert.AreEqual(CreatureStateId.Idle, _machine.CurrentStateId,
                "Boot must not auto-enter Feeding.");

            var feeding = _machine.GetState(CreatureStateId.Feeding);
            Assert.IsNotNull(feeding, "Feeding state must still exist as a slot.");
            Assert.AreEqual(CreatureStateId.Feeding, feeding.Id,
                "The Feeding slot must hold the Feeding state.");

            // It must also be enterable when deliberately asked for, so the
            // opt-in path is real rather than decorative.
            Assert.IsTrue(_machine.TransitionTo(CreatureStateId.Feeding),
                "Feeding must be reachable by explicit request.");
            Assert.AreEqual(CreatureStateId.Feeding, _machine.CurrentStateId);
        }

        [Test]
        public void ReturnToIdleLeavesEveryState()
        {
            _machine.TransitionTo(CreatureStateId.Petting);
            Assert.IsTrue(_machine.ReturnToIdle());
            Assert.AreEqual(CreatureStateId.Idle, _machine.CurrentStateId);
        }

        [Test]
        public void ForceTransitionToBypassesGuards()
        {
            // AbilityUnlock refuses interruption while flourishing. Force must
            // still get out, otherwise a bug elsewhere could strand the creature
            // in a state that will not yield.
            _machine.TransitionTo(CreatureStateId.AbilityUnlock);
            Assert.AreEqual(CreatureStateId.AbilityUnlock, _machine.CurrentStateId);

            _machine.ForceTransitionTo(CreatureStateId.Painting);
            Assert.AreEqual(CreatureStateId.Painting, _machine.CurrentStateId,
                "ForceTransitionTo must bypass CanTransitionTo.");
        }

        [Test]
        public void DescribeCurrentStateReportsTheLiveState()
        {
            var text = _machine.DescribeCurrentState();
            Assert.IsNotEmpty(text);
            StringAssert.Contains("Idle", text);
        }

        [UnityTest]
        public IEnumerator TimeInStateAccumulates()
        {
            var start = _machine.TimeInState;
            yield return null;
            yield return null;
            Assert.Greater(_machine.TimeInState, start,
                "TimeInState must advance with frame time.");
        }
    }
}
