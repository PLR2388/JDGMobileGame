using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public class SacrificeCardAbility : Ability
{
    protected readonly string cardName;

    public SacrificeCardAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        InGameInvocationCard invocationCard = playerCards.invocationCards.First(card => cardName == card.Title);
        // TODO Centralize death invocation Card
        if (invocationCard.EquipmentCard != null)
        {
            playerCards.yellowTrash.Add(invocationCard.EquipmentCard);
        }
        playerCards.invocationCards.Remove(invocationCard);
        playerCards.yellowTrash.Add(invocationCard);
    }
}
