using Cards;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;
using UnityEngine;

/// <summary>
/// Implementation of ICardCollectionService.
/// Provides access to player card collections based on current turn state.
///
/// Note: This service is in the default assembly because it depends on legacy types
/// (PlayerCardManager, PlayerCards). It will be moved to JDG.Infrastructure once
/// these types are fully refactored.
///
/// Part of Phase 4 migration - decomposes CardManager god class.
/// Phase 135: Added null checks for defensive programming.
/// </summary>
public class CardCollectionService : ICardCollectionService
{
    private readonly GameStateService _gameStateService;
    private readonly PlayerCardManager _player1CardManager;
    private readonly PlayerCardManager _player2CardManager;

    public CardCollectionService(
        GameStateService gameStateService,
        PlayerCardManager player1CardManager,
        PlayerCardManager player2CardManager)
    {
        _gameStateService = gameStateService;
        _player1CardManager = player1CardManager;
        _player2CardManager = player2CardManager;
    }

    public PlayerCards GetCurrentPlayerCards()
    {
        var isP1Turn = _gameStateService.CurrentPlayer == PlayerId.Player1;
        var cardManager = isP1Turn ? _player1CardManager : _player2CardManager;
        // Phase 135: Add null check for defensive programming
        if (cardManager == null)
        {
            Debug.LogError($"CardCollectionService: PlayerCardManager for {(isP1Turn ? "Player1" : "Player2")} is null!");
            return null;
        }
        return cardManager.PlayerCards;
    }

    public PlayerCards GetOpponentPlayerCards()
    {
        var isP1Turn = _gameStateService.CurrentPlayer == PlayerId.Player1;
        var cardManager = isP1Turn ? _player2CardManager : _player1CardManager;
        // Phase 135: Add null check for defensive programming
        if (cardManager == null)
        {
            Debug.LogError($"CardCollectionService: Opponent PlayerCardManager for {(isP1Turn ? "Player2" : "Player1")} is null!");
            return null;
        }
        return cardManager.PlayerCards;
    }
}
