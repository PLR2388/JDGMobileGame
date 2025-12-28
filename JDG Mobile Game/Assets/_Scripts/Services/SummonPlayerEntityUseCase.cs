using Cards;
using Cards.InvocationCards;
using JDG.Application.Services;

/// <summary>
/// Use case for summoning/creating a player entity card.
/// Phase 21-22: Extracted from PlayerCards.BuildPlayer().
/// Phase 62: Simplified - uses ICardFactory instead of individual ability providers.
///
/// NOTE: This use case is in the default assembly (Services folder) because it depends
/// on legacy types (InGameCard, InvocationCard) that haven't been migrated
/// to the Domain layer yet. It will be moved to JDG.Application once card types are refactored.
///
/// This use case handles the creation of the player entity card (the player avatar)
/// which is different from regular invocation cards.
/// </summary>
public class SummonPlayerEntityUseCase
{
    private readonly ICardFactory _cardFactory;

    /// <summary>
    /// Phase 62: Simplified constructor - uses ICardFactory instead of individual providers.
    /// </summary>
    public SummonPlayerEntityUseCase(ICardFactory cardFactory)
    {
        _cardFactory = cardFactory;
    }

    /// <summary>
    /// Creates a player entity card for the specified player.
    /// Phase 62: Uses ICardFactory instead of static CardFactory.CreateInGameCard.
    /// </summary>
    /// <param name="playerInvocationCard">The base invocation card for the player entity.</param>
    /// <param name="isPlayerOne">Whether this is for Player 1 (true) or Player 2 (false).</param>
    /// <returns>The created InGameCard representing the player entity.</returns>
    public InGameCard Execute(InvocationCard playerInvocationCard, bool isPlayerOne)
    {
        var owner = isPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
        return _cardFactory.CreateInvocationCard(playerInvocationCard, owner) as InGameCard;
    }
}
