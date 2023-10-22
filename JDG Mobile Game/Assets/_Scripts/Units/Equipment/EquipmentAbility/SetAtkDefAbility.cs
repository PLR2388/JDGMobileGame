using _Scripts.Units.Invocation;

/// <summary>
/// Represents the ability of an equipment card to set the attack and defense values of an invocation card.
/// </summary>
public class SetAtkDefAbility : EquipmentAbility
{
    private readonly float? atk;
    private readonly float? def;

    private float previousAtk;
    private float previousDef;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetAtkDefAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the equipment ability.</param>
    /// <param name="description">The description of the equipment ability.</param>
    /// <param name="atk">The attack value to set. If null, the attack will not be changed.</param>
    /// <param name="def">The defense value to set. If null, the defense will not be changed.</param>
    public SetAtkDefAbility(EquipmentAbilityName name, string description, float? atk = null, float? def = null)
    {
        Name = name;
        Description = description;
        this.atk = atk;
        this.def = def;
    }

    /// <summary>
    /// Applies the set attack/defense effect on the specified invocation card.
    /// Stores the previous values for restoring later.
    /// </summary>
    /// <param name="invocationCard">The invocation card affected.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        base.ApplyEffect(invocationCard, playerCards, opponentPlayerCards);
        previousAtk = invocationCard.Attack;
        previousDef = invocationCard.Defense;
        if (atk != null)
        {
            invocationCard.Attack = (float)atk;
        }

        if (def != null)
        {
            invocationCard.Defense = (float)def;
        }
    }

    /// <summary>
    /// Removes the set attack/defense effect from the specified invocation card.
    /// Restores the previous values.
    /// </summary>
    /// <param name="invocationCard">The invocation card affected.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        base.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
        if (atk != null)
        {
            invocationCard.Attack = previousAtk;
        }

        if (def != null)
        {
            invocationCard.Defense = previousDef;
        }
    }
}