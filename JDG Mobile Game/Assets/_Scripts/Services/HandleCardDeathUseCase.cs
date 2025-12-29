using _Scripts.Units.Invocation;
using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using UnityEngine;

/// <summary>
/// Use case for handling card death and moving cards to the graveyard (yellow cards).
/// Phase 21-22: Extracted from PlayerCards.OnYellowTrashAdded().
/// Phase 65: Updated to inject ICanvasProvider instead of passing Transform canvas.
///
/// NOTE: This use case is in the default assembly (Services folder) because it depends
/// on legacy types (InGameInvocationCard, PlayerCards) that haven't been migrated
/// to the Domain layer yet.
///
/// This use case handles:
/// - Resetting card stats to base values
/// - Unblocking the card
/// - Freeing the card from control/equipment
/// - Triggering OnCardDeath abilities
/// </summary>
public class HandleCardDeathUseCase
{
    private readonly IEventBus _eventBus;
    private readonly ICanvasProvider _canvasProvider;

    /// <summary>
    /// Phase 65: Updated constructor to inject ICanvasProvider.
    /// </summary>
    public HandleCardDeathUseCase(IEventBus eventBus, ICanvasProvider canvasProvider)
    {
        _eventBus = eventBus;
        _canvasProvider = canvasProvider;
    }

    /// <summary>
    /// Handles a card's death, resetting its state and triggering death abilities.
    /// Phase 65: Canvas is now obtained from ICanvasProvider instead of parameter.
    /// </summary>
    /// <param name="deadCard">The card that died.</param>
    /// <param name="ownerPlayerCards">The PlayerCards instance that owns the dead card.</param>
    /// <param name="opponentPlayerCards">The opponent's PlayerCards instance.</param>
    public void Execute(
        InGameCard deadCard,
        PlayerCards ownerPlayerCards,
        PlayerCards opponentPlayerCards)
    {
        if (deadCard is InGameInvocationCard invocationCard)
        {
            // Reset card state to base values
            invocationCard.UnblockAttack();
            invocationCard.Attack = invocationCard.BaseInvocationCard.BaseInvocationCardStats.Attack;
            invocationCard.Defense = invocationCard.BaseInvocationCard.BaseInvocationCardStats.Defense;
            invocationCard.FreeCard();
            invocationCard.ResetNewTurn();

            // Phase 65: Get canvas from provider
            var canvas = _canvasProvider.GetGameCanvas() as Transform;

            // Trigger death abilities
            foreach (var ability in invocationCard.Abilities)
            {
                ability.OnCardDeath(canvas, invocationCard, ownerPlayerCards, opponentPlayerCards);
            }

            // Publish event to notify other systems
            // Convert legacy CardOwner to Domain CardOwner
            var domainOwner = (JDG.Domain.CardOwner)(int)deadCard.CardOwner;
            _eventBus.Publish(new CardDiedEvent
            {
                DeadCard = deadCard,
                Owner = domainOwner
            });
        }
    }
}
