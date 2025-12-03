using JDG.Application.Services;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;
using UnityEngine.Events;

/// <summary>
/// Implementation of ICardDrawService.
/// Handles card drawing for the current player.
///
/// Note: This service is in the default assembly because it depends on legacy types
/// (PlayerCardManager). It will be moved to JDG.Infrastructure once
/// PlayerCardManager is fully refactored.
///
/// Part of Phase 4 migration - decomposes CardManager god class.
/// </summary>
public class CardDrawService : ICardDrawService
{
    private readonly GameStateService _gameStateService;
    private readonly PlayerCardManager _player1CardManager;
    private readonly PlayerCardManager _player2CardManager;

    public CardDrawService(
        GameStateService gameStateService,
        PlayerCardManager player1CardManager,
        PlayerCardManager player2CardManager)
    {
        _gameStateService = gameStateService;
        _player1CardManager = player1CardManager;
        _player2CardManager = player2CardManager;
    }

    public void DrawCard(UnityAction onNoCard)
    {
        var currentCardManager = GetCurrentPlayerCardManager();
        currentCardManager.DrawCard(onNoCard);
    }

    private PlayerCardManager GetCurrentPlayerCardManager()
    {
        var isP1Turn = _gameStateService.CurrentPlayer == PlayerId.Player1;
        return isP1Turn ? _player1CardManager : _player2CardManager;
    }
}
