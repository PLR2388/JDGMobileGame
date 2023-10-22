using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability that allows the player to draw a specified number of cards from the deck.
/// </summary>
public class DrawCardsAbility : Ability
{
    /// <summary>
    /// The number of cards the player should draw when the ability is applied.
    /// </summary>
    private readonly int numberCards;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawCardsAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="numberCards">The number of cards to be drawn.</param>
    public DrawCardsAbility(AbilityName name, string description, int numberCards)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
    }
    
    /// <summary>
    /// Applies the effect of the ability, enabling the player to draw up to the specified number of cards.
    /// If the deck has fewer cards than the specified number, the player draws all remaining cards.
    /// </summary>
    /// <param name="canvas">The game canvas where UI components will be displayed.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        int numberCardsInDeck = playerCards.Deck.Count;
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
                        InGameCard card = playerCards.Deck[^1];
                        playerCards.HandCards.Add(card);
                        playerCards.Deck.RemoveAt(playerCards.Deck.Count - 1);
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