using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an ability allowing a player to sacrifice a specific card.
/// </summary>
public class SacrificeCardAbility : Ability
{
    private readonly string cardName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SacrificeCardAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cardName">The name of the card to be sacrificed.</param>
    public SacrificeCardAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }

    /// <summary>
    /// Applies the effect of the ability, sacrificing the specified card.
    /// If the card to be sacrificed has an equipment card attached, it will be moved to the player's YellowCards.
    /// </summary>
    /// <param name="canvas">The canvas to display any UI elements on.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent's player cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        InGameInvocationCard invocationCardToSacrifice = playerCards.InvocationCards.First(card => cardName == card.Title);
        if (invocationCardToSacrifice.EquipmentCard != null)
        {
            playerCards.YellowCards.Add(invocationCardToSacrifice.EquipmentCard);
            invocationCardToSacrifice.EquipmentCard = null;
        }
        playerCards.InvocationCards.Remove(invocationCardToSacrifice);
        playerCards.YellowCards.Add(invocationCardToSacrifice);
    }
}
