using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public enum EffectAbilityName
{
    LimitHandCardTo5,
    Lose2Point5StarsByInvocations,
    ApplyFamilyFieldToInvocations,
    DestroyAllCardsUnderManyConditions
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

    public virtual void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards)
    {
        
    }

    public virtual void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        
    }

    public virtual void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        
    }
}