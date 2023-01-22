using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using UnityEngine;

public class CantLiveWithoutInvocationCardsAbility : Ability
{
    private string cardName;
    private List<string> conditionInvocationCards;

    public CantLiveWithoutInvocationCardsAbility(AbilityName name, string description, string cardName,
        List<string> conditionInvocationCards)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
        this.conditionInvocationCards = conditionInvocationCards;
    }
    
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyPower(playerCards);
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        ApplyPower(playerCards);
    }

    private void ApplyPower(PlayerCards playerCards)
    {
        if (CantLive(playerCards))
        {
            // TODO Add Observer on InvocationCards to know when card is dead
            InGameInvocationCard doomedCard = playerCards.invocationCards.Find(card => card.Title == cardName);
            playerCards.invocationCards.Remove(doomedCard);
            playerCards.yellowTrash.Add(doomedCard);
        }
    }

    private bool CantLive(PlayerCards playerCards)
    {
        return !playerCards.invocationCards.Exists(card => conditionInvocationCards.Contains(card.Title));
    }
}
