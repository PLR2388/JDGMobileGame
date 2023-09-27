using System.Linq;

/// <summary>
/// Represents a condition that checks if there is any invocation card on the field
/// with specific attack (atk) or defense (def) values for a card to be summoned.
/// </summary>
public class SpecificAtkDefInvocationCardOnFieldCondition : Condition
{
    /// <summary>
    /// The attack value to check against the invocation card's attack.
    /// </summary>
    private readonly float atk;
    
    /// <summary>
    /// The defense value to check against the invocation card's defense.
    /// </summary>
    private readonly float def;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpecificAtkDefInvocationCardOnFieldCondition"/> class.
    /// </summary>
    /// <param name="name">The unique name identifier of the condition.</param>
    /// <param name="description">A description that explains the condition.</param>
    /// <param name="atk">The attack value to compare with the invocation card's attack.</param>
    /// <param name="def">The defense value to compare with the invocation card's defense.</param>
    public SpecificAtkDefInvocationCardOnFieldCondition(ConditionName name, string description, float atk, float def)
    {
        Name = name;
        Description = description;
        this.atk = atk;
        this.def = def;
    }

    /// <summary>
    /// Evaluates if a card can be summoned based on whether any invocation card on the field
    /// has attack (atk) or defense (def) values equal to or greater than the specified values.
    /// </summary>
    /// <param name="playerCards">A collection of player cards to evaluate the condition against.</param>
    /// <returns><c>true</c> if the condition for summoning is met; otherwise, <c>false</c>.</returns>
    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.InvocationCards.Any(card => card.Defense >= def || card.Attack >= atk);
    }
}