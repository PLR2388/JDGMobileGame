using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class MultiplyAtkDefAbility : EquipmentAbility
{
    private float atkFactor;
    private float defenseFactor;
    private bool preventAttack;


    
    public MultiplyAtkDefAbility(EquipmentAbilityName name, string description, float atkFactor = 1f, float defenseFactor = 1f,
        bool preventAttack = false)
    {
        Name = name;
        Description = description;
        this.atkFactor = atkFactor;
        this.defenseFactor = defenseFactor;
        this.preventAttack = preventAttack;
    }

    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.ApplyEffect(invocationCard, playerCards);
        
        invocationCard.Attack *= atkFactor;
        invocationCard.Defense *= defenseFactor;

        if (preventAttack)
        {
            invocationCard.BlockAttack();
        }
    }

    public override void OnTurnStart(InGameInvocationCard invocationCard)
    {
        base.OnTurnStart(invocationCard);

        if (preventAttack)
        {
            invocationCard.BlockAttack();
        }
    }

    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.RemoveEffect(invocationCard, playerCards);
        
        invocationCard.Attack /= atkFactor;
        invocationCard.Defense /= defenseFactor;

        if (preventAttack)
        {
            invocationCard.UnblockAttack();
        }
    }
}

