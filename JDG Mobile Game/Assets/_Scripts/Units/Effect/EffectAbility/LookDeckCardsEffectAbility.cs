using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;

public class LookDeckCardsEffectAbility : EffectAbility
{
    private int numberCards;

    public LookDeckCardsEffectAbility(EffectAbilityName name, string description, int numberCards)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        return playerCards.deck.Count > 0 || opponentPlayerCard.deck.Count > 0;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Choix",
            "Veux-tu voir les cartes de ton deck (Oui) ou les cartes de ton adversaires (Non) ?");
        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            DisplayAndOrderCardMessageBox(canvas, playerCards, messageBox);
        };
        messageBox.GetComponent<MessageBox>().NegativeAction = () =>
        {
            DisplayAndOrderCardMessageBox(canvas, opponentPlayerCard, messageBox);
        };
    }

    private void DisplayAndOrderCardMessageBox(Transform canvas, PlayerCards playerCards, GameObject messageBox)
    {
        var deck = playerCards.deck;
        if (deck.Count >= numberCards)
        {
            var shortList = new List<InGameCard>();
            for (var i = 0; i < numberCards; i++)
            {
                shortList.Add(deck[deck.Count - 1 - i]);
            }

            var messageBox1 = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Change l'ordre des cartes (Non si pas voulu)", shortList, numberCardInSelection: numberCards,
                multipleCardSelection: true,
                displayOrder: true);
            messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var selectedCards = messageBox1.GetComponent<MessageBox>().GetMultipleSelectedCards();
                if (selectedCards.Count == numberCards)
                {
                    for (var i = 0; i < numberCards; i++)
                    {
                        playerCards.deck.Remove(selectedCards[i]);
                    }

                    for (var i = numberCards - 1; i >= 0; i--)
                    {
                        playerCards.deck.Add(selectedCards[i]);
                    }

                    Object.Destroy(messageBox);
                    Object.Destroy(messageBox1);
                }
                else
                {
                    MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois ordonner toutes les cartes !");
                }
            };
            messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
            {
                Object.Destroy(messageBox);
                Object.Destroy(messageBox1);
            };
        }
        else
        {
            var messageBox1 = MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Change l'ordre des cartes (Non si pas voulu)", deck, numberCardInSelection: deck.Count,
                displayOrder: true);
            messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var selectedCards = messageBox1.GetComponent<MessageBox>().GetMultipleSelectedCards();
                if (selectedCards.Count == deck.Count)
                {
                    for (var i = 0; i < deck.Count; i++)
                    {
                        playerCards.deck.Remove(selectedCards[i]);
                    }

                    for (var i = deck.Count - 1; i >= 0; i--)
                    {
                        playerCards.deck.Add(selectedCards[i]);
                    }

                    Object.Destroy(messageBox);
                    Object.Destroy(messageBox1);
                }
                else
                {
                    MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois ordonner toutes les cartes !");
                }
            };
            messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
            {
                Object.Destroy(messageBox);
                Object.Destroy(messageBox1);
            };
        }
    }
}