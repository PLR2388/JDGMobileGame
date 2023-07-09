using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectAbilityName
{
    LimitHandCardTo5
}

public abstract class EffectAbility
{
    public EffectAbilityName Name { get; set; }
    protected string Description { get; set; }

    public virtual void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        
    }
}