using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an ability to retrieve a specific card from the deck after the death of a specific card.
/// This ability inherits from <see cref="GetSpecificCardFromDeckAbility"/>.
/// </summary>
public class GetSpecificCardAfterDeathAbility : GetSpecificCardFromDeckAbility
{

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSpecificCardAfterDeathAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cardName">The name of the specific card this ability is concerned with.</param>
    public GetSpecificCardAfterDeathAbility(AbilityName name, string description, string cardName)
        : base(name, description, cardName)
    {
    }

    /// <summary>
    /// Overrides the base ApplyEffect method but currently does nothing.
    /// </summary>
    /// <param name="canvas">The game canvas where UI components will be displayed.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    /// <summary>
    /// Triggers when a card dies. If the dead card matches the invocation card and the effect isn't canceled,
    /// it attempts to retrieve the specified card from the deck.
    /// </summary>
    /// <param name="canvas">The game canvas where UI components will be displayed.</param>
    /// <param name="deadCard">The card that died.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    /// <returns>Returns the result from the base <see cref="OnCardDeath"/> method.</returns>
    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
        }
        // TODO: Test if "if" can be removed
        if (deadCard.Title == invocationCard.Title)
        {
            GetSpecificCard(canvas, playerCards);
        }

        return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
    }
}