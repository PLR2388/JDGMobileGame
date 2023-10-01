using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an effect ability to increment the number of attacks for invocation cards.
/// </summary>
public class IncrementNumberAttackEffectAbility : EffectAbility
{
    private readonly int numberAttackPerTurn;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="IncrementNumberAttackEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="numberAttackPerTurn">The number of attacks to be incremented per turn.</param>
    public IncrementNumberAttackEffectAbility(EffectAbilityName name, string description, int numberAttackPerTurn)
    {
        Name = name;
        Description = description;
        this.numberAttackPerTurn = numberAttackPerTurn;
    }

    /// <summary>
    /// Applies the effect to increment the number of attacks for invocation cards.
    /// </summary>
    /// <param name="canvas">The current UI canvas.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent.</param>
    /// <param name="playerStatus">The status of the player.</param>
    /// <param name="opponentStatus">The status of the opponent.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        foreach (var invocationCard in playerCards.InvocationCards)
        {
            invocationCard.SetRemainedAttackThisTurn(numberAttackPerTurn);
        }
    }

    /// <summary>
    /// Overrides the effect when an invocation card is added.
    /// </summary>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="invocationCard">The invocation card that was added.</param>
    public override void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        base.OnInvocationCardAdded(playerCards, invocationCard);
        invocationCard.SetRemainedAttackThisTurn(numberAttackPerTurn);
    }

    /// <summary>
    /// Overrides the effect when an invocation card is removed.
    /// </summary>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="invocationCard">The invocation card that was removed.</param>
    public override void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        base.OnInvocationCardRemoved(playerCards, invocationCard);
        invocationCard.SetRemainedAttackThisTurn(1);
    }
}
