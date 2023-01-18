using System.Collections.Generic;

namespace _Scripts.Units.Invocation.Condition
{
    public class InvocationCardOnFieldCondition : global::Condition
    {
        public InvocationCardOnFieldCondition(ConditionName name, string description, List<string> cardNames)
        {
            Name = name;
            Description = description;
            this.cardNames = cardNames;
        }
    
        private readonly List<string> cardNames;
        public override bool CanBeSummoned(PlayerCards playerCards, PlayerCards opponentPlayerCards)
        {
            return playerCards.invocationCards.Exists(card => cardNames.Contains(card.Title));
        }
    }
}
