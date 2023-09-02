using System.Collections.Generic;
using System.Collections.ObjectModel;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class KillOpponentInvocationCardAbility : Ability
{
    public KillOpponentInvocationCardAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    protected static void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_DESTROY_CARD_MESSAGE),
            showOkButton: true,
            okAction: () =>
            {
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ObservableCollection<InGameInvocationCard> invocationCards = opponentPlayerCards.InvocationCards;
        if (invocationCards.Count > 0)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_DESTROY_OPPONENT_INVOCATION_MESSAGE),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    if (invocationCards.Count == 1)
                    {
                        opponentPlayerCards.InvocationCards.Remove(invocationCards[0]);
                        opponentPlayerCards.YellowCards.Add(invocationCards[0]);
                    }
                    else
                    {
                        var config = new CardSelectorConfig(
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_DESTROY_CARD),
                            new List<InGameCard>(invocationCards),
                            showNegativeButton: true,
                            showPositiveButton: true,
                            positiveAction: (card) =>
                            {
                                if (card is InGameInvocationCard invocationCard)
                                {
                                    opponentPlayerCards.InvocationCards.Remove(invocationCard);
                                    opponentPlayerCards.YellowCards.Add(invocationCard);
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
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }
}