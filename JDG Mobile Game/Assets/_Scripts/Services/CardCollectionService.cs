using Cards;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;

/// <summary>
/// Implementation of ICardCollectionService.
/// Provides access to player card collections based on current turn state.
///
/// Note: This service is in the default assembly because it depends on legacy types
/// (PlayerCardManager, PlayerCards). It will be moved to JDG.Infrastructure once
/// these types are fully refactored.
///
/// Part of Phase 4 migration - decomposes CardManager god class.
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
        return isP1Turn ? _player1CardManager.PlayerCards : _player2CardManager.PlayerCards;
    }

    public PlayerCards GetOpponentPlayerCards()
    {
        var isP1Turn = _gameStateService.CurrentPlayer == PlayerId.Player1;
        return isP1Turn ? _player2CardManager.PlayerCards : _player1CardManager.PlayerCards;
    }
}
