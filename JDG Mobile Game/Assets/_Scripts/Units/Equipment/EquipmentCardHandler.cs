using System.Linq;
using Cards;

/// <summary>
/// Handler responsible for equipment card-specific behaviors in the game.
/// </summary>
public class EquipmentCardHandler : CardHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EquipmentCardHandler"/> class.
    /// </summary>
    /// <param name="menuScript">The in-game menu script associated with this handler.</param>
    public EquipmentCardHandler(InGameMenuScript menuScript) : base(menuScript)
    {
    }

    /// <summary>
    /// Handles the card's behavior and updates the UI elements associated with an equipment card.
    /// </summary>
    /// <param name="card">The in-game card to be handled.</param>
    public override void HandleCard(InGameCard card)
    {
        var playerCard = CardManager.Instance.GetCurrentPlayerCards();
        var opponentPlayerCard = CardManager.Instance.GetOpponentPlayerCards();
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