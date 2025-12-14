using JDG.Domain;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Domain.ValueObjects;
using UnityEngine;

/// <summary>
/// Represents an ability to skip the opponent's attack in the game.
/// </summary>
public class SkipOpponentAttackAbility : Ability
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkipOpponentAttackAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    public SkipOpponentAttackAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Displays an OK message box with the specified message.
    /// Phase 38: Use static services instead of singletons.
    /// </summary>
    /// <param name="canvas">The canvas to display the message box on.</param>
    /// <param name="message">The message to display.</param>
    private void DisplayOkMessage(Transform canvas, string message)
    {
        var config = new MessageBoxConfig(
            LocalizationService.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            message,
            showOkButton: true,
            okAction: () =>
            {
            }
        );
        DialogService.ShowMessageBoxLegacy(canvas, config);
    }

    /// <summary>
    /// Applies the effect of skipping the opponent's attack.
    /// Phase 38: Use static services instead of singletons.
    /// </summary>
    /// <param name="canvas">The canvas to display UI elements on.</param>
    /// <param name="opponentPlayerCard">The opponent's player cards.</param>
    private void ApplyEffectInternal(Transform canvas, PlayerCards opponentPlayerCard)
    {
        if (opponentPlayerCard.InvocationCards.Count > 0)
        {
            var config = new MessageBoxConfig(
                LocalizationService.GetLocalizedValue(LocalizationKeys.QUESTION_CHOICE_TITLE),
                LocalizationService.GetLocalizedValue(LocalizationKeys.QUESTION_SKIP_OPPONENT_ATTACK_MESSAGE),
                showPositiveButton: true,
                showNegativeButton: true,
                positiveAction: () =>
                {
                    List<InGameCard> list = new List<InGameCard>(opponentPlayerCard.InvocationCards);
                    var selectorConfig = new CardSelectorConfig(
                        LocalizationService.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SKIP_ATTACK),
                        list,
                        showNegativeButton: true,
                        showPositiveButton: true,
                        positiveAction: (card) =>
                        {
                            if (card is InGameInvocationCard invCard)
                            {
                                invCard.BlockAttack();
                                DisplayOkMessage(
                                    canvas,
                                    string.Format(
                                        LocalizationService.GetLocalizedValue(LocalizationKeys.INFORMATION_OPPONENT_CANT_ATTACK_MESSAGE),
                                        invCard.Title
                                    )
                                );
                            }
                            else
                            {
                                DisplayOkMessage(
                                    canvas,
                                    LocalizationService.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_SKIP_ATTACK_MESSAGE)
                                );
                            }
                        }
                    );
                    DialogService.ShowCardSelectorLegacy(canvas, selectorConfig);
                }
            );
            DialogService.ShowMessageBoxLegacy(canvas, config);
        }
    }

    /// <summary>
    /// Overrides the base ApplyEffect method to apply this ability's effect.
    /// </summary>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyEffectInternal(canvas, opponentPlayerCards);
    }

    /// <summary>
    /// Handles the OnTurnStart event to apply the ability effect at the start of the turn.
    /// </summary>
    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return;
        }

        // Phase 27: Use static GameStateService from Ability base class
        // This ability will be migrated to new IAbility system in a future phase
        var isP1Turn = GameStateService.CurrentPlayer == PlayerId.Player1;

        if (isP1Turn == playerCards.IsPlayerOne)
        {
            ApplyEffectInternal(canvas, opponentPlayerCards);
        }
    }
}