using System.Linq;
using Cards;

/// <summary>
/// Represents a condition that checks for the presence of an invocation card
/// belonging to a specific card family on the field.
/// </summary>
public class SpecificFamilyInvocationCardOnFieldCondition : Condition
{
    /// <summary>
    /// The specific card family that an invocation card should belong to, in order to meet the condition.
    /// </summary>
    private readonly CardFamily family;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpecificFamilyInvocationCardOnFieldCondition"/> class.
    /// </summary>
    /// <param name="name">The name of the condition.</param>
    /// <param name="description">The description of the condition.</param>
    /// <param name="family">The specific card family to check for.</param>
    public SpecificFamilyInvocationCardOnFieldCondition(ConditionName name, string description, CardFamily family)
    {
        Name = name;
        Description = description;
        this.family = family;
    }

    /// <summary>
    /// Determines whether an invocation card belonging to the specified card family is present on the field.
    /// </summary>
    /// <param name="playerCards">The player cards to evaluate the condition against.</param>
    /// <returns><c>true</c> if an invocation card of the specified family is on the field; otherwise, <c>false</c>.</returns>
    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.InvocationCards.Any(card => card.Families.Contains(family));
    }

}