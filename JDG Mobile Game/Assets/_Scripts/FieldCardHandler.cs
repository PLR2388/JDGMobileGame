using Cards;

public class FieldCardHandler : CardHandler
{
    public FieldCardHandler(InGameMenuScript menuScript) : base(menuScript)
    {
    }

    public override void HandleCard(InGameCard card)
    {
        var playerCard = CardManager.Instance.GetCurrentPlayerCards();
        menuScript.putCardButtonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD));
        menuScript.putCardButton.interactable = playerCard.FieldCard == null;
    }
    
    public override void HandleCardPut(InGameCard card)
    {
        var fieldCard = card as InGameFieldCard;
        InGameMenuScript.FieldCardEvent.Invoke(fieldCard);
    }
}