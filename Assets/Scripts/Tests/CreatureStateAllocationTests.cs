using System.Collections;
using NUnit.Framework;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.TestTools;
using Dab.Runtime.Creature;

namespace Dab.Tests.Creature
{
    /// <summary>
    /// Verifies the allocation contract the state machine claims in its own
    /// comments: the per-frame path must not allocate, and transitions must not
    /// construct states.
    ///
    /// Why this is measured rather than asserted by inspection
    /// -------------------------------------------------------
    /// The code in the hot path is careful: fixed state table, cached
    /// StringBuilder, no LINQ, no closures on the update path. None of that is
    /// visible from a unit test that only checks behaviour, and every one of
    /// those choices is a single careless edit away from silently regressing.
    /// A per-frame allocation on a low-end Android device is a GC pause, and a
    /// GC pause during the 2.5-second AbilityUnlock flourish is exactly when a
    /// child would notice.
    ///
    /// Honest limits of this measurement
    /// ---------------------------------
    /// This is the weakest of the tests in the suite and it is worth being clear
    /// about why rather than letting a green tick imply more than it does.
    ///
    /// The counter is "GC Allocated In Frame", which counts managed bytes the
    /// whole editor attributes to the frame, not bytes attributable to the state
    /// machine specifically. Editor and test-harness overhead lands in the same
    /// number. So the threshold below is a ceiling chosen to sit well above
    /// ambient noise and well below any realistic regression, not a precise
    /// figure. A closure or a per-frame string format in this path would move it
    /// by hundreds of bytes and fail loudly; a few dozen bytes of incidental
    /// noise would not fail and is not what this is looking for.
    ///
    /// If the profiler counter is unavailable the tests report inconclusive
    /// rather than passing silently. A profiler-disabled machine must not be able
    /// to report a clean bill of health it never actually checked.
    /// </summary>
    public sealed class CreatureStateAllocationTests
    {
        /// <summary>
        /// Bytes per frame above which the idle path is considered leaking.
        /// Chosen between observed editor noise (tens of bytes) and the cost of a
        /// single accidental allocation such as a string format or a closure
        /// (hundreds).
        /// </summary>
        private const long IdleBytesPerFrameCeiling = 512;

        /// <summary>Ceiling per accepted transition. Lower than idle: a transition touches less code.</summary>
        private const long TransitionBytesCeiling = 256;

        private const int WarmupFrames = 30;
        private const int MeasureFrames = 120;

        private GameObject _go;
        private CreatureStateMachine _machine;

        [UnitySetUp]
        public IEnumerator CreateMachine()
        {
            _go = new GameObject("AllocTestCreature");
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

        [UnityTest]
        public IEnumerator IdleUpdatePathDoesNotAllocate()
        {
            var total = 0L;
            var sampled = 0;

            var recorder = ProfilerRecorder.StartNew(
                ProfilerCategory.Memory, "GC Allocated In Frame");

            if (!recorder.Valid)
            {
                recorder.Dispose();
                Assert.Inconclusive(
                    "GC Allocated In Frame counter unavailable; allocation contract unverified.");
                yield break;
            }

            for (var i = 0; i < WarmupFrames; i++)
            {
                yield return null;
            }

            for (var i = 0; i < MeasureFrames; i++)
            {
                yield return null;
                total += recorder.LastValue;
                sampled++;
            }

            recorder.Dispose();

            Assert.Greater(sampled, 0, "No frames were sampled.");
            var perFrame = total / sampled;

            Assert.Less(perFrame, IdleBytesPerFrameCeiling,
                "Idle update path allocated " + perFrame + " bytes/frame; " +
                "the per-frame path must not allocate.");
        }

        [UnityTest]
        public IEnumerator TransitionsDoNotAllocate()
        {
            // Warm up so JIT and editor start-up do not land inside the window.
            for (var i = 0; i < WarmupFrames; i++)
            {
                yield return null;
            }

            var total = 0L;
            const int Iterations = 200;
            const int TransitionsPerIteration = 2;

            // Measured across a fixed number of frames rather than a wall clock,
            // because the counter is per frame and an uncontrolled loop would
            // produce a number that depends on how fast the machine happens to be.
            var recorder = ProfilerRecorder.StartNew(
                ProfilerCategory.Memory, "GC Allocated In Frame");

            if (!recorder.Valid)
            {
                recorder.Dispose();
                Assert.Inconclusive(
                    "GC Allocated In Frame counter unavailable; allocation contract unverified.");
                yield break;
            }

            for (var i = 0; i < Iterations; i++)
            {
                _machine.TransitionTo(CreatureStateId.Petting);
                _machine.TransitionTo(CreatureStateId.Idle);
                yield return null;
                total += recorder.LastValue;
            }

            recorder.Dispose();

            var totalTransitions = Iterations * TransitionsPerIteration;
            var perTransition = total / totalTransitions;

            Assert.Less(perTransition, TransitionBytesCeiling,
                "Transition path allocated " + perTransition + " bytes/transition; " +
                "states must be reused from the table, never constructed.");
        }

        [Test]
        public void TransitionCountTracksAcceptedTransitionsExactly()
        {
            // Structural check on the same reuse contract. If transitions
            // rebuilt state objects, the machine's bookkeeping would drift; the
            // counter matching the request count exactly is a cheap confirmation
            // that every request was accounted for once.
            var start = _machine.TransitionCount;

            for (var i = 0; i < 50; i++)
            {
                _machine.TransitionTo(CreatureStateId.Petting);
                _machine.TransitionTo(CreatureStateId.Idle);
            }

            Assert.AreEqual(start + 100, _machine.TransitionCount,
                "TransitionCount must match the number of accepted transitions.");
        }
    }
}
