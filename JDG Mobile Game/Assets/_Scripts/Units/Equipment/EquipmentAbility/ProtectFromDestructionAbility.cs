using _Scripts.Units.Invocation;

/// <summary>
/// Represents the ability of an equipment card to protect an invocation card from destruction.
/// </summary>
public class ProtectFromDestructionAbility : EquipmentAbility
{
    private int numberProtect;
    private readonly int initialNumberProtect;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtectFromDestructionAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the equipment ability.</param>
    /// <param name="description">The description of the equipment ability.</param>
    /// <param name="numberProtect">The number of times the invocation card can be protected from destruction. Defaults to 1.</param>
    public ProtectFromDestructionAbility(EquipmentAbilityName name, string description, int numberProtect = 1)
    {
        Name = name;
        Description = description;
        this.numberProtect = numberProtect;
        initialNumberProtect = numberProtect;
    }

    /// <summary>
    /// Protects the invocation card from destruction. If the number of protections is exhausted, the equipment card is moved to the yellow cards pile.
    /// </summary>
    /// <param name="invocationCard">The invocation card that is being protected.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <returns>Returns false to indicate that the invocation card is not destroyed.</returns>
    public override bool OnInvocationPreDestroy(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.OnInvocationPreDestroy(invocationCard, playerCards);
        numberProtect--;
        if (numberProtect <= 0)
        {
            playerCards.YellowCards.Add(invocationCard.EquipmentCard);
            invocationCard.EquipmentCard = null;
            numberProtect = initialNumberProtect;
        }

        return false;
    }
}
