using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionName
{
    InvocationCardOnFieldAlphaManCondition,
    InvocationCardOnFieldAlphaVCondition,
    FieldCardOnFieldAlphaVCondition,
    EquipmentCardOnCardBenzaieCondition,
    EquipmentCardOnCardCanardmanCondition
}

public abstract class Condition
{
    public ConditionName Name { get; set; }
    public string Description { get; set; }

    public abstract bool CanBeSummoned(PlayerCards playerCards, PlayerCards opponentPlayerCards);
}
