using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class WinAtkDefFamilyAbility : Ability
{
    protected CardFamily family;
    protected float attack;
    protected float defense;
    protected string cardName;

    public WinAtkDefFamilyAbility(AbilityName name, string description, string cardName, CardFamily family, float atk,
        float def)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
        this.family = family;
        attack = atk;
        defense = def;
    }
    
    protected void IncrementAtkDefInvocationCard(PlayerCards playerCards, int numberCardInvocation)
    {
        InGameInvocationCard invocationCard = playerCards.invocationCards.First(card => card.Title == cardName);
        invocationCard.Attack += numberCardInvocation * attack;
        invocationCard.Defense += numberCardInvocation * defense;
    }
    
    protected void DecrementAtkDefInvocationCard(PlayerCards playerCards, int numberCardInvocation)
    {
        InGameInvocationCard invocationCard = playerCards.invocationCards.First(card => card.Title == cardName);
        invocationCard.Attack -= numberCardInvocation * attack;
        invocationCard.Defense -= numberCardInvocation * defense;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        int numberCardInvocation = playerCards.invocationCards
            .Count(card => card.Title != cardName && card.Families.Contains(family));
        IncrementAtkDefInvocationCard(playerCards, numberCardInvocation);
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (newCard.Title != cardName && newCard.Families.Contains(family))
        {
            IncrementAtkDefInvocationCard(playerCards, 1);
        }
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (removeCard.Title != cardName && removeCard.Families.Contains(family))
        {
            DecrementAtkDefInvocationCard(playerCards, 1);
        }
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards)
    {
        deadCard.Attack = deadCard.baseInvocationCard.BaseInvocationCardStats.Attack;
        deadCard.Defense = deadCard.baseInvocationCard.BaseInvocationCardStats.Defense;
        return base.OnCardDeath(canvas, deadCard, playerCards);
    }
}
