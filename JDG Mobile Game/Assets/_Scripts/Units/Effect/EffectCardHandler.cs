using System.Linq;
using Cards;
using Cards.EffectCards;

/// <summary>
/// Handler responsible for effect card-specific behaviors in the game.
/// </summary>
public class EffectCardHandler : CardHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EffectCardHandler"/> class.
    /// </summary>
    /// <param name="menuScript">The in-game menu script associated with this handler.</param>
    public EffectCardHandler(InGameMenuScript menuScript) : base(menuScript)
    {
    }

    /// <summary>
    /// Handles the card's behavior and updates the UI elements associated with an effect card.
    /// </summary>
    /// <param name="card">The in-game card to be handled.</param>
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
    
    /// <summary>
    /// Handles the card placement behavior for effect cards.
    /// </summary>
    /// <param name="card">The in-game card that is being placed.</param>
    public override void HandleCardPut(InGameCard card)
    {
        if (card is InGameEffectCard effectCard)
        {
            InGameMenuScript.EffectCardEvent.Invoke(effectCard);    
        }
    }
}