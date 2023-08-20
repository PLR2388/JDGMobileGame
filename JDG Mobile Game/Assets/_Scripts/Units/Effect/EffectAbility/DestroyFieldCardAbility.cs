
using System.Collections.Generic;
using Cards;
using UnityEngine;

public class DestroyFieldCardAbility : EffectAbility
{
    private float costHealth;

    public DestroyFieldCardAbility(EffectAbilityName name, string description, float cost)
    {
        Name = name;
        Description = description;
        costHealth = cost;
    }
    
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var cards = new List<InGameCard>();
        if (playerCards.FieldCard != null)
        {
            cards.Add(playerCards.FieldCard);
        }

        if (opponentPlayerCard.FieldCard != null)
        {
            cards.Add(opponentPlayerCard.FieldCard);
        }

        var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte terrain à détruire", cards);

        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var card = messageBox.GetComponent<MessageBox>().GetSelectedCard();
            if (card == null)
            {
                MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte !");
            }
            else
            {
                if (card.CardOwner == CardOwner.Player1)
                {
                    if (playerCards.isPlayerOne)
                    {
                        playerCards.yellowCards.Add(card);
                        playerCards.FieldCard = null;
                    }
                    else
                    {
                        opponentPlayerCard.yellowCards.Add(card);
                        opponentPlayerCard.FieldCard = null;
                    }
                }
                else
                {
                    if (playerCards.isPlayerOne)
                    {
                        opponentPlayerCard.yellowCards.Add(card);
                        opponentPlayerCard.FieldCard = null;
                    }
                    else
                    {
                        playerCards.yellowCards.Add(card);
                        playerCards.FieldCard = null;
                    }
                }
                playerStatus.ChangePv(-costHealth);
                Object.Destroy(messageBox);
            }
        };

        messageBox.GetComponent<MessageBox>().NegativeAction = () =>
        {
            MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte !");
        };
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards,
        PlayerStatus opponentPlayerStatus)
    {
        return playerCards.FieldCard != null || opponentPlayerCards.FieldCard != null;
    }
}