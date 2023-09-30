using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents a field ability that allows players to skip their draw phase to obtain a card
/// from a specific family either from their deck or the yellow trash.
/// </summary>
public class GetCardFromFamilyIfSkipDrawAbility : FieldAbility
{
    private readonly CardFamily family;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCardFromFamilyIfSkipDrawAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the field ability.</param>
    /// <param name="description">The description of the field ability.</param>
    /// <param name="family">The card family related to this ability.</param>
    public GetCardFromFamilyIfSkipDrawAbility(FieldAbilityName name, string description, CardFamily family)
    {
        Name = name;
        Description = description;
        this.family = family;
    }

    /// <summary>
    /// Displays a message indicating the action was successful.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    private void DisplayOkMessage(Transform canvas)
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
    
    /// <summary>
    /// Checks if the provided card belongs to the family associated with this ability.
    /// </summary>
    /// <param name="card">The card to check.</param>
    /// <returns>True if the card belongs to the family; otherwise, false.</returns>
    private bool IsFromFamily(InGameCard card)
    {
        return card.Type == CardType.Invocation && (card as InGameInvocationCard)?.Families?.Contains(family) == true;
    }

    /// <summary>
    /// The behavior to execute at the start of a turn, which offers the player the choice to skip the draw phase for a card from the specified family.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The player's current set of cards.</param>
    /// <param name="playerStatus">The current player's status.</param>
    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerStatus playerStatus)
    {
        base.OnTurnStart(canvas, playerCards, playerStatus);

        var validCards = playerCards.Deck.Where(IsFromFamily).ToList();
        validCards.AddRange(playerCards.YellowCards.Where(IsFromFamily));

        if (validCards.Count > 0)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_TITLE),
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_SKIP_DRAW_FOR_FISTILAND_CARD_MESSAGE),
                showPositiveButton: true,
                showNegativeButton: true,
                positiveAction: () =>
                {
                    var config = new CardSelectorConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_CARD_FROM_DECK_YELLOW),
                        validCards,
                        showOkButton: true,
                        okAction: (selectedCard) =>
                        {
                            if (selectedCard is InGameInvocationCard invocationCard)
                            {
                                if (playerCards.YellowCards.Contains(invocationCard))
                                {
                                    playerCards.YellowCards.Remove(invocationCard);
                                    playerCards.HandCards.Add(invocationCard);
                                }
                                else if (playerCards.Deck.Contains(invocationCard))
                                {
                                    playerCards.Deck.Remove(invocationCard);
                                    playerCards.HandCards.Add(invocationCard);
                                }

                                playerCards.SkipCurrentDraw = true;
                            }
                            else
                            {
                                DisplayOkMessage(canvas);
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
}