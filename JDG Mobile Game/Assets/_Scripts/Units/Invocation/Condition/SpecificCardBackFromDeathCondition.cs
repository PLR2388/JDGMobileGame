using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation.Condition;

public class SpecificCardBackFromDeathCondition : InvocationCardOnFieldCondition
{
    public SpecificCardBackFromDeathCondition(ConditionName name, string description, List<string> cardNames) : base(name, description, cardNames)
    {
        
    }

    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return base.CanBeSummoned(playerCards) && playerCards.InvocationCards.First(card => card.Title == cardNames[0]).NumberOfDeaths > 0;
    }
}
