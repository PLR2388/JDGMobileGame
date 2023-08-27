using System.Linq;
using UnityEngine;

public class AddShieldsForUserEffectAbility : EffectAbility
{
    private readonly int numberShields;

    public AddShieldsForUserEffectAbility(EffectAbilityName name, string description, int numberShields)
    {
        Name = name;
        Description = description;
        this.numberShields = numberShields;
        NumberOfTurn = 0;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        return playerCards.invocationCards.Count == 0;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        playerStatus.SetNumberShield(numberShields);
    }

    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards,
        PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCard)
    {
        if (playerStatus.NumberShield == 0)
        {
            var effectCard = playerCards.effectCards.First(card => card.EffectAbilities.Contains(this));
            playerCards.effectCards.Remove(effectCard);
            playerCards.yellowCards.Add(effectCard);
        }
    }
}
