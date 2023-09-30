using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an ability to invoke a specific card from the deck based on its name.
/// </summary>
public class InvokeSpecificCardAbility : Ability
{
    private readonly string cardName;

    /// <summary>
    /// Initializes a new instance of the InvokeSpecificCardAbility class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cardName">The name of the specific card to be invoked.</param>
    public InvokeSpecificCardAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }

    /// <summary>
    /// Applies the effect of this ability, which involves invoking a specific card by its name.
    /// </summary>
    /// <param name="canvas">The canvas to display any UI elements.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent player's cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool hasCardInDeck = playerCards.Deck.Exists(card => card.Title == cardName);
        if (hasCardInDeck)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_INVOKE_SPECIFIC_CARD_MESSAGE),
                    cardName
                ),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    InGameInvocationCard card = playerCards.Deck.Find(card => card.Title == cardName) as InGameInvocationCard;
                    playerCards.Deck.Remove(card);
                    playerCards.InvocationCards.Add(card);
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }
}