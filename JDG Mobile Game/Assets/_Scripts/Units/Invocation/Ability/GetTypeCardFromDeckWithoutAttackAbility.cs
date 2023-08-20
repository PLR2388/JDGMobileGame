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

    protected static void DisplayOkMessage(Transform canvas, GameObject messageBox, GameObject messageBox2)
    {
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(
            canvas,
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_CARD_GET_FROM_DECK_MESSAGE)
        );
        messageBox1.GetComponent<MessageBox>().OkAction = () =>
        {
            Object.Destroy(messageBox);
            Object.Destroy(messageBox1);
            Object.Destroy(messageBox2);
        };
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool hasCardInDeck = playerCards.deck.Exists(card => card.Type == type);
        if (hasCardInDeck)
        {
            GameObject messageBox =
                MessageBox.CreateSimpleMessageBox(
                    canvas,
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                    string.Format(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_GET_TYPE_CARD_MESSAGE),
                        type.ToName()
                    )
                );
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                messageBox.SetActive(false);
                List<InGameCard> cards = playerCards.deck.FindAll(card => card.Type == type);
                GameObject messageBox1 = MessageBox.CreateMessageBoxWithCardSelector(
                    canvas,
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_DEFAULT_CHOICE_CARD),
                    cards
                );
                messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    InGameCard card = messageBox1.GetComponent<MessageBox>().GetSelectedCard();
                    if (card == null)
                    {
                        DisplayOkMessage(canvas, messageBox, messageBox1);
                    }
                    else
                    {
                        playerCards.handCards.Add(card);
                        playerCards.deck.Remove(card);
                        invocationCard.SetRemainedAttackThisTurn(0);
                        Object.Destroy(messageBox);
                        Object.Destroy(messageBox1);
                    }
                };
                messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                {
                    DisplayOkMessage(canvas, messageBox, messageBox1);
                };
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                Object.Destroy(messageBox);
            };
        }
    }
}