using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class SetAtkDefAbility : EquipmentAbility
{
    private float? atk;
    private float? def;

    private float previousAtk;
    private float previousDef;

    public SetAtkDefAbility(EquipmentAbilityName name, string description, float? atk = null, float? def = null)
    {
        Name = name;
        Description = description;
        this.atk = atk;
        this.def = def;
    }

    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.ApplyEffect(invocationCard, playerCards);
        previousAtk = invocationCard.Attack;
        previousDef = invocationCard.Defense;
        if (atk != null)
        {
            invocationCard.Attack = (float)atk;
        }

        if (def != null)
        {
            invocationCard.Defense = (float)def;
        }
    }

    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
        if (atk != null)
        {
            invocationCard.Attack = previousAtk;
        }

        if (def != null)
        {
            invocationCard.Defense = previousDef;
        }
    }
}
