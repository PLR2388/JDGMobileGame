using _Scripts.Units.Invocation;

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
    Loose2ATK
}

public abstract class EquipmentAbility
{
    public EquipmentAbilityName Name { get; set; }

    protected string Description { get; set; }

    public bool CanAlwaysBePut = false;

    public virtual void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)

    {
    }

    public virtual void OnTurnStart(InGameInvocationCard invocationCard)
    {
    }

    public virtual void OnHandCardsChange(InGameInvocationCard invocationCard, PlayerCards playerCards, int delta)
    {
    }

    public virtual void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
    }

    public virtual void OnOpponentInvocationCardAdded(InGameInvocationCard invocationCard)
    {
    }
}