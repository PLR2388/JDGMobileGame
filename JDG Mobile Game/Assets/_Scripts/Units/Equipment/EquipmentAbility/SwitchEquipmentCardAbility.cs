using _Scripts.Units.Invocation;

public class SwitchEquipmentCardAbility : EquipmentAbility
{
    public SwitchEquipmentCardAbility(EquipmentAbilityName name, string description)
    {
        Name = name;
        Description = description;
        CanAlwaysBePut = true;
    }

    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        base.ApplyEffect(invocationCard, playerCards, opponentPlayerCards);
        // As the equipment card is set after applying effect, if there is already an equipment card
        // it means it's a previous one
        var equipmentCard = invocationCard.EquipmentCard;
        if (equipmentCard != null)
        {
            playerCards.yellowCards.Add(equipmentCard);
            foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentCardEquipmentAbility.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
            }

            invocationCard.EquipmentCard = null;
        }
    }
}