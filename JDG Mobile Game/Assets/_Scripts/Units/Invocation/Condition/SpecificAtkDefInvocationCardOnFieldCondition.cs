
using System.Linq;

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
        return playerCards.InvocationCards.Any(card => card.Defense >= def || card.Attack >= atk);
    }
}