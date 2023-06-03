using System.Collections.Generic;
using System.Linq;

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
    
        protected readonly List<string> cardNames;
        public override bool CanBeSummoned(PlayerCards playerCards)
        {
            return playerCards.invocationCards.Any(card => cardNames.Contains(card.Title));
        }
    }
}
