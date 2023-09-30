using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability to retrieve cards of a specified family from the deck.
/// </summary>
public class GetFamilyInDeckAbility : Ability
{
    /// <summary>
    /// The family of cards this ability targets.
    /// </summary>
    private readonly CardFamily family;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFamilyInDeckAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cardFamily">The family of cards this ability is concerned with.</param>
    public GetFamilyInDeckAbility(AbilityName name, string description, CardFamily cardFamily)
    {
        Name = name;
        Description = description;
        family = cardFamily;
    }

    /// <summary>
    /// Applies the effect of the ability, prompting the player to select a card from the specified family.
    /// If the player selects a valid card, it's moved from the deck to the hand.
    /// </summary>
    /// <param name="canvas">The game canvas where UI components will be displayed.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
            string.Format(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_GET_CARD_FROM_FAMILY_MESSAGE),
                family.ToName()
            ),
            showNegativeButton: true,
            showPositiveButton: true,
            positiveAction: () =>
            {
                List<InGameCard> familyCards = playerCards.Deck.FindAll(card =>
                    card.Type == CardType.Invocation && (card as InGameInvocationCard)?.Families.Contains(family) == true);

                var config = new CardSelectorConfig(
                    string.Format(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_FAMILY_CARD),
                        family.ToName()
                    ),
                    familyCards,
                    showOkButton: true,
                    okAction: card =>
                    {
                        if (card == null)
                        {
                            var config = new MessageBoxConfig(
                                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARD),
                                showOkButton: true
                            );
                            MessageBox.Instance.CreateMessageBox(
                                canvas,
                                config
                            );
                        }
                        else
                        {
                            playerCards.Deck.Remove(card);
                            playerCards.HandCards.Add(card);
                        }
                    }
                );
                CardSelector.Instance.CreateCardSelection(canvas, config);
            }
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }
}