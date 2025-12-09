using System.Linq;
using _Scripts.Units.Invocation;
using JDG.Application;
using JDG.Domain.Events;

/// <summary>
/// Use case for handling an invocation card being removed from the field.
/// Phase 21-22: Extracted from PlayerCards.OnInvocationCardsRemoved().
///
/// NOTE: This use case is in the default assembly (Services folder) because it depends
/// on legacy types (InGameInvocationCard, PlayerCards) that haven't been migrated
/// to the Domain layer yet.
///
/// This use case handles triggering abilities when a card is removed from field:
/// - Existing invocation card abilities (OnCardRemove)
/// - Effect card abilities (OnInvocationCardRemoved)
/// </summary>
public class HandleCardRemovedFromFieldUseCase
{
    private readonly IEventBus _eventBus;

    public HandleCardRemovedFromFieldUseCase(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>
    /// Handles an invocation card being removed from the field.
    /// Triggers all relevant abilities for the removed card.
    /// </summary>
    /// <param name="removedInvocationCard">The card that was removed from the field.</param>
    /// <param name="ownerPlayerCards">The PlayerCards instance that owns the removed card.</param>
    public void Execute(
        InGameInvocationCard removedInvocationCard,
        PlayerCards ownerPlayerCards)
    {
        var cloneInvocationCards = ownerPlayerCards.InvocationCards.ToList();

        // Apply OnCardRemove for invocation cards that are still alive
        foreach (var ability in cloneInvocationCards.SelectMany(inGameInvocationCard => inGameInvocationCard.Abilities))
        {
            ability.OnCardRemove(removedInvocationCard, ownerPlayerCards);
        }

        // Trigger effect card abilities
        foreach (var effectAbility in ownerPlayerCards.EffectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectAbility.OnInvocationCardRemoved(ownerPlayerCards, removedInvocationCard);
        }

        // Publish event to notify other systems
        var domainOwner = (JDG.Domain.CardOwner)(int)removedInvocationCard.CardOwner;
        _eventBus.Publish(new CardRemovedFromFieldEvent
        {
            CardId = System.Guid.Empty, // Legacy cards don't have Guid IDs yet
            Owner = domainOwner
        });
    }
}
