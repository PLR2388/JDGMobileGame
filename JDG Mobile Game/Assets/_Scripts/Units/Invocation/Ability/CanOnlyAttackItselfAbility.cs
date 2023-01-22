
using _Scripts.Units.Invocation;
using UnityEngine;

public class CanOnlyAttackItselfAbility : Ability
{
    private readonly string cardName;
    
    public CanOnlyAttackItselfAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        playerCards.invocationCards.Find(card => card.Title == cardName).Aggro = true;
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
       
    }
}
