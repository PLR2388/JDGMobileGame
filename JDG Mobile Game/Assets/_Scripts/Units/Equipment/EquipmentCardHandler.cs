using System.Linq;
using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Events;

/// <summary>
/// Handler responsible for equipment card-specific behaviors in the game.
/// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
/// Phase 28: Added IPlayerStatusProvider parameter.
/// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 36: Uses IEventBus for static UnityEvent migration.
/// </summary>
public class EquipmentCardHandler : CardHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EquipmentCardHandler"/> class.
    /// Phase 17-18: Added cardCollectionService parameter.
    /// Phase 28: Added playerStatusProvider parameter.
    /// Phase 34: Added localizationService parameter.
    /// Phase 36: Added eventBus parameter.
    /// </summary>
    /// <param name="menuScript">The in-game menu script associated with this handler.</param>
    /// <param name="cardCollectionService">The service for accessing player card collections.</param>
    /// <param name="playerStatusProvider">The provider for accessing player status.</param>
    /// <param name="localizationService">The service for localized text values.</param>
    /// <param name="eventBus">The event bus for publishing card events.</param>
    public EquipmentCardHandler(
        InGameMenuScript menuScript,
        ICardCollectionService cardCollectionService,
        IPlayerStatusProvider playerStatusProvider,
        ILocalizationService localizationService,
        IEventBus eventBus)
        : base(menuScript, cardCollectionService, playerStatusProvider, localizationService, eventBus)
    {
    }

    /// <summary>
    /// Handles the card's behavior and updates the UI elements associated with an equipment card.
    /// </summary>
    /// <param name="card">The in-game card to be handled.</param>
    public override void HandleCard(InGameCard card)
    {
        // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
        var playerCard = cardCollectionService.GetCurrentPlayerCards();
        var opponentPlayerCard = cardCollectionService.GetOpponentPlayerCards();
        menuScript.putCardButtonText.SetText(localizationService.GetLocalizedValue(LocalizationKeys.BUTTON_EQUIP_INVOCATION));
        var equipmentCard = card as InGameEquipmentCard;
        menuScript.putCardButton.interactable =
            playerCard.InvocationCards.Count(inGameInvocationCard =>
                inGameInvocationCard.EquipmentCard == null) > 0 ||
            opponentPlayerCard.InvocationCards.Count(inGameInvocationCard =>
                inGameInvocationCard.EquipmentCard == null) > 0 ||
            equipmentCard?.EquipmentAbilities.Any(ability => ability.CanAlwaysBePut) == true
            ;
    }

    /// <summary>
    /// Handles the card placement behavior for equipment cards.
    /// </summary>
    /// <param name="card">The in-game card that is being placed.</param>
    public override void HandleCardPut(InGameCard card)
    {
        if (card is InGameEquipmentCard equipmentCard)
        {
            // Phase 36: Publish via EventBus (primary)
            var playerCards = cardCollectionService.GetCurrentPlayerCards();
            var owner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
            eventBus.Publish(new EquipmentCardPlayRequestedEvent
            {
                EquipmentCard = equipmentCard,
                Owner = owner
            });

            // Keep static event for backwards compatibility during migration
            InGameMenuScript.EquipmentCardEvent.Invoke(equipmentCard);
        }
    }
}