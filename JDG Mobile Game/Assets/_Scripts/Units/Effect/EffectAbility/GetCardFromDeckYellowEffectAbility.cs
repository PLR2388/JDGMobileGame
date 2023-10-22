using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

/// <summary>
/// Represents the source from which cards are drawn.
/// </summary>
public enum CardSource
{
    Deck,
    Yellow,
    Both
}

/// <summary>
/// Defines the ability to get cards either from the deck, yellow pile, or both.
/// </summary>
public class GetCardFromDeckYellowEffectAbility : EffectAbility
{
    private readonly CardSource cardSource;
    private readonly int numberCards;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCardFromDeckYellowEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="numberCards">The number of cards.</param>
    /// <param name="source">The source of the cards (Deck/Yellow/Both).</param>
    public GetCardFromDeckYellowEffectAbility(EffectAbilityName name, string description, int numberCards, CardSource source)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
        cardSource = source;
    }

    /// <summary>
    /// Retrieves the list of cards based on the selected card source.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <returns>Returns a list of cards from the specified source.</returns>
    private IEnumerable<InGameCard> GetCardSourceList(PlayerCards playerCards)
    {
        switch (cardSource)
        {
            case CardSource.Deck:
                return playerCards.Deck;
            case CardSource.Yellow:
                return playerCards.YellowCards.ToList();
            case CardSource.Both:
                return playerCards.Deck.Concat(playerCards.YellowCards);
            default:
                throw new ArgumentOutOfRangeException(nameof(cardSource), "Invalid Card Source");
        }
    }
    
    /// <summary>
    /// Handles the selection of cards.
    /// </summary>
    /// <param name="canvas">The canvas on which the UI is displayed.</param>
    /// <param name="playerCards">The player's cards.</param>
    private void HandleCardSelection(Transform canvas, PlayerCards playerCards)
    {
        var cards = GetCardSourceList(playerCards);

        if (numberCards == 1)
        {
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_CARD_FROM_DECK_YELLOW),
                cards.ToList(),
                showOkButton: true,
                okAction: (card) =>
                {
                    if (card == null)
                    {
                        DisplayWarningMessageBox(canvas);
                    }
                    else
                    {
                        MoveCardToPlayerHand(card, playerCards);
                    }
                }
            );
            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
    }

    /// <summary>
    /// Displays a warning message box.
    /// </summary>
    /// <param name="canvas">The canvas on which the UI is displayed.</param>
    private void DisplayWarningMessageBox(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARD),
            showOkButton: true
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    /// <summary>
    /// Moves the selected card to the player's hand.
    /// </summary>
    /// <param name="card">The card to move.</param>
    /// <param name="playerCards">The player's cards.</param>
    private void MoveCardToPlayerHand(InGameCard card, PlayerCards playerCards)
    {
        switch (cardSource)
        {
            case CardSource.Deck:
                playerCards.Deck.Remove(card);
                break;
            case CardSource.Yellow:
                playerCards.YellowCards.Remove(card);
                break;
            case CardSource.Both:
                if (playerCards.Deck.Contains(card))
                {
                    playerCards.Deck.Remove(card);
                }
                else
                {
                    playerCards.YellowCards.Remove(card);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        playerCards.HandCards.Add(card);
    }

    /// <summary>
    /// Checks if the effect can be used.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent player's cards.</param>
    /// <param name="opponentPlayerStatus">The opponent player's status.</param>
    /// <returns>True if the effect can be used; otherwise, false.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        return GetCardSourceList(playerCards).Count() > numberCards;
    }

    /// <summary>
    /// Applies the card selection effect.
    /// </summary>
    /// <param name="canvas">The canvas on which the UI is displayed.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent player's cards.</param>
    /// <param name="playerStatus">The player's status.</param>
    /// <param name="opponentStatus">The opponent player's status.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        HandleCardSelection(canvas, playerCards);
    }
}