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
        InGameInvocationCard invocationCardToSacrifice = playerCards.InvocationCards.First(card => cardName == card.Title);
        // TODO Centralize death invocation Card
        if (invocationCardToSacrifice.EquipmentCard != null)
        {
            playerCards.YellowCards.Add(invocationCardToSacrifice.EquipmentCard);
            invocationCardToSacrifice.EquipmentCard = null;
        }
        playerCards.InvocationCards.Remove(invocationCardToSacrifice);
        playerCards.YellowCards.Add(invocationCardToSacrifice);
    }
}
