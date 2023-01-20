using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCardOnFieldCondition : Condition
{
    public FieldCardOnFieldCondition(ConditionName name, string description, string fieldName)
    {
        Name = name;
        Description = description;
        this.fieldName = fieldName;
    }

    private readonly string fieldName;
    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.field != null && playerCards.field.Title == fieldName;
    }
}
