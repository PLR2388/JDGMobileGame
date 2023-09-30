using System.Collections.Generic;
using Cards;
using UnityEngine;

public class GetTypeCardFromDeckWithoutAttackAbility : Ability
{
    private readonly CardType type;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTypeCardFromDeckWithoutAttackAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cardType">The type of card this ability is concerned with.</param>
    public GetTypeCardFromDeckWithoutAttackAbility(AbilityName name, string description, CardType cardType)
    {
        Name = name;
        Description = description;
        type = cardType;
    }

    /// <summary>
    /// Displays a message box with an OK button indicating no valid card was found.
    /// </summary>
    /// <param name="canvas">The game canvas where the message box will be displayed.</param>
    private static void DisplayOkMessage(Transform canvas)
    {

        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_CARD_GET_FROM_DECK_MESSAGE),
            showOkButton: true
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }

    /// <summary>
    /// Applies the ability effect on the player's cards. Checks if there's a card in the player's deck that matches the specified type.
    /// If found, prompts the user with an option to select it. If not, a message box is displayed.
    /// </summary>
    /// <param name="canvas">The game canvas where UI components will be displayed.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards. (Not used in this method, but included due to override)</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool hasCardInDeck = playerCards.Deck.Exists(card => card.Type == type);
        if (hasCardInDeck)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_GET_TYPE_CARD_MESSAGE),
                    type.ToName()
                ),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    List<InGameCard> cards = playerCards.Deck.FindAll(card => card.Type == type);
                    var config = new CardSelectorConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_DEFAULT_CHOICE_CARD),
                        cards,
                        showNegativeButton: true,
                        showPositiveButton: true,
                        positiveAction: (card) =>
                        {
                            if (card == null)
                            {
                                DisplayOkMessage(canvas);
                            }
                            else
                            {
                                playerCards.HandCards.Add(card);
                                playerCards.Deck.Remove(card);
                                invocationCard.SetRemainedAttackThisTurn(0);
                            }
                        },
                        negativeAction: () =>
                        {
                            DisplayOkMessage(canvas);
                        }
                    );
                    CardSelector.Instance.CreateCardSelection(canvas, config);
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }
}