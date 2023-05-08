using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SacrificeToInvokeAbility : Ability
{
    private string cardName;

    public SacrificeToInvokeAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }
    
    protected static void DisplayOkMessage(Transform canvas, GameObject messageBox, GameObject messageBox2)
    {
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(canvas, "Information",
            "Aucune carte n'a été invoqué.");
        messageBox1.GetComponent<MessageBox>().OkAction = () =>
        {
            Object.Destroy(messageBox);
            Object.Destroy(messageBox1);
            Object.Destroy(messageBox2);
        };
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameCard> invocationCards = playerCards.yellowTrash.FindAll(card =>
            card.Type == CardType.Invocation &&
            card.Collector == false);
        if (invocationCards.Count > 0)
        {
            GameObject messageBox = MessageBox.CreateSimpleMessageBox(canvas,"Question",
                "Veux-tu sacrifier " + cardName + " pour invoquer une carte non-brillante depuis la poubelle jaune ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                messageBox.SetActive(false);
                GameObject messageBox1 =
                    MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix carte à invoquer", invocationCards);
                messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    InGameInvocationCard invocationCard = messageBox1.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                    if (invocationCard == null)
                    {
                        DisplayOkMessage(canvas, messageBox, messageBox1);
                    }
                    else
                    {
                        InGameInvocationCard currentCard =
                            playerCards.invocationCards.Find(card => card.Title == cardName);
                        playerCards.invocationCards.Remove(currentCard);
                        playerCards.yellowTrash.Add(currentCard);
                        playerCards.invocationCards.Add(invocationCard);
                        playerCards.yellowTrash.Remove(invocationCard);
                        Object.Destroy(messageBox);
                        Object.Destroy(messageBox1);
                    }
                };
                messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                {
                    DisplayOkMessage(canvas, messageBox1, messageBox);
                };
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                Object.Destroy(messageBox);
            };
        }
    }
}