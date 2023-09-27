using System.Collections.Generic;
using System.Linq;

namespace _Scripts.Units.Invocation.Condition
{
    /// <summary>
    /// Represents a condition that checks whether specific invocation cards, identified by their names,
    /// are present on the field in order to determine if a card can be summoned.
    /// </summary>
    public class InvocationCardOnFieldCondition : global::Condition
    {
        
        /// <summary>
        /// List of names of the invocation cards to check for their presence on the field.
        /// </summary>
        protected readonly List<string> CardNames;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationCardOnFieldCondition"/> class.
        /// </summary>
        /// <param name="name">The unique name identifier of the condition.</param>
        /// <param name="description">A description that explains the condition.</param>
        /// <param name="cardNames">A list of names of the invocation cards to be checked for their presence on the field.</param>
        public InvocationCardOnFieldCondition(ConditionName name, string description, List<string> cardNames)
        {
            Name = name;
            Description = description;
            CardNames = cardNames;
        }
    
        /// <summary>
        /// Evaluates if a card can be summoned based on the presence of specific invocation cards on the field.
        /// </summary>
        /// <param name="playerCards">A collection of player cards to evaluate the condition against.</param>
        /// <returns><c>true</c> if any of the specified invocation cards are on the field; otherwise, <c>false</c>.</returns>
        public override bool CanBeSummoned(PlayerCards playerCards)
        {
            return playerCards.InvocationCards.Any(card => CardNames.Contains(card.Title));
        }
    }
}
