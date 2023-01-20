using System.Collections.Generic;
using _Scripts.Units.Invocation.Condition;

public class SpecificCardBackFromDeathCondition : InvocationCardOnFieldCondition
{
    public SpecificCardBackFromDeathCondition(ConditionName name, string description, List<string> cardNames) : base(name, description, cardNames)
    {
        
    }

    public override bool CanBeSummoned(PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        return base.CanBeSummoned(playerCards, opponentPlayerCards) && playerCards.invocationCards.Find(card => card.Title == cardNames[0]).NumberOfDeaths > 0;
    }
}
