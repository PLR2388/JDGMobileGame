using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Infrastructure.Cards;

/// <summary>
/// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
/// Phase 28: Added IPlayerStatusProvider parameter.
/// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 36: Uses IEventBus for static UnityEvent migration.
/// </summary>
public class InvocationCardHandler : CardHandler
{
    public InvocationCardHandler(
        InGameMenuScript menuScript,
        ICardCollectionService cardCollectionService,
        IPlayerStatusProvider playerStatusProvider,
        ILocalizationService localizationService,
        IEventBus eventBus)
        : base(menuScript, cardCollectionService, playerStatusProvider, localizationService, eventBus)
    {
    }

    public override void HandleCard(InGameCard card)
    {
        // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
        var playerCard = cardCollectionService.GetCurrentPlayerCards();
        menuScript.putCardButtonText.SetText(localizationService.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD));
        var invocationCard = card as InGameInvocationCard;
        menuScript.putCardButton.interactable =
            invocationCard?.CanBeSummoned(playerCard) == true && playerCard.InvocationCards.Count < 4;
    }

    /// <summary>
    /// Phase 109: Publishes InvocationCardPlayRequestedEvent via EventBus only.
    /// Static event removed - all subscribers now use EventBus.
    /// </summary>
    public override void HandleCardPut(InGameCard card)
    {
        var invocationCard = card as InGameInvocationCard;

        // Phase 36/109: Publish via EventBus
        var playerCards = cardCollectionService.GetCurrentPlayerCards();
        var owner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
        eventBus.Publish(new InvocationCardPlayRequestedEvent
        {
            InvocationCard = invocationCard,
            Owner = owner
        });
    }
}