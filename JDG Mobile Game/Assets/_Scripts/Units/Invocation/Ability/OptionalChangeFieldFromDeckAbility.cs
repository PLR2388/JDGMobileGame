using JDG.Domain;
using System.Collections.Generic;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability that allows for an optional change of the field card from the deck.
/// </summary>
public class OptionalChangeFieldFromDeckAbility : Ability
{
    /// <summary>
    /// Represents an ability that allows for an optional change of the field card from the deck.
    /// </summary>
    public OptionalChangeFieldFromDeckAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Displays an OK message box with a message indicating no field card is set.
    /// Phase 38: Use static services instead of singletons.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    private void DisplayOkMessageBox(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationService.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationService.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_FIELD_CARD_SET_MESSAGE),
            showOkButton: true
        );
        DialogService.ShowMessageBoxLegacy(canvas, config);
    }

    /// <summary>
    /// Applies the effect of the ability, offering an option to change the field card from the deck,
    /// if any field cards are available in the deck.
    /// Phase 38: Use static services instead of singletons.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameCard> fieldCardsFromDeck = playerCards.Deck.FindAll(card => card.Type == Cards.CardType.Field);
        if (fieldCardsFromDeck.Count > 0)
        {
            var config = new MessageBoxConfig(
                LocalizationService.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                LocalizationService.GetLocalizedValue(LocalizationKeys.QUESTION_SET_FIELD_CARD_MESSAGE),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    var selectorConfig = new CardSelectorConfig(
                        LocalizationService.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SET_FIELD),
                        fieldCardsFromDeck,
                        showNegativeButton: true,
                        showPositiveButton: true,
                        positiveAction: (card) =>
                        {
                            if (card is InGameFieldCard fieldCard)
                            {
                                if (playerCards.FieldCard == null)
                                {
                                    playerCards.FieldCard = fieldCard;
                                }
                                else
                                {
                                    playerCards.YellowCards.Add(playerCards.FieldCard);
                                    playerCards.FieldCard = fieldCard;
                                }
                            }
                            else
                            {
                                DisplayOkMessageBox(canvas);
                            }
                        },
                        negativeAction: () =>
                        {
                            DisplayOkMessageBox(canvas);
                        }
                    );
                    DialogService.ShowCardSelectorLegacy(canvas, selectorConfig);
                },
                negativeAction: () =>
                {
                    DisplayOkMessageBox(canvas);
                }
            );
            DialogService.ShowMessageBoxLegacy(canvas, config);
        }
    }
}