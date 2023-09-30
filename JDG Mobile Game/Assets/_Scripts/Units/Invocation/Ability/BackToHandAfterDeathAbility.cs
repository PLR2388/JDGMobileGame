using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an ability where the associated card is returned to the hand after it dies a specified number of times.
/// </summary>
public class BackToHandAfterDeathAbility : Ability
{
    private readonly int numberDeathMax; // 0 = infinity

    /// <summary>
    /// Initializes a new instance of the <see cref="BackToHandAfterDeathAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="numberDeathMax">The maximum number of times the card can die and be returned to hand. A value of 0 indicates infinite times.</param>
    public BackToHandAfterDeathAbility(AbilityName name, string description, int numberDeathMax = 0)
    {
        Name = name;
        Description = description;
        this.numberDeathMax = numberDeathMax;
    }

    /// <summary>
    /// Checks if a card should return to hand after death based on the ability's conditions.
    /// </summary>
    /// <param name="canvas">The UI canvas.</param>
    /// <param name="deadCard">The card that has died.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    /// <param name="opponentPlayerCards">The opponent's collection of cards.</param>
    /// <returns>True if the base ability's conditions are met, otherwise false.</returns>
    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        var value = base.OnCardDeath(canvas, deadCard, playerCards, opponentPlayerCards);
        if (invocationCard.CancelEffect == false && deadCard.Title == invocationCard.Title &&
            (numberDeathMax == 0 || deadCard.NumberOfDeaths < numberDeathMax) &&
            !playerCards.HandCards.Contains(deadCard))
        {
            playerCards.YellowCards.Remove(deadCard);
            playerCards.HandCards.Add(deadCard);
        }
        
        return value;
    }
}