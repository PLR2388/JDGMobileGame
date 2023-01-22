using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class CantBeAttackIfFamilyAbility : Ability
{

    private string cardName;
    private CardFamily family;

    public CantBeAttackIfFamilyAbility(AbilityName name, string description, string cardName, CardFamily family)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
        this.family = family;
    }
    
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ProtectedByFamily(playerCards);
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        ProtectedByFamily(playerCards);
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        ProtectedByFamily(playerCards);
    }

    private void ProtectedByFamily(PlayerCards playerCards)
    {
        List<InGameInvocationCard> familyCards =
            playerCards.invocationCards.FindAll(card => card.Title != cardName && card.Families.Contains(family));
        playerCards.invocationCards.Find(card => card.Title == cardName).CantBeAttack = familyCards.Count > 0;
    } 
}
