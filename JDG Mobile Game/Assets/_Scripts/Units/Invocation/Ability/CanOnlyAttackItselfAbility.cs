using UnityEngine;

/// <summary>
/// Represents an ability that forces the associated card to only attack itself.
/// </summary>
public class CanOnlyAttackItselfAbility : Ability
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CanOnlyAttackItselfAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    public CanOnlyAttackItselfAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Applies the effect of the ability, setting the card's aggro status to true.
    /// </summary>
    /// <param name="canvas">The UI canvas.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    /// <param name="opponentPlayerCards">The opponent's collection of cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        invocationCard.Aggro = true;
    }

    /// <summary>
    /// Cancels the ability effect, setting the card's aggro status to false.
    /// </summary>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        invocationCard.Aggro = false;
    }

    /// <summary>
    /// Reactivates the ability effect, setting the card's aggro status to true.
    /// </summary>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        invocationCard.Aggro = true;
    }
}
