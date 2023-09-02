using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GiveAtkDefFamilyAbility : Ability
{
    private CardFamily family;
    private float attack;
    private float defense;

    public GiveAtkDefFamilyAbility(AbilityName name, string description, CardFamily cardFamily, float attack, float defense)
    {
        Name = name;
        Description = description;
        family = cardFamily;
        this.attack = attack;
        this.defense = defense;
    }

    private void IncreaseAtkDef(InGameInvocationCard inGameInvocationCard)
    {
        inGameInvocationCard.Defense += defense;
        inGameInvocationCard.Attack += attack;
    }

    private void DecreaseAtkDef(InGameInvocationCard inGameInvocationCard)
    {
        inGameInvocationCard.Defense -= defense;
        inGameInvocationCard.Attack -= attack;
    }
    
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyPower(playerCards);
    }

    private void ApplyPower(PlayerCards playerCards)
    {
        IEnumerable<InGameInvocationCard> invocationCards =
            playerCards.InvocationCards.Where(card => card.Title != invocationCard.Title && card.Families.Contains(family));
        foreach (var inGameInvocationCard in invocationCards)
        {
            IncreaseAtkDef(inGameInvocationCard);
        }
    }

    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        IEnumerable<InGameInvocationCard> invocationCards =
            playerCards.InvocationCards.Where(card => card.Title != invocationCard.Title && card.Families.Contains(family));
        foreach (var inGameInvocationCard in invocationCards)
        {
            DecreaseAtkDef(inGameInvocationCard);
        }
    }

    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        ApplyPower(playerCards);
    }

    public override void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
        base.OnCardAdded(newCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (newCard.Title != invocationCard.Title && newCard.Families.Contains(family))
        {
            IncreaseAtkDef(newCard);
        }
    }

    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
        if (removeCard.Title != invocationCard.Title && removeCard.Families.Contains(family))
        {
            DecreaseAtkDef(removeCard);
        }
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,PlayerCards opponentPlayerCards)
    {
        IEnumerable<InGameInvocationCard> invocationCards =
            playerCards.InvocationCards.Where(card => card.Title != invocationCard.Title && card.Families.Contains(family));
        foreach (var inGameInvocationCard in invocationCards)
        {
            DecreaseAtkDef(inGameInvocationCard);
        }
        return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
    }
}
