using System;
using UnityEngine;

/// <summary>
/// Enumerates the types of damage calculations based on card counts.
/// </summary>
public enum DamageType
{
    ByPlayerInvocationCount,
    ByOpponentInvocationCount,
    ByOpponentHandCount
}

/// <summary>
/// Defines an effect ability that reduces an opponent's health based on various damage types.
/// </summary>
public class LooseHPOpponentEffectAbility : EffectAbility
{
    private readonly DamageType damageType;
    private readonly float damage;

    /// <summary>
    /// Initializes a new instance of the <see cref="LooseHPOpponentEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="damage">The base damage value.</param>
    /// <param name="damageType">The type of damage calculation.</param>
    public LooseHPOpponentEffectAbility(EffectAbilityName name, string description, float damage,
        DamageType damageType)
    {
        Name = name;
        Description = description;
        this.damage = damage;
        this.damageType = damageType;
    }

    /// <summary>
    /// Determines if the effect can be applied based on card counts and damage type.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <param name="opponentPlayerStatus">The opponent's status.</param>
    /// <returns><c>true</c> if the effect can be applied, <c>false</c> otherwise.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        return damageType switch
        {
            DamageType.ByPlayerInvocationCount => playerCards.InvocationCards.Count > 0,
            DamageType.ByOpponentInvocationCount => opponentPlayerCard.InvocationCards.Count > 0,
            DamageType.ByOpponentHandCount => opponentPlayerCard.HandCards.Count > 0,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Applies the effect to the opponent based on the damage type and card counts.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <param name="playerStatus">The player's status.</param>
    /// <param name="opponentStatus">The opponent's status.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        switch (damageType)
        {
            case DamageType.ByPlayerInvocationCount:
                opponentStatus.ChangePv(-damage * playerCards.InvocationCards.Count);
                break;
            case DamageType.ByOpponentInvocationCount:
                opponentStatus.ChangePv(-damage * opponentPlayerCard.InvocationCards.Count);
                break;
            case DamageType.ByOpponentHandCount:
                opponentStatus.ChangePv(-damage * opponentPlayerCard.HandCards.Count);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}