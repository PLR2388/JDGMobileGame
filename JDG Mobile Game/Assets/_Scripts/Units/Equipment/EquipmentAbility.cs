using _Scripts.Units.Invocation;

/// <summary>
/// Enumerates different names for equipment abilities.
/// </summary>
public enum EquipmentAbilityName
{
    MultiplyDefBy2ButPreventAttack,
    Earn1ATKAndMinus1DEF,
    DirectAttack,
    EarnOneQuarterATKPerHandCards,
    PreventNewOpponentToAttack,
    Remove1ATKAnd1DEF,
    SetATKToOne,
    CantBeAttackByOtherInvocations,
    SetDefToZero,
    MultiplyAtkBy3,
    Earn2ATK,
    Earn3ATKAndMinus1DEF,
    Earn1ATKAnd1DEF,
    MultiplyAtkBy2AndDefByHalf,
    EarnOneQuarterDEFPerHandCards,
    SwitchEquipmentCard,
    Loose2ATK,
    ProtectOneTimeFromDestruction,
    CancelInvocationAbility
}

/// <summary>
/// Defines the base class for equipment abilities in the game.
/// </summary>
public abstract class EquipmentAbility
{
    /// <summary>
    /// Gets or sets the name of the equipment ability.
    /// </summary>
    public EquipmentAbilityName Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the equipment ability.
    /// </summary>
    protected string Description { get; set; }

    /// <summary>
    /// Indicates whether the equipment ability can always be put.
    /// </summary>
    public bool CanAlwaysBePut = false;

    /// <summary>
    /// Applies the equipment ability's effect on the specified invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card affected.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    public virtual void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)

    {
    }

    /// <summary>
    /// Event called at the start of a turn.
    /// </summary>
    /// <param name="invocationCard">The invocation card affected.</param>
    public virtual void OnTurnStart(InGameInvocationCard invocationCard)
    {
    }

    /// <summary>
    /// Event called when the number of hand cards changes.
    /// </summary>
    /// <param name="invocationCard">The invocation card affected.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="delta">The change in the number of hand cards.</param>
    public virtual void OnHandCardsChange(InGameInvocationCard invocationCard, PlayerCards playerCards, int delta)
    {
    }

    /// <summary>
    /// Removes the effect of the equipment ability from the specified invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card affected.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    public virtual void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
    }

    /// <summary>
    /// Event called when an opponent's invocation card is added.
    /// </summary>
    /// <param name="invocationCard">The invocation card affected.</param>
    public virtual void OnOpponentInvocationCardAdded(InGameInvocationCard invocationCard)
    {
    }

    /// <summary>
    /// Determines if the invocation card should be destroyed.
    /// </summary>
    /// <param name="invocationCard">The invocation card being checked.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <returns>True if the invocation card should be destroyed, false otherwise.</returns>
    public virtual bool OnInvocationPreDestroy(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        return true;
    }
}