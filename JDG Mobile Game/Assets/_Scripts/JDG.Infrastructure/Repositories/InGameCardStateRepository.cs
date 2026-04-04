using System.Collections.Generic;
using System.Linq;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Infrastructure.Repositories
{
    /// <summary>
    /// In-memory implementation of IInGameCardStateRepository.
    /// Phase 70: Created as part of UseCase migration.
    ///
    /// Stores runtime card state during gameplay.
    /// State is cleared when the game ends.
    ///
    /// Note: This is an in-memory implementation for single-player/local multiplayer.
    /// For networked multiplayer, a different implementation would sync state across clients.
    /// </summary>
    public class InGameCardStateRepository : IInGameCardStateRepository
    {
        // Main state storage indexed by card ID
        private readonly Dictionary<CardId, InGameCardState> _states = new Dictionary<CardId, InGameCardState>();

        // Location tracking for efficient queries
        private readonly Dictionary<CardOwner, HashSet<CardId>> _fieldInvocations = new Dictionary<CardOwner, HashSet<CardId>>();
        private readonly Dictionary<CardOwner, CardId?> _fieldCards = new Dictionary<CardOwner, CardId?>();
        private readonly Dictionary<CardOwner, HashSet<CardId>> _fieldEffects = new Dictionary<CardOwner, HashSet<CardId>>();
        private readonly Dictionary<CardOwner, List<CardId>> _handCards = new Dictionary<CardOwner, List<CardId>>();
        private readonly Dictionary<CardOwner, List<CardId>> _deckCards = new Dictionary<CardOwner, List<CardId>>();
        private readonly Dictionary<CardOwner, List<CardId>> _graveyardCards = new Dictionary<CardOwner, List<CardId>>();

        public InGameCardStateRepository()
        {
            InitializeOwnerCollections(CardOwner.Player1);
            InitializeOwnerCollections(CardOwner.Player2);
        }

        private void InitializeOwnerCollections(CardOwner owner)
        {
            _fieldInvocations[owner] = new HashSet<CardId>();
            _fieldCards[owner] = null;
            _fieldEffects[owner] = new HashSet<CardId>();
            _handCards[owner] = new List<CardId>();
            _deckCards[owner] = new List<CardId>();
            _graveyardCards[owner] = new List<CardId>();
        }

        #region Basic CRUD

        public InGameCardState GetState(CardId cardId)
        {
            return _states.TryGetValue(cardId, out var state) ? state : null;
        }

        public InvocationCardState GetInvocationState(CardId cardId)
        {
            return GetState(cardId) as InvocationCardState;
        }

        public void SaveState(InGameCardState state)
        {
            _states[state.Id] = state;
        }

        public void RemoveState(CardId cardId)
        {
            if (_states.TryGetValue(cardId, out var state))
            {
                // Remove from location tracking
                RemoveFromLocationTracking(cardId, state.Owner);
                _states.Remove(cardId);
            }
        }

        public bool HasState(CardId cardId)
        {
            return _states.ContainsKey(cardId);
        }

        #endregion

        #region Field Queries

        public IReadOnlyList<InvocationCardState> GetFieldInvocations(CardOwner owner)
        {
            if (!_fieldInvocations.ContainsKey(owner))
                return new List<InvocationCardState>();

            return _fieldInvocations[owner]
                .Select(id => GetInvocationState(id))
                .Where(s => s != null)
                .ToList()
                .AsReadOnly();
        }

        public FieldCardState GetFieldCard(CardOwner owner)
        {
            if (!_fieldCards.ContainsKey(owner) || !_fieldCards[owner].HasValue)
                return null;

            return GetState(_fieldCards[owner].Value) as FieldCardState;
        }

        public IReadOnlyList<EffectCardState> GetFieldEffects(CardOwner owner)
        {
            if (!_fieldEffects.ContainsKey(owner))
                return new List<EffectCardState>();

            return _fieldEffects[owner]
                .Select(id => GetState(id) as EffectCardState)
                .Where(s => s != null)
                .ToList()
                .AsReadOnly();
        }

        #endregion

        #region Hand/Deck Queries

        public IReadOnlyList<InGameCardState> GetHandCards(CardOwner owner)
        {
            if (!_handCards.ContainsKey(owner))
                return new List<InGameCardState>();

            return _handCards[owner]
                .Select(id => GetState(id))
                .Where(s => s != null)
                .ToList()
                .AsReadOnly();
        }

        public int GetHandCardCount(CardOwner owner)
        {
            return _handCards.ContainsKey(owner) ? _handCards[owner].Count : 0;
        }

        public IReadOnlyList<InGameCardState> GetDeckCards(CardOwner owner)
        {
            if (!_deckCards.ContainsKey(owner))
                return new List<InGameCardState>();

            return _deckCards[owner]
                .Select(id => GetState(id))
                .Where(s => s != null)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<InGameCardState> GetGraveyardCards(CardOwner owner)
        {
            if (!_graveyardCards.ContainsKey(owner))
                return new List<InGameCardState>();

            return _graveyardCards[owner]
                .Select(id => GetState(id))
                .Where(s => s != null)
                .ToList()
                .AsReadOnly();
        }

        #endregion

        #region Bulk Operations

        public IReadOnlyList<InGameCardState> GetAllCardsForOwner(CardOwner owner)
        {
            return _states.Values
                .Where(s => s.Owner == owner)
                .ToList()
                .AsReadOnly();
        }

        public void ClearAll()
        {
            _states.Clear();
            foreach (var owner in new[] { CardOwner.Player1, CardOwner.Player2 })
            {
                _fieldInvocations[owner].Clear();
                _fieldCards[owner] = null;
                _fieldEffects[owner].Clear();
                _handCards[owner].Clear();
                _deckCards[owner].Clear();
                _graveyardCards[owner].Clear();
            }
        }

        #endregion

        #region Location Management (for internal use or extension)

        /// <summary>
        /// Adds a card to the field invocations.
        /// Call after SaveState to update location tracking.
        /// </summary>
        public void AddToFieldInvocations(CardId cardId, CardOwner owner)
        {
            if (!_fieldInvocations.ContainsKey(owner))
                _fieldInvocations[owner] = new HashSet<CardId>();
            _fieldInvocations[owner].Add(cardId);
        }

        /// <summary>
        /// Sets the field card for a player.
        /// </summary>
        public void SetFieldCard(CardId cardId, CardOwner owner)
        {
            _fieldCards[owner] = cardId;
        }

        /// <summary>
        /// Adds a card to the field effects.
        /// </summary>
        public void AddToFieldEffects(CardId cardId, CardOwner owner)
        {
            if (!_fieldEffects.ContainsKey(owner))
                _fieldEffects[owner] = new HashSet<CardId>();
            _fieldEffects[owner].Add(cardId);
        }

        /// <summary>
        /// Adds a card to hand.
        /// </summary>
        public void AddToHand(CardId cardId, CardOwner owner)
        {
            if (!_handCards.ContainsKey(owner))
                _handCards[owner] = new List<CardId>();
            _handCards[owner].Add(cardId);
        }

        /// <summary>
        /// Removes a card from hand.
        /// </summary>
        public void RemoveFromHand(CardId cardId, CardOwner owner)
        {
            if (_handCards.ContainsKey(owner))
                _handCards[owner].Remove(cardId);
        }

        /// <summary>
        /// Adds a card to deck.
        /// </summary>
        public void AddToDeck(CardId cardId, CardOwner owner)
        {
            if (!_deckCards.ContainsKey(owner))
                _deckCards[owner] = new List<CardId>();
            _deckCards[owner].Add(cardId);
        }

        /// <summary>
        /// Adds a card to graveyard.
        /// </summary>
        public void AddToGraveyard(CardId cardId, CardOwner owner)
        {
            if (!_graveyardCards.ContainsKey(owner))
                _graveyardCards[owner] = new List<CardId>();
            _graveyardCards[owner].Add(cardId);
        }

        private void RemoveFromLocationTracking(CardId cardId, CardOwner owner)
        {
            if (_fieldInvocations.ContainsKey(owner))
                _fieldInvocations[owner].Remove(cardId);
            if (_fieldCards.ContainsKey(owner) && _fieldCards[owner] == cardId)
                _fieldCards[owner] = null;
            if (_fieldEffects.ContainsKey(owner))
                _fieldEffects[owner].Remove(cardId);
            if (_handCards.ContainsKey(owner))
                _handCards[owner].Remove(cardId);
            if (_deckCards.ContainsKey(owner))
                _deckCards[owner].Remove(cardId);
            if (_graveyardCards.ContainsKey(owner))
                _graveyardCards[owner].Remove(cardId);
        }

        #endregion
    }
}
