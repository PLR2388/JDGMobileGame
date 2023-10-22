using _Scripts.Units.Invocation;

/// <summary>
/// Represents an ability that allows a card to directly attack an opponent.
/// </summary>
public class DirectAttackAbility : EquipmentAbility
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DirectAttackAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    public DirectAttackAbility(EquipmentAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Applies the direct attack effect to the specified invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card to which the effect is applied.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent player's cards.</param>
    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.ApplyEffect(invocationCard, playerCards,opponentPlayerCards);
        invocationCard.CanDirectAttack = true;
    }

    /// <summary>
    /// Removes the direct attack effect from the specified invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card from which the effect is removed.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent player's cards.</param>
    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        base.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
        invocationCard.CanDirectAttack = true;
    }
}