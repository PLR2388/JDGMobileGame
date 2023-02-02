using _Scripts.Units.Invocation;
using UnityEngine;

public class BackToHandAfterDeathAbility : Ability
{

    private string cardName;

    public BackToHandAfterDeathAbility(AbilityName name, string description, string card)
    {
        Name = name;
        Description = description;
        cardName = card;
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
        if (removeCard.Title == cardName)
        {
            playerCards.handCards.Add(removeCard);
        }
    }
}
