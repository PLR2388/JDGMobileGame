using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability to invoke a specific card chosen from the deck.
/// </summary>
public class InvokeSpecificCardChoiceAbility : Ability
{
    private readonly List<string> cardChoices;

    /// <summary>
    /// Initializes a new instance of the InvokeSpecificCardChoiceAbility class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cardNames">The list of card names that can be chosen for invocation.</param>
    public InvokeSpecificCardChoiceAbility(AbilityName name, string description, List<string> cardNames)
    {
        Name = name;
        Description = description;
        cardChoices = cardNames;
    }

    /// <summary>
    /// Displays a message box with information that no card was invoked.
    /// </summary>
    /// <param name="canvas">The canvas on which to display the message box.</param>
    private static void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_INVOKED_CARD_MESSAGE),
            showOkButton: true
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    /// <summary>
    /// Applies the effect of this ability.
    /// </summary>
    /// <param name="canvas">The canvas to display any UI elements.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent player's cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool hasCardInDeck = playerCards.Deck.Exists(card => cardChoices.Contains(card.Title));
        if (hasCardInDeck)
        {
            string cardNameChoiceString = string.Join(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_OR_OPTION), cardChoices);

            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_INVOKE_SPECIFIC_CARD_MESSAGE)
                    , cardNameChoiceString
                ),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    List<InGameCard> cards =
                        playerCards.Deck.FindAll(card => cardChoices.Contains(card.Title));
                    var config = new CardSelectorConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_INVOKE),
                        cards,
                        showNegativeButton: true,
                        showPositiveButton: true,
                        positiveAction: (card) =>
                        {
                            if (card is InGameInvocationCard inGameInvocationCard)
                            {
                                playerCards.Deck.Remove(inGameInvocationCard);
                                playerCards.InvocationCards.Add(inGameInvocationCard);
                            }
                            else
                            {
                                DisplayOkMessage(canvas);
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