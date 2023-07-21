using _Scripts.Units.Invocation;
using UnityEngine;

public class SwitchAtkDefEffectAbility : EffectAbility
{
    public SwitchAtkDefEffectAbility(EffectAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        foreach (var invocationCard in playerCards.invocationCards)
        {
            (invocationCard.Attack, invocationCard.Defense) = (invocationCard.Defense, invocationCard.Attack);
        }
    }

    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards,
        PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCards)
    {
        base.OnTurnStart(canvas, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCards);
        foreach (var invocationCard in playerCards.invocationCards)
        {
            (invocationCard.Attack, invocationCard.Defense) = (invocationCard.Defense, invocationCard.Attack);
        }
    }
    
    
    public override void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        (invocationCard.Attack, invocationCard.Defense) = (invocationCard.Defense, invocationCard.Attack);
    }

    public override void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        (invocationCard.Attack, invocationCard.Defense) = (invocationCard.Defense, invocationCard.Attack);
    }
}
