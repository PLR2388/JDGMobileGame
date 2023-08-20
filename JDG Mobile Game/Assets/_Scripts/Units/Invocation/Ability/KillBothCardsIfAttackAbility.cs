using _Scripts.Units.Invocation;
using UnityEngine;

public class KillBothCardsIfAttackAbility : Ability
{

    public KillBothCardsIfAttackAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override void OnAttackCard(InGameInvocationCard attackedCard, InGameInvocationCard attacker, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (attacker.Title == invocationCard.Title)
        {
            if (playerCards.yellowCards.Contains(attacker) && !opponentPlayerCards.yellowCards.Contains(attackedCard))
            {
                opponentPlayerCards.invocationCards.Remove(attackedCard);
                opponentPlayerCards.yellowCards.Add(attackedCard);
            }
        }
    }

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
            if (opponentPlayerCards.yellowCards.Contains(attackedCard) && !playerCards.yellowCards.Contains(attacker))
            {
                playerCards.invocationCards.Remove(attacker);
                playerCards.yellowCards.Add(attacker);
            }
        }
        else
        {
            base.OnCardAttacked(canvas, attackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, opponentPlayerStatus);
        }
    }
}
