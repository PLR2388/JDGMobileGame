using _Scripts.Units.Invocation;
using UnityEngine;


/// <summary>
/// Represents an ability that destroys both the attacker and attacked cards when specific conditions are met.
/// </summary>
public class KillBothCardsIfAttackAbility : Ability
{

    /// <summary>
    /// Initializes a new instance of the <see cref="KillBothCardsIfAttackAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    public KillBothCardsIfAttackAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Handles the scenario when a card attacks another.
    /// </summary>
    /// <param name="attackedCard">The card that is attacked.</param>
    /// <param name="attacker">The card that is performing the attack.</param>
    /// <param name="playerCards">The collection of the current player's cards.</param>
    /// <param name="opponentPlayerCards">The collection of the opponent player's cards.</param>
    public override void OnAttackCard(InGameInvocationCard attackedCard, InGameInvocationCard attacker, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (attacker.Title == invocationCard.Title)
        {
            if (playerCards.YellowCards.Contains(attacker) && !opponentPlayerCards.YellowCards.Contains(attackedCard))
            {
                opponentPlayerCards.InvocationCards.Remove(attackedCard);
                opponentPlayerCards.YellowCards.Add(attackedCard);
            }
        }
    }

    /// <summary>
    /// Handles the scenario when a card is attacked by another.
    /// </summary>
    /// <param name="canvas">The canvas transform.</param>
    /// <param name="attackedCard">The card that is attacked.</param>
    /// <param name="attacker">The card that is performing the attack.</param>
    /// <param name="playerCards">The collection of the current player's cards.</param>
    /// <param name="opponentPlayerCards">The collection of the opponent player's cards.</param>
    /// <param name="currentPlayerStatus">The status of the current player.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    public override void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus currentPlayerStatus, PlayerStatus opponentPlayerStatus)
    {
        if (invocationCard.CancelEffect)
        {
            base.OnCardAttacked(canvas, attackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, opponentPlayerStatus);
        }
        if (attackedCard.Title == invocationCard.Title)
        {
            base.OnCardAttacked(canvas, attackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, opponentPlayerStatus);
            if (opponentPlayerCards.YellowCards.Contains(attackedCard) && !playerCards.YellowCards.Contains(attacker))
            {
                playerCards.InvocationCards.Remove(attacker);
                playerCards.YellowCards.Add(attacker);
            }
        }
        else
        {
            base.OnCardAttacked(canvas, attackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, opponentPlayerStatus);
        }
    }
}
