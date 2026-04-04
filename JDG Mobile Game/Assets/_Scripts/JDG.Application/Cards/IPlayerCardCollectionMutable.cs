namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface extending IPlayerCardCollection with mutation methods.
    /// Phase 73: Created as part of UseCase migration.
    ///
    /// Use cases that need to modify player card collections should depend on this interface.
    /// Read-only use cases should continue to use IPlayerCardCollection.
    ///
    /// Responsibilities:
    /// - Add/remove cards from hand
    /// - Add/remove cards from field
    /// - Add/remove cards from graveyard
    /// - Deck manipulation
    /// </summary>
    public interface IPlayerCardCollectionMutable : IPlayerCardCollection
    {
        #region Hand Operations

        /// <summary>
        /// Adds a card to the player's hand.
        /// </summary>
        void AddToHand(IInGameCard card);

        /// <summary>
        /// Removes a card from the player's hand.
        /// Returns true if the card was found and removed.
        /// </summary>
        bool RemoveFromHand(IInGameCard card);

        #endregion

        #region Field Operations - Invocations

        /// <summary>
        /// Adds an invocation card to the field.
        /// </summary>
        void AddToField(IInGameInvocationCard card);

        /// <summary>
        /// Removes an invocation card from the field.
        /// Returns true if the card was found and removed.
        /// </summary>
        bool RemoveFromField(IInGameInvocationCard card);

        #endregion

        #region Field Operations - Effects

        /// <summary>
        /// Adds an effect card to the field.
        /// </summary>
        void AddEffectToField(IInGameEffectCard card);

        /// <summary>
        /// Removes an effect card from the field.
        /// Returns true if the card was found and removed.
        /// </summary>
        bool RemoveEffectFromField(IInGameEffectCard card);

        #endregion

        #region Field Operations - Field Card

        /// <summary>
        /// Sets the field card (replaces any existing field card).
        /// Pass null to remove the field card.
        /// </summary>
        void SetFieldCard(IInGameFieldCard card);

        #endregion

        #region Graveyard Operations

        /// <summary>
        /// Adds a card to the graveyard.
        /// </summary>
        void AddToGraveyard(IInGameCard card);

        /// <summary>
        /// Removes a card from the graveyard (e.g., for resurrection abilities).
        /// Returns true if the card was found and removed.
        /// </summary>
        bool RemoveFromGraveyard(IInGameCard card);

        #endregion

        #region Deck Operations

        /// <summary>
        /// Draws the top card from the deck (removes it from deck).
        /// Returns the card, or null if deck is empty.
        /// </summary>
        IInGameCard DrawFromDeck();

        /// <summary>
        /// Adds a card to the top of the deck.
        /// </summary>
        void AddToDeck(IInGameCard card);

        /// <summary>
        /// Removes a specific card from the deck.
        /// Returns true if the card was found and removed.
        /// </summary>
        bool RemoveFromDeck(IInGameCard card);

        /// <summary>
        /// Gets the number of cards remaining in the deck.
        /// </summary>
        int DeckCount { get; }

        #endregion
    }
}
