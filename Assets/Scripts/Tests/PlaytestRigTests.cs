using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Dab.Runtime.Painting;
using Dab.Runtime.Playtest;

namespace Dab.Tests.Playtest
{
    /// <summary>
    /// Asserts the playtest rig actually builds at runtime.
    ///
    /// This test exists because every wiring mistake fixed in this project so far
    /// compiled cleanly and failed only when the scene booted: a paint controller
    /// enabling itself before it had a renderer, a stale UnityEngine.Touch API
    /// name, a bootstrap left disabled so its Awake never ran. Static checks
    /// cannot see any of that, so the rig is opened and inspected instead.
    /// </summary>
    public sealed class PlaytestRigTests
    {
        private const string ScenePath = "Assets/Scenes/Playtest.unity";

        [UnitySetUp]
        public IEnumerator OpenScene()
        {
            // Additive-free single load: the rig assumes it is the active scene.
            yield return SceneManager.LoadSceneAsync(
                ScenePath, LoadSceneMode.Single);
        }

        [UnityTest]
        public IEnumerator RigBuildsCreatureWithWorkingPaintController()
        {
            yield return null;

            var generator = Object.FindFirstObjectByType<CreatureTestMeshGenerator>();
            Assert.That(generator, Is.Not.Null, "Generator should exist after the bootstrap runs.");
            Assert.That(generator.TotalTriangleCount, Is.GreaterThan(0),
                "Generator should build a non-empty mesh.");

            var controller = Object.FindFirstObjectByType<CreaturePaintController>();
            Assert.That(controller, Is.Not.Null, "Paint controller should exist.");
            Assert.That(controller.isActiveAndEnabled, Is.True,
                "Paint controller must stay enabled. It self-disables in OnEnable " +
                "when it has no target renderer, so this catches a bind-order bug.");
            Assert.That(controller.PaintTexture, Is.Not.Null,
                "Paint texture should be allocated by OnEnable.");

            var renderer = Object.FindFirstObjectByType<MeshRenderer>();
            Assert.That(renderer, Is.Not.Null, "Creature should have a MeshRenderer.");
            Assert.That(renderer.material.shader.name, Is.EqualTo("Dab/Paint/CreatureCanvas"),
                "Renderer must use the Dab canvas shader or paint never shows.");
        }

        [UnityTest]
        public IEnumerator RigProvidesCameraAndLight()
        {
            yield return null;

            Assert.That(Camera.main, Is.Not.Null,
                "Bootstrap should create a MainCamera, otherwise nothing renders.");
            Assert.That(Object.FindFirstObjectByType<Light>(), Is.Not.Null,
                "Bootstrap should create a light, otherwise the creature is unlit.");
        }

        [UnityTest]
        public IEnumerator GeneratorExposesScreenToUvMapping()
        {
            yield return null;

            var generator = Object.FindFirstObjectByType<CreatureTestMeshGenerator>();
            Assert.That(generator, Is.Not.Null);

            // The controller is bound to this mapping. If it threw or returned a
            // delegate that fails on call, painting would break silently at the
            // first touch rather than at boot.
            var screenToUv = generator.CreateScreenToUvDelegate();
            Assert.That(screenToUv, Is.Not.Null, "Generator must expose a screen-to-UV mapping.");

            // A point at the screen centre is over the creature given the bootstrap
            // camera framing, so it should land inside the 0..1 UV square.
            var uv = screenToUv(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
            Assert.That(uv.x, Is.InRange(0f, 1f), "Mapped X should land inside the paint texture.");
            Assert.That(uv.y, Is.InRange(0f, 1f), "Mapped Y should land inside the paint texture.");
        }
    }
}
