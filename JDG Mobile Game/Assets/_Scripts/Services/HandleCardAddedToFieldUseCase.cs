using System.Linq;
using _Scripts.Units.Invocation;
using JDG.Application;
using JDG.Domain.Events;

/// <summary>
/// Use case for handling a new invocation card being added to the field.
/// Phase 21-22: Extracted from PlayerCards.OnInvocationCardAdded().
///
/// NOTE: This use case is in the default assembly (Services folder) because it depends
/// on legacy types (InGameInvocationCard, PlayerCards) that haven't been migrated
/// to the Domain layer yet.
///
/// This use case handles triggering abilities when a new card enters the field:
/// - Opponent's equipment card abilities (OnOpponentInvocationCardAdded)
/// - Existing invocation card abilities (OnCardAdded)
/// - Effect card abilities (OnInvocationCardAdded)
/// - Field card abilities (OnInvocationCardAdded)
/// </summary>
public class HandleCardAddedToFieldUseCase
{
    private readonly IEventBus _eventBus;

    public HandleCardAddedToFieldUseCase(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>
    /// Handles a new invocation card being added to the field.
    /// Triggers all relevant abilities for both players.
    /// </summary>
    /// <param name="newInvocationCard">The new invocation card that was added.</param>
    /// <param name="ownerPlayerCards">The PlayerCards instance that owns the new card.</param>
    /// <param name="opponentPlayerCards">The opponent's PlayerCards instance.</param>
    public void Execute(
        InGameInvocationCard newInvocationCard,
        PlayerCards ownerPlayerCards,
        PlayerCards opponentPlayerCards)
    {
        // Trigger opponent's equipment abilities
        foreach (var opponentCard in opponentPlayerCards.InvocationCards)
        {
            var equipmentCard = opponentCard.EquipmentCard;
            if (equipmentCard == null) continue;

            foreach (var equipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentAbility.OnOpponentInvocationCardAdded(newInvocationCard);
            }
        }

        // Trigger existing invocation card abilities on same field
        foreach (var existingCard in ownerPlayerCards.InvocationCards)
        {
            foreach (var ability in existingCard.Abilities)
            {
                ability.OnCardAdded(newInvocationCard, ownerPlayerCards);
            }
        }

        // Trigger effect card abilities
        foreach (var effectAbility in ownerPlayerCards.EffectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectAbility.OnInvocationCardAdded(ownerPlayerCards, newInvocationCard);
        }

        // Trigger field card abilities
        if (ownerPlayerCards.FieldCard?.FieldAbilities != null)
        {
            foreach (var fieldAbility in ownerPlayerCards.FieldCard.FieldAbilities)
            {
                fieldAbility.OnInvocationCardAdded(newInvocationCard, ownerPlayerCards);
            }
        }

        // Publish event to notify other systems
        // Convert legacy CardOwner to Domain CardOwner
        var domainOwner = (JDG.Domain.CardOwner)(int)newInvocationCard.CardOwner;
        _eventBus.Publish(new CardAddedToFieldEvent
        {
            AddedCard = newInvocationCard,
            Owner = domainOwner
        });
    }
}
