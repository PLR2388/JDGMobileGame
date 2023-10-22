using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
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
    /// </summary>
    /// <param name="canvas">The canvas to display the message box on.</param>
    /// <param name="message">The message to display.</param>
    private static void DisplayOkMessage(Transform canvas, string message)
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

    /// <summary>
    /// Applies the effect of skipping the opponent's attack.
    /// </summary>
    /// <param name="canvas">The canvas to display UI elements on.</param>
    /// <param name="opponentPlayerCard">The opponent's player cards.</param>
    private static void ApplyEffect(Transform canvas, PlayerCards opponentPlayerCard)
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
                        }
                    );
                    CardSelector.Instance.CreateCardSelection(canvas, config);
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }

    /// <summary>
    /// Overrides the base ApplyEffect method to apply this ability's effect.
    /// </summary>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyEffect(canvas, opponentPlayerCards);
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
        if (GameStateManager.Instance.IsP1Turn == playerCards.IsPlayerOne)
        {
            ApplyEffect(canvas, opponentPlayerCards);
        }
    }
}