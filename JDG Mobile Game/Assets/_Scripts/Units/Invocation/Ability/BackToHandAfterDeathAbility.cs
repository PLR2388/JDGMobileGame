using _Scripts.Units.Invocation;
using UnityEngine;

public class BackToHandAfterDeathAbility : Ability
{
    private int numberDeathMax = 0; // 0 = infinity

    public BackToHandAfterDeathAbility(AbilityName name, string description, int numberDeathMax = 0)
    {
        Name = name;
        Description = description;
        this.numberDeathMax = numberDeathMax;
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,PlayerCards opponentPlayerCards)
    {
        var value = base.OnCardDeath(canvas, deadCard, playerCards, opponentPlayerCards);
        if (deadCard.Title == invocationCard.Title && (numberDeathMax == 0 || deadCard.NumberOfDeaths < numberDeathMax) && !playerCards.handCards.Contains(deadCard))
        {
            playerCards.yellowCards.Remove(deadCard);
            playerCards.handCards.Add(deadCard);
        }

        return value;
    }
}
