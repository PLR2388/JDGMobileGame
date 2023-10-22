using _Scripts.Units.Invocation;
using Cards;

public class InvocationCardHandler : CardHandler
{
    public InvocationCardHandler(InGameMenuScript menuScript) : base(menuScript)
    {
    }

    public override void HandleCard(InGameCard card)
    {
        var playerCard = CardManager.Instance.GetCurrentPlayerCards();
        menuScript.putCardButtonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD));
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