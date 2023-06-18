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
    private string originCardName;

    public GiveAtkDefFamilyAbility(AbilityName name, string description, string cardName, CardFamily cardFamily, float attack, float defense)
    {
        Name = name;
        Description = description;
        family = cardFamily;
        this.attack = attack;
        this.defense = defense;
        originCardName = cardName;
    }

    private void IncreaseAtkDef(InGameInvocationCard invocationCard)
    {
        invocationCard.Defense += defense;
        invocationCard.Attack += attack;
    }

    private void DecreaseAtkDef(InGameInvocationCard invocationCard)
    {
        invocationCard.Defense -= defense;
        invocationCard.Attack -= attack;
    }
    
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        IEnumerable<InGameInvocationCard> invocationCards =
            playerCards.invocationCards.Where(card => card.Title != originCardName && card.Families.Contains(family));
        foreach (var invocationCard in invocationCards)
        {
            IncreaseAtkDef(invocationCard);
        }
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (newCard.Title != originCardName && newCard.Families.Contains(family))
        {
            IncreaseAtkDef(newCard);
        }
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (removeCard.Title != originCardName && removeCard.Families.Contains(family))
        {
            DecreaseAtkDef(removeCard);
        }
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards)
    {
        IEnumerable<InGameInvocationCard> invocationCards =
            playerCards.invocationCards.Where(card => card.Title != originCardName && card.Families.Contains(family));
        foreach (var invocationCard in invocationCards)
        {
            DecreaseAtkDef(invocationCard);
        }
        return base.OnCardDeath(canvas, deadCard, playerCards);
    }
}
