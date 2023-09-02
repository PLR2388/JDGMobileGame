using System.Linq;
using Cards;

public class NumberInvocationDeadCondition : Condition
{
    private int numberDeath;

    public NumberInvocationDeadCondition(ConditionName name, string description, int numberDeath)
    {
        Name = name;
        Description = description;
        this.numberDeath = numberDeath;
    }

    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.YellowCards.Count(card => card.Type == CardType.Invocation) >= numberDeath;
    }
}
