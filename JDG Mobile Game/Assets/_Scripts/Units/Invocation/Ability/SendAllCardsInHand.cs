using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an ability that sends all cards in the hand back to the players.
/// </summary>
public class SendAllCardsInHand : Ability
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SendAllCardsInHand"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    public SendAllCardsInHand(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Gets the cards to be removed from the invocation cards list.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <returns>An IEnumerable of <see cref="InGameInvocationCard"/> to be removed.</returns>
    private IEnumerable<InGameInvocationCard> GetRemovedCard(PlayerCards playerCards)
    {
        return playerCards.InvocationCards.Where(card => card.Title != invocationCard.Title).ToList();
    }

    /// <summary>
    /// Removes all invocation cards except one and adds them to the hand cards.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's player cards.</param>
    private void RemoveAllCardExceptOne(PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        IEnumerable<InGameInvocationCard> invocationCards = GetRemovedCard(playerCards);
        IEnumerable<InGameInvocationCard> opponentInvocationCard = GetRemovedCard(opponentPlayerCard);
        foreach (var inGameInvocationCard in invocationCards)
        {
            playerCards.InvocationCards.Remove(inGameInvocationCard);
            playerCards.HandCards.Add(inGameInvocationCard);
        }

        foreach (var inGameInvocationCard in opponentInvocationCard)
        {
            opponentPlayerCard.InvocationCards.Remove(inGameInvocationCard);
            opponentPlayerCard.HandCards.Add(inGameInvocationCard);
        }
    }

    /// <summary>
    /// Applies the effect of the ability.
    /// </summary>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        RemoveAllCardExceptOne(playerCards, opponentPlayerCards);
    }

    /// <summary>
    /// Applies the effect of the ability at the start of each turn.
    /// </summary>
    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return;
        }
        RemoveAllCardExceptOne(playerCards, opponentPlayerCards);
    }
}