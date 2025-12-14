using JDG.Domain;
using System.Collections.Generic;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability that provides the condition to destroy a field card if the user agree to loose a part of
/// its card defense or attack
/// </summary>
public class DestroyFieldAtkDefAttackConditionAbility : Ability
{
    /// <summary>
    /// Attack division factor used to determine the condition for destroying the field card.
    /// </summary>
    private readonly int divideAtkFactor;
    
    /// <summary>
    /// Defense division factor used to determine the condition for destroying the field card.
    /// </summary>
    private readonly int divideDefFactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DestroyFieldAtkDefAttackConditionAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="divideAtkFactor">The attack division factor.</param>
    /// <param name="divideDefFactor">The defense division factor.</param>
    public DestroyFieldAtkDefAttackConditionAbility(AbilityName name, string description,
        int divideAtkFactor, int divideDefFactor)
    {
        Name = name;
        Description = description;
        this.divideAtkFactor = divideAtkFactor;
        this.divideDefFactor = divideDefFactor;
    }

    /// <summary>
    /// Displays a message indicating there's no card that was destroyed
    /// Phase 38: Use static services instead of singletons.
    /// </summary>
    /// <param name="canvas">The game canvas where UI components will be displayed.</param>
    private void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationService.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationService.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_DESTROY_CARD_MESSAGE),
            showOkButton: true
        );
        DialogService.ShowMessageBoxLegacy(canvas, config);
    }

    /// <summary>
    /// Destroys the field card and adjusts the attacking card's attributes.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    /// <param name="fieldCard">The field card to be destroyed.</param>
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

    
    /// <summary>
    /// Executes the ability, checking if the conditions for destroying a field card are met.
    /// If met, it offers the option to destroy a field card.
    /// </summary>
    /// <param name="canvas">The game canvas where UI components will be displayed.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    /// <summary>
    /// Phase 38: Use static services instead of singletons.
    /// </summary>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        string condition = divideAtkFactor > 1
            ? LocalizationService.GetLocalizedValue(LocalizationKeys.YOUR_ATTACK)
            : LocalizationService.GetLocalizedValue(LocalizationKeys.YOUR_DEFENSE);
        int value = divideAtkFactor > 1 ? divideAtkFactor : divideDefFactor;

        var config = new MessageBoxConfig(
            LocalizationService.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
            string.Format(
                LocalizationService.GetLocalizedValue(LocalizationKeys.QUESTION_DIVIDE_TO_DESTROY_FIELD_MESSAGE),
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
                    var selectorConfig = new CardSelectorConfig(
                        LocalizationService.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_DESTROY_FIELD_CARD),
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
                    DialogService.ShowCardSelectorLegacy(canvas, selectorConfig);
                }
                else
                {
                    DisplayOkMessage(canvas);
                }
            }
        );
        DialogService.ShowMessageBoxLegacy(canvas, config);
    }
}