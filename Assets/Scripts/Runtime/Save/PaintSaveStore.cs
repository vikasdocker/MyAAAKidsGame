using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dab.Runtime.Painting;
using UnityEngine;

namespace Dab.Runtime.Save
{
    public sealed class PaintSaveStore
    {
        public const int CurrentSchemaVersion = 1;
        public const string JournalFileName = "dab-paint-strokes.jsonl";

        private static readonly UTF8Encoding Utf8WithoutBom = new UTF8Encoding(false);

        private readonly string _journalPath;
        private readonly object _writeLock = new object();
        private Task _pendingWrite = Task.CompletedTask;

        public PaintSaveStore(string persistentDataPath)
        {
            if (string.IsNullOrWhiteSpace(persistentDataPath))
            {
                throw new ArgumentException(
                    "A persistent-data directory is required.",
                    nameof(persistentDataPath));
            }

            _journalPath = Path.Combine(persistentDataPath, JournalFileName);
        }

        public List<PaintStroke> Load()
        {
            Flush();

            if (!File.Exists(_journalPath))
            {
                return new List<PaintStroke>();
            }

            var contents = File.ReadAllText(_journalPath, Encoding.UTF8);
            var lastNewline = contents.LastIndexOf('\n');
            var completeContent = lastNewline < 0
                ? string.Empty
                : contents.Substring(0, lastNewline + 1);
            var hasIncompleteTail = lastNewline < contents.Length - 1;
            var strokes = new List<PaintStroke>();

            if (completeContent.Length == 0)
            {
                if (hasIncompleteTail)
                {
                    RecoverTail(string.Empty);
                }

                return strokes;
            }

            var lines = completeContent.Split('\n');
            var lastRecordIndex = lines.Length - 2;

            for (var i = 0; i <= lastRecordIndex; i++)
            {
                var line = lines[i].TrimEnd('\r');
                try
                {
                    strokes.Add(ParseRecord(line));
                }
                catch (InvalidDataException exception)
                {
                    if (i != lastRecordIndex)
                    {
                        throw new InvalidDataException(
                            $"Paint journal record {i + 1} is corrupt. " +
                            "The journal was left unchanged.",
                            exception);
                    }

                    Debug.LogWarning(
                        "[PaintSaveStore] Discarded an incomplete final paint " +
                        "record and retained all earlier strokes.");
                    RecoverTail(completeContent.Substring(0, StartOfLine(lines, i)));
                    return strokes;
                }
            }

            if (hasIncompleteTail)
            {
                Debug.LogWarning(
                    "[PaintSaveStore] Discarded an incomplete final paint " +
                    "record and retained all earlier strokes.");
                RecoverTail(completeContent);
            }

            return strokes;
        }

        public Task Append(PaintStroke stroke)
        {
            ValidateStroke(stroke);

            var record = new PaintSaveJournalRecord
            {
                SchemaVersion = CurrentSchemaVersion,
                Stroke = stroke
            };
            var line = JsonUtility.ToJson(record);
            if (string.IsNullOrEmpty(line) || line[0] != '{' || line[line.Length - 1] != '}')
            {
                throw new InvalidDataException(
                    "Unity could not serialize the completed paint stroke.");
            }

            Task write;
            lock (_writeLock)
            {
                var previousWrite = _pendingWrite;
                write = Task.Run(() =>
                {
                    previousWrite.GetAwaiter().GetResult();
                    var directory = Path.GetDirectoryName(_journalPath);
                    if (string.IsNullOrEmpty(directory))
                    {
                        throw new IOException("The paint journal has no parent directory.");
                    }

                    Directory.CreateDirectory(directory);
                    File.AppendAllText(
                        _journalPath,
                        line + "\n",
                        Utf8WithoutBom);
                });
                _pendingWrite = write;
            }

            return write;
        }

        public void Flush()
        {
            Task pendingWrite;
            lock (_writeLock)
            {
                pendingWrite = _pendingWrite;
            }

            pendingWrite.GetAwaiter().GetResult();
        }

        private static PaintStroke ParseRecord(string line)
        {
            if (string.IsNullOrWhiteSpace(line) ||
                line[0] != '{' ||
                line[line.Length - 1] != '}')
            {
                throw new InvalidDataException("The paint journal record is incomplete.");
            }

            PaintSaveJournalRecord record;
            try
            {
                record = JsonUtility.FromJson<PaintSaveJournalRecord>(line);
            }
            catch (ArgumentException exception)
            {
                throw new InvalidDataException(
                    "The paint journal record is not valid JSON.",
                    exception);
            }

            if (record == null)
            {
                throw new InvalidDataException("The paint journal record is empty.");
            }

            if (record.SchemaVersion != CurrentSchemaVersion)
            {
                throw new NotSupportedException(
                    $"Paint journal schema {record.SchemaVersion} is not supported. " +
                    "The journal was left unchanged.");
            }

            ValidateStroke(record.Stroke);
            return record.Stroke;
        }

        private void RecoverTail(string validContent)
        {
            File.WriteAllText(_journalPath, validContent, Utf8WithoutBom);
        }

        private static int StartOfLine(string[] lines, int lineIndex)
        {
            var offset = 0;
            for (var i = 0; i < lineIndex; i++)
            {
                offset += lines[i].Length + 1;
            }

            return offset;
        }

        private static void ValidateStroke(PaintStroke stroke)
        {
            if (stroke == null || stroke.Samples == null || stroke.Samples.Count == 0)
            {
                throw new InvalidDataException(
                    "A paint journal record must contain at least one sample.");
            }

            for (var i = 0; i < stroke.Samples.Count; i++)
            {
                var sample = stroke.Samples[i];
                if (!IsFinite(sample.Uv.x) ||
                    !IsFinite(sample.Uv.y) ||
                    !IsFinite(sample.Color.r) ||
                    !IsFinite(sample.Color.g) ||
                    !IsFinite(sample.Color.b) ||
                    !IsFinite(sample.Color.a))
                {
                    throw new InvalidDataException(
                        $"Paint sample {i} contains a non-finite value.");
                }
            }
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        [Serializable]
        public sealed class PaintSaveJournalRecord
        {
            public int SchemaVersion;
            public PaintStroke Stroke;
        }
    }
}
