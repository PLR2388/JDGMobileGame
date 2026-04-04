using System.Collections.Generic;
using Cards;
using JDG.Infrastructure.Cards;
using UnityEngine;

/// <summary>
/// Service for initializing player decks.
/// Replaces direct GameState.Instance and UnitManager.Instance access in PlayerCards.
/// Part of Phase 8 - PlayerCards singleton removal.
///
/// Note: This interface is in the default assembly because it references legacy types
/// (InGameCard). Will be migrated once InGameCard is replaced with domain Card entity.
/// </summary>
public interface IDeckInitializationService
{
    /// <summary>
    /// Gets the initial deck cards for a player.
    /// </summary>
    /// <param name="isPlayerOne">True for player 1, false for player 2</param>
    /// <returns>List of cards in the deck</returns>
    List<InGameCard> GetPlayerDeck(bool isPlayerOne);

    /// <summary>
    /// Initializes physical card GameObjects for the deck.
    /// </summary>
    /// <param name="deck">The deck cards to instantiate</param>
    /// <param name="deckLocation">Position to place the deck</param>
    /// <param name="isPlayerOne">True for player 1, false for player 2</param>
    void InitializePhysicalCards(List<InGameCard> deck, Vector3 deckLocation, bool isPlayerOne);
}
