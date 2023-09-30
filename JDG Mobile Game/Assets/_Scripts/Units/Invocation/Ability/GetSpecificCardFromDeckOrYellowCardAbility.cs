using System.Linq;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability to get a specific card either from the deck or from the yellow cards.
/// </summary>
public class GetSpecificCardFromDeckOrYellowCardAbility : GetSpecificCardFromDeckAbility
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSpecificCardFromDeckOrYellowCardAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cardName">The name of the specific card this ability is concerned with.</param>
    public GetSpecificCardFromDeckOrYellowCardAbility(AbilityName name, string description, string cardName) : base(
        name, description, cardName)
    {
    }

    /// <summary>
    /// Applies the ability effect on the player's cards. Checks if the specified card is in the player's deck or yellow cards.
    /// If found, the card is added to the player's hand. Otherwise, no action is taken.
    /// </summary>
    /// <param name="canvas">The game canvas where UI components will be displayed.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards. (Not used in this method, but included due to override)</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool hasCardInDeck = playerCards.Deck.Exists(card => card.Title == CardName);
        bool hasCardInYellowTrash = playerCards.YellowCards.Any(card => card.Title == CardName);
        if (hasCardInDeck || hasCardInYellowTrash)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_GET_SPECIFIC_CARD_IN_DECK_AND_YELLOW_MESSAGE),
                    CardName
                ),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    if (hasCardInDeck)
                    {
                        InGameCard card = playerCards.Deck.Find(card => card.Title == CardName);
                        playerCards.Deck.Remove(card);
                        playerCards.HandCards.Add(card);
                    }
                    else
                    {
                        InGameCard card = playerCards.YellowCards.First(card => card.Title == CardName);
                        playerCards.Deck.Remove(card);
                        playerCards.HandCards.Add(card);
                    }
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }
}