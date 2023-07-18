using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;

public class GetCardFromDeckYellowEffectAbility : EffectAbility
{
    private bool fromDeck;
    private bool fromYellow;
    private int numberCards;

    public GetCardFromDeckYellowEffectAbility(EffectAbilityName name, string description, int numberCards,
        bool fromDeck = true, bool fromYellow = false)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
        this.fromDeck = fromDeck;
        this.fromYellow = fromYellow;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        var number = fromDeck ? playerCards.deck.Count : 0 + (fromYellow ? playerCards.yellowCards.Count : 0);
        return number > numberCards;
    }

    private void DisplayOkMessageBox(Transform canvas)
    {
        MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois prendre une carte");
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        var cards = new List<InGameCard>();
        if (fromDeck)
        {
            cards.AddRange(playerCards.deck);
        }

        if (fromYellow)
        {
            cards.AddRange(playerCards.yellowCards);
        }

        if (numberCards == 1)
        {
            var messageBox =
                MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix de la carte à aller chercher", cards);
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var card = messageBox.GetComponent<MessageBox>().GetSelectedCard();
                if (card == null)
                {
                    DisplayOkMessageBox(canvas);
                }
                else
                {
                    if (playerCards.deck.Contains(card))
                    {
                        playerCards.deck.Remove(card);
                    } else if (playerCards.yellowCards.Contains(card))
                    {
                        playerCards.yellowCards.Remove(card);
                    }
                    playerCards.handCards.Add(card);
                    Object.Destroy(messageBox);
                }
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () => { DisplayOkMessageBox(canvas); };
        }
    }
}