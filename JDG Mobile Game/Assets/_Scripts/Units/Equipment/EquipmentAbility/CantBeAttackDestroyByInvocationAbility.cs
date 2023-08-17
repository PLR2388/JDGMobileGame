using _Scripts.Units.Invocation;


public class CantBeAttackDestroyByInvocationAbility : EquipmentAbility
{
    public CantBeAttackDestroyByInvocationAbility(EquipmentAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.ApplyEffect(invocationCard, playerCards,opponentPlayerCards);
        invocationCard.CantBeAttack = true;
    }

    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
        invocationCard.CantBeAttack = false;
    }
}
