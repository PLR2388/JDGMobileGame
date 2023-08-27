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
        }
        else if (numberCardsInDeck > 0)
        {
            numberCardToDraw = numberCardsInDeck;
        }

        if (numberCardToDraw > 0)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_DRAW_CARDS_MESSAGE),
                    numberCardToDraw
                ),
                showPositiveButton: true,
                showNegativeButton: true,
                positiveAction: () =>
                {
                    for (int i = 0; i < numberCardToDraw; i++)
                    {
                        InGameCard card = playerCards.deck[playerCards.deck.Count - 1];
                        playerCards.handCards.Add(card);
                        playerCards.deck.RemoveAt(playerCards.deck.Count - 1);
                    }
                }
            );
            MessageBox.Instance.CreateMessageBox(
                canvas,
                config
            );
        }
    }
}