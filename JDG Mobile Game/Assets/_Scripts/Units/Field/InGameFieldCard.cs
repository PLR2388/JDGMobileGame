using Cards;
using Cards.FieldCards;

public class InGameFieldCard : InGameCard
{
    private FieldCard baseFieldCard;
    private CardFamily family;
    private FieldCardEffect fieldCardEffect;

    public FieldCardEffect FieldCardEffect => fieldCardEffect;


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
        description = baseFieldCard.Description;
        baseCard = baseFieldCard;
        detailedDescription = baseFieldCard.DetailedDescription;
        type = baseFieldCard.Type;
        materialCard = baseFieldCard.MaterialCard;
        collector = baseFieldCard.Collector;
        family = baseFieldCard.Family;
        fieldCardEffect = baseFieldCard.FieldCardEffect;
    }

    public CardFamily GetFamily()
    {
        return family;
    }
}
