using System.Collections.Generic;
using Cards;
using UnityEngine;

public class OptionalChangeFieldFromDeckAbility : Ability
{
    public OptionalChangeFieldFromDeckAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    private static void DisplayOkMessageBox(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_FIELD_CARD_SET_MESSAGE),
            showOkButton: true,
            okAction: () =>
            {
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameCard> fieldCardsFromDeck = playerCards.deck.FindAll(card => card.Type == CardType.Field);
        if (fieldCardsFromDeck.Count > 0)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_SET_FIELD_CARD_MESSAGE),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    var config = new CardSelectorConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SET_FIELD),
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
                                    playerCards.yellowCards.Add(playerCards.FieldCard);
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
                    CardSelector.Instance.CreateCardSelection(canvas, config);
                },
                negativeAction: () =>
                {
                    DisplayOkMessageBox(canvas);
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }
}