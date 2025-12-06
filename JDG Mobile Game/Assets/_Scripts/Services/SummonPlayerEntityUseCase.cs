using Cards;
using Cards.InvocationCards;
using JDG.Application;

/// <summary>
/// Use case for summoning/creating a player entity card.
/// Phase 21-22: Extracted from PlayerCards.BuildPlayer().
///
/// NOTE: This use case is in the default assembly (Services folder) because it depends
/// on legacy types (InGameCard, InvocationCard, CardFactory) that haven't been migrated
/// to the Domain layer yet. It will be moved to JDG.Application once card types are refactored.
///
/// This use case handles the creation of the player entity card (the player avatar)
/// which is different from regular invocation cards.
/// </summary>
public class SummonPlayerEntityUseCase
{
    private readonly IEventBus _eventBus;

    public SummonPlayerEntityUseCase(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>
    /// Creates a player entity card for the specified player.
    /// </summary>
    /// <param name="playerInvocationCard">The base invocation card for the player entity.</param>
    /// <param name="isPlayerOne">Whether this is for Player 1 (true) or Player 2 (false).</param>
    /// <returns>The created InGameCard representing the player entity.</returns>
    public InGameCard Execute(InvocationCard playerInvocationCard, bool isPlayerOne)
    {
        var owner = isPlayerOne ? CardOwner.Player1 : CardOwner.Player2;
        return CardFactory.CreateInGameCard(playerInvocationCard, owner);
    }
}
