using System.Collections.Generic;
using System.Collections.ObjectModel;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class KillOpponentInvocationCardAbility : Ability
{
    public KillOpponentInvocationCardAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }
    
    protected static void DisplayOkMessage(Transform canvas, GameObject messageBox, GameObject messageBox2)
    {
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(canvas, "Information",
            "Aucune carte n'a été détruite.");
        messageBox1.GetComponent<MessageBox>().OkAction = () =>
        {
            Object.Destroy(messageBox);
            Object.Destroy(messageBox1);
            Object.Destroy(messageBox2);
        };
    }
    
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ObservableCollection<InGameInvocationCard> invocationCards = opponentPlayerCards.invocationCards;
        if (invocationCards.Count > 0)
        {
            GameObject messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Action",
                "Veux-tu détruire une carte invocation de l'adversaire ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                messageBox.SetActive(false);
                if (invocationCards.Count == 1)
                {
                    opponentPlayerCards.invocationCards.Remove(invocationCards[0]);
                    opponentPlayerCards.yellowTrash.Add(invocationCards[0]);
                }
                else
                {
                    GameObject messageBox1 =
                        MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte à détruire", new List<InGameCard>(invocationCards));
                    messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                    {
                        DisplayOkMessage(canvas, messageBox, messageBox1);
                    };
                    messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                    {
                        InGameInvocationCard invocationCard = messageBox1.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                        if (invocationCard == null)
                        {
                            DisplayOkMessage(canvas, messageBox, messageBox1);
                        }
                        else
                        {
                            opponentPlayerCards.invocationCards.Remove(invocationCard);
                            opponentPlayerCards.yellowTrash.Add(invocationCard);
                        }
                    };
                }
              
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                Object.Destroy(messageBox);
            };
        }
    }
}
