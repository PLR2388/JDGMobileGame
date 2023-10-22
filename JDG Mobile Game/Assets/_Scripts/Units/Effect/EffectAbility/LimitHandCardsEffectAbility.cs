using System.Linq;
using UnityEngine;

/// <summary>
/// Represents the effect ability to limit the number of cards in a player's hand.
/// </summary>
public class LimitHandCardsEffectAbility : EffectAbility
{
    private readonly int numberCards;

    /// <summary>
    /// Initializes a new instance of the <see cref="LimitHandCardsEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="numberCards">The maximum number of cards allowed in hand.</param>
    public LimitHandCardsEffectAbility(EffectAbilityName name, string description, int numberCards)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
    }

    /// <summary>
    /// Applies the hand card limit effect on both player's cards.
    /// </summary>
    /// <param name="canvas">The UI canvas to display messages.</param>
    /// <param name="playerCards">The current player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <param name="playerStatus">The current player's status.</param>
    /// <param name="opponentPlayerStatus">The opponent player's status.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus, PlayerStatus opponentPlayerStatus)
    {
        ApplyHandCardLimit(canvas, playerCards);
        ApplyHandCardLimit(canvas, opponentPlayerCard);
    }

    /// <summary>
    /// Checks and applies the hand card limit for the given player cards.
    /// </summary>
    /// <param name="canvas">The UI canvas to display messages.</param>
    /// <param name="playerCards">The player's cards to check and adjust.</param>
    private void ApplyHandCardLimit(Transform canvas, PlayerCards playerCards)
    {
        var numberCardPlayerCard = playerCards.HandCards.Count;
        if (numberCardPlayerCard < numberCards)
        {
            DrawCardsUntilLimit(playerCards, numberCardPlayerCard);
        }
        else if (numberCardPlayerCard > numberCards)
        {
            PromptToRemoveExtraCards(canvas, playerCards, numberCardPlayerCard);
        }
    }

    /// <summary>
    /// Draws cards until the hand card limit is reached.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="currentCardCount">The current number of cards in hand.</param>
    private void DrawCardsUntilLimit(PlayerCards playerCards, int currentCardCount)
    {
        var cardsToDraw = playerCards.Deck
            .TakeLast(numberCards - currentCardCount)
            .ToList();

        foreach (var inGameCard in cardsToDraw) playerCards.HandCards.Add(inGameCard);
        foreach (var card in cardsToDraw)
        {
            playerCards.Deck.Remove(card);
        }
    }

    /// <summary>
    /// Prompts the user to remove extra cards exceeding the hand card limit.
    /// </summary>
    /// <param name="canvas">The UI canvas to display messages.</param>
    /// <param name="playerCards">The player's cards to check and adjust.</param>
    /// <param name="currentCardCount">The current number of cards in hand.</param>
    private void PromptToRemoveExtraCards(Transform canvas, PlayerCards playerCards, int currentCardCount)
    {
        int extraCards = currentCardCount - numberCards;

        var config = new CardSelectorConfig(
            string.Format(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
                playerCards.IsPlayerOne ? "1" : "2",
                extraCards),
            playerCards.HandCards.ToList(),
            numberCardSelection: extraCards,
            showOkButton: true,
            okMultipleAction: (cards) =>
            {
                if (cards.Count == extraCards)
                {
                    foreach (var inGameCard in cards)
                    {
                        playerCards.YellowCards.Add(inGameCard);
                        playerCards.HandCards.Remove(inGameCard);
                    }
                }
                else
                {
                    var warningConfig = new MessageBoxConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                        string.Format(
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARDS),
                            extraCards
                        ),
                        showOkButton: true
                    );
                    MessageBox.Instance.CreateMessageBox(canvas, warningConfig);
                }
            }
        );
        CardSelector.Instance.CreateCardSelection(canvas, config);
    }
}