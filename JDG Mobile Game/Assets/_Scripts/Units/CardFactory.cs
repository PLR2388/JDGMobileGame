using System;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using Cards.InvocationCards;

public class CardFactory
{
    public static InGameCard CreateInGameCard(Card card, CardOwner cardOwner)
    {
        return card switch
        {
            EquipmentCard equipmentCard => InGameEquipmentCard.Init(equipmentCard, cardOwner),
            EffectCard effectCard => InGameEffectCard.Init(effectCard, cardOwner),
            InvocationCard invocationCard => InGameInvocationCard.Init(invocationCard, cardOwner),
            FieldCard fieldCard => InGameFieldCard.Init(fieldCard, cardOwner),
            _ => throw new InvalidOperationException()
        };
    }
}