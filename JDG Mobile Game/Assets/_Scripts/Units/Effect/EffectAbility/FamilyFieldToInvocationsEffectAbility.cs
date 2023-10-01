using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Ability effect that applies the family of a field card to invocation cards and handles related logic.
/// </summary>
public class FamilyFieldToInvocationsEffectAbility : EffectAbility
{
    private readonly string cardName;
    private readonly float costPerTurn;

    /// <summary>
    /// Initializes a new instance of the <see cref="FamilyFieldToInvocationsEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="costPerTurn">Cost incurred per turn.</param>
    /// <param name="cardName">The name of the card.</param>
    public FamilyFieldToInvocationsEffectAbility(EffectAbilityName name, string description, float costPerTurn, string cardName)
    {
        Name = name;
        Description = description;
        this.costPerTurn = costPerTurn;
        this.cardName = cardName;
    }

    /// <summary>
    /// Applies the power of the effect on the player's cards.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    private void ApplyPower(PlayerCards playerCards)
    {
        var fieldFamily = playerCards.FieldCard.Family;
        foreach (var invocationCard in playerCards.InvocationCards)
        {
            SetNewFamilyToInvocation(invocationCard, fieldFamily);
        }
    }
    
    /// <summary>
    /// Sets a new family to the specified invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card.</param>
    /// <param name="fieldFamily">The new card family.</param>
    private static void SetNewFamilyToInvocation(InGameInvocationCard invocationCard, CardFamily fieldFamily)
    {
        invocationCard.Families = new[]
        {
            fieldFamily
        };
    }

    /// <summary>
    /// Determines whether the effect can be used based on the player's cards and opponent's status.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent player's current cards.</param>
    /// <param name="opponentPlayerStatus">The opponent player's current status.</param>
    /// <returns><c>true</c> if the effect can be used; otherwise, <c>false</c>.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        return playerCards.FieldCard != null && playerCards.InvocationCards.Count > 0;
    }

    /// <summary>
    /// Applies the effect on the player's cards.
    /// </summary>
    /// <param name="canvas">Canvas used for UI operations.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCard">The opponent player's cards.</param>
    /// <param name="playerStatus">The player's current status.</param>
    /// <param name="opponentStatus">The opponent's current status.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        ApplyPower(playerCards);
    }

    /// <summary>
    /// Handles logic at the start of the turn.
    /// </summary>
    /// <param name="canvas">Canvas used for UI operations.</param>
    /// <param name="playerStatus">The player's current status.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerStatus">The opponent player's current status.</param>
    /// <param name="opponentPlayerCards">The opponent player's current cards.</param>
    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards, PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCards)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_TITLE),
            string.Format(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_CONTINUE_APPLY_FAMILY_MESSAGE),
                costPerTurn
            ),
            showPositiveButton: true,
            showNegativeButton: true,
            positiveAction: () =>
            {
                playerStatus.ChangePv(-costPerTurn);
            },
            negativeAction: () =>
            {
                ResetInvocationsFamily(playerCards);
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }
    
    /// <summary>
    /// Resets the family of invocation cards and moves the effect card back to the yellow pile.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    private void ResetInvocationsFamily(PlayerCards playerCards)
    {
        foreach (var invocationCard in playerCards.InvocationCards)
        {
            invocationCard.Families = invocationCard.BaseInvocationCard.BaseInvocationCardStats.Families;
        }

        var effectCard = playerCards.EffectCards.First(effectCard => effectCard.Title == cardName);
        playerCards.EffectCards.Remove(effectCard);
        playerCards.YellowCards.Add(effectCard);
    }

    /// <summary>
    /// Handles logic when an invocation card is added.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="invocationCard">The added invocation card.</param>
    public override void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        base.OnInvocationCardAdded(playerCards, invocationCard);
        SetNewFamilyToInvocation(invocationCard, playerCards.FieldCard.Family);
    }

    /// <summary>
    /// Handles logic when an invocation card is removed.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="invocationCard">The removed invocation card.</param>
    public override void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        base.OnInvocationCardRemoved(playerCards, invocationCard);
        invocationCard.Families = invocationCard.BaseInvocationCard.BaseInvocationCardStats.Families;
    }
}