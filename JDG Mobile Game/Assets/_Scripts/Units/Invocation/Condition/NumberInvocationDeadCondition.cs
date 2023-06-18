using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

public class NumberInvocationDeadCondition : Condition
{
    private int numberDeath;

    public NumberInvocationDeadCondition(ConditionName name, string description, int numberDeath)
    {
        Name = name;
        Description = description;
        this.numberDeath = numberDeath;
    }

    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.yellowCards.Count(card => card.Type == CardType.Invocation) >= numberDeath;
    }
}
