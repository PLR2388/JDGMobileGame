using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public class SendAllCardsInHand : Ability
{

    public SendAllCardsInHand(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    private void RemoveAllCardExceptOne(PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        IEnumerable<InGameInvocationCard> invocationCards =
            playerCards.invocationCards.Where(card => card.Title != invocationCard.Title).ToList();
        IEnumerable<InGameInvocationCard> opponentInvocationCard =
            opponentPlayerCard.invocationCards.Where(card => card.Title != invocationCard.Title).ToList();
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
        if (invocationCard.CancelEffect)
        {
            return;
        }
        RemoveAllCardExceptOne(playerCards, opponentPlayerCards);
    }
}