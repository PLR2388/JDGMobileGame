using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookHandCardsEffectAbility : EffectAbility
{
    public LookHandCardsEffectAbility(EffectAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        return opponentPlayerCard.handCards.Count > 0;
    }

    private void DisplayOkMessage(Transform canvas)
    {
        MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte");
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var messageBox =
            MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte de l'adversaire", opponentPlayerCard.handCards);
        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            if (playerCards.handCards.Count > 0)
            {
                DisplayChoiceAboutHandCards(canvas, playerCards, opponentPlayerCard, messageBox);
            }
            else
            {
                Object.Destroy(messageBox);
            }
         
        };
        messageBox.GetComponent<MessageBox>().NegativeAction = () =>
        {
            if (playerCards.handCards.Count > 0)
            {
                DisplayChoiceAboutHandCards(canvas, playerCards, opponentPlayerCard, messageBox);
            }
            else
            {
                Object.Destroy(messageBox);
            }
        };
    }

    private void DisplayChoiceAboutHandCards(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        GameObject messageBox)
    {
        var messageBox1 = MessageBox.CreateSimpleMessageBox(canvas, "Question",
            "Est-ce que tu veux défausser une carte de ton adversaires en défaussant aussi une de tes cartes ?");
        messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var messageBox2 = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Carte de l'adversaire à défausser", opponentPlayerCard.handCards);
            messageBox2.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var opponentCard = messageBox2.GetComponent<MessageBox>().GetSelectedCard();
                if (opponentCard == null)
                {
                    DisplayOkMessage(canvas);
                }
                else
                {
                    var messageBox3 =
                        MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte à défausser",
                            playerCards.handCards);
                    messageBox3.GetComponent<MessageBox>().PositiveAction = () =>
                    {
                        var playerCard = messageBox3.GetComponent<MessageBox>().GetSelectedCard();
                        if (playerCard == null)
                        {
                            DisplayOkMessage(canvas);
                        }
                        else
                        {
                            opponentPlayerCard.handCards.Remove(opponentCard);
                            opponentPlayerCard.yellowCards.Add(opponentCard);
                            playerCards.handCards.Remove(playerCard);
                            playerCards.yellowCards.Add(playerCard);
                            Object.Destroy(messageBox);
                            Object.Destroy(messageBox1);
                            Object.Destroy(messageBox2);
                            Object.Destroy(messageBox3);
                        }
                    };
                    messageBox3.GetComponent<MessageBox>().NegativeAction = () => { DisplayOkMessage(canvas); };
                }
            };
            messageBox2.GetComponent<MessageBox>().NegativeAction = () => { DisplayOkMessage(canvas); };
        };
        messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
        {
            Object.Destroy(messageBox);
            Object.Destroy(messageBox1);
        };
    }
}