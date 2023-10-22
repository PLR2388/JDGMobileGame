using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an effect ability that allows a player to regain health.
/// </summary>
public class GetHPBackEffectAbility : EffectAbility
{
    private readonly int numberInvocationToSacrifice;
    private readonly float atkDefCondition;
    private readonly float hpToRecover;

    private const float MaxHealthRecovery = 0f;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GetHPBackEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="numberInvocations">The number of invocations required.</param>
    /// <param name="atkDefCondition">The attack/defense condition threshold.</param>
    /// <param name="hpToRecover">The amount of health to recover.</param>
    public GetHPBackEffectAbility(EffectAbilityName name, string description, int numberInvocations,
        float atkDefCondition, float hpToRecover)
    {
        Name = name;
        Description = description;
        numberInvocationToSacrifice = numberInvocations;
        this.atkDefCondition = atkDefCondition;
        this.hpToRecover = hpToRecover;
    }

    /// <summary>
    /// Determines if the effect can be used based on player cards and opponent's status.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent player's cards.</param>
    /// <param name="opponentPlayerStatus">The opponent player's status.</param>
    /// <returns>True if the effect can be used; otherwise, false.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        return playerCards.InvocationCards.Count(card =>
            card.Attack >= atkDefCondition || card.Defense >= atkDefCondition) >= numberInvocationToSacrifice;
    }

    /// <summary>
    /// Performs the action of sacrificing an invocation card.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="invocationCard">The invocation card to be sacrificed.</param>
    private void SacrificeInvocationCard(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        playerCards.YellowCards.Add(invocationCard);
        playerCards.InvocationCards.Remove(invocationCard);
    }

    /// <summary>
    /// Recovers the player's health.
    /// </summary>
    /// <param name="playerStatus">The player's status.</param>
    private void RecoverPlayerHealth(PlayerStatus playerStatus)
    {
        float amountToRecover = hpToRecover == MaxHealthRecovery ? PlayerStatus.MaxHealth : hpToRecover;
        playerStatus.ChangePv(amountToRecover);
    }
    
    /// <summary>
    /// Handles the logic for performing a single sacrifice.
    /// </summary>
    /// <param name="canvas">The UI canvas.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="playerStatus">The player's status.</param>
    private void PerformSingleSacrifice(Transform canvas, PlayerCards playerCards, PlayerStatus playerStatus)
    {
        var invocationCards = new List<InGameCard>(playerCards.InvocationCards
            .Where(card => card.Attack >= atkDefCondition || card.Defense >= atkDefCondition).ToList());
        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SACRIFICE),
            invocationCards,
            showOkButton: true,
            okAction: (card) =>
            {
                if (card is InGameInvocationCard invocationCard)
                {
                    SacrificeInvocationCard(playerCards, invocationCard);
                    RecoverPlayerHealth(playerStatus);
                }
                else
                {
                    var config = new MessageBoxConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_SACRIFICE),
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
    /// Applies the effect of this ability.
    /// </summary>
    /// <param name="canvas">The UI canvas.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent player's card.</param>
    /// <param name="playerStatus">The player's status.</param>
    /// <param name="opponentStatus">The opponent player's status.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        if (numberInvocationToSacrifice == 0)
        {
            RecoverPlayerHealth(playerStatus);
            return;
        }
        if (numberInvocationToSacrifice == 1)
        {
            PerformSingleSacrifice(canvas, playerCards, playerStatus);
        }
    }
}