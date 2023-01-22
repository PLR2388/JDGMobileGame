using _Scripts.Units.Invocation;
using UnityEngine;

public class SacrificeCardAbility : Ability
{
    private readonly string cardName;

    public SacrificeCardAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        InGameInvocationCard invocationCard = playerCards.invocationCards.Find(card => cardName == card.Title);
        // TODO Centralize death invocation Card
        playerCards.invocationCards.Remove(invocationCard);
        playerCards.yellowTrash.Add(invocationCard);
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
