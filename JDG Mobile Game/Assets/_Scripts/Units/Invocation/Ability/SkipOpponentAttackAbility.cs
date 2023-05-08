using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SkipOpponentAttackAbility : Ability
{
    public SkipOpponentAttackAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    protected static void DisplayOkMessage(Transform canvas, string message, GameObject messageBox,
        GameObject messageBox2)
    {
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(canvas, "Information",
            message);
        messageBox1.GetComponent<MessageBox>().OkAction = () =>
        {
            Object.Destroy(messageBox);
            Object.Destroy(messageBox1);
            Object.Destroy(messageBox2);
        };
    }

    private void ApplyEffect(Transform canvas, PlayerCards opponentPlayerCard)
    {
        if (opponentPlayerCard.invocationCards.Count > 0)
        {
            GameObject messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Choix",
                "Veux-tu faire sauter la phase d'attaque d'une carte adverse ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                messageBox.SetActive(false);
                List<InGameCard> list = new List<InGameCard>(opponentPlayerCard.invocationCards);
                GameObject messageBox1 =
                    MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix carte à empêcher l'attaque", list);
                messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    InGameInvocationCard invocationCard =
                        messageBox1.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                    if (invocationCard == null)
                    {
                        DisplayOkMessage(canvas, "Aucune carte n'a perdu sa capacité d'attaque", messageBox,
                            messageBox1);
                    }
                    else
                    {
                        invocationCard.SetRemainedAttackThisTurn(0); // TODO Examine maybe not working
                        DisplayOkMessage(canvas, invocationCard.Title + " ne pourra pas attaquer au prochain tour",
                            messageBox, messageBox1);
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


    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyEffect(canvas, opponentPlayerCards);
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyEffect(canvas, opponentPlayerCards);
    }
}