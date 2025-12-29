using System;
using System.Linq;
using _Scripts.Units.Invocation;
using JDG.Application;
using JDG.Domain.Events;

/// <summary>
/// Use case for handling a new invocation card being added to the field.
/// Phase 21-22: Extracted from PlayerCards.OnInvocationCardAdded().
/// Phase 80: OBSOLETE - Use JDG.Application.UseCases.HandleCardAddedToFieldUseCase instead.
///
/// This legacy version remains for backward compatibility with code that
/// passes concrete InGameInvocationCard and PlayerCards types.
/// </summary>
[Obsolete("Use JDG.Application.UseCases.HandleCardAddedToFieldUseCase with IInGameInvocationCard instead. Phase 80.")]
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
