using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GetFamilyInDeckAbility : Ability
{
    private CardFamily family;

    public GetFamilyInDeckAbility(AbilityName name, string description, CardFamily cardFamily)
    {
        Name = name;
        Description = description;
        family = cardFamily;
    }


    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        GameObject messageBox = MessageBox.CreateSimpleMessageBox(
            canvas,
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
            string.Format(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_GET_CARD_FROM_FAMILY_MESSAGE),
                family.ToName()
            )
        );
        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            messageBox.SetActive(false);

            List<InGameCard> familyCards = playerCards.deck.FindAll(card =>
                card.Type == CardType.Invocation && ((InGameInvocationCard)card).Families.Contains(family));

            GameObject messageBox1 =
                MessageBox.CreateMessageBoxWithCardSelector(
                    canvas,
                    string.Format(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_FAMILY_CARD),
                        family.ToName()
                    ),
                    familyCards);
            messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
            {
                InGameCard card = messageBox1.GetComponent<MessageBox>().GetSelectedCard();
                if (card == null)
                {
                    messageBox1.SetActive(false);
                    GameObject messageBox2 =
                        MessageBox.CreateOkMessageBox(
                            canvas,
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARD)
                        );
                    messageBox2.GetComponent<MessageBox>().OkAction = () =>
                    {
                        messageBox1.SetActive(true);
                        Object.Destroy(messageBox2);
                    };
                }
                else
                {
                    playerCards.deck.Remove(card);
                    playerCards.handCards.Add(card);
                    Object.Destroy(messageBox);
                    Object.Destroy(messageBox1);
                }
            };
            messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
            {
                Object.Destroy(messageBox);
                Object.Destroy(messageBox1);
            };
        };
        messageBox.GetComponent<MessageBox>().NegativeAction = () => { Object.Destroy(messageBox); };
    }
}