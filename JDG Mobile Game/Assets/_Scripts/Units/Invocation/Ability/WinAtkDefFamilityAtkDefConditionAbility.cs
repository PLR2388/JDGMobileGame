using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability that applies attack and defense boosts to cards in a specified family,
/// based on additional attack and defense conditions.
/// </summary>
public class WinAtkDefFamilityAtkDefConditionAbility : WinAtkDefFamilyAbility
{
    private readonly float invocationAtkCondition;
    private readonly float invocationDefCondition;

    /// <summary>
    /// Initializes a new instance of the <see cref="WinAtkDefFamilityAtkDefConditionAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="family">The card family to which the ability is applied.</param>
    /// <param name="atk">The amount of attack to be added.</param>
    /// <param name="def">The amount of defense to be added.</param>
    /// <param name="invocationAtk">The attack condition for the ability to be applied.</param>
    /// <param name="invocationDef">The defense condition for the ability to be applied.</param>
    public WinAtkDefFamilityAtkDefConditionAbility(AbilityName name, string description,
        CardFamily family, float atk, float def, float invocationAtk, float invocationDef) : base(name, description,family, atk, def)
    {
        invocationAtkCondition = invocationAtk;
        invocationDefCondition = invocationDef;
    }

    /// <summary>
    /// Applies the effect of the ability to the player's cards.
    /// </summary>
    /// <param name="canvas">The transform of the canvas.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent's cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyPower(playerCards);
    }

    /// <summary>
    /// Calculates the number of invocation cards that meet the specified conditions.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <returns>The number of invocation cards meeting the conditions.</returns>
    private int CalculateNumberInvocation(PlayerCards playerCards)
    {
        return playerCards.InvocationCards
            .Count(CheckIfCardMeetCondition);
    }

    /// <summary>
    /// Applies the power boost to the invocation card based on the number of cards meeting the condition.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    private void ApplyPower(PlayerCards playerCards)
    {
        int numberCardInvocation = CalculateNumberInvocation(playerCards);
        IncrementAtkDefInvocationCard(numberCardInvocation);
    }

    /// <summary>
    /// Reactivates the effect of the ability on the player's cards.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        ApplyPower(playerCards);
    }

    /// <summary>
    /// Cancels the effect of the ability and reverts any changes made to the invocation card.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        int numberCardInvocation = CalculateNumberInvocation(playerCards);
        DecrementAtkDefInvocationCard(numberCardInvocation);
    }

    /// <summary>
    /// Checks if a card meets the specified conditions for applying the ability.
    /// </summary>
    /// <param name="card">The invocation card to check.</param>
    /// <returns><c>true</c> if the card meets the conditions; otherwise, <c>false</c>.</returns>
    private bool CheckIfCardMeetCondition(InGameInvocationCard card)
    {
        return card.Title != invocationCard.Title && card.Families.Contains(Family) &&
               card.Attack == invocationAtkCondition && card.Defense == invocationDefCondition;
    }

    /// <summary>
    /// Handles the addition of a new card and applies the ability effect if the card meets the conditions.
    /// </summary>
    /// <param name="newCard">The newly added invocation card.</param>
    /// <param name="playerCards">The player's cards.</param>
    public override void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
        base.OnCardAdded(newCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (CheckIfCardMeetCondition(newCard))
        {
            IncrementAtkDefInvocationCard(1);
        }
    }

    /// <summary>
    /// Handles the removal of a card and reverts the ability effect if the card meets the conditions.
    /// </summary>
    /// <param name="removeCard">The removed invocation card.</param>
    /// <param name="playerCards">The player's cards.</param>
    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (CheckIfCardMeetCondition(removeCard))
        {
            DecrementAtkDefInvocationCard(1);
        }
    }
}