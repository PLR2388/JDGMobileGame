using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class DestroyFieldAtkDefAttackConditionAbility : Ability
{
    private string cardName;
    private int divideAtkFactor;
    private int divideDefFactor;

    public DestroyFieldAtkDefAttackConditionAbility(AbilityName name, string description, string cardName,
        int divideAtkFactor, int divideDefFactor)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
        this.divideAtkFactor = divideAtkFactor;
        this.divideDefFactor = divideDefFactor;
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

    private void DestroyField(PlayerCards playerCards, PlayerCards opponentPlayerCards, InGameCard fieldCard)
    {
        if (playerCards.field == fieldCard)
        {
            playerCards.yellowTrash.Add(fieldCard);
            playerCards.field = null;
        }
        else
        {
            opponentPlayerCards.yellowTrash.Add(fieldCard);
            opponentPlayerCards.field = null;
        }

        InGameInvocationCard applyNonBonusInvocationCard =
            playerCards.invocationCards.Find(invocationCard => invocationCard.Title == cardName);
        applyNonBonusInvocationCard.Attack /= divideAtkFactor;
        applyNonBonusInvocationCard.Defense /= divideDefFactor;
        applyNonBonusInvocationCard.SetRemainedAttackThisTurn(0);
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        string condition = divideAtkFactor > 1 ? "ton attaque" : "ta défense";
        int value = divideAtkFactor > 1 ? divideAtkFactor : divideDefFactor;

        GameObject messageBox =
            MessageBox.CreateSimpleMessageBox(canvas, "Question",
                "Veux-tu diviser " + condition + "par " + value + " pour détruire un terrain ?");
        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            List<InGameCard> fieldCardsToDestroy = new List<InGameCard>();
            if (playerCards.field != null)
            {
                fieldCardsToDestroy.Add(playerCards.field);
            }

            if (opponentPlayerCards.field != null)
            {
                fieldCardsToDestroy.Add(opponentPlayerCards.field);
            }

            if (fieldCardsToDestroy.Count == 1)
            {
                DestroyField(playerCards, opponentPlayerCards, fieldCardsToDestroy[0]);
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
                    }
                };
                messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                {
                    DisplayOkMessage(canvas, messageBox, messageBox1);
                };
            }
        };
        messageBox.GetComponent<MessageBox>().NegativeAction = () =>
        {
            Object.Destroy(messageBox);
        };
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
    }
}