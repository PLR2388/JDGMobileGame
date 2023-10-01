using _Scripts.Units.Invocation;

/// <summary>
/// Represents an ability that prevents a card from being attacked or destroyed by invocations.
/// </summary>
public class CantBeAttackDestroyByInvocationAbility : EquipmentAbility
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CantBeAttackDestroyByInvocationAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    public CantBeAttackDestroyByInvocationAbility(EquipmentAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Applies the effect to the specified invocation card, preventing it from being attacked.
    /// </summary>
    /// <param name="invocationCard">The invocation card to which the effect is applied.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent player's cards.</param>
    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.ApplyEffect(invocationCard, playerCards,opponentPlayerCards);
        invocationCard.CantBeAttack = true;
    }

    /// <summary>
    /// Removes the effect from the specified invocation card, allowing it to be attacked again.
    /// </summary>
    /// <param name="invocationCard">The invocation card from which the effect is removed.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent player's cards.</param>
    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
        invocationCard.CantBeAttack = false;
    }
}
