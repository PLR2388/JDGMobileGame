using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class LimitHandCardsEffectAbility : EffectAbility
{
    private int numberCards;

    public LimitHandCardsEffectAbility(EffectAbilityName name, string description, int numberCards)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus, PlayerStatus opponentPlayerStatus)
    {
        ApplyPowerLimitHandCard(canvas, playerCards);
        ApplyPowerLimitHandCard(canvas, opponentPlayerCard);
    }

    private void ApplyPowerLimitHandCard(Transform canvas, PlayerCards playerCards)
    {
        var numberCardPlayerCard = playerCards.handCards.Count;
        if (numberCardPlayerCard < numberCards)
        {
            // Draw numberCards - numberCardPlayerCard
            var size = playerCards.deck.Count;
            var number = Math.Min(numberCards - numberCardPlayerCard, playerCards.deck.Count);
            for (var i = 0; i < number; i++)
            {
                var c = playerCards.deck[size - 1 - i];
                playerCards.handCards.Add(c);
                playerCards.deck.RemoveAt(size - 1 - i);
            }
        }
        else if (numberCardPlayerCard > numberCards)
        {
            // Prompt messageBox to remove numberCardPlayerCard - numberCard cards
            var number = numberCardPlayerCard - numberCards;
            var messageBox = MessageBox.CreateMessageBoxWithCardSelector(
                canvas,
                "Joueur " + (playerCards.isPlayerOne ? "1" : "2") + " doit enlever " + number + " cartes de sa main",
                playerCards.handCards,
                multipleCardSelection: true,
                numberCardInSelection: number
            );

            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var cards = messageBox.GetComponent<MessageBox>().GetMultipleSelectedCards();
                if (cards.Count == number)
                {
                    foreach (var inGameCard in cards)
                    {
                        playerCards.yellowCards.Add(inGameCard);
                        playerCards.handCards.Remove(inGameCard);
                    }
                    Object.Destroy(messageBox);
                }
                else
                {
                    var messageBox1 = MessageBox.CreateOkMessageBox(canvas, "Attention",
                        "Tu dois selectionner " + number + " cartes.");
                    messageBox1.GetComponent<MessageBox>().OkAction = () =>
                    {
                        Object.Destroy(messageBox1);
                    };
                }
            };

            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                var messageBox1 = MessageBox.CreateOkMessageBox(canvas, "Attention", "Il faut enlever des cartes");
                messageBox1.GetComponent<MessageBox>().OkAction = () => { Object.Destroy(messageBox1); };
            };
        }
    }
}