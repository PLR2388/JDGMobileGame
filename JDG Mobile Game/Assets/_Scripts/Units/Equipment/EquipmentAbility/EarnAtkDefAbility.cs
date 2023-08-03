using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class EarnAtkDefAbility : EquipmentAbility
{
    private float defense;
    private float attack;

    private bool handCardNumber;
    
    public EarnAtkDefAbility(EquipmentAbilityName name, string description, float bonusAtk, float bonusDef, bool handCardNumber = false)
    {
        Name = name;
        Description = description;
        defense = bonusDef;
        attack = bonusAtk;
        this.handCardNumber = handCardNumber;
    }

    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.ApplyEffect(invocationCard, playerCards);
        if (handCardNumber)
        {
            invocationCard.Attack += attack * playerCards.handCards.Count;
            invocationCard.Defense += defense * playerCards.handCards.Count;
        }
        else
        {
            invocationCard.Attack += attack;
            invocationCard.Defense += defense;
        }
    }

    public override void OnHandCardsChange(InGameInvocationCard invocationCard, PlayerCards playerCards, int delta)
    {
        base.OnHandCardsChange(invocationCard, playerCards, delta);
        if (handCardNumber)
        {
            invocationCard.Attack += attack * delta;
            invocationCard.Defense += defense * delta;
        }
    }

    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.RemoveEffect(invocationCard, playerCards);
        if (handCardNumber)
        {
            invocationCard.Attack += attack * playerCards.handCards.Count;
            invocationCard.Defense += defense * playerCards.handCards.Count;
        }
        else
        {
            invocationCard.Attack -= attack;
            invocationCard.Defense -= defense;
        }
    }
}
