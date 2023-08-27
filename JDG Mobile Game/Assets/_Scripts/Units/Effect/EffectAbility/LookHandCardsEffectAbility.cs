using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LookHandCardsEffectAbility : EffectAbility
{
    public LookHandCardsEffectAbility(EffectAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        return opponentPlayerCard.handCards.Count > 0;
    }

    private void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARD),
            showOkButton: true
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_OPPONENT_CARDS),
            opponentPlayerCard.handCards.ToList(),
            positiveAction: (card) =>
            {
                if (playerCards.handCards.Count > 0)
                {
                    DisplayChoiceAboutHandCards(canvas, playerCards, opponentPlayerCard);
                }
            },
            negativeAction: () =>
            {
                if (playerCards.handCards.Count > 0)
                {
                    DisplayChoiceAboutHandCards(canvas, playerCards, opponentPlayerCard);
                }
            }
        );
        CardSelector.Instance.CreateCardSelection(canvas, config);
    }

    private void DisplayChoiceAboutHandCards(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_REMOVE_CARD_OPPONENT_HAND_MESSAGE),
            showPositiveButton: true,
            showNegativeButton: true,
            positiveAction: () =>
            {
                var config = new CardSelectorConfig(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_OPPONENT_HAND),
                    opponentPlayerCard.handCards.ToList(),
                    showOkButton: true,
                    okAction: (opponentCard) =>
                    {
                        if (opponentCard == null)
                        {
                            DisplayOkMessage(canvas);
                        }
                        else
                        {
                            var config = new CardSelectorConfig(
                                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
                                playerCards.handCards.ToList(),
                                showOkButton: true,
                                positiveAction: (playerCard) =>
                                {
                                    if (playerCard == null)
                                    {
                                        DisplayOkMessage(canvas);
                                    }
                                    else
                                    {
                                        opponentPlayerCard.handCards.Remove(opponentCard);
                                        opponentPlayerCard.yellowCards.Add(opponentCard);
                                        playerCards.handCards.Remove(playerCard);
                                        playerCards.yellowCards.Add(playerCard);
                                    }
                                }
                            );
                            CardSelector.Instance.CreateCardSelection(canvas, config);
                        }
                    }
                );
                CardSelector.Instance.CreateCardSelection(canvas, config);
            },
            negativeAction: () =>
            {

            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }
}