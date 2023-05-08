using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GetTypeCardFromDeckWithoutAttackAbility : Ability
{
    private CardType type;
    private string cardName;
    
    public GetTypeCardFromDeckWithoutAttackAbility(AbilityName name, string description, string originName, CardType cardType)
    {
        Name = name;
        Description = description;
        type = cardType;
        cardName = originName;
    }
    
    protected static void DisplayOkMessage(Transform canvas, GameObject messageBox, GameObject messageBox2)
    {
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(canvas, "Information",
            "Aucune carte n'a été récupéré du deck.");
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
                    "Aller chercher carte",
                    "Veux-tu aller chercher directement une carte de type " + type + " dans ton deck ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                messageBox.SetActive(false);
                List<InGameCard> cards = playerCards.deck.FindAll(card => card.Type == type);
                GameObject messageBox1 = MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix carte", cards);
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
                        playerCards.invocationCards.Find(card => card.Title == cardName).SetRemainedAttackThisTurn(0);
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
