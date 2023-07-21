using _Scripts.Units.Invocation;
using UnityEngine;

public class IncrementNumberAttackEffectAbility : EffectAbility
{
    private int numberAttackPerTurn;
    public IncrementNumberAttackEffectAbility(EffectAbilityName name, string description, int numberAttackPerTurn)
    {
        Name = name;
        Description = description;
        this.numberAttackPerTurn = numberAttackPerTurn;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        foreach (var invocationCard in playerCards.invocationCards)
        {
            invocationCard.SetRemainedAttackThisTurn(numberAttackPerTurn);
        }
    }

    public override void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        base.OnInvocationCardAdded(playerCards, invocationCard);
        invocationCard.SetRemainedAttackThisTurn(numberAttackPerTurn);
    }

    public override void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        base.OnInvocationCardRemoved(playerCards, invocationCard);
        invocationCard.SetRemainedAttackThisTurn(1);
    }
}
