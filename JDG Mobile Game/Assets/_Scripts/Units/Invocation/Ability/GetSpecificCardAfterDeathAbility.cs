using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class GetSpecificCardAfterDeathAbility : GetSpecificCardFromDeckAbility
{
    private string cardWhichDeadName;

    public GetSpecificCardAfterDeathAbility(AbilityName name, string description, string cardName, string deadCardName)
        : base(name, description, cardName)
    {
        cardWhichDeadName = deadCardName;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards)
    {
        // TODO: Test if "if" can be removed
        if (deadCard.Title == cardWhichDeadName)
        {
            GetSpecificCard(canvas, playerCards);
        }

        return base.OnCardDeath(canvas, deadCard, playerCards);
    }
}