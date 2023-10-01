using _Scripts.Units.Invocation;

/// <summary>
/// Represents an ability that multiplies the attack and defense values of an invocation card.
/// Optionally, it can also prevent the invocation card from attacking.
/// </summary>
public class MultiplyAtkDefAbility : EquipmentAbility
{
    private readonly float atkFactor;
    private readonly float defenseFactor;
    private readonly bool shouldPreventAttack;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiplyAtkDefAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the equipment ability.</param>
    /// <param name="description">The description of the equipment ability.</param>
    /// <param name="atkFactor">The multiplier for attack. Default is 1f.</param>
    /// <param name="defenseFactor">The multiplier for defense. Default is 1f.</param>
    /// <param name="shouldPreventAttack">Flag to determine if the invocation card should be prevented from attacking. Default is false.</param>
    public MultiplyAtkDefAbility(EquipmentAbilityName name, string description, float atkFactor = 1f, float defenseFactor = 1f,
        bool shouldPreventAttack = false)
    {
        Name = name;
        Description = description;
        this.atkFactor = atkFactor;
        this.defenseFactor = defenseFactor;
        this.shouldPreventAttack = shouldPreventAttack;
    }

    /// <summary>
    /// Applies the effect of this ability to the specified invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card to which the effect will be applied.</param>
    /// <param name="playerCards">The player's card collection.</param>
    /// <param name="opponentPlayerCards">The opponent's card collection.</param>
    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.ApplyEffect(invocationCard, playerCards, opponentPlayerCards);
        
        invocationCard.Attack *= atkFactor;
        invocationCard.Defense *= defenseFactor;

        if (shouldPreventAttack)
        {
            invocationCard.BlockAttack();
        }
    }

    /// <summary>
    /// Handles the logic when a turn starts for the given invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card to process the turn start logic.</param>
    public override void OnTurnStart(InGameInvocationCard invocationCard)
    {
        base.OnTurnStart(invocationCard);

        if (shouldPreventAttack)
        {
            invocationCard.BlockAttack();
        }
    }

    /// <summary>
    /// Removes the effect of this ability from the specified invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card from which the effect will be removed.</param>
    /// <param name="playerCards">The player's card collection.</param>
    /// <param name="opponentPlayerCards">The opponent's card collection.</param>
    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
        
        invocationCard.Attack /= atkFactor;
        invocationCard.Defense /= defenseFactor;

        if (shouldPreventAttack)
        {
            invocationCard.UnblockAttack();
        }
    }
}

