using System;
using System.IO;
using Dab.Runtime.Painting;
using Dab.Runtime.Save;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Dab.Tests.Save
{
    public sealed class PaintSaveStoreTests
    {
        private string _directory;

        [SetUp]
        public void SetUp()
        {
            _directory = Path.Combine(
                Application.temporaryCachePath,
                "DabPaintSaveTests_" + Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        [Test]
        public void Load_WhenJournalDoesNotExist_ReturnsNoStrokes()
        {
            var strokes = new PaintSaveStore(_directory).Load();

            Assert.That(strokes, Is.Empty);
        }

        [Test]
        public void AppendAndLoad_PreservesStrokeOrderSamplesAndColors()
        {
            var store = new PaintSaveStore(_directory);
            store.Append(CreateStroke(0.1f, Color.cyan));
            store.Append(CreateStroke(0.7f, new Color(0.3f, 0.5f, 0.8f, 1f)));
            store.Flush();

            var strokes = new PaintSaveStore(_directory).Load();

            Assert.That(strokes, Has.Count.EqualTo(2));
            Assert.That(strokes[0].Samples, Has.Count.EqualTo(1));
            Assert.That(strokes[0].Samples[0].Uv.x, Is.EqualTo(0.1f).Within(0.0001f));
            Assert.That(strokes[0].Samples[0].Color, Is.EqualTo(Color.cyan));
            Assert.That(strokes[1].Samples[0].Uv.x, Is.EqualTo(0.7f).Within(0.0001f));
            Assert.That(
                strokes[1].Samples[0].Color.r,
                Is.EqualTo(0.3f).Within(0.0001f));
        }

        [Test]
        public void Load_WhenFinalRecordIsTruncated_KeepsEarlierStrokesAndRepairsTail()
        {
            var store = new PaintSaveStore(_directory);
            store.Append(CreateStroke(0.2f, Color.yellow));
            store.Flush();
            File.AppendAllText(
                Path.Combine(_directory, PaintSaveStore.JournalFileName),
                "{\"SchemaVersion\":");

            LogAssert.Expect(
                LogType.Warning,
                "[PaintSaveStore] Discarded an incomplete final paint record and " +
                "retained all earlier strokes.");

            var recoveredStore = new PaintSaveStore(_directory);
            var recovered = recoveredStore.Load();
            recoveredStore.Append(CreateStroke(0.8f, Color.magenta));
            recoveredStore.Flush();
            var reloaded = new PaintSaveStore(_directory).Load();

            Assert.That(recovered, Has.Count.EqualTo(1));
            Assert.That(reloaded, Has.Count.EqualTo(2));
            Assert.That(reloaded[0].Samples[0].Uv.x, Is.EqualTo(0.2f).Within(0.0001f));
            Assert.That(reloaded[1].Samples[0].Uv.x, Is.EqualTo(0.8f).Within(0.0001f));
        }

        [Test]
        public void Load_WhenSchemaIsUnsupported_LeavesJournalUntouched()
        {
            Directory.CreateDirectory(_directory);
            var journalPath = Path.Combine(_directory, PaintSaveStore.JournalFileName);
            var record = new PaintSaveStore.PaintSaveJournalRecord
            {
                SchemaVersion = PaintSaveStore.CurrentSchemaVersion + 1,
                Stroke = CreateStroke(0.4f, Color.white)
            };
            File.WriteAllText(journalPath, JsonUtility.ToJson(record) + "\n");

            var exception = Assert.Throws<NotSupportedException>(
                () => new PaintSaveStore(_directory).Load());

            Assert.That(exception.Message, Does.Contain("journal was left unchanged"));
            Assert.That(File.Exists(journalPath), Is.True);
        }

        private static PaintStroke CreateStroke(float u, Color color)
        {
            var stroke = new PaintStroke();
            stroke.Samples.Add(new PaintSample(new Vector2(u, 0.25f), color));
            return stroke;
        }
    }
}
