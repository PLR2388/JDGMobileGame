using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class CantLiveWithoutAbility : Ability
{
    private string cardName;
    private List<string> conditionInvocationCards;
    private CardFamily family;

    public CantLiveWithoutAbility(AbilityName name, string description, string originCardName, List<string> list = null,
        CardFamily family = CardFamily.Any)
    {
        Name = name;
        Description = description;
        cardName = originCardName;
        conditionInvocationCards = list;
        this.family = family;
    }


    private void ApplyPower(PlayerCards playerCards)
    {
        if (CantLive(playerCards))
        {
            // TODO Add Observer on InvocationCards to know when card is dead
            InGameInvocationCard doomedCard = playerCards.invocationCards.First(card => card.Title == cardName);
            playerCards.invocationCards.Remove(doomedCard);
            playerCards.yellowTrash.Add(doomedCard);
        }
    }

    private bool CantLive(PlayerCards playerCards)
    {
        if (conditionInvocationCards == null)
        {
            // Check Family
            return !playerCards.invocationCards.Any(card =>
                card.Title != cardName &&
                (card.Families.Contains(family) || family == CardFamily.Any));
        }
        else
        {
            // Check existence of specific cards
            return !playerCards.invocationCards.Any(card => conditionInvocationCards.Contains(card.Title));
        }
 
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyPower(playerCards);
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (removeCard.Title != cardName)
        {
            ApplyPower(playerCards);    
        }
    }
}