using _Scripts.Units.Invocation;

/// <summary>
/// Represents the ability of an equipment card to allow a player to switch it with another equipment card.
/// </summary>
public class SwitchEquipmentCardAbility : EquipmentAbility
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchEquipmentCardAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the equipment ability.</param>
    /// <param name="description">The description of the equipment ability.</param>
    public SwitchEquipmentCardAbility(EquipmentAbilityName name, string description)
    {
        Name = name;
        Description = description;
        CanAlwaysBePut = true;
    }

    /// <summary>
    /// Applies the switch equipment card ability effect on the specified invocation card.
    /// If the invocation card already has an equipment card, the previous one is moved to the yellow cards 
    /// and its effects are removed from the invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card affected.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        base.ApplyEffect(invocationCard, playerCards, opponentPlayerCards);
        // As the equipment card is set after applying effect, if there is already an equipment card
        // it means it's a previous one
        var equipmentCard = invocationCard.EquipmentCard;
        if (equipmentCard != null)
        {
            playerCards.YellowCards.Add(equipmentCard);
            foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentCardEquipmentAbility.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
            }

            invocationCard.EquipmentCard = null;
        }
    }
}