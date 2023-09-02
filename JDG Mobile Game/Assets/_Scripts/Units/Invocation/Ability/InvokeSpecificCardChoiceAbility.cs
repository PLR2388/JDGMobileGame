using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class InvokeSpecificCardChoiceAbility : Ability
{
    private List<string> cardChoices;

    public InvokeSpecificCardChoiceAbility(AbilityName name, string description, List<string> cardNames)
    {
        Name = name;
        Description = description;
        cardChoices = cardNames;
    }

    protected static void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_INVOKED_CARD_MESSAGE),
            showOkButton: true,
            okAction: () =>
            {
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool hasCardInDeck = playerCards.Deck.Exists(card => cardChoices.Contains(card.Title));
        if (hasCardInDeck)
        {
            string cardNameChoiceString = cardChoices[0];
            for (int i = 1; i < cardChoices.Count; i++)
            {
                cardNameChoiceString += " ou " + cardChoices[i];
            }

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
                            if (card is InGameInvocationCard invocationCard)
                            {
                                playerCards.Deck.Remove(invocationCard);
                                playerCards.InvocationCards.Add(invocationCard);
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