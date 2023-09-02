using UnityEngine;

public class LooseHPOpponentEffectAbility : EffectAbility
{
    private bool fromOpponentNumberInvocationCards;
    private bool fromCurrentNumberInvocationCards;
    private bool fromOpponentNumberHandCards;
    private float damage;

    public LooseHPOpponentEffectAbility(EffectAbilityName name, string description, float damage,
        bool fromOpponentNumberInvocationCards = false, bool fromCurrentNumberInvocationCards = false,
        bool fromOpponentNumberHandCards = false)
    {
        Name = name;
        Description = description;
        this.fromOpponentNumberInvocationCards = fromOpponentNumberInvocationCards;
        this.fromCurrentNumberInvocationCards = fromCurrentNumberInvocationCards;
        this.damage = damage;
        this.fromOpponentNumberHandCards = fromOpponentNumberHandCards;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        if (fromCurrentNumberInvocationCards)
        {
            return playerCards.InvocationCards.Count > 0;
        }

        if (fromOpponentNumberInvocationCards)
        {
            return opponentPlayerCard.InvocationCards.Count > 0;
        }

        if (fromOpponentNumberHandCards)
        {
            return opponentPlayerCard.HandCards.Count > 0;
        }

        return true;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        if (fromOpponentNumberInvocationCards)
        {
            opponentStatus.ChangePv(-damage * opponentPlayerCard.InvocationCards.Count);
        }

        if (fromCurrentNumberInvocationCards)
        {
            opponentStatus.ChangePv(-damage * playerCards.InvocationCards.Count);
        }

        if (fromOpponentNumberHandCards)
        {
            opponentStatus.ChangePv(-damage * opponentPlayerCard.HandCards.Count);
        }
    }
}