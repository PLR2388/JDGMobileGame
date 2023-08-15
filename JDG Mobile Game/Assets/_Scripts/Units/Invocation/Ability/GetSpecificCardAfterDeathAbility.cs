using _Scripts.Units.Invocation;
using UnityEngine;

public class GetSpecificCardAfterDeathAbility : GetSpecificCardFromDeckAbility
{

    public GetSpecificCardAfterDeathAbility(AbilityName name, string description, string cardName)
        : base(name, description, cardName)
    {
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,PlayerCards opponentPlayerCards)
    {
        // TODO: Test if "if" can be removed
        if (deadCard.Title == invocationCard.Title)
        {
            GetSpecificCard(canvas, playerCards);
        }

        return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
    }
}