using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability allowing a player to sacrifice cards based on certain attributes
/// such as minimum attack, minimum defense, card family, and a specific number of cards.
/// </summary>
public class SacrificeCardMinAtkMinDefFamilyNumberAbility : Ability
{
    private readonly float minAtk;
    private readonly float minDef;
    private readonly CardFamily family;
    private readonly int numberCard;

    /// <summary>
    /// Initializes a new instance of the <see cref="SacrificeCardMinAtkMinDefFamilyNumberAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="atk">The minimum attack required for a card to be sacrificed.</param>
    /// <param name="def">The minimum defense required for a card to be sacrificed.</param>
    /// <param name="cardFamily">The family the card must belong to.</param>
    /// <param name="number">The number of cards to be sacrificed.</param>
    public SacrificeCardMinAtkMinDefFamilyNumberAbility(AbilityName name, string description,
        float atk = 0f, float def = 0f, CardFamily cardFamily = CardFamily.Any, int number = 1)
    {
        Name = name;
        Description = description;
        minAtk = atk;
        minDef = def;
        family = cardFamily;
        numberCard = number;
    }

    /// <summary>
    /// Retrieves a list of valid invocation cards from the player's cards based on the ability's attributes.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <returns>A list of valid <see cref="InGameInvocationCard"/>.</returns>
    private List<InGameInvocationCard> GetValidInvocationCards(PlayerCards playerCards)
    {
        return playerCards.InvocationCards.Where(card =>
            card.Title != invocationCard.Title &&
            (card.Attack >= minAtk || card.Defense >= minDef) &&
            (family == CardFamily.Any || card.Families.Contains(family))).ToList();
    }

    /// <summary>
    /// Displays a message indicating the obligation of choosing a card/cards for sacrifice.
    /// </summary>
    /// <param name="canvas">The canvas to display the message on.</param>
    /// <param name="numberOfCards">The number of cards that need to be sacrificed.</param>
    private static void DisplayOkMessage(Transform canvas, int numberOfCards)
    {
        string message = numberOfCards == 1
            ? LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_SACRIFICE)
            : string.Format(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_SACRIFICES),
                numberOfCards
            );
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            message,
            showOkButton: true,
            okAction: () =>
            {
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    /// <summary>
    /// Applies the effect of the ability, sacrificing the valid cards based on the given attributes.
    /// </summary>
    /// <param name="canvas">The canvas to display any UI elements on.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent's player cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameInvocationCard> invocationCards = GetValidInvocationCards(playerCards);

        if (invocationCards.Count == numberCard)
        {
            foreach (var inGameInvocationCard in invocationCards)
            {
                playerCards.InvocationCards.Remove(inGameInvocationCard);
                playerCards.YellowCards.Add(inGameInvocationCard);
            }
        }
        else
        {
            List<InGameCard> cards = new List<InGameCard>(invocationCards);

            bool isMultipleSelected = numberCard > 1;
            string message = isMultipleSelected
                ? LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SACRIFICES)
                : LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SACRIFICE);

            var config = new CardSelectorConfig(
                message,
                cards,
                numberCardSelection: numberCard,
                showOkButton: true,
                okAction: (card) =>
                {
                    if (card is InGameInvocationCard inGameInvocationCard)
                    {
                        playerCards.InvocationCards.Remove(inGameInvocationCard);
                        playerCards.YellowCards.Add(inGameInvocationCard);
                    }
                    else
                    {
                        DisplayOkMessage(canvas, numberCard);
                    }
                },
                okMultipleAction: (inGameCards) =>
                {
                    if (inGameCards == null)
                    {
                        DisplayOkMessage(canvas, numberCard);
                    }
                    else if (inGameCards.Count == numberCard)
                    {
                        foreach (var inGameInvocationCard in inGameCards.Cast<InGameInvocationCard>())
                        {
                            playerCards.InvocationCards.Remove(inGameInvocationCard);
                            playerCards.YellowCards.Add(inGameInvocationCard);
                        }
                    }
                    else
                    {
                        DisplayOkMessage(canvas, numberCard);
                    }
                }
                );
            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
    }
}