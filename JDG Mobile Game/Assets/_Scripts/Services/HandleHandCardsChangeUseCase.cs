using System;
using JDG.Application;
using JDG.Domain.Events;

/// <summary>
/// Use case for handling hand card count changes.
/// Phase 21-22: Extracted from PlayerCards.OnHandCardsChange().
/// Phase 78: OBSOLETE - Use JDG.Application.UseCases.HandleHandCardsChangeUseCase instead.
///
/// This legacy version remains for backward compatibility with code that
/// passes concrete PlayerCards types. New code should use the Application layer
/// version with IPlayerCardCollection interfaces.
/// </summary>
[Obsolete("Use JDG.Application.UseCases.HandleHandCardsChangeUseCase with IPlayerCardCollection instead. Phase 78.")]
public class HandleHandCardsChangeUseCase
{
    private readonly IEventBus _eventBus;

    public HandleHandCardsChangeUseCase(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>
    /// Handles hand card count changes by triggering equipment abilities.
    /// </summary>
    /// <param name="playerCards">The PlayerCards instance whose hand changed.</param>
    /// <param name="delta">The change in hand count (+1 for card added, -1 for card removed).</param>
    public void Execute(PlayerCards playerCards, int delta)
    {
        // Trigger equipment abilities that react to hand size changes
        foreach (var invocationCard in playerCards.InvocationCards)
        {
            var equipmentCard = invocationCard.EquipmentCard;
            if (equipmentCard == null) continue;

            foreach (var equipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentAbility.OnHandCardsChange(invocationCard, playerCards, delta);
            }
        }

        // Publish event to notify other systems
        _eventBus.Publish(new HandCardsChangedEvent
        {
            Owner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2,
            NewHandCount = playerCards.HandCards.Count,
            Delta = delta
        });
    }
}
