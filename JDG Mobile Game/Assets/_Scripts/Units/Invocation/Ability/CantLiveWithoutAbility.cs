using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability that enforces a dependency of the card's existence based on certain card conditions.
/// </summary>
public class CantLiveWithoutAbility : Ability
{
    private readonly List<string> conditionInvocationCards;
    private readonly CardFamily family;

    /// <summary>
    /// Initializes a new instance of the <see cref="CantLiveWithoutAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="list">The list of card titles this ability relies on. Defaults to null.</param>
    /// <param name="family">The family of cards this ability relies on. Defaults to any family.</param>
    public CantLiveWithoutAbility(AbilityName name, string description, List<string> list = null,
        CardFamily family = CardFamily.Any)
    {
        Name = name;
        Description = description;
        conditionInvocationCards = list;
        this.family = family;
    }

    /// <summary>
    /// Checks and applies the power of the ability, removing the card if conditions aren't met.
    /// </summary>
    /// <param name="playerCards">The player's collection of cards.</param>
    private void ApplyPower(PlayerCards playerCards)
    {
        if (CantLive(playerCards))
        {
            playerCards.InvocationCards.Remove(invocationCard);
            playerCards.YellowCards.Add(invocationCard);
        }
    }

    /// <summary>
    /// Determines if the card can't live based on its conditions.
    /// </summary>
    /// <param name="playerCards">The player's collection of cards.</param>
    /// <returns>True if the card can't live, otherwise false.</returns>
    private bool CantLive(PlayerCards playerCards)
    {
        if (conditionInvocationCards == null)
        {
            // Check Family
            return !playerCards.InvocationCards.Any(card =>
                card.Title != invocationCard.Title &&
                (card.Families.Contains(family) || family == CardFamily.Any));
        }
        else
        {
            // Check existence of specific cards
            return !playerCards.InvocationCards.Any(card => conditionInvocationCards.Contains(card.Title));
        }
 
    }

    /// <summary>
    /// Applies the effect of the ability.
    /// </summary>
    /// <param name="canvas">The UI canvas.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    /// <param name="opponentPlayerCards">The opponent's collection of cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyPower(playerCards);
    }

    /// <summary>
    /// Reactivates the ability effect.
    /// </summary>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        ApplyPower(playerCards);
    }

    /// <summary>
    /// Handles the scenario when a card is removed.
    /// </summary>
    /// <param name="removeCard">The card that was removed.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (removeCard.Title != invocationCard.Title)
        {
            ApplyPower(playerCards);    
        }
    }
}