using System.Collections;
using Dab.Runtime.Painting;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Dab.Tests.Save
{
    public sealed class PaintControllerRestoreTests
    {
        [UnityTest]
        public IEnumerator RestoreStroke_ReplaysDabsWithoutRaisingSaveEvent()
        {
            var shader = Shader.Find("Dab/Paint/CreatureCanvas");
            Assert.That(shader, Is.Not.Null, "The paint canvas shader must be available.");

            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            gameObject.name = "PaintRestoreTestCreature";
            gameObject.SetActive(false);

            var sourceMaterial = new Material(shader);
            gameObject.GetComponent<MeshRenderer>().sharedMaterial = sourceMaterial;

            var controller = gameObject.AddComponent<CreaturePaintController>();
            controller.TargetRenderer = gameObject.GetComponent<MeshRenderer>();
            gameObject.SetActive(true);
            yield return null;

            var saveEvents = 0;
            controller.StrokeCompleted += _ => saveEvents++;
            var dabsBeforeRestore = controller.DabCount;
            var stroke = new PaintStroke();
            stroke.Samples.Add(new PaintSample(
                new Vector2(0.25f, 0.25f),
                Color.cyan));
            stroke.Samples.Add(new PaintSample(
                new Vector2(0.28f, 0.25f),
                Color.cyan));

            controller.RestoreStroke(stroke);

            Assert.That(controller.DabCount - dabsBeforeRestore, Is.EqualTo(5));
            Assert.That(saveEvents, Is.Zero,
                "Restored data must not recursively append a new save record.");

            Object.Destroy(gameObject);
            Object.Destroy(sourceMaterial);
            yield return null;
        }
    }
}
