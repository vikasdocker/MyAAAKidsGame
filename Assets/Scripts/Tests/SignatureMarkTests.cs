using System;
using System.Collections.Generic;
using System.IO;
using Dab.Runtime.Abilities;
using Dab.Runtime.Save;
using NUnit.Framework;
using UnityEngine;

namespace Dab.Tests.Abilities
{
    public sealed class SignatureMarkTests
    {
        private string _directory;
        private GameObject _creature;

        [SetUp]
        public void SetUp()
        {
            _directory = Path.Combine(
                Application.temporaryCachePath,
                "DabSignatureMarkTests_" + Guid.NewGuid().ToString("N"));
            _creature = new GameObject("SignatureMarkTestCreature");
        }

        [TearDown]
        public void TearDown()
        {
            if (_creature != null)
            {
                UnityEngine.Object.DestroyImmediate(_creature);
            }

            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        [Test]
        public void CatalogMapsHornSwirlToOneFixedAbility()
        {
            Assert.That(
                SignatureMarkCatalog.TryGetAbility(
                    SignatureMarkId.HornSwirl,
                    out var abilityId),
                Is.True);
            Assert.That(abilityId, Is.EqualTo(CreatureAbilityId.PlayfulCharge));
            Assert.That(
                SignatureMarkCatalog.TryGetAbility(
                    SignatureMarkId.None,
                    out abilityId),
                Is.False);
            Assert.That(abilityId, Is.EqualTo(CreatureAbilityId.None));
        }

        [Test]
        public void BindingIsExplicitAndDuplicateBindingIsIdempotent()
        {
            var marks = _creature.AddComponent<CreatureSignatureMarks>();
            var boundEvents = 0;
            var stateChanges = 0;
            marks.MarkBound += _ => boundEvents++;
            marks.StateChanged += () => stateChanges++;

            Assert.That(marks.BoundMarks, Is.Empty,
                "New creatures must start without authored marks.");
            Assert.That(marks.TryGetFirstBoundAbility(out _), Is.False,
                "An unmarked creature must not have an ability to perform.");
            Assert.That(
                marks.BindMark((SignatureMarkId)999),
                Is.EqualTo(SignatureMarkBindResult.UnknownMark));
            Assert.That(
                marks.BindMark(SignatureMarkId.HornSwirl),
                Is.EqualTo(SignatureMarkBindResult.Bound));
            Assert.That(
                marks.BindMark(SignatureMarkId.HornSwirl),
                Is.EqualTo(SignatureMarkBindResult.AlreadyBound));

            Assert.That(marks.BoundMarks, Has.Count.EqualTo(1));
            Assert.That(boundEvents, Is.EqualTo(1));
            Assert.That(stateChanges, Is.EqualTo(1),
                "UI observers should refresh once when authored mark state changes.");
            Assert.That(
                () => ((IList<SignatureMarkId>)marks.BoundMarks)
                    .Add(SignatureMarkId.HornSwirl),
                Throws.TypeOf<NotSupportedException>());
            Assert.That(
                marks.TryGetFirstBoundAbility(out var abilityId),
                Is.True);
            Assert.That(abilityId, Is.EqualTo(CreatureAbilityId.PlayfulCharge));
        }

        [Test]
        public void SaveStoreRoundTripsBoundMarkIds()
        {
            var marks = new[] { SignatureMarkId.HornSwirl };
            var store = new SignatureMarkSaveStore(_directory);
            store.Save(marks);

            var loaded = new SignatureMarkSaveStore(_directory).Load();
            Assert.That(loaded, Is.EqualTo(marks));

            store.Save(Array.Empty<SignatureMarkId>());

            loaded = new SignatureMarkSaveStore(_directory).Load();

            Assert.That(loaded, Is.Empty,
                "Replacing the save must preserve the latest bound-mark state.");
        }

        [Test]
        public void LoadWhenSchemaIsUnsupportedLeavesExistingSaveUntouched()
        {
            Directory.CreateDirectory(_directory);
            var savePath = Path.Combine(
                _directory,
                SignatureMarkSaveStore.SaveFileName);
            const string json = "{\"SchemaVersion\":999,\"MarkIds\":[1]}";
            File.WriteAllText(savePath, json);

            Assert.Throws<NotSupportedException>(
                () => new SignatureMarkSaveStore(_directory).Load());
            Assert.That(File.ReadAllText(savePath), Is.EqualTo(json));
        }

        [Test]
        public void LoadWhenDocumentIsIncompleteLeavesExistingSaveUntouched()
        {
            Directory.CreateDirectory(_directory);
            var savePath = Path.Combine(
                _directory,
                SignatureMarkSaveStore.SaveFileName);
            const string json = "{";
            File.WriteAllText(savePath, json);

            Assert.Throws<InvalidDataException>(
                () => new SignatureMarkSaveStore(_directory).Load());
            Assert.That(File.ReadAllText(savePath), Is.EqualTo(json));
        }
    }
}
