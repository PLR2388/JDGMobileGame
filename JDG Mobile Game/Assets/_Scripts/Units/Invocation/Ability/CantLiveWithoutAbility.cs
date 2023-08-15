using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class CantLiveWithoutAbility : Ability
{
    private List<string> conditionInvocationCards;
    private CardFamily family;

    public CantLiveWithoutAbility(AbilityName name, string description, List<string> list = null,
        CardFamily family = CardFamily.Any)
    {
        Name = name;
        Description = description;
        conditionInvocationCards = list;
        this.family = family;
    }


    private void ApplyPower(PlayerCards playerCards)
    {
        if (CantLive(playerCards))
        {
            InGameInvocationCard doomedCard = playerCards.invocationCards.First(card => card.Title == invocationCard.Title);
            playerCards.invocationCards.Remove(doomedCard);
            playerCards.yellowCards.Add(doomedCard);
        }
    }

    private bool CantLive(PlayerCards playerCards)
    {
        if (conditionInvocationCards == null)
        {
            // Check Family
            return !playerCards.invocationCards.Any(card =>
                card.Title != invocationCard.Title &&
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
        if (removeCard.Title != invocationCard.Title)
        {
            ApplyPower(playerCards);    
        }
    }
}