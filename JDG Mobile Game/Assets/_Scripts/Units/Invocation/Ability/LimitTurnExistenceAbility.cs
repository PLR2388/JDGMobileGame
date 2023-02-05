using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class LimitTurnExistenceAbility : Ability
{
    private string cardName;
    private int numberOfTurn;

    public LimitTurnExistenceAbility(AbilityName name, string description, string cardName, int numberTurn)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
        numberOfTurn = numberTurn;
    }


    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
     
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        InGameInvocationCard invocationCard = playerCards.invocationCards.Find(card => card.Title == cardName);
        if (invocationCard.NumberOfTurnOnField >= numberOfTurn)
        {
            playerCards.yellowTrash.Add(invocationCard);
            playerCards.invocationCards.Remove(invocationCard);
        }
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
