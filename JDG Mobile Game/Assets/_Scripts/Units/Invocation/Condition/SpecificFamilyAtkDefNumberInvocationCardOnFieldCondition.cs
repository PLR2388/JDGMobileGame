using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

public class SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition : Condition
{
    private CardFamily family;
    private float attack;
    private float defense;
    private int numberOfCards;

    public SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(ConditionName name, string description, CardFamily family,
        float attack, float defense, int number)
    {
        Name = name;
        Description = description;
        this.family = family;
        this.attack = attack;
        this.defense = defense;
        numberOfCards = number;
    }

    public SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition(ConditionName name, string description,
        CardFamily family, int number)
    {
        Name = name;
        Description = description;
        this.family = family;
        attack = 0;
        defense = 0;
        numberOfCards = number;
    }
    
    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.invocationCards.FindAll(card =>
                   (card.Attack >= attack || card.Defense >= defense) && card.Families.Contains(family)).Count >=
               numberOfCards;
    }
}
