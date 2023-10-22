using System.Linq;
using Cards;

/// <summary>
/// Represents a condition that checks whether the number of invocation type cards 
/// in the player's YellowCards collection that have "died" (i.e., been used or destroyed)
/// is greater than or equal to a specified threshold, to determine if a card can be summoned.
/// </summary>
public class NumberInvocationDeadCondition : Condition
{
    /// <summary>
    /// The number of deaths to be met or exceeded for the condition to be satisfied.
    /// </summary>
    private readonly int numberDeath;

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberInvocationDeadCondition"/> class.
    /// </summary>
    /// <param name="name">The unique name identifier of the condition.</param>
    /// <param name="description">A description that explains the condition.</param>
    /// <param name="numberDeath">The number of deaths to compare with the actual death count of invocation cards.</param>
    public NumberInvocationDeadCondition(ConditionName name, string description, int numberDeath)
    {
        Name = name;
        Description = description;
        this.numberDeath = numberDeath;
    }

    /// <summary>
    /// Evaluates if a card can be summoned based on whether the number of invocation cards
    /// in the player's YellowCards collection that have "died" meets or exceeds the specified number.
    /// </summary>
    /// <param name="playerCards">A collection of player cards to evaluate the condition against.</param>
    /// <returns><c>true</c> if the condition for summoning is met; otherwise, <c>false</c>.</returns>
    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.YellowCards.Count(card => card.Type == CardType.Invocation) >= numberDeath;
    }
}
