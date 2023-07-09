using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectAbilityName
{
    LimitHandCardTo5,
    Lose2Point5StarsByInvocations
}

public abstract class EffectAbility
{
    public EffectAbilityName Name { get; set; }
    protected string Description { get; set; }

    public virtual bool CanUseEffect(PlayerCards playerCards)
    {
        return true;
    }

    public virtual void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus, PlayerStatus opponentStatus)
    {
        
    }
}