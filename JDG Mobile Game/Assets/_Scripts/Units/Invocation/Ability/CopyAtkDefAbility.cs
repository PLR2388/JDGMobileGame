using System;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public class CopyAtkDefAbility : Ability
{
    private string originCardName;
    private string cardToCopyName;

    public CopyAtkDefAbility(AbilityName name, string description, string originCard, string copiedCard)
    {
        Name = name;
        Description = description;
        originCardName = originCard;
        cardToCopyName = copiedCard;
    }

    private void ApplyCopy(PlayerCards playerCards)
    {
        try
        {
            InGameInvocationCard invocationCardToCopy =
                playerCards.invocationCards.First(card => card.Title == cardToCopyName);
            InGameInvocationCard originCard = playerCards.invocationCards.First(card => card.Title == originCardName);
            originCard.Attack = invocationCardToCopy.Attack;
            originCard.Defense = invocationCardToCopy.Defense;
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

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyCopy(playerCards);
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        ApplyCopy(playerCards);
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (removeCard.Title == cardToCopyName)
        {
            InGameInvocationCard originInvocationCard =
                playerCards.invocationCards.First(card => card.Title == originCardName);
            originInvocationCard.Attack = originInvocationCard.baseInvocationCard.BaseInvocationCardStats.Attack;
            originInvocationCard.Defense = originInvocationCard.baseInvocationCard.BaseInvocationCardStats.Defense;
        }
    }
}