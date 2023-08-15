using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GiveAtkDefFamilyAbility : Ability
{
    private CardFamily family;
    private float attack;
    private float defense;

    public GiveAtkDefFamilyAbility(AbilityName name, string description, CardFamily cardFamily, float attack, float defense)
    {
        Name = name;
        Description = description;
        family = cardFamily;
        this.attack = attack;
        this.defense = defense;
    }

    private void IncreaseAtkDef(InGameInvocationCard inGameInvocationCard)
    {
        inGameInvocationCard.Defense += defense;
        inGameInvocationCard.Attack += attack;
    }

    private void DecreaseAtkDef(InGameInvocationCard inGameInvocationCard)
    {
        inGameInvocationCard.Defense -= defense;
        inGameInvocationCard.Attack -= attack;
    }
    
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        IEnumerable<InGameInvocationCard> invocationCards =
            playerCards.invocationCards.Where(card => card.Title != invocationCard.Title && card.Families.Contains(family));
        foreach (var inGameInvocationCard in invocationCards)
        {
            IncreaseAtkDef(inGameInvocationCard);
        }
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (newCard.Title != invocationCard.Title && newCard.Families.Contains(family))
        {
            IncreaseAtkDef(newCard);
        }
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (removeCard.Title != invocationCard.Title && removeCard.Families.Contains(family))
        {
            DecreaseAtkDef(removeCard);
        }
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,PlayerCards opponentPlayerCards)
    {
        IEnumerable<InGameInvocationCard> invocationCards =
            playerCards.invocationCards.Where(card => card.Title != invocationCard.Title && card.Families.Contains(family));
        foreach (var inGameInvocationCard in invocationCards)
        {
            DecreaseAtkDef(inGameInvocationCard);
        }
        return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
    }
}
