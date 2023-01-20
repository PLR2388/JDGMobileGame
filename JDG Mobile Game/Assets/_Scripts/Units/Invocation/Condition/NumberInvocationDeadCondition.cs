using System.Collections;
using System.Collections.Generic;
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
        return playerCards.yellowTrash.FindAll(card => card.Type == CardType.Invocation).Count >= numberDeath;
    }
}
