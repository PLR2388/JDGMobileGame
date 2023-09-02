using System;
using UnityEngine;

public class DrawMoreCardsAbility : FieldAbility
{

    private int numberAdditionalCards;

    public DrawMoreCardsAbility(FieldAbilityName name, string description, int numberAdditionalCard)
    {
        Name = name;
        Description = description;
        numberAdditionalCards = numberAdditionalCard;
    }

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
