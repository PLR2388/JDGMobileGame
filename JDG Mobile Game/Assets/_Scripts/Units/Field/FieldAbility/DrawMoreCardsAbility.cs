using System;
using UnityEngine;

/// <summary>
/// Represents a field ability that allows the player to draw more cards at the start of their turn.
/// </summary>
public class DrawMoreCardsAbility : FieldAbility
{

    private readonly int numberAdditionalCards;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawMoreCardsAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the field ability.</param>
    /// <param name="description">The description of the field ability.</param>
    /// <param name="numberAdditionalCard">The number of additional cards to draw.</param>
    public DrawMoreCardsAbility(FieldAbilityName name, string description, int numberAdditionalCard)
    {
        Name = name;
        Description = description;
        numberAdditionalCards = numberAdditionalCard;
    }

    /// <summary>
    /// Invoked at the start of the turn, allowing the player to draw additional cards if available in their deck.
    /// </summary>
    /// <param name="canvas">The UI canvas where gameplay elements are displayed.</param>
    /// <param name="playerCards">The player's current set of cards.</param>
    /// <param name="playerStatus">The current status of the player.</param>
    public override void OnTurnStart(Transform canvas,PlayerCards playerCards, PlayerStatus playerStatus)
    {
        base.OnTurnStart(canvas, playerCards, playerStatus);
        var size = playerCards.Deck.Count;
        if (size > 0)
        {
            var min = Math.Min(size, numberAdditionalCards);
            for (var i = size - 1; i > size - 1 - min; i--)
            {
                var c = playerCards.Deck[i];
                playerCards.HandCards.Add(c);
                playerCards.Deck.RemoveAt(i);
            }
        }
    }
}
