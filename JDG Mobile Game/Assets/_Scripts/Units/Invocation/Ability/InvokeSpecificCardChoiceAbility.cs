using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class InvokeSpecificCardChoiceAbility : Ability
{
    private List<string> cardChoices;

    public InvokeSpecificCardChoiceAbility(AbilityName name, string description, List<string> cardNames)
    {
        Name = name;
        Description = description;
        cardChoices = cardNames;
    }

    protected static void DisplayOkMessage(Transform canvas, GameObject messageBox)
    {
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(
            canvas,
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_INVOKED_CARD_MESSAGE)
        );
        messageBox1.GetComponent<MessageBox>().OkAction = () =>
        {
            messageBox.SetActive(true);
            Object.Destroy(messageBox1);
        };
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool hasCardInDeck = playerCards.deck.Exists(card => cardChoices.Contains(card.Title));
        if (hasCardInDeck)
        {
            string cardNameChoiceString = cardChoices[0];
            for (int i = 1; i < cardChoices.Count; i++)
            {
                cardNameChoiceString += " ou " + cardChoices[i];
            }

            GameObject messageBox =
                MessageBox.CreateSimpleMessageBox(
                    canvas,
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                    string.Format(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_INVOKE_SPECIFIC_CARD_MESSAGE)
                        , cardNameChoiceString
                    )
                );
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                messageBox.SetActive(false);
                List<InGameCard> cards =
                    playerCards.deck.FindAll(card => cardChoices.Contains(card.Title));
                GameObject messageBox1 = MessageBox.CreateMessageBoxWithCardSelector(
                    canvas,
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_INVOKE),
                    cards);
                messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    InGameInvocationCard invocationCard = messageBox1.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                    if (invocationCard == null)
                    {
                        Object.Destroy(messageBox);
                        DisplayOkMessage(canvas, messageBox1);
                    }
                    else
                    {
                        playerCards.deck.Remove(invocationCard);
                        playerCards.invocationCards.Add(invocationCard);
                        Object.Destroy(messageBox);
                        Object.Destroy(messageBox1);
                    }
                };
                messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                {
                    Object.Destroy(messageBox);
                    DisplayOkMessage(canvas, messageBox1);
                };
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () => { Object.Destroy(messageBox); };
        }
    }
}