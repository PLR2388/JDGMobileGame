using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability to retrieve a specific card from the deck.
/// </summary>
public class GetSpecificCardFromDeckAbility : Ability
{
    /// <summary>
    /// The specific card's name this ability is concerned with.
    /// </summary>
    protected readonly string CardName;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSpecificCardFromDeckAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cardName">The name of the specific card this ability is concerned with.</param>
    public GetSpecificCardFromDeckAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        CardName = cardName;
    }

    /// <summary>
    /// Attempts to retrieve a specific card from the player's deck. If the card exists in the deck,
    /// it is removed from the deck and added to the player's hand. Otherwise, no action is taken.
    /// </summary>
    /// <param name="canvas">The game canvas where UI components will be displayed.</param>
    /// <param name="playerCards">The player's current cards.</param>
    protected void GetSpecificCard(Transform canvas, PlayerCards playerCards)
    {
        bool hasCardInDeck = playerCards.Deck.Exists(card => card.Title == CardName);
        if (hasCardInDeck)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_GET_SPECIFIC_CARD_IN_DECK_MESSAGE),
                    CardName
                ),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    InGameCard card = playerCards.Deck.Find(card => card.Title == CardName);
                    playerCards.Deck.Remove(card);
                    playerCards.HandCards.Add(card);
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }

    /// <summary>
    /// Applies the ability effect on the player's cards. Attempts to retrieve the specified card from the deck.
    /// </summary>
    /// <param name="canvas">The game canvas where UI components will be displayed.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards. (Not used in this method, but included due to override)</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        GetSpecificCard(canvas, playerCards);
    }

}