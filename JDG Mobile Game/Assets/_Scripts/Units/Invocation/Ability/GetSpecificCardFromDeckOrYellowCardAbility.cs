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
        bool hasCardInYellowTrash = playerCards.yellowTrash.Exists(card => card.Title == cardName);
        if (hasCardInDeck || hasCardInYellowTrash)
        {
            GameObject messageBox =
                MessageBox.CreateSimpleMessageBox(
                    canvas,
                    "Aller chercher carte",
                    "Veux-tu aller chercher directement " + cardName + " dans ton deck ou ta poubelle jaune?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                if (hasCardInDeck)
                {
                    InGameCard card = playerCards.deck.Find(card => card.Title == cardName);
                    playerCards.deck.Remove(card);
                    playerCards.handCards.Add(card);
                }
                else
                {
                    InGameCard card = playerCards.yellowTrash.Find(card => card.Title == cardName);
                    playerCards.deck.Remove(card);
                    playerCards.handCards.Add(card);
                }

                Object.Destroy(messageBox);
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () => { Object.Destroy(messageBox); };
        }
    }
}