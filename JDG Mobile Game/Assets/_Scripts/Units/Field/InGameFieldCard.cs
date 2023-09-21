using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.FieldCards;

public class InGameFieldCard : InGameCard
{
    private FieldCard baseFieldCard;
    private CardFamily family;

    public List<FieldAbility> FieldAbilities = new List<FieldAbility>();


    public static InGameFieldCard Init(FieldCard fieldCard, CardOwner cardOwner)
    {
        InGameFieldCard inGameFieldCard = new InGameFieldCard
        {
            baseFieldCard = fieldCard,
            CardOwner = cardOwner
        };
        inGameFieldCard.Reset();
        return inGameFieldCard;
    }

    private void Reset()
    {
        title = baseFieldCard.Title;
        Description = baseFieldCard.Description;
        BaseCard = baseFieldCard;
        DetailedDescription = baseFieldCard.DetailedDescription;
        type = baseFieldCard.Type;
        materialCard = baseFieldCard.MaterialCard;
        collector = baseFieldCard.Collector;
        family = baseFieldCard.Family;
        FieldAbilities = baseFieldCard.FieldAbilities.Select(
            fieldAbilityName => FieldAbilityLibrary.Instance.fieldAbilityDictionary[fieldAbilityName]
        ).ToList();
    }

    public CardFamily GetFamily()
    {
        return family;
    }
}