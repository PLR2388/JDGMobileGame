using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class WinAtkDefFamilyAbility : Ability
{
    protected CardFamily family;
    protected float attack;
    protected float defense;

    public WinAtkDefFamilyAbility(AbilityName name, string description, CardFamily family, float atk,
        float def)
    {
        Name = name;
        Description = description;
        this.family = family;
        attack = atk;
        defense = def;
    }
    
    protected void IncrementAtkDefInvocationCard(PlayerCards playerCards, int numberCardInvocation)
    {
        invocationCard.Attack += numberCardInvocation * attack;
        invocationCard.Defense += numberCardInvocation * defense;
    }
    
    protected void DecrementAtkDefInvocationCard(PlayerCards playerCards, int numberCardInvocation)
    {
        invocationCard.Attack -= numberCardInvocation * attack;
        invocationCard.Defense -= numberCardInvocation * defense;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyPower(playerCards);
    }

    private void ApplyPower(PlayerCards playerCards)
    {
        int numberCardInvocation = playerCards.invocationCards
            .Count(card => card.Title != invocationCard.Title && card.Families.Contains(family));
        IncrementAtkDefInvocationCard(playerCards, numberCardInvocation);
    }

    public override void CancelEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.CancelEffect(playerCards, opponentPlayerCards);
        int numberCardInvocation = playerCards.invocationCards
            .Count(card => card.Title != invocationCard.Title && card.Families.Contains(family));
        DecrementAtkDefInvocationCard(playerCards, numberCardInvocation);
    }

    public override void ReactivateEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.ReactivateEffect(playerCards, opponentPlayerCards);
        ApplyPower(playerCards);
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (newCard.Title != invocationCard.Title && newCard.Families.Contains(family))
        {
            IncrementAtkDefInvocationCard(playerCards, 1);
        }
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (removeCard.Title != invocationCard.Title && removeCard.Families.Contains(family))
        {
            DecrementAtkDefInvocationCard(playerCards, 1);
        }
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
        }
        deadCard.Attack = deadCard.baseInvocationCard.BaseInvocationCardStats.Attack;
        deadCard.Defense = deadCard.baseInvocationCard.BaseInvocationCardStats.Defense;
        return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
    }
}
