using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using UnityEngine;
using Object = UnityEngine.Object;

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
                    var invocationCard = (InGameInvocationCard)messageBox.GetComponent<MessageBox>().GetSelectedCard();
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
        else if (numbers == 1)
        {
            if (mustThrowHandCard)
            {
                var handCards = playerCards.handCards;
                var messageBox =
                    MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte à enlever de la main",
                        handCards);
                messageBox.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    var handCard = messageBox.GetComponent<MessageBox>().GetSelectedCard();
                    if (handCard == null)
                    {
                        DisplayOkMessage(canvas);
                    }
                    else
                    {
                        playerCards.handCards.Remove(handCard);
                        playerCards.yellowCards.Add(handCard);

                        var cards = new List<InGameCard>();

                        if (playerCards.invocationCards.Count > 0)
                        {
                            cards.AddRange(playerCards.invocationCards);
                        }

                        if (playerCards.effectCards.Count > 0)
                        {
                            cards.AddRange(playerCards.effectCards);
                        }

                        if (playerCards.field != null)
                        {
                            cards.Add(playerCards.field);
                        }
                        
                        if (opponentPlayerCard.invocationCards.Count > 0)
                        {
                            cards.AddRange(opponentPlayerCard.invocationCards);
                        }

                        if (opponentPlayerCard.effectCards.Count > 0)
                        {
                            cards.AddRange(opponentPlayerCard.effectCards);
                        }

                        if (opponentPlayerCard.field != null)
                        {
                            cards.Add(opponentPlayerCard.field);
                        }

                        var messageBox1 =
                            MessageBox.CreateMessageBoxWithCardSelector(canvas, "Carte à détruire", cards);

                        messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                        {
                            var card = messageBox1.GetComponent<MessageBox>().GetSelectedCard();
                            if (card == null)
                            {
                                DisplayOkMessage(canvas);
                            }
                            else
                            {
                                DestroyCard(playerCards, opponentPlayerCard, card, card.CardOwner == CardOwner.Player1);
                                Object.Destroy(messageBox1);
                            }
                        };
                        messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                        {
                            DisplayOkMessage(canvas);
                        };
                        
                        Object.Destroy(messageBox);
                    }
                };
                messageBox.GetComponent<MessageBox>().NegativeAction = () => { DisplayOkMessage(canvas); };
            }
        }
    }

    private static void DestroyCard(PlayerCards playerCards, PlayerCards opponentPlayerCard, InGameCard card, bool isP1)
    {
        if (playerCards.isPlayerOne)
        {
            switch (card.Type)
            {
                case CardType.Effect:
                    if (isP1)
                    {
                        playerCards.effectCards.Remove(card as InGameEffectCard);
                    }
                    else
                    {
                        opponentPlayerCard.effectCards.Remove(card as InGameEffectCard);
                    }
                   
                    break;
                case CardType.Field:
                    if (isP1)
                    {
                        playerCards.field = null;
                    }
                    else
                    {
                        opponentPlayerCard.field = null;
                    }
                  
                    break;
                case CardType.Invocation:
                    if (isP1)
                    {
                        playerCards.invocationCards.Remove(card as InGameInvocationCard);
                    }
                    else
                    {
                        opponentPlayerCard.invocationCards.Remove(card as InGameInvocationCard);
                    }
                  
                    break;
            }

            if (isP1)
            {
                playerCards.yellowCards.Add(card);
            }
            else
            {
                opponentPlayerCard.yellowCards.Add(card);   
            }
        }
        else
        {
            switch (card.Type)
            {
                case CardType.Effect:
                    if (isP1)
                    {
                        opponentPlayerCard.effectCards.Remove(card as InGameEffectCard);
                    }
                    else
                    {
                        playerCards.effectCards.Remove(card as InGameEffectCard);
                    }
                    break;
                case CardType.Field:
                    if (isP1)
                    {
                        opponentPlayerCard.field = null;
                    }
                    else
                    {
                        playerCards.field = null;
                    }
             
                    break;
                case CardType.Invocation:
                    if (isP1)
                    {
                        opponentPlayerCard.invocationCards.Remove(card as InGameInvocationCard);
                    }
                    else
                    {
                        playerCards.invocationCards.Remove(card as InGameInvocationCard);
                    }
                 
                    break;
            }

            if (isP1)
            {
                opponentPlayerCard.yellowCards.Add(card);
            }
            else
            {
                playerCards.yellowCards.Add(card);
            }
        }
    }
}