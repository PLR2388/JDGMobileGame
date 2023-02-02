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

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
  
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
     
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
    
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
      
    }

    protected override void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
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
