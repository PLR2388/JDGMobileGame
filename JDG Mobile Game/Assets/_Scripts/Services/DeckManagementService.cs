using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;
using Menu;
using UnityEngine;

/// <summary>
/// Implementation of IDeckManagementService.
/// Manages deck data storage and tutorial deck building.
/// Migrated from GameState singleton in Phase 17-18.
///
/// Phase 17-18 Fix: Now uses ResourceSystem to load card data instead of GameState.
/// Phase 24-25: Added IEventBus and ICardCollectionService dependencies for CardFactory.
/// Phase 8: Uses ICardDataProvider instead of ResourceSystem.Instance.
/// Phase 7: Added IAbilityProvider for ability system migration.
/// Phase 46: Removed optional constructor params (VContainer doesn't support them).
///           ICardCollectionService and IAbilityProvider are passed directly to CardFactory
///           by callers (CardChoice) who have access to them.
/// </summary>
public class DeckManagementService : IDeckManagementService
{
    private List<Card> _allCards;
    private readonly IEventBus _eventBus;
    private readonly ICardDataProvider _cardDataProvider;

    public List<Card> Deck1AllCards { get; private set; } = new List<Card>();
    public List<Card> Deck2AllCards { get; private set; } = new List<Card>();
    public List<InGameCard> Player1DeckCards { get; set; } = new List<InGameCard>();
    public List<InGameCard> Player2DeckCards { get; set; } = new List<InGameCard>();

    public DeckManagementService(
        IEventBus eventBus,
        ICardDataProvider cardDataProvider)
    {
        _eventBus = eventBus;
        _cardDataProvider = cardDataProvider;

        // Phase 8: Use ICardDataProvider instead of ResourceSystem.Instance
        if (_cardDataProvider.IsLoaded)
        {
            // Get cards from the provider
            if (_cardDataProvider is CardDataProvider typedProvider)
            {
                _allCards = typedProvider.GetAllCardsTyped();
            }
            else
            {
                // Fallback for other implementations
                _allCards = _cardDataProvider.GetAllCards().Cast<Card>().ToList();
            }

            if (_allCards != null && _allCards.Count > 0)
            {
                InitializeCardPools(_allCards);
            }
        }
        else
        {
            Debug.LogWarning("DeckManagementService: Card data not loaded yet");
        }
    }

    public void InitializeCardPools(List<Card> allCards)
    {
        _allCards = allCards;
        ResetDeckPools();
    }

    public void ResetDeckPools()
    {
        if (_allCards == null || _allCards.Count == 0)
        {
            Debug.LogWarning("DeckManagementService: Cannot reset deck pools - allCards is empty");
            return;
        }

        Deck1AllCards.Clear();
        Deck2AllCards.Clear();

        foreach (var card in _allCards)
        {
            Deck1AllCards.Add(Object.Instantiate(card));
            Deck2AllCards.Add(Object.Instantiate(card));
        }
    }

    public void BuildTutorialDecks()
    {
        ResetDeckPools();
        BuildPlayer1TutorialDeck();
        BuildPlayer2TutorialDeck();
    }

    /// <summary>
    /// Builds the tutorial deck for Player 1.
    /// Uses specific cards: Fisti, JeanMichelBruitages, LePyroBarbare, Fistiland, MerdeMagiqueEnPlastiqueRose
    /// Plus 25 random cards.
    /// </summary>
    private void BuildPlayer1TutorialDeck()
    {
        var player1Deck = InstantiateSpecificCards(
            new List<CardNames>
            {
                CardNames.Fisti,
                CardNames.JeanMichelBruitages,
                CardNames.LePyroBarbare,
                CardNames.Fistiland,
                CardNames.MerdeMagiqueEnPlastiqueRose
            },
            Deck1AllCards
        );

        CardChoice.GetRandomDeck(DeckConfiguration.MaxDeckCards - player1Deck.Count, ref player1Deck, Deck1AllCards);
        player1Deck.Reverse();
        // ICardCollectionService and IAbilityProvider are null for tutorial decks (passed by callers like CardChoice)
        Player1DeckCards = player1Deck.Select(card => CardFactory.CreateInGameCard(card, CardOwner.Player1, _eventBus, null, null)).ToList();
    }

    /// <summary>
    /// Builds the tutorial deck for Player 2.
    /// Uses specific cards: ClichéRaciste, MusiqueDeMegaDrive, LElfette, Tentacules
    /// Plus 24 random cards, ensuring Tentacules is included.
    /// </summary>
    private void BuildPlayer2TutorialDeck()
    {
        var player2Deck = InstantiateSpecificCards(
            new List<CardNames>
            {
                CardNames.ClichéRaciste,
                CardNames.MusiqueDeMegaDrive,
                CardNames.LElfette
            },
            Deck2AllCards
        );

        CardChoice.GetRandomDeck(DeckConfiguration.MaxDeckCards - player2Deck.Count - 1, ref player2Deck, Deck2AllCards);

        // Ensure Tentacules card is in the deck
        var tentaculesCard = CardChoice.GetSpecificCard(CardNames.Tentacules, Deck2AllCards);
        if (tentaculesCard != null && !player2Deck.Contains(tentaculesCard))
        {
            player2Deck.Add(tentaculesCard);
        }

        player2Deck.Reverse();
        // ICardCollectionService and IAbilityProvider are null for tutorial decks
        Player2DeckCards = player2Deck.Select(card => CardFactory.CreateInGameCard(card, CardOwner.Player2, _eventBus, null, null)).ToList();
    }

    /// <summary>
    /// Instantiates specific cards based on the card names provided.
    /// </summary>
    /// <param name="cardNames">The names of the cards to be instantiated.</param>
    /// <param name="deck">The deck to get the cards from.</param>
    /// <returns>A list of instantiated cards.</returns>
    private List<Card> InstantiateSpecificCards(List<CardNames> cardNames, List<Card> deck)
    {
        return cardNames.Select(cardName => CardChoice.GetSpecificCard(cardName, deck)).ToList();
    }
}
