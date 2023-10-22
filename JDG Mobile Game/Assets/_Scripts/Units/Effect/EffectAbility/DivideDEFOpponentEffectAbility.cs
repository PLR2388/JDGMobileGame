using UnityEngine;

/// <summary>
/// Represents an ability to divide the defense of the opponent's invocation cards.
/// </summary>
public class DivideDEFOpponentEffectAbility : EffectAbility
{
    private readonly float divideFactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DivideDEFOpponentEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="divideFactor">The factor by which the defense of the opponent's invocation cards is divided.</param>
    public DivideDEFOpponentEffectAbility(EffectAbilityName name, string description, float divideFactor)
    {
        Name = name;
        Description = description;
        this.divideFactor = divideFactor;
    }

    /// <summary>
    /// Determines if the effect can be used based on the player and opponent's cards and status.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCard">The opponent's current cards.</param>
    /// <param name="opponentPlayerStatus">The opponent's current status.</param>
    /// <returns><c>true</c> if the effect can be used; otherwise, <c>false</c>.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        return opponentPlayerCard.InvocationCards.Count > 0;
    }

    /// <summary>
    /// Applies the effect to divide the defense of the opponent's invocation cards.
    /// </summary>
    /// <param name="canvas">Canvas used for UI operations.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCard">The opponent's current cards.</param>
    /// <param name="playerStatus">The player's current status.</param>
    /// <param name="opponentStatus">The opponent's current status.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        foreach (var invocationCard in opponentPlayerCard.InvocationCards)
        {
            invocationCard.Defense /= divideFactor;
        }
    }

    /// <summary>
    /// Resets the defense of the opponent's invocation cards on the start of a turn.
    /// </summary>
    /// <param name="canvas">Canvas used for UI operations.</param>
    /// <param name="playerStatus">The player's current status.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerStatus">The opponent's current status.</param>
    /// <param name="opponentPlayerCard">The opponent's current cards.</param>
    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards,
        PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCard)
    {
        base.OnTurnStart(canvas, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCard);
        foreach (var invocationCard in opponentPlayerCard.InvocationCards)
        {
            invocationCard.Defense *= divideFactor;
        }
    }
}
