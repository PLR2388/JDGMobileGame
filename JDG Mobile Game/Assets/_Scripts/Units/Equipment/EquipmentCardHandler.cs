using System.Linq;
using Cards;

/// <summary>
/// Handler responsible for equipment card-specific behaviors in the game.
/// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
/// </summary>
public class EquipmentCardHandler : CardHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EquipmentCardHandler"/> class.
    /// Phase 17-18: Added cardCollectionService parameter.
    /// </summary>
    /// <param name="menuScript">The in-game menu script associated with this handler.</param>
    /// <param name="cardCollectionService">The service for accessing player card collections.</param>
    public EquipmentCardHandler(InGameMenuScript menuScript, ICardCollectionService cardCollectionService)
        : base(menuScript, cardCollectionService)
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
        menuScript.putCardButtonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_EQUIP_INVOCATION));
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
            InGameMenuScript.EquipmentCardEvent.Invoke(equipmentCard);    
        }
    }
}