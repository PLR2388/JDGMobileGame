using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public class ProtectBehindDuringAttackAbility : Ability
{
    
    private string protectorName;
    
    public ProtectBehindDuringAttackAbility(AbilityName name, string description, string protector)
    {
        Name = name;
        Description = description;
        protectorName = protector;
    }

    public override void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus currentPlayerStatus, PlayerStatus opponentPlayerStatus)
    {
        if (invocationCard.CancelEffect)
        {
            base.OnCardAttacked(canvas, attackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, opponentPlayerStatus);
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
