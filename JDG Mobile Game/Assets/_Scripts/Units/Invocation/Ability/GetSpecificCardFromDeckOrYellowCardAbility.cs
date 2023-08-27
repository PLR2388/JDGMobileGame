using System.Linq;
using Cards;
using UnityEngine;

public class GetSpecificCardFromDeckOrYellowCardAbility : GetSpecificCardFromDeckAbility
{
    public GetSpecificCardFromDeckOrYellowCardAbility(AbilityName name, string description, string cardName) : base(
        name, description, cardName)
    {
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool hasCardInDeck = playerCards.deck.Exists(card => card.Title == cardName);
        bool hasCardInYellowTrash = playerCards.yellowCards.Any(card => card.Title == cardName);
        if (hasCardInDeck || hasCardInYellowTrash)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_GET_SPECIFIC_CARD_IN_DECK_AND_YELLOW_MESSAGE),
                    cardName
                ),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    if (hasCardInDeck)
                    {
                        InGameCard card = playerCards.deck.Find(card => card.Title == cardName);
                        playerCards.deck.Remove(card);
                        playerCards.handCards.Add(card);
                    }
                    else
                    {
                        InGameCard card = playerCards.yellowCards.First(card => card.Title == cardName);
                        playerCards.deck.Remove(card);
                        playerCards.handCards.Add(card);
                    }
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }
}