using System.Linq;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an effect ability that allows changing the Field card.
/// </summary>
public class ChangeFieldCardEffectAbility : EffectAbility
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeFieldCardEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="numberTurn">The number of turns for which the effect lasts.</param>
    public ChangeFieldCardEffectAbility(EffectAbilityName name, string description, int numberTurn)
    {
        Name = name;
        Description = description;
        NumberOfTurn = numberTurn;
    }

    /// <summary>
    /// Determines if the effect can be used based on the player's card deck.
    /// </summary>
    /// <param name="playerCards">The cards held by the player.</param>
    /// <param name="opponentPlayerCards">The cards held by the opponent.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    /// <returns>True if the effect can be used; otherwise, false.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        return playerCards.Deck.Any(card => card.Type == CardType.Field);
    }

    /// <summary>
    /// Applies the effect of changing the Field card.
    /// </summary>
    /// <param name="canvas">Transform reference for positioning UI elements.</param>
    /// <param name="playerCards">The cards held by the player.</param>
    /// <param name="opponentPlayerCard">The cards held by the opponent.</param>
    /// <param name="playerStatus">The current status of the player.</param>
    /// <param name="opponentStatus">The current status of the opponent.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        var fieldCards = playerCards.Deck.Where(card => card.Type == CardType.Field).ToList();
        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_FIELD),
            fieldCards,
            showOkButton: true,
            okAction: (card) =>
            {
                if (card is InGameFieldCard fieldCard)
                {
                    if (playerCards.FieldCard == null)
                    {
                        playerCards.FieldCard = fieldCard;
                    }
                    else
                    {
                        playerCards.YellowCards.Add(playerCards.FieldCard);
                        playerCards.FieldCard = fieldCard;
                    }
                }
                else
                {
                    var config = new MessageBoxConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_FIELD_CARD),
                        showOkButton: true
                    );
                    MessageBox.Instance.CreateMessageBox(
                        canvas,
                        config
                    );
                }
            }
        );
        CardSelector.Instance.CreateCardSelection(canvas, config);
    }
}