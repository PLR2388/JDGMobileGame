using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an ability that allows for an optional sacrifice of a card to boost Attack and Defense.
/// </summary>
public class OptionalSacrificeForAtkDefAbility : Ability
{
    private readonly string cardNameToSacrifice;

    private readonly string requiredField;

    private readonly float newAtk;
    private readonly float newDef;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalSacrificeForAtkDefAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cardNameToSacrifice">The name of the card to be optionally sacrificed.</param>
    /// <param name="requiredField">The required field for the ability to be activated.</param>
    /// <param name="newAtk">The new Attack value after the sacrifice.</param>
    /// <param name="newDef">The new Defense value after the sacrifice.</param>
    public OptionalSacrificeForAtkDefAbility(AbilityName name, string description,
        string cardNameToSacrifice, string requiredField, float newAtk, float newDef)
    {
        Name = name;
        Description = description;
        this.cardNameToSacrifice = cardNameToSacrifice;
        this.requiredField = requiredField;
        this.newAtk = newAtk;
        this.newDef = newDef;
    }

    /// <summary>
    /// Applies the effect of the ability, offering an option to sacrifice a card to boost Attack and Defense,
    /// if the required field and card to sacrifice are present.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool isFieldPresent = playerCards.FieldCard?.Title == requiredField;
        bool isInvocationPresent = playerCards.InvocationCards.Any(card => card.Title == cardNameToSacrifice);

        if (isFieldPresent && isInvocationPresent)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_SACRIFICE_TO_BOOST_MESSAGE),
                    cardNameToSacrifice,
                    newAtk,
                    newDef,
                    invocationCard.Title
                ),
                showPositiveButton: true,
                showNegativeButton: true,
                positiveAction: () =>
                {
                    InGameInvocationCard invocationCardToSacrifice =
                        playerCards.InvocationCards.First(card => card.Title == cardNameToSacrifice);
                    playerCards.InvocationCards.Remove(invocationCardToSacrifice);
                    playerCards.YellowCards.Add(invocationCardToSacrifice);

                    invocationCard.Attack = newAtk;
                    invocationCard.Defense = newDef;
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }

    /// <summary>
    /// Handles the behavior when a card is dead. Resets the Attack and Defense values of the dead card.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="deadCard">The card that has died.</param>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    /// <returns>A boolean value indicating whether the base method should be called.</returns>
    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (deadCard.CancelEffect)
        {
            return base.OnCardDeath(canvas, deadCard, playerCards, opponentPlayerCards);
        }
        deadCard.Attack = deadCard.BaseInvocationCard.BaseInvocationCardStats.Attack;
        deadCard.Defense = deadCard.BaseInvocationCard.BaseInvocationCardStats.Defense;
        return base.OnCardDeath(canvas, deadCard, playerCards, opponentPlayerCards);
    }
}