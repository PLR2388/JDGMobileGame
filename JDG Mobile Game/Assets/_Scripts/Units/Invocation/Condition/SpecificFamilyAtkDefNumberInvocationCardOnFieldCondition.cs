using System.Linq;
using Cards;


/// <summary>
/// Represents a condition that checks for the presence of a specific number of
/// invocation cards belonging to a specific card family with the specified attack and defense on the field.
/// </summary>
public class SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition : Condition
{
    private readonly CardFamily family;
    private readonly float attack;
    private readonly float defense;
    private readonly int numberOfCards;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition"/> class with specified attack and defense.
    /// </summary>
    /// <param name="name">The name of the condition.</param>
    /// <param name="description">The description of the condition.</param>
    /// <param name="family">The specific card family to check for.</param>
    /// <param name="attack">The minimum attack value of the card.</param>
    /// <param name="defense">The minimum defense value of the card.</param>
    /// <param name="number">The minimum number of cards required on the field.</param>
    public SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(ConditionName name, string description, CardFamily family,
        float attack, float defense, int number)
    {
        Name = name;
        Description = description;
        this.family = family;
        this.attack = attack;
        this.defense = defense;
        numberOfCards = number;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition"/> class without specifying attack and defense.
    /// </summary>
    /// <param name="name">The name of the condition.</param>
    /// <param name="description">The description of the condition.</param>
    /// <param name="family">The specific card family to check for.</param>
    /// <param name="number">The minimum number of cards required on the field.</param>
    public SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(ConditionName name, string description,
        CardFamily family, int number)
    {
        Name = name;
        Description = description;
        this.family = family;
        attack = 0;
        defense = 0;
        numberOfCards = number;
    }
    
    
    /// <summary>
    /// Determines whether the specific number of invocation cards with the specified attack and defense belonging to the specific card family are present on the field.
    /// </summary>
    /// <param name="playerCards">The player cards to evaluate the condition against.</param>
    /// <returns><c>true</c> if the condition is met; otherwise, <c>false</c>.</returns>
    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.InvocationCards.Count(card => (card.Attack >= attack || card.Defense >= defense) && card.Families.Contains(family)) >=
               numberOfCards;
    }
}
