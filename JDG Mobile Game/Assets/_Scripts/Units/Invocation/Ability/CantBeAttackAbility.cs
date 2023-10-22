using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability that can prevent the associated card from being attacked based on certain conditions.
/// </summary>
public class CantBeAttackAbility : Ability
{
    private readonly CardFamily family;

    /// <summary>
    /// Initializes a new instance of the <see cref="CantBeAttackAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="family">The family of cards that this ability relies on for protection. Defaults to any family.</param>
    public CantBeAttackAbility(AbilityName name, string description,
        CardFamily family = CardFamily.Any)
    {
        Name = name;
        Description = description;
        this.family = family;
    }

    /// <summary>
    /// Checks if the associated card is protected based on the conditions.
    /// </summary>
    /// <param name="playerCards">The player's collection of cards.</param>
    /// <returns>True if the card is protected, otherwise false.</returns>
    private bool IsProtected(PlayerCards playerCards)
    {
        return family == CardFamily.Any ||
               playerCards.InvocationCards.Any(card => card.Title != invocationCard.Title && card.Families.Contains(family));
    }

    /// <summary>
    /// Applies the effect of the ability, updating the attack status of the associated card.
    /// </summary>
    /// <param name="canvas">The UI canvas.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    /// <param name="opponentPlayerCards">The opponent's collection of cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        invocationCard.CantBeAttack = IsProtected(playerCards);
    }

    /// <summary>
    /// Cancels the ability effect, making the associated card attackable.
    /// </summary>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        invocationCard.CantBeAttack = false;
    }

    /// <summary>
    /// Reactivates the ability effect, updating the attack status of the associated card.
    /// </summary>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        invocationCard.CantBeAttack = IsProtected(playerCards);
    }

    /// <summary>
    /// Handles the scenario when a card is added, updating the attack status of the associated card.
    /// </summary>
    /// <param name="newCard">The card that was added.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
        base.OnCardAdded(newCard, playerCards);
        invocationCard.CantBeAttack = IsProtected(playerCards);
    }

    /// <summary>
    /// Handles the scenario when a card is removed, updating the attack status of the associated card.
    /// </summary>
    /// <param name="removeCard">The card that was removed.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
        invocationCard.CantBeAttack = IsProtected(playerCards);
    }
}