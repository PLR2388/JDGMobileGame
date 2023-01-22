using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class SendAllCardsInHand : Ability
{
    private string exceptedCardName;

    public SendAllCardsInHand(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        exceptedCardName = cardName;
    }

    private void RemoveAllCardExceptOne(PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        List<InGameInvocationCard> invocationCards =
            playerCards.invocationCards.FindAll(card => card.Title != exceptedCardName);
        List<InGameInvocationCard> opponentInvocationCard =
            opponentPlayerCard.invocationCards.FindAll(card => card.Title != exceptedCardName);
        foreach (var invocationCard in invocationCards)
        {
            playerCards.invocationCards.Remove(invocationCard);
            playerCards.handCards.Add(invocationCard);
        }

        foreach (var invocationCard in opponentInvocationCard)
        {
            opponentPlayerCard.invocationCards.Remove(invocationCard);
            opponentPlayerCard.handCards.Add(invocationCard);
        }
    }
    
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        RemoveAllCardExceptOne(playerCards, opponentPlayerCards);
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        RemoveAllCardExceptOne(playerCards, opponentPlayerCards);
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
