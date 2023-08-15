using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SacrificeToInvokeAbility : Ability
{

    public SacrificeToInvokeAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
        IsAction = true;
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

    public override bool IsActionPossible(InGameInvocationCard currentCard, PlayerCards playerCards,
        PlayerCards opponentCards)
    {
        return playerCards.yellowCards.Any(card =>
            card.Type == CardType.Invocation &&
            card.Collector == false);
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameCard> invocationCards = playerCards.yellowCards.TakeWhile(card =>
            card.Type == CardType.Invocation &&
            card.Collector == false).ToList();
        if (invocationCards.Count > 0)
        {
            GameObject messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Question",
                "Veux-tu sacrifier " + invocationCard.Title + " pour invoquer une carte non-brillante depuis la poubelle jaune ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                messageBox.SetActive(false);
                GameObject messageBox1 =
                    MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix carte à invoquer", invocationCards);
                messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    InGameInvocationCard newlyInvoke =
                        messageBox1.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                    if (newlyInvoke == null)
                    {
                        DisplayOkMessage(canvas, messageBox, messageBox1);
                    }
                    else
                    {
                        playerCards.invocationCards.Remove(invocationCard);
                        playerCards.yellowCards.Add(invocationCard);
                        playerCards.yellowCards.Remove(newlyInvoke);
                        playerCards.invocationCards.Add(newlyInvoke);
                        Object.Destroy(messageBox);
                        Object.Destroy(messageBox1);
                    }
                };
                messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                {
                    DisplayOkMessage(canvas, messageBox1, messageBox);
                };
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () => { Object.Destroy(messageBox); };
        }
    }

    public override void OnCardActionTouched(Transform canvas, InGameInvocationCard currentCard,
        PlayerCards playerCards,
        PlayerCards opponentCards)
    {
        ApplyEffect(canvas, playerCards, opponentCards);
    }
}