using System.Collections.Generic;
using Cards;
using JDG.Infrastructure.Cards;

/// <summary>
/// Service interface for managing deck data and card pools.
/// Replaces GameState singleton deck storage from Phase 17-18.
///
/// This service maintains:
/// - All available cards for each player (deck1AllCards, deck2AllCards)
/// - Selected decks for gameplay (Player1DeckCards, Player2DeckCards)
/// - Tutorial deck building functionality
/// </summary>
public interface IDeckManagementService
{
    /// <summary>
    /// All available cards for Player 1's deck selection.
    /// </summary>
    List<Card> Deck1AllCards { get; }

    /// <summary>
    /// All available cards for Player 2's deck selection.
    /// </summary>
    List<Card> Deck2AllCards { get; }

    /// <summary>
    /// Player 1's selected deck for gameplay.
    /// </summary>
    List<InGameCard> Player1DeckCards { get; set; }

    /// <summary>
    /// Player 2's selected deck for gameplay.
    /// </summary>
    List<InGameCard> Player2DeckCards { get; set; }

    /// <summary>
    /// Initializes the card pools with all available cards.
    /// </summary>
    /// <param name="allCards">The complete set of available cards.</param>
    void InitializeCardPools(List<Card> allCards);

    /// <summary>
    /// Resets the deck pools for both players by creating fresh instances.
    /// </summary>
    void ResetDeckPools();

    /// <summary>
    /// Builds predefined tutorial decks for both players.
    /// </summary>
    void BuildTutorialDecks();
}
