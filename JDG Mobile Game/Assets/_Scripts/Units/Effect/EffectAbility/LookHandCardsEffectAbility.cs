using System.Linq;
using UnityEngine;

/// <summary>
/// Represents an effect ability that allows a player to look at the opponent's hand cards.
/// </summary>
public class LookHandCardsEffectAbility : EffectAbility
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookHandCardsEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    public LookHandCardsEffectAbility(EffectAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Determines if the effect can be used based on the opponent's hand card count.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <param name="opponentPlayerStatus">The opponent's status.</param>
    /// <returns><c>true</c> if the opponent has hand cards; otherwise, <c>false</c>.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        return opponentPlayerCard.HandCards.Count > 0;
    }

    /// <summary>
    /// Displays a message box with an OK button.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
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

    /// <summary>
    /// Applies the effect, allowing the player to view the opponent's hand cards.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <param name="playerStatus">The player's status.</param>
    /// <param name="opponentStatus">The opponent's status.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_OPPONENT_CARDS),
            opponentPlayerCard.HandCards.ToList(),
            showOkButton: true,
            numberCardSelection: 0,
            okAction: _ =>
            {
                if (playerCards.HandCards.Count > 0)
                {
                    DisplayChoiceAboutHandCards(canvas, playerCards, opponentPlayerCard);
                }
            }
        );
        CardSelector.Instance.CreateCardSelection(canvas, config);
    }

    /// <summary>
    /// Displays a choice for the player regarding hand cards: If he wants to remove one card from the opponent
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
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
                    opponentPlayerCard.HandCards.ToList(),
                    showOkButton: true,
                    okAction: opponentCard =>
                    {
                        if (opponentCard == null)
                        {
                            DisplayOkMessage(canvas);
                        }
                        else
                        {
                            var config = new CardSelectorConfig(
                                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
                                playerCards.HandCards.ToList(),
                                showOkButton: true,
                                okAction: (playerCard) =>
                                {
                                    if (playerCard == null)
                                    {
                                        DisplayOkMessage(canvas);
                                    }
                                    else
                                    {
                                        opponentPlayerCard.HandCards.Remove(opponentCard);
                                        opponentPlayerCard.YellowCards.Add(opponentCard);
                                        playerCards.HandCards.Remove(playerCard);
                                        playerCards.YellowCards.Add(playerCard);
                                    }
                                }
                            );
                            CardSelector.Instance.CreateCardSelection(canvas, config);
                        }
                    }
                );
                CardSelector.Instance.CreateCardSelection(canvas, config);
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }
}