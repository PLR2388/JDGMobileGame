using System.Linq;
using UnityEngine;

public class CanOnlyAttackItselfAbility : Ability
{
    public CanOnlyAttackItselfAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        playerCards.invocationCards.First(card => card.Title == invocationCard.Title).Aggro = true;
    }
}
