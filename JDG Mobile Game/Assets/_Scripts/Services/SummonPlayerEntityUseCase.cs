using Cards;
using Cards.InvocationCards;
using JDG.Application;
using JDG.Application.Services;

/// <summary>
/// Use case for summoning/creating a player entity card.
/// Phase 21-22: Extracted from PlayerCards.BuildPlayer().
/// Phase 24-25: Added ICardCollectionService dependency for CardFactory.
/// Phase 7: Added IAbilityProvider for ability system migration.
/// Phase 61: Added all ability providers (required by CardFactory).
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
    private readonly ICardCollectionService _cardCollectionService;
    private readonly IAbilityProvider _abilityProvider;
    private readonly IFieldAbilityProvider _fieldAbilityProvider;
    private readonly IEquipmentAbilityProvider _equipmentAbilityProvider;
    private readonly IEffectAbilityProvider _effectAbilityProvider;
    private readonly IConditionProvider _conditionProvider;

    /// <summary>
    /// Phase 61: Updated to require all ability providers.
    /// </summary>
    public SummonPlayerEntityUseCase(
        IEventBus eventBus,
        ICardCollectionService cardCollectionService,
        IAbilityProvider abilityProvider,
        IFieldAbilityProvider fieldAbilityProvider,
        IEquipmentAbilityProvider equipmentAbilityProvider,
        IEffectAbilityProvider effectAbilityProvider,
        IConditionProvider conditionProvider)
    {
        _eventBus = eventBus;
        _cardCollectionService = cardCollectionService;
        _abilityProvider = abilityProvider;
        _fieldAbilityProvider = fieldAbilityProvider;
        _equipmentAbilityProvider = equipmentAbilityProvider;
        _effectAbilityProvider = effectAbilityProvider;
        _conditionProvider = conditionProvider;
    }

    /// <summary>
    /// Creates a player entity card for the specified player.
    /// Phase 24-25: Passes dependencies to CardFactory.
    /// Phase 7: Passes IAbilityProvider to CardFactory.
    /// Phase 61: Passes all ability providers to CardFactory.
    /// </summary>
    /// <param name="playerInvocationCard">The base invocation card for the player entity.</param>
    /// <param name="isPlayerOne">Whether this is for Player 1 (true) or Player 2 (false).</param>
    /// <returns>The created InGameCard representing the player entity.</returns>
    public InGameCard Execute(InvocationCard playerInvocationCard, bool isPlayerOne)
    {
        var owner = isPlayerOne ? CardOwner.Player1 : CardOwner.Player2;
        return CardFactory.CreateInGameCard(
            playerInvocationCard, owner, _eventBus, _cardCollectionService, _abilityProvider,
            _fieldAbilityProvider, _equipmentAbilityProvider, _effectAbilityProvider, _conditionProvider);
    }
}
