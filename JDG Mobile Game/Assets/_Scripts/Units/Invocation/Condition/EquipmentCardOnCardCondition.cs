using System.Linq;

/// <summary>
/// Represents a condition that checks whether a specific equipment card is attached to a specific invocation card 
/// on the field to determine if a card can be summoned.
/// </summary>
public class EquipmentCardOnCardCondition : Condition
{
    /// <summary>
    /// The name of the equipment card to check for its attachment to the invocation card.
    /// </summary>
    private readonly string equipmentCardName;
    
    /// <summary>
    /// The name of the invocation card to check for the presence of the equipment card.
    /// </summary>
    private readonly string invocationCardName;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EquipmentCardOnCardCondition"/> class.
    /// </summary>
    /// <param name="name">The unique name identifier of the condition.</param>
    /// <param name="description">A description that explains the condition.</param>
    /// <param name="equipmentCardName">The name of the equipment card to be checked for its attachment.</param>
    /// <param name="invocationCardName">The name of the invocation card to be checked for the presence of the equipment card.</param>
    public EquipmentCardOnCardCondition(ConditionName name, string description, string equipmentCardName,
        string invocationCardName)
    {
        Name = name;
        Description = description;
        this.equipmentCardName = equipmentCardName;
        this.invocationCardName = invocationCardName;
    }

    /// <summary>
    /// Evaluates if a card can be summoned based on the presence of a specific equipment card attached to a 
    /// specific invocation card on the field.
    /// </summary>
    /// <param name="playerCards">A collection of player cards to evaluate the condition against.</param>
    /// <returns><c>true</c> if the specified equipment card is attached to the specified invocation card; otherwise, <c>false</c>.</returns>
    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.InvocationCards.Any(card =>
            card.Title == invocationCardName && card.EquipmentCard != null &&
            card.EquipmentCard.Title == equipmentCardName);
    }
}
