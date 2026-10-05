using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dab.Runtime.Abilities;
using UnityEngine;

namespace Dab.Runtime.Save
{
    public sealed class SignatureMarkSaveStore
    {
        public const int CurrentSchemaVersion = 1;
        public const string SaveFileName = "dab-signature-marks.json";

        private static readonly UTF8Encoding Utf8WithoutBom = new UTF8Encoding(false);

        private readonly string _savePath;

        public SignatureMarkSaveStore(string persistentDataPath)
        {
            if (string.IsNullOrWhiteSpace(persistentDataPath))
            {
                throw new ArgumentException(
                    "A persistent-data directory is required.",
                    nameof(persistentDataPath));
            }

            _savePath = Path.Combine(persistentDataPath, SaveFileName);
        }

        public List<SignatureMarkId> Load()
        {
            if (!File.Exists(_savePath))
            {
                return new List<SignatureMarkId>();
            }

            var json = File.ReadAllText(_savePath, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(json) ||
                json[0] != '{' ||
                json[json.Length - 1] != '}')
            {
                throw new InvalidDataException(
                    "The signature-mark save is incomplete. It was left unchanged.");
            }

            SignatureMarkSaveData data;
            try
            {
                data = JsonUtility.FromJson<SignatureMarkSaveData>(json);
            }
            catch (ArgumentException exception)
            {
                throw new InvalidDataException(
                    "The signature-mark save is not valid JSON. It was left unchanged.",
                    exception);
            }

            if (data == null || data.MarkIds == null)
            {
                throw new InvalidDataException(
                    "The signature-mark save has no mark list. It was left unchanged.");
            }

            if (data.SchemaVersion != CurrentSchemaVersion)
            {
                throw new NotSupportedException(
                    $"Signature-mark save schema {data.SchemaVersion} is not supported. " +
                    "The save was left unchanged.");
            }

            var markIds = new List<SignatureMarkId>(data.MarkIds.Length);
            for (var i = 0; i < data.MarkIds.Length; i++)
            {
                var markId = (SignatureMarkId)data.MarkIds[i];
                if (!SignatureMarkCatalog.TryGetAbility(markId, out _))
                {
                    throw new NotSupportedException(
                        $"Signature-mark ID {data.MarkIds[i]} is not in the catalog. " +
                        "The save was left unchanged.");
                }

                if (markIds.Contains(markId))
                {
                    throw new InvalidDataException(
                        $"Signature-mark ID {data.MarkIds[i]} is duplicated. " +
                        "The save was left unchanged.");
                }

                markIds.Add(markId);
            }

            return markIds;
        }

        public void Save(IReadOnlyList<SignatureMarkId> markIds)
        {
            if (markIds == null)
            {
                throw new ArgumentNullException(nameof(markIds));
            }

            var data = new SignatureMarkSaveData
            {
                SchemaVersion = CurrentSchemaVersion,
                MarkIds = new int[markIds.Count]
            };

            for (var i = 0; i < markIds.Count; i++)
            {
                if (!SignatureMarkCatalog.TryGetAbility(markIds[i], out _))
                {
                    throw new InvalidDataException(
                        $"Cannot save unknown signature mark '{markIds[i]}'.");
                }

                for (var previous = 0; previous < i; previous++)
                {
                    if (markIds[previous] == markIds[i])
                    {
                        throw new InvalidDataException(
                            $"Cannot save duplicate signature mark '{markIds[i]}'.");
                    }
                }

                data.MarkIds[i] = (int)markIds[i];
            }

            var json = JsonUtility.ToJson(data);
            if (string.IsNullOrEmpty(json) ||
                json[0] != '{' ||
                json[json.Length - 1] != '}')
            {
                throw new InvalidDataException(
                    "Unity could not serialize the signature-mark save.");
            }

            var directory = Path.GetDirectoryName(_savePath);
            if (string.IsNullOrEmpty(directory))
            {
                throw new IOException(
                    "The signature-mark save has no parent directory.");
            }

            Directory.CreateDirectory(directory);
            var temporaryPath = _savePath + ".tmp";
            File.WriteAllText(temporaryPath, json, Utf8WithoutBom);

            if (File.Exists(_savePath))
            {
                File.Replace(temporaryPath, _savePath, null);
            }
            else
            {
                File.Move(temporaryPath, _savePath);
            }
        }

        [Serializable]
        public sealed class SignatureMarkSaveData
        {
            public int SchemaVersion;
            public int[] MarkIds;
        }
    }
}
