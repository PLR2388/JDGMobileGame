using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class WinAtkDefFamilityAtkDefConditionAbility : WinAtkDefFamilyAbility
{
    private float invocationAtkCondition;
    private float invocationDefCondition;

    public WinAtkDefFamilityAtkDefConditionAbility(AbilityName name, string description,
        CardFamily family, float atk, float def, float invocationAtk, float invocationDef) : base(name, description,family, atk, def)
    {
        invocationAtkCondition = invocationAtk;
        invocationDefCondition = invocationDef;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyPower(playerCards);
    }

    private void ApplyPower(PlayerCards playerCards)
    {
        int numberCardInvocation = playerCards.InvocationCards
            .Count(card => card.Title != invocationCard.Title && card.Families.Contains(family) &&
                           card.Defense == invocationDefCondition && card.Attack == invocationAtkCondition);
        IncrementAtkDefInvocationCard(playerCards, numberCardInvocation);
    }

    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        ApplyPower(playerCards);
    }

    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        int numberCardInvocation = playerCards.InvocationCards
            .Count(card => card.Title != invocationCard.Title && card.Families.Contains(family) &&
                           card.Defense == invocationDefCondition && card.Attack == invocationAtkCondition);
        DecrementAtkDefInvocationCard(playerCards, numberCardInvocation);
    }

    public override void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
        base.OnCardAdded(newCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (newCard.Title != invocationCard.Title && newCard.Families.Contains(family) &&
            newCard.Attack == invocationAtkCondition && newCard.Defense == invocationDefCondition)
        {
            IncrementAtkDefInvocationCard(playerCards, 1);
        }
    }

    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            
        }
        if (removeCard.Title != invocationCard.Title && removeCard.Families.Contains(family) &&
            removeCard.Attack == invocationAtkCondition && removeCard.Defense == invocationDefCondition)
        {
            DecrementAtkDefInvocationCard(playerCards, 1);
        }
    }
}