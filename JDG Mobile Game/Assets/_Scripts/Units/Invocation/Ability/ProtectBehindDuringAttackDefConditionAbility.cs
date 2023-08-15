using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public class ProtectBehindDuringAttackDefConditionAbility : Ability
{

    public ProtectBehindDuringAttackDefConditionAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards,
        PlayerStatus currentPlayerStatus, PlayerStatus opponentPlayerStatus)
    {
        IEnumerable<InGameInvocationCard> invocationCards =
            opponentPlayerCards.invocationCards.Where(card => card.Defense > attackedCard.Defense);
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
                // TODO Change
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