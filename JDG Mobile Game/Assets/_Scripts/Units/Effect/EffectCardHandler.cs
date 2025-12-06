using System.Linq;
using Cards;
using Cards.EffectCards;

/// <summary>
/// Handler responsible for effect card-specific behaviors in the game.
/// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
/// </summary>
public class EffectCardHandler : CardHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EffectCardHandler"/> class.
    /// Phase 17-18: Added cardCollectionService parameter.
    /// </summary>
    /// <param name="menuScript">The in-game menu script associated with this handler.</param>
    /// <param name="cardCollectionService">The service for accessing player card collections.</param>
    public EffectCardHandler(InGameMenuScript menuScript, ICardCollectionService cardCollectionService)
        : base(menuScript, cardCollectionService)
    {
    }

    /// <summary>
    /// Handles the card's behavior and updates the UI elements associated with an effect card.
    /// </summary>
    /// <param name="card">The in-game card to be handled.</param>
    public override void HandleCard(InGameCard card)
    {
        // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
        var playerCard = cardCollectionService.GetCurrentPlayerCards();
        var opponentPlayerCard = cardCollectionService.GetOpponentPlayerCards();
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