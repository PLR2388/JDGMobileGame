using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class CantBeAttackAbility : Ability
{
    private string cardName;
    private CardFamily family;

    public CantBeAttackAbility(AbilityName name, string description, string cardName,
        CardFamily family = CardFamily.Any)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
        this.family = family;
    }

    private bool IsProtected(PlayerCards playerCards)
    {
        return family == CardFamily.Any ||
               playerCards.invocationCards.Exists(card => card.Title != cardName && card.Families.Contains(family));
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        playerCards.invocationCards.Find(card => card.Title == cardName).CantBeAttack = IsProtected(playerCards);
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        playerCards.invocationCards.Find(card => card.Title == cardName).CantBeAttack = IsProtected(playerCards);
    }
}