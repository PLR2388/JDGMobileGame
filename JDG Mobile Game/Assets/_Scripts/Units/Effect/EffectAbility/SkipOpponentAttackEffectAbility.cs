using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkipOpponentAttackEffectAbility : EffectAbility
{
    private string cardName;
    public SkipOpponentAttackEffectAbility(EffectAbilityName name, string description, string cardName, int numberTurn = 1)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
        NumberOfTurn = numberTurn;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        opponentStatus.BlockAttack = true;
    }

    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards, PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCards)
    {
        counter++;
        if (counter > NumberOfTurn)
        {
            counter = 0;
            opponentPlayerStatus.BlockAttack = false;
            var card = playerCards.effectCards.First(effectCard => effectCard.Title == cardName);
            playerCards.effectCards.Remove(card);
            playerCards.yellowCards.Add(card);
        }
    }
}
