using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Dab.Runtime.Abilities
{
    public enum SignatureMarkBindResult
    {
        Bound = 0,
        AlreadyBound = 1,
        UnknownMark = 2
    }

    [DisallowMultipleComponent]
    public sealed class CreatureSignatureMarks : MonoBehaviour
    {
        [SerializeField]
        private List<SignatureMarkId> _boundMarks = new List<SignatureMarkId>();

        private ReadOnlyCollection<SignatureMarkId> _readOnlyBoundMarks;

        public event Action<SignatureMarkId> MarkBound;
        public event Action StateChanged;

        public IReadOnlyList<SignatureMarkId> BoundMarks => _readOnlyBoundMarks;

        private void Awake()
        {
            if (_boundMarks == null)
            {
                _boundMarks = new List<SignatureMarkId>();
            }

            _readOnlyBoundMarks = _boundMarks.AsReadOnly();
        }

        public SignatureMarkBindResult BindMark(SignatureMarkId markId)
        {
            if (!SignatureMarkCatalog.TryGetAbility(markId, out _))
            {
                return SignatureMarkBindResult.UnknownMark;
            }

            if (IsBound(markId))
            {
                return SignatureMarkBindResult.AlreadyBound;
            }

            _boundMarks.Add(markId);
            MarkBound?.Invoke(markId);
            StateChanged?.Invoke();
            return SignatureMarkBindResult.Bound;
        }

        public bool IsBound(SignatureMarkId markId)
        {
            for (var i = 0; i < _boundMarks.Count; i++)
            {
                if (_boundMarks[i] == markId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetFirstBoundAbility(out CreatureAbilityId abilityId)
        {
            if (_boundMarks.Count > 0 &&
                SignatureMarkCatalog.TryGetAbility(_boundMarks[0], out abilityId))
            {
                return true;
            }

            abilityId = CreatureAbilityId.None;
            return false;
        }

        internal void RestoreBoundMarks(IReadOnlyList<SignatureMarkId> markIds)
        {
            if (markIds == null)
            {
                throw new ArgumentNullException(nameof(markIds));
            }

            var restoredMarks = new List<SignatureMarkId>(markIds.Count);
            for (var i = 0; i < markIds.Count; i++)
            {
                var markId = markIds[i];
                if (!SignatureMarkCatalog.TryGetAbility(markId, out _))
                {
                    throw new ArgumentException(
                        $"Cannot restore unknown signature mark '{markId}'.",
                        nameof(markIds));
                }

                if (restoredMarks.Contains(markId))
                {
                    throw new ArgumentException(
                        $"Signature mark '{markId}' appears more than once.",
                        nameof(markIds));
                }

                restoredMarks.Add(markId);
            }

            _boundMarks.Clear();
            _boundMarks.AddRange(restoredMarks);
            StateChanged?.Invoke();
        }
    }
}
