using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class CantBeAttackAbility : Ability
{
    private CardFamily family;

    public CantBeAttackAbility(AbilityName name, string description,
        CardFamily family = CardFamily.Any)
    {
        Name = name;
        Description = description;
        this.family = family;
    }

    private bool IsProtected(PlayerCards playerCards)
    {
        return family == CardFamily.Any ||
               playerCards.invocationCards.Any(card => card.Title != invocationCard.Title && card.Families.Contains(family));
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        playerCards.invocationCards.First(card => card.Title == invocationCard.Title).CantBeAttack = IsProtected(playerCards);
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        playerCards.invocationCards.First(card => card.Title == invocationCard.Title).CantBeAttack = IsProtected(playerCards);
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        playerCards.invocationCards.First(card => card.Title == invocationCard.Title).CantBeAttack = IsProtected(playerCards);
    }
}