using _Scripts.Units.Invocation;

/// <summary>
/// Represents the ability of an equipment card to prevent newly added opponent invocation cards from attacking.
/// </summary>
public class PreventAttackNewOpponentInvocationAbility : EquipmentAbility
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PreventAttackNewOpponentInvocationAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the equipment ability.</param>
    /// <param name="description">The description of the equipment ability.</param>
    public PreventAttackNewOpponentInvocationAbility(EquipmentAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Blocks the attack of a newly added opponent invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card that is being added to the opponent's side.</param>
    public override void OnOpponentInvocationCardAdded(InGameInvocationCard invocationCard)
    {
        base.OnOpponentInvocationCardAdded(invocationCard);
        invocationCard.BlockAttack();
    }

    /// <summary>
    /// Removes the blocking effect from opponent's invocation cards, allowing them to attack again.
    /// </summary>
    /// <param name="invocationCard">The invocation card that had the equipment ability.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent.</param>
    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
        foreach (var inGameInvocationCard in opponentPlayerCards.InvocationCards)
        {
            inGameInvocationCard.UnblockAttack();
        }
    }
}
