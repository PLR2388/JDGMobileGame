using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class WinAtkDefFamilityAtkDefConditionAbility : WinAtkDefFamilyAbility
{
    private float invocationAtkCondition;
    private float invocationDefCondition;

    public WinAtkDefFamilityAtkDefConditionAbility(AbilityName name, string description, string cardName,
        CardFamily family, float atk, float def, float invocationAtk, float invocationDef) : base(name, description,
        cardName, family, atk, def)
    {
        invocationAtkCondition = invocationAtk;
        invocationDefCondition = invocationDef;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        int numberCardInvocation = playerCards.invocationCards
            .Count(card => card.Title != cardName && card.Families.Contains(family) &&
                             card.Defense == invocationDefCondition && card.Attack == invocationAtkCondition);
        IncrementAtkDefInvocationCard(playerCards, numberCardInvocation);
    }
    
    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (newCard.Title != cardName && newCard.Families.Contains(family) &&
            newCard.Attack == invocationAtkCondition && newCard.Defense == invocationDefCondition)
        {
            IncrementAtkDefInvocationCard(playerCards, 1);
        }
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (removeCard.Title != cardName && removeCard.Families.Contains(family) &&
            removeCard.Attack == invocationAtkCondition && removeCard.Defense == invocationDefCondition)
        {
            DecrementAtkDefInvocationCard(playerCards, 1);
        }
    }
}