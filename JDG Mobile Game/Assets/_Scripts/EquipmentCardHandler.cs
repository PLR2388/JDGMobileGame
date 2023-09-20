using System.Linq;
using Cards;

public class EquipmentCardHandler : CardHandler
{
    public EquipmentCardHandler(InGameMenuScript menuScript) : base(menuScript)
    {
    }

    public override void HandleCard(InGameCard card)
    {
        var playerCard = CardManager.Instance.GetCurrentPlayerCards();
        var opponentPlayerCard = CardManager.Instance.GetOpponentPlayerCards();
        menuScript.putCardButtonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_EQUIP_INVOCATION));
        var equipmentCard = card as InGameEquipementCard;
        menuScript.putCardButton.interactable =
            playerCard.InvocationCards.Count(inGameInvocationCard =>
                inGameInvocationCard.EquipmentCard == null) > 0 ||
            opponentPlayerCard.InvocationCards.Count(inGameInvocationCard =>
                inGameInvocationCard.EquipmentCard == null) > 0 ||
            equipmentCard?.EquipmentAbilities.Any(ability => ability.CanAlwaysBePut) == true
            ;
    }
    
    public override void HandleCardPut(InGameCard card)
    {
        var equipmentCard = card as InGameEquipementCard;
        InGameMenuScript.EquipmentCardEvent.Invoke(equipmentCard);
    }
}