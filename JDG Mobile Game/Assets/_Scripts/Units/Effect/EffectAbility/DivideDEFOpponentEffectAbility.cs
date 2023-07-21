using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideDEFOpponentEffectAbility : EffectAbility
{
    private float divideFactor;

    public DivideDEFOpponentEffectAbility(EffectAbilityName name, string description, float divideFactor)
    {
        Name = name;
        Description = description;
        this.divideFactor = divideFactor;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        return opponentPlayerCard.invocationCards.Count > 0;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        foreach (var invocationCard in opponentPlayerCard.invocationCards)
        {
            invocationCard.Defense /= divideFactor;
        }
    }

    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards,
        PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCard)
    {
        base.OnTurnStart(canvas, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCard);
        foreach (var invocationCard in opponentPlayerCard.invocationCards)
        {
            invocationCard.Defense *= divideFactor;
        }
    }
}
