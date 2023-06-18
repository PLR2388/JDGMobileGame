using _Scripts.Units.Invocation;
using UnityEngine;

public class BackToHandAfterDeathAbility : Ability
{

    private string cardName;
    private int numberDeathMax = 0; // 0 = infinity

    public BackToHandAfterDeathAbility(AbilityName name, string description, string card, int numberDeathMax = 0)
    {
        Name = name;
        Description = description;
        cardName = card;
        this.numberDeathMax = numberDeathMax;
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards)
    {
        var value = base.OnCardDeath(canvas, deadCard, playerCards);
        if (deadCard.Title == cardName && (numberDeathMax == 0 || deadCard.NumberOfDeaths < numberDeathMax))
        {
            playerCards.yellowTrash.Remove(deadCard);
            playerCards.handCards.Add(deadCard);
        }

        return value;
    }
}
