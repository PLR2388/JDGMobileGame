using System.Linq;
using Cards;
using Cards.EffectCards;

public class EffectCardHandler : CardHandler
{
    public EffectCardHandler(InGameMenuScript menuScript) : base(menuScript)
    {
    }

    public override void HandleCard(InGameCard card)
    {
        var playerCard = CardManager.Instance.GetCurrentPlayerCards();
        var opponentPlayerCard = CardManager.Instance.GetOpponentPlayerCards();
        var opponentPlayerStatus = PlayerManager.Instance.GetOpponentPlayerStatus();
        var effectCard = card as InGameEffectCard;
        menuScript.putCardButtonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD));
        menuScript.putCardButton.interactable =
            effectCard?.EffectAbilities.All(elt =>
                elt.CanUseEffect(playerCard, opponentPlayerCard, opponentPlayerStatus)) == true && playerCard.EffectCards.Count < 4;
    }
    
    public override void HandleCardPut(InGameCard card)
    {
        var effectCard = card as InGameEffectCard;
        InGameMenuScript.EffectCardEvent.Invoke(effectCard);
    }
}