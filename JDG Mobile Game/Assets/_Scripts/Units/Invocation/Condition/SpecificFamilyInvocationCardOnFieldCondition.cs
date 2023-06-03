using System.Linq;
using Cards;

public class SpecificFamilyInvocationCardOnFieldCondition : Condition
{
    private CardFamily family;

    public SpecificFamilyInvocationCardOnFieldCondition(ConditionName name, string description, CardFamily family)
    {
        Name = name;
        Description = description;
        this.family = family;
    }

    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.invocationCards.Any(card => card.Families.Contains(family));
    }

}