using System.Collections.Generic;
using Cards;
using UnityEngine;

/// <summary>
/// Implementation of IDeckInitializationService.
/// Phase 17-18: Refactored to use DI services instead of GameState and UnitManager singletons.
///
/// Now uses:
/// - IDeckManagementService for deck data (replaces GameState.Instance)
/// - ICardInstantiationService for GameObject creation (replaces UnitManager.Instance)
/// </summary>
public class DeckInitializationService : IDeckInitializationService
{
    private readonly IDeckManagementService _deckManagementService;
    private readonly ICardInstantiationService _cardInstantiationService;

    public DeckInitializationService(
        IDeckManagementService deckManagementService,
        ICardInstantiationService cardInstantiationService)
    {
        _deckManagementService = deckManagementService;
        _cardInstantiationService = cardInstantiationService;
    }

    public List<InGameCard> GetPlayerDeck(bool isPlayerOne)
    {
        return isPlayerOne
            ? _deckManagementService.Player1DeckCards
            : _deckManagementService.Player2DeckCards;
    }

    public void InitializePhysicalCards(List<InGameCard> deck, Vector3 deckLocation, bool isPlayerOne)
    {
        _cardInstantiationService.InitializePhysicalCards(
            deck,
            deckLocation.x,
            deckLocation.y,
            deckLocation.z,
            isPlayerOne);
    }
}
