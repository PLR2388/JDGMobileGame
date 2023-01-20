
public class SpecificAtkDefInvocationCardOnFieldCondition : Condition
{
    private float atk;
    private float def;

    public SpecificAtkDefInvocationCardOnFieldCondition(ConditionName name, string description, float atk, float def)
    {
        Name = name;
        Description = description;
        this.atk = atk;
        this.def = def;
    }

    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.invocationCards.Exists(card => card.Defense >= def || card.Attack >= atk);
    }
}