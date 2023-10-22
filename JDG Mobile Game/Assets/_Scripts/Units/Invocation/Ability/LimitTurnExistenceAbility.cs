using UnityEngine;

/// <summary>
/// Represents an ability that limits the number of turns an invocation card can exist on the field.
/// </summary>
public class LimitTurnExistenceAbility : Ability
{
    private readonly int numberOfTurn;

    /// <summary>
    /// Initializes a new instance of the <see cref="LimitTurnExistenceAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="numberTurn">The number of turns the invocation card can exist on the field.</param>
    public LimitTurnExistenceAbility(AbilityName name, string description, int numberTurn)
    {
        Name = name;
        Description = description;
        numberOfTurn = numberTurn;
    }

    /// <summary>
    /// Invoked at the start of each turn, this method checks the existence duration of the associated
    /// invocation card and moves it to the yellow cards collection if the limit is reached.
    /// </summary>
    /// <param name="canvas">The canvas transform.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent player's cards.</param>
    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (invocationCard.NumberOfTurnOnField >= numberOfTurn)
        {
            var equipmentCard = invocationCard.EquipmentCard;
            playerCards.YellowCards.Add(invocationCard);
            playerCards.InvocationCards.Remove(invocationCard);
            if (equipmentCard != null)
            {
                playerCards.YellowCards.Add(equipmentCard);
                invocationCard.EquipmentCard = null;
            } 
        }
        
    }
}