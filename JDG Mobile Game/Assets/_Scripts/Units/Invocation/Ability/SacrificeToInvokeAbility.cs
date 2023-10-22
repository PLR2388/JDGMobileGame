using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability allowing a player to sacrifice a card to invoke another.
/// </summary>
public class SacrificeToInvokeAbility : Ability
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SacrificeToInvokeAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    public SacrificeToInvokeAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
        IsAction = true;
    }

    /// <summary>
    /// Displays a message indicating that no cards were invoked.
    /// </summary>
    /// <param name="canvas">The canvas to display the message on.</param>
    private static void DisplayOkMessage(Transform canvas)
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

    /// <summary>
    /// Checks if the action is possible with the given player's cards.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <returns>True if action is possible, false otherwise.</returns>
    public override bool IsActionPossible(PlayerCards playerCards)
    {
        return playerCards.YellowCards.Any(card =>
            card.Type == CardType.Invocation &&
            card.Collector == false);
    }

    /// <summary>
    /// Applies the effect of the ability.
    /// </summary>
    /// <param name="canvas">The canvas to display any UI elements on.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent's player cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameCard> invocationCards = playerCards.YellowCards.TakeWhile(card =>
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
                                playerCards.InvocationCards.Remove(invocationCard);
                                playerCards.YellowCards.Add(invocationCard);
                                playerCards.YellowCards.Remove(newlyInvoke);
                                playerCards.InvocationCards.Add(newlyInvoke);
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

    /// <summary>
    /// Handles the touch action on the card and applies the effect if conditions are met.
    /// </summary>
    /// <param name="canvas">The canvas to display any UI elements on.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentCards">The opponent's player cards.</param>
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