
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
        if (playerCards.field != null)
        {
            cards.Add(playerCards.field);
        }

        if (opponentPlayerCard.field != null)
        {
            cards.Add(opponentPlayerCard.field);
        }

        var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte terrain à détruire", cards);

        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var card = messageBox.GetComponent<MessageBox>().GetSelectedCard();
            if (card == null)
            {
                MessageBox.CreateOkMessageBox(canvas, "Information", "Tu dois sélectionner une carte");
            }
            else
            {
                if (card.CardOwner == CardOwner.Player1)
                {
                    if (playerCards.isPlayerOne)
                    {
                        playerCards.yellowCards.Add(card);
                        playerCards.field = null;
                    }
                    else
                    {
                        opponentPlayerCard.yellowCards.Add(card);
                        opponentPlayerCard.field = null;
                    }
                }
                else
                {
                    if (playerCards.isPlayerOne)
                    {
                        opponentPlayerCard.yellowCards.Add(card);
                        opponentPlayerCard.field = null;
                    }
                    else
                    {
                        playerCards.yellowCards.Add(card);
                        playerCards.field = null;
                    }
                }
                playerStatus.ChangePv(-costHealth);
                Object.Destroy(messageBox);
            }
        };

        messageBox.GetComponent<MessageBox>().NegativeAction = () =>
        {
            MessageBox.CreateOkMessageBox(canvas, "Information", "Tu dois sélectionner une carte");
        };
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards,
        PlayerStatus opponentPlayerStatus)
    {
        return playerCards.field != null || opponentPlayerCards.field != null;
    }
}