using System;

public class DrawMoreCardsAbility : FieldAbility
{

    private int numberAdditionalCards;

    public DrawMoreCardsAbility(FieldAbilityName name, string description, int numberAdditionalCard)
    {
        Name = name;
        Description = description;
        numberAdditionalCards = numberAdditionalCard;
    }

    public override void OnTurnStart(PlayerCards playerCards, PlayerStatus playerStatus)
    {
        base.OnTurnStart(playerCards, playerStatus);
        var size = playerCards.deck.Count;
        if (size > 0)
        {
            var min = Math.Min(size, numberAdditionalCards);
            for (var i = size - 1; i > size - 1 - min; i--)
            {
                var c = playerCards.deck[i];
                playerCards.handCards.Add(c);
                playerCards.deck.RemoveAt(i);
            }
        }
    }
}
