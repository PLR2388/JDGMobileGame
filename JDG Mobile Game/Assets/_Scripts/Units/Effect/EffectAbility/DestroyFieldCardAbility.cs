using System.Collections.Generic;
using Cards;
using UnityEngine;

/// <summary>
/// Represents the ability to destroy field cards.
/// </summary>
public class DestroyFieldCardAbility : EffectAbility
{
    private readonly float costHealth;

    /// <summary>
    /// Initializes a new instance of the <see cref="DestroyFieldCardAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cost">The health cost for using the ability.</param>
    public DestroyFieldCardAbility(EffectAbilityName name, string description, float cost)
    {
        Name = name;
        Description = description;
        costHealth = cost;
    }

    /// <summary>
    /// Applies the effect of the ability.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    /// <param name="playerStatus">The status of the player.</param>
    /// <param name="opponentStatus">The status of the opponent player.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var cards = CollectFieldCards(playerCards, opponentPlayerCard);

        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_DESTROY_FIELD_CARD),
            cards,
            showOkButton: true,
            okAction: (card) =>
            {
                HandleSelectedCard(canvas, playerCards, opponentPlayerCard, playerStatus, card);
            }
        );
        CardSelector.Instance.CreateCardSelection(canvas, config);
    }
    
    /// <summary>
    /// Handles the selected card.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    /// <param name="playerStatus">The status of the player.</param>
    /// <param name="card">The selected card to handle.</param>
    private void HandleSelectedCard(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus, InGameCard card)
    {

        if (card == null)
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
        else
        {
            if (card.CardOwner == CardOwner.Player1)
            {
                DestroyField(playerCards, opponentPlayerCard, card);
            }
            else
            {
                DestroyField(opponentPlayerCard, playerCards, card);
            }
            playerStatus.ChangePv(-costHealth);
        }
    }
    
    /// <summary>
    /// Destroys a field based on the selected card's owner.
    /// </summary>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    /// <param name="card">The card to destroy.</param>
    private static void DestroyField(PlayerCards playerCards, PlayerCards opponentPlayerCard, InGameCard card)
    {

        if (playerCards.IsPlayerOne)
        {
            playerCards.YellowCards.Add(card);
            playerCards.FieldCard = null;
        }
        else
        {
            opponentPlayerCard.YellowCards.Add(card);
            opponentPlayerCard.FieldCard = null;
        }
    }
    
    /// <summary>
    /// Collects the field cards from both players.
    /// </summary>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    /// <returns>A list of field cards from both players.</returns>
    private static List<InGameCard> CollectFieldCards(PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        var cards = new List<InGameCard>();
        if (playerCards.FieldCard != null)
        {
            cards.Add(playerCards.FieldCard);
        }

        if (opponentPlayerCard.FieldCard != null)
        {
            cards.Add(opponentPlayerCard.FieldCard);
        }
        return cards;
    }

    /// <summary>
    /// Determines if the effect can be used.
    /// </summary>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    /// <returns>True if the effect can be used, false otherwise.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards,
        PlayerStatus opponentPlayerStatus)
    {
        return playerCards.FieldCard != null || opponentPlayerCards.FieldCard != null;
    }
}