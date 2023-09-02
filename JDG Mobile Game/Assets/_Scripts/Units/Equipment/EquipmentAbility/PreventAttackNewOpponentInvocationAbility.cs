using _Scripts.Units.Invocation;

public class PreventAttackNewOpponentInvocationAbility : EquipmentAbility
{
    public PreventAttackNewOpponentInvocationAbility(EquipmentAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override void OnOpponentInvocationCardAdded(InGameInvocationCard invocationCard)
    {
        base.OnOpponentInvocationCardAdded(invocationCard);
        invocationCard.BlockAttack();
    }

    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
        foreach (var inGameInvocationCard in opponentPlayerCards.InvocationCards)
        {
            inGameInvocationCard.UnblockAttack();
        }
    }
}
