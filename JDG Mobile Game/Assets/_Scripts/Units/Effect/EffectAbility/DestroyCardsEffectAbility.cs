using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class DestroyCardsEffectAbility : EffectAbility
{
    private int numbers; // 0 = ALL
    private bool mustThrowFirstDeck;
    private bool mustSacrificeInvocation;
    private bool mustThrowHandCard;

    public DestroyCardsEffectAbility(EffectAbilityName name, string description, int numberCards,
        bool mustThrowFirstDeck = false, bool mustSacrificeInvocation = false, bool mustThrowHandCard = false)
    {
        Name = name;
        Description = description;
        numbers = numberCards;
        this.mustThrowFirstDeck = mustThrowFirstDeck;
        this.mustSacrificeInvocation = mustSacrificeInvocation;
        this.mustThrowHandCard = mustThrowHandCard;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerStatus opponentPlayerStatus)
    {
        bool canThrowFirstDeck = !mustThrowFirstDeck || playerCards.deck.Count > 0;
        bool canSacrificeInvocation = !mustSacrificeInvocation || playerCards.invocationCards.Count > 0;
        bool canThrowHandCard = !mustThrowHandCard || playerCards.handCards.Count > 0;
        return canThrowFirstDeck && canSacrificeInvocation && canThrowHandCard;
    }

    private void DisplayOkMessage(Transform canvas)
    {
        MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte");
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        if (numbers == 0)
        {
            // Only case for the moment
            if (mustThrowFirstDeck)
            {
                var cardDeck = playerCards.deck[playerCards.deck.Count - 1];
                playerCards.deck.Remove(cardDeck);
                playerCards.yellowCards.Add(cardDeck);
            }

            if (mustSacrificeInvocation && mustThrowHandCard)
            {
                // Only case for the moment
                var invocationCards = new List<InGameCard>(playerCards.invocationCards);
                var handCards = playerCards.handCards;
                var messageBox =
                    MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte à sacrifier", invocationCards);

                messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    var invocationCard = (InGameInvocationCard) messageBox.GetComponent<MessageBox>().GetSelectedCard();
                    if (invocationCard == null)
                    {
                        DisplayOkMessage(canvas);
                    }
                    else
                    {
                        var messageBox1 =
                            MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte à enlever de la main",
                                handCards);
                        messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                        {
                            var handCard = messageBox1.GetComponent<MessageBox>().GetSelectedCard();
                            if (handCard == null)
                            {
                                DisplayOkMessage(canvas);
                            }
                            else
                            {
                                playerCards.invocationCards.Remove(invocationCard);
                                playerCards.handCards.Remove(handCard);
                                playerCards.yellowCards.Add(invocationCard);
                                playerCards.yellowCards.Add(handCard);

                                var copyInvocationCards = playerCards.invocationCards.ToList();
                                var copyOpponentInvocationCards = opponentPlayerCard.invocationCards.ToList();
                                var copyEffectCards = playerCards.effectCards.ToList();
                                var copyOpponentEffectCards = opponentPlayerCard.effectCards.ToList();

                                foreach (var inGameInvocationCard in copyInvocationCards)
                                {
                                    playerCards.invocationCards.Remove(inGameInvocationCard);
                                    playerCards.yellowCards.Add(inGameInvocationCard);
                                }
                                
                                foreach (var inGameInvocationCard in copyOpponentInvocationCards)
                                {
                                    opponentPlayerCard.invocationCards.Remove(inGameInvocationCard);
                                    playerCards.yellowCards.Add(inGameInvocationCard);
                                }
                                
                                foreach (var inGameEffectCard in copyEffectCards)
                                {
                                    playerCards.effectCards.Remove(inGameEffectCard);
                                    playerCards.effectCards.Add(inGameEffectCard);
                                }
                                
                                foreach (var inGameEffectCard in copyOpponentEffectCards)
                                {
                                    opponentPlayerCard.effectCards.Remove(inGameEffectCard);
                                    playerCards.effectCards.Add(inGameEffectCard);
                                }

                                if (playerCards.field != null)
                                {
                                    playerCards.yellowCards.Add(playerCards.field);
                                    playerCards.field = null;
                                }

                                if (opponentPlayerCard.field != null)
                                {
                                    opponentPlayerCard.yellowCards.Add(opponentPlayerCard.field);
                                    opponentPlayerCard.field = null;
                                }
                                Object.Destroy(messageBox);
                                Object.Destroy(messageBox1);
                            }
                        };
                        messageBox1.GetComponent<MessageBox>().NegativeAction = () => { DisplayOkMessage(canvas); };
                    }
                };
                messageBox.GetComponent<MessageBox>().NegativeAction = () => { DisplayOkMessage(canvas); };
            }
        }
        else
        {
        }
    }
}