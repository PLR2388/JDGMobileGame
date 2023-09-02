using System;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public class CopyAtkDefAbility : Ability
{
    private string cardToCopyName;

    public CopyAtkDefAbility(AbilityName name, string description, string copiedCard)
    {
        Name = name;
        Description = description;
        cardToCopyName = copiedCard;
    }

    private void ApplyCopy(PlayerCards playerCards)
    {
        try
        {
            InGameInvocationCard invocationCardToCopy =
                playerCards.InvocationCards.First(card => card.Title == cardToCopyName);
            invocationCard.Attack = invocationCardToCopy.Attack;
            invocationCard.Defense = invocationCardToCopy.Defense;
        }
        catch (Exception e)
        {
            // invocation is not present amongs invocationCards
            Console.WriteLine(e);
        }
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyCopy(playerCards);
    }

    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        invocationCard.Attack = invocationCard.baseInvocationCard.BaseInvocationCardStats.Attack;
        invocationCard.Defense = invocationCard.baseInvocationCard.BaseInvocationCardStats.Defense;
    }

    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        ApplyCopy(playerCards);
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return;
        }

        ApplyCopy(playerCards);
    }

    public override void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
        base.OnCardAdded(newCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }

        ApplyCopy(playerCards);
    }

    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }

        if (removeCard.Title == cardToCopyName)
        {
            invocationCard.Attack = invocationCard.baseInvocationCard.BaseInvocationCardStats.Attack;
            invocationCard.Defense = invocationCard.baseInvocationCard.BaseInvocationCardStats.Defense;
        }
    }
    
}