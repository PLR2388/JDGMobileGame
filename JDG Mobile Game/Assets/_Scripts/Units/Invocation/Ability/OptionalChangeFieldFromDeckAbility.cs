using System.Collections.Generic;
using Cards;
using UnityEngine;

public class OptionalChangeFieldFromDeckAbility : Ability
{
    public OptionalChangeFieldFromDeckAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }
    
    private static void DisplayOkMessageBox(Transform canvas, GameObject messageBox, GameObject messageBox1)
    {
        GameObject messageBox2 =
            MessageBox.CreateOkMessageBox(canvas, "Information", "Aucune carte terrain n'a été posé");
        messageBox2.GetComponent<MessageBox>().OkAction = () =>
        {
            Object.Destroy(messageBox);
            if (messageBox1 != null)
            {
                Object.Destroy(messageBox1);    
            }
            Object.Destroy(messageBox2);
        };
    }
    
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameCard> fieldCardsFromDeck = playerCards.deck.FindAll(card => card.Type == CardType.Field);
        if (fieldCardsFromDeck.Count > 0)
        {
            GameObject messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Question",
                "Veux-tu poser une carte terrain depuis ta pioche ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                messageBox.SetActive(false);
                GameObject messageBox1 =
                    MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte terrain à poser", fieldCardsFromDeck);
                messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    InGameFieldCard fieldCard = messageBox1.GetComponent<MessageBox>().GetSelectedCard() as InGameFieldCard;
                    if (fieldCard == null)
                    {
                        DisplayOkMessageBox(canvas, messageBox, messageBox1);
                    }
                    else
                    {
                        if (playerCards.FieldCard == null)
                        {
                            playerCards.FieldCard = fieldCard;
                        }
                        else
                        {
                            playerCards.yellowCards.Add(playerCards.FieldCard);
                            playerCards.FieldCard = fieldCard;
                        }
                    }
                    Object.Destroy(messageBox1);
                };
                messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                {
                    DisplayOkMessageBox(canvas, messageBox, messageBox1);
                };
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                DisplayOkMessageBox(canvas, messageBox, null);
            };
        }
    }
}
