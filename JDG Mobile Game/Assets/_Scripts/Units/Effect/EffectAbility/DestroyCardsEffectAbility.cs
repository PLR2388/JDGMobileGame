using System;
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
    private bool fromCurrentPlayer;
    private bool fromOpponentPlayer;

    private List<CardType> types = new List<CardType>
    {
        CardType.Effect,
        CardType.Equipment,
        CardType.Invocation,
        CardType.Field
    };

    public DestroyCardsEffectAbility(EffectAbilityName name, string description, int numberCards,
        bool mustThrowFirstDeck = false, bool mustSacrificeInvocation = false, bool mustThrowHandCard = false,
        List<CardType> types = null, bool fromCurrentPlayer = true, bool fromOpponentPlayer = true)
    {
        Name = name;
        Description = description;
        numbers = numberCards;
        this.mustThrowFirstDeck = mustThrowFirstDeck;
        this.mustSacrificeInvocation = mustSacrificeInvocation;
        this.mustThrowHandCard = mustThrowHandCard;
        this.fromCurrentPlayer = fromCurrentPlayer;
        this.fromOpponentPlayer = fromOpponentPlayer;
        if (types != null)
        {
            this.types = types;
        }
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards,
        PlayerStatus opponentPlayerStatus)
    {
        bool canThrowFirstDeck = !mustThrowFirstDeck || playerCards.deck.Count > 0;
        bool canSacrificeInvocation = !mustSacrificeInvocation || playerCards.invocationCards.Count > 0;
        bool canThrowHandCard = !mustThrowHandCard || playerCards.handCards.Count > 0;

        bool haveCardsToDestroy = false;

        foreach (var type in types)
        {
            switch (type)
            {
                case CardType.Contre:
                    break;
                case CardType.Effect:
                    haveCardsToDestroy = haveCardsToDestroy || (fromCurrentPlayer && playerCards.effectCards.Count > 0) ||
                                         (fromOpponentPlayer && opponentPlayerCards.effectCards.Count > 0);
                    break;
                case CardType.Equipment:
                    haveCardsToDestroy = haveCardsToDestroy ||
                                         (fromCurrentPlayer && playerCards.invocationCards.Count(card => card.EquipmentCard != null) > 0) ||
                                         (fromOpponentPlayer && opponentPlayerCards.invocationCards.Count(card => card.EquipmentCard != null) >
                                         0);
                    break;
                case CardType.Field:
                    haveCardsToDestroy = haveCardsToDestroy || (fromCurrentPlayer && playerCards.FieldCard != null) ||
                                         (fromOpponentPlayer && opponentPlayerCards.FieldCard != null);
                    break;
                case CardType.Invocation:
                    haveCardsToDestroy = haveCardsToDestroy || (fromCurrentPlayer && playerCards.invocationCards.Count > 0) ||
                                         (fromOpponentPlayer && opponentPlayerCards.invocationCards.Count > 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (haveCardsToDestroy)
            {
                break;
            }
        }

        return canThrowFirstDeck && canSacrificeInvocation && canThrowHandCard && haveCardsToDestroy;
    }

    private void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARD),
            showOkButton: true
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
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
                var config = new CardSelectorConfig(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SACRIFICE),
                    invocationCards,
                    showNegativeButton: true,
                    showPositiveButton: true,
                    positiveAction: (card) =>
                    {
                        if (card is InGameInvocationCard invocationCard)
                        {
                                           var config = new CardSelectorConfig(
                                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
                                handCards.ToList(),
                                showPositiveButton: true,
                                showNegativeButton: true,
                                positiveAction: (handCard) =>
                                {
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

                                        if (playerCards.FieldCard != null)
                                        {
                                            playerCards.yellowCards.Add(playerCards.FieldCard);
                                            playerCards.FieldCard = null;
                                        }

                                        if (opponentPlayerCard.FieldCard != null)
                                        {
                                            opponentPlayerCard.yellowCards.Add(opponentPlayerCard.FieldCard);
                                            opponentPlayerCard.FieldCard = null;
                                        }
                                    }
                                },
                                negativeAction: () =>
                                {
                                    DisplayOkMessage(canvas);
                                }
                            );
                            CardSelector.Instance.CreateCardSelection(canvas, config);
                        }
                        else
                        {
                            DisplayOkMessage(canvas);
                        }
                    },
                    negativeAction: () =>
                    {
                        DisplayOkMessage(canvas);
                    }
                );
                CardSelector.Instance.CreateCardSelection(canvas, config);
            }
        }
        else if (numbers == 1)
        {
            var cards = BuildAllPossibleCardsToDestroy(playerCards, opponentPlayerCard);

            if (mustThrowHandCard)
            {
                var handCards = playerCards.handCards;
                var config = new CardSelectorConfig(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
                    handCards.ToList(),
                    showNegativeButton: true,
                    showPositiveButton: true,
                    positiveAction: (handCard) =>
                    {
                        if (handCard == null)
                        {
                            DisplayOkMessage(canvas);
                        }
                        else
                        {
                            playerCards.handCards.Remove(handCard);
                            playerCards.yellowCards.Add(handCard);
                            DisplayMessageBoxToDestroyOneCard(canvas, playerCards, opponentPlayerCard, cards);
                        }
                    },
                    negativeAction: () =>
                    {
                        DisplayOkMessage(canvas);
                    }
                );
                CardSelector.Instance.CreateCardSelection(canvas, config);
            }
            else
            {
                // No Condition case
                DisplayMessageBoxToDestroyOneCard(canvas, playerCards, opponentPlayerCard, cards);
            }
        }
    }

    private void DisplayMessageBoxToDestroyOneCard(Transform canvas, PlayerCards playerCards,
        PlayerCards opponentPlayerCard, List<InGameCard> cards)
    {
        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_DESTROY_CARD),
            cards,
            showNegativeButton: true,
            showPositiveButton: true,
            positiveAction: (card) =>
            {
                if (card == null)
                {
                    DisplayOkMessage(canvas);
                }
                else
                {
                    DestroyCard(playerCards, opponentPlayerCard, card, card.CardOwner == CardOwner.Player1);
                }
            },
            negativeAction: () =>
            {
                DisplayOkMessage(canvas);
            }
        );
        CardSelector.Instance.CreateCardSelection(canvas, config);
    }

    private List<InGameCard> BuildAllPossibleCardsToDestroy(PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        var cards = new List<InGameCard>();

        if (fromCurrentPlayer)
        {
            if (playerCards.invocationCards.Count > 0)
            {
                cards.AddRange(playerCards.invocationCards);

                var equipmentCards = playerCards.invocationCards.Select(card => card.EquipmentCard)
                    .Where(elt => elt != null);
                var inGameEquipementCards = equipmentCards.ToList();
                if (inGameEquipementCards.Any())
                {
                    cards.AddRange(inGameEquipementCards);
                }
            }

            if (playerCards.effectCards.Count > 0)
            {
                cards.AddRange(playerCards.effectCards);
            }

            if (playerCards.FieldCard != null)
            {
                cards.Add(playerCards.FieldCard);
            }
        }

        if (fromOpponentPlayer)
        {
            if (opponentPlayerCard.invocationCards.Count > 0)
            {
                cards.AddRange(opponentPlayerCard.invocationCards);

                var equipmentCards = opponentPlayerCard.invocationCards.Select(card => card.EquipmentCard)
                    .Where(elt => elt != null);
                var inGameEquipementCards = equipmentCards.ToList();
                if (inGameEquipementCards.Any())
                {
                    cards.AddRange(inGameEquipementCards);
                }
            }

            if (opponentPlayerCard.effectCards.Count > 0)
            {
                cards.AddRange(opponentPlayerCard.effectCards);
            }

            if (opponentPlayerCard.FieldCard != null)
            {
                cards.Add(opponentPlayerCard.FieldCard);
            }
        }

        cards = cards.Where(card => types.Contains(card.Type)).ToList();

        return cards;
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
                        playerCards.FieldCard = null;
                    }
                    else
                    {
                        opponentPlayerCard.FieldCard = null;
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
                case CardType.Equipment:
                    if (isP1)
                    {
                        var invocationCard = playerCards.invocationCards.First(invocationCard =>
                            invocationCard.EquipmentCard != null && invocationCard.EquipmentCard.Title == card.Title);
                        invocationCard.EquipmentCard = null;
                    }
                    else
                    {
                        var invocationCard = opponentPlayerCard.invocationCards.First(invocationCard =>
                            invocationCard.EquipmentCard != null && invocationCard.EquipmentCard.Title == card.Title);
                        invocationCard.EquipmentCard = null;
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
                        opponentPlayerCard.FieldCard = null;
                    }
                    else
                    {
                        playerCards.FieldCard = null;
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
                case CardType.Equipment:
                    if (isP1)
                    {
                        var invocationCard = opponentPlayerCard.invocationCards.First(invocationCard =>
                            invocationCard.EquipmentCard != null && invocationCard.EquipmentCard.Title == card.Title);
                        invocationCard.EquipmentCard = null;
                    }
                    else
                    {
                        var invocationCard = playerCards.invocationCards.First(invocationCard =>
                            invocationCard.EquipmentCard != null && invocationCard.EquipmentCard.Title == card.Title);
                        invocationCard.EquipmentCard = null;
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