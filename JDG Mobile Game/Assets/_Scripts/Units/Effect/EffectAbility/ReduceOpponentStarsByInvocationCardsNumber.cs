using UnityEngine;

public class ReduceOpponentStarsByInvocationCardsNumber : EffectAbility
{
    private float damage;
    
    public ReduceOpponentStarsByInvocationCardsNumber(EffectAbilityName name, string description, float unitDamage)
    {
        Name = name;
        Description = description;
        damage = unitDamage;
    }

    public override bool CanUseEffect(PlayerCards playerCards)
    {
        return playerCards.invocationCards.Count > 0;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus, PlayerStatus opponentStatus)
    {
        opponentStatus.ChangePv(-damage * playerCards.invocationCards.Count);
    }
}
