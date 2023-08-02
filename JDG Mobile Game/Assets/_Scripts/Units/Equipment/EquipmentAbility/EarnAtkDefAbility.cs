using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class EarnAtkDefAbility : EquipmentAbility
{
    private float defense;
    private float attack;
    
    public EarnAtkDefAbility(EquipmentAbilityName name, string description, float bonusAtk, float bonusDef)
    {
        Name = name;
        Description = description;
        defense = bonusDef;
        attack = bonusAtk;
    }

    public override void ApplyEffect(InGameInvocationCard invocationCard)
    {
        base.ApplyEffect(invocationCard);
        invocationCard.Attack += attack;
        invocationCard.Defense += defense;
    }

    public override void RemoveEffect(InGameInvocationCard invocationCard)
    {
        base.RemoveEffect(invocationCard);
        invocationCard.Attack -= attack;
        invocationCard.Defense -= defense;
    }
}
