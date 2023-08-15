using UnityEngine;

public class LimitTurnExistenceAbility : Ability
{
    private int numberOfTurn;

    public LimitTurnExistenceAbility(AbilityName name, string description, int numberTurn)
    {
        Name = name;
        Description = description;
        numberOfTurn = numberTurn;
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (invocationCard.NumberOfTurnOnField >= numberOfTurn)
        {
            playerCards.yellowCards.Add(invocationCard);
            playerCards.invocationCards.Remove(invocationCard);
        }
        
    }
}