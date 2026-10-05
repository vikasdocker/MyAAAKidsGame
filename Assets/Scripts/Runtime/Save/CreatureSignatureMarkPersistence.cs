using System;
using System.IO;
using Dab.Runtime.Abilities;
using UnityEngine;

namespace Dab.Runtime.Save
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CreatureSignatureMarks))]
    public sealed class CreatureSignatureMarkPersistence : MonoBehaviour
    {
        private CreatureSignatureMarks _marks;
        private SignatureMarkSaveStore _store;
        private bool _persistenceAvailable = true;
        private bool _failureReported;

        private void Start()
        {
#if UNITY_EDITOR
            if (Application.isBatchMode)
            {
                return;
            }
#endif

            _marks = GetComponent<CreatureSignatureMarks>();
            if (_marks == null)
            {
                ReportFailure(
                    "A CreatureSignatureMarks component is required to restore or save marks.");
                return;
            }

            _store = new SignatureMarkSaveStore(Application.persistentDataPath);
            try
            {
                _marks.RestoreBoundMarks(_store.Load());
            }
            catch (InvalidDataException exception)
            {
                ReportFailure(exception.Message);
                return;
            }
            catch (NotSupportedException exception)
            {
                ReportFailure(exception.Message);
                return;
            }
            catch (IOException exception)
            {
                ReportFailure(exception.Message);
                return;
            }
            catch (UnauthorizedAccessException exception)
            {
                ReportFailure(exception.Message);
                return;
            }
            catch (ArgumentException exception)
            {
                ReportFailure(exception.Message);
                return;
            }

            _marks.MarkBound += HandleMarkBound;
        }

        private void OnDestroy()
        {
            if (_marks != null)
            {
                _marks.MarkBound -= HandleMarkBound;
            }
        }

        private void HandleMarkBound(SignatureMarkId markId)
        {
            if (!_persistenceAvailable)
            {
                return;
            }

            try
            {
                _store.Save(_marks.BoundMarks);
            }
            catch (InvalidDataException exception)
            {
                ReportFailure(exception.Message);
            }
            catch (IOException exception)
            {
                ReportFailure(exception.Message);
            }
            catch (UnauthorizedAccessException exception)
            {
                ReportFailure(exception.Message);
            }
            catch (NotSupportedException exception)
            {
                ReportFailure(exception.Message);
            }
            catch (ArgumentException exception)
            {
                ReportFailure(exception.Message);
            }
        }

        private void ReportFailure(string message)
        {
            _persistenceAvailable = false;
            if (_failureReported)
            {
                return;
            }

            _failureReported = true;
            Debug.LogError(
                $"[{nameof(CreatureSignatureMarkPersistence)}] Signature marks " +
                $"could not be loaded or saved. Existing save data is preserved. {message}",
                this);
        }
    }
}
