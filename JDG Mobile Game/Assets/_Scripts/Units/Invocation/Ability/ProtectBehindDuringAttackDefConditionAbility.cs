using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an ability that protects a card positioned behind during an attack, 
/// based on certain defensive conditions.
/// </summary>
public class ProtectBehindDuringAttackDefConditionAbility : Ability
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtectBehindDuringAttackDefConditionAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    public ProtectBehindDuringAttackDefConditionAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Triggered when a card is attacked. Determines whether the ability should protect
    /// another card positioned behind based on the defensive condition.
    /// </summary>
    /// <param name="canvas">The canvas to display any UI elements on.</param>
    /// <param name="attackedCard">The card being attacked.</param>
    /// <param name="attacker">The card initiating the attack.</param>
    /// <param name="playerCards">The collection of cards of the player being attacked.</param>
    /// <param name="opponentPlayerCards">The collection of cards of the attacking player.</param>
    /// <param name="currentPlayerStatus">The status of the player being attacked.</param>
    /// <param name="opponentPlayerStatus">The status of the attacking player.</param>
    public override void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards,
        PlayerStatus currentPlayerStatus, PlayerStatus opponentPlayerStatus)
    {
        if (invocationCard.CancelEffect)
        {
            base.OnCardAttacked(canvas, attackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus,
                opponentPlayerStatus);
        }
        IEnumerable<InGameInvocationCard> invocationCards =
            opponentPlayerCards.InvocationCards.Where(card => card.Defense > attackedCard.Defense);
        var inGameInvocationCards = invocationCards.ToList();
        if (attackedCard.Title == invocationCard.Title && inGameInvocationCards.Any())
        {
            if (inGameInvocationCards.Count() == 1)
            {
                InGameInvocationCard newAttackedCard = inGameInvocationCards[0];
                base.OnCardAttacked(canvas, newAttackedCard, attacker, playerCards, opponentPlayerCards,
                    currentPlayerStatus, opponentPlayerStatus);
            }
            else
            {
                // Random choice for the moment
                InGameInvocationCard newAttackedCard = inGameInvocationCards[Random.Range(0, inGameInvocationCards.Count)];
                base.OnCardAttacked(canvas, newAttackedCard, attacker, playerCards, opponentPlayerCards,
                    currentPlayerStatus, opponentPlayerStatus);
            }
        }
        else
        {
            base.OnCardAttacked(canvas, attackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus,
                opponentPlayerStatus);
        }
    }
}