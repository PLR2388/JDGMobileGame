using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class GetSpecificCardAfterDeathAbility : GetSpecificCardFromDeckAbility
{
    private string cardWhichDeadName;
    
    public GetSpecificCardAfterDeathAbility(AbilityName name, string description, string cardName, string deadCardName) : base(name, description, cardName)
    {
        cardWhichDeadName = deadCardName;
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (removeCard.Title == cardWhichDeadName)
        {
            GetSpecificCard(canvas, playerCards);    
        }
    }
}
