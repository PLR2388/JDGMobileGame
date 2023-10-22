using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an ability that provides protection to a specific card during an attack.
/// </summary>
public class ProtectBehindDuringAttackAbility : Ability
{
    
    private readonly string protectorName;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtectBehindDuringAttackAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="protector">The name of the protector card.</param>
    public ProtectBehindDuringAttackAbility(AbilityName name, string description, string protector)
    {
        Name = name;
        Description = description;
        protectorName = protector;
    }

    /// <summary>
    /// Handles the behavior when a card is attacked.
    /// If the invocation card effect is not canceled and there exists a protector card,
    /// the attack is redirected to the protector card.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="attackedCard">The card being attacked.</param>
    /// <param name="attacker">The attacking card.</param>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    /// <param name="currentPlayerStatus">The status of the current player.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    public override void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus currentPlayerStatus, PlayerStatus opponentPlayerStatus)
    {
        if (invocationCard.CancelEffect)
        {
            base.OnCardAttacked(canvas, attackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, opponentPlayerStatus);
            return;
        }
        if (attackedCard.Title == invocationCard.Title &&
            opponentPlayerCards.InvocationCards.Any(card => card.Title == protectorName))
        {
            InGameInvocationCard newAttackedCard =
                opponentPlayerCards.InvocationCards.First(card => card.Title == protectorName);
            base.OnCardAttacked(canvas, newAttackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, opponentPlayerStatus);
        }
        else
        {
            base.OnCardAttacked(canvas, attackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, opponentPlayerStatus);
        }
    }
}
