using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SacrificeCardMinAtkMinDefFamilyNumberAbility : Ability
{
    private float minAtk;
    private float minDef;
    private CardFamily family;
    private int numberCard;

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

    private List<InGameInvocationCard> GetValidInvocationCards(PlayerCards playerCards)
    {
        return playerCards.invocationCards.Where(card =>
            card.Title != invocationCard.Title &&
            (card.Attack >= minAtk || card.Defense >= minDef) &&
            (family == CardFamily.Any || card.Families.Contains(family))).ToList();
    }

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

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameInvocationCard> invocationCards = GetValidInvocationCards(playerCards);

        if (invocationCards.Count == numberCard)
        {
            foreach (var invocationCard in invocationCards)
            {
                playerCards.invocationCards.Remove(invocationCard);
                playerCards.yellowCards.Add(invocationCard);
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
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: (card) =>
                {
                    if (card is InGameInvocationCard invocationCard)
                    {
                        playerCards.invocationCards.Remove(invocationCard);
                        playerCards.yellowCards.Add(invocationCard);
                    }
                    else
                    {
                        DisplayOkMessage(canvas, numberCard);
                    }
                },
                positiveMultipleAction: (cards) =>
                {
                    if (cards == null)
                    {
                        DisplayOkMessage(canvas, numberCard);
                    }
                    else if (cards.Count == numberCard)
                    {
                        foreach (InGameInvocationCard invocationCard in cards)
                        {
                            playerCards.invocationCards.Remove(invocationCard);
                            playerCards.yellowCards.Add(invocationCard);
                        }
                    }
                    else
                    {
                        DisplayOkMessage(canvas, numberCard);
                    }
                },
                negativeAction: () =>
                {
                    DisplayOkMessage(canvas, numberCard);
                }
                );
            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
    }
}