using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Cards;
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
/// Phase 46: IAbilityProvider is required (registered in SharedServicesScope).
///           ICardCollectionService is null (only available in Game scene).
/// Phase 61: Added all ability providers (required by CardFactory).
/// Phase 62: Uses ICardFactory via DI instead of static CardFactory.CreateInGameCard.
/// </summary>
public class DeckManagementService : IDeckManagementService
{
    private List<Card> _allCards;
    private readonly ICardDataProvider _cardDataProvider;
    private readonly ICardFactory _cardFactory;

    public List<Card> Deck1AllCards { get; private set; } = new List<Card>();
    public List<Card> Deck2AllCards { get; private set; } = new List<Card>();
    public List<InGameCard> Player1DeckCards { get; set; } = new List<InGameCard>();
    public List<InGameCard> Player2DeckCards { get; set; } = new List<InGameCard>();

    /// <summary>
    /// Phase 62: Simplified constructor - uses ICardFactory instead of individual providers.
    /// </summary>
    public DeckManagementService(
        ICardDataProvider cardDataProvider,
        ICardFactory cardFactory)
    {
        _cardDataProvider = cardDataProvider;
        _cardFactory = cardFactory;

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

        // Filter out special cards that shouldn't be in player decks (e.g., Player entity card)
        const string PlayerCardTitle = "Player";

        foreach (var card in _allCards)
        {
            // Skip the Player card - it's used for direct attack targeting, not playable
            if (card.Title == PlayerCardTitle)
                continue;

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
    /// Builds the tutorial deck for Player 1 (AI/Opponent).
    /// Uses specific cards: Fisti, JeanMichelBruitages, LePyroBarbare, Fistiland, MerdeMagiqueEnPlastiqueRose
    /// JeanMichelBruitages is the attack target for the tutorial attack flow.
    /// Plus 25 random cards.
    /// </summary>
    private void BuildPlayer1TutorialDeck()
    {
        var player1Deck = InstantiateSpecificCards(
            new List<CardNames>
            {
                CardNames.Fisti,
                CardNames.JeanMichelBruitages,  // Tutorial attack target
                CardNames.LePyroBarbare,
                CardNames.Fistiland,
                CardNames.MerdeMagiqueEnPlastiqueRose
            },
            Deck1AllCards
        );

        CardChoice.GetRandomDeck(DeckConfiguration.MaxDeckCards - player1Deck.Count, ref player1Deck, Deck1AllCards);
        player1Deck.Reverse();
        // Phase 62: Use ICardFactory instead of static CardFactory.CreateInGameCard
        Player1DeckCards = player1Deck
            .Select(card => _cardFactory.CreateCard(card, JDG.Domain.CardOwner.Player1) as InGameCard)
            .Where(card => card != null)
            .ToList();
    }

    /// <summary>
    /// Builds the tutorial deck for Player 2 (Human).
    /// Phase 143: Tentacules must stay in deck for Cliché Raciste ability to invoke it.
    /// Uses specific cards in hand: ClichéRaciste, MusiqueDeMegaDrive, LElfette
    /// Tentacules is added at deck position 0 (drawn last) so it stays in deck.
    /// Plus 26 random cards.
    /// </summary>
    private void BuildPlayer2TutorialDeck()
    {
        var player2Deck = InstantiateSpecificCards(
            new List<CardNames>
            {
                CardNames.ClichéRaciste,
                CardNames.MusiqueDeMegaDrive,
                CardNames.LElfette
                // Tentacules NOT here - needs to stay in deck for ability to invoke it
            },
            Deck2AllCards
        );

        // Get Tentacules separately - it will be added at deck beginning to stay in deck
        var tentaculesCard = CardChoice.GetSpecificCard(CardNames.Tentacules, Deck2AllCards);

        // Get random cards (minus 1 to leave room for Tentacules)
        CardChoice.GetRandomDeck(DeckConfiguration.MaxDeckCards - player2Deck.Count - 1, ref player2Deck, Deck2AllCards);

        player2Deck.Reverse();
        // Phase 62: Use ICardFactory instead of static CardFactory.CreateInGameCard
        Player2DeckCards = player2Deck
            .Select(card => _cardFactory.CreateCard(card, JDG.Domain.CardOwner.Player2) as InGameCard)
            .Where(card => card != null)
            .ToList();

        // Phase 143: Add Tentacules at position 0 (beginning of deck)
        // Since DrawFromDeck draws from the END, position 0 is drawn last
        // This ensures Tentacules stays in the deck for Cliché Raciste's ability to invoke it
        if (tentaculesCard != null)
        {
            var tentaculesInGame = _cardFactory.CreateCard(tentaculesCard, JDG.Domain.CardOwner.Player2) as InGameCard;
            if (tentaculesInGame != null)
            {
                Player2DeckCards.Insert(0, tentaculesInGame);
            }
        }
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
