using Cards;
using UnityEngine;

public class GetSpecificCardFromDeckAbility : Ability
{
    protected string cardName;

    public GetSpecificCardFromDeckAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }

    protected void GetSpecificCard(Transform canvas, PlayerCards playerCards)
    {
        bool hasCardInDeck = playerCards.deck.Exists(card => card.Title == cardName);
        if (hasCardInDeck)
        {
            GameObject messageBox =
                MessageBox.CreateSimpleMessageBox(
                    canvas,
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                    string.Format(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_GET_SPECIFIC_CARD_IN_DECK_MESSAGE),
                        cardName
                    )
                );
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                InGameCard card = playerCards.deck.Find(card => card.Title == cardName);
                playerCards.deck.Remove(card);
                playerCards.handCards.Add(card);
                Object.Destroy(messageBox);
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                Object.Destroy(messageBox);
            };
        }
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        GetSpecificCard(canvas, playerCards);
    }

}