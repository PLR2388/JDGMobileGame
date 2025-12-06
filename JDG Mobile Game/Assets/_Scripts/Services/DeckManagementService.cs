using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Domain.ValueObjects;
using Menu;
using UnityEngine;

/// <summary>
/// Implementation of IDeckManagementService.
/// Manages deck data storage and tutorial deck building.
/// Migrated from GameState singleton in Phase 17-18.
///
/// Uses Strangler Fig pattern: Temporarily accesses GameState to initialize card pools
/// until all callsites are migrated.
/// </summary>
public class DeckManagementService : IDeckManagementService
{
    private List<Card> _allCards;

    public List<Card> Deck1AllCards { get; private set; } = new List<Card>();
    public List<Card> Deck2AllCards { get; private set; } = new List<Card>();
    public List<InGameCard> Player1DeckCards { get; set; } = new List<InGameCard>();
    public List<InGameCard> Player2DeckCards { get; set; } = new List<InGameCard>();

    public DeckManagementService()
    {
        // Strangler Fig: Temporarily initialize from GameState singleton
        // This will be replaced with proper card loading once GameState is removed
        if (GameState.Instance != null)
        {
            var field = typeof(GameState).GetField("allCards",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            _allCards = (List<Card>)field?.GetValue(GameState.Instance);

            if (_allCards != null && _allCards.Count > 0)
            {
                InitializeCardPools(_allCards);
            }
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
        Player1DeckCards = player1Deck.Select(card => CardFactory.CreateInGameCard(card, CardOwner.Player1)).ToList();
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
        Player2DeckCards = player2Deck.Select(card => CardFactory.CreateInGameCard(card, CardOwner.Player2)).ToList();
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
