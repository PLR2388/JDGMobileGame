using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GetTypeCardFromDeckWithoutAttackAbility : Ability
{
    private CardType type;
    public GetTypeCardFromDeckWithoutAttackAbility(AbilityName name, string description, CardType cardType)
    {
        Name = name;
        Description = description;
        type = cardType;
    }
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
       
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
   
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
       
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        
    }
}
