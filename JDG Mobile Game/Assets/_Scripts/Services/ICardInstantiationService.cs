using System.Collections.Generic;
using Cards;
using UnityEngine;

/// <summary>
/// Service interface for instantiating physical card GameObjects.
/// Replaces UnitManager singleton from Phase 17-18.
///
/// Lives in default assembly (not JDG.Application) because it depends on:
/// - Cards namespace (InGameCard from default assembly)
/// - UnityEngine (GameObject)
///
/// This is an adapter service during migration, following the same pattern as
/// other services in LegacyServicesScope.
/// </summary>
public interface ICardInstantiationService
{
    /// <summary>
    /// Initializes physical card GameObjects for a deck.
    /// </summary>
    /// <param name="deck">The deck of cards to instantiate.</param>
    /// <param name="deckLocationX">X position of the deck.</param>
    /// <param name="deckLocationY">Y position of the deck.</param>
    /// <param name="deckLocationZ">Z position of the deck.</param>
    /// <param name="isPlayerOne">True if cards belong to Player 1, affects rotation.</param>
    void InitializePhysicalCards(
        List<InGameCard> deck,
        float deckLocationX,
        float deckLocationY,
        float deckLocationZ,
        bool isPlayerOne);

    /// <summary>
    /// Retrieves the GameObject for a specific card by name.
    /// </summary>
    /// <param name="cardName">The unique name of the card (e.g., "FistiP1").</param>
    /// <param name="cardGameObject">The found GameObject, or null if not found.</param>
    /// <returns>True if the card GameObject was found; otherwise, false.</returns>
    bool TryGetCardGameObject(string cardName, out GameObject cardGameObject);

    /// <summary>
    /// Gets the count of cards currently registered in the dictionary.
    /// Useful for debugging to verify all cards were instantiated.
    /// </summary>
    /// <returns>The number of registered card GameObjects.</returns>
    int GetDictionaryCount();
}
