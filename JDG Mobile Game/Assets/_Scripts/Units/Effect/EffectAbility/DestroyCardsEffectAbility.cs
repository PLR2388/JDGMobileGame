using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using UnityEngine;

public class DestroyCardsEffectAbility : EffectAbility
{
    private readonly int numbers; // 0 = ALL
    private readonly bool mustThrowFirstDeck;
    private readonly bool mustSacrificeInvocation;
    private readonly bool mustThrowHandCard;
    private readonly bool fromCurrentPlayer;
    private readonly bool fromOpponentPlayer;

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
        bool canThrowFirstDeck = !mustThrowFirstDeck || playerCards.Deck.Count > 0;
        bool canSacrificeInvocation = !mustSacrificeInvocation || playerCards.InvocationCards.Count > 0;
        bool canThrowHandCard = !mustThrowHandCard || playerCards.HandCards.Count > 0;

        bool haveCardsToDestroy = false;

        foreach (var type in types)
        {
            switch (type)
            {
                case CardType.Contre:
                    break;
                case CardType.Effect:
                    haveCardsToDestroy = haveCardsToDestroy || (fromCurrentPlayer && playerCards.EffectCards.Count > 0) ||
                                         (fromOpponentPlayer && opponentPlayerCards.EffectCards.Count > 0);
                    break;
                case CardType.Equipment:
                    haveCardsToDestroy = haveCardsToDestroy ||
                                         (fromCurrentPlayer && playerCards.InvocationCards.Count(card => card.EquipmentCard != null) > 0) ||
                                         (fromOpponentPlayer && opponentPlayerCards.InvocationCards.Count(card => card.EquipmentCard != null) >
                                         0);
                    break;
                case CardType.Field:
                    haveCardsToDestroy = haveCardsToDestroy || (fromCurrentPlayer && playerCards.FieldCard != null) ||
                                         (fromOpponentPlayer && opponentPlayerCards.FieldCard != null);
                    break;
                case CardType.Invocation:
                    haveCardsToDestroy = haveCardsToDestroy || (fromCurrentPlayer && playerCards.InvocationCards.Count > 0) ||
                                         (fromOpponentPlayer && opponentPlayerCards.InvocationCards.Count > 0);
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
                var cardDeck = playerCards.Deck[playerCards.Deck.Count - 1];
                playerCards.Deck.Remove(cardDeck);
                playerCards.YellowCards.Add(cardDeck);
            }

            if (mustSacrificeInvocation && mustThrowHandCard)
            {
                // Only case for the moment
                var invocationCards = new List<InGameCard>(playerCards.InvocationCards);
                var handCards = playerCards.HandCards;
                var config = new CardSelectorConfig(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SACRIFICE),
                    invocationCards,
                    showOkButton: true,
                    okAction: (card) =>
                    {
                        if (card is InGameInvocationCard invocationCard)
                        {
                            var config = new CardSelectorConfig(
                                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
                                handCards.ToList(),
                                showOkButton: true, 
                                okAction: (handCard) =>
                                {
                                    if (handCard == null)
                                    {
                                        DisplayOkMessage(canvas);
                                    }
                                    else
                                    {
                                        playerCards.InvocationCards.Remove(invocationCard);
                                        playerCards.HandCards.Remove(handCard);
                                        playerCards.YellowCards.Add(invocationCard);
                                        playerCards.YellowCards.Add(handCard);

                                        var copyInvocationCards = playerCards.InvocationCards.ToList();
                                        var copyOpponentInvocationCards = opponentPlayerCard.InvocationCards.ToList();
                                        var copyEffectCards = playerCards.EffectCards.ToList();
                                        var copyOpponentEffectCards = opponentPlayerCard.EffectCards.ToList();

                                        foreach (var inGameInvocationCard in copyInvocationCards)
                                        {
                                            playerCards.InvocationCards.Remove(inGameInvocationCard);
                                            playerCards.YellowCards.Add(inGameInvocationCard);
                                        }

                                        foreach (var inGameInvocationCard in copyOpponentInvocationCards)
                                        {
                                            opponentPlayerCard.InvocationCards.Remove(inGameInvocationCard);
                                            playerCards.YellowCards.Add(inGameInvocationCard);
                                        }

                                        foreach (var inGameEffectCard in copyEffectCards)
                                        {
                                            playerCards.EffectCards.Remove(inGameEffectCard);
                                            playerCards.EffectCards.Add(inGameEffectCard);
                                        }

                                        foreach (var inGameEffectCard in copyOpponentEffectCards)
                                        {
                                            opponentPlayerCard.EffectCards.Remove(inGameEffectCard);
                                            playerCards.EffectCards.Add(inGameEffectCard);
                                        }

                                        if (playerCards.FieldCard != null)
                                        {
                                            playerCards.YellowCards.Add(playerCards.FieldCard);
                                            playerCards.FieldCard = null;
                                        }

                                        if (opponentPlayerCard.FieldCard != null)
                                        {
                                            opponentPlayerCard.YellowCards.Add(opponentPlayerCard.FieldCard);
                                            opponentPlayerCard.FieldCard = null;
                                        }
                                    }
                                }
                            );
                            CardSelector.Instance.CreateCardSelection(canvas, config);
                        }
                        else
                        {
                            DisplayOkMessage(canvas);
                        }
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
                var handCards = playerCards.HandCards;
                var config = new CardSelectorConfig(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
                    handCards.ToList(),
                    showOkButton: true,
                    okAction: (handCard) =>
                    {
                        if (handCard == null)
                        {
                            DisplayOkMessage(canvas);
                        }
                        else
                        {
                            playerCards.HandCards.Remove(handCard);
                            playerCards.YellowCards.Add(handCard);
                            DisplayMessageBoxToDestroyOneCard(canvas, playerCards, opponentPlayerCard, cards);
                        }
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
            showOkButton: true,
            okAction: (card) =>
            {
                if (card == null)
                {
                    DisplayOkMessage(canvas);
                }
                else
                {
                    DestroyCard(playerCards, opponentPlayerCard, card, card.CardOwner == CardOwner.Player1);
                }
            }
        );
        CardSelector.Instance.CreateCardSelection(canvas, config);
    }

    private List<InGameCard> BuildAllPossibleCardsToDestroy(PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        var cards = new List<InGameCard>();

        if (fromCurrentPlayer)
        {
            if (playerCards.InvocationCards.Count > 0)
            {
                cards.AddRange(playerCards.InvocationCards);

                var equipmentCards = playerCards.InvocationCards.Select(card => card.EquipmentCard)
                    .Where(elt => elt != null);
                var inGameEquipementCards = equipmentCards.ToList();
                if (inGameEquipementCards.Any())
                {
                    cards.AddRange(inGameEquipementCards);
                }
            }

            if (playerCards.EffectCards.Count > 0)
            {
                cards.AddRange(playerCards.EffectCards);
            }

            if (playerCards.FieldCard != null)
            {
                cards.Add(playerCards.FieldCard);
            }
        }

        if (fromOpponentPlayer)
        {
            if (opponentPlayerCard.InvocationCards.Count > 0)
            {
                cards.AddRange(opponentPlayerCard.InvocationCards);

                var equipmentCards = opponentPlayerCard.InvocationCards.Select(card => card.EquipmentCard)
                    .Where(elt => elt != null);
                var inGameEquipementCards = equipmentCards.ToList();
                if (inGameEquipementCards.Any())
                {
                    cards.AddRange(inGameEquipementCards);
                }
            }

            if (opponentPlayerCard.EffectCards.Count > 0)
            {
                cards.AddRange(opponentPlayerCard.EffectCards);
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
        if (playerCards.IsPlayerOne)
        {
            switch (card.Type)
            {
                case CardType.Effect:
                    if (isP1)
                    {
                        playerCards.EffectCards.Remove(card as InGameEffectCard);
                    }
                    else
                    {
                        opponentPlayerCard.EffectCards.Remove(card as InGameEffectCard);
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
                        playerCards.InvocationCards.Remove(card as InGameInvocationCard);
                    }
                    else
                    {
                        opponentPlayerCard.InvocationCards.Remove(card as InGameInvocationCard);
                    }

                    break;
                case CardType.Equipment:
                    if (isP1)
                    {
                        var invocationCard = playerCards.InvocationCards.First(invocationCard =>
                            invocationCard.EquipmentCard != null && invocationCard.EquipmentCard.Title == card.Title);
                        invocationCard.EquipmentCard = null;
                    }
                    else
                    {
                        var invocationCard = opponentPlayerCard.InvocationCards.First(invocationCard =>
                            invocationCard.EquipmentCard != null && invocationCard.EquipmentCard.Title == card.Title);
                        invocationCard.EquipmentCard = null;
                    }
                    break;
            }

            if (isP1)
            {
                playerCards.YellowCards.Add(card);
            }
            else
            {
                opponentPlayerCard.YellowCards.Add(card);
            }
        }
        else
        {
            switch (card.Type)
            {
                case CardType.Effect:
                    if (isP1)
                    {
                        opponentPlayerCard.EffectCards.Remove(card as InGameEffectCard);
                    }
                    else
                    {
                        playerCards.EffectCards.Remove(card as InGameEffectCard);
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
                        opponentPlayerCard.InvocationCards.Remove(card as InGameInvocationCard);
                    }
                    else
                    {
                        playerCards.InvocationCards.Remove(card as InGameInvocationCard);
                    }

                    break;
                case CardType.Equipment:
                    if (isP1)
                    {
                        var invocationCard = opponentPlayerCard.InvocationCards.First(invocationCard =>
                            invocationCard.EquipmentCard != null && invocationCard.EquipmentCard.Title == card.Title);
                        invocationCard.EquipmentCard = null;
                    }
                    else
                    {
                        var invocationCard = playerCards.InvocationCards.First(invocationCard =>
                            invocationCard.EquipmentCard != null && invocationCard.EquipmentCard.Title == card.Title);
                        invocationCard.EquipmentCard = null;
                    }
                    break;
            }

            if (isP1)
            {
                opponentPlayerCard.YellowCards.Add(card);
            }
            else
            {
                playerCards.YellowCards.Add(card);
            }
        }
    }
}