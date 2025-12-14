using JDG.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability that allows a player to destroy an opponent's Invocation Card.
/// </summary>
public class KillOpponentInvocationCardAbility : Ability
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KillOpponentInvocationCardAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    public KillOpponentInvocationCardAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Displays an informational message box indicating that there is no card available to destroy.
    /// Phase 38: Use static services instead of singletons.
    /// </summary>
    /// <param name="canvas">The canvas transform.</param>
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
    /// Applies the effect of the ability, allowing the player to destroy an opponent's Invocation Card if available.
    /// Phase 38: Use static services instead of singletons.
    /// </summary>
    /// <param name="canvas">The canvas transform.</param>
    /// <param name="playerCards">The collection of the current player's cards.</param>
    /// <param name="opponentPlayerCards">The collection of the opponent player's cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ObservableCollection<InGameInvocationCard> invocationCards = opponentPlayerCards.InvocationCards;
        if (invocationCards.Count > 0)
        {
            var config = new MessageBoxConfig(
                LocalizationService.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                LocalizationService.GetLocalizedValue(LocalizationKeys.QUESTION_DESTROY_OPPONENT_INVOCATION_MESSAGE),
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
                        var selectorConfig = new CardSelectorConfig(
                            LocalizationService.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_DESTROY_CARD),
                            new List<InGameCard>(invocationCards),
                            showNegativeButton: true,
                            showPositiveButton: true,
                            positiveAction: card =>
                            {
                                if (card is InGameInvocationCard inGameInvocationCard)
                                {
                                    opponentPlayerCards.InvocationCards.Remove(inGameInvocationCard);
                                    opponentPlayerCards.YellowCards.Add(inGameInvocationCard);
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
                        DialogService.ShowCardSelectorLegacy(canvas, selectorConfig);
                    }
                }
            );
            DialogService.ShowMessageBoxLegacy(canvas, config);
        }
    }
}