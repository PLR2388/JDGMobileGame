using _Scripts.Units.Invocation;
using UnityEngine;

public class ProtectBehindDuringAttackAbility : Ability
{

    private string protegeeName;
    private string protectorName;
    
    public ProtectBehindDuringAttackAbility(AbilityName name, string description, string protector, string protegee)
    {
        Name = name;
        Description = description;
        protectorName = protector;
        protegeeName = protegee;
    }

    public override void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus currentPlayerStatus, PlayerStatus opponentPlayerStatus)
    {
        if (attackedCard.Title == protegeeName &&
            opponentPlayerCards.invocationCards.Exists(card => card.Title == protectorName))
        {
            InGameInvocationCard newAttackedCard =
                opponentPlayerCards.invocationCards.Find(card => card.Title == protectorName);
            base.OnCardAttacked(canvas, newAttackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, opponentPlayerStatus);
        }
        else
        {
            base.OnCardAttacked(canvas, attackedCard, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, opponentPlayerStatus);
        }
    }
}
