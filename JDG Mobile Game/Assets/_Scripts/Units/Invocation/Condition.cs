using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionName
{
    InvocationCardOnFieldCondition
}

public abstract class Condition
{
    public ConditionName Name { get; set; }
    public string Description { get; set; }

    public abstract bool CanBeSummoned(PlayerCards playerCards, PlayerCards opponentPlayerCards);
}
