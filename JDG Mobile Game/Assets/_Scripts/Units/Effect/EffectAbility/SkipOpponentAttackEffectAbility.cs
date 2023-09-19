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
        opponentStatus.EnableBlockAttack();
    }

    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards, PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCards)
    {
        counter++;
        if (counter > NumberOfTurn)
        {
            counter = 0;
            opponentPlayerStatus.DisableBlockAttack();
            var card = playerCards.EffectCards.First(effectCard => effectCard.Title == cardName);
            playerCards.EffectCards.Remove(card);
            playerCards.YellowCards.Add(card);
        }
    }
}
