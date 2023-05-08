using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class DrawCardsAbility : Ability
{
    private int numberCards;
    
    public DrawCardsAbility(AbilityName name, string description, int numberCards)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        int numberCardsInDeck = playerCards.deck.Count;
        int numberCardToDraw = 0;
        if (numberCardsInDeck >= numberCards)
        {
            numberCardToDraw = numberCards;
        } else if (numberCardsInDeck > 0)
        {
            numberCardToDraw = numberCardsInDeck;
        }

        if (numberCardToDraw > 0)
        {
            GameObject messageBox =
                MessageBox.CreateSimpleMessageBox(canvas, "Question", "Veux-tu piocher " + numberCardToDraw + " ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                for (int i = 0; i < numberCardToDraw; i++)
                {
                    InGameCard card = playerCards.deck[playerCards.deck.Count - 1];
                    playerCards.handCards.Add(card);
                    playerCards.deck.RemoveAt(playerCards.deck.Count - 1);
                }
                Object.Destroy(messageBox);
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {   
                Object.Destroy(messageBox);
            };
        }
    }
}
