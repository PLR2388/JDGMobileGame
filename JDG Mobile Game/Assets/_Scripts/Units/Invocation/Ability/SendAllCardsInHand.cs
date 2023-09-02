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
            playerCards.InvocationCards.Where(card => card.Title != invocationCard.Title).ToList();
        IEnumerable<InGameInvocationCard> opponentInvocationCard =
            opponentPlayerCard.InvocationCards.Where(card => card.Title != invocationCard.Title).ToList();
        foreach (var invocationCard in invocationCards)
        {
            playerCards.InvocationCards.Remove(invocationCard);
            playerCards.HandCards.Add(invocationCard);
        }

        foreach (var invocationCard in opponentInvocationCard)
        {
            opponentPlayerCard.InvocationCards.Remove(invocationCard);
            opponentPlayerCard.HandCards.Add(invocationCard);
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