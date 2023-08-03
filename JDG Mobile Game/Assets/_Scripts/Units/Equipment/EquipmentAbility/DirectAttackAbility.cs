using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class DirectAttackAbility : EquipmentAbility
{

    public DirectAttackAbility(EquipmentAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.ApplyEffect(invocationCard, playerCards);
        invocationCard.CanDirectAttack = true;
    }

    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.RemoveEffect(invocationCard, playerCards);
        invocationCard.CanDirectAttack = true;
    }
}
