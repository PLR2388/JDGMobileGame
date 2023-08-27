using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SacrificeToInvokeAbility : Ability
{

    public SacrificeToInvokeAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
        IsAction = true;
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

    public override bool IsActionPossible(PlayerCards playerCards)
    {
        return playerCards.yellowCards.Any(card =>
            card.Type == CardType.Invocation &&
            card.Collector == false);
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameCard> invocationCards = playerCards.yellowCards.TakeWhile(card =>
            card.Type == CardType.Invocation &&
            card.Collector == false).ToList();
        if (invocationCards.Count > 0)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_INVOKE_NON_COLLECTOR_BY_SACRFICE_MESSAGE),
                    invocationCard.Title
                ),
                showPositiveButton: true,
                showNegativeButton: true,
                positiveAction: () =>
                {
                    var config = new CardSelectorConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_INVOKE),
                        invocationCards,
                        showNegativeButton: true,
                        showPositiveButton: true,
                        positiveAction: (selectedCard) =>
                        {
                            if (selectedCard is InGameInvocationCard newlyInvoke)
                            {
                                playerCards.invocationCards.Remove(invocationCard);
                                playerCards.yellowCards.Add(invocationCard);
                                playerCards.yellowCards.Remove(newlyInvoke);
                                playerCards.invocationCards.Add(newlyInvoke);
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

    public override void OnCardActionTouched(Transform canvas, PlayerCards playerCards, PlayerCards opponentCards)
    {
        base.OnCardActionTouched(canvas, playerCards, opponentCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        ApplyEffect(canvas, playerCards, opponentCards);
    }
}