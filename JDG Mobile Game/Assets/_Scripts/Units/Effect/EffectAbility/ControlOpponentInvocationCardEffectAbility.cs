using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an effect ability that allows control of an opponent's Invocation card.
/// </summary>
public class ControlOpponentInvocationCardEffectAbility : EffectAbility
{
    private string invocationControlled;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlOpponentInvocationCardEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    public ControlOpponentInvocationCardEffectAbility(EffectAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Checks if the effect can be used based on the current state of the player's and opponent's cards.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <param name="opponentPlayerStatus">The opponent's current status.</param>
    /// <returns>True if the effect can be used; otherwise, false.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        return opponentPlayerCard.InvocationCards.Count > 0;
    }

    /// <summary>
    /// Applies the effect of controlling an opponent's Invocation card.
    /// </summary>
    /// <param name="canvas">Transform reference for positioning UI elements.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <param name="playerStatus">The player's current status.</param>
    /// <param name="opponentStatus">The opponent's current status.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_CONTROLLED_INVOCATION),
            new List<InGameCard>(opponentPlayerCard.InvocationCards.ToList()),
            showOkButton: true,
            okAction: (card) =>
            {
                if (card is InGameInvocationCard invocationCard)
                {
                    invocationControlled = invocationCard.Title;
                    invocationCard.ControlCard();
                    invocationCard.UnblockAttack();
                    opponentPlayerCard.InvocationCards.Remove(invocationCard);
                    playerCards.InvocationCards.Add(invocationCard);
                }
                else
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
            }
        );
        CardSelector.Instance.CreateCardSelection(canvas, config);
    }

    /// <summary>
    /// Executed at the start of a turn to handle any changes to controlled Invocation cards.
    /// </summary>
    /// <param name="canvas">Transform reference for positioning UI elements.</param>
    /// <param name="playerStatus">The player's current status.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerStatus">The opponent's current status.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards,
        PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCard)
    {
        base.OnTurnStart(canvas, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCard);

        var invocationCard =
            playerCards.InvocationCards.FirstOrDefault(elt => elt.Title == invocationControlled);
        if (invocationCard != null)
        {
            invocationCard.UnblockAttack();
            invocationCard.FreeCard();
            playerCards.InvocationCards.Remove(invocationCard);
            opponentPlayerCard.InvocationCards.Add(invocationCard);
        }
    }
}