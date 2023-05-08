
using _Scripts.Units.Invocation;
using UnityEngine;

public class CanOnlyAttackItselfAbility : Ability
{
    private readonly string cardName;
    
    public CanOnlyAttackItselfAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        playerCards.invocationCards.Find(card => card.Title == cardName).Aggro = true;
    }
}
