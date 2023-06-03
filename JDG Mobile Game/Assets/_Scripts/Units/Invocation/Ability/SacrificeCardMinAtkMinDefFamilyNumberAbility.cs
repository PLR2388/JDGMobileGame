using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SacrificeCardMinAtkMinDefFamilyNumberAbility : Ability
{
    private string originCardName;
    private float minAtk;
    private float minDef;
    private CardFamily family;
    private int numberCard;

    public SacrificeCardMinAtkMinDefFamilyNumberAbility(AbilityName name, string description, string cardName,
        float atk = 0f, float def = 0f, CardFamily cardFamily = CardFamily.Any, int number = 1)
    {
        Name = name;
        Description = description;
        originCardName = cardName;
        minAtk = atk;
        minDef = def;
        family = cardFamily;
        numberCard = number;
    }

    private List<InGameInvocationCard> GetValidInvocationCards(PlayerCards playerCards)
    {
        return playerCards.invocationCards.Where(card =>
            card.Title != originCardName &&
            (card.Attack >= minAtk || card.Defense >= minDef) &&
            (family == CardFamily.Any || card.Families.Contains(family))).ToList();
    }

    private static void DisplayOkMessage(Transform canvas, GameObject messageBox, int numberOfCards)
    {
        string message = numberOfCards == 1
            ? "Tu dois sélectionner une carte à sacrifier"
            : "Tu dois sélectionner " + numberOfCards + " cartes à sacrifier";
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(canvas, "Attention",
            message);
        messageBox1.GetComponent<MessageBox>().OkAction = () =>
        {
            messageBox.SetActive(true);
            Object.Destroy(messageBox1);
        };
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameInvocationCard> invocationCards = GetValidInvocationCards(playerCards);

        if (invocationCards.Count == numberCard)
        {
            foreach (var invocationCard in invocationCards)
            {
                playerCards.invocationCards.Remove(invocationCard);
                playerCards.yellowTrash.Add(invocationCard);
            }
        }
        else
        {
            List<InGameCard> cards = new List<InGameCard>(invocationCards);

            bool isMultipleSelected = numberCard > 1;
            string message = isMultipleSelected ? "Choisis les cartes à sacrifier" : "Choisis la carte à sacrifier";
            
            GameObject messageBox =
                MessageBox.CreateMessageBoxWithCardSelector(canvas, message, cards, multipleCardSelection: isMultipleSelected, numberCardInSelection: numberCard);
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                if (isMultipleSelected)
                {
                    List<InGameCard> cards = messageBox.GetComponent<MessageBox>().GetMultipleSelectedCards();
                    if (cards == null)
                    {
                        DisplayOkMessage(canvas, messageBox, numberCard);
                    }
                    else if (cards.Count == numberCard)
                    {
                        foreach (InGameInvocationCard invocationCard in cards)
                        {
                            playerCards.invocationCards.Remove(invocationCard);
                            playerCards.yellowTrash.Add(invocationCard);
                        }
                        Object.Destroy(messageBox);
                    }
                    else
                    {
                        DisplayOkMessage(canvas, messageBox, numberCard);
                    }
                }
                else
                {
                    InGameInvocationCard invocationCard = messageBox.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                    if (invocationCard == null)
                    {
                        DisplayOkMessage(canvas, messageBox, numberCard);
                    }
                    else
                    {
                        playerCards.invocationCards.Remove(invocationCard);
                        playerCards.yellowTrash.Add(invocationCard);
                        Object.Destroy(messageBox);
                    }
                }
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                DisplayOkMessage(canvas, messageBox, numberCard);
            };
        }
    }
}