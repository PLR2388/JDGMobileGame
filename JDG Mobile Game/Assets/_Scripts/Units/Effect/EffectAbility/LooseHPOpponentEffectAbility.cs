using UnityEngine;

public class LooseHPOpponentEffectAbility : EffectAbility
{
    private bool fromOpponentNumberInvocationCards;
    private bool fromCurrentNumberInvocationCards;
    private float damage;

    public LooseHPOpponentEffectAbility(EffectAbilityName name, string description, float damage, bool fromOpponentNumberInvocationCards = false, bool fromCurrentNumberInvocationCards = false)
    {
        Name = name;
        Description = description;
        this.fromOpponentNumberInvocationCards = fromOpponentNumberInvocationCards;
        this.fromCurrentNumberInvocationCards = fromCurrentNumberInvocationCards;
        this.damage = damage;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        if (fromCurrentNumberInvocationCards)
        {
            return playerCards.invocationCards.Count > 0;
        }

        if (fromOpponentNumberInvocationCards)
        {
            return opponentPlayerCard.invocationCards.Count > 0;
        }

        return true;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        if (fromOpponentNumberInvocationCards)
        {
            opponentStatus.ChangePv(-damage * opponentPlayerCard.invocationCards.Count);
        }

        if (fromCurrentNumberInvocationCards)
        {
            opponentStatus.ChangePv(-damage * playerCards.invocationCards.Count);
        }
    }
}
