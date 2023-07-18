using UnityEngine;

public class DirectAttackEffectAbility : EffectAbility
{
    private float limitHpOpponent;

    public DirectAttackEffectAbility(EffectAbilityName name, string description, float limitHpOpponent)
    {
        Name = name;
        Description = description;
        this.limitHpOpponent = limitHpOpponent;
    }

    public override bool CanUseEffect(PlayerCards playerCards,PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        return opponentPlayerStatus.GetCurrentPv() < limitHpOpponent;
    }
}
