using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class ProtectBehindDuringAttackDefConditionAbility : Ability
{
    private string protegee;

    public ProtectBehindDuringAttackDefConditionAbility(AbilityName name, string description, string protegee)
    {
        Name = name;
        Description = description;
        this.protegee = protegee;
    }

    public override void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards,
        PlayerStatus currentPlayerStatus, PlayerStatus opponentPlayerStatus)
    {
        List<InGameInvocationCard> invocationCards =
            opponentPlayerCards.invocationCards.FindAll(card => card.Defense > attackedCard.Defense);
        if (attackedCard.Title == protegee && invocationCards.Count > 0)
        {
            if (invocationCards.Count == 1)
            {
                InGameInvocationCard newAttackedCard = invocationCards[0];
                base.OnCardAttacked(canvas, newAttackedCard, attacker, playerCards, opponentPlayerCards,
                    currentPlayerStatus, opponentPlayerStatus);
            }
            else
            {
                // Random choice for the moment
                // TODO Change
                InGameInvocationCard newAttackedCard = invocationCards[Random.Range(0, invocationCards.Count)];
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