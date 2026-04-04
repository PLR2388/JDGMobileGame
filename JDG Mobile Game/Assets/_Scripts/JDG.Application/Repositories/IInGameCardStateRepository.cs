using System.Collections.Generic;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;

namespace JDG.Application.Repositories
{
    /// <summary>
    /// Repository interface for managing runtime card state during gameplay.
    /// Phase 70: Created as part of UseCase migration.
    ///
    /// This repository manages the mutable gameplay state of cards,
    /// separate from card definitions (ICardRepository) and presentation (InGameCard).
    ///
    /// Responsibilities:
    /// - Store and retrieve runtime card state
    /// - Query cards by owner, type, and field position
    /// - Manage card lifecycle during gameplay
    /// </summary>
    public interface IInGameCardStateRepository
    {
        #region Basic CRUD

        /// <summary>
        /// Gets the state for a card by its instance ID.
        /// Returns null if no state exists.
        /// </summary>
        InGameCardState GetState(CardId cardId);

        /// <summary>
        /// Gets the invocation state for a card by its instance ID.
        /// Returns null if not an invocation card or no state exists.
        /// </summary>
        InvocationCardState GetInvocationState(CardId cardId);

        /// <summary>
        /// Saves or updates the state for a card.
        /// </summary>
        void SaveState(InGameCardState state);

        /// <summary>
        /// Removes the state for a card (e.g., when card is removed from game).
        /// </summary>
        void RemoveState(CardId cardId);

        /// <summary>
        /// Checks if state exists for a card.
        /// </summary>
        bool HasState(CardId cardId);

        #endregion

        #region Field Queries

        /// <summary>
        /// Gets all invocation cards currently on the field for a player.
        /// </summary>
        IReadOnlyList<InvocationCardState> GetFieldInvocations(CardOwner owner);

        /// <summary>
        /// Gets the field card for a player (if any).
        /// Returns null if no field card is active.
        /// </summary>
        FieldCardState GetFieldCard(CardOwner owner);

        /// <summary>
        /// Gets all effect cards on the field for a player.
        /// </summary>
        IReadOnlyList<EffectCardState> GetFieldEffects(CardOwner owner);

        #endregion

        #region Hand/Deck Queries

        /// <summary>
        /// Gets all cards in a player's hand.
        /// </summary>
        IReadOnlyList<InGameCardState> GetHandCards(CardOwner owner);

        /// <summary>
        /// Gets the count of cards in a player's hand.
        /// </summary>
        int GetHandCardCount(CardOwner owner);

        /// <summary>
        /// Gets all cards in a player's deck.
        /// </summary>
        IReadOnlyList<InGameCardState> GetDeckCards(CardOwner owner);

        /// <summary>
        /// Gets all cards in a player's graveyard.
        /// </summary>
        IReadOnlyList<InGameCardState> GetGraveyardCards(CardOwner owner);

        #endregion

        #region Bulk Operations

        /// <summary>
        /// Gets all card states for a player.
        /// </summary>
        IReadOnlyList<InGameCardState> GetAllCardsForOwner(CardOwner owner);

        /// <summary>
        /// Clears all card states (e.g., at game end).
        /// </summary>
        void ClearAll();

        #endregion
    }
}
