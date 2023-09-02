using System.Collections.Generic;
using Cards;
using JetBrains.Annotations;
using UnityEngine;

public class DestroyFieldAtkDefAttackConditionAbility : Ability
{
    private int divideAtkFactor;
    private int divideDefFactor;

    public DestroyFieldAtkDefAttackConditionAbility(AbilityName name, string description,
        int divideAtkFactor, int divideDefFactor)
    {
        Name = name;
        Description = description;
        this.divideAtkFactor = divideAtkFactor;
        this.divideDefFactor = divideDefFactor;
    }

    private static void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_DESTROY_CARD_MESSAGE),
            showOkButton: true,
            okAction: () =>
            {
            }
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }

    private void DestroyField(PlayerCards playerCards, PlayerCards opponentPlayerCards, InGameCard fieldCard)
    {
        if (playerCards.FieldCard == fieldCard)
        {
            playerCards.YellowCards.Add(fieldCard);
            playerCards.FieldCard = null;
        }
        else
        {
            opponentPlayerCards.YellowCards.Add(fieldCard);
            opponentPlayerCards.FieldCard = null;
        }

        invocationCard.Attack /= divideAtkFactor;
        invocationCard.Defense /= divideDefFactor;
        invocationCard.SetRemainedAttackThisTurn(0);
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        string condition = divideAtkFactor > 1
            ? LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.YOUR_ATTACK)
            : LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.YOUR_DEFENSE);
        int value = divideAtkFactor > 1 ? divideAtkFactor : divideDefFactor;

        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
            string.Format(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_DIVIDE_TO_DESTROY_FIELD_MESSAGE),
                condition, value
            ),
            showPositiveButton: true,
            showNegativeButton: true,
            positiveAction: () =>
            {
                List<InGameCard> fieldCardsToDestroy = new List<InGameCard>();
                if (playerCards.FieldCard != null)
                {
                    fieldCardsToDestroy.Add(playerCards.FieldCard);
                }

                if (opponentPlayerCards.FieldCard != null)
                {
                    fieldCardsToDestroy.Add(opponentPlayerCards.FieldCard);
                }

                if (fieldCardsToDestroy.Count == 1)
                {
                    DestroyField(playerCards, opponentPlayerCards, fieldCardsToDestroy[0]);
                }
                else if (fieldCardsToDestroy.Count > 1)
                {
                    var config = new CardSelectorConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_DESTROY_FIELD_CARD),
                        fieldCardsToDestroy,
                        showNegativeButton: true,
                        showPositiveButton: true,
                        positiveAction: (fieldCard) =>
                        {
                            if (fieldCard == null)
                            {
                                DisplayOkMessage(canvas);
                            }
                            else
                            {
                                DestroyField(playerCards, opponentPlayerCards, fieldCard);
                            }
                        },
                        negativeAction: () =>
                        {
                            DisplayOkMessage(canvas);
                        }
                        );
                    CardSelector.Instance.CreateCardSelection(canvas, config);
                }
                else
                {
                    DisplayOkMessage(canvas);
                }
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }
}