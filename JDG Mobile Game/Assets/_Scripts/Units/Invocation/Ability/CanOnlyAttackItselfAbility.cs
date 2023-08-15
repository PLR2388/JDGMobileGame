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
        invocationCard.Aggro = true;
    }

    public override void CancelEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.CancelEffect(playerCards, opponentPlayerCards);
        invocationCard.Aggro = false;
    }

    public override void ReactivateEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.ReactivateEffect(playerCards, opponentPlayerCards);
        invocationCard.Aggro = true;
    }
}
