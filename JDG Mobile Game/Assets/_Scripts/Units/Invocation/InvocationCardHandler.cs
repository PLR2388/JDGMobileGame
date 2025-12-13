using _Scripts.Units.Invocation;
using Cards;
using JDG.Application.Services;

/// <summary>
/// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
/// Phase 28: Added IPlayerStatusProvider parameter.
/// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// </summary>
public class InvocationCardHandler : CardHandler
{
    public InvocationCardHandler(
        InGameMenuScript menuScript,
        ICardCollectionService cardCollectionService,
        IPlayerStatusProvider playerStatusProvider,
        ILocalizationService localizationService)
        : base(menuScript, cardCollectionService, playerStatusProvider, localizationService)
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
    
    public override void HandleCardPut(InGameCard card)
    {
        var invocationCard = card as InGameInvocationCard;
        InGameMenuScript.InvocationCardEvent.Invoke(invocationCard);
    }
}