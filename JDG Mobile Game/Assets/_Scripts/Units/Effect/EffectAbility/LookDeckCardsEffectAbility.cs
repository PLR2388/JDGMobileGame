using System.Collections.Generic;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an effect ability that allows a player to look at the deck cards and rearrange them if he wants.
/// </summary>
public class LookDeckCardsEffectAbility : EffectAbility
{
    private readonly int numberCards;

    /// <summary>
    /// Initializes a new instance of the <see cref="LookDeckCardsEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="numberCards">The number of cards the player can look at.</param>
    public LookDeckCardsEffectAbility(EffectAbilityName name, string description, int numberCards)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
    }

    /// <summary>
    /// Determines if the effect can be used based on the deck card count.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <param name="opponentPlayerStatus">The opponent's status.</param>
    /// <returns><c>true</c> if there are deck cards; otherwise, <c>false</c>.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        return playerCards.Deck.Count > 0 || opponentPlayerCard.Deck.Count > 0;
    }

    /// <summary>
    /// Applies the effect, allowing the player to view the deck cards.
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

        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_CHOICE_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_CHOICE_SEE_DECK_MESSAGE),
            showNegativeButton: true,
            showPositiveButton: true,
            positiveAction: () =>
            {
                DisplayAndOrderCardMessageBox(canvas, playerCards);
            },
            negativeAction: () =>
            {
                DisplayAndOrderCardMessageBox(canvas, opponentPlayerCard);
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    /// <summary>
    /// Displays and handles ordering of deck cards for the player.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The player's cards to display and potentially rearrange.</param>
    private void DisplayAndOrderCardMessageBox(Transform canvas, PlayerCards playerCards)
    {
        var deck = playerCards.Deck;
        if (deck.Count >= numberCards)
        {
            var shortList = new List<InGameCard>();
            for (var i = 0; i < numberCards; i++)
            {
                shortList.Add(deck[deck.Count - 1 - i]);
            }

            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHANGE_ORDER_CARTES),
                shortList,
                numberCardSelection: numberCards,
                showOrder: true,
                showPositiveButton: true,
                showNegativeButton: true,
                positiveMultipleAction: (selectedCards) =>
                {
                    if (selectedCards.Count == numberCards)
                    {
                        RearrangeCards(playerCards, selectedCards);
                    }
                    else
                    {
                        DisplayWarningOrderMessage(canvas);
                    }
                }
            );

            CardSelector.Instance.CreateCardSelection(
                canvas, config);
        }
        else
        {
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHANGE_ORDER_CARTES),
                deck,
                numberCardSelection: deck.Count,
                showOrder: true,
                showNegativeButton: true,
                showPositiveButton: true,
                positiveMultipleAction: (selectedCards) =>
                {
                    if (selectedCards.Count == deck.Count)
                    {
                        RearrangeCards(playerCards, selectedCards);
                    }
                    else
                    {
                        DisplayWarningOrderMessage(canvas);
                    }
                }
            );

            CardSelector.Instance.CreateCardSelection(
                canvas,
                config
            );
        }
    }
    
    /// <summary>
    /// Displays a warning message when cards are not ordered.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    private static void DisplayWarningOrderMessage(Transform canvas)
    {

        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_ORDER_CARDS),
            showOkButton: true
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }
    
    /// <summary>
    /// Rearranges the order of the player's deck based on selected cards.
    /// </summary>
    /// <param name="playerCards">The player's cards to rearrange.</param>
    /// <param name="selectedCards">The selected cards in the desired order.</param>
    private static void RearrangeCards(PlayerCards playerCards, List<InGameCard> selectedCards)
    {
        foreach (var selectedCard in selectedCards)
        {
            playerCards.Deck.Remove(selectedCard);
        }

        for (var i = selectedCards.Count - 1; i >= 0; i--)
        {
            playerCards.Deck.Add(selectedCards[i]);
        }
    }
}