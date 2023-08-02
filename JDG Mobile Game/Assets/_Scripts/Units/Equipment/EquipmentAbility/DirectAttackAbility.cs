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

    public override void ApplyEffect(InGameInvocationCard invocationCard)
    {
        base.ApplyEffect(invocationCard);
        invocationCard.CanDirectAttack = true;
    }

    public override void RemoveEffect(InGameInvocationCard invocationCard)
    {
        base.RemoveEffect(invocationCard);
        invocationCard.CanDirectAttack = true;
    }
}
