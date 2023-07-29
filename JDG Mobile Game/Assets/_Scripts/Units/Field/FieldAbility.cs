using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public enum FieldAbilityName
{
    Earn1DEFForSpatialFamily
}

public abstract class FieldAbility
{
    public FieldAbilityName Name { get; set; }

    protected string Description { get; set; }

    public virtual void ApplyEffect(Transform canvas, PlayerCards playerCards)
    {
        
    }

    public virtual void OnInvocationCardAdded(InGameInvocationCard invocationCard)
    {
        
    }

    public virtual void OnFieldCardRemoved(PlayerCards playerCards)
    {
        
    }

}