using Cards;
using JDG.Infrastructure.Services;
using UnityEngine;

/// <summary>
/// Adapter that bridges ICardCollectionService to CardCollectionService.
/// Part of Phase 6 - temporary adapter during migration.
///
/// Phase 17-18 Fix: Now creates CardCollectionService directly instead of using deleted CardManager.
/// Phase 27: Removed ServiceLocator usage - now uses constructor injection.
/// Finds PlayerCardManager components from the scene.
///
/// This adapter allows CardPlacementService to be dependency-injected while
/// finding scene dependencies. Once PlayerCardManager is fully migrated to services,
/// this adapter can be replaced with direct CardCollectionService registration in DI.
///
/// Requirements:
/// - Scene must have exactly 2 GameObjects with PlayerCardManager components
/// - Player1's PlayerCardManager should have a lower InstanceID (be first in hierarchy)
/// - Typically these are on "Player1Cards" and "Player2Cards" GameObjects
/// </summary>
public class CardCollectionServiceAdapter : ICardCollectionService
{
    private readonly CardCollectionService _cardCollectionService;

    public CardCollectionServiceAdapter(GameStateService gameStateService)
    {
        // Phase 27: Use injected GameStateService instead of ServiceLocator

        // Find the two PlayerCardManager components in the scene
        // Note: Using InstanceID sort to maintain backwards-compatible order
        var playerCardManagers = Object.FindObjectsByType<PlayerCardManager>(FindObjectsSortMode.InstanceID);

        if (playerCardManagers.Length < 2)
        {
            var errorMessage = $"CardCollectionServiceAdapter: Expected 2 PlayerCardManagers, found {playerCardManagers.Length}.\n" +
                "HOW TO FIX:\n" +
                "1. Ensure the Game scene has two PlayerCardManager components (for Player1 and Player2)\n" +
                "2. These are typically on 'Player1Cards' and 'Player2Cards' GameObjects\n" +
                "3. Verify the scene is properly loaded before GameSceneScope.Configure() runs\n" +
                "4. Check that these GameObjects are active in the scene hierarchy";
            Debug.LogError(errorMessage);
            throw new System.InvalidOperationException(errorMessage);
        }

        // PlayerCardManager for player 1 should be first (by convention/scene order)
        var player1CardManager = playerCardManagers[0];
        var player2CardManager = playerCardManagers[1];

        _cardCollectionService = new CardCollectionService(
            gameStateService,
            player1CardManager,
            player2CardManager
        );
    }

    public PlayerCards GetCurrentPlayerCards()
    {
        return _cardCollectionService.GetCurrentPlayerCards();
    }

    public PlayerCards GetOpponentPlayerCards()
    {
        return _cardCollectionService.GetOpponentPlayerCards();
    }
}
