using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using UnityEngine;

/// <summary>
/// Represents the ability of a card to destroy other cards.
/// </summary>
public class DestroyCardsEffectAbility : EffectAbility
{
    private readonly int numbers; // 0 = ALL
    private readonly bool mustThrowFirstDeck;
    private readonly bool mustSacrificeInvocation;
    private readonly bool mustThrowHandCard;
    private readonly bool fromCurrentPlayer;
    private readonly bool fromOpponentPlayer;

    private readonly List<CardType> types = new List<CardType>
    {
        CardType.Effect,
        CardType.Equipment,
        CardType.Invocation,
        CardType.Field
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="DestroyCardsEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="numberCards">The number of cards involved in the ability.</param>
    /// <param name="mustThrowFirstDeck">Indicates if the first deck card must be thrown.</param>
    /// <param name="mustSacrificeInvocation">Indicates if an invocation card must be sacrificed.</param>
    /// <param name="mustThrowHandCard">Indicates if a hand card must be thrown.</param>
    /// <param name="types">The types of cards that can be affected.</param>
    /// <param name="fromCurrentPlayer">Indicates if cards from the current player can be destroyed.</param>
    /// <param name="fromOpponentPlayer">Indicates if cards from the opponent player can be destroyed.</param>
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

    /// <summary>
    /// Determines whether the effect can be used based on the current state of the game.
    /// </summary>
    /// <param name="playerCards">The cards of the player using the ability.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    /// <returns>True if the effect can be used; otherwise, false.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        bool canThrowFirstDeck = !mustThrowFirstDeck || playerCards.Deck.Any();
        bool canSacrificeInvocation = !mustSacrificeInvocation || playerCards.InvocationCards.Any();
        bool canThrowHandCard = !mustThrowHandCard || playerCards.HandCards.Any();
        bool haveCardsToDestroy = HaveCardsToDestroy(playerCards, opponentPlayerCards);

        return canThrowFirstDeck && canSacrificeInvocation && canThrowHandCard && haveCardsToDestroy;
    }

    /// <summary>
    /// Applies the effect ability to the game.
    /// </summary>
    /// <param name="canvas">The canvas where messages are displayed.</param>
    /// <param name="playerCards">The cards of the player using the ability.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    /// <param name="playerStatus">The status of the player using the ability.</param>
    /// <param name="opponentStatus">The status of the opponent player.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus, PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        switch (numbers)
        {
            case 0:
                HandleCase0(canvas, playerCards, opponentPlayerCard);
                break;
            case 1:
                HandleCase1(canvas, playerCards, opponentPlayerCard);
                break;
        }
    }

    /// <summary>
    /// Checks if there are any cards to destroy.
    /// </summary>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCards">The cards of the opponent player.</param>
    /// <returns>true if there are cards to destroy; otherwise, false.</returns>
    private bool HaveCardsToDestroy(PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        return types.Any(type => TypeHasCardsToDestroy(type, playerCards, opponentPlayerCards));
    }

    /// <summary>
    /// Determines if a card type has cards that can be destroyed.
    /// </summary>
    /// <param name="type">The card type to check.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent player's cards.</param>
    /// <returns>True if the type has cards that can be destroyed, otherwise false.</returns>
    private bool TypeHasCardsToDestroy(CardType type, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        switch (type)
        {
            case CardType.Contre:
                return false;
            case CardType.Effect:
                return (fromCurrentPlayer && playerCards.EffectCards.Any()) ||
                       (fromOpponentPlayer && opponentPlayerCards.EffectCards.Any());
            case CardType.Equipment:
                return (fromCurrentPlayer && playerCards.InvocationCards.Any(card => card.EquipmentCard != null)) ||
                       (fromOpponentPlayer && opponentPlayerCards.InvocationCards.Any(card => card.EquipmentCard != null));
            case CardType.Field:
                return (fromCurrentPlayer && playerCards.FieldCard != null) ||
                       (fromOpponentPlayer && opponentPlayerCards.FieldCard != null);
            case CardType.Invocation:
                return (fromCurrentPlayer && playerCards.InvocationCards.Any()) ||
                       (fromOpponentPlayer && opponentPlayerCards.InvocationCards.Any());
            default:
                throw new ArgumentOutOfRangeException(nameof(type), "Invalid card type");
        }
    }

    /// <summary>
    /// Displays a warning message to the player.
    /// </summary>
    /// <param name="canvas">The canvas on which the message box is displayed.</param>
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
    
    /// <summary>
    /// Handles the first case scenario for card actions.
    /// </summary>
    /// <param name="canvas">The canvas to use for UI operations.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent player's card.</param>
    private void HandleCase0(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        if (mustThrowFirstDeck)
        {
            var cardDeck = playerCards.Deck[^1];
            playerCards.Deck.Remove(cardDeck);
            playerCards.YellowCards.Add(cardDeck);
        }

        if (mustSacrificeInvocation && mustThrowHandCard)
        {
            var invocationCards = new List<InGameCard>(playerCards.InvocationCards);

            CardSelectorConfig config = CreateSacrificeInvocationConfig(canvas, playerCards, opponentPlayerCard, invocationCards);
            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
    }

    /// <summary>
    /// Handles the second case scenario for card actions.
    /// </summary>
    /// <param name="canvas">The canvas to use for UI operations.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent player's card.</param>
    private void HandleCase1(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        var cards = BuildAllPossibleCardsToDestroy(playerCards, opponentPlayerCard);

        if (mustThrowHandCard)
        {
            CardSelectorConfig config = CreateRemoveHandCardConfig(canvas, playerCards, opponentPlayerCard, cards);
            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
        else
        {
            DisplayMessageBoxToDestroyOneCard(canvas, playerCards, opponentPlayerCard, cards);
        }
    }

    /// <summary>
    /// Creates a configuration for the card selector to allow sacrificing an invocation card.
    /// </summary>
    /// <param name="canvas">The canvas on which the card selector is displayed.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent player's cards.</param>
    /// <param name="invocationCards">The list of invocation cards.</param>
    /// <returns>A configuration for the card selector.</returns>
    private CardSelectorConfig CreateSacrificeInvocationConfig(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, List<InGameCard> invocationCards)
    {
        return new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SACRIFICE),
            invocationCards,
            showOkButton: true,
            okAction: (card) =>
            {
                if (card is InGameInvocationCard invocationCard)
                {
                    CardSelectorConfig innerConfig = CreateRemoveHandCardAfterSacrificeConfig(canvas, playerCards, opponentPlayerCard, invocationCard);
                    CardSelector.Instance.CreateCardSelection(canvas, innerConfig);
                }
                else
                {
                    DisplayOkMessage(canvas);
                }
            }
        );
    }

    /// <summary>
    /// Creates a configuration for the card selector to remove a card from hand after sacrificing.
    /// </summary>
    /// <param name="canvas">The canvas on which the card selector is displayed.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent player's cards.</param>
    /// <param name="invocationCard">The sacrificed invocation card.</param>
    /// <returns>A configuration for the card selector.</returns>
    private CardSelectorConfig CreateRemoveHandCardAfterSacrificeConfig(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        InGameInvocationCard invocationCard)
    {
        return new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
            playerCards.HandCards.ToList(),
            showOkButton: true,
            okAction: handCard => HandleOkAction(handCard, canvas, playerCards, opponentPlayerCard, invocationCard)
        );
    }

    /// <summary>
    /// Handles the OK action for the card selector.
    /// </summary>
    /// <param name="handCard">The selected hand card.</param>
    /// <param name="canvas">The canvas to use for UI operations.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent player's cards.</param>
    /// <param name="invocationCard">The sacrificed invocation card.</param>
    private void HandleOkAction(InGameCard handCard, Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, InGameInvocationCard invocationCard)
    {
        if (handCard == null)
        {
            DisplayOkMessage(canvas);
            return;
        }

        playerCards.InvocationCards.Remove(invocationCard);
        playerCards.HandCards.Remove(handCard);
        playerCards.YellowCards.Add(invocationCard);
        playerCards.YellowCards.Add(handCard);

        MoveCardsToYellow(playerCards, opponentPlayerCard);
    }

    /// <summary>
    /// Moves all eligible cards to the yellow zone.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent player's cards.</param>
    private void MoveCardsToYellow(PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        foreach (var inGameInvocationCard in playerCards.InvocationCards.ToList())
        {
            playerCards.InvocationCards.Remove(inGameInvocationCard);
            playerCards.YellowCards.Add(inGameInvocationCard);
        }

        foreach (var inGameInvocationCard in opponentPlayerCard.InvocationCards.ToList())
        {
            opponentPlayerCard.InvocationCards.Remove(inGameInvocationCard);
            playerCards.YellowCards.Add(inGameInvocationCard);
        }

        foreach (var inGameEffectCard in playerCards.EffectCards.ToList())
        {
            playerCards.EffectCards.Remove(inGameEffectCard);
            playerCards.EffectCards.Add(inGameEffectCard);
        }

        foreach (var inGameEffectCard in opponentPlayerCard.EffectCards.ToList())
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

    /// <summary>
    /// Creates a configuration for a card selector that handles removing a card from a player's hand.
    /// </summary>
    /// <param name="canvas">Transform reference for positioning UI elements.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <param name="cardsToDestroy">List of cards eligible for destruction.</param>
    /// <returns>Returns a CardSelectorConfig for removing cards.</returns>
    private CardSelectorConfig CreateRemoveHandCardConfig(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, List<InGameCard> cardsToDestroy)
    {
        return new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
            playerCards.HandCards.ToList(),
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
                    DisplayMessageBoxToDestroyOneCard(canvas, playerCards, opponentPlayerCard, cardsToDestroy);
                }
            }
        );
    }

    /// <summary>
    /// Displays a message box prompting the user to select a card to destroy.
    /// </summary>
    /// <param name="canvas">Transform reference for positioning UI elements.</param>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <param name="cards">List of cards eligible for destruction.</param>
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

    /// <summary>
    /// Builds a list of all possible cards that can be destroyed from both the player's and opponent's collections.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <param name="opponentPlayerCard">The opponent's cards.</param>
    /// <returns>Returns a list of cards eligible for destruction.</returns>
    private List<InGameCard> BuildAllPossibleCardsToDestroy(PlayerCards playerCards, PlayerCards opponentPlayerCard)
    {
        var cards = new List<InGameCard>();

        if (fromCurrentPlayer)
        {
            cards.AddRange(GetCardsToDestroyForPlayer(playerCards));
        }

        if (fromOpponentPlayer)
        {
            cards.AddRange(GetCardsToDestroyForPlayer(opponentPlayerCard));
        }

        return cards.Where(card => types.Contains(card.Type)).ToList();
    }

    /// <summary>
    /// Retrieves a list of cards from a player that can be destroyed.
    /// This includes Invocation, Effect, and Field cards.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <returns>Returns a list of cards eligible for destruction from the specified player.</returns>
    private IEnumerable<InGameCard> GetCardsToDestroyForPlayer(PlayerCards playerCards)
    {
        var result = new List<InGameCard>();

        // Invocation cards
        if (playerCards.InvocationCards.Count > 0)
        {
            result.AddRange(playerCards.InvocationCards);

            var equipmentCards = playerCards.InvocationCards
                .Select(card => card.EquipmentCard)
                .Where(elt => elt != null)
                .ToList();

            if (equipmentCards.Any())
            {
                result.AddRange(equipmentCards);
            }
        }

        // Effect cards
        if (playerCards.EffectCards.Count > 0)
        {
            result.AddRange(playerCards.EffectCards);
        }

        // Field card
        if (playerCards.FieldCard != null)
        {
            result.Add(playerCards.FieldCard);
        }

        return result;
    }

    /// <summary>
    /// Destroys a card.
    /// </summary>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    /// <param name="card">The card to be destroyed.</param>
    /// <param name="isP1">Indicates whether the player is P1.</param>
    private static void DestroyCard(PlayerCards playerCards, PlayerCards opponentPlayerCard, InGameCard card, bool isP1)
    {
        PlayerCards actingPlayer = playerCards.IsPlayerOne ? playerCards : opponentPlayerCard;
        PlayerCards receivingPlayer = playerCards.IsPlayerOne ? opponentPlayerCard : playerCards;

        PlayerCards targetPlayerCards = isP1 ? actingPlayer : receivingPlayer;

        switch (card.Type)
        {
            case CardType.Effect:
                targetPlayerCards.EffectCards.Remove(card as InGameEffectCard);
                break;

            case CardType.Field:
                targetPlayerCards.FieldCard = null;
                break;

            case CardType.Invocation:
                targetPlayerCards.InvocationCards.Remove(card as InGameInvocationCard);
                break;

            case CardType.Equipment:
                var invocationCard = targetPlayerCards.InvocationCards.First(invocation =>
                    invocation.EquipmentCard != null && invocation.EquipmentCard.Title == card.Title);
                invocationCard.EquipmentCard = null;
                break;
        }

        targetPlayerCards.YellowCards.Add(card);
    }

}