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
        invocationCard.CantBeAttack = IsProtected(playerCards);
    }

    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        invocationCard.CantBeAttack = false;
    }

    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        invocationCard.CantBeAttack = IsProtected(playerCards);
    }

    public override void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
        base.OnCardAdded(newCard, playerCards);
        invocationCard.CantBeAttack = IsProtected(playerCards);
    }

    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
        invocationCard.CantBeAttack = IsProtected(playerCards);
    }
}