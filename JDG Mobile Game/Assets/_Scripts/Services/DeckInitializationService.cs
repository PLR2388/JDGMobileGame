using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementation of IDeckInitializationService.
/// Bridges to legacy GameState and UnitManager singletons during migration.
///
/// Part of Phase 8 - Removes singleton dependencies from PlayerCards.
/// This service will be refactored once GameState and UnitManager are fully replaced.
/// </summary>
public class DeckInitializationService : IDeckInitializationService
{
    public List<InGameCard> GetPlayerDeck(bool isPlayerOne)
    {
        return isPlayerOne
            ? GameState.Instance.Player1DeckCards
            : GameState.Instance.Player2DeckCards;
    }

    public void InitializePhysicalCards(List<InGameCard> deck, Vector3 deckLocation, bool isPlayerOne)
    {
        UnitManager.Instance.InitPhysicalCards(deck, deckLocation, isPlayerOne);
    }
}
