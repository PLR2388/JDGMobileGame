using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a condition that checks whether a specific card, that has previously been 'dead',
/// is back on the field for a card to be summoned.
/// </summary>
public class SpecificCardBackFromDeathCondition : InvocationCardOnFieldCondition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpecificCardBackFromDeathCondition"/> class.
    /// </summary>
    /// <param name="name">The unique name identifier of the condition.</param>
    /// <param name="description">A description that explains the condition.</param>
    /// <param name="cardNames">The names of the cards relevant to this condition.</param>
    public SpecificCardBackFromDeathCondition(ConditionName name, string description, List<string> cardNames) : base(name, description, cardNames)
    {
        
    }

    /// <summary>
    /// Evaluates if the specific card can be summoned, based on the condition that
    /// the card mentioned in cardNames has been brought back from death and is currently on the field.
    /// </summary>
    /// <param name="playerCards">A collection of player cards to evaluate the condition against.</param>
    /// <returns><c>true</c> if the condition for summoning is met; otherwise, <c>false</c>.</returns>
    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return base.CanBeSummoned(playerCards) && playerCards.InvocationCards.First(card => card.Title == CardNames[0]).NumberOfDeaths > 0;
    }
}
