using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

public class ChangeFieldCardEffectAbility : EffectAbility
{
    public ChangeFieldCardEffectAbility(EffectAbilityName name, string description, int numberTurn)
    {
        Name = name;
        Description = description;
        NumberOfTurn = numberTurn;
    }

    public override bool CanUseEffect(PlayerCards playerCards,PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        return playerCards.deck.Any(card => card.Type == CardType.Field);
    }

    private void DisplayOkMessage(Transform canvas)
    {
        MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte terrain");
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        var fieldCards = playerCards.deck.Where(card => card.Type == CardType.Field).ToList();
        var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte terrain à joueur", fieldCards);
        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var fieldCard = (InGameFieldCard) messageBox.GetComponent<MessageBox>().GetSelectedCard();
            if (fieldCard == null)
            {
                DisplayOkMessage(canvas);
            }
            else
            {
                if (playerCards.FieldCard == null)
                {
                    playerCards.FieldCard = fieldCard;
                }
                else
                {
                    playerCards.yellowCards.Add(playerCards.FieldCard);
                    playerCards.FieldCard = fieldCard;
                }
                Object.Destroy(messageBox);
            }
        };
        messageBox.GetComponent<MessageBox>().NegativeAction = () => { DisplayOkMessage(canvas); };
    }
}