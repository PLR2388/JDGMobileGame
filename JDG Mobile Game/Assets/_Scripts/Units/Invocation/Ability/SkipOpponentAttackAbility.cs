using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SkipOpponentAttackAbility : Ability
{
    public SkipOpponentAttackAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    protected static void DisplayOkMessage(Transform canvas, string message)
    {

        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            message,
            showOkButton: true,
            okAction: () =>
            {
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    private void ApplyEffect(Transform canvas, PlayerCards opponentPlayerCard)
    {
        if (opponentPlayerCard.InvocationCards.Count > 0)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_CHOICE_TITLE),
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_SKIP_OPPONENT_ATTACK_MESSAGE),
                showPositiveButton: true,
                showNegativeButton: true,
                positiveAction: () =>
                {
                    List<InGameCard> list = new List<InGameCard>(opponentPlayerCard.InvocationCards);
                    var config = new CardSelectorConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SKIP_ATTACK),
                        list,
                        showNegativeButton: true,
                        showPositiveButton: true,
                        positiveAction: (card) =>
                        {
                            if (card is InGameInvocationCard invocationCard)
                            {
                                invocationCard.BlockAttack();
                                DisplayOkMessage(
                                    canvas,
                                    string.Format(
                                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_OPPONENT_CANT_ATTACK_MESSAGE),
                                        invocationCard.Title
                                    )
                                );
                            }
                            else
                            {
                                DisplayOkMessage(
                                    canvas,
                                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_SKIP_ATTACK_MESSAGE)
                                );
                            }
                        },
                        negativeAction: () =>
                        {
                        }
                    );
                    CardSelector.Instance.CreateCardSelection(canvas, config);
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }


    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyEffect(canvas, opponentPlayerCards);
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (GameStateManager.Instance.IsP1Turn == playerCards.IsPlayerOne)
        {
            ApplyEffect(canvas, opponentPlayerCards);
        }
    }
}