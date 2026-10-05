using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Dab.Runtime.Painting;
using UnityEngine;

namespace Dab.Runtime.Save
{
    [DisallowMultipleComponent]
    public sealed class CreaturePaintPersistence : MonoBehaviour
    {
        private CreaturePaintController _paint;
        private PaintSaveStore _store;
        private Task _pendingWrite;
        private bool _persistenceAvailable = true;
        private bool _observingWrites;
        private bool _writeFailureReported;

        private void Start()
        {
#if UNITY_EDITOR
            if (Application.isBatchMode)
            {
                return;
            }
#endif

            _paint = GetComponent<CreaturePaintController>();
            if (_paint == null || !_paint.isActiveAndEnabled || _paint.PaintTexture == null)
            {
                Debug.LogError(
                    $"[{nameof(CreaturePaintPersistence)}] A live paint controller " +
                    "is required; authored paint cannot be restored or saved.",
                    this);
                _persistenceAvailable = false;
                return;
            }

            _store = new PaintSaveStore(Application.persistentDataPath);
            try
            {
                var strokes = _store.Load();
                for (var i = 0; i < strokes.Count; i++)
                {
                    _paint.RestoreStroke(strokes[i]);
                }
            }
            catch (InvalidDataException exception)
            {
                ReportLoadFailure(exception);
                return;
            }
            catch (NotSupportedException exception)
            {
                ReportLoadFailure(exception);
                return;
            }
            catch (IOException exception)
            {
                ReportLoadFailure(exception);
                return;
            }
            catch (UnauthorizedAccessException exception)
            {
                ReportLoadFailure(exception);
                return;
            }
            catch (ArgumentException exception)
            {
                ReportLoadFailure(exception);
                return;
            }

            _paint.StrokeCompleted += HandleStrokeCompleted;
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                FlushPendingWrites();
            }
        }

        private void OnApplicationQuit()
        {
            FlushPendingWrites();
        }

        private void OnDestroy()
        {
            if (_paint != null)
            {
                _paint.StrokeCompleted -= HandleStrokeCompleted;
            }

            FlushPendingWrites();
        }

        private void HandleStrokeCompleted(PaintStroke stroke)
        {
            if (!_persistenceAvailable)
            {
                return;
            }

            try
            {
                _pendingWrite = _store.Append(stroke);
            }
            catch (InvalidDataException exception)
            {
                ReportWriteFailure(exception);
                return;
            }
            catch (ArgumentException exception)
            {
                ReportWriteFailure(exception);
                return;
            }

            if (!_observingWrites)
            {
                _observingWrites = true;
                StartCoroutine(ObserveWrites());
            }
        }

        private IEnumerator ObserveWrites()
        {
            while (true)
            {
                var observedWrite = _pendingWrite;
                while (observedWrite != null && !observedWrite.IsCompleted)
                {
                    yield return null;
                }

                if (observedWrite == _pendingWrite)
                {
                    break;
                }
            }

            if (_pendingWrite != null)
            {
                try
                {
                    _pendingWrite.GetAwaiter().GetResult();
                }
                catch (IOException exception)
                {
                    ReportWriteFailure(exception);
                }
                catch (UnauthorizedAccessException exception)
                {
                    ReportWriteFailure(exception);
                }
            }

            _observingWrites = false;
        }

        private void FlushPendingWrites()
        {
            if (!_persistenceAvailable || _store == null)
            {
                return;
            }

            try
            {
                _store.Flush();
            }
            catch (IOException exception)
            {
                ReportWriteFailure(exception);
            }
            catch (UnauthorizedAccessException exception)
            {
                ReportWriteFailure(exception);
            }
        }

        private void ReportLoadFailure(Exception exception)
        {
            _persistenceAvailable = false;
            Debug.LogError(
                $"[{nameof(CreaturePaintPersistence)}] Could not load the local " +
                $"paint journal. It was left unchanged. {exception.Message}",
                this);
        }

        private void ReportWriteFailure(Exception exception)
        {
            _persistenceAvailable = false;
            if (_writeFailureReported)
            {
                return;
            }

            _writeFailureReported = true;
            Debug.LogError(
                $"[{nameof(CreaturePaintPersistence)}] Could not save completed " +
                $"paint. Further saves are paused to protect the existing journal. " +
                $"{exception.Message}",
                this);
        }
    }
}
