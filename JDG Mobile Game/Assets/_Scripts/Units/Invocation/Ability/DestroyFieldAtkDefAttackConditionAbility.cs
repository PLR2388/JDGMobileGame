using System.Collections.Generic;
using Cards;
using UnityEngine;

public class DestroyFieldAtkDefAttackConditionAbility : Ability
{
    private int divideAtkFactor;
    private int divideDefFactor;

    public DestroyFieldAtkDefAttackConditionAbility(AbilityName name, string description,
        int divideAtkFactor, int divideDefFactor)
    {
        Name = name;
        Description = description;
        this.divideAtkFactor = divideAtkFactor;
        this.divideDefFactor = divideDefFactor;
    }

    private static void DisplayOkMessage(Transform canvas, GameObject messageBox, GameObject messageBox2)
    {
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(canvas, "Information",
            "Aucune carte n'a été détruite.");
        messageBox1.GetComponent<MessageBox>().OkAction = () =>
        {
            Object.Destroy(messageBox);
            Object.Destroy(messageBox1);
            if (messageBox2 != null)
            {
                Object.Destroy(messageBox2);
            }
        };
    }

    private void DestroyField(PlayerCards playerCards, PlayerCards opponentPlayerCards, InGameCard fieldCard)
    {
        if (playerCards.FieldCard == fieldCard)
        {
            playerCards.yellowCards.Add(fieldCard);
            playerCards.FieldCard = null;
        }
        else
        {
            opponentPlayerCards.yellowCards.Add(fieldCard);
            opponentPlayerCards.FieldCard = null;
        }
        
        invocationCard.Attack /= divideAtkFactor;
        invocationCard.Defense /= divideDefFactor;
        invocationCard.SetRemainedAttackThisTurn(0);
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        string condition = divideAtkFactor > 1 ? "ton attaque" : "ta défense";
        int value = divideAtkFactor > 1 ? divideAtkFactor : divideDefFactor;

        GameObject messageBox =
            MessageBox.CreateSimpleMessageBox(canvas, "Question",
                "Veux-tu diviser " + condition + " par " + value + " pour détruire un terrain ?");
        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            List<InGameCard> fieldCardsToDestroy = new List<InGameCard>();
            if (playerCards.FieldCard != null)
            {
                fieldCardsToDestroy.Add(playerCards.FieldCard);
            }

            if (opponentPlayerCards.FieldCard != null)
            {
                fieldCardsToDestroy.Add(opponentPlayerCards.FieldCard);
            }

            if (fieldCardsToDestroy.Count == 1)
            {
                DestroyField(playerCards, opponentPlayerCards, fieldCardsToDestroy[0]);
                Object.Destroy(messageBox);
            } else if (fieldCardsToDestroy.Count > 1)
            {
                GameObject messageBox1 =
                    MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix terrain à détruire", fieldCardsToDestroy);
                messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    InGameCard fieldCard = messageBox1.GetComponent<MessageBox>().GetSelectedCard();
                    if (fieldCard == null)
                    {
                        DisplayOkMessage(canvas, messageBox1, messageBox);
                    }
                    else
                    {
                        DestroyField(playerCards, opponentPlayerCards, fieldCard);
                        Object.Destroy(messageBox);
                        Object.Destroy(messageBox1);
                    }
                };
                messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                {
                    DisplayOkMessage(canvas, messageBox, messageBox1);
                };
            }
            else
            {
                DisplayOkMessage(canvas, messageBox, null);
            }
        };
        messageBox.GetComponent<MessageBox>().NegativeAction = () =>
        {
            Object.Destroy(messageBox);
        };
    }
}