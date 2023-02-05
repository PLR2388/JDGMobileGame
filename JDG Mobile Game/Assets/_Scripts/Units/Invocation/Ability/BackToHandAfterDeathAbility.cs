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
        if (removeCard.Title == cardName && (numberDeathMax == 0 || removeCard.NumberOfDeaths < numberDeathMax))
        {
            playerCards.handCards.Add(removeCard);
        }
    }
}
