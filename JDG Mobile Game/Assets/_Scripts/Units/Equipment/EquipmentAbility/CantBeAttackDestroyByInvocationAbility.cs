using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class CantBeAttackDestroyByInvocationAbility : EquipmentAbility
{
    public CantBeAttackDestroyByInvocationAbility(EquipmentAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.ApplyEffect(invocationCard, playerCards);
        invocationCard.CantBeAttack = true;
    }

    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
        invocationCard.CantBeAttack = false;
    }
}
