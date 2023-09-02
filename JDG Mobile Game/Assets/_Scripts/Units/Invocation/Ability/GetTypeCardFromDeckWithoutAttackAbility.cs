using System.Collections.Generic;
using Cards;
using UnityEngine;

public class GetTypeCardFromDeckWithoutAttackAbility : Ability
{
    private CardType type;

    public GetTypeCardFromDeckWithoutAttackAbility(AbilityName name, string description, CardType cardType)
    {
        Name = name;
        Description = description;
        type = cardType;
    }

    protected static void DisplayOkMessage(Transform canvas)
    {

        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_CARD_GET_FROM_DECK_MESSAGE),
            showOkButton: true,
            okAction: () =>
            {
            }
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool hasCardInDeck = playerCards.Deck.Exists(card => card.Type == type);
        if (hasCardInDeck)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_GET_TYPE_CARD_MESSAGE),
                    type.ToName()
                ),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    List<InGameCard> cards = playerCards.Deck.FindAll(card => card.Type == type);
                    var config = new CardSelectorConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_DEFAULT_CHOICE_CARD),
                        cards,
                        showNegativeButton: true,
                        showPositiveButton: true,
                        positiveAction: (card) =>
                        {
                            if (card == null)
                            {
                                DisplayOkMessage(canvas);
                            }
                            else
                            {
                                playerCards.HandCards.Add(card);
                                playerCards.Deck.Remove(card);
                                invocationCard.SetRemainedAttackThisTurn(0);
                            }
                        },
                        negativeAction: () =>
                        {
                            DisplayOkMessage(canvas);
                        }
                    );
                    CardSelector.Instance.CreateCardSelection(canvas, config);
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }
}