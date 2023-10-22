using System.Linq;
using UnityEngine;

/// <summary>
/// Represents an effect ability that adds shields for the user.
/// </summary>
public class AddShieldsForUserEffectAbility : EffectAbility
{
    private readonly int numberShields;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddShieldsForUserEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="numberShields">The number of shields to add.</param>
    public AddShieldsForUserEffectAbility(EffectAbilityName name, string description, int numberShields)
    {
        Name = name;
        Description = description;
        this.numberShields = numberShields;
        NumberOfTurn = 0;
    }

    /// <summary>
    /// Determines if the effect can be used based on the player's cards.
    /// </summary>
    /// <param name="playerCards">The cards held by the player.</param>
    /// <param name="opponentPlayerCard">The cards held by the opponent.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    /// <returns>True if the effect can be used; otherwise, false.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        return playerCards.InvocationCards.Count == 0;
    }

    /// <summary>
    /// Applies the effect of adding shields for the user.
    /// </summary>
    /// <param name="canvas">Transform reference for positioning UI elements.</param>
    /// <param name="playerCards">The cards held by the player.</param>
    /// <param name="opponentPlayerCard">The cards held by the opponent.</param>
    /// <param name="playerStatus">The current status of the player.</param>
    /// <param name="opponentStatus">The current status of the opponent.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        playerStatus.SetShieldCount(numberShields);
    }

    /// <summary>
    /// Logic to execute at the start of the turn.
    /// </summary>
    /// <param name="canvas">Transform reference for positioning UI elements.</param>
    /// <param name="playerStatus">The current status of the player.</param>
    /// <param name="playerCards">The cards held by the player.</param>
    /// <param name="opponentPlayerStatus">The current status of the opponent.</param>
    /// <param name="opponentPlayerCard">The cards held by the opponent.</param>
    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards,
        PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCard)
    {
        if (playerStatus.NumberShield == 0)
        {
            var effectCard = playerCards.EffectCards.First(card => card.EffectAbilities.Contains(this));
            playerCards.EffectCards.Remove(effectCard);
            playerCards.YellowCards.Add(effectCard);
        }
    }
}
