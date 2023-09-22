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
        int numberCardInvocation = playerCards.InvocationCards
            .Count(card => card.Title != invocationCard.Title && card.Families.Contains(family));
        IncrementAtkDefInvocationCard(playerCards, numberCardInvocation);
    }

    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        int numberCardInvocation = playerCards.InvocationCards
            .Count(card => card.Title != invocationCard.Title && card.Families.Contains(family));
        DecrementAtkDefInvocationCard(playerCards, numberCardInvocation);
    }

    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        ApplyPower(playerCards);
    }

    public override void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
        base.OnCardAdded(newCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (newCard.Title != invocationCard.Title && newCard.Families.Contains(family))
        {
            IncrementAtkDefInvocationCard(playerCards, 1);
        }
    }

    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
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
        deadCard.Attack = deadCard.BaseInvocationCard.BaseInvocationCardStats.Attack;
        deadCard.Defense = deadCard.BaseInvocationCard.BaseInvocationCardStats.Defense;
        return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
    }
}
